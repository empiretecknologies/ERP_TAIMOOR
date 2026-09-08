var empr_BarcodePrint = {
    reportTypes: [],
    InitEvents: function () {
        $(document).ready(function () {
            empr_BarcodePrint.InitGrid();
            empr_BarcodePrint.InitReportTypeDDL();

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_BarcodePrint.PrepareDataForValidation();
            });

            if (Permissions != "Admin") {
                !Permissions.r_PRINT && $('#BtnPrint').hide();
            }
        });
    },
    PrepareDataForValidation: function () {

        if ($('#gridContainer').dxDataGrid('instance').hasEditData()) {
            $('#gridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var response = empr_BarcodePrint.GetGridData();
                response.then((data) => {
                    debugger;
                    var result = empr_BarcodePrint.ValidateMainInfo(data);
                    console.log(result);
                    //return result;
                    if (result) {
                        empr_BarcodePrint.GetBarcodeReport(data);
                    }
                });
            });
        }
        else {
            var data = $('#gridContainer').dxDataGrid('instance').getSelectedRowKeys();
            var result = empr_BarcodePrint.ValidateMainInfo(data);
            console.log(result);
            //return result;
            if (result) {
                empr_BarcodePrint.GetBarcodeReport(data);
            }
            //return empr_BarcodePrint.ValidateMainInfo($('#gridContainer').dxDataGrid('instance').getSelectedRowKeys());
        }
    },
    GetGridData: async function () {
        return await $('#gridContainer').dxDataGrid('instance').getSelectedRowKeys();
    },
    ValidateMainInfo: function (detailRecords) {

        var valid = true;
        //var detailRecords = $('#gridContainer').dxDataGrid('instance').getSelectedRowKeys();

        //var detailRecords = [];
        //if ($('#gridContainer').dxDataGrid('instance').hasEditData()) {
        //    $('#gridContainer').dxDataGrid('instance').saveEditData().done(function () {
        //        detailRecords = $('#gridContainer').dxDataGrid('instance').getSelectedRowKeys();
        //    });
        //}
        //else {
        //    detailRecords = $('#gridContainer').dxDataGrid('instance').getSelectedRowKeys();
        //}

        //var detailRecords = empr_BarcodePrint.PrepareDataToPrint();
        if (detailRecords.length == 0) {
            empr_helper.notify("Please select items to print barcodes.", 2);
            valid = false;
            return valid;
        }

        $.each(detailRecords, function (index, item) {
            if (item.issale && item.iswholesale) {
                empr_helper.notify("Please select only one rate", 2);
                valid = false;
                return valid;
            }

            if (item.iswholesale && item.isretail) {
                empr_helper.notify("Please select only one rate", 2);
                valid = false;
                return valid;
            }

            if (item.issale && item.isretail) {
                empr_helper.notify("Please select only one rate", 2);
                valid = false;
                return valid;
            }

            //if (!(item.issale || item.iswholesale || item.isretail)) {
            //    empr_helper.notify("Please select one rate", 2);
            //    valid = false;
            //    return valid;
            //}

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
                console.log("Item at index " + index + " has incorrect Quantity.");
            }

            if ((item.srate == "" || item.srate == null || item.srate == undefined) && item.sRate) {
                empr_helper.notify("Please enter sale rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty sale rate.");
            }

            if (item.srate <= 0 && item.sRate) {
                empr_helper.notify("Please enter correct sale rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has incorrect sale rate.");
            }

            if ((item.wsale == "" || item.wsale == null || item.wsale == undefined) && item.wRate) {
                empr_helper.notify("Please enter wholesale rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty wholesale rate.");
            }

            if (item.wsale <= 0 && item.wRate) {
                empr_helper.notify("Please enter correct wholesale rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has incorrect wholesale rate.");
            }

            if ((item.rrate == "" || item.rrate == null || item.rrate == undefined) && item.rRate) {
                empr_helper.notify("Please enter retail rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty retail rate.");
            }

            if (item.rrate <= 0 && item.rRate) {
                empr_helper.notify("Please enter correct retail rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has incorrect retail rate.");
            }
        });

        return valid;
    },
    InitGrid: function () {
        empr_BarcodePrint.GetBarcodePrintData();
    },
    GetBarcodePrintData: function () {
        ajaxHelper.ajaxGetJson('/BarcodePrint/GetBarcodePrintData', function (data) {
            if (data.msgType == 1) {
                empr_BarcodePrint.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        var col = [
            { dataField: 'iteM_CODE', caption: 'Item Code', allowEditing: false },
            { dataField: 'iteM_ID', caption: 'Item Id', allowEditing: false },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false },
            { dataField: 'iteM_GROUP', caption: 'Item Group', allowEditing: false },
            { dataField: 'remarks', caption: 'Remarks', allowEditing: false },
            { dataField: 'barcode', caption: 'Barcode', allowEditing: false },
            { dataField: 'barcodE_TEXT', caption: 'Barcode Text', allowEditing: false },
            { dataField: 'sizE_NAME', caption: 'Size', allowEditing: false },
            { dataField: 'coloR_NAME', caption: 'Color', allowEditing: false },
            {
                dataField: 'srate', caption: 'S. Rate', allowSorting: false, allowFiltering: false,
                headerCellTemplate: function (container) {
                    //var $header = $("<div>");
                    //$("<span>S. Rate</span>").appendTo($header);
                    //$("<input type='checkbox'>")
                    //    .appendTo($header)
                    //    .dxCheckBox({
                    //        onValueChanged: function (e) {
                    //            var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                    //            items.forEach(function (item) {
                    //                item.issale = e.value;
                    //            });
                    //            $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
                    //        }
                    //    });
                    var $header = $("<div>");
                    $('<span>S. Rate</span> <input class="RateCheckbox checkbox_animated" onclick="empr_BarcodePrint.BindRate(this)" type="checkbox" data="SRATE"></div>')
                        .appendTo($header);
                    container.append($header);
                },
            },
            {
                dataField: 'wsale', caption: 'W. Rate', allowSorting: false, allowFiltering: false,
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    $('<span>W. Rate</span> <input class="RateCheckbox checkbox_animated" onclick="empr_BarcodePrint.BindRate(this)" type="checkbox" data="WRATE"></div>')
                        .appendTo($header);
                    //$("<input type='checkbox'>")
                    //    .appendTo($header)
                    //    .dxCheckBox({
                    //        onValueChanged: function (e) {
                    //            var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                    //            items.forEach(function (item) {
                    //                item.iswholesale = e.value;
                    //            });
                    //            $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
                    //        }
                    //    });
                    
                    container.append($header);
                },
            },
            {
                dataField: 'rrate', caption: 'R. Rate', allowSorting: false, allowFiltering: false,
                headerCellTemplate: function (container) {
                    //var $header = $("<div>");
                    //$("<span>R. Rate</span>").appendTo($header);
                    //$("<input type='checkbox'>")
                    //    .appendTo($header)
                    //    .dxCheckBox({
                    //        onValueChanged: function (e) {
                    //            var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                    //            items.forEach(function (item) {
                    //                item.isretail = e.value;
                    //            });
                    //            $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
                    //        }
                    //    });
                    //container.append($header);

                    var $header = $("<div>");
                    $('<span>R. Rate</span> <input class="RateCheckbox checkbox_animated" onclick="empr_BarcodePrint.BindRate(this)" type="checkbox" data="RRATE"></div> ')
                        .appendTo($header);
                    container.append($header);
                },
            },
            { dataField: 'qty', caption: 'Qty' },
        ];
        empr_helper.editableDxGridbinding('#gridContainer', col, dataSrc, "BarcodePrint");
    },
    GetBarcodeReport: function (dataModel) {

        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        console.log(MD_ID);
        let REPORT_NAME = empr_BarcodePrint.reportTypes.filter(x => x.mD_ID == MD_ID)[0];
        console.log(REPORT_NAME);
        var dataModel = {
            data: dataModel,
            MD_ID: MD_ID,
            REPORT_NAME: REPORT_NAME.reporT_NAME,

        }
        debugger;
        ajaxHelper.ajaxPostJsonData({ data: dataModel }, "/BarcodePrint/GetBarcodeReport", function (data) {
            if (data.msgType == 1) {
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html(`<center><object data="${url}" width="1100" height="600"></object></center>`);
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
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
    BindRate: function (element) {
        var value = $(element).prop("checked");
        if (value) {
            $('.RateCheckbox').not(element).prop("checked", false);
            var data = $(element).attr('data');
            $(".RateCheckboxdiv[data='" + data + "']").prop("checked", true);
            if (data == "SRATE") {
                var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                items.forEach(function (item) {
                    item.issale = true;
                    item.iswholesale = false;
                    item.isretail = false;
                });
                $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
            }

            if (data == "WRATE") {
                var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                items.forEach(function (item) {
                    item.issale = false;
                    item.iswholesale = true;
                    item.isretail = false;
                });
                $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
            }

            if (data == "RRATE") {
                var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                items.forEach(function (item) {
                    item.issale = false;
                    item.iswholesale = false;
                    item.isretail = true;
                });
                $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
            }
        }
        else {
            var data = $(element).attr('data');
            if (data == "SRATE") {
                var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                items.forEach(function (item) {
                    item.issale = false;
                });
                $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
            }

            if (data == "WRATE") {
                var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                items.forEach(function (item) {
                    item.iswholesale = false;
                });
                $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
            }

            if (data == "RRATE") {
                var items = $('#gridContainer').dxDataGrid('instance').option("dataSource");
                items.forEach(function (item) {
                    item.isretail = false;
                });
                $("#gridContainer").dxDataGrid("instance").option("dataSource", items);
            }
        }
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/BarcodePrint/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                empr_BarcodePrint.reportTypes = data.data;
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
}