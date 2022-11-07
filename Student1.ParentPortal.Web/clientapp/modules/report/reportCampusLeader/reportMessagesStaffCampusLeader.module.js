angular.module('app')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('app.reportMessagesStaffCampusLeader', {
            url: '/reportmessagestaffcl',
            requireADLogin: true,
            views: {
                'content@': { component: 'reportMessagesStaffCampusLeader' }
            }
        });
    }])
    .component('reportMessagesStaffCampusLeader', {
        bindings: {
            model: "<",
            tooltips: "<"
        }, // One way data binding.
        templateUrl: 'clientapp/modules/report/reportCampusLeader/reportMessagesStaffCampusLeader.view.html',
        controllerAs: 'ctrl',
        controller: ['api', '$translate', function (api) {

            var ctrl = this;
            ctrl.messagesSent = 0;
            ctrl.messagesReceived = 0;
            ctrl.unreadMessages = 0;
            ctrl.role = "";
            ctrl.campusName = "";
            ctrl.staff;
            ctrl.schools = [];
            ctrl.seletedschool = [];
            
            ctrl.$onInit = function () {
                ctrl.staff = ctrl.model[0].staffMessagesReports; 
                ctrl.role = ctrl.model[0].staffMessagesReports[0].staffRole == 'Parent' ? 'Family Member':'Staff';
                ctrl.campusName = ctrl.model[0].campusReport.campus;
                ctrl.schools = ctrl.model.map(x => x.campusReport.campus);
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

            ctrl.exportCSV = function () {
                ctrl.downloadFile(ctrl.staff, 'campusListMessages');
            };

            ctrl.exportAllStaffCSV = function ($event) {//Not implemented
                $event.preventDefault();
                api.logAccess.getStaffMessageByDistrict().then(function (r) {
                    ctrl.downloadFile(r, "staffListMessages");
                });
            };

            ctrl.exportStaffCSV = function ($event, campus) { 
                $event.preventDefault();
                ctrl.downloadFile(ctrl.model[campus].staffMessagesReports, 'campusListMessages');
            };

            ctrl.downloadFile = function (data, filename = 'data') {
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

            ctrl.changeCampus = function ($event, $index) {
                $event.preventDefault();
                ctrl.seletedschool = ctrl.schools[$index]; 
                ctrl.staff = ctrl.model[$index].staffMessagesReports; 
                ctrl.campusName = ctrl.model[$index].campusReport.campus; 
                return false;
            };
            ctrl.OnSchoolChange = function () {
                var index = ctrl.schools.findIndex(x => x == ctrl.seletedschool);
                ctrl.staff = ctrl.model[index].staff;
                ctrl.campusName = ctrl.model[index].campusLogReports.campus;
            }

        }]
    });