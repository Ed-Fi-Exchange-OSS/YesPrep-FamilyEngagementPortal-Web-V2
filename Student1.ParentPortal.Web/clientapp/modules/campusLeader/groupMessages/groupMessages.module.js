﻿angular.module('app')
    .component('groupMessages', {
        bindings: {
            grades: '<',
            programs: '<',
            school: '<'
        },
        templateUrl: 'clientapp/modules/campusLeader/groupMessages/groupMessages.view.html',
        controllerAs: 'ctrl',
        controller: ['api', '$translate', '$uibModal', 'stateStorage', function (api, $translate, $uibModal, stateStorage) {

            var ctrl = this;
            ctrl.resetStylesTextArea = '';
            ctrl.resetStylesInput = '';
            ctrl.showDescription = false;
            ctrl.message = {};
            ctrl.familyCountDescription = {};
            ctrl.gradeIds = [];
            ctrl.programIds = [];
            ctrl.modalBody = "";
            ctrl.buttonLabelCustom = "";
            ctrl.audience = "";
            ctrl.stateKey = 'groupMessage';

            ctrl.$onInit = function() {
                ctrl.selectedGrade = ctrl.grades[0];
                
                ctrl.selectedProgram = ctrl.programs[0];
                
                ctrl.gradeIds = ctrl.grades.map(function(g) { return g.id; });

                ctrl.programIds = ctrl.programs.map(function(p) { return p.id; });

                var state = JSON.parse(stateStorage.getState(ctrl.stateKey));
                if (state != null) {
                    if (state.gradelevel != null) {
                        ctrl.selectedGrade = ctrl.grades.find(function (g) { return g.id == state.gradelevel });
                        ctrl.OnGradeChange();
                    }

                    if (state.program != null) {
                        ctrl.selectedProgram = ctrl.programs.find(function (p) { return p.id == state.program });
                        ctrl.OnProgramChange();
                    }
                    
                    if (state.subject != null)
                        ctrl.message.subject = state.subject;
                    if (state.body != null)
                        ctrl.message.body = state.body;
                } else {
                    if (ctrl.grades != undefined && ctrl.grades.length > 0)
                        ctrl.getParentStudentCount();
                }
            };

            ctrl.getParentStudentCount = function() {
                if (ctrl.gradeIds[0] == 0)
                    ctrl.gradeIds.shift()
                if (ctrl.programIds[0] == 0)
                    ctrl.programIds.shift();

                api.communications
                    .getParentStudentCountInGradesAndPrograms({ grades: ctrl.gradeIds, programs: ctrl.programIds, schoolId: ctrl.school })
                    .then(function(data) {
                        ctrl.showDescription = true;
                        ctrl.familyCountDescription = data;

                        ctrl.audience = `${ctrl.selectedGrade.name} and ${ctrl.selectedProgram.name} (${ctrl.familyCountDescription.familyMembersCount} people)`;
                    });
            };

            
            ctrl.OnGradeChange = function () {
                if (ctrl.selectedGrade == undefined)
                    ctrl.selectedGrade = ctrl.grades[0];
                if (ctrl.selectedGrade.id != 0) {
                    ctrl.gradeIds = [];
                    ctrl.gradeIds.push(ctrl.selectedGrade.id);
                } else {
                    ctrl.gradeIds = ctrl.grades.map(function(g) { return g.id; });
                    ctrl.selectedGrade = ctrl.grades[0];
                }
                ctrl.getParentStudentCount();
            };

            ctrl.OnProgramChange = function () {
                if (ctrl.selectedProgram == undefined)
                    ctrl.selectedProgram = ctrl.programs[0];
                if (ctrl.selectedProgram.id != 0) {
                    ctrl.programIds = [];
                    ctrl.programIds.push(ctrl.selectedProgram.id);

                } else {
                    ctrl.programIds = ctrl.programs.map(function(p) {
                        return p.id;
                    });
                }
                ctrl.getParentStudentCount();
            };

            ctrl.sendMessage = function () {

                if (ctrl.gradeIds[0] == 0)
                    ctrl.gradeIds.shift()
                if (ctrl.programIds[0] == 0)
                    ctrl.programIds.shift();

                var message = ctrl.message.body.replace(/\n/g, " <br/> ");

                //ctrl.sendprocessedMessage(message);

                var file = document.getElementById("file");
                if (file === null) {
                    ctrl.sendprocessedMessage(message);
                }else
                if (file.files[0] !== undefined) {
                    var reader = new FileReader();
                    reader.readAsDataURL(file.files[0]);
                    reader.onload = function () {
                        //console.log(reader.result)

                        ctrl.sendprocessedMessage(message + ' ' + reader.result);
                    }

                    reader.onerror = function (error) {
                        console.log("Error on load image:", error);
                    }
                } 
            };

            ctrl.sendprocessedMessage = function (message) {

                var request = {
                    gradeLevels: ctrl.gradeIds,
                    programs: ctrl.programIds,
                    message: message,
                    subject: ctrl.message.subject,
                    schoolId: ctrl.school,
                    audience: ctrl.audience
                };
                 
                api.communications.sendPrincipalGroupMessage(request).then(function (data) {
                    toastr.success($translate.instant('Message Sent'));
                    ctrl.message = {};
                    document.getElementById("messageSubject").classList.remove("ng-touched");
                    document.getElementById("messageBody").classList.remove("ng-touched");
                    ctrl.resetStylesTextArea = 'ng-untouched';
                    ctrl.resetStylesInput = 'ng-untouched';
                    stateStorage.removeState(ctrl.stateKey);
                });
            };
            ctrl.confirmSend = function() {
                return new Promise(function (resolve) {

                    ctrl.modalBody = $translate.instant('Principals Modal Message Body', { familyCount: ctrl.familyCountDescription.familyMembersCount });
                    ctrl.buttonLabelCustom = $translate.instant('Principals Modal Button Label', { familyCount: ctrl.familyCountDescription.familyMembersCount });

                    var modalInstance = $uibModal.open({
                        component: 'confirmationModal',
                        resolve: {
                            body: function() {
                                return ctrl.modalBody;
                            },
                            confirmButtonLabel: function() {
                                return ctrl.buttonLabelCustom;
                            }
                        }
                    }).result;

                    modalInstance.then(function(result) {
                        if (result)
                            ctrl.sendMessage();
                    }).catch(function (dismiss) {
                        console.log('cancelled')
                    });
                    resolve();
                });
            };
           
            ctrl.saveState = function () {
                var sub = ctrl.message.subject;
                var body = ctrl.message.body;
                if (sub == undefined)
                    sub = null;
                if (body == undefined)
                    body = null;

                stateStorage.setState(ctrl.stateKey, JSON.stringify({ subject: sub, body: body, program: ctrl.selectedProgram.id, gradelevel: ctrl.selectedGrade.id }))
            }
        }]
    });