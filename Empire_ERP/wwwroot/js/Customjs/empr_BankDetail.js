var empr_BankDetail = {
    totalCount: 0,
    rowsCount: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_BankDetail.InitQuickSearchGrid();
            empr_BankDetail.ResetForm();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_BankDetail.GetBankDetailByCode(data.traN_ID);
                }
            });
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_BankDetail.GetBankDetailByCode(id);
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_BankDetail.ValidateMainInfo()) {
                            empr_BankDetail.Save();
                        }
                    }
                } else {
                    if (empr_BankDetail.ValidateMainInfo()) {
                        empr_BankDetail.Save();
                    }
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_BankDetail.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_BankDetail.ResetForm();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_BankDetail.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_BankDetail.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                debugger;
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_BankDetail.AddSodaToDelivery();
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
        });
    },

    CreateGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_BankDetail.rowsCount = dataSrc.length - 1;
            
        }
        const invalidDates = [
            '1900-01-01', '01-01-1900', '01-Jan-1900', '1/1/1900', '01/01/1900',
            '1/1/1900 12:00:00 AM', '01/01/1900 12:00:00 AM',
            '2000-01-01', '01-01-2000', '01-Jan-2000', '1/1/2000', '01/01/2000',
            '1/1/2000 12:00:00 AM', '01/01/2000 12:00:00 AM',
            '00-01-01', '01-01-00', '01-Jan-00', '1/1/00', '01/01/00',
            '1/1/00 12:00:00 AM', '01/01/00 12:00:00 AM', '01-Jan-00 12:00:00 AM'
        ];

        dataSrc.forEach(item => {
            if (invalidDates.includes(item.eta)) {
                item.eta = null;
            }
            if (item.fI_REQ == '1') {
                item.fI_REQ = true;
            } else {
                item.fI_REQ = false;
            }
        });

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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_BankDetail.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_BankDetail.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_BankDetail.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_BankDetail.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_BankDetail.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_BankDetail.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                dataField: 'originaL_DOC',
                caption: 'Original Doc',
                width: 250,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    const inputGroup = $('<div>').addClass('input-group');

                    const fileInput = $('<input>')
                        .attr({
                            type: 'file',
                            accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*'
                        })
                        .addClass('form-control')
                        .css({ "display": "block" });


                    fileInput.on('change', function (event) {
                        const file = event.target.files[0];

                        if (file) {
                            let formData = new FormData();
                            formData.append('model', file, file.name);

                            $.ajax({
                                url: '/BankDetail/SaveImage',
                                data: formData,
                                processData: false,
                                contentType: false,
                                type: "POST",
                                success: function (data) {
                                    if (data.msgType == '1') {
                                        let grid = options.component;
                                        let rowIndex = options.rowIndex;
                                        let dataSource = grid.option("dataSource");

                                        dataSource[rowIndex].originaL_DOC = data.data;
                                        grid.repaint(); // Ensures the UI updates correctly
                                    } else {
                                        console.error("Upload Error:", data);
                                        empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                                    }
                                },
                                error: function (error) {
                                    console.error("File upload failed:", error);
                                    empr_helper.notify("File upload failed. Please try again.", 2);
                                }
                            });
                        }
                    });

                    inputGroup.append(fileInput);
                    if (options.data.originaL_DOC != null && options.data.originaL_DOC !== '') {
                        const viewButton = $('<div>')
                            .addClass('input-group-append')
                            .append(
                                $('<a>')
                                    .attr('href', 'javascript:;')
                                    .addClass('input-group-text')
                                    .on('click', function () {
                                        const fileUrl = options.data.originaL_DOC;
                                        if (fileUrl) {
                                            window.open(fileUrl, '_blank');
                                        } else {
                                            alert('No document available to view.');
                                        }
                                    })
                                    .append($('<i>').addClass('fa fa-eye'))
                            );

                        inputGroup.append(viewButton);
                    }
                    $(container).append(inputGroup);
                }
            },
            {
                dataField: 'dhL_NO',
                caption: 'DHL No',
                width: 120
            },
            {
                dataField: 'eta',
                caption: 'ETA',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy',
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) {
                            year += 2000;
                        }
                        var formattedDate = new Date(year, date.getMonth(), date.getDate());
                        newData.eta = formattedDate;
                    } else {
                        newData.eta = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },
            {
                dataField: 'booK_TYPE',
                caption: 'Bank Name',
                width: 200,
                allowSorting: false,
                lookup: {
                    dataSource: {
                        store: Accounts.data,
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    showClearButton: true,
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    }
                }
            },
            {
                dataField: 'bank',
                caption: 'Bank',
                width: 60,
                allowEditing: false,
                dataType: "number",
                cellTemplate: function (container, options) {
                    var $checkBoxContainer = $("<div>").addClass("custom-checkbox-container");

                    $("<div>")
                        .dxCheckBox({
                            value: options.value == "1",
                            onValueChanged: function (e) {
                                var gridInstance = $("#DetailContainer").dxDataGrid("instance");
                                gridInstance.cellValue(options.rowIndex, "bank", e.value ? "1" : "0");
                            }
                        })
                        .appendTo($checkBoxContainer);

                    $(container).append($checkBoxContainer);
                }
            },
            {
                dataField: 'fI_REQ',
                caption: 'FI.Req',
                width: 80,
                allowEditing: false,
                dataType: "number",
                cellTemplate: function (container, options) {
                    var $checkBoxContainer = $("<div>").addClass("custom-checkbox-container");

                    $("<div>")
                        .dxCheckBox({
                            value: options.value == "1",
                            onValueChanged: function (e) {
                                var gridInstance = $("#DetailContainer").dxDataGrid("instance");
                                gridInstance.cellValue(options.rowIndex, "fI_REQ", e.value ? "1" : "0");
                            }
                        })
                        .appendTo($checkBoxContainer);

                    $(container).append($checkBoxContainer);
                }
            },
            {
                dataField: 'fi',
                caption: 'FI',
                width: 40,
                allowEditing: false,
                dataType: "number",
                cellTemplate: function (container, options) {
                    var $checkBoxContainer = $("<div>").addClass("custom-checkbox-container");

                    $("<div>")
                        .dxCheckBox({
                            value: options.value == "1",
                            onValueChanged: function (e) {
                                var gridInstance = $("#DetailContainer").dxDataGrid("instance");
                                gridInstance.cellValue(options.rowIndex, "fi", e.value ? "1" : "0");
                            }
                        })
                        .appendTo($checkBoxContainer);

                    $(container).append($checkBoxContainer);
                }
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                width: 250,
            },
            //{
            //    dataField: 'voucheR_NO',
            //    caption: 'Pick.Id #',
            //    width: 170,
            //    allowEditing: false,
            //},
            {
                dataField: 'voucheR_NO', caption: 'Pick.Id #',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_BankDetail.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
                        .appendTo(container);
                }
            },
            {
                dataField: 'picK_ID',
                caption: 'Pick Id',
                visible: false,
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "BankDetails", "iteM_CODE");
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
        debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_BankDetail.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_BankDetail.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_BankDetail.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_BankDetail.GenerateKey(36);
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
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_BankDetail.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_BankDetail.GenerateKey(36), priority: 'N' });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_BankDetail.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_BankDetail.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_BankDetail.GenerateKey(36), priority: 'N' });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },

    DeleteRow: function (index, dtCode) {
        debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_BankDetail.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/BankDetail/DeleteBankDetailDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_BankDetail.rowsCount -= 1;
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

    dropDownBoxEditorTemplate: function (cellElement, cellInfo) {
        return $('<div>').dxDropDownBox({
            dropDownOptions: { width: 500 },
            //dataSource: new DevExpress.data.DataSource({
            //    //load: function (loadOptions) {
            //    //    return $.ajax({
            //    //        url: "/BankDetail/GetItems",
            //    //        method: "GET"
            //    //    });
            //    //}
            //    load(loadOptions) {
            //        const deferred = $.Deferred();
            //        $.ajax({
            //            url: "/BankDetail/GetItems",
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
                    //    //        url: "/BankDetail/GetItems",
                    //    //        method: "GET"
                    //    //    });
                    //    //}
                    //    load(loadOptions) {
                    //        const deferred = $.Deferred();
                    //        $.ajax({
                    //            url: "/BankDetail/GetItems",
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

    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    InitQuickSearchGrid: function () {
        empr_BankDetail.GetBankDetails();
    },

    GetBankDetails: function () {
        ajaxHelper.ajaxGetJson('/BankDetail/GetBankDetails', function (data) {
            if (data.msgType == 1) {
                empr_BankDetail.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {
        console.log(dataSrc)
        if (dataSrc.length > 0 ? dataSrc[0].picK_DATA != undefined : false) {
            var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger

                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                               </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', visible: false},
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
                { dataField: 'voucheR_NO', caption: 'Transaction #', },
                {
                    dataField: 'picK_DATA', caption: 'Pick.Id #',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_BankDetail.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
                            .appendTo(container);
                    }
                },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'remarks', caption: 'Comment', },
            { dataField: 'eta', caption: 'ETA', },
                { dataField: 'ETA', caption: 'Bank Status', },
                { dataField: 'booK_TYPE', caption: 'Book Type', },
                { dataField: 'dhL_NO', caption: 'DHL #', },
                { dataField: 'fI_REQ', caption: 'FI Req', },
                { dataField: 'fi', caption: 'FI', },
            { dataField: 'dT_DESC', caption: 'Description', },
            ];
            empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "BankDetailQSD");
        } else {
            var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger

                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                               </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
                { dataField: 'voucheR_NO', caption: 'Transaction #', },
                {
                    dataField: 'pvoucheR_NO', caption: 'Pick Data #',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_BankDetail.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
                            .appendTo(container);
                    }
                },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'remarks', caption: 'Comment', },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
            ];
            empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "BankDetailQS");
        }
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    GetBankDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/BankDetail/GetBankDetailByCode?code=' + code, function (data) {
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
                    empr_BankDetail.CreateGrid(data.detail.data);
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

    GetBankDetailDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/BankDetail/GetBankDetailDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_BankDetail.CreateGrid(data.data);
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
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            REF: REF,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS
        }
        debugger;
        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }
        //console.log(detailRecords);
        //if (detailRecords.length > 0) {
        //    $.each(detailRecords, function (index, item) {
        //        if (!item.fI_REQ || item.fI_REQ == 'N') {
        //            item.fI_REQ = "N";
        //        }
        //        else {
        //            item.fI_REQ = "Y";
        //        }
        //    });
        //}
        //if (empr_BankDetail.rowsCount == detailRecords.length) {
        //    var modelRecord = {
        //        Master: masterRecord,
        //        Detail: detailRecords
        //    };
        //    return modelRecord;
        //}
        //else {

        //}
        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords
        };
        return modelRecord;
    },

    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_BankDetail.GetDataToSave();
        console.log(data)
        if (data.Master.V_DATE == '') {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please pick data.", 2);
            valid = false;
            return valid;
        }

        var dateValid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

        if (!dateValid) {
            return false;
        }

        $.each(data.Detail, function (index, item) {
            // Check if Item_Code or Quantity is empty
            console.log(item)
            if (item.picK_ID == "" || item.picK_ID == null || item.picK_ID == undefined) {
                empr_helper.notify("Please pick data first", 2);
                valid = false;
                return valid;
            }

            if (item.originaL_DOC == "" || item.originaL_DOC == null || item.originaL_DOC == undefined) {
                empr_helper.notify("Please Upload original Document " + index, 2);
                valid = false;
                return valid;
            }

            if (item.dhL_NO == "" || item.dhL_NO == null || item.dhL_NO == undefined) {
                empr_helper.notify("Please Enter DHL NO" + index, 2);
                valid = false;
                return valid;
            }

            if (!(item.eta == "" || item.eta == null || item.eta == undefined)) {
                item.eta = empr_helper.PrepareDate(item.eta);
            }

        });


        return valid;
    },

    Save: function () {
        debugger;
        var dataModel = empr_BankDetail.GetDataToSave();
        debugger;
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/BankDetail/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    $('.vHide').show();
                }
                empr_BankDetail.GetBankDetailDetailByCode($('#Code').val());
                $('#BtnDelete').show();
            }
        }, false, true);
    },

    ResetForm: function () {

        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        empr_BankDetail.CreateGrid([{ priority: 'N' }]);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/BankDetail/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_BankDetail.ResetForm();
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

    InitSodaPickGrid: function () {
        ajaxHelper.ajaxGetJson('/BankDetail/GetSodaBookFeedingDetail', function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_BankDetail.CreateSodaPickGrid(data.data);
                    $('#SodaPickModal').modal('show');
                } else {
                    empr_helper.notify("Pick data not found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateSodaPickGrid: function (dataSrc) {
        var existingPickIds = empr_BankDetail.GetAllPickIds();
        var filteredDataSrc = dataSrc.filter(item => !existingPickIds.includes(item.picK_ID));
        var col = [
            { dataField: 'picK_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Transaction #', allowEditing: false },
            { dataField: 'ref', caption: 'Reference #', allowEditing: false },
            { dataField: 'remarks', caption: 'Comment', allowEditing: false },
            { dataField: 'shipmenT_DOC', caption: 'Shipment Doc', allowEditing: false, },
            { dataField: 'qty', caption: 'Quantity', allowEditing: false, },
            { dataField: 'shiP_DATE', caption: 'Ship Date', allowEditing: false, },
            { dataField: 'bL_NO', caption: 'BL.No', allowEditing: false, },
            { dataField: 'dT_DESC', caption: 'Description', allowEditing: false, },
        ];
        empr_helper.editableDxGridbindingForTransactions('#SodaPickGridContainer', col, filteredDataSrc, "SodaFeedingDetails", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_BankDetail.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.picK_ID != "" && item.picK_ID != null && item.picK_ID != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_BankDetail.SetData(selectedSodas);
                    var finalData = existingData.concat(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_BankDetail.SetData(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var data = empr_BankDetail.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.picK_ID != "" && item.picK_ID != null && item.picK_ID != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                //selectedSodas = empr_BankDetail.SetData(selectedSodas);
                var finalData = existingData.concat(selectedSodas);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                //selectedSodas = empr_BankDetail.SetData(selectedSodas);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
            }

            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },

    GetAllPickIds: function () {
        var gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option('dataSource');

        var pickIds = dataSource.map(item => item.picK_ID).filter(id => id !== undefined && id !== null);

        return pickIds;
    },

    openVoucherPage(link, tran_Id) {
        console.log(link)
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
        });
    },

}