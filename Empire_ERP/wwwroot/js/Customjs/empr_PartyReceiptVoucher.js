var empr_PartyReceiptVoucher = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    InitEvents: function () {
        $(document).ready(function () {
            empr_PartyReceiptVoucher.ResetForm();
            empr_PartyReceiptVoucher.InitReportTypeDDL();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_PartyReceiptVoucher.GetPartyReceiptVoucherByCode(data.traN_ID);
                }
            });
            $('body').on('click', '#BtnQuickSearch', function () {
                empr_PartyReceiptVoucher.InitQuickSearchGrid();
            });
            $('body').on('click', '#docBrowseBtn', function () {
                $('#DOC').val('');
                $('#hdnDOC').val('');
                $('#DOCName').val('');
                $('#DOC').click();
            });
            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_PartyReceiptVoucher.GeneratePrintReport();
            });

            $('body').on('click', '#BtnSave', function () {

                $("#Loader").show().css('display', 'flex');
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);

                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);

                    } else {
                        empr_PartyReceiptVoucher.ValidateAndPrepareDataForSave();
                        setTimeout(function () {
                            $("#Loader").hide();
                        }, 500);

                    }
                } else {

                    empr_PartyReceiptVoucher.ValidateAndPrepareDataForSave();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);

                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_PartyReceiptVoucher.GetPartyReceiptVoucherByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/PartyReceiptVoucher/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_PartyReceiptVoucher.GetPartyReceiptVoucherByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_PartyReceiptVoucher.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_PartyReceiptVoucher.ResetForm();
            });

            $('body').on('click', '.btn-print,#BtnGenerateReport', function () {
                empr_PartyReceiptVoucher.GeneratePrintReport();
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
        empr_PartyReceiptVoucher.CreateGrid([{ __KEY__: empr_PartyReceiptVoucher.GenerateKey(36), dC_TYPE: empr_PartyReceiptVoucher.DC_TYPE }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
        $('#hdnDOC').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        empr_PartyReceiptVoucher.InitCurrencyDDL();
        empr_PartyReceiptVoucher.InitPartyDDL();
        //empr_PartyReceiptVoucher.InitAccountDDL();
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
        console.log(dataSrc)
        if (dataSrc.length > 0) {
            dataSrc.forEach(item => {
                if (
                    item.chQ_DATE == '1900-01-01' || item.chQ_DATE == '01-01-1900' || item.chQ_DATE == '01-Jan-1900' || item.chQ_DATE == '1/1/1900 12:00:00 AM' || item.chQ_DATE == '01/01/1900 12:00:00 AM' || item.chQ_DATE == '1/1/1900' ||
                    item.chQ_DATE == '2000-01-01' || item.chQ_DATE == '01-01-2000' || item.chQ_DATE == '01-Jan-2000' || item.chQ_DATE == '1/1/2000 12:00:00 AM' || item.chQ_DATE == '01/01/2000 12:00:00 AM' || item.chQ_DATE == '1/1/2000' ||
                    item.chQ_DATE == '00-01-01' || item.chQ_DATE == '01-01-00' || item.chQ_DATE == '01-Jan-00' || item.chQ_DATE == '1/1/00 12:00:00 AM' || item.chQ_DATE == '01/01/00 12:00:00 AM' || item.chQ_DATE == '1/1/00'
                ) {
                    item.chQ_DATE = undefined;
                }
            });
            empr_PartyReceiptVoucher.rowsCount = dataSrc.length - 1;
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
                    debugger
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PartyReceiptVoucher.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PartyReceiptVoucher.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PartyReceiptVoucher.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_PartyReceiptVoucher.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                       <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_PartyReceiptVoucher.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_PartyReceiptVoucher.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                dataField: 'booK_TYPE',
                caption: 'Book Type',
                width: 450,
                allowSorting: false,
                lookup: {
                    dataSource: {
                        store: BookTypes,
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
                //setCellValue: function (newData, value, currentRowData) {
                //    var selectedAccount = Accounts.filter(u => u.key == value);
                //    if (selectedAccount.length > 0) {
                //        newData.custoM_ACT_CODE = selectedAccount[0].key;
                //        newData.acT_CODE = selectedAccount[0].accountCode;
                //        newData.partY_CODE = selectedAccount[0].partyCode;
                //    }
                //}
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
            {
                dataField: 'dT_DESC',
                caption: 'Description',
            },
            {
                dataField: 'amt',
                caption: 'Amount',
                dataType: 'number',
                format: { type: 'fixedPoint', precision: 0 }
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
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "PartyReceiptVoucher", "booK_TYPE");
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

                empr_PartyReceiptVoucher.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_PartyReceiptVoucher.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            });
        }
        else {
            empr_PartyReceiptVoucher.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_PartyReceiptVoucher.GenerateKey(36);
                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
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
                empr_PartyReceiptVoucher.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_PartyReceiptVoucher.GenerateKey(36), dC_TYPE: empr_PartyReceiptVoucher.DC_TYPE });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_PartyReceiptVoucher.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_PartyReceiptVoucher.GenerateKey(36), dC_TYPE: empr_PartyReceiptVoucher.DC_TYPE });
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
                    empr_PartyReceiptVoucher.rowsCount -= 1;
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
                        ajaxHelper.ajaxPostJsonData({ tranID: $('#Code').val(), code: dtCode }, "/PartyReceiptVoucher/DeletePartyReceiptVoucherDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_PartyReceiptVoucher.rowsCount -= 1;
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

    InitCurrencyDDL: function (_selectedValue) {
        $.ajax({
            url: 'PartyReceiptVoucher/GetCurrencies',
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
                console.error('Error fetching data:', error);
            }
        });
    },

    InitPartyDDL: function (_selectedValue) {
        $.ajax({
            url: '/PartyReceiptVoucher/GetParties',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#PARTY_CODE').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'customizedKey',
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

                            if (e.value != '' && e.value != null) {
                                var items = e.component._dataSource._items;
                                var item = items.filter(i => i.customizedKey == e.value);
                                if (item.length > 0) {
                                    console.log(item[0].key)
                                    $('#PartyCode').val(item[0].key);
                                }
                            }
                            else {
                                $('#PartyCode').val('');
                            }
                        },
                    });
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitQuickSearchGrid: function () {
        empr_PartyReceiptVoucher.GetPartyReceiptVouchers();
    },

    GetPartyReceiptVouchers: function () {
        ajaxHelper.ajaxGetJson('/PartyReceiptVoucher/GetPartyReceiptVouchers', function (data) {
            if (data.msgType == 1) {
                empr_PartyReceiptVoucher.CreateQuickSearchGrid(data.data);
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
                }else {
                    $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                       <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                       <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                       </div>`).appendTo(container);
                }
            }
        },

        { dataField: 'traN_ID', caption: 'Code', visible: false },
        { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'astatus', caption: 'Status', },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'curR_CODE', caption: 'Currency', },
        { dataField: 'crate', caption: 'Currency Rate', },
        { dataField: 'partY_NAME', caption: 'Party', },
        { dataField: 'remarks', caption: 'Remarks', },
        { dataField: 'booK_TYPE', caption: 'Book Type' },
        { dataField: 'dC_TYPE', caption: 'Type', },
        { dataField: 'dT_DESC', caption: 'Desc', },
        { dataField: 'amt', caption: 'Amount', },
        { dataField: 'chQ_NO', caption: 'Ref #', },
        { dataField: 'chQ_DATE', caption: 'Ref Date', },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "PartyReceiptVoucherQS", "multiple");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/PartyReceiptVoucher/GetPartyReceiptVouchers", "traN_ID", "SodaBookFeeding");
    },

    ValidateAndPrepareDataForSave: function () {
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                var IsValid = true;
                var V_DATE = $("#V_DATE").val();
                //var ACT_CODE = $('#ACT_CODE').dxSelectBox('option', 'value');
                var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');

                if (V_DATE == '' || V_DATE == null || V_DATE == undefined) {
                    empr_helper.notify("Transaction date is required.", 2);
                    IsValid = false;
                    return false;
                }

                //if (ACT_CODE == '' || ACT_CODE == null || ACT_CODE == undefined) {
                //    empr_helper.notify("Please select Account type.", 2);
                //    IsValid = false;
                //    return false;
                //}

                if (PARTY_CODE == '' || PARTY_CODE == null || PARTY_CODE == undefined) {
                    empr_helper.notify("Please select party type.", 2);
                    IsValid = false;
                    return false;
                }

                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

                $.each(detailRecords, function (index, item) {
                    if (item.booK_TYPE == "" || item.booK_TYPE == null || item.booK_TYPE == undefined) {
                        empr_helper.notify("Please select book type at index " + index, 2);
                        IsValid = false;
                        return false;
                        console.log("Account at index " + index + " has empty account code.");
                    }

                    if (item.dC_TYPE == "" || item.dC_TYPE == null || item.dC_TYPE == undefined) {
                        empr_helper.notify("Please enter type at index " + index, 2);
                        IsValid = false;
                        return false;
                        console.log("Account at index " + index + " has empty type.");
                    }

                    if (item.amt == "" || item.amt == null || item.amt == undefined) {
                        empr_helper.notify("Please enter amount at index " + index, 2);
                        IsValid = false;
                        return false;
                        console.log("Account at index " + index + " has empty amount.");
                    }

                    if (!(item.chQ_DATE == "" || item.chQ_DATE == null || item.chQ_DATE == undefined)) {
                        item.chQ_DATE = empr_helper.PrepareDate(item.chQ_DATE);
                    }
                });

                if (!IsValid) return;

                IsValid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

                if (IsValid) {
                    const dcTypeRows = detailRecords.filter(item => item.dC_TYPE === empr_PartyReceiptVoucher.DC_TYPE);
                    if (dcTypeRows.length > 0) {
                        if (empr_PartyReceiptVoucher.DC_TYPE === "D") {
                            const debitRows = detailRecords.filter(item => item.dC_TYPE === "D");
                            const creditRows = detailRecords.filter(item => item.dC_TYPE === "C");
                            const debitSum = debitRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                            const creditSum = creditRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                            if (creditSum > debitSum) {
                                empr_helper.notify("Credit amount must be less than or equal to debit amount.", 2);
                                IsValid = false;
                            }
                        }

                        if (empr_PartyReceiptVoucher.DC_TYPE === "C") {
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
                        var dcType = empr_PartyReceiptVoucher.DC_TYPE === "D" ? "debit" : "credit";
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

                    detailRecords.forEach(obj => {
                        obj.TRAN_ID = $("#Code").val();
                        obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
                        obj.V_DATE = $("#V_DATE").val();
                        obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                        obj.CURR_CODE = $('#Currency').dxSelectBox('option', 'value');
                        obj.BOOK_TYPE = $('#BookType').dxSelectBox('option', 'value');
                        obj.partY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');
                        obj.CRATE = $("#Rate").val();
                        obj.REMARKS = $("#REMARKS").val();
                        obj.DOC = $("#hdnDOC").val();
                    });

                    empr_PartyReceiptVoucher.SaveInfo(detailRecords);
                }
            });
        }
        else {
            var IsValid = true;
            var V_DATE = $("#V_DATE").val();
            //var ACT_CODE = $('#ACT_CODE').dxSelectBox('option', 'value');
            var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');

            if (V_DATE == '' || V_DATE == null || V_DATE == undefined) {
                empr_helper.notify("Transaction date is required.", 2);
                IsValid = false;
                return false;
            }

            //if (ACT_CODE == '' || ACT_CODE == null || ACT_CODE == undefined) {
            //    empr_helper.notify("Please select Account type.", 2);
            //    IsValid = false;
            //    return false;
            //}

            if (PARTY_CODE == '' || PARTY_CODE == null || PARTY_CODE == undefined) {
                empr_helper.notify("Please select party type.", 2);
                IsValid = false;
                return false;
            }

            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

            $.each(detailRecords, function (index, item) {
                if (item.booK_TYPE == "" || item.booK_TYPE == null || item.booK_TYPE == undefined) {
                    empr_helper.notify("Please select book type at index " + index, 2);
                    IsValid = false;
                    return false;
                    console.log("Account at index " + index + " has empty account code.");
                }

                if (item.dC_TYPE == "" || item.dC_TYPE == null || item.dC_TYPE == undefined) {
                    empr_helper.notify("Please enter type at index " + index, 2);
                    IsValid = false;
                    return false;
                    console.log("Account at index " + index + " has empty type.");
                }

                if (item.amt == "" || item.amt == null || item.amt == undefined) {
                    empr_helper.notify("Please enter amount at index " + index, 2);
                    IsValid = false;
                    return false;
                    console.log("Account at index " + index + " has empty amount.");
                }

                if (!(item.chQ_DATE == "" || item.chQ_DATE == null || item.chQ_DATE == undefined)) {
                    item.chQ_DATE = empr_helper.PrepareDate(item.chQ_DATE);
                }
            });

            if (!IsValid) return;

            IsValid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

            if (IsValid) {
                const dcTypeRows = detailRecords.filter(item => item.dC_TYPE === empr_PartyReceiptVoucher.DC_TYPE);
                if (dcTypeRows.length > 0) {
                    if (empr_PartyReceiptVoucher.DC_TYPE === "D") {
                        const debitRows = detailRecords.filter(item => item.dC_TYPE === "D");
                        const creditRows = detailRecords.filter(item => item.dC_TYPE === "C");
                        const debitSum = debitRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                        const creditSum = creditRows.reduce((acc, item) => acc + parseFloat(item.amt), 0);
                        if (creditSum > debitSum) {
                            empr_helper.notify("Credit amount must be less than or equal to debit amount.", 2);
                            IsValid = false;
                        }
                    }

                    if (empr_PartyReceiptVoucher.DC_TYPE === "C") {
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
                    var dcType = empr_PartyReceiptVoucher.DC_TYPE === "D" ? "debit" : "credit";
                    empr_helper.notify("Voucher must have at least one " + dcType + " row.", 2);
                    IsValid = false;
                }
            }

            if (IsValid) {
                if ($("#Code").val() == 0
                    || $("#Code").val() == null
                    || $("#Code").val() == undefined
                    || $("#Code").val() == "") {
                    detailRecords.reverse();
                }

                detailRecords.forEach(obj => {
                    obj.TRAN_ID = $("#Code").val();
                    obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
                    obj.V_DATE = $("#V_DATE").val();
                    obj.VOUCHER_NO = $("#VOUCHER_NO").val();
                    obj.CURR_CODE = $('#Currency').dxSelectBox('option', 'value');
                    //obj.acT_CODE = $('#ACT_CODE').dxSelectBox('option', 'value');
                    obj.partY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');
                    obj.CRATE = $("#Rate").val();
                    obj.REMARKS = $("#REMARKS").val();
                    obj.DOC = $("#hdnDOC").val();
                });

                empr_PartyReceiptVoucher.SaveInfo(detailRecords);
            }
        }
    },

    SaveInfo: function () {
        console.log('Final Object before saving');
        console.log("save info", detailRecords);

        ajaxHelper.ajaxPostJsonData({ modelRecord: detailRecords }, "/PartyReceiptVoucher/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if ($("#Code").val() == 0
                    || $("#Code").val() == null
                    || $("#Code").val() == undefined
                    || $("#Code").val() == "") {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                
                if (dataClear == 1) {
                    empr_PartyReceiptVoucher.GetPartyReceiptVoucherByCode($("#Code").val());
                }
                else {
                    empr_PartyReceiptVoucher.ResetForm();
                }
            }
        }, false, true);
    },

    UploadDoc: function () {
        debugger;
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
        debugger;
        var hdnUrl = $('#hdnDOC').val();
        if (!hdnUrl) {
            empr_helper.notify("Please upload a file to view.", 2);
        } else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    GetPartyReceiptVoucherByCode: function (code) {
        ajaxHelper.ajaxGetJson('/PartyReceiptVoucher/GetPartyReceiptVoucherByCode?code=' + code, function (data) {
            console.log('griddata',data);
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#Currency').dxSelectBox('instance').option('value', response.curR_CODE);
                    $('#PARTY_CODE').dxSelectBox('instance').option('value', response.partY_CODE.toString());
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
                    empr_PartyReceiptVoucher.CreateGrid(data.detail.data);
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

    GetPartyReceiptVoucherDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/PartyReceiptVoucher/GetPartyReceiptVoucherDetailsByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_PartyReceiptVoucher.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/PartyReceiptVoucher/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_PartyReceiptVoucher.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/PartyReceiptVoucher/GetReportTypes", function (data) {
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
        empr_PartyReceiptVoucher.InitReportTypeDDL();
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
        ajaxHelper.ajaxPostJsonData(dataModel, "/PartyReceiptVoucher/GetPrintReport", function (data) {
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

    }
}