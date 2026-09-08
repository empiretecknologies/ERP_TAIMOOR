var empr_PartyReports = {
    debitAverage: 0,
    creditAverage: 0,
    selectedRportId: 0,
    IsCustomSelection: false,
    ControlData: [],
    RegionData: [],
    ItemGroupData: [],
    parties: [],
    NatureData: [],
    reportDataSrc: [],
    InitEvents: function () {
        console.log('BranchWithPeriod', BranchWithPeriod);
        empr_PartyReports.GetReportTypes();
        empr_PartyReports.InitControlDDL();
        empr_PartyReports.InitAccountDDL();
        empr_PartyReports.InitCategoryDDL();
        //empr_PartyReports.InitRegionDDL();
        empr_PartyReports.InitNatureDDL('', false);
        empr_PartyReports.BranchWithPeriod("");

        $('body').on('click', '#BtnGenerate, #BtnGenerateReport', function () {
            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_PartyReports.ValidateInfo()) {
                    empr_PartyReports.GenerateReport();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
            }, 200);
        });

        $(document).on('change', 'input[name="reportRadio"]', function () {
            var report = empr_PartyReports.GetDataToSave();

            empr_PartyReports.selectedRportId = report.REPORTID;
            console.log(report, "data ids")
            debugger
            if (report.REPORTID == 136 || report.REPORTID == 52) {
                var customerCode = 4;
                console.log("nature DAta", empr_PartyReports.NatureData);
                empr_PartyReports.InitNatureDDL(customerCode, true);
                empr_PartyReports.InitAccountDDL('');
                empr_PartyReports.InitControlDDL('');

            } else if (report.REPORTID == 143 || report.REPORTID == 51) {
                var vendorCode = 3;
                empr_PartyReports.InitNatureDDL(vendorCode, true);
                empr_PartyReports.InitAccountDDL('');
                empr_PartyReports.InitControlDDL('');


            }
            else {
                empr_PartyReports.InitNatureDDL('', false);
                empr_PartyReports.InitAccountDDL('');
                empr_PartyReports.InitControlDDL('');

            }

        });
    },

    InitControlDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/PartyReports/GetControls", function (data) {
            if (data.msgType == 1) {
                empr_PartyReports.ControlData = data.data;
                $('#CONTROL_NAME').dxSelectBox({
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
                            if (!empr_PartyReports.IsCustomSelection) {
                                $('#PARTY_NAME').dxSelectBox('instance').option('value', '');
                            }

                        }
                        else {
                            $('#PARTY_NAME').dxSelectBox('instance').option('value', '');
                        }
                    },
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    //InitRegionDDL: function (selectedValue) {
    //    ajaxHelper.ajaxGetJson("/PartyReports/GetRegions", function (data) {
    //        if (data.msgType == 1) {
    //            empr_PartyReports.RegionData = data.data;
    //            $('#REGION_NAME').dxSelectBox({
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
    //                        if (!empr_PartyReports.IsCustomSelection) {
    //                            //$('#PARTY_NAME').dxSelectBox('instance').option('value', '');
    //                            //$('#CONTROL_NAME').dxSelectBox('instance').option('value', '');
    //                        }
    //                    }
    //                    else {
    //                        //$('#PARTY_NAME').dxSelectBox('instance').option('value', '');
    //                        //$('#CONTROL_NAME').dxSelectBox('instance').option('value', '');
    //                    }
    //                },
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //    }, false, true);
    //},

    InitNatureDDL: function (selectedValue, isDisabled) {
        ajaxHelper.ajaxGetJson("/AccountNature/GetAccountNature", function (data) {
            if (data.msgType == 1) {
                empr_PartyReports.NatureData = data.data;
                $('#NATURE').dxSelectBox({
                    dataSource: data.data,
                    displayExpr: 'grouP_NAME',
                    valueExpr: 'grouP_CODE',
                    value: selectedValue,
                    searchEnabled: true,
                    disabled: isDisabled ? true : false,
                    width: '100%',
                    placeholder: 'Search',
                    showClearButton: true,
                    dropDownOptions: {
                        height: 'auto',
                    },
                    pagingEnabled: true,
                    searchTimeout: 500,
                    //onValueChanged: function (e) {
                    //    if (e.value != '' && e.value != null) {
                    //        if (!empr_PartyReports.IsCustomSelection) {
                    //            $('#PARTY_NAME').dxSelectBox('instance').option('value', '');
                    //            $('#CONTROL_NAME').dxSelectBox('instance').option('value', '');
                    //        }
                    //    }
                    //    else {
                    //        $('#PARTY_NAME').dxSelectBox('instance').option('value', '');
                    //        $('#CONTROL_NAME').dxSelectBox('instance').option('value', '');
                    //    }
                    //},
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    BranchWithPeriod: function (selectedValue) {
        $('#BRANCH').dxSelectBox({
            dataSource: BranchWithPeriod,
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

    InitAccountDDL: function (selectedValue) {
        debugger;
        //ajaxHelper.ajaxGetJson("/PartyReports/GetParties", function (data) {
        //    if (data.msgType == 1) {
        //        debugger;

                empr_PartyReports.parties = Parties;
                $('#PARTY_NAME').dxSelectBox({
                    dataSource: {
                        //store: data.data,
                        store: Parties,
                        paginate: true,
                        pageSize: 50
                    },
                    //paging: {
                    //    enabled: true,
                    //    pageSize: 50,
                    //},
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
                        debugger;
                        if (e.value != '' && e.value != null) {
                            var currentReportID = empr_PartyReports.selectedRportId;
                            var targetParty = empr_PartyReports.parties.filter(i => i.key == e.value);
                            if (targetParty.length > 0) {
                                var partyNature = targetParty[0].natureCode;
                                if ((currentReportID == 136 || currentReportID == 52) && partyNature == '3') {
                                    empr_helper.notify("You cannot select a Vendor party for this Customer Report!", 2);
                                    //e.component.option('value', e.previousValue);
                                    setTimeout(function () {
                                        var selectBoxInstance = $('#PARTY_NAME').dxSelectBox('instance');
                                        if (selectBoxInstance) {
                                            selectBoxInstance.option('value', e.previousValue);
                                        }
                                    }, 50);
                                    return;
                                } else if ((currentReportID == 143 || currentReportID == 51) && partyNature == '4') {
                                    empr_helper.notify("You cannot select a Customer party for this Vendor Report!", 2);
                                    //e.component.option('value', e.previousValue);
                                    setTimeout(function () {
                                        var selectBoxInstance = $('#PARTY_NAME').dxSelectBox('instance');
                                        if (selectBoxInstance) {
                                            selectBoxInstance.option('value', e.previousValue);
                                        }
                                    }, 50);
                                    return;
                                }
                            }
                        }
                        if (e.value != '' && e.value != null) {
                            var items = empr_PartyReports.parties;
                            var item = items.filter(i => i.key == e.value);
                            if (item.length > 0) {
                                debugger
                                var natures = empr_PartyReports.NatureData;
                                var nature = natures.filter(i => i.grouP_CODE == item[0].natureCode);
                                var controls = empr_PartyReports.ControlData;
                                var control = controls.filter(i => i.key == item[0].accountCode);
                                var regions = empr_PartyReports.RegionData;
                                var region = regions.filter(i => i.key == item[0].regionCode);
                                if (control.length > 0) {
                                    empr_PartyReports.IsCustomSelection = true;
                                    $('#CONTROL_NAME').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_PartyReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    $('#CONTROL_NAME').dxSelectBox('instance').option('value', null);
                                }

                                //if (region.length > 0) {
                                //    empr_PartyReports.IsCustomSelection = true;
                                //    $('#REGION_NAME').dxSelectBox('instance').option('value', region[0].key);
                                //    setTimeout(function () {
                                //        empr_PartyReports.IsCustomSelection = false;
                                //    }, 500);
                                //} else {
                                //    $('#REGION_NAME').dxSelectBox('instance').option('value', null);
                                //}

                                if (nature.length > 0) {
                                    empr_PartyReports.IsCustomSelection = true;
                                    $('#NATURE').dxSelectBox('instance').option('value', nature[0].grouP_CODE);
                                    setTimeout(function () {
                                        empr_PartyReports.IsCustomSelection = false;
                                    }, 500);
                                } else {
                                    //$('#REGION_NAME').dxSelectBox('instance').option('value', null);
                                }
                            }
                        }
                    },
                    //onSelectionChanged: function (e) {
                    //    if (e.selectedItem) {
                    //        var currentReportID = empr_PartyReports.selectedRportId;
                    //        var partyNature = e.selectedItem.natureCode; 
                    //        if ((currentReportID == 136 || currentReportID == 52) && partyNature == '3') {
                    //            empr_helper.notify("You cannot select a Vendor party for this Customer Report!", 2);
                    //            //setTimeout(function () {
                    //            //    var selectBoxInstance = $('#PARTY_NAME').dxSelectBox('instance');
                    //            //    if (selectBoxInstance) {
                    //            //        selectBoxInstance.option('value', e.component.option('value') === e.previousValue ? null : e.previousValue);
                    //            //    }
                    //            //}, 20);
                    //            return;
                    //        }
                    //        else if ((currentReportID == 143 || currentReportID == 51) && partyNature == '4') {
                    //            empr_helper.notify("You cannot select a Customer party for this Vendor Report!", 2);
                    //            console.log("Validation Failed: Customer selected for Vendor Report ID " + currentReportID);
                    //            setTimeout(function () {
                    //                var selectBoxInstance = $('#PARTY_NAME').dxSelectBox('instance');
                    //                if (selectBoxInstance) {
                    //                    selectBoxInstance.option('value', e.component.option('value') === e.previousValue ? null : e.previousValue);
                    //                }
                    //            }, 20);
                    //            return;
                    //        }
                    //    }
                    //}
                });
            //}
        //    else {
        //        empr_helper.notify(data.data, data.msgType);
        //    }
        //}, false, true);
    },

    GetReportTypes: function () {
        debugger;
        ajaxHelper.ajaxGetJson("/PartyReports/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                empr_PartyReports.InitReportTypeGrid(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    //InitReportTypeGrid: function (dataSrc) {

    //    var col = [
    //        { dataField: 'sno', caption: 'Code', width: '70px' },
    //        { dataField: 'reporT_NAME', caption: 'Name' },
    //    ];
    //    empr_helper.dxGridbindingForReports('#GridContainer', col, dataSrc, "ReportTypesList", 'single');
    //    setTimeout(function () {
    //        $('#GridContainer').dxDataGrid('instance').selectRowsByIndexes([0]);
    //    }, 500);
    //},

    InitReportTypeGrid: function (dataSrc) {
        console.log('dataSrc', dataSrc);
        reportDataSrc = dataSrc;
        var html = '';

        $.each(dataSrc, function (index, item) {
            html += `
        <div>
            <label>
                ${item.sno} — 
                <input type="radio" name="reportRadio" data-index="${index}">
                <span style="font-size:12px;">${item.reporT_NAME}</span>
            </label>
        </div>
        `;
        });

        $('#GridContainer').html(html);
    },

    GetDataToSave: function () {

        //var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();
        var selectedIndex = $('input[name="reportRadio"]:checked').data('index');
        var selectedRowKey = [];
        var partyCode = '';
        var periodId = '';
        if (selectedIndex !== undefined) {
            selectedRowKey.push(reportDataSrc[selectedIndex]);
        }
        var reportID = '0';
        debugger;
        if (selectedRowKey.length > 0) {
            reportID = selectedRowKey[0].r_ID;
            console.log('reportID', reportID);
            empr_helper.reportName = selectedRowKey[0].reporT_NAME;
        }


        //var reportID = '0', partyCode = '';
        //if (selectedRowKey.length > 0) {
        //    reportID = selectedRowKey[0].r_ID;
        //    empr_helper.reportName = selectedRowKey[0].reporT_NAME;
        //}
        var parties = empr_PartyReports.parties;
        if (parties.length > 0) {
            var party = parties.filter(i => i.key == $('#PARTY_NAME').dxSelectBox('option', 'value'));
            if (party.length > 0) {
                partyCode = party[0].partyCode;
            }
        }

        if (BranchWithPeriod.length > 0) {
            var selectedBranch = BranchWithPeriod.filter(i => i.key == $('#BRANCH').dxSelectBox('option', 'value'));
            if (selectedBranch.length > 0) {
                periodId = selectedBranch[0].pid;
            }
        }

        var FROM_DATE = $("#FROM_DATE").val();
        var PASS = $("#PASS").val();
        var HIDDEN_FROM_DATE = $("#HIDDEN_FROM_DATE").val();
        var TO_DATE = $("#TO_DATE").val();
        var HIDDEN_TO_DATE = $("#HIDDEN_TO_DATE").val();
        var REPORT_ID = reportID;
        var CONTROL_CODE = $('#CONTROL_NAME').dxSelectBox('option', 'value');
        var CAT_CODE = $('#CATDDL').dxSelectBox('option', 'value');
        var REGION_CODE = 0;
        var NATURE = $('#NATURE').dxSelectBox('option', 'value');
        var BRANCH = $('#BRANCH').dxSelectBox('option', 'value');
        var PARTY_CODE = partyCode;

        if (BranchWithPeriod.length > 0) {
            var selectedBranch = BranchWithPeriod.filter(i => i.key == BRANCH);
            if (selectedBranch.length > 0) {
                periodId = selectedBranch[0].pid;
            }
        }

        var record = {
            PASS: PASS,
            FROMDATE: FROM_DATE,
            TODATE: TO_DATE,
            HIDDENFROMDATE: HIDDEN_FROM_DATE,
            HIDDENTODATE: HIDDEN_TO_DATE,
            REPORTID: REPORT_ID,
            CONTROLCODE: CONTROL_CODE,
            REGIONCODE: REGION_CODE,
            PARTYCODE: PARTY_CODE,
            BUYERCONTROLCODE: CONTROL_CODE,
            BUYERREGIONCODE: REGION_CODE,
            BUYERPARTYCODE: PARTY_CODE,
            NATURE: NATURE,
            BRANCH: BRANCH,
            PERIOD: periodId,
            CAT_CODE: CAT_CODE
        }
        return record;
    },

    GenerateReport: function () {
        debugger;
        var dataModel = empr_PartyReports.GetDataToSave();
        if ((dataModel.REPORTID == 38 || dataModel.REPORTID == 39) && (dataModel.PASS == null || dataModel.PASS == "")) {
            var controls = empr_PartyReports.ControlData;
            var pass = controls.filter(i => i.key == dataModel.CONTROLCODE)[0].pass;
            if (pass != null && pass != "") {
                $('.bs-example-modal-md').modal('show');
            } else {
                empr_PartyReports.GetReport(dataModel);
            }
        }
        else {
            empr_PartyReports.GetReport(dataModel);
        }

    },

    GetReport: function (dataModel) {
        //debugger;
        var agingReportId = 51;
        ajaxHelper.ajaxPostJsonData(dataModel, "/PartyReports/GenerateReport", function (data) {
            if (data.msgType == 1) {
                empr_PartyReports.InitReportGrid(data.data, dataModel.REPORTID);
                console.log(dataModel.REPORTID)
                if (dataModel.REPORTID == 10) {

                    $('.tradeTab').show();
                    $('.agingTab').show();

                    // Trade report
                    let tradeModel = { ...dataModel, REPORTID: 17 };
                    ajaxHelper.ajaxPostJsonData(tradeModel, "/SPartyReports/GenerateReport", function (tradeData) {
                        if (tradeData.msgType == 1) {

                            empr_PartyReports.InitTradeReportGrid(tradeData.data, 17);
                        } else {
                            empr_helper.notify(data.msg, data.msgType);
                        }
                    }, false, true);

                    // Aging report (separate call)
                    let agingModel = { ...dataModel, REPORTID: 51 };
                    ajaxHelper.ajaxPostJsonData(agingModel, "/PartyReports/GenerateReport", function (agingData) {
                        if (agingData.msgType == 1) {
                            empr_PartyReports.InitAgingReportGrid(agingData.data, 51);
                        } else {
                            empr_helper.notify(data.msg, data.msgType);
                        }
                    }, false, true);
                }


                else if (dataModel.REPORTID == 38) {
                    $('.tradeTab').show();
                    $('.agingTab').hide();

                    dataModel.REPORTID = 17;
                    ajaxHelper.ajaxPostJsonData(dataModel, "/SPartyReports/GenerateReport", function (data) {
                        if (data.msgType == 1) {
                            empr_PartyReports.InitTradeReportGrid(data.data, dataModel.REPORTID);
                        }
                        else {
                            empr_helper.notify(data.msg, data.msgType);
                        }
                    }, false, true);
                    $('.bs-example-modal-md').modal('hide');
                    $('#PASS').val("");
                }
                else if (dataModel.REPORTID == 39) {
                    $('.tradeTab').hide();
                    $('.agingTab').hide();

                    $('#ViewTrade').hide();
                    $('#ViewReport').show();
                    $('.bs-example-modal-md').modal('hide');
                    $('#PASS').val("");
                }
                else {
                    $('.tradeTab').hide();
                    $('.agingTab').hide();

                    $('#ViewTrade').hide();
                    $('#ViewReport').show();
                }
            }
            else {
                $('#PASS').val("");
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    //GetReport: function (dataModel) {
    //    debugger;
    //    var agingReportId = 51;

    //    ajaxHelper.ajaxPostJsonData(dataModel, "/PartyReports/GenerateReport", function (data) {

    //        if (data.msgType == 1) {

    //            empr_PartyReports.InitReportGrid(data.data, dataModel.REPORTID);
    //            console.log(dataModel.REPORTID)

    //            if (dataModel.REPORTID == 10) {

    //                $('.tradeTab').show();
    //                $('.agingTab').show();


    //                let tradeModel = { ...dataModel };
    //                tradeModel.REPORTID = 17;

    //                ajaxHelper.ajaxPostJsonData(tradeModel, "/SPartyReports/GenerateReport", function (tradeResponse) {
    //                    if (tradeResponse.msgType == 1) {
    //                        empr_PartyReports.InitTradeReportGrid(tradeResponse.data, 17);
    //                    }
    //                }, false, true);


    //                let agingModel = { ...dataModel };
    //                agingModel.REPORTID = agingReportId;

    //                ajaxHelper.ajaxPostJsonData(agingModel, "/SPartyReports/GenerateReport", function (agingResponse) {
    //                    if (agingResponse.msgType == 1) {
    //                        empr_PartyReports.InitAgingReportGrid(agingResponse.data, 51);
    //                    }
    //                }, false, true);

    //            }

    //        } else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }

    //    }, false, true);
    //},

    InitReportGrid: function (dataSrc, reportId) {
        debugger;
        console.log(dataSrc);
        var col = [];
        var isShortReport = $('#SHORTLEDGER').prop('checked');
        empr_helper.lastAmt = 0;
        empr_helper.lastDate = '';
        console.log(dataSrc)
        if (reportId == 10 || reportId == 38 || reportId == 70) {
            let totalDebit = 0;
            let totalCredit = 0;
            dataSrc.forEach(item => {
                totalDebit += item.debit || 0;
                totalCredit += item.credit || 0;
            });
            empr_helper.balanceAmount = totalDebit - totalCredit;
            col = [
                { dataField: 'bName', caption: 'Branch'},
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                //{ dataField: 'voucherNo', caption: 'Transaction #', width: 200 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    visible: !isShortReport,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            //.attr('onclick', 'empr_PartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .attr('onclick', 'empr_PartyReports.openVoucherPageWithBranch('
                                + JSON.stringify(options.data.link) + ', '
                                + JSON.stringify(options.data.traN_ID) + ', '
                                + JSON.stringify(options.data.bcode) + ')')
                            .appendTo(container);
                            //.appendTo(container);


                    }
                },
                { dataField: 'chqNo', caption: 'Chq No', width: 100 },
                { dataField: 'chqDate', caption: 'Chq Date', dataType: 'date', format: 'dd/MM/yyyy', width: 100 },
                {
                    dataField: 'accountName',
                    caption: 'A/c Name',
                    groupIndex: 1,
                },
                {
                    dataField: 'accountDescription',
                    caption: 'Description',
                    visible: !isShortReport,
                    cellTemplate: function (container, options) {
                        $('<div>')
                            .text(options.value)
                            .css({
                                'white-space': 'normal',
                                'word-wrap': 'break-word',
                                'overflow-wrap': 'break-word'
                            })
                            .appendTo(container);
                    }
                },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 120 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 120 },
                {
                    dataField: 'balance2',
                    caption: 'Balance ',
                    dataType: 'number',
                    width: 120,
                    calculateCellValue: function (data) {
                        return data.balance < 0 ? `(${new Intl.NumberFormat().format(Math.abs(data.balance))})` : new Intl.NumberFormat().format(data.balance);
                    },
                    cellTemplate: function (container, options) {
                        const isNegative = options.data.balance < 0;
                        $('<div>')
                            .text(options.value)
                            .css({
                                color: isNegative ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                },
            ];
        }
        if (reportId == 58) {
            let previousBalance = null;
            let totalDebit = 0;
            let totalCredit = 0;
            dataSrc.forEach(item => {
                totalDebit += item.debit || 0;
                totalCredit += item.credit || 0;
            });
            empr_helper.balanceAmount = totalDebit - totalCredit;
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    visible: !isShortReport,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_PartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                {
                    dataField: 'accountName',
                    caption: 'A/c Name',
                    groupIndex: 0,
                },
                {
                    dataField: 'accountDescription',
                    caption: 'Description',
                    visible: !isShortReport,
                    cellTemplate: function (container, options) {
                        $('<div>')
                            .text(options.value)
                            .css({
                                'white-space': 'normal',
                                'word-wrap': 'break-word',
                                'overflow-wrap': 'break-word'
                            })
                            .appendTo(container);
                    }
                },
                { dataField: 'qty', caption: 'Quantity', dataType: 'number', width: 100 },
                { dataField: 'rate', caption: 'Rate', dataType: 'number', width: 100 },
                { dataField: 'disc', caption: 'Discount', dataType: 'number', width: 100 },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 120 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 120 },
                {
                    caption: 'Balance',
                    dataType: 'number',
                    width: 120,
                    calculateCellValue: function (data) {
                        if (data.balance === previousBalance) {
                            return "";
                        }
                        previousBalance = data.balance;
                        return data.balance < 0 ? `(${new Intl.NumberFormat().format(Math.abs(data.balance))})` : new Intl.NumberFormat().format(data.balance);
                    },
                    cellTemplate: function (container, options) {
                        const isNegative = options.data.balance < 0;
                        $('<div>')
                            .text(options.value)
                            .css({
                                color: isNegative ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                },
            ];
        }
        else if (reportId == 11) {
            col = [
                //{ dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'accountName', caption: 'A/c Name', groupIndex: 1, width: 200 },
                { dataField: 'category', caption: 'Category', groupIndex: 2, width: 200 },
                { dataField: 'partyName', caption: 'Party' },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 150 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 150 },
            ];
        }
        else if (reportId == 39) {
            col = [
                { dataField: 'accountName', caption: 'A/c Name', groupIndex: 0, width: 200 },
                { dataField: 'partyName', caption: 'Party' },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 150 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 150 },
            ];
        }
        else if (reportId == 51 || reportId == 52) {
            if (dataSrc.length > 0) {
                empr_helper.lastAmt = dataSrc[0].lastAmount;
                empr_helper.lastDate = dataSrc[0].lastDate;
            }
            col = [
                { dataField: 'accountName', caption: 'A/c Name', groupIndex: 0, },
                { dataField: 'category', caption: 'Category', groupIndex: 1, width: 200 },
                { dataField: 'partyName', caption: 'Party Name', },
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'billType', caption: 'Bill Type', width: 80 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 150 },
                { dataField: 'dueDate', caption: 'Due Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },
                { dataField: 'amount', caption: 'Amount', dataType: 'number', width: 120 },
                { dataField: 'dueAmount', caption: 'Due Amount', dataType: 'number', width: 120 },
                { dataField: 'dueYear', caption: 'Due Year', dataType: 'number', width: 120 },
                { dataField: 'dueMonth', caption: 'Due Month', dataType: 'number', width: 120 },
                { dataField: 'dueDay', caption: 'Due Days', dataType: 'number', width: 120 },
                {
                    caption: 'Balance',
                    dataType: 'number',
                    width: 120,
                    calculateCellValue: function (data) {
                        return data.balance < 0 ? `(${new Intl.NumberFormat().format(Math.abs(data.balance))})` : new Intl.NumberFormat().format(data.balance);
                    },
                    cellTemplate: function (container, options) {
                        const isNegative = options.data.balance < 0;
                        $('<div>')
                            .text(options.value)
                            .css({
                                color: isNegative ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                },
            ];
        }
        else if (reportId === 97) {
            col = [
                {
                    dataField: 'voucherDate',
                    caption: 'Date',
                    dataType: 'date',
                    width: 80,
                    allowSorting: false,
                    format: function (date) {
                        if (!date) return '';
                        const month = date.toLocaleString('en-US', { month: 'short' });
                        const year = date.getFullYear();
                        return `${month}-${year}`;
                    }
                },
                {
                    dataField: 'accountName',
                    caption: 'Account Name',
                    groupIndex: 0,
                },
                {
                    dataField: 'partyName',
                    caption: 'Party Name',

                },
                {
                    dataField: 'amount',
                    caption: 'Amount',
                    dataType: 'number',
                    width: 80,
                },
            ];
        }
        else if (reportId === 98) {
            col = [
                {
                    dataField: 'voucherDate',
                    caption: 'Date',
                    dataType: 'date',
                    width: 80,
                    allowSorting: false,
                    format: function (date) {
                        if (!date) return '';
                        const month = date.toLocaleString('en-US', { month: 'short' });
                        const year = date.getFullYear();
                        return `${month}-${year}`;
                    }
                },
                {
                    dataField: 'accountName',
                    caption: 'Account Name',
                    groupIndex: 0,

                },
                {
                    dataField: 'partyName',
                    caption: 'Party Name',

                },
                {
                    dataField: 'amount',
                    caption: 'Amount',
                    dataType: 'number',
                    width: 80,
                },
            ];
        }
        else if (reportId === 102) {
            col = [
                {
                    dataField: 'accountNature',
                    caption: 'Account Nature',
                    groupIndex: 0,

                },
                {
                    dataField: 'partyName',
                    caption: 'Party Name',
                    groupIndex: 1,

                },
                {
                    dataField: 'chqNo',
                    caption: 'Cheque No',

                },
                {
                    dataField: 'chqDate',
                    caption: 'Cheque Date',
                    dataType: 'date',
                    format: 'dd/MM/yyyy', width: 120
                },
                {
                    dataField: 'accountDescription',
                    caption: 'Description',
                    visible: !isShortReport,
                    cellTemplate: function (container, options) {
                        $('<div>')
                            .text(options.value)
                            .css({
                                'white-space': 'normal',
                                'word-wrap': 'break-word',
                                'overflow-wrap': 'break-word'
                            })
                            .appendTo(container);
                    }
                },
                {
                    dataField: 'amt',
                    caption: 'Amount',
                    dataType: 'number',
                },
            ];
        }
        else if (reportId === 99) {
            let totalDebit = 0;
            let totalCredit = 0;
            dataSrc.forEach(item => {
                totalDebit += item.debit || 0;
                totalCredit += item.credit || 0;
            });
            empr_helper.balanceAmount = totalDebit - totalCredit;
            col = [
                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 100 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 180,
                    visible: !isShortReport,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_PartyReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'chqNo', caption: 'Cheque No', width: 100 },
                { dataField: 'chqDate', caption: 'Cheque Date', dataType: 'date', format: 'dd/MM/yyyy', width: 100 },
                { dataField: 'accountName', caption: 'A/c Name', groupIndex: 0, width: 150 },
                {
                    dataField: 'accountDescription',
                    caption: 'Description',
                    visible: !isShortReport,
                    width: 250,
                    cellTemplate: function (container, options) {
                        $('<div>')
                            .text(options.value)
                            .css({
                                'white-space': 'normal',
                                'word-wrap': 'break-word',
                                'overflow-wrap': 'break-word'
                            })
                            .appendTo(container);
                    }
                },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 100 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 100 },
                {
                    caption: 'Balance',
                    dataType: 'number',
                    width: 110,
                    calculateCellValue: function (data) {
                        return data.balance < 0 ? `(${new Intl.NumberFormat().format(Math.abs(data.balance))})` : new Intl.NumberFormat().format(data.balance);
                    },
                    cellTemplate: function (container, options) {
                        const isNegative = options.data.balance < 0;
                        $('<div>')
                            .text(options.value)
                            .css({
                                color: isNegative ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                },
            ];
        }

        else if (reportId === 136) {
            col = [
                {
                    dataField: 'acT_NAME',
                    caption: 'Account Name',
                    groupIndex: 0,

                },
                { dataField: 'category', caption: 'Category', groupIndex: 1, width: 200 },

                {
                    dataField: 'partY_NAME',
                    caption: 'Party Name',
                    //groupIndex: 1,

                },
                {
                    dataField: 'aging_0_15',
                    caption: '0 To 15',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'aging_16_30',
                    caption: '16 To 30',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'aging_31_45',
                    caption: '31 To 45',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'aging_46_60',
                    caption: '46 To 60',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'aging_61_90',
                    caption: '61 To 90',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'aging_91_120',
                    caption: '91 To 120',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'aging_120_Plus',
                    caption: '120 Plus',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },
                {
                    dataField: 'balanceWithTotal',
                    caption: 'Balance',
                    dataType: 'number',
                    format: '#,##0.00',
                    customizeText: function (cellInfo) {
                        return cellInfo.value === 0 ? "" : cellInfo.valueText;
                    }
                },

                //{
                //    dataField: 'amt',
                //    caption: 'Amount',
                //    dataType: 'number',
                //},
            ];
        }
        if (reportId === 10 || reportId === 11 || reportId == 136 || reportId === 52 || reportId === 51) {
            empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName,true);
        }
        else {
            empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
        }


        setTimeout(function () {
            if (reportId === 99 || reportId === 10 || reportId === 11 || reportId === 98 || reportId === 97 || reportId === 98 || reportId === 102 || reportId === 52 || reportId === 51 || reportId == 136) {
                empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName,true);
            }
            else {
                empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }
        }, 1000);

        setTimeout(function () {
            //var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();

            var selectedIndex = $('input[name="reportRadio"]:checked').data('index');
            var selectedRowKey = [];
            //if (selectedIndex !== undefined) {
            //    selectedRowKey = reportDataSrc[selectedIndex];
            //}
            if (selectedIndex !== undefined) {
                selectedRowKey.push(reportDataSrc[selectedIndex]);
            }

            if (selectedRowKey.length > 0) {
                $('#REPORT_NAME').text(selectedRowKey[0].reporT_NAME);
            }
            $('#OptionTab').removeClass('active');
            $('#OptionTabContent').removeClass('active');
            $('#ViewTab').click();
            $('.tab-pane').removeClass('fade');
            $('#ViewTab').addClass('active')
            $('#ViewTabContent').addClass('active');
        }, 1000);
    },

    InitTradeReportGrid(dataSrc, reportId) {
        debugger;
        console.log(dataSrc)
        const totalStockInSum = dataSrc.reduce((sum, item) => {
            return sum + parseFloat(item.totaL_STOCK_IN || 0);
        }, 0);
        const totalDebit = dataSrc.reduce((sum, item) => {
            return sum + parseFloat(item.amT_DEBIT || 0);
        }, 0);

        empr_helper.debitAverage = totalDebit / totalStockInSum;

        const totalStockOutSum = dataSrc.reduce((sum, item) => {
            return sum + parseFloat(item.totaL_STOCK_OUT || 0);
        }, 0);
        const totalCredit = dataSrc.reduce((sum, item) => {
            return sum + parseFloat(item.amT_CREDIT || 0);
        }, 0);

        empr_helper.creditAverage = totalCredit / totalStockOutSum;

        var col = [];
        //if (reportId == 15) {
        //    col = [
        //        { dataField: 'voucherDate', caption: 'Date', groupIndex: 0 },
        //        { dataField: 'voucherNo', caption: 'Transaction #', width: 160 },
        //        { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 300 },
        //        { dataField: 'sellerName', caption: 'Seller A/c Name', width: 300 },
        //        { dataField: 'remarks', caption: 'Description', width: 300 },
        //        { dataField: 'itemName', caption: 'Item', width: 250 },
        //        { dataField: 'qty', caption: 'QTY', width: 80 },
        //        { dataField: 'unit', caption: 'Unit', width: 60 },
        //        { dataField: 'arivalStatus', caption: 'Arival Status', width: 90 },
        //    ];
        //}
        //else if (reportId == 14) {
        //    col = [
        //        { dataField: 'voucherDate', caption: 'Date', groupIndex: 0 },
        //        { dataField: 'voucherNo', caption: 'Transaction #', width: 150 },
        //        { dataField: 'brokerName', caption: 'Broker A/c Name', width: 300 },
        //        { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 300 },
        //        { dataField: 'sellerName', caption: 'Seller A/c Name', width: 300 },
        //        { dataField: 'remarks', caption: 'Description', width: 300 },
        //        { dataField: 'itemName', caption: 'Item', width: 250 },
        //        { dataField: 'cond', caption: 'Cond', width: 80 },
        //        { dataField: 'qty', caption: 'QTY', width: 70 },
        //        { dataField: 'unit', caption: 'Unit', width: 70 },
        //        { dataField: 'rate', caption: 'Rate', width: 70 },
        //        { dataField: 'rT_TYPE', caption: 'RT', width: 50 },
        //        { dataField: 'amt', caption: 'Amt', width: 80 },
        //    ];
        //}
        //else if (reportId == 13) {
        //    col = [
        //        { dataField: 'voucherDate', caption: 'Date', width: 150 },
        //        { dataField: 'voucherNo', caption: 'Transaction #', width: 150 },
        //        { dataField: 'buyerName', caption: 'Buyer A/c Name', width: 300 },
        //        { dataField: 'sellerName', caption: 'Seller A/c Name', width: 300 },
        //        { dataField: 'ins', caption: 'Insurance', width: 120 },
        //        { dataField: 'remarks', caption: 'Description', width: 300 },
        //        { dataField: 'itemName', caption: 'Item', width: 250 },
        //        { dataField: 'qty', caption: 'QTY', width: 70 },
        //        { dataField: 'unit', caption: 'Unit', width: 70 },
        //    ];
        //}
        //else 
        if (reportId == 17) {
            col = [
                { dataField: 'voucherDate', caption: 'Date', width: 150 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 150 },
                { dataField: 'buyerName', caption: 'Party Name', width: 300 },
                { dataField: 'itemName', caption: 'Item', width: 250 },
                { dataField: 'totaL_STOCK_IN', caption: 'Stock In', width: 100 },
                { dataField: 'totaL_STOCK_OUT', caption: 'Stock Out', width: 100 },
                { dataField: 'amT_DEBIT', caption: 'DEBIT', width: 100, visible: false },
                { dataField: 'amT_CREDIT', caption: 'Credit', width: 100, visible: false },
                { dataField: 'unit', caption: 'Unit', width: 70 },
            ];
        }
        empr_helper.DxGridBindingForReportsWithSetting('#TradeGridContainer', col, dataSrc, "Trade Report");

        setTimeout(function () {

            var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();
            //if (selectedRowKey.length > 0) {
            //    $('#REPORT_NAME').text(selectedRowKey[0].reporT_NAME);
            //}
            $('#OptionTab').removeClass('active');
            $('#OptionTabContent').removeClass('active');
            $('#ViewTab').click();
            $('.tab-pane').removeClass('fade');
            $('#ViewTab').addClass('active')
            $('#ViewTabContent').addClass('active');
        }, 500);
    },


    InitAgingReportGrid(dataSrc, reportId) {
        debugger;

        //const reportId = 51;
        console.log("aging report data", dataSrc);

        if (dataSrc.length > 0) {
            empr_helper.lastAmt = parseFloat(dataSrc[0].lastAmount) || 0;
            empr_helper.lastDate = dataSrc[0].lastDate || null;
        }

        var col = [];
        if (reportId == 51) {
            col = [
                //{ dataField: 'accountName', caption: 'A/c Name', groupIndex: 0 },
                { dataField: 'partyName', caption: 'A/c Name', groupIndex: 1 },

                { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy' },
                { dataField: 'billType', caption: 'Bill Type', width: 80 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 150 },
                { dataField: 'dueDate', caption: 'Due Date', dataType: 'date', format: 'dd/MM/yyyy', width: 120 },

                { dataField: 'amount', caption: 'Amount', dataType: 'number', width: 120 },
                { dataField: 'dueAmount', caption: 'Due Amount', dataType: 'number', width: 120 },
                { dataField: 'dueYear', caption: 'Due Year', dataType: 'number', width: 120 },
                { dataField: 'dueMonth', caption: 'Due Month', dataType: 'number', width: 120 },
                { dataField: 'dueDay', caption: 'Due Days', dataType: 'number', width: 120 },

                {
                    dataField: 'balance',
                    caption: 'Balance',
                    width: 120,

                    calculateCellValue: function (data) {
                        let bal = parseFloat(data.balance) || 0;
                        return bal < 0
                            ? `(${new Intl.NumberFormat().format(Math.abs(bal))})`
                            : new Intl.NumberFormat().format(bal);
                    },

                    cellTemplate: function (container, options) {
                        let bal = parseFloat(options.data.balance) || 0;
                        const display = bal < 0
                            ? `(${new Intl.NumberFormat().format(Math.abs(bal))})`
                            : new Intl.NumberFormat().format(bal);

                        $('<div>')
                            .text(display)
                            .css({
                                color: bal < 0 ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                },
            ];
        }


        empr_helper.DxGridBindingForReportsWithSetting('#AgingReportGridContainer', col, dataSrc, "Aging Report");

        setTimeout(function () {
            $('#OptionTab').removeClass('active');
            $('#OptionTabContent').removeClass('active');
            $('#ViewTab').click();
            $('.tab-pane').removeClass('fade');
            $('#ViewTab').addClass('active');
            $('#ViewTabContent').addClass('active');
        }, 500);
    },


    ValidateInfo: function () {

        var valid = true;
        var data = empr_PartyReports.GetDataToSave();

        var userfromDateObj = new Date(data.FROMDATE);
        var usertoDateObj = new Date(data.TODATE);

        var periodFromDateObj = new Date(data.HIDDENFROMDATE);
        var periodToDateObj = new Date(data.HIDDENTODATE);


        if ((data.REPORTID == 38 || data.REPORTID == 39) && (data.CONTROLCODE == null || data.CONTROLCODE == 0 || data.CONTROLCODE == "")) {
            empr_helper.notify("Please select Control name", 2);
            valid = false;
        }

        if (data.REPORTID == 97 && data.NATURE != 3) {
            empr_helper.notify("Please Select Vendor in nature", 2);
            valid = false;
        }

        if (data.REPORTID == 98 && data.NATURE != 4) {
            empr_helper.notify("Please Select Customer in nature", 2);
            valid = false;
        }

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
        debugger;

        if (data.REPORTID < 1) {
            empr_helper.notify("Please select report type.", 2);

        }
        debugger;
        var partyValue = $('#PARTY_NAME').dxSelectBox('option', 'value');
        if (data.REPORTID == 10 || data.REPORTID == 99) {
            if (partyValue === null || partyValue === "" || partyValue === undefined) {
                empr_helper.notify("Please select a Party before generating the report.", 2);
                valid = false;
            }
        }

        $("#Loader").hide();
        return valid;
    },

    InitItemGroupDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/SPartyReports/GetItemGroup", function (data) {
            if (data.msgType == 1) {
                empr_PartyReports.ItemGroupData = data.data;
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
                            if (!empr_PartyReports.IsCustomSelection) {
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

    InitItemMasterDDL: function (selectedValue) {
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
                                var controls = empr_PartyReports.ItemGroupData;
                                var control = controls.filter(i => i.key == item[0].groupCode);
                                if (control.length > 0) {
                                    empr_PartyReports.IsCustomSelection = true;
                                    $('#ITEM_GROUP').dxSelectBox('instance').option('value', control[0].key);
                                    setTimeout(function () {
                                        empr_PartyReports.IsCustomSelection = false;
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
    InitCategoryDDL: function (_selectedValue) {

        $.ajax({
            url: 'Category/GetCategory',
            method: 'GET',
            data: null,
            success: function (data) {
                var response = data.data;
                console.log(response, "cat data a");
                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#category_hidden").val(_selectedValue);
                        $("#displayExpr_category").val(selectedobj[0].value);
                    }
                }


                $('#CATDDL').dxSelectBox({
                    dataSource: response,
                    displayExpr: 'value',
                    valueExpr: 'key',
                    value: _selectedValue,
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
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });


    },
    openVoucherPage(link, tran_Id,bcode) {
        debugger;
        console.log(link)
        if (currentBranch == branch) {
            var newWindow = window.open(link, '_blank');

            newWindow.addEventListener('load', function () {
                setTimeout(function () {
                    newWindow.postMessage({ traN_ID: tran_Id }, '*');
                }, 1000);
            });
        }
     
    },
    openVoucherPageWithBranch(link, tran_Id,bcode) {
        debugger;
        console.log(link)
        var branch = parseInt(currentBranch);
        if (branch == bcode) {
            var newWindow = window.open(link, '_blank');

            newWindow.addEventListener('load', function () {
                setTimeout(function () {
                    newWindow.postMessage({ traN_ID: tran_Id }, '*');
                }, 500);
            });
        } else {
            empr_helper.notify("This voucher does not belong to the current branch.", 2);
        }
     
    },
}