﻿angular.module('app')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('app.langTranslate', {
            url: '/langTranslate',
            requireADLogin: true,
            views: {
                'content@': { component: 'langTranslate' }
            },
            resolve: {
                languages: ['api', function (api) { return api.translate.getAvailableLanguages() }],
            }
        });
    }])
    .component('langTranslate', {
        bindings: {
            languages: "<",
        }, // One way data binding.
        templateUrl: 'clientapp/modules/tools/langTransPack/langTranslate.view.html',
        controllerAs: 'ctrl',
        controller: ['api', '$translate', function (api, $translate) {

            var ctrl = this;
            ctrl.model = {};
            ctrl.newElement = {};
            ctrl.$onInit = function () {
                ctrl.model.type = true;
                ctrl.model.allLanguages = false;
            }

            ctrl.uploadImage = function (e) {
                ctrl.model.file = e.target.files[0];
            }

            ctrl.translate = function () {
                if (ctrl.model.code == undefined && !ctrl.model.allLanguages) {
                    alert("Please select a language!");
                    return;
                }

                return api.translate.translatePackage({ code: ctrl.model.code, type: ctrl.model.type, allLanguages: ctrl.model.allLanguages }).then(function(data) {
                    console.log('data ', data)

                    if (ctrl.model.allLanguages) {
                        console.log('ctrl.model.allLanguages: ', ctrl.model.allLanguages)
                        console.log('ctrl.model.type: ', ctrl.model.type)

                        data.translatedPackages.forEach(function(lang, index) {
                            setTimeout(function () {
                                console.log('descargando: ', lang.fileName )

                                if (ctrl.model.type)
                                    download(lang.fileName + ".js", lang.fileContent);
                                else
                                    download(lang.fileName + ".json", lang.fileContent);
                            }, 3000 * (index +1 ));
                        });
                    }
                    else {
                        var result = data.translatedPackages[0]
                        console.log('ctrl.model.type: ', ctrl.model.type)

                        if (ctrl.model.type)
                            download(result.fileName + ".js", result.fileContent);
                        else
                            download(result.fileName + ".json", result.fileContent);
                    }
                    ctrl.model.allLanguages = false;
                });
            }

            ctrl.getBase64 = function (file) {
                return new Promise(function (resolve, reject) {
                    const reader = new FileReader();
                    reader.readAsDataURL(file);
                    reader.onload = function () { resolve(reader.result) };
                    reader.onerror = function (error) { reject(error) };
                });
            }

            ctrl.addNewElement = function () {
                api.translate.addNewElementToPackage({ key: ctrl.newElement.key, value: ctrl.newElement.value }).then(function (data) {
                    if (data) {
                        toastr.success("Element added success");
                        toastr.info("Please download all the packages or an specific package!");
                    }
                });
            }

            function download(filename, text) {
                var element = document.createElement('a');
                element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
                element.setAttribute('download', filename);

                element.style.display = 'none';
                document.body.appendChild(element);

                element.click();

                document.body.removeChild(element);
            }
        }]
    });