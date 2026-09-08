var empr_PurchaseBillReports = {
    IsCustomSelection: false,
    BuyerControlData: [],
    RegionData: [],
    PartyData: [],
    ItemGroupData: [],

    InitEvents() {
        empr_PurchaseBillReports.GetReportTypes();
        empr_PurchaseBillReports.InitControlDDL();
        empr_PurchaseBillReports.InitAccountDDL();
        empr_PurchaseBillReports.InitRegionDDL();
        empr_PurchaseBillReports.InitItemMasterDDL();
        empr_PurchaseBillReports.InitItemGroupDDL();
        empr_PurchaseBillReports.InitArivalStatusDDL();

        $('body').on('click', '#BtnGenerate', function () {
            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_PurchaseBillReports.ValidateInfo()) {
                    empr_PurchaseBillReports.GenerateReport();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
            }, 200);
        });
    },

    InitControlDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/PartyReports/GetControls", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBillReports.ControlData = data.data;
                $('#BUYER_CONTROL').dxSelectBox({
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
                            if (!empr_PurchaseBillReports.IsCustomSelection) {
                                $('#BUYER_NAME').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            $('#BUYER_NAME').dxSelectBox('instance').option('value', '');
                        }
                    },
                });
                $('#SELLER_CONTROL').dxSelectBox({
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
                            if (!empr_PurchaseBillReports.IsCustomSelection) {
                                $('#SELLER_NAME').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            $('#SELLER_NAME').dxSelectBox('instance').option('value', '');
                        }
                    },
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitRegionDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/PartyReports/GetRegions", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBillReports.RegionData = data.data;
                $('#BUYER_REGION').dxSelectBox({
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
                            if (!empr_PurchaseBillReports.IsCustomSelection) {
                                $('#BUYER_NAME').dxSelectBox('instance').option('value', '');
                                $('#BUYER_CONTROL').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            $('#BUYER_NAME').dxSelectBox('instance').option('value', '');
                            $('#BUYER_CONTROL').dxSelectBox('instance').option('value', '');
                        }
                    },
                });
                $('#SELLER_REGION').dxSelectBox({
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
                            if (!empr_PurchaseBillReports.IsCustomSelection) {
                                $('#SELLER_NAME').dxSelectBox('instance').option('value', '');
                                $('#SELLER_CONTROL').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            $('#SELLER_NAME').dxSelectBox('instance').option('value', '');
                            $('#SELLER_CONTROL').dxSelectBox('instance').option('value', '');
                        }
                    },
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitAccountDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/PartyReports/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBillReports.PartyData = data.data;
                $('#BUYER_NAME').dxSelectBox({
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
                            var items = e.component._dataSource._items;
                            var item = items.filter(i => i.key == e.value);
                            if (item.length > 0) {
                                var controls = empr_PurchaseBillReports.ControlData;
                                var control = controls.filter(i => i.key == item[0].accountCode);
                                var regions = empr_PurchaseBillReports.RegionData;
                                var region = regions.filter(i => i.key == item[0].regionCode);
                                if (control.length > 0) {
                                    empr_PurchaseBillReports.IsCustomSelection = true;
                                    $('#BUYER_CONTROL').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_PurchaseBillReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#BUYER_CONTROL').dxSelectBox('instance').option('value', null);
                                }

                                if (region.length > 0) {
                                    empr_PurchaseBillReports.IsCustomSelection = true;
                                    $('#BUYER_REGION').dxSelectBox('instance').option('value', region[0].key);
                                    setTimeout(function () {
                                        empr_PurchaseBillReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#BUYER_REGION').dxSelectBox('instance').option('value', null);
                                }
                            }
                        }
                    },
                });
                $('#SELLER_NAME').dxSelectBox({
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
                            var items = e.component._dataSource._items;
                            var item = items.filter(i => i.key == e.value);
                            if (item.length > 0) {
                                var controls = empr_PurchaseBillReports.ControlData;
                                var control = controls.filter(i => i.key == item[0].accountCode);
                                var regions = empr_PurchaseBillReports.RegionData;
                                var region = regions.filter(i => i.key == item[0].regionCode);
                                if (control.length > 0) {
                                    empr_PurchaseBillReports.IsCustomSelection = true;
                                    $('#SELLER_CONTROL').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_PurchaseBillReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#SELLER_CONTROL').dxSelectBox('instance').option('value', null);
                                }

                                if (region.length > 0) {
                                    empr_PurchaseBillReports.IsCustomSelection = true;
                                    $('#SELLER_REGION').dxSelectBox('instance').option('value', region[0].key);
                                    setTimeout(function () {
                                        empr_PurchaseBillReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#SELLER_REGION').dxSelectBox('instance').option('value', null);
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

    GetReportTypes() {
        ajaxHelper.ajaxGetJson("/PartyReports/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBillReports.InitReportTypeGrid(data.data);
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
        var parties = empr_PurchaseBillReports.PartyData;
        var reportID = '0', buyerPartyCode = '', sellerPartyCode = '';

        if (selectedRowKey.length > 0) {
            reportID = selectedRowKey[0].r_ID;
            empr_helper.reportName = selectedRowKey[0].reporT_NAME;
        }

        if (parties.length > 0) {
            var buyerParty = parties.filter(i => i.key == $('#BUYER_NAME').dxSelectBox('option', 'value'));
            if (buyerParty.length > 0) {
                buyerPartyCode = buyerParty[0].partyCode;
            }
        }

        if (parties.length > 0) {
            var sellerParty = parties.filter(i => i.key == $('#SELLER_NAME').dxSelectBox('option', 'value'));
            if (sellerParty.length > 0) {
                sellerPartyCode = sellerParty[0].partyCode;
            }
        }

        var FROM_DATE = $("#FROM_DATE").val();
        var HIDDEN_FROM_DATE = $("#HIDDEN_FROM_DATE").val();
        var TO_DATE = $("#TO_DATE").val();
        var HIDDEN_TO_DATE = $("#HIDDEN_TO_DATE").val();
        var REPORT_ID = reportID;
        var BUYER_CONTROL_CODE = $('#BUYER_CONTROL').dxSelectBox('option', 'value');
        var BUYER_REGION_CODE = $('#BUYER_REGION').dxSelectBox('option', 'value');
        var SELLER_CONTROL_CODE = $('#SELLER_CONTROL').dxSelectBox('option', 'value');
        var SELLER_REGION_CODE = $('#SELLER_REGION').dxSelectBox('option', 'value');
        var ITEM = $('#ITEM_MASTER').dxSelectBox('option', 'value');
        var GROUP = $('#ITEM_GROUP').dxSelectBox('option', 'value');
        var ARIVAL_STATUS = $('#ARIVAL_STATUS').dxSelectBox('option', 'value');
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var BUYER_PARTY_CODE = buyerPartyCode;
        var SELLER_PARTY_CODE = sellerPartyCode;
        debugger;
        BUYER_CONTROL_CODE === '0' ? "" : BUYER_CONTROL_CODE;
        ITEM === '0' ? "" : ITEM;
        BUYER_REGION_CODE === '0' ? "" : BUYER_REGION_CODE;

        var record = {
            FROMDATE: FROM_DATE,
            TODATE: TO_DATE,
            HIDDENFROMDATE: HIDDEN_FROM_DATE,
            HIDDENTODATE: HIDDEN_TO_DATE,
            REPORTID: REPORT_ID,
            BUYERCONTROLCODE: BUYER_CONTROL_CODE,
            SELLERCONTROLCODE: SELLER_CONTROL_CODE,
            BUYERREGIONCODE: BUYER_REGION_CODE,
            SELLERREGIONCODE: SELLER_REGION_CODE,
            BUYERPARTYCODE: BUYER_PARTY_CODE,
            SELLERPARTYCODE: SELLER_PARTY_CODE,
            ARIVALSTATUS: ARIVAL_STATUS,
            ASTATUS: ASTATUS,
            ITEM: ITEM,
            GROUP: GROUP,
        }
        return record;


    },

    GenerateReport() {
        debugger;
        var dataModel = empr_PurchaseBillReports.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseBillReports/GenerateReport", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBillReports.InitReportGrid(data.data, dataModel.REPORTID);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitReportGrid(dataSrc, reportId) {
        var col = [];
        debugger;
        if (reportId == 16) {
            col = [
                { dataField: 'voucherDate', caption: 'Date' ,groupIndex: 0 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_PurchaseBillReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.mtraN_ID) + ')')
                            .appendTo(container);
                    }
                },
                //{ dataField: 'voucherNo', caption: 'Transaction #', width: 200 },
                { dataField: 'ref', caption: 'Ref', width: 70 },
                { dataField: 'buyerName', caption: 'Party A/c Name', width: 300 },
                { dataField: 'remarks', caption: 'Description', width: 400 },
                { dataField: 'itemName', caption: 'Item', width: 250 },
            ];
            if (ConQty == 2) {
                col.push(
                    { dataField: 'qty', caption: 'Qty', width: 70 },
                    { dataField: 'unit', caption: 'Unit', width: 70 },
                    { dataField: 'qtY2', caption: 'Qty2', width: 70 },
                    { dataField: 'baL_QTY', caption: 'Balance Qty', width: 70 },
                );
            } else {
                col.push(
                    { dataField: 'qty', caption: 'Qty', width: 70 },
                    { dataField: 'unit', caption: 'Unit', width: 70 },
                );
            }
            col.push(
                { dataField: 'rate', caption: 'Rate', width: 70 },
                { dataField: 'amt', caption: 'Amt', width: 100 },
                { dataField: 'disc', caption: 'Disc', width: 70 },
                { dataField: 'disC_AMT', caption: 'Dis Amt', width: 100 },
                { dataField: 'tax', caption: 'Tax', width: 70 },
                { dataField: 'taX_AMT', caption: 'Tax Amt', width: 100 },
                { dataField: 'neT_AMT', caption: 'Net Amt', width: 120 }
            );
        }
        else if (reportId == 104) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width : 150 },
                //{ dataField: 'voucherNo', caption: 'Transaction #', width: 200 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_PurchaseBillReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.mtraN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'ref', caption: 'Ref', width: 70 },
                { dataField: 'buyerName', caption: 'Party A/c Name', width: 300, groupIndex: 0 },
                { dataField: 'remarks', caption: 'Description', width: 400 },
                { dataField: 'itemName', caption: 'Item', width: 250 },
            ];

            if (ConQty == 2) {
                col.push(
                    { dataField: 'qty', caption: 'Qty', width: 70 },
                    { dataField: 'unit', caption: 'Unit', width: 70 },
                    { dataField: 'qtY2', caption: 'Qty2', width: 70 },
                    { dataField: 'baL_QTY', caption: 'Balance Qty', width: 70 },
                );
            } else {
                col.push(
                    { dataField: 'qty', caption: 'Qty', width: 70 },
                    { dataField: 'unit', caption: 'Unit', width: 70 },
                );
            }

            col.push(
                { dataField: 'rate', caption: 'Rate', width: 70 },
                { dataField: 'amt', caption: 'Amt', width: 100 },
                { dataField: 'disc', caption: 'Disc', width: 70 },
                { dataField: 'disC_AMT', caption: 'Dis Amt', width: 100 },
                { dataField: 'tax', caption: 'Tax', width: 70 },
                { dataField: 'taX_AMT', caption: 'Tax Amt', width: 100 },
                { dataField: 'neT_AMT', caption: 'Net Amt', width: 120 }
            );
        }
        else if (reportId == 105) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 150 },
                //{ dataField: 'voucherNo', caption: 'Transaction #', width: 220 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 220,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_PurchaseBillReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.mtraN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'ref', caption: 'Ref', width: 70 },
                { dataField: 'buyerName', caption: 'Party A/c Name', width: 300, },
                { dataField: 'remarks', caption: 'Description', width: 400 },
                { dataField: 'itemName', caption: 'Item', width: 250, groupIndex: 0 },
            ];

            if (ConQty == 2) {
                col.push(
                    { dataField: 'qty', caption: 'Qty', width: 70 },
                    { dataField: 'unit', caption: 'Unit', width: 70 },
                    { dataField: 'qtY2', caption: 'Qty2', width: 70 },
                    { dataField: 'baL_QTY', caption: 'Balance Qty', width: 70 },
                );
            } else {
                col.push(
                    { dataField: 'qty', caption: 'Qty', width: 70 },
                    { dataField: 'unit', caption: 'Unit', width: 70 },
                );
            }

            col.push(
                { dataField: 'rate', caption: 'Rate', width: 70 },
                { dataField: 'amt', caption: 'Amt', width: 100 },
                { dataField: 'disc', caption: 'Disc', width: 70 },
                { dataField: 'disC_AMT', caption: 'Dis Amt', width: 100 },
                { dataField: 'tax', caption: 'Tax', width: 70 },
                { dataField: 'taX_AMT', caption: 'Tax Amt', width: 100 },
                { dataField: 'neT_AMT', caption: 'Net Amt', width: 120 }
            );
        }

        //empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName , true);
        empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName , true);
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

        var valid = true;
        var data = empr_PurchaseBillReports.GetDataToSave();

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
        if (data.REPORTID < 1) {
            empr_helper.notify("Please select report type.", 2);
            valid = false;
        }
        $("#Loader").hide();
        return valid;
    },

    InitItemGroupDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/PurchaseBillReports/GetItemGroup", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseBillReports.ItemGroupData = data.data;
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
                            if (!empr_PurchaseBillReports.IsCustomSelection) {
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

    InitItemMasterDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/PurchaseBillReports/GetItemMaster", function (data) {
            if (data.msgType == 1) {
                $('#ITEM_MASTER').dxSelectBox({
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
                            var items = e.component._dataSource._items;
                            var item = items.filter(i => i.key == e.value);
                            if (item.length > 0) {
                                var controls = empr_PurchaseBillReports.ItemGroupData;
                                var control = controls.filter(i => i.key == item[0].groupCode);
                                if (control.length > 0) {
                                    empr_PurchaseBillReports.IsCustomSelection = true;
                                    $('#ITEM_GROUP').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_PurchaseBillReports.IsCustomSelection = false;
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

    InitArivalStatusDDL(selectedValue) {

        var dataSource = [
            { key: 'P', value: 'Port' },
            { key: 'G', value: 'Godown' },
        ];

        $('#ARIVAL_STATUS').dxSelectBox({
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
        });
    },
    openVoucherPage(link, mtraN_ID) {
        debugger
        console.log(link)
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: mtraN_ID }, '*');
            }, 1000);
        });
    },
}