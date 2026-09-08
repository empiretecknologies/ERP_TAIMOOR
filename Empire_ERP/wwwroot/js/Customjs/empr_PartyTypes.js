var empr_partyTypes = {
    salesman_Acode: 0,

    initEvents: function () {

        $(document).ready(function () {
            debugger;
            empr_partyTypes.resetForm();
            
            $('.saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_partyTypes.validateMainInfo()) {
                            empr_partyTypes.saveAttempt();
                        }
                    }
                } else {
                    if (empr_partyTypes.validateMainInfo()) {
                        empr_partyTypes.saveAttempt();
                    }
                }
            });

            $('body').on('input', '#NTN', function (e) {
                var value = $(this).val();
                debugger;
                if (value.length > 30) {
                    $(this).val(value.substring(0, 30)); // Limit to 30 characters
                    empr_helper.notify('Maximum 30 characters allowed!', 2);
                }
            });

            $('body').on('click', '#saveBranchInfo', function () {
                if (Permissions != "Admin") {
                    if (!$("#dCode").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#dCode").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_partyTypes.validateBranchInfo()) {
                            empr_partyTypes.SaveBranchInfo();
                        }
                    }
                } else {
                    if (empr_partyTypes.validateBranchInfo()) {
                        empr_partyTypes.SaveBranchInfo();
                    }
                }
            });

            $('#DocumentFile').change(function () {
                empr_partyTypes.uploadFile();
            })

            $('#CnicFile').change(function () {
                empr_partyTypes.uploadCNIC();
            })

            $('.delete').click(function () {
                var partytypecode = $('#Code').val();
                empr_partyTypes.delete(partytypecode);
            });

            $('body').on('click', '#quicksearch', function () {
                empr_partyTypes.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var rportid = $(this).attr("rportid")
                empr_partyTypes.GetPartyTypeByPartyTypeCode(rportid);
            });

            $('body').on('click', '.elm_copy', function () {
                var name = $(this).attr("partyName");
                var partyCode = $(this).attr("rportid");
                var partyTypeCode = $(this).attr("recordPartyTypeCode");
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
                    $('#updatedName').val(name);
                    empr_helper.selectedBill = partyCode;
                    empr_helper.partyTypeCode = partyTypeCode;
                    $('#CopyViewModalName').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, PARTY_TYPE_CODE: empr_helper.partyTypeCode, PARTY_NAME: $('#updatedName').val() }, "/PartyTypes/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_partyTypes.GetPartyTypeByPartyTypeCode(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '.elm_editBranchInfo', function () {
                var rportid = $(this).attr("rportid")
                var partyid = $('#Code').val();
                empr_partyTypes.GetBranchbyCode(rportid, partyid);
            });

            $('body').on('click', '#resetForm', function () {
                debugger;
                empr_partyTypes.resetForm();
                empr_partyTypes.InitBranchInfoForm();
            });

            $('#deleteBranchInfo').click(function () {
                empr_partyTypes.DeleteBranchInfo();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#resetForm').hide();
                !Permissions.r_VIEW && $('#quicksearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('.saveAttempt').hide();
            }

            //$('body').on('keyup ', '#COMM', function () {
            //    debugger;
            //    var v = $(this).val();
            //    if (parseInt(v) < 0 || parseInt(v) > 100) {
            //        empr_helper.notify(2, "Commission value can not be greater than 100 or less than 0.");
            //        $(this).val('');
            //    }
            //});
        });
    },
    validURl: function (url) {
        //var pattern = /^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$/;
        var pattern = /^[a-zA-Z0-9.-]+\.(com)$/i;
        return pattern.test(url);
    },
    validateMainInfo: function () {

        var valid = true;
        var data = empr_partyTypes.getDataToSave();
        console.log(data)
        if (data.PARTY_NAME.trim() == '') {
            empr_helper.notify("Party name is required.", 2);
            valid = false;
        }

        if (data.ACT_CODE == '') {
            empr_helper.notify("Chart of account is required.", 2);
            valid = false;
        }

        if (data.REGION == '' || data.REGION == null) {
            empr_helper.notify("Region is required.", 2);
            valid = false;
        }

        if (data.EMAIL != '') {
            if (!empr_helper.isEmail(data.EMAIL)) {
                empr_helper.notify("Please add valid email address.", 2);
                valid = false;
            }
        }

        //if (data.NTN != '') {

        //    if (!empr_partyTypes.onlyNumberHyphon(data.NTN)) {
        //        empr_helper.notify("Add valid NTN.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.CELL != '') {
        //    if (!empr_partyTypes.onlynumber(data.CELL)) {
        //        empr_helper.notify("Add valid cell.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.WB != '') {
        //    if (!empr_partyTypes.onlynumber(data.WB)) {
        //        empr_helper.notify("Add valid watsapp web.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.TELL != '') {
        //    if (!empr_partyTypes.onlynumber(data.TELL)) {
        //        empr_helper.notify("Add valid telephone.", 2);
        //        valid = false;
        //    }

        //}

        if (data.WEBSITE != '') {
            if (!empr_partyTypes.validURl(data.WEBSITE)) {
                empr_helper.notify("Add valid url.", 2);
                valid = false;
            }

        }

        //if (data.COMM != '') {
        //    if (!empr_partyTypes.isnumber(data.COMM)) {
        //        empr_helper.notify("Add valid value for commission.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.PARTY_NAME.trim() != '') {
        //    if (!empr_partyTypes.isValidAlphabet(data.PARTY_NAME)) {
        //        empr_helper.notify("Add valid PARTY_NAME.", 2);
        //        valid = false;
        //    }
        //}

        //if (data.BRANCH_NAME != '') {
        //    if (!empr_partyTypes.isValidAlphabet(data.BRANCH_NAME)) {
        //        empr_helper.notify("Add valid BRANCH NAME.", 2);
        //        valid = false;
        //    }
        //}

        //if (data.BANK_NAME != '') {
        //    if (!empr_partyTypes.isValidAlphabet(data.BANK_NAME)) {
        //        empr_helper.notify("Add valid BANK NAME.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.ACCOUNT_NUM != '') {
        //    if (!empr_partyTypes.isnumber(data.ACCOUNT_NUM)) {
        //        empr_helper.notify("Add valid ACCOUNT_NUM.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.SERVICE_TAX != '') {
        //    if (!empr_partyTypes.isnumber(data.SERVICE_TAX)) {
        //        empr_helper.notify("Add valid SERVICE_TAX.", 2);
        //        valid = false;
        //    }
        //}

        return valid;
    },
    validateBranchInfo: function () {

        var valid = true;
        var data = empr_partyTypes.getBranchInfoTabData();

        if (data.BRANCH_NAME.trim() == '') {
            //if (!empr_partyTypes.isValidAlphabet(data.BRANCH_NAME)) {
            //    empr_helper.notify("Add valid BRANCH_NAME.", 2);
            //    valid = false;
            //}
            //else {
                empr_helper.notify("Add valid BRANCH_NAME.", 2);
                valid = false;
            //}
        }

        //if (data.CONTACT_PERSON == '') {
        //    if (!empr_partyTypes.isValidAlphabet(data.CONTACT_PERSON)) {
        //        empr_helper.notify("Add valid CONTACT_PERSON.", 2);
        //        valid = false;
        //    }
        //}

        if (data.EMAIL != '') {
            if (!empr_helper.isEmail(data.EMAIL)) {
                empr_helper.notify("Add valid EMAIL.", 2);
                valid = false;
            }
        }

        //if (data.CEL != '') {
        //    if (!empr_partyTypes.isnumber(data.CEL)) {
        //        empr_helper.notify("Add valid CELL number.", 2);
        //        valid = false;
        //    }

        //}

        //if (data.TEL != '') {
        //    if (!empr_partyTypes.isnumber(data.TEL)) {
        //        empr_helper.notify("Add valid TELEPHONE number.", 2);
        //        valid = false;
        //    }

        //}

        if (data.PARTY_CODE > 0) {

        } else {
            empr_helper.notify("Add party type first.", 2);
            valid = false;
        }

        return valid;
    },
    InitQuickSearch: function () {

        empr_partyTypes.GetDataForQuickSeachGrid();
    },
    GetDataForQuickSeachGrid: function () {

        var xhr = ajaxHelper.ajaxGetJson('/PartyTypes/QuickSearchParty', function (data) {
            empr_partyTypes.CreateGrid(data.data);
        }, false, true);

    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc);
        var col = [
            {
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
                            <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                            <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" partyName="${options.data.partY_NAME}" rportid=${options.data.id} recordPartyTypeCode=${options.data.partY_TYPE_CODE} title="COPY"><i class="fa fa-copy"></i></a>
                            </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', visible: false },
            { dataField: 'partY_NAME', caption: 'Name' },
            { dataField: 'partY_SHORT_NAME', caption: 'Short Name' },
            { dataField: 'acT_NAME', caption: 'Account Name' },
            { dataField: 'paddress', caption: 'Address' },
            { dataField: 'comm', caption: 'Commission%' },
            { dataField: 'category', caption: 'Category' },
            { dataField: 'ntn', caption: 'NTN' },
            { dataField: 'email', caption: 'Email' },
            { dataField: 'cnic', caption: 'CNIC' },
            { dataField: 'cniC_EXP', caption: 'CNIC Expiry', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'cell', caption: 'Cell No.' },
            { dataField: 'contacT_PERSON', caption: 'Contact Person' },
            { dataField: 'wb', caption: 'Whatsapp No.' },
            { dataField: 'tell', caption: 'Telephone No.' },
            { dataField: 'website', caption: 'Website' },
            { dataField: 'paymenT_TERMS', caption: 'Terms' },
            { dataField: 'crediT_LIMIT', caption: 'Limit' },
            { dataField: 'salesperson', caption: 'Salesman' },
            { dataField: 'regioN_NAME', caption: 'Region' },
            { dataField: 'gst', caption: 'GST' },
            { dataField: 'servicE_TAX', caption: 'Service Tax' },
            { dataField: 't_CAT', caption: 'Tax Cat' },
            { dataField: 'wht', caption: 'WHT' },
            {
                dataField: 'exempT_DATE',
                caption: 'Exemption Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                calculateCellValue: function (rowData) {
                    if (rowData.t_CAT === 'Exempted' && rowData.exempT_DATE) {
                        return new Date(rowData.exempT_DATE); 
                    }
                    return null;
                }
            },

            { dataField: 'f_NAME', caption: 'Tax Payer' },
            { dataField: 'entitY_NAME', caption: 'Entity' },
            { dataField: 'statuS_NAME', caption: 'Status' },
            { dataField: 'accounT_NUM', caption: 'Account No.' },
            { dataField: 'banK_NAME', caption: 'Bank Name' },
            { dataField: 'brancH_NAME', caption: 'Branch Name' },
            { dataField: 'BANK_ADDRESS', caption: 'Bank Address' },
            { dataField: 'remarks', caption: 'Remarks' },
            { dataField: 'termS_CONDITION', caption: 'Condition' },
            //{ dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            //{ dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            //{ dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            //{ dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
            //{ dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            //{ dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            //{ dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            //{ dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            //{ dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
            //{ dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "PartyTypes", 'single');
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/PartyTypes/QuickSearchLazyLoading", "id", "PartyTypes");
    },
    GetPartyTypeByPartyTypeCode: function (id) {
        empr_partyTypes.resetForm();
        var xhr = ajaxHelper.ajaxGetJson('/PartyTypes/GetPartyTypeByPartyCode?partyCode=' + id, function (data) {
            var partydata = data.partyData.data;

            empr_partyTypes.InitCategoryDDL(partydata.caT_CODE);
            empr_partyTypes.InitRegionDDL(partydata.region);
            empr_partyTypes.InitEntityDDL(partydata.entity);
            empr_partyTypes.InitSalesManDDL(partydata.s_CODE);
            empr_partyTypes.InitChartOfAccountDDL(partydata.acT_CODE);
            debugger;

            $('#taxpayer').dxSelectBox('instance').option("value", partydata.f_CODE);
            $('#T_CAT').dxSelectBox('instance').option("value", partydata.t_CAT);

            $('#ASTATUS').dxSelectBox('instance').option("value", partydata.astatus);
            try {
                $('#chartofaccount').dxDropDownBox('instance').option("disabled", true);
            } catch (e) {

            }

            $("#Code").val(partydata.id);
            $("#MENU_ID").val(partydata.menU_ID);
            $("#PARTY_NAME").val(partydata.partY_NAME);
            $("#PARTY_SHORT_NAME").val(partydata.partY_SHORT_NAME);
            $("#PADDRESS").val(partydata.paddress);
            $("#NTN").val(partydata.ntn);
            $("#CNIC").val(partydata.cnic);
            $("#CONTACT_PERSON").val(partydata.contacT_PERSON);
            $("#CELL").val(partydata.cell);
            $("#WB").val(partydata.wb);
            $("#TELL").val(partydata.tell);
            $("#EMAIL").val(partydata.email);
            $("#WEBSITE").val(partydata.website);
            $("#WHT").val(partydata.wht);
            var Eday = ("0" + new Date(partydata.exempT_DATE).getDate()).slice(-2);
            var Emonth = ("0" + (new Date(partydata.exempT_DATE).getMonth() + 1)).slice(-2);
            var Edate = new Date(partydata.exempT_DATE).getFullYear() + "-" + (Emonth) + "-" + (Eday);
            $("#EXEM_DATE").val(Edate);



            //var day = ("0" + new Date(partydata.cniC_EXP).getDate()).slice(-2);
            //var month = ("0" + (new Date(partydata.cniC_EXP).getMonth() + 1)).slice(-2);
            //var sdate = new Date(partydata.cniC_EXP).getFullYear() + "-" + (month) + "-" + (day);
            $("#CNIC_EXP").val(partydata.cniC_EXP);


            $("#REMARKS").val(partydata.remarks);
            $("#PAYMENT_TERMS").val(partydata.paymenT_TERMS);
            $("#CREDIT_LIMIT").val(partydata.crediT_LIMIT);
            $("#SACT_CODE").val(partydata.sacT_CODE);
            $("#GST").val(partydata.gst);
            $("#SERVICE_TAX").val(partydata.servicE_TAX);
            $("#ACCOUNT_NUM").val(partydata.accounT_NUM);
            $("#TAX").val(partydata.tax);
            $("#BANK_NAME").val(partydata.banK_NAME);
            $("#BRANCH_NAME").val(partydata.brancH_NAME);
            $("#BANK_ADDRESS").val(partydata.banK_ADDRESS);
            $("#PartytypesDocs_hidden").val(partydata.doC_PIC);
            $("#PartytypesCNIC_hidden").val(partydata.cniC_PIC);
            $("#TERMS_CONDITION").val(partydata.termS_CONDITION);
            $("#COMM").val(partydata.comm);
            $("#DISC").val(partydata.disc);

            $('#pills-warningprofile-tab').show();
            $('#pills-warningcontact-tab').show();
            //$('.btn-delete').show();
            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
                }
                if (Permissions.r_EDIT) {
                    $('.saveAttempt').show();
                    $('#saveBranchInfo').show();
                }
                else {
                    $('.saveAttempt').hide();
                    $('#saveBranchInfo').hide();
                }
            } else {
                $('.saveAttempt').show();
                $('#saveBranchInfo').show();
                $('.btn-delete').show();
            }

            var branchInfoData = data.branchInfoData.data;
            if (branchInfoData != null) {
                empr_partyTypes.InitBranchInfoGrid(branchInfoData);
            }
            $('.modal').modal('hide');
            
        }, false, true);
    },
    InitBranchInfoGrid: function (dataSrc) {
        var col = [
            {
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
                            <a href="javascript:;"  class="grid-action-icon elm_editBranchInfo" rportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                            </div>`).appendTo(container);
                }
            },
            'id',
            'partY_CODE',
            'brancH_NAME',
            'contacT_PERSON',
            'email',
            'cel',
            'tel',
            'pB_ADD',
            { dataField: 'adD_DATE', visible: false },
            { dataField: 'adD_COMPUTER_NAME', visible: false },
            { dataField: 'adD_IP_ADDRESS', visible: false },
            { dataField: 'ediT_USER_ID', visible: false },
            { dataField: 'ediT_DATE', visible: false },
            { dataField: 'ediT_COMPUTER_NAME', visible: false },
            { dataField: 'ediT_IP_ADDRESS', visible: false },
            { dataField: 'adD_POSTALCODE', visible: false },
            { dataField: 'ediT_POSTALCODE', visible: false },
            { dataField: 'adD_USER_ID', visible: false }, 
        ]
        empr_helper.dxGridbindingVouchers('#DetailgridContainer', col, dataSrc, "PartyTypesBranches");
    },
    InitBranchInfoForm: function () {
        $('.btn-detaildelete').hide();
        $("#ID").val(0);
        $("#detailId").val('');
        $("#CreatedMenuId").val('');
        $("#name").val('');
        $("#contact").val('');
        $("#email").val('');
        $("#Cellnumber").val('');
        $("#telephone").val('');
        $("#address").val('');
        $('#dCode').val('');
    },
    SaveBranchInfo: function () {
        debugger;
        var data = empr_partyTypes.getBranchInfoTabData();
        var xhr = ajaxHelper.ajaxPostJsonData(data, "/PartyTypes/saveBranchInfo", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#branchinfoDelete').show();
                $('#dCode').val(data.data);
                empr_partyTypes.InitBranchInfoForm();
                empr_partyTypes.GetBranches();
            }
        }, false, true);
    },
    getBranchInfoTabData: function () {

        var ID = $("#ID").val();
        var detailId = $("#detailId").val();
        var CreatedMenuId = $("#CreatedMenuId").val();
        var CreatedAccountId = $("#chartofaccount_hidden").val();
        var name = $("#name").val();
        var PARTY_NAME = $("#PARTY_NAME").val();
        var contact = $("#contact").val();
        var email = $("#email").val();
        var Cellnumber = $("#Cellnumber").val();
        var telephone = $("#telephone").val();
        var address = $("#address").val();
        var branchCode = $('#dCode').val();// branch id
        var partyid = $('#Code').val();
        var modelRecord = {
            PARTY_CODE: partyid,
            ID: detailId,
            MENU_ID: CreatedMenuId,
            ACT_CODE: CreatedAccountId,
            BRANCH_NAME: name,
            PARTY_NAME: PARTY_NAME,
            EMAIL: email,
            CONTACT_PERSON: contact,
            CEL: Cellnumber,
            TEL: telephone,
            Code: branchCode,
            PB_ADD: address
        }
        return modelRecord;
    },
    DeleteBranchInfo: function (id) {
        debugger;
        id = $('#dCode').val();
        code = $('#Code').val();
        var xhr = ajaxHelper.ajaxPostJsonData(null, "/PartyTypes/DeleteBranchInfo?branchId=" + id + "&partyCode=" + code, function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#branchinfoDelete').hide();
                empr_partyTypes.InitBranchInfoForm();
                empr_partyTypes.GetBranches();
            }
        }, false, true);
    },
    GetBranches: function () {

        var partyid = $('#Code').val();
        if (partyid > 0) {
            var xhr = ajaxHelper.ajaxGetJson('/PartyTypes/GetBranchesByPartyCode?partyCode=' + partyid, function (data) {

                debugger;
                var data = data.data;
                empr_partyTypes.InitBranchInfoGrid(data);

            }, false, true);
        }

    },
    GetBranchbyCode: function (id, partyid) {

        var xhr = ajaxHelper.ajaxGetJson('/PartyTypes/GetBranchInfoByBranch?branchCode=' + id + "&partyCode=" + partyid, function (data) {

            debugger;
            var data = data.data;
            $("#ID").val(data.id);
            $("#detailId").val(data.id);
            $("#CreatedMenuId").val(data.id);
            $("#name").val(data.brancH_NAME);
            $("#contact").val(data.contacT_PERSON);
            $("#email").val(data.email);
            $("#Cellnumber").val(data.cel);
            $("#telephone").val(data.tel);
            $("#address").val(data.pB_ADD);
            $('#dCode').val(data.id);


            //$('.btn-detaildelete').show();

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-detaildelete').show();
                }
                if (Permissions.r_EDIT) {
                    $('#saveBranchInfo').show();
                }
                else {
                    $('#saveBranchInfo').hide();
                }
            } else {
                $('#saveBranchInfo').show();
                $('.btn-detaildelete').show();
            }
        }, false, true);

    },
    delete: function (_id) {
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
            var ACT_CODE = $("#chartofaccount_hidden").val();
            var xhr = ajaxHelper.ajaxPostJsonData({ partycode: _id, actCode: ACT_CODE }, "/PartyTypes/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {

                    empr_partyTypes.resetForm();
                }
            }, false, true);
        });
        //var xhr = ajaxHelper.ajaxPostJsonData({ partycode: _id }, "/PartyTypes/Delete", function (data) {
        //    empr_helper.notify(data.msg, data.msgType);
        //    if (data.msgType == 1) {

        //        empr_partyTypes.resetForm();
        //    }
        //}, false, true);

    },
    getDataToSave: function () {
        debugger;
        var ACT_CODE = $("#chartofaccount_hidden").val();
        //var CAT_CODE = $("#category_hidden").val();
        var S_CODE = $("#salesman_hidden").val();
        var ENTITY = $("#entity_hidden").val();
        //var REGION = $("#REGION_hidden").val();
        var REGION = $("#REGION").dxSelectBox('instance').option('value');
        var TAX = $("#TAX").val();
        var CAT_CODE = $("#CATDDLDX").dxSelectBox('instance').option('value');
        var F_CODE = $("#taxpayer").dxSelectBox('instance').option('value');
        var T_CAT = $("#T_CAT").dxSelectBox('instance').option('value');
        var WHT = $("#WHT").val();
        var EXEMPT_DATE = $("#EXEM_DATE").val();

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');

        var ID = $("#Code").val();
        var MENU_ID = $("#MENU_ID").val();
        var PARTY_CODE = $("#Code").val();
        var PARTY_NAME = $("#PARTY_NAME").val();
        var PARTY_SHORT_NAME = $("#PARTY_SHORT_NAME").val();
        var PADDRESS = $("#PADDRESS").val();
        var NTN = $("#NTN").val();
        var CNIC = $("#CNIC").val();
        var CONTACT_PERSON = $("#CONTACT_PERSON").val();
        var CELL = $("#CELL").val();
        var WB = $("#WB").val();
        var TELL = $("#TELL").val();
        var EMAIL = $("#EMAIL").val();
        var WEBSITE = $("#WEBSITE").val();
        var CNIC_EXP = $("#CNIC_EXP").val();
        var REMARKS = $("#REMARKS").val();
        var PAYMENT_TERMS = $("#PAYMENT_TERMS").val();
        var CREDIT_LIMIT = $("#CREDIT_LIMIT").val();
        var SACT_CODE = empr_partyTypes.salesman_Acode;
        var GST = $("#GST").val();
        var SERVICE_TAX = $("#SERVICE_TAX").val();
        var ACCOUNT_NUM = $("#ACCOUNT_NUM").val();
        var BANK_NAME = $("#BANK_NAME").val();
        var BRANCH_NAME = $("#BRANCH_NAME").val();
        var BANK_ADDRESS = $("#BANK_ADDRESS").val();
        var DOC_PIC = $("#PartytypesDocs_hidden").val();
        var CNIC_PIC = $("#PartytypesCNIC_hidden").val();
        var TERMS_CONDITION = $("#TERMS_CONDITION").val();
        var COMM = $("#COMM").val();
        var DISC = $("#DISC").val();
        var modelRecord = {
            ID: ID,
            MENU_ID: MENU_ID,
            PARTY_TYPE_CODE: PARTY_CODE,// need to discuss
            PARTY_CODE: PARTY_CODE,
            PARTY_NAME: PARTY_NAME,
            PARTY_SHORT_NAME: PARTY_SHORT_NAME,
            ACT_CODE: ACT_CODE,
            CAT_CODE: CAT_CODE,
            PADDRESS: PADDRESS,
            NTN: NTN,
            CNIC: CNIC,
            CONTACT_PERSON: CONTACT_PERSON,
            CELL: CELL,
            WB: WB,
            TELL: TELL,
            TAX: TAX,
            EMAIL: EMAIL,
            WEBSITE: WEBSITE,
            CNIC_EXP: CNIC_EXP,
            REMARKS: REMARKS,
            PAYMENT_TERMS: PAYMENT_TERMS,
            CREDIT_LIMIT: CREDIT_LIMIT,
            S_CODE: S_CODE,
            SACT_CODE: SACT_CODE,
            GST: GST,
            SERVICE_TAX: SERVICE_TAX,
            F_CODE: F_CODE,
            T_CAT: T_CAT,
            WHT: WHT,
            EXEMPT_DATE: EXEMPT_DATE,
            ENTITY: ENTITY,
            ASTATUS: ASTATUS,
            ACCOUNT_NUM: ACCOUNT_NUM,
            BANK_NAME: BANK_NAME,
            BRANCH_NAME: BRANCH_NAME,
            BANK_ADDRESS: BANK_ADDRESS,
            DOC_PIC: DOC_PIC,
            CNIC_PIC: CNIC_PIC,
            TERMS_CONDITION: TERMS_CONDITION,
            REGION: REGION,
            COMM: COMM,
            DISC: DISC
        }
        return modelRecord;
      
    },
    saveAttempt: function () {
        debugger;
        var dataModel = empr_partyTypes.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(dataModel, "/PartyTypes/SaveMainOtherInfo", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {

                $('#Code').val(data.data);
                $('#pills-warningprofile-tab').show();
                $('#pills-warningcontact-tab').show();
                //$('#pills-warningprofile').show();
                //$('#pills-warningcontact').show();
                $('.btn-delete').show();

                try {
                    $('#chartofaccount').dxDropDownBox('instance').option("disabled", true);
                }catch(e){

                }

                //if (dataModel.PARTY_CODE > 0) {
                //    empr_partyTypes.resetForm();
                //}
            }
        }, false, true);
    },
    resetForm: function () {

        
        $('#pills-warningprofile-tab').hide();
        $('#pills-warningcontact-tab').hide();
        $('#myTab li:first-child a').click();
        $('.tab-pane').removeClass('fade');
        $('.btn-delete').hide();
        $('#EXEM_DIV').hide();

        
        
        try {
            $('#chartofaccount').dxDropDownBox('instance').option("disabled", false);
        } catch (e) {

        }

        $('#CnicFile').val('');
        $('#DocumentFile').val('');
        $("#Code").val('');
        $("#MENU_ID").val('');
        $("#TAX").val('');
        $("#Code").val('');
        $("#PARTY_NAME").val('');
        $("#PARTY_SHORT_NAME").val('');
        $("#PADDRESS").val('');
        $("#NTN").val('');
        $("#CNIC").val('');
        $("#CONTACT_PERSON").val();
        $("#CELL").val('');
        $("#WB").val('');
        $("#TELL").val('');
        $("#EMAIL").val('');
        $("#WEBSITE").val('');
        $("#CNIC_EXP").val('');
        $("#REMARKS").val('');
        $("#PAYMENT_TERMS").val('');
        $("#CREDIT_LIMIT").val('');
        $("#SACT_CODE").val('');
        $("#GST").val('');
        $("#SERVICE_TAX").val('');
        $("#ACCOUNT_NUM").val('');
        $("#BANK_NAME").val('');
        $("#BRANCH_NAME").val('');
        $("#BANK_ADDRESS").val('');
        $("#TERMS_CONDITION").val('');
        $("#COMM").val('');
        $("#DISC").val('');
        $("#CONTACT_PERSON").val(''); 

        $("#chartofaccount_hidden").val('');
        $("#displayExpr_chartofaccount").val('');
        $("#category_hidden").val('');
        $("#displayExpr_category").val('');
        $("#salesman_hidden").val('');
        $("#displayExpr_salesman").val('');
        $("#entity_hidden").val('');
        $("#displayExpr_entity").val('');
        $("#REGION_hidden").val('');
        $("#displayExpr_REGION").val('');
        $("#PartytypesDocs_hidden").val('');
        $("#PartytypesCNIC_hidden").val('');
        $("#T_CAT").val('');
        $("#EXEM_DATE").val(''); 
        $("#WHT").val(''); 



        empr_partyTypes.InitCategoryDDL();
        empr_partyTypes.InitRegionDDL();
        empr_partyTypes.InitEntityDDL();
        debugger;
        empr_partyTypes.InitTaxPayerDDL();

        empr_partyTypes.InitTaxCatDDL();
        empr_partyTypes.InitSalesManDDL();
        empr_partyTypes.InitChartOfAccountDDL();
        //if ($('#chartofaccount').dxDropDownBox('instance') != undefined) {
        //    $('#chartofaccount').dxDropDownBox('instance').option("disable", false)
        //}
        $('#taxpayer').dxSelectBox('instance').option("value", "2");
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('.saveAttempt').show();
            } else {
                $('.saveAttempt').hide();
            }
        }
        empr_partyTypes.salesman_Acode = 0;
    },
    uploadFile: function () {

        var input = document.getElementById('DocumentFile');
        var files = input.files;
        var formData = new FormData();

        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax(
            {
                url: "/PartyTypes/UploadImage",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#PartytypesDocs_hidden").val(data.data);
                    }
                }
            }
        );

        },
    uploadCNIC: function () {

        var input = document.getElementById('CnicFile');
        var files = input.files;
        var formData = new FormData();

        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax(
            {
                url: "/PartyTypes/UploadImage",
                //url: "/User/uploadfiles",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {

                    if (data.msgType == '1') {

                        $("#PartytypesCNIC_hidden").val(data.data);

                    }

                }
            }
        );

    },
    InitCategoryDDL: function (_selectedValue) {
   
        $.ajax({
            url: 'Category/GetCategory',
            method: 'GET',
            data: null,
            success: function (data) {
                var response = data.data;
                console.log(response, "cat data a");
                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#category_hidden").val(_selectedValue);
                        $("#displayExpr_category").val(selectedobj[0].value);
                    }  
                }

   
                $('#CATDDLDX').dxSelectBox({
                    dataSource: response,
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
                
                //empr_partyTypes.bindDxGridBoxDdl('#CATDDLDX', data.data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_category', function (selectedvalue, hidden) {

                //    if (selectedvalue.selectedRowsData.length > 0) {

                //        var value = selectedvalue.selectedRowsData[0]['key'];
                //        var branch = selectedvalue.selectedRowsData[0]['value'];
                //        $('#category_hidden').val(value);
                //        $('#displayExpr_category').val(branch);

                //    }
                //    else {

                //        $('#category_hidden').val('');
                //        $('#displayExpr_category').val('');

                //    }

                //});
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
        

    },
    InitRegionDDL: function (_selectedValue) {

        $.ajax({
            url: 'Region/GetRegionsDropDown',
            method: 'GET',
            data: null,
            success: function (data) {
                var response = data.data;
                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#REGION_hidden").val(selectedvalue);
                        $("#displayExpr_REGION").val(selectedobj[0].value); 
                    }                    
                }

                $('#REGION').dxSelectBox({
                    dataSource: response,
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
                //empr_partyTypes.bindDxGridBoxDdl('#REGION', data.data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_REGION', function (selectedvalue, hidden) {

                //    if (selectedvalue.selectedRowsData.length > 0) {

                //        var value = selectedvalue.selectedRowsData[0]['key'];
                //        var branch = selectedvalue.selectedRowsData[0]['value'];
                //        $('#REGION_hidden').val(value);
                //        $('#displayExpr_REGION').val(branch);

                //    }
                //    else {

                //        $('#REGION_hidden').val('');
                //        $('#displayExpr_REGION').val('');

                //    }

                //});

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });

    },
    InitEntityDDL: function (_selectedValue) {

        $.ajax({
            url: 'Entity/GetEntity',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue !=null) {
                    selectedobj = data.data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#entity_hidden").val(_selectedValue);
                        $("#displayExpr_entity").val(selectedobj[0].value); 
                    }
                }
           
                empr_partyTypes.bindDxGridBoxDdl('#ENTITY', data.data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_entity', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {

                        var value = selectedvalue.selectedRowsData[0]['key'];
                        var branch = selectedvalue.selectedRowsData[0]['value'];
                        $('#entity_hidden').val(value);
                        $('#displayExpr_entity').val(branch);

                    }
                    else {
                        $('#entity_hidden').val('');
                        $('#displayExpr_entity').val('');
                    }

                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });

    },
    InitTaxPayerDDL: function () {

        var  dataSource= [
            { value: '1', key: 'Other' },
            { value: '2', key: 'Filer' },
            { value: '3', key: 'Non-Filer' }
        ];

        $('#taxpayer').dxSelectBox({
            dataSource: dataSource,
                    displayExpr: 'key',
                    valueExpr: 'value',
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
    InitTaxCatDDL: function () {

        var dataSource = [
            { value: '1', key: 'Exempted' },
            { value: '2', key: 'Taxable' },
            { value: '3', key: 'Undertaking' }
        ];

        $('#T_CAT').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'key',
            valueExpr: 'key',
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
                if (e.value === 'Exempted') {
                    $('#EXEM_DIV').show();
                } else {
                    $('#EXEM_DIV').hide();


                }

            }
        });
    },
    InitSalesManDDL: function (_selectedValue) {
        debugger;
        $.ajax({
            url: 'SalesMan/GetAllSalesMan',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#salesman_hidden").val(_selectedValue);
                        $("#displayExpr_salesman").val(selectedobj[0].value);
                    } 
                }

                empr_partyTypes.bindDxGridBoxDdl('#salesman', data.data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_salesman', function (selectedvalue, hidden) {

                    if (selectedvalue.selectedRowsData.length > 0) {

                        var value = selectedvalue.selectedRowsData[0]['key'];
                        var selectedobjs = data.data.filter(x => x.key == value);
                        if (selectedobjs.length > 0) {
                            empr_partyTypes.salesman_Acode = selectedobjs[0].acode;
                        }
                        var branch = selectedvalue.selectedRowsData[0]['value'];
                        $('#salesman_hidden').val(value);
                        $('#displayExpr_salesman').val(branch);

                    }
                    else {
                        $('#salesman_hidden').val('');
                        $('#displayExpr_salesman').val('');
                    }

                });

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });

        


    },
    InitChartOfAccountDDL: function (_selectedValue) {
        debugger;
        $.ajax({
            url: 'PartyTypes/GetChartOfAccounts',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#chartofaccount_hidden").val(_selectedValue);
                        $("#displayExpr_chartofaccount").val(selectedobj[0].value);                    
                    }
                }

                empr_partyTypes.bindDxGridBoxDdl('#chartofaccount', data.data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_chartofaccount', function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var value = selectedvalue.selectedRowsData[0]['key'];
                        var branch = selectedvalue.selectedRowsData[0]['value'];
                        $('#chartofaccount_hidden').val(value);
                        $('#displayExpr_chartofaccount').val(branch);
                    }
                    else {
                        $('#chartofaccount_hidden').val('');
                        $('#displayExpr_chartofaccount').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
}