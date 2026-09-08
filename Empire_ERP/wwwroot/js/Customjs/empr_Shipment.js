var empr_Shipment = {
    totalCount: 0,
    rowsCount: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_Shipment.InitQuickSearchGrid();
            empr_Shipment.ResetForm();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_Shipment.GetShipmentByCode(data.traN_ID);
                }
            });
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_Shipment.GetShipmentByCode(id);
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Shipment.ValidateMainInfo()) {
                            empr_Shipment.Save();
                        }
                    }
                } else {
                    if (empr_Shipment.ValidateMainInfo()) {
                        empr_Shipment.Save();
                    }
                }
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_Shipment.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_Shipment.ResetForm();
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_Shipment.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_Shipment.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                debugger;
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_Shipment.AddSodaToDelivery();
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
        console.log('bbb',dataSrc);
        if (dataSrc.length > 0) {
            empr_Shipment.rowsCount = dataSrc.length - 1;
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
            if (invalidDates.includes(item.shiP_DATE)) {
                item.shiP_DATE = null;
            }
            if (item.allow == "1") {
                item.allow = true;
            } else {
                item.allow = false;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_Shipment.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_Shipment.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_Shipment.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon" onclick="empr_Shipment.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_Shipment.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_Shipment.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                dataField: 'qty',
                caption: 'Qty',
                width: 80,
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    newData.amt = (currentRowData.rate || 0) * (value || 0);
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                allowSorting: false,
                width: 100,
                lookup: {
                    dataSource: {
                        store: Units,
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
                dataField: 'rate',
                caption: 'Rate',
                width: 80,
            },
            {
                dataField: 'amt',
                caption: 'AMT',
                width: 80,
                allowEditing: false,    
            },
            {
                dataField: 'cqty',
                caption: 'Con.Qty',
                width: 80
            },
            {
                dataField: 'shipmenT_DATE',
                caption: 'Shipment Date',
                dataType: 'date',
                width: 100,
                visible: false,
                format: 'dd-MM-yyyy'
            },
            {
                dataField: 'shiP_DATE',
                caption: 'Ship Date',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy',
                setCellValue: function (newData, value, currentRowData) {
                    newData.shiP_DATE = value;

                    const shipmentDate = new Date(currentRowData.shipmenT_DATE);
                    const shipDate = new Date(value);

                    if (shipDate > shipmentDate) {
                        newData.allow = 1;
                    } else {
                        newData.allow = 0;
                    }
                }
            },
            {
                dataField: "allow",
                caption: "Allow",
                width: 100,
                allowEditing: false,
                dataType: "number",
                cellTemplate: function (container, options) {
                    var $checkBoxContainer = $("<div>").addClass("custom-checkbox-container");

                    $("<div>")
                        .dxCheckBox({
                            value: options.value == 1,
                            onValueChanged: function (e) {
                                var gridInstance = $("#DetailContainer").dxDataGrid("instance");
                                gridInstance.cellValue(options.rowIndex, "allow", e.value ? 1 : 0);
                            }
                        })
                        .appendTo($checkBoxContainer);

                    $(container).append($checkBoxContainer);
                }
            },
            {
                dataField: 'bL_NO',
                caption: 'BL No.',
                width: 100,
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
            },
            //{
            //    dataField: 'voucheR_NO',
            //    caption: 'Pick.Id yahaa #',
            //    width: 150,
            //    allowEditing: false,
            //},
            {
                dataField: 'voucheR_NO', caption: 'Pick Data #',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_Shipment.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
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

                empr_Shipment.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_Shipment.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_Shipment.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_Shipment.GenerateKey(36);
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
                empr_Shipment.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_Shipment.GenerateKey(36), priority: 'N' });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_Shipment.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_Shipment.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_Shipment.GenerateKey(36), priority: 'N' });
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
                    empr_Shipment.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/Shipment/DeleteShipmentDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_Shipment.rowsCount -= 1;
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

    InitQuickSearchGrid: function () {
        empr_Shipment.GetShipments();
    },

    GetShipments: function () {
        ajaxHelper.ajaxGetJson('/Shipment/GetShipments', function (data) {
            if (data.msgType == 1) {
                empr_Shipment.CreateQuickSearchGrid(data.data);
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
                    console.log(options)
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.shipmenT_DOC != null && options.data.shipmenT_DOC != '' && options.data.shipmenT_DOC != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="empr_Shipment.OpenQuickSearchDoc('${options.data.shipmenT_DOC}')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>`;
                    html += '</div>';
                    $(html).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', visible: false},
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
                { dataField: 'voucheR_NO', caption: 'Transaction #', },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'remarks', caption: 'Comment', },
            { dataField: 'qty', caption: 'Qty', },
            { dataField: 'unit', caption: 'Unit', },
            { dataField: 'cqty', caption: 'Con.Qty' },
            { dataField: 'shiP_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'bL_NO', caption: 'BL No.', },
            { dataField: 'desc', caption: 'Description', },
                //{ dataField: 'picK_DATA', caption: 'Pick.Id #', },
                {
                    dataField: 'picK_DATA', caption: 'Pick.Id #',
                    cellTemplate: function (container, options) {
                        $('<a>')
                            .addClass('dx-link')
                            .text(options.value)
                            .attr('href', '#')
                            .attr('onclick', 'empr_Shipment.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
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
            empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ShipmentQSD");
        } else {
            var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    console.log(options)
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.shipmenT_DOC != null && options.data.shipmenT_DOC != '' && options.data.shipmenT_DOC != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="empr_Shipment.OpenQuickSearchDoc('${options.data.shipmenT_DOC}')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>`;
                    html += '</div>';
                    $(html).appendTo(container);
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
                            .attr('onclick', 'empr_Shipment.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.picK_ID) + ')')
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
            empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ShipmentQS");
        }
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    GetShipmentByCode: function (code) {
        ajaxHelper.ajaxGetJson('/Shipment/GetShipmentByCode?code=' + code, function (data) {
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
                    $("#hdnDOC").val(response.shipmenT_DOC);
                    empr_Shipment.InitPartyCodeDDL(response.partY_CODE);
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
                    empr_Shipment.CreateGrid(data.detail.data);
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

    GetShipmentDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/Shipment/GetShipmentDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_Shipment.CreateGrid(data.data);
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
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');
        var SHIPMENT_DOC = $("#hdnDOC").val();
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            REF: REF,
            REMARKS: REMARKS,
            SHIPMENT_DOC: SHIPMENT_DOC,
            ASTATUS: ASTATUS,
            PARTY_CODE: PARTY_CODE
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
                if (!(item.shiP_DATE == "" || item.shiP_DATE == null || item.shiP_DATE == undefined)) {
                    item.shiP_DATE = empr_helper.PrepareDate(item.shiP_DATE);
                }
            });
        }

        if (empr_Shipment.rowsCount == detailRecords.length) {
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

    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_Shipment.GetDataToSave();
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
            console.log(item)
            if (item.picK_ID == "" || item.picK_ID == null || item.picK_ID == undefined) {
                empr_helper.notify("Please pick data", 2);
                valid = false;
                return valid;
            }
            console.log("status : " + item.allow)

            //if (item.allow == true || item.allow == "Y") {
            //    item.allow = "Y";
            //}
            //else {
            //    item.allow = "N";
            //}
        });


        return valid;
    },

    Save: function () {
        debugger;
        var dataModel = empr_Shipment.GetDataToSave();
        console.log('Save Data',dataModel);
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/Shipment/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {    
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    $('.vHide').show();
                }
                empr_Shipment.GetShipmentDetailByCode($('#Code').val());
                $('#BtnDelete').show();
            }
        }, false, true);
    },

    ResetForm: function () {

        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('.vHide').hide();
        $('#BtnDelete').hide();
        $('#REMARKS').val('');
        $("#hdnDOC").val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        empr_Shipment.InitPartyCodeDDL();
        empr_Shipment.CreateGrid([{ priority: 'N' }]);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/Shipment/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Shipment.ResetForm();
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
        ajaxHelper.ajaxGetJson('/Shipment/GetSodaBookFeedingDetail', function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_Shipment.CreateSodaPickGrid(data.data);
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
        var existingPickIds = empr_Shipment.GetAllPickIds();
        var filteredDataSrc = dataSrc.filter(item => !existingPickIds.includes(item.picK_ID));
        var col = [
            { dataField: 'picK_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Transaction #', allowEditing: false },
            { dataField: 'remarks', caption: 'Comment', allowEditing: false },
            { dataField: 'imporT_PERMIT', caption: 'Import Permit', allowEditing: false, },
            { dataField: 'sdoc', caption: 'Contract Document', allowEditing: false, visible: false},
            { dataField: 'imporT_PERMIT_DOC', caption: 'Permit Document', allowEditing: false, visible: false},
            { dataField: 'tqty', caption: 'Total.Qty', allowEditing: false, },
            { dataField: 'iqty', caption: 'Issue.Qty', allowEditing: false, },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, },
            { dataField: 'shipmenT_DATE', caption: 'Ship Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'issuE_DATE', caption: 'Issue Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'exP_DATE', caption: 'Exp Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'partY_NAME', caption: 'Party Name', allowEditing: false },
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
                var data = empr_Shipment.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.picK_ID != "" && item.picK_ID != null && item.picK_ID != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_Shipment.SetData(selectedSodas);
                    var finalData = existingData.concat(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    //selectedSodas = empr_Shipment.SetData(selectedSodas);
                    $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var data = empr_Shipment.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.picK_ID != "" && item.picK_ID != null && item.picK_ID != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                //selectedSodas = empr_Shipment.SetData(selectedSodas);
                var finalData = existingData.concat(selectedSodas);
                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                //selectedSodas = empr_Shipment.SetData(selectedSodas);
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

    UploadDoc: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/Shipment/UploadShipmentDocs",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnDOC").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);
                }
            }
        );
    },

    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    OpenQuickSearchDoc: function (hdnUrl) {
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    InitPartyCodeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/DeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                $('#PARTY_CODE').dxSelectBox({
                    dataSource: {
                        store: data.data,
                        paginate: true,
                        pageSize: 50
                    },
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    },
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
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        
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