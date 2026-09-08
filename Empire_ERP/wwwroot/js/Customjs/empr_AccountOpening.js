var empr_AccountOpening = {
    editedRows: [],
    InitEvents: function () {
        $(document).ready(function () {
            empr_AccountOpening.InitGrid();
        });

        $('body').on('click', '#BtnSave', function () {
            empr_AccountOpening.Save();
        });

        if (Permissions != "Admin") {
            (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            !Permissions.r_VIEW && $('#gridContainer').hide();
        }
    },
    InitGrid: function () {
        empr_AccountOpening.GetAccountOpenings();
    },
    GetAccountOpenings: function () {
        ajaxHelper.ajaxGetJson('/OpeningBalance/GetAccountOpenings', function (data) {
            if (data.msgType == 1) {
                empr_AccountOpening.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
        var col = [
            { dataField: 'oP_ID', caption: 'OP ID', visible: false },
            { dataField: 'acT_GR_CODE', caption: 'Group Code', allowEditing: false },
            { dataField: 'controL_NAME', caption: 'Control Name', allowEditing: false },
            { dataField: 'acT_NAME', caption: 'Account Name', allowEditing: false },
            {
                dataField: 'debit', caption: 'Debit', dataType: 'number'
                //editorOptions: {
                //    onValueChanged: function (e) {
                //        if (e.value !== null) {
                //            //var row = e.component.getDataSource().items()[e.rowIndex];
                //            //if (row.credit != 0) {
                //            //    row.credit = null; // Clear value of "Credit" column
                //            //}
                //            var rowIndex = e.rowIndex;
                //            var row = $('#gridContainer').dxDataGrid('instance').getDataSource().items()[rowIndex];
                //            if (row.credit != 0) {
                //                row.credit = null; // Clear value of "Debit" column
                //                $('#gridContainer').dxDataGrid('instance').refresh();
                //            }
                //        }
                //    }
                //} 
            },
            {
                dataField: 'credit', caption: 'Credit', dataType: 'number',
                //editorOptions: {
                //    onValueChanged: function (e) {
                //        if (e.value !== null) {
                //            var rowIndex = e.rowIndex;
                //            var row = $('#gridContainer').dxDataGrid('instance').getDataSource().items()[rowIndex];
                //            if (row.debit != 0) {
                //                row.debit = null; // Clear value of "Debit" column
                //                $('#gridContainer').dxDataGrid('instance').refresh();
                //            }
                //        }
                //    }
                //}
            },
            {
                dataField: 'astatus', caption: 'Status', lookup: {
                    dataSource: [
                        { key: 'Y', value: 'Active' },
                        { key: 'N', value: 'In-Active' }
                    ],
                    displayExpr: 'value',
                    valueExpr: 'key',
                    value: 'Y'
                }
            },
        ];
        empr_helper.editableDxGridbinding('#gridContainer', col, dataSrc, "AccountOpening");
    },
    GetEditedRows: function () {
        debugger;
        var originalArray = empr_AccountOpening.editedRows.filter(function (row) {
            return row !== undefined;
        });

        var keyMap = {
            "acT_CODE": "ACT_CODE",
            "acT_GR_CODE": "ACT_GR_CODE",
            "acT_NAME": "ACT_NAME",
            "astatus": "ASTATUS",
            "controL_NAME": "CONTROL_NAME",
            "credit": "CREDIT",
            "debit": "DEBIT",
            "oP_ID": "OP_ID"
        };
        
        //return empr_AccountOpening.RenameKeys(originalArray, keyMap);

        const newArray = originalArray.map(({
            acT_CODE: ACT_CODE,
            acT_GR_CODE: ACT_GR_CODE,
            acT_NAME: ACT_NAME,
            astatus: ASTATUS,
            controL_NAME: CONTROL_NAME,
            credit: CREDIT,
            debit: DEBIT,
            oP_ID: OP_ID
        }) => ({
            ACT_CODE,
            ACT_GR_CODE,
            ACT_NAME,
            ASTATUS,
            CONTROL_NAME,
            CREDIT,
            DEBIT,
            OP_ID
        }));

        return newArray;
    },
    RenameKeys: function (originalArray, keyMap) {
        //var renamedArray = {};
        //for (var key in originalArray) {
        //    if (keyMap.hasOwnProperty(key)) {
        //        renamedArray[keyMap[key]] = originalArray[key];
        //    } else {
        //        renamedArray[key] = originalArray[key];
        //    }
        //}
        //return renamedArray;

        var renamedArray = {};
        $.each(originalArray, function (key, value) {
            var newKey = keyMap[key] || key; // Use the mapped key if exists, otherwise keep the original key
            renamedArray[newKey] = value;
        });
        return renamedArray;
    },
    Save: function () {
        debugger;
        if ($('#gridContainer').dxDataGrid('instance').hasEditData()) {
            $('#gridContainer').dxDataGrid('instance').saveEditData().done(function () {

                var dataModel = empr_AccountOpening.GetEditedRows();
                if (dataModel.length > 0) {
                    ajaxHelper.ajaxPostJsonData({ accountOpenings: dataModel }, "/OpeningBalance/Save", function (data) {
                        empr_helper.notify(data.msg, data.msgType);
                        if (data.msgType == 1) {
                            $('#IsValidate').val('true');
                            empr_AccountOpening.editedRows = [];
                        }
                    }, false, true);
                }
                console.log("Data saved successfully");
            }).fail(function (error) {
                console.error("Error occurred while saving the data:", error);
                empr_helper.notify("Error occurred while saving the data.", 2);
                $('#gridContainer').dxDataGrid('instance').cancelEditData();
            });
        } else {
            empr_helper.notify("Please update the opening first.", 2);
        }
    },
}