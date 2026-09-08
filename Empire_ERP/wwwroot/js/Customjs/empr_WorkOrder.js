var empr_WorkOrder = {
    totalCount: 0,
    rowsCount: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_WorkOrder.ResetForm();
            empr_WorkOrder.InitReportTypeDDL();
            empr_WorkOrder.InitAccountGroupGridBox(null);

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_WorkOrder.InitQuickSearchGrid();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_WorkOrder.GetWorkOrderByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/WorkOrder/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_WorkOrder.GetWorkOrderByCode(data.data.code);
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
                        if (empr_WorkOrder.ValidateInfo()) {
                            empr_WorkOrder.SaveInfo();
                        }
                    }
                } else {
                    if (empr_WorkOrder.ValidateInfo()) {
                        empr_WorkOrder.SaveInfo();
                    }
                }
            });

            $('body').on('click', '#BtnNew', function () {
                empr_WorkOrder.ResetForm();
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_WorkOrder.Delete();
            });

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_WorkOrder.GeneratePrintReport();
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_WorkOrder.GeneratePrintReport();
            });

            $('body').on('click', '#BtnBatchPick', function () {
                empr_WorkOrder.InitBatchPickGrid();
            });

            $('body').on('click', '#BtnAddBatch', function () {
                debugger;
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_WorkOrder.AddBatchToWorkOrder();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }

            // Ammar
            //const mfgInput = document.getElementById('MFG_DATE');
            //const expInput = document.getElementById('EXP_DATE');

            //expInput.addEventListener('change', function () {
            //    const mfgValue = mfgInput.value;
            //    const expValue = expInput.value;

            //    if (mfgValue && expValue && expValue < mfgValue) {
            //        empr_helper.notify("Expiry Date must be same or after Manufacturing Date.", 2);
            //        expInput.value = ""; // Clear invalid value
            //        expInput.focus();
            //    }
            //});
        });
    },
    GeneratePrintReport: function () {
        empr_WorkOrder.InitReportTypeDDL();
        let TRAN_ID = $('#Code').val();
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/WorkOrder/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
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
        ajaxHelper.ajaxGetJson("/WorkOrder/GetReportTypes", function (data) {
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
        empr_WorkOrder.CreateGrid([{ __KEY__: empr_WorkOrder.GenerateKey(36) }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #START_D').val('');
        $("#ACT_GROUP_hidden").val('');
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
        var dataSource = [
            {
                "Action": "",
                "dT_CODE": "abc123",
                "iteM_CODE": "xyz456",
                "qty": 42,
                "unit": "kg",
                "qtY2": 24,
                "baL_QTY": 168,
                "dT_DESC": "Lorem ipsum",
                "rate": 5.99,
                "amt": 251.58,
                "duE_DATE": "2024-04-15"
            },
            {
                "Action": "",
                "dT_CODE": "def789",
                "iteM_CODE": "uvw012",
                "qty": 73,
                "unit": "lbs",
                "qtY2": 12,
                "baL_QTY": 876,
                "dT_DESC": "Dolor sit amet",
                "rate": 8.49,
                "amt": 741.72,
                "duE_DATE": "2024-05-03"
            },
            {
                "Action": "",
                "dT_CODE": "ghi345",
                "iteM_CODE": "rst678",
                "qty": 91,
                "unit": "pcs",
                "qtY2": 36,
                "baL_QTY": 3276,
                "dT_DESC": "Consectetur adipiscing elit",
                "rate": 12.99,
                "amt": 4252.04,
                "duE_DATE": "2024-06-21"
            }
        ];
        empr_WorkOrder.CreateGrid([{ __KEY__: empr_WorkOrder.GenerateKey(36) }]);
        if ($('#FinishItem').data('dxSelectBox') != null) {
            $('#FinishItem').dxSelectBox('instance').dispose();
        }
        if ($('#Process').data('dxSelectBox') != null) {
            $('#Process').dxSelectBox('instance').dispose();
        }
        //empr_WorkOrder.InitFinishItemsDDL();
        //empr_WorkOrder.InitProcessesDDL();
        empr_WorkOrder.InitAccountGroupGridBox();
        //const mfgInput = document.getElementById('MFG_DATE');
        //const today = new Date();
        //const monthValue = today.toISOString().slice(0, 7); // Format: "yyyy-MM"
        //mfgInput.value = monthValue;
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
        if (dataSrc.length > 0) {
            empr_WorkOrder.rowsCount = dataSrc.length - 1;
            dataSrc.forEach(item => {
                if (
                    item.mfG_DATE == '1900-01-01' || item.mfG_DATE == '01-01-1900' || item.mfG_DATE == '01-Jan-1900' || item.mfG_DATE == '1/1/1900 12:00:00 AM' || item.mfG_DATE == '01/01/1900 12:00:00 AM' || item.mfG_DATE == '1/1/1900' ||
                    item.mfG_DATE == '2000-01-01' || item.mfG_DATE == '01-01-2000' || item.mfG_DATE == '01-Jan-2000' || item.mfG_DATE == '1/1/2000 12:00:00 AM' || item.mfG_DATE == '01/01/2000 12:00:00 AM' || item.mfG_DATE == '1/1/2000' ||
                    item.mfG_DATE == '00-01-01' || item.mfG_DATE == '01-01-00' || item.mfG_DATE == '01-Jan-00' || item.mfG_DATE == '1/1/00 12:00:00 AM' || item.mfG_DATE == '01/01/00 12:00:00 AM' || item.mfG_DATE == '1/1/00' || item.mfG_DATE == '01-Jan-00 12:00:00 AM'
                ) {
                    item.mfG_DATE = null;
                }
                if (
                    item.exP_DATE == '1900-01-01' || item.exP_DATE == '01-01-1900' || item.exP_DATE == '01-Jan-1900' || item.exP_DATE == '1/1/1900 12:00:00 AM' || item.exP_DATE == '01/01/1900 12:00:00 AM' || item.exP_DATE == '1/1/1900' ||
                    item.exP_DATE == '2000-01-01' || item.exP_DATE == '01-01-2000' || item.exP_DATE == '01-Jan-2000' || item.exP_DATE == '1/1/2000 12:00:00 AM' || item.exP_DATE == '01/01/2000 12:00:00 AM' || item.exP_DATE == '1/1/2000' ||
                    item.exP_DATE == '00-01-01' || item.exP_DATE == '01-01-00' || item.exP_DATE == '01-Jan-00' || item.exP_DATE == '1/1/00 12:00:00 AM' || item.exP_DATE == '01/01/00 12:00:00 AM' || item.exP_DATE == '1/1/00' || item.exP_DATE == '01-Jan-00 12:00:00 AM'
                ) {
                    item.exP_DATE = null;
                }
            });
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_WorkOrder.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_WorkOrder.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_WorkOrder.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_WorkOrder.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_WorkOrder.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_WorkOrder.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
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
                width: 150,
                allowSorting: false,
                lookup: {
                    dataSource: FinishItems,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.iteM_CODE = value;
                    var selectedItem = FinishItems.filter(u => u.key == value);
                    if (selectedItem.length > 0) {
                        newData.unit = selectedItem[0].unit;
                    }
                }
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 0;
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
                            newData.amt = newData.baL_QTY * rate;
                        }
                        else {
                            newData.baL_QTY = qty;
                            newData.amt = newData.baL_QTY * rate;
                        }
                    }
                }
            },
            {
                dataField: 'qtY2',
                caption: 'Qunatity 2',
                visible: CompCond == 2,
                setCellValue: function (newData, value, currentRowData) {
                    newData.qtY2 = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 0;
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
                            newData.amt = newData.baL_QTY * rate;
                        }
                        else {
                            newData.baL_QTY = qty;
                            newData.amt = newData.baL_QTY * rate;
                        }
                    }
                }
            },
            {
                dataField: 'baL_QTY',
                caption: 'Balance Quantity',
                visible: CompCond == 2,
                allowEditing: false,
                cellTemplate: function (container, options) {
                        var $cell = $("<div>").addClass("custom-cell");
                        var isChecked = options.data.chK1;
                        var quantity = options.data.baL_QTY;
                        if (quantity == 0 || quantity == '' || quantity == null || quantity == undefined) {
                            quantity = '';
                        }
                        console.log("isChecked:", isChecked);
                        var $checkbox = $("<input type='checkbox'>")
                            .addClass("chkCell")
                            .prop('checked', isChecked)
                            .on('change', function () {
                                var item = options.data;
                                var qty = parseFloat(item.qty) || 0;
                                var qtY2 = parseFloat(item.qtY2) || 0;
                                var rate = parseFloat(item.rate) || 0;
                                if (isNaN(qty)) {
                                    empr_helper.notify("Please enter the correct quantity.", 2);
                                }
                                if (isNaN(qtY2)) {
                                    empr_helper.notify("Please enter the correct quantity2.", 2);
                                }
                                if (!isNaN(qty) && !isNaN(qtY2)) {
                                    if (this.checked) {
                                        item.baL_QTY = qty * qtY2;
                                        item.chk = "1";
                                        item.amt = item.baL_QTY * rate;
                                    }
                                    else {
                                        item.baL_QTY = qty;
                                        item.chk = "0";
                                        item.amt = item.baL_QTY * rate;
                                    }
                                    item.chK1 = this.checked;
                                    container.find('span').text(item.baL_QTY);
                                }


                                var gridInstance = $('#DetailContainer').dxDataGrid('instance');
                                var dataSource = gridInstance.option("dataSource");
                                dataSource[options.rowIndex] = item;
                                gridInstance.option("dataSource", dataSource);
                            });

                        var $span = $('<span>' + quantity + '</span>');
                        $cell.append($checkbox).append($span);
                        container.append($cell);
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                allowEditing: true,
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            //{
            //    dataField: 'dT_DESC',
            //    caption: 'Description',
            //},

            {
                dataField: 'rate',
                caption: 'Rate',
                setCellValue: function (newData, value, currentRowData) {
                    newData.rate = value;
                    var balancedQTY = parseFloat(currentRowData.baL_QTY) || 0;
                    if (isNaN(newData.rate)) {
                        empr_helper.notify("Please enter the correct rate.", 2);
                    }
                    if (isNaN(balancedQTY)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }

                    if (!isNaN(balancedQTY) && !isNaN(newData.rate)) {
                        newData.amt = balancedQTY * newData.rate;
                    }
                }
            },
            {
                dataField: 'amt',
                caption: 'Amount',
                allowEditing: false,
            },
            //{
            //    dataField: 'mfG_DATE',
            //    caption: 'MFG Date',
            //    dataType: 'date',
            //    format: 'MM-yyyy',
            //    setCellValue: function (newData, value) {
            //        newData.mfG_DATE = value; // ammar add line
            //        if (value) {
            //            var date = new Date(value);
            //            var year = date.getFullYear();
            //            if (year < 100) {
            //                year += 2000;
            //            }
            //            var formattedDate = new Date(year, date.getMonth(), date.getDate());
            //            newData.mfG_DATE = formattedDate;
            //        } else {
            //            newData.mfG_DATE = value;
            //        }
            //    },
            //    cellTemplate: function (container, options) {
            //        var $dateCell = $('<div>').appendTo(container);
            //        var dateValue = options.value;
            //        if (dateValue) {
            //            var date = new Date(dateValue);
            //            var day = ("0" + date.getDate()).slice(-2);
            //            var month = ("0" + (date.getMonth() + 1)).slice(-2);
            //            var year = date.getFullYear()
            //            var formattedDate = day + '-' + month + '-' + year;
            //            $dateCell.text(formattedDate);
            //        }
            //    },
            //},
            //{
            //    dataField: 'exP_DATE',
            //    caption: 'EXP Date',
            //    dataType: 'date',
            //    format: 'MM-yyyy',
            //    setCellValue: function (newData, value) {
            //        newData.exP_DATE = value; // ammar add line
            //        if (value) {
            //            var date = new Date(value);
            //            var year = date.getFullYear();
            //            if (year < 100) {
            //                year += 2000;
            //            }
            //            var formattedDate = new Date(year, date.getMonth(), date.getDate());
            //            newData.exP_DATE = formattedDate;
            //        } else {
            //            newData.exP_DATE = value;
            //        }
            //    },
            //    cellTemplate: function (container, options) {
            //        var $dateCell = $('<div>').appendTo(container);
            //        var dateValue = options.value;
            //        if (dateValue) {
            //            var date = new Date(dateValue);
            //            var day = ("0" + date.getDate()).slice(-2);
            //            var month = ("0" + (date.getMonth() + 1)).slice(-2);
            //            var year = date.getFullYear()
            //            var formattedDate = day + '-' + month + '-' + year;
            //            $dateCell.text(formattedDate);
            //        }
            //    },
            //},
            //{
            //    dataField: 'batch',
            //    caption: 'Batch',
            //},
            {
                dataField: 'picK_ID',
                caption: 'Pick Id',
                visible: false
            }

        ];
        empr_helper.editableDxGridbindingForTransactions('#DetailContainer', col, dataSrc, "WorkOrder", "iteM_CODE", '');
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }
    },
    CloneRow: function (index) {
        debugger;
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_WorkOrder.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_WorkOrder.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_WorkOrder.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_WorkOrder.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
            }
        }
    },
    AddRow: function () {
        debugger;
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_WorkOrder.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_WorkOrder.GenerateKey(36) });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_WorkOrder.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_WorkOrder.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_WorkOrder.GenerateKey(36) });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    DeleteRow: function (index) {
        debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_WorkOrder.rowsCount -= 1;
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
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/WorkOrder/DeleteWorkOrderDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_WorkOrder.rowsCount -= 1;
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
    //    $.ajax({
    //        url: "WorkOrder/GetFinishItems",
    //        type: "GET",
    //        success: function (response) {
    //            empr_WorkOrder.BindDxDDL("FinishItem", response.data, selectedValue, "key", "value", "Select", function (d) {
    //                $('#FinishItem_Hidden').val(d.value)
    //                if (d.value == null) {
    //                    $('#FinishItem_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},
    //InitProcessesDDL: function (selectedValue) {
    //    $.ajax({
    //        url: "WorkOrder/GetProcesses",
    //        type: "GET",
    //        success: function (response) {
    //            empr_WorkOrder.BindDxDDL("Process", response.data, selectedValue, "key", "value", "Select", function (d) {

    //                $('#Process_Hidden').val(d.value)
    //                if (d.value == null) {
    //                    $('#Process_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},
    BindDxDDL: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },
    InitQuickSearchGrid: function () {
        empr_WorkOrder.GetWorkOrders();
    },
    GetWorkOrders: function () {
        ajaxHelper.ajaxGetJson('/WorkOrder/GetWorkOrders', function (data) {
            if (data.msgType == 1) {
                empr_WorkOrder.CreateQuickSearchGrid(data.data);
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
                console.log(options);
                    debugger
                    if (Permissions != "Admin" && !Permissions.r_PRINT) {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            { dataField: 'traN_ID', caption: 'Code', },
            { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'astatus', caption: 'Status', },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'starT_D', caption: 'Start Date', },
            { dataField: 'starT_E', caption: 'End Date', },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "WorkOrderQS");
    },
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var START_D = $("#START_D").val();
        var START_E = $('#START_E').val();
        var REMARKS = $("#REMARKS").val();
        var DEP = $("#ACT_GROUP_hidden").val();

        
        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            ASTATUS: ASTATUS,
            VOUCHER_NO: VOUCHER_NO,
            START_D: START_D,
            START_E: START_E,
            REMARKS: REMARKS,
            DEP: DEP,
        }
        // validate data
        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }
        if (empr_WorkOrder.rowsCount == detailRecords.length) {
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
        var data = empr_WorkOrder.GetDataToSave();

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
        console.log(data.Detail);
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
            if (!(item.exP_DATE == "" || item.exP_DATE == null || item.exP_DATE == undefined)) {
                item.exP_DATE = empr_helper.PrepareDate(item.exP_DATE);
            }

            if (!(item.mfG_DATE == "" || item.mfG_DATE == null || item.mfG_DATE == undefined)) {
                item.mfG_DATE = empr_helper.PrepareDate(item.mfG_DATE);
            }
        });

        return valid;
    },
    SaveInfo: function () {
        debugger;
        var dataModel = empr_WorkOrder.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/WorkOrder/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                empr_WorkOrder.GetWorkOrderDetailsByCode($('#Code').val());
                $('#BtnDelete').show();
            }
        }, false, true);
    },
    GetWorkOrderByCode: function (code) {
        ajaxHelper.ajaxGetJson('/WorkOrder/GetWorkOrderByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                console.log(masterData);
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
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#CostPercent').val(response.cost);
                    $('#NormalLossPercent').val(response.loss);
                    $('#START_D').val(response.starT_D);
                    $('#FinishItem_Hidden').val(response.iteM_CODE);
                    $('#Process_Hidden').val(response.process);
                    $('#BatchNo').val(response.batchno);
                    $('#START_E').val(response.starT_E); 
                    $('#REMARKS').val(response.remarks);
                    //empr_WorkOrder.InitFinishItemsDDL(parseInt(response.iteM_CODE));
                    //empr_WorkOrder.InitProcessesDDL(parseInt(response.process));
                    empr_WorkOrder.InitAccountGroupGridBox(response.dep);
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
                    empr_WorkOrder.CreateGrid(data.detail.data);
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
    GetWorkOrderDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/WorkOrder/GetWorkOrderDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_WorkOrder.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/WorkOrder/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_WorkOrder.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    InitBatchPickGrid: function () {
        var data = empr_WorkOrder.GetDataToSave();
        console.log(data.Master.SDATE);
        if (data.Master.PROCESS == "" || data.Master.PROCESS == null || data.Master.PROCESS == undefined) {
            empr_helper.notify("Please select the process first.", 2);
        }
        else {
            empr_WorkOrder.GetBatchDetailByProcess(data.Master.PROCESS);
        }
    },
    GetBatchDetailByProcess: function (process) {
        ajaxHelper.ajaxGetJson('/WorkOrder/GetBatchDetailByProcess?process=' + process, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_WorkOrder.CreateBatchGrid(data.data);
                    $('#SodaPickModal').modal('show');
                } else {
                    empr_helper.notify("No soda found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateBatchGrid: function (dataSrc) {
        var col = [
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'process', caption: 'Process', allowEditing: false, },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
            { dataField: 'batch', caption: 'Batch', allowEditing: false, },
            { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false },
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
            { dataField: 'qty', caption: 'Batch Qty', allowEditing: false, },
            { dataField: 'baL_QTY', caption: 'B.Qty', allowEditing: false, visible: false },
            { dataField: 'unit', caption: 'Unit', allowEditing: false, visible: false },
            { dataField: 'mfG_DATE', caption: 'MFG Date', dataType: 'date', allowEditing: false, format: 'MM-yyy' },
            { dataField: 'exP_DATE', caption: 'EXP Date', dataType: 'date', allowEditing: false, format: 'MM-yyy' },
            //{ dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "WorkOrderPick", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    AddBatchToWorkOrder: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_WorkOrder.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_WorkOrder.SetData(selectedSodas);
                    var finalData = existingData.concat(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_WorkOrder.SetData(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var data = empr_WorkOrder.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                var finalData = existingData.concat(selectedSodas);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
            }

            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },
    SetData: function (dataSource) {
        $.each(dataSource, function (index, item) {
            item.chK1 = false;
            item.chk = "0";
            if (item.unit != '') {
                var selectedUnit = Units.filter(u => u.key == item.unit);
                if (selectedUnit.length > 0) {
                    item.qtY2 = selectedUnit[0].qty;
                }
            }
            else {
                item.qtY2 = 0;
            }

            var qty = parseFloat(item.qty) || 0;
            var qtY2 = parseFloat(item.qtY2) || 1;
            var rate = parseFloat(item.rate) || 0;
            var rT_TYPE = parseFloat(item.rT_TYPE) || 0;
            if (!isNaN(qty) && !isNaN(qtY2)) {
                if (item.chK1) {
                    item.baL_QTY = qty * qtY2;
                    if (empr_WorkOrder.Branch_RT_TYPE == 'Y') {
                        item.amt = item.baL_QTY * rate;
                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                        if (perRate > -1 && perRate != 'Infinity') {
                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                        }
                    }

                    if (empr_WorkOrder.Branch_RT_TYPE == 'N') {
                        item.amt = qty * rate;
                    }
                    item.neT_AMT = item.amt;
                }
                else {
                    item.baL_QTY = qty;
                    if (empr_WorkOrder.Branch_RT_TYPE == 'Y') {
                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                        if (perRate > -1 && perRate != 'Infinity') {
                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                        }
                    }

                    if (empr_WorkOrder.Branch_RT_TYPE == 'N') {
                        item.amt = qty * rate;
                    }
                    item.neT_AMT = item.amt;
                }
            }

            item.__KEY__ = empr_WorkOrder.GenerateKey(36);
        });

        return dataSource;
    },
    InitAccountGroupGridBox: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "AccountGroup/GetAccountGroups",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.code == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].code;
                        $('#ACT_GROUP_hidden').val(selectedObject[0].code);
                        $('#displayExprAccGroup').val(selectedObject[0].descr);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#accountgroupgrdiBox").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "code",
                    displayExpr: function (item) {
                        return item ? `${item.descr}` : "Select a value...";
                    },
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.code === e.value);
                            if (selectedData) {
                                $('#ACT_GROUP_hidden').val(selectedData.code);
                                $('#displayExprAccGroup').val(selectedData.descr);
                            }
                        } else {
                            $('#ACT_GROUP_hidden').val('');
                            $('#displayExprAccGroup').val('');
                        }
                    },
                    onFocusIn: function (e) {
                        if (!isProgrammaticOpen) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 100);
                        }
                    },
                    onOpened: function (e) {
                        if (!currentSearchTerm) {
                            if (gridInstance) {
                                gridInstance.getDataSource().filter(null);
                                gridInstance.refresh();
                            }
                        }

                        setTimeout(() => {
                            const input = e.component._$element.find(".dx-texteditor-input").first();
                            input.focus();
                            if (input.val()) {
                                input.select();
                            }
                        }, 50);
                    },
                    onInput: function (e) {
                        currentSearchTerm = e.event.target.value;

                        if (!e.component.option("opened")) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 100);
                        }

                        if (gridInstance) {
                            applyGridFilter(gridInstance, currentSearchTerm);
                        }
                    },
                    contentTemplate: function (e) {
                        gridInstance = $("<div>").dxDataGrid({
                            dataSource: new DevExpress.data.DataSource({
                                store: Datasource,
                                key: "code"
                            }),
                            columns: [
                                {
                                    dataField: "code",
                                    caption: "Code",
                                    width: '60px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "descr",
                                    caption: "Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "grouP_TYPE",
                                    caption: "Control Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                }
                            ],
                            selection: {
                                mode: "single",
                                showCheckBoxesMode: "always"
                            },
                            hoverStateEnabled: true,
                            height: 300,
                            keyboardNavigation: {
                                enabled: true,
                                enterKeyAction: "select",
                                editOnKeyPress: true
                            },
                            onSelectionChanged: function (selectedItems) {
                                const selected = selectedItems.selectedRowsData[0];
                                if (selected) {
                                    e.component.option("value", selected.code);
                                    e.component.close();

                                    // Update hidden and display fields
                                    $('#ACT_GROUP_hidden').val(selected.code);
                                    $('#displayExprAccGroup').val(selected.descr);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].code], false);
                                    }
                                }
                            }
                        }).dxDataGrid("instance");

                        gridInstance.element().on('click', function (event) {
                            event.stopPropagation();
                        });

                        return gridInstance.element();
                    }
                });

                function applyGridFilter(grid, searchTerm) {
                    const dataSource = grid.getDataSource();
                    if (searchTerm) {
                        dataSource.filter([
                            ["descr", "contains", searchTerm],
                            "or",
                            ["code", "contains", searchTerm],
                            "or",
                            ["grouP_TYPE", "contains", searchTerm]
                        ]);
                    } else {
                        dataSource.filter(null);
                    }
                    dataSource.load();
                }

                function highlightText(container, value) {
                    if (!value) return;

                    const text = value.toString();
                    if (!currentSearchTerm || !text.toLowerCase().includes(currentSearchTerm.toLowerCase())) {
                        container.text(text);
                        return;
                    }

                    const regex = new RegExp(currentSearchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), "gi");
                    const highlighted = text.replace(regex, match =>
                        `<span style="background-color: #ffeb3b; font-weight: bold;">${match}</span>`
                    );
                    container.html(highlighted);
                }

                // Handle clear button
                $(document).on("dxclick", "#accountgroupgrdiBox .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#ACT_GROUP_hidden').val('');
                    $('#displayExprAccGroup').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
            }
        });
    },
}