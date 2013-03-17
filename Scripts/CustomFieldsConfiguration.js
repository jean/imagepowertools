(function() {

    var module = angular.module('CustomFieldsConfiguration', ['ui']);

    var controllers = {};
    controllers.CustomFieldsConfigurationCtrl = function ($scope, customFields, options) {
        var self = this;
        $scope.customFields = customFields;
        
        $scope.addField = function (fieldName, fieldDisplayName, fieldType) {
            
            var position = getFieldPosition(fieldName);
            if (position >= 0) {
                
            }

            $scope.customFields.push({
                name: fieldName,
                displayName: fieldDisplayName,
                type: fieldType
            });
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

        $scope.notDuplicate = function(value) {
            var position = getFieldPosition(value);
            return position < 0;
        };

    };
    module.controller(controllers);

})();



