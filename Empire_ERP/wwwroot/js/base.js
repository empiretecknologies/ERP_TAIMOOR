var empr_Base = {

    Init: function () {
        console.log("js connected successfully");
        $('#gearBtn').on('click', function () {
            //debugger;
            var $dropdown = $('#gearDropdown');

            if ($dropdown.is(':visible')) {
                $dropdown.hide();
            } else {
                empr_Base.GetSettings($(this));
            }
        });

        $('#gearDropdown').on('click', function (e) {
            e.stopPropagation(); 
        });

        $(document).on('click', function () {
            $('#gearDropdown').hide(); 
        });


        $('#BtnApply').on('click', function () {
            //debugger;

            var settingsRecord = {
                SEARCH: $('#Search-Chkbox').is(':checked') ? 'D' : 'M',
                DATA_CLEAR: $('#DClear-ChkBox').is(':checked') ? '1' : '0',
                DATA_RLIMIT: $('#RowLimit').val()
            };

            console.log("settings", settingsRecord);

            ajaxHelper.ajaxPostJsonData(
                settingsRecord,
                "/Base/UpdateSettings",
                function (data) {
                    empr_helper.notify(data.msg, data.msgType);

                    if (data.msgType == 1) {
                        empr_Base.GetSettings();
                    }
                },
                false,
                true
            );
        });
    },

    GetSettings: function ($btn) {
        ajaxHelper.ajaxGetJson('/Base/GetSettings', function (data) {
            //debugger;
            console.log("settings from backend", data);
            $('#Search-Chkbox').prop('checked', data.search === 'D');
            $('#DClear-ChkBox').prop('checked', data.datA_CLEAR === 1);
            $('#RowLimit').val(data.roW_LIMIT);
            const settingsPayload = {
                search: data.search,
                dataClear: data.datA_CLEAR,
                rowLimit: data.roW_LIMIT
            };
            window.dispatchEvent(
                new CustomEvent("settingsUpdated", {
                    detail: settingsPayload
                })
            );
            var $dropdown = $('#gearDropdown');
            var offset = $btn.offset();
            $dropdown.css({
                top: offset.top + $btn.outerHeight(),
                left: offset.left
            }).show();
            
        }, false, true);
    }
};

// ✅ Document Ready OUTSIDE object
$(document).ready(function () {
    empr_Base.Init();
});
