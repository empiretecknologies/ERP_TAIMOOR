var empr_ItemOpening = {
    InitEvents: function () {
        $(document).ready(function () {

            window.exampleModalInstance = new bootstrap.Modal(document.getElementById('EditModal'), {
                backdrop: 'static',
                keyboard: false
            });

            empr_ItemOpening.InitGrid();
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                var name = $(this).attr("name");
                console.log(name);
                $("#ITEM_CODE").val(id);
                $("#myExtraLargeModalLabel").text('Item Name : ' + name);
                empr_ItemOpening.ResetForm();
                empr_ItemOpening.GetItemOpeningDetailByCode(id);
                $('#EditModal').modal('show');
            });

            $('body').on('click', '#BtnSave', function () {    
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ItemOpening.ValidateMainInfo()) {
                            empr_ItemOpening.Save();
                        }
                    }
                } else {
                    if (empr_ItemOpening.ValidateMainInfo()) {
                        empr_ItemOpening.Save();
                    }
                }
            });

            $('body').on('click', '.elm_edit_detail', function () {
                var id = $(this).attr("reportid");
                empr_ItemOpening.ResetForm();
                empr_ItemOpening.GetItemOpeningByCode(id);
            });

            $('#QTY, #QTY2').keyup(function () {
                empr_ItemOpening.SetBalanceQuantity();
            });

            $('#CHK').on('click', function () {
                let balancedQTY = 0;
                if ($(this).is(':checked')) {
                    let QTY = parseFloat($('#QTY').val()) || 0;
                    let QTY2 = parseFloat($('#QTY2').val()) || 1;
                    balancedQTY = QTY * QTY2;
                } else {
                    let QTY = parseFloat($('#QTY').val()) || 0;
                    balancedQTY = QTY;
                }
                if (balancedQTY != 0) {
                    $('#BAL_QTY').val(balancedQTY);
                }
                else {
                    $('#BAL_QTY').val('');
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_ItemOpening.Delete();
            });

            $('body').on('click', '.elm_edit_detail_barcode', function () {
                var id = $(this).attr("reportid");
                $('#ITEM_OP_ID').val(id);
                empr_ItemOpening.ShowBarcodePopup();
            });

            $('body').on('click', '.elm_edit_barcode', function () {
                var id = $(this).attr("reportid");
                empr_ItemOpening.ResetBarCodeForm();
                empr_ItemOpening.GetBarcodeByCode(id);
            });

            $('body').on('click', '#BtnBarcodeSave', function () {
                if (empr_ItemOpening.ValidateBarcodeInfo()) {
                    empr_ItemOpening.SaveBarcode();
                }
            });

            $('body').on('click', '#BtnBarcodeDelete', function () {
                empr_ItemOpening.DeleteBarcode();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
            }
        });
    },
    ShowBarcodePopup: function () {
        $.ajax({
            url: 'ItemOpening/GetBarcodes',
            method: 'GET',
            data: { itemCode: $('#ITEM_CODE').val() },
            success: function (data) {
                if (data.length > 0) {
                    empr_ItemOpening.ResetBarCodeForm();
                    empr_ItemOpening.GetBarCodeOpeningByCode($('#ITEM_OP_ID').val());
                    $('#BarcodeModal').modal('show');
                    if (Permissions != "Admin") {
                        (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnBarcodeSave').hide();
                    }
                }
                else {
                    empr_helper.notify("Please create barcode first", 2);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    SetBalanceQuantity: function () {
        let QTY = parseFloat($('#QTY').val()) || 0;
        let QTY2 = parseFloat($('#QTY2').val()) || 1;
        let balancedQTY = 0;
        debugger
        var isChecked = $('#CHK').prop('checked');
        if (isChecked) {
            balancedQTY = QTY * QTY2;
        }
        else {
            balancedQTY = QTY;
        }
        if (balancedQTY != 0) {
            $('#BAL_QTY').val(balancedQTY);
        }
        else {
            $('#BAL_QTY').val('');
        }
    },
    InitGrid: function () {
        empr_ItemOpening.GetItemOpenings();
    },
    GetItemOpenings: function () {
        ajaxHelper.ajaxGetJson('/ItemOpening/GetItemOpenings', function (data) {
            if (data.msgType == 1) {
                empr_ItemOpening.CreateGrid(data.data);
            }
            else {
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

                    $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} name="${options.data.iteM_NAME}" title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);
                }
            },
		    { dataField: 'iteM_NAME', caption: 'Name' },
		    { dataField: 'iteM_SHORT_NAME', caption: 'Short Name' },
		    { dataField: 'iteM_ID', caption: 'Item Id' },
		    { dataField: 'grouP_NAME', caption: 'Group Name' },
		    { dataField: 'qty', caption: 'Quantity' },
		    { dataField: 'qtY2', caption: 'Quantity 2' },
		    { dataField: 'baL_QTY', caption: 'Balance Quantity' },
            { dataField: "astatus", caption: "Active", width: 100, alignment: "center" },
		    { dataField: 'unit', caption: 'Unit' },
		    { dataField: 'uniT_PACKING', caption: 'Packing Unit' },
            { dataField: 'adD_USER_ID', caption: 'Add USER', visible: false },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Add COMPUTER NAME', visible: false },
            { dataField: 'adD_DATE', caption: 'Add DATE', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'adD_IP_ADDRESS', caption: 'Add IP ADDRESS', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Add POSTALCODE', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Edit USER', visible: false },
            { dataField: 'ediT_DATE', caption: 'Edit DATE', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Edit COMPUTER NAME', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Edit IP ADDRESS', visible: false },
            { dataField: 'ediT_POSTALCODE', caption: 'Edit POSTALCODE', visible: false },                  
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ItemOpening");
    },
    ResetForm: function () { 

        $('.open input').not('#ITEM_CODE, #ASTATUS, .dx-texteditor-input').val('');
        $('#BtnDelete').hide();
        $('#BILL_DATE').val(periodStartDate);

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        }
        else {
            $('#saveAttempt').show();
        }

        empr_ItemOpening.InitUnitDDL();
        empr_ItemOpening.InitPackingUnitDDL();
        empr_ItemOpening.InitColorDDL();
        empr_ItemOpening.InitSizeDDL();
        empr_ItemOpening.InitGradeDDL();
        empr_ItemOpening.InitWarehouseDDL();
    },
    ResetBarCodeForm: function () {

        $('.barcode input').not('#ITEM_OP_ID, #BASTATUS, .dx-texteditor-input').val('');
        $('#BtnBarcodeDelete').hide();
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnBarcodeSave').show();
            } else {
                $('#BtnBarcodeSave').hide();
            }
        } else {
            $('#BtnBarcodeSave').show();
        }
        empr_ItemOpening.InitBarcodeStatusDDL();
        empr_ItemOpening.InitBarcodeUnitDDL();
        empr_ItemOpening.InitBarcodeDDL();
    },
    GetItemOpeningDetailByCode: function (id) {
        ajaxHelper.ajaxGetJson('/ItemOpening/GetItemOpeningDetailByCode?code=' + id, function (data) {
            if (data.msgType == 1) {
                empr_ItemOpening.CreateDetailGrid(data.data);
                if (Permissions != "Admin") {
                    (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetBarCodeOpeningByCode: function (id) {

        ajaxHelper.ajaxGetJson('/ItemOpening/GetBarcodeDetailByCode?code=' + id, function (data) {
            if (data.msgType == 1) {
                empr_ItemOpening.CreateBarcodeGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_ItemOpening.GetDataToSave();

        if (data.RATE == '') {
            empr_helper.notify("Rate is required.", 2);
            valid = false;
            return valid;
        }

        if (data.QTY == '') {
            empr_helper.notify("Quantity is required.", 2);
            valid = false;
            return valid;
        }

        if (data.UNIT == '') {
            empr_helper.notify("Unit is required.", 2);
            valid = false;
            return valid;
        }

        if (data.PACK_UNIT == '') {
            empr_helper.notify("Packing unit is required.", 2);
            valid = false;
            return valid;
        }

        if (data.EXP_DATE != null && data.MFG_DATE != null) {
            var mfgDateObj = new Date(data.MFG_DATE);
            var expDateObj = new Date(data.EXP_DATE);

            if (expDateObj <= mfgDateObj) {
                empr_helper.notify("please select valid dates.", 2);
                valid = false;
                return valid;
            }
        }

        return valid;
    },
    ValidateBarcodeInfo: function () {

        var valid = true;
        var data = empr_ItemOpening.GetBarcodeDataToSave();

        if (data.BARCODE == '') {
            empr_helper.notify("barcode is required.", 2);
            valid = false;
            return valid;
        }

        if (data.UNIT == '') {
            empr_helper.notify("Unit is required.", 2);
            valid = false;
            return valid;
        }

        if (data.QTY == '') {
            empr_helper.notify("Quantity is required.", 2);
            valid = false;
            return valid;
        }

        if (data.RATE == '') {
            empr_helper.notify("Rate is required.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var CHK = $('#CHK').prop('checked');
        if (CHK) {
            CHK = 1;
        }
        else {
            CHK = 0;
        }
        
        var ITEM_CODE = $("#ITEM_CODE").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');        
        var RATE = $("#RATE").val();
        var REF = $("#REF").val();
        var BATCH = $("#BATCH").val();
        var LOT = $("#LOT").val();
        var MFG_DATE = $("#MFG_DATE").val();
        var EXP_DATE = $("#EXP_DATE").val();
        var BILL_DATE = $("#BILL_DATE").val();
        var QTY = $("#QTY").val();
        var IUNIT_CODE = $("#unitcode_hidden").val();
        var QTY2 = $("#QTY2").val();
        var BAL_QTY = $("#BAL_QTY").val();
        var PUNIT_CODE = $("#package_hidden").val();
        var COLOR = $("#colorsingle_hidden").val();
        var SIZE = $("#sizesingle_hidden").val();
        var GRADE = $("#grade_hidden").val();
        var WAREHOUSE = $("#warehouse_hidden").val();

        var modelRecord = {
            OP_ID: CODE,
            ITEM_CODE: ITEM_CODE,
            RATE: RATE,
            REF: REF,
            BATCH: BATCH,
            LOT: LOT,
            MFG_DATE: MFG_DATE,
            EXP_DATE: EXP_DATE,
            BILL_DATE: BILL_DATE,
            QTY: QTY,
            UNIT: IUNIT_CODE,
            QTY2: QTY2,
            BAL_QTY: BAL_QTY,
            PACK_UNIT: PUNIT_CODE,
            COLOR: COLOR,
            SIZE: SIZE,
            GRADE: GRADE,
            ASTATUS: ASTATUS,
            CHK: CHK,
            WAREHOUSE: WAREHOUSE,
        };
        return modelRecord;
    },
    Save: function () {
        debugger;
        var dataModel = empr_ItemOpening.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/ItemOpening/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_ItemOpening.ResetForm();
                empr_ItemOpening.GetItemOpeningDetailByCode(dataModel.ITEM_CODE);
            }
        }, false, true);
    },
    Delete: function () {

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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/ItemOpening/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ItemOpening.ResetForm();
                    empr_ItemOpening.GetItemOpeningDetailByCode($('#ITEM_CODE').val());
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    GetBarcodeDataToSave: function () {

        var ID = $("#BCode").val();
        var ITEM_OP_ID = $("#ITEM_OP_ID").val();
        var RATE = $("#BRATE").val();
        var QTY = $("#BQTY").val();
        var UNIT = $("#barcodeunit_hidden").val();
        var BARCODE = $("#barcode_hidden").val();
        var BASTATUS = $('#BASTATUS').dxSelectBox('option', 'value');
        var modelRecord = {
            OP_ID: ID,
            ITEM_OP_ID: ITEM_OP_ID,
            RATE: RATE,
            QTY: QTY,
            BARCODE: BARCODE,
            UNIT: UNIT,
            ASTATUS: BASTATUS,
        }

        return modelRecord;
    },
    SaveBarcode: function () {
        debugger;
        var dataModel = empr_ItemOpening.GetBarcodeDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/ItemOpening/SaveBarcode", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_ItemOpening.ResetBarCodeForm();
                empr_ItemOpening.GetBarCodeOpeningByCode($('#ITEM_OP_ID').val());
            }
        }, false, true);
    },
    DeleteBarcode: function () {

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
            ajaxHelper.ajaxPostJsonData({ code: $('#BCode').val() }, "/ItemOpening/DeleteBarcode", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ItemOpening.ResetBarCodeForm();
                    empr_ItemOpening.GetBarCodeOpeningByCode($('#ITEM_OP_ID').val());
                    $('#BtnBarcodeDelete').hide();
                }
            }, false, true);
        });
    },
    CreateDetailGrid: function (dataSrc) {
        console.log(dataSrc);
        var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger

                    if (B_I == 'B') {
                        $(`<div class="btn-group btn-group-sm">
                                        <a href="javascript:;"  class="grid-action-icon elm_edit_detail" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                                        <a href="javascript:;"  class="grid-action-icon elm_edit_detail_barcode" reportid=${options.data.id} title="Edit Barcode" style="padding-left: 5px;"><i class="fa fa-barcode"></i></a>
                                        </div>`).appendTo(container);
                    }else {
                        $(`<div class="btn-group btn-group-sm">
                                        <a href="javascript:;"  class="grid-action-icon elm_edit_detail" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                                        </div>`).appendTo(container);
                    }
                }
            },
            { dataField: "astatus", caption: "Active", width: 100, alignment: "center" },
            { dataField: 'iteM_NAME', caption: 'Name' },
            { dataField: 'iteM_GROUP', caption: 'Item Group' },
            { dataField: 'rate', caption: 'Rate' },
            { dataField: 'ref', caption: 'Reference No' },
            { dataField: 'batch', caption: 'Batch No' },
            { dataField: 'lot', caption: 'Lot No', },
            { dataField: 'warehouse', caption: 'Warehouse', },
            { dataField: 'mfG_DATE', caption: 'Manufacture Date', dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'exP_DATE', caption: 'Expire Date', dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'qty', caption: 'Quantity' },
            { dataField: 'unit', caption: 'Unit' },
            { dataField: 'qtY2', caption: 'Quantity 2' },
            { dataField: 'baL_QTY', caption: 'Balance Quantity' },
            { dataField: 'coloR_NAME', caption: 'Color', },
            { dataField: 'sizE_NAME', caption: 'Size', },
            { dataField: 'gradE_NAME', caption: 'Grade', },            
            { dataField: 'pacK_UNIT', caption: 'Packing Unit' },
            { dataField: 'adD_USER_ID', caption: 'Add USER', visible: false },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Add COMPUTER NAME', visible: false },
            { dataField: 'adD_DATE', caption: 'Add DATE', visible: false, dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'adD_IP_ADDRESS', caption: 'Add IP ADDRESS', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Add POSTALCODE', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Edit USER', visible: false },
            { dataField: 'ediT_DATE', caption: 'Edit DATE', visible: false, dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Edit COMPUTER NAME', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Edit IP ADDRESS', visible: false },
            { dataField: 'ediT_POSTALCODE', caption: 'Edit POSTALCODE', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#detailContainer', col, dataSrc, "ItemOpeningDetail");
    },
    CreateBarcodeGrid: function (dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger

                $(`<div class="btn-group btn-group-sm">
                    <a href="javascript:;"  class="grid-action-icon elm_edit_barcode" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                    </div>`).appendTo(container);
            }
        },
        { dataField: "astatus", caption: "Active", width: 100, alignment: "center" },
        { dataField: 'iteM_NAME', caption: 'Name' },
        { dataField: 'barcode', caption: 'BarCode' },
        { dataField: 'qty', caption: 'Quantity' },
        { dataField: 'rate', caption: 'Rate' },
        { dataField: 'unit', caption: 'Unit' },
        { dataField: 'coloR_NAME', caption: 'Color', },
        { dataField: 'sizE_NAME', caption: 'Size', },
        { dataField: 'gradE_NAME', caption: 'Grade', },

        //    { dataField: 'adD_USER_ID', caption: 'Add USER', visible: false },
        //{ dataField: 'adD_COMPUTER_NAME', caption: 'Add COMPUTER NAME', visible: false },
        //{ dataField: 'adD_DATE', caption: 'Add DATE', visible: false },
        //{ dataField: 'adD_IP_ADDRESS', caption: 'Add IP ADDRESS', visible: false },
        //{ dataField: 'adD_POSTALCODE', caption: 'Add POSTALCODE', visible: false },
        //{ dataField: 'ediT_USER_ID', caption: 'Edit USER', visible: false },
        //{ dataField: 'ediT_DATE', caption: 'Edit DATE', visible: false },
        //{ dataField: 'ediT_COMPUTER_NAME', caption: 'Edit COMPUTER NAME', visible: false },
        //{ dataField: 'ediT_IP_ADDRESS', caption: 'Edit IP ADDRESS', visible: false },
        //    { dataField: 'ediT_POSTALCODE', caption: 'Edit POSTALCODE', visible: false },

        ];
        empr_helper.dxGridbindingVouchers('#BarCodeDetailContainer', col, dataSrc, "ItemOpeningBarcodeDetail");
    },
    GetItemOpeningByCode: function (id) {

        ajaxHelper.ajaxGetJson('/ItemOpening/GetItemOpeningByCode?code=' + id, function (data) {
            if (data.msgType == 1) {
                var response = data.data;
                $("#Code").val(response.id);
                $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                if (response.baL_QTY != 0) {
                    $("#BAL_QTY").val(response.baL_QTY);
                }
                $("#BATCH").val(response.batch);
                $("#ITEM_CODE").val(response.iteM_CODE);
                $("#LOT").val(response.lot);
                $("#MFG_DATE").val(response.mfG_DATE);
                $("#EXP_DATE").val(response.exP_DATE);
                $("#BILL_DATE").val(response.bilL_DATE);
                if (response.qtY2 != 0) {
                    $("#QTY2").val(response.qtY2);
                }
                $("#QTY").val(response.qty);
                $("#RATE").val(response.rate);
                $("#REF").val(response.ref);

                if (response.chk == 1) {
                    $('#CHK').prop('checked', true);
                } else {
                    $('#CHK').prop('checked', false);
                }

                empr_ItemOpening.InitUnitDDL(response.unit);
                empr_ItemOpening.InitPackingUnitDDL(response.pacK_UNIT);
                empr_ItemOpening.InitColorDDL(response.color);
                empr_ItemOpening.InitSizeDDL(response.size);
                empr_ItemOpening.InitGradeDDL(response.grade);
                empr_ItemOpening.InitWarehouseDDL(response.warehouse);
                //$('#BtnDelete').show();
                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
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
                }
                empr_ItemOpening.SetBalanceQuantity();
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetBarcodeByCode: function (id) {
        ajaxHelper.ajaxGetJson('/ItemOpening/GetBarcodeByCode?code=' + id, function (data) {
                if (data.msgType == 1) {
                    var response = data.data;
                    empr_ItemOpening.InitBarcodeDDL(response.barcode);
                    empr_ItemOpening.InitBarcodeUnitDDL(response.unit);
                    $("#BCode").val(response.id);
                    $('#BASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $("#BQTY").val(response.qty);
                    $("#BRATE").val(response.rate);
                    //$('#BtnBarcodeDelete').show();
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnBarcodeDelete').show();
                        }
                        if (Permissions.r_EDIT) {
                            $('#BtnBarcodeSave').show();
                        }
                        else {
                            $('#BtnBarcodeSave').hide();
                        }
                    } else {
                        $('#BtnBarcodeSave').show();
                        $('#BtnBarcodeDelete').show();
                    }
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
            }, false, true);
    },
    InitUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'ItemOpening/GetUnits',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#unitcode_hidden").val(selectedvalue);
                        $("#displayExpr_unitcode").val(selectedobj[0].value);
                    }
                }

                empr_ItemOpening.bindDxGridBoxDdl('#IUNIT_CODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
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
    InitPackingUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'ItemOpening/GetUnits',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#package_hidden").val(selectedvalue);
                        $("#displayExpr_package").val(selectedobj[0].value);
                    }
                }

                empr_ItemOpening.bindDxGridBoxDdl('#PUNIT_CODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_package', function (selectedvalue, hidden) {

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
    InitColorDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemOpening/GetColors',
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

                empr_ItemOpening.bindDxGridBoxDdl('#ColorSingle', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_colorsingle', function (selectedvalue, hidden) {

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
    InitBarcodeUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'ItemOpening/GetUnits',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#barcodeunit_hidden").val(selectedvalue);
                        $("#displayExpr_barcodeunit").val(selectedobj[0].value);
                    }
                }

                empr_ItemOpening.bindDxGridBoxDdl('#BUNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_barcodeunit', function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#barcodeunit_hidden').val(key);
                        $('#displayExpr_barcodeunit').val(value);
                    }
                    else {
                        $('#barcodeunit_hidden').val('');
                        $('#displayExpr_barcodeunit').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });


    },
    InitBarcodeDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemOpening/GetBarcodes',
            method: 'GET',
            data: { itemCode: $('#ITEM_CODE').val() },
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#barcode_hidden").val(selectedvalue);
                        $("#displayExpr_barcode").val(selectedobj[0].value);
                    }
                }

                $('#BARCODE').dxSelectBox({
                    dataSource: data,
                    displayExpr: 'value',
                    valueExpr: 'key',
                    value: _selectedValue,
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
                        if (e.value != '' && e.value != null) {
                            var key = e.value;
                            var ids = e.component._dataSource._items;
                            var value = ids.filter(i => i.key == e.value)[0].value;
                            $('#barcode_hidden').val(key);
                            $('#displayExpr_barcode').val(value);
                            
                        }
                        else {
                            $('#barcode_hidden').val('');
                            $('#displayExpr_barcode').val('');
                        }
                    },
                });

                //ati_dxHelper.DxGridBoxDropdownWithSearch('#BARCODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_barcode', function (selectedvalue, hidden) {
                //    if (selectedvalue.selectedRowsData.length > 0) {
                //        var key = selectedvalue.selectedRowsData[0]['key'];
                //        var value = selectedvalue.selectedRowsData[0]['value'];
                //        $('#barcode_hidden').val(key);
                //        $('#displayExpr_barcode').val(value);
                //    }
                //    else {
                //        $('#barcode_hidden').val('');
                //        $('#displayExpr_barcode').val('');
                //    }
                //});

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitBarcodeStatusDDL: function () {

        $('#BASTATUS').dxSelectBox({
            dataSource: [
                { value: 'Y', text: 'Active' },
                { value: 'N', text: 'In-Active' }
            ],
            valueExpr: 'value',
            displayExpr: 'text',
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onInitialized: function (e) {
                e.component.option('value', 'Y');
            }
        });
    },
    InitSizeDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemOpening/GetSizes',
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

                empr_ItemOpening.bindDxGridBoxDdl('#SizeSingle', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_sizesingle', function (selectedvalue, hidden) {

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
    InitGradeDDL: function (_selectedValue) {
        $.ajax({
            url: 'ItemOpening/GetGrades',
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


                empr_ItemOpening.bindDxGridBoxDdl('#Grade', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_grade', function (selectedvalue, hidden) {

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
    //InitWarehouseDDL: function (_selectedValue) {
    //    $.ajax({
    //        url: 'ItemOpening/GetWarehouses',
    //        method: 'GET',
    //        success: function (data) {

    //            var selectedvalue = 0;
    //            var selectedobj = [];
    //            if (_selectedValue != null) {
    //                selectedobj = data.filter(x => x.key == _selectedValue);
    //                if (selectedobj.length > 0) {
    //                    selectedvalue = _selectedValue;
    //                    $("#warehouse_hidden").val(selectedvalue);
    //                    $("#displayExpr_warehouse").val(selectedobj[0].value);
    //                }
    //            }


    //            empr_ItemOpening.bindDxGridBoxDdl('#WAREHOUSE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_warehouse', function (selectedvalue, hidden) {

    //                if (selectedvalue.selectedRowsData.length > 0) {
    //                    var key = selectedvalue.selectedRowsData[0]['key'];
    //                    var value = selectedvalue.selectedRowsData[0]['value'];
    //                    $('#warehouse_hidden').val(key);
    //                    $('#displayExpr_warehouse').val(value);
    //                }
    //                else {
    //                    $('#warehouse_hidden').val('');
    //                    $('#displayExpr_warehouse').val('');
    //                }
    //            });

    //        },
    //        error: function (error) {
    //            console.error('Error fetching data:', error);
    //        }
    //    });
    //},
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
    InitWarehouseDDL: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "ItemOpening/GetWarehouses",
            type: "GET",
            success: function (response) {
                console.log(response)
                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = response.filter(x => { return x.key == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].key;
                        $('#warehouse_hidden').val(selectedObject[0].key);
                        $('#displayExpr_warehouse').val(selectedObject[0].value);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#WAREHOUSE").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "key",
                    displayExpr: function (item) {
                        return item ? `${item.value}` : "Select a value...";
                    },
                    dataSource: response,
                    placeholder: 'Select a value...',
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.key === e.value);
                            if (selectedData) {
                                $('#warehouse_hidden').val(selectedData.key);
                                $('#displayExpr_warehouse').val(selectedData.value);
                            }
                        } else {
                            $('#warehouse_hidden').val('');
                            $('#displayExpr_warehouse').val('');
                        }
                    },
                    //onFocusIn: function (e) {
                    //    if (!isProgrammaticOpen) {
                    //        isProgrammaticOpen = true;
                    //        e.component.open();
                    //        setTimeout(() => { isProgrammaticOpen = false; }, 100);
                    //    }
                    //},
                    onOpened: function (e) {
                        if (!currentSearchTerm) {
                            if (gridInstance) {
                                gridInstance.getDataSource().filter(null);
                                gridInstance.refresh();
                            }
                        }

                        setTimeout(() => {
                            const input = e.component._$element.find(".dx-texteditor-input").first();
                            input.focus();
                            if (input.val()) {
                                input.select();
                            }
                        }, 50);
                    },
                    onInput: function (e) {
                        currentSearchTerm = e.event.target.value;

                        if (!e.component.option("opened")) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 100);
                        }

                        if (gridInstance) {
                            applyGridFilter(gridInstance, currentSearchTerm);
                        }
                    },
                    contentTemplate: function (e) {
                        gridInstance = $("<div>").dxDataGrid({
                            dataSource: new DevExpress.data.DataSource({
                                store: response,
                                key: "key"
                            }),
                            columns: [
                                //{
                                //    dataField: "key",
                                //    caption: "Code",
                                //    width: '60px',
                                //    cellTemplate: function (container, options) {
                                //        highlightText(container, options.value);
                                //    }
                                //},
                                {
                                    dataField: "value",
                                    caption: "As Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "name",
                                    caption: "Control Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                }
                            ],
                            selection: {
                                mode: "single",
                                showCheckBoxesMode: "always"
                            },
                            hoverStateEnabled: true,
                            height: 150,
                            keyboardNavigation: {
                                enabled: true,
                                enterKeyAction: "select",
                                editOnKeyPress: true
                            },
                            onSelectionChanged: function (selectedItems) {
                                const selected = selectedItems.selectedRowsData[0];
                                if (selected) {
                                    e.component.option("value", selected.key);
                                    e.component.close();

                                    // Update hidden and display fields
                                    $('#warehouse_hidden').val(selected.key);
                                    $('#displayExpr_warehouse').val(selected.value);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].key], false);
                                    }
                                }
                            }
                        }).dxDataGrid("instance");

                        gridInstance.element().on('click', function (event) {
                            event.stopPropagation();
                        });

                        return gridInstance.element();
                    }
                });

                function applyGridFilter(grid, searchTerm) {
                    const dataSource = grid.getDataSource();
                    if (searchTerm) {
                        dataSource.filter([
                            ["value", "contains", searchTerm],
                            "or",
                            ["key", "contains", searchTerm]
                        ]);
                    } else {
                        dataSource.filter(null);
                    }
                    dataSource.load();
                }

                function highlightText(container, value) {
                    if (!value) return;

                    const text = value.toString();
                    if (!currentSearchTerm || !text.toLowerCase().includes(currentSearchTerm.toLowerCase())) {
                        container.text(text);
                        return;
                    }

                    const regex = new RegExp(currentSearchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), "gi");
                    const highlighted = text.replace(regex, match =>
                        `<span style="background-color: #ffeb3b; font-weight: bold;">${match}</span>`
                    );
                    container.html(highlighted);
                }

                // Handle clear button
                $(document).on("dxclick", "#WAREHOUSE .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#warehouse_hidden').val('');
                    $('#displayExpr_warehouse').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
            }
        });
    },
}