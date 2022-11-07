using System;
using System.IO;
using System.Web;

namespace Student1.ParentPortal.Resources.Providers.Url
{
    public interface IUrlProvider
    {
        string GetApplicationBasePath();
        string GetStudentDetailUrl(int studentUsi);
        string GetLoginUrl();
        string GetLogoUrl();
        string GetShortenedHomeUrl();
    }

    public class UrlProvider : IUrlProvider
    {
        public string GetApplicationBasePath()
        {
            String strUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/";

            return strUrl;
        }

        public string GetStudentDetailUrl(int studentUsi)
        {
            return Path.Combine(GetApplicationBasePath(),$"#/studentdetail/{studentUsi}");
        }

        public string GetLoginUrl()
        {
            return Path.Combine(GetApplicationBasePath(), $"#/login");
        }

        public string GetLogoUrl()
        {
            if (GetApplicationBasePath().Contains("localhost"))
                return $"https://ypfamilyportal.azurewebsites.net/clientapp/assets/img/logo.png";

            var returnUrl = new Uri(new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath), $"clientapp/assets/img/logo.png");
            return Path.Combine(GetApplicationBasePath(), $"clientapp/assets/img/logo.png");
        }

        public string GetShortenedHomeUrl()
        {
            return "https://bit.ly/yppsfp";
        }
    }
}
