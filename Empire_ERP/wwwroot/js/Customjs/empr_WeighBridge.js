var empr_WeighBridge = {
    InitEvents: function () {
        $(document).ready(function () {
            empr_WeighBridge.InitGrid();

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#GridContainer').hide();
                !Permissions.r_PRINT && $('#BtnGenerate').hide();
            }
        });
    },
    InitGrid: function () {
        empr_WeighBridge.GetGridInformation();
    },
    GetGridInformation: function () {
        ajaxHelper.ajaxGetJson('/WeighBridge/GetGridInformation', function (data) {
            if (data.msgType == 1) {
                empr_WeighBridge.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
        var col = [
            { dataField: 'serial', caption: 'Serial' },
            { dataField: 'vehicle', caption: 'Vehicle' },
            { dataField: 'customer', caption: 'Customer' },
            { dataField: 'material', caption: 'Material' },
            { dataField: 'container', caption: 'Container' },
            { dataField: 'avg', caption: 'AVG' },
            { dataField: 'quantity', caption: 'Quantity' },
            { dataField: 'pon', caption: 'PON' },
            { dataField: 'manual', caption: 'Manual' },
            { dataField: 'mound', caption: 'Mound' },
            { dataField: 'index', caption: 'Index' },
            { dataField: 'lot', caption: 'Lot' },
            { dataField: 'packing', caption: 'Packing' },
            { dataField: 'credit', caption: 'Credit' },
            { dataField: 'ftime', caption: 'First Time' },
            { dataField: 'stime', caption: 'Second Time' },
            { dataField: 'gross', caption: 'Gross' },
            { dataField: 'tare', caption: 'Tare' },
            { dataField: 'net', caption: 'Net' },
            { dataField: 'rupees', caption: 'Rupees' },
            { dataField: 'fdate', caption: 'First Date' },
            { dataField: 'sdate', caption: 'Second Date' },
            { dataField: 'foperator', caption: 'First Operator' },
            { dataField: 'soperator', caption: 'Second Operator' },
            { dataField: 'fw_wb', caption: 'First WB#' },
            { dataField: 'sw_wb', caption: 'Second WB#' },
            { dataField: 'driver', caption: 'Driver' },
            { dataField: 'complete', caption: 'Complete' }

        ];
        empr_helper.dxGridbindingVouchers('#GridContainer', col, dataSrc, "Database Information", 'none');
    }
}