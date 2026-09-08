var empr_Currency = {
    initEvents: function () {

        $(document).ready(function () {

            empr_Currency.InitQuickSearch();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Currency.validateForm()) {
                            empr_Currency.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Currency.validateForm()) {
                        empr_Currency.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_Currency.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_Currency.GetCurrencyByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_Currency.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Currency.DeleteRecord();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    DeleteRecord: function () {

        swal({
            title: 'Are you sure you want to remove this record?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#0CC27E',
            cancelButtonColor: '#FF586B',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            confirmButtonClass: 'btn btn-success mr-5',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function () {
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Currency/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Currency.resetForm();
                    empr_Currency.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#DESCR").val('');
        $("#DESCR2").val('');
        $("#SHORT_NAME").val('');
        $("#RATE").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
    },
    validateForm: function () {

        var valid = true;

        var DESCR = $("#DESCR").val().trim();
        if (DESCR == '') {
            valid = false;
            empr_helper.notify("Please enter higher currency name.", 2);
        }

        var RATE = $("#RATE").val().trim();
        if (RATE == '' || RATE == 0) {
            valid = false;
            empr_helper.notify("Please enter rate.", 2);
        }

        return valid;
    },
    GetDataToSave: function () {

        var ID = $("#Code").val();
        var DESCR = $("#DESCR").val().trim();
        var DESCR2 = $("#DESCR2").val().trim();
        var SHORT_NAME = $("#SHORT_NAME").val().trim();
        var RATE = $("#RATE").val().trim();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var modelRecord = {
            CODE: ID,
            DESCR: DESCR,
            DESCR2: DESCR2,
            SHORT_NAME: SHORT_NAME,
            RATE: RATE,
            ASTATUS: ASTATUS
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_Currency.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/Currency/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Currency.resetForm();
                empr_Currency.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_Currency.GetAllCurrencys();
    },
    GetAllCurrencys: function () {
        ajaxHelper.ajaxGetJson('/Currency/QuickSearch', function (data) {
            empr_Currency.CreateGrid(data.data);
        }, false, true);
    },
    GetCurrencyByID: function (id) {
        ajaxHelper.ajaxGetJson('/Currency/GetCurrencyByID?id=' + id, function (data) {
            empr_Currency.resetForm();
            if (data.msgType == 1) {

                var record = data.data;

                $("#Code").val(record.code);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#DESCR").val(record.descr);
                $("#DESCR2").val(record.descR2);
                $("#SHORT_NAME").val(record.shorT_NAME);
                $("#RATE").val(record.rate);

                $('.modal').modal('hide');

                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
                    }
                    if (Permissions.r_ADD) {
                        $('#BtnNew').show();
                    }
                    if (Permissions.r_EDIT) {
                        $('#BtnSave').show();
                    }
                    else {
                        $('#BtnSave').hide();
                    }
                } else {
                    $('#BtnSave').show();
                    $('#BtnDelete').show();
                    $('#BtnNew').show();
                }
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger
                var html = '<div class="btn-group btn-group-sm">';
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
            { dataField: 'descr', caption: 'Higher Currency' },
            { dataField: 'descR2', caption: 'Lower Currency' },
            { dataField: 'shorT_NAME', caption: 'Short Name' },
            { dataField: 'rate', caption: 'Rate' },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false },
            { dataField: 'adD_DATE', caption: 'Created Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Created PostalCode', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false },
            { dataField: 'ediT_DATE', caption: 'Updated Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'ediT_POSTALCODE', caption: 'Updated PostalCode', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "CurrencyQS");
    },
}