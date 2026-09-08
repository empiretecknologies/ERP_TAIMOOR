var empr_PendingToSRB = {
    isValueAssigned: false,
    ClosingData: [],
    ClosingBalance: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_PendingToSRB.InitQuickSearchGrid();
            empr_PendingToSRB.InitReportType();
            empr_PendingToSRB.GetMapData();

            $('#BtnGet').click(function () {
                empr_PendingToSRB.InitQuickSearchGrid();
            });
            $('#BtnClosed').click(function () {
                $('#Loader').appendTo('body'); // modal ke upar le jao
                $("#Loader").css({
                    display: 'flex'
                });
                setTimeout(function () {
                    empr_PendingToSRB.SRBHelper();
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }, 200);
                //empr_PendingToSRB.PrintModal();

            });

            document.querySelectorAll(".close").forEach(function (closeBtn) {
                closeBtn.addEventListener("click", function () {
                    $('#printModal').modal('hide');
                });
            });
        });
    },

    SRBHelper: async function () {
        debugger;
        var gridInstance = $("#gridContainer").dxDataGrid("instance");
        var allRows = gridInstance.getSelectedRowsData();

        for (let row of allRows) {
            var TranId = row.traN_ID;
            var voucher = 'SRBForm';
            var SrbName = $('#srbName').val();
            var SrbId = $('#srbId').val();
            var SrbNtn = $('#srbNtn').val();
            var SrbUser = $('#posUser').val();
            var SrbPass = $('#posPass').val();
            var SrbUrl = $('#srbUrl').val();

            console.log("Processing TranId:", TranId);

            let data = await new Promise((resolve, reject) => {
                ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSTransactionByCode?code=' + TranId + '&voucher=' + voucher,
                    function (response) {
                        resolve(response);
                    });
            });
            debugger;
            console.log(data);

            if (data.master.msgType == 1) {
                var response = data.master.data[0];
                if (response) {
                    $('#VOUCHER_NO').text(response.voucheR_NO);
                    $('#NET_TOTAL').val(response.neT_TOTAL);
                    $('#RECV').val(response.recv);
                    $('#CASHTAX').val(response.cashtax);
                    $('#BANKTAX').val(response.banktax);
                    $('#PARTYTAX').val(response.partytax);
                    $('#SRBInvoiceId').val(response.srbinv);
                    let rateValue;
                    if (response.cashtax && response.cashtax !== 0) {
                        rateValue = response.cashtax;
                    } else if (response.banktax && response.banktax !== 0) {
                        rateValue = response.banktax;
                    } else if (response.partytax && response.partytax !== 0) {
                        rateValue = response.partytax;
                    }

                    if ($('#srbStatus').val() == 'Y') {
                        var now = new Date(response.v_DATE);
                        //var formattedDateTime = now.getFullYear() + "-" +
                        //    String(now.getMonth() + 1).padStart(2, '0') + "-" +
                        //    String(now.getDate()).padStart(2, '0') + " " +
                        //    String(now.getHours()).padStart(2, '0') + ":" +
                        //    String(now.getMinutes()).padStart(2, '0') + ":" +

                        var datePart = response.deldate;
                        var now = new Date();
                        var timePart = now.toLocaleTimeString('en-US');
                        var formattedDateTime = datePart + ' ' + timePart;

                        var jsonText = {
                            posId: SrbId,
                            name: SrbName,
                            ntn: SrbNtn,
                            invoiceID: response.voucheR_NO,
                            invoiceDateTime: formattedDateTime,
                            invoiceType: 1,
                            rateValue: rateValue,
                            saleValue: response.neT_TOTAL,
                            taxAmount: (response.recv - response.neT_TOTAL) || "N/A",
                            consumerNTN: response.cntn || "N/A",
                            address: response.caddr || "N/A",
                            tariffCode: "N/A",
                            extraInf: "N/A",
                            pos_user: SrbUser,
                            pos_pass: SrbPass,
                            SrbUrl: SrbUrl
                        };

                        console.log(jsonText);

                        /* Uncomment to POST to SRB*/

                        await new Promise((resolvePost, rejectPost) => {
                            $.ajax({
                                type: "POST",
                                url: "/PendingToSRB/PostToSRB",
                                contentType: "application/json",
                                data: JSON.stringify(jsonText),
                                success: function (srbRes) {
                                    if (srbRes.resCode === '00') {
                                        var SRBInvoiceId = srbRes.srbInvoceId;
                                        empr_PendingToSRB.InitQuickSearchGrid();
                                    } else {
                                        console.log("Error: " + srbRes.err);
                                    }
                                    setTimeout(() => {
                                        empr_PendingToSRB.ApiStatus_Update(response.voucheR_NO, SRBInvoiceId);
                                        resolvePost();
                                    }, 500);
                                },
                                error: function (xhr) {
                                    console.log("SRB Server Error:", xhr.responseText);
                                    resolvePost();
                                }
                            });
                        });

                    }
                    else {
                        empr_helper.notify("PLease Set Account Mapping On SRB", 2);
                    }
                }
            }
        }
        empr_PendingToSRB.InitQuickSearchGrid();
        empr_helper.notify("All SRB transactions processed.", 1);
        console.log("✅ All SRB transactions processed.");
    },

    ApiStatus_Update(voucherNo, InvoiceId) {
        ajaxHelper.ajaxGetJson('/POSTransactions/SRBApi_Status?voucherno=' + voucherNo + '&invoiceId=' + InvoiceId, function (data) {

        }, false, true);
    },

    GetMapData: function() {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetMapData', function (data) {
            if (data.msgType == 1) {
                debugger;
                if (data.data.length > 0) {
                    $('#srbName').val(data.data[0].srbname);
                    $('#srbNtn').val(data.data[0].srbntn);
                    $('#posUser').val(data.data[0].posuser);
                    $('#posPass').val(data.data[0].pospass);
                    $('#srbId').val(data.data[0].srbid);
                    $('#srbStatus').val(data.data[0].srbstatus);
                    $('#srbUrl').val(data.data[0].srburl);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GetDataToSave: function () {
        debugger;
        var FromDate = empr_PendingToSRB.formatDate($('#FromDate').val());
        var ToDate = empr_PendingToSRB.formatDate($('#ToDate').val());
        var closingBalance = empr_PendingToSRB.ClosingBalance;
        var masterRecord = {
            FromDate: FromDate,
            ToDate: ToDate,
            closingBalance: closingBalance,
        }

        var detailRecords = empr_PendingToSRB.ClosingData.data;
        console.log("Ali",detailRecords);
        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords
        };
        return modelRecord;
    },

    formatDate(dateString) {
        var date = new Date(dateString);
        var day = String(date.getDate()).padStart(2, '0');
        var month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
        var year = date.getFullYear();
        return `${day}-${month}-${year}`;
    },
    InitQuickSearchGrid: function () {
        empr_PendingToSRB.QuickSearch();
    },
    InitReportType: function () {
        ajaxHelper.ajaxGetJson("/PendingToSRB/GetReportTypes", function (data) {
            debugger
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    var selectedValue = data.data[0].mD_ID;
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
    UpdateClosedData: function () {
        debugger;
        var FromDate = $('#FromDate').val();
        var ToDate = $('#ToDate').val();
        ajaxHelper.ajaxGetJson('/PendingToSRB/UpdateClosedData?FromDate=' + FromDate + '&ToDate=' + ToDate, function (data) {
            if (data.msgType == 1) {
                empr_PendingToSRB.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);

    },

    
    QuickSearch: function () {
        debugger;
        var FromDate = $('#FromDate').val();
        var ToDate = $('#ToDate').val();

        empr_helper.fromDate = FromDate;
        empr_helper.toDate = ToDate;

        ajaxHelper.ajaxGetJson('/PendingToSRB/GetPendingRecords?FromDate=' + FromDate + '&ToDate=' + ToDate, function (data) {
            empr_PendingToSRB.ClosingData = data;
            if (data.msgType == 1) {
                empr_PendingToSRB.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    SaveClosing: function (dataToSend) {
        debugger;
        $.ajax({
            url: '/PendingToSRB/SaveClosing',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),  
            success: function (response) {
                console.log("Data saved successfully", response);
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
            }
        });
    },
    CreateQuickSearchGrid: function (dataSrc) {
        console.log(dataSrc);
        dataSrc.forEach(item => {
            item.v_DATE = new Date(item.v_DATE); // Convert string to Date object
        });
        var today = new Date();
        var columns = [
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            {
                dataField: 'v_DATE',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                sortOrder: 'desc',
                sortIndex: 0,
                //filterValue: today
            },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'cname', caption: 'Name', },
            //{ dataField: 'waiter', caption: 'Waiter', },
            //{ dataField: 'table', caption: 'Table', },
            { dataField: 'billmode', caption: 'Mode', },
            { dataField: 'recv', caption: 'Total Value', },
            { dataField: 'bilL_STATUS', caption: 'Status', },
            { dataField: 'srbinv', caption: 'SRB Responce', },
            /*{ dataField: 'closing', caption: 'Closing Status', filterValue: 'Opened' },*/
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
        localStorage.removeItem("POSTransactionQSP");
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "POSTransactionQSP");
    },
    GetData: function () {
        debugger;
        var CODE = $("#Code").val();
        var description = $("#Description").val().trim();
        var status = $("#ASTATUS").dxSelectBox('instance').option('value');
        var SelectedBranches = $("#branches_hidden").val();
        var SelectedItems = $("#design_hidden").val();
        var SelectedItemGroups = $("#group_hidden").val();
        var FromDate = $("#FromDate").val();
        var ToDate = $("#ToDate").val();
        var DiscountPercent = $("#DiscountPercent").val();
        var DiscountExpired = $("#DiscountExpired").prop('checked') ? "1" : "0";

        var modelRecord = {
            CODE: CODE,
            DESCR: description,
            ASTATUS: status,
            SELECTEDBRANCHES: SelectedBranches,
            SELECTEDITEMS: SelectedItems,
            SELECTEDITEMGROUPS: SelectedItemGroups,
            FDATE: FromDate,
            TDATE: ToDate,
            DISC: DiscountPercent,
            DISC_EXP: DiscountExpired
        }

        return modelRecord;
    },
    ValidateInfo: function () {

        var valid = true;
        var data = empr_PendingToSRB.GetData();

        if (data.DESCR == '' || data.DESCR == null || data.DESCR == undefined) {
            empr_helper.notify("Please enter the description.", 2);
            valid = false;
            return valid;
        }

        if (data.SELECTEDBRANCHES == '' || data.SELECTEDBRANCHES == null || data.SELECTEDBRANCHES == undefined) {
            empr_helper.notify("Please select at least one branch.", 2);
            valid = false;
            return valid;
        }

        if ((data.SELECTEDITEMS == '' || data.SELECTEDITEMS == null || data.SELECTEDITEMS == undefined) && (data.SELECTEDITEMGROUPS == '' || data.SELECTEDITEMGROUPS == null || data.SELECTEDITEMGROUPS == undefined)) {
            empr_helper.notify("Please select at least one item or one item group.", 2);
            valid = false;
            return valid;
        }

        if (data.DISC == '' || data.DISC == null || data.DISC == undefined) {
            empr_helper.notify("Please enter the discount percentage.", 2);
            valid = false;
            return valid;
        }

        if (data.FDATE == '' || data.FDATE == null || data.FDATE == undefined) {
            empr_helper.notify("Please enter the from date.", 2);
            valid = false;
            return valid;
        }

        if (data.TDATE != null && data.FDATE != null) {
            var fromDate = new Date(data.FDATE);
            var toDate = new Date(data.TDATE);

            if (toDate < fromDate) {
                empr_helper.notify("please select valid ToDate.", 2);
                valid = false;
                return valid;
            }
        }

        return valid;
    },
    SaveInfo: function () {
        debugger;
        var data = empr_PendingToSRB.GetData();
        ajaxHelper.ajaxPostJsonData(data, "/PendingToSRB/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_PendingToSRB.ResetForm();
            }
        }, false, true);
    },
    ResetForm: function () {
        debugger;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #FromDate').val('');
        $('#DiscountExpired').prop('checked', false);
        empr_PendingToSRB.InitQuickSearchGrid();
        empr_PendingToSRB.InitBranchesDDL();
        empr_PendingToSRB.InitDesignNoDDL();
        empr_PendingToSRB.InitItemGroupDDL();
        $('#BtnDelete, #BtnNew').hide();
    },
    GetPendingToSRBByCode: function (id) {

        ajaxHelper.ajaxGetJson('/PendingToSRB/GetPendingToSRBByCode?code=' + id, function (data) {
            debugger;
            empr_PendingToSRB.ResetForm();
            if (data.msgType == 1) {
                var response = data.data[0];

                $("#Code").val(response.code);
                $("#Description").val(response.descr);
                $("#ASTATUS").dxSelectBox('instance').option('value', response.astatus);
                $("#branches_hidden").val(response.bcode);
                $("#design_hidden").val(response.iteM_CODE);
                $("#group_hidden").val(response.iteM_GROUP);
                if (response.fdate != '1900-01-01') {
                    $("#FromDate").val(response.fdate);
                }

                if (response.tdate != '1900-01-01') {
                    $("#ToDate").val(response.tdate);
                }

                $("#DiscountPercent").val(response.disc);
                if (response.disC_EXP == "1") {
                    $("#DiscountExpired").prop('checked', true);
                }

                setTimeout(function () {
                    var branchRow = $('#Branches_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.bcode);
                    if (branchRow[0] != null) {
                        var selectedRow = branchRow[0];
                        $('#Branches_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                    var designRow = $('#DesignNo_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.iteM_CODE);
                    if (designRow[0] != null) {
                        var selectedRow = designRow[0];
                        $('#DesignNo_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                    var groupRow = $('#ItemGroup_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.iteM_GROUP);
                    if (groupRow[0] != null) {
                        var selectedRow = groupRow[0];
                        $('#ItemGroup_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                }, 1000);
                $('#BtnDelete').show();
                $('#BtnNew').show();

                $('.modal').modal('hide');
            }
        }, false, true);
    },
    Delete: function (_id) {

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
            ajaxHelper.ajaxPostJsonData({ code: _id }, "/PendingToSRB/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    window.location.reload();
                }
            }, false, true);
        });
    },
    InitBranchesDDL: function () {
        $.ajax({
            url: 'PendingToSRB/GetBranches',
            method: 'GET',
            success: function (data) {
                empr_PendingToSRB.MultipleDxGridBoxDropdown('#Branches', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_branches", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#branches_hidden').val(keys);
                        $('#displayExpr_branches').val(values);
                        empr_PendingToSRB.isValueAssigned = false;
                    }
                    else {
                        $('#branches_hidden').val('');
                        $('#displayExpr_branches').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitDesignNoDDL: function (_selectedValue) {
        $.ajax({
            url: 'PendingToSRB/GetDesignNos',
            method: 'GET',
            success: function (data) {
                empr_PendingToSRB.MultipleDxGridBoxDropdown('#DesignNo', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_design", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#design_hidden').val(keys);
                        $('#displayExpr_design').val(values);
                        empr_PendingToSRB.isValueAssigned = false;
                        $('#group_hidden').val('');
                        $('#displayExpr_group').val('');
                        $('#ItemGroup_grid').dxDataGrid('instance').selectRows([]);
                    }
                    else {
                        $('#design_hidden').val('');
                        $('#displayExpr_design').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitItemGroupDDL: function (_selectedValue) {
        $.ajax({
            url: 'PendingToSRB/GetItemGroups',
            method: 'GET',
            success: function (data) {
                empr_PendingToSRB.MultipleDxGridBoxDropdown('#ItemGroup', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_group", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#group_hidden').val(keys);
                        $('#displayExpr_group').val(values);
                        empr_PendingToSRB.isValueAssigned = false;
                        $('#design_hidden').val('');
                        $('#displayExpr_design').val('');
                        $('#DesignNo_grid').dxDataGrid('instance').selectRows([]);
                    }
                    else {
                        $('#group_hidden').val('');
                        $('#displayExpr_group').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    MultipleDxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.MultipleDxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
}