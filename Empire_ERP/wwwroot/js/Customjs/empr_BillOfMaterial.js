var empr_BillOfMaterial = {
    totalCount: 0,
    rowsCount: 0,
    itemCode: 0,
    processCode: 0,
    InitEvents: function () {
        $(document).ready(function () {
            //console.log('localStorage', localStorage);
            //localStorage.setItem('BillOfMaterial', '');
            empr_BillOfMaterial.ResetForm();

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_BillOfMaterial.InitQuickSearchGrid();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                empr_helper.selectedBill = id;
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_BillOfMaterial.GetBillOfMaterialByCode(id);
            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var date = $(this).attr("reportdate");
                var batchName = $(this).attr("reportitemcode");
                //empr_BillOfMaterial.processCode = parseInt($(this).attr("reportprocesscode"));
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
                    $('#updated_Date').val(date);
                    empr_helper.selectedBill = id;
                    //empr_BillOfMaterial.InitCopiedFinishItemsDDL(empr_BillOfMaterial.itemCode);
                    //empr_BillOfMaterial.InitCopiedProcessesDDL(empr_BillOfMaterial.processCode);
                    debugger;
                    $('#updatedBatchName').val(batchName);
                    $('#CopyViewModalBOM').modal('show');
                });
            });


            $('body').on('click', '#saveCopiedRecord', function () {
                var abc = {
                    tran_ID: empr_helper.selectedBill,
                    v_DATE: $('#updated_Date').val(),
                    batcH_NAME: $('#updatedBatchName').val(),
                    //finish_ITEM: empr_BillOfMaterial.itemCode,
                    //process: empr_BillOfMaterial.processCode
                }
                console.log('copy data before',abc);
                ajaxHelper.ajaxPostJsonData(abc, "/BillOfMaterial/CopyRecord", function (data) {
                    console.log('saveCopiedRecord',data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_BillOfMaterial.GetBillOfMaterialByCode(data.data.code);
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
                        if (empr_BillOfMaterial.ValidateInfo()) {
                            empr_BillOfMaterial.SaveInfo();
                        }
                    }
                } else {
                    if (empr_BillOfMaterial.ValidateInfo()) {
                        empr_BillOfMaterial.SaveInfo();
                    }
                }
            });

            $('body').on('click', '#BtnNew', function () {
                empr_BillOfMaterial.ResetForm();
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_BillOfMaterial.Delete();
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_BillOfMaterial.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    //GeneratePrintReport: function () {
    //    empr_BillOfMaterial.InitReportTypeDDL();
    //    let TRAN_ID = $('#Code').val();
    //    let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
    //    if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
    //        empr_helper.notify("Please open the bill in edit mode.", 2);
    //        return;
    //    }
    //    var dataModel = {
    //        TRAN_ID: TRAN_ID,
    //        MD_ID: MD_ID,
    //    }
    //    ajaxHelper.ajaxPostJsonData(dataModel, "/BillOfMaterial/GetPrintReport", function (data) {
    //        if (data.msgType == 1) {
    //            $('#ModalBody').empty();
    //            setTimeout(function () {
    //                $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
    //                $('#ShowReportModal').show();
    //                $('#ShowReportModal').modal('show');
    //            }, 100);
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},
    GeneratePrintReport: function () {
        debugger
        empr_BillOfMaterial.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = 0;
        let reportName = "";
        //let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        let selectedItem = $('#ReportType').dxSelectBox('option', 'selectedItem');
        if (selectedItem) {
            MD_ID = selectedItem.mD_ID;
            reportName = selectedItem.reporT_NAME;
        }
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
            REPORT_NAME: reportName,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/BillOfMaterial/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ReportType').dxSelectBox('instance').option('value', MD_ID);
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/BillOfMaterial/GetReportTypes", function (data) {
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
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
        
        empr_BillOfMaterial.CreateGrid([{ __KEY__: empr_BillOfMaterial.GenerateKey(36) }]);
        if ($('#FinishItem').data('dxSelectBox') != null) {
            $('#FinishItem').dxSelectBox('instance').dispose();
        }
        if ($('#Process').data('dxSelectBox') != null) {
            $('#Process').dxSelectBox('instance').dispose();
        }
        empr_BillOfMaterial.InitFinishItemsDDL();
        empr_BillOfMaterial.InitProcessesDDL();
        empr_BillOfMaterial.InitReportTypeDDL();
    },
    CreateGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_BillOfMaterial.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_BillOfMaterial.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_BillOfMaterial.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_BillOfMaterial.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_BillOfMaterial.CloneRow(`+ options.rowIndex +`)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_BillOfMaterial.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_BillOfMaterial.DeleteRow(`+ options.rowIndex +`)" title="Delete"><i class="fa fa-trash"></i></a>
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
                width: 450,
                allowSorting: false,
                alignment: 'center',
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.iteM_CODE = value;
                    var selectedItem = Items.filter(u => u.key == value);
                    if (selectedItem.length > 0) {
                        newData.unit = selectedItem[0].unit;
                    }
                }
            },
            {
                dataField: 'qty',
                caption: 'Qty',
                width: 150,
                alignment: 'right',
                dataType: 'number', 
                format: "#,##0.##########",
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 0;
                    //var rate = parseFloat(currentRowData.rate) || 0;
                    var loss = parseFloat(currentRowData.loss) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }

                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            //newData.amt = newData.baL_QTY * rate;
                        }
                        else {
                            newData.baL_QTY = qty;
                            //newData.amt = newData.baL_QTY * rate;
                            newData.losS_WEIGHT = (loss * qty) / 100;
                        }
                    }
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                width: 150,
                allowEditing: false,
                alignment: 'center',
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
                //format: { type: 'fixedPoint', precision: 2 },
                dataType: 'number',
                format: "#,##0.##########",
                alignment: 'right',
                width: 150,
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
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "BillOfMaterial", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        setTimeout(function () {
            var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
            $('#DetailContainer').dxDataGrid('instance').focus(nextElement);  
        }, 1500);
    },
    CloneRow: function (index) {
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_BillOfMaterial.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_BillOfMaterial.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_BillOfMaterial.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_BillOfMaterial.GenerateKey(36);
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
                empr_BillOfMaterial.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_BillOfMaterial.GenerateKey(36) });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_BillOfMaterial.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_BillOfMaterial.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_BillOfMaterial.GenerateKey(36) });
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
                    empr_BillOfMaterial.rowsCount -= 1;
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
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/BillOfMaterial/DeleteBillOfMaterialDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_BillOfMaterial.rowsCount -= 1;
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
    //InitFinishItemsDDL: function (selectedValue) {
    //    console.log(selectedValue);
    //    $.ajax({
    //        url: "BillOfMaterial/GetFinishItems",
    //        type: "GET",
    //        success: function (response) {
    //            empr_BillOfMaterial.BindDxDDL("FinishItem", response.data, selectedValue, "key", "value", "Select", function (d) {
    //                $('#FinishItem_Hidden').val(d.value)
    //                if (d.value == null) {
    //                    $('#FinishItem_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},

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
            searchTimeout: 500
        });
    },

    //InitCopiedFinishItemsDDL: function (selectedValue) {
    //    console.log("Value:", selectedValue, "| Type:", typeof selectedValue);
    //    $.ajax({
    //        url: "BillOfMaterial/GetFinishItems",
    //        type: "GET",
    //        success: function (response) {
    //            empr_BillOfMaterial.BindDxDDL("copiedFinishItem", response.data, selectedValue, "key", "value", "Select", function (d) {
    //                empr_BillOfMaterial.itemCode = d.value
    //                if (d.value == null) {
    //                    $('#copiedFinishItem_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},
    //InitProcessesDDL: function (selectedValue) {
    //    $.ajax({
    //        url: "BillOfMaterial/GetProcesses",
    //        type: "GET",
    //        success: function (response) {
    //            empr_BillOfMaterial.BindDxDDL("Process", response.data, selectedValue, "key", "value", "Select", function (d) {

    //                $('#Process_Hidden').val(d.value)
    //                if (d.value == null) {
    //                    $('#Process_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},

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

    //InitCopiedProcessesDDL: function (selectedValue) {
    //    $.ajax({
    //        url: "BillOfMaterial/GetProcesses",
    //        type: "GET",
    //        success: function (response) {
    //            empr_BillOfMaterial.BindDxDDL("copiedProcess", response.data, selectedValue, "key", "value", "Select", function (d) {
    //                empr_BillOfMaterial.processCode = d.value
    //                if (d.value == null) {
    //                    $('#copiedProcess_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},
    BindDxDDL: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },
    InitQuickSearchGrid: function () {
        empr_BillOfMaterial.GetBillOfMaterials();
    },
    GetBillOfMaterials: function () {
        ajaxHelper.ajaxGetJson('/BillOfMaterial/GetBillOfMaterials', function (data) {
            if (data.msgType == 1) {
                empr_BillOfMaterial.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        console.log(dataSrc);
        var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger;

                    $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                       <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportid=${options.data.traN_ID} reportdate=${options.data.v_DATE} reportitemcode='${options.data.batcH_NAME}' title="COPY"><i class="fa fa-copy"></i></a>
                       </div>`).appendTo(container);
                }
            },
            { dataField: 'traN_ID', caption: 'Code', },
            { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'astatus', caption: 'Status', },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'iteM_NAME', caption: 'Item Name', },
            { dataField: 'batcH_NAME', caption: 'Batch Name', },
            { dataField: 'cost', caption: 'Cost%', },
            { dataField: 'loss', caption: 'Normal Loss%', },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "BillOfMaterialQS");
    },
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var REF = $("#REF").val();
        var BATCH_NAME = $("#BATCH_NAME").val();
        var COSTPERCENT = $("#CostPercent").val();
        //var NORMALLOSSPERCENT = $("#NormalLossPercent").val();
        var ITEM_CODE = $('#FinishItem').dxSelectBox('option', 'value');
        var PROCESS = $('#Process').dxSelectBox('option', 'value'); 
        //var ITEM_CODE = $('#FinishItem_Hidden').val();
        //var PROCESS = $('#Process_Hidden').val();
        var BQTY = $('#BatchQuantity').val();
        var REMARKS = $("#REMARKS").val();
        
        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            ASTATUS: ASTATUS,
            ITEM_CODE: ITEM_CODE,
            REF: REF,
            BATCH_NAME: BATCH_NAME,
            COST: COSTPERCENT,
            //LOSS: NORMALLOSSPERCENT,
            REMARKS: REMARKS,
            PROCESS: PROCESS,
            BQTY: BQTY,
            ASTATUS: ASTATUS
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
        if (empr_BillOfMaterial.rowsCount == detailRecords.length) {
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
        var data = empr_BillOfMaterial.GetDataToSave();

        //if (data.Master.ITEM_CODE == '') {
        //    empr_helper.notify("Item is required.", 2);
        //    valid = false;
        //    return valid;
        //}

        if (data.Master.ITEM_CODE == undefined) {
            empr_helper.notify("Finish Item is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.BATCH_NAME == '') {
            empr_helper.notify("Batch Name is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.PROCESS == undefined) {
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
                empr_helper.notify("Please select item at line No " + (index+1), 2);
                valid = false;
                return valid;
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity at line No " + (index + 1), 2);
                valid = false;
                return valid;
            }

            if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
                empr_helper.notify("Please enter correct item quantity2 at line No " + (index + 1), 2);
                valid = false;
                return valid;
            }
        });

        return valid;
    },
    SaveInfo: function () {
        var dataModel = empr_BillOfMaterial.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/BillOfMaterial/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                empr_BillOfMaterial.GetBillOfMaterialDetailsByCode($('#Code').val());
                $('#BtnDelete').show();
            }
        }, false, true);
    },
    GetBillOfMaterialByCode: function (code) {
        ajaxHelper.ajaxGetJson('/BillOfMaterial/GetBillOfMaterialByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    if ($('#FinishItem').data('dxSelectBox') != null) {
                        $('#FinishItem').dxSelectBox('instance').dispose();
                    }
                    if ($('#Process').data('dxSelectBox') != null) {
                        $('#Process').dxSelectBox('instance').dispose();
                    }
                    $('#REF').val(response.ref);
                    $('#BATCH_NAME').val(response.batcH_NAME);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#CostPercent').val(response.cost);
                    //$('#NormalLossPercent').val(response.loss);
                    $('#BatchQuantity').val(response.bqty);
                    $('#FinishItem_Hidden').val(response.iteM_CODE);
                    $('#Process_Hidden').val(response.process);
                    empr_BillOfMaterial.InitFinishItemsDDL(parseInt(response.iteM_CODE));
                    empr_BillOfMaterial.InitProcessesDDL(parseInt(response.process));
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
                    empr_BillOfMaterial.CreateGrid(data.detail.data);
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
    GetBillOfMaterialDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/BillOfMaterial/GetBillOfMaterialDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_BillOfMaterial.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/BillOfMaterial/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_BillOfMaterial.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
}