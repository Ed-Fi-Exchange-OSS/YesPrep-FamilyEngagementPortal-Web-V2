angular.module('app')
    .config([
        '$stateProvider', function ($stateProvider) {
            $stateProvider.state('app.login',
                {
                    url: '/login',
                    views: {
                        'navbar@': '',
                        'content@': 'login'
                    },
                    resolve: {
                        isApplicationOffline: ['api', function (api) {
                            return api.application.getIsApplicationOffline();
                        }]//['api', function (api) { return api.application.getIsApplicationOffline(); }]
                    }
                });
        }
    ])
    .component('login', {
        bindings: { isApplicationOffline: "<" },
        templateUrl: 'clientapp/modules/login/login.view.html',
        controllerAs: 'ctrl',
        controller: ['adalAuthenticationService', '$state', 'appConfig', 'landingRouteService', '$rootScope', 'api', '$window', 'impersonateService', function (adalService, $state, appConfig, landingRouteService, $rootScope, api, $window, impersonateService) {
            var ctrl = this; ctrl.showAppButtons = false;
            ctrl.externalUrl = "";
            api.customParams.getAll().then(function (data) {
                ctrl.externalUrl = data.feedbackExternalUrl;
            });

            ctrl.model = {
                version: appConfig.version,
                userInfo: adalService.userInfo,
                showAppButtons: false
            };

            ctrl.loginSSO = function () {
                adalService.login();
            };

            ctrl.logOutSSO = function () {
                /*remove all the sessions impersonate variables*/
                sessionStorage.removeItem('adal.oldidtoken');
                sessionStorage.removeItem('adal.impersonate');

                adalService.logOut();
            };

            ctrl.showFeedbackModal = function () {
                $rootScope.feedback = true;
            };

            ctrl.showFeedbackModal = function () {
                $rootScope.feedback = true;
            };

            ctrl.goToExternalUrl = function () {
                $window.open(ctrl.externalUrl);
            };

            if (adalService.userInfo.isAuthenticated) {


                var browserName = (function (agent) {
                    switch (true) {
                        case agent.indexOf("edge") > -1: return "MS Edge";
                        case agent.indexOf("edg/") > -1: return "Edge (chromium based)";
                        case agent.indexOf("opr") > -1 && !!window.opr: return "Opera";
                        case agent.indexOf("chrome") > -1 && !!window.chrome: return "Chrome";
                        case agent.indexOf("trident") > -1: return "MS IE";
                        case agent.indexOf("firefox") > -1: return "Mozilla Firefox";
                        case agent.indexOf("safari") > -1: return "Safari";
                        default: return "other";
                    }
                })(window.navigator.userAgent.toLowerCase());

                if (adalService.userInfo.profile === undefined) {
                    var user = impersonateService.getImpersonateUser();
                    adalService.userInfo.profile = user;
                }
                var token = " idToken: " + sessionStorage.getItem('adal.idtoken');
                ctrl.model.tokenExpires = new Date(adalService.userInfo.profile.exp * 1000);
                api.log.saveLogInfo({ message: "Identity verified for email : " + (ctrl.model.userInfo.userName || ctrl.model.userInfo.profile.emails[0]) + token });
                api.me.getMyProfile().then(function (profile) {
                    api.logAccess.save({
                        email: ctrl.model.userInfo.profile.emails[0],
                        usi: profile.usi,
                        uniqueId: profile.uniqueId,
                        personType: profile.personTypeId,
                        platform: "Web",
                        platformInfo: browserName
                    });
                });

                // Redirect user to appropriate landing page based on role.
                landingRouteService.redirectToLanding();
            }

            if (window.screen.width <= 768) {
                ctrl.model.showAppButtons = true;
            }
        }]
    });