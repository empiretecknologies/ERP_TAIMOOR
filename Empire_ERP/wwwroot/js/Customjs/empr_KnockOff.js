var empr_KnockOff = {

    initEvents: function () {
        $(document).ready(function () {
            console.log('PickData', PickData);
            empr_KnockOff.SetPickData();
            empr_KnockOff.InitSaleInvoiceGrid();
            empr_KnockOff.InitKnockOffGrid();
            empr_KnockOff.SetRemainingAmount();


            //let element = $("#displayRemainingAmount");
            //element.text(PickData.amount);
            //element.css("color", "green");
            //$("#displayRemainingAmount").text(PickData.amount);
            $("#ko_amt_span").text(PickData.amount.toLocaleString());
            $("#ko_bt_span").text(PickData.booK_NAME);
            $("#ko_party_span").text(PickData.partY_NAME);
            $("#ko_party_wht").text(PickData.whT_RATE ?? 0);

            $("#saleInvoicegridContainer").dxDataGrid("instance").option("onRowDblClick", function (e) {
                empr_KnockOff.AddRow(e.data);
                empr_KnockOff.SetRemainingAmount();
            });

            $('#knockOffBtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        empr_KnockOff.ValidateAndPrepareDataForSave();
                    }
                } else {
                    empr_KnockOff.ValidateAndPrepareDataForSave();
                }
            });

        });
    },

    SetRemainingAmount: function () {
        //debugger;
        var grid = $('#knockOffgridContainer').dxDataGrid('instance');
        grid.closeEditCell();
        //grid.saveEditData();
        //if (grid.hasEditData()) {
        //    grid.saveEditData().done(function () {
        //        knockOffRecords = grid.option("dataSource");
        //    });
        //} else {
        //    knockOffRecords = grid.option("dataSource");
        //}
        var knockOffRecords = grid.option("dataSource");
        var totalKO = knockOffRecords.reduce((sum, item) => {
            return sum + (Number(item.whT_NET_AMT) || 0);
        }, 0);
        debugger;

        var RemainingAmount = PickData.amount - totalKO;

        let numericAmount = parseFloat(RemainingAmount) || 0;

        let formattedAmount = numericAmount.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

        let element = $("#displayRemainingAmount");
        element.text(formattedAmount);
        element.css("color", "green");

        if (numericAmount >= 0) {
            element.css("color", "green");
        } else {
            element.css("color", "red");
        }
    },

    resetForm: function () {

        empr_KnockOff.InitSaleInvoiceGrid();
        empr_KnockOff.InitKnockOffGrid();

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#knockOffBtnSave').show();
            } else {
                $('#knockOffBtnSave').hide();
            }
        } else {
            $('#knockOffBtnSave').show();
        }
    },

    ValidateAndPrepareDataForSave: function () {
        var valid = true;
        var grid = $('#knockOffgridContainer').dxDataGrid('instance');
        grid.closeEditCell(); // force cell to close and save value
        grid.saveEditData();  // commit any pending edits

        var outerAmount = $("#P_AMOUNT").val();
        var knockOffRecords = [];

        if (grid.hasEditData()) {
            grid.saveEditData().done(function () {
                knockOffRecords = grid.option("dataSource");
            });
        } else {
            knockOffRecords = grid.option("dataSource");
        }

        debugger;

        $.each(knockOffRecords, function (index, item) {
            if (item.kO_AMT == "" || item.kO_AMT == null || item.kO_AMT == undefined) {
                empr_helper.notify("Please enter amount at Row no " + (index + 1), 2);
                valid = false;
                return false;
            }

            if (Number(item.kO_AMT) < 0) {
                empr_helper.notify("Please enter valid amount at Row no " + (index + 1), 2);
                valid = false;
                return false;
            }

            if (Number(item.kO_AMT.toFixed(2)) > Number(item.invoicE_VALUE.toFixed(2))) {
                empr_helper.notify("KnockOff amount cannot be greater than the SI amount. at Row no " + (index + 1), 2);
                valid = false;
                return false;
            }


        });

        if (Array.isArray(knockOffRecords) && knockOffRecords.length > 0) {
            var totalKO = knockOffRecords.reduce((sum, item) => {
                return sum + (Number(item.whT_NET_AMT) || 0);
            }, 0);

            //if (totalKO > Number(outerAmount) ) {
            //    empr_helper.notify('KnockOff amount cannot be greater than the PO amount.', 2);
            //    valid = false;
            //}

            // Dono amounts ko 2 decimal places tak fix karke compare karein
            if (Number(totalKO).toFixed(2) !== Number(outerAmount).toFixed(2)) {
                empr_helper.notify('Total KnockOff amount must match the Received amount.', 2);
                valid = false;
            }

            //if (totalKO !== Number(outerAmount)) {
            //    empr_helper.notify('KnockOff Amount is not equal', 2);
            //    valid = false;
            //}

        } else {
            empr_helper.notify('No records found in grid', 2);
        }

        if (valid) {
            var kId = $("#PDT_CODE").val();

            var obj = {
                K_ID: kId,
                Data: knockOffRecords
            }
            empr_KnockOff.saveAttempt(obj);
        }
    },

    saveAttempt: function (model) {
        console.log('KO SaveAttempt', model);
        ajaxHelper.ajaxPostJsonData(model, "/KnockOff/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_KnockOff.resetForm();
                //empr_KnockOff.InitSaleInvoiceGrid();
                //$('#optmodal').modal('hide');
                //$('#costCenterBtnDelete').hide();
                //$('#costCenterBtnNew').hide();
                //empr_KnockOff.edit_Id = '';
            }
        }, false, true);

    },

    InitSaleInvoiceGrid: function () {
        empr_KnockOff.GetAllSaleInvoices();
    },

    InitKnockOffGrid: function () {
        empr_KnockOff.GetAllKnockOff();
    },

    GetAllSaleInvoices: function () {

        var PARTY_CODE = $('#PPARTY_CODE').val();
        var ACT_CODE = $('#PACT_CODE').val();

        var obj = {
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE
        };

        ajaxHelper.ajaxPostJsonData(obj, "/KnockOff/GetAllSaleInvoices", function (data) {

            console.log('GetAllSaleInvoices', data);

            empr_KnockOff.CreateSaleInvoiceGrid(data.data);
        }, false, true);
    },

    GetAllKnockOff: function () {

        var DT_CODE = $('#PDT_CODE').val();
        var PARTY_CODE = $('#PPARTY_CODE').val();
        var ACT_CODE = $('#PACT_CODE').val();

        var obj = {
            DT_CODE: DT_CODE,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE
        };

        ajaxHelper.ajaxPostJsonData(obj, "/KnockOff/GetAllKnockOff", function (data) {
            console.log('GetAllSaleInvoices', data);
            empr_KnockOff.CreateKnockOffGrid(data.data);
        }, false, true);
    },

    CreateSaleInvoiceGrid: function (dataSrc) {
        var col = [
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy', alignment: 'center', allowEditing: false },
            //{ dataField: 'voucheR_NO', caption: 'Transaction#', alignment: 'center', allowEditing: false },
            {
                dataField: 'voucheR_NO', caption: 'Transaction#', alignment: 'center', allowEditing: false,
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_KnockOff.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.picK_ID) + ')')
                        .appendTo(container);
                }
            },
            { dataField: 'remarks', caption: 'Remarks', alignment: 'center', allowEditing: false },
            { dataField: 'qty', caption: 'T.Qty', alignment: 'right', allowEditing: false },
            { dataField: 'amount', caption: 'Amount', alignment: 'right', allowEditing: false, dataType: 'number', format: "#,##0.##", }

        ];
        empr_helper.dxGridbindingKnockOff('#saleInvoicegridContainer', col, dataSrc, "SetupSubType");
    },

    CreateKnockOffGrid: function (dataSrc) {
        console.log('CreateSaleInvoiceGrid', dataSrc);
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
                    console.log('options', options);
                    var html = '<div class="btn-group btn-group-sm">';
                    //if (options.data.gpic != null && options.data.gpic != '' && options.data.gpic != undefined) {
                    //    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.gpic}')"><i class="fa fa-eye"></i></a>`;
                    //}
                    /*html += `<a href="javascript:;" class="grid-action-icon cc_elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;*/
                    html += `<a href="javascript:;" class="grid-action-icon Delete" style="" onclick="empr_KnockOff.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                    html += '</div>';
                    $(html).appendTo(container);
                }
            },
            {
                dataField: 'astatus',
                caption: 'Status',
                lookup: {
                    dataSource: [
                        { value: 'Y', text: 'Active' },
                        { value: 'N', text: 'In-Active' }
                    ],
                    displayExpr: 'text',
                    valueExpr: 'value'
                },
                width: 80,
                alignment: 'center',
            },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy', alignment: 'center', allowEditing: false, width: 80 },
            { dataField: 'voucheR_NO', caption: 'Transaction#', alignment: 'center', allowEditing: false, width: 140 },
            //{ dataField: 'remarks', caption: 'Remarks', alignment: 'center', allowEditing: false },
            {
                dataField: 'qty', caption: 'T.Qty', alignment: 'right', allowEditing: false, width: 100, dataType: 'number', format: "#,##0.####", },
            {
                dataField: 'picK_AMT',
                caption: 'Bill Amount',
                dataType: 'number',
                format: "#,##0.##",
                alignment: 'right',
                allowEditing: false,
            },
            {
                dataField: 'kO_AMT',
                caption: 'KnockOff Amount',
                alignment: 'right',
                dataType: 'number',
                format: "#,##0.##",
                setCellValue: function (newData, value, currentRowData) {
                    newData.kO_AMT = value;
                    var IntValue = parseFloat(value) || 0;
                    var wht = PickData.whT_RATE;
                    var whtAmt = IntValue * wht / 100 || 0;
                    var netAmt = IntValue - whtAmt;
                    newData.whT_AMT = whtAmt.toFixed(2);
                    newData.whT_NET_AMT = netAmt.toFixed(2);

                },
            },
            {
                dataField: 'wht',
                dataType: 'number',
                format: "#,##0.##",
                visible: false,
            },
            {
                dataField: 'whT_AMT',
                caption: 'WHT Amount',
                dataType: 'number',
                format: "#,##0.##",
                alignment: 'right',
                allowEditing: false,
            },
            {
                dataField: 'whT_NET_AMT',
                caption: 'Net Amount',
                dataType: 'number',
                format: "#,##0.##",
                alignment: 'right',
                allowEditing: false,
            },
            { dataField: 'traN_ID', visible: false, },
            { dataField: 'picK_ID', visible: false, },
            { dataField: 'pmenU_ID', visible: false, },
            { dataField: 'invoicE_VALUE', visible: false, },

        ];
        empr_helper.dxGridbindingKnockOff('#knockOffgridContainer', col, dataSrc, "SetupSubType");
    },

    SetPickData: function () {
        $("#PTRAN_ID").val(PickData.traN_ID);
        $("#PDT_CODE").val(PickData.dT_CODE);
        $("#P_AMOUNT").val(PickData.amount);
        $("#PPARTY_CODE").val(PickData.partY_CODE);
        $("#PACT_CODE").val(PickData.acT_CODE);
    },

    GetGridSum: function (dataSrc, gridId) {
        return dataSrc.reduce(function (sum, row) {
            var amount = parseFloat(row.amount) || 0;
            var rowId = row.grouP_CODE;

            if (gridId != null && String(rowId) === String(gridId)) {
                return sum;
            }
            return sum + amount;
        }, 0);
    },

    AddRow: function (selectedRow) {
        console.log('AddRow', selectedRow);
        const gridInstance = $('#knockOffgridContainer').dxDataGrid('instance');
        let dataSource = gridInstance.option("dataSource") || [];

        // fallback: agar dataSource array nahi hai
        if (!Array.isArray(dataSource)) {
            dataSource = gridInstance.getVisibleRows().map(r => r.data);
        }

        const isDuplicate = dataSource.some(function (row) {
            return row.voucheR_NO === selectedRow.voucheR_NO;
        });
        if (isDuplicate) {
            empr_helper.notify('Duplicate not allowed', 2);
            return;
        }

        var whT_RATE_safe = PickData.whT_RATE ?? 0;
        var whtAmt = (selectedRow.amount * whT_RATE_safe) / 100;
        var netAmt = selectedRow.amount - whtAmt;

        const newRow = {
            __KEY__: empr_KnockOff.GenerateKey(36),
            voucheR_NO: selectedRow.voucheR_NO,
            qty: selectedRow.qty,
            remarks: selectedRow.remarks,
            picK_ID: selectedRow.picK_ID,
            pmenU_ID: selectedRow.pmenU_ID,
            v_DATE: selectedRow.v_DATE,
            invoicE_VALUE: selectedRow.amount,
            astatus: 'Y',
            wht: PickData.whT_RATE,
            picK_AMT: Number(selectedRow.amount.toFixed(2)),
            kO_AMT: Number(selectedRow.amount.toFixed(2)),
            whT_AMT: Number(whtAmt.toFixed(2)),
            whT_NET_AMT: Number(netAmt.toFixed(2))
        };

        if (gridInstance.hasEditData()) {
            gridInstance.saveEditData().done(function () {
                dataSource.unshift(newRow);
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        } else {
            dataSource.unshift(newRow);
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
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

    DeleteRow: function (index) {
        debugger;
        const gridInstance = $('#knockOffgridContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {

            var row = dataSource[index];
            if (row.traN_ID == '' || row.traN_ID == null || row.traN_ID == undefined) {
                gridInstance.deleteRow(index);
                //empr_DailyProduction.rowsCount -= 1;
                gridInstance.saveEditData();
                empr_KnockOff.SetRemainingAmount();
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
                    ajaxHelper.ajaxPostJsonData({ id: row.traN_ID }, "/KnockOff/Delete", function (data) {
                        empr_helper.notify(data.msg, data.msgType);
                        if (data.msgType == 1) {
                            gridInstance.deleteRow(index);
                            ////empr_DailyProduction.rowsCount -= 1;
                            gridInstance.saveEditData();
                            empr_KnockOff.resetForm();
                            empr_KnockOff.SetRemainingAmount();
                        }
                    }, false, true);
                });
            }
            
        }
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