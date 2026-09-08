var empr_AccountingReports = {
    reportDataSrc: [],
    InitEvents: function () {
        empr_AccountingReports.GetReportTypes();
        empr_AccountingReports.InitControlDDL();
        empr_AccountingReports.InitAccountDDL();
        //empr_AccountingReports.InitControlDDL();
        //empr_AccountingReports.InitAccountDDL();
        empr_AccountingReports.InitBranchDDL("");

        $('body').on('click', '#BtnGenerate', function () {
            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_AccountingReports.ValidateInfo()) {
                    empr_AccountingReports.GenerateReport();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
            }, 200);
        });

        //$('body').on('click', 'input[name="reportRadio"]', function () {
        //    $("#Loader").show();
        //    $("#Loader").css('display', 'flex');
        //    setTimeout(function () {
        //        if (empr_AccountingReports.ValidateInfo()) {
        //            empr_AccountingReports.GenerateReport();
        //            setTimeout(function () {
        //                $("#Loader").hide();
        //            }, 500);
        //        }
        //    }, 200);
        //});
    },

    //InitControlDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/PartyReports/GetControls", function (data) {
    //        if (data.msgType == 1) {
    //            empr_AccountingReports.ControlData = data.data;
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
    //                        if (!empr_AccountingReports.IsCustomSelection) {
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

    //InitAccountDDL(selectedValue) {
    //    ajaxHelper.ajaxGetJson("/PartyReports/GetParties", function (data) {
    //        if (data.msgType == 1) {
    //            empr_AccountingReports.PartyData = data.data;
    //            $('#PARTY_NAME').dxSelectBox({
    //                //dataSource: data.data,
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
    //                dataSource: {
    //                    store: data.data,
    //                    paginate: true,
    //                    pageSize: 50
    //                },
    //                paging: {
    //                    enabled: true,
    //                    pageSize: 50,
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    if (e.value != '' && e.value != null) {
    //                        var items = e.component._dataSource._items;
    //                        var item = items.filter(i => i.key == e.value);
    //                        if (item.length > 0) {
    //                            var controls = empr_AccountingReports.ControlData;
    //                            var control = controls.filter(i => i.key == item[0].accountCode);
    //                            if (control.length > 0) {
    //                                empr_AccountingReports.IsCustomSelection = true;
    //                                $('#CONTROL_NAME').dxSelectBox('instance').option('value', control[0].key);
    //                                setTimeout(function () {
    //                                    empr_AccountingReports.IsCustomSelection = false;
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

    InitBranchDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/InvoiceReports/GetBranch", function (data) {
            if (data.msgType == 1) {
                empr_AccountingReports.BranchData = data.data;
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

    InitControlDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/AccountingReports/GetControls", function (data) {
            if (data.msgType == 1) {
                $('#CONTROL_NAME').dxSelectBox({
                    dataSource: data.data,
                    displayExpr: 'value',
                    valueExpr: 'accountGRCode',
                    value: selectedValue,
                    searchEnabled: true,
                    width: '100%',
                    placeholder: 'Search',
                    showClearButton: true,
                    dropDownOptions: {
                        height: 'auto',
                    },
                    pagingEnabled: true,
                    searchTimeout: 500
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitAccountDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/AccountingReports/GetSubsidiarities", function (data) {
            empr_AccountingReports.PartyData = data.data;
            if (data.msgType == 1) {
                $('#PARTY_NAME').dxSelectBox({
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
                    searchTimeout: 500
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    GetReportTypes: function () {
        ajaxHelper.ajaxGetJson("/AccountingReports/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                empr_AccountingReports.InitReportTypeGrid(data.data);
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
    //    empr_helper.dxGridbindingForReports('#GridContainer', col, dataSrc, "ReportTypes", 'single');
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
        var selectedIndex = $('input[name="reportRadio"]:checked').data('index');
        var selectedRowKey = [];
        //if (selectedIndex !== undefined) {
        //    selectedRowKey = reportDataSrc[selectedIndex];
        //}

        //var sellerPartyCode = '';
        //var parties = empr_AccountingReports.PartyData;
        //debugger;
        //if (parties.length > 0) {
        //    var selectedParty = parties.filter(i => i.key == $('#PARTY_NAME').dxSelectBox('option', 'value'));
        //    if (selectedParty.length > 0) {
        //        sellerPartyCode = selectedParty[0].partyCode;
        //    }
        //}

        if (selectedIndex !== undefined) {
            selectedRowKey.push(reportDataSrc[selectedIndex]);
        }
        var reportID = '0';
        if (selectedRowKey.length > 0) {
            reportID = selectedRowKey[0].r_ID;
            console.log('reportID', reportID);
            empr_helper.reportName = selectedRowKey[0].reporT_NAME;
        }
        var FROM_DATE = $("#FROM_DATE").val();
        var HIDDEN_FROM_DATE = $("#HIDDEN_FROM_DATE").val();
        var TO_DATE = $("#TO_DATE").val();
        var HIDDEN_TO_DATE = $("#HIDDEN_TO_DATE").val();
        var REPORT_ID = reportID;
        var CONTROL_CODE = $('#CONTROL_NAME').dxSelectBox('option', 'value');
        var ACCOUNT_CODE = $('#PARTY_NAME').dxSelectBox('option', 'value');
        var BRANCH = $('#Branch').dxSelectBox('option', 'value');
        var record = {
            FROMDATE: FROM_DATE,
            TODATE: TO_DATE,
            HIDDENFROMDATE: HIDDEN_FROM_DATE,
            HIDDENTODATE: HIDDEN_TO_DATE,
            REPORTID: REPORT_ID,
            CONTROLCODE: CONTROL_CODE,
            ACCOUNTCODE: ACCOUNT_CODE,
            BRANCH: BRANCH,
        }
        return record;
    },

    GenerateReport: function () {
        debugger;
        var dataModel = empr_AccountingReports.GetDataToSave();
        console.log(dataModel);
        ajaxHelper.ajaxPostJsonData(dataModel, "/AccountingReports/GenerateReport", function (data) {
            if (data.msgType == 1) {
                empr_AccountingReports.InitReportGrid(data.data, dataModel.REPORTID);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitReportGrid: function (dataSrc, reportId) {
        console.log(dataSrc);
        empr_helper.typeOneTotal = 0;
        empr_helper.typeTwoTotal = 0;
        empr_helper.typeThreeTotal = 0;
        empr_helper.typeFourTotal = 0;

        if (reportId == 3) {
            let totalDebit = 0;
            let totalCredit = 0;
            dataSrc.forEach(item => {
                totalDebit += item.debit || 0;
                totalCredit += item.credit || 0;
                if (
                    item.chqDate == '1900-01-01' || item.chqDate == '01-01-1900' || item.chqDate == '01-Jan-1900' || item.chqDate == '1/1/1900 12:00:00 AM' || item.chqDate == '01/01/1900 12:00:00 AM' || item.chqDate == '1/1/1900' ||
                    item.chqDate == '2000-01-01' || item.chqDate == '01-01-2000' || item.chqDate == '01-Jan-2000' || item.chqDate == '1/1/2000 12:00:00 AM' || item.chqDate == '01/01/2000 12:00:00 AM' || item.chqDate == '1/1/2000' ||
                    item.chqDate == '00-01-01' || item.chqDate == '01-01-00' || item.chqDate == '01-Jan-00' || item.chqDate == '1/1/00 12:00:00 AM' || item.chqDate == '01/01/00 12:00:00 AM' || item.chqDate == '1/1/00' || item.bilL_DATE == '01-Jan-00 12:00:00 AM'
                    || item.bilL_DATE == '1900-01-01' || item.bilL_DATE == '01-01-1900' || item.bilL_DATE == '01-Jan-1900' || item.bilL_DATE == '1/1/1900 12:00:00 AM' || item.bilL_DATE == '01/01/1900 12:00:00 AM' || item.bilL_DATE == '1/1/1900' ||
                    item.bilL_DATE == '2000-01-01' || item.bilL_DATE == '01-01-2000' || item.bilL_DATE == '01-Jan-2000' || item.bilL_DATE == '1/1/2000 12:00:00 AM' || item.bilL_DATE == '01/01/2000 12:00:00 AM' || item.bilL_DATE == '1/1/2000' ||
                    item.bilL_DATE == '00-01-01' || item.bilL_DATE == '01-01-00' || item.bilL_DATE == '01-Jan-00' || item.bilL_DATE == '1/1/00 12:00:00 AM' || item.bilL_DATE == '01/01/00 12:00:00 AM' || item.bilL_DATE == '1/1/00' || item.bilL_DATE == '01-Jan-00 12:00:00 AM'
                ) {
                    item.chqDate = null;
                    item.bilL_DATE = null;
                }
            });
            empr_helper.balanceAmount = totalDebit - totalCredit;
            var col = [
                { dataField: 'bName', caption: 'Branch' },
                { dataField: 'voucherDate', caption: 'Date', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            //.attr('onclick', 'empr_AccountingReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .attr('onclick', 'empr_AccountingReports.openVoucherPageWithBranch('
                                + JSON.stringify(options.data.link) + ', '
                                + JSON.stringify(options.data.traN_ID) + ', '
                                + JSON.stringify(options.data.bcode) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'bilL_NO', caption: 'CHQ', width: 110 },
                { dataField: 'bilL_DATE', caption: 'CHQ.Date', width: 110 },
                { dataField: 'accountName', caption: 'A/c Name', groupIndex: 0 },
                {
                    dataField: 'accountDescription',
                    caption: 'Description',
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
                    caption: 'Balance',
                    dataType: 'number',
                    width: 120,
                    calculateCellValue: function (data) {
                        return data.balance2 < 0 ? `(${new Intl.NumberFormat().format(Math.abs(data.balance2))})` : new Intl.NumberFormat().format(data.balance2);
                    },
                    cellTemplate: function (container, options) {
                        const isNegative = options.data.balance2 < 0;
                        $('<div>')
                            .text(options.value)
                            .css({
                                color: isNegative ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                }
            
            ];
        }
        else if (reportId == 4) {
            var col = [
                //{ dataField: 'bName', caption: 'Branch', groupIndex: 0 },
                { dataField: 'parentName', caption: 'Parent Name', groupIndex: 1 },
                { dataField: 'accountName', caption: 'A/c Name' },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 120 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 120 }
            ];
        }
        else if (reportId == 119) {
            var col = [
                { dataField: 'parentName', caption: 'Parent Name', groupIndex: 0 },
                { dataField: 'accountName', caption: 'A/c Name' },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 120 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 120 }
            ];
        }
        else if (reportId == 6) {
            var col = [
                {
                    dataField: 'accountCode',
                    caption: 'Type',
                    groupIndex: 0,
                    calculateSortValue: function (rowData) {
                        return rowData.accountCode; // sort code ke hisaab se hoga
                    },
                    customizeText: function (cellInfo) {
                        switch (cellInfo.value) {
                            case 4: return 'Revenue';
                            case 5: return 'Expenses';
                            default: return 'Other';
                        }
                    }
                },
                /*{ dataField: 'accountType', caption: 'Type', , },*/
                { dataField: 'grCode', caption: 'GR Code', visible: false },
                { dataField: 'parentName', caption: 'Parent Name', groupIndex: 1, },
                { dataField: 'accountName', caption: 'Account', },
                {
                    dataField: 'balances',
                    caption: 'Balance',
                    dataType: 'number',
                    format: {
                        type: 'fixedPoint', // ensures decimal formatting
                        precision: 2        // 2 decimal places
                    },
                    alignment: 'right' // optional: aligns numbers neatly
                }
            ];
        }
        else if (reportId == 7) {
            var col = [
                { dataField: 'accountCode', caption: 'Code', visible: false },
                { dataField: 'accountType', caption: 'Type', groupIndex: 0 },
                { dataField: 'grCode', caption: 'GR Code', visible: false },
                { dataField: 'parentName', caption: 'Parent Name', groupIndex: 1, },
                { dataField: 'accountName', caption: 'Account', },
                { dataField: 'balance', caption: 'Balance', }
            ];
        }
        else if (reportId == 121) {

            $.each(dataSrc, function (index, item) {
                if (item.vC_TYPE === 1 && item.debit !== null) {
                    empr_helper.typeOneTotal += item.debit;
                }
                if (item.vC_TYPE === 2 && item.debit !== null) {
                    empr_helper.typeTwoTotal += item.debit;
                }
                if (item.vC_TYPE === 3 && item.debit !== null) {
                    empr_helper.typeThreeTotal += item.debit;
                }
                if (item.vC_TYPE === 4 && item.debit !== null) {
                    empr_helper.typeFourTotal += item.debit;
                }
            });
            console.log(empr_helper.typeOneTotal);
            console.log(empr_helper.typeTwoTotal);
            console.log(empr_helper.typeThreeTotal);
            console.log(empr_helper.typeFourTotal);

            var col = [
                { dataField: 'parentName', caption: 'Control Name', groupIndex: 1 },
                { dataField: 'accountName', caption: 'A/c Name' },
                {
                    dataField: 'debit',
                    caption: 'Amount',
                    dataType: 'number',
                    width: 120,
                    cellTemplate: function (container, options) {
                        const value = options.value;
                        const isNegative = value < 0;
                        const formattedValue = isNegative
                            ? `(${new Intl.NumberFormat().format(Math.abs(value))})`
                            : new Intl.NumberFormat().format(value);

                        $('<div>')
                            .text(formattedValue)
                            .css({
                                color: isNegative ? 'red' : 'black',
                                'text-align': 'right'
                            })
                            .appendTo(container);
                    },
                },
                {
                    dataField: 'vC_TYPE',
                    caption: '',
                    dataType: 'number',
                    width: 120,
                    groupIndex: 0,
                    visible: false,
                    groupCellTemplate: function (container) {
                        container.html("");
                    }
                }
            ];
        }
        else if (reportId == 37) {
            empr_helper.balanceAmount = "";
            var col = [
                { dataField: 'voucherDate', caption: 'Date', width: 120 },
                { dataField: 'voucherNo', caption: 'Transaction #', width: 150, },
                { dataField: 'accountName', caption: 'A/c Name' },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 120 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 120 },
                { dataField: 'accountDescription', caption: 'Description', width: 250 },
            ];
        }
        else if (reportId == 92) {
            var col = [
                { dataField: 'accountCode', caption: 'Code', width: 120, visible: false },
                { dataField: 'accountName', caption: 'Account Name' },
                { dataField: 'parentName', caption: 'Parent Name', groupIndex: 0, },
                { dataField: 'debit', caption: 'Amount', dataType: 'number', width: 120 },
            ]
        }
        else if (reportId == 101) {
            var col = [
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_AccountingReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'chq', caption: 'CHQ', width: 70 },
                { dataField: 'chqDate', caption: 'CHQ.Date', width: 110 },
                { dataField: 'amt', caption: 'Amount', dataType: 'number', width: 80 },
                { dataField: 'bankName', caption: 'Bank Name', width: 200 },
                { dataField: 'desc', caption: 'Description' },
                { dataField: 'bType', caption: 'Type', groupIndex: 0 },
            ];
        }
        else if (reportId == 103) {
            debugger;
            let natures = [...new Set(dataSrc.map(x => x.accountNature))].sort((a, b) => a - b);
            let grossTotal = 0;
            let sales = 0;
            let salesReturn = 0;
            let purchase = 0;
            let formattedTotal;
            // loop karke har nature ka debit-credit calculate karo
            natures.forEach(nature => {
                let debitSum = dataSrc
                    .filter(x => x.accountNature === nature)
                    .reduce((sum, x) => sum + (x.debit || 0), 0);

                let creditSum = dataSrc
                    .filter(x => x.accountNature === nature)
                    .reduce((sum, x) => sum + (x.credit || 0), 0);

                if (nature == 5)
                    sales = creditSum - debitSum;
                if (nature == 6)
                    salesReturn = debitSum - creditSum;
                if (nature == 7)
                    purchase = debitSum - creditSum;

            });
            grossTotal = (sales - salesReturn) - purchase;
            formattedTotal = grossTotal.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            var col = [
                {
                    dataField: 'accountNature',
                    caption: 'Nature',
                    groupIndex: 0,
                    allowSorting: false,
                    calculateGroupValue: function (rowData) {
                        return rowData.accountNature;
                    },
                    groupCellTemplate: function (container, options) {
                        var map = {
                            1: "CASH A/C",
                            2: "BANK A/C",
                            3: "VENDOR",
                            4: "CUSTOMER",
                            5: "SALES",
                            6: "SALES RETURN",
                            7: "PURCHASE",
                            7.2: `Gross Total :        ${formattedTotal}`,
                            8: "PURCHASE RETRUN",
                            9: "SALES PERSON",
                            10: "TRANSPOTERS",
                            11: "Fixed Assets",
                            15: "Stiching Unit",
                            20: "OTHERS",
                            21: "LABOUR",
                            22: "Cash Sales",
                            23: "Cash Sales Return",
                            24: "Advance",
                            25: "Transfer Shop",
                            26: "Expenses",
                            27: "Shop Expenses"
                        };

                        var div = $("<div>")
                            .css({
                                "text-align": "center",
                                "font-weight": "bold",
                                "width": "100%"
                            })
                            .text(map[options.value] || options.value)
                            .appendTo(container);

                        if (options.value === 7.2) { // GROSS TOTAL
                            div.css("text-align", "right");
                            div.css("font-weight", "bold");
                            div.css("color", "rgba(51, 51, 51, .7)");
                        }
                    },
                    rowTemplate: function (container, row) {
                        var rowData = row.data;
                        if (rowData.isGrossTotal) {
                            $("<tr>")
                                .css({ "font-weight": "bold", "background-color": "#eef" })
                                .append(
                                    $("<td>").text(rowData.accountName),
                                    $("<td>").text(""), // Account Name column empty
                                    $("<td>").text(""), // Debit empty
                                    $("<td>").text(rowData.credit.toLocaleString('en-US')) // Credit = grossTotal
                                )
                                .appendTo(container);
                        } else {
                            // Normal row rendering
                            this.defaultRowTemplate(container, row);
                        }
                    }
                },
                { dataField: 'accountName', caption: 'Account Name'},
                { dataField: 'debit', caption: 'Debit', width: 200 },
                { dataField: 'credit', caption: 'Credit', width: 200 }
            ];


        }
        else if (reportId == 106 || reportId == 107 || reportId == 108 || reportId == 109) {
            var col = [
                { dataField: 'voucherDate', caption: 'Date', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_AccountingReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'chqDate', caption: 'CHQ.Date', width: 120 },
                { dataField: 'chqNo', caption: 'CHQ.No', width: 100 },
                { dataField: 'accountName', caption: 'Account Name', width: 200 },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 100 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 100 },
                { dataField: 'accountDescription', caption: 'Account Description' },
                { dataField: 'bType', caption: 'Type', groupIndex: 0 },
            ];
        }
        else if (reportId == 110 || reportId == 111 || reportId == 116) {
            var col = [
                { dataField: 'voucherDate', caption: 'Date', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_AccountingReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'chqDate', caption: 'CHQ.Date', width: 120 },
                { dataField: 'chqNo', caption: 'CHQ.No', width: 100 },
                { dataField: 'accountName', caption: 'Account Name', width: 200, groupIndex: 0 },
                { dataField: 'bType', caption: 'Type' },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 100 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 100 },
                { dataField: 'accountDescription', caption: 'Account Description' },
            ];
        }
        else if (reportId == 112 || reportId == 113 || reportId == 114 || reportId == 115) {
            var col = [
                { dataField: 'voucherDate', caption: 'Date', width: 120 },
                {
                    dataField: 'voucherNo', caption: 'Transaction #', width: 200,
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_AccountingReports.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                            .appendTo(container);
                    }
                },
                { dataField: 'chqDate', caption: 'CHQ.Date', width: 120 },
                { dataField: 'chqNo', caption: 'CHQ.No', width: 100 },
                { dataField: 'accountName', caption: 'Account Name', width: 200, groupIndex: 0 },
                { dataField: 'bType', caption: 'Type', width: 250 },
                { dataField: 'qty', caption: 'Qty', width: 70 },
                { dataField: 'rate', caption: 'Rate', width: 70 },
                { dataField: 'debit', caption: 'Debit', dataType: 'number', width: 100 },
                { dataField: 'credit', caption: 'Credit', dataType: 'number', width: 100 },
                { dataField: 'accountDescription', caption: 'A.Description' },
            ];
        }
        else if (reportId == 121) {
            var col = [
                { dataField: 'parentName', caption: 'Control Name', },
                { dataField: 'accountName', caption: 'Account Name', },
                { dataField: 'debit', caption: 'DEBIT', },
                { dataField: 'voucherType', caption: 'V_Type', groupIndex: 0, visible: false, },
            ];
        }
        if ($('#ReportGridContainer').data('dxDataGrid') != undefined) {
            $('#ReportGridContainer').data('dxDataGrid').dispose();
        }
        debugger;
        if (reportId == 3 || reportId == 4) {
            empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
        } else if (reportId >= 106 && reportId <= 116) {
            empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName, true);
        }
        else {
            empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
        }
        setTimeout(function () {
            if (reportId == 3 || reportId == 4) {
                empr_helper.DxGridBindingForReportsWithSetting_Aging('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }else if (reportId >= 106 && reportId <= 116) {
                empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName, true);
            } else if (reportId == 103) {
                empr_helper.DxGridBindingForReportsWithSetting_IncomeReport('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }
            else {
                empr_helper.DxGridBindingForReportsWithSetting('#ReportGridContainer', col, dataSrc, empr_helper.reportName);
            }
        }, 500);
        
        setTimeout(function () {

            var selectedIndex = $('input[name="reportRadio"]:checked').data('index');
            var selectedRowKey = [];
            //if (selectedIndex !== undefined) { 
            //    selectedRowKey = reportDataSrc[selectedIndex];
            //}
            if (selectedIndex !== undefined) {
                selectedRowKey.push(reportDataSrc[selectedIndex]);
            }

            //var selectedRowKey = $('#GridContainer').dxDataGrid('instance').getSelectedRowKeys();
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

    ValidateInfo: function () {

        var valid = true;
        var data = empr_AccountingReports.GetDataToSave();


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

    openVoucherPage(link, tran_Id) {
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
            //newWindow.postMessage({ traN_ID: tran_Id }, '*');
        });
    },
    openVoucherPageWithBranch(link, tran_Id, bcode) {
        debugger;
        console.log(link)
        var branch = parseInt(currentBranch);
        if (branch == bcode) {
            var newWindow = window.open(link, '_blank');

            newWindow.addEventListener('load', function () {
                setTimeout(function () {
                    newWindow.postMessage({ traN_ID: tran_Id }, '*');
                }, 1000);
            });
        } else {
            empr_helper.notify("This voucher does not belong to the current branch.", 2);
        }

    },

}