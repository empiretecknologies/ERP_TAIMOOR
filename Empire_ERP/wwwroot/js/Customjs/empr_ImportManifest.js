var empr_ImportManifest = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    tranId: 0,
    
    InitEvents: function () {
        $(document).ready(function () {
            empr_ImportManifest.InitQuickSearchGrid();
            empr_ImportManifest.ResetForm();
            empr_ImportManifest.InitReportTypeDDL();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_ImportManifest.GetImportManifestByCode(data.traN_ID);
                }
            });
            $('body').on('click', '#BtnQuickSearch', function () {
                empr_ImportManifest.InitQuickSearchGrid();
                $('#gridDiv').show();
                $('.formDiv').hide();
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ImportManifest.ValidateInfo()) {
                            empr_ImportManifest.SaveInfo();
                        }
                    }
                } else {
                    if (empr_ImportManifest.ValidateInfo()) {
                        empr_ImportManifest.SaveInfo();
                    }
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                empr_ImportManifest.GetImportManifestByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/ImportManifest/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_ImportManifest.GetImportManifestByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_ImportManifest.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_ImportManifest.ResetForm();
                $('#gridDiv').hide();
                $('.formDiv').show();
            });

            $('body').on('click', '.btn-print, #BtnGenerateReport', function () {
                empr_ImportManifest.GeneratePrintReport();
            });

            $('body').on('click', '.elm_print', function () {
                empr_ImportManifest.tranId = $(this).attr("reportid");
                empr_ImportManifest.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    
    ResetForm: function () {
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        empr_ImportManifest.CreateGrid([]);
        empr_ImportManifest.InitDropdowns();
        $('#V_DATE').val(todayDate);
        $('#V_DATE').focus();
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
    
    CreateGrid: function (dataSrc) {
        dataSrc.forEach(item => {
            if (item.d_DATE == '1900-01-01' || item.d_DATE == '01-01-1900' || item.d_DATE == '01-Jan-1900' || item.d_DATE == '1/1/1900 12:00:00 AM' || item.d_DATE == '1/1/1900') {
                item.d_DATE = undefined;
            }
        });
        if (dataSrc.length > 0) {
            empr_ImportManifest.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_ImportManifest.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_ImportManifest.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_ImportManifest.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_ImportManifest.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                       <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_ImportManifest.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_ImportManifest.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                dataField: 'd_DATE',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy'
            },
            {
                dataField: 'shippeR_DDL',
                caption: 'Shipper',
                width: 250,
                allowSorting: false,
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
                //lookup: {
                //    dataSource: Parties,
                //    displayExpr: 'value',
                //    valueExpr: 'customizedKey',
                //    allowClearing: true
                //},
                setCellValue: function (newData, value, currentRowData) {
                    var selectedParty = Parties.filter(u => u.customizedKey == value);
                    if (selectedParty.length > 0) {
                        newData.shippeR_DDL = selectedParty[0].customizedKey;
                        newData.shiP_CODE = selectedParty[0].key;
                        newData.sacT_CODE = selectedParty[0].accountCode;
                    } else {
                        newData.shippeR_DDL = null;
                        newData.shiP_CODE = null;
                        newData.sacT_CODE = null;
                    }
                }
            },
            {
                dataField: 'shiP_CODE',
                caption: 'Seller',
                visible: false
            },
            {
                dataField: 'sacT_CODE',
                caption: 'Act',
                visible: false
            },
            {
                dataField: 'importeR_DDL',
                caption: 'Importer',
                width: 250,
                allowSorting: false,
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
                //lookup: {
                //    dataSource: Parties,
                //    displayExpr: 'value',
                //    valueExpr: 'customizedKey',
                //    allowClearing: true
                //},
                setCellValue: function (newData, value, currentRowData) {
                    var selectedParty = Parties.filter(u => u.customizedKey == value);
                    if (selectedParty.length > 0) {
                        newData.importeR_DDL = selectedParty[0].customizedKey;
                        newData.imporT_CODE = selectedParty[0].key;
                        newData.iacT_CODE = selectedParty[0].accountCode;
                    } else {
                        newData.importeR_DDL = null;
                        newData.imporT_CODE = null;
                        newData.iacT_CODE = null;
                    }
                }
            },
            {
                dataField: 'imporT_CODE',
                caption: 'Buyer',
                visible: false
            },
            {
                dataField: 'iacT_CODE',
                caption: 'Act',
                visible: false
            },
            {
                dataField: 'qty',
                caption: 'Qty'
            },
            {
                dataField: 'lot',
                caption: 'Lot #',
            },
            {
                dataField: 'comment',
                caption: 'Comment',
            },
            {
                dataField: 'warehouse',
                caption: 'Warehouse',
                //lookup: {
                //    dataSource: Warehouses,
                //    displayExpr: 'value',
                //    valueExpr: 'key'
                //}
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "ImportManifest", "d_DATE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }
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

                empr_ImportManifest.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_ImportManifest.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_ImportManifest.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_ImportManifest.GenerateKey(36);
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
                empr_ImportManifest.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_ImportManifest.GenerateKey(36), dC_TYPE: empr_ImportManifest.DC_TYPE });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_ImportManifest.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_ImportManifest.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_ImportManifest.GenerateKey(36), dC_TYPE: empr_ImportManifest.DC_TYPE });
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
                    empr_ImportManifest.rowsCount -= 1;
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
                        ajaxHelper.ajaxPostJsonData({ tranID: $('#Code').val(), code: dtCode }, "/ImportManifest/DeleteImportManifestDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_ImportManifest.rowsCount -= 1;
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
    
    InitDropdowns: function () {
        ajaxHelper.ajaxGetJson("/ImportManifest/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_ImportManifest.InitItemCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    
    InitDropdownsWithValue: function (iCode) {
        ajaxHelper.ajaxGetJson("/ImportManifest/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_ImportManifest.InitItemCodeDDL(data.data, iCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    
    InitItemCodeDDL: function (dataSource, selectedValue) {
        $('#ITEM_CODE').dxSelectBox({
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
            searchTimeout: 500
        });
    },
    
    InitQuickSearchGrid: function () {
        empr_ImportManifest.GetImportManifests();
    },
    
    GetImportManifests: function () {
        ajaxHelper.ajaxGetJson('/ImportManifest/GetImportManifests', function (data) {
            if (data.msgType == 1) {
                empr_ImportManifest.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    
    CreateQuickSearchGrid: function (dataSrc) {
        var columns = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin" && !Permissions.r_PRINT) {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            //{ dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            //{ dataField: 'voucheR_NO', caption: 'Voucher No' },
            { dataField: 'iteM_CODE', caption: 'Item', },
            { dataField: 'traN_ID', caption: 'Code', visible: false },
            //{ dataField: 'astatus', caption: 'Status' },
        ];
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/ImportManifest/GetImportManifests", "traN_ID", "ImportManifest");
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "ImportManifestQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    
    ValidateInfo: function () {

        var valid = true;
        var sellerValid = true;
        var buyerValid = true;
        var data = empr_ImportManifest.GetDataToSave();

        if (data.Master.ITEM_CODE == "" || data.Master.ITEM_CODE == null || data.Master.ITEM_CODE == undefined) {
            empr_helper.notify("Please select item.", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add data.", 2);
            valid = false;
            return valid;
        }



        $.each(data.Detail, function (index, item) {
            if (item.shiP_CODE == "" || item.shiP_CODE == null || item.shiP_CODE == undefined) {
                sellerValid = false;
            }

            if (item.imporT_CODE == "" || item.imporT_CODE == null || item.imporT_CODE == undefined) {
                buyerValid = false;
            }

            if (sellerValid == false && buyerValid == false) {
                empr_helper.notify("Please select shipper or importer at index " + index, 2);
                console.log("Shipper & Importer at index " + index + " has empty Code.");
                valid = false;
                return valid;
            }
        });

        return valid;
    },
    
    SaveInfo: function () {
        var dataModel = empr_ImportManifest.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/ImportManifest/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    empr_ImportManifest.tranId = data.data.code;
                }
                if (dataClear == 1) {
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
                        $('#BtnDelete').show();
                    }
                }
                else {
                    empr_ImportManifest.ResetForm();
                }
            }
        }, false, true);
    },
    
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value');
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');

        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            ITEM_CODE: ITEM_CODE,
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

        if (empr_ImportManifest.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource")
            };
        }

        modelRecord.Detail.forEach(item => {
            if (item.d_DATE != undefined) {
                item.d_DATE = empr_ImportManifest.formatDateToDDMMYYYY(item.d_DATE);
            }
        });

        return modelRecord;
    },
    
    formatDateToDDMMYYYY: function (dateStr) {
        const date = new Date(dateStr);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0'); // getMonth() is zero-indexed
        const year = date.getFullYear();
        return `${year}-${month}-${day}`;
    },
    
    GetImportManifestByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/ImportManifest/GetImportManifestByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    empr_ImportManifest.tranId = response.traN_ID
                    $('#Code').val(response.traN_ID);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#ITEM_CODE').dxSelectBox('instance').option('value', response.iteM_CODE);
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
                    console.log(data.detail.data)
                    empr_ImportManifest.CreateGrid(data.detail.data);

                    $('.card-body').addClass('customHighlightForModifiedCells');
                    $("#Loader").hide();
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                    $("#Loader").hide();
                }
                $('#gridDiv').hide();
                $('.formDiv').show();
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
                $("#Loader").hide();
            }
        }, false, true);
    },
    
    GetImportManifestDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/ImportManifest/GetImportManifestDetailsByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_ImportManifest.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/ImportManifest/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ImportManifest.ResetForm();
                    empr_ImportManifest.InitQuickSearchGrid();
                    $('#gridDiv').show();
                    $('.formDiv').hide();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/ImportManifest/GetReportTypes", function (data) {
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

    GeneratePrintReport: function () {
        let TRAN_ID = empr_ImportManifest.tranId;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/ImportManifest/GetPrintReport", function (data) {
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
}