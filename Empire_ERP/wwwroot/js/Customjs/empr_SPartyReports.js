var empr_SPartyReports = {
    IsCustomSelection: false,
    RegionGrCode: '',
    BuyerControlData: [],
    RegionData: [],
    PartyData: [],
    ItemGroupData: [],

    InitEvents() {
        empr_SPartyReports.GetReportTypes();
        empr_SPartyReports.InitControlDDL();
        empr_SPartyReports.InitAccountDDL();
        empr_SPartyReports.InitRegionDDL();
        empr_SPartyReports.InitItemMasterDDL();
        empr_SPartyReports.InitItemGroupDDL();
        empr_SPartyReports.InitArivalStatusDDL();
        empr_SPartyReports.InitTypeDDL();

        $('body').on('click', '#BtnGenerate', function () {
            $("#Loader").show();    
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_SPartyReports.ValidateInfo()) {
                    empr_SPartyReports.GenerateReport();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
            }, 200);
        });
        $('body').on('click', '#BtnUpdate', function () {
            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                empr_SPartyReports.UpdateSodeBookFeedingReport();
                setTimeout(function () {
                    $("#Loader").hide();
                }, 500);
            }, 200);
        });
    },

    InitTypeDDL: function (_selectedValue) {
        $('#SBF_TYPE').dxSelectBox({
            dataSource: empr_helper.SBF_TYPE,
            valueExpr: 'value',
            displayExpr: 'text',
            searchEnabled: true,
            value: _selectedValue,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
        });
    },

    InitControlDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/PartyReports/GetControls", function (data) {
            if (data.msgType == 1) {
                empr_SPartyReports.ControlData = data.data;
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
                            if (!empr_SPartyReports.IsCustomSelection) {
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
                            if (!empr_SPartyReports.IsCustomSelection) {
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
                empr_SPartyReports.RegionData = data.data;
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
                            let selectedItem = e.component.getDataSource().items().find(item => item.key === e.value);
                            if (selectedItem) {
                                let grcode = selectedItem.grcode; // Get the grcode value
                                empr_SPartyReports.BRegionGrCode = grcode;
                            }
                            if (!empr_SPartyReports.IsCustomSelection) {
                                //$('#BUYER_NAME').dxSelectBox('instance').option('value', '');
                                //$('#BUYER_CONTROL').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            empr_SPartyReports.BRegionGrCode = '';
                            //$('#BUYER_NAME').dxSelectBox('instance').option('value', '');
                            //$('#BUYER_CONTROL').dxSelectBox('instance').option('value', '');
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
                            let selectedItem = e.component.getDataSource().items().find(item => item.key === e.value);
                            if (selectedItem) {
                                let grcode = selectedItem.grcode; // Get the grcode value
                                empr_SPartyReports.SRegionGrCode = grcode;
                            }
                            if (!empr_SPartyReports.IsCustomSelection) {
                                //$('#SELLER_NAME').dxSelectBox('instance').option('value', '');
                                //$('#SELLER_CONTROL').dxSelectBox('instance').option('value', '');
                            }
                        }
                        else {
                            empr_SPartyReports.SRegionGrCode = '';
                            //$('#SELLER_NAME').dxSelectBox('instance').option('value', '');
                            //$('#SELLER_CONTROL').dxSelectBox('instance').option('value', '');
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
                empr_SPartyReports.PartyData = data.data;
                $('#BUYER_NAME').dxSelectBox({
                    dataSource: {
                        store: data.data,
                        paginate: true,
                        pageSize: 50
                    },
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    },
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
                                var controls = empr_SPartyReports.ControlData;
                                var control = controls.filter(i => i.key == item[0].accountCode);
                                var regions = empr_SPartyReports.RegionData;
                                var region = regions.filter(i => i.key == item[0].regionCode);
                                if (control.length > 0) {
                                    empr_SPartyReports.IsCustomSelection = true;
                                    $('#BUYER_CONTROL').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_SPartyReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#BUYER_CONTROL').dxSelectBox('instance').option('value', null);
                                }

                                if (region.length > 0) {
                                    empr_SPartyReports.IsCustomSelection = true;
                                    $('#BUYER_REGION').dxSelectBox('instance').option('value', region[0].key);
                                    setTimeout(function () {
                                        empr_SPartyReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#BUYER_REGION').dxSelectBox('instance').option('value', null);
                                }
                            }
                        }
                    },
                });
                $('#SELLER_NAME').dxSelectBox({
                    dataSource: {
                        store: data.data,
                        paginate: true,
                        pageSize: 50
                    },
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    },
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
                                var controls = empr_SPartyReports.ControlData;
                                var control = controls.filter(i => i.key == item[0].accountCode);
                                var regions = empr_SPartyReports.RegionData;
                                var region = regions.filter(i => i.key == item[0].regionCode);
                                if (control.length > 0) {
                                    empr_SPartyReports.IsCustomSelection = true;
                                    $('#SELLER_CONTROL').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_SPartyReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#SELLER_CONTROL').dxSelectBox('instance').option('value', null);
                                }

                                if (region.length > 0) {
                                    empr_SPartyReports.IsCustomSelection = true;
                                    $('#SELLER_REGION').dxSelectBox('instance').option('value', region[0].key);
                                    setTimeout(function () {
                                        empr_SPartyReports.IsCustomSelection = false;
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
                empr_SPartyReports.InitReportTypeGrid(data.data);
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
        var parties = empr_SPartyReports.PartyData;
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
        var BUYER_REGION_GR = empr_SPartyReports.BRegionGrCode;
        var SELLER_CONTROL_CODE = $('#SELLER_CONTROL').dxSelectBox('option', 'value');
        var SELLER_REGION_CODE = $('#SELLER_REGION').dxSelectBox('option', 'value');
        var SELLER_REGION_GR = empr_SPartyReports.SRegionGrCode;
        var ITEM = $('#ITEM_MASTER').dxSelectBox('option', 'value');
        var GROUP = $('#ITEM_GROUP').dxSelectBox('option', 'value');
        var INSURANCE = $('#INSURANCE').prop('checked');
        var ARIVAL_STATUS = $('#ARIVAL_STATUS').dxSelectBox('option', 'value');
        var SBF_TYPE = $('#SBF_TYPE').dxSelectBox('option', 'value');
        var BUYER_PARTY_CODE = buyerPartyCode;
        var SELLER_PARTY_CODE = sellerPartyCode;

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
            ITEM: ITEM,
            GROUP: GROUP,
            INSURANCE: INSURANCE,
            SBFTYPE: SBF_TYPE,
            BUYERREGIONGR: BUYER_REGION_GR,
            SELLERREGIONGR: SELLER_REGION_GR,
        }
        console.log(record);
        return record;


    },

    UpdateDataToReport() {

        var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();
        var parties = empr_SPartyReports.PartyData;
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
        var BUYER_REGION_GR = empr_SPartyReports.BRegionGrCode;
        var SELLER_CONTROL_CODE = $('#SELLER_CONTROL').dxSelectBox('option', 'value');
        var SELLER_REGION_CODE = $('#SELLER_REGION').dxSelectBox('option', 'value');
        var SELLER_REGION_GR = empr_SPartyReports.SRegionGrCode;
        var ITEM = $('#ITEM_MASTER').dxSelectBox('option', 'value');
        var GROUP = $('#ITEM_GROUP').dxSelectBox('option', 'value');
        var INSURANCE = $('#INSURANCE').prop('checked');
        var ARIVAL_STATUS = $('#ARIVAL_STATUS').dxSelectBox('option', 'value');
        var SBF_TYPE = $('#SBF_TYPE').dxSelectBox('option', 'value');
        var BUYER_PARTY_CODE = buyerPartyCode;
        var SELLER_PARTY_CODE = sellerPartyCode;

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
            ITEM: ITEM,
            GROUP: GROUP,
            INSURANCE: INSURANCE,
            SBFTYPE: SBF_TYPE,
            BUYERREGIONGR: BUYER_REGION_GR,
            SELLERREGIONGR: SELLER_REGION_GR,
        }

        var detailRecords = [];
        detailRecords = $('#ReportGridContainer').dxDataGrid('instance').getSelectedRowsData();

        console.log(detailRecords);
        var modelRecord = {
            Master: record,
            Detail: detailRecords
        };
        return modelRecord;


    },

    GenerateReport() {
        debugger;
        var dataModel = empr_SPartyReports.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/SPartyReports/GenerateReport", function (data) {
            if (data.msgType == 1) {
                console.log(data);
                empr_SPartyReports.InitReportGrid(data.data, dataModel.REPORTID);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    UpdateSodeBookFeedingReport() {
        debugger;
        var dataModel = empr_SPartyReports.UpdateDataToReport();
        console.log(dataModel);
        ajaxHelper.ajaxPostJsonData(dataModel, "/SPartyReports/UpdateSodeBookFeedingReport", function (data) {
            if (data.msgType == 1) {
                console.log(data);
                empr_SPartyReports.GenerateReport();
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitReportGrid(dataSrc, reportId) {
        console.log(reportId)
        console.log(dataSrc)
        var col = [];
        if (reportId == 15) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', groupIndex: 0 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 200 },
                { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 300 },
                { dataField: 'sellerName', caption: 'Seller A/c Name', width: 300 },
                { dataField: 'remarks', caption: 'Description', width: 300 },
                { dataField: 'itemName', caption: 'Item', width: 250 },
                { dataField: 'qty', caption: 'QTY', width: 80 },
                { dataField: 'unit', caption: 'Unit', width: 60 },
                { dataField: 'arivalStatus', caption: 'Arival Status', width: 90 },
                { dataField: 'arivalStatus', visible: false },
            ];
        }
        else if (reportId == 14) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', groupIndex: 0 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 230,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SPartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }  
              },
                { dataField: 'brokerName', caption: 'Broker A/c Name', width: 250 },
                { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 250 },
                { dataField: 'sellerName', caption: 'Seller A/c Name', width: 250 },
                /*{ dataField: 'remarks', caption: 'Description', width: 200 },*/
                { dataField: 'itemName', caption: 'Item', width: 200 },
                { dataField: 'cond', caption: 'Cond', width: 120 },
                { dataField: 'qty', caption: 'QTY', width: 100 },
                { dataField: 'unit', caption: 'Unit', width: 80 },
                { dataField: 'rate', caption: 'Rate', width: 80 },
                { dataField: 'rT_TYPE', caption: 'RT', width: 100 },
                { dataField: 'amt', caption: 'Amt', width: 130 },
            ];
        }
        else if (reportId == 40 || reportId == 50 || reportId == 68 || reportId == 69) {
            col = [
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 230,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SPartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'voucherDate', caption: 'Date', width: 130 },
                { dataField: 'amt', caption: 'Amount', width: 130, visible: false, summaryType: 'sum' },
                { dataField: 'qty', caption: 'QTY', width: 90, summaryType: 'sum' },
                { dataField: 'unit', caption: 'Unit', width: 70 },
                { dataField: 'itemNameWithAvg', caption: 'Item', width: 300 },
                { dataField: 'rate', caption: 'Rate', width: 70 },
                { dataField: 'rT_TYPE', caption: 'RT Type', width: 90 },
                { dataField: 'cond', caption: 'Cond', width: 120 },
                { dataField: 'sellerName', caption: 'Seller A/c Name', width: 300 },
                { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 300 },
                { dataField: 'brokerName', caption: 'Broker A/c Name', width: 300 },
                { dataField: 'avg', caption: 'AVG', width: 90, visible: false, summaryType: 'sum' },
                //{ dataField: 'remarks', caption: 'Description', width: 300 },
            ];
        }
        else if (reportId == 13) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 150 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 250 },
                { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 300 },
                { dataField: 'sellerName', caption: 'Seller A/c Name', width: 300 },
                { dataField: 'ins', caption: 'Insurance', width: 120 },
                { dataField: 'remarks', caption: 'Description', width: 250 },
                { dataField: 'itemName', caption: 'Item', width: 250 },
                { dataField: 'qty', caption: 'QTY', width: 70 },
                { dataField: 'unit', caption: 'Unit', width: 70 },
                { dataField: 'brokerName', caption: 'Broker A/c Name', width: 300 },
            ];
        }
        else if (reportId == 17) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 120 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 180 },
                { dataField: 'buyerName', caption: 'Party Name', width: 300 },
                { dataField: 'itemName', caption: 'Item', width: 250 },
                { dataField: 'totaL_STOCK_IN', caption: 'Stock In', width: 100 },
                { dataField: 'totaL_STOCK_OUT', caption: 'Stock Out', width: 100 },
                { dataField: 'unit', caption: 'Unit', width: 70 },
            ];
        }
        else if (reportId == 72) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 100, cssClass: 'centered-column', },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200, cssClass: 'centered-column',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SPartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },

                { dataField: 'unit', caption: 'Unit', width: 130, cssClass: 'centered-column', },
                { dataField: 'sellerName', caption: 'Seller', width: 200, cssClass: 'centered-column', },
                { dataField: 'buyerName', caption: 'Buyer', width: 200, cssClass: 'centered-column',},
                { dataField: 'itemName', caption: 'Item', width: 200, cssClass: 'centered-column', },
                { dataField: 'rate', caption: 'Rate', width: 70, cssClass: 'centered-column', },
                { dataField: 'rT_TYPE', caption: 'RT Type', width: 90, cssClass: 'centered-column', },
                {
                    caption: "Condition",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>Condition</strong>");
                    },
                    columns: [
                        { dataField: 'cond', caption: 'Type', width: 80 },
                        { dataField: 'duE_DATE', caption: 'Date', width: 80 },
                        { dataField: 'crediT_DAYS', caption: 'Days', width: 60 }
                    ]
                },
                {
                    caption: "Quantity",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>Quantity</strong>");
                    },
                    columns: [
                        { dataField: 'oqty', caption: 'Soda', dataType: 'number', width: 60 },
                        { dataField: 'iqty', caption: 'Del', dataType: 'number', width: 60 },
                        {
                            dataField: 'bqty',
                            caption: 'Bal',
                            dataType: 'number',
                            width: 60,
                            calculateCellValue: function (data) {
                                return data.bqty < 0
                                    ? `(${new Intl.NumberFormat().format(Math.abs(data.bqty))})`
                                    : new Intl.NumberFormat().format(data.bqty);
                            },
                            cellTemplate: function (container, options) {
                                const isNegative = options.data.bqty < 0;
                                $('<div>')
                                    .text(options.value)
                                    .css({
                                        color: isNegative ? 'red' : 'black',
                                        'text-align': 'right'
                                    })
                                    .appendTo(container);
                            }
                        }
                    ]
                },
                { dataField: 'dT_CODE', caption: 'CODE', width: 70, visible: false, cssClass: 'centered-column', alignment: 'center', },
                
            ];
        }
        else if (reportId == 77 || reportId == 78 || reportId == 79 || reportId == 80 || reportId == 81 || reportId == 82 || reportId == 83 || reportId == 84 || reportId == 123 || reportId == 124) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 100, cssClass: 'centered-column', }, 
                { dataField: 'avg', caption: 'Amount', width: 90, visible: false, summaryType: 'sum', cssClass: 'centered-column' },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 150, cssClass: 'centered-column',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_SPartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'itemNameWithAvg', caption: 'Item', cssClass: 'centered-column', width: 150 },
                { dataField: 'rate', caption: 'Rate', width: 70, cssClass: 'centered-column', },
                { dataField: 'ratE1', caption: 'Unit/Rate', width: 70, cssClass: 'centered-column' },
                { dataField: 'rT_TYPE', caption: 'RT Type', width: 90, cssClass: 'centered-column', },
                {
                    caption: "Condition",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>Condition</strong>");
                    },
                    columns: [
                        { dataField: 'cond', caption: 'Type', width: 80 },
                        { dataField: 'duE_DATE', caption: 'Date', width: 80 },
                        { dataField: 'crediT_DAYS', caption: 'Days', width: 60 }

                    ]
                },
                {
                    caption: "Quantity",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>Quantity</strong>");
                    },
                    columns: [
                        { dataField: 'oqty', caption: 'Order', dataType: 'number', width: 90 },
                        { dataField: 'iqty', caption: 'Issue', dataType: 'number', width: 90 },
                        {
                            dataField: 'bqty',
                            caption: 'Bal',
                            dataType: 'number',
                            width: 90,
                            calculateCellValue: function (data) {
                                return data.bqty;
                            },
                            cellTemplate: function (container, options) {
                                const isNegative = options.value < 0;
                                $('<div>')
                                    .text(
                                        isNegative
                                            ? `(${Math.abs(options.value).toLocaleString('en-US')})`
                                            : options.value.toLocaleString('en-US')
                                    )
                                    .css({
                                        color: isNegative ? 'red' : 'black',
                                        'text-align': 'right'
                                    })
                                    .appendTo(container);
                            }
                        }
                    ]
                },
                { dataField: 'sellerName', caption: 'Seller', width: 220, cssClass: 'centered-column', },
                { dataField: 'buyerName', caption: 'Buyer', width: 220, cssClass: 'centered-column',},
            ];
        }
        else if (reportId == 85 || reportId == 86) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 100, cssClass: 'centered-column', },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 150, cssClass: 'centered-column'
                },
                { dataField: 'itemName', caption: 'Item', cssClass: 'centered-column', },
                { dataField: 'rate', caption: 'Rate', width: 60, cssClass: 'centered-column', },
                {
                    caption: "Quantity",
                    alignment: "center",
                    headerCellTemplate(container) {
                        container.addClass("merged-header").html("<strong>Quantity</strong>");
                    },
                    columns: [
                        { dataField: 'oqty', caption: 'Order', dataType: 'number', width: 60 },
                        { dataField: 'iqty', caption: 'Issue', dataType: 'number', width: 60 },
                    ]
                },
                { dataField: 'sellerName', caption: 'Seller', width: 200, cssClass: 'centered-column', },
                { dataField: 'buyerName', caption: 'Buyer', width: 200, cssClass: 'centered-column',},
            ];
        }
        if (reportId != 72) {
            debugger;
            if (reportId == 17 ) {
                //empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
                empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }
            else {
                //empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName,true);
                empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName,true);
            }
            $('.update').css('display', 'none');
        }
        else {
            empr_helper.editableDxGridbinding('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            $('.update').css('display', '');
        }
        setTimeout(function () {
            if (reportId != 72) {

                var grid = $("#ReportGridContainer").dxDataGrid("instance");
                if (grid) {
                    grid.dispose(); 
                    $("#ReportGridContainer").empty(); 
                }

                if (reportId == 17 ) {
                    //empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
                    empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
                }
                else {
                    //empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName, true);
                    empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName, true);
                }
                $('.update').css('display', 'none');

                
            }
            else {
                empr_helper.editableDxGridbinding('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
                $('.update').css('display', '');
            }
        }, 300);
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
        var data = empr_SPartyReports.GetDataToSave();

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
        ajaxHelper.ajaxGetJson("/SPartyReports/GetItemGroup", function (data) {
            if (data.msgType == 1) {
                empr_SPartyReports.ItemGroupData = data.data;
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
                            if (!empr_SPartyReports.IsCustomSelection) {
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
        ajaxHelper.ajaxGetJson("/SPartyReports/GetItemMaster", function (data) {
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
                                var controls = empr_SPartyReports.ItemGroupData;
                                var control = controls.filter(i => i.key == item[0].groupCode);
                                if (control.length > 0) {
                                    empr_SPartyReports.IsCustomSelection = true;
                                    $('#ITEM_GROUP').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_SPartyReports.IsCustomSelection = false;
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

    openVoucherPage(link, tran_Id) {
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
        });
    },
}