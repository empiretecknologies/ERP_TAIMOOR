var empr_MenuDetails = {
    initEvents: function () {

        $(document).ready(function () {

            empr_MenuDetails.InitMenu();
            empr_MenuDetails.InitQuickSearch();
            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_MenuDetails.validateForm()) {
                            empr_MenuDetails.saveAttempt();
                        }
                    }
                } else {
                    if (empr_MenuDetails.validateForm()) {
                        empr_MenuDetails.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_MenuDetails.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_MenuDetails.GetMenuDetailsByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_MenuDetails.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_MenuDetails.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/MenuDetails/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_MenuDetails.resetForm();
                    empr_MenuDetails.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },

    resetForm: function () {
        $("#Code").val('');
        $("#MD_NAME").val('');
        $("#REPORT_NAME").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $("#menuhidden").val('');
        $("#SNO").val(0);
        $('#MMENU_ID').dxSelectBox('instance').option('value', null);
        empr_MenuDetails.InitMenu();
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
        var MD_NAME = $("#MD_NAME").val().trim();
        var REPORT_NAME = $("#REPORT_NAME").val().trim();
        
        if (MD_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        if (REPORT_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter report name.", 2);
        }

        return valid;
    },

    GetDataToSave: function () {

        var ID = $("#Code").val();
        var SNO = $("#SNO").val();
        var MD_NAME = $("#MD_NAME").val().trim();
        var REPORT_NAME = $("#REPORT_NAME").val().trim();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var MMENU_ID = $("#MMENU_ID").dxSelectBox('instance').option('value');
        var modelRecord = {
            MD_ID: ID,
            MD_NAME: MD_NAME,
            ASTATUS: ASTATUS,
            REPORT_NAME: REPORT_NAME,
            MMENU_ID: MMENU_ID,
            SNO: SNO
        }
        return modelRecord;
    },

    saveAttempt: function () {

        var obj = empr_MenuDetails.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/MenuDetails/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_MenuDetails.resetForm();
                empr_MenuDetails.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },

    InitQuickSearch: function () {
        empr_MenuDetails.GetAllMenuDetailss();
    },

    GetAllMenuDetailss: function () {
        ajaxHelper.ajaxGetJson('/MenuDetails/QuickSearch', function (data) {
            empr_MenuDetails.CreateGrid(data.data);
        }, false, true);
    },

    GetMenuDetailsByID: function (id) {
        ajaxHelper.ajaxGetJson('/MenuDetails/GetMenuDetailsByID?id=' + id, function (data) {
            empr_MenuDetails.resetForm();
            if (data.msgType == 1) {

                var record = data.data;

                $("#Code").val(record.mD_ID);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#MD_NAME").val(record.mD_NAME);
                $("#REPORT_NAME").val(record.reporT_NAME);
                $('#menuhidden').val(record.mmenU_ID);
                $('#MMENU_ID').dxSelectBox('instance').option("value", parseInt(record.mmenU_ID));
                $("#SNO").val(record.sno);
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
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.mD_ID} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
            { dataField: 'mD_NAME', caption: 'Name' },
            { dataField: 'reporT_NAME', caption: 'Report Name' },
            { dataField: 'menU_NAME', caption: 'Menu Name' },
            { dataField: 'sno', caption: 'Serial Number' },
            { dataField: 'astatus', caption: 'Active' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "MenuDetails");
    },

    InitMenu: function (selectedValue) {
        ajaxHelper.ajaxGetJson('/MenuDetails/MenuList', function (data) {
            empr_MenuDetails.bindDxDdl("MMENU_ID", data, null, "key", "value", "Select", function (d) {
                $('#menuhidden').val(d.value)
                let newArray = data;
                let filteredItem = newArray.filter(m => m.key === d.value);
                if (filteredItem.length > 0) {
                    $('#SNO').val(filteredItem[0].sno)
                    if (d.value == null) {
                        $('#menuhidden').val('');
                    }
                }

            });
        }, false, true);

        

    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
}