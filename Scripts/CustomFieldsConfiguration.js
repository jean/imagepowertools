(function() {

    var module = angular.module('CustomFieldsConfiguration', ['ui']);

    var controllers = {};
    controllers.CustomFieldsConfigurationCtrl = function($scope, customFields, options) {
        var self = this;
        $scope.customFields = customFields;

        $scope.addField = function (formName) {

            if ($.trim($scope.fieldName).length == 0 || getFieldPosition($.trim($scope.fieldName)) > -1) {
                return;
            }
            $scope.customFields.push({
                name: $.trim($scope.fieldName),
                displayName: $.trim($scope.fieldDisplayName),
                type: $scope.fieldType
            });
            $scope.fieldName = "";
            $scope.fieldDisplayName = "";
            $scope.fieldType = "text";
        };

        function getFieldPosition(fieldName) {
            for (var i = 0; i < $scope.customFields.length; i++) {
                if ($scope.customFields[i].name == fieldName) {
                    return i;
                }
            }
            return -1;
        };

        $scope.removeField = function(fieldName) {
            var position = getFieldPosition(fieldName);
            if (position >= 0) {
                $scope.customFields.splice(position, 1);
            }
        };
    };
    module.controller(controllers);

})();



