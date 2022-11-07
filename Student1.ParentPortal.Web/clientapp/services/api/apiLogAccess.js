angular.module('app.api')
    .service('apiLogAccess', ['$http', 'appConfig', function ($http, appConfig) {

        var rootApiUri = appConfig.api.rootApiUri;
        var apiResourceUri = rootApiUri + 'logaccess';

        return {
            save: function (model) { return $http.post(apiResourceUri + '/logsave', model).then(function (response) { return response; }); },
            getReportStaffLoginSummary: function () { return $http.get(apiResourceUri + '/logreportstaffloginsummary').then(function (response) { return response.data; }); },
            getReport: function () { return $http.get(apiResourceUri + '/logReport').then(function (response) { return response.data; }); },
            getByDistrict: function () { return $http.get(apiResourceUri + '/logbydistrict/').then(function (response) { return response.data; }); },
            getByCampus: function (campus) { return $http.get(apiResourceUri + '/logbycampus/'+ campus).then(function (response) { return response.data; }); },
            getReportParent: function () { return $http.get(apiResourceUri + '/logReportParent').then(function (response) { return response.data; }); },
            getParentByDistrict: function () { return $http.get(apiResourceUri + '/logparentbydistrict/').then(function (response) { return response.data; }); },
            getParentByCampus: function (campus) { return $http.get(apiResourceUri + '/logparentbycampus/' + campus).then(function (response) { return response.data; }); },
            getCampusLeaderReport: function (id) { return $http.get(apiResourceUri + '/logReportcampusleader/' + id).then(function (response) { return response.data; }); },
            getCampusLeaderReportParent: function (id) { return $http.get(apiResourceUri + '/parentReportcampusleader/' + id).then(function (response) { return response.data; }); },
            getCampusLeaderReportStaffMessages: function (id) { return $http.get(apiResourceUri + '/messagesStaffReportcampusleader/' + id).then(function (response) { return response.data; }); },
            getCampusLeaderReportParentMessages: function (id) { return $http.get(apiResourceUri + '/messagesParentReportcampusleader/' + id).then(function (response) { return response.data; }); },
            getStaffMessage: function () { return $http.get(apiResourceUri + '/staffmessagereport').then(function (response) { return response.data; }); },
            getStaffMessageByDistrict: function () { return $http.get(apiResourceUri + '/staffmessagereportbydistrict/').then(function (response) { return response.data; }); },
            getStaffMessageByCampus: function (campus) { return $http.get(apiResourceUri + '/staffmessagereportbycampus/' + campus).then(function (response) { return response.data; }); },
            getParentMessage: function () { return $http.get(apiResourceUri + '/parentmessagereport').then(function (response) { return response.data; }); },
            getParentMessageByDistrict: function () { return $http.get(apiResourceUri + '/parentmessagereportbydistrict/').then(function (response) { return response.data; }); },
            getParentMessageByCampus: function (campus) { return $http.get(apiResourceUri + '/parentmessagereportbycampus/' + campus).then(function (response) { return response.data; }); },
            getTeacherReport: function (id) { return $http.get(apiResourceUri + '/logReportTeacher/' + id).then(function (response) { return response.data; }); },
            getTeacherReportMessages: function (id) { return $http.get(apiResourceUri + '/messagesReportTeacher/' + id).then(function (response) { return response.data; }); },
            }
    }]);