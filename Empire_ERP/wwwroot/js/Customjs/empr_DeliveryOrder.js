$(document).keydown(function (e) {
    //Check Ctrl key is pressed and S key is pressed Save
    if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
        if (empr_DeliveryOrder.ValidateMainInfo()) {
            empr_DeliveryOrder.Save();
        }
        return false;
    }
    //Check Ctrl key is pressed and D key is pressed Delete
    if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
        e.preventDefault();
        if ($("#Code").val() != '') {
            empr_DeliveryOrder.Delete();
        } else {
            empr_helper.notify("Please select any record for delete..", 2);
        }
        return false;
    }
    //Check Alt key is pressed and R key is pressed Refresh
    if ((e.altKey || e.metaKey) && e.key === 'a') {
        e.preventDefault();
        empr_DeliveryOrder.ResetForm();
        return false;
    }
    //Check Ctrl key is pressed and f key is pressed Show Modal
    if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
        e.preventDefault();
        $('.card .modal').modal('show');
        return false;
    }
    //Check Enter key is pressed Detail Row Add
    if (e.key === 'Enter') {
        e.preventDefault();
        return false;
    }
});
var empr_DeliveryOrder = {
    totalCount: 0,
    rowsCount: 0,
    pickIds: [],
    originalValues: {},
    //BtnSodaPick
    InitEvents: function () {
        $(document).ready(function () {
            empr_DeliveryOrder.InitQuickSearchGrid();
            empr_DeliveryOrder.ResetForm();
            empr_DeliveryOrder.InitPartyType();
            empr_DeliveryOrder.InitItemIds();
            empr_DeliveryOrder.InitReportTypeDDL();

            var Id = 0;
            empr_DeliveryOrder.InitSalesman(Id);
            empr_DeliveryOrder.InitDeliveryman(Id);
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_DeliveryOrder.GetDeliveryOrderByCode(data.traN_ID);
                }
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_DeliveryOrder.GeneratePrintReport();
            });

            $('.chkCell').prop('checked', true);

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_DeliveryOrder.GetDeliveryOrderByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/DeliveryOrder/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_DeliveryOrder.GetDeliveryOrderByCode(data.data.code);
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
                            if (empr_DeliveryOrder.ValidateMainInfo()) {
                                empr_DeliveryOrder.Save();
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
                        if (empr_DeliveryOrder.ValidateMainInfo()) {
                            empr_DeliveryOrder.Save();
                        }
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);
                    }, 200);
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_DeliveryOrder.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_DeliveryOrder.ResetForm();
                $('#REF').focus();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_DeliveryOrder.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_DeliveryOrder.GeneratePrintReport();
            });

            $('body').on('click', '#BtnGetItems', function () {
                var itemId = $("#itemIdHidden").val();
                var itemQty = $("#IQty").val();
                debugger
                if (itemId != '' && itemQty != '') {
                    if (Permissions != "Admin") {
                        if (!$("#Code").val() && !Permissions.r_ADD) {
                            empr_helper.notify("You are not allowed to add new record !", 2);
                        }
                        else {
                            empr_DeliveryOrder.GetDeliveryOrderDetailByItem(itemId, itemQty);
                        }
                    } else {
                        empr_DeliveryOrder.GetDeliveryOrderDetailByItem(itemId, itemQty);
                    }
                } else {
                    empr_helper.notify("Please fill all fields.", 2);
                }
            });

            $('body').on('click', '#BtnSodaPick', function () {
                 empr_DeliveryOrder.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddBarcodes', function () {
                var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedBarcodes.length > 0) {
                    empr_DeliveryOrder.AddBarcodeToGrid();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_DeliveryOrder.AddToDelivery();
                }
                else {
                    empr_helper.notify("Please select record first.", 2);
                }
            });

            $('body').on('click', '#BtnAddSodaDetailToDelivery', function () {
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_DeliveryOrder.AddSodaToDelivery();
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

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    InitSodaPickGrid: function () {
        //var data = empr_DeliveryOrder.GetDataToSave();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        console.log(PARTY_CODE);
        if (PARTY_CODE == "" || PARTY_CODE == null || PARTY_CODE == undefined || PARTY_CODE == 0) {
            empr_helper.notify("Please select party first.", 2);
        }
        else {
            empr_DeliveryOrder.GetPickDataByParty(PARTY_CODE, ACT_CODE);
        }
    },
    GetPickDataByParty: function (PARTY_CODE, ACT_CODE) {
        ajaxHelper.ajaxGetJson('/DeliveryOrder/GetPickDataByParty?partyCode=' + PARTY_CODE + '&actCode=' + ACT_CODE, function (data) {
            console.log(data)
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_DeliveryOrder.CreatePickGrid(data.data);
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
    InitBarcodePickGrid: function () {
        if (empr_DeliveryOrder.pickIds.length > 0) {
            empr_helper.notify("Cannot open on return data.", 2);
        } else {
            ajaxHelper.ajaxGetJson('/DeliveryOrder/GetBarcodeList', function (data) {
                console.log(data)
                if (data.msgType == 1) {
                    if (data.data.length > 0) {
                        debugger
                        //if ($('#BarcodePickGridContainer').data('dxDataGrid') != undefined) {
                        //    $('#BarcodePickGridContainer').data('dxDataGrid').dispose();
                        //}
                        empr_DeliveryOrder.CreateBarcodeGrid(data.data);
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
        }
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
        empr_helper.dxGridbindingVouchers('#BarcodePickGridContainer', col, dataSrc, "DeliveryOrderBarcodes");
        //setTimeout(function () {
        //    $('#BarcodePickGridContainer').dxDataGrid('instance').resize();
        //}, 500);
    },
    AddBarcodeToGrid: function () {
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
            dataSource = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        });
        var IsDataAvailableInGrid = false;
        $.each(dataSource, function (index, item) {
            if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                IsDataAvailableInGrid = true;
            }
        });

        if (IsDataAvailableInGrid) {
            var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
            selectedBarcodes = empr_DeliveryOrder.SetData(selectedBarcodes);
            console.log(selectedBarcodes);
            dataSource.unshift(...selectedBarcodes);
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
            empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
        }
        else {
            var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
            selectedBarcodes = empr_DeliveryOrder.SetData(selectedBarcodes);
            console.log(selectedBarcodes);
            $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedBarcodes);
        }

        $('.modal').hide();
        $('#V_DATE').focus();
    },
    SetData: function (dataSource) {
        $.each(dataSource, function (index, item) {
            if (Type != "I") {
                item.iteM_CODE = parseInt(item.barcodE_CODE);
            }
            item.dT_CODE = 0;
            item.__KEY__ = empr_DeliveryOrder.GenerateKey(36);
        });

        return dataSource;
    },
    CreatePickGrid: function (dataSrc) {
        var col = [
            { dataField: 'picK_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
            { dataField: 'partY_NAME', caption: 'Party', allowEditing: false, },
            { dataField: 'ref', caption: 'Ref', allowEditing: false, },
            { dataField: 'iteM_CODE', caption: 'Item', allowEditing: false, visible: false },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
            { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false, },
            { dataField: 'unit', caption: 'Unit', allowEditing: false, visible: false },
            { dataField: 'dT_DESC', caption: 'Description', allowEditing: false, },
            { dataField: 'qty', caption: 'Quantity', allowEditing: false, },
            { dataField: 'baL_QTY', caption: 'Quantity', allowEditing: false, visible: false, },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, },
            { dataField: 'amt', caption: 'Amount', allowEditing: false, },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "DeliveryOrderPick", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    CreatePickDetailGrid: function (dataSrc) {
        var existingdata = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
        var updatedData = dataSrc
            .map(item => {
                const matchingRows = existingdata.filter(row => row.picK_ID_D === item.picK_ID_D);
                const totalQty = matchingRows.reduce((sum, row) => parseInt(sum) + parseInt(row.qty), 0);
                console.log(totalQty);
                item.qty -= totalQty;
                console.log(item.qty);

                return item.qty > 0 ? item : null;
            })
            .filter(item => item !== null);
        empr_DeliveryOrder.originalValues = {};
        updatedData.forEach(row => {
            empr_DeliveryOrder.originalValues[row.picK_ID_D] = row.qty;
        });
        var col = [
            {
                caption: 'Item', allowEditing: false, width: 140,
                cellTemplate: function (container, options) {
                    const selectedIds = Items.filter(x => x.key === options.data.iteM_CODE);
                    const customText = selectedIds.length > 0 ? selectedIds[0].itemID : '';
                    container.text(customText);
                }
            },
            {
                dataField: 'size', caption: 'Size', allowEditing: false, width: 140,
                cellTemplate: function (container, options) {
                    const selectedSize = Sizes.filter(x => x.key === options.data.size);
                    const customText = selectedSize.length > 0 ? selectedSize[0].value : '';
                    container.text(customText);
                }
            },
            {
                dataField: 'color', caption: 'Color', allowEditing: false, width: 170,
                cellTemplate: function (container, options) {
                    const selectedColor = Colors.filter(x => x.key === options.data.color);
                    const customText = selectedColor.length > 0 ? selectedColor[0].value : '';
                    container.text(customText);
                }
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                allowSorting: false,
                allowFiltering: false,
                dataType: 'number',
                width: 90,
                setCellValue: function (newData, value, currentRowData) {
                    const originalQty = empr_DeliveryOrder.originalValues[currentRowData.picK_ID_D];
                    if (value <= originalQty) {
                        newData.qty = value;
                    } else {
                        newData.qty = originalQty;
                    }
                    var rate = parseFloat(currentRowData.rate) || 0;
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
                        if (!isNaN(rate)) {
                            newData.amt = (newData.baL_QTY * rate).toFixed(2);

                            var Amount = newData.amt;
                            var Dis = parseFloat(currentRowData.disc) || 0;
                            var disSum = Amount * Dis / 100 || 0;
                            var Adv = parseFloat(currentRowData.adv) || 0;
                            var Tax = parseFloat(currentRowData.tax) || 0;
                            var DiscountAmount = disSum;
                            var TaxAmount = Amount - DiscountAmount || 0;

                            var TaxSum = TaxAmount * Tax / 100 || 0;
                            var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

                            newData.disC_AMT = disSum.toFixed(2);
                            newData.taX_AMT = TaxSum.toFixed(2);
                            newData.adV_AMT = AdvSum.toFixed(2);

                            var NetAmount = Amount - DiscountAmount || 0;
                            if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                                newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                            } else {
                                newData.neT_AMT = 0;
                            }
                        }
                    }
                },
                calculateCellValue: function (data) {
                    const originalQty = empr_DeliveryOrder.originalValues[data.picK_ID_D];
                    if (data.qty > originalQty) {
                        data.qty = originalQty;
                    }
                    var rate = parseFloat(data.rate) || 0;
                    var qty = parseFloat(data.qty) || 0;
                    var qtY2 = parseFloat(data.qtY2) || 0;
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (data.chK1) {
                            data.baL_QTY = qty + qtY2;
                        }
                        else {
                            data.baL_QTY = qty;
                        }
                        if (!isNaN(rate)) {
                            data.amt = (data.baL_QTY * rate).toFixed(2);

                            var Amount = data.amt;
                            var Dis = parseFloat(data.disc) || 0;
                            var disSum = Amount * Dis / 100 || 0;
                            var Adv = parseFloat(data.adv) || 0;
                            var Tax = parseFloat(data.tax) || 0;
                            var DiscountAmount = disSum;
                            var TaxAmount = Amount - DiscountAmount || 0;

                            var TaxSum = TaxAmount * Tax / 100 || 0;
                            var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

                            data.disC_AMT = disSum.toFixed(2);
                            data.taX_AMT = TaxSum.toFixed(2);
                            data.adV_AMT = AdvSum.toFixed(2);

                            var NetAmount = Amount - DiscountAmount || 0;
                            if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                                data.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                            } else {
                                data.neT_AMT = 0;
                            }
                        }
                    }
                    return data.qty;
                }
            },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, dataType: 'number', width: 90, },
            {
                caption: 'Barcode', allowEditing: false,
                cellTemplate: function (container, options) {
                    const customText = Items.filter(x => x.key === options.data.iteM_CODE)[0].value;
                    container.text(customText);
                }
            },
            { dataField: 'dT_CODE', visible: false, },
            { dataField: 'iteM_CODE', visible: false, },
            { dataField: 'unit', visible: false, },
            { dataField: 'qtY2', visible: false, },
            { dataField: 'baL_QTY', visible: false, },
            { dataField: 'rate', visible: false, },
            { dataField: 'amt', visible: false, },
            { dataField: 'disc', visible: false, },
            { dataField: 'disC_AMT', visible: false, },
            { dataField: 'tax', visible: false, },
            { dataField: 'taX_AMT', visible: false, },
            { dataField: 'adv', visible: false, },
            { dataField: 'adV_AMT', visible: false, },
            { dataField: 'neT_AMT', caption: 'Net Amt', visible: true, },
            { dataField: 'dT_DESC', visible: false, },
            { dataField: 'color', visible: false, },
            { dataField: 'size', visible: false, },
            { dataField: 'grade', visible: false, },
            { dataField: 'warehouse', visible: false, },
            { dataField: 'deL_DATE', visible: false, },
            { dataField: 'duE_DATE', visible: false, },
            { dataField: 'duE_DAYS', visible: false, },
            { dataField: 'veh', visible: false, },
            { dataField: 'chk', visible: false, },
            { dataField: 'chK1', visible: false, },
            { dataField: 'picK_ID', visible: false, },
            { dataField: 'picK_ID_D', visible: false, },
        ];
        empr_helper.editableDxGridbinding('#SodaPickDetailGridContainer', col, updatedData, "SodaPickDetailGrid");
        setTimeout(function () {
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').refresh();
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    CreateGrid: function (dataSrc) {
        console.log('CreateGrid', dataSrc);
        dataSrc.forEach(item => {
            if (
                item.diS_DATE == '1900-01-01' || item.diS_DATE == '01-01-1900' || item.diS_DATE == '01-Jan-1900' || item.diS_DATE == '1/1/1900 12:00:00 AM' || item.diS_DATE == '01/01/1900 12:00:00 AM' || item.diS_DATE == '1/1/1900' ||
                item.diS_DATE == '2000-01-01' || item.diS_DATE == '01-01-2000' || item.diS_DATE == '01-Jan-2000' || item.diS_DATE == '1/1/2000 12:00:00 AM' || item.diS_DATE == '01/01/2000 12:00:00 AM' || item.diS_DATE == '1/1/2000' ||
                item.diS_DATE == '00-01-01' || item.diS_DATE == '01-01-00' || item.diS_DATE == '01-Jan-00' || item.diS_DATE == '1/1/00 12:00:00 AM' || item.diS_DATE == '01/01/00 12:00:00 AM' || item.diS_DATE == '1/1/00'
            ) {
                item.diS_DATE = undefined;
            }
            console.log(item.diS_TIME)
            if (item.picK_ID_D > 0) {
                empr_DeliveryOrder.pickIds.push(item.picK_ID_D);
                $('#pickItems').hide();
            }
        });
        if (dataSrc.length > 0) {
            empr_DeliveryOrder.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_DeliveryOrder.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_DeliveryOrder.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_DeliveryOrder.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const searchAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_DeliveryOrder.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}${searchAction}</div>`;
                        $(actions).appendTo(container);
                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_DeliveryOrder.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_DeliveryOrder.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_DeliveryOrder.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                           <a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_DeliveryOrder.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>
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
                dataField: 'iteM_CODE',
                caption: Caption,
                width: Caption == 'Item' ? 320 : 150,
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
                    newData.iteM_CODE = value;
                    if (Type != "I") {
                        if (value != '') {
                            var selectedItem = Items.filter(u => u.key == value);
                            if (selectedItem.length > 0) {
                                newData.rate = selectedItem[0].wsale;
                                newData.iteM_ID = selectedItem[0].itemID;
                                newData.color = selectedItem[0].color;
                                newData.size = selectedItem[0].size;
                            }
                        }
                        else {
                            newData.rate = 0;
                        }
                    }
                }
            },
            {
                dataField: 'iteM_ID',
                caption: 'Item',
                disabled: true,
                allowEditing: false,
                visible: Type == "B",
                calculateCellValue: function (rowData) {
                    if (Type != "I") {
                        if (rowData.iteM_CODE) {
                            const selectedItem = Items.find(u => u.key === parseInt(rowData.iteM_CODE));
                            if (selectedItem) {
                                return selectedItem.itemID;
                            }
                        }
                        return null;
                    } else {
                        return null;
                    }
                }
            },
            {
                dataField: 'qty',
                caption: 'Qty',
                width: 75,
                alignment: 'right',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 0 },
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var rate = parseFloat(currentRowData.rate) || 0;
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
                        if (!isNaN(rate)) {
                            newData.amt = (newData.baL_QTY * rate).toFixed(2);

                            var Amount = newData.amt;
                            var Dis = parseFloat(currentRowData.disc) || 0;
                            var disSum = Amount * Dis / 100 || 0 ;
                            var Adv = parseFloat(currentRowData.adv) || 0;
                            var Tax = parseFloat(currentRowData.tax) || 0;
                            var DiscountAmount = disSum;
                            var TaxAmount = Amount - DiscountAmount || 0;

                            var TaxSum = TaxAmount * Tax / 100 || 0;
                            var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

                            newData.disC_AMT = disSum.toFixed(2);
                            newData.taX_AMT = TaxSum.toFixed(2);
                            newData.adV_AMT = AdvSum.toFixed(2);

                            var NetAmount = Amount - DiscountAmount || 0;
                            if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                                newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                            } else {
                                newData.neT_AMT = 0;
                            }
                        }
                    }
                }
            },
            {
                dataField: 'picK_ID',
                caption: 'Pick ID',
                visible: false
            },
            {
                dataField: 'picK_ID_D',
                caption: 'Pick Detail ID',
                visible: false
            },
        ];
        if (Type === "I") {
            col.push(
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
                    caption: 'Qty 2',
                    visible: CompCond == 2,
                    width: 75,
                    setCellValue: function (newData, value, currentRowData) {
                        newData.qtY2 = value;
                        var qty = parseFloat(currentRowData.qty) || 0;
                        var qtY2 = parseFloat(newData.qtY2) || 0;
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
                            }
                            else {
                                newData.baL_QTY = qty;
                            }

                            if (!isNaN(newData.baL_QTY) && !isNaN(rate)) {
                                newData.amt = (newData.baL_QTY * rate).toFixed(2);
                            } else {
                                newData.amt = 0;
                            }

                            var Amount = newData.amt || 0;
                            var Discount = parseFloat(currentRowData.disc) || 0;
                            var Tax = parseFloat(currentRowData.tax) || 0;
                            var Adv = parseFloat(currentRowData.adv) || 0;
                            var DiscountAmount = Amount * Discount / 100 || 0;
                            var TaxAmount = Amount - DiscountAmount;
                            var TaxSum = TaxAmount * Tax / 100 || 0
                            var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0
                            var NetAmount = Amount - DiscountAmount || 0;
                            if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                                newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                            } else {
                                newData.neT_AMT = 0;
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


                                var gridInstance = $('#DetailContainer').dxDataGrid('instance');
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
                    width: 400,
                    wordWrapEnabled: true,
                },
                {
                    dataField: 'warehouse',
                    caption: 'Warehouse',
                    lookup: {
                        dataSource: Warehouses,
                        displayExpr: 'value',
                        valueExpr: 'key'
                    }
                },
                {
                    dataField: 'diS_DATE',
                    caption: 'Dispatch Date',
                    dataType: 'date',
                },
                {
                    dataField: 'diS_TIME',
                    caption: 'Dispatch Time',
                    editorType: 'dxDateBox',
                    editorOptions: {
                        type: 'time',
                        displayFormat: 'hh:mm a', // 12-hour format with AM/PM
                        interval: 15,              // Minute step (default is 30, setting to 1 for fine granularity)
                        useMaskBehavior: true,
                        showDropDownButton: false
                    }
                },
                {
                    dataField: 'vehicle',
                    caption: 'Vehicle #',
                }
            );
        }
        else {
            col.push(
                {
                    dataField: 'qtY2',
                    caption: 'Qty 2',
                    visible: CompCond == 2,
                    width: 75,
                    setCellValue: function (newData, value, currentRowData) {
                        newData.qtY2 = value;
                        var qty = parseFloat(currentRowData.qty) || 0;
                        var qtY2 = parseFloat(newData.qtY2) || 0;
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
                            }
                            else {
                                newData.baL_QTY = qty;
                            }

                            if (!isNaN(newData.baL_QTY) && !isNaN(rate)) {
                                newData.amt = (newData.baL_QTY * rate).toFixed(2);
                            } else {
                                newData.amt = 0;
                            }

                            var Amount = newData.amt || 0;
                            var Discount = parseFloat(currentRowData.disc) || 0;
                            var Tax = parseFloat(currentRowData.tax) || 0;
                            var Adv = parseFloat(currentRowData.adv) || 0;
                            var DiscountAmount = Amount * Discount / 100 || 0;
                            var TaxAmount = Amount - DiscountAmount;
                            var TaxSum = TaxAmount * Tax / 100 || 0
                            var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0
                            var NetAmount = Amount - DiscountAmount || 0;
                            if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                                newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                            } else {
                                newData.neT_AMT = 0;
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


                                var gridInstance = $('#DetailContainer').dxDataGrid('instance');
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
                    wordWrapEnabled: true,
                },
                {
                    dataField: 'warehouse',
                    caption: 'Warehouse',
                    lookup: {
                        dataSource: Warehouses,
                        displayExpr: 'value',
                        valueExpr: 'key'
                    }
                },
                {
                    dataField: 'diS_DATE',
                    caption: 'Dispatch Date',
                    dataType: 'date',
                    format: 'dd-MM-yyyy',
                },
                {
                    dataField: 'diS_TIME',
                    caption: 'Dispatch Time',
                    dataType: 'string', // Treat as plain string (not Date)
                    editorType: 'dxDateBox',
                    editorOptions: {
                        type: 'time', // Time-only picker
                        displayFormat: 'hh:mm a', // Show in 12-hour format (e.g., "02:30 PM")
                        valueFormat: 'HH:mm', // Store as 24-hour string (e.g., "14:30")
                        pickerType: 'rollers', // Better UX for time selection
                        useMaskBehavior: true,
                        showDropDownButton: false,
                        onValueChanged: function (e) {
                            // Force the value to stay as HH:mm string
                            if (e.value && !(typeof e.value === 'string')) {
                                const date = new Date(e.value);
                                const hours = String(date.getHours()).padStart(2, '0');
                                const minutes = String(date.getMinutes()).padStart(2, '0');
                                e.component.option('value', `${hours}:${minutes}`);
                            }
                        }
                    }
                },
                {
                    dataField: 'vehicle',
                    caption: 'Vehicle #',
                }
            );
        }
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "DeliveryOrder", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        setTimeout(function () {
            var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
            $('#DetailContainer').dxDataGrid('instance').focus(nextElement);
        }, 1500);
    },
    CloneRow: function (index) {
        if (empr_DeliveryOrder.pickIds.length > 0) {
            empr_helper.notify("Cannot clone row on return data.", 2);
        } else {
            const gridIns = $('#DetailContainer').dxDataGrid('instance');
            const dataSrc = gridIns.option("dataSource");

            if (dataSrc.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
                $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                    empr_DeliveryOrder.rowsCount += 1;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    if (dataSource.length > 0) {
                        let clonedRowData = $.extend(true, {}, dataSource[index]);
                        if (clonedRowData.hasOwnProperty('dT_CODE')) {
                            delete clonedRowData.dT_CODE;
                        }
                        clonedRowData.__KEY__ = empr_DeliveryOrder.GenerateKey(36);
                        clonedRowData.dT_CODE = 0;
                        let newDataSource = [clonedRowData].concat(dataSource);
                        gridInstance.option("dataSource", newDataSource);
                        gridInstance.refresh();
                    }
                });
            }
            else {
                empr_DeliveryOrder.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_DeliveryOrder.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            }
        }
    },
    AddRow: function () {
        if (empr_DeliveryOrder.pickIds.length > 0) {
            empr_helper.notify("Cannot add row on return data.", 2);
        } else {
            const gridIns = $('#DetailContainer').dxDataGrid('instance');
            const dataSrc = gridIns.option("dataSource");

            if (dataSrc.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
                $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                    empr_DeliveryOrder.rowsCount += 1;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    const dataSource = gridInstance.option("dataSource");

                    dataSource.unshift({ __KEY__: empr_DeliveryOrder.GenerateKey(36), dT_CODE: 0 });
                    gridInstance.option("dataSource", dataSource);
                    gridInstance.refresh();
                    empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
                });
            }
            else {
                empr_DeliveryOrder.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_DeliveryOrder.GenerateKey(36), dT_CODE: 0 });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
            }
        }
    },
    DeleteRow: function (index, dtCode) {
        ;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_DeliveryOrder.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/DeliveryOrder/DeleteDeliveryOrderDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_DeliveryOrder.rowsCount -= 1;
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
        empr_DeliveryOrder.GetDeliveryOrder();
        //empr_DeliveryOrder.CreateQuickSearchGrid();
    },
    GetDeliveryOrder: function () {
        ajaxHelper.ajaxGetJson('/DeliveryOrder/GetDeliveryOrder', function (data) {
            if (data.msgType == 1) {
                empr_DeliveryOrder.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        console.log(dataSrc)
        var col = [];
        if (dataSrc.length > 0 ? dataSrc[0].amt != undefined : false) {
            col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin" && !Permissions.r_PRINT) {
                        $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
                               </div>`).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
                               <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.code} title="PRINT"><i class="fa fa-print"></i></a>
                               <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>
                               </div>`).appendTo(container);
                    }
                }
            },
            { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
            { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'partY_NAME', caption: 'Party Name', },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'remarks', caption: 'Description', },
            { dataField: 'amt', caption: 'Amount', },
            { dataField: 'astatus', caption: 'Status', },
            ];
        }
        else {
            col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {


                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
                               <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.code} title="PRINT"><i class="fa fa-print"></i></a>
                               <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>
                               </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
            { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'partY_NAME', caption: 'Party Name', },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'remarks', caption: 'Remarks', },
            { dataField: 'astatus', caption: 'Status', },
            ]
        }
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/DeliveryOrder/GetDeliveryOrder", "id", "PartyOpening");
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "DeliveryOrderQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    GetDeliveryOrderByCode: function (code) {
        ajaxHelper.ajaxGetJson('/DeliveryOrder/GetDeliveryOrderByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    $('#pickItems').show();
                    empr_DeliveryOrder.pickIds = [];
                    var response = masterData[0];
                    console.log(response);
                    var filteredData = $.grep(PartyType, function (item) {
                        return item.partyCode === response.partY_CODE && item.accountCode === response.acT_CODE.toString();
                    });
                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#HS_CODE').val(response.hS_CODE);
                    $('#COMM').val(response.comm);
                    $('#DISC').val(response.disc);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#hdnDOC').val(response.doc);
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
                }

                if (data.detail.msgType == 1) {
                    empr_DeliveryOrder.CreateGrid(data.detail.data);
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
    GetDeliveryOrderDetailByItem: function (code, qty) {
        ajaxHelper.ajaxGetJson('/DeliveryOrder/GetDeliveryOrderDetailByItem?code=' + code + '&qty=' + qty, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_DeliveryOrder.rowsCount += data.data.length;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                        dataSource = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
                    });
                    if ((dataSource[0].dT_CODE == undefined || dataSource[0].dT_CODE == 0) && (dataSource[0].iteM_CODE == "" || dataSource[0].iteM_CODE == null || dataSource[0].iteM_CODE == undefined)) {
                        empr_DeliveryOrder.CreateGrid(data.data);
                    } else {
                        dataSource.unshift(...data.data);
                        gridInstance.option("dataSource", dataSource);
                        gridInstance.refresh();
                        empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
                    }
                }
                $('.bs-example-modal-lg').modal('hide');
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetDeliveryOrderDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/DeliveryOrder/GetDeliveryOrderDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_DeliveryOrder.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetDeliveryOrderPickDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/DeliveryOrder/GetDeliveryOrderPickDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                if (Type == "I") {
                    empr_DeliveryOrder.CreateGrid(data.data);
                } else {
                    if (data.data.length > 0) {
                        if ($('#SodaPickDetailGridContainer').data('dxDataGrid') != undefined) {
                            $('#SodaPickDetailGridContainer').data('dxDataGrid').dispose();
                        }
                        console.log(data.data)
                        empr_DeliveryOrder.CreatePickDetailGrid(data.data);
                        $('#SodaPickDetailModal').modal('show');
                    } else {
                        empr_helper.notify("No data found.", 2);
                    }
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetDataToSave: function () { 
        debugger
        var ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var DRIVER_NAME = $("#DRIVER_NAME").val();
        var BATCH_NO = $("#BATCH_NO").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var SCODE = $("#SCODE").dxSelectBox('option', 'value');
        var DEL_CODE = $("#DEL_CODE").dxSelectBox('option', 'value');
        var BTYPE = $("#BTYPE").dxSelectBox('option', 'value');
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            SCODE: SCODE,
            DEL_CODE: DEL_CODE,
            BTYPE: BTYPE,
            REF: REF,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS,
            BATCH_NO: BATCH_NO,
            DRIVER_NAME: DRIVER_NAME
        }
        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }
        if (empr_DeliveryOrder.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,        
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_DeliveryOrder.GetDataToSave();

        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }
        if (data.Master.PARTY_CODE == '') {
            empr_helper.notify("Please select Party Type.", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

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

            if (!(item.diS_DATE == "" || item.diS_DATE == null || item.diS_DATE == undefined)) {
                item.diS_DATE = empr_helper.PrepareDate(item.diS_DATE);
            }

        });
        console.log(data.Detail)
        if (data.Detail.length > Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    Save: function () {
        var dataModel = empr_DeliveryOrder.GetDataToSave();
        console.log(dataModel)
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/DeliveryOrder/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                if (dataClear == 1) {
                    empr_DeliveryOrder.GetDeliveryOrderDetailByCode(data.data.code);
                    $('#BtnDelete').show();
                }
                else {
                    empr_DeliveryOrder.ResetForm();
                }
                
            }
        }, false, true);
    },
    ResetForm: function () {
        empr_DeliveryOrder.CreateGrid([{ __KEY__: empr_DeliveryOrder.GenerateKey(36), chK1: false, chk: "0", dT_CODE: 0 }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
        $('#hdnDOC').val('');
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('#BTYPE').dxSelectBox('instance').option('value', 'CR');
        $('#Code').val(0);
        empr_DeliveryOrder.pickIds = [];
        var Id = 0;
        empr_DeliveryOrder.InitSalesman(Id);
        empr_DeliveryOrder.InitDeliveryman(Id);
        empr_DeliveryOrder.InitPartyType();
        $('#PARTY_CODE').dxSelectBox('instance').option('value', '');
        $('#REF').focus();
        $('#pickItems').show();
        $('#V_DATE').val(todayDate);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/DeliveryOrder/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_DeliveryOrder.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    UploadDoc: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/Common/UploadVoucherDocs",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnDOC").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },
    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
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
    InitPartyType: function () {
        console.log(PartyType)
        empr_DeliveryOrder.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
            console.log(d)
            if (d.value == null || d.value == '') {
                $('#partyhidden').val('');
                $('#acthidden').val('');
                empr_DeliveryOrder.InitSalesman(0);
                empr_DeliveryOrder.InitDeliveryman(0);
            }
            else {
                var filteredData = $.grep(PartyType, function (item) {
                    return item.key === d.value;
                });
                $('#partyhidden').val(filteredData[0].partyCode)
                $('#acthidden').val(filteredData[0].accountCode)
                $('#DISC').val(filteredData[0].disc)
                empr_DeliveryOrder.InitSalesman(filteredData[0].partyCode);
                empr_DeliveryOrder.InitDeliveryman(filteredData[0].partyCode);
            }

        });

    },
    InitDeliveryman: function (Id) {
        console.log(Deliveryman)
        var xhr = ajaxHelper.ajaxGetJson('/DeliveryOrder/GetDeliverymanByParty?id=' + Id, function (data) {
            empr_DeliveryOrder.bindDxDdl("DEL_CODE", data.data, null, "partY_CODE", "partY_NAME", "Select", function (d) {
                $('#deliverymanhidden').val(d.value);
                if (selectedvalue.selectedRowsData.length > 0) {
                    var value = selectedvalue.selectedRowsData[0]['partY_CODE'];
                    var displayname = selectedvalue.selectedRowsData[0]['partY_NAME'];
                    $('#deliverymanhidden').val(value);
                }
            });
        });
    },
    InitItemIds: function () {
        debugger;
        empr_DeliveryOrder.bindDxDdl("PICK_ITEM", ItemIds, null, "key", "itemId", "Select", function (d) {
            console.log(d.value)
            if (d.value == null || d.value == '') {
                $('#itemIdHidden').val('');
            }
            else {
                $('#itemIdHidden').val(d.value)
            }
        });

    },
    InitSalesman: function (Id) {
        debugger
        var xhr = ajaxHelper.ajaxGetJson('/DeliveryOrder/GetSalesmanByParty?id=' + Id, function (data) {
            empr_DeliveryOrder.bindDxDdl("SCODE", data.data, null, "partY_CODE", "partY_NAME", "Select", function (d) {
                $('#salesmanhidden').val(d.value);
                if (selectedvalue.selectedRowsData.length > 0) {
                    var value = selectedvalue.selectedRowsData[0]['partY_CODE'];
                    var displayname = selectedvalue.selectedRowsData[0]['partY_NAME'];
                    $('#salesmanhidden').val(value);
                }
            });
        });
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/DeliveryOrder/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    selectedValue = data.data[0].mD_ID;
                }
                console.log(data.data);
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
        empr_DeliveryOrder.InitReportTypeDDL();
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/DeliveryOrder/GetPrintReport", function (data) {
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
    AddSodaToDelivery: function () {
        if ($('#SodaPickDetailGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_DeliveryOrder.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });
                if (IsDataAvailableInGrid) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    var finalData = existingData.concat(selectedSodas);
                    empr_DeliveryOrder.pickIds = finalData.map(x => x.picK_ID);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    empr_DeliveryOrder.pickIds = selectedSodas.map(x => x.picK_ID);
                    console.log(empr_DeliveryOrder.pickIds);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                if (empr_DeliveryOrder.pickIds.length > 0) {
                    $('#pickItems').hide();
                }
                $('.modal').hide();
            });
        }
        else {
            var data = empr_DeliveryOrder.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });
            if (IsDataAvailableInGrid) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                var finalData = existingData.concat(selectedSodas);
                empr_DeliveryOrder.pickIds = finalData.map(x => x.picK_ID_D);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                empr_DeliveryOrder.pickIds = selectedSodas.map(x => x.picK_ID_D);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
            }
            if (empr_DeliveryOrder.pickIds.length > 0) {
                $('#pickItems').hide();
            }
            $('.modal').hide();
        }
    },
    AddToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_DeliveryOrder.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    var finalData = existingData.concat(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var data = empr_DeliveryOrder.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                var finalData = existingData.concat(selectedSodas);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
            }

            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },
}