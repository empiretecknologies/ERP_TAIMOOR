var empr_DeliveryFormat = {
    initEvents: function () {

        $(document).ready(function () {
            empr_DeliveryFormat.InitDropdowns();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_DeliveryFormat.GetDeliveryFormatByID(data.traN_ID);
                }
            });
            //empr_DeliveryFormat.InitTree();
            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_DeliveryFormat.validateForm()) {
                            empr_DeliveryFormat.saveAttempt();
                        }
                    }
                } else {
                    if (empr_DeliveryFormat.validateForm()) {
                        empr_DeliveryFormat.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_DeliveryFormat.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_DeliveryFormat.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_helper.selectedBill = rportid;
                empr_DeliveryFormat.GetDeliveryFormatByID(rportid);

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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/DeliveryFormat/CopyRecord", function (data) {
                    console.log(data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_DeliveryFormat.GetDeliveryFormatByID(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_DeliveryFormat.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_DeliveryFormat.DeleteRecord();
            })

            $('body').on('click', '#BtnSodaPick', function () {
                empr_DeliveryFormat.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_DeliveryFormat.AddSodaToDelivery();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_DeliveryFormat.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }
        });

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/DeliveryFormat/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_DeliveryFormat.resetForm();
                    empr_DeliveryFormat.InitTree();
                    empr_DeliveryFormat.reFreshTree();
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
        $('#ASTATUS').dxSelectBox('option', 'value');
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", false);
        $('#ASTATUS').dxSelectBox('instance').option('disabled', false);

        $("#V_DATE").val('');
        $("#VOUCHER_NO").val('');
        $('#PARTY_CODE').dxSelectBox('instance').option('value', 0);
        $("#S_DATE").val('');
        $("#S_NO").val('');
        $('#BROKER_CODE').dxSelectBox('instance').option('value', 0);
        $("#GODOWN").val('');
        $("#KANTA").val('');
        $("#COND").val('');
        $("#C_NAME").val('');
        $("#CELL").val('');
        $("#LOT_NO").val('');
        $("#ORIGIN").val('');
        $('#ITEM_CODE').dxSelectBox('instance').option('value', 0);
        $("#TBAG").val(0);
        $('#V_DATE').val(todayDate);
        $('#S_DATE').val(todayDate);
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
        empr_DeliveryFormat.InitDropdowns();
    },
    validateForm: function () {

        var valid = true;
        //var ID = $("#ID").val().trim();
        //var ADD_USER_ID = $("#ADD_USER_ID").val()
        //var ADD_DATE = $("#ADD_DATE").val()
        //var ADD_COMPUTER_NAME = $("#ADD_COMPUTER_NAME").val();
        //var ADD_IP_ADDRESS = $("#ADD_IP_ADDRESS").val();
        //var ADD_POSTALCODE = $("#ADD_POSTALCODE").val();
        //var MENU_ID = $("#MENU_ID").val()
        //var ACT_GR_CODE = $("#ACT_GR_CODE").val();

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value')
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value')

        if (ASTATUS == '' || ASTATUS == null) {
            valid = false;
            empr_helper.notify("Please select active.", 2);
        }
        if (ITEM_CODE == '' || ITEM_CODE == null) {
            valid = false;
            empr_helper.notify("Please select Item.", 2);
        }
        if (PARTY_CODE == '' || PARTY_CODE == null) {
            valid = false;
            empr_helper.notify("Please select Customer Account.", 2);
        }

        if (!valid) return;

        valid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

        return valid;
    },
    getDataToSave: function () {
        var ID = $("#ID").val();
        var ADD_USER_ID = $("#ADD_USER_ID").val()
        var ADD_DATE = $("#ADD_DATE").val()
        var ADD_COMPUTER_NAME = $("#ADD_COMPUTER_NAME").val();
        var ADD_IP_ADDRESS = $("#ADD_IP_ADDRESS").val();
        var ADD_POSTALCODE = $("#ADD_POSTALCODE").val();
        var MENU_ID = $("#MENU_ID").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var Code = empr_helper.getCode();
        var TRAN_ID = $("#Code").val().trim();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');
        var S_DATE = $("#S_DATE").val();
        var S_NO = $("#S_NO").val();
        var BROKER_CODE = $('#BROKER_CODE').dxSelectBox('option', 'value');
        var GODOWN = $("#GODOWN").val();
        var KANTA = $("#KANTA").val();
        var COND = $("#COND").val();
        var C_NAME = $("#C_NAME").val();
        var CELL = $("#CELL").val();
        var LOT_NO = $("#LOT_NO").val();
        var ORIGIN = $("#ORIGIN").val();
        var ITEM_CODE = $('#ITEM_CODE').dxSelectBox('option', 'value');
        var TBAG = $("#TBAG").val();
        var UNIT = $("#unitcode_hidden").val();

        var modelRecord = {
            ID: ID,
            Code: Code,
            ADD_USER_ID: ADD_USER_ID,
            ADD_DATE: ADD_DATE,
            ADD_COMPUTER_NAME: ADD_COMPUTER_NAME,
            ADD_IP_ADDRESS: ADD_IP_ADDRESS,
            ADD_POSTALCODE: ADD_POSTALCODE,
            MENU_ID: MENU_ID,
            ASTATUS: ASTATUS,
            TRAN_ID: TRAN_ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            PARTY_CODE: PARTY_CODE,
            S_DATE: S_DATE,
            S_NO: S_NO,
            BROKER_CODE: BROKER_CODE,
            GODOWN: GODOWN,
            KANTA: KANTA,
            COND: COND,
            C_NAME: C_NAME,
            CELL: CELL,
            LOT_NO: LOT_NO,
            ORIGIN: ORIGIN,
            ITEM_CODE: ITEM_CODE,
            TBAG: TBAG,
            UNIT: UNIT
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_DeliveryFormat.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/DeliveryFormat/save", function (data) {
            console.log(data)
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                //empr_DeliveryFormat.resetForm();
                $('#Code').val(data.data);
                $('#VOUCHER_NO').val(data.data2);
                $('#optmodal').modal('hide');
                //$('#resetall').hide();
                empr_helper.selectedBill = data.data;
            }
            
            if (dataClear == 1) {
                empr_DeliveryFormat.GetDeliveryFormatByID(data.data);
                $('.btn-delete').show();
                $('.btn-print').show();
            }
            else {
                empr_DeliveryFormat.resetForm();
            }

        }, false, true);

    },
    InintQuickSearch: function () {
        empr_DeliveryFormat.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/DeliveryFormat/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_DeliveryFormat.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllDeliveryFormats: function () {

        var xhr = ajaxHelper.ajaxGetJson('/DeliveryFormat/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_DeliveryFormat.CreateGrid(data.data);

        }, false, true);

    },
    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },
    GetDeliveryFormatByID: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/DeliveryFormat/DeliveryFormatByid?id=' + id, function (data) {
            //$('.btn-delete').show();
            //$('.btn-print').show();
            //$('#resetall').show();

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
            empr_DeliveryFormat.makeReadOnly(true);

            $('#Code').val(data.data.id);

            $("#V_DATE").val(data.data.v_DATE);
            $("#S_DATE").val(data.data.s_DATE);
            $("#VOUCHER_NO").val(data.data.voucheR_NO);
            $("#S_NO").val(data.data.s_NO);
            $("#GODOWN").val(data.data.godown);
            $("#KANTA").val(data.data.kanta);
            $("#COND").val(data.data.cond);
            $("#C_NAME").val(data.data.c_NAME);
            $("#CELL").val(data.data.cell);
            $("#LOT_NO").val(data.data.loT_NO);
            $("#ORIGIN").val(data.data.origin);
            $("#TBAG").val(data.data.tbag);
            $('#activestatushidden').val(data.data.astatus);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);

            empr_DeliveryFormat.InitDropdownsWithValue(data.data.partY_CODE, data.data.brokeR_CODE, data.data.iteM_CODE, data.data.unit);


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
                debugger

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

        { dataField: 'traN_ID', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'v_DATE', caption: 'Voucher Date' },
        { dataField: 'voucheR_NO', caption: 'Voucher Number' },
        { dataField: 'iteM_CODE', caption: 'Item Name' },
        { dataField: 'astatus', caption: 'Status' },
        { dataField: 'partY_CODE', caption: 'Party Name' },
        { dataField: 'brokeR_CODE', caption: 'Broker Name' },
        { dataField: 's_DATE', caption: 'Soda Date' },
        { dataField: 's_NO', caption: 'Soda Number' },
        { dataField: 'godown', caption: 'Godown' },
        { dataField: 'kanta', caption: 'Kanta' },
        { dataField: 'cond', caption: 'Condition' },
        { dataField: 'c_NAME', caption: 'Cont. Person' },
        { dataField: 'cell', caption: 'Cell' },
        { dataField: 'origin', caption: 'Origin' },
        { dataField: 'tbag', caption: 'Total Bag', dataType: 'number' },
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
                console.log(data);
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
                        empr_DeliveryFormat.GetDeliveryFormatByID(clickedRowData.acT_CODE);
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
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
        empr_DeliveryFormat.InitReportTypeDDL();
        empr_DeliveryFormat.InitUnitDDL();
        ajaxHelper.ajaxGetJson("/DeliveryFormat/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_DeliveryFormat.InitItemCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/DeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_DeliveryFormat.InitPartyCodeDDL(data.data);
                empr_DeliveryFormat.InitBrokerCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    InitDropdownsWithValue: function (pCode, bCode, iCode, unit) {
        empr_DeliveryFormat.InitUnitDDL(unit);
        empr_DeliveryFormat.InitReportTypeDDL();
        ajaxHelper.ajaxGetJson("/DeliveryFormat/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_DeliveryFormat.InitItemCodeDDL(data.data, iCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/DeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_DeliveryFormat.InitPartyCodeDDL(data.data, pCode);
                empr_DeliveryFormat.InitBrokerCodeDDL(data.data, bCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    InitPartyCodeDDL: function (dataSource, selectedValue) {
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
            searchTimeout: 500
        });
    },
    InitBrokerCodeDDL: function (dataSource, selectedValue) {
        $('#BROKER_CODE').dxSelectBox({
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
        ajaxHelper.ajaxGetJson("/DeliveryFormat/GetReportTypes", function (data) {
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
        empr_DeliveryFormat.InitReportTypeDDL();
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


        ajaxHelper.ajaxPostJsonData(dataModel, "/DeliveryFormat/GetPrintReport", function (data) {
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
    InitSodaPickGrid: function () {
        debugger
        var data = empr_DeliveryFormat.getDataToSave();
        console.log(data.S_DATE)
        if (data.S_DATE == "" || data.S_DATE == null || data.S_DATE == undefined) {
            empr_helper.notify("Please select the soda date first.", 2);
        }
        else {
            empr_DeliveryFormat.GetSodaBookFeedingDetailBySodaDate(data.S_DATE);
        }
    },
    GetSodaBookFeedingDetailBySodaDate: function (sodaDate) {
        debugger
        ajaxHelper.ajaxGetJson('/DeliveryFeeding/GetSodaBookFeedingDetailBySodaDate?sodaDate=' + sodaDate, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_DeliveryFormat.CreateSodaPickGrid(data.data);
                    $('#SodaPickModal').modal('show');
                } else {
                    empr_helper.notify("No soda found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateSodaPickGrid: function (dataSrc) {
        debugger
        console.log(dataSrc)
        var col = [
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
            { dataField: 'seller', caption: 'Seller', allowEditing: false, },
            { dataField: 'buyer', caption: 'Buyer', allowEditing: false, },
            { dataField: 'broker', caption: 'Broker', allowEditing: false, },
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
            { dataField: 'unit', caption: 'Unit', allowEditing: false, visible: false },
            { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false },
            { dataField: 'qty', caption: 'S. QTY', allowEditing: false, },
            { dataField: 'dqty', caption: 'I. QTY', allowEditing: false, },
            { dataField: 'baL_QTY', caption: 'Balance Quantity', allowEditing: false, },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, },
            { dataField: 'amt', caption: 'Amount', allowEditing: false, },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "DeliveryFormatPick", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_DeliveryFormat.getDataToSave();
                var IsDataAvailableInGrid = false;
     
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    $('#S_NO').val(selectedSodas[0].voucheR_NO);
                    $('#TBAG').val(selectedSodas[0].qty);
                    empr_DeliveryFormat.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].iteM_CODE);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
            if (selectedSodas.length > 0) {
                $('#S_NO').val(selectedSodas[0].voucheR_NO);
                $('#TBAG').val(selectedSodas[0].qty);
                empr_DeliveryFormat.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].iteM_CODE, selectedSodas[0].unit);
            }
            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },
    InitUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'DeliveryFormat/GetUnits',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#unitcode_hidden").val(selectedvalue);
                        $("#displayExpr_unitcode").val(selectedobj[0].value);
                    }
                }

                empr_DeliveryFormat.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#unitcode_hidden').val(key);
                        $('#displayExpr_unitcode').val(value);
                    }
                    else {
                        $('#unitcode_hidden').val('');
                        $('#displayExpr_unitcode').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
}