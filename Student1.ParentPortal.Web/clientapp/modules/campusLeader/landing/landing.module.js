﻿angular.module('app')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('app.campusLeaderLanding', {
            url: '/campusLeaderLanding',
            requireADLogin: true,
            views: {
                'content@': { component: 'campusLeaderLanding' }
            },
            resolve: {
                schools: ['api', function (api) { return api.schools.getSchools(); }],
                languages: ['api', function (api) { return api.translate.getAvailableLanguages(); }],
                sender: ['api', function (api) { return api.me.getMyBriefProfile(); }],
            }
        });
    }]).component('campusLeaderLanding', {
        bindings: {
            schools: "<",
            languages: "<",
            sender: "<",
        }, // One way data binding.
        templateUrl: 'clientapp/modules/campusLeader/landing/landing.view.html',
        controllerAs: 'ctrl',
        controller: ['api', '$translate', '$rootScope', function (api, $translate, $rootScope) {

            var ctrl = this;
            ctrl.gradeLevels = [];
            ctrl.programs = [];
            ctrl.queues = [];

            ctrl.getQueues = function () {
                $rootScope.$broadcast('getQueues', true);
            }

            ctrl.$onInit = function () {
                ctrl.seletedSchool = ctrl.schools[0];

                ctrl.getGradesProgramsAndQueuesData();   
            }

            ctrl.OnSchoolChange = function () {
                ctrl.getGradesProgramsAndQueuesData(); 
                $rootScope.$broadcast('changeSchoolEvent', true);
            }

            ctrl.getGradesProgramsAndQueuesData = function () {
                api.schools.getGrades(ctrl.seletedSchool.schoolId).then(function (data) {
                    ctrl.gradeLevels = data;
                });

                api.schools.getPrograms(ctrl.seletedSchool.schoolId).then(function (data) {
                    ctrl.programs = data;
                });

                api.communications.getGroupMessagesQueues(ctrl.seletedSchool.schoolId, { from: null, to: null, gradeLevels: [], programs: [] }).then(function (data) {
                    ctrl.queues = data;
                });
            }
        }]
    });