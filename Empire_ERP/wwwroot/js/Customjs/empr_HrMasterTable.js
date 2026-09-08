var empr_HrMasterTable = {
    ddData: [],
    initEvents: function () {

        $(document).ready(function () {
            empr_HrMasterTable.InitMenu();
            empr_HrMasterTable.resetForm();
            empr_HrMasterTable.InitQuickSearch();
            //empr_HrMasterTable.InitMenuDropDown();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_HrMasterTable.validateForm()) {
                            empr_HrMasterTable.saveAttempt();
                        }
                    }
                } else {
                    if (empr_HrMasterTable.validateForm()) {
                        empr_HrMasterTable.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_HrMasterTable.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_HrMasterTable.GetHrTableTypeByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_HrMasterTable.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_HrMasterTable.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/HRMasterTable/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_HrMasterTable.resetForm();
                    empr_HrMasterTable.InitQuickSearch();
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
        $("#Image").val('');
        $("#GROUP_PIC").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");

        // ✅ Save the current value before resetting
        //var currentMenu = $("#MMENU_ID").dxSelectBox('instance')?.option('value');

        // ✅ Rebind menu, then reapply selection
        

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
        var GROUP_NAME = $("#GROUP_NAME").val();
        var GROUP_PIC = $("#GROUP_PIC").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var MENU_ID = $("#MMENU_ID").dxSelectBox('instance').option('value')

        var modelRecord = {
            GROUP_CODE: ID,
            GROUP_NAME: GROUP_NAME,
            ASTATUS: ASTATUS,
            GPIC: GROUP_PIC,
            MENU_ID: MENU_ID,
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_HrMasterTable.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/HRMasterTable/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_HrMasterTable.resetForm();
                empr_HrMasterTable.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    SaveImage: function () {
        $('#BtnSave').prop('disabled', true);
        //var files = document.getElementById('Image').files;
        //var formData = new FormData();
        //for (var i = 0; i !== files.length; i++) {
        //    formData.append("model", files[i]);
        //}
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob);
        $.ajax({
            url: "/HRMasterTable/SaveImage",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    $("#GROUP_PIC").val(data.data);
                }
                else {
                    console.log(data);
                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                }
                $('#BtnSave').prop('disabled', false);
            }
        }
        );
    },
    InitQuickSearch: function () {
        empr_HrMasterTable.GetAllHrTable();
    },
    GetAllHrTable: function () {
        ajaxHelper.ajaxGetJson('/HRMasterTable/QuickSearch', function (data) {
            empr_HrMasterTable.CreateGrid(data.data);
        }, false, true);
    },
    GetHrTableTypeByID: function (id) {
        ajaxHelper.ajaxGetJson('/HRMasterTable/GetHrTableById?id=' + id, function (data) {
            empr_HrMasterTable.resetForm();
            if (data.msgType == 1) {
                debugger;
                var record = data.data;
                console.log(record);
                //print(record)

                $("#Code").val(record.grouP_CODE);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#GROUP_NAME").val(record.grouP_NAME);
                $("#GROUP_PIC").val(record.gpic);
                empr_HrMasterTable.InitMenuDropDown(parseInt(record.menU_ID));

                $('.modal').modal('hide');
                //$('#BtnDelete').show();
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

        var col = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.gpic != null && options.data.gpic != '' && options.data.gpic != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.gpic}')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += `<a href="javascript:;" class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                    html += '</div>';
                    $(html).appendTo(container);
                }
            },
            {
                dataField: 'gpic',
                caption: 'Image',
                width: 120,
                visible: false,
                cellTemplate(container, options) {
                    if (options.value != null && options.value != '' && options.value != undefined) {
                        $('<div>')
                            .append($('<img>', { src: options.value, height: '100px', width: '100px' }))
                            .appendTo(container);
                    }
                },
            },
            { dataField: 'grouP_NAME', caption: 'Name' },
            { dataField: 'menU_NAME', caption: 'Menu Name' },    // << Added this line
            { dataField: 'astatus', caption: 'Active' },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false },
            { dataField: 'adD_DATE', caption: 'Created Date', visible: false, dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Created PostalCode', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false },
            { dataField: 'ediT_DATE', caption: 'Updated Date', visible: false, dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'ediT_POSTALCODE', caption: 'Updated PostalCode', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "SetupSubType");
    },
    InitMenu: function (selectedValue) {
        debugger;
        ajaxHelper.ajaxGetJson('/MenuDetails/MenuList', function (data) {
            if (data != null) {
                empr_HrMasterTable.ddData = data
                $('#MMENU_ID').dxSelectBox({
                    dataSource: data,
                    displayExpr: 'value',
                    valueExpr: 'key',
                    value: selectedValue,
                    searchEnabled: true,
                    width: '100%',
                    placeholder: 'Search',
                    showClearButton: true,
                    dropDownOptions: {
                        height: 'auto',
                    },
                    pagingEnabled: true,
                    searchTimeout: 500
                });
            }
            
        }, false, true);
    },
    InitMenuDropDown: function (selectedValue) {
        debugger;
        $('#MMENU_ID').dxSelectBox({
            dataSource: empr_HrMasterTable.ddData,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue,
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500
        });
    }

}