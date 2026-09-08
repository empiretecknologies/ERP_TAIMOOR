var empr_StockAdjustment = {
    totalCount: 0,
    rowsCount: 0,
    BLABEL: '',
    IsBLabelValid: true,
    IsPickData: false,
    InitEvents: function () {
        $(document).ready(function () {
            console.log('Items', Items);
            empr_StockAdjustment.InitQuickSearchGrid();

            empr_StockAdjustment.ResetForm();
            empr_StockAdjustment.InitItemIds();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_StockAdjustment.GetStockAdjustmentByCode(data.traN_ID);
                }
            });
            $('.chkCell').prop('checked', true);
            $('body').on('click', '.elm_edit', function () {
                empr_StockAdjustment.ResetForm();
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_StockAdjustment.GetStockAdjustmentByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/StockAdjustment/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_StockAdjustment.GetStockAdjustmentByCode(data.data.code);
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
                            if (empr_StockAdjustment.ValidateMainInfo()) {
                                empr_StockAdjustment.Save();
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
                        if (empr_StockAdjustment.ValidateMainInfo()) {
                            empr_StockAdjustment.Save();
                        }
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);
                    }, 200);
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_StockAdjustment.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_StockAdjustment.ResetForm();
                //$('#REF').focus();
                $('#ButtonsDiv').hide();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_StockAdjustment.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnGenerateCartonSticker', function () {
                empr_StockAdjustment.InitCartonStickerGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_StockAdjustment.GeneratePrintReport();
            });

            $('body').on('click', '#BtnGenerateSticker', function () {
                empr_StockAdjustment.GenerteCartonSticker();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_StockAdjustment.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_StockAdjustment.AddSodaToDelivery();
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
                            empr_StockAdjustment.GetPurchaseBillDetailByItem(itemId, itemQty);
                        }
                    } else {
                        empr_StockAdjustment.GetPurchaseBillDetailByItem(itemId, itemQty);
                    }
                } else {
                    empr_helper.notify("Please fill all fields.", 2);
                }
            });

            $('body').on('click', '#BtnAddBarcodes', function () {
                var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedBarcodes.length > 0) {
                    empr_StockAdjustment.AddBarcodeToGrid();
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
                empr_StockAdjustment.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_ADD && $('#BtnSodaPick').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }

            //$("#BARCODE").on("keydown", function (e) {
            //    if (e.key === "Enter") {
            //        e.preventDefault();
            //        e.stopImmediatePropagation();

            //        var inputVal = $(this).val().trim();
            //        if (inputVal === "") return;

            //        var matches = Items.filter(u =>
            //            u.value == inputVal || u.value.endsWith(inputVal)
            //        );

            //        if (matches.length > 0) {
            //            let selectedItem;

            //            // ✅ Check if numeric (pure digits only)
            //            if (/^\d+$/.test(matches[0].value)) {
            //                if (matches.length > 1) {
            //                    // multiple numeric → lowest lo
            //                    matches.sort((a, b) => parseInt(a.value) - parseInt(b.value));
            //                }
            //                selectedItem = matches[0];
            //            } else {
            //                // ✅ Prefix case (like zoj-000001)
            //                if (matches.length === 1) {
            //                    selectedItem = matches[0];
            //                } else {
            //                    empr_helper.notify("Multiple matches, please type full barcode", 2);
            //                    setTimeout(() => { $("#BARCODE").focus(); }, 100);
            //                    return;
            //                }
            //            }

            //            // ✅ Row add & values set
            //            empr_StockAdjustment.AddRow();
            //            var grid = $("#StockAdjustmentDetailContainer").dxDataGrid("instance");
            //            var rowIndex = 0;

            //            grid.cellValue(rowIndex, "iteM_CODE", selectedItem.key);
            //            grid.cellValue(rowIndex, "qty", 1);

            //            setTimeout(() => { $("#BARCODE").val("").focus(); }, 100);

            //        } else {
            //            empr_helper.notify("Item not found", 2);
            //            setTimeout(() => { $("#BARCODE").focus(); }, 100);
            //        }
            //    }
            //});

            $("#BARCODE").on("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    empr_StockAdjustment.processBarcode();
                }
            });

            $("#barcodeBtn").on("click", function () {
                empr_StockAdjustment.processBarcode();
            });



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
            empr_StockAdjustment.AddRow();
            var grid = $("#StockAdjustmentDetailContainer").dxDataGrid("instance");
            var rowIndex = 0;

            grid.cellValue(rowIndex, "iteM_CODE", selectedItem.key);
            grid.cellValue(rowIndex, "qty", 1);

            setTimeout(() => { $("#BARCODE").val("").focus(); }, 100);

        } else {
            empr_helper.notify("Item not found", 2);
            setTimeout(() => { $("#BARCODE").focus(); }, 100);
        }
    },
    //AddBarcodeToGrid: function () {
    //    debugger;
    //    const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
    //    var dataSource = gridInstance.option("dataSource");
    //    $('#StockAdjustmentDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
    //        dataSource = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option("dataSource");
    //    });
    //    var IsDataAvailableInGrid = false;
    //    $.each(dataSource, function (index, item) {
    //        if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
    //            IsDataAvailableInGrid = true;
    //        }
    //    });

    //    if (IsDataAvailableInGrid) {
    //        var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //        selectedBarcodes = empr_StockAdjustment.SetData(selectedBarcodes);
    //        dataSource.unshift(...selectedBarcodes);
    //        gridInstance.option("dataSource", dataSource);
    //        gridInstance.refresh();
    //        empr_helper.MoveFocusToGridWithouTab('#StockAdjustmentDetailContainer', 0, 'iteM_CODE')
    //    }
    //    else {
    //        var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //        selectedBarcodes = empr_StockAdjustment.SetData(selectedBarcodes);
    //        $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource', selectedBarcodes);
    //    }
    //    $('#BarcodePickModal').modal('hide');
    //    // yaha modal close
    //    //$('.modal').hide();
    //    $('#V_DATE').focus();
    //},
    AddBarcodeToGrid: function () {
        debugger;
        const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
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
            selectedBarcodes = empr_StockAdjustment.SetData(selectedBarcodes);

            if (IsDataAvailableInGrid) {
                dataSource.unshift(...selectedBarcodes);
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                //empr_helper.MoveFocusToGridWithouTab('#StockAdjustmentDetailContainer', 0, 'iteM_CODE');
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
    GetPurchaseBillDetailByItem: function (code, qty) {
        ajaxHelper.ajaxGetJson('/StockAdjustment/GetStockAdjustmentDetailByItem?code=' + code + '&qty=' + qty, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_StockAdjustment.rowsCount += data.data.length;
                    const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    $('#StockAdjustmentDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                        dataSource = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option("dataSource");
                    });
                    console.log(dataSource[0].dT_CODE);
                    if ((dataSource[0].dT_CODE == undefined || dataSource[0].dT_CODE == 0) && (dataSource[0].iteM_CODE == "" || dataSource[0].iteM_CODE == null || dataSource[0].iteM_CODE == undefined)) {

                        empr_StockAdjustment.CreateGrid(data.data);
                    } else {
                        dataSource.unshift(...data.data);
                        gridInstance.option("dataSource", dataSource);
                        gridInstance.refresh();
                        empr_helper.MoveFocusToGridWithouTab('#StockAdjustmentDetailContainer', 0, 'iteM_CODE')
                    }
                }
                $('.bs-example-modal-lg').modal('hide');
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    InitItemIds: function () {
        debugger;
        empr_StockAdjustment.bindDxDdl("PICK_ITEM", ItemIds, null, "key", "itemId", "Select", function (d) {
            console.log(d.value)
            if (d.value == null || d.value == '') {
                $('#itemIdHidden').val('');
            }
            else {
                $('#itemIdHidden').val(d.value)
            }
        });

    },
    CreateGrid: function (dataSrc) {
        console.log('CreateGrid',dataSrc)
        empr_StockAdjustment.IsPickData = dataSrc.some(item => item.grouP_NAME !== "");
        if (dataSrc.length > 0) {
            empr_StockAdjustment.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_StockAdjustment.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_StockAdjustment.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_StockAdjustment.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_StockAdjustment.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_StockAdjustment.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_StockAdjustment.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                           <a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_StockAdjustment.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>
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
                /*{
                    dataField: 'grouP_NAME',
                    caption: 'Group',
                    allowEditing: false,
                    visible: empr_StockAdjustment.IsPickData,
                },*/
             
                {
                    dataField: 'iteM_CODE',
                    caption: Caption,
                    width: 320,
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
                    onValueChanged: function (e) {
                        var selectedItemKey = e.value;
                        $.ajax({
                            url: 'StockAdjustment/GetItemId',
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
                                if (empr_StockAdjustment.BLABEL == '') {
                                    empr_StockAdjustment.BLABEL = selectedItem[0].blabel;
                                }
                                newData.rate = selectedItem[0].rate;
                                newData.unit = selectedItem[0].unit;
                                newData.blabel = selectedItem[0].blabel;
                                newData.iteM_ID = selectedItem[0].itemID;
                            }
                        }
                    }
                },
              
                {
                    dataField: 'qty',
                    caption: 'Qty',
                    width: 75,
                    setCellValue: function (newData, value, currentRowData) {
                        newData.qty = value;
                        var qty = parseFloat(newData.qty) || 0;
                        var qtY2 = parseFloat(currentRowData.qtY2) || 0;
                        var rate = parseFloat(currentRowData.rate) || 0;
                        if (isNaN(qty)) {
                            empr_helper.notify("Please enter the correct quantity.", 2);
                        }
                        if (isNaN(qtY2)) {
                            empr_helper.notify("Please enter the correct quantity2.", 2);
                        }
                        if (!isNaN(qty) && !isNaN(qtY2)) {
                            if (currentRowData.chK1) {
                                newData.baL_QTY = qty + qtY2;
                                newData.amt = qty * rate;
                            }
                            else {
                                newData.baL_QTY = qty;
                                

                            }
                        }
                        newData.amt = qty * rate;

                    }
                },
                {
                    dataField: 'unit',
                    width: 100,
                    caption: 'Unit',
                    lookup: {
                        dataSource: Units,
                        displayExpr: 'value',
                        valueExpr: 'key'
                    }
                },
                {
                    dataField: 'qtY2',
                    caption: 'Qty 2',
                    width: 75,
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


                                var gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
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
                    dataField: 'rate',
                    caption: 'Rate',
                    width: 80,
                    //allowEditing: false,
                    visible: true,
                    setCellValue: function (newData, value, currentRowData) {
                        newData.rate = value;
                        var rate = parseFloat(newData.rate) || 0;
                        var qty = parseFloat(currentRowData.qty) || 0;
                        if (!isNaN(qty) && !isNaN(rate)) {
                            newData.amt = (qty * rate).toFixed(2);
                        } else {
                            newData.amt = 0;
                        }

                    }
                    //calculateCellValue: function (rowData) {
                    //    if (rowData.iteM_CODE != '' && rowData.iteM_CODE != 0) {
                    //        var selectedItem = Items.filter(u => u.key == rowData.iteM_CODE);
                    //        if (selectedItem.length > 0) {
                    //            var qty = rowData.qty;
                    //            var rate = selectedItem[0].rate;
                    //            var calc = qty * rate;
                    //            rowData.amt = qty * rate;
                    //            return rate;
                    //        }
                    //    } else {
                    //        rowData.amt = 0;
                    //        return 0;
                    //    }
                    //}
                },
                {
                    dataField: 'amt',
                    caption: 'Amount',
                    allowEditing: false,
                    width: 150,

                },
                {
                    dataField: 'dT_DESC',
                    caption: 'Description',
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
                {
                    dataField: 'iteM_ID',
                    caption: 'Item',
                    visible:false,
                    allowEditing: false,
                }
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
                    onValueChanged: function (e) {
                        var selectedItemKey = e.value;
                        $.ajax({
                            url: 'StockAdjustment/GetItemId',
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
                                if (empr_StockAdjustment.BLABEL == '') {
                                    empr_StockAdjustment.BLABEL = selectedItem[0].blabel;
                                }
                                newData.blabel = selectedItem[0].blabel;
                                newData.iteM_ID = selectedItem[0].itemID;
                                newData.color = selectedItem[0].color;
                                newData.size = selectedItem[0].size;
                            }
                        }
                    }
                },
                {
                    dataField: 'qty',
                    width: 75,
                    caption: 'Qty',
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
                    caption: 'Qty 2',
                    width: 75,
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


                                var gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
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
                    //setCellValue: function (newData, value, currentRowData) {
                    //    console.log(value)
                    //    if (value == 0 || value == null) {
                    //        var selectedItem = Items.filter(u => u.key == currentRowData.iteM_CODE);
                    //        if (selectedItem.length > 0) {
                    //            newData.color = selectedItem[0].color;
                    //            newData.size = selectedItem[0].size;
                    //        }
                    //    }
                    //}
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
        empr_helper.editableDxGridbindingForTransactionsVouchers('#StockAdjustmentDetailContainer', col, dataSrc, "StockAdjustment", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#StockAdjustmentDetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#StockAdjustmentDetailContainer').dxDataGrid('instance').saveEditData();
            });
        }
    },
    CloneRow: function (index) {
        ;
        if ($('#StockAdjustmentDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockAdjustmentDetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_StockAdjustment.rowsCount += 1;
                const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
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
                    clonedRowData.__KEY__ = empr_StockAdjustment.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            });
        }
        else {
            empr_StockAdjustment.rowsCount += 1;
            const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
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
                clonedRowData.__KEY__ = empr_StockAdjustment.GenerateKey(36);
                clonedRowData.dT_CODE = 0;
                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
            }
        }
    },
    AddRow: function () {
        ;
        if ($('#StockAdjustmentDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockAdjustmentDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_StockAdjustment.rowsCount += 1;
                const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");
                if (dataSource.length >= Limit && Limit != 0) {
                    empr_helper.notify("You can only add  " + Limit + " records.", 2);
                    return;
                }
                if (Type == "B") {
                    dataSource.unshift({ __KEY__: empr_StockAdjustment.GenerateKey(36), qty: 1, qtY2: 1, baL_QTY: 1, dT_CODE: 0 });
                }
                else {
                    dataSource.unshift({ __KEY__: empr_StockAdjustment.GenerateKey(36), qtY2: 1, dT_CODE: 0 });
                }
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_StockAdjustment.rowsCount += 1;
            const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");
            if (dataSource.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if (Type == "B") {
                dataSource.unshift({ __KEY__: empr_StockAdjustment.GenerateKey(36), qty: 1, qtY2: 1, baL_QTY: 1, dT_CODE: 0 });
            }
            else {
                dataSource.unshift({ __KEY__: empr_StockAdjustment.GenerateKey(36), qtY2: 1, dT_CODE: 0 });
            }
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    DeleteRow: function (index) {
        ;
        const gridInstance = $('#StockAdjustmentDetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_StockAdjustment.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/StockAdjustment/DeleteStockAdjustmentDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_StockAdjustment.rowsCount -= 1;
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
        empr_StockAdjustment.GetStockAdjustment();
    },
    GetStockAdjustment: function () {
        ajaxHelper.ajaxGetJson('/StockAdjustment/GetStockAdjustment', function (data) {
            if (data.msgType == 1) {
                empr_StockAdjustment.CreateQuickSearchGrid(data.data);
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
        { dataField: 'am', caption: 'Type', },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "StockAdjustmentQS");
    },
    GetStockAdjustmentByCode: function (code) {
        ajaxHelper.ajaxGetJson('/StockAdjustment/GetStockAdjustmentByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#STYPE').dxSelectBox('instance').option('value', response.am);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#PERIOD_ID').val(response.perioD_ID);
                    //$('#TPERIOD_ID').val(response.tperioD_ID);
                    //$('#BtnDelete').show();
                    $('#ButtonsDiv').show();
                    //$('#BtnPrint').show();
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
                        //$('#ButtonsDiv').show();
                    }
                    $("#STYPE").dxSelectBox("instance").option("disabled", true);
                }

                if (data.detail.msgType == 1) {
                    empr_StockAdjustment.CreateGrid(data.detail.data);
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
    GetStockAdjustmentDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/StockAdjustment/GetStockAdjustmentDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_StockAdjustment.CreateGrid(data.data);
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
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var AM = $('#STYPE').dxSelectBox('option', 'value');
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            REF: REF,
            REMARKS: REMARKS,
            AM: AM,
            ASTATUS: ASTATUS
        }
        var detailRecords = [];
        if ($('#StockAdjustmentDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#StockAdjustmentDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option("dataSource");
        }
        if (empr_StockAdjustment.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_StockAdjustment.GetDataToSave();

        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }
        if (data.Master.AM == '') {
            empr_helper.notify("Please select stock type", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option("dataSource");

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

        //if (!empr_StockAdjustment.IsBLabelValid) {
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
        var dataModel = empr_StockAdjustment.GetDataToSave();
        var checkBLABEL = empr_StockAdjustment.FindNonMatchingIndexes(dataModel.Detail);
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/StockAdjustment/Save", function (data) {
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
                empr_StockAdjustment.GetStockAdjustmentDetailByCode($('#Code').val());
                $('#BtnDelete').show();
                //$('#ButtonsDiv').show();
                $('#V_DATE').focus();
            }
        }, false, true);
    },
    ResetForm: function () {
        $("#STYPE").dxSelectBox("instance").option("disabled", false);
        empr_StockAdjustment.IsPickData = false;
        empr_StockAdjustment.CreateGrid([{ __KEY__: empr_StockAdjustment.GenerateKey(36), qtY2: 1, dT_CODE: 0 }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('#STYPE').dxSelectBox('instance').option('value', 'I');
        empr_StockAdjustment.InitReportTypeDDL();
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/StockAdjustment/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_StockAdjustment.ResetForm();
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
        var response = empr_StockAdjustment.GetGridData();
        response.then((data) => {
            if (data.length > 0) {
                data.forEach(obj => {
                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                });
                empr_StockAdjustment.GetReport(data);
            }
        });
    },
    GetGridData: async function () {
        //return await $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource');
        return await $('#GenerateCartonStickerGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    },
    GetReport: function (dataModel) {
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, '/StockAdjustment/GetReport', function (data) {
            if (data.msgType == 1) {
                var codes = dataModel.map(obj => `'${obj.dT_CODE}'`).join(', ');
                console.log(dataModel);
                console.log(codes);
                empr_StockAdjustment.UpdatePrintStatus(codes);
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='500'></object></center>");
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
        empr_StockAdjustment.InitReportTypeDDL();
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/StockAdjustment/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='500'></object></center>");
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
        //    url: "/StockAdjustment/GetPrintReport",
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
        ajaxHelper.ajaxGetJson('/StockAdjustment/GetStockAdjustmentDetailByCode?code=' + $('#Code').val(), function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#GenerateCartonStickerGridContainer').data('dxDataGrid') != undefined) {
                        $('#GenerateCartonStickerGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_StockAdjustment.CreateCartonStickerGrid(data.data);
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
        empr_helper.editableDxGridbindingForTransactions('#GenerateCartonStickerGridContainer', col, dataSrc, "StockAdjustmentDetails", "iteM_CODE", 'multiple');
        setTimeout(function () {
            $('#GenerateCartonStickerGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    UpdatePrintStatus: function (codes) {
        ajaxHelper.ajaxPostJsonData({ codes: codes }, '/StockAdjustment/UpdatePrintStatus', function (data) { }, false, true);
    },
    InitSodaPickGrid: function () {
        //var data = empr_StockAdjustment.GetDataToSave();
        //if (data.Master.V_DATE == "" || data.Master.V_DATE == null || data.Master.V_DATE == undefined) {
        //    empr_helper.notify("Please select the voucher date first.", 2);
        //}
        //if (data.Master.TBCODE == "" || data.Master.TBCODE == null || data.Master.TBCODE == undefined) {
        //    empr_helper.notify("Please select the branch to.", 2);
        //}
        //else {
        //    empr_StockAdjustment.GetSodaBookFeedingDetailBySodaDate(data.Master.V_DATE, data.Master.TBCODE);
        //}
    },
    GetSodaBookFeedingDetailBySodaDate: function (sodaDate, branchTo) {
        //ajaxHelper.ajaxPostJsonData({ sodaDate: sodaDate, branchTo: branchTo }, '/StockAdjustment/GetSodaBookFeedingDetailBySodaDate', function (data) {
        //    if (data.msgType == 1) {
        //        if (data.data.length > 0) {
        //            if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
        //                $('#SodaPickGridContainer').data('dxDataGrid').dispose();
        //            }
        //            empr_StockAdjustment.CreateSodaPickGrid(data.data);
        //            $('#SodaPickModal').modal('show');
        //        } else {
        //            empr_helper.notify("No soda found.", 2);
        //        }
        //    }
        //    else {
        //        empr_helper.notify(data.msg, data.msgType);
        //    }
        //}, false, true);
    },
    CreateSodaPickGrid: function (dataSrc) {
        //var col = [
        //    { dataField: 'traN_ID', caption: 'Code', visible: false, },
        //    { dataField: 'grouP_NAME', caption: 'Group', allowEditing: false, },
        //    { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
        //    { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
        //    { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
        //    { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
        //    { dataField: 'qty', caption: 'S. QTY', allowEditing: false, },
        //    { dataField: 'dqty', caption: 'I. QTY', allowEditing: false, },
        //    { dataField: 'baL_QTY', caption: 'Balance Quantity', allowEditing: false, },
        //];
        //empr_helper.dxGridbinding('#SodaPickGridContainer', col, dataSrc, "SodaFeedingDetails", 'multiple');
        //setTimeout(function () {
        //    $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        //}, 500);
    },
    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            empr_StockAdjustment.IsPickData = true;
            empr_StockAdjustment.CreateGrid([{ __KEY__: empr_StockAdjustment.GenerateKey(36), qtY2: 1 }]);
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_StockAdjustment.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    selectedSodas = empr_StockAdjustment.SetData(selectedSodas);
                    var finalData = existingData.concat(selectedSodas);
                    $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    selectedSodas = empr_StockAdjustment.SetData(selectedSodas);
                    $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            empr_StockAdjustment.IsPickData = true;
            empr_StockAdjustment.CreateGrid([{ __KEY__: empr_StockAdjustment.GenerateKey(36), qtY2: 1 }]);
            var data = empr_StockAdjustment.GetDataToSave();
            var IsDataAvailableInGrid = false;

            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                selectedSodas = empr_StockAdjustment.SetData(selectedSodas);
                var finalData = existingData.concat(selectedSodas);
                $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                selectedSodas = empr_StockAdjustment.SetData(selectedSodas);
                $('#StockAdjustmentDetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
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
            item.__KEY__ = empr_StockAdjustment.GenerateKey(36);
        });
        return dataSource;
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/StockAdjustment/GetReportTypes", function (data) {
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
    // modal Open
    InitBarcodePickGrid: function () {
        ajaxHelper.ajaxGetJson('/StockAdjustment/GetBarcodeList', function (data) {
            console.log(data)
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_StockAdjustment.CreateBarcodeGrid(data.data);
                    $('#BarcodePickModal').modal('show');
                    //$('#BarcodePickModal').css('display', 'block');
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
            { dataField: 'iteM_ID', caption: 'Item', visible: true },
            { dataField: 'barcode', caption: 'Barcode', allowEditing: false },
            { dataField: 'sizE_NAME', caption: 'Size', allowEditing: false, width: 120 },
            { dataField: 'coloR_NAME', caption: 'Color', allowEditing: false, width: 120 },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, width: 120 },
        ];
        empr_helper.dxGridbindingVouchers('#BarcodePickGridContainer', col, dataSrc, "StockAdjustmentBarcodeGrid");
    },
}