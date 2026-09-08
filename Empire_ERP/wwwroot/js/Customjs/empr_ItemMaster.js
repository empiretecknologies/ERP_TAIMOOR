var empr_ItemMaster = {
    isValueAssigned: false,
    initEvents: function () {
        $(document).ready(function () {
            empr_ItemMaster.InitUnitDDL();
            empr_ItemMaster.InitPackageDDL();
            empr_ItemMaster.InitTaxStatusDDL();
            empr_ItemMaster.InitItemBarcodeTypeDDL();
            empr_ItemMaster.InitBarcodeTypeDDL();
            //empr_ItemMaster.InitColorSingleDDL();
            empr_ItemMaster.InitColorDDL();
            empr_ItemMaster.InitSizeDDL();
            empr_ItemMaster.InitSizeMultipleDDL();
            empr_ItemMaster.InitColorMultipleDDL();
            //empr_ItemMaster.InitSizeSingleDDL();
            empr_ItemMaster.InitGradesDDL();
            empr_ItemMaster.InitBarcodeLabelDDL();
            empr_ItemMaster.InitCategoryDDL();

            empr_ItemMaster.InitItemGroupDDL();
            empr_ItemMaster.InitSubCategoryDDL();
            empr_ItemMaster.InitBrandDDL();
            empr_ItemMaster.InitStyleDDL();
            empr_ItemMaster.InitSeasonDDL();
            empr_ItemMaster.InitFabricDDL();
            empr_ItemMaster.resetForm();
            empr_ItemMaster.InitBarcodeInfoForm(true);
            //$("#Size").dxDropDownBox("instance").option("disabled", false);
            //$("#Color").dxDropDownBox("instance").option("disabled", false);
            //$("#BarCodeType").dxSelectBox("instance").option("disabled", false);


            $('#ITEM_NAME').on('input', function () {
                let val = $(this).val();
                let match = val.match(/\(([^)]+)\)/);
                if (match) {
                    $('#ITEM_SHORT_NAME').val(match[1]);
                } else {
                    $('#ITEM_SHORT_NAME').val('');
                }
            });
            $('#BtnSave').click(function () {
                if (empr_ItemMaster.ValidateMainInfo()) {
                    empr_ItemMaster.SaveAttempt();
                }
            });

            $('#AttributeBtnSave').click(function () {
                if (empr_ItemMaster.AttributeValidateMainInfo()) {
                    empr_ItemMaster.AttributeSaveAttempt();
                }
            });

            $('body').on('click', '#BtnSaveBarcode', function () {
                if (empr_ItemMaster.ValidateBarcodeInfo()) {
                    empr_ItemMaster.SaveBarcodeInfo();
                }
            });

            $('#ProductImage').change(function () {
                empr_ItemMaster.UploadImage();
            })

            $('#BtnDelete').click(function () {
                empr_ItemMaster.Delete($('#Code').val());
            });

            //$('#AttributeBtnDelete').click(function () {
            //    empr_ItemMaster.AttributeDelete($('#ATT_CODE').val());
            //});

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_ItemMaster.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid")
                empr_ItemMaster.resetForm();
                empr_ItemMaster.InitBarcodeInfoForm(true);
                empr_ItemMaster.GetItemMasterByCode(id);
                $("#Size").dxDropDownBox("instance").option("disabled", false);
                $("#Color").dxDropDownBox("instance").option("disabled", false);
                $("#BarCodeType").dxSelectBox("instance").option("disabled", false);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, iteM_NAME: $('#updatedName').val() }, "/ItemMaster/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_ItemMaster.GetItemMasterByCode(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '.elm_editBarcodeInfo', function () {
                var reportid = $(this).attr("reportid")
                var code = $('#Code').val();
                empr_ItemMaster.GetBarcodeInfoById(reportid, code);
            });

            $('body').on('click', '#BtnNew', function () {
                debugger;
                empr_ItemMaster.resetForm();
                empr_ItemMaster.InitBarcodeInfoForm(true);
                $("#Size").dxDropDownBox("instance").option("disabled", false);
                $("#Color").dxDropDownBox("instance").option("disabled", false);
                $("#BarCodeType").dxSelectBox("instance").option("disabled", false);
            });

            $('#BtnDeleteBarcode').click(function () {
                empr_ItemMaster.DeleteBarcodeInfo();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSaveBarcode').hide();
            }
            $("#barcode").prop("disabled", true);
        });
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_ItemMaster.GetDataToSave();

        if (data.ITEM_NAME == '') {
            empr_helper.notify("Item name is required.", 2);
            valid = false;
        }

        if (data.GROUP_CODE == '' || data.GROUP_CODE == null || data.GROUP_CODE == undefined) {
            empr_helper.notify("Item group is required.", 2);
            valid = false;
        }

        if (data.ITEM_TYPE == '' || data.ITEM_TYPE == null || data.ITEM_TYPE == undefined) {
            empr_helper.notify("Item type is required.", 2);
            valid = false;
        }

        //if (data.CAT_CODE == '' || data.CAT_CODE == null || data.CAT_CODE == undefined) {
        //    empr_helper.notify("Category is required.", 2);
        //    valid = false;
        //}

        //if (data.SUB_CAT_CODE == '' || data.SUB_CAT_CODE == null || data.SUB_CAT_CODE == undefined) {
        //    empr_helper.notify("Sub Category is required.", 2);
        //    valid = false;
        //}

        if (data.IUNIT_CODE == '' || data.IUNIT_CODE == null || data.IUNIT_CODE == undefined) {
            empr_helper.notify("Unit is required.", 2);
            valid = false;
        }

        if (data.PACK != '' && data.PACK != null && data.PACK != undefined) {
            if (data.PUNIT_CODE == '' || data.PUNIT_CODE == null || data.PUNIT_CODE == undefined) {
                empr_helper.notify("Packing Unit is required.", 2);
                valid = false;
            }
        }

        if (data.ITEM_MAX != '') {
            var max = 0;
            var min = 0;
            try {
                max = parseFloat(data.ITEM_MAX);
                if (isNaN(max)) {
                    empr_helper.notify("Please enter the correct maximum amount.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct maximum amount.", 2);
                valid = false;
                return valid;
            }

            try {
                min = parseFloat(data.ITEM_MIN);
                if (isNaN(min)) {
                    empr_helper.notify("Please enter the correct minimum amount.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct minimum amount.", 2);
                valid = false;
                return valid;
            }

            if (max < min) {
                empr_helper.notify("Minimum amount must be less than the maximum amount.", 2);
                valid = false;
            }
        }

        if (data.ITEM_MIN != '') {
            var max = 0;
            var min = 0;
            try {
                max = parseFloat(data.ITEM_MAX);
                if (isNaN(max)) {
                    empr_helper.notify("Please enter the correct maximum amount.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct maximum amount.", 2);
                valid = false;
                return valid;
            }

            try {
                min = parseFloat(data.ITEM_MIN);
                if (isNaN(min)) {
                    empr_helper.notify("Please enter the correct minimum amount.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct minimum amount.", 2);
                valid = false;
                return valid;
            }
        }

        return valid;
    },
    AttributeValidateMainInfo: function () {

        var valid = true;
        var data = empr_ItemMaster.AttributeGetDataToSave();

        //if (data.ITEM_CODE == '' || data.ITEM_CODE == null || data.ITEM_CODE == undefined) {
        //    empr_helper.notify("Item Code is Required.", 2);
        //    IsValid = false;
        //    return false;
        //}

        if (data.CAT_CODE == '' || data.CAT_CODE == null || data.CAT_CODE == undefined) {
            empr_helper.notify("Select Category.", 2);
            IsValid = false;
            return false;
        }

        if (data.SUB_CAT_CODE == '' || data.SUB_CAT_CODE == null || data.SUB_CAT_CODE == undefined) {
            empr_helper.notify("Select Sub Category.", 2);
            IsValid = false;
            return false;
        }

        if (data.FABRIC == '' || data.FABRIC == null || data.FABRIC == undefined) {
            empr_helper.notify("Select Fabric.", 2);
            IsValid = false;
            return false;
        }

        if (data.SEASON == '' || data.SEASON == null || data.SEASON == undefined) {
            empr_helper.notify("Select Season.", 2);
            IsValid = false;
            return false;
        }

        if (data.BRAND == '' || data.BRAND == null || data.BRAND == undefined) {
            empr_helper.notify("Select Brand.", 2);
            IsValid = false;
            return false;
        }

        if (data.STYLE == '' || data.STYLE == null || data.STYLE == undefined) {
            empr_helper.notify("Select Style.", 2);
            IsValid = false;
            return false;
        }

        return valid;
    },
    ValidateBarcodeInfo: function () {

        var valid = true;
        var data = empr_ItemMaster.GetBarcodeInfoTabData();

        if (data.CODE == '' || data.CODE == null || data.CODE == undefined) {
            if (data.BARCODE_TYPE == '' || data.BARCODE_TYPE == null || data.BARCODE_TYPE == undefined) {
                empr_helper.notify("Please enter the barcode type.", 2);
                valid = false;
                return valid;
            }

            if (data.BARCODE_TYPE == 1) {
                if (data.BARCODE == '' || data.BARCODE == null || data.BARCODE == undefined) {
                    empr_helper.notify("Please enter the barcode.", 2);
                    valid = false;
                    return valid;
                }

                if (data.COLOR == '' || data.COLOR == null || data.COLOR == undefined) {
                    empr_helper.notify("Please select the color.", 2);
                    valid = false;
                    return valid;
                }

                if (data.SIZE == '' || data.SIZE == null || data.SIZE == undefined) {
                    empr_helper.notify("Please select the size.", 2);
                    valid = false;
                    return valid;
                }
            }
            else {
                if (data.SELECTEDCOLORS == '' || data.SELECTEDCOLORS == null || data.SELECTEDCOLORS == undefined) {
                    empr_helper.notify("Please select the colors.", 2);
                    valid = false;
                    return valid;
                }

                if (data.SELECTEDSIZES == '' || data.SELECTEDSIZES == null || data.SELECTEDSIZES == undefined) {
                    empr_helper.notify("Please select the sizes.", 2);
                    valid = false;
                    return valid;
                }
            }
        }

        if (data.BLABEL == '' || data.BLABEL == null || data.BLABEL == undefined) {
            empr_helper.notify("Please select the barcode label.", 2);
            valid = false;
            return valid;
        }

        if (data.PRATE == '' || data.PRATE == null || data.PRATE == undefined) {
            empr_helper.notify("Please enter the purchase rate.", 2);
            valid = false;
            return valid;
        }
        else {
            try {
                var v = parseInt(data.PRATE);
                if (isNaN(v)) {
                    empr_helper.notify("Please enter the correct purchase rate.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct purchase rate.", 2);
                valid = false;
                return valid;
            }
        }


        if (data.SRATE == '' || data.SRATE == null || data.SRATE == undefined) {
            empr_helper.notify("Please enter the sale rate.", 2);
            valid = false;
            return valid;
        }
        else {
            try {
                var v = parseInt(data.SRATE);
                if (isNaN(v)) {
                    empr_helper.notify("Please enter the correct sale rate.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct sale rate.", 2);
                valid = false;
                return valid;
            }
        }

        if (data.WSALE == '' || data.WSALE == null || data.WSALE == undefined) {
            empr_helper.notify("Please enter the wholesale rate.", 2);
            valid = false;
            return valid;
        }
        else {
            try {
                var v = parseInt(data.WSALE);
                if (isNaN(v)) {
                    empr_helper.notify("Please enter the correct wholesale rate.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct wholesale rate.", 2);
                valid = false;
                return valid;
            }
        }

        if (data.RRATE == '' || data.RRATE == null || data.RRATE == undefined) {
            empr_helper.notify("Please enter the retail rate.", 2);
            valid = false;
            return valid;
        }
        else {
            try {
                var v = parseInt(data.RRATE);
                if (isNaN(v)) {
                    empr_helper.notify("Please enter the correct retail rate.", 2);
                    valid = false;
                    return valid;
                }
            }
            catch (e) {
                console.log(e);
                empr_helper.notify("Please enter the correct retail rate.", 2);
                valid = false;
                return valid;
            }
        }

        //if (data.DRATE == '' || data.DRATE == null || data.DRATE == undefined) {
        //    empr_helper.notify("Please enter the discount rate.", 2);
        //    valid = false;
        //    return valid;
        //}
        //else {
        //    try {
        //        var v = parseInt(data.DRATE);
        //        if (isNaN(v)) {
        //            empr_helper.notify("Please enter the correct discount rate.", 2);
        //            valid = false;
        //            return valid;
        //        }
        //    }
        //    catch (e) {
        //        console.log(e);
        //        empr_helper.notify("Please enter the correct discount rate.", 2);
        //        valid = false;
        //        return valid;
        //    }
        //}
        return valid;
    },
    InitQuickSearch: function () {
        empr_ItemMaster.GetDataForQuickSeachGrid();
    },
    GetDataForQuickSeachGrid: function () {
        ajaxHelper.ajaxGetJson('/ItemMaster/QuickSearch', function (data) {
            empr_ItemMaster.CreateGrid(data.data);
        }, false, true);
    },
    //CreateGrid: function (dataSrc) {

    //    var columns = [
    //        {
    //            dataField: "Action",
    //            width: 100,
    //            alignment: 'center',
    //            fixed: true,
    //            fixedPosition: "left",
    //            allowExporting: false,
    //            cellTemplate: function (container, options) {
    //                console.log(options);
    //                debugger;
    //                var html = '<div class="btn-group btn-group-sm">';
    //                if (options.data.ipic != null && options.data.ipic != '' && options.data.ipic != undefined) {
    //                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.ipic}')"><i class="fa fa-eye"></i></a>`;
    //                }
    //                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>`;
    //                html += `<a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportname="${options.data.iteM_NAME}" reportid=${options.data.id} title="COPY"><i class="fa fa-copy"></i></a>`;
    //                html += '</div>';
    //                $(html).appendTo(container);
    //            }
    //        },
    //        {
    //            dataField: 'ipic',
    //            caption: 'Image',
    //            width: 120,
    //            visible: false,
    //            cellTemplate(container, options) {
    //                if (options.value != null && options.value != '' && options.value != undefined) {
    //                    $('<div>')
    //                        .append($('<img>', { src: options.value, height: '100px', width: '100px' }))
    //                        .appendTo(container);
    //                }
    //            },
    //        },
    //        {
    //            dataField: 'iteM_NAME',
    //            caption: 'Name',
    //        },
    //        {
    //            dataField: 'iteM_SHORT_NAME',
    //            caption: 'Short Name',
    //        },
    //        {
    //            dataField: 'astatus',
    //            caption: 'Status',
    //        },
    //        {
    //            dataField: 'iteM_TYPE',
    //            caption: 'Item Type',
    //        },
    //        {
    //            dataField: 'caT_CODE',
    //            caption: 'Category',
    //        },
    //        {
    //            dataField: 'suB_CAT_CODE',
    //            caption: 'Sub Category',
    //        },
    //        {
    //            dataField: 'fabric',
    //            caption: 'Fabric',
    //        },
    //        {
    //            dataField: 'season',
    //            caption: 'Season',
    //        },
    //        {
    //            dataField: 'brand',
    //            caption: 'Brand',
    //        },
    //        {
    //            dataField: 'style',
    //            caption: 'Style',
    //        },
    //        {
    //            dataField: 'pack',
    //            caption: 'Pack.',
    //        },
    //        {
    //            dataField: 'salE_RATE',
    //            caption: 'Sale Rate.',
    //        },
    //        {
    //            dataField: 'purchasE_RATE',
    //            caption: 'Purchase Rate.',
    //        },
    //        {
    //            dataField: 'salestax',
    //            caption: 'Sale Tax',
    //        },
    //        {
    //            dataField: 'iteM_MAX',
    //            caption: 'Max',
    //        },
    //        {
    //            dataField: 'iteM_MINI',
    //            caption: 'Min',
    //        },
    //        {
    //            dataField: 'iteM_ID',
    //            caption: 'Item ID',
    //            visible: false,
    //        },
    //        {
    //            dataField: 'remarks',
    //            caption: 'Remarks',
    //            visible: false,
    //        },
    //        //{
    //        //    dataField: 'adD_USER_ID',
    //        //    caption: 'Created By',
    //        //    visible: false,
    //        //},
    //        //{
    //        //    dataField: 'adD_DATE',
    //        //    caption: 'Created Date',
    //        //    dataType: 'date',
    //        //    visible: false,
    //        //    format: 'dd-MM-yyy'
    //        //},
    //        //{
    //        //    dataField: 'adD_COMPUTER_NAME',
    //        //    caption: 'Created Computer',
    //        //    visible: false,
    //        //},
    //        //{
    //        //    dataField: 'adD_IP_ADDRESS',
    //        //    caption: 'Created IP',
    //        //    visible: false,
    //        //},
    //        //{
    //        //    dataField: 'ediT_USER_ID',
    //        //    caption: 'Updated By',
    //        //    visible: false,
    //        //},
    //        //{
    //        //    dataField: 'ediT_DATE',
    //        //    caption: 'Updated Date',
    //        //    dataType: 'date',
    //        //    visible: false,
    //        //    format: 'dd-MM-yyy'
    //        //},
    //        //{
    //        //    dataField: 'ediT_COMPUTER_NAME',
    //        //    caption: 'Updated Computer',
    //        //    visible: false,
    //        //},
    //        //{
    //        //    dataField: 'ediT_IP_ADDRESS',
    //        //    caption: 'Updated IP',
    //        //    visible: false,
    //        //},
    //    ];
    //    empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "ItemMaster");
    //    //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/ItemMaster/QuickSearch", "id", "ItemMaster");
    //},
    CreateGrid: function (dataSrc) {

        var columns = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    console.log(options);
                    debugger;
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.ipic != null && options.data.ipic != '' && options.data.ipic != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.ipic}')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>`;
                    html += `<a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportname="${options.data.iteM_NAME}" reportid=${options.data.id} title="COPY"><i class="fa fa-copy"></i></a>`;
                    html += '</div>';
                    $(html).appendTo(container);
                }
            },
            {
                dataField: 'ipic',
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
           
            {
                dataField: 'astatus',
                caption: 'Status',
            },
            {
                dataField: 'id',
                caption: 'Code',
                alignment:'center'
            },
            {
                dataField: 'iteM_NAME',
                caption: 'Name',
            },
            {
                dataField: 'iteM_SHORT_NAME',
                caption: 'Model #',
            },
            {
                dataField: 'iteM_TYPE',
                caption: 'Item Type',
            },
            {
                dataField: 'grouP_NAME',
                caption: ' Item Group',
            },
            {
                dataField: 'unit',
                caption: 'Unit',
            },
            {
                dataField: 'pack',
                caption: 'Pack.',
            },
            {
                dataField: 'punit',
                caption: 'Packing Unit',
            },
            {
                dataField: 'salestax',
                caption: 'Sale Tax',
            },
            {
                dataField: 'taX_TYPE',
                caption: 'Tax Type',
            },
            {
                dataField: 'weight',
                caption: 'Weight',
            },
            {
                dataField: 'salE_RATE',
                caption: 'Sale Rate.',
            },
            {
                dataField: 'purchasE_RATE',
                caption: 'Purchase Rate.',
            },
            {
                dataField: 'iteM_MAX',
                caption: 'Max',
            },
            {
                dataField: 'iteM_MINI',
                caption: 'Min',
            },
            {
                dataField: 'caT_CODE',
                caption: 'Category',
            },
            {
                dataField: 'hS_CODE',
                caption: 'HS Code',
            },
            {
                dataField: 'remarks',
                caption: 'Remarks',
                visible: false,
            },
            {
                dataField: 'suB_CAT_CODE',
                caption: 'Sub Category',
            },
            {
                dataField: 'fabric',
                caption: 'Fabric',
            },
            {
                dataField: 'season',
                caption: 'Season',
            },
            {
                dataField: 'brand',
                caption: 'Brand',
            },
            {
                dataField: 'style',
                caption: 'Style',
            },
            {
                dataField: 'iteM_ID',
                caption: 'Item ID',
                visible: false,
            },
            //{
            //    dataField: 'adD_USER_ID',
            //    caption: 'Created By',
            //    visible: false,
            //},
            //{
            //    dataField: 'adD_DATE',
            //    caption: 'Created Date',
            //    dataType: 'date',
            //    visible: false,
            //    format: 'dd-MM-yyy'
            //},
            //{
            //    dataField: 'adD_COMPUTER_NAME',
            //    caption: 'Created Computer',
            //    visible: false,
            //},
            //{
            //    dataField: 'adD_IP_ADDRESS',
            //    caption: 'Created IP',
            //    visible: false,
            //},
            {
                dataField: 'ediT_USER_ID',
                caption: 'Created By',
                visible: false,
            },
            {
                dataField: 'ediT_DATE',
                caption: 'Create Date',
                dataType: 'date',
                visible: false,
                format: 'dd-MM-yyy'
            },
            //{
            //    dataField: 'ediT_COMPUTER_NAME',
            //    caption: 'Updated Computer',
            //    visible: false,
            //},
            //{
            //    dataField: 'ediT_IP_ADDRESS',
            //    caption: 'Updated IP',
            //    visible: false,
            //},
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "ItemMaster");
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/ItemMaster/QuickSearch", "id", "ItemMaster");
    },

    GetItemMasterByCode: function (id) {

        ajaxHelper.ajaxGetJson('/ItemMaster/GetItemMasterByCode?code=' + id, function (data) {
            console.log('GetItemMasterByCode', data);
            debugger;
            if (data.mainData.msgType == 1) {
                var response = data.mainData.data;
                var attributeResponse = data.mainData.data2;

                //$('#pills-warningprofile-tab').removeClass('active');

                empr_ItemMaster.InitUnitDDL(response.iuniT_CODE);
                empr_ItemMaster.InitPackageDDL(response.puniT_CODE);
                //$('#ITAX_STATUS').dxSelectBox('instance').option("value", iTaxStatusValue);
                empr_ItemMaster.InitTaxStatusDDL(response.itaX_STATUS);
                empr_ItemMaster.InitGradesDDL(response.grade);
                empr_ItemMaster.InitItemGroupDDL(response.grouP_CODE);
                empr_ItemMaster.InitItemTypeDDL(response.iteM_TYPE);
                //empr_ItemMaster.InitCategoryDDL(response.caT_CODE);
                //empr_ItemMaster.InitSubCategoryDDL(response.suB_CAT_CODE);
                $('#ASTATUS').dxSelectBox('instance').option("value", response.astatus);
                $('#COLOR').dxSelectBox('instance').option("value", response.color);
                $('#SIZE').dxSelectBox('instance').option("value", response.size);
                $("#ID").val(response.id)
                $("#HS_CODE").val(response.hS_CODE)
                $("#Code").val(response.iteM_CODE)
                $("#ITEM_ID").val(response.iteM_ID)
                $("#ADD_USER_ID").val(response.adD_USER_ID)
                $("#ADD_DATE").val(response.adD_DATE)
                $("#ADD_COMPUTER_NAME").val(response.adD_COMPUTER_NAME)
                $("#ADD_IP_ADDRESS").val(response.adD_IP_ADDRESS)
                $("#ADD_POSTALCODE").val(response.adD_POSTALCODE)
                $("#MENU_ID").val(response.menU_ID)
                $("#ITEM_NAME").val(response.iteM_NAME)
                $("#ITEM_SHORT_NAME").val(response.iteM_SHORT_NAME)
                $("#REMARKS").val(response.remarks)
                $("#PACK").val(response.pack)
                $("#WEIGHT").val(response.weight)
                $("#SALE_RATE").val(response.salE_RATE)
                $("#PURCHASE_RATE").val(response.purchasE_RATE)
                $("#SALESTAX").val(response.salestax)
                $("#ITEM_MAX").val(response.iteM_MAX)
                $("#ITEM_MIN").val(response.iteM_MIN)
                $("#ProductImageHidden").val(response.ipic)
                $('#pills-warningattributes-tab').show();
                if (response.bitype == null || response.bitype == '' || response.bitype == undefined) {
                    $('#BITYPE').dxSelectBox('instance').option("value", 2);
                } else {
                    $('#BITYPE').dxSelectBox('instance').option("value", response.bitype);
                }
                $("#BARCODE").val(response.barcode);
                // Attribute Data filling
                if (attributeResponse.code == 0) {
                    $("#ATT_CODE").val();
                }
                else {
                    $("#ATT_CODE").val(attributeResponse.code);
                }
                empr_ItemMaster.InitCategoryDDL(attributeResponse.caT_CODE);
                empr_ItemMaster.InitSubCategoryDDL(attributeResponse.suB_CAT_CODE);
                empr_ItemMaster.InitBrandDDL(attributeResponse.brand);
                empr_ItemMaster.InitStyleDDL(attributeResponse.style);
                empr_ItemMaster.InitSeasonDDL(attributeResponse.season);
                empr_ItemMaster.InitFabricDDL(attributeResponse.fabric);


                if (BarcodeVisible == 'B')
                    $('#pills-warningprofile-tab').show();

                //$('#BtnDelete').show();
                //$('#BtnNew').show();
                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
                        //$('#AttributeBtnDelete').show();
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
                    //if ($('#ATT_CODE').val() > 0) {
                    //    $('#AttributeBtnDelete').show();
                    //}
                    
                }

                var barcodeData = data.barCodeData.data;
                if (barcodeData != null) {
                    empr_ItemMaster.InitBarcodeInfoGrid(barcodeData);
                }
                $('.modal').modal('hide');
            }
            else {
                empr_helper.notify(data.mainData.msg, 2);
            }
        }, false, true);
    },
    InitBarcodeInfoGrid: function (dataSrc) {
        var col = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger
                    var data = JSON.stringify(options.data);
                    var data_ = encodeURI(data);
                    var code = $()
                    $(`<div class="btn-group btn-group-sm">
                            <a href="javascript:;"  class="grid-action-icon elm_editBarcodeInfo" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                            <a href="javascript:;"  class="grid-action-icon Delete" style = "margin-left: 8px" onclick = "empr_ItemMaster.DeleteBarcodeInfo(${options.data.id})" title = "Delete" > <i class="fa fa-trash"></i></a>
                            </div>`).appendTo(container);
                }
            },
            {
                dataField: 'barcode',
                caption: 'Bar Code',
            },
            {
                dataField: 'color',
                caption: 'Color',
            },
            {
                dataField: 'size',
                caption: 'Size',
            },
            {
                dataField: 'blabel',
                caption: 'Barcode Label',
            },
            {
                dataField: 'prate',
                caption: 'Purchase',
            },
            {
                dataField: 'srate',
                caption: 'Sale',
            },
            {
                dataField: 'wsale',
                caption: 'Whole Sale.',
            },
            {
                dataField: 'rrate',
                caption: 'Retail.',
            },
            {
                dataField: 'drate',
                caption: 'Discount',
            },
            {
                dataField: 'adD_USER_ID',
                caption: 'Created By',
                visible: false,
            },
            {
                dataField: 'adD_DATE',
                caption: 'Created Date',
                visible: false,
                dataType: 'date',
                format: 'dd-MM-yyy'
            },
            {
                dataField: 'adD_COMPUTER_NAME',
                caption: 'Created Computer',
                visible: false,
            },
            {
                dataField: 'adD_IP_ADDRESS',
                caption: 'Created IP',
                visible: false,
            },
            {
                dataField: 'ediT_USER_ID',
                caption: 'Updated By',
                visible: false,
            },
            {
                dataField: 'ediT_DATE',
                caption: 'Updated Date',
                visible: false,
                dataType: 'date',
                format: 'dd-MM-yyy'
            },
            {
                dataField: 'ediT_COMPUTER_NAME',
                caption: 'Updated Computer',
                visible: false,
            },
            {
                dataField: 'ediT_IP_ADDRESS',
                caption: 'Updated IP',
                visible: false,
            },
        ]
        empr_helper.dxGridbindingVouchers('#DetailgridContainer', col, dataSrc, "BarcodeInfoGrid");
    },
    InitBarcodeInfoForm: function (IsReset) {
        $('#BtnDeleteBarcode').hide();
        $('#detailId').val('');
        $("#barcode").val('');
        $("#BarCodeType").dxSelectBox('instance').option('value', 2);
        $("#barcode").prop("disabled", true);
        $("#colorsingle_hidden").val('');
        $("#sizesingle_hidden").val('');
        $("#displayExpr_colorsingle").val('');
        $("#displayExpr_sizesingle").val('');
        $("#color_hidden").val('');
        $("#size_hidden").val('');
        $("#displayExpr_color").val('');
        $("#displayExpr_size").val('');
        $("#Purchase").val('');
        $("#Sale").val('');
        $("#WEIGHT").val('');
        $("#WholeSale").val('');
        $("#Retail").val('');
        $("#Discount").val('');
        $("#ColorSingle").empty();
        $("#SizeSingle").empty();
        $("#Color").empty();
        $("#Size").empty();
        empr_ItemMaster.InitColorMultipleDDL();
        empr_ItemMaster.InitSizeMultipleDDL();

        if (IsReset) {
            $("#label_hidden").val('');
            $("#displayExpr_label").val('');
            empr_ItemMaster.InitBarcodeLabelDDL();
        }
        $("#Color").hide();
        $("#Size").hide();
        $("#ColorSingle").hide();
        $("#SizeSingle").hide();
        $("#Color").show();
        $("#Size").show();
        //$("#barcode").prop("disabled", false);

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSaveBarcode').show();
            } else {
                $('#BtnSaveBarcode').hide();
            }
        } else {
            $('#BtnSaveBarcode').show();
        }
    },
    SaveBarcodeInfo: function () {
        debugger;
        var data = empr_ItemMaster.GetBarcodeInfoTabData();
        ajaxHelper.ajaxPostJsonData(data, "/ItemMaster/SaveBarcodeInfo", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                //$('#BtnDeleteBarcode').show();
                empr_ItemMaster.InitBarcodeInfoForm();
                empr_ItemMaster.GetBarcodeInfo();
                $("#Size").dxDropDownBox("instance").option("disabled", false);
                $("#Color").dxDropDownBox("instance").option("disabled", false);
                $("#BarCodeType").dxSelectBox("instance").option("disabled", false);
            }
        }, false, true);
    },
    GetBarcodeInfoTabData: function () {

        var ITEM_CODE = $("#Code").val();
        var CODE = $("#detailId").val();
        var barcode = $("#barcode").val().trim();
        var BarCodeType = $("#BarCodeType").dxSelectBox('instance').option('value');
        var Color = $("#colorsingle_hidden").val();
        var Size = $("#sizesingle_hidden").val();
        var SelectedColors = $("#color_hidden").val();
        var SelectedSizes = $("#size_hidden").val();
        var Purchase = $("#Purchase").val();
        var Sale = $("#Sale").val();
        var WholeSale = $("#WholeSale").val();
        var Retail = $("#Retail").val();
        var Discount = $("#Discount").val();
        var Size = $("#sizesingle_hidden").val();
        var BLabel = $("#label_hidden").val();
        var modelRecord = {
            CODE: CODE,
            ITEM_CODE: ITEM_CODE,
            BARCODE: barcode,
            BARCODE_TYPE: BarCodeType,
            COLOR: Color,
            SIZE: Size,
            SELECTEDCOLORS: SelectedColors,
            SELECTEDSIZES: SelectedSizes,
            PRATE: Purchase,
            SRATE: Sale,
            WSALE: WholeSale,
            RRATE: Retail,
            DRATE: Discount,
            BLABEL: BLabel
        }
        return modelRecord;
    },
    DeleteBarcodeInfo: function (id) {
        console.log(id);
        debugger;
        if (!id) {
            id = $('#detailId').val();
        }

        code = $('#Code').val();
        var xhr = ajaxHelper.ajaxPostJsonData(null, "/ItemMaster/DeleteBarcodeInfo?detailID=" + id + "&code=" + code, function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#BtnDeleteBarcode').hide();
                empr_ItemMaster.InitBarcodeInfoForm();
                empr_ItemMaster.GetBarcodeInfo();
                $("#SizeSingle").dxDropDownBox("instance").option("disabled", false);
                $("#ColorSingle").dxDropDownBox("instance").option("disabled", false);
                $("#BarCodeType").dxSelectBox("instance").option("disabled", false);
            }
        }, false, true);
    },
    GetBarcodeInfo: function () {

        var code = $('#Code').val();
        if (code > 0) {
            ajaxHelper.ajaxGetJson('/ItemMaster/GetBarcodeInfoByCode?code=' + code, function (data) {

                debugger;
                empr_ItemMaster.InitBarcodeInfoGrid(data.data);

            }, false, true);
        }

    },
    GetBarcodeInfoById: function (id, code) {
        ajaxHelper.ajaxGetJson('/ItemMaster/GetBarcodeInfoById?barcodeID=' + id + "&code=" + code, function (data) {
            debugger;
            var response = data.data;
            //$('#BtnDeleteBarcode').show();

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('#BtnDeleteBarcode').show();
                }
                if (Permissions.r_EDIT) {
                    $('#BtnSaveBarcode').show();
                }
                else {
                    $('#BtnSaveBarcode').hide();
                }
            } else {
                $('#BtnSaveBarcode').show();
                $('#BtnDeleteBarcode').show();
            }

            $("#detailId").val(response.code)
            empr_ItemMaster.InitBarcodeTypeDDL(response.barcodE_TYPE);
            empr_ItemMaster.InitColorSingleDDL(response.color);
            empr_ItemMaster.InitSizeSingleDDL(response.size);
            empr_ItemMaster.InitBarcodeLabelDDL(response.blabel);

            //$("#colorsingle_hidden").val(response.color);
            //("#sizesingle_hidden").val(response.size);

            $("#Purchase").val(response.prate)
            $("#Sale").val(response.srate)
            $("#WholeSale").val(response.wsale)
            $("#Retail").val(response.rrate)
            $("#Discount").val(response.drate)
            $("#barcode").val(response.barcode)
            //if (response.barcodE_TYPE == "1") {
            //    $("#color").addClass("hide");
            //    $("#size").addClass("hide");
            //    $("#barcode").prop("disabled", false);
            //}
            //else {
            //    $("#color").addClass("hide");
            //    $("#size").addClass("hide");
            //    $("#barcode").prop("disabled", true);
            //}

            if (response.barcodE_TYPE == 1) {
                $("#barcode").prop("disabled", false);
            } else {
                $("#barcode").prop("disabled", true);
            }

            $("#color").empty();
            $("#size").empty();
            //$("#barcode").prop("disabled", false);
            $("#BarCodeType").dxSelectBox("instance").option("disabled", true);
            $("#SizeSingle").dxDropDownBox("instance").option("disabled", true);
            $("#ColorSingle").dxDropDownBox("instance").option("disabled", true);

            empr_ItemMaster.InitColorSingleDDL(response.color);
            empr_ItemMaster.InitSizeSingleDDL(response.size);

        }, false, true);
    },
    Delete: function (_id) {

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
            ajaxHelper.ajaxPostJsonData({ code: _id }, "/ItemMaster/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ItemMaster.resetForm();
                }
            }, false, true);
        });
    },

    //AttributeDelete: function (_id) {

    //    swal({
    //        title: 'Are you sure you want to remove this record?',
    //        text: "You won't be able to revert this!",
    //        type: 'warning',
    //        showCancelButton: true,
    //        confirmButtonColor: '#0CC27E',
    //        cancelButtonColor: '#FF586B',
    //        confirmButtonText: 'Yes, delete it!',
    //        cancelButtonText: 'No, cancel!',
    //        confirmButtonClass: 'btn btn-success mr-5',
    //        cancelButtonClass: 'btn btn-danger',
    //        buttonsStyling: false
    //    }).then(function () {
    //        ajaxHelper.ajaxPostJsonData({ code: _id }, "/ItemMaster/AttributeDelete", function (data) {
    //            empr_helper.notify(data.msg, data.msgType);
    //            if (data.msgType == 1) {
    //                $("#ATT_CODE").val('');
    //                empr_ItemMaster.InitCategoryDDL();
    //                empr_ItemMaster.InitSubCategoryDDL();
    //                empr_ItemMaster.InitBrandDDL();
    //                empr_ItemMaster.InitStyleDDL();
    //                empr_ItemMaster.InitSeasonDDL();
    //                empr_ItemMaster.InitFabricDDL();
    //                //$('#AttributeBtnDelete').hide();
    //            }
    //        }, false, true);
    //    });
    //},
    GetDataToSave: function () {

        var ID = $("#Code").val();
        var HS_CODE = $("#HS_CODE").val();
        var ITEM_NAME = $("#ITEM_NAME").val().trim();
        var ITEM_ID = $("#ITEM_ID").val().trim();
        var ITEM_SHORT_NAME = $("#ITEM_SHORT_NAME").val().trim();
        var REMARKS = $("#REMARKS").val().trim();
        var IUNIT_CODE = $("#unitcode_hidden").val();
        var PACK = $("#PACK").val();
        var PUNIT_CODE = $("#package_hidden").val();
        var BITYPE = $("#BITYPE").dxSelectBox('instance').option('value');
        var BARCODE = $("#BARCODE").val();
        var ITAX_STATUS = $("#ITAX_STATUS").dxSelectBox('instance').option('value');
        var SALE_RATE = $("#SALE_RATE").val();
        var PURCHASE_RATE = $("#PURCHASE_RATE").val();
        var SALESTAX = $("#SALESTAX").val();
        var WEIGHT = $("#WEIGHT").val();
        var ITEM_MAX = $("#ITEM_MAX").val();
        var ITEM_MIN = $("#ITEM_MIN").val();
        var ProductImage = $("#ProductImageHidden").val();
        var GRADE = $("#grade_hidden").val();
        var GROUP_CODE = $("#GROUP_CODE").dxSelectBox('instance').option('value');
        var ASTATUS = $('#ASTATUS').dxSelectBox('instance').option('value');
        var ITEM_TYPE = $('#ITEM_TYPE').dxSelectBox('instance').option('value');
        var COLOR = $('#COLOR').dxSelectBox('instance').option('value');
        var SIZE = $('#SIZE').dxSelectBox('instance').option('value');
        //var CAT_CODE = $('#CAT_CODE').dxSelectBox('instance').option('value');
        //var SUB_CAT_CODE = $('#SUB_CAT_CODE').dxSelectBox('instance').option('value');
        var modelRecord = {
            ITEM_CODE: ID,
            HS_CODE: HS_CODE,
            ITEM_ID: ITEM_ID,
            ITEM_NAME: ITEM_NAME,
            ITEM_SHORT_NAME: ITEM_SHORT_NAME,
            ASTATUS: ASTATUS,
            REMARKS: REMARKS,
            IUNIT_CODE: IUNIT_CODE,
            PACK: PACK,
            PUNIT_CODE: PUNIT_CODE,
            SALE_RATE: SALE_RATE,
            PURCHASE_RATE: PURCHASE_RATE,
            SALESTAX: SALESTAX,
            ITAX_STATUS: ITAX_STATUS,
            ITEM_MAX: ITEM_MAX,
            ITEM_MIN: ITEM_MIN,
            IPIC: ProductImage,
            GRADE: GRADE,
            GROUP_CODE: GROUP_CODE,
            ITEM_TYPE: ITEM_TYPE,
            WEIGHT: WEIGHT,
            COLOR: COLOR,
            SIZE: SIZE,
            BITYPE: BITYPE,
            BARCODE: BARCODE
            //CAT_CODE: CAT_CODE,
            //SUB_CAT_CODE: SUB_CAT_CODE
        }

        return modelRecord;

    },
    AttributeGetDataToSave: function () {

        var ID = $("#Code").val();
        //var ITEM_CODE = $("#Code").val();
        var CAT_CODE = $("#CAT_CODE").dxSelectBox('instance').option('value');
        var SUB_CAT_CODE = $("#SUB_CAT_CODE").dxSelectBox('instance').option('value');
        var FABRIC = $("#FABRIC").dxSelectBox('instance').option('value');
        var SEASON = $("#SEASON").dxSelectBox('instance').option('value');
        var BRAND = $("#BRAND").dxSelectBox('instance').option('value');
        var STYLE = $("#STYLE").dxSelectBox('instance').option('value');

        var modelRecord = {
            CODE: ID,
            //ITEM_CODE: ITEM_CODE,
            CAT_CODE: CAT_CODE,
            SUB_CAT_CODE: SUB_CAT_CODE,
            FABRIC: FABRIC,
            SEASON: SEASON,
            BRAND: BRAND,
            STYLE: STYLE,
        }

        return modelRecord;

    },
    SaveAttempt: function () {
        debugger;
        var dataModel = empr_ItemMaster.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/ItemMaster/SaveMainInfo", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#pills-warningattributes-tab').show();
                if (BarcodeVisible == 'B')
                    $('#pills-warningprofile-tab').show();
                if (dataClear == 1) {
                    $('#BtnNew').show();
                    $('#BtnDelete').show();
                    empr_ItemMaster.GetBarcodeInfo();

                } else {
                    empr_ItemMaster.resetForm();
                }

          
            }
        }, false, true);
    },
    AttributeSaveAttempt: function () {
        debugger;
        var dataModel = empr_ItemMaster.AttributeGetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/ItemMaster/SaveAttributeInfo", function (data) {
            //console.log('SaveAttributeInfo', data);
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                //$('#Code').val(data.data);
                //$('#pills-warningattributes-tab').show();
                //if (BarcodeVisible == 'B')
                //    $('#pills-warningprofile-tab').show();

                //$('#BtnNew').show();
                //$('#Code').val(data.data);
                //$('#AttributeBtnDelete').show();
                //empr_ItemMaster.GetBarcodeInfo();
            }
        }, false, true);
    },
    CalculateRate: function (element) {

        var inputValue = $(element).val();
        var sanitizedValue = '';

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 12) {
                if (/[\d]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);

        empr_ItemMaster.CalculateSaleRate();
        empr_ItemMaster.CalculateWholeRate();
        empr_ItemMaster.CalculateRetailRate();
    },
    CalculateSaleRate: function (element) {
        var inputValue = $(element).val();
        var sanitizedValue = '';

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 12) {
                if (/[\d]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);

        var purchase = parseFloat($("#Purchase").val()) || 0;
        var purchasePer = parseFloat($("#PurchasePer").val()) || 0;
        if (purchasePer > 0) {
            var purchasePerAmt = (purchasePer * purchase) / 100;
            $("#Sale").val((purchase + purchasePerAmt).toFixed(0));
        }
    },
    CalculateWholeRate: function (element) {
        var inputValue = $(element).val();
        var sanitizedValue = '';

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 12) {
                if (/[\d]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);

        var sale = parseFloat($("#Sale").val()) || 0;
        var salePer = parseFloat($("#SalePer").val()) || 0;
        if (salePer > 0) {
            var salePerAmt = (salePer * sale) / 100;
            $("#WholeSale").val((sale + salePerAmt).toFixed(0));
        }
    },
    CalculateRetailRate: function (element) {
        var inputValue = $(element).val();
        var sanitizedValue = '';

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 12) {
                if (/[\d]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);

        var sale = parseFloat($("#Sale").val()) || 0;
        var wholeSalePer = parseFloat($("#WholeSalePer").val()) || 0;
        if (wholeSalePer > 0) {
            var wholePerAmt = (wholeSalePer * sale) / 100;
            $("#Retail").val((sale + wholePerAmt).toFixed(0));
        }
    },
    resetForm: function () {

        $('#pills-warningprofile-tab').hide();
        $('#pills-warningattributes-tab').hide();
        $('#pills-warningtab li:first-child a').click();
        $('.tab-pane').removeClass('fade');
        $('#pills-warninghome-tab').addClass('active')
        $('#BtnDelete').hide();
        //$('#AttributeBtnDelete').hide();
        $('#BtnNew').hide();
        $('#pills-warningprofile-tab').removeClass('active');
        $('#pills-warningattributes-tab').removeClass('active');

        $("#unitcode_hidden").val('');
        $("#displayExpr_unitcode").val('');
        $("#package_hidden").val('');
        $("#displayExpr_package").val('');
        $("#grade_hidden").val('');
        $("#displayExpr_grade").val('');

        empr_ItemMaster.InitUnitDDL();
        empr_ItemMaster.InitPackageDDL();
        empr_ItemMaster.InitTaxStatusDDL();
        empr_ItemMaster.InitGradesDDL();
        empr_ItemMaster.InitItemBarcodeTypeDDL(2);
        empr_ItemMaster.InitItemTypeDDL();

        empr_ItemMaster.InitCategoryDDL();
        empr_ItemMaster.InitSubCategoryDDL();
        empr_ItemMaster.InitFabricDDL();
        empr_ItemMaster.InitSeasonDDL();
        empr_ItemMaster.InitBrandDDL();
        empr_ItemMaster.InitStyleDDL();

        $("#ATT_CODE").val('');
        $("#Code").val('');
        $("#ITEM_NAME").val('');
        $("#ITEM_ID").val('');
        $("#BARCODE").val(''); 
        $("#HS_CODE").val('');
        $("#ITEM_SHORT_NAME").val('');
        $("#REMARKS").val('');
        $("#PACK").val('');
        $("#SALE_RATE").val('');
        $("#PURCHASE_RATE").val('');
        $("#SALESTAX").val('');
        $("#ITEM_MAX").val('');
        $("#ITEM_MIN").val('');
        $("#ProductImage").val('');
        $("#ProductImageHidden").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $("#barcode").prop("disabled", true);

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
    UploadImage: function () {

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
        $.ajax(
            {
                url: "/ItemMaster/UploadImage",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#ProductImageHidden").val(data.data);
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
    //InitUnitDDL: function (_selectedValue) {

    //    $.ajax({
    //        url: 'ItemMaster/GetUnits',
    //        method: 'GET',
    //        data: null,
    //        success: function (data) {

    //            var selectedvalue = 0;
    //            var selectedobj = [];
    //            if (_selectedValue != null) {
    //                selectedobj = data.filter(x => x.key == _selectedValue);
    //                if (selectedobj.length > 0) {
    //                    selectedvalue = _selectedValue;
    //                    $("#unitcode_hidden").val(selectedvalue);
    //                    $("#displayExpr_unitcode").val(selectedobj[0].value);
    //                }
    //            }

    //            empr_ItemMaster.bindDxGridBoxDdl('#IUNIT_CODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
    //                if (selectedvalue.selectedRowsData.length > 0) {
    //                    var key = selectedvalue.selectedRowsData[0]['key'];
    //                    var value = selectedvalue.selectedRowsData[0]['value'];
    //                    $('#unitcode_hidden').val(key);
    //                    $('#displayExpr_unitcode').val(value);
    //                }
    //                else {
    //                    $('#unitcode_hidden').val('');
    //                    $('#displayExpr_unitcode').val('');
    //                }
    //            });
    //        },
    //        error: function (error) {
    //            console.error('Error fetching data:', error);
    //        }
    //    });


    //},
    InitUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'ItemMaster/GetUnits',
            method: 'GET',
            data: null,
            success: function (data) {
                if (!data || data.length === 0) return; 

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                }
                if (selectedobj.length === 0) {
                    selectedobj = [data[0]]; 
                    selectedvalue = data[0].key;
                } else {
                    selectedvalue = _selectedValue;
                }

                $("#unitcode_hidden").val(selectedvalue);
                $("#displayExpr_unitcode").val(selectedobj[0].value);
                empr_ItemMaster.bindDxGridBoxDdl('#IUNIT_CODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#unitcode_hidden').val(key);
                        $('#displayExpr_unitcode').val(value);
                    }
                    else {
                        $('#unitcode_hidden').val('');
                        $('#displayExpr_unitcode').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitPackageDDL: function (_selectedValue) {

        $.ajax({
            url: 'ItemMaster/GetUnits',
            method: 'GET',
            success: function (data) {
                if (!data || data.length === 0) return;

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                }

                if (selectedobj.length === 0) {
                    selectedobj = [data[0]];
                    selectedvalue = data[0].key;
                } else {
                    selectedvalue = _selectedValue;
                }
                $("#package_hidden").val(selectedvalue);
                $("#displayExpr_package").val(selectedobj[0].value);
                empr_ItemMaster.bindDxGridBoxDdl('#PUNIT_CODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_package', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#package_hidden').val(key);
                        $('#displayExpr_package').val(value);
                    }
                    else {
                        $('#package_hidden').val('');
                        $('#displayExpr_package').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitItemBarcodeTypeDDL: function (selectedValue) {
        function toggleBarcode(value) {
            if (value === 1) {
                $('#BARCODE').prop('readonly', false);
                $('#BARCODE').val('');
            } else if (value === 2) {
                $('#BARCODE').prop('readonly', true);
                $('#BARCODE').val('');
            } else {
                $('#BARCODE').prop('readonly', true);
                $('#BARCODE').val('');
            }
        }
        var selectedValue = 2;
        var dataSource = [
            { key: 1, value: 'Manual' },
            { key: 2, value: 'Auto' }
        ];

        $('#BITYPE').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue,
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search',
            //showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onInitialized: function (e) {
                toggleBarcode(e.component.option("value"));
            },
            onValueChanged: function (e) {
                toggleBarcode(e.value);
            }

        });
    },
    InitBarcodeTypeDDL: function (selectedValue) {

        var dataSource = [
            { key: 1, value: 'Manual' },
            { key: 2, value: 'Auto' }
        ];

        $('#BarCodeType').dxSelectBox({
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
            onValueChanged: function (e) {
                //var value = e.value;
                if ($("#detailId").val() == '') {
                    if (e.value == 1) {
                        $("#ColorSingle").empty();
                        $("#SizeSingle").empty();
                        $("#Color").empty();
                        $("#Size").empty();
                        empr_ItemMaster.InitColorSingleDDL();
                        empr_ItemMaster.InitSizeSingleDDL();
                        $("#Color").hide();
                        $("#Size").hide();
                        $("#ColorSingle").show();
                        $("#SizeSingle").show();
                        $("#barcode").prop("disabled", false);
                    }
                    else if (e.value == 2) {
                        //$("#sizesingle").addClass("hide");
                        //$("#colorsingle").addClass("hide");
                        //$("#color").removeClass("hide");
                        //$("#size").removeClass("hide");

                        $("#ColorSingle").empty();
                        $("#SizeSingle").empty();
                        $("#Color").empty();
                        $("#Size").empty();
                        empr_ItemMaster.InitColorMultipleDDL();
                        empr_ItemMaster.InitSizeMultipleDDL();
                        $("#ColorSingle").hide();
                        $("#SizeSingle").hide();
                        $("#Color").show();
                        $("#Size").show();
                        $("#barcode").val('');
                        $("#barcode").prop("disabled", true);
                    }
                    else {

                        $("#ColorSingle").empty();
                        $("#SizeSingle").empty();
                        $("#Color").empty();
                        $("#Size").empty();
                        empr_ItemMaster.InitColorSingleDDL();
                        empr_ItemMaster.InitSizeSingleDDL();
                        $("#Color").hide();
                        $("#Size").hide();
                        $("#ColorSingle").show();
                        $("#SizeSingle").show();
                        $("#barcode").prop("disabled", false);
                    }
                }
            },
            onInitialized: function (e) {
                e.component.option('value', 2);
                $("#ColorSingle").empty();
                $("#SizeSingle").empty();
                $("#Color").empty();
                $("#Size").empty();
                empr_ItemMaster.InitColorMultipleDDL();
                empr_ItemMaster.InitSizeMultipleDDL();
                $("#ColorSingle").hide();
                $("#SizeSingle").hide();
                $("#Color").show();
                $("#Size").show();
                $("#barcode").val('');
                $("#barcode").prop("disabled", true);
            }
        });

    },
    InitTaxStatusDDL: function (selectedValue) {

        var dataSource = [
            { key: 0, value: 'Exempt' },
            { key: 1, value: 'Taxable' }
        ];

        $('#ITAX_STATUS').dxSelectBox({
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
    InitColorDDL: function (selectedValue) {
        $('#COLOR').dxSelectBox({
            dataSource: Colors,
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
    InitSizeDDL: function (selectedValue) {
        $('#SIZE').dxSelectBox({
            dataSource: Sizes,
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
    InitColorSingleDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemMaster/GetColors',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#colorsingle_hidden").val(selectedvalue);
                        $("#displayExpr_colorsingle").val(selectedobj[0].value);
                    }
                }

                empr_ItemMaster.bindDxGridBoxDdl('#ColorSingle', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_colorsingle', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#colorsingle_hidden').val(key);
                        $('#displayExpr_colorsingle').val(value);
                    }
                    else {
                        $('#colorsingle_hidden').val('');
                        $('#displayExpr_colorsingle').val('');
                    }
                });

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitSizeSingleDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemMaster/GetSizes',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#sizesingle_hidden").val(selectedvalue);
                        $("#displayExpr_sizesingle").val(selectedobj[0].value);
                    }
                }

                empr_ItemMaster.bindDxGridBoxDdl('#SizeSingle', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_sizesingle', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#sizesingle_hidden').val(key);
                        $('#displayExpr_sizesingle').val(value);
                    }
                    else {
                        $('#sizesingle_hidden').val('');
                        $('#displayExpr_sizesingle').val('');
                    }
                });

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitGradesDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemMaster/GetGrades',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#grade_hidden").val(selectedvalue);
                        $("#displayExpr_grade").val(selectedobj[0].value);
                    }
                }


                empr_ItemMaster.bindDxGridBoxDdl('#Grade', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_grade', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#grade_hidden').val(key);
                        $('#displayExpr_grade').val(value);
                    }
                    else {
                        $('#grade_hidden').val('');
                        $('#displayExpr_grade').val('');
                    }
                });

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitColorMultipleDDL: function () {
        $.ajax({
            url: 'ItemMaster/GetColors',
            method: 'GET',
            success: function (data) {
                empr_ItemMaster.MultipleDxGridBoxDropdown('#Color', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'id', 'name', "#displayExpr_color", function (selectedvalue, hidden) {

                    //console.log(selectedvalue.selectedRowsData); 
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#color_hidden').val(keys);
                        $('#displayExpr_color').val(values);
                        empr_ItemMaster.isValueAssigned = false;
                    }
                    else {
                        $('#color_hidden').val('');
                        $('#displayExpr_color').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitSizeMultipleDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemMaster/GetSizes',
            method: 'GET',
            success: function (data) {
                empr_ItemMaster.MultipleDxGridBoxDropdown('#Size', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_size", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        //var key = selectedvalue.currentSelectedRowKeys[0]['key'];
                        //var value = selectedvalue.currentSelectedRowKeys[0]['value'];
                        $('#size_hidden').val(keys);
                        $('#displayExpr_size').val(values);
                        //$('#size_hidden').val($('#size_hidden').val() + ',' + key);
                        //$('#displayExpr_size').val($('#displayExpr_size').val() + ',' + value);
                        empr_ItemMaster.isValueAssigned = false;
                    }
                    else {
                        $('#size_hidden').val('');
                        $('#displayExpr_size').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitBarcodeLabelDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemMaster/GetBarcodeLabels',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#label_hidden").val(selectedvalue);
                        $("#displayExpr_label").val(selectedobj[0].value);
                    }
                }
                empr_ItemMaster.bindDxGridBoxDdl('#BarcodeLabel', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_label', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#label_hidden').val(key);
                        $('#displayExpr_label').val(value);
                    }
                    else {
                        $('#label_hidden').val('');
                        $('#displayExpr_label').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitItemGroupDDL: function (selectedValue) {

        $.ajax({
            url: 'ItemMaster/GetItemGroups',
            method: 'GET',
            data: null,
            success: function (data) {
                $('#GROUP_CODE').dxSelectBox({
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
                    searchTimeout: 500,
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitItemTypeDDL: function (selectedValue) {

        var dataSource = [
            { key: 'A', value: 'Active' },
            { key: 'N', value: 'Non Active' },
            { key: 'P', value: 'Packing Material' },
            { key: 'F', value: 'Finish Goods' },
        ];

        $('#ITEM_TYPE').dxSelectBox({
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
    InitCategoryDDL: function (selectedValue) {

        $.ajax({
            url: 'ItemMaster/GetCategories',
            method: 'GET',
            success: function (data) {
                $('#CAT_CODE').dxSelectBox({
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
                    searchTimeout: 500,
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });

    },
    InitFabricDDL: function (selectedValue) {

        $('#FABRIC').dxSelectBox({
            dataSource: Fabric,
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
    InitStyleDDL: function (selectedValue) {

        $('#STYLE').dxSelectBox({
            dataSource: Style,
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
    InitSeasonDDL: function (selectedValue) {

        $('#SEASON').dxSelectBox({
            dataSource: Season,
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
    InitBrandDDL: function (selectedValue) {
        console.log('Brand', Brand);
        $('#BRAND').dxSelectBox({
            dataSource: Brand,
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
    InitSubCategoryDDL: function (selectedValue) {

        $.ajax({
            url: 'ItemMaster/GetSubCategories',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#SUB_CAT_CODE').dxSelectBox({
                        dataSource: data.data,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });

    },
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
    MultipleDxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.MultipleDxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
    OpenDoc: function () {
        var hdnUrl = $('#ProductImageHidden').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a image to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            ShowImage(fileURL);
        }
    },
}