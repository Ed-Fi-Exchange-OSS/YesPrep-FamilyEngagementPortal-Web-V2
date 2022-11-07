angular.module('app')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('app.landingReportCampusLeader', {
            url: '/reportscl/:usi',
            requireADLogin: true,
            views: {
                'content@': { component: 'landingReportCampusLeader' }
            },  
            resolve: {
                modelStaff: ['api', '$stateParams', function (api, $stateParams) {
                    return api.logAccess.getCampusLeaderReport($stateParams.usi);
                }],
                modelParent: ['api', '$stateParams', function (api, $stateParams) {
                    return api.logAccess.getCampusLeaderReportParent($stateParams.usi);
                }],
                campusLeaderReportStaffMessages: ['api', '$stateParams', function (api, $stateParams) {
                    return api.logAccess.getCampusLeaderReportStaffMessages($stateParams.usi);
                }],
                campusLeaderReportParentMessages: ['api', '$stateParams', function (api, $stateParams) {
                    return api.logAccess.getCampusLeaderReportParentMessages($stateParams.usi);
                }],
                customParams: ['api', function (api) {
                    return api.customParams.getAll();
                }],
            }
        });
    }])
    .component('landingReportCampusLeader', {
        bindings: {
            modelStaff: "<",
            modelParent: "<",
            campusLeaderReportStaffMessages: "<",
            campusLeaderReportParentMessages: "<",
            customParams: "<"
        }, // One way data binding.
        templateUrl: 'clientapp/modules/report/landingReportCampusLeader/landingReportCampusLeader.view.html',
        controllerAs: 'ctrl',
        controller: ['api', '$translate','$stateParams', function (api, ngTableParams, $stateParams) {

            var ctrl = this;
            ctrl.tooltips;

            ctrl.$onInit = function () {
                ctrl.tooltips = ctrl.customParams.tooltips;
            }
            api.me.getRole().then(function (role) {
                ctrl.role = role;
            });


        }]
    });