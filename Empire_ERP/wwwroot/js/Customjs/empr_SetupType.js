var empr_SetupType = {
    initEvents: function () {

        $(document).ready(function () {

            empr_SetupType.InitQuickSearch();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_SetupType.validateForm()) {
                            empr_SetupType.saveAttempt();
                        }
                    }
                } else {
                    if (empr_SetupType.validateForm()) {
                        empr_SetupType.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_SetupType.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_SetupType.GetSetupTypeByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_SetupType.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_SetupType.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/SetupType/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_SetupType.resetForm();
                    empr_SetupType.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#GROUP_NAME").val('');
        $("#QTY").val('');
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
        var GROUP_NAME = $("#GROUP_NAME").val().trim();
        
        if (GROUP_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        return valid;
    },
    GetDataToSave: function () {

        var ID = $("#Code").val();
        var GROUP_NAME = $("#GROUP_NAME").val().trim();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var Quantity = $("#QTY").val().trim();
        var modelRecord = {
            GROUP_CODE: ID,
            GROUP_NAME: GROUP_NAME,
            ASTATUS: ASTATUS,
            QTY: Quantity
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_SetupType.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/SetupType/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_SetupType.resetForm();
                empr_SetupType.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_SetupType.GetAllSetupTypes();
    },
    GetAllSetupTypes: function () {
        ajaxHelper.ajaxGetJson('/SetupType/QuickSearch', function (data) {
            empr_SetupType.CreateGrid(data.data);
        }, false, true);
    },
    GetSetupTypeByID: function (id) {
        ajaxHelper.ajaxGetJson('/SetupType/GetSetupTypeByID?id=' + id, function (data) {
            empr_SetupType.resetForm();
            if (data.msgType == 1) {

                var record = data.data;

                $("#Code").val(record.grouP_CODE);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#GROUP_NAME").val(record.grouP_NAME);
                $("#QTY").val(record.qty);

                $('.modal').modal('hide');
                //$('#BtnDelete').show();
                //$('#BtnNew').show();

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
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
            { dataField: 'grouP_NAME', caption: 'Name' },
            { dataField: 'astatus', caption: 'Active' },
            { dataField: 'qty', caption: 'Quantity' },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "SetupType");
    },
}