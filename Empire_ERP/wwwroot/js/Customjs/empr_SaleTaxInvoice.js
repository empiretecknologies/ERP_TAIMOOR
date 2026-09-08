$(document).keydown(function (e) {
    //Check Ctrl key is pressed and S key is pressed Saveempr_SaleTaxInvoice
    if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
        if (empr_SaleTaxInvoice.ValidateMainInfo()) {
            empr_SaleTaxInvoice.Save();
        }
        return false;
    }
    //Check Ctrl key is pressed and D key is pressed Delete
    if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
        e.preventDefault();
        if ($("#Code").val() != '') {
            empr_SaleTaxInvoice.Delete();
        } else {
            empr_helper.notify("Please select any record for delete..", 2);
        }
        return false;
    }
    //Check Alt key is pressed and R key is pressed Refresh
    if ((e.altKey || e.metaKey) && e.key === 'a') {
        e.preventDefault();
        //$("#SCODE").dxSelectBox("instance")?.option("value", "");
        empr_SaleTaxInvoice.ResetForm();
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
var empr_SaleTaxInvoice = {
    //formName: typeForm,
    rowsCount: 0,
    pickIds: [],
    originalValues: {},
    firstClick: 0,

    InitEvents: function () {
        $(document).ready(function () {
            console.log('Items', Items);
            console.log('fbrType', fbrType);
            empr_SaleTaxInvoice.InitQuickSearchGrid();
            empr_SaleTaxInvoice.ResetForm();
            empr_SaleTaxInvoice.InitPartyType();
            empr_SaleTaxInvoice.InitReportTypeDDL();


            var Id = 0;
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    empr_helper.selectedBill = data.traN_ID;
                    $('#Code').val(data.traN_ID);
                    empr_SaleTaxInvoice.GetSaleTaxInvoiceByCode(data.traN_ID);
                }
            });

            $('body').on('click', '#docBrowseBtn', function () {
                $('#DOC').val('');
                $('#hdnDOC').val('');
                $('#DOCName').val('');
                $('#DOC').click();
            });

            $('body').on('click', '#BtnfBRpOST', function () {
                var code = $('#Code').val();
                if (code == null || code == 0) {
                    empr_helper.notify('Create Entry first', 2);
                } else {
                    empr_SaleTaxInvoice.GetAndPostDataToApi(code);
                }
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_SaleTaxInvoice.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_SaleTaxInvoice.GetSaleTaxInvoiceByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/SaleTaxInvoice/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_SaleTaxInvoice.GetSaleTaxInvoiceByCode(data.data.code);
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
                            if (empr_SaleTaxInvoice.ValidateMainInfo()) {
                                empr_SaleTaxInvoice.Save();
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
                        if (empr_SaleTaxInvoice.ValidateMainInfo()) {
                            empr_SaleTaxInvoice.Save();
                        }
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);
                    }, 200);
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_SaleTaxInvoice.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_SaleTaxInvoice.ResetForm();
                $('#REF').focus();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_SaleTaxInvoice.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_SaleTaxInvoice.GeneratePrintReport();
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
    SetData: function (dataSource) {
        $.each(dataSource, function (index, item) {
            //if (Type != "I") {
            //    item.iteM_CODE = parseInt(item.barcodE_CODE);
            //}
            item.dT_CODE = 0;
            item.__KEY__ = empr_SaleTaxInvoice.GenerateKey(36);
        });

        return dataSource;
    },
    CreateGrid: function (dataSrc) {
        
        dataSrc.forEach(item => {
        });
        if (dataSrc.length > 0) {
            empr_SaleTaxInvoice.rowsCount = dataSrc.length - 1;
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
                        const copyAction = !Permissions.r_COPY ? '' : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_SaleTaxInvoice.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT) ? '' : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_SaleTaxInvoice.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT ? '' : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_SaleTaxInvoice.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_SaleTaxInvoice.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_SaleTaxInvoice.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_SaleTaxInvoice.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                caption: "Item",
                allowSorting: false,
                width: 200,
                fixed: true,
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
                    allowClearing: true,
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    }
                },

                setCellValue: function (rowData, newValue) {

                    this.defaultSetCellValue(rowData, newValue);

                    const selected = Items.find(x => x.key === newValue);

                    if (selected) {
                        rowData.hS_CODE = selected.hscode;
                        rowData.unit = selected.unit;
                    } else {
                        rowData.hS_CODE = "";
                        rowData.unit = 0;
                    }
                }
            },
            {
                dataField: 'hS_CODE',
                caption: 'HS Code',
                allowEditing:false
            },
            {
                dataField: 'qty',
                caption: 'Qty',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;

                    var rate = parseFloat(currentRowData.rate) || 0;
                    var qty = parseFloat(newData.qty) || 0;

                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                        return;
                    }

                    if (!isNaN(rate)) {

                        // Amount sirf qty * rate se
                        newData.amt = (qty * rate).toFixed(2);

                        var Amount = parseFloat(newData.amt) || 0;
                        var Dis = parseFloat(currentRowData.disc) || 0;
                        var Adv = parseFloat(currentRowData.adv) || 0;
                        var Tax = parseFloat(currentRowData.tax) || 0;

                        var disSum = (Amount * Dis / 100) || 0;
                        var TaxAmount = Amount - disSum || 0;

                        var TaxSum = (TaxAmount * Tax / 100) || 0;
                        var AdvSum = ((TaxAmount + TaxSum) * Adv / 100) || 0;

                        newData.disC_AMT = disSum.toFixed(2);
                        newData.taX_AMT = TaxSum.toFixed(2);
                        newData.adV_AMT = AdvSum.toFixed(2);

                        var NetAmount = TaxAmount + TaxSum + AdvSum;

                        newData.neT_AMT = (!isNaN(NetAmount)) ? NetAmount.toFixed(2) : 0;
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
                dataField: 'rate',
                caption: 'Rate',
                setCellValue: function (newData, value, currentRowData) {

                    newData.rate = value;

                    var rate = parseFloat(value) || 0;
                    var qty = parseFloat(currentRowData.qty) || 0; // ✅ qty use karo

                    // Amount calculation
                    var Amount = (!isNaN(qty) && !isNaN(rate)) ? (qty * rate) : 0;
                    newData.amt = Amount.toFixed(2);

                    // Discount & Tax
                    var Discount = parseFloat(currentRowData.disc) || 0;
                    var Tax = parseFloat(currentRowData.tax) || 0;

                    var DiscountAmount = (Amount * Discount / 100) || 0;
                    var TaxableAmount = Amount - DiscountAmount;
                    var TaxSum = (TaxableAmount * Tax / 100) || 0;

                    newData.disC_AMT = DiscountAmount.toFixed(2);
                    newData.taX_AMT = TaxSum.toFixed(2);

                    // ✅ Net Amount (ADV removed)
                    var NetAmount = TaxableAmount + TaxSum;
                    newData.neT_AMT = (!isNaN(NetAmount)) ? NetAmount.toFixed(2) : "0.00";

                    // ✅ ADV completely removed
                    newData.adV_AMT = "0.00";
                }
            },

            {
                dataField: 'amt',
                caption: 'Amount',
                allowEditing: false,
            },
            {
                dataField: 'disc',
                caption: 'Disc %',
                setCellValue: function (newData, value, currentRowData) {
                    newData.disc = value;
                    var Amount = parseFloat(currentRowData.amt) || 0;
                    var Discount = parseFloat(newData.disc) || 0;

                    if (!isNaN(Amount) && !isNaN(Discount)) {
                        newData.disC_AMT = (Amount * Discount / 100).toFixed(2);
                    } else {
                        newData.disC_AMT = 0;
                    }

                    var Tax = parseFloat(currentRowData.tax) || 0;
                    var Adv = parseFloat(currentRowData.adv) || 0;
                    var DiscountAmount = parseFloat(newData.disC_AMT) || 0;
                    var TaxAmount = Amount - DiscountAmount;
                    var TaxSum = TaxAmount * Tax / 100 || 0;
                    var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;
                    newData.taX_AMT = TaxSum.toFixed(2);
                    newData.adV_AMT = AdvSum.toFixed(2);
                    var NetAmount = Amount - DiscountAmount || 0;
                    if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                        newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                    } else {
                        newData.neT_AMT = 0;
                    }
                }
            },
            {
                dataField: 'disC_AMT',
                caption: 'Disc Amt',
                allowEditing: false,
            },
            {
                dataField: 'tax',
                caption: 'Tax',
                setCellValue: function (newData, value, currentRowData) {
                    newData.tax = value;
                    var Amount = parseFloat(currentRowData.amt) || 0;
                    var DiscountAmount = parseFloat(currentRowData.disC_AMT) || 0;
                    var Tax = parseFloat(newData.tax) || 0;
                    var Adv = parseFloat(currentRowData.adv) || 0;
                    var TaxAmount = Amount - DiscountAmount || 0;
                    var TaxSum = TaxAmount * Tax / 100 || 0;
                    var AdvSum = (TaxAmount + TaxSum) * Adv / 100 || 0;

                    if (!isNaN(TaxAmount) && !isNaN(Tax)) {
                        newData.taX_AMT = TaxSum.toFixed(2);
                        newData.adV_AMT = AdvSum.toFixed(2);
                    } else {
                        newData.taX_AMT = 0;
                    }

                    var NetAmount = Amount - DiscountAmount || 0;
                    if (!isNaN(NetAmount) && !isNaN(TaxSum) && !isNaN(AdvSum)) {
                        newData.neT_AMT = (NetAmount + TaxSum + AdvSum).toFixed(2);
                    } else {
                        newData.neT_AMT = 0;
                    }
                }
            },
            {
                dataField: 'taX_AMT',
                caption: 'Tax Amount',
                allowEditing: false,
            },
            {
                dataField: 'neT_AMT',
                caption: 'Net Amount',
                allowEditing: false,
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                wordWrapEnabled: true,
            },
            {
                dataField: 'fbR_TYPE',
                caption: "FBR Type",
                allowSorting: false,
                width: 200,
                lookup: {
                    dataSource: {
                        store: fbrType,
                        paginate: true,
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    allowClearing: true
                },

                setCellValue: function (rowData, newValue, currentRowData) {
                    rowData.fbR_TYPE = newValue;

                    const selected = fbrType.find(x => x.key === newValue);

                    if (selected) {
                        rowData.iteM_SNO = selected.item_Sno;
                        rowData.schedulE_NO = selected.sche_No;
                        rowData.seriaL_NO = selected.seri_No;
                    } else {
                        rowData.iteM_SNO = "";
                        rowData.schedulE_NO = "";
                        rowData.seriaL_NO = "";
                    }
                }
            },
            {
                dataField: 'iteM_SNO',
                caption: 'Item SNO',
                allowEditing: false
            },
            {
                dataField: 'schedulE_NO',
                caption: 'Schedule No',
                allowEditing: false
            },
            {
                dataField: 'seriaL_NO',
                caption: 'Serial No',
                allowEditing: false
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "PurchaseBill", "iteM_CODE");
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
        if (empr_SaleTaxInvoice.pickIds.length > 0) {
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

                    empr_SaleTaxInvoice.rowsCount += 1;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    var dataSource = gridInstance.option("dataSource");
                    if (dataSource.length > 0) {
                        let clonedRowData = $.extend(true, {}, dataSource[index]);
                        if (clonedRowData.hasOwnProperty('dT_CODE')) {
                            delete clonedRowData.dT_CODE;
                        }
                        clonedRowData.__KEY__ = empr_SaleTaxInvoice.GenerateKey(36);
                        clonedRowData.dT_CODE = 0;
                        let newDataSource = [clonedRowData].concat(dataSource);
                        gridInstance.option("dataSource", newDataSource);
                        gridInstance.refresh();
                    }
                });
            }
            else {
                empr_SaleTaxInvoice.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_SaleTaxInvoice.GenerateKey(36);
                    clonedRowData.dT_CODE = 0;
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            }
        }
    },
    AddRow: function () {
        if (empr_SaleTaxInvoice.pickIds.length > 0) {
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
                    empr_SaleTaxInvoice.rowsCount += 1;
                    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                    const dataSource = gridInstance.option("dataSource");

                    dataSource.unshift({ __KEY__: empr_SaleTaxInvoice.GenerateKey(36), dT_CODE: 0 });
                    gridInstance.option("dataSource", dataSource);
                    gridInstance.refresh();
                    empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
                });
            }
            else {
                empr_SaleTaxInvoice.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_SaleTaxInvoice.GenerateKey(36), dT_CODE: 0 });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                empr_helper.MoveFocusToGridWithouTab('#DetailContainer', 0, 'iteM_CODE')
            }
        }
    },
    DeleteRow: function (index, dtCode) {
        debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_SaleTaxInvoice.rowsCount -= 1;
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
                            debugger
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/SaleTaxInvoice/DeleteSaleTaxInvoiceDetailByCode", function (data) {
                                debugger;
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_SaleTaxInvoice.rowsCount -= 1;
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
    InitQuickSearchGrid: function () {
        empr_SaleTaxInvoice.GetQuickSearchData();
    },
    GetQuickSearchData: function () {
        ajaxHelper.ajaxGetJson('/SaleTaxInvoice/GetQuickSearchData', function (data) {
            if (data.msgType == 1) {
                empr_SaleTaxInvoice.CreateQuickSearchGrid(data.data);
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
        { dataField: 'id', caption: 'Code', alignment: "center" },
        { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'astatus', caption: 'Status', },
        { dataField: 'voucheR_NO', caption: 'Transaction #', },
        { dataField: 'partY_NAME', caption: 'Party Name', },
        { dataField: 'btype', caption: 'Type', },
        { dataField: 'ref', caption: 'Reference No', },
        { dataField: 'remarks', caption: 'Description', },
        ];

        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "PurchaseBillQS", "single");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    GetSaleTaxInvoiceByCode: function (code) {
        empr_SaleTaxInvoice.ResetForm();
        ajaxHelper.ajaxGetJson('/SaleTaxInvoice/GetSaleTaxInvoiceByCode?code=' + code, function (data) {
            console.log('editData', data);
            if (data.master.msgType == 1 && data.detail.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    var filteredData = $.grep(PartyType, function (item) {
                        return item.partyCode === response.partY_CODE && item.accountCode === response.acT_CODE.toString();
                    });
                    if (data.qrcode != "") {
                        $("#qrImg").attr("src", "data:image/png;base64," + data.qrcode);
                    }

                    $('#Code').val(response.traN_ID);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#REF').val(response.ref);
                    $('#FBR_RES').val(response.fbR_NO);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE_STRING);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#hdnDOC').val(response.doc);
                    var DocPath = response.doc;
                    var DocName = DocPath.split('/').pop();
                    $("#DOCName").val(DocName);

                    $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);
                    $('#partyhidden').val(filteredData[0].partyCode)

                    //if ($('#FBR_RES').val().trim() === "") {
                    //    $('#BtnfBRpOST').show();
                    //} else {
                    //    $('#BtnfBRpOST').hide();
                    //    $('#BtnDelete').hide();
                    //    $('#BtnSave').hide();
                    //}

                    if (Permissions != "Admin") {
                        if ($('#FBR_RES').val().trim() === "") {
                            if (Permissions.r_DLT) {
                                $('#BtnDelete').show();
                            }
                            if (Permissions.r_EDIT) {
                                $('#BtnSave').show();
                                $('#BtnfBRpOST').show();
                            }
                            else {
                                $('#BtnSave').hide();
                            }
                        } else {
                            $('#BtnSave').hide();
                            $('#BtnDelete').hide();
                        }
                    }
                    else {
                        //debugger;
                        if ($('#FBR_RES').val().trim() === "") {
                            $('#BtnfBRpOST').show();
                            $('#BtnSave').show();
                            $('#BtnDelete').show();
                        } else {
                            $('#BtnfBRpOST').hide();
                            $("#verifiedText").show();
                            $('#BtnSave').hide();
                            $('#BtnDelete').hide();
                            $('#BtnPrint').css('margin-right', '5%');
                        }
                    }
                }

                if (data.detail.msgType == 1) {
                    debugger;
                    empr_SaleTaxInvoice.CreateGrid(data.detail.data);
                }
            }
            else {
                if (data.master.msgType != 1) {
                    empr_helper.notify(data.master.msg, data.master.msgType);
                }
                else {
                    empr_helper.notify(data.detail.msg, data.detail.msgType);
                }
            }
        }, false, true);
    },
    GetDataToSave: function () {
        var ID = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var REF = $("#REF").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var BTYPE = $("#BTYPE").dxSelectBox('option', 'value');
        var DOC = $("#hdnDOC").val();
        var REMARKS = $("#REMARKS").val();

        var masterRecord = {
            TRAN_ID: ID,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            REF: REF,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            BTYPE: BTYPE,
            DOC: DOC,
            REMARKS: REMARKS
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

        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords,
        };
        return modelRecord;

        //if (empr_SaleTaxInvoice.rowsCount == detailRecords.length) {

        //}
        //else {
        //    var modelRecord = {
        //        Master: masterRecord,
        //        Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource"),
        //    };
        //    return modelRecord;
        //}
    },
    //ValidateMainInfo: function () {

    //    var valid = true;
    //    var data = empr_SaleTaxInvoice.GetDataToSave();
    //    console.log('ValidateMainInfo', data);
    //    if (data.Master.PARTY_CODE == '') {
    //        empr_helper.notify("Please select Party Type.", 2);
    //        valid = false;
    //        //return valid;
    //    }

    //    if (data.Detail.length > 0) {
    //        $.each(data.Detail, function (index, item) {
    //            console.log('iteM loop', item);
    //            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
    //                empr_helper.notify("Please select Item on Line No " + (index + 1), 2);
    //                valid = false;
    //                return valid;
    //            }
    //            if (item.qty == "" || item.qty == "0" || item.qty == 0 || item.qty == null || item.qty == undefined || !item.qty) {
    //                empr_helper.notify("Please enter Item quantity on Line No " + (index + 1), 2);
    //                valid = false;
    //                return valid;
    //            }
    //            if (item.qty <= 0) {
    //                empr_helper.notify("Please enter correct item quantity on Line No " + (index + 1), 2);
    //                valid = false;
    //                return valid;
    //            }
    //            if (item.fbR_TYPE == 0 || item.fbR_TYPE == null || item.fbR_TYPE == undefined) {
    //                empr_helper.notify("Please Select FBR Type on Line No " + (index + 1), 2);
    //                valid = false;
    //                return valid;
    //            }

    //        });
    //    }
    //    else
    //    {
    //        empr_helper.notify("You cannot save record without any detail", 2);
    //        valid = false;
    //    }



    //    if (data.Detail.length > Limit && Limit != 0) {
    //        empr_helper.notify("You can only add  " + Limit + " records.", 2);
    //        valid = false;
    //        return valid;
    //    }

    //    return valid;
    //},

    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_SaleTaxInvoice.GetDataToSave();

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
                empr_helper.notify("Please select Item on Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity on Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantityon Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            if (item.fbR_TYPE == 0 || item.fbR_TYPE == null || item.fbR_TYPE == undefined) {
                empr_helper.notify("Please Select FBR Type on Line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            //if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
            //    empr_helper.notify("Please enter correct item quantity2 at index " + index, 2);
            //    valid = false;
            //    return valid;
            //    console.log("Item at index " + index + " has empty Quantity2.");
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
        var dataModel = empr_SaleTaxInvoice.GetDataToSave();
        console.log('SaveAttempt', dataModel)
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/SaleTaxInvoice/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {

                empr_SaleTaxInvoice.GetSaleTaxInvoiceByCode(id);

            }
        }, false, true);
    },
    ResetForm: function () {



        $('.detailInfo a').tab('show');

        empr_SaleTaxInvoice.CreateGrid([{ __KEY__: empr_SaleTaxInvoice.GenerateKey(36), chK1: false, chk: "0", dT_CODE: 0 }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
        $('#hdnDOC').val('');
        $('#qrImg').attr('src', '/Client/Company/notPostedYet.jpg');
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('#BTYPE').dxSelectBox('instance').option('value', 'CR');

        $('#Code').val();
        empr_SaleTaxInvoice.pickIds = [];
        empr_SaleTaxInvoice.firstClick = 0;
        empr_SaleTaxInvoice.InitPartyType();
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

        $('#BtnfBRpOST').show();
        $("#verifiedText").hide();
        $('#BtnPrint').css('margin-right', '1.8%');

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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/SaleTaxInvoice/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_SaleTaxInvoice.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
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
        empr_SaleTaxInvoice.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
            if (d.value == null || d.value == '') {
                $('#partyhidden').val('');
                $('#acthidden').val('');
            }
            else {
                var filteredData = $.grep(PartyType, function (item) {
                    return item.key === d.value;
                });
                $('#partyhidden').val(filteredData[0].partyCode)
                $('#acthidden').val(filteredData[0].accountCode)
                if (filteredData[0].partyCode != "" && filteredData[0].partyCode != 0) {
                }
            }

        });

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/SaleTaxInvoice/GetReportTypes", function (data) {
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
        empr_SaleTaxInvoice.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = 0;
        let reportName = "";

        let selectedItem = $('#ReportType').dxSelectBox('option', 'selectedItem');
        if (selectedItem) {
            MD_ID = selectedItem.mD_ID;
            reportName = selectedItem.reporT_NAME;
        }

        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Something went wrong..", 2);
            return;
        }

        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
            REPORT_NAME: reportName,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/SaleTaxInvoice/GetPrintReport", function (data) {
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


    GetAndPostDataToApi: function (code) {

        ajaxHelper.ajaxGetJson('/SaleTaxInvoice/GetDataForApi?code=' + code, function (data) {
            debugger;
            console.log("fbr_respone", data);
            //if (data.msgType == 1) {

            //    let d = data.data[0];
            //    console.log('GetDataForApi', d);
            if (data.msgType === 1) {

                console.log('GetDataForApi Response', data.response);

                var fbrData = JSON.parse(data.response);

                var displayText =
                    fbrData.invoiceNumber ||
                    fbrData.validationResponse?.error ||
                    "Unknown response";

                var statusUpdateRes =
                    empr_SaleTaxInvoice.ApiStatus_Update(code, displayText);

                $('#FBR_RES').val(displayText);
                $('#FBR_RES').val(displayText);
             
                if (data.qrCode) {
                    $("#qrImg").attr(
                        "src",
                        "data:image/png;base64," + data.qrCode
                    );
                }

                $('#BtnfBRpOST').hide();
                $("#verifiedText").show();
            }
            else {
                empr_helper.notify("No data found / FBR error" ,2);
            }

            $("#Loader").hide();
        },false, true);

                //var rateValue = `${d.tax}%`;
                //var valueSalesExcludingSTValue = d.valuE_SALES_EXCLUDING;
                ////debugger;
                //if (d.s_NAME == 'Exempt goods') {
                //    console.log('First true');
                //    rateValue = 'Exempt';
                //    valueSalesExcludingSTValue = d.totaL_VALUES;
                //}
                //else if (d.s_NAME == 'Goods at zero-rate') {
                //    console.log('Second true');
                //    rateValue = `${d.tax}%`;
                //    valueSalesExcludingSTValue = d.totaL_VALUES;
                //}
                //else {
                //    console.log('Else true');
                //    rateValue = `${d.tax}%`;
                //}

                ////rateValue = d.s_NAME == 'Exempt goods' ? 'Exempt' : `${d.tax}%`;

                //var FBRModel = {
                //    FBRPostModel: {
                //        invoiceType: "Sale Invoice",
                //        invoiceDate: `${d.invoicE_DATE}`,
                //        sellerNTNCNIC: `${d.selleR_NTN}`,
                //        sellerBusinessName: `${d.selleR_BNAME}`,
                //        sellerProvince: `${d.selleR_PROVINCE}`,
                //        sellerAddress: `${d.selleR_ADDRESS}`,
                //        buyerNTNCNIC: `${d.buyeR_NTN}`,
                //        buyerBusinessName: `${d.buyeR_BNAME}`,
                //        buyerProvince: `${d.buyeR_PROVINCE}`,
                //        buyerAddress: `${d.buyeR_ADDRESS}`,
                //        buyerRegistrationType: `${d.buyeR_REG_TYPE}`,
                //        invoiceRefNo: "",
                //        scenarioId: `${d.scenariO_ID}`,
                //        items: [
                //            {
                //                hsCode: `${d.hS_CODE}`,
                //                productDescription: `${d.iteM_NAME}`,
                //                rate: `${rateValue}`,
                //                uoM: `${d.uom}`,
                //                quantity: d.qty,
                //                totalValues: d.totaL_VALUES,
                //                valueSalesExcludingST: valueSalesExcludingSTValue,
                //                fixedNotifiedValueOrRetailPrice: d.fixedvaluE_RETAILPRICE,
                //                salesTaxApplicable: d.sT_APPLICABLE,
                //                salesTaxWithheldAtSource: 0,
                //                extraTax: 0,
                //                furtherTax: 0,
                //                sroScheduleNo: `${d.srO_SCH_NO}`,
                //                fedPayable: 0,
                //                discount: 0,
                //                saleType: `${d.s_NAME}`,
                //                sroItemSerialNo: `${d.seriaL_NO}`,
                //            }
                //        ],

                    //},
                    //FBRUrlToken: {
                    //    Url: "https://gw.fbr.gov.pk/di_data/v1/di/postinvoicedata_sb",
                    //    Token: "331a9d2a-d1f3-3a39-85b9-3bc3e952b9c9",
                    //}
              

        //        console.log("Request JSON:\n" + JSON.stringify(FBRModel.FBRPostModel, null, 2));

        //        $('#Loader').appendTo('body');
        //        $("#Loader").css({
        //            display: 'flex'
        //        });
        //        var FBRModel = data.data;
        //        $.ajax({
        //            type: "POST",
        //            url: "/SaleTaxInvoice/PostToFBR",
        //            contentType: "application/json",
        //            data: JSON.stringify(FBRModel),
        //            success: function (res) {
        //                console.log('PostToFBRRes',res);
        //                debugger;
        //                var data = JSON.parse(res.response);
        //                var displayText = data.invoiceNumber || data.validationResponse?.error || "Unknown response";
        //                var statusUpdateRes = empr_SaleTaxInvoice.ApiStatus_Update(code, displayText);


        //                $("#Loader").hide();
        //                $('#FBR_RES').val(displayText);
        //                if (res.qrCode) {
        //                    $("#qrImg").attr("src", "data:image/png;base64," + res.qrCode);
        //                }
        //            },
        //        });

        //        $('#BtnfBRpOST').hide();
        //        $("#verifiedText").show();

        //    } else {
        //        empr_helper.notify(data.msg, data.msgType);
        //    }

        //}, false, true);
    },

    ApiStatus_Update(code, apiResponce) {
        ajaxHelper.ajaxGetJson('/SaleTaxInvoice/FBRApi_Status?code=' + code + '&apiResponce=' + apiResponce, function (data) {

        }, false, true);
    },

    //GetQrCode: function(){
    //    ajaxHelper.ajaxGetJson("/SaleTaxInvoice/GenerateQrCode", function (data) {
    //        console.log('GetQrCode',data);
    //    }, false, true);
    //},



}