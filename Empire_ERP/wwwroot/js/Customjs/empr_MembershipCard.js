var empr_MembershipCard = {
    initEvents: function () {

        $(document).ready(function () {
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_MembershipCard.GetMembershipCardByID(data.traN_ID);
                }
            });

            empr_MembershipCard.InitCardTypeDDL();

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_MembershipCard.validateForm()) {
                            empr_MembershipCard.saveAttempt();
                        }
                    }
                } else {
                    if (empr_MembershipCard.validateForm()) {
                        empr_MembershipCard.saveAttempt();
                    }
                }
            })

            $('#generateCode').click(function () {
                empr_MembershipCard.GenerateCardNo();
            })

            $('body').on('click', '#quicksearch', function () {
                empr_MembershipCard.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_MembershipCard.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_MembershipCard.GetMembershipCardByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_MembershipCard.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_MembershipCard.DeleteRecord();
            })

            $('body').on('click', '#BtnSodaPick', function () {
                empr_MembershipCard.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_MembershipCard.AddSodaToDelivery();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_MembershipCard.GeneratePrintReport();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/MembershipCard/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('.btn-print').hide();
                    $('#resetall').hide();
                    empr_MembershipCard.resetForm();
                    empr_MembershipCard.InitTree();
                    empr_MembershipCard.reFreshTree();
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
        $('#ASTATUS').dxSelectBox('instance').option('disabled', false);
        $('#CARD_TYPE').dxSelectBox('instance').option('value', '');
        $("#PointStartValue").val('');
        $("#PointRate").val('');
        $("#MaxPointsBeforeDiscount").val('');
        document.getElementById('CLOSED').checked = false;
        $("#V_DATE, #START_D, #END_D, #CLOSED, #FIRST_NAME, #LAST_NAME, #CARD_NO, #PHONE_NO, #CNIC_NO, #EMAIL").val('');
        $("#CARD_NO").prop("disabled", false);
        $('#V_DATE').val(todayDate);
        $('#generateCode').show();
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
    },

    validateForm: function () {
        debugger;
        var valid = true;

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');

        var CARD_TYPE = $('#CARD_TYPE').dxSelectBox('instance').option('value');

        var EMAIL = $('#EMAIL').val();

        var CARD_NO = $('#CARD_NO').val();

        valid = empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

        if (ASTATUS == '' || ASTATUS == null) {
            valid = false;
            empr_helper.notify("Please select active.", 2);
        }

        if (CARD_TYPE == '' || CARD_TYPE == null) {
            valid = false;
            empr_helper.notify("Please select Card Type.", 2);
        }

        if (CARD_NO == '' || CARD_NO == null) {
            valid = false;
            empr_helper.notify("Please enter card number.", 2);
        }

        if (EMAIL != '') {
            if (!empr_helper.isEmail(EMAIL)) {
                valid = false;
                empr_helper.notify("Please add valid email address.", 2);
            }
        }


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
        var FIRST_NAME = $("#FIRST_NAME").val();
        var LAST_NAME = $("#LAST_NAME").val();
        var CARD_NO = $("#CARD_NO").val();
        var CNIC_NO = $("#CNIC_NO").val();
        var PHONE_NO = $("#PHONE_NO").val();
        var EMAIL = $("#EMAIL").val();
        var COMMENT = $("#COMMENT").val();
        var START_D = $("#START_D").val();
        var END_D = $("#END_D").val();
        var DISCOUNT = $("#DISCOUNT").val();
        var PointStartValue = $("#PointStartValue").val();
        var PointRate = $("#PointRate").val();
        var MaxPointsBeforeDiscount = $("#MaxPointsBeforeDiscount").val();
        var CLOSED = document.getElementById('CLOSED').checked ? 1 : 0;
        var CARD_TYPE = $('#CARD_TYPE').dxSelectBox('instance').option('value');

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
            FIRST_NAME: FIRST_NAME,
            LAST_NAME: LAST_NAME,
            CARD_NO: CARD_NO,
            CNIC_NO: CNIC_NO,
            PHONE_NO: PHONE_NO,
            EMAIL: EMAIL,
            COMMENT: COMMENT,
            START_D: START_D,
            END_D: END_D,
            DISCOUNT: DISCOUNT,
            CLOSED: CLOSED,
            CARD_TYPE: CARD_TYPE,
            PointStartValue: PointStartValue,
            PointRate: PointRate,
            MaxPointsBeforeDiscount: MaxPointsBeforeDiscount
        }
        return modelRecord;
    },
    saveAttempt: function () {
        
        var obj = empr_MembershipCard.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/MembershipCard/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#optmodal').modal('hide');
                $('.btn-delete').show();
                $('.btn-print').show();
                $('#resetall').show();

            }
        }, false, true);
    },
    InintQuickSearch: function () {
        empr_MembershipCard.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/MembershipCard/QuickSearch', function (data) {
            if (data.msgType == 1) {
                console.log(data.data)
                empr_MembershipCard.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllMembershipCards: function () {

        var xhr = ajaxHelper.ajaxGetJson('/MembershipCard/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_MembershipCard.CreateGrid(data.data);

        }, false, true);

    },
    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },
    GetMembershipCardByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/MembershipCard/MembershipCardByid?id=' + id, function (data) {

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
            $('#generateCode').hide();

            console.log(data.data);

            $('.modal').modal('hide')

            $('#Code').val(data.data.id);
            $("#V_DATE").val(data.data.v_DATE);
            $("#FIRST_NAME").val(data.data.firsT_NAME);
            $("#LAST_NAME").val(data.data.lasT_NAME);
            $("#CARD_NO").val(data.data.carD_NO);
            $("#CNIC_NO").val(data.data.cniC_NO);
            $("#PHONE_NO").val(data.data.phonE_NO);
            $("#EMAIL").val(data.data.email);
            $("#COMMENT").val(data.data.comment);
            $("#START_D").val(data.data.starT_D);
            $("#END_D").val(data.data.enD_D);
            $('#activestatushidden').val(data.data.astatus);
            $('#PointStartValue').val(data.data.pointStartValue);
            $('#PointRate').val(data.data.pointRate);
            $('#MaxPointsBeforeDiscount').val(data.data.maxPointDisc);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            $('#CARD_TYPE').dxSelectBox('instance').option("value", data.data.carD_TYPE);
            $("#CARD_NO").prop("disabled", true);
            console.log()
            if (data.data.closed == 1)
                document.getElementById('CLOSED').checked = true;

            //empr_MembershipCard.InitDropdownsWithValue(data.data.partY_CODE, data.data.brokeR_CODE, data.data.iteM_CODE, data.data.unit);


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

                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                            <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                                </div>`).appendTo(container);


            }
        },

        { dataField: 'traN_ID', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'v_DATE', caption: 'Date' },
        { dataField: 'firsT_NAME', caption: 'First Name' },
        { dataField: 'lasT_NAME', caption: 'Last Name' },
        { dataField: 'carD_TYPE', caption: 'Card Type' },
        { dataField: 'cniC_NO', caption: 'CNIC' },
        { dataField: 'phonE_NO', caption: 'Phone' },
        { dataField: 'email', caption: 'Email' },
        { dataField: 'comment', caption: 'Comment' },
        { dataField: 'starT_D', caption: 'Start Date' },
        { dataField: 'enD_D', caption: 'End Date' },
        { dataField: 'closed', caption: 'Closed' },
        { dataField: 'astatus', caption: 'Status' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "MembershipCardQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/MembershipCard/QuickSearchLazyLoading", "traN_ID", "MembershipCards");
    },
    InitTree: function () {

        $.ajax({
            url: 'MembershipCard/GetAccountsForTreeView',
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
                        empr_MembershipCard.GetMembershipCardByID(clickedRowData.acT_CODE);
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
    InitCardTypeDDL: function (_selectedValue) {
        console.log(CardType);

        $('#CARD_TYPE').dxSelectBox({
            dataSource: CardType,
            displayExpr: "value",    // ✅ quotes mein hona chahiye
            valueExpr: "key",        // ✅ quotes mein hona chahiye
            value: _selectedValue,   // ✅ aapka function param
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

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/MembershipCard/GetReportTypes", function (data) {
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
        empr_MembershipCard.InitReportTypeDDL();
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


        ajaxHelper.ajaxPostJsonData(dataModel, "/MembershipCard/GetPrintReport", function (data) {
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

    GenerateCardNo: function () {
        ajaxHelper.ajaxGetJson("/MembershipCard/GenerateCardNo", function (data) {
            $('#CARD_NO').val(data);
        }, false, true);
    },

    InitSodaPickGrid: function () {
        debugger
        var data = empr_MembershipCard.getDataToSave();
        console.log(data.S_DATE)
        if (data.S_DATE == "" || data.S_DATE == null || data.S_DATE == undefined) {
            empr_helper.notify("Please select the soda date first.", 2);
        }
        else {
            empr_MembershipCard.GetSodaBookFeedingDetailBySodaDate(data.S_DATE);
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
                    empr_MembershipCard.CreateSodaPickGrid(data.data);
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
        empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "MembershipCardPick", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_MembershipCard.getDataToSave();
                var IsDataAvailableInGrid = false;

                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    $('#S_NO').val(selectedSodas[0].voucheR_NO);
                    $('#TBAG').val(selectedSodas[0].qty);
                    empr_MembershipCard.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].iteM_CODE);
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
                empr_MembershipCard.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].iteM_CODE, selectedSodas[0].unit);
            }
            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },
    InitUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'MembershipCard/GetUnits',
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

                empr_MembershipCard.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
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