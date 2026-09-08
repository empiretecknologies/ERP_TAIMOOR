var empr_BatchIssue = {
    totalCount: 0,
    rowsCount: 0,
    BatchName: [],
    CurrentStock: [],
    //BtnUpdate
    InitEvents: function () {
        $(document).ready(function () {
            empr_BatchIssue.ResetForm();
            console.log('stkStatus', stkStatus);

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_BatchIssue.InitQuickSearchGrid();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_BatchIssue.GetBatchIssueByCode(id);
            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var date = $(this).attr("reportdate");
                swal({
                    title: 'Are you sure you want to Copy this record?',
                    text: "",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#0CC27E',
                    cancelButtonColor: '#FF586B',
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No',
                    confirmButtonClass: 'btn btn-success mr-5',
                    cancelButtonClass: 'btn btn-danger',
                    buttonsStyling: false
                }).then(function () {
                    $('#updatedDate').val(date);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModal').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/BatchIssue/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_BatchIssue.GetBatchIssueByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_BatchIssue.ValidateInfo()) {
                            empr_BatchIssue.SaveInfo();
                        }
                    }
                } else {
                    if (empr_BatchIssue.ValidateInfo()) {
                        empr_BatchIssue.SaveInfo();
                    }
                }
            });

            $('body').on('click', '#BtnNew', function () {
                empr_BatchIssue.ResetForm();
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_BatchIssue.Delete();
            });

            $('body').on('click', '#BtnPrint', function () {
                empr_BatchIssue.GeneratePrintReport();
            });

            $('body').on('click', '#BtnUpdate', function () {
                empr_BatchIssue.UpdateBatch();

            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }

            const mfgInput = document.getElementById('MFG_DATE');
            const expInput = document.getElementById('EXP_DATE');

            expInput.addEventListener('change', function () {
                const mfgValue = mfgInput.value;
                const expValue = expInput.value;

                if (mfgValue && expValue && expValue < mfgValue) {
                    empr_helper.notify("Expiry Date must be same or after Manufacturing Date.", 2);
                    expInput.value = ""; // Clear invalid value
                    expInput.focus();
                }
            });
        });
    },

    UpdateBatch: function () {




        //var ITEM_CODE = $('#FinishItem_Hidden').val();
        //var PROCESS = $('#Process_Hidden').val();

        var BQTY = $('#BatchQuantity').val();
        //var BATCH_NAME = $('#BATCH_NAME').val();
        var BATCH_NAME = $('#BATCH_NAME').dxSelectBox('option', 'value');
        var ITEM_CODE = $('#FinishItem').dxSelectBox('option', 'value');
        var PROCESS = $('#Process').dxSelectBox('option', 'value');

        if (isEmpty(ITEM_CODE)) {
            empr_helper.notify("Select Finish Item first", 2);
            return false;
        }

        if (isEmpty(BATCH_NAME)) {
            empr_helper.notify("Select Batch Name first", 2);
            return false;
        }

        if (isEmpty(BQTY)) {
            empr_helper.notify("Insert Batch Quantity", 2);
            return false;
        }

        if (isEmpty(PROCESS)) {
            empr_helper.notify("Select Process first", 2);
            return false;
        }

        function isEmpty(val) {
            return val === null || val === undefined || val === "" || val === 0;
        }

        $('#Loader').show();
        $("#Loader").css('display', 'flex');

        var obj = {
            BQTY: BQTY,
            BATCH_NAME: BATCH_NAME,
            ITEM_CODE: ITEM_CODE,
            PROCESS: PROCESS,
        };

        ajaxHelper.ajaxPostJsonData(obj, "/BatchIssue/BatchUpdate", function (data) {
            console.log('BatchUpdate', data);
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    var updatedData = data.data.map(item => {
                        var stockRecord = empr_BatchIssue.CurrentStock.find(s => s.itemId == item.iteM_CODE);
                        return {
                            ...item,
                            currentStock: stockRecord ? stockRecord.balance : 0
                        };
                    });

                    setTimeout(function () {

                        $('#CostPercent').val(updatedData[0].cost);

                        var grid = $('#DetailContainer').dxDataGrid('instance');

                        grid.saveEditData().done(function () {
                            var existingData = grid.getDataSource().items();
                            console.log(existingData);

                            existingData = existingData.filter(function (row) {
                                return row && (
                                    row.iteM_CODE ||
                                    row.qty ||
                                    row.unit ||
                                    row.dT_DESC ||
                                    row.rate ||
                                    row.amt ||
                                    row.loss
                                );
                            });

                            var mergedData = existingData.concat(updatedData);
                            grid.option('dataSource', mergedData);
                            grid.refresh();
                        });
                        /*empr_BatchIssue.CreateGrid(data.data);*/

                        empr_helper.notify(data.msg, data.msgType);
                        $('#BtnUpdate').prop('disabled', true);
                        $('#BatchQuantity').prop('disabled', true);
                        $('#BATCH_NAME').dxSelectBox('instance').option('disabled', true);
                        $('#FinishItem').dxSelectBox('instance').option('disabled', true);
                        $('#Process').dxSelectBox('instance').option('disabled', true);
                        $('#Loader').hide();


                    }, 1000);
                } else {
                    empr_helper.notify('No Batch Found', 2);
                    $('#Loader').hide();
                }
            }
            else {
                $('#Loader').hide();
                empr_helper.notify(data.msg, data.msgType);
            }







        }, false, true);
    },

    GeneratePrintReport: function () {
        empr_BatchIssue.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = 0;
        let reportName = "";

        let selectedItem = $('#ReportType').dxSelectBox('option', 'selectedItem');
        if (selectedItem) {
            MD_ID = selectedItem.mD_ID;
            reportName = selectedItem.reporT_NAME;
        }

        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Something went wrong..", 2);
            return;
        }

        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
            REPORT_NAME: reportName,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/BatchIssue/GetPrintReport", function (data) {
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

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/BatchIssue/GetReportTypes", function (data) {
            if (data.msgType == 1) {
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

    ResetForm: function () {
        empr_BatchIssue.InitReportTypeDDL();
        empr_BatchIssue.GetCurrentStock();
        empr_helper.selectedBill = 0;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        $('#BtnUpdate').prop('disabled', false);
        $('#BatchQuantity').prop('disabled', false);

        


        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }

        empr_BatchIssue.CreateGrid([{ __KEY__: empr_BatchIssue.GenerateKey(36) }]);

        if ($('#FinishItem').data('dxSelectBox') != null) {
            $('#FinishItem').dxSelectBox('instance').dispose();
        }
        if ($('#Process').data('dxSelectBox') != null) {
            $('#Process').dxSelectBox('instance').dispose();
        }
        empr_BatchIssue.InitFinishItemsDDL();
        empr_BatchIssue.InitProcessesDDL();
        empr_BatchIssue.InitBatchNameDDL();
        empr_BatchIssue.InitUnitDDL();
        $('#BATCH_NAME').dxSelectBox('instance').option('disabled', false);
        $('#MFG_DATE').val(todayDate);
        console.log('todayDate',todayDate);

        var date = new Date(todayDate);
        date.setFullYear(date.getFullYear() + 2);
        date.setDate(date.getDate() - 1);
        var expDate = date.toISOString().split('T')[0];
        $('#EXP_DATE').val(expDate);
    },

    CreateGrid: function (dataSrc, isEditable = true) {
        console.log('creategrid', dataSrc);
        if (dataSrc.length > 0) {
            empr_BatchIssue.rowsCount = dataSrc.length - 1;
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
                    if (isEditable) {
                        if (Permissions != "Admin") {
                            const copyAction = !Permissions.r_COPY
                                ? ''
                                : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_BatchIssue.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                            const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                                ? ''
                                : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_BatchIssue.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                            const deleteAction = !Permissions.r_DLT
                                ? ''
                                : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_BatchIssue.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                            const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                            $(actions).appendTo(container);
                        } else {
                            $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_BatchIssue.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_BatchIssue.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_BatchIssue.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                           </div>`).appendTo(container);
                        }
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
                width: 450,
                alignment: 'center',
                allowSorting: false,
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.iteM_CODE = value;
                    var selectedItem = Items.find(u => u.key == value);
                    if (selectedItem) {
                        newData.unit = selectedItem.unit;
                    }

                    var stockRecord = empr_BatchIssue.CurrentStock.find(s => s.itemId == value);

                    if (stockRecord) {
                        newData.currentStock = stockRecord ? stockRecord.balance : 0;
                    } else {
                        newData.currentStock = 0;
                    }
                }
            },
            {
                dataField: 'currentStock',
                caption: 'Current Stock',
                allowEditing: false,
                alignment: 'right',
                dataType: 'number',
                format: "#,##0.##########",
                cellTemplate: function (container, options) {
                    // options.value raw number hai (12345.67)
                    // options.text formatted string hai ("12,345.67")
                    var displayValue = options.text;
                    var rawValue = options.value || 0;

                    var color = rawValue < 0 ? 'red' : 'black';

                    $('<span>')
                        .text(displayValue) // formatted text yahan use hoga
                        .css('color', color)
                        .css('font-weight', rawValue < 0 ? 'bold' : 'normal')
                        .appendTo(container);
                },
            },
            {
                dataField: 'qty',
                caption: 'Qty',
                width: 150,
                alignment: 'right',
                dataType: 'number',
                format: "#,##0.##########",
                setCellValue: function (newData, value, currentRowData) {
                    // Set quantity
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    //var rate = parseFloat(currentRowData.rate) || 0;
                    var tax = parseFloat(currentRowData.taX_AMT) || 0;
                    var disc = parseFloat(currentRowData.disC_AMT) || 0;
                    var loss = parseFloat(currentRowData.loss) || 0;

                    // Validation
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    //newData.amt = (qty * rate).toFixed(2);
                    newData.losS_WEIGHT = (loss * qty) / 100;
                    newData.neT_AMT = (parseFloat(newData.amt) + tax) - disc;
                }

            },
            {
                dataField: 'unit',
                caption: 'Unit',
                width: 150,
                alignment: 'center',
                allowEditing: false,
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                alignment: 'center',
            },
            {
                dataField: 'loss',
                caption: 'Normal Loss%',
                width: 150,
                alignment: 'right',
                dataType: 'number',
                format: "#,##0.##########",
                allowEditing: true,
                setCellValue: function (newData, value, currentRowData) {
                    newData.loss = value;
                    var qty = parseFloat(currentRowData.qty) || 0;

                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }

                    if (!isNaN(qty) && !isNaN(newData.loss)) {
                        newData.losS_WEIGHT = (qty * newData.loss) / 100;
                    }
                }
            },
            {
                dataField: 'losS_WEIGHT',
                caption: 'Loss Weight',
                dataType: 'number',
                format: "#,##0.##########",
                alignment: 'right',
                width: 150,
                allowEditing: false,
            }
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "BatchIssue", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        //setTimeout(function () {
        //    var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#DetailContainer').dxDataGrid('instance').focus(nextElement);  
        //}, 1500);
    },

    CloneRow: function (index) {
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_BatchIssue.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_BatchIssue.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_BatchIssue.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_BatchIssue.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
            }
        }
    },

    AddRow: function () {
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_BatchIssue.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_BatchIssue.GenerateKey(36) });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_BatchIssue.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_BatchIssue.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_BatchIssue.GenerateKey(36) });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },

    DeleteRow: function (index) {
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_BatchIssue.rowsCount -= 1;
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
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/BatchIssue/DeleteBatchIssueDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_BatchIssue.rowsCount -= 1;
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

    GenerateKey: function (keyLength) {

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

    InitFinishItemsDDL: function (selectedValue) {
        $('#FinishItem').dxSelectBox({
            dataSource: FinishItems,
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
                if (e.value) {
                    ajaxHelper.ajaxGetJson('/BatchIssue/GetBatchName?itemCode=' + e.value, function (data) {
                        if (data.msgType == 1) {
                            empr_BatchIssue.BatchName = data.data;
                            empr_BatchIssue.InitBatchNameDDL();
                        }
                        else {
                            empr_helper.notify(data.data, data.msgType);
                        }
                    }, false, true);
                }
                else {
                    empr_BatchIssue.BatchName = [];
                    empr_BatchIssue.InitBatchNameDDL();
                }
            },
        });
    },

    InitBatchNameDDL: function (selectedValue) {
        $('#BATCH_NAME').dxSelectBox({
            dataSource: empr_BatchIssue.BatchName,
            displayExpr: 'batchName',
            valueExpr: 'batchName',
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

    InitProcessesDDL: function (selectedValue) {
        $('#Process').dxSelectBox({
            dataSource: Processes,
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

    InitUnitDDL: function (selectedValue) {
        $('#Unit').dxSelectBox({
            dataSource: Unit,
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

    BindDxDDL: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },

    InitQuickSearchGrid: function () {
        empr_BatchIssue.GetBatchIssues();
    },

    GetBatchIssues: function () {
        ajaxHelper.ajaxGetJson('/BatchIssue/GetBatchIssues', function (data) {
            if (data.msgType == 1) {
                empr_BatchIssue.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    
    GetCurrentStock: function () {
        ajaxHelper.ajaxGetJson('/BatchIssue/GetCurrentStock', function (data) {
            debugger;
            if (data.length > 0) {
                empr_BatchIssue.CurrentStock = data;
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {

                $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                       <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                       </div>`).appendTo(container);
            }
        },
        { dataField: 'traN_ID', caption: 'Code', },
        { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'astatus', caption: 'Status', },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'ref', caption: 'Reference No', },
        { dataField: 'iteM_NAME', caption: 'Item Name', },
        { dataField: 'cost', caption: 'Cost%', },
        { dataField: 'process', caption: 'Process', },
        { dataField: 'remarks', caption: 'Remarks', },
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
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "BatchIssueQS");
    },

    GetDataToSave: function () {
        //getdata
        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var REF = $("#REF").val();
        var COSTPERCENT = $("#CostPercent").val();
        //var NORMALLOSSPERCENT = $("#NormalLossPercent").val();

        var ITEM_CODE = $('#FinishItem').dxSelectBox('option', 'value');
        var PROCESS = $('#Process').dxSelectBox('option', 'value');
        var UNIT = $('#Unit').dxSelectBox('option', 'value');
        var BATCH_NAME = $('#BATCH_NAME').dxSelectBox('option', 'value');
        //var ITEM_CODE = $('#FinishItem_Hidden').val();
        //var PROCESS = $('#Process_Hidden').val();
        //var UNIT = $('#Unit_Hidden').val();
        var BQTY = $('#BatchQuantity').val();
        var REMARKS = $("#REMARKS").val();
        var BATCHNO = $("#BatchNo").val();
        var MFGDATE = $("#MFG_DATE").val();
        var EXPDATE = $("#EXP_DATE").val();


        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            ASTATUS: ASTATUS,
            ITEM_CODE: ITEM_CODE,
            REF: REF,
            COST: COSTPERCENT,
            //LOSS: NORMALLOSSPERCENT,
            REMARKS: REMARKS,
            PROCESS: PROCESS,
            UNIT: UNIT,
            BQTY: BQTY,
            ASTATUS: ASTATUS,
            BATCHNO: BATCHNO,
            BATCH_NAME: BATCH_NAME,
            MFGDATE: MFGDATE,
            EXPDATE: EXPDATE
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
        if (empr_BatchIssue.rowsCount == detailRecords.length) {
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

    ValidateInfo: function () {

        var valid = true;
        var data = empr_BatchIssue.GetDataToSave();

        if (data.Master.ITEM_CODE == '') {
            empr_helper.notify("Item is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.PROCESS == '') {
            empr_helper.notify("Process is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.BQTY == '') {
            empr_helper.notify("Batch quantity is required.", 2);
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
                empr_helper.notify("Please select item at index " + (1 + index), 2);
                valid = false;
                return valid;
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at index " + (1 + index), 2);
                valid = false;
                return valid;
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity at index " + (1 + index), 2);
                valid = false;
                return valid;
            } 

            if (stkStatus == 'Y') {
                if (item.currentStock <= 0) {
                    empr_helper.notify("Stock unavailable at line No " + (1 + index), 2);
                    valid = false;
                    return false;
                }
                else if (item.qty > item.currentStock) {
                    empr_helper.notify("Quantity exceeds available stock (" + item.currentStock + ") at line No " + (1 + index), 2);
                    valid = false;
                    return false;
                }
            }
            

        });

        return valid;
    },

    SaveInfo: function () {
        var dataModel = empr_BatchIssue.GetDataToSave();
        console.log('SaveAttemp', dataModel);
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/BatchIssue/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    empr_BatchIssue.GetCurrentStock();
                }
                empr_BatchIssue.GetBatchIssueDetailsByCode($('#Code').val());
                $('#BtnDelete').show();
            }
        }, false, true);
    },

    GetBatchIssueByCode: function (code) {
        empr_BatchIssue.ResetForm();
        ajaxHelper.ajaxGetJson('/BatchIssue/GetBatchIssueByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    console.log('edit data', response);
                    empr_helper.selectedBill = response.traN_ID;

                    

                    $('#Code').val(response.traN_ID);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    if ($('#FinishItem').data('dxSelectBox') != null) {
                        $('#FinishItem').dxSelectBox('instance').dispose();
                    }
                    if ($('#Process').data('dxSelectBox') != null) {
                        $('#Process').dxSelectBox('instance').dispose();
                    }
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#CostPercent').val(response.cost);
                    //$('#NormalLossPercent').val(response.loss);
                    $('#BatchQuantity').val(response.bqty);
                    $('#FinishItem_Hidden').val(response.iteM_CODE);
                    $('#Process_Hidden').val(response.process);
                    $('#Unit_Hidden').val(response.unit);
                    $('#BatchNo').val(response.batchno);
                    $('#MFG_DATE').val(response.mfgdate.substring(0, 7));
                    console.log('MFG_DATE',response.expdate);
                    $('#EXP_DATE').val(response.expdate.substring(0, 7));
                    console.log("Unit")
                    console.log(response.unit)

                    var itemCode = parseInt(response.iteM_CODE);
                    var selectedBatch = response.batcH_NAME;
                    ajaxHelper.ajaxGetJson('/BatchIssue/GetBatchName?itemCode=' + itemCode, function (data) {
                        console.log('GetBatchName', data);
                        if (data.data.length > 0) {
                            empr_BatchIssue.BatchName = data.data;
                            
                            empr_BatchIssue.InitBatchNameDDL(selectedBatch);
                        }
                    }, false, true);
                    empr_BatchIssue.InitFinishItemsDDL(itemCode);
                    empr_BatchIssue.InitProcessesDDL(parseInt(response.process));
                    empr_BatchIssue.InitUnitDDL(parseInt(response.unit));

                    $('#BtnUpdate').prop('disabled', true);
                    $('#BatchQuantity').prop('disabled', true);
                    $('#BATCH_NAME').dxSelectBox('instance').option('disabled', true);
                    $('#FinishItem').dxSelectBox('instance').option('disabled', true);
                    $('#Process').dxSelectBox('instance').option('disabled', true);

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

                    var updatedDetailData = data.detail.data.map(item => {
                        var stockRecord = empr_BatchIssue.CurrentStock.find(s => s.itemId == item.iteM_CODE);

                        return {
                            ...item,
                            currentStock: stockRecord ? stockRecord.balance : 0
                        };
                    });

                    empr_BatchIssue.CreateGrid(updatedDetailData, true);
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

    GetBatchIssueDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/BatchIssue/GetBatchIssueDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_BatchIssue.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    Delete: function () {

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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/BatchIssue/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_BatchIssue.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
}