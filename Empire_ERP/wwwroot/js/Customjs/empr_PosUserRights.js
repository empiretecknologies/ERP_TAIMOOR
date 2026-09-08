var empr_PosUserRights = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    InitEvents: function () {
        $(document).ready(function () {
            empr_PosUserRights.InItUsers();
            empr_PosUserRights.InitQuickSearchGrid();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_PosUserRights.GetPosUserByCode(data.traN_ID);
                }
            });
            $('body').on('click', '#BtnQuickSearch', function () {
                empr_PosUserRights.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSave', function () {
                console.log('dataClear', dataClear);
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        debuger
                        if (empr_PosUserRights.validateForm()) {
                            empr_PosUserRights.Save();
                            //empr_PosUserRights.resetForm();
                        }
                    }
                } else {
                    if (empr_PosUserRights.validateForm()) {
                        empr_PosUserRights.Save();
                    }
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_PosUserRights.GetPosUserByCode(id);
            });

            $('body').on('click', '.elm_copy', function () {
                //debugger
                var id = $(this).attr("reportid");
                var selected_value = $(this).attr("party");
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
                    //debugger
                    //empr_PosUserRights.InitPartyDDL(selected_value, 'PartyModal')
                    empr_helper.selectedBill = id;
                    $('#CopyParty').modal('show');
                });
            });
         
            $('body').on('click', '#saveCopiedRecord', function () {
                var selectedParty = $('#PartyModal').dxSelectBox('instance').option('value');
                ajaxHelper.ajaxPostJsonData({ grouP_CODE: empr_helper.selectedBill, party: selectedParty }, "/CommMap/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_PosUserRights.GetPosUserByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_PosUserRights.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_PosUserRights.ResetForm();
                $('#PARTY_CODE').dxSelectBox('instance').option('value', null);

                
            });

            //$('body').on('click', '.btn-print,#BtnGenerateReport', function () {
            //    empr_PosUserRights.GeneratePrintReport();
            //});

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
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

    InitQuickSearchGrid: function () {
        empr_PosUserRights.GetPosUsers();
    },

    GetPosUsers: function () {
        ajaxHelper.ajaxGetJson('/POSUserRights/GetPosUsers', function (data) {
            if (data.msgType == 1) {
                empr_PosUserRights.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {

        console.log("datass",dataSrc)
        //debugger;
        var columns = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                //<a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>

                if (Permissions != "Admin" && !Permissions.r_PRINT) {
                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>
                               </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>
                               <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.party} reportid=${options.data.grouP_CODE} title="COPY"><i class="fa fa-copy"></i></a>
                               </div>`).appendTo(container);
                }
            }
        },

            { dataField: 'grouP_CODE', caption: 'Code', },
            { dataField: 'dT_CODE', caption: 'Group Name' },
            { dataField: 'grouP_NAME', caption: 'User Name' },

            //{ dataField: 'sacT_CODE', caption: 'Sact Code ', visible: false },

            { dataField: 'username', caption: 'Party Name' },

            //{ dataField: 'grouP_CODE', caption: 'Code',  },
            //{ dataField: 'salesmaN_NAME', caption: 'Salesman', },
            //{ dataField: 'salesman', caption: 'Salesman', visible: false },
   

        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "CashReceiptVoucherQS", "single");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/CashReceiptVoucher/GetCommisionMap", "dT_CODE", "CashReceiptVoucher", "multiple");
    },
    ResetForm: function () {
        $("#Code").val('');
        $("#G_NAME").val('');
        $("#SETT_T").val('');
        $("#SETT_F").val('');
        $('#USERNAME').dxSelectBox('instance').option('value', "");

        document.getElementById('RATE').checked = false;
        document.getElementById('M_DISC').checked = false;
        document.getElementById('D_DISC').checked = false;
        document.getElementById('B_RETURN').checked = false;
        document.getElementById('I_RETURN').checked = false;
        document.getElementById('S_CHARGES').checked = false;
        document.getElementById('C_DISC').checked = false;
        document.getElementById('EXPENSE').checked = false;
        document.getElementById('COMM').checked = false;
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        } else {
            $('#saveAttempt').show();
        }
        $('#BtnDelete').hide();
    },
    validateForm: function () {

        var valid = true
        var G_NAME = $("#G_NAME").val()
        var USERNAME = $('#USERNAME').dxSelectBox('option', 'value')
        var SETT_F = $("#SETT_F").val()
        var SETT_T = $("#SETT_T").val();

        if (USERNAME == '' || USERNAME == null || USERNAME == undefined)
        {
            valid = false;
            empr_helper.notify("Username is requires ",2)
        }
        if (G_NAME == '' || G_NAME == null || G_NAME == undefined) {
            valid = false;
            empr_helper.notify("Group Name is requires ", 2)
        }
        if ((SETT_F && !SETT_T) || (!SETT_F && SETT_T)) {
            empr_helper.notify("Please enter both Settlement Feilds",2);
            return false;
        }
        if (SETT_T <= SETT_F) {
            valid = false;
            empr_helper.notify("Settlement is invalid ", 2)
        }
        return valid;

    },
    getDataToSave: function () {

        var CODE = $("#Code").val().trim();
        var G_NAME = $("#G_NAME").val()
        var USERNAME = $('#USERNAME').dxSelectBox('option', 'value')
        var SETT_F = $("#SETT_F").val()
        var SETT_T = $("#SETT_T").val();

        var RATE = document.getElementById('RATE').checked ? 1 : 0;
        var M_DISC = document.getElementById('M_DISC').checked ? 1 : 0;
        var D_DISC = document.getElementById('D_DISC').checked ? 1 : 0;
        var B_RETURN = document.getElementById('B_RETURN').checked ? 1 : 0;
        var I_RETURN = document.getElementById('I_RETURN').checked ? 1 : 0;
        var S_CHARGES = document.getElementById('S_CHARGES').checked ? 1 : 0;
        var C_DISC = document.getElementById('C_DISC').checked ? 1 : 0;
        var EXPENSE = document.getElementById('EXPENSE').checked ? 1 : 0;
        var COMM = document.getElementById('COMM').checked ? 1 : 0;
        var modelRecord = {
            CODE:CODE,
            G_NAME:G_NAME,
            USERNAME:USERNAME,
            SETT_F:SETT_F,
            SETT_T:SETT_T,
            RATE:RATE,
            M_DISC:M_DISC,
            D_DISC:D_DISC,
            B_RETURN:B_RETURN,
            I_RETURN:I_RETURN,
            S_CHARGES:S_CHARGES,
            C_DISC:C_DISC,
            EXPENSE:EXPENSE,
            COMM:COMM



        }
        return modelRecord;

    },

    Save: function () {
        var obj = empr_PosUserRights.getDataToSave();
      
        ajaxHelper.ajaxPostJsonData(obj,"/POSUserRights/Save", function (data) {
            console.log('SaveInfo Responce', data);
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgError != null) {
                empr_helper.notify(data.msgError, 2);
            }
            if (data.msgType == 1) {
                debugger;
                if ($("#Code").val() == 0
                    || $("#Code").val() == null
                    || $("#Code").val() == undefined
                    || $("#Code").val() == "") {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    //empr_PosUserRights.ResetForm();
                //    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                if (dataClear == 1) {
                    empr_PosUserRights.GetPosUserByCode($('#Code').val());
                    //empr_PosUserRights.ResetForm();   
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                    } else {
                        $('#BtnDelete').show();
                    }
                }
                else {
                    empr_PosUserRights.ResetForm();          
                }


            }
        }, false, true);
    },
    InItUsers: function (selectedValue) {
        console.log("Users", Users);
        $('#USERNAME').dxSelectBox({
                    dataSource: Users,
                    displayExpr: 'name',
                    valueExpr: 'name',
                    value: selectedValue,
                    searchEnabled: true,
                    width: '100%',
                    //placeholder: "Select Chart Type",
                    showClearButton: true,
                    dropDownOptions: {
                        height: 'auto',
                    },
                    pagingEnabled: true,
                    searchTimeout: 300,
                });
            
    },
    GetPosUserByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/POSUserRights/GetPosUserByCode?code=' + code, function (data) {
            debugger;
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                console.log("new data ", masterData);
                if (masterData.length == 1) {
                    debugger;
                    var response = masterData[0];
                    $('#Code').val(response.code);
                    //$('#ACT_CODE').val(response.sacT_CODE);
                    $('#USERNAME').dxSelectBox('instance').option("value", response.username);
                    $("#G_NAME").val(response.g_NAME);
                    $("#SETT_T").val(response.setT_T);
                    $("#SETT_F").val(response.setT_F);
                    if (response.rate == 1) 
                        document.getElementById('RATE').checked = true;
                    if (response.m_DISC == 1)
                        document.getElementById('M_DISC').checked = true;
                    if (response.d_DISC == 1)
                        document.getElementById('D_DISC').checked = true;
                    if (response.b_RETURN == 1)
                        document.getElementById('B_RETURN').checked = true;
                    if (response.i_RETURN == 1)
                        document.getElementById('I_RETURN').checked = true;
                    if (response.s_CHARGES == 1)
                        document.getElementById('S_CHARGES').checked = true;
                    if (response.c_DISC == 1)
                        document.getElementById('C_DISC').checked = true;
                    if (response.expenses == 1)
                        document.getElementById('EXPENSE').checked = true;
                    if (response.comm == 1)
                        document.getElementById('COMM').checked = true;

                 
                   
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
                $("#Loader").hide();
            
            }
            else {
                empr_helper.notify("1" + data.msg, data.msgType);
                $("#Loader").hide();
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/CustomerPricing/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_PosUserRights.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

 
}