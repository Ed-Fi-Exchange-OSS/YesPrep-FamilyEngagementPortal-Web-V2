angular.module('app.directives')
    .component('staarCard', {
        bindings: {
            model: "<",
            showAll: "="
        },
        templateUrl: 'clientapp/modules/directives/staarCard/staarCard.view.html',
        controllerAs: 'ctrl',
        controller: [function () {
            var ctrl = this;
        }]
    });