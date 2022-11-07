﻿angular.module('app')
    .config(['$stateProvider', function($stateProvider) {
        $stateProvider.state('app.reportMessageStaff', {
                url: '/reportmessagestaff',
                requireADLogin: true,
                views: {
                    'content@': { component:'reportMessageStaff' }
                }
            });
    }])
    .component('reportMessageStaff', {
        bindings: {
            model: "<",
            tooltips: "<"
        }, // One way data binding.
        templateUrl: 'clientapp/modules/report/reportMessage/reportMessageStaff.view.html',
        controllerAs:'ctrl',
        controller: ['api', '$translate', function (api) {

            var ctrl = this; 
            ctrl.messagesSent= 0;
            ctrl.messagesReceived = 0;
            ctrl.unreadMessages = 0;
            ctrl.role = "";

            ctrl.$onInit = function () {
                ctrl.model.forEach(function (c) {
                    ctrl.messagesSent += c.messagesSent;
                    ctrl.messagesReceived += c.messagesReceived;
                    ctrl.unreadMessages += c.unreadMessages;
                });
            }

            
            ctrl.column = 'campus';
            ctrl.reverse = false;

            ctrl.sortColumn = function (col) {
                ctrl.column = col;
                if (ctrl.reverse) {
                    ctrl.reverse = false;
                    ctrl.reverseclass = 'arrow-up';
                } else {
                    ctrl.reverse = true;
                    ctrl.reverseclass = 'arrow-down';
                }
            };

            ctrl.sortClass = function (col) {
                if (ctrl.column == col) {
                    if (ctrl.reverse) {
                        return 'arrow-down';
                    } else {
                        return 'arrow-up';
                    }
                } else {
                    return '';
                }
            };

            ctrl.exportCSV = function () {
                ctrl.downloadFile(ctrl.model, 'campusListMessages');
            };

            ctrl.exportAllStaffCSV = function ($event) {
                $event.preventDefault();
                api.logAccess.getStaffMessageByDistrict().then(function (r) {
                    ctrl.downloadFile(r, "staffListMessages");
                });
            };

            ctrl.exportStaffCSV = function ($event, campus, educationOrganizationId) {
                $event.preventDefault();
                api.logAccess.getStaffMessageByCampus(educationOrganizationId).then(function (r) {
                    ctrl.downloadFile(r, campus);
                });
            };

            ctrl.downloadFile = function(data, filename = 'data') {
                var keys = [];
                Object.keys(data[0]).forEach(function (key) {
                    keys.push(key);
                });
                if (filename == "campusListMessages") {
                    keys.pop();
                }

                let csvData = this.ConvertToCSV(data, keys);

                
                let blob = new Blob(['\ufeff' + csvData], { type: 'text/csv;charset=utf-8;' });
                let dwldLink = document.createElement("a");
                let url = URL.createObjectURL(blob);
                let isSafariBrowser = navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1;
                if (isSafariBrowser) {  //if Safari open in new window to save file with random filename.
                    dwldLink.setAttribute("target", "_blank");
                }
                dwldLink.setAttribute("href", url);
                dwldLink.setAttribute("download", filename + ".csv");
                dwldLink.style.visibility = "hidden";
                document.body.appendChild(dwldLink);
                dwldLink.click();
                document.body.removeChild(dwldLink);
            }

            ctrl.ConvertToCSV = function (objArray, headerList) {
                let array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
                let str = '';
                let row = 'S.No,';
                for (let index in headerList) {
                    row += headerList[index] + ',';
                }
                row = row.slice(0, -1);
                str += row + '\r\n';
                for (let i = 0; i < array.length; i++) {
                    let line = (i + 1) + '';
                    for (let index in headerList) {
                        let head = headerList[index];
                        line += ',' + array[i][head];
                    }
                    str += line + '\r\n';
                }
                return str;
            }
            
        }]
    });