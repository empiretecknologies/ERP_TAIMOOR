var empr_PurchaseSaleFormat = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    sellerAverage: 0,
    buyerAverage: 0,
    tranId: 0,
    RT_TYPES: new DevExpress.data.ArrayStore({
        key: "key",
        data: [
            { key: 1, value: '1 KG' },
            { key: 40, value: '40 kG' },
        ]
    }),
    InitEvents: function () {
        $(document).ready(function () {
            empr_PurchaseSaleFormat.InitQuickSearchGrid();
            empr_PurchaseSaleFormat.ResetForm();
            empr_PurchaseSaleFormat.InitReportTypeDDL();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_PurchaseSaleFormat.GetPurchaseSaleFormatByCode(data.traN_ID);
                }
            });
            $('body').on('click', '#BtnQuickSearch', function () {
                empr_PurchaseSaleFormat.InitQuickSearchGrid();
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
                        if (empr_PurchaseSaleFormat.ValidateInfo()) {
                            empr_PurchaseSaleFormat.SaveInfo();
                        }
                    }
                } else {
                    if (empr_PurchaseSaleFormat.ValidateInfo()) {
                        empr_PurchaseSaleFormat.SaveInfo();
                    }
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                empr_PurchaseSaleFormat.GetPurchaseSaleFormatByCode(id);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_PurchaseSaleFormat.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_PurchaseSaleFormat.ResetForm();
                $('#gridDiv').hide();
                $('.formDiv').show();
            });

            $('body').on('click', '.btn-print, #BtnGenerateReport', function () {
                empr_helper.printMultipleBills();
            });

            $('body').on('click', '.elm_print', function () {
                empr_PurchaseSaleFormat.tranId = $(this).attr("reportid");
                empr_helper.selectedBills = [empr_PurchaseSaleFormat.tranId];
                empr_helper.printMultipleBills();
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
        empr_PurchaseSaleFormat.CreateGrid([]);
        empr_PurchaseSaleFormat.InitDropdowns();
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
            if (item.sodA_DATE == '1900-01-01' || item.sodA_DATE == '01-01-1900' || item.sodA_DATE == '01-Jan-1900' || item.sodA_DATE == '1/1/1900 12:00:00 AM' || item.sodA_DATE == '1/1/1900') {
                item.sodA_DATE = undefined;
            }
            if (item.bsodA_DATE == '1900-01-01' || item.bsodA_DATE == '01-01-1900' || item.bsodA_DATE == '01-Jan-1900' || item.bsodA_DATE == '1/1/1900 12:00:00 AM' || item.bsodA_DATE == '1/1/1900') {
                item.bsodA_DATE = undefined;
            }
        });
        if (dataSrc.length > 0) {
            empr_PurchaseSaleFormat.rowsCount = dataSrc.length - 1;
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
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PurchaseSaleFormat.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PurchaseSaleFormat.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PurchaseSaleFormat.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PurchaseSaleFormat.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                       <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PurchaseSaleFormat.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PurchaseSaleFormat.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                dataField: 'comment',
                caption: 'Comment',
            },
            {
                dataField: 'rT_TYPE',
                caption: 'RT Type',
                lookup: {
                    dataSource: empr_PurchaseSaleFormat.RT_TYPES,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                editorOptions: {
                    acceptCustomValue: true,
                    searchEnabled: true,
                    onCustomItemCreating: function (args) {
                        if (args.text != undefined) {
                            var newItem = {};
                            newItem.key = parseInt(args.text);
                            if (isNaN(newItem.key)) {
                                empr_helper.notify("Please enter only number in RT Type.", 2);
                                return;
                            }
                            else {
                                if (args.text.toLowerCase().indexOf('kg') != -1) {
                                    newItem.value = args.text;
                                }
                                else {
                                    newItem.value = args.text + ' KG';
                                }
                                empr_PurchaseSaleFormat.RT_TYPES.insert(newItem);
                                args.customItem = newItem;
                                setTimeout(function () {
                                    $('#DetailContainer').dxDataGrid('instance')
                                        .columnOption("rT_TYPE", "lookup", {
                                            dataSource: empr_PurchaseSaleFormat.RT_TYPES,
                                            displayExpr: 'value',
                                            valueExpr: 'key'
                                        });
                                });
                            }
                        }
                    }
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.rT_TYPE = value;

                    var sqty = parseFloat(currentRowData.sqty) || 0;
                    var srate = parseFloat(currentRowData.srate) || 0;
                    if (!isNaN(sqty) && !isNaN(srate)) {
                        var perRate = parseFloat(srate / newData.rT_TYPE) || 0;
                        if (perRate > -1 && perRate != 'Infinity') {
                            newData.samt = (sqty * perRate).toFixed(2);
                        }
                    }

                    var bqty = parseFloat(currentRowData.bqty) || 0;
                    var brate = parseFloat(currentRowData.brate) || 0;
                    if (!isNaN(bqty) && !isNaN(brate)) {
                        var perRate = parseFloat(brate / newData.rT_TYPE) || 0;
                        if (perRate > -1 && perRate != 'Infinity') {
                            newData.bamt = (bqty * perRate).toFixed(2);
                        }
                    }
                }
            },
            {
                dataField: 'seller',
                caption: 'Seller',
                width: 150
            },
            {
                dataField: 'sodA_DATE',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy'
            },
            {
                dataField: 'scond',
                caption: 'Cond',
            },
            {
                dataField: 'sqty',
                caption: 'Qty',
                setCellValue: function (newData, value, currentRowData) {
                    newData.sqty = value;
                    var qty = parseFloat(newData.sqty) || 0;
                    var rate = parseFloat(currentRowData.srate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    var perRate = parseFloat(rate / rT_TYPE) || 0;
                    if (perRate > -1 && perRate != 'Infinity') {
                        newData.samt = (qty * perRate).toFixed(2);
                        empr_PurchaseSaleFormat.CalculateSellerAverage(qty / rT_TYPE, qty * perRate);
                    } else {
                        newData.samt = (0).toFixed(2);
                    }
                }
            },
            {
                dataField: 'srate',
                caption: 'Rate',
                setCellValue: function (newData, value, currentRowData) {
                    newData.srate = value;
                    var qty = parseFloat(currentRowData.sqty) || 0;
                    var rate = parseFloat(newData.srate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    var perRate = parseFloat(rate / rT_TYPE) || 0;
                    if (perRate > -1 && perRate != 'Infinity') {
                        newData.samt = (qty * perRate).toFixed(2);
                        empr_PurchaseSaleFormat.CalculateSellerAverage(qty / rT_TYPE, qty * perRate);
                    } else {
                        newData.samt = (0).toFixed(2);
                    }
                }
            },
            {
                dataField: 'samt',
                caption: 'Amount',
                allowEditing: false
            },
            {
                dataField: '',
                caption: '',
                allowEditing: false
            },
            {
                dataField: 'buyer',
                caption: 'Buyer',
                width: 150
            },
            {
                dataField: 'bsodA_DATE',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy'
            },
            {
                dataField: 'bcond',
                caption: 'Cond',
            },
            {
                dataField: 'bqty',
                caption: 'Qty',
                setCellValue: function (newData, value, currentRowData) {
                    newData.bqty = value;
                    var qty = parseFloat(newData.bqty) || 0;
                    var rate = parseFloat(currentRowData.brate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    var perRate = parseFloat(rate / rT_TYPE) || 0;
                    if (perRate > -1 && perRate != 'Infinity') {
                        newData.bamt = (qty * perRate).toFixed(2);
                        empr_PurchaseSaleFormat.CalculateBuyerAverage(qty / rT_TYPE, qty * perRate);
                    } else {
                        newData.bamt = (0).toFixed(2);
                    }
                }
            },
            {
                dataField: 'brate',
                caption: 'Rate',
                setCellValue: function (newData, value, currentRowData) {
                    newData.brate = value;
                    var qty = parseFloat(currentRowData.bqty) || 0;
                    var rate = parseFloat(newData.brate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    var perRate = parseFloat(rate / rT_TYPE) || 0;
                    if (perRate > -1 && perRate != 'Infinity') {
                        newData.bamt = (qty * perRate).toFixed(2);
                        empr_PurchaseSaleFormat.CalculateBuyerAverage(qty / rT_TYPE, qty * perRate);
                    } else {
                        newData.bamt = (0).toFixed(2);
                    }
                }
            },
            {
                dataField: 'bamt',
                caption: 'Amount',
                allowEditing: false
            },
        ];
        empr_helper.editableDxGridbindingForPurchaseSale('#DetailContainer', col, dataSrc, "PurchaseSaleFormat");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }
    },

    CalculateBuyerAverage: function (quantity, amount) {
        var detailRecords = [];
        var gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.getDataSource();
        //var data = dataSource.items();
        var data = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        console.log("Buyer Data : ")
        console.log(data)

        var totalBqty = data.reduce(function (total, item) {
            return total + ((parseFloat(item.bqty) / parseFloat(item.rT_TYPE)) || 0);
        }, 0);
        var totalBamount = data.reduce(function (total, item) {
            return total + (parseFloat(item.bamt) || 0);
        }, 0);

        var hasZeroRate = data.some(function (item) {
            return parseFloat(item.brate) === 0;
        });
        console.log("Buyer Has Zero : " + hasZeroRate)

        if (hasZeroRate || amount == 0) {
            empr_helper.buyerAverage = (0).toFixed(2);
        } else {
            totalBqty = totalBqty + quantity;
            totalBamount = totalBamount + amount;
            empr_helper.buyerAverage = (totalBamount / totalBqty).toFixed(2);
            console.log(totalBamount / totalBqty);
        }
    },

    CalculateSellerAverage: function (quantity, amount) {
        debugger
        var detailRecords = [];
        var gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.getDataSource();
        var data = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        console.log("Seller Data : ")
        console.log(data)

        var totalBqty = data.reduce(function (total, item) {
            return total + ((parseFloat(item.sqty) / parseFloat(item.rT_TYPE)) || 0);
        }, 0);
        var totalBamount = data.reduce(function (total, item) {
            return total + (parseFloat(item.samt) || 0);
        }, 0);

        var hasZeroRate = data.some(function (item, index) {
            return parseFloat(item.srate) === 0 && index > 0;
        });
        console.log("Seller Has Zero : " + hasZeroRate)

        if (hasZeroRate || amount == 0) {
            empr_helper.sellerAverage = (0).toFixed(2);
        } else {
            totalBqty = totalBqty + quantity;
            totalBamount = totalBamount + amount;
            empr_helper.sellerAverage = (totalBamount / totalBqty).toFixed(2);
            console.log(totalBamount / totalBqty);
        }
    },

    CloneRow: function (index) {
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_PurchaseSaleFormat.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_PurchaseSaleFormat.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_PurchaseSaleFormat.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_PurchaseSaleFormat.GenerateKey(36);
                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
            }
        }
    },

    AddRow: function () {
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_PurchaseSaleFormat.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.push({ __KEY__: empr_PurchaseSaleFormat.GenerateKey(36), dC_TYPE: empr_PurchaseSaleFormat.DC_TYPE });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_PurchaseSaleFormat.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.push({ __KEY__: empr_PurchaseSaleFormat.GenerateKey(36), dC_TYPE: empr_PurchaseSaleFormat.DC_TYPE });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },

    DeleteRow: function (index, dtCode) {
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_PurchaseSaleFormat.rowsCount -= 1;
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
                        ajaxHelper.ajaxPostJsonData({ tranID: $('#Code').val(), code: dtCode }, "/PurchaseSaleFormat/DeletePurchaseSaleFormatDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_PurchaseSaleFormat.rowsCount -= 1;
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
        ajaxHelper.ajaxGetJson("/PurchaseSaleFormat/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseSaleFormat.InitItemCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitDropdownsWithValue: function (iCode) {
        ajaxHelper.ajaxGetJson("/PurchaseSaleFormat/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_PurchaseSaleFormat.InitItemCodeDDL(data.data, iCode);
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
        empr_PurchaseSaleFormat.GetPurchaseSaleFormats();
    },

    GetPurchaseSaleFormats: function () {
        ajaxHelper.ajaxGetJson('/PurchaseSaleFormat/GetPurchaseSaleFormats', function (data) {
            if (data.msgType == 1) {
                empr_PurchaseSaleFormat.CreateQuickSearchGrid(data.data);
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
            //{ dataField: 'seller', caption: 'Seller' },
            //{ dataField: 'samt', caption: 'Amount' },
            //{ dataField: 'buyer', caption: 'Buyer'  },
            //{ dataField: 'bamt', caption: 'Amount' },
            //{ dataField: 'traN_ID', caption: 'Code', visible: false },
            //{ dataField: 'astatus', caption: 'Status' },
        ];
        empr_helper.dxGridbindingForMultiBillPrint('#gridContainer', columns, dataSrc, "PurchaseSaleFormat");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/PurchaseSaleFormat/GetPurchaseSaleFormats", "traN_ID", "PurchaseSaleFormat");
    },

    ValidateInfo: function () {

        var valid = true;
        var sellerValid = true;
        var buyerValid = true;
        var data = empr_PurchaseSaleFormat.GetDataToSave();

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
            if (item.seller == "" || item.seller == null) {
                sellerValid = false;
            }

            if (item.buyer == "" || item.buyer == null) {
                buyerValid = false;
            }

            if (sellerValid == false && buyerValid == false) {
                empr_helper.notify("Please select seller or buyer at index " + index, 2);
                console.log("Seller & Buyer at index " + index + " has empty Code.");
                valid = false;
                return valid;
            }
        });

        if (!valid) {
            return;
        }
        else {
            return valid;
        }

        
    },

    SaveInfo: function () {
        var dataModel = empr_PurchaseSaleFormat.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseSaleFormat/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
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
                //empr_PurchaseSaleFormat.ResetForm();
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

        if (empr_PurchaseSaleFormat.rowsCount == detailRecords.length) {
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
            if (item.sodA_DATE != undefined) {
                item.sodA_DATE = empr_PurchaseSaleFormat.formatDateToDDMMYYYY(item.sodA_DATE);
            }
            if (item.bsodA_DATE != undefined) {
                item.bsodA_DATE = empr_PurchaseSaleFormat.formatDateToDDMMYYYY(item.bsodA_DATE);
            }
        });

        var totalBqty = modelRecord.Detail.reduce(function (total, item) {
            return total + ((parseFloat(item.bqty) / parseFloat(item.rT_TYPE)) || 0);
            //return total + (parseFloat(item.bqty) || 0);
        }, 0);
        var totalBamount = modelRecord.Detail.reduce(function (total, item) {
            return total + (parseFloat(item.bamt) || 0);
        }, 0);
        var hasZeroRateBuyer = modelRecord.Detail.some(function (item) {
            return parseFloat(item.brate) === 0 || item.brate === "" || item.brate === null;
        });
        if (hasZeroRateBuyer) {
            empr_helper.buyerAverage = 0;
        } else {
            empr_helper.buyerAverage = totalBamount / totalBqty;
        }

        var totalSqty = modelRecord.Detail.reduce(function (total, item) {
            return total + ((parseFloat(item.sqty) / parseFloat(item.rT_TYPE)) || 0);
            //return total + (parseFloat(item.sqty) || 0);
        }, 0);
        var totalSamount = modelRecord.Detail.reduce(function (total, item) {
            return total + (parseFloat(item.samt) || 0);
        }, 0);
        var hasZeroRateSeller = modelRecord.Detail.some(function (item) {
            return parseFloat(item.srate) === 0 || item.srate === "" || item.srate === null;
        });
        if (hasZeroRateSeller) {
            empr_helper.sellerAverage = 0;
        } else {
            empr_helper.sellerAverage = totalSamount / totalSqty;
        }

        return modelRecord;
    },

    formatDateToDDMMYYYY: function (dateStr) {
        const date = new Date(dateStr);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0'); // getMonth() is zero-indexed
        const year = date.getFullYear();
        return `${year}-${month}-${day}`;
    },

    GetPurchaseSaleFormatByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/PurchaseSaleFormat/GetPurchaseSaleFormatByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    empr_PurchaseSaleFormat.tranId = response.traN_ID;
                    empr_helper.selectedBills = [empr_PurchaseSaleFormat.tranId];
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
                    empr_PurchaseSaleFormat.CreateGrid(data.detail.data);

                    var totalBqty = data.detail.data.reduce(function (total, item) {
                        return total + ((parseFloat(item.bqty) / parseFloat(item.rT_TYPE)) || 0);
                        //return total + (parseFloat(item.bqty) || 0);
                    }, 0);
                    var totalBamount = data.detail.data.reduce(function (total, item) {
                        return total + (parseFloat(item.bamt) || 0);
                    }, 0);
                    var hasZeroRateBuyer = data.detail.data.some(function (item) {
                        return parseFloat(item.brate) === 0;
                    });
                    if (hasZeroRateBuyer) {
                        empr_helper.buyerAverage = 0;
                    } else {
                        empr_helper.buyerAverage = totalBamount / totalBqty;
                    }

                    var totalSqty = data.detail.data.reduce(function (total, item) {
                        //return total + (parseFloat(item.sqty) || 0);
                        return total + ((parseFloat(item.sqty) / parseFloat(item.rT_TYPE)) || 0);
                    }, 0);
                    var totalSamount = data.detail.data.reduce(function (total, item) {
                        return total + (parseFloat(item.samt) || 0);
                    }, 0);
                    var hasZeroRateSeller = data.detail.data.some(function (item) {
                        return parseFloat(item.srate) === 0;
                    });
                    if (hasZeroRateSeller) {
                        empr_helper.sellerAverage = 0;
                    } else {
                        empr_helper.sellerAverage = totalSamount / totalSqty;
                    }
                    $('#gridDiv').hide();
                    $('.formDiv').show();
                    $('.card-body').addClass('customHighlightForModifiedCells');
                    $("#Loader").hide();
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                    $("#Loader").hide();
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
                $("#Loader").hide();
            }
        }, false, true);
    },

    GetPurchaseSaleFormatDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/PurchaseSaleFormat/GetPurchaseSaleFormatDetailsByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_PurchaseSaleFormat.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/PurchaseSaleFormat/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_PurchaseSaleFormat.ResetForm();
                    empr_PurchaseSaleFormat.InitQuickSearchGrid();
                    $('#gridDiv').show();
                    $('.formDiv').hide();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/PurchaseSaleFormat/GetReportTypes", function (data) {
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
        debugger
        let TRAN_ID = empr_PurchaseSaleFormat.tranId;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseSaleFormat/GetPrintReport", function (data) {
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