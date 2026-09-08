var empr_Grade = {
    initEvents: function () {

        $(document).ready(function () {

            empr_Grade.InitQuickSearch();
            empr_Grade.InitSizeDDL();
            empr_Grade.InitItemsDDL();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Grade.validateForm()) {
                            empr_Grade.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Grade.validateForm()) {
                        empr_Grade.saveAttempt();
                    }
                }
            });

            //$('body').on('click', '#QuickSearch', function () {
            //    empr_Grade.InitQuickSearch();
            //});

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_Grade.GetGradeByID(reportid);
            });

            $('body').on('click', '#docBrowseBtn', function () {
                $('#DOC').val('');
                $('#hdnDOC').val('');
                $('#DOCName').val('');
                $('#DOC').click();
            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var name = $(this).attr("reportname");
                swal({
                    title: 'Are you sure you want to Copy this record?',
                    text: "",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#0CC27E',
                    cancelButtonColor: '#FF586B',
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No',
                    confirmButtonClass: 'btn btn-success mr-5',
                    cancelButtonClass: 'btn btn-danger',
                    buttonsStyling: false
                }).then(function () {
                    $('#updatedName').val(name);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModalName').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, iteM_NAME: $('#updatedName').val() }, "/Grade/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        //empr_ItemMaster.GetItemMasterByCode(data.data);
                        empr_Grade.InitQuickSearch();
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                //$('#BtnNew').hide();
                empr_Grade.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Grade.DeleteRecord();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
            //$('#Image').change(function () {
            //    empr_Grade.SaveImage();
            //})
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Grade/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Grade.resetForm();
                    empr_Grade.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    //$('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#GROUP_NAME").val('');
        $("#Image").val(''); 
        $("#hdnDOC").val(''); 
        $("#RATE").val(''); 
        $("#DOCName").val(''); 
        //$("#GROUP_PIC").val('');
        empr_Grade.InitSizeDDL();
        empr_Grade.InitItemsDDL();
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
        var GROUP_NAME = $("#GROUP_NAME").val();
        var GROUP_PIC = $("#hdnDOC").val();
        var RATE = $("#RATE").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');   
        var SIZE = $('#SIZE').dxSelectBox('option', 'value');
        var ITEM_CODE = $('#ITEM').dxSelectBox('option', 'value');
        var modelRecord = {
            GROUP_CODE: ID,
            GROUP_NAME: GROUP_NAME,
            ASTATUS: ASTATUS,
            GPIC: GROUP_PIC,
            SIZE: SIZE,
            RATE: RATE,
            ITEM_CODE: ITEM_CODE,
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_Grade.GetDataToSave();
        console.log('Save Attempt', obj);
        ajaxHelper.ajaxPostJsonData(obj, "/Grade/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Grade.resetForm();
                empr_Grade.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                //$('#BtnNew').hide();
            }
        }, false, true);
    },
    //SaveImage: function () {
    //    $('#BtnSave').prop('disabled', true);
    //    //var files = document.getElementById('Image').files;
    //    //var formData = new FormData();
    //    //for (var i = 0; i !== files.length; i++) {
    //    //    formData.append("model", files[i]);
    //    //}
    //    var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
    //    var binaryData = atob(base64String);
    //    var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
    //        return char.charCodeAt(0);
    //    }))], { type: 'image/png' });

    //    var formData = new FormData();
    //    formData.append('model', blob);
    //    $.ajax({
    //            url: "/Grade/SaveImage",
    //            data: formData,
    //            processData: false,
    //            contentType: false,
    //            type: "POST",
    //            success: function (data) {
    //                if (data.msgType == '1') {
    //                    $("#GROUP_PIC").val(data.data);
    //                }
    //                else {
    //                    console.log(data);
    //                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
    //                }
    //                $('#BtnSave').prop('disabled', false);
    //            }
    //        }
    //    );
    //},
    InitQuickSearch: function () {
        empr_Grade.GetAllGrades();
    },
    GetAllGrades: function () {
        ajaxHelper.ajaxGetJson('/Grade/QuickSearch', function (data) {
            console.log('quick search',data.data);
            empr_Grade.CreateGrid(data.data);
        }, false, true);
    },
    GetGradeByID: function (id) {
        ajaxHelper.ajaxGetJson('/Grade/GetGradeByID?id=' + id, function (data) {
            empr_Grade.resetForm();
            if (data.msgType == 1) {

                var record = data.data;

                $("#Code").val(record.grouP_CODE);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#GROUP_NAME").val(record.grouP_NAME);
                $("#RATE").val(record.rate);
                $('#hdnDOC').val(record.gpic);
                var DocPath = record.gpic;
                var DocName = DocPath.split('/').pop();
                $("#DOCName").val(DocName);

                //$("#GROUP_PIC").val(record.gpic);
                empr_Grade.InitSizeDDL(record.size);
                empr_Grade.InitItemsDDL(record.iteM_CODE);

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

        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger;
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.gpic != null && options.data.gpic != '' && options.data.gpic != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.gpic}')"><i class="fa fa-eye"></i></a>`;
                }
                html += `<a href="javascript:;" class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += `<a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportname="${options.data.grouP_NAME}" reportid=${options.data.grouP_CODE} title="COPY"><i class="fa fa-copy"></i></a>`;
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
            { dataField: 'iteM_NAME', caption: 'Item' },
            { dataField: 'rate', caption: 'Rate' },
            { dataField: 'sizE_NAME', caption: 'Size' },
            { dataField: 'astatus', caption: 'Active' },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Grade");
    },

    InitSizeDDL: function (selectedValue) {
        //console.log(selectedValue);
        $('#SIZE').dxSelectBox({
            dataSource: {
                store: Size,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
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
            searchTimeout: 500,
            //disabled: true
        });
    },

    InitItemsDDL: function (selectedValue) {
        $('#ITEM').dxSelectBox({
            dataSource: {
                store: Items,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
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
            searchTimeout: 500,
            //disabled: true
        });
    },

    UploadDoc: function () {
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        if (files.length > 0) {
            $('#DOCName').val(files[0].name);
        }

        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax({
            url: "/Common/UploadVoucherDocs",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    $("#hdnDOC").val(data.data);
                    empr_helper.notify("File uploaded successfully.", 1);
                } else {
                    $('#DOCName').val("No File");
                    empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                }
                $('#saveAttempt').prop('disabled', false);
            }
        });
    },

    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (!hdnUrl) {
            empr_helper.notify("Please upload a file to view.", 2);
        } else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },
}