var empr_ImportReports = {
    reportDataSrc: [],
    InitEvents: function () {
        console.log('controls',controls);
        console.log('subsidiary',subsidiary);
        empr_ImportReports.GetReportTypes();
        empr_ImportReports.InitControlDDL();
        empr_ImportReports.InitSubsidiaryDDL ();

        $('body').on('click', '#BtnGenerate', function () {
            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_ImportReports.ValidateInfo()) {
                    empr_ImportReports.GenerateReport();
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
        //        if (empr_ImportReports.ValidateInfo()) {
        //            empr_ImportReports.GenerateReport();
        //            setTimeout(function () {
        //                $("#Loader").hide();
        //            }, 500);
        //        }
        //    }, 200);
        //});
    },

    InitControlDDL: function (selectedValue) {
        $('#CONTROL').dxSelectBox({
            dataSource: controls,
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
    },

    InitSubsidiaryDDL: function (selectedValue) {
        $('#SUBSIDIARY').dxSelectBox({
            dataSource: subsidiary,
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
                var selectedItem = e.component.option("selectedItem");

                if (selectedItem) {
                    var selectedPCode = selectedItem.pCode;
                    empr_ImportReports.InitControlDDL(selectedPCode);
                }
            }
        });
    },

    GetReportTypes: function () {
        ajaxHelper.ajaxGetJson("/ImportReport/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                empr_ImportReports.InitReportTypeGrid(data.data);
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
        debugger;
        var selectedRowKey = [];
        //if (selectedIndex !== undefined) {
        //    selectedRowKey = reportDataSrc[selectedIndex];
        //}
        if (selectedIndex !== undefined) {
            selectedRowKey.push(reportDataSrc[selectedIndex]);
        console.log('selectedIndex', selectedIndex);
        }
        var reportID = '0';
        var reportName = '';
        if (selectedRowKey.length > 0) {
            reportID = selectedRowKey[0].r_ID;
            reportName = selectedRowKey[0].reporT_NAME;
            empr_helper.reportName = selectedRowKey[0].reporT_NAME;
        }
        var FROM_DATE = $("#FROM_DATE").val();
        var HIDDEN_FROM_DATE = $("#HIDDEN_FROM_DATE").val();
        var TO_DATE = $("#TO_DATE").val();
        var HIDDEN_TO_DATE = $("#HIDDEN_TO_DATE").val();
        var REPORT_ID = reportID;
        var CONTROL_CODE = $('#CONTROL').dxSelectBox('option', 'value');
        var SUBSIDIARY_CODE = $('#SUBSIDIARY').dxSelectBox('option', 'value');
        var record = {
            FROMDATE: FROM_DATE,
            TODATE: TO_DATE,
            HIDDENFROMDATE: HIDDEN_FROM_DATE,
            HIDDENTODATE: HIDDEN_TO_DATE,
            REPORTID: REPORT_ID,
            REPORT_NAME: reportName,
            CONTROLCODE: CONTROL_CODE,
            SUBSIDIARYCODE: SUBSIDIARY_CODE,
        }
        return record;
    },

    GenerateReport: function () {
        var dataModel = empr_ImportReports.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/ImportReport/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                //empr_ImportReports.InitReportGrid(data.data, dataModel.REPORTID);
                if (data.msgType == 1) {
                    $('#ModalBody').empty();
                    setTimeout(function () {
                        $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                        $('#ShowReportModal').show();
                        $('#ShowReportModal').modal('show');
                        //$('#ReportType').dxSelectBox('instance').option('value', MD_ID);
                    }, 100);
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

    GeneratePrintReport: function () {
        //empr_PurchaseBill.InitReportTypeDDL();
        //let TRAN_ID = empr_helper.selectedBill;
        //let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseBill/GetPrintReport", function (data) {
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

    

    ValidateInfo: function () {

        var valid = true;
        var data = empr_ImportReports.GetDataToSave();

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

        if (data.FROMDATE != null && data.HIDDENFROMDATE != null) {
            var fromDateObj = new Date(data.FROMDATE);
            var hiddenFromDateObj = new Date(data.HIDDENFROMDATE);

            if (fromDateObj < hiddenFromDateObj) {
                empr_helper.notify("please select valid date in FromDate starting from " + hiddenFromDateObj.toLocaleDateString(), 2);
                valid = false;
            }
        }
        if (data.TODATE != null && data.HIDDENTODATE != null) {
            var toDateObj = new Date(data.TODATE);
            var hiddenToDateObj = new Date(data.HIDDENTODATE);

            if (toDateObj > hiddenToDateObj) {
                empr_helper.notify("please select valid date in ToDate which ends at " + hiddenToDateObj.toLocaleDateString(), 2);
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

}