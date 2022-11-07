angular.module('app')
    .config(['$stateProvider', function($stateProvider) {
        $stateProvider.state('app.landingReport', {
                url: '/reports',
                requireADLogin: true,
                views: {
                    'content@': { component:'landingReport' }
                },
            resolve: {
                    modelStaffLoginSummary: ['api', function (api) { return api.logAccess.getReportStaffLoginSummary(); }],
                    modelStaff: ['api', function (api) { return api.logAccess.getReport(); }],
                    modelParent: ['api', function (api) { return api.logAccess.getReportParent(); }],
                    modelStaffMessages: ['api', function (api) { return api.logAccess.getStaffMessage(); }],
                    modelParentMessages: ['api', function (api) { return api.logAccess.getParentMessage(); }],
                    customParams: ['api', function (api) {
                        return api.customParams.getAll();
                    }],
                }
            });
    }])
    .component('landingReport', {
        bindings: {
            modelStaffLoginSummary: "<",
            modelStaff: "<",
            modelParent: "<",
            modelStaffMessages: "<",
            modelParentMessages: "<",
            customParams: "<"
        }, // One way data binding.
        templateUrl: 'clientapp/modules/report/landingReport/landingReport.view.html',
        controllerAs:'ctrl',
        controller: ['api', '$translate', function (api) {
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