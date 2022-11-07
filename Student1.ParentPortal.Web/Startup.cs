using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using Student1.ParentPortal.Web.App_Start;
using Student1.ParentPortal.Web.Security;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using System.Text;
using Microsoft.Owin.Security.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // IoC Configuration
            var ioCContainer = new IoCConfig().GetContainer();
            app.Use(async (context, next) =>
            {
                using (AsyncScopedLifestyle.BeginScope(ioCContainer))
                {
                    await next();
                }
            });

            var config = new HttpConfiguration
            {
                DependencyResolver = new SimpleInjectorWebApiDependencyResolver(ioCContainer)
            };
            IdentityModelEventSource.ShowPII = true;
            // Authentication Configuration
            ConfigureAuth(app, ioCContainer);

            // WebApi Configuration
            WebApiConfig.Register(config, ioCContainer);
            app.UseWebApi(config);

            // Enable SignalR
            var signalrEnvironment = ConfigurationManager.AppSettings["signalr.env"];

            if (signalrEnvironment == "local")
                app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true });
            else // It has to be azure
                app.MapAzureSignalR(this.GetType().FullName);
        }

        private void ConfigureAuth(IAppBuilder app, Container ioCContainer)
        {
            using (AsyncScopedLifestyle.BeginScope(ioCContainer))
            {
                var mode = ConfigurationManager.AppSettings["authentication.azure.mode"];
                var tenant = ConfigurationManager.AppSettings["authentication.azure.tenant"];
                var audience = ConfigurationManager.AppSettings["authentication.azure.audience"];
                var instance = ConfigurationManager.AppSettings["authentication.azure.instance"];
                var policy = ConfigurationManager.AppSettings["authentication.azure.policy"];

                var jwtAudience = ConfigurationManager.AppSettings["Jwt:Audience"];
                var jwtKey = ConfigurationManager.AppSettings["Jwt:Key"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var jwtIssuer = ConfigurationManager.AppSettings["Jwt:Issuer"];

                var appleJwtAudience = ConfigurationManager.AppSettings["authentication.apple.audience"];
                var appleJwtIssuer = ConfigurationManager.AppSettings["authentication.apple.issuer"];
                var appleJwtKeys = new JsonWebKeySet(GetAppleKeys().GetAwaiter().GetResult()).Keys;

                var customJwtAzureOAuthBearerAuthenticationProvider = ioCContainer.GetInstance<CustomAzureAdOAuthBearerAuthenticationProvider>();

                app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
                {
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key
                    },
                    Provider = customJwtAzureOAuthBearerAuthenticationProvider
                });


                app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
                {
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = appleJwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = appleJwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = appleJwtKeys
                    }
                });


                if (mode == "B2C")
                {
                    var stsDiscoveryEndpoint = string.Format(instance, tenant, policy);
                    //var stsDiscoveryEndpoint = "https://student1ypfpaadb2c.b2clogin.com/Student1YPFPAADB2C.onmicrosoft.com/B2C_1_UF_Policy_Main/v2.0/.well-known/openid-configuration";
                    var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
                    var config = configManager.GetConfigurationAsync().Result;

                    app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                        new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                        {
                            Tenant = tenant,
                            TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidAudience = audience,
                                ValidIssuer = config.Issuer,
                                IssuerSigningKeys = config.SigningKeys.ToList(),
                            },
                            Provider = customJwtAzureOAuthBearerAuthenticationProvider
                        });
                }

                if (mode == "B2B")
                {
                    app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                        new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                        {
                            Tenant = tenant,
                            TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidAudience = audience,
                            },
                            Provider = customJwtAzureOAuthBearerAuthenticationProvider
                        });
                }
            }
        }

        private async Task<string> GetAppleKeys()
        {
            return await new HttpClient().GetStringAsync(ConfigurationManager.AppSettings["authentication.apple.keys"]);
        }
    }
}