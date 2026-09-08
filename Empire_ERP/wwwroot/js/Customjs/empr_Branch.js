var empr_Branch = {
    initEvents: function () {

        $(document).ready(function () {

            empr_Branch.InitQuickSearch();
            empr_Branch.InitCompany();
            empr_Branch.InitRTTypeDDL();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Branch.validateForm()) {
                            empr_Branch.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Branch.validateForm()) {
                        empr_Branch.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_Branch.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_Branch.GetBranchByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_Branch.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Branch.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Branch/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Branch.resetForm();
                    empr_Branch.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#B_NAME").val('');
        $("#B_SHORT_NAME").val('');
        $("#B_ADDRESS").val('');
        $("#B_TEL").val('');
        $("#B_GST").val('');
        $("#B_NTN").val('');
        $("#TIME_IN").val('');
        $("#TIME_OUT").val('');
        $("#CONTACT_NAME1").val('');
        $("#CONTACT_NO1").val('');
        $("#CONTACT_NAME2").val('');
        $("#CONTACT_NO2").val('');
        $("#CONTACT_NAME3").val('');
        $("#CONTACT_NO3").val('');
        $("#GROUP_PIC").val('');
        $("#BARCODE").val('');
        $("#companyhidden").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $('#CCODE').dxSelectBox('instance').option('value', null);

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
        var GROUP_NAME = $("#B_NAME").val().trim();
        
        if (GROUP_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        return valid;
    },
    GetDataToSave: function () {
        debugger
        var ID = $("#Code").val();
        var B_NAME = $("#B_NAME").val();
        var B_SHORT_NAME = $("#B_SHORT_NAME").val();
        var B_ADDRESS = $("#B_ADDRESS").val();
        var B_TEL = $("#B_TEL").val();
        var B_GST = $("#B_GST").val();
        var B_NTN = $("#B_NTN").val();
        var TIME_IN = $("#TIME_IN").val();
        var TIME_OUT = $("#TIME_OUT").val();
        var CONTACT_NAME1 = $("#CONTACT_NAME1").val();
        var CONTACT_NO1 = $("#CONTACT_NO1").val();
        var CONTACT_NAME2 = $("#CONTACT_NAME2").val();
        var CONTACT_NO2 = $("#CONTACT_NO2").val();
        var CONTACT_NAME3 = $("#CONTACT_NAME3").val();
        var CONTACT_NO3 = $("#CONTACT_NO3").val();
        var BARCODE = $("#BARCODE").val();
        var B_LOGO = $("#GROUP_PIC").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');   
        var CCODE = $("#CCODE").dxSelectBox('instance').option('value');
        var RT_TYPE = $("#RT_TYPE").dxSelectBox('instance').option('value');
        var modelRecord = {
            BCODE: ID,
            B_NAME: B_NAME,
            B_SHORT_NAME: B_SHORT_NAME,
            B_ADDRESS: B_ADDRESS,
            B_TEL: B_TEL,
            B_GST: B_GST,
            B_NTN: B_NTN,
            TIME_IN: TIME_IN,
            TIME_OUT: TIME_OUT,
            CONTACT_NAME1: CONTACT_NAME1,
            CONTACT_NO1: CONTACT_NO1,
            CONTACT_NAME2: CONTACT_NAME2,
            CONTACT_NO2: CONTACT_NO2,
            CONTACT_NAME3: CONTACT_NAME3,
            CONTACT_NO3: CONTACT_NO3,
            B_LOGO: B_LOGO,
            CCODE: CCODE,
            BARCODE: BARCODE,
            ASTATUS: ASTATUS,
            RT_TYPE: RT_TYPE
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_Branch.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/Branch/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Branch.resetForm();
                empr_Branch.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    SaveImage: function () {
        $('#BtnSave').prop('disabled', true);
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob);
        $.ajax({
            url: "/Branch/SaveImage",
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
        empr_Branch.GetAllBranchTypes();
    },
    GetAllBranchTypes: function () {
        ajaxHelper.ajaxGetJson('/Branch/QuickSearch', function (data) {
            empr_Branch.CreateGrid(data.data);
        }, false, true);
    },
    GetBranchByID: function (id) {
        ajaxHelper.ajaxGetJson('/Branch/GetBranchById?id=' + id, function (data) {
            empr_Branch.resetForm();
            if (data.msgType == 1) {
                debugger
                var record = data.data;
                $("#Code").val(record.bcode);
                $('#companyhidden').val(data.data.ccode);
                $('#CCODE').dxSelectBox('instance').option("value", parseInt(data.data.ccode));
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $('#RT_TYPE').dxSelectBox('instance').option("value", parseInt(data.data.rT_TYPE));
                $("#B_NAME").val(record.b_NAME);
                $("#B_SHORT_NAME").val(record.b_SHORT_NAME);
                $("#B_ADDRESS").val(record.b_ADDRESS);
                $("#B_TEL").val(record.b_TEL);
                $("#B_GST").val(record.b_GST);
                $("#B_NTN").val(record.b_NTN);
                $("#TIME_IN").val(record.timE_IN);
                $("#TIME_OUT").val(record.timE_OUT);
                $("#CONTACT_NAME1").val(record.contacT_NAME1);
                $("#CONTACT_NO1").val(record.contacT_NO1);
                $("#CONTACT_NAME2").val(record.contacT_NAME2);
                $("#CONTACT_NO2").val(record.contacT_NO2);
                $("#CONTACT_NAME3").val(record.contacT_NAME3);
                $("#CONTACT_NO3").val(record.contacT_NO3);
                $("#GROUP_PIC").val(record.b_LOGO);
                $("#BARCODE").val(record.barcode);
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
        console.log(dataSrc)
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
                if (options.data.b_LOGO != null && options.data.b_LOGO != '' && options.data.b_LOGO != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.b_LOGO}')"><i class="fa fa-eye"></i></a>`;
                }
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.bcode} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
            {
                dataField: 'b_LOGO',
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
            { dataField: 'bcode', caption: 'Code' },
            { dataField: 'b_NAME', caption: 'Name' },
            { dataField: 'b_SHORT_NAME', caption: 'Short Name' },
            { dataField: 'b_TEL', caption: 'Telephone' },
            { dataField: 'b_GST', caption: 'GST #' },
            { dataField: 'b_NTN', caption: 'NTN #' },
            { dataField: 'timE_IN', caption: 'Time In' },
            { dataField: 'timE_OUT', caption: 'Time Out' },
            { dataField: 'b_ADDRESS', caption: 'Address' },
            { dataField: 'astatus', caption: 'Active' },
            { dataField: 'RT_TYPE', caption: 'RT Type' },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Branch");
    },
    InitCompany: function () {

        empr_Branch.bindDxDdl("CCODE", Company, null, "key", "value", "Select", function (d) {

            $('#companyhidden').val(d.value)
            if (d.value == null) {
                $('#companyhidden').val('');
            }

        });

    },
    InitRTTypeDDL: function (selectedValue) {

        var dataSource = [
            { key: 'Y', value: 'Y' },
            { key: 'N', value: 'N' },
        ];

        $('#RT_TYPE').dxSelectBox({
            dataSource: dataSource,
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
        });
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
}