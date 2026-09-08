var empr_StockTransferRequisition = {
    totalCount: 0,
    rowsCount: 0,
    BLABEL: '',
    IsBLabelValid: true,
    SelectedGroup: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_StockTransferRequisition.InitQuickSearchGrid();
            empr_StockTransferRequisition.ResetForm();
            empr_StockTransferRequisition.InitBranchTo();
            empr_StockTransferRequisition.InitBranchFrom();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_StockTransferRequisition.GetStockTransferRequisitionByCode(data.traN_ID);
                }
            });
            $('.chkCell').prop('checked', true);
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                empr_helper.selectedBill = id;
                $('.modal').modal('hide');
                empr_StockTransferRequisition.GetStockTransferRequisitionByCode(id);
            });

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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/StockTransferRequisition/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_StockTransferRequisition.GetStockTransferRequisitionByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_StockTransferRequisition.ValidateMainInfo()) {
                            empr_StockTransferRequisition.Save();
                        }
                    }
                } else {
                    if (empr_StockTransferRequisition.ValidateMainInfo()) {
                        empr_StockTransferRequisition.Save();
                    }
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_StockTransferRequisition.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_StockTransferRequisition.ResetForm();
                //$('#REF').focus();
                $('#ButtonsDiv').hide();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_StockTransferRequisition.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnGenerateCartonSticker', function () {
                empr_StockTransferRequisition.InitCartonStickerGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_StockTransferRequisition.GeneratePrintReport();
            });

            $('body').on('click', '#BtnGenerateSticker', function () {
                empr_StockTransferRequisition.GenerteCartonSticker();
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_StockTransferRequisition.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    CreateGrid: function (dataSrc) {
        empr_StockTransferRequisition.SelectedGroup = dataSrc[0].iteM_GROUP;
        if (dataSrc.length > 0) {
            empr_StockTransferRequisition.rowsCount = dataSrc.length - 1;
        }
        var col = [
            {
                dataField: "Action",
                width: 120,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_StockTransferRequisition.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_StockTransferRequisition.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_StockTransferRequisition.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_StockTransferRequisition.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_StockTransferRequisition.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_StockTransferRequisition.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'iteM_GROUP',
                caption: 'Group',
                width: 150,
                allowSorting: false,
                lookup: {
                    dataSource: Groups,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {
                    if (value != '') {
                        newData.iteM_GROUP = value;
                        empr_StockTransferRequisition.SelectedGroup = value;
                    }
                }
            },
            {
                dataField: 'iteM_CODE',
                caption: 'Item',
                width: 150,
                allowSorting: false,
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                onValueChanged: function (e) {
                    var selectedItemKey = e.value;
                    //e.rowElement.css({ "color": "white", "background-color": "red" });
                    $.ajax({
                        url: 'StockTransferRequisition/GetItemId',
                        method: 'GET',
                        data: { Id: selectedItemKey },
                        success: function (response) {
                            console.log(response);
                        },
                    });
                },
                setCellValue: function (newData, value, currentRowData) {
                    if (value != '') {
                        newData.iteM_CODE = value;
                        var selectedItem = Items.filter(u => u.key == value);
                        if (selectedItem.length > 0) {
                            if (empr_StockTransferRequisition.BLABEL == '') {
                                empr_StockTransferRequisition.BLABEL = selectedItem[0].blabel;
                            }
                            newData.unit = selectedItem[0].unit;
                            newData.blabel = selectedItem[0].blabel;
                            newData.iteM_ID = selectedItem[0].itemID;
                        }
                    }
                }
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty + qtY2;
                        }
                        else {
                            newData.baL_QTY = qty;
                        }
                    }

                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'qtY2',
                caption: 'Quantity 2',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qtY2 = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty + qtY2;
                        }
                        else {
                            newData.baL_QTY = qty;
                        }
                    }
                }
            },
            {
                dataField: 'baL_QTY',
                caption: 'Balance Quantity',
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var $cell = $("<div>").addClass("custom-cell");
                    var isChecked = options.data.chK1;
                    var quantity = options.data.baL_QTY;
                    if (quantity == 0 || quantity == '' || quantity == null || quantity == undefined) {
                        quantity = '';
                    }
                    console.log("isChecked:", isChecked);
                    var $checkbox = $("<input type='checkbox'>")
                        .addClass("chkCell")
                        .prop('checked', isChecked)
                        .on('change', function () {
                            var item = options.data;
                            var qty = parseFloat(item.qty) || 0;
                            var qtY2 = parseFloat(item.qtY2) || 0;
                            if (isNaN(qty)) {
                                empr_helper.notify("Please enter the correct quantity.", 2);
                            }
                            if (isNaN(qtY2)) {
                                empr_helper.notify("Please enter the correct quantity2.", 2);
                            }
                            if (!isNaN(qty) && !isNaN(qtY2)) {
                                if (this.checked) {
                                    item.baL_QTY = qty + qtY2;
                                    item.chk = "1";
                                }
                                else {
                                    item.baL_QTY = qty;
                                    item.chk = "0";
                                }
                                item.chK1 = this.checked;
                                container.find('span').text(item.baL_QTY);
                            }


                            var gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
                            var dataSource = gridInstance.option("dataSource");
                            dataSource[options.rowIndex] = item;
                            gridInstance.option("dataSource", dataSource);
                        });

                    var $span = $('<span>' + quantity + '</span>');
                    $cell.append($checkbox).append($span);
                    container.append($cell);
                }
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
            }
            ,
            {
                dataField: 'iteM_ID',
                caption: 'Item Id',
                //disabled: true
                allowEditing: false,
            }
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#StockDetailContainer', col, dataSrc, "StockRequisition", "iteM_GROUP");
        if (dataSrc.length == 0) {
            $('#StockDetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#StockDetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        //setTimeout(function () {
        //    var nextElement = $('#StockDetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#StockDetailContainer').dxDataGrid('instance').focus(nextElement);  
        //}, 1500);
    },
    CloneRow: function (index) {
        ;
        if ($('#StockDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_StockTransferRequisition.rowsCount += 1;
                const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_StockTransferRequisition.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            });
        }
        else {
            empr_StockTransferRequisition.rowsCount += 1;
            const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_StockTransferRequisition.GenerateKey(36);
                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
            }
        }
    },
    AddRow: function () {
        if ($('#StockDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_StockTransferRequisition.rowsCount += 1;
                const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");
                console.log(empr_StockTransferRequisition.SelectedGroup);
                dataSource.unshift({ __KEY__: empr_StockTransferRequisition.GenerateKey(36), qtY2: 1, iteM_GROUP: empr_StockTransferRequisition.SelectedGroup });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_StockTransferRequisition.rowsCount += 1;
            const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            console.log(empr_StockTransferRequisition.SelectedGroup);
            dataSource.unshift({ __KEY__: empr_StockTransferRequisition.GenerateKey(36), qtY2: 1, iteM_GROUP: empr_StockTransferRequisition.SelectedGroup });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    DeleteRow: function (index) {
        const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_StockTransferRequisition.rowsCount -= 1;
                    gridInstance.saveEditData();
                }
                else {
                    var availableRows = dataSource.filter(x => x.dT_CODE > 0);
                    if (availableRows.length > 1) {
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
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/StockTransferRequisition/DeleteStockTransferRequisitionDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_StockTransferRequisition.rowsCount -= 1;
                                gridInstance.saveEditData();
                            }
                        }, false, true);
                    });
                    } else {
                        empr_helper.notify("You are not allowed to delete the last row.", 2);
                    }
                }
            }
            else {
                empr_helper.notify("You are not allowed to delete the last row.", 2);
            }
        }
    },
    dropDownBoxEditorTemplate: function (cellElement, cellInfo) {
        return $('<div>').dxDropDownBox({
            dropDownOptions: { width: 500 },
            dataSource: [
                { key: 1, value: 'Value 1' },
                { key: 2, value: 'Value 2' },
                { key: 3, value: 'Value 3' }
            ],
            value: cellInfo.value,
            valueExpr: 'value',
            keyExpr: 'key',
            displayExpr: 'value',
            inputAttr: { 'aria-label': 'Owner' },
            contentTemplate(e) {
                return $('<div>').dxDataGrid({
                    dataSource: [
                        { key: 1, value: 'Value 1' },
                        { key: 2, value: 'Value 2' },
                        { key: 3, value: 'Value 3' }
                    ],
                    remoteOperations: true,
                    columns: [
                        { dataField: "key", caption: "Code" },
                        { dataField: "value", caption: "Name" },
                        { dataField: "name", caption: "Control Name" }
                    ],
                    hoverStateEnabled: true,
                    scrolling: { mode: 'virtual' },
                    height: 250,
                    selection: { mode: 'single' },
                    selectedRowKeys: [cellInfo.value],
                    keyExpr: 'key',
                    onSelectionChanged(selectionChangedArgs) {
                        e.component.option('value', selectionChangedArgs.selectedRowKeys[0]);
                        cellInfo.setValue(selectionChangedArgs.selectedRowKeys[0]);
                        if (selectionChangedArgs.selectedRowKeys.length > 0) {
                            e.component.close();
                        }
                    },
                });
            },
        });
    },
    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
    InitQuickSearchGrid: function () {
        empr_StockTransferRequisition.GetStockTransferRequisition();
    },
    GetStockTransferRequisition: function () {
        ajaxHelper.ajaxGetJson('/StockTransferRequisition/GetStockTransferRequisition', function (data) {
            if (data.msgType == 1) {
                empr_StockTransferRequisition.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                if (Permissions != "Admin" && !Permissions.r_PRINT) {
                    $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.id} title="PRINT"><i class="fa fa-print"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.id} title="COPY"><i class="fa fa-copy"></i></a>
                           </div>`).appendTo(container);
                }
            }
        },
        { dataField: 'id', caption: 'Code', },
        { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'ref', caption: 'Reference No', },
        { dataField: 'remarks', caption: 'Remarks', },
        { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
        { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
        { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
        { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
        { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
        { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "StockTransferRequisitionQS");
    },
    GetStockTransferRequisitionByCode: function (code) {
        ajaxHelper.ajaxGetJson('/StockTransferRequisition/GetStockTransferRequisitionByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    console.log(response);
                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    //$('#AM').dxSelectBox('instance').option('value', response.am);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    empr_StockTransferRequisition.InitBranchFrom(response.deP_ID);
                    $('#BCODE').dxSelectBox('instance').option('value', response.bcode);
                    empr_StockTransferRequisition.InitBranchTo(response.deP_ID);
                    $('#TBCODE').dxSelectBox('instance').option('value', response.tbcode);
                    $('#PERIOD_ID').val(response.perioD_ID);
                    $('#TPERIOD_ID').val(response.tperioD_ID);
                    //$('#BtnDelete').show();
                    //$('#ButtonsDiv').show();
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                        if (Permissions.r_PRINT) {
                            $('#ButtonsDiv').show();
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
                        $('#ButtonsDiv').show();
                    }
                }

                if (data.detail.msgType == 1) {
                    empr_StockTransferRequisition.CreateGrid(data.detail.data);
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetStockTransferRequisitionDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/StockTransferRequisition/GetStockTransferRequisitionDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_StockTransferRequisition.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetDataToSave: function () {

        var ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var BCODE = $("#BCODE").dxSelectBox('option', 'value');
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        //var AM = $('#AM').dxSelectBox('option', 'value');
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            BCODE: BCODE,
            TBCODE: TBCODE,
            REF: REF,
            REMARKS: REMARKS,
            //AM: AM,
            ASTATUS: ASTATUS
        }
        var detailRecords = [];
        if ($('#StockDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#StockDetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#StockDetailContainer').dxDataGrid('instance').option("dataSource");
        }
        if (empr_StockTransferRequisition.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#StockDetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_StockTransferRequisition.GetDataToSave();

        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }
        if (data.Master.TBCODE == '') {
            empr_helper.notify("Please select branch to.", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#StockDetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add items.", 2);
            valid = false;
            return valid;
        }

        $.each(data.Detail, function (index, item) {
            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
                empr_helper.notify("Please select item at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty ItemCode.");
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
            }

            if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
                empr_helper.notify("Please enter correct item quantity2 at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity2.");
            }

        });

        //if (!empr_StockTransferRequisition.IsBLabelValid) {
        //    empr_helper.notify("Please remove the red row.", 2);
        //    valid = false;
        //    return valid;
        //}

        return valid;
    },
    Save: function () {
        ;
        var dataModel = empr_StockTransferRequisition.GetDataToSave();
        var checkBLABEL = empr_StockTransferRequisition.FindNonMatchingIndexes(dataModel.Detail);
        if (checkBLABEL.length > 0) {
            empr_helper.notify("Please remove the red rows.", 2);
            return;
        }
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/StockTransferRequisition/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    $('.vHide').show();
                }
                empr_StockTransferRequisition.GetStockTransferRequisitionDetailByCode($('#Code').val());
                $('#BtnDelete').show();
                $('#ButtonsDiv').show();
                $('#V_DATE').focus();
            }
        }, false, true);
    },
    ResetForm: function () {
        empr_StockTransferRequisition.CreateGrid([{ __KEY__: empr_StockTransferRequisition.GenerateKey(36), qtY2: 1 }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        //$('#AM').dxSelectBox('instance').option('value', 'M');
        empr_StockTransferRequisition.InitBranchFrom();
        empr_StockTransferRequisition.InitBranchTo();
        //$('#BCODE').dxSelectBox('instance').option('value', '');
        $('#TBCODE').dxSelectBox('instance').option('value', '');
        empr_StockTransferRequisition.InitReportTypeDDL();
        $('#V_DATE').val(todayDate);
        $('#V_DATE').focus();

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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/StockTransferRequisition/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_StockTransferRequisition.ResetForm();
                    $('#BtnDelete').hide();
                    $('#ButtonsDiv').hide();
                }
            }, false, true);
        });
    },
    GenerateKey: function (keyLength) {

        var key = "";
        var characters = "abcdef0123456789";
        for (var i = 0; i < keyLength; i++) {
            if (i === 8 || i === 13 || i === 18 || i === 23) {
                key += "-";
            } else {
                key += characters.charAt(Math.floor(Math.random() * characters.length));
            }
        }
        return key;
    },
    InitBranchTo: function () {

        empr_StockTransferRequisition.bindDxDdl("TBCODE", BranchTo, null, "key", "value", "Select", function (d) {
            console.log(d)
            $('#branchtohidden').val(d.value)
            if (d.value == null) {
                $('#branchtohidden').val('');
            }

        });

    },
    InitBranchFrom: function () {

        empr_StockTransferRequisition.bindDxDdl("BCODE", BranchFrom, null, "key", "value", "Select", function (d) {
            $('#branchfromhidden').val(d.value)
            if (d.value == null) {
                $('#branchfromhidden').val('');
            }

        });
        var dropdownInstance = $('#BCODE').dxSelectBox('instance');
        if (BranchTo.length > 0) {
            dropdownInstance.option('value', BranchFrom[0].key); // Bind the key of the first item
        }
        dropdownInstance.option('disabled', true);

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    CheckBLABEL: function (objects) {
        //if (objects.length === 0) {
        //    return false;
        //}

        //const firstBLABEL = objects[0].BLABEL;

        //for (let i = 1; i < objects.length; i++) {
        //    if (objects[i].BLABEL !== firstBLABEL) {
        //        return false;
        //    }
        //}

        //return true;
        if (objects.length === 0 || objects.length === 1 || objects.length === 2) {
            return false;
        }

        // Find the last object with BLABEL property
        let lastBLABEL;
        for (let i = objects.length - 1; i >= 0; i--) {
            if (objects[i].hasOwnProperty('BLABEL')) {
                lastBLABEL = objects[i].BLABEL;
                break;
            }
        }

        // If BLABEL property is missing in all objects, return false
        if (!lastBLABEL) {
            return false;
        }

        // Check if BLABEL is same on all objects with BLABEL property
        for (let i = objects.length - 1; i >= 0; i--) {
            if (objects[i].hasOwnProperty('BLABEL') && objects[i].BLABEL !== lastBLABEL) {
                return false;
            }
        }

        return true;
    },
    FindMatchingIndexes: function (objects) {
        const matchingIndexes = [];

        if (objects.length === 0) {
            return matchingIndexes;
        }

        const standardBLABEL = parseInt(objects[objects.length - 1].BLABEL); // Convert BLABEL to integer

        // Iterate over objects and compare BLABEL with the standard BLABEL
        for (let i = 0; i < objects.length; i++) {
            const obj = objects[i];
            const objBLABEL = parseInt(obj.BLABEL); // Convert BLABEL to integer for comparison
            if (obj.hasOwnProperty('iteM_CODE') && obj.hasOwnProperty('BLABEL') && objBLABEL === standardBLABEL) {
                matchingIndexes.push(i); // If BLABEL matches and iteM_CODE exists, add index to matchingIndexes
            }
        }

        return matchingIndexes;
    },
    FindNonMatchingIndexes: function (objects) {
        const nonMatchingIndexes = [];

        if (objects.length === 0) {
            return nonMatchingIndexes;
        }

        const standardBLABEL = parseInt(objects[objects.length - 1].BLABEL); // Convert BLABEL to integer

        // Iterate over objects and compare BLABEL with the standard BLABEL
        for (let i = 0; i < objects.length; i++) {
            const obj = objects[i];
            const objBLABEL = parseInt(obj.BLABEL); // Convert BLABEL to integer for comparison
            if (obj.hasOwnProperty('iteM_CODE') && obj.hasOwnProperty('BLABEL') && objBLABEL !== standardBLABEL) {
                nonMatchingIndexes.push(i); // If BLABEL does not match and iteM_CODE exists, add index to nonMatchingIndexes
            }
        }

        return nonMatchingIndexes;
    },
    GenerteCartonSticker: function () {

        //if ($('#StockDetailContainer').dxDataGrid('instance').hasEditData()) {
        //    $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
        //        var response = empr_StockTransferRequisition.GetGridData();
        //        response.then((data) => {
        //            if (data.length > 0) {
        //                data.forEach(obj => {
        //                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
        //                    obj.FBCODE = $("#BCODE").dxSelectBox('option', 'value');
        //                    obj.TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
        //                });
        //                console.log('Final Object before Generating Sticker');
        //                console.log(data);
        //                empr_StockTransferRequisition.GetReport(data);
        //            }
        //        });
        //    });
        //}
        //else {
        //    var response = empr_StockTransferRequisition.GetGridData();
        //    response.then((data) => {
        //        if (data.length > 0) {
        //            data.forEach(obj => {
        //                obj.VOUCHER_NO = $("#VOUCHER_NO").val();
        //                obj.FBCODE = $("#BCODE").dxSelectBox('option', 'value');
        //                obj.TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
        //            });
        //            console.log('Final Object before Generating Sticker');
        //            console.log(data);
        //            empr_StockTransferRequisition.GetReport(data);
        //        }
        //    });
        //}

        var response = empr_StockTransferRequisition.GetGridData();
        response.then((data) => {
            if (data.length > 0) {
                data.forEach(obj => {
                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                    obj.FBCODE = $("#BCODE").dxSelectBox('option', 'value');
                    obj.TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
                });
                console.log('Final Object before Generating Sticker');
                console.log(data);
                empr_StockTransferRequisition.GetReport(data);
            }
        });

    },
    GetGridData: async function () {
        //return await $('#StockDetailContainer').dxDataGrid('instance').option('dataSource');
        return await $('#GenerateCartonStickerGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    },
    GetReport: function (dataModel) {
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, '/StockTransferRequisition/GetReport', function (data) {
            if (data.msgType == 1) {
                var codes = dataModel.map(obj => `'${obj.dT_CODE}'`).join(', ');
                empr_StockTransferRequisition.UpdatePrintStatus(codes);
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html(`<center><object data="${url}" width="1100" height="500"></object></center>`);
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 500);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GeneratePrintReport: function () {

        empr_StockTransferRequisition.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        debugger
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/StockTransferRequisition/GetPrintReport", function (data) {
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
    InitCartonStickerGrid: function () {
        ajaxHelper.ajaxGetJson('/StockTransferRequisition/GetStockTransferRequisitionDetailByCode?code=' + $('#Code').val(), function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#GenerateCartonStickerGridContainer').data('dxDataGrid') != undefined) {
                        $('#GenerateCartonStickerGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_StockTransferRequisition.CreateCartonStickerGrid(data.data);
                    $('#GenerateStickerModal').modal('show');
                } else {
                    empr_helper.notify("No detail found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateCartonStickerGrid: function (dataSrc) {
        var col = [
            {
                dataField: 'iteM_CODE',
                caption: "Barcode",
                width: 150,
                allowEditing: false,
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                allowEditing: false,
            },
            {
                dataField: 'qtY2',
                caption: 'Qunatity 2',
                allowEditing: false,
            },
            {
                dataField: 'baL_QTY',
                caption: 'Balance Quantity',
                allowEditing: false,
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                allowEditing: false,
            },
            {
                dataField: 'iteM_ID',
                caption: 'Item Id',
                allowEditing: false,
            },
            {
                dataField: 'print',
                caption: 'Printed',
                allowEditing: false,
            },
        ];
        empr_helper.editableDxGridbindingForTransactions('#GenerateCartonStickerGridContainer', col, dataSrc, "StockTransferRequisitionDetails", "iteM_CODE", 'multiple');
        setTimeout(function () {
            $('#GenerateCartonStickerGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    UpdatePrintStatus: function (codes) {
        ajaxHelper.ajaxPostJsonData({ codes: codes }, '/StockTransferRequisition/UpdatePrintStatus', function (data) { }, false, true);
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/StockTransferRequisition/GetReportTypes", function (data) {
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
}


$(function () {
    //window.addEventListener('beforeprint', function () {
    //    alert('Printing is about to start!');
    //});

    //window.addEventListener('afterprint', function () {
    //    alert('Printing has finished!');
    //});

    //document.addEventListener('DOMContentLoaded', function () {
    //    var objReport = document.getElementById('objReport');
    //    var embeddedDocument = objReport.contentDocument || objReport.contentWindow.document;

    //    if (embeddedDocument) {
    //        embeddedDocument.addEventListener('click', function (event) {
    //            if (event.target.tagName.toLowerCase() === 'button' && event.target.getAttribute('aria-label') === 'Print') {
    //                alert('Print button clicked!');
    //            }
    //        });
    //    }
    //});
})