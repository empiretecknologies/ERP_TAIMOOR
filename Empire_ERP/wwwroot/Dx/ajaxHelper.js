/// <reference path="ati_common.js" />

var ajaxHelper = {
    allXhrRequests: [],
    ajaxPostJsonData: function (jsonData, url, successFunc, isAsync) {
 
        var xhr = $.ajax({
            type: 'POST',
            url: url,
            //datatype: 'json',
            //contentType: 'application/json; charset=utf-8',
            async: isAsync,
            data: jsonData,
            cache: false,
            success: function (data) {
                
                ajaxHelper.isSessionExpired(data);
                successFunc(data);
            },
            error: function (jqXHR, exception) {
                //var msg = ati_common.ajaxError(jqXHR, exception);
                //ati_common.notify(msg, 2);
                //tnt_login.hideLoginSpinner();
            }
        }).done(function (data) {
            
            //ajaxHelper.isSessionExpired(data);
            //if (ajaxHelper.isSessionExpired(data)) {
            //    ajaxHelper.redirectToSessionExpiredPage();
            //    return;
            //}
            //else {
            //    /*successFunc(data);*/
            //}
        });
        return xhr;
    },
    isSessionExpired: function (data) {
        
        if (data && typeof (data) == "string" && data.indexOf("Login") > -1) {
            window.location.href = "/Login/Index";
        }
        else
        {
            
        }

     },
    redirectToSessionExpiredPage: function () {
    /*window.location.href = window.location.protocol + "//" + window.location.host + "/Login/LoginExpireAlert";*/
    //ati_common.showModal("mdlSessionExpire");
    window.location.href = "/Login/Index";
},
    ajaxPostsingleValue: function ( Data, url, isAsync, Modal) {
        var xhr = $.ajax({
            type: 'POST',
            url: url,
            async: isAsync,
            data: { 'Suggestion': Data },
            cache: false,
            success: function (data) {
                $(Modal).modal('hide');
                $('#Loader').css('display', 'none');
            },
            error: function (jqXHR, exception) {
                console.log(jqXHR);
                $('#Loader').css('display', 'none');
                alert("An error occur while submitting suggestion! Please contact support");
            }
        });
        return xhr;
    },

    ajaxPostJsonDataAndExtraParam: function (jsonData, url, successFunc, isPopup, isAsync, successParam) {
        var xhr = $.ajax({
            type: 'POST',
            url: url,
            datatype: 'json',
            contentType: 'application/json; charset=utf-8',
            async: isAsync,
            data: jsonData,
            cache: false,
            success: function (data) {
                ajaxHelper.isSessionExpired(data);
                successFunc(data, successParam);
            },
            error: function (jqXHR, exception) {
                var msg = ati_common.ajaxError(jqXHR, exception);
                ati_common.notify(msg, 2);
            }
        });
        return xhr;
    },
    ajaxGetJson: function (url, successFunc, isPopup, isAsync) {
        var xhr = $.ajax({
            type: 'GET',
            async: isAsync,
            url: url,
            datatype: 'json',
            cache: false,
            success: function (data) {
                //if (data.isSession) {
                    successFunc(data);
                //}
            },
            error: function (jqXHR, exception) {
                //var msg = ati_common.ajaxError(jqXHR, exception);
                //ati_common.notify(msg, 2);
            }
        });
        return xhr;
    },
    ajaxGetHtml: function (url, successFunc, isPopup, isAsync, successParam) {
        $.get(url, function (data) {
            return data;
        }).done(function (data) {
            successFunc(data);
        }).fail(function (e, v) {
        });
    },
    ajaxGetJsonAndExtraParam: function (url, successFunc, isPopup, isAsync, successParam) {
        var xhr = $.ajax({
            type: 'GET',
            async: isAsync,
            url: url,
            datatype: 'json',
            cache: false,
            success: function (data) {
                successFunc(data, successParam);
            },
            error: function (jqXHR, exception) {
                var msg = ati_common.ajaxError(jqXHR, exception);
                ati_common.notify(msg, 2);
            }
        });
        return xhr;
    },
    ajaxPostSerializeData: function (serializedData, url, successFunc, isPopup, isAsync) {
        var xhr = $.ajax({
            type: 'POST',
            url: url,
            async: isAsync,
            data: serializedData,
            cache: false,
            success: function (data) {
                successFunc(data);
            },
            error: function (jqXHR, exception) {
                var msg = ati_common.ajaxError(jqXHR, exception);
                ati_common.notify(msg, 2);
            }
        });
        return xhr;
    },
    ajaxGetHtml: function (url, successFunc, isPopup, isAsync, successParam) {
        var xhr = $.ajax({
            type: 'GET',
            async: isAsync,
            url: url,
            dataType: 'html',
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                successFunc(data, successParam);
            },
            error: function (jqXHR, exception) {
                var msg = ati_common.ajaxError(jqXHR, exception);
                ati_common.notify(msg, 2);
            }
        });
        return xhr;
    },

    //Usage: example
    //var fileData = new FormData();
    //var files = fileControl.files;
    //fileData.append( files[0].name, files[0] );
    ajaxPostFileData: function (fileData, url, successFunc, isPopup, isAsync) {
        var xhr = $.ajax({
            url: url,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            async: isAsync,
            data: fileData,
            success: function (data) {
                successFunc(data);
            },
            error: function (jqXHR, exception) {
                var msg = ati_common.ajaxError(jqXHR, exception);
                ati_common.notify(msg, 2);
            }
        });
        return xhr;
    },
    ajaxPostImage: function (url, formdata, successFunc) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url);
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var picGuid = xhr.responseText.replace('"', '');
                picGuid = picGuid.substr(0, picGuid.indexOf('"'));
                successFunc(picGuid);
            }
        };
    }
};