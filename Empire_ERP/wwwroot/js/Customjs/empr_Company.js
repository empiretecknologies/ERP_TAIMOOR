var empr_Company = {
    initEvents: function () {
        $(document).ready(function () {

            empr_Company.InitQuickSearch();
            empr_Company.InitNature();

            $('#BtnSave').click(function () {
                if (empr_Company.validateForm()) {
                    empr_Company.saveAttempt();
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_Company.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_Company.GetCompanyByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_Company.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Company.DeleteRecord();
            });
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Company/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Company.resetForm();
                    empr_Company.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#C_NAME").val('');
        $("#C_ADDRESS").val('');
        $("#C_TEL").val('');
        $("#C_GST").val('');
        $("#C_NTN").val('');
        $("#naturehidden").val('');
        $('#BUS_NATURE').dxSelectBox('instance').option('value', null);
    },
    validateForm: function () {

        var valid = true;
        var C_NAME = $("#C_NAME").val().trim();

        if (C_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        return valid;
    },
    GetDataToSave: function () {
        debugger
        var ID = $("#Code").val();
        var C_NAME = $("#C_NAME").val();
        var C_ADDRESS = $("#C_ADDRESS").val();
        var C_TEL = $("#C_TEL").val();
        var C_GST = $("#C_GST").val();
        var C_NTN = $("#C_NTN").val();
        var C_LOGO = $("#GROUP_PIC").val();
        var C_WATER = $("#WATER_PIC").val();
        var BUS_NATURE = $("#BUS_NATURE").dxSelectBox('instance').option('value')
        var modelRecord = {
            CCODE: ID,
            C_NAME: C_NAME,
            C_ADDRESS: C_ADDRESS,
            C_TEL: C_TEL,
            C_GST: C_GST,
            C_NTN: C_NTN,
            C_LOGO: C_LOGO,
            C_WATER: C_WATER,
            BUS_NATURE: BUS_NATURE,
        }
        return modelRecord;
    },
    saveAttempt: function () {
        debugger;
        var obj = empr_Company.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/Company/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Company.resetForm();
                empr_Company.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_Company.GetAllCompanyTypes();
    },
    GetAllCompanyTypes: function () {
        ajaxHelper.ajaxGetJson('/Company/QuickSearch', function (data) {
            empr_Company.CreateGrid(data.data);
        }, false, true);
    },
    GetCompanyByID: function (id) {
        ajaxHelper.ajaxGetJson('/Company/GetCompanyById?id=' + id, function (data) {
            empr_Company.resetForm();
            if (data.msgType == 1) {
                debugger
                var record = data.data;
                $("#Code").val(record.ccode);
                $('#naturehidden').val(data.data.buS_NATURE);
                $('#BUS_NATURE').dxSelectBox('instance').option("value", parseInt(data.data.buS_NATURE));
                $("#C_NAME").val(record.c_NAME);
                $("#C_ADDRESS").val(record.c_ADDRESS);
                $("#C_TEL").val(record.c_TEL);
                $("#C_GST").val(record.c_GST);
                $("#C_NTN").val(record.c_NTN);
                $("#GROUP_PIC").val(record.c_LOGO);
                $("#WATER_PIC").val(record.c_WATER);
                $('.modal').modal('hide');
                $('#BtnDelete').show();
                $('#BtnNew').show();
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
                var html = '<div class="btn-group btn-group-sm">';
                //if (options.data.c_LOGO != null && options.data.c_LOGO != '' && options.data.c_LOGO != undefined) {
                //    html += `<a href="javascript:;" class="grid-action-icon" title="View Logo" onclick="ShowImage('/images/upload/logos/${options.data.c_LOGO}')"><i class="fa fa-eye"></i></a>`;
                //}
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.ccode} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        { dataField: 'ccode', caption: 'Code' },
        { dataField: 'c_NAME', caption: 'Name' },
        { dataField: 'c_TEL', caption: 'Telephone' },
        { dataField: 'c_GST', caption: 'GST #' },
        { dataField: 'c_NTN', caption: 'NTN #' },
        {
            // dataField: 'c_LOGO', caption: 'Logo '
            dataField: 'c_LOGO', caption: 'Logo',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.c_LOGO != null && options.data.c_LOGO != '' && options.data.c_LOGO != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('/images/upload/logos/${options.data.c_LOGO}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        {
            //dataField: 'c_WATER', caption: 'Watermark',
            dataField: 'c_WATER', caption: 'Watermark',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.c_WATER != null && options.data.c_WATER != '' && options.data.c_WATER != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('/images/upload/logos/${options.data.c_WATER}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        { dataField: 'c_ADDRESS', caption: 'Address' },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "CompanyQS");
    },
    InitNature: function () {

        empr_Company.bindDxDdl("BUS_NATURE", Nature, null, "key", "value", "Select", function (d) {

            $('#naturehidden').val(d.value)
            if (d.value == null) {
                $('#naturehidden').val('');
            }

        });

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    SaveImage(imageName) {
        $('#BtnSave').prop('disabled', true);
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob);
        $.ajax({
            url: "/Company/SaveImage",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    if (imageName != "" && imageName != null && imageName != undefined) {
                        $("#WATER_PIC").val(data.data);
                    }
                    else {
                        $("#GROUP_PIC").val(data.data);
                    }
                }
                else {
                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                }
                $('#BtnSave').prop('disabled', false);
            }
        });
    },
}