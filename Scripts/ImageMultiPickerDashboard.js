function ImageMultiPickerDashboard(options) {
    var base = this;

    base.add = function(file) {
        renderFileItem(file);
        syncData();
    };

    base.remove = function(file) {
        processDashboardItems(function($el) {
            var val = $el.val();
            if (val == file) {
                $el.parent().parent().remove();
            }
        });
        syncData();
    };

    base.selected = function() {
        return getItemFilePaths();
    };

    function init() {
        base.options = $.extend({}, base.defaultOptions, options);

        initDashboard();

        initSelectorButton();

        initClearLink();
        initResetLink();
    }

    function initClearLink() {
        base.options.$clearLink.click(function() {
            var doClear = confirm("Are you sure want to clear field?");
            if (doClear) {
                base.options.$dataHidden.val("[]");
                base.options.$dashBoardBox.find("tbody").html("");
            }
        });
    }

    function initResetLink() {
        base.options.$resetLink.click(function () {
            var doReset = confirm("Are you sure want to reset field?");
            if (doReset) {
                base.options.$dataHidden.val(JSON.stringify(base.options.originData));
                base.options.$dashBoardBox.find('tbody').html("");
                renderOriginData();
            }
        });
        
    }

    function processDashboardItems(processor) {
        base.options.$dashBoardBox.find('.file-path-hidden').each(function (i, el) {
            var $el = $(el);
            processor($el);
        });
    }

    function getItemFilePaths() {
        var result = [];
        processDashboardItems(function ($el) {
            var val = $el.val();
            result.push(val);
        });
        return result;
    }

    function appendDescription(fileName, descr) {
        var data = JSON.parse(base.options.$dataHidden.val());
        for (var i = 0; i < data.length; i++) {
            var el = data[i];
            if (el.file == fileName) {
                el.descr = descr;
                base.options.$dataHidden.val(JSON.stringify(data));
                return;
            }
        }
    }

    function syncData() {
        var data = [];
        processDashboardItems(function ($el) {
            var val = $el.val();
            var descr = $el.parent().find('.image-item-descr').val();
            data.push({file:val, descr:descr});
        });

        base.options.$dataHidden.val(JSON.stringify(data));
    }

    function renderOriginData() {
        $.each(base.options.originData, function (i, el) {
            if (el.file === undefined || el.file == null || el.file == "")
                return;
            renderFileItem(el.file, el.descr);
        });
    }

    function initDashboard() {
        renderOriginData();
        base.options.$dashBoardBox.find('tbody')
            .sortable({
                stop: syncData
            });
    }

    function initSelectorButton() {
        base.options.$pickerBrowseLink.click(function () {
            openPicker("");
            return false;
        });
    }
    
    function openPicker(mediaPath) {
        var newWin = window.open("/Amba.ImagePowerTools/Multipicker/Index?scope=" + base.options.scope + "&mediaPath=" + encodeURI(mediaPath),
               "Select",
               "width=450,height=600,resizable=yes,scrollbars=yes,status=yes");
        newWin.focus();
    }
    
    base.defaultOptions = {
        scope: '',
        $pickerBrowseLink: null,
        $dashBoardBox: null,
        originData: [],
        $dataHidden: null,
        $clearLink: null,
        $resetLink: null
    };

    function renderFileItem(filePath, descr) {
        var $item = genreateItem(filePath, descr);
        $item.find('.image-file-delete').click(function () {
            base.remove(filePath);
        });
        $item.find('.image-item-descr').change(function () {
            appendDescription(filePath, $(this).val());
        });
        
        var $placeHolder = $item.find(".file-breadcrumb-placeholder");
        var name = filePath.replace(/^\/Media\/[^\/]+\//, '');
        
        var paths = name.split('\/');
        for (var i = 0; i < paths.length - 1; i++) {
            var path = "";
            for (var j = 0; j <= i; j++) {
                path += (j != 0 ? "/" : "") + paths[j];
            }
            
            var folderName = paths[i];
            var $folder = $.tmpl("breadcrumbFolderTemplate", { folder: folderName, path: path });
            initBreadCrumbFolderLink($folder.find("a"), path);
            $placeHolder.append($folder);
        }
        var $file = $.tmpl("breadcrumbFileTemplate", { fileName: paths[paths.length - 1], filePath: filePath });
        $placeHolder.append($file);
        base.options.$dashBoardBox.find('tbody').append($item);
    }
    
    function initBreadCrumbFolderLink($folder, path) {
        $folder.click(function () {
            openPicker(path);
        });
    }

    function genreateItem(name, descr) {
        var origin = name;
        file = origin;
        var result = $.tmpl("imageItemTemplate", { origin: origin, file: file, descr: descr });
        return result;
    }

    function compileDefaultTemplate() {

        var breadcrumbFileTemplate = '<a href="${filePath}" target="_blank">${fileName}</a>';
        $.template("breadcrumbFileTemplate", breadcrumbFileTemplate);

        var breadcrumbFolderTemplate =
'<span><a href="javascript:void(0);" title="${path}">${folder}</a><span class="slash">/</span></span>';

        $.template("breadcrumbFolderTemplate", breadcrumbFolderTemplate);

        var imageItemTemplate =
'<tr class="image-file-item-row">\
    <td class="image-file-preview"><img src="/ipt/resize${file}?width=100"/></td>\
    <td class="image-file-info">\
        <input class="file-path-hidden" type="hidden" value="${origin}"/>\
        <div class="file-breadcrumb-placeholder"> </div>\
        <textarea class="image-item-descr" rows="1">${descr}</textarea>\
    </td>\
    <td class="image-file-item-buttons"><a class="image-file-delete" href="javascript:void(0);">Delete</a></td>\
</tr>';

        $.template("imageItemTemplate", imageItemTemplate.replace(/([>}])\s+([<{])/g, '$1$2'));
    }
    compileDefaultTemplate();
    
    init();
}