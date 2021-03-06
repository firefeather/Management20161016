﻿
function uploadProof() {
    var action = Resource.UrlAddFile;
    if (Resource.IsAddRecord == 'False') {
        action = Resource.UrlEditFile;
    }
    $('#ssi-upload').uploadify({
        uploader: action,
        swf: Resource.UrlSwf,
        cancelImage: Resource.UrlCancelImage,
        buttonText: "浏览",
        auto: false,
        width: 60,
        multi: true,
        postData: { token: Resource.Auth },
        //checkExisting: false,
        fileTypeDesc: '*.jpg;*.jpeg;*.png',
        //removeCompleted: true,
        fileTypeExts: '*.jpg;*.jpeg;*.png',
        fileSizeLimit: 10240,
        onQueueComplete: function () {
            window.location = Resource.ReturnUrl;
        },
        onError: function (event, ID, fileObj, errorObj) {
            alert("上传失败，请稍后再试！");
        },
        onUploadError: function (file, data) {
        },
        onUploadSuccess: function (file, data, response) {

        }
    });
}

function loadUplodify() {
    if (Resource.IsAddRecord == 'False') {
        $('#markerFile li').each(function (i, li) {
            var filePath = $(li).attr('filePath');
            var fileName = $(li).attr('fileName');
            var fileID = $(li).attr('fileID');
            var viewPicturePath = String.format("{0}?fileName={1}&filePath={2}", Resource.UrlViewPicture, fileName, filePath);
            var target = String.format("{0}?fileName={1}&filePath={2}", Resource.UrlDownLoad, fileName, filePath);
            $('.uploadifyQueue').append(String.format('<div class="uploadifyQueueItem"><div class="cancel"><div id="{3}" style="cursor:pointer" fileName="{1}" filePath="{5}" class="btndownload"><a href="{0}" target="_top"><img src="{6}" /></a></div><div id="{3}" style="cursor:pointer" class="btnDel"><img src="{2}" /></div></div><span class="fileName"><a href="#" target="_blank" filePath="{5}" id="DownLoadFile" fileName="{1}"  class="upload_fileName" >{1}</a></span></div>', target, fileName, Resource.UrlCancelImage, fileID, viewPicturePath, filePath, Resource.ImgDownLoad));
        })
    }

    $(document).on('click', '#DownLoadFile', function (e) {
        e.preventDefault();
        var fileName = $(this).attr('fileName');

        $('#jq22 li img[fileName=' + '"' + fileName + '"' + ']').trigger('click');
        console.log($('#jq22 li img[fileName=' + '"' + fileName + '"' + ']'));

    });

    $(document).on('click', '.btnDel', function () {
        //console.log(this);
        $(this).parents('.uploadifyQueueItem').remove();
        Resource.delID.push($(this).attr('id'));
    })

    if (Resource.IsViewRecord == 'True') {
        $('div.uploadify').remove();
    }

    //$('#btnSave').click(function (e) {
    //    e.preventDefault();
    //    var errorLength = $('.field-validation-error').length;
    //    if (errorLength == 0) {
    //        var webEntity = {};
    //        var MaintainRecordID = $('[name="MaintainRecordID"]').val();
    //        webEntity['webMaintainRecord.ID'] = MaintainRecordID;
    //        webEntity['webMaintainRecord.DeviceID'] = $('[name="DeviceID"]').val();
    //        webEntity['webMaintainRecord.Operator'] = $('#Operator').val();
    //        webEntity['webMaintainRecord.MaintainDate'] = $('#MaintainDate').val();
    //        webEntity['webMaintainRecord.Note'] = $('#Note').val();

    //        for (var i = 0; i < Resource.delID.length; i++) {
    //            webEntity['delIDList[' + i + ']'] = Resource.delID[i];
    //        }

    //        var fileLenth = $('div.uploadifyProgress').length;
    //        var handleAction;
    //        if (Resource.IsAddRecord == 'False') {
    //            webEntity.reviewID = -1;
    //            handleAction = Resource.UrlEdit;
    //            SaveAjax(handleAction, webEntity, fileLenth, MaintainRecordID);
    //        } else {
    //            handleAction = Resource.UrlAdd;
    //            SaveAjax(handleAction, webEntity, fileLenth, MaintainRecordID);
    //        }
    //    }
    //});
}

function SaveEvent() {

    $('form').submit(function (e) {
        e.preventDefault();
        var returnUrl = window.Resource.returnUrl;

        var param = {};
        var WebUser = {};
        var DeviceID = $('[Name="DeviceID"]').val();
        var DeviceName = $('[Name="DeviceID"]').val();
        var Operator = $('[Name="Operator"]').val();
        var RepairDate = $('[Name="RepairDate"]').val();
        var Note = $('textarea[Name="Note"]').val();
        var ID = $('[Name="RepairRecordID"]').val();
        var Describe = $('[Name="Describe"]').val();
        var Solution = $('[Name="Solution"]').val();

        param['webRepairRecord.DeviceID'] = DeviceID;
        param['webRepairRecord.DeviceName'] = DeviceName;
        param['webRepairRecord.Operator'] = Operator;
        param['webRepairRecord.RepairDate'] = RepairDate;
        param['webRepairRecord.Note'] = Note;
        param['webRepairRecord.Solution'] = Solution;
        param['webRepairRecord.Describe'] = Describe;
        param['webRepairRecord.ID'] = ID;
        for (var i = 0; i < Resource.delID.length; i++) {
            param['delIDList[' + i + ']'] = Resource.delID[i];
        }

        var fileLenth = $('div.uploadifyProgress').length;
        var handleAction;
        if ($('span.errorMessage').length == 0 && $('.field-validation-error').length == 0) {
            if (Resource.Action == 'Add') {
                handleAction = Resource.UrlAdd;
                SaveAjax(handleAction, param, fileLenth, ID);
            } else {
                param.reviewID = -1;
                handleAction = Resource.UrlEdit;
                SaveAjax(handleAction, param, fileLenth, ID);
            }
        }
    });
    $('select').change(function () {
        $(this).parent().find('.errorMessage').remove();
    })
}

function AjaxEvent(url, param) {
    $.post(url, param, function (data) {
        //var result = Json.parse(data);
        if (data.Code == '0') {
            window.location.href = Resource.ReturnUrl;
        } else {
            window.alert(data.Msg);
        }
    });
}

function SaveAjax(handleAction, webEntity, fileLenth, materialID) {
    $.post(handleAction, webEntity, function (data) {
        var id;
        if (data.Code == 0) {
            if (fileLenth > 0) {
                if (Resource.IsAddRecord == 'True') {
                    id = data.Data;
                } else {
                    id = materialID;
                }
                SaveUpload(handleAction, id);
            } else {
                window.location.href = Resource.ReturnUrl;
            }
        } else {
            alert(data.Msg);
        }
    });
}

function SaveUpload(handleAction, id) {
    var upload = $('#ssi-upload');
    upload.uploadifySettings('postData', { maintainRecordID: id });
    upload.uploadifyUpload('*');
}
