$(document).keydown(function (e) {
    //Check Ctrl key is pressed and S key is pressed Saveempr_TexSalesInvoice
    if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
        if (empr_TexSalesInvoice.ValidateMainInfo()) {
            empr_TexSalesInvoice.Save();
        }
        return false;
    }
    //Check Ctrl key is pressed and D key is pressed Delete
    if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
        e.preventDefault();
        if ($("#Code").val() != '') {
            empr_TexSalesInvoice.Delete();
        } else {
            empr_helper.notify("Please select any record for delete..", 2);
        }
        return false;
    }
    //Check Alt key is pressed and R key is pressed Refresh
    if ((e.altKey || e.metaKey) && e.key === 'a') {
        e.preventDefault();
        empr_TexSalesInvoice.ResetForm();
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
var empr_TexSalesInvoice = {
    totalCount: 0,
    formName: typeForm,
    rowsCount: 0,
    pickIds: [],
    originalValues: {},
    firstClick: 0,
    //BtnSodaPick
    //BtnAddSodaDetailToDelivery
    InitEvents: function () {
        $(document).ready(function () {
            console.log('Currencies', Currencies);
            empr_TexSalesInvoice.InitQuickSearchGrid();
            empr_TexSalesInvoice.InitPartyType();
            empr_TexSalesInvoice.InitItemIds();
            empr_TexSalesInvoice.InitReportTypeDDL();
            //empr_TexSalesInvoice.InitClientPODDL();
            empr_TexSalesInvoice.ResetForm();

            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_TexSalesInvoice.GetPurchaseBillByCode(data.traN_ID);
                }
            });

            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    empr_helper.selectedBill = data.traN_ID;
                    $('#Code').val(data.traN_ID);
                    empr_TexSalesInvoice.GetPurchaseBillByCode(data.traN_ID);
                }
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_TexSalesInvoice.GeneratePrintReport();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_TexSalesInvoice.GeneratePrintReport();
            });

            $('body').on('click', '#docBrowseBtn', function () {
                $('#DOC').val('');
                $('#hdnDOC').val('');
                $('#DOCName').val('');
                $('#DOC').click();
            });

            $('.chkCell').prop('checked', true);

            //$('#VC_TYPE').change(function () {
            //    var VC_TYPE = $(this).is(':checked') ? 1 : 0;

            //    if (VC_TYPE === 1) {
            //        $('#VC_PREFIX').prop('readonly', false);
            //    } else {
            //        $('#VC_PREFIX').prop('readonly', true);
            //    }
            //});

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_TexSalesInvoice.GetPurchaseBillByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/TexSalesInvoice/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_TexSalesInvoice.GetPurchaseBillByCode(data.data.code);
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
                            if (empr_TexSalesInvoice.ValidateMainInfo()) {
                                empr_TexSalesInvoice.Save();
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
                        if (empr_TexSalesInvoice.ValidateMainInfo()) {
                            empr_TexSalesInvoice.Save();
                        }
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);
                    }, 200);
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_TexSalesInvoice.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_TexSalesInvoice.ResetForm();
                $('#REF').focus();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_TexSalesInvoice.InitQuickSearchGrid();
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
                            empr_TexSalesInvoice.GetPurchaseBillDetailByItem(itemId, itemQty);
                        }
                    } else {
                        empr_TexSalesInvoice.GetPurchaseBillDetailByItem(itemId, itemQty);
                    }
                } else {
                    empr_helper.notify("Please fill all fields.", 2);
                }
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_TexSalesInvoice.InitSodaPickGrid();
            });

            //$('#COMM').on('input', function () {
            //    empr_TexSalesInvoice.CalculateCommition();
            //});

            //$('#COMM_VAL').on('input', function () {
            //    empr_TexSalesInvoice.CalculateCommition();
            //});

            $('body').on('click', '#BtnAddBarcodes', function () {
                var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedBarcodes.length > 0) {
                    empr_TexSalesInvoice.AddBarcodeToGrid();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    //console.log(selectedSodas)
                    var id = selectedSodas[0].id;
                    var disc = selectedSodas[0].disc;
                    $('#DISC').val(disc);
                    $('.modal').modal('hide');
                    empr_TexSalesInvoice.GetPurchaseBillPickDetailByCode(id);
                }
                else {
                    empr_helper.notify("Please select the bill first.", 2);
                }
            });

            $('body').on('click', '#BtnAddSodaDetailToDelivery', function () {
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    //console.log('picked');
                    empr_TexSalesInvoice.AddSodaToDelivery();
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
        //debugger;
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        //var ACT_CODE = $("#acthidden").val();
        ////console.log(PARTY_CODE);
        ////console.log(empr_TexSalesInvoice.formName);
        if (PARTY_CODE == "" || PARTY_CODE == null || PARTY_CODE == undefined || PARTY_CODE == 0 || ACT_CODE == "" || ACT_CODE == null || ACT_CODE == undefined || ACT_CODE == 0) {
            empr_helper.notify("Please select Supplier first.", 2);
        }
        else {
            empr_TexSalesInvoice.GetPickDataByClientPo(PARTY_CODE, ACT_CODE);
        }
        //if ((PARTY_CODE == "" || PARTY_CODE == null || PARTY_CODE == undefined || PARTY_CODE == 0) && empr_TexSalesInvoice.formName != 'MPO') {
        //    empr_helper.notify("Please select party first.", 2);
        //} else {
        //    empr_TexSalesInvoice.GetPickDataByClientPo(PARTY_CODE, ACT_CODE);
        //}
        //empr_TexSalesInvoice.GetPickDataByClientPo(0, 0);

    }, //yaha open
    GetPickDataByClientPo: function (PARTY_CODE, ACT_CODE) {
        ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetPickDataBySupplier?pCode=' + PARTY_CODE + '&actCode=' + ACT_CODE, function (data) {
            //console.log(data);
            ////debugger;
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_TexSalesInvoice.CreatePickDetailGrid(data.data);
                    $('#SodaPickDetailModal').modal('show');
                } else {
                    empr_helper.notify("No PO Detail found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    InitBarcodePickGrid: function () {
        if (empr_TexSalesInvoice.pickIds.length > 0) {
            empr_helper.notify("Cannot open on return data.", 2);
        } else {
            ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetBarcodeList', function (data) {
                //console.log(data)
                if (data.msgType == 1) {
                    if (data.data.length > 0) {
                        //if ($('#BarcodePickGridContainer').data('dxDataGrid') != undefined) {
                        //    $('#BarcodePickGridContainer').data('dxDataGrid').dispose();
                        //}
                        empr_TexSalesInvoice.CreateBarcodeGrid(data.data);
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
        empr_helper.dxGridbindingVouchers('#BarcodePickGridContainer', col, dataSrc, "PurchaseBillBarcodes");
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
            selectedBarcodes = empr_TexSalesInvoice.SetData(selectedBarcodes);
            //console.log(selectedBarcodes);
            dataSource.unshift(...selectedBarcodes);
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
            empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
        }
        else {
            var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
            selectedBarcodes = empr_TexSalesInvoice.SetData(selectedBarcodes);
            //console.log(selectedBarcodes);
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
            item.__KEY__ = empr_TexSalesInvoice.GenerateKey(36);
        });

        return dataSource;
    },
    CreatePickGrid: function (dataSrc) {
        var col = [
            //{
            //    dataField: "Action",
            //    width: 100,
            //    alignment: 'center',
            //    fixed: true,
            //    fixedPosition: "left",
            //    allowExporting: false,
            //    cellTemplate: function (container, options) {


            //        $(`<div class="btn-group btn-group-sm">
            //               <a href="javascript:;"  class="grid-action-icon elm_edit_pick" reportid=${options.data.id} disc=${options.data.disc} title="Edit"><i class="fa fa-edit"></i></a>
            //               </div>`).appendTo(container);
            //    }
            //},
            { dataField: 'id', caption: 'Code', visible: true, },
            { dataField: 'lB_DATE', caption: 'Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
            { dataField: 'partY_NAME', caption: 'Seller', allowEditing: false, },
            { dataField: 'ref', caption: 'Ref', allowEditing: false, },
            { dataField: 'qty', caption: 'Quantity', allowEditing: false, },
            { dataField: 'amt', caption: 'Amount', allowEditing: false, },
            { dataField: 'disc', caption: 'Discount', allowEditing: false, },
        ];
        empr_helper.dxGridbindingVouchers('#SodaPickGridContainer', col, dataSrc, "PurchaseBillPick", "single");
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    CreatePickDetailGrid: function (dataSrc) {
        console.log("PICK GRID", dataSrc);
        var col = [];
        var updatedData = [];

        if (empr_TexSalesInvoice.formName == 'MPO') {
            existingdata = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
            updatedData = dataSrc
                .map(item => {
                    const matchingRows = existingdata.filter(row => row.picK_ID_D === item.picK_ID_D);
                    const totalQty = matchingRows.reduce((sum, row) => parseInt(sum) + parseInt(row.qty), 0);
                    item.qty -= totalQty;

                    const totalCommRate = matchingRows.reduce((sum, row) => parseFloat(sum) + parseFloat(row.comM_RATE || 0), 0);
                    item.comM_RATE -= totalCommRate;

                    const totalCommAmt = matchingRows.reduce((sum, row) => parseFloat(sum) + parseFloat(row.comM_AMT || 0), 0);
                    item.comM_AMT -= totalCommAmt;


                    return item.qty > 0 ? item : null;
                })
                .filter(item => item !== null);
            empr_TexSalesInvoice.originalValues = {};
            updatedData.forEach(row => {
                empr_TexSalesInvoice.originalValues[row.picK_ID_D] = row.qty;
            });

            col = [
                { dataField: 'lB_DATE', caption: 'Date', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'voucheR_NO', caption: 'Transaction#', visible: true, allowEditing: false, alignment: 'center' },
                {
                    dataField: 'voucheR_NO',
                    caption: 'Transaction#',
                    alignment: 'center',
                    allowEditing: false,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_TexSalesInvoice.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.id) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'partY_NAME', caption: 'Party Name', visible: true, width: 150, allowEditing: false, alignment: 'center' },
                { dataField: 'suP_NAME', caption: 'Supplier Name', visible: true, width: 150, allowEditing: false, alignment: 'center' },
                { dataField: 'clienT_PO', caption: 'Model# / PO#', visible: true, width: 150, allowEditing: false, alignment: 'center' },


                { dataField: 'joB_NO', caption: 'Job NO#', visible: true, allowEditing: false, alignment: 'center' },
                { dataField: 'iteM_NAME', caption: 'Item', visible: true, width: 200, allowEditing: false, alignment: 'center' },
                { dataField: 'uniT_NAME', caption: 'Unit', visible: true, allowEditing: false, alignment: 'center' },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 90, allowEditing: false, alignment: 'center' },
                {
                    dataField: 'qty',
                    caption: 'Quantity',
                    allowSorting: false,
                    allowFiltering: false,
                    dataType: 'number',
                    width: 90,
                    allowEditing: false,
                    alignment: 'center',
                    setCellValue: function (newData, value, currentRowData) {
                        const originalQty = empr_TexSalesInvoice.originalValues[currentRowData.picK_ID_D];
                        debugger;
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
                        const originalQty = empr_TexSalesInvoice.originalValues[data.picK_ID_D];
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
                {
                    dataField: 'amt',
                    caption: 'Amt',
                    visible: true,
                    allowEditing: false,
                    alignment: 'center'
                },
                { dataField: 'comM_TYPE', caption: 'Comm Type', visible: true, allowEditing: false, alignment: 'center', width: 120 },
                {
                    dataField: 'comM_RATE',
                    caption: 'Comm Rate',
                    visible: true,
                    allowEditing: false,
                    alignment: 'center',
                    width: 120
                },
                {
                    dataField: 'comM_AMT',
                    caption: 'Comm Amt',
                    visible: true,
                    allowEditing: false,
                    alignment: 'center',
                    width: 120,
                    calculateCellValue: function (data) {
                        const amt = parseFloat(data.amt) || 0;
                        const rate = parseFloat(data.comM_RATE) || 0;
                        const comm = parseFloat((amt * rate / 100).toFixed(2));
                        data.comM_AMT = comm; // ✅ Original data ko update kar diya
                        return comm;
                    }
                },

                { dataField: 'descr', caption: 'Curr.Type', alignment: 'center' },
                { dataField: 'crate', caption: 'Curr.Rate', alignment: 'center' },
                { dataField: 'curR_CODE', caption: 'Curr Code', visible: false, },
                { dataField: 'dT_CODE', caption: 'Net Amt', visible: false, },


                //{ dataField: 'disc', caption: 'Disc %', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'disC_AMT', caption: 'Disc Amt', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'tax', caption: 'Tax %', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'taX_AMT', caption: 'Tax Amt', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'neT_AMT', caption: 'Net Amt', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'coloR_NAME', caption: 'Color', visible: true, allowEditing: false, alignment: 'center' },
                //{ dataField: 'sizE_NAME', caption: 'Size', visible: true, allowEditing: false, alignment: 'center' },

                //{ dataField: 'partY_CODE', caption: 'Party Code', visible: false, },
                //{ dataField: 'partY_DDL', visible: false, },
                //{ dataField: 'iteM_CODE', visible: false, },
            ];

            updatedData = dataSrc;
        }

        empr_helper.editableDxGridbinding('#SodaPickDetailGridContainer', col, updatedData, "SodaPickDetailGrid");
        setTimeout(function () {
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').refresh();
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    CreateGrid: function (dataSrc) {
        console.log('CreateGrid', dataSrc);
        ////debugger;
        dataSrc.forEach(item => {
            if (
                item.deL_DATE == '1900-01-01' || item.deL_DATE == '01-01-1900' || item.deL_DATE == '01-Jan-1900' || item.deL_DATE == '1/1/1900 12:00:00 AM' || item.deL_DATE == '01/01/1900 12:00:00 AM' || item.deL_DATE == '1/1/1900' ||
                item.deL_DATE == '2000-01-01' || item.deL_DATE == '01-01-2000' || item.deL_DATE == '01-Jan-2000' || item.deL_DATE == '1/1/2000 12:00:00 AM' || item.deL_DATE == '01/01/2000 12:00:00 AM' || item.deL_DATE == '1/1/2000' ||
                item.deL_DATE == '00-01-01' || item.deL_DATE == '01-01-00' || item.deL_DATE == '01-Jan-00' || item.deL_DATE == '1/1/00 12:00:00 AM' || item.deL_DATE == '01/01/00 12:00:00 AM' || item.deL_DATE == '1/1/00'
            ) {
                item.deL_DATE = undefined;
            }

            if (
                item.duE_DATE == '1900-01-01' || item.duE_DATE == '01-01-1900' || item.duE_DATE == '01-Jan-1900' || item.duE_DATE == '1/1/1900 12:00:00 AM' || item.duE_DATE == '01/01/1900 12:00:00 AM' || item.duE_DATE == '1/1/1900' ||
                item.duE_DATE == '2000-01-01' || item.duE_DATE == '01-01-2000' || item.duE_DATE == '01-Jan-2000' || item.duE_DATE == '1/1/2000 12:00:00 AM' || item.duE_DATE == '01/01/2000 12:00:00 AM' || item.duE_DATE == '1/1/2000' ||
                item.duE_DATE == '00-01-01' || item.duE_DATE == '01-01-00' || item.duE_DATE == '01-Jan-00' || item.duE_DATE == '1/1/00 12:00:00 AM' || item.duE_DATE == '01/01/00 12:00:00 AM' || item.duE_DATE == '1/1/00'
            ) {
                item.duE_DATE = undefined;
            }
            //console.log(item.picK_ID)
            if (item.picK_ID_D > 0) {
                empr_TexSalesInvoice.pickIds.push(item.picK_ID_D);
                $('#pickItems').hide();
            }
        });
        if (dataSrc.length > 0) {
            empr_TexSalesInvoice.rowsCount = dataSrc.length - 1;
        }
        //var Caption = "";
        //if (Type == "I") {
        //    Caption = "Item";
        //}
        //else {
        //    Caption = "Bar Code";
        //}
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_TexSalesInvoice.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_TexSalesInvoice.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_TexSalesInvoice.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        //const searchAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                        //    ? ''
                        //    : `<a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_TexSalesInvoice.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_TexSalesInvoice.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_TexSalesInvoice.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_TexSalesInvoice.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                           
                           </div>`).appendTo(container);
                    }
                }
            },
            // <a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_TexSalesInvoice.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>
            {
                dataField: 'voucheR_NO',
                caption: 'PS Tran#',
                alignment: 'center',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_TexSalesInvoice.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.id) + ')')
                        .appendTo(container);
                }
            },
            {
                dataField: 'clienT_PO',
                caption: 'Model# / PO#',
                alignment: 'center',
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'partY_DDL',
                caption: 'Buyer',
                width: 250,
                alignment: 'center',
                allowSorting: false,
                lookup: {
                    dataSource: Parties,
                    displayExpr: 'value',
                    valueExpr: 'customizedKey',
                    allowClearing: true
                },
                setCellValue: function (newData, value, currentRowData) {
                    var selectedParty = Parties.filter(u => u.customizedKey == value);
                    if (selectedParty.length > 0) {
                        newData.partY_DDL = selectedParty[0].customizedKey;
                        newData.partY_CODE = selectedParty[0].key;
                        newData.acT_CODE = selectedParty[0].accountCode;
                    } else {
                        newData.partY_DDL = null;
                        newData.partY_CODE = null;
                        newData.acT_CODE = null;
                    }
                }
            },
            {
                dataField: 'partY_CODE',
                visible: false
            },
            {
                dataField: 'acT_CODE',
                visible: false
            },
            {
                dataField: 'iteM_CODE',
                caption: 'Item',
                width: 320,
                allowSorting: false,
                fixed: true,
                alignment: 'center',
                fixedPosition: "left",
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
            },
            {
                dataField: 'qty',
                caption: 'Qty',
                width: 100,
                alignment: 'center',
                setCellValue: function (newData, value, currentRowData) {
                    // Quantity update
                    newData.qty = parseFloat(value) || 0;

                    // Rate fetch, 0 default
                    const rate = parseFloat(currentRowData.rate) || 0;
                    newData.neT_AMT = (newData.qty * rate);

                    // Commission calculation if rate exists
                    const commRate = parseFloat(currentRowData.comM_RATE) || 0;
                    newData.comM_AMT = (newData.neT_AMT * commRate / 100);
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                alignment: 'center',
                width: 100,
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'rate',
                caption: 'Rate',
                alignment: 'center',
                width: 100,
                setCellValue: function (newData, value, currentRowData) {
                    newData.rate = parseFloat(value) || 0;

                    const qty = parseFloat(currentRowData.qty) || 0;
                    newData.neT_AMT = (qty * newData.rate);

                    const commRate = parseFloat(currentRowData.comM_RATE) || 0;
                    newData.comM_AMT = (newData.neT_AMT * commRate / 100);
                }
            },
            {
                dataField: 'neT_AMT',
                width: 220,
                alignment: 'center',
                caption: 'Net Amount',
                allowEditing: false,
            },
            {
                dataField: 'comM_TYPE',
                caption: 'Comm Type',
                lookup: {
                    dataSource: empr_helper.commType,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                width: 150,
                alignment: 'center',
                visible: false
            },
            {
                dataField: 'comM_RATE',
                caption: 'Comm Rate',
                alignment: 'center',
                setCellValue: function (newData, value, currentRowData) {
                    newData.comM_RATE = parseFloat(value) || 0;

                    const netAmt = parseFloat(currentRowData.neT_AMT) || 0;
                    newData.comM_AMT = (netAmt * newData.comM_RATE / 100);
                }
            },
            //{
            //    dataField: 'comM_AMT',
            //    caption: 'Comm Amt',
            //    alignment: 'center',
            //    setCellValue: function (newData, value, currentRowData) {
            //        newData.comM_AMT = parseFloat(value) || 0;

            //        const netAmt = parseFloat(currentRowData.neT_AMT) || 0;
            //        newData.comM_RATE = netAmt ? ((newData.comM_AMT / netAmt) * 100).toFixed(2) : 0;
            //        newData.comM_RATE = parseFloat(newData.comM_RATE);
            //    }
            //},
            {
                dataField: 'comM_AMT',
                caption: 'Comm Amt',
                alignment: 'center',
                format: {
                    type: 'fixedPoint',
                    precision: 2 
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.comM_AMT = parseFloat(value) || 0;

                    const netAmt = parseFloat(currentRowData.neT_AMT) || 0;
                    newData.comM_RATE = netAmt
                        ? parseFloat(((newData.comM_AMT / netAmt) * 100).toFixed(2))
                        : 0;
                }
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                alignment: 'center',
                width: 400,
                wordWrapEnabled: true,
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
            //{
            //    dataField: 'curR_CODE',
            //    caption: 'Curr.Code',
            //    alignment: 'center',
            //    //visible: false
            //},
            {
                dataField: 'curR_CODE',
                caption: 'Curr.Code',
                alignment: 'center',
                width: 100,
                lookup: {
                    dataSource: Currencies,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {

                    newData.curR_CODE = value;

                    let selected = Currencies.find(c => c.key == value);

                    if (selected) {
                        newData.crate = selected.rate;
                    }
                }
            },
            {
                dataField: 'crate',
                caption: 'Curr.Rate',
                alignment: 'center',
                allowEditing: false,
            },
            //{
            //    dataField: 'amt',
            //    caption: 'Amount',
            //    width: 100,
            //    allowEditing: false,
            //    visible: false
            //},


        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "PurchaseBill", "iteM_CODE");

        var grid = $('#DetailContainer').dxDataGrid('instance');

        grid.on('editorPreparing', function (e) {
            if (e.parentType === 'dataRow') {
                if (e.dataField === 'comM_RATE' && e.row.data.comM_TYPE === 'RS') {
                    e.editorOptions.readOnly = true;
                }

                if (e.dataField === 'comM_AMT' && e.row.data.comM_TYPE === 'PR') {
                    e.editorOptions.readOnly = true;
                }
            }
        });

        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        setTimeout(function () {
            var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
            //$('#DetailContainer').dxDataGrid('instance').focus(nextElement);
        }, 1500);
    },
    CloneRow: function (index) {
        if (empr_TexSalesInvoice.pickIds.length > 0) {
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

                    empr_TexSalesInvoice.rowsCount += 1;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    if (dataSource.length > 0) {
                        let clonedRowData = $.extend(true, {}, dataSource[index]);
                        if (clonedRowData.hasOwnProperty('dT_CODE')) {
                            delete clonedRowData.dT_CODE;
                        }
                        clonedRowData.__KEY__ = empr_TexSalesInvoice.GenerateKey(36);
                        clonedRowData.dT_CODE = 0;
                        let newDataSource = [clonedRowData].concat(dataSource);
                        gridInstance.option("dataSource", newDataSource);
                        gridInstance.refresh();
                    }
                });
            }
            else {
                empr_TexSalesInvoice.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_TexSalesInvoice.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            }
        }
    },
    AddRow: function () {
        if (empr_TexSalesInvoice.pickIds.length > 0) {
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
                    empr_TexSalesInvoice.rowsCount += 1;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    const dataSource = gridInstance.option("dataSource");

                    dataSource.unshift({ __KEY__: empr_TexSalesInvoice.GenerateKey(36), dT_CODE: 0 });
                    gridInstance.option("dataSource", dataSource);
                    gridInstance.refresh();
                    empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
                });
            }
            else {
                empr_TexSalesInvoice.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_TexSalesInvoice.GenerateKey(36), dT_CODE: 0 });
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
                    empr_TexSalesInvoice.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/TexSalesInvoice/DeletePurchaseBillDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_TexSalesInvoice.rowsCount -= 1;
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
        empr_TexSalesInvoice.GetPurchaseBill();
        //empr_TexSalesInvoice.CreateQuickSearchGrid();
    },
    GetPurchaseBill: function () {
        ajaxHelper.ajaxGetJson('/TexSalesInvoice/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_TexSalesInvoice.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        ////debugger;
        //console.log('CreateQuickSearchGrid',dataSrc)
        var col = [];
        //if (dataSrc.length > 0 ? dataSrc[0].amt != undefined : false) {
        //    col = [{
        //        dataField: "Action",
        //        width: 100,
        //        alignment: 'center',
        //        fixed: true,
        //        fixedPosition: "left",
        //        allowExporting: false,
        //        cellTemplate: function (container, options) {
        //            if (Permissions != "Admin" && !Permissions.r_PRINT) {
        //                $(`<div class="btn-group btn-group-sm">
        //                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
        //                       </div>`).appendTo(container);
        //            } else {
        //                $(`<div class="btn-group btn-group-sm">
        //                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
        //                       <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.code} title="PRINT"><i class="fa fa-print"></i></a>
        //                       <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>
        //                       </div>`).appendTo(container);
        //            }
        //        }
        //    },
        //    { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
        //    { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
        //    { dataField: 'voucheR_NO', caption: 'Voucher No', },
        //    { dataField: 'partY_NAME', caption: 'Party Name', },
        //    { dataField: 'ref', caption: 'Reference No', },
        //    { dataField: 'remarks', caption: 'Description', },
        //    { dataField: 'amt', caption: 'Amount', },
        //    { dataField: 'astatus', caption: 'Status', },
        //    ];
        //}
        //else {

        //}

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
        {
            dataField: 'doc',
            caption: 'Doc',
            width: 100,
            alignment: 'center',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.doc != null && options.data.doc != '' && options.data.doc != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="window.open('${options.data.doc}', '_blank')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'voucheR_NO', caption: 'Invoice No', },
        { dataField: 'partY_NAME', caption: 'Party Name', },
        //{ dataField: 'clienT_PO', caption: 'Client PO #', },
        //{ dataField: 'joB_NO', caption: 'Job No#', },
        { dataField: 'ref', caption: 'Reference No', },
        { dataField: 'remarks', caption: 'Remarks', },
        { dataField: 'astatus', caption: 'Status', },
        ]

        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/TexSalesInvoice/GetPurchaseBill", "id", "PartyOpening");
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "PurchaseBillQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    GetPurchaseBillByCode: function (code) {
        ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetPurchaseBillByCode?code=' + code, function (data) {
            //console.log('editData', data);
            ////debugger;
            if (data.master.msgType == 1) {

                var masterData = data.master.data;
                if (masterData.length == 1) {
                    $('#pickItems').show();
                    empr_TexSalesInvoice.pickIds = [];
                    var response = masterData[0];

                    var filteredData = $.grep(PartyType, function (item) {
                        return item.partyCode === response.partY_CODE && item.accountCode === response.acT_CODE.toString();
                    });

                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    //$("#VC_TYPE").prop("checked", response.vC_TYPE == 0 ? false : true);
                    $('#ReportType').dxSelectBox('instance').option('value', response.mD_ID);
                    //$('#CLIENT_PO').dxSelectBox('instance').option('value', response.clienT_PO);
                    $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);
                    //$('#Currency').dxSelectBox('instance').option('value', response.curR_CODE);
                    //$('#Rate').val(response.crate);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#SUP_INVNO').val(response.suP_INVNO);
                    $('#hdnDOC').val(response.doc);

                    var fullPath = response.doc;
                    var fileName = fullPath.split('/').pop();
                    $('#DOCName').val(fileName);




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
                    empr_TexSalesInvoice.CreateGrid(data.detail.data);
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
    GetPurchaseBillDetailByItem: function (code, qty) {
        ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetPurchaseBillDetailByItem?code=' + code + '&qty=' + qty, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_TexSalesInvoice.rowsCount += data.data.length;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                        dataSource = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
                    });
                    if ((dataSource[0].dT_CODE == undefined || dataSource[0].dT_CODE == 0) && (dataSource[0].iteM_CODE == "" || dataSource[0].iteM_CODE == null || dataSource[0].iteM_CODE == undefined)) {
                        empr_TexSalesInvoice.CreateGrid(data.data);
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
    GetPurchaseBillDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetPurchaseBillDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_TexSalesInvoice.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetPurchaseBillPickDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetPurchaseBillPickDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                if (Type == "I") {
                    empr_TexSalesInvoice.CreateGrid(data.data);
                } else {
                    if (data.data.length > 0) {
                        if ($('#SodaPickDetailGridContainer').data('dxDataGrid') != undefined) {
                            $('#SodaPickDetailGridContainer').data('dxDataGrid').dispose();
                        }
                        empr_TexSalesInvoice.CreatePickDetailGrid(data.data);
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
        var ID = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var V_DATE = $("#V_DATE").val();
        var MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var REF = $("#REF").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var DOC = $("#hdnDOC").val();
        var SUP_INVNO = $("#SUP_INVNO").val();
        var REMARKS = $("#REMARKS").val();

        var masterRecord = {
            TRAN_ID: ID,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            MD_ID: MD_ID,
            VOUCHER_NO: VOUCHER_NO,
            REF: REF,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            DOC: DOC,
            SUP_INVNO: SUP_INVNO,
            REMARKS: REMARKS,
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
        if (empr_TexSalesInvoice.rowsCount == detailRecords.length) {
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
        var data = empr_TexSalesInvoice.GetDataToSave();

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

        //if (data.Master.DOC == '') {
        //    empr_helper.notify("Please Select Document", 2);
        //    valid = false;
        //    return valid;
        //}




        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add items.", 2);
            valid = false;
            return valid;
        }

        $.each(data.Detail, function (index, item) {
            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
                empr_helper.notify("Please select Item at Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }


        });

        if (data.Detail.length > Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    Save: function () {
        var dataModel = empr_TexSalesInvoice.GetDataToSave();
        //console.log(dataModel)
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/TexSalesInvoice/Save", function (data) {
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
                    empr_TexSalesInvoice.GetPurchaseBillDetailByCode(data.data.code);
                    $('#BtnDelete').show();
                }
                else {
                    empr_TexSalesInvoice.ResetForm();
                }

            }
        }, false, true);
    },
    ResetForm: function () {
        empr_TexSalesInvoice.CreateGrid([{ __KEY__: empr_TexSalesInvoice.GenerateKey(36), chK1: false, chk: "0", dT_CODE: 0 }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
        $('#hdnDOC').val('');
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        $('#SUP_INVNO').val('');
        //$('#VC_PREFIX').val('');
        //$('#HS_CODE').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        //$("#VC_TYPE").prop("checked", true);
        $('#Code').val();
        empr_TexSalesInvoice.pickIds = [];
        empr_TexSalesInvoice.firstClick = 0;
        //empr_TexSalesInvoice.CreateGrid([]);
        var Id = 0;
        //empr_TexSalesInvoice.InitSalesman(Id);
        empr_TexSalesInvoice.InitPartyType();
        //empr_TexSalesInvoice.InitCurrencyDDL();
        //empr_TexSalesInvoice.InitCommissionAmtDDL("PR");
        $('#PARTY_CODE').dxSelectBox('instance').option('value', '');
        //$('#CLIENT_PO').dxSelectBox('instance').option('value', '');
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/TexSalesInvoice/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_TexSalesInvoice.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    //UploadDoc: function () {
    //    $('#BtnSave').prop('disabled', true);
    //    var files = document.getElementById('DOC').files;
    //    var formData = new FormData();
    //    for (var i = 0; i !== files.length; i++) {
    //        formData.append("model", files[i]);
    //    }
    //    $.ajax(
    //        {
    //            url: "/Common/UploadVoucherDocs",
    //            data: formData,
    //            processData: false,
    //            contentType: false,
    //            type: "POST",
    //            success: function (data) {
    //                if (data.msgType == '1') {
    //                    $("#hdnDOC").val(data.data);
    //                }
    //                else {
    //                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
    //                }
    //                $('#BtnSave').prop('disabled', false);

    //            }
    //        }
    //    );
    //},
    //OpenDoc: function () {
    //    var hdnUrl = $('#hdnDOC').val();
    //    if (hdnUrl == "" || hdnUrl == null) {
    //        empr_helper.notify("Please upload a file to view.", 2);
    //    }
    //    else {
    //        const fileURL = window.location.origin + hdnUrl;
    //        window.open(fileURL, '_blank');
    //    }
    //},

    UploadDoc: function () {
        console.log('UploadDoc Call');
        debugger;
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        if (files.length > 0) {
            $('#DOCName').val(files[0].name); // file ka naam dikhaye
        }

        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax({
            url: "/Common/UploadVoucherDocs",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    $("#hdnDOC").val(data.data);
                    empr_helper.notify("File uploaded successfully.", 1);
                } else {
                    $('#DOCName').val("No File");
                    empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                }
                $('#saveAttempt').prop('disabled', false);
            }
        });
    },

    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (!hdnUrl) {
            empr_helper.notify("Please upload a file to view.", 2);
        } else {
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
        //console.log(PartyType)
        empr_TexSalesInvoice.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
            //console.log(d)
            if (d.value == null || d.value == '') {
                $('#partyhidden').val('');
                $('#acthidden').val('');
                //empr_TexSalesInvoice.InitSalesman(0);
            }
            else {
                var filteredData = $.grep(PartyType, function (item) {
                    return item.key === d.value;
                });
                $('#partyhidden').val(filteredData[0].partyCode)
                $('#acthidden').val(filteredData[0].accountCode)
                $('#DISC').val(filteredData[0].disc)
                //if (filteredData[0].partyCode != "" && filteredData[0].partyCode != 0) {
                //    empr_TexSalesInvoice.InitSalesman(filteredData[0].partyCode);
                //}
            }

        });

    },
    InitItemIds: function () {
        empr_TexSalesInvoice.bindDxDdl("PICK_ITEM", ItemIds, null, "key", "itemId", "Select", function (d) {
            //console.log(d.value)
            if (d.value == null || d.value == '') {
                $('#itemIdHidden').val('');
            }
            else {
                $('#itemIdHidden').val(d.value)
            }
        });

    },
    //InitSalesman: function (Id) {
    //    var xhr = ajaxHelper.ajaxGetJson('/TexSalesInvoice/GetSalesmanByParty?id=' + Id, function (data) {
    //        empr_TexSalesInvoice.bindDxDdl("SCODE", data.data, null, "partY_CODE", "partY_NAME", "Select", function (d) {
    //            $('#salesmanhidden').val(d.value);
    //            if (selectedvalue.selectedRowsData.length > 0) {
    //                var value = selectedvalue.selectedRowsData[0]['partY_CODE'];
    //                var displayname = selectedvalue.selectedRowsData[0]['partY_NAME'];
    //                $('#salesmanhidden').val(value);
    //                $('#salesmanhidden').val(displayname);
    //            }
    //        });
    //    });
    //},
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitReportTypeDDL: function (selectedValue) {

        if (ReportTypes.value.data.length > 0) {
            selectedValue = ReportTypes.value.data[0].mD_ID;
        }

        $('#ReportType').dxSelectBox({
            dataSource: ReportTypes.value.data,
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
        });
        //ajaxHelper.ajaxGetJson("/TexSalesInvoice/GetReportTypes", function (data) {
        //    if (data.msgType == 1) {
        //        if (data.data.length > 0) {
        //            selectedValue = data.data[0].mD_ID;
        //        }
        //        $('#ReportType').dxSelectBox({
        //            dataSource: ReportTypes,
        //            displayExpr: 'mD_NAME',
        //            valueExpr: 'mD_ID',
        //            value: selectedValue,
        //            searchEnabled: true,
        //            width: '100%',
        //            placeholder: 'Search',
        //            showClearButton: true,
        //            dropDownOptions: {
        //                height: 'auto',
        //            },
        //            pagingEnabled: true,
        //            searchTimeout: 500,
        //            onValueChanged: function (e) {
        //            },
        //        });
        //    }
        //    else {
        //        empr_helper.notify(data.data, data.msgType);
        //    }
        //}, false, true);
    },
    GeneratePrintReport: function () {
        //empr_TexSalesInvoice.InitReportTypeDDL();
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/TexSalesInvoice/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                    $('#ReportType').dxSelectBox('instance').option('value', MD_ID);
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    AddSodaToDelivery: function () {
        //debugger;
        if ($('#SodaPickDetailGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_TexSalesInvoice.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });
                if (IsDataAvailableInGrid || empr_TexSalesInvoice.firstClick == 1) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    var finalData = existingData.concat(selectedSodas);
                    empr_TexSalesInvoice.pickIds = finalData.map(x => x.picK_ID);
                    //$('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                    empr_TexSalesInvoice.CreateGrid(finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    empr_TexSalesInvoice.pickIds = selectedSodas.map(x => x.picK_ID);
                    //$('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                    empr_TexSalesInvoice.CreateGrid(selectedSodas);
                    empr_TexSalesInvoice.firstClick = 1;
                }
                if (empr_TexSalesInvoice.pickIds.length > 0) {
                    $('#pickItems').hide();
                }
                $('.modal').hide();
            });
        }
        else {
            var data = empr_TexSalesInvoice.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });
            debugger;
            if (IsDataAvailableInGrid || empr_TexSalesInvoice.firstClick == 1) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource') || [];
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                var finalData = existingData.concat(selectedSodas);
                empr_TexSalesInvoice.pickIds = finalData.map(x => x.picK_ID_D);
                //$('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                empr_TexSalesInvoice.CreateGrid(finalData);

            }
            else {
                debugger;
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                console.log('selectedSodas', selectedSodas);
                //$('#Currency').dxSelectBox('instance').option('value', selectedSodas[0].curR_CODE);


                $('#hdnDOC').val(selectedSodas[0].doc);
                var fullPath = selectedSodas[0].doc;
                var fileName = fullPath.split('/').pop();
                $('#DOCName').val(fileName);

                empr_TexSalesInvoice.pickIds = selectedSodas.map(x => x.picK_ID_D);
                empr_TexSalesInvoice.CreateGrid(selectedSodas);
                empr_TexSalesInvoice.firstClick = 1;
            }
            if (empr_TexSalesInvoice.pickIds.length > 0) {
                $('#pickItems').hide();
            }
            var modalEl = document.getElementById('SodaPickDetailModal');
            var modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) {
                modalInstance.hide();
            }
        }
    },

    //InitCurrencyDDL: function (_selectedValue) {

    //    $('#Currency').dxSelectBox({
    //        dataSource: Currencies,
    //        displayExpr: 'value',
    //        valueExpr: 'key',
    //        value: _selectedValue,
    //        searchEnabled: true,
    //        width: '100%',
    //        placeholder: 'Search',
    //        showClearButton: true,
    //        dropDownOptions: {
    //            height: 'auto',
    //        },
    //        pagingEnabled: true,
    //        searchTimeout: 500,
    //        onValueChanged: function (e) {

    //            if (e.value != '' && e.value != null) {
    //                var items = e.component._dataSource._items;
    //                var item = items.filter(i => i.key == e.value);
    //                if (item.length > 0) {
    //                    $('#Rate').val(item[0].rate);
    //                }
    //            }
    //            else {
    //                $('#Rate').val('');
    //            }
    //        },
    //    });
    //},


    //CalculateCommition: function () {
    //    var grid = $("#DetailContainer").dxDataGrid("instance");
    //    var visibleRows = grid.getVisibleRows(); // Get latest UI data

    //    var amt = visibleRows.reduce(function (sum, row) {
    //        return sum + (parseFloat(row.data.neT_AMT) || 0);
    //    }, 0);

    //    //console.log("Total net_amt:", amt);

    //    var commissionType = $("#COMM_AMT").dxSelectBox("option", "value");

    //    let comm = parseFloat($('#COMM').val());
    //    let commVal = parseFloat($('#COMM_VAL').val());

    //    if (isNaN(amt) || amt <= 0) return;

    //    if (commissionType == 'PR') {
    //        let calcCommVal = (amt * comm) / 100;
    //        $('#COMM_VAL').val(calcCommVal.toFixed(2));
    //    }
    //    else if (commissionType == 'RS'){
    //        let calcComm = (commVal * 100) / amt;
    //        $('#COMM').val(calcComm.toFixed(2));
    //    }


    //},

    calculateNetAmtTotal: function () {
        var grid = $("#DetailContainer").dxDataGrid("instance");
        var visibleRows = grid.getVisibleRows(); // Get latest UI data

        var totalNetAmt = visibleRows.reduce(function (sum, row) {
            return sum + (parseFloat(row.data.neT_AMT) || 0);
        }, 0);
    },

    openVoucherPage(link, tran_Id) {
        debugger;
        console.log('openVoucherPage',link)
        console.log('openVoucherPage', tran_Id)
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
        });
    },
}