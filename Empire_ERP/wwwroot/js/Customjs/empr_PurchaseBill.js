$(document).keydown(function (e) {
    //Check Ctrl key is pressed and S key is pressed Saveempr_PurchaseBill
    if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
        if (empr_PurchaseBill.ValidateMainInfo()) {
            empr_PurchaseBill.Save();
        }
        return false;
    }
    //Check Ctrl key is pressed and D key is pressed Delete
    //if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
    //    e.preventDefault();
    //    if ($("#Code").val() != '') {
    //        empr_PurchaseBill.Delete();
    //    } else {
    //        empr_helper.notify("Please select any record for delete..", 2);
    //    }
    //    return false;
    //}
    //Check Alt key is pressed and R key is pressed Refresh
    if ((e.altKey || e.metaKey) && e.key === 'a') {
        e.preventDefault();
        $("#SCODE").dxSelectBox("instance")?.option("value", "");

        empr_PurchaseBill.ResetForm();
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

document.addEventListener('keydown', function (e) {
    if (e.key !== 'Tab') return;

    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
    const activeElement = document.activeElement;
    const isShift = e.shiftKey;

    let $partyElement = $('#PARTY_CODE');
    let partyEditor = $partyElement.dxSelectBox('instance');

    if ($(activeElement).closest('#PARTY_CODE').length > 0) {

        if (isShift) {
            e.preventDefault();
            document.getElementById('V_DATE').focus();
            return;
        }

        if (!isShift) {
            e.preventDefault();
            const firstCell = gridInstance.getCellElement(0, 'iteM_CODE');
            gridInstance.focus(firstCell);
            gridInstance.editCell(0, 'iteM_CODE');
            return;
        }
    }
    if (isShift) {
        const $focusedCell = $('#DetailContainer').find('.dx-focused');

        if ($focusedCell.length > 0) {
            const columnIndex = $focusedCell.index();
            const visibleColumns = gridInstance.getVisibleColumns();
            const currentColumn = visibleColumns[columnIndex];

            if (currentColumn && currentColumn.dataField === 'iteM_CODE') {
                e.preventDefault();

                if (partyEditor) {
                    partyEditor.focus();
                }

                return;
            }
        }
    }

}, true);
var empr_PurchaseBill = {
    totalCount: 0,
    formName: typeForm,
    rowsCount: 0,
    pickIds: [],
    ItemsOnParty: [],
    vDate: '',
    CurrentStock: [],
    originalValues: {},
    firstClick: 0,
    CommissionTranId: 0,
    PartysaleTax: 0,
    PartyDisc: 0,
    isSalesman: false,
    isReverting: false,
    // BtnSodaPick
    // BtnAddSodaDetailToDelivery
    InitEvents: function () {
        $(document).ready(function () {
            console.log('stkStatus', stkStatus);
            console.log('stock', empr_PurchaseBill.CurrentStock);
            empr_PurchaseBill.GetCurrentStock();
            empr_PurchaseBill.InitQuickSearchGrid();
            empr_PurchaseBill.ResetForm();

            empr_PurchaseBill.InitPartyType();
            empr_PurchaseBill.InitItemIds();
            empr_PurchaseBill.InitReportTypeDDL();
            empr_PurchaseBill.InitRegionDDL();

            var Id = 0;
            empr_PurchaseBill.InitSalesman(Id);
            window.addEventListener('message', function (event) {

                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    empr_helper.selectedBill = data.traN_ID;
                    $('#Code').val(data.traN_ID);
                    empr_PurchaseBill.GetPurchaseBillByCode(data.traN_ID);
                }
            });
            $("#ShowReportModal .modal-dialog").draggable({
                handle: ".modal-header" 
            });
          
            //$("#TERMS").on("input", function () {
            //    let payTerm = parseInt($(this).val(), 10) || 0;
            //    let gridInstance = $("#DetailContainer").dxDataGrid("instance");
            //    let deliveryDate = gridInstance.cellValue(0, "deL_DATE");
            //    let result = empr_PurchaseBill.calculateDue(deliveryDate, payTerm);
            //    if (!result || !result.isValid) {
            //        empr_helper.notify("Past date allow nahi hai", 2);
            //        gridInstance.cellValue(0, "duE_DATE", null);
            //        gridInstance.cellValue(0, "duE_DAYS", 0);
            //    }
            //    else {
            //        gridInstance.cellValue(0, "duE_DATE", result.dueDate);
            //        gridInstance.cellValue(0, "duE_DAYS", result.dueDays);
            //    }
            //});

            $("#TERMS").on("input", function () {
                let payTerm = parseInt($(this).val(), 10) || 0;

                let gridInstance = $("#DetailContainer").dxDataGrid("instance");
                let rowCount = gridInstance.getDataSource().items().length;

                for (let i = 0; i < rowCount; i++) {
                    //let deliveryDate = gridInstance.cellValue(i, "deL_DATE");
                    //var deliveryDate = $('#RINV_DATE').val();

                    var deliveryDate = $('#RINV_DATE').val(); // user ka input

                    // agar empty hai, aaj ka date set karo
                    if (!deliveryDate) {
                        var today = new Date().toISOString().split('T')[0]; // "yyyy-MM-dd"
                        deliveryDate = today;
                    }

                    if (!deliveryDate) {
                        gridInstance.cellValue(i, "duE_DATE", null);
                        gridInstance.cellValue(i, "duE_DAYS", 0);
                        continue;
                    }

                    let result = empr_PurchaseBill.calculateDue(deliveryDate, payTerm);

                    if (!result || !result.isValid) {
                        //empr_helper.notify("Something wrong in Delivery Date", 2);
                        gridInstance.cellValue(i, "duE_DATE", null);
                        gridInstance.cellValue(i, "duE_DAYS", 0);
                    } else {
                        gridInstance.cellValue(i, "duE_DATE", result.dueDate);
                        gridInstance.cellValue(i, "duE_DAYS", result.dueDays);
                    }
                }
            });

            $("#RINV_DATE").on("input", function () {
                let payTerm = parseInt($('#TERMS').val(), 10) || 0;

                let gridInstance = $("#DetailContainer").dxDataGrid("instance");
                let rowCount = gridInstance.getDataSource().items().length;

                for (let i = 0; i < rowCount; i++) {
                    //let deliveryDate = gridInstance.cellValue(i, "deL_DATE");
                    //var deliveryDate = $('#RINV_DATE').val();

                    var deliveryDate = $(this).val(); // user ka input

                    // agar empty hai, aaj ka date set karo
                    //if (!deliveryDate) {
                    //    var today = new Date().toISOString().split('T')[0]; // "yyyy-MM-dd"
                    //    deliveryDate = today;
                    //}

                    if (!deliveryDate) {
                        gridInstance.cellValue(i, "duE_DATE", null);
                        gridInstance.cellValue(i, "duE_DAYS", 0);
                        continue;
                    }

                    let result = empr_PurchaseBill.calculateDue(deliveryDate, payTerm);

                    if (!result || !result.isValid) {
                        //empr_helper.notify("Something wrong in Delivery Date", 2);
                        gridInstance.cellValue(i, "duE_DATE", null);
                        gridInstance.cellValue(i, "duE_DAYS", 0);
                    } else {
                        gridInstance.cellValue(i, "duE_DATE", result.dueDate);
                        gridInstance.cellValue(i, "duE_DAYS", result.dueDays);
                    }
                }
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_PurchaseBill.GeneratePrintReport();
            });

            $('.chkCell').prop('checked', true);

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_PurchaseBill.GetPurchaseBillByCode(id);
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

            $('body').on('click', '#docBrowseBtn', function () {
                $('#DOC').val('');
                $('#hdnDOC').val('');
                $('#DOCName').val('');
                $('#DOC').click();
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/PurchaseBill/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_PurchaseBill.GetPurchaseBillByCode(data.data.code);
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
            //                if (empr_PurchaseBill.ValidateMainInfo()) {
            //                    empr_PurchaseBill.Save();
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
            //            if (empr_PurchaseBill.ValidateMainInfo()) {
            //                empr_PurchaseBill.Save();
            //            }
            //            setTimeout(function () {
            //                $("#Loader").hide();
            //            }, 500);
            //        }, 200);
            //    }
            //});

            $('body').on('click', '#BtnSave', function (e) {
                e.preventDefault();

                if ($('#BtnSave').prop('disabled')) {
                    return false;
                }

                //$('#BtnSave').prop('disabled', true).hide();

                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                        $('#BtnSave').prop('disabled', false).show();
                        //setTimeout(function () {
                        //    $("#Loader").hide();
                        //}, 500);

                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                        $('#BtnSave').prop('disabled', false).show();
                        //setTimeout(function () {
                        //    $("#Loader").hide();
                        //}, 500);

                    } else {
                        processSave();

                    }
                } else {
                    processSave();
                }
            });

            function processSave() {
                $("#Loader").show();
                $("#Loader").css('display', 'flex');

                setTimeout(function () {
                    if (empr_PurchaseBill.ValidateMainInfo()) {
                        empr_PurchaseBill.Save();
                    } else {
                        $("#Loader").hide();
                        $('#BtnSave').prop('disabled', false).show();
                    }

                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);

                }, 200);
            }

            $('body').on('click', '#BtnDelete', function () {
                empr_PurchaseBill.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                $("#SCODE").dxSelectBox("instance")?.option("value", "");
                empr_PurchaseBill.ResetForm();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_PurchaseBill.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_PurchaseBill.GeneratePrintReport();
            });

            //$('body').on('click', '#BtnGetItems', function () {
            //    var itemId = $("#itemIdHidden").val();
            //    var itemQty = $("#IQty").val();
            //    if (itemId != '' && itemQty != '') {
            //        if (Permissions != "Admin") {
            //            if (!$("#Code").val() && !Permissions.r_ADD) {
            //                empr_helper.notify("You are not allowed to add new record !", 2);
            //            }
            //            else {
            //                empr_PurchaseBill.GetPurchaseBillDetailByItem(itemId, itemQty);
            //            }
            //        } else {
            //            empr_PurchaseBill.GetPurchaseBillDetailByItem(itemId, itemQty);
            //        }
            //    } else {
            //        empr_helper.notify("Please fill all fields.", 2);
            //    }
            //});

            $('body').on('click', '#BtnSodaPick', function () {
                empr_PurchaseBill.InitSodaPickGrid();
            });

            $('#COMM').on('input', function () {
                empr_PurchaseBill.CalculateCommition();
            });

            $('#COMM_VAL').on('input', function () {
                empr_PurchaseBill.CalculateCommition();
            });

            $('body').on('click', '#BtnAddBarcodes', function () {
                var selectedBarcodes = $('#BarcodePickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedBarcodes.length > 0) {
                    empr_PurchaseBill.AddBarcodeToGrid();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                //if (dcType == 'SO') {

                //}
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_PurchaseBill.AddSalesQuotationToOrder(selectedSodas);
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnAddSodaDetailToDelivery', function () {
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_PurchaseBill.AddSodaToDelivery();
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

            $('body').on('click', '#BtnGenerateSticker', function () {
                debugger;
                empr_PurchaseBill.GenerteCartonSticker();
            });
            $('body').on('click', '#BtnGenerateBarcode', function () {
                debugger;
                //$("#Loader").show().css('display', 'flex');
                empr_PurchaseBill.PrepareDataForValidation();
            });
        });
    },

    GetCurrentStock: function () {
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetCurrentStock', function (data) {
            debugger;
            if (data.length > 0) {
                empr_PurchaseBill.CurrentStock = data;
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitSodaPickGrid: function () {
        debugger;
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var REGION = 0;
        if ($('#REGION').data('dxSelectBox')) {
            REGION = $('#REGION').dxSelectBox('option', 'value');
        }

        if (PARTY_CODE == "" || PARTY_CODE == null || PARTY_CODE == undefined || PARTY_CODE == 0) {
            empr_helper.notify("Please select party first.", 2);
        } else {
            empr_PurchaseBill.GetPickDataByParty(PARTY_CODE, ACT_CODE, REGION);
        }

    },

    AddSalesQuotationToOrder: function (selectedRows) {
        if (!selectedRows || selectedRows.length == 0) {
            empr_helper.notify("Please select the items first.", 2);
            return;
        }

        var firstRow = selectedRows[0];
        if (firstRow.remarks) {
            $('#REMARKS').val(firstRow.remarks);
        }

        var mappedRows = selectedRows.map(function (item) {
            var qty = parseFloat(item.qty) || 0;
            var rate = parseFloat(item.rate) || 0;
            var amount = parseFloat(item.amount != null ? item.amount : item.amt);
            if (isNaN(amount)) {
                amount = qty * rate;
            }
            var pickDtCode = item.picK_ID_D > 0 ? item.picK_ID_D : (item.dT_CODE || 0);
            var stockRecord = empr_PurchaseBill.CurrentStock.find(function (s) {
                return s.itemId == item.iteM_CODE;
            });

            return {
                ...item,
                dT_CODE: 0,
                __KEY__: empr_PurchaseBill.GenerateKey(36),
                qty: qty,
                rate: rate,
                amt: amount,
                amount: amount,
                neT_AMT: amount,
                picK_ID: item.picK_ID || item.traN_ID || 0,
                picK_ID_D: pickDtCode,
                currentStock: stockRecord ? stockRecord.balance : 0
            };
        });

        var grid = $('#DetailContainer').dxDataGrid('instance');
        grid.saveEditData();
        var existingData = grid.option('dataSource') || [];
        var hasItem = existingData.some(function (row) {
            return row.iteM_CODE != "" && row.iteM_CODE != null && row.iteM_CODE != undefined;
        });

        if (hasItem) {
            grid.option('dataSource', existingData.concat(mappedRows));
        } else {
            grid.option('dataSource', mappedRows);
        }

        empr_PurchaseBill.pickIds = (grid.option('dataSource') || []).map(function (x) {
            return x.picK_ID_D;
        }).filter(function (id) {
            return id > 0;
        });
        if (empr_PurchaseBill.pickIds.length > 0) {
            $('#pickItems').hide();
        }
        grid.refresh();
        $('.modal').modal('hide');
        $('#V_DATE').focus();
    },

    GetPickDataByParty: function (PARTY_CODE, ACT_CODE, REGION) {
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetPickDataByParty?partyCode=' + PARTY_CODE + '&actCode=' + ACT_CODE + '&region=' + REGION, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if (dcType == 'SO') {
                        if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                            $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                        }
                        empr_PurchaseBill.CreatePickGrid(data.data);
                        $('#SodaPickModal').modal('show');
                    }
                    else{
                        empr_PurchaseBill.CreatePickDetailGrid(data.data);

                        var modal = new bootstrap.Modal(document.getElementById('SodaPickDetailModal'));
                        modal.show();
                    }

                } else {
                    empr_helper.notify(dcType == 'SO' ? "Sales Quotation not found." : "Purchase Order not found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    SetData: function (dataSource) {
        $.each(dataSource, function (index, item) {
            if (Type != "I") {
                item.iteM_CODE = parseInt(item.barcodE_CODE);
            }
            item.dT_CODE = 0;
            item.__KEY__ = empr_PurchaseBill.GenerateKey(36);
        });

        return dataSource;
    },

    CreatePickGrid: function (dataSrc) {
        var col = [];
        var selectionMode = "multiple";
        //selectionMode = "multiple";
        col = [
            { dataField: 'id', caption: 'Code', visible: false, },
            { dataField: 'iteM_NAME', caption: 'Item', allowEditing: false, },
            { dataField: 'qty', caption: 'Qty', allowEditing: false, },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, },
            { dataField: 'amt', caption: 'Amount', allowEditing: false, },
        ];
        empr_helper.dxGridbindingVouchers('#SodaPickGridContainer', col, dataSrc, "PurchaseBillPick", selectionMode);
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    CreatePickDetailGrid: function (dataSrc) {
        console.log('PickData', dataSrc);
        var col = [];
        var updatedData = [];
        if (empr_PurchaseBill.formName == 'MPO') {
            var grid = $('#DetailContainer').dxDataGrid('instance');
            grid.saveEditData();
            //var data = grid.option('dataSource');

            //existingdata = $('#DetailContainer').dxDataGrid('instance').option('dataSource');

            existingdata = grid.option('dataSource');
            debugger;
            updatedData = dataSrc
                .map(item => {
                    debugger;
                    const matchingRows = existingdata.filter(row => row.picK_ID_D === item.picK_ID_D && (row.dT_CODE == null || row.dT_CODE == 0));
                    const totalPack = matchingRows.reduce((sum, row) => parseInt(sum) + parseInt(row.qty), 0);
                    item.totaL_PACK -= totalPack;
                    item.rqty += totalPack;

                    const totalQty = matchingRows.reduce((sum, row) => parseInt(sum) + parseInt(row.qty), 0);
                    item.qty -= totalQty;

                    var Amount = item.totaL_PACK * item.rate;
                    item.amt = Amount;

                    var Dis = parseFloat(item.disc) || 0;
                    var disSum = Amount * Dis / 100 || 0;
                    var Adv = parseFloat(item.adv) || 0;
                    var Tax = parseFloat(item.tax) || 0;
                    var TaxAmount = Amount - disSum || 0;

                    var TaxSum = TaxAmount * Tax / 100 || 0;
                    var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

                    item.disC_AMT = disSum.toFixed(2);
                    item.taX_AMT = TaxSum.toFixed(2);
                    item.adV_AMT = AdvSum.toFixed(2);

                    if (!isNaN(Amount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                        item.neT_AMT = (Amount + TaxSum + AdvSum).toFixed(2);
                    } else {
                        item.neT_AMT = 0;
                    }

                    return item;
                })
                .filter(item => item !== null);
            empr_PurchaseBill.originalValues = {};
            updatedData.forEach(row => {
                empr_PurchaseBill.originalValues[row.picK_ID_D] = row.totaL_PACK;
            });

            col = [
                { dataField: 'lB_DATE', caption: 'Date', visible: true, allowEditing: false, },
                {
                    dataField: 'voucheR_NO', caption: 'Transaction #', width: 230, visible: true, allowEditing: false,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_PurchaseBill.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.id) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'partY_NAME', caption: 'Party Name', visible: true, width: 150, allowEditing: false, fixed: false },
                { dataField: 'ref', caption: 'Ref', visible: true, allowEditing: false, fixed: false },
                { dataField: 'iteM_NAME', caption: 'Item', visible: true, width: 200, allowEditing: false, fixed: false },
                //{ dataField: 'warehousE_NAME', caption: 'Warehouse', visible: true, width: 200, allowEditing: false, fixed: false },
                //{ dataField: 'deL_DATE', caption: 'Del Date', visible: true, allowEditing: false, dataType: 'date', format: 'dd-MM-yyy' },
                { dataField: 'iqty', caption: 'I.Qty', dataType: 'number', width: 90, allowEditing: false, },
                {
                    dataField: 'totaL_PACK',
                    caption: 'B.Qty',
                    allowSorting: false,
                    allowFiltering: false,
                    allowEditing: false,
                    dataType: 'number',
                    width: 90,
                    setCellValue: function (newData, value, currentRowData) {
                        const originalQty = empr_PurchaseBill.originalValues[currentRowData.picK_ID_D];
                        if (value <= originalQty) {
                            newData.totaL_PACK = value;
                        } else {
                            newData.totaL_PACK = originalQty;
                        }
                        var rate = parseFloat(currentRowData.rate) || 0;
                        var totaL_PACK = parseFloat(newData.totaL_PACK) || 0;
                        var qtY2 = parseFloat(currentRowData.qtY2) || 0;
                        if (isNaN(totaL_PACK)) {
                            empr_helper.notify("Please enter the correct quantity.", 2);
                        }
                        if (isNaN(qtY2)) {
                            empr_helper.notify("Please enter the correct quantity2.", 2);
                        }
                        if (!isNaN(totaL_PACK) && !isNaN(qtY2)) {

                            if (currentRowData.chK1) {
                                newData.baL_QTY = totaL_PACK + qtY2;
                            }
                            else {
                                newData.baL_QTY = totaL_PACK;
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
                },
                { dataField: 'rqty', caption: 'R.Qty', dataType: 'number', width: 90, allowEditing: false, },
                { dataField: 'uniT_NAME', caption: 'Unit', visible: true, allowEditing: false, },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 90, allowEditing: false, },
                { dataField: 'amt', caption: 'Amt', visible: true, allowEditing: false, dataType: 'number', format: "#,##0.##", },
                //{ dataField: 'disc', caption: 'Disc %', visible: true, allowEditing: false, dataType: 'number', format: "#,##0.##", },
                //{ dataField: 'disC_AMT', caption: 'Disc Amt', visible: true, allowEditing: false, dataType: 'number', format: "#,##0.##", },
                //{ dataField: 'tax', caption: 'Tax %', visible: true, allowEditing: false, dataType: 'number', format: "#,##0.##", },
                //{ dataField: 'taX_AMT', caption: 'Tax Amt', visible: true, allowEditing: false, dataType: 'number', format: "#,##0.##", },
                //{ dataField: 'neT_AMT', caption: 'Net Amt', visible: true, allowEditing: false, dataType: 'number', format: "#,##0.##", },
                { dataField: 'coloR_NAME', caption: 'Color', visible: true, allowEditing: false, },
                { dataField: 'sizE_NAME', caption: 'Size', visible: true, allowEditing: false, },
                //{ dataField: 'gradE_NAME', caption: 'Grade', visible: true, allowEditing: false, },
                { dataField: 'dT_CODE', caption: 'Net Amt', visible: false, allowEditing: false, },
                { dataField: 'partY_CODE', caption: 'Party Code', visible: false, allowEditing: false, },
                { dataField: 'partY_DDL', visible: false, allowEditing: false, },
                { dataField: 'iteM_CODE', visible: false, allowEditing: false, },
                { dataField: 'comm', visible: false, allowEditing: false, },
                { dataField: 'comM_AMT', visible: false, allowEditing: false, },
                { dataField: 'comM_VAL', visible: false, allowEditing: false, },
            ];

            //updatedData = dataSrc;
        }
        empr_helper.editableDxGridbinding('#SodaPickDetailGridContainer', col, updatedData, "SodaPickDetailGrid");
        setTimeout(function () {
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').refresh();
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    CreateGrid: function (dataSrc, dueCalc) {

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
            if (item.picK_ID_D > 0) {
                empr_PurchaseBill.pickIds.push(item.picK_ID_D);
                $('#pickItems').hide();
            }
        });
        if (dataSrc.length > 0) {
            empr_PurchaseBill.rowsCount = dataSrc.length - 1;
        }
        var data_source;
        if (Items != null && Items.length > 0) {
            data_source = Items;
        }
        //} else {
        //    data_source = function () {
        //        return empr_PurchaseBill.ItemsOnParty;
        //    };
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PurchaseBill.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PurchaseBill.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PurchaseBill.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const searchAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_PurchaseBill.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PurchaseBill.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PurchaseBill.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PurchaseBill.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                           </div>`).appendTo(container);
                    }
                    //<a href="javascript:;" class="grid-action-icon Search" style="margin-left: 8px" onclick="empr_PurchaseBill.InitBarcodePickGrid()" title="Search"><i class="fa fa-search"></i></a>
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
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
                //width: 320,
                minWidth: 150,
                fixed: true,
                //allowSorting: false,
                lookup: {
                    dataSource: {
                        store: data_source,
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    showClearButton: true,
                    //paging: {
                    //    enabled: true,
                    //    pageSize: 50,
                    //}
                },

                //lookup: {
                //    dataSource: data_source,
                //    displayExpr: 'value',
                //    valueExpr: 'key',
                //    searchEnabled: true,
                //    allowClearing: true,
                //    showClearButton: true,
                //    paging: {
                //        enabled: true,
                //        pageSize: 50
                //    }
                //},
                //calculateDisplayValue: function (rowData) {

                //    var itemss = empr_PurchaseBill.ItemsOnParty;

                //    const item = Array.isArray(itemss) ? itemss.find(x => x.key === rowData.iteM_CODE) : null;
                //    return item ? item.value : '';
                //},
                //setCellValue: function (newData, value, currentRowData) {
                //    //newData.iteM_CODE = value;
                //    //await empr_PurchaseBill.populateRowData(newData, value);

                //    this.defaultSetCellValue(newData, value, currentRowData);
                //    debugger;

                //    empr_PurchaseBill.setPurchaseBillItemData(newData, value, currentRowData.qty);

                //    var stockRecord = empr_PurchaseBill.CurrentStock.find(s => s.itemId == value);

                //    if (stockRecord) {
                //        newData.currentStock = stockRecord ? stockRecord.balance : 0;
                //    } else {
                //        newData.currentStock = 0;
                //    }
                //}
                setCellValue: async function (newData, value, currentRowData) {
                    debugger;
                    var itemss = empr_PurchaseBill.ItemsOnParty;

                    const item = Array.isArray(itemss) ? itemss.find(x => x.key === value) : null;
                    debugger;
                    await empr_PurchaseBill.populateRowData(newData, value, currentRowData);
                    debugger;

                    if (item) {
                        newData.barcode = item.adv;
                        newData.size = item.size;
                        newData.color = item.color;
                    }
                    this.defaultSetCellValue(newData, value, currentRowData);

                        var stockRecord = empr_PurchaseBill.CurrentStock.find(s => s.itemId == value);

                        if (stockRecord) {
                            newData.currentStock = stockRecord ? stockRecord.balance : 0;
                        } else {
                            newData.currentStock = 0;
                        }

                },


            },

            {
                dataField: 'currentStock',
                caption: 'Current Stock',
                allowEditing: false,
                alignment: 'right',
                cellTemplate: function (container, options) {
                    var value = options.value;
                    var color = value < 0 ? 'red' : 'black';

                    $('<span>')
                        .text(value)
                        .css('color', color)
                        .css('font-weight', value < 0 ? 'bold' : 'normal') // Optional: minus ko bold bhi krdega
                        .appendTo(container);
                },
            },
            {
                dataField: 'lasT_RATE',
                caption: 'Last Rate ',
                allowEditing: false,
                alignment: 'right',
            },
            {
                dataField: 'barcode',
                caption: 'Barcode ',
                width: 100,
                allowEditing: false,
                alignment: 'right',
            },
            //{
            //    dataField: 'hS_CODE',
            //    caption: 'HS Code',
            //    alignment: 'center',
            //    allowEditing: false,
            //},
            {
                dataField: 'qty',
                caption: 'Qty',
                width: 75,
                alignment: 'right',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
                setCellValue: function (newData, value, currentRowData) {
                    // dcType ki condition ko comment kar diya gaya hai
                    //if (dcType == 'SB' || dcType == 'SO') {
                    //    var updatedItems = empr_PurchaseBill.ItemsOnParty;
                    //}
                    //else {
                    //    var updatedItems = Items;
                    //}

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

                            // dcType logic commented
                            //if (dcType == 'SB' || dcType == 'SO') {
                            //    newData.amt = (newData.baL_QTY * pack * rate).toFixed(2);
                            //} else {
                            //    newData.amt = (newData.baL_QTY * rate).toFixed(2);
                            //}

                            var Amount = parseFloat(newData.amt) || 0;
                            var Dis = parseFloat(currentRowData.disc) || 0;
                            var disSum = Amount * Dis / 100 || 0;

                            newData.disC_AMT = disSum.toFixed(2);
                            var NetAmount = Amount - disSum || 0;
                            newData.neT_AMT = NetAmount.toFixed(2);

                            setTimeout(function () {
                                empr_PurchaseBill.CalculateCommition();
                            }, 0);
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
            {
                dataField: 'totaL_PACK',
                caption: 'Pack',
                width: 75,
                allowEditing: false,
                alignment: 'right',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                width: 100,
                alignment: 'center',
                defaultCellValue: function () {
                    if (Units && Units.length > 0) {
                        return Units[0].key; // Pehli value automatically select ho jayegi
                    }
                    return null;
                },
                lookup: {
                    dataSource: Units,
                    allowClearing: true,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },

                calculateCellValue: function (rowData) {
                    if (rowData.unit !== undefined && rowData.unit !== null && rowData.unit !== "") {
                        return rowData.unit;
                    }
                    if (Units && Units.length > 0) {
                        return Units[0].key;
                    }
                    return rowData.unit;
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.unit = value;
                }
            },
            {
                dataField: 'color',
                caption: 'Color',
                alignment: 'center',
                lookup: {
                    dataSource: Colors,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'size',
                caption: 'Size',
                alignment: 'center',
                width: 120,
                //allowEditing: false,
                lookup: {
                    dataSource: Sizes,
                    allowClearing: true,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            //{
            //    dataField: 'pack',
            //    caption: 'Pcs',
            //    width: 75,
            //    alignment: 'right',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    setCellValue: function (newData, value, currentRowData) {
            //        newData.pack = value || 0;

            //        var pack = value || 0;
            //        var qty = newData.qty ?? currentRowData.qty ?? 0;
            //        var rate = parseFloat(currentRowData.rate) || 0;
            //        var tPack = pack * qty;

            //        newData.totaL_PACK = tPack;

            //        if (dcType == 'SB' || dcType == 'SO') {
            //            if (!isNaN(qty) && !isNaN(rate)) {
            //                newData.amt = (qty * pack * rate).toFixed(2);
            //            } else {
            //                newData.amt = 0;
            //            }
            //        }
            //    }

            //},

            {
                dataField: 'weight',
                caption: 'Weight',
                width: 80,
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
                alignment: 'right',
                visible: false

            },
            {
                dataField: 'rate',
                caption: 'Rate',
                width: 80,
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
                setCellValue: function (newData, value, currentRowData) {
                    newData.rate = value;
                    var rate = parseFloat(newData.rate) || 0;
                    var qty = parseFloat(currentRowData.baL_QTY) || 0;
                    if (!isNaN(qty) && !isNaN(rate)) {
                        newData.amt = (qty * rate).toFixed(2);
                    } else {
                        newData.amt = 0;
                    }

                    var Amount = parseFloat(newData.amt) || 0;
                    var Discount = parseFloat(currentRowData.disc) || 0;
                    var DiscountAmount = Amount * Discount / 100 || 0;

                    newData.disC_AMT = DiscountAmount.toFixed(2);

                    // Net Amount calculation
                    var NetAmount = Amount - DiscountAmount || 0;

                    if (!isNaN(NetAmount)) {
                        newData.neT_AMT = NetAmount.toFixed(2);
                    } else {
                        newData.neT_AMT = 0;
                    }
                }
            },
            {
                dataField: 'amt',
                caption: 'Amount',
                width: 100,
                allowEditing: false,
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 }
            },
            {
                dataField: 'disc',
                width: 75,
                caption: 'Disc %',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
                visible: false,

                setCellValue: function (newData, value, currentRowData) {
                    newData.disc = value;
                    var Amount = parseFloat(currentRowData.amt) || 0;
                    var Discount = parseFloat(newData.disc) || 0;

                    if (!isNaN(Amount) && !isNaN(Discount)) {
                        newData.disC_AMT = (Amount * Discount / 100).toFixed(2);
                    } else {
                        newData.disC_AMT = 0;
                    }

                    // Commented out Tax and Adv calculations
                    // var Tax = parseFloat(currentRowData.tax) || 0;
                    // var Adv = parseFloat(currentRowData.adv) || 0;
                    var DiscountAmount = parseFloat(newData.disC_AMT) || 0;

                    // var TaxAmount = Amount - DiscountAmount;
                    // var TaxSum = TaxAmount * Tax / 100 || 0;
                    // var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

                    // newData.taX_AMT = TaxSum.toFixed(2);
                    // newData.adV_AMT = AdvSum.toFixed(2);

                    var NetAmount = Amount - DiscountAmount || 0;

                    // Logic updated to only use NetAmount
                    if (!isNaN(NetAmount)) {
                        newData.neT_AMT = (NetAmount).toFixed(2);
                    } else {
                        newData.neT_AMT = 0;
                    }
                }
            },
            {
                dataField: 'disC_AMT',
                caption: 'Disc Amt',
                width: 100,
                allowEditing: false,
                dataType: 'number',
                visible: false,
                format: { type: 'fixedPoint', precision: 2 },
            },
            //{
            //    dataField: 'tax',
            //    caption: 'Tax',
            //    width: 75,
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    setCellValue: function (newData, value, currentRowData) {
            //        newData.tax = value;
            //        var Amount = parseFloat(currentRowData.amt) || 0;
            //        var DiscountAmount = parseFloat(currentRowData.disC_AMT) || 0;
            //        var Tax = parseFloat(newData.tax) || 0;
            //        var Adv = parseFloat(currentRowData.adv) || 0;
            //        var TaxAmount = Amount - DiscountAmount || 0;
            //        var TaxSum = TaxAmount * Tax / 100 || 0;
            //        var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

            //        if (!isNaN(TaxAmount) && !isNaN(Tax)) {
            //            newData.taX_AMT = TaxSum.toFixed(2);
            //            newData.adV_AMT = AdvSum.toFixed(2);
            //        } else {
            //            newData.taX_AMT = 0;
            //        }

            //        var NetAmount = Amount - DiscountAmount || 0;
            //        if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
            //            newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
            //        } else {
            //            newData.neT_AMT = 0;
            //        }
            //    }
            //},
            //{
            //    dataField: 'taX_AMT',
            //    caption: 'Tax Amount',
            //    width: 100,
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    allowEditing: false,
            //},
            //{
            //    dataField: 'adv',
            //    width: 75,
            //    caption: 'A.Tax',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    setCellValue: function (newData, value, currentRowData) {
            //        newData.adv = value;
            //        var Amount = parseFloat(currentRowData.amt) || 0;
            //        var DiscountAmount = parseFloat(currentRowData.disC_AMT) || 0;
            //        var Adv = parseFloat(newData.adv) || 0;
            //        var TaxAmount = Amount - DiscountAmount || 0;
            //        var Tax = parseFloat(currentRowData.tax) || 0;
            //        var TaxSum = TaxAmount * Tax / 100 || 0;
            //        var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

            //        if (!isNaN(TaxAmount) && !isNaN(Adv)) {
            //            newData.adV_AMT = AdvSum.toFixed(2);
            //        } else {
            //            newData.adV_AMT = 0;
            //        }

            //        var NetAmount = Amount - DiscountAmount || 0;
            //        if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
            //            newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
            //        } else {
            //            newData.neT_AMT = 0;
            //        }
            //    }
            //},
            //{
            //    dataField: 'adV_AMT',
            //    width: 100,
            //    caption: 'A.Tax Amt',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    allowEditing: false,
            //},
            {
                dataField: 'neT_AMT',
                width: 120,
                caption: 'Net Amount',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
                allowEditing: false,
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                width: 400,
                alignment: 'center',
                wordWrapEnabled: true,
            },

            //{
            //    dataField: 'size',
            //    caption: 'Size',
            //    alignment: 'center',
            //    width: 120,
            //    //allowEditing: false,
            //    lookup: {
            //        dataSource: Sizes,
            //        allowClearing: true,
            //        displayExpr: 'value',
            //        valueExpr: 'key'
            //    }
            //},
            //{
            //    dataField: 'grade',
            //    caption: 'Grade',
            //    alignment: 'center',
            //    width: 120,
            //    lookup: {
            //        dataSource: Grades,
            //        allowClearing: true,
            //        displayExpr: 'value',
            //        valueExpr: 'key'
            //    },

            //},
            {
                dataField: "warehouse",
                caption: "Warehouse",
                visible: false,
                alignment: 'center',
                calculateCellValue: function (rowData) {
                    let item = Warehouse.find(x => x.key === rowData.warehouse);
                    return item ? item.value : "";
                },
                editorType: "dxDropDownBox",
                editorOptions: {
                    dataSource: Warehouse,
                    valueExpr: "key",
                    displayExpr: "value",
                    contentTemplate: function (e, cellInfo) {
                        let $grid = $("<div>").dxDataGrid({
                            dataSource: Warehouse,

                            keyExpr: "key",
                            columns: [
                                { dataField: "grcode", caption: "Group Code", width: 80 },
                                { dataField: "value", caption: "Name" },
                                { dataField: "name", caption: "Control " }
                            ],
                            selection: { mode: "single" },
                            hoverStateEnabled: true,
                            height: 200,
                            searchPanel: {
                                visible: true,
                                width: 480,
                                placeholder: "Search..."
                            },
                            onSelectionChanged: function (selectedItems) {
                                let selectedKey = selectedItems.selectedRowKeys[0];
                                e.component.option("value", selectedKey);   // dropdown ke liye
                                cellInfo.setValue = selectedKey;             // parent grid ke liye
                                e.component.close();
                            }
                        });

                        return $grid;
                    }
                },
                width: 500,
            },
            {
                dataField: 'deL_DATE',
                caption: 'Delivery Date',
                alignment: 'center',
                dataType: 'date',
                visible: false,

                format: "dd-MM-yyyy",
            },
            {
                dataField: 'duE_DATE',
                caption: 'Due Date',
                alignment: 'center',
                dataType: 'date',
                format: "dd-MM-yyyy",
                allowEditing: false,
                visible: false,

            },
            {
                dataField: 'duE_DAYS',
                caption: 'Due Days',
                alignment: 'center',
                allowEditing: false,
                visible: false,

            },
            {
                dataField: 'veh',
                caption: 'Vehicle #',
                alignment: 'center',
                visible: false,

            },
            {
                dataField: 'id',
                visible: false,
            },
            {
                dataField: 'voucheR_NO', caption: 'Pick Tran#',
                allowEditing: false,
                visible: false,

                alignment: 'center',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_PurchaseBill.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.id) + ')')
                        .appendTo(container);
                }
            }
        ];

        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, menuName, "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        setTimeout(function () {
            //var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
            //$('#DetailContainer').dxDataGrid('instance').focus(nextElement);

            let gridInstance = $("#DetailContainer").dxDataGrid("instance");
            let iteM_CODE = gridInstance.cellValue(0, "iteM_CODE");
            let picK_ID_D = gridInstance.cellValue(0, "picK_ID_D");
            if (dueCalc) {
                if (!iteM_CODE || picK_ID_D) {
                    var deliveryDate = $('#RINV_DATE').val();
                    let payTerm = parseInt($('#TERMS').val(), 10) || 0;

                    if (!deliveryDate) {
                        let today = new Date();
                        today.setHours(0, 0, 0, 0);
                        deliveryDate = today;
                    }

                    let result = empr_PurchaseBill.calculateDue(deliveryDate, payTerm);

                    gridInstance.cellValue(0, "duE_DATE", result.dueDate);
                    gridInstance.cellValue(0, "duE_DAYS", result.dueDays);
                }
            }






        }, 500);
    },

    CreateCommGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_PurchaseBill.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PurchaseBill.CommCloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PurchaseBill.CommAddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PurchaseBill.CommDeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PurchaseBill.CommCloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PurchaseBill.CommAddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PurchaseBill.CommDeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'bilL_TRAN_ID',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'traN_ID',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'iteM_CODE',
                caption: "Items",
                alignment: 'center',
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
                dataField: 'comM_UNIT',
                caption: 'Commission Unit',
                alignment: 'center',
                lookup: {
                    dataSource: {
                        store: empr_helper.commType,
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
                dataField: 'comM_VALUE',
                caption: 'Commission value',
                alignment: 'center',
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#CommContainer', col, dataSrc, "PurchaseBill", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#CommContainer').dxDataGrid('instance').addRow().done(function () {
                $('#CommContainer').dxDataGrid('instance').saveEditData();
            });
        }

        setTimeout(function () {
            var nextElement = $('#CommContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
            $('#CommContainer').dxDataGrid('instance').focus(nextElement);
        }, 500);
    },

    CloneRow: function (index) {
        //if (empr_PurchaseBill.pickIds.length > 0) {
        //    empr_helper.notify("Cannot clone row on return data.", 2);
        //} else {
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_PurchaseBill.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_PurchaseBill.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            });
        }
        else {
            empr_PurchaseBill.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_PurchaseBill.GenerateKey(36);
                clonedRowData.dT_CODE = 0;
                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
            }
        }
        //}
    },

    CommCloneRow: function (index) {
        if (empr_PurchaseBill.pickIds.length > 0) {
            empr_helper.notify("Cannot clone row on return data.", 2);
        } else {
            const gridIns = $('#CommContainer').dxDataGrid('instance');
            const dataSrc = gridIns.option("dataSource");

            if (dataSrc.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if ($('#CommContainer').dxDataGrid('instance').hasEditData()) {
                $('#CommContainer').dxDataGrid('instance').saveEditData().done(function () {

                    empr_PurchaseBill.rowsCount += 1;
                    const gridInstance = $('#CommContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    if (dataSource.length > 0) {
                        let clonedRowData = $.extend(true, {}, dataSource[index]);
                        if (clonedRowData.hasOwnProperty('dT_CODE')) {
                            delete clonedRowData.dT_CODE;
                        }
                        clonedRowData.__KEY__ = empr_PurchaseBill.GenerateKey(36);
                        clonedRowData.dT_CODE = 0;
                        let newDataSource = [clonedRowData].concat(dataSource);
                        gridInstance.option("dataSource", newDataSource);
                        gridInstance.refresh();
                    }
                });
            }
            else {
                empr_PurchaseBill.rowsCount += 1;
                const gridInstance = $('#CommContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_PurchaseBill.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            }
        }
    },

    AddRow: function () {
        //if (empr_PurchaseBill.pickIds.length > 0) {
        //    empr_helper.notify("Cannot add row on return data.", 2);
        //}
        //else
        //{
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_PurchaseBill.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                //dataSource.unshift({ __KEY__: empr_PurchaseBill.GenerateKey(36), dT_CODE: 0 });
                let payTerm = parseInt($('#TERMS').val(), 10) || 0;



                var deliveryDate = $('#RINV_DATE').val();

                if (!deliveryDate) {
                    let today = new Date();
                    today.setHours(0, 0, 0, 0);
                    deliveryDate = today;
                }

                let result = empr_PurchaseBill.calculateDue(deliveryDate, payTerm);

                // New row with deL_DATE
                const newRow = {
                    __KEY__: empr_PurchaseBill.GenerateKey(36),
                    dT_CODE: 0,
                    //deL_DATE: deliveryDate,
                    duE_DATE: result.dueDate,
                    duE_DAYS: result.dueDays,
                    adv: empr_PurchaseBill.PartysaleTax,
                    deL_DATE: todayDate,
                    warehouse: 2,
                    disc: empr_PurchaseBill.PartyDisc
                };

                dataSource.unshift(newRow);
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
            });
        }
        else {
            empr_PurchaseBill.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            //dataSource.unshift({ __KEY__: empr_PurchaseBill.GenerateKey(36), dT_CODE: 0 });
            let payTerm = parseInt($('#TERMS').val(), 10) || 0;

            var deliveryDate = $('#RINV_DATE').val();

            if (!deliveryDate) {
                let today = new Date();
                today.setHours(0, 0, 0, 0);
                deliveryDate = today;
            }

            let result = empr_PurchaseBill.calculateDue(deliveryDate, payTerm);

            // New row with deL_DATE
            const newRow = {
                __KEY__: empr_PurchaseBill.GenerateKey(36),
                dT_CODE: 0,
                //deL_DATE: deliveryDate,
                duE_DATE: result.dueDate,
                duE_DAYS: result.dueDays,
                adv: empr_PurchaseBill.PartysaleTax,
                deL_DATE: todayDate,
                warehouse: 2,
                disc: empr_PurchaseBill.PartyDisc
            };

            dataSource.unshift(newRow);
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
            empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
        }
        //}
    },

    CommAddRow: function () {
        if (empr_PurchaseBill.pickIds.length > 0) {
            empr_helper.notify("Cannot add row on return data.", 2);
        } else {
            const gridIns = $('#CommContainer').dxDataGrid('instance');
            const dataSrc = gridIns.option("dataSource");

            if (dataSrc.length >= Limit && Limit != 0) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            if ($('#CommContainer').dxDataGrid('instance').hasEditData()) {
                $('#CommContainer').dxDataGrid('instance').saveEditData().done(function () {
                    empr_PurchaseBill.rowsCount += 1;
                    const gridInstance = $('#CommContainer').dxDataGrid('instance');
                    const dataSource = gridInstance.option("dataSource");

                    dataSource.unshift({ __KEY__: empr_PurchaseBill.GenerateKey(36), dT_CODE: 0, traN_ID: empr_PurchaseBill.CommissionTranId });
                    gridInstance.option("dataSource", dataSource);
                    gridInstance.refresh();
                    empr_helper.MoveFocusToGridWithouTab('#CommContainer', 0, 'iteM_CODE')
                });
            }
            else {
                empr_PurchaseBill.rowsCount += 1;
                const gridInstance = $('#CommContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_PurchaseBill.GenerateKey(36), dT_CODE: 0, traN_ID: empr_PurchaseBill.CommissionTranId });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                empr_helper.MoveFocusToGridWithouTab('#CommContainer', 0, 'iteM_CODE')
            }
        }
    },

    DeleteRow: function (index, dtCode) {
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_PurchaseBill.rowsCount -= 1;
                    gridInstance.saveEditData();
                }
                else {
                    var availableRows = dataSource.filter(x => x.dT_CODE > 0);
                    if (availableRows.length > 0) {
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/PurchaseBill/DeletePurchaseBillDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_PurchaseBill.rowsCount -= 1;
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
    populateRowData: async function (rowData, itemKey, currentRowData) {
        debugger;
        rowData.iteM_CODE = itemKey;
        if (itemKey) {
            //if (Type == "I") {
            try {
                debugger;
                const rate = await empr_PurchaseBill.GetLastRate(itemKey);
                rowData.lasT_RATE = rate;
                rowData.rate = rate;
                var qty = parseFloat(currentRowData.qty) || 0;
                var updatedAmt = rate * qty;

                rowData.amt = updatedAmt;
                rowData.neT_AMT = updatedAmt;
            } catch (e) {
                rowData.rate = 0;
                rowData.lasT_RATE = 0;
            }
            //}
        } else {
            rowData.rate = 0;
        }
        return rowData;
    },

    GetLastRate: function (bcode) {
        return new Promise((resolve, reject) => {
            var dataModel = empr_PurchaseBill.GetDataToSave();
            dataModel.Master.BARCODE_ID = bcode;
            //dataModel.bcode = bcode;
            ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseBill/GetLastRate", function (data) {
                if (data.msgType == 1 && data.data && data.data.length > 0) {
                    debugger;
                    resolve(data.data[0].rate);
                } else {
                    resolve(0);
                }
            }, function (err) {
                resolve(0);
            });
        });
    },

    CommDeleteRow: function (index, dtCode) {
        const gridInstance = $('#CommContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            /*if (dataSource.length > 1) {*/
            var row = dataSource[index];
            if (dtCode == '' || dtCode == null || dtCode == undefined) {
                gridInstance.deleteRow(index);
                empr_PurchaseBill.rowsCount -= 1;
                gridInstance.saveEditData();
            }
            else {
                //var availableRows = dataSource.filter(x => x.dT_CODE > 0);
                //if (availableRows.length > 1) {
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
                    ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/PurchaseBill/DeleteCommDetailByCode", function (data) {
                        empr_helper.notify(data.msg, data.msgType);
                        if (data.msgType == 1) {
                            gridInstance.deleteRow(index);
                            empr_PurchaseBill.rowsCount -= 1;
                            gridInstance.saveEditData();
                        }
                    }, false, true);
                });
                //} else {
                //    empr_helper.notify("You are not allowed to delete the last row.", 2);
                //}
            }
            //}
            //else {
            //    empr_helper.notify("You are not allowed to delete the last row.", 2);
            //}
        }
    },

    //ItemsBehalfOnParty: function (PARTY_CODE, ACT_CODE) {
    //    let resultItems = Items.filter(item => {
    //        return (
    //            (item.partyCode === PARTY_CODE && item.accountCode === ACT_CODE) ||
    //            (item.partyCode === 0 && item.accountCode === 0)
    //        );
    //    });
    //    empr_PurchaseBill.ItemsOnParty = resultItems;

    //},

    //ItemsBehalfOnParty: function (PARTY_CODE, ACT_CODE) {

    //    ajaxHelper.ajaxGetJson('/PurchaseBill/ItemsBehalfOnParty?partyCode=' + PARTY_CODE + '&actCode=' + ACT_CODE, function (data) {
    //        if (data.length > 0) {
    //            empr_PurchaseBill.ItemsOnParty = data;
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},

    ItemsBehalfOnParty: function (PARTY_CODE, ACT_CODE) {
        ajaxHelper.ajaxGetJson('/PurchaseBill/ItemsBehalfOnParty?partyCode=' + PARTY_CODE + '&actCode=' + ACT_CODE, function (data) {
            if (data.length > 0) {
                empr_PurchaseBill.ItemsOnParty = data;
                var grid = $('#DetailContainer').dxDataGrid('instance');

                grid.columnOption('iteM_CODE', 'lookup', {
                    dataSource: {
                        store: data, 
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    showClearButton: true
                    //dataSource: data,
                    //displayExpr: 'value',
                    //valueExpr: 'key',
                    //searchEnabled: true,
                    //allowClearing: true,
                    //showClearButton: true,
                    //paging: {
                    //    enabled: true,
                    //    pageSize: 50
                    //}
                });

                grid.refresh();  // Refresh karna agar data update karna hai
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
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
        empr_PurchaseBill.GetPurchaseBill();
        //empr_PurchaseBill.CreateQuickSearchGrid();
    },

    GetPurchaseBill: function () {
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetPurchaseBill', function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBill.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {
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
            //{ dataField: 'region', caption: 'Region', },
            { dataField: 'ref', caption: 'Refrence #', },
            { dataField: 'remarks', caption: 'Description', },
            { dataField: 'amt', caption: 'Amount', },
            { dataField: 'astatus', caption: 'Status', },
                //{ dataField: 'rinV_NO', caption: 'R.Inv No', },
                //{ dataField: 'rinV_DATE', caption: 'R.Inv Date', dataType: 'date', format: 'dd-MM-yyy' },
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
            //{ dataField: 'region', caption: 'Region', },
            { dataField: 'ref', caption: 'Refrence #', },
            { dataField: 'remarks', caption: 'Remarks', },
            { dataField: 'astatus', caption: 'Status', },
                //{ dataField: 'rinV_NO', caption: 'R.Inv No', },
                //{ dataField: 'rinV_DATE', caption: 'R.Inv Date', dataType: 'date', format: 'dd-MM-yyy' },
            ]
        }
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/PurchaseBill/GetPurchaseBill", "id", "PartyOpening");
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "PurchaseBillQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    GetPurchaseBillByCode: function (code) {
        empr_PurchaseBill.ResetForm();
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetPurchaseBillByCode?code=' + code, function (data) {
            debugger;
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                console.log('edit Data', data);
                if (masterData.length == 1) {
                    debugger;
                    $('#pickItems').show();
                    empr_PurchaseBill.pickIds = [];
                    var response = masterData[0];
                    empr_PurchaseBill.vDate = response.v_DATE;
                    empr_helper.invoiceNo = response.voucheR_NO;
                    var filteredData = $.grep(PartyType, function (item) {
                        return item.partyCode === response.partY_CODE && item.accountCode === response.acT_CODE;
                    });
                    //empr_PurchaseBill.ItemsBehalfOnParty(filteredData[0].partyCode, filteredData[0].accountCode);

                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);
                    $('#partyhidden').val(filteredData[0].partyCode);
                    $('#Currency').dxSelectBox('instance').option('value', response.curR_CODE);
                    $('#Rate').val(response.crate);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#CARTAGE').val(response.cartage);
                    //$('#HS_CODE').val(response.hS_CODE);
                    //$('#COMM').val(response.comm);
                    $('#TERMS').val(response.terms);
                    //$('#COMM_VAL').val(response.comM_VAL);
                    //$('#COMM_AMT').dxSelectBox('instance').option('value', response.comM_AMT);
                    //empr_PurchaseBill.InitSalesman(0, parseInt(response.scode));
                    //empr_PurchaseBill.InitRegionDDL(response.region);
                    //$('#SCODE').dxSelectBox('instance').option('value', parseInt(response.scode));
                    $('#DISC').val(response.disc);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);

                    $('#hdnDOC').val(response.doc);
                    var DocPath = response.doc;
                    var DocName = DocPath.split('/').pop();
                    $("#DOCName").val(DocName);

                    //$('#RINV_NO').val(response.rinV_NO);
                    //$('#RINV_DATE').val(response.rinV_DATE);
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
                $('#print-tab-li').removeClass('d-none');
                if (data.detail.msgType == 1) {

                    var updatedDetailData = data.detail.data.map(item => {
                        var stockRecord = empr_PurchaseBill.CurrentStock.find(s => s.itemId == item.iteM_CODE);

                        return {
                            ...item,
                            currentStock: stockRecord ? stockRecord.balance : 0
                        };
                    });
                    debugger;

                    empr_PurchaseBill.CreateGrid(updatedDetailData, false);
                }
                else {
                    empr_helper.notify(data.detail.msg, data.detail.msgType);
                }
                if (data.printData.msgType == 1) {
                    //setTimeout(function () {
                    empr_PurchaseBill.CreateItemPrintGrid(data.printData.data);
                    //}, 500);
                }

            }
            else {
                empr_helper.notify(data.master.msg, data.master.msgType);
            }
        }, false, true);
    },

    //GetPurchaseBillDetailByItem: function (code, qty) {
    //    ajaxHelper.ajaxGetJson('/PurchaseBill/GetPurchaseBillDetailByItem?code=' + code + '&qty=' + qty, function (data) {
    //        if (data.msgType == 1) {
    //            if (data.data.length > 0) {
    //                empr_PurchaseBill.rowsCount += data.data.length;
    //                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
    //                var dataSource = gridInstance.option("dataSource");
    //                $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
    //                    dataSource = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
    //                });
    //                if ((dataSource[0].dT_CODE == undefined || dataSource[0].dT_CODE == 0) && (dataSource[0].iteM_CODE == "" || dataSource[0].iteM_CODE == null || dataSource[0].iteM_CODE == undefined)) {
    //                    empr_PurchaseBill.CreateGrid(data.data);
    //                } else {
    //                    dataSource.unshift(...data.data);
    //                    gridInstance.option("dataSource", dataSource);
    //                    gridInstance.refresh();
    //                    empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
    //                }
    //            }
    //            $('.bs-example-modal-lg').modal('hide');
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},

    GetPurchaseBillDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetPurchaseBillDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBill.CreateGrid(data.data, false);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    //GetPurchaseBillPickDetailByCode: function (code) {
    //    ajaxHelper.ajaxGetJson('/PurchaseBill/GetPurchaseBillPickDetailByCode?code=' + code, function (data) {
    //        if (data.msgType == 1) {
    //            if (Type == "I") {
    //                empr_PurchaseBill.CreateGrid(data.data);
    //            } else {
    //                if (data.data.length > 0) {
    //                    if ($('#SodaPickDetailGridContainer').data('dxDataGrid') != undefined) {
    //                        $('#SodaPickDetailGridContainer').data('dxDataGrid').dispose();
    //                    }
    //                    console.log(data.data)
    //                    empr_PurchaseBill.CreatePickDetailGrid(data.data);
    //                    $('#SodaPickDetailModal').modal('show');
    //                } else {
    //                    empr_helper.notify("No data found.", 2);
    //                }
    //            }
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},
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
                allowEditing: false
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
    GetDataToSave: function () {
        var ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        //var COMM = $("#COMM").val();
        //var COMM_VAL = $("#COMM_VAL").val();
        //var COMM_AMT = $("#COMM_AMT").dxSelectBox('option', 'value');
        var CARTAGE = $("#CARTAGE").val();
        var DISC = $("#DISC").val();
        var TERMS = $("#TERMS").val();

        //var SCODE = $("#SCODE").dxSelectBox('option', 'value');
        var BTYPE = $("#BTYPE").dxSelectBox('option', 'value');
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        //var REGION = $('#REGION').dxSelectBox('option', 'value');
        var DOC = $("#hdnDOC").val();
        var CRATE = $("#Rate").val();
        var CURR_CODE = $('#Currency').dxSelectBox('option', 'value');
        //var RINV_NO = $("#RINV_NO").val();
        //var RINV_DATE = $("#RINV_DATE").val();

        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            CARTAGE: CARTAGE,
            //COMM_VAL: COMM_VAL,
            //COMM_AMT: COMM_AMT,
            DISC: DISC,
            //SCODE: SCODE,
            BTYPE: BTYPE,
            REF: REF,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS,
            //REGION: REGION,
            //HS_CODE: HS_CODE,
            CURR_CODE: CURR_CODE,
            CRATE: CRATE,
            DOC: DOC,
            //RINV_NO: RINV_NO,
            //RINV_DATE: RINV_DATE,
            TERMS: TERMS
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
        var commRecords = [];
        if ($('#CommContainer').dxDataGrid('instance').hasEditData()) {
            $('#CommContainer').dxDataGrid('instance').saveEditData().done(function () {
                commRecords = $('#CommContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            commRecords = $('#CommContainer').dxDataGrid('instance').option("dataSource");
        }
        if (empr_PurchaseBill.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords,
                Commission: commRecords
            };
            return modelRecord;
        }
        else {

            var detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

            $.each(detailRecords, function (index, item) {
                if (!(item.duE_DATE == "" || item.duE_DATE == null || item.duE_DATE == undefined)) {
                    item.duE_DATE = empr_helper.PrepareDate(item.duE_DATE);
                }

                if (!(item.deL_DATE == "" || item.deL_DATE == null || item.deL_DATE == undefined)) {
                    item.deL_DATE = empr_helper.PrepareDate(item.deL_DATE);
                }
            });

            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords,
                Commission: $('#CommContainer').dxDataGrid('instance').option("dataSource"),
            };
            return modelRecord;
        }
    },

    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_PurchaseBill.GetDataToSave();

        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.RINV_DATE == '') {
            empr_helper.notify("R.Inv date is required.", 2);
            valid = false;
            return valid;
        }
        debugger;

        if (data.Master.PARTY_CODE == '') {
            empr_helper.notify("Please select Party.", 2);
            valid = false;
            return valid;
        }
        //if (data.Master.REGION == undefined) {
        //    empr_helper.notify("Please select Region.", 2);
        //    valid = false;
        //    return valid;
        //}

        //if (dcType == 'SB') {
        //    if (data.Master.REF == '') {
        //        empr_helper.notify("Please enter Sales Invoice #.", 2);
        //        valid = false;
        //        return valid;
        //    }
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
                empr_helper.notify("Please enter correct item quantity at Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            //if (item.rate <= 0) {
            //    empr_helper.notify("Please enter correct Rate at Line No " + (index + 1), 2);
            //    valid = false;
            //    return valid;
            //}

            //if (item.amt <= 0) {
            //    empr_helper.notify("Amount is null Something Went Wrong" + (index + 1), 2);
            //    valid = false;
            //    return valid;
            //}

            if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
                empr_helper.notify("Please enter correct item quantity2 at Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            //if (stkStatus == 'Y') {
            //    if (item.currentStock <= 0) {
            //        empr_helper.notify("Stock unavailable at line No " + (1 + index), 2);
            //        valid = false;
            //        return false;
            //    }
            //    else if (item.qty > item.currentStock) {
            //        empr_helper.notify("Quantity exceeds available stock (" + item.currentStock + ") at line No " + (1 + index), 2);
            //        valid = false;
            //        return false;
            //    }
            //}

        });

        if (data.Detail.length > Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },

    Save: function () {
        var dataModel = empr_PurchaseBill.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        debugger;
        ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseBill/Save", function (data) {
            $('#BtnSave').prop('disabled', false).show();
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                debugger;
                if (dataClear == 1) {
                    empr_PurchaseBill.GetPurchaseBillByCode(data.data.code);
                    //empr_PurchaseBill.GetPurchaseBillDetailByCode(data.data.code);
                    $('#BtnDelete').show();
                }
                else {
                    $("#SCODE").dxSelectBox("instance")?.option("value", "");
                    empr_PurchaseBill.ResetForm();
                }

            }
        }, false, true);
    },

    ResetForm: function () {
        $('.detailInfo a').tab('show');
        if ($("#CommContainer").data("dxDataGrid")) {
            $("#CommContainer").dxDataGrid("dispose");
            $("#CommContainer").removeData("dxDataGrid");
        }
        if (dcType == 'SO') {
            $('#BtnSodaPick').show();
        }


        empr_PurchaseBill.CreateGrid([{ __KEY__: empr_PurchaseBill.GenerateKey(36), chK1: false, chk: "0", dT_CODE: 0, warehouse: 2, deL_DATE: todayDate }], true);
        empr_PurchaseBill.CreateCommGrid([{ __KEY__: empr_PurchaseBill.GenerateKey(96), chK1: false, chk: "0", dT_CODE: 0 }]);


        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
        $("#hdnDOC").val('');
        $("#DOCName").val('');
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        $('#DISC').val('');
        //$('#RINV_NO').val();
        //$('#RINV_DATE').val();
        //$('#HS_CODE').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('#BTYPE').dxSelectBox('instance').option('value', 'CR');

        $('#Code').val();
        $('.nav-item.commission').hide();
        $('#print-tab-li').addClass('d-none');
        $('#pills-warninghome-tab').tab('show');
        empr_helper.dcType = dcType;
        empr_PurchaseBill.pickIds = [];
        empr_PurchaseBill.ItemsOnParty = [];
        empr_PurchaseBill.firstClick = 0;
        empr_PurchaseBill.PartysaleTax = 0;
        empr_PurchaseBill.PartyDisc = 0;
        empr_PurchaseBill.CommissionTranId = 0;
        empr_PurchaseBill.isSalesman = false;
        //empr_PurchaseBill.CreateGrid([]);
        empr_PurchaseBill.InitPartyType();
        //empr_PurchaseBill.InitRegionDDL();
        empr_PurchaseBill.InitCurrencyDDL();
        //empr_PurchaseBill.InitCommissionAmtDDL("PR");
        $('#PARTY_CODE').dxSelectBox('instance').option('value', '');

        //$('#REF').focus();
        $('#pickItems').show();
        $('#V_DATE').val(todayDate);
        $('#RINV_DATE').val(todayDate);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/PurchaseBill/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    $("#SCODE").dxSelectBox("instance")?.option("value", "");
                    empr_PurchaseBill.ResetForm();
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
        empr_PurchaseBill.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
            empr_PurchaseBill.OnPartyChange(d);
        });

    },

    //OnPartyChange: function (d) {
    //    if (d.value == null || d.value == '') {
    //        $('#partyhidden').val('');
    //        $('#acthidden').val('');
    //        $('#TERMS').val('');
    //        $('#PARTY_BALANCE').val('');

    //        //empr_PurchaseBill.InitSalesman(0);
    //    }
    //    else {
    //        var filteredData = $.grep(PartyType, function (item) {
    //            return item.key === d.value;
    //        });
    //        $('#partyhidden').val(filteredData[0].partyCode);
    //        $('#acthidden').val(filteredData[0].accountCode);
    //        let balance = parseFloat(filteredData[0].balance || 0);

    //        if (balance < 0) {
    //            $('#PARTY_BALANCE')
    //                .val(`(${Math.abs(balance)})`)
    //                .css('color', 'red');
    //        } else {
    //            $('#PARTY_BALANCE')
    //                .val(balance)
    //                .css('color', 'black');
    //        }
    //        //empr_PurchaseBill.PartysaleTax = parseFloat(filteredData[0].sTax) || 0;
    //        //empr_PurchaseBill.PartyDisc = parseFloat(filteredData[0].disc) || 0;
    //        //empr_PurchaseBill.SetSaleTax(filteredData[0].sTax, filteredData[0].disc);
    //        empr_PurchaseBill.ItemsOnParty = [];
    //        empr_PurchaseBill.ItemsBehalfOnParty(filteredData[0].partyCode, filteredData[0].accountCode);
    //        //empr_PurchaseBill.CreateGrid([{ __KEY__: empr_PurchaseBill.GenerateKey(36), chK1: false, chk: "0", dT_CODE: 0, warehouse: 2, deL_DATE: todayDate }], true);
    //        var rinvDate = $('#RINV_DATE').val();

    //        let result = empr_PurchaseBill.calculateDue(rinvDate, filteredData[0].paymentTerms);


    //        var grid = $("#DetailContainer").dxDataGrid("instance");
    //        var defaultRow = {
    //            __KEY__: empr_PurchaseBill.GenerateKey(36),
    //            chK1: false,
    //            chk: "0",
    //            dT_CODE: 0,
    //            warehouse: 2,
    //            //deL_DATE: todayDate,
    //            //duE_DATE: result.dueDate,
    //            //duE_DAYS: result.dueDays
    //        };
    //        grid.option("dataSource", [defaultRow]);

    //        //$('#TERMS').val(filteredData[0].paymentTerms);
    //        $('#TERMS').val(filteredData[0].paymentTerms).trigger('input'); // ya trigger('change')
    //        //$('#DISC').val(filteredData[0].disc);
    //        //if (filteredData[0].partyCode != "" && filteredData[0].partyCode != 0) {
    //        //    empr_PurchaseBill.InitSalesman(filteredData[0].partyCode, parseInt(filteredData[0].scode));
    //        //}
    //    }
    //},

    OnPartyChange: function (d) {
        // 1. Agar hum khud value revert kar rahe hain, toh event ko yahin rok dein
        if (empr_PurchaseBill.isReverting) {
            return;
        }

        if (d.value == null || d.value == '') {
            $('#partyhidden').val('');
            $('#acthidden').val('');
            $('#TERMS').val('');
            $('#PARTY_BALANCE').val('');
        }
        else {
            var filteredData = $.grep(PartyType, function (item) {
                return item.key === d.value;
            });

            var grid = $("#DetailContainer").dxDataGrid("instance");
            var currentData = grid.option("dataSource") || [];

            var hasSavedRecords = currentData.some(function (row) {
                return row.dT_CODE && row.dT_CODE > 0;
            });

            if (hasSavedRecords) {
                empr_helper.notify("Records already in DB! You cannot change the party. Please delete the detail records first to change the party.", 2);

                // 2. Revert karne se pehle flag ko true karein taaki loop na bane
                empr_PurchaseBill.isReverting = true;
                d.component.option("value", d.previousValue);
                empr_PurchaseBill.isReverting = false; // 3. Revert hone ke baad flag wapas false kar dein

                return;
            }

            // --- Aapka baki ka saara normal code niche waise hi rahega ---
            var defaultRow = {
                __KEY__: empr_PurchaseBill.GenerateKey(36),
                chK1: false,
                chk: "0",
                dT_CODE: 0,
                warehouse: 2,
            };
            grid.option("dataSource", [defaultRow]);

            $('#partyhidden').val(filteredData[0].partyCode);
            $('#acthidden').val(filteredData[0].accountCode);
            debugger;
            var code = $('#Code').val();

            if (code) {

                

                //ajaxHelper.ajaxGetJson('/PurchaseBill/GetPartyCurrentBalance?vDate=' + empr_PurchaseBill.vDate, function (data) 
                ajaxHelper.ajaxGetJson('/PurchaseBill/GetPartyCurrentBalance?vDate=' + empr_PurchaseBill.vDate + '&partyCode=' + filteredData[0].partyCode + '&accountCode=' + filteredData[0].accountCode, function (data) {
                    debugger;
                    if (data.length > 0) {
                        var newPartyData = $.grep(data, function (item) {
                            return item.key === d.value;
                        });

                        let balance = parseFloat(newPartyData[0].balance || 0);

                        if (balance < 0) {
                            $('#PARTY_BALANCE').val(`(${Math.abs(balance)})`).css('color', 'red');
                        } else {
                            $('#PARTY_BALANCE').val(balance).css('color', 'black');
                        }
                    }
                }, false, true);
            }
            else {
                let balance = parseFloat(filteredData[0].balance || 0);

                if (balance < 0) {
                    $('#PARTY_BALANCE').val(`(${Math.abs(balance)})`).css('color', 'red');
                } else {
                    $('#PARTY_BALANCE').val(balance).css('color', 'black');
                }
            }



            empr_PurchaseBill.ItemsOnParty = [];
            empr_PurchaseBill.ItemsBehalfOnParty(filteredData[0].partyCode, filteredData[0].accountCode);
            var rinvDate = $('#RINV_DATE').val();
            let result = empr_PurchaseBill.calculateDue(rinvDate, filteredData[0].paymentTerms);

            $('#TERMS').val(filteredData[0].paymentTerms).trigger('input');
        }
    },

    InitItemIds: function () {
        empr_PurchaseBill.bindDxDdl("PICK_ITEM", ItemIds, null, "key", "itemId", "Select", function (d) {
            if (d.value == null || d.value == '') {
                $('#itemIdHidden').val('');
            }
            else {
                $('#itemIdHidden').val(d.value)
            }
        });

    },

    InitSalesman: function (Id, selectedValue = null) {
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetSalesmanByParty?id=' + Id, function (data) {
            if (data.data.table.length > 0) {
                empr_PurchaseBill.bindDxDdl("SCODE", data.data.table, selectedValue, "partY_CODE", "partY_NAME", "Select", function (selected) {

                    if (selected && selected.selectedRowsData && selected.selectedRowsData.length > 0) {
                        var value = selected.selectedRowsData[0]['partY_CODE'];
                        var displayName = selected.selectedRowsData[0]['partY_NAME'];

                        $('#salesmanhidden').val(value);
                        $('#salesmanNameHidden').val(displayName);
                    } else {
                        $('#salesmanhidden').val('');
                        $('#salesmanNameHidden').val('');
                    }
                });
                if (selectedValue != 0 && selectedValue != null) {
                    $('#SCODE').dxSelectBox('instance').option('value', selectedValue);
                }
            }
            else {
                ajaxHelper.ajaxGetJson('/PurchaseBill/GetSalesmanByParty?id=' + 0, function (data) {
                    empr_PurchaseBill.bindDxDdl("SCODE", data.data.table, selectedValue, "partY_CODE", "partY_NAME", "Select", function (selected) {

                        if (selected && selected.selectedRowsData && selected.selectedRowsData.length > 0) {
                            var value = selected.selectedRowsData[0]['partY_CODE'];
                            var displayName = selected.selectedRowsData[0]['partY_NAME'];

                            $('#salesmanhidden').val(value);
                            $('#salesmanNameHidden').val(displayName);
                        } else {
                            $('#salesmanhidden').val('');
                            $('#salesmanNameHidden').val('');
                        }
                    });
                });
            }
        });
    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/PurchaseBill/GetReportTypes", function (data) {
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
                    placeholder: 'Select',
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
        empr_PurchaseBill.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = 0;
        let reportName = "";
        //let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        let selectedItem = $('#ReportType').dxSelectBox('option', 'selectedItem');
        let balance = $('#PARTY_BALANCE').val()
        debugger;
        if (selectedItem) {
            MD_ID = selectedItem.mD_ID;
            reportName = selectedItem.reporT_NAME;
        }
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
            REPORT_NAME: reportName,
            BALANCE: balance,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseBill/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ReportType').dxSelectBox('instance').option('value', MD_ID);
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
        debugger;
        if ($('#SodaPickDetailGridContainer').dxDataGrid('instance').hasEditData()) {
            debugger;
            $('#SodaPickDetailGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                debugger;
                var data = empr_PurchaseBill.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });
                if (IsDataAvailableInGrid || empr_PurchaseBill.firstClick == 1) {
                    debugger;
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    var finalData = existingData.concat(selectedSodas);
                    empr_PurchaseBill.pickIds = finalData.map(x => x.picK_ID);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    empr_PurchaseBill.pickIds = selectedSodas.map(x => x.picK_ID);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                    empr_PurchaseBill.firstClick = 1;
                }
                if (empr_PurchaseBill.pickIds.length > 0) {
                    $('#pickItems').hide();
                }
                $('.modal').hide();
            });
        }
        else {
            debugger;
            var grid = $('#DetailContainer').dxDataGrid('instance');
            grid.saveEditData();
            var data = grid.option('dataSource');

            var IsDataAvailableInGrid = false;
            $.each(data, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid || empr_PurchaseBill.firstClick == 1) {
                debugger;
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource') || [];
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();

                selectedSodas = selectedSodas.map(x => {
                    if (x.qty < 0) x.qty = 0;
                    return x;
                });

                selectedSodas.forEach(function (pickedItem) {
                    var existingItemIndex = existingData.findIndex(e => e.picK_ID_D === pickedItem.picK_ID_D);
                    if (existingItemIndex !== -1) {
                        existingData[existingItemIndex].qty = (parseFloat(existingData[existingItemIndex].qty) || 0) + (parseFloat(pickedItem.qty) || 0);
                        existingData[existingItemIndex].totaL_PACK = (parseFloat(existingData[existingItemIndex].totaL_PACK) || 0) + (parseFloat(pickedItem.totaL_PACK) || 0);

                        selectedSodas = selectedSodas.filter(s => s.picK_ID_D !== pickedItem.picK_ID_D);
                    }
                });

                var finalData = existingData.concat(selectedSodas);
                empr_PurchaseBill.pickIds = finalData.map(x => x.picK_ID_D);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);

            }
            else {
                debugger;
                var selectedSodas = $('#SodaPickDetailGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                console.log('selectedSodas', selectedSodas);
                var filteredData = $.grep(PartyType, function (item) {
                    return item.partyCode === selectedSodas[0].spartY_CODE && item.accountCode === selectedSodas[0].sacT_CODE.toString();
                });
                if (filteredData.length > 0)
                    $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);
                empr_PurchaseBill.InitSalesman(0, parseInt(selectedSodas[0].spartY_CODE));
                //empr_PurchaseBill.InitSalesman(0, parseInt(selectedSodas[0].spartY_CODE));
                //$('#SCODE').dxSelectBox('instance').option('value', parseInt(selectedSodas[0].sacT_CODE));

                //empr_PurchaseBill.InitCurrencyDDL(selectedSodas[0].curR_CODE);
                //empr_PurchaseBill.InitRegionDDL(selectedSodas[0].region);
                //$('#COMM_AMT').dxSelectBox('instance').option('value', selectedSodas[0].comM_AMT);
                //$("#COMM_VAL").val(selectedSodas[0].crate);
                $("#TERMS").val(selectedSodas[0].terms);
                //$("#COMM").val(selectedSodas[0].comm);
                $("#REF").val(selectedSodas[0].ref);
                //$("#COMM_VAL").val(selectedSodas[0].comM_VAL);
                //$('#Currency').dxSelectBox('instance').option('value', selectedSodas[0].crR_CODE);
                $("#Rate").val(selectedSodas[0].crate);
                $("#DISC").val(selectedSodas[0].disC_M);
                $("#RINV_NO").val(selectedSodas[0].rinV_NO);
                $("#RINV_DATE").val(selectedSodas[0].rinV_DATE); //yaha
                debugger;
                if (selectedSodas[0].doc != undefined) {
                    $('#hdnDOC').val(selectedSodas[0].doc);
                    var DocPath = selectedSodas[0].doc;
                    var DocName = DocPath.split('/').pop();
                    $("#DOCName").val(DocName);
                }

                empr_PurchaseBill.pickIds = selectedSodas.map(x => x.picK_ID_D);

                var updatedData = selectedSodas.map(item => {
                    var stockRecord = empr_PurchaseBill.CurrentStock.find(s => s.itemId == item.iteM_CODE);
                    return {
                        ...item,
                        currentStock: stockRecord ? stockRecord.balance : 0
                    };
                });

                $('#DetailContainer').dxDataGrid('instance').option('dataSource', updatedData);

                empr_PurchaseBill.firstClick = 1;
            }
            if (empr_PurchaseBill.pickIds.length > 0) {
                $('#pickItems').hide();
            }
            $('.modal').hide();
        }
    },

    InitCurrencyDDL: function (_selectedValue) {
        $.ajax({
            url: 'CashReceiptVoucher/GetCurrencies',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    if (_selectedValue == undefined || _selectedValue == null) {
                        _selectedValue = data.data[0].key;
                        $('#Rate').val(data.data[0].rate);
                    }
                    $('#Currency').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
                        searchEnabled: true,
                        width: '100%',
                        placeholder: 'Select',
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
                                if (item.length > 0) {
                                    $('#Rate').val(item[0].rate);
                                }
                            }
                            else {
                                $('#Rate').val('');
                            }
                        },
                    });
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
            }
        });
    },

    InitCommissionAmtDDL: function (selectedValue) {

        var dataSource = empr_helper.commType;

        $('#COMM_AMT').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue,
            searchEnabled: true,
            width: '100%',
            placeholder: 'Select',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onValueChanged: function (e) {
                if (e.value === 'RS') {
                    $('#COMM').prop('disabled', true);
                    $('#COMM').val('');
                    $('#COMM_VAL').prop('disabled', false).removeAttr('readonly');
                } else {
                    $('#COMM').prop('disabled', false);
                    $('#COMM_VAL').prop('disabled', true).attr('readonly', true);
                    $('#COMM_VAL').val('');
                }
            },
        });
    },

    CalculateCommition: function () {
        var grid = $("#DetailContainer").dxDataGrid("instance");
        var visibleRows = grid.getVisibleRows(); // Get latest UI data

        var amt = visibleRows.reduce(function (sum, row) {
            return sum + (parseFloat(row.data.neT_AMT) || 0);
        }, 0);


        var commissionType = $("#COMM_AMT").dxSelectBox("option", "value");

        let comm = parseFloat($('#COMM').val());
        let commVal = parseFloat($('#COMM_VAL').val());

        if (isNaN(amt) || amt <= 0) return;

        if (commissionType == 'PR') {
            let calcCommVal = (amt * comm) / 100;
            $('#COMM_VAL').val(calcCommVal.toFixed(2));
        }
        else if (commissionType == 'RS') {
            let calcComm = (commVal * 100) / amt;
            $('#COMM').val(calcComm.toFixed(2));
        }

        //if (!isNaN(comm)) {
        //    let calcCommVal = (amt * comm) / 100;
        //    $('#COMM_VAL').val(calcCommVal.toFixed(2));
        //}

        //if (!isNaN(commVal)) {
        //    let calcComm = (commVal * 100) / amt;
        //    $('#COMM').val(calcComm.toFixed(2));
        //}

    },
    PrepareDataForValidation: function () {

        if ($('#ItemPrintContainer').dxDataGrid('instance').hasEditData()) {
            $('#ItemPrintContainer').dxDataGrid('instance').saveEditData().done(function () {
                //var response = empr_BarcodePrint.GetGridData();
                //response.then((data) => {
                //    debugger;
                var data = $('#ItemPrintContainer').dxDataGrid('instance').getSelectedRowKeys();
                var result = empr_PurchaseBill.ValidateMainInfo(data);
                //return result;
                if (result) {
                    empr_PurchaseBill.GetBarcodePrint(data);
                }
                //});
            });
        }
        else {
            var data = $('#ItemPrintContainer').dxDataGrid('instance').getSelectedRowKeys();
            //var result = empr_StockTransfer.ValidateMainInfo(data);
            console.log(result);
            //return result;
            //if (result) {
                empr_PurchaseBill.GetBarcodePrint(data);
            //}
            //return empr_BarcodePrint.ValidateMainInfo($('#gridContainer').dxDataGrid('instance').getSelectedRowKeys());
        }
    },
    GetBarcodePrint: function (dataModel) {

        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        console.log(MD_ID);
        debugger;
        //let REPORT_NAME = empr_StockTransfer.reportTypes.filter(x => x.mD_ID == MD_ID)[0];
        let REPORT_NAME = "BarcodeSticker";
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
        //$("#Loader").show().css('display', 'flex');
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, "/StockTransfer/GetBarcodeReport", function (data) {
            if (data.msgType == 1) {
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody1').empty();
                //setTimeout(function () {
                //    $("#Loader").hide();
                //}, 100);
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
    //calculateNetAmtTotal: function () {
    //    var grid = $("#DetailContainer").dxDataGrid("instance");
    //    var visibleRows = grid.getVisibleRows(); // Get latest UI data

    //    var totalNetAmt = visibleRows.reduce(function (sum, row) {
    //        return sum + (parseFloat(row.data.neT_AMT) || 0);
    //    }, 0);

    //    // Store in global variable if needed
    //    //window.totalNetAmt = totalNetAmt;

    //    //// Optional: Show in DOM
    //    //$("#netAmtTotal").text(totalNetAmt.toFixed(2));
    //},

    calculateDue: function (deliveryDate, payTerm) {
        if (!deliveryDate) return null;

        let today = new Date();
        today.setHours(0, 0, 0, 0);

        let delDate = new Date(deliveryDate);
        delDate.setHours(0, 0, 0, 0);

        if (delDate < today) {
            return {
                isValid: false
            };
        }

        let dayDiff = Math.ceil(
            (delDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)
        );

        let totalDays = dayDiff + payTerm;

        let dueDate = new Date(delDate);
        dueDate.setDate(dueDate.getDate() + totalDays);

        return {
            isValid: true,
            dueDate: dueDate,
            dueDays: totalDays
        };
    },

    //SetSaleTax: function (sTax, disc) {
    //    var saleTax = parseFloat(sTax) || 0;
    //    var discount = parseFloat(disc) || 0;
    //    let gridInstance = $("#DetailContainer").dxDataGrid("instance");
    //    let rowCount = gridInstance.getDataSource().items().length;

    //    for (let i = 0; i < rowCount; i++) {

    //        if (saleTax < 0) {
    //            gridInstance.cellValue(i, "adv", null);
    //            gridInstance.cellValue(i, "disc", null);
    //        } else {
    //            gridInstance.cellValue(i, "adv", saleTax);
    //            gridInstance.cellValue(i, "disc", discount);
    //        }
    //    }
    //},


    UploadDoc: function () {
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        if (files.length > 0) {
            $('#DOCName').val(files[0].name);
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
                    //empr_helper.notify("File uploaded successfully.", 1);
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

    openVoucherPage(link, tran_Id) {
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
        });
    },

    setPurchaseBillItemData: function (newData, itemCode, qty) {
        debugger;

        if (!itemCode) return;

        //var source = (dcType == 'SB' || dcType == 'SO') ? empr_PurchaseBill.ItemsOnParty : Items;

        var source = empr_PurchaseBill.ItemsOnParty;

        console.log("items data", source);

        if (!source || source.length === 0) return;

        var selectedItem = source.filter(
            u => String(u.key) === String(itemCode)
        );

        if (selectedItem.length === 0) return;

        var item = selectedItem[0];


        var weight = parseFloat(item.weight || 0);
        var pack = parseFloat(item.pack || 0);
        var quantity = parseFloat(qty) || 0;

        newData.rate = item.rate;
        //newData.pack = item.pack;
        newData.totaL_PACK = pack;
        if (!item.pack) {
            newData.totaL_PACK = item.pack;
        }
        newData.disc = item.disc;
        //newData.adv = item.adv;
        if (!item.disc) {
            newData.disC_AMT = item.disc;
        }
        newData.hS_CODE = item.hscode;
        newData.unit = item.unit;
        newData.tax = item.saleTax;
        //var wei = weight * pack * quantity;
        //newData.weight = wei;

        newData.qty = null;
        //newData.totaL_PACK = item.pacK;
        newData.amt = null;
        newData.disC_AMT = null;
        newData.taX_AMT = null;
        newData.adV_AMT = null;
        newData.neT_AMT = null;
    },

    InitRegionDDL: function (selectedValue) {

        $('#REGION').dxSelectBox({
            dataSource: Regions,
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

}