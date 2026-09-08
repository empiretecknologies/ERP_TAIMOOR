var empr_StockReceive = {
    totalCount: 0,
    rowsCount: 0,
    PickVoucherNo: '',
    transferDirection: "right",
    InitEvents: function () {
        $(document).ready(function () {
            empr_StockReceive.InitQuickSearchGrid();
            //empr_StockReceive.CreateRightGrid();
            empr_StockReceive.ResetForm();
            empr_StockReceive.InitDoubleClickEvents();

            // Arrow Button
            $("#BtnTransfer").on("click", function () {
                var icon = $(this).find("i");

                if (empr_StockReceive.transferDirection === "right") {
                    empr_StockReceive.transferDirection = "left";
                    icon.removeClass("fa-long-arrow-right").addClass("fa-long-arrow-left");
                } else {
                    empr_StockReceive.transferDirection = "right";
                    icon.removeClass("fa-long-arrow-left").addClass("fa-long-arrow-right");
                }
            });

            $("#BtnEnter").on("click", function () {
                var itemName = $("#PICK_ITEM").val().trim();
                if (itemName) {
                    empr_StockReceive.performTransfer(itemName);
                    $("#PICK_ITEM").val("").focus();

                    empr_StockReceive.SaveAttempt();
                }
            });

            $("#PICK_ITEM").on("keypress", function (e) {
                if (e.which === 13) {
                    $("#BtnEnter").click();
                    e.preventDefault();
                }
            });

            $("#PICK_QTY").on("keypress", function (e) {
                if (e.which === 13) {
                    $("#BtnEnter").click();
                    e.preventDefault();
                }
            });

            $('body').on('click', '#BtnSodaPick', function () {

                var rightGrid = $("#StockReceiveContainer").dxDataGrid("instance");
                var rowCount = rightGrid.totalCount();

                if (rowCount > 0) {
                    empr_helper.notify("Please complete or clear transfer before loading new Transfer.", 2);
                    return;
                }
                else {
                    var pickData = $('#PICK_DATA').val();
                    empr_StockReceive.PickVoucherNo = pickData;
                    empr_StockReceive.GetPickData(pickData);
                }
            });

            $('.chkCell').prop('checked', true);
            $('body').on('click', '.elm_edit', function () {
                empr_StockReceive.ResetForm();
                var id = $(this).attr("reportid");
                $('#PICK_DATA').val(id);
                //empr_helper.selectedBill = id;
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_StockReceive.GetPickData(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/StockTransfer/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_StockReceive.GetStockTransferByCode(data.data.code);
                    }
                }, false, true);
            });

            //$('body').on('click', '#BtnSave', function () {
            //    if (Permissions != "Admin") {
            //        if (!$("#Code").val() && !Permissions.r_ADD) {
            //            empr_helper.notify("You are not allowed to add new record !", 2);
            //        }
            //        else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
            //            empr_helper.notify("You are not allowed to edit records !", 2);
            //        } else {
            //            $("#Loader").show();
            //            $("#Loader").css('display', 'flex');
            //            setTimeout(function () {
            //                if (empr_StockReceive.ValidateMainInfo()) {
            //                    empr_StockReceive.Save();
            //                }
            //                setTimeout(function () {
            //                    $("#Loader").hide();
            //                }, 500);
            //            }, 200);
            //        }
            //    } else {
            //        $("#Loader").show();
            //        $("#Loader").css('display', 'flex');
            //        setTimeout(function () {
            //            if (empr_StockReceive.ValidateMainInfo()) {
            //                empr_StockReceive.Save();
            //            }
            //            setTimeout(function () {
            //                $("#Loader").hide();
            //            }, 500);
            //        }, 200);
            //    }
            //});

            $('body').on('click', '#BtnDelete', function () {
                empr_StockReceive.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_StockReceive.ResetForm();
                //$('#REF').focus();
                $('#ButtonsDiv').hide();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_StockReceive.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_StockReceive.GeneratePrintReport();
            });


            


            $('.bs-example-modal-lg').on('hidden.bs.modal', function () {
                $("#itemIdHidden").val("");
                $("#IQty").val("");
                $('#PICK_ITEM').dxSelectBox('instance').option('value', '');
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_StockReceive.GeneratePrintReport();
            });


            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_ADD && $('#BtnSodaPick').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    performTransfer: function (itemName) {
        var leftGrid = $("#StockDetailContainer").dxDataGrid("instance");
        var rightGrid = $("#StockReceiveContainer").dxDataGrid("instance");
        var leftData = leftGrid.option("dataSource");
        var rightData = rightGrid.option("dataSource");

        var pickQty = parseInt($("#PICK_QTY").val()) || 1;
        var pickId = '';
        var iteM_CODE = '';

        if (empr_StockReceive.transferDirection === "right") {
            var leftRow = leftData.find(x => x.iteM_NAME === itemName);
            if (!leftRow) return empr_helper.notify("Item not found in stock!", 2);

            pickId = parseInt(leftRow.picK_ID);
            iteM_CODE = parseInt(leftRow.iteM_CODE);

        }
        else
        {
            // Right -> Left
            var rightRow = rightData.find(x => x.iteM_NAME === itemName);
            if (!rightRow) return empr_helper.notify("Item not found in right stock!", 2);

            var availableQty = parseInt(rightRow.baL_QTY);
            if (availableQty <= 0) return empr_helper.notify("All qty already in left!", 2);

            pickId = parseInt(rightRow.picK_ID);
            iteM_CODE = parseInt(rightRow.iteM_CODE);
            pickQty = '-' + pickQty;
        }


        $('#ITEM_NAME').val(iteM_CODE);
        $('#REC_QTY').val(pickQty);
        $('#PICK_ID').val(pickId);


        $("#PICK_QTY").val("");
    },
    InitDoubleClickEvents: function () {
        var leftGrid = $("#StockDetailContainer").dxDataGrid("instance");
        var rightGrid = $("#StockReceiveContainer").dxDataGrid("instance");

        leftGrid.on("rowDblClick", function (e) {
            var rowData = e.data;
            if (!rowData || parseInt(rowData.baL_QTY) <= 0) return;

            empr_StockReceive.transferDirection = "right";
            $("#BtnTransfer i").removeClass("fa-long-arrow-left").addClass("fa-long-arrow-right");

            var iteM_CODE = parseInt(rowData.iteM_CODE);
            var picK_ID = parseInt(rowData.picK_ID);
            var qtyToTransfer = parseInt(rowData.baL_QTY);

            
            $('#ITEM_NAME').val(iteM_CODE);
            $('#REC_QTY').val(qtyToTransfer);
            $('#PICK_ID').val(picK_ID);

            empr_StockReceive.SaveAttempt();

        });

        rightGrid.on("rowDblClick", function (e) {
            debugger;
            var rowData = e.data;
            if (!rowData || parseInt(rowData.baL_QTY) <= 0) return;

            empr_StockReceive.transferDirection = "left";
            $("#BtnTransfer i").removeClass("fa-long-arrow-right").addClass("fa-long-arrow-left");

            var iteM_CODE = parseInt(rowData.iteM_CODE);
            var qtyToTransfer = parseInt(rowData.rqty);
            var picK_ID = parseInt(rowData.picK_ID);

            $('#ITEM_NAME').val(iteM_CODE);
            $('#REC_QTY').val('-'+qtyToTransfer);
            $('#PICK_ID').val(picK_ID);

            empr_StockReceive.SaveAttempt();

        });
    },
    GetDataToSave: function () {

        // Collect master data
        var ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');

        var masterRecord = {
            TRAN_ID: ID,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            REF: REF,
            REMARKS: REMARKS
        }

        // Collect detail data
        var ITEM_CODE = $('#ITEM_NAME').val();
        var QTY = $('#REC_QTY').val();
        var PICK_ID = $('#PICK_ID').val();

        var detailRecords = {
            ITEM_CODE: parseInt(ITEM_CODE),
            B_QTY: parseInt(QTY),
            PICK_ID: parseInt(PICK_ID)
        }

        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords
        };
        return modelRecord;
    },
    SaveAttempt: function () {

        var dataModel = empr_StockReceive.GetDataToSave();
        console.log('SaveAttempt', dataModel);

        ajaxHelper.ajaxPostJsonData(dataModel, "/StockReceive/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $("#Code").val(data.data.code);
                $("#V_DATE").val(data.data.vdate);
                $("#VOUCHER_NO").val(data.data.voucherNo);
                $("#REF").val(data.data.ref);
                $("#REMARKS").val(data.data.remarks);

                $('#ITEM_NAME').val('');
                $('#REC_QTY').val('');
                $('#PICK_ID').val('');

                console.log('SaveAttemptResponce',data);
            }
        }, false, true);

        empr_StockReceive.GetPickData(empr_StockReceive.PickVoucherNo);

    },
    CreateGrid: function (dataSrc) {
        console.log('CreateGrid',dataSrc);
        var Caption = "";
        if (Type == "I") {
            Caption = "Item";
        }
        else {
            Caption = "Bar Code";
        }
        var col = [
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            
        ];
        if (Type === "I") {
            col.push(
                {
                    dataField: 'iteM_NAME',
                    caption: Caption,
                    allowSorting: false,
                },
                {
                    dataField: 'iteM_CODE',
                    visible: false,
                },
                {
                    dataField: 'remarks',
                    caption: 'Remarks',
                    width: 110,
                },
                //{
                //    dataField: 'i_QTY',
                //    caption: 'QTY',
                //    width: 50,
                //},
                //{
                //    dataField: 'r_QTY',
                //    caption: 'QTY',
                //    width: 50,
                //},
                {
                    dataField: 'baL_QTY',
                    caption: 'QTY',
                    width: 70,
                },
                {
                    dataField: 'unit',
                    caption: 'Unit',
                    width: 70,
                },
                {
                    dataField: 'color',
                    caption: 'Color',
                    width: 70,
                },
                {
                    dataField: 'size',
                    caption: 'Size',
                    width: 70,
                },
            );
        }
        else {
            col.push(
                {
                    dataField: 'iteM_NAME',
                    caption: Caption,
                    width: 130,
                    allowSorting: false,
                },
                {
                    dataField: 'iteM_CODE',
                    visible: false,
                },
                {
                    dataField: 'remarks',
                    caption: 'Remarks',
                    width: 110,
                },
                {
                    dataField: 'iqty',
                    caption: 'I.Qty',
                    width: 60,
                },
                {
                    dataField: 'rqty',
                    caption: 'R.Qty',
                    width: 60,
                },
                {
                    dataField: 'baL_QTY',
                    caption: 'B.Qty',
                    width: 60,
                },
                //{
                //    dataField: 'unit',
                //    caption: 'Unit',
                //    width: 70,
                //},
                {
                    dataField: 'color',
                    caption: 'Color',
                    width: 70,
                },
                {
                    dataField: 'size',
                    caption: 'Size',
                    width: 50,
                },
            );
        }
        empr_helper.dxGridbindingForStockReceive('#StockDetailContainer', col, dataSrc, "StockTransfer", "iteM_CODE");
        //if (dataSrc.length == 0) {
        //    $('#StockDetailContainer').dxDataGrid('instance').addRow().done(function () {
        //        $('#StockDetailContainer').dxDataGrid('instance').saveEditData();
        //    });
        //}

        //setTimeout(function () {
        //    var nextElement = $('#StockDetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#StockDetailContainer').dxDataGrid('instance').focus(nextElement);  
        //}, 1500);
    },
    CreateRightGrid: function (dataSrc) {
        console.log('CreateRightGrid',dataSrc);
        //empr_StockReceive.IsPickData = dataSrc.some(item => item.grouP_NAME !== "");
        //if (dataSrc.length > 0) {
        //    empr_StockReceive.rowsCount = dataSrc.length - 1;
        //}
        var Caption = "";
        if (Type == "I") {
            Caption = "Item";
        }
        else {
            Caption = "Bar Code";
        }
        var col = [
            {
                dataField: 'picK_ID',
                caption: 'picK_ID',
                visible: false,
            },
        ];

        if (Type === "I") {
            col.push(
                {
                    dataField: 'iteM_NAME',
                    caption: Caption,
                    allowSorting: false,
                },
                {
                    dataField: 'iteM_CODE',
                    visible: false,
                },
                {
                    dataField: 'traN_ID',
                    visible: false,
                },
                {
                    dataField: 'remarks',
                    caption: 'Remarks',
                    width: 110,
                },
                //{
                //    dataField: 'i_QTY',
                //    caption: 'QTY',
                //    width: 50,
                //},
                //{
                //    dataField: 'r_QTY',
                //    caption: 'QTY',
                //    width: 50,
                //},
                {
                    dataField: 'baL_QTY',
                    caption: 'QTY',
                    width: 70,
                },
                {
                    dataField: 'unit',
                    caption: 'Unit',
                    width: 70,
                },
                {
                    dataField: 'color',
                    caption: 'Color',
                    width: 70,
                },
                {
                    dataField: 'size',
                    caption: 'Size',
                    width: 70,
                },
            );
        }
        else {
            col.push(
                {
                    dataField: 'iteM_NAME',
                    caption: Caption,
                    width: 170,
                    allowSorting: false,
                },
                {
                    dataField: 'iteM_CODE',
                    visible: false,
                },
                {
                    dataField: 'traN_ID',
                    visible: false,
                },
                {
                    dataField: 'remarks',
                    caption: 'Remarks',
                    width: 150,
                },
                {
                    dataField: 'rqty',
                    caption: 'R.Qty',
                    width: 70,
                },
                //{
                //    dataField: 'unit',
                //    caption: 'Unit',
                //    width: 70,
                //},
                {
                    dataField: 'color',
                    caption: 'Color',
                    width: 70,
                },
                {
                    dataField: 'size',
                    caption: 'Size',
                    width: 70,
                },
            );
        }
        empr_helper.dxGridbindingForStockReceive('#StockReceiveContainer', col, dataSrc, "StockTransfer", "iteM_CODE");
        //if (dataSrc.length == 0) {
        //    $('#StockDetailContainer').dxDataGrid('instance').addRow().done(function () {
        //        $('#StockDetailContainer').dxDataGrid('instance').saveEditData();
        //    });
        //}

        //setTimeout(function () {
        //    var nextElement = $('#StockDetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#StockDetailContainer').dxDataGrid('instance').focus(nextElement);  
        //}, 1500);
    },
    InitBarcodePickGrid: function () {
        ajaxHelper.ajaxGetJson('/StockTransfer/GetBarcodeList', function (data) {
            console.log(data)
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_StockReceive.CreateBarcodeGrid(data.data);
                    $('#BarcodePickModal').modal('show');
                    $('#BarcodePickModal').css('display', 'block');
                } else {
                    empr_helper.notify("No barcodes found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateBarcodeGrid: function (dataSrc) {
        var col = [
            { dataField: 'barcodE_CODE_CODE', caption: 'Barcode Code', visible: false, },
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
            { dataField: 'amt', caption: 'Amount', visible: false, allowEditing: false },
            { dataField: 'neT_AMT', caption: 'Net Amount', visible: false, allowEditing: false },
            { dataField: 'color', caption: 'Color', visible: false, allowEditing: false },
            { dataField: 'size', caption: 'Size', visible: false, allowEditing: false },
            { dataField: 'iteM_ID', caption: 'Item', visible: true, width: 120 },
            { dataField: 'barcode', caption: 'Barcode', allowEditing: false, width: 120 },
            { dataField: 'sizE_NAME', caption: 'Size', allowEditing: false, width: 120 },
            { dataField: 'coloR_NAME', caption: 'Color', allowEditing: false, width: 120 },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, width: 120 },
        ];
        empr_helper.dxGridbindingVouchers('#BarcodePickGridContainer', col, dataSrc, "StockTransferBarcodeGrid");
        setTimeout(function () {
            $('#BarcodePickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    DeleteRow: function (index) {
        ;
        const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    //empr_StockReceive.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/StockTransfer/DeleteStockTransferDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_StockReceive.rowsCount -= 1;
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
        empr_StockReceive.GetStockReceive();
    },
    GetStockReceive: function () {
        ajaxHelper.ajaxGetJson('/StockReceive/GetStockReceive', function (data) {
            if (data.msgType == 1) {
                empr_StockReceive.CreateQuickSearchGrid(data.data);
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
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.picK_DATA} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.picK_DATA} title="Edit"><i class="fa fa-edit"></i></a>
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
        { dataField: 'picK_DATA', caption: 'Transfer #', },
        { dataField: 'iteM_NAME', caption: 'Item Name', },
        { dataField: 'iteM_REMARKS', caption: 'Item Remarks', },
        { dataField: 'qty', caption: 'Qty', },
        { dataField: 'color', caption: 'Color', },
        { dataField: 'size', caption: 'Size', },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "StockTransferQS");
    },
    GetStockTransferByCode: function (code) {
        ajaxHelper.ajaxGetJson('/StockTransfer/GetStockTransferByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    //$('#AM').dxSelectBox('instance').option('value', response.am);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    //empr_StockReceive.InitBranchFrom(response.deP_ID);
                    $('#BCODE').dxSelectBox('instance').option('value', response.bcode);
                    //empr_StockReceive.InitBranchTo(response.deP_ID);
                    //$('#TBCODE').dxSelectBox('instance').option('value', response.tbcode);
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
                    empr_StockReceive.CreateGrid(data.detail.data);
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
    GetStockTransferDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/StockTransfer/GetStockTransferDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_StockReceive.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_StockReceive.GetDataToSave();

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
        if (data.Master.AM == '') {
            empr_helper.notify("Please select auto/manual", 2);
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

        //if (!empr_StockReceive.IsBLabelValid) {
        //    empr_helper.notify("Please remove the red row.", 2);
        //    valid = false;
        //    return valid;
        //}

        if (data.Detail.length > Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    ResetForm: function () {
        empr_StockReceive.PickVoucherNo = '';
        empr_StockReceive.CreateGrid();
        empr_StockReceive.CreateRightGrid();
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        empr_StockReceive.InitReportTypeDDL();
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/StockTransfer/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_StockReceive.ResetForm();
                    $('#BtnDelete').hide();
                    $('#ButtonsDiv').hide();
                }
            }, false, true);
        });
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    GetReport: function (dataModel) {
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, '/StockTransfer/GetReport', function (data) {
            if (data.msgType == 1) {
                var codes = dataModel.map(obj => `'${obj.dT_CODE}'`).join(', ');
                empr_StockReceive.UpdatePrintStatus(codes);
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html(`<center><object data="${url}" width="1100" height="500"></object></center>`);
                    //$('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='500'></object></center>");
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
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/StockTransfer/GetPrintReport", function (data) {
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
                caption: 'Item',
                allowEditing: false,
            },
            {
                dataField: 'print',
                caption: 'Printed',
                allowEditing: false,
            },
        ];
        empr_helper.editableDxGridbindingForTransactions('#GenerateCartonStickerGridContainer', col, dataSrc, "StockTransferDetails", "iteM_CODE", 'multiple');
        setTimeout(function () {
            $('#GenerateCartonStickerGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    UpdatePrintStatus: function (codes) {
        ajaxHelper.ajaxPostJsonData({ codes: codes }, '/StockTransfer/UpdatePrintStatus', function (data) { }, false, true);
    },
    InitSodaPickGrid: function () {
        var data = empr_StockReceive.GetDataToSave();
        if (data.Master.V_DATE == "" || data.Master.V_DATE == null || data.Master.V_DATE == undefined) {
            empr_helper.notify("Please select the voucher date first.", 2);
        }
        if (data.Master.TBCODE == "" || data.Master.TBCODE == null || data.Master.TBCODE == undefined) {
            empr_helper.notify("Please select the branch to.", 2);
        }
        else {
            empr_StockReceive.GetSodaBookFeedingDetailBySodaDate(data.Master.V_DATE, data.Master.TBCODE);
        }
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/StockTransfer/GetReportTypes", function (data) {
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
    GetPickData: function (pickId) {
        ajaxHelper.ajaxPostJsonData({ pickId: pickId }, '/StockReceive/GetPickData', function (data) {
            console.log('GetPickData', data.data3);
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_StockReceive.CreateGrid(data.data);
                    empr_StockReceive.CreateRightGrid(data.data2);
                    $('#PICK_DATA').prop('disabled', true);
                    debugger;
                    if (data.data3) {
                        $("#Code").val(data.data3.traN_ID);
                        $("#V_DATE").val(data.data3.v_DATE);
                        $("#VOUCHER_NO").val(data.data3.voucheR_NO);
                        $("#REF").val(data.data3.ref);
                        $("#REMARKS").val(data.data3.remarks);
                    }

                    $("#PICK_ITEM").focus();
                } else {
                    empr_helper.notify("No Stock Tranfer found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
}



