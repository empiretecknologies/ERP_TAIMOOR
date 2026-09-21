var empr_CashReceiptVoucher = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    DC_TYPE1: '',
    isSavingProcess: false,

    InitEvents: function () {
        $(document).ready(function () {
            //console.log('account', Accounts);
            empr_CashReceiptVoucher.ResetForm();
            empr_CashReceiptVoucher.InitReportTypeDDL();
            empr_CashReceiptVoucher.InitQuickSearchGrid();
            //empr_CashReceiptVoucher.InitChargesDDL();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_CashReceiptVoucher.GetCashReceiptVoucherByCode(data.traN_ID);
                }
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_CashReceiptVoucher.InitQuickSearchGrid();
            });

            $('body').on('click', '#docBrowseBtn', function () {
                $('#DOC').val('');
                $('#hdnDOC').val('');
                $('#DOCName').val('');
                $('#DOC').click();
            });

            $('#V_DATE').blur(function () {
                var selectedDate = $(this).val();
                //console.log('User selected date:', selectedDate);

                let gridInstance = $("#DetailContainer").dxDataGrid("instance");
                let rowCount = gridInstance.getDataSource().items().length;

                for (let i = 0; i < rowCount; i++) {
                    gridInstance.cellValue(i, "chQ_DATE", selectedDate);
                }
            });

            $('body').on('keydown', '#DC_AMOUNT', function (e) {
                if (e.key === 'ArrowDown') {
                    empr_CashReceiptVoucher.SaveDetailCharges();
                }
            });

            $('body').on('click', '#BtnAddDetailCharges', function () {
                empr_CashReceiptVoucher.SaveDetailCharges();
            });

            //$('body').on('click', '#BtnSave', function () {
            //    $('#BtnSave').hide();
            //    if (Permissions != "Admin") {
            //        if (!$("#Code").val() && !Permissions.r_ADD) {
            //            empr_helper.notify("You are not allowed to add new record !", 2);
            //        }
            //        else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
            //            empr_helper.notify("You are not allowed to edit records !", 2);
            //        } else {
            //            empr_CashReceiptVoucher.ValidateAndPrepareDataForSave();
            //        }
            //    } else {
            //        empr_CashReceiptVoucher.ValidateAndPrepareDataForSave();
            //    }
            //});

            $('body').on('click', '#BtnSave', function (e) {
                //e.preventDefault(); // Default behavior ko rokne ke liye
                //debugger;
                //var $btn = $(this);
                //if (empr_CashReceiptVoucher.isSavingProcess = false || $btn.prop('disabled') || $btn.hasClass('inside-process')) {
                //    return false;
                //}
                ////if ($('#BtnSave').prop('disabled')) {
                ////    return false;SS
                ////}
                //$('#BtnSave').prop('disabled', true).hide();
                //empr_CashReceiptVoucher.isSavingProcess = true;
                //$btn.prop('disabled', true).addClass('inside-process').hide();
                //$btn.prop('disabled', true).addClass('inside-process');
                $("#Loader").show().css('display', 'flex');
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                        //empr_CashReceiptVoucher.isSavingProcess = false;
                        //$btn.prop('disabled', false).removeClass('inside-process');                 
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                        //empr_CashReceiptVoucher.isSavingProcess = false;
                        // $btn.prop('disabled', false).removeClass('inside-process');
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);
                    } else {
                        empr_CashReceiptVoucher.ValidateAndPrepareDataForSave();
                        //empr_CashReceiptVoucher.isSavingProcess = false
                        //$btn.prop('disabled', false).removeClass('inside-process');\
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);

                    }
                } else {
                    empr_CashReceiptVoucher.ValidateAndPrepareDataForSave();
                    //empr_CashReceiptVoucher.isSavingProcess = false
                    //$btn.prop('disabled', false).removeClass('inside-process');
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
                //$('#BtnSave').prop('disabled', false).show();
            });

            $('body').on('click', '#DetailChargesSave', function () {
                //console.log('dataClear', dataClear);
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        empr_CashReceiptVoucher.SaveDetailCharges();
                    }
                } else {
                    empr_CashReceiptVoucher.SaveDetailCharges();
                }
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_CashReceiptVoucher.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_CashReceiptVoucher.GetCashReceiptVoucherByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/CashReceiptVoucher/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_CashReceiptVoucher.GetCashReceiptVoucherByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_CashReceiptVoucher.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_CashReceiptVoucher.ResetForm();
            });

            $('body').on('click', '.btn-print,#BtnGenerateReport', function () {
                empr_CashReceiptVoucher.GeneratePrintReport();
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
        empr_CashReceiptVoucher.CreateGrid([{ __KEY__: empr_CashReceiptVoucher.GenerateKey(36), dC_TYPE: empr_CashReceiptVoucher.DC_TYPE, chQ_DATE: todayDate }]);
        empr_CashReceiptVoucher.CreateDetailChargesGrid([{ __KEY__: empr_CashReceiptVoucher.GenerateKey(36), dC_TYPE1: empr_CashReceiptVoucher.DC_TYPE1 }]);
        $('.Record input').not(' .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
        $('#hdnDOC').val('');
        $('#ASTATUS').val(null);
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        empr_CashReceiptVoucher.InitCurrencyDDL();
        empr_CashReceiptVoucher.InitBookTypeDDL();
        $('#V_DATE').val(todayDate);
        $('#V_DATE').focus();

        //empr_CashReceiptVoucher.InitChargesDDL();
        $('#DC_AMOUNT').val('');
        $('#hidden_tranId').val('');
        $('#hidden_dtCode').val('');

        $('#hidden_partyCode').val('');
        $('#hidden_actCode').val('')

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
        //console.log('CreateGrid', dataSrc);
        if (dataSrc.length > 0) {
            empr_CashReceiptVoucher.rowsCount = dataSrc.length - 1;
            dataSrc.forEach(item => {
                if (
                    item.chQ_DATE == '1900-01-01' || item.chQ_DATE == '01-01-1900' || item.chQ_DATE == '01-Jan-1900' || item.chQ_DATE == '1/1/1900 12:00:00 AM' || item.chQ_DATE == '01/01/1900 12:00:00 AM' || item.chQ_DATE == '1/1/1900' ||
                    item.chQ_DATE == '2000-01-01' || item.chQ_DATE == '01-01-2000' || item.chQ_DATE == '01-Jan-2000' || item.chQ_DATE == '1/1/2000 12:00:00 AM' || item.chQ_DATE == '01/01/2000 12:00:00 AM' || item.chQ_DATE == '1/1/2000' ||
                    item.chQ_DATE == '00-01-01' || item.chQ_DATE == '01-01-00' || item.chQ_DATE == '01-Jan-00' || item.chQ_DATE == '1/1/00 12:00:00 AM' || item.chQ_DATE == '01/01/00 12:00:00 AM' || item.chQ_DATE == '1/1/00' || item.chQ_DATE == '01-Jan-00 12:00:00 AM'
                ) {
                    item.chQ_DATE = null;
                }
            });
        }
        var col = [
            {
                dataField: "Action",
                width: 150,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    //console.log('options', options);
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_CashReceiptVoucher.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 10px" onclick="empr_CashReceiptVoucher.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 10px" onclick="empr_CashReceiptVoucher.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        let costCenterAction = '';
                        let knockOffAction = '';
                        let bCharges = '';
                        debugger;
                        if ($('#Code').val() != '') {
                            if (options.data.dC_TYPE === "D" && options.data.partY_CODE == 0 && options.data.dT_CODE) {
                                costCenterAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 10px; color:#FFD700" onclick="empr_CashReceiptVoucher.ShowCostCenterModal(${options.data.traN_ID},${options.data.dT_CODE},'${options.data.dT_DESC}',${options.data.amt},${options.data.partY_CODE})"><i class="fa fa-coins"></i></a>`;
                            }
                            if (options.data.partY_CODE > 0 && options.data.dT_CODE) {
                                debugger;
                                //console.log('options', options);
                                knockOffAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 10px;" title="KnockOff" onclick="empr_CashReceiptVoucher.ShowKnockOffModal(${options.data.traN_ID},${options.data.dT_CODE},${options.data.amt},${options.data.partY_CODE},${options.data.acT_CODE},'${options.data.partY_NAME}','${options.data.booK_NAME}','${options.data.whT_RATE}')"><i class="fa fa-link"></i></a>`;
                            }
                            if (options.data.dT_CODE > 0) {
                                bCharges = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 10px;" title="Charges" onclick="empr_CashReceiptVoucher.DetailCharges(${options.data.traN_ID},${options.data.dT_CODE},${options.data.amt},${options.data.partY_CODE},${options.data.acT_CODE})"><i class="fa fa-info-circle"></i></a>`;
                            }
                        }
                        $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_CashReceiptVoucher.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                       <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 10px" onclick="empr_CashReceiptVoucher.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 10px" onclick="empr_CashReceiptVoucher.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                       ${costCenterAction}
                       ${knockOffAction}
                       ${bCharges}
                       </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            //{
            //    dataField: 'custoM_ACT_CODE',
            //    caption: 'Account',
            //    width: 450,
            //    allowSorting: false,
            //    lookup: {
            //        dataSource: {
            //            store: Accounts,
            //            paginate: true,
            //            pageSize: 50
            //        },
            //        displayExpr: 'value',
            //        valueExpr: 'key',
            //        searchEnabled: true,
            //        showClearButton: true,
            //        paging: {
            //            enabled: true,
            //            pageSize: 50,
            //        }
            //    },
            //    setCellValue: function (newData, value, currentRowData) {
            //        var selectedAccount = Accounts.filter(u => u.key == value);
            //        if (selectedAccount.length > 0) {
            //            newData.custoM_ACT_CODE = selectedAccount[0].key;
            //            newData.acT_CODE = selectedAccount[0].accountCode;
            //            newData.partY_CODE = selectedAccount[0].partyCode;
            //            newData.wht = selectedAccount[0].wht;
            //        }
            //    }
            //},
            {
                dataField: 'custoM_ACT_CODE',
                caption: 'Account',
                width: 450,
                allowSorting: false,
                lookup: {
                    dataSource: {
                        store: Accounts,
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    showClearButton: true
                },
                setCellValue: function (newData, value, currentRowData) {
                    var selectedAccount = Accounts.filter(u => u.key == value);

                    if (selectedAccount.length > 0) {
                        newData.custoM_ACT_CODE = selectedAccount[0].key;
                        newData.acT_CODE = selectedAccount[0].accountCode;
                        newData.partY_CODE = selectedAccount[0].partyCode;
                        newData.wht = selectedAccount[0].wht || 0;
                    } else {
                        newData.wht = 0;
                    }

                    // 🔥 YEH LINE MISSING THI
                    newData.amt = currentRowData.amt || 0;
                    empr_CashReceiptVoucher.calculateAmounts(newData);
                }
            },
            {
                dataField: 'dC_TYPE',
                caption: 'Type',
                width: 100,
                allowSorting: false,
                lookup: {
                    dataSource: [
                        { key: 'C', value: 'CREDIT' },
                        { key: 'D', value: 'DEBIT' },
                    ],
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
            },
            //{
            //    dataField: 'amt',
            //    caption: 'Amount',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 0 }
            //},
            //{
            //    dataField: 'wht',
            //    caption: 'WHT',
            //    dataType: 'number',
            //    //format: { type: 'fixedPoint', precision: 2 }
            //},
            //{
            //    dataField: 'whT_AMT',
            //    caption: 'WHT Amount',
            //    dataType: 'number',
            //    /*format: { type: 'fixedPoint', precision: 0 }*/
            //},
            //{
            //    dataField: 'neT_AMT',
            //    caption: 'Net Amount',
            //    dataType: 'number',
            //    /*format: { type: 'fixedPoint', precision: 0 }*/
            //},
            {
                dataField: 'amt',
                caption: 'Amount',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 2 },
                setCellValue: function (newData, value, currentRowData) {
                    newData.amt = value || 0;
                    newData.wht = currentRowData.wht || 0;

                    empr_CashReceiptVoucher.calculateAmounts(newData);
                }
            },
            //{
            //    dataField: 'wht',
            //    caption: 'WHT %',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    setCellValue: function (newData, value, currentRowData) {
            //        newData.wht = value || 0;
            //        newData.amt = currentRowData.amt || 0;

            //        empr_CashReceiptVoucher.calculateAmounts(newData);
            //    }
            //},
            //{
            //    dataField: 'whT_AMT',
            //    caption: 'WHT Amount',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    allowEditing: false
            //},
            //{
            //    dataField: 'neT_AMT',
            //    caption: 'Net Amount',
            //    dataType: 'number',
            //    format: { type: 'fixedPoint', precision: 2 },
            //    allowEditing: false
            //},
            {
                dataField: 'dT_DESC',
                caption: 'Description',
            },
            {
                dataField: 'chQ_NO',
                caption: 'Ref #',
            },
            {
                dataField: 'chQ_DATE',
                caption: 'Ref Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) {
                            year += 2000;
                        }
                        var formattedDate = new Date(year, date.getMonth(), date.getDate());
                        newData.chQ_DATE = formattedDate;
                    } else {
                        newData.chQ_DATE = value;
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
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "CashReceiptVoucher", "custoM_ACT_CODE");
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

    calculateAmounts: function (row) {
        var amt = row.amt || 0;
        var wht = row.wht || 0;

        row.whT_AMT = (amt * wht) / 100;
        row.neT_AMT = amt - row.whT_AMT;
    },

    GetRecordsInSaveSequence: function (records) {
        if (!Array.isArray(records)) {
            return records;
        }
        var existing = [];
        var newlyAdded = [];
        $.each(records, function (index, item) {
            if (item.dT_CODE == '' || item.dT_CODE == null || item.dT_CODE == undefined || item.dT_CODE == 0) {
                newlyAdded.push(item);
            } else {
                existing.push(item);
            }
        });
        existing.sort(function (a, b) {
            return parseInt(a.dT_CODE) - parseInt(b.dT_CODE);
        });
        newlyAdded.reverse();
        return existing.concat(newlyAdded);
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

                empr_CashReceiptVoucher.rowsCount += 1;
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
                    clonedRowData.__KEY__ = empr_CashReceiptVoucher.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_CashReceiptVoucher.rowsCount += 1;
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
                clonedRowData.__KEY__ = empr_CashReceiptVoucher.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
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
                empr_CashReceiptVoucher.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_CashReceiptVoucher.GenerateKey(36), dC_TYPE: empr_CashReceiptVoucher.DC_TYPE });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_CashReceiptVoucher.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_CashReceiptVoucher.GenerateKey(36), dC_TYPE: empr_CashReceiptVoucher.DC_TYPE });
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
                    empr_CashReceiptVoucher.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ tranID: $('#Code').val(), code: dtCode }, "/CashReceiptVoucher/DeleteCashReceiptVoucherDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_CashReceiptVoucher.rowsCount -= 1;
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
                var row = dataSource[index];
                if (dtCode != '' && dtCode != null && dtCode != undefined) {
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
                            ajaxHelper.ajaxPostJsonData({ tranID: $('#Code').val(), code: dtCode }, "/CashReceiptVoucher/DeleteCashReceiptVoucherDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    empr_CashReceiptVoucher.CreateGrid([{ __KEY__: empr_CashReceiptVoucher.GenerateKey(36), dC_TYPE: empr_CashReceiptVoucher.DC_TYPE }]);
                                }
                            }, false, true);
                        });
                    } else {
                        empr_helper.notify("You are not allowed to delete the last row.", 2);
                    }
                } else {
                    empr_CashReceiptVoucher.CreateGrid([{ __KEY__: empr_CashReceiptVoucher.GenerateKey(36), dC_TYPE: empr_CashReceiptVoucher.DC_TYPE }]);
                    empr_helper.notify("You are not allowed to delete the last row.", 2);
                }
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

    InitCurrencyDDL: function (_selectedValue) {
        $.ajax({
            url: 'CashReceiptVoucher/GetCurrencies',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    if (_selectedValue == undefined || _selectedValue == null) {
                        _selectedValue = data.data[0].key;
                        $('#Rate').val(data.data[0].rate);
                    }
                    $('#Currency').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                            debugger;
                            if (e.value != '' && e.value != null) {
                                var items = e.component._dataSource._items;
                                var item = items.filter(i => i.key == e.value);
                                if (item.length > 0) {
                                    $('#Rate').val(item[0].rate);
                                }
                            }
                            else {
                                $('#Rate').val('');
                            }
                        },
                    });
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                //console.error('Error fetching data:', error);
            }
        });
    },

    InitBookTypeDDL: function (_selectedValue) {
        $.ajax({
            url: 'CashReceiptVoucher/GetBookTypes',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#BookType').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                //console.error('Error fetching data:', error);
            }
        });
    },

    InitQuickSearchGrid: function () {
        empr_CashReceiptVoucher.GetCashReceiptVouchers();
    },

    GetCashReceiptVouchers: function () {
        ajaxHelper.ajaxGetJson('/CashReceiptVoucher/GetCashReceiptVouchers', function (data) {
            if (data.msgType == 1) {
                empr_CashReceiptVoucher.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {
        var columns = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {

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

        { dataField: 'traN_ID', caption: 'Code', visible: false },
        { dataField: 'dT_CODE', caption: 'DetailCode', visible: false },
        { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'astatus', caption: 'Status', },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'booK_TYPE', caption: 'Book Type' },
        { dataField: 'curR_CODE', caption: 'Currency', },
        { dataField: 'crate', caption: 'Currency Rate', },
        { dataField: 'remarks', caption: 'Remarks', },
        { dataField: 'partY_NAME', caption: 'Account', },
        { dataField: 'dC_TYPE', caption: 'Type', },
        { dataField: 'dT_DESC', caption: 'Desc', },
        { dataField: 'amt', caption: 'Amount', },
        { dataField: 'chQ_NO', caption: 'Ref #', },
        { dataField: 'chQ_DATE', caption: 'Ref Date', },
        { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
        { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
        { dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
        { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
        { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
        { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
        { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        { dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, }
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "CashReceiptVoucherQS", "multiple");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/CashReceiptVoucher/GetCashReceiptVouchers", "dT_CODE", "CashReceiptVoucher", "multiple");
    },

    ValidateAndPrepareDataForSave: function () {
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                var IsValid = true;
                var V_DATE = $("#V_DATE").val();
                var BOOK_TYPE = $('#BookType').dxSelectBox('option', 'value');

                if (V_DATE == '' || V_DATE == null || V_DATE == undefined) {
                    empr_helper.notify("Transaction date is required.", 2);
                    IsValid = false;
                    return false;
                }

                if (BOOK_TYPE == '' || BOOK_TYPE == null || BOOK_TYPE == undefined) {
                    empr_helper.notify("Please select book type.", 2);
                    IsValid = false;
                    return false;
                }

                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
                if (Array.isArray(detailRecords) && detailRecords.some(item => item.key !== undefined)) {
                    detailRecords = detailRecords.flatMap(group => group.items || []);
                }
                //console.log(detailRecords)

                $.each(detailRecords, function (index, item) {
                    if (item.acT_CODE == "" || item.acT_CODE == null || item.acT_CODE == undefined) {
                        empr_helper.notify("Please select account name at index " + index, 2);
                        IsValid = false;
                        return false;
                        //console.log("Account at index " + index + " has empty account code.");
                    }

                    if (item.dC_TYPE == "" || item.dC_TYPE == null || item.dC_TYPE == undefined) {
                        empr_helper.notify("Please enter type at index " + index, 2);
                        IsValid = false;
                        return false;
                        //console.log("Account at index " + index + " has empty type.");
                    }

                    if (item.amt == "" || item.amt == null || item.amt == undefined) {
                        empr_helper.notify("Please enter amount at index " + index, 2);
                        IsValid = false;
                        return false;
                        //console.log("Account at index " + index + " has empty amount.");
                    }

                    if (!(item.chQ_DATE == "" || item.chQ_DATE == null || item.chQ_DATE == undefined)) {
                        item.chQ_DATE = empr_helper.PrepareDate(item.chQ_DATE);
                    }
                });
                if (!IsValid) return;
                IsValid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);
                if (IsValid) {
                    const dcTypeRows = detailRecords.filter(item => item.dC_TYPE === empr_CashReceiptVoucher.DC_TYPE);
                    if (dcTypeRows.length > 0) {
                        if (empr_CashReceiptVoucher.DC_TYPE === "D") {
                            const debitRows = detailRecords.filter(item => item.dC_TYPE === "D");
                            const creditRows = detailRecords.filter(item => item.dC_TYPE === "C");
                            const debitSum = debitRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                            const creditSum = creditRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                            if (creditSum > debitSum) {
                                empr_helper.notify("Credit amount must be less than or equal to debit amount.", 2);
                                IsValid = false;
                            }
                        }
                        if (empr_CashReceiptVoucher.DC_TYPE === "C") {
                            const debitRows = detailRecords.filter(item => item.dC_TYPE === "D");
                            const creditRows = detailRecords.filter(item => item.dC_TYPE === "C");
                            const debitSum = debitRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                            const creditSum = creditRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                            if (debitSum > creditSum) {
                                empr_helper.notify("Debit amount must be less than or equal to credit amount.", 2);
                                IsValid = false;
                            }
                        }
                    }
                    else {
                        var dcType = empr_CashReceiptVoucher.DC_TYPE === "D" ? "debit" : "credit";
                        empr_helper.notify("Voucher must have at least one " + dcType + " row.", 2);
                        IsValid = false;
                    }
                }
                if (IsValid) {
                    //if ($("#Code").val() == 0
                    //    || $("#Code").val() == null
                    //    || $("#Code").val() == undefined
                    //    || $("#Code").val() == "") {
                    //    detailRecords.reverse();
                    //}

                    detailRecords = empr_CashReceiptVoucher.GetRecordsInSaveSequence(detailRecords);
                    detailRecords.forEach(obj => {
                        obj.TRAN_ID = $("#Code").val();
                        obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
                        obj.V_DATE = $("#V_DATE").val();
                        obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                        obj.CURR_CODE = $('#Currency').dxSelectBox('option', 'value');
                        obj.BOOK_TYPE = $('#BookType').dxSelectBox('option', 'value');
                        obj.CRATE = $("#Rate").val();
                        obj.REMARKS = $("#REMARKS").val();
                        obj.DOC = $("#hdnDOC").val();
                    });

                    empr_CashReceiptVoucher.SaveInfo(detailRecords);
                }
            });
        }
        else {
            var IsValid = true;
            var V_DATE = $("#V_DATE").val();
            var BOOK_TYPE = $('#BookType').dxSelectBox('option', 'value');

            if (V_DATE == '' || V_DATE == null || V_DATE == undefined) {
                empr_helper.notify("Transaction date is required.", 2);
                IsValid = false;
                return false;
            }

            if (BOOK_TYPE == '' || BOOK_TYPE == null || BOOK_TYPE == undefined) {
                empr_helper.notify("Please select book type.", 2);
                IsValid = false;
                return false;
            }

            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

            detailRecords.forEach(obj => {
                obj.booK_TYPE = BOOK_TYPE;
            });
            $.each(detailRecords, function (index, item) {

                if (item.acT_CODE == "" || item.acT_CODE == null || item.acT_CODE == undefined) {
                    empr_helper.notify("Please select account name at index " + index, 2);
                    IsValid = false;
                    return false;
                    //console.log("Account at index " + index + " has empty account code.");
                }

                if (item.dC_TYPE == "" || item.dC_TYPE == null || item.dC_TYPE == undefined) {
                    empr_helper.notify("Please enter type at index " + index, 2);
                    IsValid = false;
                    return false;
                    //console.log("Account at index " + index + " has empty type.");
                }

                if (item.amt == "" || item.amt == null || item.amt == undefined) {
                    empr_helper.notify("Please enter amount at index " + index, 2);
                    IsValid = false;
                    return false;
                    //console.log("Account at index " + index + " has empty amount.");
                }

                if (!(item.chQ_DATE == "" || item.chQ_DATE == null || item.chQ_DATE == undefined)) {
                    item.chQ_DATE = empr_helper.PrepareDate(item.chQ_DATE);
                }
            });

            if (!IsValid) return;

            IsValid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

            if (IsValid) {
                const dcTypeRows = detailRecords.filter(item => item.dC_TYPE === empr_CashReceiptVoucher.DC_TYPE);
                if (dcTypeRows.length > 0) {
                    if (empr_CashReceiptVoucher.DC_TYPE === "D") {
                        const debitRows = detailRecords.filter(item => item.dC_TYPE === "D");
                        const creditRows = detailRecords.filter(item => item.dC_TYPE === "C");
                        const debitSum = debitRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                        const creditSum = creditRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                        if (creditSum > debitSum) {
                            empr_helper.notify("Credit amount must be less than or equal to debit amount.", 2);
                            IsValid = false;
                        }
                    }

                    if (empr_CashReceiptVoucher.DC_TYPE === "C") {
                        const debitRows = detailRecords.filter(item => item.dC_TYPE === "D");
                        const creditRows = detailRecords.filter(item => item.dC_TYPE === "C");
                        const debitSum = debitRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                        const creditSum = creditRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                        if (debitSum > creditSum) {
                            empr_helper.notify("Debit amount must be less than or equal to credit amount.", 2);
                            IsValid = false;
                        }
                    }
                }
                else {
                    var dcType = empr_CashReceiptVoucher.DC_TYPE === "D" ? "debit" : "credit";
                    empr_helper.notify("Voucher must have at least one " + dcType + " row.", 2);
                    IsValid = false;
                }
            }

            if (IsValid) {
                detailRecords = empr_CashReceiptVoucher.GetRecordsInSaveSequence(detailRecords);
                detailRecords.forEach(obj => {
                    obj.TRAN_ID = $("#Code").val();
                    obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
                    obj.V_DATE = $("#V_DATE").val();
                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                    obj.CURR_CODE = $('#Currency').dxSelectBox('option', 'value');
                    obj.BOOK_TYPE = $('#BookType').dxSelectBox('option', 'value');
                    obj.CRATE = $("#Rate").val();
                    obj.REMARKS = $("#REMARKS").val();
                    obj.DOC = $("#hdnDOC").val();
                });

                empr_CashReceiptVoucher.SaveInfo(detailRecords);
            }
        }
    },

    //SaveInfo: function (detailRecords) {
    //    console.log("saveinfo", detailRecords)

    //    ajaxHelper.ajaxPostJsonData({ modelRecord: detailRecords }, "/CashReceiptVoucher/Save", function (data) {
    //        $('#BtnSave').show();
    //        empr_helper.notify(data.msg, data.msgType);

    //        if (data.msgType == 1) {
    //            if ($("#Code").val() == 0
    //                || $("#Code").val() == null
    //                || $("#Code").val() == undefined
    //                || $("#Code").val() == "") {
    //                $('#Code').val(data.data.code);
    //                empr_helper.selectedBill = data.data.code;
    //                $('#VOUCHER_NO').val(data.data.voucherNo);
    //            }
    //            if (dataClear == 1) {
    //                empr_CashReceiptVoucher.GetCashReceiptVoucherDetailsByCode($("#Code").val());
    //                if (Permissions != "Admin") {
    //                    if (Permissions.r_DLT) {
    //                        $('#BtnDelete').show();
    //                    }
    //                } else {
    //                    $('#BtnDelete').show();
    //                }
    //            }
    //            else {
    //                empr_CashReceiptVoucher.ResetForm();
    //            }
    //        }
    //        else {
    //            empr_helper.notify(data.msgError, 2);
    //        }
    //    }, false, true);
    //},

    SaveInfo: function (detailRecords) {
        console.log("saveinfo", detailRecords)
        //var $btn = $('#BtnSave');
        ajaxHelper.ajaxPostJsonData({ modelRecord: detailRecords }, "/CashReceiptVoucher/Save", function (data) {


            empr_helper.notify(data.msg, data.msgType);

            if (data.msgType == 1) {
                if ($("#Code").val() == 0 || $("#Code").val() == null || $("#Code").val() == undefined || $("#Code").val() == "") {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                if (dataClear == 1) {
                    empr_CashReceiptVoucher.GetCashReceiptVoucherDetailsByCode($("#Code").val());
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                    } else {
                        $('#BtnDelete').show();
                    }
                    //$btn.prop('disabled', false).show();
                }
                else {
                    empr_CashReceiptVoucher.ResetForm();
                    //empr_CashReceiptVoucher.isSavingProcess = false;
                    //$btn.prop('disabled', false).removeClass('inside-process');
                }
            }
            else {
                empr_helper.notify(data.msgError, 2);
                //empr_CashReceiptVoucher.isSavingProcess = false;
                // $btn.prop('disabled', false).removeClass('inside-process');
            }
        }, false, true);
    },



    UploadDoc: function () {
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        if (files.length > 0) {
            $('#DOCName').val(files[0].name);
        }

        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax({
            url: "/Common/UploadVoucherDocs",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    $("#hdnDOC").val(data.data);
                    empr_helper.notify("File uploaded successfully.", 1);
                } else {
                    $('#DOCName').val("No File");
                    empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                }
                $('#saveAttempt').prop('disabled', false);
            }
        });
    },

    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (!hdnUrl) {
            empr_helper.notify("Please upload a file to view.", 2);
        } else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    //UploadDoc: function () {
    //    $('#BtnSave').prop('disabled', true);
    //    var files = document.getElementById('DOC').files;
    //    var formData = new FormData();
    //    for (var i = 0; i !== files.length; i++) {
    //        formData.append("model", files[i]);
    //    }
    //    $.ajax(
    //        {
    //            url: "/Common/UploadVoucherDocs",
    //            data: formData,
    //            processData: false,
    //            contentType: false,
    //            type: "POST",
    //            success: function (data) {
    //                if (data.msgType == '1') {
    //                    $("#hdnDOC").val(data.data);
    //                }
    //                else {
    //                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
    //                }
    //                $('#BtnSave').prop('disabled', false);

    //            }
    //        }
    //    );
    //},

    //OpenDoc: function () {
    //    var hdnUrl = $('#hdnDOC').val();
    //    if (hdnUrl == "" || hdnUrl == null) {
    //        empr_helper.notify("Please upload a file to view.", 2);
    //    }
    //    else {
    //        const fileURL = window.location.origin + hdnUrl;
    //        window.open(fileURL, '_blank');
    //    }
    //},

    GetCashReceiptVoucherByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/CashReceiptVoucher/GetCashReceiptVoucherByCode?code=' + code, function (data) {
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#Currency').dxSelectBox('instance').option('value', response.curR_CODE);
                    $('#BookType').dxSelectBox('instance').option('value', response.booK_TYPE);
                    $('#Rate').val(response.crate);
                    $('#hdnDOC').val(response.doc);
                    var DocPath = response.doc;
                    var DocName = DocPath.split('/').pop();
                    $("#DOCName").val(DocName);
                    $('#REMARKS').val(response.remarks);
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
                    empr_CashReceiptVoucher.CreateGrid(data.detail.data);
                    $('.card-body').addClass('customHighlightForModifiedCells');
                    $("#Loader").hide();
                }
                else {
                    empr_helper.notify("2" + data.msg, data.msgType);
                    $("#Loader").hide();
                }
            }
            else {
                empr_helper.notify("1" + data.msg, data.msgType);
                $("#Loader").hide();
            }
        }, false, true);
    },

    GetCashReceiptVoucherDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/CashReceiptVoucher/GetCashReceiptVoucherDetailsByCode?code=' + code, function (data) {
            //console.log(data)
            if (data.msgType == 1) {
                empr_CashReceiptVoucher.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
            //empr_CashReceiptVoucher.isSavingProcess = false;
            //var $btn = $('#BtnSave');
            //$btn.prop('disabled', false).removeClass('inside-process');
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/CashReceiptVoucher/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_CashReceiptVoucher.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/CashReceiptVoucher/GetReportTypes", function (data) {
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
        empr_CashReceiptVoucher.InitReportTypeDDL();
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/CashReceiptVoucher/GetPrintReport", function (data) {
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

    ShowCostCenterModal: function (tranId, dtCode, desc, amt, partyCode) {
        //console.log('partyCode', partyCode);
        if (amt == null || amt === undefined || amt <= 0) {
            empr_helper.notify('Amount should be greater than zero.', 2);
            return;
        }
        if (partyCode > 0) {
            empr_helper.notify('This Account not allow to add Cost Center.', 2);
            return;
        }

        var obj = {
            TranId: tranId,
            DtCode: dtCode,
            DESCR: desc,
            Amt: amt
        };

        $.ajax({
            url: 'CostCenter/Index',
            method: 'GET',
            data: obj,
            success: function (result) {
                $('#costCenterModalBody').html(result);
                $('#costCenterModal').modal('show');
            },
            error: function (error) {
                //console.error('Error fetching data:', error);
            }
        });
    },

    ShowKnockOffModal: function (tranId, dtCode, amt, partyCode, actCode, partyName, bookName, whtRate) {
        //if (amt == null || amt === undefined || amt <= 0) {
        //    empr_helper.notify('Amount should be greater than zero.', 2);
        //    return;
        //}
        //if (partyCode > 0) {
        //    empr_helper.notify('This Account not allow to add Cost Center.', 2);
        //    return;
        //}
        debugger;
        var SumDetailCharges = 0;

        ajaxHelper.ajaxGetJson('/CashReceiptVoucher/GetSumDetailCharges?tranId=' + tranId + '&dtCode=' + dtCode, function (data) {
            debugger;
            if (data.msgType == 1) {
                SumDetailCharges = data.data;

                var finalAmt = amt + SumDetailCharges;
                var obj = {
                    TRAN_ID: tranId,
                    DT_CODE: dtCode,
                    PARTY_CODE: partyCode,
                    ACT_CODE: actCode,
                    AMOUNT: finalAmt,
                    PARTY_NAME: partyName,
                    BOOK_NAME: bookName,
                    WHT_RATE: whtRate
                };
                $.ajax({
                    url: '/KnockOff/Index',
                    method: 'GET',
                    data: obj,
                    success: function (result) {
                        $('#knockOffModalBody').html(result);
                        $('#knockOffModal').modal('show');
                    },
                    error: function (error) {
                        //console.error('Error fetching data:', error);
                    }
                });
            }
            else {
                empr_helper.notify(data.msgError, 2);
            }
        }, false, true);

    },

    DetailCharges: function (tranId, dtCode, amt, partyCode, actCode) {
        $('#hidden_tranId').val(tranId);
        $('#hidden_dtCode').val(dtCode);
        $('#hidden_partyCode').val(partyCode);
        $('#hidden_actCode').val(actCode);
        //$('#hidden_amt').val(amt);

        empr_CashReceiptVoucher.GetDetailCharges(tranId, dtCode, partyCode, actCode);

        $('#DetailCharges').modal('show');

    },

    GetDetailCharges: function (tranId, dtCode, partyCode, actCode) {
        ajaxHelper.ajaxGetJson('/CashReceiptVoucher/GetDetailCharges?tranId=' + tranId + '&dtCode=' + dtCode + '&partyCode=' + partyCode + '&actCode=' + actCode, function (data) {
            //console.log('GetDetailCharges', data);
            debugger;
            if (data.data.msgType == 1) {
                empr_CashReceiptVoucher.CreateDetailChargesGrid(data.data.data);
            }
            else {
                empr_helper.notify(data.msgError, 2);
            }
        }, false, true);
    },

    //InitChargesDDL: function (selectedValue) {
    //    $('#chargesDDL').dxSelectBox({
    //        dataSource: DetailCharges,
    //        displayExpr: 'value',
    //        valueExpr: 'key',
    //        value: selectedValue,
    //        searchEnabled: true,
    //        width: '100%',
    //        placeholder: 'Search',
    //        showClearButton: true,
    //        dropDownOptions: {
    //            height: 'auto',
    //        },
    //        pagingEnabled: true,
    //        searchTimeout: 500,
    //        onValueChanged: function (e) {
    //        },
    //    });
    //},

    CreateDetailChargesGrid: function (dataSrc) {
        //console.log('CreateDetailChargesGrid', dataSrc);
        if (dataSrc.length == 0) {
            daraSrc = [{ code: 0, amt: 0 }]
        }
        var col = [
            {
                dataField: "Action",
                width: 50,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_CashReceiptVoucher.DeleteDetailCharges(${options.rowIndex},${options.data.code},${options.data.p_DT_CODE},${options.data.p_TRAN_ID})" title="Delete"><i class="fa fa-trash"></i></a>
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_CashReceiptVoucher.AddDetailChargesGridRow()" title="Add"><i class="fa fa-add"></i></a>
                       </div>`).appendTo(container);
                }
            },
            {
                dataField: 'acT_CODE',
                caption: 'Account',
                alignment: 'left',
                width: 250,
                lookup: {
                    dataSource: DetailCharges,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                allowEditing: true

            },
            {
                dataField: 'amt',
                caption: 'Amount',
                alignment: 'right',
                allowEditing: true,
                dataType: 'number',
                format: "#,##0.##",
            },
            {
                dataField: 'code',
                visible: false,
            },
            {
                dataField: 'p_DT_CODE',
                visible: false,
            },
            {
                dataField: 'p_TRAN_ID',
                visible: false,
            },

        ];


        debugger;
        empr_helper.dxGridbindingKnockOff('#DetailChargesGridContainer', col, dataSrc, "DetailCharges");
        if (dataSrc.length == 0) {
            $('#DetailChargesGridContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailChargesGridContainer').dxDataGrid('instance').saveEditData();
            });
        }

    },

    //SaveDetailCharges: function () {
    //    debugger;
    //    var P_TRAN_ID = $('#hidden_tranId').val();
    //    var P_DT_CODE = $('#hidden_dtCode').val();
    //    //var P_AMOUNT = $('#hidden_amt').val();


    //    var GridRecords = [];

    //    if ($('#DetailChargesGridContainer').dxDataGrid('instance').hasEditData()) {
    //        $('#DetailChargesGridContainer').dxDataGrid('instance').saveEditData().done(function () {
    //            var gridInstance = $('#DetailChargesGridContainer').dxDataGrid('instance');
    //            GridRecords = gridInstance.option("dataSource");
    //            gridInstance.refresh();

    //           //console.log("Data after save and refresh:", GridRecords);
    //        }).fail(function () {
    //           //console.error("Failed to save changes.");
    //        });
    //    } else {
    //        GridRecords = $('#DetailChargesGridContainer').dxDataGrid('instance').option("dataSource");
    //       //console.log("No changes made, data:", GridRecords);
    //    }

    //    var obj = {
    //        P_TRAN_ID: P_TRAN_ID,
    //        P_DT_CODE: P_DT_CODE,
    //        Detail: GridRecords
    //    }

    //    ajaxHelper.ajaxPostJsonData({ modelRecord: obj }, "/CashReceiptVoucher/SaveDetailCharges", function (data) {
    //        if (data.msgType == 1) {
    //            empr_CashReceiptVoucher.GetDetailCharges(P_TRAN_ID, P_DT_CODE);
    //        }
    //    }, false, true);
    //},



    SaveDetailCharges: function () {
        debugger;
        var P_TRAN_ID = $('#hidden_tranId').val();
        var P_DT_CODE = $('#hidden_dtCode').val();
        var GridRecords = [];
        if ($('#DetailChargesGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailChargesGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                debugger;
                var gridInstance = $('#DetailChargesGridContainer').dxDataGrid('instance');
                GridRecords = gridInstance.option("dataSource");
                gridInstance.refresh();
                var IsValid = true;

                $.each(GridRecords, function (index, item) {
                    if (item.acT_CODE == "" || item.acT_CODE == null || item.acT_CODE == undefined) {
                        empr_helper.notify("Please select account name at line " + (index + 1), 2);
                        IsValid = false;
                        return false;
                    }

                    if (item.amt == "" || item.amt == null || item.amt == undefined || item.amt == 0) {
                        empr_helper.notify("Please enter amount at line " + (index + 1), 2);
                        IsValid = false;
                        return false;
                    }
                });
                if (!IsValid) return;

                //console.log("Data after save and refresh:", GridRecords);
                var obj = {
                    P_TRAN_ID: P_TRAN_ID,
                    P_DT_CODE: P_DT_CODE,
                    Detail: GridRecords
                }
                ajaxHelper.ajaxPostJsonData({ modelRecord: obj }, "/CashReceiptVoucher/SaveDetailCharges", function (data) {
                    if (data.msgType == 1) {
                        empr_helper.notify(data.msg, data.msgType);
                        empr_CashReceiptVoucher.GetDetailCharges(P_TRAN_ID, P_DT_CODE);
                    }
                }, false, true);
            }).fail(function () {
                //console.error("Failed to save changes.");
            });
        } else {
            debugger;
            GridRecords = $('#DetailChargesGridContainer').dxDataGrid('instance').option("dataSource");
            var IsValid = true;
            $.each(GridRecords, function (index, item) {
                if (item.acT_CODE == "" || item.acT_CODE == null || item.acT_CODE == undefined) {
                    empr_helper.notify("Please select account name at line " + (index + 1), 2);
                    IsValid = false;
                    return false;
                    //console.log("Account at index " + index + " has empty account code.");
                }

                if (item.amt == "" || item.amt == null || item.amt == undefined) {
                    empr_helper.notify("Please enter amount at line " + (index + 1), 2);
                    IsValid = false;
                    return false;
                    //console.log("Account at index " + index + " has empty amount.");
                }
            });
            if (!IsValid) return;
            //console.log("No changes made, data:", GridRecords);
            var obj = {
                P_TRAN_ID: P_TRAN_ID,
                P_DT_CODE: P_DT_CODE,
                Detail: GridRecords
            }
            ajaxHelper.ajaxPostJsonData({ modelRecord: obj }, "/CashReceiptVoucher/SaveDetailCharges", function (data) {
                if (data.msgType == 1) {
                    empr_CashReceiptVoucher.GetDetailCharges(P_TRAN_ID, P_DT_CODE);
                }
            }, false, true);
        }
    },
    DeleteDetailCharges: function (rowIndex, code, dtCode, tranId) {
        debugger;
        if (dtCode == '' || dtCode == null || dtCode == undefined) {
            const gridInstance = $('#DetailChargesGridContainer').dxDataGrid('instance');

            gridInstance.deleteRow(rowIndex);
            gridInstance.saveEditData();
        }
        else {
            var obj = {
                CODE: code,
                P_TRAN_ID: tranId,
                P_DT_CODE: dtCode
            }

            ajaxHelper.ajaxPostJsonData(obj, "/CashReceiptVoucher/DeleteDetailCharges", function (data) {
                if (data.msgType == 1) {
                    empr_CashReceiptVoucher.GetDetailCharges(tranId, dtCode);
                }
            }, false, true);
        }


    },

    AddDetailChargesGridRow: function () {
        const gridInstance = $('#DetailChargesGridContainer').dxDataGrid('instance');
        const dataSource = gridInstance.option("dataSource");

        dataSource.unshift({ code: 0, amt: 0 });
        gridInstance.option("dataSource", dataSource);
        gridInstance.refresh();

    }
}