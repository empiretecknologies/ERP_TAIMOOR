var empr_StockTransfer = {
    totalCount: 0,
    rowsCount: 0,
    BLABEL: '',
    IsBLabelValid: true,
    IsPickData: false,
    InitEvents: function () {
        $(document).ready(function () {
            empr_StockTransfer.InitQuickSearchGrid();

            empr_StockTransfer.ResetForm();
            empr_StockTransfer.InitBranchTo();
            empr_StockTransfer.InitItemIds();
            empr_StockTransfer.InitBranchFrom();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_StockTransfer.GetStockTransferByCode(data.traN_ID);
                }
            });
            $('.chkCell').prop('checked', true);

            //$('#pills-warningsummary-tab').click(function () {
            //    debugger;
            //    if ($('#ItemPrintContainer').is(':empty')) {
            //        $('#ItemPrintContainer').html('<div class="p-3">Loading Summary Grid...</div>');

            //    }
            //});
            $('body').on('click','#BtnGenerateBarcode', function () {
                empr_StockTransfer.PrepareDataForValidation();
            });
            $('body').on('click', '.elm_edit', function () {
                empr_StockTransfer.ResetForm();
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                empr_helper.selectedBill = id;
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_StockTransfer.GetStockTransferByCode(id);
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
                        empr_StockTransfer.GetStockTransferByCode(data.data.code);
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
                        $("#Loader").show();
                        $("#Loader").css('display', 'flex');
                        setTimeout(function () {
                            if (empr_StockTransfer.ValidateMainInfo()) {
                                empr_StockTransfer.Save();
                            }
                            setTimeout(function () {
                                $("#Loader").hide();
                            }, 500);
                        }, 200);
                    }
                } else {
                    $("#Loader").show();
                    $("#Loader").css('display', 'flex');
                    setTimeout(function () {
                        if (empr_StockTransfer.ValidateMainInfo()) {
                            empr_StockTransfer.Save();
                        }
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);
                    }, 200);
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_StockTransfer.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_StockTransfer.ResetForm();
                //$('#REF').focus();
                $('#ButtonsDiv').hide();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_StockTransfer.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnGenerateCartonSticker', function () {
                empr_StockTransfer.InitCartonStickerGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_StockTransfer.GeneratePrintReport();
            });

            $('body').on('click', '#BtnGenerateSticker', function () {
                empr_StockTransfer.GenerteCartonSticker();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_StockTransfer.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_StockTransfer.AddSodaToDelivery();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnGetItems', function () {
                var itemId = $("#itemIdHidden").val();
                var itemQty = $("#IQty").val();
                if (itemId != '' && itemQty != '') {
                    if (Permissions != "Admin") {
                        if (!$("#Code").val() && !Permissions.r_ADD) {
                            empr_helper.notify("You are not allowed to add new record !", 2);
                        }
                        else {
                            empr_StockTransfer.GetPurchaseBillDetailByItem(itemId, itemQty);
                        }
                    } else {
                        empr_StockTransfer.GetPurchaseBillDetailByItem(itemId, itemQty);
                    }
                } else {
                    empr_helper.notify("Please fill all fields.", 2);
                }
            });

            $('body').on('click', '#BtnAddBarcodes', function () {
                var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedBarcodes.length > 0) {
                    empr_StockTransfer.AddBarcodeToGrid();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('.bs-example-modal-lg').on('hidden.bs.modal', function () {
                $("#itemIdHidden").val("");
                $("#IQty").val("");
                $('#PICK_ITEM').dxSelectBox('instance').option('value', '');
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_StockTransfer.GeneratePrintReport();
            });

            $("#BARCODE").on("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    empr_StockTransfer.processBarcode();
                }
            });

            $("#barcodeBtn").on("click", function () {
                empr_StockTransfer.processBarcode();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_ADD && $('#BtnSodaPick').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    processBarcode: function () {
        var inputVal = $("#BARCODE").val().trim();
        if (inputVal === "") return;

        var matches = Items.filter(u =>
            u.value == inputVal || u.value.endsWith(inputVal)
        );

        if (matches.length > 0) {
            let selectedItem;

            // Pure numeric case
            if (/^\d+$/.test(matches[0].value)) {
                if (matches.length > 1) {
                    matches.sort((a, b) => parseInt(a.value) - parseInt(b.value));
                }
                selectedItem = matches[0];
            } else {
                // Prefix case
                if (matches.length === 1) {
                    selectedItem = matches[0];
                } else {
                    empr_helper.notify("Multiple matches, please type full barcode", 2);
                    setTimeout(() => { $("#BARCODE").focus(); }, 100);
                    return;
                }
            }

            // Row add & values set
            empr_StockTransfer.AddRow();
            var grid = $("#StockDetailContainer").dxDataGrid("instance");
            var rowIndex = 0;

            grid.cellValue(rowIndex, "iteM_CODE", selectedItem.key);
            grid.cellValue(rowIndex, "qty", 1);

            setTimeout(() => { $("#BARCODE").val("").focus(); }, 100);

        } else {
            empr_helper.notify("Item not found", 2);
            setTimeout(() => { $("#BARCODE").focus(); }, 100);
        }
    },

    CreateItemPrintGrid: function (dataSrc) {
        console.log(dataSrc)
      
        var col = [

            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },


            //{
            //    dataField: 'grouP_NAME',
            //    caption: 'Group',
            //    allowEditing: false,
            //    visible: empr_StockTransfer.IsPickData,

            {
                dataField: 'item',
                caption: 'Item',
                allowEditing:false
                //visible: false,
            },
            {
                dataField: 'barcode',
                caption: 'Barcode',

                allowEditing: false,
                //visible: false,
            },
            {
                dataField: 'qty',
                caption: 'Qty',
                allowEditing: true

                //visible: false,
            },
            //{
            //    dataField: 'rate',
            //    caption: 'Rate',
            //    allowEditing:true
            //    //visible: false,
            //},

        ];
        //empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "StockTransferQS");
        empr_helper.editableDxGridbinding('#ItemPrintContainer', col, dataSrc, "ItemPrint", "multiple");
        if (dataSrc.length == 0) {
            $('#ItemPrintContainer').dxDataGrid('instance').addRow().done(function () {
                $('#ItemPrintContainer').dxDataGrid('instance').saveEditData();
            });
        }

        //setTimeout(function () {
        //    var nextElement = $('#StockDetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#StockDetailContainer').dxDataGrid('instance').focus(nextElement);  
        //}, 1500);
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
        empr_StockTransfer.IsPickData = dataSrc.some(item => item.grouP_NAME !== "");
        if (dataSrc.length > 0) {
            empr_StockTransfer.rowsCount = dataSrc.length - 1;
        }
        var Caption = "";
        if (Type == "I") {
            Caption = "Item";
        }
        else {
            Caption = "Bar Code";
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_StockTransfer.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_StockTransfer.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_StockTransfer.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const searchAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_StockTransfer.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}${searchAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_StockTransfer.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_StockTransfer.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_StockTransfer.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                           <a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_StockTransfer.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
        ];
        if (Type === "I") {
            col.push(
                //{
                //    dataField: 'grouP_NAME',
                //    caption: 'Group',
                //    allowEditing: false,
                //    visible: empr_StockTransfer.IsPickData,

                {
                    dataField: 'iteM_CODE',
                    caption: Caption,
                    //width: 150,
                    allowSorting: false,
                    //lookup: {
                    //    dataSource: Items,
                    //    displayExpr: 'value',
                    //    valueExpr: 'key'
                    //},
                    lookup: {
                        dataSource: {
                            store: Items,
                            paginate: true,
                            pageSize: 50
                        },
                        displayExpr: 'value',
                        valueExpr: 'key',
                        searchEnabled: true,
                        showClearButton: true
                    },
                    setCellValue: function (newData, value, currentRowData) {
                        if (value != '') {
                            newData.iteM_CODE = value;
                            var selectedItem = Items.filter(u => u.key == value);
                            if (selectedItem.length > 0) {
                                debugger;
                                if (empr_StockTransfer.BLABEL == '') {
                                    empr_StockTransfer.BLABEL = selectedItem[0].blabel;
                                }
                                debugger;
                                newData.rate = selectedItem[0].rate;
                                newData.currentStock = selectedItem[0].balance;
                                newData.pack = selectedItem[0].pack;
                                newData.unit = parseInt(selectedItem[0].unit, 10);
                                newData.blabel = selectedItem[0].blabel;
                                newData.iteM_ID = selectedItem[0].itemID;
                            }
                        }
                    },
                },
                {
                    dataField: 'currentStock',
                    caption: 'Stk Balance',
                    allowEditing: false,
                    dataType: 'number',
                    width: 100,
                },
                {
                    dataField: 'qty',
                    caption: 'Quantity',
                    width: 100,
                    alignment: 'right',
                    setCellValue: function (newData, value, currentRowData) {
                        newData.qty = value;
                        var qty = parseFloat(newData.qty) || 0;
                        var rate = parseFloat(currentRowData.rate) || 0;
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
                        if (!isNaN(qty)) {
                            var amt = qty * rate;
                            newData.amt = amt;
                        }

                    }
                },
                {
                    dataField: 'pack',
                    caption: 'T.Pack',
                    width: 100,
                    alignment: 'right',
                    allowEditing: false
                },
                {
                    dataField: 'unit',
                    caption: 'Unit',
                    width: 100,
                    lookup: {
                        dataSource: Units,
                        displayExpr: 'value',
                        valueExpr: 'key'
                    }
                },
                //},
                {
                    dataField: 'rate',
                    caption: 'Rate',
                    //allowEditing: false,
                    alignment: 'right',
                    visible: true,
                    width: 100,
                    //calculateCellValue: function (rowData) {



                    //    if (rowData.iteM_CODE != '' && rowData.iteM_CODE != 0) {
                    //        var selectedItem = Items.filter(u => u.key == rowData.iteM_CODE);
                    //        if (selectedItem.length > 0) {
                    //            return selectedItem[0].rate;
                    //        }
                    //    } else {
                    //        return 0;
                    //    }


                    //},
                    setCellValue: function (newData, value, currentRowData) {
                        debugger;
                        newData.rate = value;
                        var qty = parseFloat(currentRowData.qty) || 0;
                        var rate = parseFloat(newData.rate) || 0;

                        if (!isNaN(qty)) {
                            var amt = qty * rate;
                            newData.amt = amt;
                        }
                    }
                },
                {
                    dataField: 'amt',
                    caption: 'Amount',
                    width: 100,
                    alignment: 'right',
                    allowEditing: false,
                    //visible: false
                },

                {
                    dataField: 'qtY2',
                    caption: 'Quantity 2',
                    visible: CompCond == 2,
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
                    visible: CompCond == 2,
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
                    width: 300,
                },
                //{
                //    dataField: 'color',
                //    caption: 'Color',
                //    lookup: {
                //        dataSource: Colors,
                //        displayExpr: 'value',
                //        valueExpr: 'key'
                //    }
                //},
                //{
                //    dataField: 'size',
                //    caption: 'Size',
                //    lookup: {
                //        dataSource: Sizes,
                //        displayExpr: 'value',
                //        valueExpr: 'key'
                //    }
                //},
                //{
                //    dataField: 'grade',
                //    caption: 'Grade',
                //    lookup: {
                //        dataSource: Grades,
                //        displayExpr: 'value',
                //        valueExpr: 'key'
                //    }
                //},
                //{
                //    dataField: 'iteM_ID',
                //    caption: 'Item',
                //    allowEditing: false,
                //}
            );
        }
        else {
            col.push(
                {
                    dataField: 'iteM_CODE',
                    caption: Caption,
                    width: 150,
                    allowSorting: false,
                    lookup: {
                        dataSource: {
                            store: Items,
                            paginate: true,
                            pageSize: 50
                        },
                        displayExpr: 'value',
                        valueExpr: 'key',
                        searchEnabled: true,
                        showClearButton: true,
                        paging: {
                            enabled: true,
                            pageSize: 50,
                        }
                    },
                    setCellValue: function (newData, value, currentRowData) {
                        if (value != '') {
                            newData.iteM_CODE = value;
                            var selectedItem = Items.filter(u => u.key == value);
                            if (selectedItem.length > 0) {
                                if (empr_StockTransfer.BLABEL == '') {
                                    empr_StockTransfer.BLABEL = selectedItem[0].blabel;
                                }
                                newData.blabel = selectedItem[0].blabel;
                                newData.iteM_ID = selectedItem[0].itemID;
                                newData.color = selectedItem[0].color;
                                newData.size = selectedItem[0].size;
                            }
                        }
                    },
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
                    dataField: 'qtY2',
                    caption: 'Qunatity 2',
                    visible: CompCond == 2,
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
                    visible: CompCond == 2,
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
                },
                {
                    dataField: 'iteM_ID',
                    caption: 'Item',
                    disabled: true
                },
                {
                    dataField: 'color',
                    caption: 'Color',
                    allowEditing: false,
                    lookup: {
                        dataSource: Colors,
                        displayExpr: 'value',
                        valueExpr: 'key'
                    }
                },
                {
                    dataField: 'size',
                    caption: 'Size',
                    allowEditing: false,
                    lookup: {
                        dataSource: Sizes,
                        displayExpr: 'value',
                        valueExpr: 'key'
                    }
                },
            );
        }
        empr_helper.editableDxGridbindingForTransactionsVouchers('#StockDetailContainer', col, dataSrc, "StockTransfer", "iteM_CODE");
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
    InitBarcodePickGrid: function () {
        ajaxHelper.ajaxGetJson('/StockTransfer/GetBarcodeList', function (data) {
            console.log(data)
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    debugger
                    //if ($('#BarcodePickGridContainer').data('dxDataGrid') != undefined) {
                    //    $('#BarcodePickGridContainer').data('dxDataGrid').dispose();
                    //}
                    empr_StockTransfer.CreateBarcodeGrid(data.data);
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
            //{ dataField: 'rate', caption: 'Rate', allowEditing: false, width: 120 },

        ];
        empr_helper.dxGridbindingVouchers('#BarcodePickGridContainer', col, dataSrc, "StockTransferBarcodeGrid");
        setTimeout(function () {
            $('#BarcodePickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    //AddBarcodeToGrid: function () {
    //    const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
    //    var dataSource = gridInstance.option("dataSource");
    //    $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
    //        dataSource = $('#StockDetailContainer').dxDataGrid('instance').option("dataSource");
    //    });
    //    var IsDataAvailableInGrid = false;
    //    $.each(dataSource, function (index, item) {
    //        if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
    //            IsDataAvailableInGrid = true;
    //        }
    //    });

    //    if (IsDataAvailableInGrid) {
    //        var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //        selectedBarcodes = empr_StockTransfer.SetData(selectedBarcodes);
    //        dataSource.unshift(...selectedBarcodes);
    //        gridInstance.option("dataSource", dataSource);
    //        gridInstance.refresh();
    //        empr_helper.MoveFocusToGridWithouTab('#StockDetailContainer', 0, 'iteM_CODE')
    //    }
    //    else {
    //        var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //        selectedBarcodes = empr_StockTransfer.SetData(selectedBarcodes);
    //        $('#StockDetailContainer').dxDataGrid('instance').option('dataSource', selectedBarcodes);
    //    }

    //    $('.modal').hide();
    //    $('#V_DATE').focus();
    //},
    AddBarcodeToGrid: function () {
        debugger;
        const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");

        gridInstance.saveEditData().done(function () {

            dataSource = gridInstance.option("dataSource");
            var IsDataAvailableInGrid = false;

            $.each(dataSource, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                    return false;
                }
            });

            var barcodeGrid = $('#BarcodePickGridContainer').dxDataGrid('instance');
            if (!barcodeGrid) {
                return;
            }

            var selectedBarcodes = barcodeGrid.getSelectedRowKeys();
            selectedBarcodes = empr_StockTransfer.SetData(selectedBarcodes);

            if (IsDataAvailableInGrid) {
                dataSource.unshift(...selectedBarcodes);
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                //empr_helper.MoveFocusToGridWithouTab('#StockDetailContainer', 0, 'iteM_CODE');
            } else {
                gridInstance.option('dataSource', selectedBarcodes);
            }

            try {
                barcodeGrid.dispose();
            } catch (ex) {
            }

            $('#BarcodePickModal').modal('hide');

            $('#V_DATE').focus();
        });
    },
    CloneRow: function (index) {
        ;
        if ($('#StockDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_StockTransfer.rowsCount += 1;
                const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length >= Limit && Limit != 0) {
                    empr_helper.notify("You can only add  " + Limit + " records.", 2);
                    return;
                }
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_StockTransfer.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            });
        }
        else {
            empr_StockTransfer.rowsCount += 1;
            const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if (dataSource.length > 0) {
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_StockTransfer.GenerateKey(36);
                clonedRowData.dT_CODE = 0;
                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
            }
        }
    },
    AddRow: function () {
        ;
        if ($('#StockDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_StockTransfer.rowsCount += 1;
                const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");
                if (dataSource.length >= Limit && Limit != 0) {
                    empr_helper.notify("You can only add  " + Limit + " records.", 2);
                    return;
                }
                if (Type == "B") {
                    dataSource.unshift({ __KEY__: empr_StockTransfer.GenerateKey(36), qty: 1, qtY2: 1, baL_QTY: 1, dT_CODE: 0 });
                }
                else {
                    dataSource.unshift({ __KEY__: empr_StockTransfer.GenerateKey(36), qtY2: 1, dT_CODE: 0 });
                }
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_StockTransfer.rowsCount += 1;
            const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");
            if (dataSource.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if (Type == "B") {
                dataSource.unshift({ __KEY__: empr_StockTransfer.GenerateKey(36), qty: 1, qtY2: 1, baL_QTY: 1, dT_CODE: 0 });
            }
            else {
                dataSource.unshift({ __KEY__: empr_StockTransfer.GenerateKey(36), qtY2: 1, dT_CODE: 0 });
            }
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
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
                    empr_StockTransfer.rowsCount -= 1;
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
                                    empr_StockTransfer.rowsCount -= 1;
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
        empr_StockTransfer.GetStockTransfer();
    },
    GetStockTransfer: function () {
        ajaxHelper.ajaxGetJson('/StockTransfer/GetStockTransfer', function (data) {
            if (data.msgType == 1) {
                empr_StockTransfer.CreateQuickSearchGrid(data.data);
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
        { dataField: 'am', caption: 'Auto/Manual', },
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
                    $('#AM').dxSelectBox('instance').option('value', response.am);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    empr_StockTransfer.InitBranchFrom(response.deP_ID);
                    $('#BCODE').dxSelectBox('instance').option('value', response.bcode);
                    empr_StockTransfer.InitBranchTo(response.deP_ID);
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
                $('#print-tab-li').removeClass('d-none');
                debugger;
                if (data.detail.msgType == 1) {





                    var updatedDetailData = data.detail.data.map(item => {
                        var stockRecord = Items.find(s => s.key == item.iteM_CODE);

                        return {
                            ...item,
                            currentStock: stockRecord ? stockRecord.balance : 0
                        };
                    });

                    empr_StockTransfer.CreateGrid(updatedDetailData);
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
                if (data.printData.msgType == 1) {
                    //setTimeout(function () {
                        empr_StockTransfer.CreateItemPrintGrid(data.printData.data);
                    //}, 500);
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
                empr_StockTransfer.CreateGrid(data.data);
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
        var AM = $('#AM').dxSelectBox('option', 'value');
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            BCODE: BCODE,
            TBCODE: TBCODE,
            REF: REF,
            REMARKS: REMARKS,
            AM: AM,
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
        if (empr_StockTransfer.rowsCount == detailRecords.length) {
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
        var data = empr_StockTransfer.GetDataToSave();

        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }
        //if (data.Master.TBCODE == '') {
        //    empr_helper.notify("Please select branch to.", 2);
        //    valid = false;
        //    return valid;
        //}
        //if (data.Master.AM == '') {
        //    empr_helper.notify("Please select auto/manual", 2);
        //    valid = false;
        //    return valid;
        //}

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

        //if (!empr_StockTransfer.IsBLabelValid) {
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
    Save: function () {
        ;
        var dataModel = empr_StockTransfer.GetDataToSave();
        var checkBLABEL = empr_StockTransfer.FindNonMatchingIndexes(dataModel.Detail);
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/StockTransfer/Save", function (data) {
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
                empr_StockTransfer.GetStockTransferDetailByCode($('#Code').val());
                $('#BtnDelete').show();
                $('#ButtonsDiv').show();
                $('#V_DATE').focus();
            }
        }, false, true);
    },
    ResetForm: function () {
        empr_StockTransfer.IsPickData = false;
        debugger;
        //$('#ItemPrintContainer').empty();
        var $summaryGrid = $("#ItemPrintContainer");
        if ($summaryGrid.length && $summaryGrid.data("dxDataGrid")) {
            // DevExtreme ka safe tree-shake/clean method
            $summaryGrid.dxDataGrid("dispose");
        }
        $summaryGrid.empty();
        $('#print-tab-li').addClass('d-none');
        $('#pills-warninghome-tab').tab('show');
        empr_StockTransfer.CreateGrid([{ __KEY__: empr_StockTransfer.GenerateKey(36), qtY2: 1, dT_CODE: 0 }]);
        //empr_StockTransfer.CreateItemPrintGrid([{ __KEY__: empr_StockTransfer.GenerateKey(36), qtY2: 1, dT_CODE: 0 }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        $('#BtnSodaPick').hide();
        $('#REMARKS').val('');
    
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('#AM').dxSelectBox('instance').option('value', 'A');
        empr_StockTransfer.InitBranchFrom();
        empr_StockTransfer.InitBranchTo();
        //$('#BCODE').dxSelectBox('instance').option('value', '');
        $('#TBCODE').dxSelectBox('instance').option('value', '');
        empr_StockTransfer.InitReportTypeDDL();
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
                    empr_StockTransfer.ResetForm();
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

        empr_StockTransfer.bindDxDdl("TBCODE", BranchTo, null, "key", "value", "Select", function (d) {
            console.log(d)
            $('#branchtohidden').val(d.value)
            if (d.value == null) {
                $('#branchtohidden').val('');
            }

        });

    },
    InitBranchFrom: function () {

        empr_StockTransfer.bindDxDdl("BCODE", BranchFrom, null, "key", "value", "Select", function (d) {
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
    InitItemIds: function () {
        empr_StockTransfer.bindDxDdl("PICK_ITEM", ItemIds, null, "key", "itemId", "Select", function (d) {
            console.log(d.value)
            if (d.value == null || d.value == '') {
                $('#itemIdHidden').val('');
            }
            else {
                $('#itemIdHidden').val(d.value)
            }
        });

    },
    GetPurchaseBillDetailByItem: function (code, qty) {
        debugger
        ajaxHelper.ajaxGetJson('/StockTransfer/GetStockTransferDetailByItem?code=' + code + '&qty=' + qty, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    console.log("In else")
                    empr_StockTransfer.rowsCount += data.data.length;
                    const gridInstance = $('#StockDetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    $('#StockDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                        dataSource = $('#StockDetailContainer').dxDataGrid('instance').option("dataSource");
                    });
                    console.log(data.data);
                    if ((dataSource[0].dT_CODE == undefined || dataSource[0].dT_CODE == 0) && (dataSource[0].iteM_CODE == "" || dataSource[0].iteM_CODE == null || dataSource[0].iteM_CODE == undefined)) {
                        empr_StockTransfer.CreateGrid(data.data);
                    } else {
                        dataSource.unshift(...data.data);
                        gridInstance.option("dataSource", dataSource);
                        gridInstance.refresh();
                    }
                }
                //empr_StockTransfer.CreateGrid(data.data);
                $('.bs-example-modal-lg').modal('hide');
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
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
        //        var response = empr_StockTransfer.GetGridData();
        //        response.then((data) => {
        //            if (data.length > 0) {
        //                data.forEach(obj => {
        //                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
        //                    obj.FBCODE = $("#BCODE").dxSelectBox('option', 'value');
        //                    obj.TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
        //                });
        //                console.log('Final Object before Generating Sticker');
        //                console.log(data);
        //                empr_StockTransfer.GetReport(data);
        //            }
        //        });
        //    });
        //}
        //else {
        //    var response = empr_StockTransfer.GetGridData();
        //    response.then((data) => {
        //        if (data.length > 0) {
        //            data.forEach(obj => {
        //                obj.VOUCHER_NO = $("#VOUCHER_NO").val();
        //                obj.FBCODE = $("#BCODE").dxSelectBox('option', 'value');
        //                obj.TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
        //            });
        //            console.log('Final Object before Generating Sticker');
        //            console.log(data);
        //            empr_StockTransfer.GetReport(data);
        //        }
        //    });
        //}

        var response = empr_StockTransfer.GetGridData();
        response.then((data) => {
            if (data.length > 0) {
                data.forEach(obj => {
                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                    obj.FBCODE = $("#BCODE").dxSelectBox('option', 'value');
                    obj.TBCODE = $("#TBCODE").dxSelectBox('option', 'value');
                });
                console.log('Final Object before Generating Sticker');
                console.log(data);
                empr_StockTransfer.GetReport(data);
            }
        });

    },
    GetGridData: async function () {
        //return await $('#StockDetailContainer').dxDataGrid('instance').option('dataSource');
        return await $('#GenerateCartonStickerGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    },
    GetReport: function (dataModel) {
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, '/StockTransfer/GetReport', function (data) {
            if (data.msgType == 1) {
                var codes = dataModel.map(obj => `'${obj.dT_CODE}'`).join(', ');
                empr_StockTransfer.UpdatePrintStatus(codes);
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
        //var serializedData = JSON.stringify(dataModel);
        //$.ajax({
        //    url: "/StockTransfer/GetPrintReport",
        //    type: 'POST',
        //    datatype: 'json',
        //    contentType: 'application/json; charset=utf-8',
        //    cache: false,
        //    data: { model: serializedData },
        //    success: function (response) {
        //        if (response.msgType == 1) {
        //            $('#ModalBody').empty();
        //            setTimeout(function () {
        //                $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + response.data + "' width='1100' height='500'></object></center>");
        //                $('#ShowReportModal').show();
        //                $('#ShowReportModal').modal('show');
        //            }, 100);
        //        }
        //        else {
        //            empr_helper.notify(response.msg, response.msgType);
        //        }
        //    },
        //    error: function (xhr, status, error) {
        //         //Handle errors
        //    }
        //});
    },
    InitCartonStickerGrid: function () {
        ajaxHelper.ajaxGetJson('/StockTransfer/GetStockTransferDetailByCode?code=' + $('#Code').val(), function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#GenerateCartonStickerGridContainer').data('dxDataGrid') != undefined) {
                        $('#GenerateCartonStickerGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_StockTransfer.CreateCartonStickerGrid(data.data);
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
        var data = empr_StockTransfer.GetDataToSave();
        if (data.Master.V_DATE == "" || data.Master.V_DATE == null || data.Master.V_DATE == undefined) {
            empr_helper.notify("Please select the voucher date first.", 2);
        }
        if (data.Master.TBCODE == "" || data.Master.TBCODE == null || data.Master.TBCODE == undefined) {
            empr_helper.notify("Please select the branch to.", 2);
        }
        else {
            empr_StockTransfer.GetSodaBookFeedingDetailBySodaDate(data.Master.V_DATE, data.Master.TBCODE);
        }
    },
    GetSodaBookFeedingDetailBySodaDate: function (sodaDate, branchTo) {
        ajaxHelper.ajaxPostJsonData({ sodaDate: sodaDate, branchTo: branchTo }, '/StockTransfer/GetSodaBookFeedingDetailBySodaDate', function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_StockTransfer.CreateSodaPickGrid(data.data);
                    $('#SodaPickModal').modal('show');
                } else {
                    empr_helper.notify("No soda found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateSodaPickGrid: function (dataSrc) {
        console.log(dataSrc)
        var col = [
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'grouP_NAME', caption: 'Group', allowEditing: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
            { dataField: 'qty', caption: 'S. QTY', allowEditing: false, },
            { dataField: 'dqty', caption: 'I. QTY', allowEditing: false, },
            { dataField: 'baL_QTY', caption: 'Balance Quantity', allowEditing: false, },
        ];
        empr_helper.dxGridbindingVouchers('#SodaPickGridContainer', col, dataSrc, "StockTransferSodaFeedingDetails", 'multiple');
        //empr_helper.editableDxGridbindingForTransactions('#SodaPickGridContainer', col, dataSrc, "SodaFeedingDetails", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            empr_StockTransfer.IsPickData = true;
            empr_StockTransfer.CreateGrid([{ __KEY__: empr_StockTransfer.GenerateKey(36), qtY2: 1 }]);
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_StockTransfer.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#StockDetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    selectedSodas = empr_StockTransfer.SetData(selectedSodas);
                    var finalData = existingData.concat(selectedSodas);
                    $('#StockDetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    selectedSodas = empr_StockTransfer.SetData(selectedSodas);
                    $('#StockDetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            empr_StockTransfer.IsPickData = true;
            empr_StockTransfer.CreateGrid([{ __KEY__: empr_StockTransfer.GenerateKey(36), qtY2: 1 }]);
            var data = empr_StockTransfer.GetDataToSave();
            var IsDataAvailableInGrid = false;

            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#StockDetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                selectedSodas = empr_StockTransfer.SetData(selectedSodas);
                var finalData = existingData.concat(selectedSodas);
                $('#StockDetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                selectedSodas = empr_StockTransfer.SetData(selectedSodas);
                $('#StockDetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
            }
            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },
    SetData: function (dataSource) {
        $.each(dataSource, function (index, item) {
            if (Type != "I") {
                item.iteM_CODE = item.barcodE_CODE;
            }
            item.dT_CODE = 0;
            item.__KEY__ = empr_StockTransfer.GenerateKey(36);
        });

        return dataSource;
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

    PrepareDataForValidation: function () {

        if ($('#ItemPrintContainer').dxDataGrid('instance').hasEditData()) {
            $('#ItemPrintContainer').dxDataGrid('instance').saveEditData().done(function () {
                //var response = empr_BarcodePrint.GetGridData();
                //response.then((data) => {
                //    debugger;
                     var data = $('#ItemPrintContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //var result = empr_StockTransfer.ValidateMainInfo(data);
                    //return result;
                    //if (result) {
                        empr_StockTransfer.GetBarcodePrint(data);
                    //}
                //});
            });
        }
        else {
            var data = $('#ItemPrintContainer').dxDataGrid('instance').getSelectedRowKeys();
            var result = empr_StockTransfer.ValidateMainInfo(data);
            console.log(result);
            //return result;
            if (result) {
                empr_StockTransfer.GetBarcodePrint(data);
            }
            //return empr_BarcodePrint.ValidateMainInfo($('#gridContainer').dxDataGrid('instance').getSelectedRowKeys());
        }
    },
    GetBarcodePrint: function (dataModel) {

        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        console.log(MD_ID);
        debugger;
        //let REPORT_NAME = empr_StockTransfer.reportTypes.filter(x => x.mD_ID == MD_ID)[0];
        let REPORT_NAME ="BarcodeSticker";
        console.log(REPORT_NAME);
        var dataModel = {
            data: dataModel,
            MD_ID: MD_ID,
            REPORT_NAME: REPORT_NAME
            //REPORT_NAME: REPORT_NAME.reporT_NAME,
        }
        if (!dataModel.data || dataModel.data.length === 0) {

            empr_helper.notify("Please Select Barcode First", 2);
            return;
        }
        
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, "/StockTransfer/GetBarcodeReport", function (data) {
            if (data.msgType == 1) {
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody1').empty();
                setTimeout(function () {
                    $('#ModalBody1').html(`<center><object data="${url}" width="1100" height="600"></object></center>`);
                    $('#ShowReportModalForSticker').show();
                    //ShowReportModalForSticker
                    $('#ShowReportModalForSticker').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);


        //ajaxHelper.ajaxPostJsonData({ data: dataModel }, '/BarcodePrint/GetBarcodeReport', function (data) {
        //    if (data.msgType == 1) {
        //        $('#ModalBody').empty();
        //        setTimeout(function () {
        //            $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
        //            $('#ShowReportModal').show();
        //            $('#ShowReportModal').modal('show');
        //        }, 1000);
        //    }
        //    else {
        //        empr_helper.notify(data.msg, data.msgType);
        //    }
        //}, false, true);
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