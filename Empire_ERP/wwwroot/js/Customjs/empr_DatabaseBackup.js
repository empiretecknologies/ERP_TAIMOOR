var empr_DatabaseBackup = {
    InitEvents: function () {
        $(document).ready(function () {
            empr_DatabaseBackup.InitGrid();

            $('body').on('click', '#BtnGenerate', function () {
                empr_DatabaseBackup.GenerateDBBackup();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#GridContainer').hide();
                !Permissions.r_PRINT && $('#BtnGenerate').hide();
            }
        });
    },
    InitGrid: function () {
        empr_DatabaseBackup.GetDBInformation();
    },
    GetDBInformation: function () {
        ajaxHelper.ajaxGetJson('/DatabaseBackup/GetDBInformation', function (data) {
            if (data.msgType == 1) {
                empr_DatabaseBackup.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
        var col = [
            { dataField: 'databasE_NAME', caption: 'Database Name', visible: false },
            { dataField: 'databasE_SIZE', caption: 'Database Size' },
            { dataField: 'tablE_NAME', caption: 'Table Name' },
            { dataField: 'roW_COUNTS', caption: 'Rows Count' },
            { dataField: 'createD_DATE', caption: 'Created Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'modifieD_DATE', caption: 'Last Modified Date', dataType: 'date', format: 'dd-MM-yyy' },
        ];
        empr_helper.dxGridbindingVouchers('#GridContainer', col, dataSrc, "Database Information", 'none');
    },
    GenerateDBBackup: function () {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/DatabaseBackup/GenerateDBBackup', function (data) {
            if (data.msgType == 1) {
                //window.location = window.location.origin + data.data;
                empr_helper.notify("Backup file sent successfully.", data.msgType);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
            $("#Loader").hide();
        }, false, true);
    },
}