var empr_GatePass = {
    initEvents: function () {
        $(document).ready(function () {
            empr_GatePass.InitDropdowns();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_GatePass.GetGatePassByID(data.traN_ID);
                }
            });
            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_GatePass.validateForm()) {
                            empr_GatePass.saveAttempt();
                        }
                    }
                } else {
                    if (empr_GatePass.validateForm()) {
                        empr_GatePass.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_GatePass.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_GatePass.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_helper.selectedBill = rportid;
                empr_GatePass.GetGatePassByID(rportid);

            })

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var date = $(this).attr("reportdate");
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
                    $('#updatedDate').val(date);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModal').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/GatePass/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_GatePass.GetGatePassByID(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_GatePass.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_GatePass.DeleteRecord();
            })

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_GatePass.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/GatePass/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_GatePass.resetForm();
                    empr_GatePass.InitTree();
                    empr_GatePass.reFreshTree();
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('.btn-print').hide();
                    $('#resetall').hide();
                }
            }, false, true);
        });
    },
    
    resetForm: function () {
        $("#Code").val('')
        $("#ADD_USER_ID").val('');
        $("#MENU_ID").val();
        $('#ASTATUS').dxSelectBox('option', 'value');
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", false);
        $('#ASTATUS').dxSelectBox('instance').option('disabled', false);
        $("#V_DATE").val('');
        $("#VOUCHER_NO").val('');
        $("#PARTY").val('');
        $("#DUE_NO").val('');
        $("#VEHICLE").val('');
        $("#DRIVER").val('');
        $("#QTY").val('');
        $('#ITEM_CODE').dxSelectBox('instance').option('value', 0);
        $('#LOT').dxSelectBox('instance').option('value', 0);
        $('#PARTY_CODE').dxSelectBox('instance').option('value', 0);
        $("#unitcode_hidden").val('');
        $('#V_DATE').val(todayDate);
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        } else {
            $('#saveAttempt').show();
        }
        empr_GatePass.InitDropdowns();
    },
    
    validateForm: function () {
        var valid = true;

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value')
        var LOT = $('#LOT').dxSelectBox('option', 'value')

        var V_DATE = $("#V_DATE").val();
        var UNIT = $("#unitcode_hidden").val();
        var DUE_NO = $("#DUE_NO").val();
        var DRIVER = $("#DRIVER").val();
        var VEHICLE = $("#VEHICLE").val();
        var QTY = $("#QTY").val();

        if (ASTATUS == '' || ASTATUS == null) {
            valid = false;
            empr_helper.notify("Please select active.", 2);
        }
        if (ITEM_CODE == '' || ITEM_CODE == null) {
            valid = false;
            empr_helper.notify("Please select Item.", 2);
        }
        if (LOT == '' || LOT == null) {
            valid = false;
            empr_helper.notify("Please select Lot No.", 2);
        }
        if (V_DATE == '' || V_DATE == null) {
            valid = false;
            empr_helper.notify("Please enter date.", 2);
        }
        if (UNIT == '' || UNIT == null) {
            valid = false;
            empr_helper.notify("Please select unit.", 2);
        }
        if (DUE_NO == '' || DUE_NO == null) {
            valid = false;
            empr_helper.notify("Please enter due number.", 2);
        }
        if (DRIVER == '' || DRIVER == null) {
            valid = false;
            empr_helper.notify("Please enter driver.", 2);
        }
        if (VEHICLE == '' || VEHICLE == null) {
            valid = false;
            empr_helper.notify("Please enter vehicle.", 2);
        }
        if (QTY == '' || QTY == null) {
            valid = false;
            empr_helper.notify("Please enter Quantity.", 2);
        }

        return valid;
    },
    
    getDataToSave: function () {
        var ID = $("#ID").val();
        var ADD_USER_ID = $("#ADD_USER_ID").val()
        var ADD_DATE = $("#ADD_DATE").val()
        var ADD_COMPUTER_NAME = $("#ADD_COMPUTER_NAME").val();
        var ADD_IP_ADDRESS = $("#ADD_IP_ADDRESS").val();
        var ADD_POSTALCODE = $("#ADD_POSTALCODE").val();
        var MENU_ID = $("#MENU_ID").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var Code = empr_helper.getCode();
        var TRAN_ID = $("#Code").val().trim();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var LOT = $('#LOT').dxSelectBox('option', 'value');
        var UNIT = $("#unitcode_hidden").val();
        var DUE_NO = $("#DUE_NO").val();
        var DRIVER = $("#DRIVER").val();
        var VEHICLE = $("#VEHICLE").val();
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value');
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');
        var QTY = $("#QTY").val();

        var modelRecord = {
            ID: ID,
            Code: Code,
            ADD_USER_ID: ADD_USER_ID,
            ADD_DATE: ADD_DATE,
            ADD_COMPUTER_NAME: ADD_COMPUTER_NAME,
            ADD_IP_ADDRESS: ADD_IP_ADDRESS,
            ADD_POSTALCODE: ADD_POSTALCODE,
            MENU_ID: MENU_ID,
            ASTATUS: ASTATUS,
            TRAN_ID: TRAN_ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            LOT: LOT,
            PARTY_CODE: PARTY_CODE,
            DUE_NO: DUE_NO,
            DRIVER: DRIVER,
            VEHICLE: VEHICLE,
            ITEM_CODE: ITEM_CODE,
            QTY: QTY,
            UNIT: UNIT
        }
        return modelRecord;
    },
    
    saveAttempt: function () {
        var obj = empr_GatePass.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/GatePass/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataClear == 1) {
                    $('#Code').val(data.data);
                    empr_helper.selectedBill = data.data;
                    $('#VOUCHER_NO').val(data.data2);
                    $('#optmodal').modal('hide');
                    $('.btn-delete').show();
                    $('.btn-print').show();
                }
                else {
                    empr_GatePass.resetForm();
                }
                
            }
        }, false, true);
    },
    
    InintQuickSearch: function () {
        empr_GatePass.GetQuickSearch();
    },
    
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/GatePass/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_GatePass.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    
    GetAllGatePasss: function () {
        var xhr = ajaxHelper.ajaxGetJson('/GatePass/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {
            empr_GatePass.CreateGrid(data.data);
        }, false, true);
    },
    
    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },
    
    GetGatePassByID: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/GatePass/GatePassByid?id=' + id, function (data) {

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
                }
                if (Permissions.r_PRINT) {
                    $('.btn-print').show();
                }
                if (Permissions.r_ADD) {
                    $('#resetall').show();
                }
                if (Permissions.r_EDIT) {
                    $('#saveAttempt').show();
                }
                else {
                    $('#saveAttempt').hide();
                }
            } else {
                $('#saveAttempt').show();
                $('.btn-delete').show();
                $('.btn-print').show();
                $('#resetall').show();
            }

            $('.modal').modal('hide')
            empr_GatePass.makeReadOnly(true);
            $('#Code').val(data.data.id);
            $("#V_DATE").val(data.data.v_DATE);
            $("#VOUCHER_NO").val(data.data.voucheR_NO);
            $("#DUE_NO").val(data.data.duE_NO);
            $("#VEHICLE").val(data.data.vehicle);
            $("#DRIVER").val(data.data.driver);
            $("#QTY").val(data.data.qty);
            $('#activestatushidden').val(data.data.astatus);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            console.log(data.data)
            empr_GatePass.InitDropdownsWithValue(data.data.lot, data.data.iteM_CODE, data.data.unit, data.data.partY_CODE);
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
                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);
                if (Permissions != "Admin" && !Permissions.r_PRINT) {
                    $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                                </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                                <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                                </div>`).appendTo(container);
                }


            }
        },

        { dataField: 'traN_ID', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'v_DATE', caption: 'Voucher Date' },
        { dataField: 'voucheR_NO', caption: 'Voucher Number' },
        { dataField: 'loT_NO', caption: 'Lot' },
        { dataField: 'partY_CODE', caption: 'Party' },
        { dataField: 'duE_NO', caption: 'Due No.' },
        { dataField: 'driver', caption: 'Driver' },
        { dataField: 'vehicle', caption: 'Vehicle' },
        { dataField: 'iteM_CODE', caption: 'Item Name' },
        { dataField: 'qty', caption: 'Qty' },
        { dataField: 'unit', caption: 'Unit' },
        { dataField: 'astatus', caption: 'Status' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "GatePasssQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    
    InitDropdowns: function () {
        debugger
        empr_GatePass.InitReportTypeDDL();
        empr_GatePass.InitUnitDDL();
        ajaxHelper.ajaxGetJson("/GatePass/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_GatePass.InitItemCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/GatePass/GetLots", function (data) {
            console.log(data)
            if (data.msgType == 1) {
                empr_GatePass.InitLotDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/GatePass/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_GatePass.InitPartyCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    
    InitDropdownsWithValue: function (lot, iCode, unit, pCode) {
        debugger
        empr_GatePass.InitUnitDDL(unit);
        empr_GatePass.InitReportTypeDDL();
        ajaxHelper.ajaxGetJson("/GatePass/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_GatePass.InitItemCodeDDL(data.data, iCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/GatePass/GetLots", function (data) {
            if (data.msgType == 1) {
                empr_GatePass.InitLotDDL(data.data, lot);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/GatePass/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_GatePass.InitPartyCodeDDL(data.data, pCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    
    InitItemCodeDDL: function (dataSource, selectedValue) {
        $('#ITEM_CODE').dxSelectBox({
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
            searchTimeout: 500
        });
    },
    
    InitLotDDL: function (dataSource, selectedValue) {
        $('#LOT').dxSelectBox({
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
                if (e.value != '' && e.value != null) {
                    var items = e.component._dataSource._items;
                    var item = items.filter(i => i.key == e.value);
                    if (item && item.length > 0) {
                        $('#PARTY_CODE').dxSelectBox('instance').option('value', item[0].customizedKey);
                    }
                }
            },
        });
    },
    
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
    
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/GatePass/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    selectedValue = data.data[0].mD_ID;
                }
                $('#ReportType').dxSelectBox({
                    dataSource: data.data,
                    displayExpr: 'mD_NAME',
                    valueExpr: 'mD_ID',
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
                    },
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    
    GeneratePrintReport: function () {
        empr_GatePass.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the delivery in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/GatePass/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    
    InitUnitDDL: function (_selectedValue) {
        $.ajax({
            url: 'GatePass/GetUnits',
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

                empr_GatePass.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
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

    InitPartyCodeDDL: function (dataSource, selectedValue) {
        $('#PARTY_CODE').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'customizedKey',
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
    },
}