var empr_ImportPermit = {
    totalCount: 0,
    rowsCount: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_ImportPermit.InitQuickSearchGrid();
            empr_ImportPermit.ResetForm();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_ImportPermit.GetImportPermitByCode(data.traN_ID);
                }
            });
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_ImportPermit.GetImportPermitByCode(id);
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ImportPermit.ValidateMainInfo()) {
                            empr_ImportPermit.Save();
                        }
                    }
                } else {
                    if (empr_ImportPermit.ValidateMainInfo()) {
                        empr_ImportPermit.Save();
                    }
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_ImportPermit.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_ImportPermit.ResetForm();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_ImportPermit.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_ImportPermit.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                debugger;
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_ImportPermit.AddSodaToDelivery();
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
            empr_ImportPermit.rowsCount = dataSrc.length - 1;
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
            if (invalidDates.includes(item.issuE_DATE)) {
                item.issuE_DATE = null;
            }
            if (invalidDates.includes(item.exP_DATE)) {
                item.exP_DATE = null;
            }
            if (invalidDates.includes(item.inS_DATE)) {
                item.inS_DATE = null;
            }
            if (item.insrancE_STATUS == "Y") {
                item.insrancE_STATUS = true;
            } else {
                item.insrancE_STATUS = false;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_ImportPermit.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_ImportPermit.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_ImportPermit.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_ImportPermit.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_ImportPermit.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_ImportPermit.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                dataField: 'imporT_PERMIT',
                caption: 'Import Permit No',
                width: 160,
                setCellValue: function (newData, value, currentRowData) {
                    newData.imporT_PERMIT = value;

                    let data = PermitDetails;
                    let selectedPermit = data.filter(x => x.key == value);

                    let gridInstance = $("#DetailContainer").dxDataGrid("instance");
                    let allData = gridInstance.getDataSource().items();

                    let matchingRows = allData.filter(x => x.imporT_PERMIT == value);

                    let permitCount = matchingRows.length;

                    let totalTQty = matchingRows.reduce((sum, row) => {
                        let tqty = parseFloat(row.tqty) || 0;
                        let iqty = parseFloat(row.iqty) || 0;
                        return sum + (tqty - iqty);
                    }, 0);
                    if (selectedPermit != null && selectedPermit.length > 0) {
                        newData.tqty = selectedPermit[0].value + totalTQty;
                    } else {
                        newData.tqty = 0;
                    }
                    console.log("Permit:", value);
                    console.log("Count in Grid:", permitCount);
                    console.log("Total tqty:", totalTQty);
                }
            },
            {
                dataField: 'sdoc',
                caption: 'Sign Contract',
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

                    const fileLabel = $('<span>')                           // 📌 (1) File label added
                        .css({ "margin-left": "10px", "font-size": "12px", "font-style": "italic" });

                    fileInput.on('change', function (event) {
                        const file = event.target.files[0];

                        if (file) {
                            fileLabel.text(file.name);                     // 📌 (2) Set file name on select

                            let formData = new FormData();
                            formData.append('model', file, file.name);

                            $.ajax({
                                url: '/ImportPermit/SaveImage',
                                data: formData,
                                processData: false,
                                contentType: false,
                                type: "POST",
                                success: function (data) {
                                    if (data.msgType == '1') {
                                        let grid = options.component;
                                        let rowIndex = options.rowIndex;
                                        let dataSource = grid.option("dataSource");

                                        dataSource[rowIndex].sdoc = data.data;
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
                        } else {
                            fileLabel.text('');                            // 📌 (3) Clear name if no file
                        }
                    });

                    inputGroup.append(fileInput).append(fileLabel);        // 📌 (4) Append label to group

                    if (options.data.sdoc != null && options.data.sdoc !== '') {
                        const viewButton = $('<div>')
                            .addClass('input-group-append')
                            .append(
                                $('<a>')
                                    .attr('href', 'javascript:;')
                                    .addClass('input-group-text')
                                    .on('click', function () {
                                        const fileUrl = options.data.sdoc;
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
                dataField: 'imporT_PERMIT_DOC',
                caption: 'Import Permit Doc',
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
                                url: '/ImportPermit/SaveImage',
                                data: formData,
                                processData: false,
                                contentType: false,
                                type: "POST",
                                success: function (data) {
                                    if (data.msgType == '1') {
                                        let grid = options.component;
                                        let rowIndex = options.rowIndex;
                                        let dataSource = grid.option("dataSource");

                                        dataSource[rowIndex].imporT_PERMIT_DOC = data.data;
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
                    if (options.data.imporT_PERMIT_DOC != null && options.data.imporT_PERMIT_DOC !== '') {
                        const viewButton = $('<div>')
                            .addClass('input-group-append')
                            .append(
                                $('<a>')
                                    .attr('href', 'javascript:;')
                                    .addClass('input-group-text')
                                    .on('click', function () {
                                        const fileUrl = options.data.imporT_PERMIT_DOC;
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
                dataField: 'tqty',
                caption: 'Total Qty',
                width: 79
            },
            {
                dataField: 'iqty',
                caption: 'Issue Qty',
                width: 79
            },
            {
                dataField: 'bqty',
                caption: 'Balance',
                width: 79,
                allowEditing: false,
                calculateCellValue: function (rowData) {
                    const tqty = rowData.tqty || 0;
                    const iqty = rowData.iqty || 0;
                    const balance = tqty - iqty;
                    // Update PermitDetails tracking
                    //this.updatePermitDetails(rowData, balance);
                    return balance;
                },
                updatePermitDetails: function (rowData, currentBalance) {
                    const tqty = rowData.tqty || 0;
                    const iqty = rowData.iqty || 0;
                    const permitKey = rowData.imporT_PERMIT;
                    const rowKey = rowData.__KEY__;

                    let data = PermitDetails;
                    let selectedPermit = data.filter(x => x.key == rowData.imporT_PERMIT);

                    if (selectedPermit == null || selectedPermit.length == 0) {
                        PermitDetails.push({
                            key: rowData.imporT_PERMIT,
                            value: tqty - iqty
                        });
                    } else if (rowKey && rowKey !== selectedPermit[0].code) {
                        // New row for same permit - update tracking
                        selectedPermit[0].value = currentBalance + selectedPermit[0].value;
                        selectedPermit[0].code = rowKey;
                    }
                }
            },
            {
                dataField: 'issuE_DATE',
                caption: 'Issue Date',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy'
            },
            {
                dataField: 'exP_DATE',
                caption: 'Exp Date',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy',
            },
            {
                dataField: 'customizedKey',
                caption: 'Shipper',
                allowSorting: false,
                width: 160,
                lookup: {
                    dataSource: {
                        store: Parties,
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'customizedKey',
                    searchEnabled: true,
                    showClearButton: true,
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    }
                },
                setCellValue: function (newData, value, currentRowData) {
                    var selectedAccount = Parties.filter(u => u.customizedKey == value);
                    if (selectedAccount.length > 0) {
                        newData.customizedKey = selectedAccount[0].customizedKey;
                        newData.acT_CODE = selectedAccount[0].accountCode;
                        newData.partY_CODE = selectedAccount[0].key;
                    }
                }
            },
            {
                dataField: 'partY_CODE',
                caption: 'Party Code',
                visible: false
            },
            {
                dataField: 'acT_CODE',
                caption: 'Account Code',
                visible: false
            },
            {
                dataField: 'origin',
                caption: 'Origin',
                allowSorting: false,
                width: 120,
                lookup: {
                    dataSource: {
                        store: Regions,
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
                },
            },
            {
                dataField: "insrancE_STATUS",
                caption: "Ins Status",
                width: 100,
                cellTemplate: function (container, options) {
                    let checkbox = $("<input>")
                        .addClass("form-check-input")
                        .attr("type", "checkbox")
                        .attr("id", "SHOW_SELECTED")
                        .prop("checked", options.value)
                        .on("change", function () {
                            options.setValue(this.checked);
                        });
                    $("<div>").addClass("form-check form-switch").append(checkbox).appendTo(container);
                },
                allowEditing:false
            },
            {
                dataField: 'coveR_NO',
                caption: 'Cover No.',
                width: 90,
            },
            {
                dataField: 'inS_DATE',
                caption: 'Ins Date',
                width: 100,
                dataType: 'date',
                format: 'dd-MM-yyyy',
            },
            {
                dataField: 'dT_DESC',
                width: 190,
                caption: 'Description',
            },
            //{
            //    dataField: 'voucheR_NO',
            //    caption: 'Pick.Id #',
            //    width: 150,
            //    allowEditing: false,
            //},
            {
                dataField: 'voucheR_NO', caption: 'Pick.Id #',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_ImportPermit.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.picK_ID) + ')')
                        .appendTo(container);
                }
            },
            {
                dataField: 'picK_ID',
                caption: 'Pick Id',
                visible: false,
            },
        ];
        empr_helper.editableDxGridbindingForTransactions('#DetailContainer', col, dataSrc, "AccountOpening", "iteM_CODE");
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

                empr_ImportPermit.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_ImportPermit.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_ImportPermit.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_ImportPermit.GenerateKey(36);
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
                empr_ImportPermit.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_ImportPermit.GenerateKey(36), priority: 'N' });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
                //console.log(dataSource)
                //dataSource.forEach(row => {
                //    const tqty = row.tqty || 0;
                //    const iqty = row.iqty || 0;
                //    const permit = row.imporT_PERMIT;
                //    const diff = tqty - iqty;

                //    let selectedPermit = PermitDetails.find(x => x.key === permit);

                //    if (!selectedPermit) {
                //        PermitDetails.push({ key: permit, value: diff });
                //    } else {
                //        selectedPermit.value += diff;
                //    }
                //    console.log(PermitDetails)
                //});
            });
        }
        else {
            //empr_ImportPermit.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_ImportPermit.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_ImportPermit.GenerateKey(36), priority: 'N' });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
            //console.log(dataSource)
            //dataSource.forEach(row => {
            //    const tqty = row.tqty || 0;
            //    const iqty = row.iqty || 0;
            //    const permit = row.imporT_PERMIT;
            //    const diff = tqty - iqty;

            //    let selectedPermit = PermitDetails.find(x => x.key === permit);

            //    if (!selectedPermit) {
            //        PermitDetails.push({ key: permit, value: diff });
            //    } else {
            //        selectedPermit.value += diff;
            //    }
            //    console.log(PermitDetails)
            //});
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
                    empr_ImportPermit.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/ImportPermit/DeleteImportPermitDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_ImportPermit.rowsCount -= 1;
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
            //    //        url: "/ImportPermit/GetItems",
            //    //        method: "GET"
            //    //    });
            //    //}
            //    load(loadOptions) {
            //        const deferred = $.Deferred();
            //        $.ajax({
            //            url: "/ImportPermit/GetItems",
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
                    //    //        url: "/ImportPermit/GetItems",
                    //    //        method: "GET"
                    //    //    });
                    //    //}
                    //    load(loadOptions) {
                    //        const deferred = $.Deferred();
                    //        $.ajax({
                    //            url: "/ImportPermit/GetItems",
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
        empr_ImportPermit.GetImportPermits();
    },

    GetImportPermits: function () {
        ajaxHelper.ajaxGetJson('/ImportPermit/GetImportPermits', function (data) {
            if (data.msgType == 1) {
                empr_ImportPermit.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {
        console.log('aaa', dataSrc);
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
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'remarks', caption: 'Comment', },
            { dataField: 'imporT_PERMIT', caption: 'Permit', },
            { dataField: 'sdoc', caption: 'Document', visible: false },
            { dataField: 'imporT_PERMIT_DOC', caption: 'Permit Document', },
            { dataField: 'tqty', caption: 'Total Qty', },
            { dataField: 'iqty', caption: 'Issue Qty', },
            { dataField: 'issuE_DATE', caption: 'Issue Date', dataType: 'date', format: 'dd-MM-yyy', },
            { dataField: 'exP_DATE', caption: 'Exp Date', dataType: 'date', format: 'dd-MM-yyy', },
            { dataField: 'partY_NAME', caption: 'Shipper', },
            { dataField: 'origin', caption: 'Origin', },
            { dataField: 'insrancE_STATUS', caption: 'Insurance Status', },
            { dataField: 'coveR_NO', caption: 'Cover No.', },
            { dataField: 'inS_DATE', caption: 'Insurance Date', dataType: 'date', format: 'dd-MM-yyy', },
            { dataField: 'desc', caption: 'Description', },
                { dataField: 'picK_DATA', caption: 'Pick.Id #', },
                {
                    dataField: 'picK_DATA', caption: 'Pick.Id #',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_ImportPermit.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
                            .appendTo(container);
                    }
                },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
            ];
            empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ImportPermitQSD");
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
                            .attr('onclick', 'empr_ImportPermit.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.picK_ID) + ')')
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
            empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ImportPermitQS");
        }
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    GetImportPermitByCode: function (code) {
        ajaxHelper.ajaxGetJson('/ImportPermit/GetImportPermitByCode?code=' + code, function (data) {
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
                    empr_ImportPermit.CreateGrid(data.detail.data);
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

    GetImportPermitDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/ImportPermit/GetImportPermitDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_ImportPermit.CreateGrid(data.data);
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
        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }

        if (detailRecords.length > 0) {
            $.each(detailRecords, function (index, item) {
                if (!(item.issuE_DATE == "" || item.issuE_DATE == null || item.issuE_DATE == undefined)) {
                    item.issuE_DATE = empr_helper.PrepareDate(item.issuE_DATE);
                }
                if (!(item.exP_DATE == "" || item.exP_DATE == null || item.exP_DATE == undefined)) {
                    item.exP_DATE = empr_helper.PrepareDate(item.exP_DATE);
                }
                if (!(item.inS_DATE == "" || item.inS_DATE == null || item.inS_DATE == undefined)) {
                    item.inS_DATE = empr_helper.PrepareDate(item.inS_DATE);
                }
                console.log("status : " + item.insrancE_STATUS)
                if (item.insrancE_STATUS == true || item.insrancE_STATUS == "Y") {
                    item.insrancE_STATUS = "Y";
                }
                else {
                    item.insrancE_STATUS = "N";
                }
            });
        }
        console.log(detailRecords)
        if (empr_ImportPermit.rowsCount == detailRecords.length) {
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
        console.log(modelRecord);
    },

    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_ImportPermit.GetDataToSave();
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

            if (item.imporT_PERMIT == "" || item.imporT_PERMIT == null || item.imporT_PERMIT == undefined) {
                empr_helper.notify("Please enter Permit Number at index " + index, 2);
                valid = false;
                return valid;
                console.log("Permit Number at index " + index + " has empty Sales Contract.");
            }

            if (item.sdoc == "" || item.sdoc == null || item.sdoc == undefined) {
                empr_helper.notify("Please add Sales Document at index " + index, 2);
                valid = false;
                return valid;
                console.log("Document at index " + index + " is missing.");
            }

            if (item.customizedKey == "" || item.customizedKey == null || item.customizedKey == undefined) {
                empr_helper.notify("Please Select Shipper ", 2);
                valid = false;
                return valid;
            }

        });


        return valid;
    },

    Save: function () {
        debugger;
        var dataModel = empr_ImportPermit.GetDataToSave();
        console.log(dataModel)
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/ImportPermit/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    $('.vHide').show();
                }
                empr_ImportPermit.GetImportPermitDetailByCode($('#Code').val());
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
        empr_ImportPermit.CreateGrid([{ __KEY__: empr_ImportPermit.GenerateKey(36), priority: 'N' }]);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/ImportPermit/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ImportPermit.ResetForm();
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
        ajaxHelper.ajaxGetJson('/ImportPermit/GetSodaBookFeedingDetail', function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_ImportPermit.CreateSodaPickGrid(data.data);
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
        var existingPickIds = empr_ImportPermit.GetAllPickIds();
        var filteredDataSrc = dataSrc.filter(item => !existingPickIds.includes(item.picK_ID));
        var col = [
            { dataField: 'picK_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Transaction #', allowEditing: false },
            { dataField: 'saleS_CONTRACT', caption: 'Sales Contract', allowEditing: false, },
            { dataField: 'sdoc', caption: 'Document', allowEditing: false, visible: false },
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
                var data = empr_ImportPermit.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.picK_ID != "" && item.picK_ID != null && item.picK_ID != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_ImportPermit.SetData(selectedSodas);
                    var finalData = existingData.concat(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_ImportPermit.SetData(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var data = empr_ImportPermit.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.picK_ID != "" && item.picK_ID != null && item.picK_ID != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                //selectedSodas = empr_ImportPermit.SetData(selectedSodas);
                var finalData = existingData.concat(selectedSodas);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                //selectedSodas = empr_ImportPermit.SetData(selectedSodas);
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