var empr_ImportGeneralManifest = {
    totalCount: 0,
    rowsCount: 0,
    RT_TYPES: new DevExpress.data.ArrayStore({
        key: "key",
        data: [
            { key: 1, value: '1 KG' },
            { key: 40, value: '40 kG' },
        ]
    }),
    Branch_RT_TYPE: '',
    InitEvents() {
        $(document).ready(function () {
            empr_ImportGeneralManifest.ResetForm();

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_ImportGeneralManifest.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_ImportGeneralManifest.ResetForm();
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ImportGeneralManifest.ValidateInfo()) {
                            empr_ImportGeneralManifest.SaveInfo();
                        }
                    }
                } else {
                    if (empr_ImportGeneralManifest.ValidateInfo()) {
                        empr_ImportGeneralManifest.SaveInfo();
                    }
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_ImportGeneralManifest.GetImportGeneralManifestByCode(id);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_ImportGeneralManifest.Delete();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },

    ResetForm() {
        empr_ImportGeneralManifest.CreateGrid([{ __KEY__: empr_ImportGeneralManifest.GenerateKey(36), chK1: true, chk: "1" }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        empr_ImportGeneralManifest.InitDropdowns();
        $('#V_DATE').focus();
        $('#ARIVAL_STATUS').dxSelectBox('instance').option('value', 'P');

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
    },

    CreateGrid(dataSrc) {
        if (dataSrc.length > 0) {
            empr_ImportGeneralManifest.rowsCount = dataSrc.length - 1;
        }
        var col = [
            {
                dataField: "Action",
                width: 120,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_ImportGeneralManifest.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_ImportGeneralManifest.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_ImportGeneralManifest.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_ImportGeneralManifest.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_ImportGeneralManifest.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_ImportGeneralManifest.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'iteM_CODE',
                caption: 'Item',
                allowSorting: false,
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                width: 120,
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 1;
                    var rate = parseFloat(currentRowData.rate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                width: 120,
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {

                    if (value != '') {
                        newData.unit = value;
                        var selectedUnit = Units.filter(u => u.key == value);
                        if (selectedUnit.length > 0) {
                            newData.qtY2 = selectedUnit[0].qty;
                        }
                    }
                    else {
                        newData.qtY2 = 0;
                    }

                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 1;
                    var rate = parseFloat(currentRowData.rate) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'Y') {
                                newData.amt = newData.baL_QTY * rate;
                            }

                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'Y') {
                                newData.amt = newData.baL_QTY * rate;
                            }

                            if (empr_ImportGeneralManifest.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                }
            },
        ];
        empr_helper.editableDxGridbindingForTransactions('#DetailContainer', col, dataSrc, "ImportGeneralManifest");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        //setTimeout(function () {
        //    var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#DetailContainer').dxDataGrid('instance').focus(nextElement);
        //    //$('#V_DATE').focus();
        //}, 1500);
    },

    CloneRow(index) {
        debugger;
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_ImportGeneralManifest.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    //let clonedRowData = dataSource[index];
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    //gridInstance.addRow();
                    //$.each(clonedRowData, function (key, value) {
                    //    if (key == 'dT_CODE') {
                    //        gridInstance.cellValue(0, 'dT_CODE', '');
                    //    }
                    //    else {
                    //        gridInstance.cellValue(0, key, value);
                    //    }
                    //});
                    // After adding the row, insert it at the first position
                    //gridInstance.insertRow(clonedRowData, 0); // Assuming you want to insert at the first position
                    //dataSource.unshift(clonedRowData); // Add the cloned row data at the beginning of the dataSource array
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    //delete clonedRowData.dT_CODE;
                    clonedRowData.__KEY__ = empr_ImportGeneralManifest.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_ImportGeneralManifest.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                //let clonedRowData = dataSource[index];
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                //gridInstance.addRow();
                //$.each(clonedRowData, function (key, value) {
                //    if (key == 'dT_CODE') {
                //        gridInstance.cellValue(0, 'dT_CODE', '');
                //    }
                //    else {
                //        gridInstance.cellValue(0, key, value);
                //    }
                //});
                // After adding the row, insert it at the first position
                //gridInstance.insertRow(clonedRowData, 0); // Assuming you want to insert at the first position
                //dataSource.unshift(clonedRowData); // Add the cloned row data at the beginning of the dataSource array
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_ImportGeneralManifest.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
            }
        }
    },

    AddRow() {
        debugger;
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_ImportGeneralManifest.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_ImportGeneralManifest.GenerateKey(36), chK1: true, chk: "1" });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_ImportGeneralManifest.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_ImportGeneralManifest.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_ImportGeneralManifest.GenerateKey(36), chK1: true, chk: "1" });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },

    DeleteRow(index) {
        debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_ImportGeneralManifest.rowsCount -= 1;
                    gridInstance.saveEditData();
                }
                else {
                    swal({
                        title: 'Are you sure you want to remove this record?',
                        text: "You won't be able to revert this!",
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#0CC27E',
                        cancelButtonColor: '#FF586B',
                        confirmButtonText: 'Yes, delete it!',
                        cancelButtonText: 'No, cancel!',
                        confirmButtonClass: 'btn btn-success mr-5',
                        cancelButtonClass: 'btn btn-danger',
                        buttonsStyling: false
                    }).then(function () {
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/ImportGeneralManifest/DeleteImportGeneralManifestDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_ImportGeneralManifest.rowsCount -= 1;
                                gridInstance.saveEditData();
                            }
                        }, false, true);
                    });
                }
            }
            else {
                empr_helper.notify("You are not allowed to delete the last row.", 2);
            }
        }
    },

    GenerateKey(keyLength) {

        var key = "";
        var characters = "abcdef0123456789";
        for (var i = 0; i < keyLength; i++) {
            if (i === 8 || i === 13 || i === 18 || i === 23) {
                key += "-";
            } else {
                key += characters.charAt(Math.floor(Math.random() * characters.length));
            }
        }
        return key;
    },

    GetDataToSave() {

        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var SELLER_CODE = $('#SellerCode').dxSelectBox('option', 'value');
        var BUYER_CODE = $('#BuyerCode').dxSelectBox('option', 'value');
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var ARIVAL_STATUS = $('#ARIVAL_STATUS').dxSelectBox('option', 'value');

        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            SELLER_CODE: SELLER_CODE,
            BUYER_CODE: BUYER_CODE,
            REF: REF,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS,
            ARIVAL_STATUS: ARIVAL_STATUS,
        }

        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }

        if (empr_ImportGeneralManifest.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },

    GetGridData: async function () {
        return await $('#DetailContainer').dxDataGrid('instance').option("dataSource");
    },

    ValidateInfo() {

        var valid = true;
        var data = empr_ImportGeneralManifest.GetDataToSave();

        //if (data.Master.SELLER_CODE == "" || data.Master.SELLER_CODE == null || data.Master.SELLER_CODE == undefined) {
        //    empr_helper.notify("Please select seller.", 2);
        //    valid = false;
        //    return valid;
        //}

        if (data.Master.BUYER_CODE == "" || data.Master.BUYER_CODE == null || data.Master.BUYER_CODE == undefined) {
            empr_helper.notify("Please select buyer.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.ARIVAL_STATUS == "" || data.Master.ARIVAL_STATUS == null || data.Master.ARIVAL_STATUS == undefined) {
            empr_helper.notify("Please select Arival Status.", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add items.", 2);
            valid = false;
            return valid;
        }

        $.each(data.Detail, function (index, item) {
            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
                empr_helper.notify("Please select item at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty ItemCode.");
            }
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
                console.log("Item at index " + index + " has empty Quantity.");
            }

            if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
                empr_helper.notify("Please enter correct item quantity2 at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity2.");
            }

            if (item.rate != "" && item.rate != null && item.rate != undefined && item.rate <= 0) {
                empr_helper.notify("Please enter correct rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty rate.");
            }
        });

        return valid;
    },

    SaveInfo() {
        var dataModel = empr_ImportGeneralManifest.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/ImportGeneralManifest/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                empr_ImportGeneralManifest.ResetForm();
            }
        }, false, true);
    },

    InitQuickSearchGrid() {
        empr_ImportGeneralManifest.GetImportGeneralManifests();
    },

    GetImportGeneralManifests() {
        ajaxHelper.ajaxGetJson('/ImportGeneralManifest/GetImportGeneralManifests', function (data) {
            if (data.msgType == 1) {
                empr_ImportGeneralManifest.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid(dataSrc) {
        console.log(dataSrc)
        var columns = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger

                $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                               </div>`).appendTo(container);
            }
        },
        { dataField: 'traN_ID', caption: 'Code', visible: false },
        { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'astatus', caption: 'Status', },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'ref', caption: 'Reference No', },
        { dataField: 'selleR_CODE', caption: 'Seller', },
        { dataField: 'sacT_CODE', caption: 'Seller Code', visible: false },
        { dataField: 'buyeR_CODE', caption: 'Buyer', },
        { dataField: 'bacT_CODE', caption: 'Buyer Code', visible: false },
        { dataField: 'remarks', caption: 'Remarks', },

        { dataField: 'iteM_CODE', caption: 'Item', },
        { dataField: 'qty', caption: 'Quantity', },
        { dataField: 'unit', caption: 'Unit', },

        { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
        { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
        { dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
        { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
        { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
        { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
        { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        { dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, },
            //{ dataField: 'detail', calculateFilterExpression: function (value) {
            //        return [function (data) {
            //            var details = data.detail,
            //                detail;

            //            for (var i = 0; i < details.length; i++) {
            //                detail = details[i];
            //                for (var fieldName in detail) {
            //                    if (detail[fieldName] && detail[fieldName].toString().toLowerCase().indexOf(value.toLowerCase()) >= 0) {
            //                        return true;
            //                    }
            //                }
            //            }
            //            return false;
            //        }, "=", true]
            //    },
            //    visible: false,
            //    showInColumnChooser: false 
            //}
        ];
        //var detailColumns = [
        //    { dataField: 'iteM_CODE', caption: 'Item Name' },
        //    { dataField: 'qty', caption: 'Qty', },
        //    { dataField: 'unit', caption: 'Unit', },
        //    { dataField: 'qtY2', caption: 'Qty2', },
        //    { dataField: 'baL_QTY', caption: 'Balance Quantity', },
        //    { dataField: 'rate', caption: 'Rate', },
        //    { dataField: 'rT_TYPE', caption: 'RT Type', },
        //    { dataField: 'amt', caption: 'Amount', },
        //    { dataField: 'dT_DESC', caption: 'Description', visible: false },
        //];
        //empr_helper.MasterDetailDxGridBinding('#gridContainer', columns, detailColumns, dataSrc, "ImportGeneralManifest");
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "ImportGeneralManifest");
    },

    GetImportGeneralManifestByCode(code) {
        ajaxHelper.ajaxGetJson('/ImportGeneralManifest/GetImportGeneralManifestByCode?code=' + code, function (data) {
            console.log(data)
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#ARIVAL_STATUS').dxSelectBox('instance').option('value', response.arivaL_STATUS);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    empr_ImportGeneralManifest.InitDropdownsWithValue(response.selleR_CODE, response.buyeR_CODE);
                    //$('#BtnDelete').show();
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                        if (Permissions.r_EDIT) {
                            $('#BtnSave').show();
                        }
                        else {
                            $('#BtnSave').hide();
                        }
                    } else {
                        $('#BtnSave').show();
                        $('#BtnDelete').show();
                    }
                }

                if (data.detail.msgType == 1) {
                    empr_ImportGeneralManifest.CreateGrid(data.detail.data);
                    $('.card-body').addClass('customHighlightForModifiedCells');
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

    GetImportGeneralManifestDetailsByCode(code) {
        ajaxHelper.ajaxGetJson('/ImportGeneralManifest/GetImportGeneralManifestDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_ImportGeneralManifest.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    Delete() {

        swal({
            title: 'Are you sure you want to remove this record?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#0CC27E',
            cancelButtonColor: '#FF586B',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            confirmButtonClass: 'btn btn-success mr-5',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function () {
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/ImportGeneralManifest/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ImportGeneralManifest.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

    InitDropdowns() {
        debugger;
        empr_ImportGeneralManifest.InitArivalStatusDDL();
        $('#ARIVAL_STATUS').dxSelectBox('instance').option('value', 'P');
        ajaxHelper.ajaxGetJson("/ImportGeneralManifest/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_ImportGeneralManifest.InitSellerCodeDDL(data.data);
                empr_ImportGeneralManifest.InitBuyerCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitDropdownsWithValue(sCode, bCode) {
        debugger;
        //empr_ImportGeneralManifest.InitArivalStatusDDL(status);
        ajaxHelper.ajaxGetJson("/ImportGeneralManifest/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_ImportGeneralManifest.InitSellerCodeDDL(data.data, sCode);
                empr_ImportGeneralManifest.InitBuyerCodeDDL(data.data, bCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitSellerCodeDDL(dataSource, selectedValue) {
        $('#SellerCode').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'customizedKey',
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

    InitBuyerCodeDDL(dataSource, selectedValue) {
        $('#BuyerCode').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'customizedKey',
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
    }
}