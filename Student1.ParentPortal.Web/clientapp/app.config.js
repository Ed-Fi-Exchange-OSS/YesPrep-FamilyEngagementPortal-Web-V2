angular.module("app.config", [])
    .constant("appConfig", {
        api: {
            rootApiUri: "api/",
            key: "",
            secret: ""
        },
        azureAd: {
            instance: "https://login.microsoftonline.com/",
            tenant: "toolwise.onmicrosoft.com",
            clientId: "c34f1634-092c-476e-baa9-29e429717879",
            policy: "B2C_1_FirstTest",
            scope: "openid"
        },
        //azureAd: {
        //    instance: "https://YESPrepPublicSchoolsB2C.b2clogin.com/",
        //    tenant: "YESPrepPublicSchoolsB2C.onmicrosoft.com/B2C_1_FirstTest",
        //    clientId: "5cc234e5-edc2-40d4-b8b9-495fbad80d9f",
        //    policy: "B2C_1_FirstTest",
        //    scope: "openid",
        //},
        hero: {
            client: 'YP'
        },
        version: "v1.0.0"
    });