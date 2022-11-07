angular.module('app.directives')
    .directive('customFileInput', [function () {
        return {
            link: function (scope, element, attrs) {
                element.on('change', function (evt) {
                    var files = evt.target.files;
                    scope.filename = files[0].name
                });
            }
        }
    }]);