var empr_PartyOpening = {
    InitEvents: function () {
        $(document).ready(function () {
            empr_PartyOpening.InitGrid();
            $('body').on('click', '.elm_edit', function () {
                $('#IsEdit').val('false');
                var id = $(this).attr("reportid");
                var name = $(this).attr("reportname");
                $('#PARTY_CODE').val(id);
                $("#myExtraLargeModalLabel").text('Party Name : ' + name);
                empr_PartyOpening.GetPartyOpeningDetails(id);
            });

            $('#AMOUNT, #STAX_AMT').keyup(function () {
                empr_PartyOpening.SetNetAmount();
            });

            $('body').on('click', '#BtnSave', function () {
                if (empr_PartyOpening.ValidateMainInfo()) {
                    empr_PartyOpening.Save();
                }
            });

            $('body').on('click', '.elm_edit_detail', function () {
                var id = $(this).attr("reportid");
                $('#IsEdit').val('true');
                empr_PartyOpening.ResetForm();
                empr_PartyOpening.GetPartyOpeningByCode(id);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_PartyOpening.Delete();
            });

            if (Permissions != "Admin") {
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    InitGrid: function () {
        empr_PartyOpening.GetPartyOpenings();
    },
    GetPartyOpenings: function () {
        ajaxHelper.ajaxGetJson('/PartyOpeningBalance/GetPartyOpenings', function (data) {
            if (data.msgType == 1) {
                empr_PartyOpening.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
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

                    $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} reportname="${options.data.partY_NAME}"  title="Edit"><i class="fa fa-edit"></i></a>
                       </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', width: '80', cssClass: 'text-center', },
            { dataField: 'partY_NAME', caption: 'Name' },
            { dataField: 'partY_SHORT_NAME', caption: 'Short Name' },
            { dataField: 'acT_NAME', caption: 'Account Name' },
            { dataField: 'dC_TYPE', caption: 'Debit / Credit' },
            { dataField: 'amount', caption: 'Total Amount' },
            { dataField: 'astatus', caption: 'Status' },
            { dataField: 'category', caption: 'Category' },
            { dataField: 'contacT_PERSON', caption: 'Contact Person' },
            { dataField: 'cell', caption: 'Cell' },
            { dataField: 'paymenT_TERMS', caption: 'Payment Terms' },
            { dataField: 'crediT_LIMIT', caption: 'Credit Limit' },
            { dataField: 'salesperson', caption: 'Sales Person', visible: false, },
            { dataField: 'saleS_ACT_NAME', caption: 'Sales Account Name', visible: false, },                  
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "PartyOpening");
    },
    GetPartyOpeningDetails: function (id) {
        ajaxHelper.ajaxGetJson('/PartyOpeningBalance/GetPartyOpeningDetails?code=' + id, function (data) {
            if (data.msgType == 1) {
                var response = data.data;
                if (response.length > 0) {
                    empr_PartyOpening.ResetForm();

                    $("#BTYPE").val(response[0].type);
                    $("#ACT_CODE").val(response[0].acT_CODE);
                    $("#PARTYTYPE_CODE").val(response[0].partY_TYPE_CODE);
                    if (response[0].paymenT_TERMS == 0) {
                        $("#TERMS").val('');
                    }
                    else {
                        $("#TERMS").val(response[0].paymenT_TERMS);
                    }
                    $("#SALES_CODE").val(response[0].s_CODE);
                    $("#SALESACT_CODE").val(response[0].sacT_CODE);
                    $("#DEFAULT_DC_TYPE").val(response[0].dC_TYPE);
                    empr_PartyOpening.InitTransactionTypeDDL(response[0].dC_TYPE);
                    empr_PartyOpening.GetPartyOpeningDetailByCode(id);
                    $('#EditModal').modal('show');

                    if (Permissions != "Admin") {
                        (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
                    }
                }
                else {
                    empr_helper.notify("Something went wrong! please try again later.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetPartyOpeningDetailByCode: function (id) {
        ajaxHelper.ajaxGetJson('/PartyOpeningBalance/GetPartyOpeningDetailByCode?code=' + id, function (data) {
            if (data.msgType == 1) {
                empr_PartyOpening.CreateDetailGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateDetailGrid: function (dataSrc) {
        var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger

                    $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;"  class="grid-action-icon elm_edit_detail" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                       </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', cssClass: 'text-center' },
            { dataField: 'btype', caption: 'Type' },
            { dataField: "astatus", caption: "Status", width: 100, alignment: "center" },
            { dataField: 'bilL_NO', caption: 'Bill No' },
            { dataField: 'bilL_DATE', caption: 'Bill Date', dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'dC_TYPE', caption: 'Debit/Credit' },
            { dataField: 'amount', caption: 'Amount' },
            { dataField: 'staX_AMT', caption: 'Sales Tax' },
            { dataField: 'tamt', caption: 'Net Amount' },
            { dataField: 'comM_AMT', caption: 'Commission Amount' },
            { dataField: 'terms', caption: 'Payment Terms' },
            { dataField: 'crate', caption: 'Rate' },
            { dataField: 'curR_NAME', caption: 'Currency' },
            { dataField: 'ddesc', caption: 'Description' },
            { dataField: 'adD_USER_ID', caption: 'Add USER', visible: false },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Add COMPUTER NAME', visible: false },
            { dataField: 'adD_DATE', caption: 'Add DATE', visible: false, dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'adD_IP_ADDRESS', caption: 'Add IP ADDRESS', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Add POSTALCODE', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Edit USER', visible: false },
            { dataField: 'ediT_DATE', caption: 'Edit DATE', visible: false, dataType: 'date', format: 'dd-MM-yyyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Edit COMPUTER NAME', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Edit IP ADDRESS', visible: false },
            { dataField: 'ediT_POSTALCODE', caption: 'Edit POSTALCODE', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#detailContainer', col, dataSrc, "PartyOpeningDetail");
    },
    SetNetAmount: function () {
        let amount = parseFloat($('#AMOUNT').val()) || 0;
        let taxAmount = parseFloat($('#STAX_AMT').val()) || 0;
        $('#TAMT').val(amount + taxAmount);  
    },
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var PARTY_CODE = $("#PARTY_CODE").val();
        var ACT_CODE = $("#ACT_CODE").val();
        var SALES_CODE = $("#SALES_CODE").val();
        var SALESACT_CODE = $("#SALESACT_CODE").val();
        var PARTYTYPE_CODE = $("#PARTYTYPE_CODE").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var BILL_NO = $("#BILL_NO").val();
        var BTYPE = $("#BTYPE").val();
        var BILL_DATE = $("#BILL_DATE").val();
        var DC_TYPE = $('#DC_TYPE').dxSelectBox('option', 'value');
        var AMOUNT = $("#AMOUNT").val();
        var STAX_AMT = $("#STAX_AMT").val();
        var TAMT = $("#TAMT").val();
        var COMM_AMT = $("#COMM_AMT").val();
        var TERMS = $("#TERMS").val();
        var CURR_CODE = $("#currency_hidden").val();
        var CRATE = $("#CRATE").val();
        var DDESC = $("#DDESC").val();
        var modelRecord = {
            OP_ID: CODE,
            BTYPE: BTYPE,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            SALES_CODE: SALES_CODE,
            SALESACT_CODE: SALESACT_CODE,
            PARTYTYPE_CODE: PARTYTYPE_CODE,
            ASTATUS: ASTATUS,
            BILL_NO: BILL_NO,
            BILL_DATE: BILL_DATE,
            DC_TYPE: DC_TYPE,
            AMOUNT: AMOUNT,
            STAX_AMT: STAX_AMT,
            TAMT: TAMT,
            COMM_AMT: COMM_AMT,
            TERMS: TERMS,
            CURR_CODE: CURR_CODE,
            CRATE: CRATE,
            DDESC: DDESC,
        }
        return modelRecord;
    },
    ValidateMainInfo: function () {

        var valid = true;
        var data = empr_PartyOpening.GetDataToSave();

        if (data.DC_TYPE == '') {
            empr_helper.notify("Please select debit/credit.", 2);
            valid = false;
            return valid;
        }

        if (data.AMOUNT == '') {
            empr_helper.notify("Amount is required.", 2);
            valid = false;
            return valid;
        }

        if (data.CURR_CODE == '') {
            empr_helper.notify("Please select currency.", 2);
            valid = false;
            return valid;
        }

        if (data.CRATE == '') {
            empr_helper.notify("Rate is required.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    Save: function () {
        debugger;
        var dataModel = empr_PartyOpening.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(dataModel, "/PartyOpeningBalance/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#IsEdit').val('false');
                empr_PartyOpening.ResetForm();
                empr_PartyOpening.GetPartyOpeningDetailByCode(dataModel.PARTY_CODE);
                empr_PartyOpening.InitGrid();
            }
        }, false, true);
    },
    ResetForm: function () {

        $('.bs-example-modal-xl input').not('#PARTY_CODE, #ASTATUS, .dx-texteditor-input, #BTYPE, #ACT_CODE, #PARTYTYPE_CODE, #SALES_CODE, #SALESACT_CODE, #DEFAULT_DC_TYPE').val('');
        $('#currency_hidden').val('');
        $('#displayExpr_currency').val('');
        $('#BILL_DATE').val(periodStartDate);
        $('#BtnDelete').hide();
        empr_PartyOpening.InitTransactionTypeDDL($("#DEFAULT_DC_TYPE").val());
        empr_PartyOpening.InitCurrencyDDL();

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            }
            else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
    },
    GetPartyOpeningByCode: function (id) {
        ajaxHelper.ajaxGetJson('/PartyOpeningBalance/GetPartyOpeningByCode?code=' + id, function (data) {
            if (data.msgType == 1) {
                var response = data.data;
                $("#Code").val(response.id);
                $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                $("#BILL_NO").val(response.bilL_NO);
                $("#BILL_DATE").val(response.bilL_DATE);
                if (response.amount == 0) {
                    $("#AMOUNT").val('');
                }
                else {
                    $("#AMOUNT").val(response.amount);
                }
                if (response.staX_AMT == 0) {
                    $("#STAX_AMT").val('');
                }
                else {
                    $("#STAX_AMT").val(response.staX_AMT);
                }
                if (response.tamt == 0) {
                    $("#TAMT").val('');
                }
                else {
                    $("#TAMT").val(response.tamt);
                }

                if (response.comM_AMT == 0) {
                    $("#COMM_AMT").val('');
                }
                else {
                    $("#COMM_AMT").val(response.comM_AMT);
                }

                if (response.terms == 0) {
                    $("#TERMS").val('');
                }
                else {
                    $("#TERMS").val(response.terms);
                }

                if (response.crate == 0) {
                    $("#CRATE").val('');
                }
                else {
                    $("#CRATE").val(response.crate);
                }

                $("#DDESC").val(response.ddesc);
                $('#currency_hidden').val('');
                $('#displayExpr_currency').val('');
                empr_PartyOpening.InitTransactionTypeDDL(response.dC_TYPE);
                empr_PartyOpening.InitCurrencyDDL(response.curR_CODE);
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

                empr_PartyOpening.SetNetAmount();
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/PartyOpeningBalance/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_PartyOpening.ResetForm();
                    empr_PartyOpening.GetPartyOpeningDetailByCode($('#PARTY_CODE').val());
                    $('#BtnDelete').hide();
                    empr_PartyOpening.InitGrid();
                }
            }, false, true);
        });
    },
    InitCurrencyDDL: function (_selectedValue) {
        $.ajax({
            url: 'PartyOpeningBalance/GetCurrencies',
            method: 'GET',
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#currency_hidden").val(selectedvalue);
                        $("#displayExpr_currency").val(selectedobj[0].value);
                    }
                }
                else {
                    if ($('#IsEdit').val() == 'false') {
                        if (data.length > 0) {
                            selectedobj.push(data[0]);
                            if (selectedobj.length > 0) {
                                selectedvalue = selectedobj[0].key;
                                $("#currency_hidden").val(selectedvalue);
                                $("#displayExpr_currency").val(selectedobj[0].value);
                                $("#CRATE").val(selectedobj[0].rate);
                            }
                        }
                    }
                }

                empr_PartyOpening.BindDxGridBoxDdl('#CURR_CODE', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }, { dataField: 'rate', caption: 'Rate' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_currency', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        var rate = selectedvalue.selectedRowsData[0]['rate'];
                        $('#currency_hidden').val(key);
                        $('#displayExpr_currency').val(value);
                        $('#CRATE').val(rate);
                    }
                    else {
                        $('#currency_hidden').val('');
                        $('#displayExpr_currency').val('');
                        $('#CRATE').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitTransactionTypeDDL: function (selectedValue) {
        if (selectedValue == '1' || selectedValue == 'D') {
            selectedValue = 'D';
        }
        else {
            selectedValue = 'C';
        }

        $('#DC_TYPE').dxSelectBox({
            dataSource: [
                { value: 'D', text: 'Debit' },
                { value: 'C', text: 'Credit' }
            ],
            valueExpr: 'value',
            value: selectedValue,
            displayExpr: 'text',
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onInitialized: function (e) {
                e.component.option('value', 'C');
            }
        });
    },
    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
}