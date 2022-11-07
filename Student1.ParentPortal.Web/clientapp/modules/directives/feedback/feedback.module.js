﻿angular.module('app')
    .component('feedback', {
        templateUrl: 'clientapp/modules/directives/feedback/feedback.view.html',
        controllerAs: 'ctrl',
        controller: ['$rootScope', 'api', '$location', '$translate', function ($rootScope, api, $location, $translate) {
            var ctrl = this;

            ctrl.model = {
                feedback: true
            };

            $rootScope.$watch('feedback', function (newVal, oldVal) {
                ctrl.model.feedback = newVal;
            });

            ctrl.model.feedback = $rootScope.feedback;

            ctrl.onLoginScreen = function(){
                return $location.url().indexOf('login') != -1;
            };

            ctrl.persistFeedback = function () {
                const modeltoSend = {
                    issue: ctrl.model.issue,
                    subject: ctrl.model.subject,
                    description: ctrl.model.description,
                    name: ctrl.model.name,
                    email: ctrl.model.email,
                    currentUrl: $location.url(),
                    isFromLoginScreen: ctrl.onLoginScreen()
                };
                if (ctrl.onLoginScreen()) {
                    return api.feedback.persistPublic(modeltoSend).then(function (result) {
                        $rootScope.feedback = false;
                        toastr.success($translate.instant('We have successfully received your feedback') + ".");

                        modeltoSend = {
                            issue: "",
                            subject: "",
                            description: "",
                            name: "",
                            email: "",
                            currentUrl: $location.url(),
                            isFromLoginScreen: ctrl.onLoginScreen()
                        }
                    });
                }
                return api.feedback.persist(modeltoSend).then(function (result) {
                    $rootScope.feedback = false;
                    toastr.success($translate.instant('We have successfully received your feedback') + ".");
                    modeltoSend = {
                        issue: "",
                        subject: "",
                        description: "",
                        name: "",
                        email: "",
                        currentUrl: $location.url(),
                        isFromLoginScreen: ctrl.onLoginScreen()
                    }
                });

              
            }

            ctrl.close = function () {
                delete ctrl.model.subject;
                delete ctrl.model.issue;
                delete ctrl.model.description;

                $rootScope.feedback = false;
            }
        }]
    });