var empr_MerchantPurchaseOrder = {
    PartiesData : [],
    initEvents: function () {

        $(document).ready(function () {
            $('#resetall').show();
            empr_MerchantPurchaseOrder.InitDropdowns();
            console.log('ClientPO', ClientPO);
            console.log('PartiesData', empr_MerchantPurchaseOrder.PartiesData);
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_MerchantPurchaseOrder.GetMerchantPurchaseOrderByID(data.traN_ID);
                }
            });
            //empr_MerchantPurchaseOrder.InitTree();
            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_MerchantPurchaseOrder.validateForm()) {
                            empr_MerchantPurchaseOrder.saveAttempt();
                        }
                    }
                } else {
                    if (empr_MerchantPurchaseOrder.validateForm()) {
                        empr_MerchantPurchaseOrder.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_MerchantPurchaseOrder.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_MerchantPurchaseOrder.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_helper.selectedBill = rportid;
                empr_MerchantPurchaseOrder.GetMerchantPurchaseOrderByID(rportid);

            })

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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/MerchantPurchaseOrder/CopyRecord", function (data) {
                    //console.log(data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_MerchantPurchaseOrder.GetMerchantPurchaseOrderByID(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                //$('#resetall').hide();
                empr_MerchantPurchaseOrder.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_MerchantPurchaseOrder.DeleteRecord();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_MerchantPurchaseOrder.InitSodaPickGrid();
            });

            $('#AMT').on('input', function () {
                empr_MerchantPurchaseOrder.calculateRate();
                empr_MerchantPurchaseOrder.calculateCommition();
                //$('#COMM').val('');
                //$('#COMM_VAL').val('');
            });

            $('#QTY').on('input', function () {
                empr_MerchantPurchaseOrder.calculateRate();
                empr_MerchantPurchaseOrder.calculateCommition();
                //$('#COMM').val('');
                //$('#COMM_VAL').val('');
            });

            $("#RATE").on('input', function () {
                debugger;
                empr_MerchantPurchaseOrder.calculateCommition();
            });

            $('#COMM').on('input', function () {
                empr_MerchantPurchaseOrder.calculateCommition();
            });

            $('#COMM_VAL').on('input', function () {
                empr_MerchantPurchaseOrder.calculateCommition();
            });

            //$('body').on('click', '#BtnAddSodaToDelivery', function () {
            //    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
            //    if (selectedSodas.length > 0) {
            //        empr_MerchantPurchaseOrder.AddSodaToDelivery();
            //    }
            //    else {
            //        empr_helper.notify("Please select the items first.", 2);
            //    }
            //});

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_MerchantPurchaseOrder.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }
        });

    },
    calculateRate: function () {
        var AMT = parseFloat($("#AMT").val()) || 0;
        var QTY = parseFloat($("#QTY").val()) || 0;

        if (QTY > 0) {
            var RATE = AMT / QTY;

            // Check if integer or decimal
            if (RATE % 1 === 0) {
                $("#RATE").val(RATE).trigger('input');
            } else {
                $("#RATE").val(RATE.toFixed(2)).trigger('input');
            }

        } else {
            $("#RATE").val(0);
        }
    },

    calculateCommition: function () { 
        debugger;
        let amt = parseFloat($('#AMT').val());
        let comm = parseFloat($('#COMM').val());
        let commVal = parseFloat($('#COMM_VAL').val());
        var COMM_AMT = $('#COMM_AMT').dxSelectBox('option', 'value');

        // Validation: Make sure amount is a valid number
        if (isNaN(amt) || amt <= 0) return;

        // If COMM changed, calculate COMM_VAL

        if (COMM_AMT == 'PR') {
            let calcCommVal = (amt * comm) / 100;
            $('#COMM_VAL').val(calcCommVal.toFixed(2));

            //if (!isNaN(comm)) {
                
            //}
        }
        else if (COMM_AMT == 'RS') {
            let calcComm = (commVal * 100) / amt;
            $('#COMM').val(calcComm.toFixed(2));

            //if (!isNaN(commVal)) {
                
            //}
        }

        //if ($('#COMM').is(':focus')) {
        //    if (!isNaN(comm)) {
        //        let calcCommVal = (amt * comm) / 100;
        //        $('#COMM_VAL').val(calcCommVal.toFixed(2));
        //    }
        //}

        //// If COMM_VAL changed, calculate COMM
        //else if ($('#COMM_VAL').is(':focus')) {
        //    if (!isNaN(commVal)) {
        //        let calcComm = (commVal * 100) / amt;
        //        $('#COMM').val(calcComm.toFixed(2));
        //    }
        //}
    },

    DeleteRecord: function () {

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/MerchantPurchaseOrder/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_MerchantPurchaseOrder.resetForm();
                    empr_MerchantPurchaseOrder.InitTree();
                    empr_MerchantPurchaseOrder.reFreshTree();
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('.btn-print').hide();
                    $('#resetall').hide();
                }
            }, false, true);

        });

    },
    resetForm: function () {


        $("#Code").val('')
        $("#ADD_USER_ID").val('');
        $("#MENU_ID").val();
        //$('#ASTATUS').dxSelectBox('option', 'value');
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", false);
        //$('#ASTATUS').dxSelectBox('instance').option('disabled', false);

        $("#V_DATE").val('');
        $("#VOUCHER_NO").val('');
        $('#PARTY_CODE').dxSelectBox('instance').option('value', 0);
        //$("#S_DATE").val('');
        $("#REF").val('');
        $('#SPARTY_CODE').dxSelectBox('instance').option('value', 0);
        $("#JOB_NO").val('');
        $("#CLIENT_PO").val('');
        //$('#CLIENT_PO').dxSelectBox('instance').option('value', 0);
        //$('#CLIENT_PO').dxSelectBox('instance').option('disabled', false);
        //$('#FABRIC').dxSelectBox('instance').option('disabled', false);
        //$('#GSM').dxSelectBox('instance').option('disabled', false);
        $("#QTY").val('');
        $("#RATE").val('');
        $("#AMT").val('');
        $("#COMM").val('');
        $("#COMM_VAL").val('');
        $("#REMARKS").val('');
        $("#hdnDOC").val('');
        $('#DOCName').val('');
        //$("#CELL").val('');
        //$("#LOT_NO").val('');
        //$("#ORIGIN").val('');
        $('#ITEM_CODE').dxSelectBox('instance').option('value', 0);
        //$("#TBAG").val(0);
        $('#V_DATE').val(todayDate);
        //$('#S_DATE').val();
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        } else {
            $('#saveAttempt').show();
        }
        if ($('#treeListContainer').dxTreeList('instance') != undefined) {
            $('#treeListContainer').dxTreeList('instance').clearFilter();
        }
        empr_MerchantPurchaseOrder.InitDropdowns();
    },
    validateForm: function () {

        var valid = true;

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value')
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value')
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value')
        var CLIENT_PO = $('#CLIENT_PO').val();
        var QTY = $('#QTY').val();
        var DEP_ID = $('#DEP').dxSelectBox('option', 'value');
        var DOC = $('#hdnDOC').val();
        var SPARTY_CODE = $('#SPARTY_CODE').dxSelectBox('option', 'value');

        if (CLIENT_PO == '' || CLIENT_PO == null) {
            valid = false;
            empr_helper.notify("Please Enter Client PO#.", 2);
        }

        //if (DOC == '' || DOC == null) {
        //    valid = false;
        //    empr_helper.notify("Please Select Document.", 2);
        //}
        
        if (SPARTY_CODE == '' || SPARTY_CODE == null || SPARTY_CODE == '00') {
            valid = false;
            empr_helper.notify("Please select Supplier.", 2);
        }

        if (ASTATUS == '' || ASTATUS == null) {
            valid = false;
            empr_helper.notify("Please select active.", 2);
        }

        if (DEP_ID == '' || DEP_ID == null) {
            valid = false;
            empr_helper.notify("Please select Department.", 2);
        }

        if (QTY == '' || QTY == null) {
            valid = false;
            empr_helper.notify("Please select Quantity.", 2);
        }
        if (ITEM_CODE == '' || ITEM_CODE == null) {
            valid = false;
            empr_helper.notify("Please select Item.", 2);
        }
        if (PARTY_CODE == '' || PARTY_CODE == null) {
            valid = false;
            empr_helper.notify("Please select Client Account.", 2);
        }

        if (!valid) return;

        valid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

        return valid;
    },
    getDataToSave: function () {
        var ID = $("#ID").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var Code = empr_helper.getCode();
        var TRAN_ID = $("#Code").val().trim();
        var V_DATE = $("#V_DATE").val();
        var REF = $("#REF").val();
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');
        //var DEP_ID = $("#department_hidden").val();
        //var UNIT = $("#unitcode_hidden").val();
        //var EMP = $("#empcode_hidden").val();
        var DEP_ID = $('#DEP').dxSelectBox('option', 'value');
        var EMP = $('#EMP').dxSelectBox('option', 'value');
        var UNIT = $('#UNIT').dxSelectBox('option', 'value');
        var JOB_NO = $("#JOB_NO").val();
        var CLIENT_PO = $('#CLIENT_PO').val();
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value');
        var GRADE = $('#GRADE').dxSelectBox('option', 'value');
        var SPARTY_CODE = $('#SPARTY_CODE').dxSelectBox('option', 'value');
        var TERMS = $('#TERMS').dxSelectBox('option', 'value');
        //var FABRIC = $('#FABRIC').dxSelectBox('option', 'value');
        //var GSM = $('#GSM').dxSelectBox('option', 'value');
        //var SHIP_DATE = $("#S_DATE").val();
        var QTY = $("#QTY").val();
        var RATE = $("#RATE").val();
        var CURR_CODE = $('#CURR_CODE').dxSelectBox('option', 'value');
        var CRATE = $("#CRATE").val();
        var COMM = $("#COMM").val();
        var COMM_VAL = $("#COMM_VAL").val();
        var COMM_AMT = $('#COMM_AMT').dxSelectBox('option', 'value');
        var AMT = $("#AMT").val();
        var REMARKS = $("#REMARKS").val();
        var DOC = $('#hdnDOC').val();

        var modelRecord = {
            ID: ID,
            Code: Code,
            ASTATUS: ASTATUS,
            TRAN_ID: TRAN_ID,
            V_DATE: V_DATE,
            REF: REF,
            PARTY_CODE: PARTY_CODE,
            DEP_ID: DEP_ID,
            JOB_NO: JOB_NO,
            EMP: EMP,
            CLIENT_PO: CLIENT_PO,
            ITEM_CODE: ITEM_CODE,
            SPARTY_CODE: SPARTY_CODE,
            QTY: QTY,
            GRADE: GRADE,
            //FABRIC: FABRIC,
            //GSM: GSM,
            RATE: RATE,
            CURR_CODE: CURR_CODE,
            CRATE: CRATE,
            COMM: COMM,
            COMM_VAL: COMM_VAL,
            COMM_AMT: COMM_AMT,
            UNIT: UNIT,
            AMT: AMT,
            REMARKS: REMARKS,
            TERMS: TERMS,
            DOC: DOC,
        }
        //console.log(modelRecord);
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_MerchantPurchaseOrder.getDataToSave();
        console.log('saveattempt',obj);
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/MerchantPurchaseOrder/save", function (data) {
            //console.log(data)
            empr_helper.notify(data.msg, data.msgType);
            //debugger;
            if (data.msgType == 1) {
                //empr_MerchantPurchaseOrder.resetForm();
                //$('#Code').val(data.data);
                //$('#VOUCHER_NO').val(data.data2);
                //$('#optmodal').modal('hide');
                //$('#resetall').hide();
                empr_helper.selectedBill = data.data;

                if (dataClear == 1) {
                    empr_MerchantPurchaseOrder.GetMerchantPurchaseOrderByID(data.data);
                    $('.btn-delete').show();
                    $('.btn-print').show();
                }
                else {
                    empr_MerchantPurchaseOrder.resetForm();
                }
            }


        }, false, true);

    },
    InintQuickSearch: function () {
        empr_MerchantPurchaseOrder.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/MerchantPurchaseOrder/QuickSearch', function (data) {
            //console.log('quicksearch', data);
            if (data.msgType == 1) {
                empr_MerchantPurchaseOrder.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    //GetAllDeliveryFormats: function () {

    //    var xhr = ajaxHelper.ajaxGetJson('/DeliveryFormat/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

    //        empr_MerchantPurchaseOrder.CreateGrid(data.data);

    //    }, false, true);

    //},
    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },
    GetMerchantPurchaseOrderByID: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/MerchantPurchaseOrder/MerchantPurchaseOrderByid?id=' + id, function (data) {
            debugger;
            if (data.msgType == 1) {
                console.log('edit', data);
                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('.btn-delete').show();
                    }
                    if (Permissions.r_PRINT) {
                        $('.btn-print').show();
                    }
                    if (Permissions.r_ADD) {
                        $('#resetall').show();
                    }
                    if (Permissions.r_EDIT) {
                        $('#saveAttempt').show();
                    }
                    else {
                        $('#saveAttempt').hide();
                    }
                } else {
                    $('#saveAttempt').show();
                    $('.btn-delete').show();
                    $('.btn-print').show();
                    $('#resetall').show();
                }

                $('.modal').modal('hide')
                empr_MerchantPurchaseOrder.makeReadOnly(true);
                debugger;
                $('#Code').val(data.data.traN_ID); //methods
                $("#V_DATE").val(data.data.v_DATE);
                $("#VOUCHER_NO").val(data.data.voucheR_NO);
                $("#REF").val(data.data.ref);
                $("#JOB_NO").val(data.data.joB_NO);
                $("#CLIENT_PO").val(data.data.clienT_PO);
                //empr_MerchantPurchaseOrder.InitClientPODDL(data.data.clienT_PO);
                //$('#CLIENT_PO').dxSelectBox('instance').option('disabled', true);
                //empr_MerchantPurchaseOrder.InitFabricDDL(data.data.fabric);
                //empr_MerchantPurchaseOrder.InitGSMDDL(data.data.gsm);
                $("#QTY").val(data.data.qty);
                $("#RATE").val(data.data.rate);
                $("#AMT").val(data.data.amt);
                $("#CRATE").val(data.data.crate);
                empr_MerchantPurchaseOrder.InitCommissionAmtDDL(data.data.comM_AMT);
                empr_MerchantPurchaseOrder.InitDepartmentDDL(data.data.deP_ID);
                empr_MerchantPurchaseOrder.InitEmpDDL(data.data.emP_ID);
                $("#COMM").val(data.data.comm);
                $("#COMM_VAL").val(data.data.comM_VAL);
                $("#REMARKS").val(data.data.remarks);
                $('#activestatushidden').val(data.data.astatus);
                $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
                $('#UNIT').dxSelectBox('instance').option('value', data.data.unit);
                //$('#DEP').dxSelectBox('instance').option('value', data.data.deP_ID);
                $('#hdnDOC').val(data.data.doc);


                var fullPath = data.data.doc;
                var fileName = fullPath.split('/').pop();
                $('#DOCName').val(fileName);


                empr_MerchantPurchaseOrder.InitDropdownsWithValue(data.data.partY_CODE, data.data.spartY_CODE, data.data.iteM_CODE, data.data.emP_ID, data.data.deP_ID, data.data.terms, data.data.unit, data.data.curR_CODE, data.data.grade);
            }
            else {
                //empr_helper.notify(data.msg, data.msgType);
            }
            //debugger;
            

        }, false, true);

    },
    CreateGrid: function (dataSrc) {

        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                //debugger

                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);
                if (Permissions != "Admin" && !Permissions.r_PRINT) {
                    $(`<div class="btn-group btn-group-sm">
                            <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                            <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                            </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                            <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                            <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                            <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                            </div>`).appendTo(container);
                }


            }
        },
            {
                dataField: 'doc',
                caption: 'Doc',
                width: 100,
                alignment: 'center',
                cellTemplate: function (container, options) {
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.doc != null && options.data.doc != '' && options.data.doc != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="window.open('${options.data.doc}', '_blank')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += '</div>';
                    $(html).appendTo(container);
                }
            },
        { dataField: 'traN_ID', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'astatus', caption: 'Status' },
        { dataField: 'v_DATE', caption: 'Voucher Date' },
        { dataField: 'voucheR_NO', caption: 'Voucher Number' },
        { dataField: 'ref', caption: 'Refrence No #' },
        { dataField: 'clienT_PO', caption: 'Model# / PO#', alignment: 'center' },
        { dataField: 'joB_NO', caption: 'Job No#', alignment: 'center' },
        { dataField: 'clienT_NAME', caption: 'Client Name' },
        { dataField: 'descr', caption: 'Department' },
        { dataField: 'clienT_PO', caption: 'Model# / PO#', alignment: 'center' },
        { dataField: 'joB_NO', caption: 'Job No#', alignment: 'center' },
        { dataField: 'ename', caption: 'Employee' },
        { dataField: 'supplieR_NAME', caption: 'Supplier' },
        { dataField: 'termS_NAME', caption: 'Terms' },
        { dataField: 'currencY_NAME', caption: 'Currency' },
        { dataField: 'crate', caption: 'C Rate' },
        { dataField: 'iteM_NAME', caption: 'Item' },
        { dataField: 'qty', caption: 'Qty' },
        { dataField: 'uniT_NAME', caption: 'Unit' },
        { dataField: 'rate', caption: 'Rate' },
        { dataField: 'amt', caption: 'Amount' },
        { dataField: 'comM_TYPE', caption: 'Comm.Unit' },
        { dataField: 'comm', caption: 'Commition' },
        { dataField: 'remarks', caption: 'Remarks' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "DeliveryFormatQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/DeliveryFormat/QuickSearchLazyLoading", "traN_ID", "DeliveryFormats");
    },
    InitTree: function () {

        $.ajax({
            url: 'DeliveryFormat/GetAccountsForTreeView',
            method: 'GET',
            data: { Code: empr_helper.getCode() },
            success: function (data) {
                //console.log(data);
                $('#treeListContainer').dxTreeList({
                    dataSource: data.data,
                    keyExpr: 'acT_CODE',
                    parentIdExpr: 'acT_PARENT_CODE',
                    headerFilter: {
                        visible: true,
                        allowSearch: true
                    },
                    searchPanel: {
                        visible: true,
                        highlightCaseSensitive: true,
                    },
                    scrolling: {
                        mode: 'virtual'
                    },
                    height: "350px",
                    columns: [
                        {
                            dataField: 'acT_NAME',
                            caption: 'Name'
                        }
                    ],
                    expandedRowKeys: [0],
                    showRowLines: true,
                    onRowDblClick: function (info) {
                        const clickedRowData = info.data;
                        empr_MerchantPurchaseOrder.GetMerchantPurchaseOrderByID(clickedRowData.acT_CODE);
                    }
                });
            },
            error: function (error) {
                //console.error('Error fetching data:', error);
            }
        });


    },
    reFreshTree: function () {
        $("#treeListContainer").dxTreeList("instance").refresh();
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitDropdowns: function () {
        empr_MerchantPurchaseOrder.InitReportTypeDDL();
        empr_MerchantPurchaseOrder.InitEmpDDL();
        empr_MerchantPurchaseOrder.InitDepartmentDDL();
        empr_MerchantPurchaseOrder.InitCurrencyDDL();
        empr_MerchantPurchaseOrder.InitCommissionAmtDDL("PR");
        empr_MerchantPurchaseOrder.InitUnitDDL();
        //empr_MerchantPurchaseOrder.InitClientPODDL();
        //empr_MerchantPurchaseOrder.InitFabricDDL();
        //empr_MerchantPurchaseOrder.InitGSMDDL();
        ajaxHelper.ajaxGetJson("/DeliveryFormat/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_MerchantPurchaseOrder.InitItemCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);

        ajaxHelper.ajaxGetJson("/DeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                debugger;
                //console.log('GetParties', data);
                empr_MerchantPurchaseOrder.PartiesData = data.data;
                empr_MerchantPurchaseOrder.InitPartyCodeDDL(data.data);
                empr_MerchantPurchaseOrder.InitBrokerCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);

        ajaxHelper.ajaxGetJson("/MerchantPurchaseOrder/PayTermsDropdown", function (data) {
            //console.log('PayTermsDropdown', data);
            empr_MerchantPurchaseOrder.InitPayTermsDDL(data);
        }, false, true);

        ajaxHelper.ajaxGetJson("/MerchantPurchaseOrder/GetBrand", function (data) {
            //console.log('PayTermsDropdown', data);
            empr_MerchantPurchaseOrder.InitBrandDDL(data);
        }, false, true);

    },
    InitDropdownsWithValue: function (pCode, bCode, iCode, emp, dep, terms, unit, curr, grade) {
        //debugger;
        //empr_MerchantPurchaseOrder.InitEmpDDL(emp);
        empr_MerchantPurchaseOrder.InitReportTypeDDL();
        //empr_MerchantPurchaseOrder.InitDepartmentDDL(dep);
        ajaxHelper.ajaxGetJson("/DeliveryFormat/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_MerchantPurchaseOrder.InitItemCodeDDL(data.data, iCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/DeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_MerchantPurchaseOrder.InitPartyCodeDDL(data.data, pCode);
                empr_MerchantPurchaseOrder.InitBrokerCodeDDL(data.data, bCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/MerchantPurchaseOrder/PayTermsDropdown", function (data) {
            empr_MerchantPurchaseOrder.InitPayTermsDDL(data, terms);
            //empr_MerchantPurchaseOrder.InitUnitDDL(unit);
            empr_MerchantPurchaseOrder.InitCurrencyDDL(curr);
        }, false, true);

        ajaxHelper.ajaxGetJson("/MerchantPurchaseOrder/GetBrand", function (data) {
            //debugger;
            empr_MerchantPurchaseOrder.InitBrandDDL(data, grade);
        }, false, true);
    },
    InitPartyCodeDDL: function (dataSource, selectedValue) {
        //console.log(selectedValue);
        $('#PARTY_CODE').dxSelectBox({
            dataSource: {
                store: dataSource,
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
            searchTimeout: 500,
            //disabled: true
        });
    },
    InitBrokerCodeDDL: function (dataSource, selectedValue) {
        $('#SPARTY_CODE').dxSelectBox({
            dataSource: {
                store: dataSource,
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
    },
    InitPayTermsDDL: function (dataSource, selectedValue) {
        $('#TERMS').dxSelectBox({
            dataSource: {
                store: dataSource,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
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
    InitBrandDDL: function (dataSource, selectedValue) {
        $('#GRADE').dxSelectBox({
            dataSource: {
                store: dataSource,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
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
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {

        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);

    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/MerchantPurchaseOrder/GetReportTypes", function (data) {
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
        empr_MerchantPurchaseOrder.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the delivery in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }


        ajaxHelper.ajaxPostJsonData(dataModel, "/MerchantPurchaseOrder/GetPrintReport", function (data) {
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
    //InitSodaPickGrid: function () {
    //    //debugger
    //    var data = empr_MerchantPurchaseOrder.getDataToSave();
    //    //console.log(data.S_DATE)
    //    if (data.S_DATE == "" || data.S_DATE == null || data.S_DATE == undefined) {
    //        empr_helper.notify("Please select the soda date first.", 2);
    //    }
    //    else {
    //        empr_MerchantPurchaseOrder.GetSodaBookFeedingDetailBySodaDate(data.S_DATE);
    //    }
    //},
    //GetSodaBookFeedingDetailBySodaDate: function (sodaDate) {
    //    //debugger
    //    ajaxHelper.ajaxGetJson('/DeliveryFeeding/GetSodaBookFeedingDetailBySodaDate?sodaDate=' + sodaDate, function (data) {
    //        if (data.msgType == 1) {
    //            if (data.data.length > 0) {
    //                if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
    //                    $('#SodaPickGridContainer').data('dxDataGrid').dispose();
    //                }
    //                empr_MerchantPurchaseOrder.CreateSodaPickGrid(data.data);
    //                $('#SodaPickModal').modal('show');
    //            } else {
    //                empr_helper.notify("No soda found.", 2);
    //            }
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},
    //CreateSodaPickGrid: function (dataSrc) {
    //    //debugger
    //    //console.log(dataSrc)
    //    var col = [
    //        { dataField: 'traN_ID', caption: 'Code', visible: false, },
    //        { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
    //        { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
    //        { dataField: 'seller', caption: 'Seller', allowEditing: false, },
    //        { dataField: 'buyer', caption: 'Buyer', allowEditing: false, },
    //        { dataField: 'broker', caption: 'Broker', allowEditing: false, },
    //        { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
    //        { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
    //        { dataField: 'unit', caption: 'Unit', allowEditing: false, visible: false },
    //        { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false },
    //        { dataField: 'qty', caption: 'S. QTY', allowEditing: false, },
    //        { dataField: 'dqty', caption: 'I. QTY', allowEditing: false, },
    //        { dataField: 'baL_QTY', caption: 'Balance Quantity', allowEditing: false, },
    //        { dataField: 'rate', caption: 'Rate', allowEditing: false, },
    //        { dataField: 'amt', caption: 'Amount', allowEditing: false, },
    //    ];
    //    empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "DeliveryFormatPick", "v_DATE", 'multiple');
    //    setTimeout(function () {
    //        $('#SodaPickGridContainer').dxDataGrid('instance').resize();
    //    }, 500);
    //},
    //AddSodaToDelivery: function () {
    //    if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
    //        $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
    //            var data = empr_MerchantPurchaseOrder.getDataToSave();
    //            var IsDataAvailableInGrid = false;

    //            var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //            if (selectedSodas.length > 0) {
    //                $('#REF').val(selectedSodas[0].voucheR_NO);
    //                $('#TBAG').val(selectedSodas[0].qty);
    //                empr_MerchantPurchaseOrder.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].SPARTY_CODE, selectedSodas[0].iteM_CODE);
    //            }
    //            $('.modal').hide();
    //            $('#V_DATE').focus();
    //        });
    //    }
    //    else {
    //        var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //        if (selectedSodas.length > 0) {
    //            $('#REF').val(selectedSodas[0].voucheR_NO);
    //            $('#TBAG').val(selectedSodas[0].qty);
    //            empr_MerchantPurchaseOrder.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].SPARTY_CODE, selectedSodas[0].iteM_CODE, selectedSodas[0].unit);
    //        }
    //        $('.modal').hide();
    //        $('#V_DATE').focus();
    //    }
    //},
    
    

    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    InitCurrencyDDL: function (_selectedValue) {
        $.ajax({
            url: 'MerchantPurchaseOrder/GetCurrencies',
            method: 'GET',
            success: function (data) {
                //console.log(data.data)
                if (data.msgType == 1) {
                    $('#CURR_CODE').dxSelectBox({
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
                                    $('#CRATE').val(item[0].rate);
                                }
                            }
                            else {
                                $('#CRATE').val('');
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

    InitCommissionAmtDDL: function (selectedValue) {
        //debugger;
        var dataSource = empr_helper.commType;

        $('#COMM_AMT').dxSelectBox({
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
            searchTimeout: 500,
            onValueChanged: function (e) {
                if (e.value === 'RS') {
                    $('#COMM').prop('disabled', true);
                    $('#COMM').val('');
                    $('#COMM_VAL').prop('disabled', false).removeAttr('readonly');
                    $('#COMM_VAL').val('');
                } else {
                    $('#COMM').prop('disabled', false);
                    $('#COMM_VAL').prop('disabled', true).attr('readonly', true);
                    $('#COMM_VAL').val('');
                    $('#COMM').val('');
                }
            },
        });
    },

    //InitUnitDDL: function (_selectedValue) {

    //    $.ajax({
    //        url: 'DeliveryFormat/GetUnits',
    //        method: 'GET',
    //        data: null,
    //        success: function (data) {

    //            var selectedvalue = 0;
    //            var selectedobj = [];
    //            if (_selectedValue != null) {
    //                selectedobj = data.filter(x => x.key == _selectedValue);
    //                if (selectedobj.length > 0) {
    //                    selectedvalue = _selectedValue;
    //                    $("#unitcode_hidden").val(selectedvalue);
    //                    $("#displayExpr_unitcode").val(selectedobj[0].value);
    //                }
    //            }

    //            empr_MerchantPurchaseOrder.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
    //                if (selectedvalue.selectedRowsData.length > 0) {
    //                    var key = selectedvalue.selectedRowsData[0]['key'];
    //                    var value = selectedvalue.selectedRowsData[0]['value'];
    //                    $('#unitcode_hidden').val(key);
    //                    $('#displayExpr_unitcode').val(value);
    //                }
    //                else {
    //                    $('#unitcode_hidden').val('');
    //                    $('#displayExpr_unitcode').val('');
    //                }
    //            });
    //        },
    //        error: function (error) {
    //        }
    //    });
    //},

    //InitDepartmentDDL: function (_selectedValue) {
    //    $.ajax({
    //        url: 'MerchantPurchaseOrder/GetDepartments',
    //        method: 'GET',
    //        success: function (data) {

    //            var selectedvalue = 0;
    //            var selectedobj = [];
    //            if (_selectedValue != null) {
    //                selectedobj = data.filter(x => x.key == _selectedValue);
    //                if (selectedobj.length > 0) {
    //                    selectedvalue = _selectedValue;
    //                    $("#department_hidden").val(selectedvalue);
    //                    $("#displayExpr_department").val(selectedobj[0].value);
    //                }
    //            }

    //            empr_MerchantPurchaseOrder.BindDxGridBoxDdl('#DEP', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_department', function (selectedvalue, hidden) {

    //                if (selectedvalue.selectedRowsData.length > 0) {
    //                    var key = selectedvalue.selectedRowsData[0]['key'];
    //                    var value = selectedvalue.selectedRowsData[0]['value'];
    //                    var rate = selectedvalue.selectedRowsData[0]['rate'];
    //                    $('#department_hidden').val(key);
    //                    $('#displayExpr_department').val(value);
    //                }
    //                else {
    //                    $('#department_hidden').val('');
    //                    $('#displayExpr_department').val('');
    //                }
    //            });
    //        },
    //        error: function (error) {
    //            //console.error('Error fetching data:', error);
    //        }
    //    });
    //},

    //InitEmpDDL: function (_selectedValue) {

    //    $.ajax({
    //        url: 'MerchantPurchaseOrder/GetEmp',
    //        method: 'GET',
    //        data: null,
    //        success: function (data) {
    //            //console.log('Emp', data);
    //            var selectedvalue = 0;
    //            var selectedobj = [];
    //            if (_selectedValue != null) {
    //                selectedobj = data.filter(x => x.key == _selectedValue);
    //                if (selectedobj.length > 0) {
    //                    selectedvalue = _selectedValue;
    //                    $("#empcode_hidden").val(selectedvalue);
    //                    $("#displayExpr_empcode").val(selectedobj[0].value);
    //                }
    //            }

    //            empr_MerchantPurchaseOrder.bindDxGridBoxDdl('#EMP', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_empcode', function (selectedvalue, hidden) {
    //                if (selectedvalue.selectedRowsData.length > 0) {
    //                    var key = selectedvalue.selectedRowsData[0]['key'];
    //                    var value = selectedvalue.selectedRowsData[0]['value'];
    //                    $('#empcode_hidden').val(key);
    //                    $('#displayExpr_empcode').val(value);
    //                }
    //                else {
    //                    $('#empcode_hidden').val('');
    //                    $('#displayExpr_empcode').val('');
    //                }
    //            });
    //        },
    //        error: function (error) {
    //            //console.error('Error fetching data:', error);
    //        }
    //    });


    //},

    InitUnitDDL: function (selectedValue) {

        $('#UNIT').dxSelectBox({
            dataSource: Units,
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
            searchTimeout: 500,
        });
    },

    InitDepartmentDDL: function (selectedValue) {

        $('#DEP').dxSelectBox({
            dataSource: Departments,
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
            searchTimeout: 500,
        });
    },

    InitEmpDDL: function (selectedValue) {

        $('#EMP').dxSelectBox({
            dataSource: Employee,
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
            searchTimeout: 500,
        });
    },



    //UploadDoc: function () {
    //    $('#saveAttempt').prop('disabled', true);
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
    //                $('#saveAttempt').prop('disabled', false);

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

    UploadDoc: function () {
        debugger;
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        if (files.length > 0) {
            $('#DOCName').val(files[0].name); // file ka naam dikhaye
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


}