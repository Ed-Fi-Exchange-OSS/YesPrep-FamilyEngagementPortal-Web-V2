angular.module('app')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('app.landingReportTeacher', {
            url: '/reportteacher/:usi',
            requireADLogin: true,
            views: {
                'content@': { component: 'landingReportTeacher' }
            },  
            resolve: {
                modelStaff: ['api', '$stateParams', function (api, $stateParams) {
                    return api.logAccess.getTeacherReport($stateParams.usi);
                }],
                teacherReportStaffMessages: ['api', '$stateParams', function (api, $stateParams) {
                    return api.logAccess.getTeacherReportMessages($stateParams.usi);
                }],
                customParams: ['api', function (api) {
                    return api.customParams.getAll();
                }],
            }
        });
    }])
    .component('landingReportTeacher', {
        bindings: {
            modelStaff: "<",
            teacherReportStaffMessages: "<",
            customParams: "<"
        }, // One way data binding.
        templateUrl: 'clientapp/modules/report/landingReportTeacher/landingReportTeacher.view.html',
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