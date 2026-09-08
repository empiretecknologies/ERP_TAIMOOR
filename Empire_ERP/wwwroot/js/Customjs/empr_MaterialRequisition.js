var empr_MaterialRequisition = {
    totalCount: 0,
    rowsCount: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_MaterialRequisition.InitQuickSearchGrid();
            empr_MaterialRequisition.InitReportTypeDDL();
            empr_MaterialRequisition.ResetForm();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_MaterialRequisition.GetMaterialRequisitionByCode(data.traN_ID);
                }
            });
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                empr_helper.selectedBill = id;
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_MaterialRequisition.GetMaterialRequisitionByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/MaterialRequisition/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_MaterialRequisition.GetMaterialRequisitionByCode(data.data.code);
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
                        if (empr_MaterialRequisition.ValidateMainInfo()) {
                            empr_MaterialRequisition.Save();
                        }
                    }
                } else {
                    if (empr_MaterialRequisition.ValidateMainInfo()) {

                        empr_MaterialRequisition.Save();
                    }
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_MaterialRequisition.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_MaterialRequisition.ResetForm();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_MaterialRequisition.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_MaterialRequisition.GeneratePrintReport();
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_MaterialRequisition.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },

    CreateGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_MaterialRequisition.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_MaterialRequisition.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_MaterialRequisition.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_MaterialRequisition.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_MaterialRequisition.CloneRow(`+ options.rowIndex +`)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_MaterialRequisition.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_MaterialRequisition.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                //width: 10,
                allowSorting: false,
                fixed: true,
                fixedPosition: "left",
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'

                },
                //validationRules: [{ type: 'required' }],
                //editCellTemplate: empr_MaterialRequisition.dropDownBoxEditorTemplate,
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                        }
                        else {
                            newData.baL_QTY = qty;
                        }
                    }
                    
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                //width: 10,
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'qtY2',
                caption: 'Qunatity 2',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qtY2 = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                        }
                        else {
                            newData.baL_QTY = qty;
                        }
                    }
                }
            },
            {
                dataField: 'baL_QTY',
                caption: 'Balance Quantity',
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
                                }
                                else {
                                    item.baL_QTY = qty;
                                    item.chk = "0";
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
                dataField: 'dT_DESC',
                caption: 'Description',
            },
            {
                dataField: 'color',
                caption: 'Color',
                //width: 10,
                lookup: {
                    dataSource: Colors,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'size',
                caption: 'Size',
                //width: 10,
                lookup: {
                    dataSource: Sizes,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'grade',
                caption: 'Grade',
                //width: 10,
                lookup: {
                    dataSource: Grades,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'warehouse',
                caption: 'Warehouse',
                //width: 10,
                lookup: {
                    dataSource: Warehouses,
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'priority',
                caption: 'Priority',
                //width: 10,
                lookup: {
                    dataSource: [
                        { key: 'N', value: 'Normal' },
                        { key: 'U', value: 'Urgent' },
                        { key: 'M', value: 'Most Urgent' },
                    ],
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'deL_DATE',
                caption: 'Delivery Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
            },
            {
                dataField: 'reQ_TYPE',
                caption: 'Request Type',
                lookup: {
                    dataSource: [
                        { key: 'E', value: 'External' },
                        { key: 'I', value: 'Internal' },
                    ],
                    displayExpr: 'value',
                    valueExpr: 'key'
                }
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "MaterialRequisition", "iteM_CODE");
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
        //debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_MaterialRequisition.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_MaterialRequisition.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_MaterialRequisition.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_MaterialRequisition.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
            }
        }
    },

    AddRow: function () {
        //debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_MaterialRequisition.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_MaterialRequisition.GenerateKey(36), priority: 'N' });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_MaterialRequisition.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_MaterialRequisition.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_MaterialRequisition.GenerateKey(36), priority: 'N' });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },

    DeleteRow: function (index, dtCode) {
        //debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_MaterialRequisition.rowsCount -= 1;
                    gridInstance.saveEditData();
                }
                else {
                    var availableRows = dataSource.filter(x => x.dT_CODE > 0);
                    if (availableRows.length > 1) {
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
                        ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/MaterialRequisition/DeleteMaterialRequisitionDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_MaterialRequisition.rowsCount -= 1;
                                gridInstance.saveEditData();
                            }
                        }, false, true);
                    });
                    } else {
                        empr_helper.notify("You are not allowed to delete the last row.", 2);
                    }
                }
            }
            else {
                empr_helper.notify("You are not allowed to delete the last row.", 2);
            }
        }
    },

    dropDownBoxEditorTemplate: function(cellElement, cellInfo) {
        return $('<div>').dxDropDownBox({
            dropDownOptions: { width: 500 },
            //dataSource: new DevExpress.data.DataSource({
            //    //load: function (loadOptions) {
            //    //    return $.ajax({
            //    //        url: "/MaterialRequisition/GetItems",
            //    //        method: "GET"
            //    //    });
            //    //}
            //    load(loadOptions) {
            //        const deferred = $.Deferred();
            //        $.ajax({
            //            url: "/MaterialRequisition/GetItems",
            //            method: "GET",
            //            success(result) {
            //                deferred.resolve(result, {
            //                    totalCount: result.length,
            //                    summary: result.summary,
            //                    groupCount: result.groupCount,
            //                });
            //            },
            //            error() {
            //                deferred.reject('Data Loading Error');
            //            },
            //            timeout: 5000,
            //        });

            //        return deferred.promise();
            //    },
            //}),
            dataSource: [
                { key: 1, value: 'Value 1' },
                { key: 2, value: 'Value 2' },
                { key: 3, value: 'Value 3' }
            ],
            value: cellInfo.value,
            valueExpr: 'value',
            keyExpr: 'key',
            displayExpr: 'value',
            inputAttr: { 'aria-label': 'Owner' },
            contentTemplate(e) {
                return $('<div>').dxDataGrid({
                    //dataSource: new DevExpress.data.DataSource({
                    //    //load: function (loadOptions) {
                    //    //    return $.ajax({
                    //    //        url: "/MaterialRequisition/GetItems",
                    //    //        method: "GET"
                    //    //    });
                    //    //}
                    //    load(loadOptions) {
                    //        const deferred = $.Deferred();
                    //        $.ajax({
                    //            url: "/MaterialRequisition/GetItems",
                    //            method: "GET",
                    //            success(result) {
                    //                deferred.resolve(result, {
                    //                    totalCount: result.length,
                    //                    summary: result.summary,
                    //                    groupCount: result.groupCount,
                    //                });
                    //            },
                    //            error() {
                    //                deferred.reject('Data Loading Error');
                    //            },
                    //            timeout: 5000,
                    //        });

                    //        return deferred.promise();
                    //    },
                    //}),
                    dataSource: [
                        { key: 1, value: 'Value 1' },
                        { key: 2, value: 'Value 2' },
                        { key: 3, value: 'Value 3' }
                    ],
                    remoteOperations: true,
                    columns: [
                        { dataField: "key", caption: "Code" },
                        { dataField: "value", caption: "Name" },
                        { dataField: "name", caption: "Control Name" }
                    ],
                    hoverStateEnabled: true,
                    scrolling: { mode: 'virtual' },
                    height: 250,
                    selection: { mode: 'single' },
                    selectedRowKeys: [cellInfo.value],
                    keyExpr: 'key',
                    onSelectionChanged(selectionChangedArgs) {
                        e.component.option('value', selectionChangedArgs.selectedRowKeys[0]);
                        cellInfo.setValue(selectionChangedArgs.selectedRowKeys[0]);
                        if (selectionChangedArgs.selectedRowKeys.length > 0) {
                            e.component.close();
                        }
                    },
                });
            },
        });
    },

    InitDepartmentDDL: function (_selectedValue) {
        $.ajax({
            url: 'MaterialRequisition/GetDepartments',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#department_hidden").val(selectedvalue);
                        $("#displayExpr_department").val(selectedobj[0].value);
                    }
                }
                
                empr_MaterialRequisition.BindDxGridBoxDdl('#DEP_ID', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_department', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        var rate = selectedvalue.selectedRowsData[0]['rate'];
                        $('#department_hidden').val(key);
                        $('#displayExpr_department').val(value);
                    }
                    else {
                        $('#department_hidden').val('');
                        $('#displayExpr_department').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    InitQuickSearchGrid: function () {
        empr_MaterialRequisition.GetMaterialRequisitions();
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/MaterialRequisition/GetReportTypes", function (data) {
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

    GetMaterialRequisitions: function () {
        ajaxHelper.ajaxGetJson('/MaterialRequisition/GetMaterialRequisitions', function (data) {
            if (data.msgType == 1) {
                empr_MaterialRequisition.CreateQuickSearchGrid(data.data);
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
                    if (Permissions != "Admin" && !Permissions.r_PRINT) {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.id} title="PRINT"><i class="fa fa-print"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.id} title="COPY"><i class="fa fa-copy"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            { dataField: 'id', caption: 'Code', },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Transaction #', },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'emP_ID', caption: 'Employee', },
            { dataField: 'remarks', caption: 'Remarks', },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "MaterialRequisitionQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    GetMaterialRequisitionByCode: function (code) {
        ajaxHelper.ajaxGetJson('/MaterialRequisition/GetMaterialRequisitionByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.id);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    empr_MaterialRequisition.InitDepartmentDDL(response.deP_ID);
                    $('#EMP_ID').val(response.emP_ID);
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
                    empr_MaterialRequisition.CreateGrid(data.detail.data);
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

    GetMaterialRequisitionDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/MaterialRequisition/GetMaterialRequisitionDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_MaterialRequisition.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GetDataToSave: function () {

        var ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var DEP_ID = $("#department_hidden").val();
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var EMP_ID = $("#EMP_ID").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            DEP_ID: DEP_ID,
            REF: REF,
            REMARKS: REMARKS,
            EMP_ID: EMP_ID,
            ASTATUS: ASTATUS
        }
        //var Detailrecords = [];
        ////debugger;
        //$('#DetailContainer').dxDataGrid('instance').getVisibleRows().forEach(function (row) {
        //    var record = {
        //        DetailId: row.data['id'] ?? 0,
        //        item: row.data['iteM_CODE'],
        //        quantity: row.data['qty'],
        //        unit: row.data['unit'],
        //        quantity2: row.data['qtY2'],
        //        Balance: row.data['baL_QTY'],
        //        Description: row.data['dT_DESC'],
        //        Color: row.data['color'],
        //        Size: row.data['size'],
        //        Grade: row.data['grade'],
        //        wharehouse: row.data['warehouse'],
        //        priority: row.data['prioirty'],
        //        deliverydate: row.data['deL_DATE'],
        //        requesttype: row.data['reQ_TYPE']
        //    };
        //    Detailrecords.push(record);
        //});
        //var modelRecord = {
        //    Master: masterRecord,
        //    Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource")
        //};
        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else{
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }
        if (empr_MaterialRequisition.rowsCount == detailRecords.length) {
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

    GeneratePrintReport: function () {
        empr_MaterialRequisition.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/MaterialRequisition/GetPrintReport", function (data) {
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

    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_MaterialRequisition.GetDataToSave();

        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.DEP_ID == '') {
            empr_helper.notify("Please select department.", 2);
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
            // Check if Item_Code or Quantity is empty
            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
                empr_helper.notify("Please select item at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty ItemCode.");
                // Do something here, such as marking the item or taking appropriate action
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
                // Do something here, such as marking the item or taking appropriate action
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
                // Do something here, such as marking the item or taking appropriate action
            }

            if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
                empr_helper.notify("Please enter correct item quantity2 at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity2.");
                // Do something here, such as marking the item or taking appropriate action
            }

        });

        return valid;
    },

    Save: function () {
        //debugger;
        var dataModel = empr_MaterialRequisition.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/MaterialRequisition/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    $('.vHide').show();
                }
                if (dataClear == 1) {
                    empr_MaterialRequisition.GetMaterialRequisitionDetailByCode($('#Code').val());
                    $('#BtnDelete').show();
                }
                else {
                    empr_MaterialRequisition.ResetForm();
                }
                
            }
        }, false, true);
    },

    ResetForm: function () {

        //$('.Record input').not('#Code, #ASTATUS, .dx-texteditor-input, #BTYPE, #ACT_CODE, #PARTYTYPE_CODE, #SALES_CODE, #SALESACT_CODE, #DEFAULT_DC_TYPE').val('');
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #EMP_ID, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        empr_MaterialRequisition.CreateGrid([{ priority: 'N' }]);
        empr_MaterialRequisition.InitDepartmentDDL();
        $('#V_DATE').val(todayDate);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/MaterialRequisition/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_MaterialRequisition.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
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
}