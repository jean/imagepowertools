(function () {

    var multipickerModule = angular.module('MultipickerDashboard', ['ui']);

    var controllers = {};
    controllers.DashboardCtrl = function ($scope, data, options) {
        var self = this;
        self.originData = data;
        self.pickerId = "picker_" + options.id;
        self.fieldFolder = options.fieldFolder;

        $scope.data = data;

        $scope.test = function () {
            alert('bi');
        };

        $scope.selectImages = function () {
            initPickerClient();
            openPicker(":last", self.pickerId);
        };

        $scope.reset = function () {
            $scope.data = originData;
        };

        $scope.clear = function () {
            $scope.data = [];
        };

        $scope.deleteImage = function (file) {
            removeFile(file);

        };

        $scope.onDragEnter = function () {

        };
        $scope.onDragOver = function () {

        };
        $scope.onDragLeave = function () {

        };
        $scope.onDrop = function ($event) {

            $event.originalEvent.dataTransfer.files.forEach(function (file) {
                uploadFile(file);
                /*
                var reader = new FileReader();
                reader.onload = function (e) {
                    //$uploadArea.append(['<img src="', e.target.result, '" title="', file.name, '" width="50" />'].join(''));
                };
                reader.readAsDataURL(file);
                */
            });
        };
        
        function addFile(file) {
            $scope.$apply(function () {
                $scope.data.push({ file: file });
            });
        }

        function removeFile(file) {
            
                for (var i = 0; i < $scope.data.length; i++) {
                    if ($scope.data[i].file == file) {
                        $scope.data.splice(i, 1);
                        return;
                    }
                }
                
        }


        var uploadFile = function (file) {

            var uploadProgress = function (event) {
                var percent = parseInt(event.loaded / event.total * 100);
                console.log('Загрузка: ' + percent + '%');
            };

            var stateChange = function (event) {
                if (event.target.readyState == 4) {
                    if (event.target.status == 200) {
                        console.log('Загрузка успешно завершена!');
                        addFile(self.fieldFolder + "/" + file.name);
                    } else {
                        console.log('Произошла ошибка!');
                        //$uploadArea.addClass('error');
                    }
                }
            };

            var xhr = new XMLHttpRequest();
            xhr.upload.addEventListener('progress', uploadProgress, false);
            xhr.onreadystatechange = stateChange;
            xhr.open('POST', '/ipt/upload');

            xhr.setRequestHeader("Cache-Control", "no-cache");
            xhr.setRequestHeader("Connection", "keep-alive");

            xhr.setRequestHeader('X-FILE-NAME', file.name);
            var fd = new FormData();
            fd.append("folder", self.fieldFolder);
            fd.append("file", file);
            xhr.send(fd);

        };
        

        function initPickerClient() {
            if (window[self.pickerId]) {
                return;
            }
            window[self.pickerId] = {
                selected: function () {
                    var result = [];
                    $scope.data.forEach(function (fileInfo) {
                        result.push(fileInfo.file);
                    });
                    return result;
                },
                isMultiselect: function () {
                    return true;
                },
                add: function (file) {

                    addFile(file);

                },
                remove: function (file) {
                    $scope.$apply(function () {
                        removeFile(file);
                    });
                    
                }
            };
        }

        function openPicker(mediaPath, clientId) {
            var newWin = window.open("/Amba.ImagePowerTools/Multipicker/Index?scope=" + clientId + "&mediaPath=" + encodeURI(mediaPath),
                   "Select",
                   "width=450,height=600,resizable=yes,scrollbars=yes,status=yes");
            newWin.focus();
        }

    };
    multipickerModule.controller(controllers);

    multipickerModule.filter('filename', function() {
        return function (input) {
            return input.replace(/^\/Media\/Default\//g, '');
        };
    });
    

    if (!FileList.prototype.forEach) {
        FileList.prototype.forEach = function (fn, scope) {
            for (var i = 0, len = this.length; i < len; ++i) {
                fn.call(scope, this[i], i, this);
            }
        };
    }

})();



