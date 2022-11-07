﻿angular.module('app.api',[])
    .service('api', ['customParams', 'apiLog', 'apiLogAccess', 'apiParents', 'apiStudents', 'apiTeachers', 'apiMe', 'apiTypes', 'apiTranslate', 'apiAlerts', 'apiCommunications', 'apiApplication', 'apiFeedback', 'apiImage', 'apiAdmin','apiSchools',
        function (customParams, apiLog, apiLogAccess, apiParents, apiStudents, apiTeachers, apiMe, apiTypes, apiTranslate, apiAlerts, apiCommunications, apiApplication, apiFeedback, apiImage, apiAdmin, apiSchools) {
        return {
            customParams: customParams,
            parents: apiParents,
            students: apiStudents,
            teachers: apiTeachers,
            translate: apiTranslate,
            me: apiMe,
            image: apiImage,
            feedback: apiFeedback,
            types: apiTypes,
            alerts: apiAlerts,
            communications: apiCommunications,
            application: apiApplication,
            log: apiLog,
            logAccess: apiLogAccess,
            admin: apiAdmin,
            schools: apiSchools
        };
    }]);