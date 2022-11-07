﻿angular.module('app')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('app.reportStaffCampusLeader', {
            url: '/reportstaffcl',
            requireADLogin: true,
            views: {
                'content@': { component: 'reportStaffCampusLeader' }
            }
        });
    }])
    .component('reportStaffCampusLeader', {
        bindings: {
            model: "<",
            tooltips: "<"
        }, // One way data binding.
        templateUrl: 'clientapp/modules/report/reportCampusLeader/reportStaffCampusLeader.view.html',
        controllerAs: 'ctrl',
        controller: ['api', '$translate','$stateParams', function (api, ngTableParams, $stateParams) {

            var ctrl = this;
            ctrl.eligibleStaff = 0;
            ctrl.eligibleStaffLogin = 0;
            ctrl.teacher = 0;
            ctrl.teacherLogin = 0;
            ctrl.campusLeader = 0;
            ctrl.campusLeaderLogin = 0;
            ctrl.role = "";
            ctrl.staff;
            ctrl.campusName = "";
            ctrl.schools = [];
            ctrl.seletedschool = [];


            ctrl.$onInit = function () {
                ctrl.staff = ctrl.model[0].staff;
                ctrl.campusName = ctrl.model[0].campusLogReports.campus;
                ctrl.schools = ctrl.model.map(x => x.campusLogReports.campus);
                ctrl.seletedschool = ctrl.schools[0];
            }

            ctrl.column = 'staffName';
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
            api.me.getRole().then(function (role) {
                ctrl.role = role;
            });

            ctrl.changeCampus = function ($event, $index) {
                $event.preventDefault();
                ctrl.seletedschool = ctrl.schools[$index];
                ctrl.staff = ctrl.model[$index].staff;
                ctrl.campusName = ctrl.model[$index].campusLogReports.campus;
                return false;
            };
            ctrl.onschoolchange = function ($event, $index) {
                $event.preventDefault();
                ctrl.staff = ctrl.model[$index].staff;
                ctrl.campusName = ctrl.model[$index].campusLogReports.campus;
                return false;
            };

            ctrl.OnSchoolChange = function () {
                var index = ctrl.schools.findIndex(x => x == ctrl.seletedschool);
                ctrl.staff = ctrl.model[index].staff;
                ctrl.campusName = ctrl.model[index].campusLogReports.campus;
            }

            ctrl.exportCSV = function () {
                ctrl.staff.forEach(function (v) { delete v.staffPositionTitle; });
                ctrl.downloadFile(ctrl.staff, 'staffList.csv');
            };

            ctrl.downloadFile = function (data, filename = 'data') {
                var keys = [];
                Object.keys(data[0]).forEach(function (key) {
                    keys.push(key);
                });
                keys.pop();

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