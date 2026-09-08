var empr_SalesInvoiceReport = {
    IsCustomSelection: false,
    PartyData: [],
    SalesmanData: [],
    ItemGroupData: [],
    BranchData: [],
    ItemData: [],
    ControlData: [],

    InitEvents() {
        empr_SalesInvoiceReport.InitItemMasterDDL();
        empr_SalesInvoiceReport.InitItemGroupDDL();
        empr_SalesInvoiceReport.InitBranchDDL(Branch);

        empr_SalesInvoiceReport.GetReportTypes();
        //empr_SalesInvoiceReport.InitControlDDL();
        //empr_SalesInvoiceReport.InitSControlDDL();
        //empr_SalesInvoiceReport.InitAccountDDL();
        //empr_SalesInvoiceReport.InitSalesmanDDL();
        //empr_SalesInvoiceReport.InitItemIdDDL();
        //empr_SalesInvoiceReport.InitCategoryDDL();
        //empr_SalesInvoiceReport.InitSubCategoryDDL();
        //empr_SalesInvoiceReport.InitBarcodeDDL();
        //empr_SalesInvoiceReport.InitColorDDL();
        //empr_SalesInvoiceReport.InitSizeDDL();
        //empr_SalesInvoiceReport.InitLotDDL();

        $('body').on('click', '#BtnGenerate', function () {
            //debugger;
            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_SalesInvoiceReport.ValidateInfo()) {
                    empr_SalesInvoiceReport.GenerateReport();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
            }, 200);
        });
    },

    //InitControlDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/PartyReports/GetControls", function (data) {
    //        if (data.msgType == 1) {
    //            empr_SalesInvoiceReport.ControlData = data.data;
    //            $('#CONTROL_NAME').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    if (e.value != '' && e.value != null) {
    //                        if (!empr_SalesInvoiceReport.IsCustomSelection) {
    //                            $('#PARTY_NAME').dxSelectBox('instance').option('value', '');
    //                        }
    //                    }
    //                    else {
    //                        $('#PARTY_NAME').dxSelectBox('instance').option('value', '');
    //                    }
    //                },
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitSControlDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetControlsForSalesman", function (data) {
    //        if (data.msgType == 1) {
    //            empr_SalesInvoiceReport.ControlData = data.data;
    //            $('#SCONTROL_NAME').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    if (e.value != '' && e.value != null) {
    //                        if (!empr_SalesInvoiceReport.IsCustomSelection) {
    //                            $('#SALESMAN_NAME').dxSelectBox('instance').option('value', '');
    //                        }
    //                    }
    //                    else {
    //                        $('#SALESMAN_NAME').dxSelectBox('instance').option('value', '');
    //                    }
    //                },
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitAccountDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/PartyReports/GetParties", function (data) {
    //        if (data.msgType == 1) {
    //            empr_SalesInvoiceReport.PartyData = data.data;
    //            $('#PARTY_NAME').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    if (e.value != '' && e.value != null) {
    //                        var items = e.component._dataSource._items;
    //                        var item = items.filter(i => i.key == e.value);
    //                        if (item.length > 0) {
    //                            var controls = empr_SalesInvoiceReport.ControlData;
    //                            var control = controls.filter(i => i.key == item[0].accountCode);
    //                            if (control.length > 0) {
    //                                empr_SalesInvoiceReport.IsCustomSelection = true;
    //                                $('#CONTROL_NAME').dxSelectBox('instance').option('value', control[0].key);
    //                                setTimeout(function () {
    //                                    empr_SalesInvoiceReport.IsCustomSelection = false;
    //                                }, 500);
    //                            } else {
    //                                $('#CONTROL_NAME').dxSelectBox('instance').option('value', null);
    //                            }
    //                        }
    //                    }
    //                },
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitSalesmanDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetSalesman", function (data) {
    //        if (data.msgType == 1) {
    //            empr_SalesInvoiceReport.SalesmanData = data.data;
    //            $('#SALESMAN_NAME').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    if (e.value != '' && e.value != null) {
    //                        var items = e.component._dataSource._items;
    //                        var item = items.filter(i => i.key == e.value);
    //                        if (item.length > 0) {
    //                            var controls = empr_SalesInvoiceReport.ControlData;
    //                            var control = controls.filter(i => i.key == item[0].accountCode);
    //                            if (control.length > 0) {
    //                                empr_SalesInvoiceReport.IsCustomSelection = true;
    //                                $('#SCONTROL_NAME').dxSelectBox('instance').option('value', control[0].key);
    //                                setTimeout(function () {
    //                                    empr_SalesInvoiceReport.IsCustomSelection = false;
    //                                }, 500);
    //                            } else {
    //                                $('#SCONTROL_NAME').dxSelectBox('instance').option('value', null);
    //                            }
    //                        }
    //                    }
    //                },
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    GetReportTypes() {
        //debugger;
        ajaxHelper.ajaxGetJson("/PartyReports/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                empr_SalesInvoiceReport.InitReportTypeGrid(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitReportTypeGrid(dataSrc) {

        var col = [
            { dataField: 'sno', caption: 'Code', width: '70px' },
            { dataField: 'reporT_NAME', caption: 'Name' },
        ];
        empr_helper.dxGridbindingForReports('#GridContainer', col, dataSrc, "ReportTypes", 'single');
        setTimeout(function () {
            $('#GridContainer').dxDataGrid('instance').selectRowsByIndexes([0]);
        }, 500);
    },

    GetDataToSave() {

        var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();
        var parties = empr_SalesInvoiceReport.PartyData;
        var Salesmans = empr_SalesInvoiceReport.SalesmanData;
        var reportID = '0', sellerPartyCode = '', salesmanCode;
        debugger;
        if (selectedRowKey.length > 0) {
            reportID = selectedRowKey[0].r_ID;
            empr_helper.reportName = selectedRowKey[0].reporT_NAME;
        }

        if (parties.length > 0) {
            var sellerParty = parties.filter(i => i.key == $('#PARTY_NAME').dxSelectBox('option', 'value'));
            if (sellerParty.length > 0) {
                sellerPartyCode = sellerParty[0].partyCode;
            }
        }

        if (Salesmans.length > 0) {
            var sellerParty = parties.filter(i => i.key == $('#SALESMAN_NAME').dxSelectBox('option', 'value'));
            if (sellerParty.length > 0) {
                salesmanCode = sellerParty[0].partyCode;
            }
        }

        var HIDDEN_FROM_DATE = $("#HIDDEN_FROM_DATE").val();
        var HIDDEN_TO_DATE = $("#HIDDEN_TO_DATE").val();
        var FROM_DATE = $("#FROM_DATE").val();
        var TO_DATE = $("#TO_DATE").val();
        var REPORT_ID = reportID;
        var ITEM = $('#ITEM_MASTER').dxSelectBox('option', 'value');
        var BRANCH = $('#Branch').dxSelectBox('option', 'value');
        var SALESMAN = salesmanCode;
        var PARTYCODE = sellerPartyCode;
        var ACTCODE = $('#CONTROL_NAME').dxSelectBox('option', 'value');
        var SACODE_CODE = $('#SCONTROL_NAME').dxSelectBox('option', 'value');
        var GROUP = $('#ITEM_GROUP').dxSelectBox('option', 'value');
        var CATEGORY = $('#CATEGORY').dxSelectBox('option', 'value');
        var SUBCATEGORY = $('#SUBCATEGORY').dxSelectBox('option', 'value');
        var BARCODE = $('#BARCODE').dxSelectBox('option', 'value');
        var SIZE = $('#SIZE').dxSelectBox('option', 'value');
        var COLOR = $('#COLOR').dxSelectBox('option', 'value');
        var LOT = $('#LOT').dxDropDownBox('option', 'value');
        var CONTACT = $("#CMOB").val();

        var record = {
            FROMDATE: FROM_DATE,
            TODATE: TO_DATE,
            HIDDENFROMDATE: HIDDEN_FROM_DATE,
            HIDDENTODATE: HIDDEN_TO_DATE,
            REPORTID: REPORT_ID,
            ITEM: ITEM,
            BRANCH: BRANCH,
            PARTYCODE: PARTYCODE,
            ACTCODE: ACTCODE,
            GROUP: GROUP,
            CATEGORY: CATEGORY,
            SUBCATEGORY: SUBCATEGORY,
            BARCODE: BARCODE,
            SIZE: SIZE,
            LOT: LOT,
            COLOR: COLOR,
            CONTACT: CONTACT,
            SACTCODE: SACODE_CODE,
            SALESMAN: SALESMAN
        }
        return record;
    },

    GenerateReport() {
        //debugger;
        var dataModel = empr_SalesInvoiceReport.GetDataToSave();
        console.log("data model : ", dataModel)
        ajaxHelper.ajaxPostJsonData(dataModel, "/SalesInvoiceReport/GenerateReport", function (data) {
            if (data.msgType == 1) {
                empr_SalesInvoiceReport.InitReportGrid(data.data, dataModel.REPORTID);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitReportGrid(dataSrc, reportId) {
        debugger;
        console.log("InitReportGrid dataSrc", dataSrc)

        console.log(reportId)
        var col = [];
        var detailColumns = [];
        let serialNumber = 0;
        empr_helper.grandTotalPrice = 0;
        empr_helper.grandTotalWPrice = 0;
        empr_helper.grandTotalSPrice = 0;
        if (reportId == 18) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                {
                    dataField: 'voucherDate',
                    caption: 'Date',
                    dataType: 'date',
                    format: 'dd/MM/yyyy',
                    groupIndex: 1,
                    groupCellTemplate: function (element, info) {
                        const date = new Date(info.value);
                        const formattedDate = date.toLocaleDateString('en-GB');
                        const voucherNo = info.data.items[0].voucherNo;
                        const traN_ID = info.data.items[0].traN_ID;
                        const address = info.data.items[0].link;

                        element.append(`Date: ${formattedDate} Transaction#: `);

                        const anchor = $('<a>', {
                            href: '#',
                            text: voucherNo,
                            class: 'dx-link',
                            click: function (e) {
                                e.preventDefault();
                                empr_SalesInvoiceReport.openVoucherPage(address, traN_ID);
                            }
                        });

                        element.append(anchor);
                    }
                },
                { dataField: 'partyName', caption: 'Party Name', width: 250 },
                { dataField: 'itemId', caption: 'Model#' },
                { dataField: 'itemName', caption: 'Product Name', width: 450 },
                { dataField: 'color', caption: 'Color', visible: false },
                { dataField: 'size', caption: 'Size', visible: false },
                { dataField: 'barcode', caption: 'Barcode', visible: false },
                { dataField: 'qty', caption: 'Qty' },
                { dataField: 'rate', caption: 'Rate' },
                { dataField: 'amt', caption: 'Amount', format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 19) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'partyName', caption: 'Party Name', groupIndex: 1 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 90 },
                {
                    dataField: 'voucherNo', caption: 'Transaction#', width: 150,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemId', caption: 'Model#' },
                { dataField: 'itemName', caption: 'Product Name', width: 450 },
                { dataField: 'color', caption: 'Color', visible: false },
                { dataField: 'size', caption: 'Size', visible: false },
                { dataField: 'barcode', caption: 'Barcode', width: 120, visible: false },
                { dataField: 'qty', caption: 'Qty', width: 80 },
                { dataField: 'rate', caption: 'Rate', width: 120 },
                { dataField: 'amt', caption: 'Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 20) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                {
                    dataField: 'itemId',
                    caption: 'Item',
                    groupIndex: 1,
                    groupCellTemplate: function (element, info) {
                        element.text(`Design#: ${info.value} - ${info.data.items[0].itemName}`);
                    }
                },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 90 },
                {
                    dataField: 'voucherNo', caption: 'Transaction#', width: 160,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'partyName', caption: 'Party Name', width: 250 },
                { dataField: 'color', caption: 'Color', visible: false },
                { dataField: 'size', caption: 'Size', visible: false },
                { dataField: 'barcode', caption: 'Barcode', visible: false },
                { dataField: 'qty', caption: 'Qty', width: 120 },
                { dataField: 'rate', caption: 'Rate', width: 120 },
                { dataField: 'amt', caption: 'Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 21) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'partyName', caption: 'Party Name', groupIndex: 1 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 90 },
                {
                    dataField: 'voucherNo', caption: 'Transaction#',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'amt', caption: 'Bill Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'disc', caption: 'Bill Discount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'returnAmt', caption: 'Return Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'returnDisc', caption: 'Return Discount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 22) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', groupIndex: 1 },
                {
                    dataField: 'sno',
                    caption: 'Sno.',
                    cellTemplate: function (container, options) {
                        if (options.rowType === 'data') {
                            if (options.data.voucherDate !== options.component.prevGroupValue) {
                                serialNumber = 1;
                                options.component.prevGroupValue = options.data.voucherDate;
                            } else {
                                serialNumber++;
                            }
                            container.text(serialNumber);
                        }
                    },
                    width: 60
                },
                {
                    dataField: 'voucherNo', caption: 'Transaction#', width: 160,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'partyName', caption: 'Party Name' },
                { dataField: 'amt', caption: 'Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 23) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', groupIndex: 1 },
                {
                    caption: 'Sno.',
                    cellTemplate: function (container, options) {
                        if (options.rowType === 'data') {
                            if (options.data.voucherDate !== options.component.prevGroupValue) {
                                serialNumber = 1;
                                options.component.prevGroupValue = options.data.voucherDate;
                            } else {
                                serialNumber++;
                            }
                            container.text(serialNumber);
                        }
                    },
                    width: 40
                },
                {
                    dataField: 'voucherNo', caption: 'Transaction#', width: 160,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'category', caption: 'Category' },
                { dataField: 'amt', caption: 'Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 24) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200, alignment: 'center',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemId', caption: 'Design#', width: 150 },
                { dataField: 'category', caption: 'Category', width: 110 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 90 },
                { dataField: 'size', caption: 'Size', width: 90 },
                { dataField: 'desc', caption: 'Description', width: 320, },
                {
                    dataField: 'barcode', caption: 'Barcode', groupIndex: 8,
                },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 80 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 85 },
                { dataField: 'balance2', caption: 'Balance', dataType: 'number', width: 80 },
            ];
        }
        else if (reportId == 25) {
            col = [
                { dataField: 'itemId', caption: 'Design#', groupIndex: 0 },
                { dataField: 'category', caption: 'Category' },
                { dataField: 'subCategory', caption: 'Sub Category' },
                { dataField: 'color', caption: 'Color', width: 100 },
                { dataField: 'size', caption: 'Size', width: 100 },
                { dataField: 'barcode', caption: 'Barcode', width: 100 },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 80 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 85 },
            ];
        }
        else if (reportId == 26) {
            col = [
                { dataField: 'itemId', caption: 'Design#', width: 150, groupIndex: 0 },
                { dataField: 'barcode', caption: 'Barcode', width: 100 },
                { dataField: 'color', caption: 'Color', width: 100 },
                { dataField: 'size', caption: 'Size', width: 100 },
                { dataField: 'category', caption: 'Category' },
                { dataField: 'subCategory', caption: 'Sub Category' },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 100 },
            ];
        }
        else if (reportId == 27) {
            col = [
                { dataField: 'category', caption: 'Category', groupIndex: 0 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200, alignment: 'center',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemId', caption: 'Design#', width: 150 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 90 },
                { dataField: 'size', caption: 'Size', width: 90 },
                { dataField: 'desc', caption: 'Description', width: 250 },
                { dataField: 'barcode', caption: 'Barcode', width: 120 },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 80 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 85 },
                { dataField: 'balance', caption: 'Balance', dataType: 'number', width: 80 },
            ];
        }
        else if (reportId == 28) {
            col = [
                { dataField: 'category', caption: 'Category' },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 90 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 90 },
            ];
        }
        else if (reportId == 29) {
            col = [
                { dataField: 'category', caption: 'Category' },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 }
            ];
        }
        else if (reportId == 30) {
            col = [
                { dataField: 'itemId', caption: 'Design#', groupIndex: 0 },
                { dataField: 'sizeSet', caption: 'Size Set', groupIndex: 1 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 180,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'desc', caption: 'Description' },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 80 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 85 },

                //{datafeild}
                { dataField: 'balance', caption: 'Balance', dataType: 'number', width: 80 },

            ];
        }
        else if (reportId == 31) {
            col = [
                { dataField: 'itemId', caption: 'Design#' },
                { dataField: 'sizeSet', caption: 'Size Set', width: 120 },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 },

            ];
        }
        else if (reportId == 32) {
            col = [
                { dataField: 'itemId', caption: 'Design#' },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 80 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 85 },
                { dataField: 'sizeSet', caption: 'Size Set', width: 120 },

            ];
        }
        else if (reportId == 33) {
            col = [
                { dataField: 'itemId', caption: 'Design#' },
                { dataField: 'pPrice', caption: 'Price', dataType: 'number', width: 120 },
                { dataField: 'wPrice', caption: 'Whole Price', dataType: 'number', width: 120 },
                { dataField: 'sPrice', caption: 'Shop Price', dataType: 'number', width: 120 },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 },

            ];
        }
        else if (reportId == 34) {
            col = [
                { dataField: 'itemId', caption: 'Design#', groupIndex: 1, },
                { dataField: 'iteM_CODE', caption: '.', groupIndex: 0 },
                { dataField: 'barcode', caption: 'Barcode', },
                { dataField: 'size', caption: 'Size', width: 120, },
                { dataField: 'color', caption: 'Color', width: 120 },
                { dataField: 'price', caption: 'Price', dataType: 'number', width: 120 },
                { dataField: 'wholePrice', caption: 'Whole Price', dataType: 'number', width: 120 },
                { dataField: 'shopPrice', caption: 'Shop Price', dataType: 'number', width: 120 },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120, },
            ];
        }
        else if (reportId == 35) {
            col = [
                { dataField: 'itemId', caption: 'Design#', alignment: 'center' },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 },
            ];
        }
        else if (reportId == 41) {
            col = [

                { dataField: 'itemId', caption: 'Lot', groupIndex: 0 },
                { dataField: 'partyName', caption: 'Party Name', width: 200, alignment: 'center' },
                { dataField: 'itemName', caption: 'Item Name', width: 200, alignment: 'center' },
                { dataField: 'unit', caption: 'Unit', dataType: 'number', width: 100 },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120, alignment: 'center' }
            ];
        }
        else if (reportId == 42) {
            col = [
                { dataField: 'itemId', caption: 'Lot', groupIndex: 0 },
                { dataField: 'partyName', caption: 'Party' },
                { dataField: 'unit', caption: 'Unit', dataType: 'number', width: 100 },

                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 }
            ];
        }
        else if (reportId == 43) {
            col = [
                { dataField: 'itemId', caption: 'Lot', groupIndex: 0, minWidth: 100 },
                { dataField: 'itemName', caption: 'Item' },
                { dataField: 'unit', caption: 'Unit', dataType: 'number' },

                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 }
            ];
        }
        else if (reportId == 44) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 90 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 160,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemName', caption: 'Item' },
                { dataField: 'partyName', caption: 'Party' },
                { dataField: 'itemId', caption: 'Lot', groupIndex: 0 },
                { dataField: 'desc', caption: 'Description' },
                { dataField: 'debit', caption: 'Stock In', dataType: 'number', width: 120 },
                { dataField: 'credit', caption: 'Stock Out', dataType: 'number', width: 120 },


                { dataField: 'balance', caption: 'Balance', dataType: 'number', width: 120 }
            ];
        }
        else if (reportId == 45) {
            col = [
                { dataField: 'itemName', caption: 'Item Name' },
                { dataField: 'remarks', caption: 'Remarks' },
                { dataField: 'category', caption: 'Category' },
                { dataField: 'subCategory', caption: 'Sub Category' },
                { dataField: 'posQty', caption: 'Quantity', dataType: 'number' },
                { dataField: 'amt', caption: 'Net Amount', dataType: 'number' },
            ];
        }
        else if (reportId == 46) {
            col = [
                { dataField: 'category', caption: 'Category' },
                { dataField: 'posQty', caption: 'Quantity', dataType: 'number' },
                { dataField: 'amt', caption: 'Net Amount', dataType: 'number' },
            ];
        }
        else if (reportId == 47) {
            col = [
                { dataField: 'subCategory', caption: 'Sub Category' },
                { dataField: 'posQty', caption: 'Quantity', dataType: 'number' },
                { dataField: 'amt', caption: 'Net Amount', dataType: 'number' },
            ];
        }
        else if (reportId == 48) {
            col = [
                { dataField: 'groupName', caption: 'Group' },
                { dataField: 'posQty', caption: 'Quantity', dataType: 'number' },
                { dataField: 'amt', caption: 'Net Amount', dataType: 'number' },
            ];
        }
        else if (reportId == 59) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', groupIndex: 0 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 120, },
                { dataField: 'itemName', caption: 'Item', width: 150, },
                { dataField: 'remarks', caption: 'Remarks', width: 150 },
                { dataField: 'category', caption: 'Category', width: 120 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 75 },
                { dataField: 'size', caption: 'Size', width: 70 },
                { dataField: 'barcode', caption: 'Barcode', width: 150 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 70, alignment: 'center', },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80, },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80, },
                { dataField: 'disc', caption: 'Disc %', dataType: 'number', width: 70, },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', dataType: 'number', width: 80, },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100, },
            ];
        }
        else if (reportId == 60) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                {
                    dataField: 'voucherNo', caption: 'Voucher No', width: 120,
                },
                { dataField: 'itemName', caption: 'Item', width: 80, groupIndex: 0 },
                { dataField: 'remarks', caption: 'Remarks', width: 80 },
                { dataField: 'category', caption: 'Category', width: 120 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 80 },
                { dataField: 'size', caption: 'Size', width: 80 },
                { dataField: 'barcode', caption: 'Barcode', width: 100 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 80 },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80 },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 80 },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', dataType: 'number', width: 80 },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
            ];
        }
        else if (reportId == 61) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                {
                    dataField: 'voucherNo', caption: 'Voucher No', width: 120,
                },
                { dataField: 'itemName', caption: 'Item', width: 80, },
                { dataField: 'remarks', caption: 'Remarks', width: 80 },
                { dataField: 'category', caption: 'Category', width: 80, groupIndex: 0 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 80 },
                { dataField: 'size', caption: 'Size', width: 80 },
                { dataField: 'barcode', caption: 'Barcode', width: 100 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 80 },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80 },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 80 },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', dataType: 'number', width: 80 },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
            ];
        }
        else if (reportId == 62) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                {
                    dataField: 'voucherNo', caption: 'Voucher No', width: 120,
                },
                { dataField: 'itemName', caption: 'Item', width: 80, },
                { dataField: 'remarks', caption: 'Remarks', width: 80 },
                { dataField: 'category', caption: 'Category', width: 120 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150, groupIndex: 0 },
                { dataField: 'color', caption: 'Color', width: 80 },
                { dataField: 'size', caption: 'Size', width: 80 },
                { dataField: 'barcode', caption: 'Barcode', width: 100 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 80 },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80 },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 80 },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', dataType: 'number', width: 80 },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
            ];
        }
        else if (reportId == 63) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                {
                    dataField: 'voucherNo', caption: 'Voucher No', width: 120,
                },
                { dataField: 'itemName', caption: 'Item', width: 80, },
                { dataField: 'remarks', caption: 'Remarks', width: 80 },
                { dataField: 'category', caption: 'Category', width: 120 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 80 },
                { dataField: 'size', caption: 'Size', width: 80 },
                { dataField: 'barcode', caption: 'Barcode', width: 100, groupIndex: 0 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 80 },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80 },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 80 },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', dataType: 'number', width: 80 },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
            ];
        }
        else if (reportId == 64) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number' },
                { dataField: 'totalSales', caption: 'Total Sales', dataType: 'number' },
                { dataField: 'cash', caption: 'Cash', dataType: 'number' },
                { dataField: 'cardType', caption: 'Card Type', dataType: 'number' },
                { dataField: 'party', caption: 'Party', dataType: 'number' }
            ];
        }
        else if (reportId == 65) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', groupIndex: 0 },
                {
                    dataField: 'voucherNo', caption: 'Voucher No', width: 120,
                },
                { dataField: 'itemName', caption: 'Item', width: 80, },
                { dataField: 'remarks', caption: 'Remarks', width: 80 },
                { dataField: 'category', caption: 'Category', width: 120 },
                { dataField: 'subCategory', caption: 'Sub Category', width: 150 },
                { dataField: 'color', caption: 'Color', width: 80 },
                { dataField: 'size', caption: 'Size', width: 80 },
                { dataField: 'barcode', caption: 'Barcode', width: 120 },
                { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 80 },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80 },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 80 },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', dataType: 'number', width: 80 },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
                { dataField: 'stock', caption: 'Stock', dataType: 'number', width: 80 },
            ];
        }
        else if (reportId == 66) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', groupIndex: 0 },
                {
                    dataField: 'voucherNo', caption: 'Voucher No', width: 130,
                    cssClass: 'centered-column'
                },
                { dataField: 'itemName', caption: 'Item', width: 80, cssClass: 'centered-column' },
                { dataField: 'size', caption: 'Size', width: 80, cssClass: 'centered-column' },
                {
                    caption: "SALE",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>SALE</strong>");
                    },
                    columns: [
                        { dataField: 'posQty', caption: 'Qty', dataType: 'number', width: 80 },
                        { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 80 },
                        { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                        { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 80 },
                        { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
                    ]
                },
                {
                    caption: "PURCHASE",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>PURCHASE</strong>");
                    },
                    columns: [
                        { dataField: 'pbRate', caption: 'Rate', dataType: 'number', width: 80 },
                        { dataField: 'pAmt', caption: 'Amount', dataType: 'number', width: 80 },
                    ]
                },
                { dataField: 'profitAndLoss', caption: 'Profit / Loss', dataType: 'number', width: 90, cssClass: 'centered-column' },

            ];
        }
        else if (reportId == 36) {
            col = [
                {
                    dataField: 'bTransfer', caption: 'Branch', dataType: 'text', groupIndex: 0
                },
                { dataField: 'itemId', caption: 'Item Name', groupIndex: 1 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110, cssClass: 'centered-column' },
                //{
                //    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center',
                //    cssClass: 'centered-column'
                //},
                {
                    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    },
                    width: 150,
                },
                { dataField: 'size', caption: 'Size', cssClass: 'centered-column', width: 90 },
                { dataField: 'color', caption: 'Color', cssClass: 'centered-column', width: 90 },
                { dataField: 'qty', caption: 'Qty', alignment: 'center', width: 80, dataType: 'number' },
                {
                    caption: "WHOLESALE",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>WHOLESALE</strong>");
                    },
                    columns: [
                        { dataField: 'wRate', caption: 'Rate', dataType: 'number', width: 100 },
                        { dataField: 'wAmt', caption: 'Amount', dataType: 'number', width: 100 },
                    ]
                },
                {
                    caption: "RETAIL",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>RETAIL</strong>");
                    },
                    columns: [
                        { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 100 },
                        { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 100, },
                    ]
                },
            ];
        }
        else if (reportId == 67) {
            col = [
                { dataField: 'bName', caption: 'Branch', dataType: 'text', cssClass: 'centered-column', groupIndex: 0 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #',
                    cssClass: 'centered-column',
                },
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'partyName', caption: 'Name', width: 280 },
                { dataField: 'total', caption: 'Gross Sale', width: 105 },
                { dataField: 'disc', caption: 'Disc %', width: 70 },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', width: 105 },
                { dataField: 'partyTax', caption: 'Tax %', width: 70 },
                { dataField: 'taxAmt', caption: 'Tax Amt', width: 105 },
                { dataField: 'netAmt', caption: 'Included Tax', width: 105 },
            ];
        }
        else if (reportId == 74) {
            col = [
                {
                    dataField: 'bTransfer', caption: 'Branch', dataType: 'text', groupIndex: 0
                },
                { dataField: 'itemId', caption: 'Item Name', groupIndex: 1 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110, cssClass: 'centered-column' },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center',
                    cssClass: 'centered-column'
                },
                { dataField: 'size', caption: 'Size', cssClass: 'centered-column', width: 80 },
                { dataField: 'color', caption: 'Color', cssClass: 'centered-column', width: 80 },
                { dataField: 'qty', caption: 'Qty', alignment: 'center', width: 80, dataType: 'number' },
                {
                    caption: "WHOLESALE",
                    alignment: "center", width: 200,
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>WHOLESALE</strong>");
                    },
                    columns: [
                        { dataField: 'wRate', caption: 'Rate', dataType: 'number', width: 100 },
                        { dataField: 'wAmt', caption: 'Amount', dataType: 'number', width: 100 },
                    ]
                },
                {
                    caption: "RETAIL",
                    alignment: "center", width: 200,
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>RETAIL</strong>");
                    },
                    columns: [
                        { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 100 },
                        { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 100, },
                    ]
                },
            ];
        }
        else if (reportId == 75) {
            col = [
                { dataField: 'itemId', caption: 'Item Name', cssClass: 'centered-column' },
                { dataField: 'category', caption: 'Category', cssClass: 'centered-column' },
                { dataField: 'cset', caption: 'Set', cssClass: 'centered-column', width: 100, alignment: 'center' },
                { dataField: 'qty', caption: 'Qty', alignment: 'center', width: 80, dataType: 'number', cssClass: 'centered-column' },
                {
                    caption: "AMOUNT",
                    alignment: "center", width: 200,
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>AMOUNT</strong>");
                    },
                    columns: [
                        { dataField: 'amt', caption: 'Retail', dataType: 'number', width: 100 },
                        { dataField: 'wAmt', caption: 'Wholesale', dataType: 'number', width: 100 },
                    ]
                },

            ];
        }
        else if (reportId == 76) {
            col = [
                { dataField: 'bName', caption: 'Barnch', groupIndex: 0 },
                { dataField: 'actName', caption: 'Account Name', groupIndex: 1 },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'desc', caption: 'Description', alignment: 'center' },
                { dataField: 'amt', caption: 'Amount', alignment: 'center', dataType: 'number' },
            ];
        }
        else if (reportId == 87) {
            col = [
                { dataField: 'mobile', caption: 'Mobile', width: 150 },
                { dataField: 'cName', caption: 'Company', width: 150 },
                { dataField: 'cAdd', caption: 'Address' },
                { dataField: 'recv', caption: 'Receive', dataType: 'number', width: 100 },
            ];
        }
        else if (reportId == 88) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemName', caption: 'Product Name', groupIndex: 1 },
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'groupName', caption: 'Group', },
                { dataField: 'desc', caption: 'Description', width: 320, },
                //{ dataField: 'debit', caption: 'Stock In', dataType: 'number', },
                //{ dataField: 'credit', caption: 'Stock Out', dataType: 'number', },
                //{ dataField: 'balance2', caption: 'Balance', dataType: 'number', }
                {
                    dataField: 'debit',
                    caption: 'Stock In',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },
                {
                    dataField: 'credit',
                    caption: 'Stock Out',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },
                {
                    dataField: 'balance2',
                    caption: 'Balance',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                }
            ];
        }
        else if (reportId == 89) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'itemName', caption: 'Product Name', },
                { dataField: 'category', caption: 'Group' },
                { dataField: 'subCategory', caption: 'Model#' },
                //{ dataField: 'debit', caption: 'Stock In', dataType: 'number' },
                //{ dataField: 'credit', caption: 'Stock Out', dataType: 'number' },
                {
                    dataField: 'debit',
                    caption: 'Stock In',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },
                {
                    dataField: 'credit',
                    caption: 'Stock Out',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },
                //{ dataField: 'balance2', caption: 'Balance', dataType: 'number', width: 85 }
            ];
        }
        else if (reportId == 90) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'itemName', caption: 'Product Name', },
                { dataField: 'category', caption: 'Group' },
                { dataField: 'subCategory', caption: 'Model#' },
                //{ dataField: 'balance', caption: 'Balance', dataType: 'number', width: 100 }
                {
                    dataField: 'balance',
                    caption: 'Balance',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                }
            ];
        }
        else if (reportId == 94) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center', width: 130
                },
                { dataField: 'mobile', caption: 'Mobile', groupIndex: 0 },
                { dataField: 'cName', caption: 'Company', width: 150 },
                { dataField: 'cAdd', caption: 'Address' },
                { dataField: 'recv', caption: 'Receive', dataType: 'number', width: 100 },
                { dataField: 'mode', caption: 'Mode', width: 110 },
            ];
        }
        else if (reportId == 117) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center', width: 130
                },
                { dataField: 'mobile', caption: 'Mobile' },
                { dataField: 'cName', caption: 'Name', width: 150 },
                { dataField: 'cAdd', caption: 'Address' },
                { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
                { dataField: 'deliveryCharges', caption: 'Delivery Charges', dataType: 'number', width: 100 },
                { dataField: 'advance', caption: 'Advance', dataType: 'number', width: 100 },
                { dataField: 'total', caption: 'Remaining', dataType: 'number', width: 100 },
                { dataField: 'mode', caption: 'Mode', width: 110 }
            ];
            detailColumns = [
                { dataField: 'itemName', caption: 'Item', width: 150 },
                { dataField: 'qty', caption: 'Qty', dataType: 'number', width: 50 },
            ]
        }
        else if (reportId == 118) {
            col = [
                //{ dataField: 'bName', caption: 'Branch' },
                { dataField: 'cmob', caption: 'Mobile', width: 200 },
                { dataField: 'cname', caption: 'Name', width: 200 },
                { dataField: 'cadd', caption: 'Address' },
            ];
        }
        else if (reportId == 125) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 100, },
                { dataField: 'partyName', caption: 'Party', width: 150, },
                { dataField: 'itemName2', caption: 'Item', width: 150, },
                { dataField: 'rate', caption: 'Rate', width: 80, alignment: 'center', },
                { dataField: 'qty', caption: 'Qty', width: 70, alignment: 'center', },
                { dataField: 'amt', caption: 'Amount', width: 100, alignment: 'center', },
                { dataField: 'disc', caption: 'Disc %', width: 70, },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', width: 80, },
                { dataField: 'tax', caption: 'Tax %', width: 90, alignment: 'center', },
                { dataField: 'taxAmt', caption: 'Tax Amt', width: 80, },
                { dataField: 'billMode', caption: 'Bill Mode', width: 100, alignment: 'center', },
                { dataField: 'branch', caption: 'Branch', width: 150, groupIndex: 0 },
                { dataField: 'inclTax', caption: 'Incl Tax', width: 100, alignment: 'center', },
                { dataField: 'remarks', caption: 'Remarks', width: 160, },
            ];
        }
        else if (reportId == 126) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 70 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemName2', caption: 'Item', width: 200 },
                { dataField: 'desc', caption: 'Remarks', width: 200 },
                { dataField: 'stockIn', caption: 'stockIn', dataType: 'number', },
                { dataField: 'stockOut', caption: 'stockOut', dataType: 'number', },
                //{ dataField: 'traN_ID', caption: 'Code', dataType: 'number', },
                { dataField: 'warehouse', caption: 'Warehouse', groupIndex: 0 },
                { dataField: 'balance2', caption: 'Balance', dataType: 'number', }
            ];
        }
        else if (reportId == 127) {
            col = [
                { dataField: 'itemName2', caption: 'Item', width: 150, },
                { dataField: 'warehouse', caption: 'Warehouse', width: 150, groupIndex: 0 },
                { dataField: 'category', caption: 'Category', width: 80, },
                { dataField: 'subCategory', caption: 'SubCategory', width: 80, },
                { dataField: 'stockIn', caption: 'stockIn', dataType: 'number', width: 80, },
                { dataField: 'stockOut', caption: 'stockOut', dataType: 'number', width: 80, },
                //{ dataField: 'balance2', caption: 'Balance', dataType: 'number', width: 85 }
            ];
        }
        else if (reportId == 128) {
            col = [
                { dataField: 'itemName2', caption: 'Item', width: 150, },
                { dataField: 'category', caption: 'Category', width: 80, },
                { dataField: 'subCategory', caption: 'SubCategory', width: 80, },
                { dataField: 'warehouse', caption: 'Warehouse', width: 150, groupIndex: 0 },
                { dataField: 'balance', caption: 'Balance', dataType: 'number', width: 80, },
            ];
        }
        else if (reportId == 129) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 100 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    },
                    width: 150,
                },
                { dataField: 'qty', caption: 'Qty', dataType: 'number', width: 100, },
                { dataField: 'debit', caption: 'Debit', dataType: 'number' },
                { dataField: 'credit', caption: 'Credit', dataType: 'number' },
                { dataField: 'bTransfer', caption: 'Transfer', groupIndex: 0 },
                { dataField: 'balance2', caption: 'Balance', dataType: 'number', },
            ];
        }
        else if (reportId == 130) {
            col = [
                { dataField: 'bTransfer', caption: 'Transfer', dataType: 'number', alignment: 'left', },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 150, },
            ];
        }
        else if (reportId == 95) {
            //debugger;
            col = [

                { dataField: 'category', caption: 'Categories', width: 120 },
                { dataField: 'pPrice', caption: 'Price', dataType: 'number', width: 120 },
                { dataField: 'wPrice', caption: 'Whole Price', dataType: 'number', width: 120 },
                { dataField: 'sPrice', caption: 'Shop Price', dataType: 'number', width: 120 },
                { dataField: 'totalBalance', caption: 'Balance', dataType: 'number', width: 120 },
            ];
        }
        else if (reportId == 100) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                {
                    dataField: 'voucherDate', caption: 'Date', dataType: 'date',
                    allowSorting: false,
                    format: function (date) {
                        if (!date) return '';
                        const month = date.toLocaleString('en-US', { month: 'short' });
                        const year = date.getFullYear();
                        return `${month}-${year}`;
                    }
                },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 130, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'desc', caption: 'Discount', dataType: 'number', width: 130, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'returnAmt', caption: 'R.Amount', width: 130, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'returnDisc', caption: 'R.Discount', width: 130, format: { type: 'fixedPoint', precision: 0 } },
            ];
        }
        else if (reportId == 132 || reportId == 133) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'voucherNo', caption: 'Transaction #', },
                { dataField: 'total', caption: 'Bill Amount' },
                { dataField: 'tax', caption: 'Tax' },
                { dataField: 'recv', caption: 'Recieved Amt', dataType: 'number' },
                { dataField: 'billMode', caption: 'Qty', dataType: 'number', alignment: 'center', },
                { dataField: 'srB_INV', caption: 'SRB Invoice' },
            ];
        }
        else if (reportId == 134) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'voucherNo', caption: 'Transaction #', },
                { dataField: 'recv', caption: 'Bill Amount', dataType: 'number' },
                { dataField: 'cname', caption: 'Customer Name' },
                { dataField: 'cmob', caption: 'Customer Number' },
                { dataField: 'salesman', caption: 'Salesman', groupIndex: 0 },
                { dataField: 'percentage', caption: 'Commission %' },
                { dataField: 'commission', caption: 'Commission Value' },
            ];
        }
        ///add report pos salesman item wise 
        else if (reportId == 135) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'voucherNo', caption: 'Transaction #', },
                { dataField: 'recv', caption: 'Bill Amount', dataType: 'number' },
                { dataField: 'cname', caption: 'Customer Name' },
                { dataField: 'cmob', caption: 'Customer Number' },
                { dataField: 'remarks', caption: 'Item' },
                { dataField: 'salesman', caption: 'Salesman', groupIndex: 0 },
                { dataField: 'percentage', caption: 'Commission %' },
                { dataField: 'commission', caption: 'Commission Value' },
            ];
        }
        else if (reportId == 137) {
            col = [
                { dataField: 'duE_DATE', caption: 'Due Date', dataType: 'date', format: 'dd/MM/yyyy', width: 100, alignment: 'left', },
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'voucherNo', caption: 'Transaction #', },
                { dataField: 'partyName', caption: 'Party' },
                { dataField: 'branch', caption: 'Branch', width: 150, groupIndex: 0 },
                { dataField: 'itemName2', caption: 'Item', width: 150 },
                { dataField: 'amt', caption: 'Amount', width: 150, alignment: 'center', },

                { dataField: 'remarks', caption: 'Remarks' },

            ]
        }
        else if (reportId == 138) {
            col = [
                { dataField: 'vDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 100, },
                { dataField: 'partyName', caption: 'Party', width: 150, },
                { dataField: 'itemName2', caption: 'Item', width: 150, },
                { dataField: 'rate', caption: 'Rate', width: 80, alignment: 'center', },
                { dataField: 'qty', caption: 'Qty', width: 70, alignment: 'center', },
                { dataField: 'amt', caption: 'Amount', width: 100, alignment: 'center', },
                { dataField: 'waiter', caption: 'Waiter', width: 120, alignment: 'center', },
                { dataField: 'table', caption: 'Table', width: 120, alignment: 'center', },
                { dataField: 'disc', caption: 'Disc %', width: 70, },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', width: 80, },
                { dataField: 'tax', caption: 'Tax %', width: 90, alignment: 'center', },
                { dataField: 'taxAmt', caption: 'Tax Amt', width: 80, },
                { dataField: 'billMode', caption: 'Bill Mode', width: 100, alignment: 'center', },
                { dataField: 'branch', caption: 'Branch', width: 150, groupIndex: 0 },
                { dataField: 'inclTax', caption: 'Incl Tax', width: 100, alignment: 'center', },
                { dataField: 'remarks', caption: 'Remarks', width: 160, },
            ];
        }
        else if (reportId == 140) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'partyName', caption: 'Party Name', groupIndex: 1 },
                { dataField: 'region', caption: 'Region', visible: false },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 90 },
                {
                    dataField: 'voucherNo', caption: 'Transaction#', width: 150,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SalesInvoiceReport.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemId', caption: 'Model#' },
                { dataField: 'itemName', caption: 'Product Name', width: 250 },
                { dataField: 'color', caption: 'Color', visible: false },
                { dataField: 'size', caption: 'Size', visible: false },
                { dataField: 'barcode', caption: 'Barcode', width: 120, visible: false },
                { dataField: 'qty', caption: 'Carton', width: 80 },
                { dataField: 'pack', caption: 'Pcs', width: 80, visible: false },
                { dataField: 'totalPack', caption: 'T.Pcs', width: 80, visible: false },
                { dataField: 'weight', caption: 'Weight', width: 80, visible: false },
                { dataField: 'rate', caption: 'Rate', width: 120 },
                { dataField: 'amt', caption: 'Amount', width: 120, format: { type: 'fixedPoint', precision: 0 } },
                { dataField: 'disc', caption: 'Disc', width: 120, visible: false },
                { dataField: 'mDisc_Amt', caption: 'Disc Amt', width: 120, visible: false },
                { dataField: 'tax', caption: 'Tax', width: 120, visible: false },
                { dataField: 'taxAmt', caption: 'Tax Amt', width: 120, visible: false },
                { dataField: 'adV_TAX', caption: 'Adv Tax', width: 120, visible: false },
                { dataField: 'adV_TAX_AMT', caption: 'Adv Tax Amt', width: 120, visible: false },
                { dataField: 'netAmt', caption: 'Net Amt', width: 120, visible: false },

            ];
        }
        else if (reportId == 141) {
            col = [
                { dataField: 'month', caption: 'Month' },
                { dataField: 'totaL_QTY', caption: 'Qty' },
                { dataField: 'totaL_R_PRICE', caption: 'Shop Price' },

                { dataField: 'totaL_DISC', caption: 'Disc Amt' },
                { dataField: 'totaL_PRICE', caption: 'Total Sale' },
                { dataField: 'totaL_WS_PRICE', caption: 'Whole Sale Price' },

                { dataField: 'stkT_WS_PRICE', caption: 'Stock Transfer' },
                { dataField: 'totaL_EXP', caption: 'Shop Expenses' },
                { dataField: 'factorY_TRANSFER', caption: 'Factory Transfer' },

                { dataField: 'casH_SALE', caption: 'Cash Sale' },
                { dataField: 'banK_W_CH', caption: 'Bank Sale' },
                {
                    dataField: "neT_PROFIT1",
                    caption: "Net Profit (WS Basis)",
                    dataType: "number",
                    format: {
                        type: "fixedPoint",
                        precision: 0
                    },
                    cellTemplate: function (container, options) {

                        if (options.value < 0) {
                            $("<span>")
                                .text("(" + Math.abs(options.value) + ")")
                                .css("color", "red")
                                .appendTo(container);
                        } else {
                            $("<span>")
                                .text(options.valueText)
                                .appendTo(container);
                        }
                    }
                },
                { dataField: 'totaL_COST', caption: 'Total Cost' },
                { dataField: 'neT_PROFIT2', caption: ' Net Profit (Cost Basis)' },
            ];
        }
        else if (reportId == 91) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'itemName', caption: 'Product Name' },
                {
                    dataField: 'rate', caption: 'Rate',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },
                {
                    dataField: 'amt', caption: 'Amount',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },
                {
                    dataField: 'balance',
                    caption: 'Qty',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined || e.value === 0) return '';

                        return e.value.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    }
                },


            ];
        }

        else if (reportId == 145) {
            col = [
                { dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'itemName', caption: 'Product Name', },
                { dataField: 'category', caption: 'Category' },
                { dataField: 'subCategory', caption: 'Sub Category' },
                { dataField: 'lastSaleDate', caption: 'Last Sale Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                {
                    dataField: 'totalSales', caption: 'Days Since Last Sale',
                    customizeText: function (e) {
                        if (e.value === 0 || e.value === null || e.value === undefined) {
                            return '';
                        }
                        return e.value.toFixed(0);
                    }
                },
                {
                    dataField: 'balance',
                    caption: 'Balance',
                    dataType: 'number',
                    customizeText: function (e) {
                        if (e.value === null || e.value === undefined) return '';

                        // Negative value ko bina minus sign ke format karne ke liye Math.abs use kiya hai
                        let absoluteValue = Math.abs(e.value);

                        return absoluteValue.toLocaleString('en-US', {
                            useGrouping: false,
                            maximumFractionDigits: 20
                        });
                    },
                    cellTemplate: function (container, options) {
                        if (options.value === null || options.value === undefined) return;

                        if (options.value < 0) {
                            // Agar value negative hai toh red color aur round brackets () lagenge
                            $("<span>")
                                .text("(" + options.text + ")") // options.text me customizeText wala formatted number ayega
                                .css("color", "red")
                                .appendTo(container);
                        } else {
                            // Positive value normal black color me dikhegi
                            $("<span>")
                                .text(options.text)
                                .appendTo(container);
                        }
                    }
                }
            ];
        }
        //debugger;
        if ($('#ReportGridContainer').data('dxDataGrid') != undefined) {
            $('#ReportGridContainer').data('dxDataGrid').dispose();
        }
        if (reportId == 117) {
            empr_helper.MasterDetailDxGridBinding('#ReportGridContainer', col, detailColumns, dataSrc, "Advance");
        }
        debugger;

        ////landscape
        if (reportId == 138 || reportId == 60 || reportId == 137 || reportId == 59 | reportId == 61 || reportId == 62 || reportId == 63 || reportId == 65 || reportId == 125 || reportId == 145) {
            empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName, true);
            console.log('1 DxGridBindingForReportsWithSetting_Aging');
        }
        debugger;
        ////potrait
        if (reportId == 21 || reportId == 22 || reportId == 23 || reportId == 100 || reportId == 45 || reportId == 24 || reportId == 27 || reportId == 64 || reportId == 66 || reportId == 63 || reportId == 76 || reportId == 87
            || reportId == 65 || reportId == 18 || reportId == 19 || reportId == 20 || reportId == 46 || reportId == 47 || reportId == 48 || reportId == 88 || reportId == 89 || reportId == 90 || reportId == 91 || reportId == 94 || reportId == 117
            || reportId == 118 || reportId == 132 || reportId == 133 || reportId == 134 || reportId == 135 || reportId == 67 || reportId == 140 || reportId == 141) {
            empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            console.log('2 DxGridBindingForReportsWithSetting_Aging');
        }
        else {
            empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            console.log('3 DxGridBindingForReportsWithSetting');
        }
        debugger;

        setTimeout(function () {
            if (reportId == 117) {
                empr_helper.MasterDetailDxGridBinding('#ReportGridContainer', col, detailColumns, dataSrc, "Advance");
            }

            if (reportId == 21 || reportId == 22 || reportId == 23 || reportId == 100 || reportId == 45 || reportId == 24 || reportId == 27 || reportId == 66 || reportId == 64 || reportId == 62 || reportId == 63 || reportId == 76 || reportId == 87 ||
                reportId == 65 || reportId == 18 || reportId == 19 || reportId == 20 || reportId == 46 || reportId == 47 || reportId == 48 || reportId == 88 || reportId == 89 || reportId == 90 || reportId == 91 || reportId == 94 || reportId == 117
                || reportId == 118 || reportId == 132 || reportId == 133 || reportId == 134 || reportId == 135 || reportId == 67 || reportId == 140 || reportId == 141 ) {
                empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }
            else {
                empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }
            if (reportId == 138 || reportId == 60 || reportId == 137 || reportId == 59 | reportId == 61 || reportId == 62 || reportId == 63 || reportId == 65 || reportId == 125 || reportId == 145) {
                empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName, true);

            }
        }, 400);
        setTimeout(function () {

            var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();
            if (selectedRowKey.length > 0) {
                $('#REPORT_NAME').text(selectedRowKey[0].reporT_NAME);
            }
            $('#OptionTab').removeClass('active');
            $('#OptionTabContent').removeClass('active');
            $('#ViewTab').click();
            $('.tab-pane').removeClass('fade');
            $('#ViewTab').addClass('active')
            $('#ViewTabContent').addClass('active');
        }, 500);
    },

    ValidateInfo() {
        //debugger;
        var valid = true;
        var data = empr_SalesInvoiceReport.GetDataToSave();
        //if (!data.FROMDATE >= StartD || !data.TODATE > EndD ) {
        //    empr_helper.notify("Date not in allowed period", 2);
        //    valid = false;
        //}

        var userfromDateObj = new Date(data.FROMDATE);
        var usertoDateObj = new Date(data.TODATE);

        var periodFromDateObj = new Date(data.HIDDENFROMDATE);
        var periodToDateObj = new Date(data.HIDDENTODATE);





        if (data.FROMDATE == "" || data.FROMDATE == null || data.FROMDATE == undefined) {
            empr_helper.notify("Please select FromDate.", 2);
            valid = false;
        } else {
            empr_helper.fromDate = data.FROMDATE;
        }

        if (data.TODATE == "" || data.TODATE == null || data.TODATE == undefined) {
            empr_helper.notify("Please select ToDate.", 2);
            valid = false;
        } else {
            empr_helper.toDate = data.TODATE;
        }

        if (data.FROMDATE > data.TODATE) {
            empr_helper.notify("'To Date' must be greater than or equal to 'From Date'.", 2);
            valid = false;
        }

        if (data.FROMDATE != null && data.HIDDENFROMDATE != null) {

            if (userfromDateObj < periodFromDateObj || userfromDateObj > periodToDateObj) {
                empr_helper.notify("Selected dates are outside the active period.", 2);
                valid = false;
            }
        }

        if (data.TODATE != null && data.HIDDENTODATE != null) {

            if (usertoDateObj > periodToDateObj || usertoDateObj < periodFromDateObj) {
                empr_helper.notify("Selected dates are outside the active period.", 2);
                valid = false;
            }
        }

        //if (data.TODATE != null && data.HIDDENTODATE != null) {
        //    var toDateObj = new Date(data.TODATE);
        //    var hiddenToDateObj = new Date(data.HIDDENTODATE);

        //    if (toDateObj > hiddenToDateObj) {
        //        empr_helper.notify("please select valid date in ToDate which ends at " + hiddenToDateObj.toLocaleDateString(), 2);
        //        valid = false;
        //    }
        //}

        if (data.REPORTID < 1) {
            empr_helper.notify("Please select report type.", 2);
            valid = false;
        }
        $("#Loader").hide();
        return valid;
    },

    InitItemGroupDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetItemGroup", function (data) {
            if (data.msgType == 1) {
                empr_SalesInvoiceReport.ItemGroupData = data.data;
                $('#ITEM_GROUP').dxSelectBox({
                    dataSource: data.data,
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
                            if (!empr_SalesInvoiceReport.IsCustomSelection) {
                                $('#ITEM_MASTER').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            $('#ITEM_MASTER').dxSelectBox('instance').option('value', '');
                        }
                    },
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitBranchDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetBranch", function (data) {
            if (data.msgType == 1) {
                empr_SalesInvoiceReport.BranchData = data.data;
                $('#Branch').dxSelectBox({
                    dataSource: data.data,
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
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitItemMasterDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetItemMaster", function (data) {
            if (data.msgType == 1) {
                empr_SalesInvoiceReport.ItemData = data.data;
                $('#ITEM_MASTER').dxSelectBox({
                    //dataSource: data.data,
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
                    dataSource: {
                        store: data.data,
                        paginate: true,
                        pageSize: 50
                    },
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    },
                    pagingEnabled: true,
                    searchTimeout: 500,
                    onValueChanged: function (e) {
                        if (e.value != '' && e.value != null) {
                            var items = e.component._dataSource._items;
                            var item = items.filter(i => i.key == e.value);
                            if (item.length > 0) {
                                var controls = empr_SalesInvoiceReport.ItemGroupData;
                                var control = controls.filter(i => i.key == item[0].groupCode);
                                if (control.length > 0) {
                                    empr_SalesInvoiceReport.IsCustomSelection = true;
                                    $('#ITEM_GROUP').dxSelectBox('instance').option('value', control[0].key);
                                    //if (item[0].itemId != "" && item[0].itemId != null) {
                                    //    $('#ITEM_ID').dxSelectBox('instance').option('value', item[0].itemId);
                                    //} else {
                                    //    $('#ITEM_ID').dxSelectBox('instance').option('value', '');
                                    //}
                                    setTimeout(function () {
                                        empr_SalesInvoiceReport.IsCustomSelection = false;
                                    }, 500);
                                }
                            }
                        }
                    },
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    //InitItemIdDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetItemIds", function (data) {
    //        if (data.msgType == 1) {
    //            $('#ITEM_ID').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    if (e.value != '' && e.value != null) {
    //                        var ids = e.component._dataSource._items;
    //                        var id = ids.filter(i => i.key == e.value);
    //                        if (id.length > 0) {
    //                            var items = empr_SalesInvoiceReport.ItemData;
    //                            var item = items.filter(i => i.itemId == id[0].key);
    //                            if (item.length > 0) {
    //                                console.log(item[0].itemId)
    //                                if (item[0].itemId != "" && item[0].itemId != null) {
    //                                    $('#ITEM_GROUP').dxSelectBox('instance').option('value', item[0].groupCode);
    //                                    $('#ITEM_MASTER').dxSelectBox('instance').option('value', item[0].key);
    //                                } else {
    //                                    $('#ITEM_GROUP').dxSelectBox('instance').option('value', '');
    //                                    $('#ITEM_MASTER').dxSelectBox('instance').option('value', '');
    //                                }
    //                            } else {
    //                                $('#ITEM_GROUP').dxSelectBox('instance').option('value', '');
    //                                $('#ITEM_MASTER').dxSelectBox('instance').option('value', '');
    //                            }
    //                        }
    //                    }
    //                },
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitCategoryDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetCategories", function (data) {
    //        if (data.msgType == 1) {
    //            $('#CATEGORY').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitSubCategoryDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetSubCategories", function (data) {
    //        if (data.msgType == 1) {
    //            $('#SUBCATEGORY').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitBarcodeDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetBarcodes", function (data) {
    //        if (data.msgType == 1) {
    //            $('#BARCODE').dxSelectBox({
    //                //dataSource: data.data,
    //                dataSource: {
    //                    store: data.data,
    //                    paginate: true,
    //                    pageSize: 50
    //                },
    //                paging: {
    //                    enabled: true,
    //                    pageSize: 50,
    //                },
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitColorDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetColors", function (data) {
    //        if (data.msgType == 1) {
    //            $('#COLOR').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    //InitLotDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetLots", function (data) {
    //        if (data.msgType == 1) {

    //            let gridInstance;
    //            let currentSearchTerm = "";
    //            let isProgrammaticOpen = false;

    //            $("#LOT").dxDropDownBox({
    //                value: selectedValue,
    //                valueExpr: "grcode",
    //                displayExpr: function (item) {
    //                    return item ? `${item.value}` : "Select a value...";
    //                },
    //                dataSource: data.data,
    //                acceptCustomValue: true,
    //                showClearButton: true,
    //                deferRendering: false,
    //                openOnFieldClick: false,
    //                onValueChanged: function (e) {
    //                    if (e.value && gridInstance) {
    //                        const selectedData = gridInstance.getDataSource().items().find(item => item.grcode === e.value);
    //                        if (selectedData) {
    //                            $('#LOT_hidden').val(selectedData.grcode);
    //                            $('#displayExpr_LOT').val(selectedData.value);
    //                        }
    //                    } else {
    //                        $('#LOT_hidden').val('');
    //                        $('#displayExpr_LOT').val('');
    //                    }
    //                },
    //                onOpened: function (e) {
    //                    if (!currentSearchTerm && gridInstance) {
    //                        gridInstance.getDataSource().filter(null);
    //                        gridInstance.refresh();
    //                    }

    //                    setTimeout(() => {
    //                        const input = e.component._$element.find(".dx-texteditor-input").first();
    //                        input.focus();
    //                        if (input.val()) {
    //                            input.select();
    //                        }
    //                    }, 50);
    //                },
    //                onInput: function (e) {
    //                    currentSearchTerm = e.event.target.value;

    //                    if (!e.component.option("opened")) {
    //                        isProgrammaticOpen = true;
    //                        e.component.open();
    //                        setTimeout(() => { isProgrammaticOpen = false; }, 100);
    //                    }

    //                    if (gridInstance) {
    //                        applyGridFilter(gridInstance, currentSearchTerm);
    //                    }
    //                },
    //                contentTemplate: function (e) {
    //                    gridInstance = $("<div>").dxDataGrid({
    //                        dataSource: new DevExpress.data.DataSource({
    //                            store: data.data,
    //                            key: "grcode"
    //                        }),
    //                        keyExpr: "grcode",
    //                        columns: [
    //                            {
    //                                dataField: "value",
    //                                caption: "As Name",
    //                                cellTemplate: function (container, options) {
    //                                    highlightText(container, options.value);
    //                                }
    //                            },
    //                            {
    //                                dataField: "name",
    //                                caption: "Control Name",
    //                                cellTemplate: function (container, options) {
    //                                    highlightText(container, options.value);
    //                                },
    //                                width: 200
    //                            }
    //                        ],
    //                        selection: { mode: "single", showCheckBoxesMode: "always" },
    //                        hoverStateEnabled: true,
    //                        height: 200,
    //                        keyboardNavigation: {
    //                            enabled: true,
    //                            enterKeyAction: "select",
    //                            editOnKeyPress: true
    //                        },
    //                        onSelectionChanged: function (selectedItems) {
    //                            const selected = selectedItems.selectedRowsData[0];
    //                            if (selected) {
    //                                e.component.option("value", selected.grcode);
    //                                e.component.close();

    //                                $('#LOT_hidden').val(selected.grcode);
    //                                $('#displayExpr_LOT').val(selected.value);
    //                            }
    //                        },
    //                        onContentReady: function (e) {
    //                            if (currentSearchTerm) {
    //                                const items = e.component.getDataSource().items();
    //                                if (items.length > 0) {
    //                                    e.component.selectRows([items[0].grcode], false);
    //                                }
    //                            }
    //                        }
    //                    }).dxDataGrid("instance");

    //                    gridInstance.element().on('click', function (event) {
    //                        event.stopPropagation();
    //                    });

    //                    return gridInstance.element();
    //                }
    //            });

    //            // helper functions
    //            function applyGridFilter(grid, searchTerm) {
    //                const dataSource = grid.getDataSource();
    //                if (searchTerm) {
    //                    dataSource.filter([
    //                        ["value", "contains", searchTerm],
    //                        "or",
    //                        ["name", "contains", searchTerm],
    //                        "or",
    //                        ["grcode", "contains", searchTerm]
    //                    ]);
    //                } else {
    //                    dataSource.filter(null);
    //                }
    //                dataSource.load();
    //            }

    //            function highlightText(container, value) {
    //                if (!value) return;
    //                const text = value.toString();
    //                if (!currentSearchTerm || !text.toLowerCase().includes(currentSearchTerm.toLowerCase())) {
    //                    container.text(text);
    //                    return;
    //                }
    //                const regex = new RegExp(currentSearchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), "gi");
    //                const highlighted = text.replace(regex, match =>
    //                    `<span style="background-color: #ffeb3b; font-weight: bold;">${match}</span>`
    //                );
    //                container.html(highlighted);
    //            }

    //            // handle clear button
    //            $(document).on("dxclick", "#LOT .dx-clear-button-area", function (e) {
    //                currentSearchTerm = "";
    //                $('#LOT_hidden').val('');
    //                $('#displayExpr_LOT').val('');
    //                if (gridInstance) {
    //                    gridInstance.getDataSource().filter(null);
    //                    gridInstance.refresh();
    //                }
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},


    //InitSizeDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/SalesInvoiceReport/GetSizes", function (data) {
    //        if (data.msgType == 1) {
    //            $('#SIZE').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'value',
    //                valueExpr: 'key',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    openVoucherPage(link, tran_Id) {
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
        });
    },
}