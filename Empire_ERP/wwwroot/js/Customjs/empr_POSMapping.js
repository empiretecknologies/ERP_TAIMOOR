var empr_POSMapping = {
    $uploadCrop: null,
    rawImg: null,
    initEvents: function () {
        $(document).ready(function () {
            empr_POSMapping.InitDropdowns();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                if (data.traN_ID != 0) {
                    $('#Code').val(data.traN_ID);
                    empr_POSMapping.GetPOSMappingByID(data.traN_ID);
                }
            });
            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_POSMapping.validateForm()) {
                            empr_POSMapping.saveAttempt();
                            empr_POSMapping.resetForm();
                        }
                    }
                } else {
                    if (empr_POSMapping.validateForm()) {
                        empr_POSMapping.saveAttempt();
                        empr_POSMapping.resetForm();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_POSMapping.InintQuickSearch();
            })

            $('body').on('click', '.elm_edit', function () {
                empr_POSMapping.resetForm();
                var rportid = $(this).attr("rportid")
                empr_POSMapping.GetPOSMappingByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_POSMapping.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_POSMapping.DeleteRecord();
            })

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_POSMapping.GeneratePrintReport();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/POSMapping/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    $('.btn-delete').hide();
                    $('.btn-print').hide();
                    $('#resetall').hide();
                    empr_POSMapping.resetForm();
                    $('#optmodal').modal('hide');
                }
            }, false, true);
        });
    },

    resetForm: function () {
        $("#Code").val('');
        $("#GROUP_NAME").val('');
        $('#CACCOUNT').dxSelectBox('instance').option('value', "");
        $("#CASH_TAX").val('');
        $('#BACCOUNT').dxSelectBox('instance').option('value', "");
        $("#BANK_TAX").val('');
        $("#P_PRINTER").val('');
        $("#BANK_CHARGES").val('');
        $('#BRANCH').dxSelectBox('instance').option('value', "");
        /*$('#PAY_ACTCODE').dxSelectBox('instance').option('value', "");*/
        $("#PARTY_TAX").val('');
        $("#SRB_NAME").val('');
        $("#SRB_NTN").val('');
        $("#POS_USER").val('');
        $("#POS_PASS").val('');
        $("#SRB_ID").val('');
        $("#SRB_URL").val('');
        $("#FB_LINK").val('');
        $("#INSTA_LINK").val('');
        $("#WEB_LINK").val('');
        $("#TIKTOK_LINK").val('');
        $("#YOUTUBE_LINK").val('');
        $("#WIFI_NAME").val('');
        $("#WIFI_PASSWORD").val('');
        $("#Group_IMG").val('');
        $("#Item_IMG").val('');
        $("#Table_IMG").val('');
        $("#Waiter_IMG").val('');
        $("#WHATSAPP_URL").val('');
        $("#WHATSAPP_TOKEN").val('');
        $("#WHATSAPP_MSG").val('');
        $("#WHATSAPP_ADV").val('');
        $("#WHATSAPP_RTN").val('');
        $("#WHATSAPP_CC").val('');
        $("#WHT_ADV_COM").val('');
        $("#WHT_ADV_COM").val('');
        $("#POS_PRINT_L").val('');
        $("#S_IMG").val('');
        $("#LOC_SNAME").val('');
        $("#POS_PRINT_L").val('');
        $("#POSPRINTL").val('');
        $("#SImg").val('');
        $("#PAY_COPY").val('');
        $("#PRE_COPY").val('');
        $("#DUP_COPY").val('');
        document.getElementById('advancecheck').checked = false;
        document.getElementById('cashcheck').checked = false;
        document.getElementById('bankcheck').checked = false;
        document.getElementById('partycheck').checked = false;
        document.getElementById('splitcheck').checked = false;
        document.getElementById('advancebtn').checked = false;
        document.getElementById('kotbtn').checked = false;
        document.getElementById('salesmanReq').checked = false;
        document.getElementById('srbcheck').checked = false;
        //document.getElementById('RATE').checked = false;
        document.getElementById('QR_CODE').checked = false;
        document.getElementById('P_WINDOW').checked = false;
        document.getElementById('PAY_MODAL').checked = false;
        document.getElementById('PRE_MODAL').checked = false;
        document.getElementById('DUP_MODAL').checked = false;


        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        } else {
            $('#saveAttempt').show();
        }
    },

    validateForm: function () {
        var valid = true;

        var ModalData = empr_POSMapping.getDataToSave();

        var CASHACT = $('#CACCOUNT').dxSelectBox('option', 'value');
        var BANKACT = $('#BACCOUNT').dxSelectBox('option', 'value');
        var BRANCH = $('#BRANCH').dxSelectBox('option', 'value');
        var GroupName = $("#GROUP_NAME").val();

        if (CASHACT == '' || CASHACT == null) {
            valid = false;
            empr_helper.notify("Please select Cash Account.", 2);
        }
        if (BRANCH == '' || BRANCH == null) {
            valid = false;
            empr_helper.notify("Please select Branch.", 2);
        }
        if (BANKACT == '' || BANKACT == null) {
            valid = false;
            empr_helper.notify("Please select Bank Account.", 2);
        }
        if (GroupName == '' || GroupName == null) {
            valid = false;
            empr_helper.notify("Please enter Group Name.", 2);
        }

        //if (ModalData.SRB_NAME != '' || ModalData.SRB_NTN != '' || ModalData.SRB_ID != '' || ModalData.SRB_URL != '' || ModalData.POS_USER != '' || ModalData.POS_PASS != '' || ModalData.srbcheck == 'Y') {

        //    if (ModalData.SRB_NAME == '' || ModalData.SRB_NAME == null) {
        //        valid = false;
        //        empr_helper.notify("Please enter SRB Name.", 2);
        //    }
        //    if (ModalData.SRB_NTN == '' || ModalData.SRB_NTN == null) {
        //        valid = false;
        //        empr_helper.notify("Please enter SRB NTN.", 2);
        //    }
        //    if (ModalData.SRB_ID == '' || ModalData.SRB_ID == null) {
        //        valid = false;
        //        empr_helper.notify("Please enter SRB ID.", 2);
        //    }
        //    if (!ModalData.SRB_URL || !isValidURL(ModalData.SRB_URL)) {
        //        valid = false;
        //        empr_helper.notify("Please enter a valid SRB URL.", 2);
        //    }
        //    if (ModalData.POS_USER == '' || ModalData.POS_USER == null) {
        //        valid = false;
        //        empr_helper.notify("Please enter SRB USER.", 2);
        //    }
        //    if (ModalData.POS_PASS == '' || ModalData.POS_PASS == null) {
        //        valid = false;
        //        empr_helper.notify("Please enter SRB PASS.", 2);
        //    }
        //    function isValidURL(url) {
        //        var pattern = /^(https?:\/\/)?([\w.-]+)\.([a-z]{2,6})(\/[\w.-]*)*\/?$/i;
        //        return pattern.test(url);
        //    }
        //}

        return valid;
    },

    getDataToSave: function () {
        var TRAN_ID = $("#Code").val().trim();
        var GROUP_NAME = $("#GROUP_NAME").val()
        var CASH_ACCOUNT = $('#CACCOUNT').dxSelectBox('option', 'value');
        var CASH_TAX = $("#CASH_TAX").val();
        var BANK_ACCOUNT = $('#BACCOUNT').dxSelectBox('option', 'value');
        var BANK_TAX = $("#BANK_TAX").val();
        var BANK_CHARGES = $("#BANK_CHARGES").val();
        var BRANCH = $('#BRANCH').dxSelectBox('option', 'value');
        var PAY_ACCOUNT = $('#PAY_ACTCODE').dxSelectBox('option', 'value');
        var PARTY_TAX = $("#PARTY_TAX").val();
        var SRB_NAME = $("#SRB_NAME").val();
        var SRB_NTN = $("#SRB_NTN").val();
        var POS_USER = $("#POS_USER").val();
        var POS_PASS = $("#POS_PASS").val();
        var SRB_ID = $("#SRB_ID").val();
        var SRB_URL = $("#SRB_URL").val();
        var FB_LINK = $("#FB_LINK").val();
        var INSTA_LINK = $("#INSTA_LINK").val();
        var WEB_LINK = $("#WEB_LINK").val();
        var TIKTOK_LINK = $("#TIKTOK_LINK").val();
        var YOUTUBE_LINK = $("#YOUTUBE_LINK").val();
        var WIFI_NAME = $("#WIFI_NAME").val();
        var WIFI_PASSWORD = $("#WIFI_PASSWORD").val();
        var WHATSAPP_URL = $("#WHATSAPP_URL").val();
        var WHATSAPP_TOKEN = $("#WHATSAPP_TOKEN").val();
        var WHATSAPP_MSG = $("#WHATSAPP_MSG").val();
        var WHATSAPP_CC = $("#WHATSAPP_CC").val();
        var WHATSAPP_RTN = $("#WHATSAPP_RTN").val();
        var WHATSAPP_ADV = $("#WHATSAPP_ADV").val();
        var WHT_ADV_COM = $("#WHT_ADV_COM").val();
        var WHT_PARTY_MSG = $("#WHT_PARTY_MSG").val();
        var SER_CHARGES = $("#SER_CHARGES").val();
        let url = SRB_URL.trim();
        if (url.startsWith("https//")) {
            url = url.replace("https//", "https://");
            SRB_URL = url;
        }
        var Group_IMG = $("#Group_IMG").val();
        var Item_IMG = $("#Item_IMG").val();
        var Table_IMG = $("#Table_IMG").val();
        var Waiter_IMG = $("#Waiter_IMG").val();
        var cashcheck = document.getElementById('cashcheck').checked ? "Y" : "N";
        var advancecheck = document.getElementById('advancecheck').checked ? "Y" : "N";
        var bankcheck = document.getElementById('bankcheck').checked ? "Y" : "N";
        var partycheck = document.getElementById('partycheck').checked ? "Y" : "N";
        var splitcheck = document.getElementById('splitcheck').checked ? "Y" : "N";
        var advancebtn = document.getElementById('advancebtn').checked ? "Y" : "N";
        var kotbtn = document.getElementById('kotbtn').checked ? "Y" : "N";
        var salesmanReq = document.getElementById('salesmanReq').checked ? "Y" : "N";
        var srbcheck = document.getElementById('srbcheck').checked ? "Y" : "N";
        debugger;

        //var RATE = document.getElementById('RATE').checked ? 1 : 0;
        var QR_CODE = document.getElementById('QR_CODE').checked ? 1 : 0;
        var P_WINDOW = document.getElementById('P_WINDOW').checked ? 1 : 0;
        var S_IMG = $("#S_IMG").val();
        var LOGO_IMG = $("#POS_PRINT_L").val();
        var LOC_SNAME = $("#LOC_SNAME").val();
        var P_PRINTER = $("#P_PRINTER").val();
        var PAY_COPIES = $("#PAY_COPY").val();
        var PRE_COPIES = $("#PRE_COPY").val();
        var DUP_COPIES = $("#DUP_COPY").val();

        var PAY_MODAL = document.getElementById('PAY_MODAL').checked ? 1 : 0;
        var PRE_MODAL = document.getElementById('PRE_MODAL').checked ? 1 : 0;
        var DUP_MODAL = document.getElementById('DUP_MODAL').checked ? 1 : 0;

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            GROUP_NAME: GROUP_NAME,
            CASH_ACCOUNT: CASH_ACCOUNT,
            CASH_TAX: CASH_TAX,
            BANK_ACCOUNT: BANK_ACCOUNT,
            BANK_TAX: BANK_TAX,
            BANK_CHARGES: BANK_CHARGES,
            BRANCH: BRANCH,
            PAY_ACCOUNT: PAY_ACCOUNT,
            PARTY_TAX: PARTY_TAX,
            SRB_NAME: SRB_NAME,
            SRB_NTN: SRB_NTN,
            POS_USER: POS_USER,
            POS_PASS: POS_PASS,
            SRB_ID: SRB_ID,
            SRB_URL: SRB_URL,
            Group_IMG: Group_IMG,
            Item_IMG: Item_IMG,
            Table_IMG: Table_IMG,
            Waiter_IMG: Waiter_IMG,
            cashcheck: cashcheck,
            advancecheck: advancecheck,
            bankcheck: bankcheck,
            partycheck: partycheck,
            splitcheck: splitcheck,
            advancebtn: advancebtn,
            kotbtn: kotbtn,
            salesmanReq: salesmanReq,
            srbcheck: srbcheck,
            //RATE: RATE,
            FB_LINK: FB_LINK,
            INSTA_LINK: INSTA_LINK,
            WEB_LINK: WEB_LINK,
            TIKTOK_LINK: TIKTOK_LINK,
            YOUTUBE_LINK: YOUTUBE_LINK,
            WIFI_NAME: WIFI_NAME,
            WIFI_PASSWORD: WIFI_PASSWORD,
            WHATSAPP_URL: WHATSAPP_URL,
            WHATSAPP_TOKEN: WHATSAPP_TOKEN,
            WHATSAPP_MSG: WHATSAPP_MSG,
            WHATSAPP_CC: WHATSAPP_CC,
            WHATSAPP_RTN: WHATSAPP_RTN,
            WHATSAPP_ADV: WHATSAPP_ADV,
            WHT_ADV_COM: WHT_ADV_COM,
            WHT_PARTY_MSG: WHT_PARTY_MSG,
            SER_CHARGES: SER_CHARGES,
            QR_CODE: QR_CODE,
            P_WINDOW: P_WINDOW,
            S_IMG: S_IMG,
            LOC_SNAME: LOC_SNAME,
            LOGO_IMG: LOGO_IMG,
            P_PRINTER: P_PRINTER,
            PRE_MODAL: PRE_MODAL,
            PAY_MODAL: PAY_MODAL,
            DUP_MODAL: DUP_MODAL,
            PAY_COPIES: PAY_COPIES,
            PRE_COPIES: PRE_COPIES,
            DUP_COPIES: DUP_COPIES
        };
        return modelRecord;
    },

    saveAttempt: function () {
        debugger;
        var obj = empr_POSMapping.getDataToSave();
        console.log(obj)
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/POSMapping/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
            }
        }, false, true);
    },

    InintQuickSearch: function () {
        empr_POSMapping.GetQuickSearch();
    },

    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/POSMapping/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_POSMapping.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GetAllPOSMappings: function () {
        var xhr = ajaxHelper.ajaxGetJson('/POSMapping/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {
            empr_POSMapping.CreateGrid(data.data);
        }, false, true);
    },

    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },

    GetPOSMappingByID: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/POSMapping/POSMappingByid?id=' + id, function (data) {
            debugger;
            console.log(Permissions);
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
                $('#resetall').show();
            }
            $('#Code').val(data.data[0].tranid);
            $("#GROUP_NAME").val(data.data[0].groupname);
            empr_POSMapping.InitCashAccount(data.data[0].cashact);
            /*$('#CACCOUNT').dxSelectBox('instance').option("key", );*/
            $("#CASH_TAX").val(data.data[0].cashtax);
            $('#BACCOUNT').dxSelectBox('instance').option("value", data.data[0].bankact);
            $("#BANK_TAX").val(data.data[0].banktax);
            $("#BANK_CHARGES").val(data.data[0].bankchar);
            $('#BRANCH').dxSelectBox('instance').option("value", data.data[0].branch);
            /*$('#PAY_ACTCODE').dxSelectBox('instance').option("value", data.data[0].partyact);*/
            $("#PARTY_TAX").val(data.data[0].partytax);
            $("#VEHICLE").val(data.data[0].vehicle);
            $("#SRB_NAME").val(data.data[0].srbname);
            $("#SRB_NTN").val(data.data[0].srbntn);
            $('#POS_USER').val(data.data[0].posuser);
            $('#POS_PASS').val(data.data[0].pospass);
            $('#SRB_ID').val(data.data[0].srbid);
            $('#SRB_URL').val(data.data[0].srburl);
            $('#FB_LINK').val(data.data[0].fB_LINK)
            $('#INSTA_LINK').val(data.data[0].instA_LINK)
            $('#WEB_LINK').val(data.data[0].weB_LINK)
            $('#TIKTOK_LINK').val(data.data[0].tiktoK_LINK)
            $('#YOUTUBE_LINK').val(data.data[0].youtubE_LINK)
            $('#WIFI_NAME').val(data.data[0].wifI_NAME)
            $('#WIFI_PASSWORD').val(data.data[0].wifI_PASSWORD)
            $('#WHATSAPP_URL').val(data.data[0].whatsapP_URL)
            $('#WHATSAPP_TOKEN').val(data.data[0].whatsapP_TOKEN)
            $('#WHATSAPP_MSG').val(data.data[0].whatsapP_MSG)
            $('#WHATSAPP_CC').val(data.data[0].whatsapP_CC)
            $('#WHATSAPP_ADV').val(data.data[0].whatsapP_ADV)
            $('#WHATSAPP_RTN').val(data.data[0].whatsapP_RTN)
            $('#WHT_ADV_COM').val(data.data[0].whT_ADV_COM)
            $('#WHT_PARTY_MSG').val(data.data[0].whT_PARTY_MSG)
            $('#SER_CHARGES').val(data.data[0].seR_CHARGES)
            $('#Group_IMG').val(data.data[0].groupimg);
            $('#Item_IMG').val(data.data[0].itemimg);
            $('#Waiter_IMG').val(data.data[0].waiterimg);
            $('#Table_IMG').val(data.data[0].tableimg);
            $('#P_PRINTER').val(data.data[0].pprinter);
            $('#PAY_COPY').val(data.data[0].paY_COPIES);
            $('#PRE_COPY').val(data.data[0].prE_COPIES);
            $('#DUP_COPY').val(data.data[0].duP_COPIES);
            debugger;
            $('#S_IMG').val(data.data[0].simg);
            $('#POS_PRINT_L').val(data.data[0].logO_IMG);
            $('#LOC_SNAME').val(data.data[0].loC_SNAME);

            if (data.data[0].srbstatus == 'Y')
                document.getElementById('srbcheck').checked = true;
            if (data.data[0].radvance == 'Y')
                document.getElementById('advancecheck').checked = true;
            if (data.data[0].rcash == 'Y')
                document.getElementById('cashcheck').checked = true;
            if (data.data[0].rcard == 'Y')
                document.getElementById('bankcheck').checked = true;
            if (data.data[0].rparty == 'Y')
                document.getElementById('partycheck').checked = true;
            if (data.data[0].rsplit == 'Y')
                document.getElementById('splitcheck').checked = true;
            if (data.data[0].advanceBtn == 'Y')
                document.getElementById('advancebtn').checked = true;
            if (data.data[0].kotBtn == 'Y')
                document.getElementById('kotbtn').checked = true;
            if (data.data[0].salesmanReq == 'Y')
                document.getElementById('salesmanReq').checked = true;
            //if (data.data[0].rate == 1)
            //    document.getElementById('RATE').checked = true;
            if (data.data[0].qR_CODE == 1)
                document.getElementById('QR_CODE').checked = true;
            if (data.data[0].paY_MODAL == 1)
                document.getElementById('PAY_MODAL').checked = true;
            if (data.data[0].prE_MODAL == 1)
                document.getElementById('PRE_MODAL').checked = true;
            if (data.data[0].duP_MODAL == 1)
                document.getElementById('DUP_MODAL').checked = true;
            if (data.data[0].pwindow == 1)
                document.getElementById('P_WINDOW').checked = true;


        }, false, true);
        $('.modal').modal('hide');
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
                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);
                if (Permissions != "Admin") {
                    const editAction = !Permissions.r_EDIT
                        ? ''
                        : `<a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>`;
                    const actions = `<div class="btn-group btn-group-sm">${editAction}</div>`;
                    $(actions).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);
                }

            }
        },
        { dataField: 'traN_ID', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'groupname', caption: 'Group Name' },
        { dataField: 'cashact', caption: 'Cash Account' },
        { dataField: 'cashtax', caption: 'Cash Tax' },
        { dataField: 'bankact', caption: 'Bank Account' },
        { dataField: 'banktax', caption: 'Bank Tax' },
        { dataField: 'bankchar', caption: 'Bank Charges' },
        { dataField: 'branch', caption: 'Branch' },
        { dataField: 'partytax', caption: 'Party Tax' },
        { dataField: 'partyact', caption: 'Party Account' },
        { dataField: 'srbname', caption: 'SRB Name' },
        { dataField: 'srbntn', caption: 'SRB NTN' },
        { dataField: 'posuser', caption: 'SRB User' },
        { dataField: 'pospass', caption: 'SRB Pass' },
        { dataField: 'srbid', caption: 'SRB Id' },
        { dataField: 'srbstatus', caption: 'SRB Status' },
        { dataField: 'srburl', caption: 'SRB Url' },
        {
            dataField: 'groupimg', caption: 'Group Image',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.groupimg != null && options.data.groupimg != '' && options.data.groupimg != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.groupimg}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        {
            dataField: 'simg', caption: 'Sticker Image',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.simg != null && options.data.simg != '' && options.data.simg != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.simg}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },

        {
            dataField: 'itemimg', caption: 'Item Image',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.itemimg != null && options.data.itemimg != '' && options.data.itemimg != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.itemimg}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        {
            dataField: 'waiterimg', caption: 'Waiter image',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.waiterimg != null && options.data.waiterimg != '' && options.data.waiterimg != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.waiterimg}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        {
            dataField: 'tableimg', caption: 'Table Image',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.tableimg != null && options.data.tableimg != '' && options.data.tableimg != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.tableimg}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        {
            dataField: 'logO_IMG', caption: 'Print Logo',
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.logO_IMG != null && options.data.logO_IMG != '' && options.data.logO_IMG != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.logO_IMG}')"><i class="fa fa-eye"></i></a>`;
                }
                html += '</div>';
                $(html).appendTo(container);
            }
        },



        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "POSMappings");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    InitDropdowns: function () {
        debugger;
        empr_POSMapping.InitCashAccount("");
        empr_POSMapping.InitBankAccount("");
        empr_POSMapping.InitPartyAccount("");
        empr_POSMapping.InitBranch("");

    },

    InitDropdownsWithValue: function (lot, iCode, unit, pCode) {
        debugger
        empr_POSMapping.InitUnitDDL(unit);
        empr_POSMapping.InitReportTypeDDL();
        ajaxHelper.ajaxGetJson("/POSMapping/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_POSMapping.InitItemCodeDDL(data.data, iCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/POSMapping/GetLots", function (data) {
            if (data.msgType == 1) {
                empr_POSMapping.InitLotDDL(data.data, lot);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/POSMapping/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_POSMapping.InitPartyCodeDDL(data.data, pCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitCashAccount: function (selectedValue) {
        debugger;
        $('#CACCOUNT').dxSelectBox({
            dataSource: CashAccount,
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

    InitBankAccount: function (selectedValue) {
        $('#BACCOUNT').dxSelectBox({
            dataSource: BankAccount,
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

    InitPartyAccount: function (selectedValue) {
        $('#PAY_ACTCODE').dxSelectBox({
            dataSource: PartyAccount,
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

    InitBranch: function (selectedValue) {
        $('#BRANCH').dxSelectBox({
            dataSource: Branch,
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

    InitLotDDL: function (dataSource, selectedValue) {
        $('#LOT').dxSelectBox({
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
                if (e.value != '' && e.value != null) {
                    var items = e.component._dataSource._items;
                    var item = items.filter(i => i.key == e.value);
                    if (item && item.length > 0) {
                        $('#PARTY_CODE').dxSelectBox('instance').option('value', item[0].customizedKey);
                    }
                }
            },
        });
    },

    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/POSMapping/GetReportTypes", function (data) {
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
        let TRAN_ID = $("#Code").val();
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the delivery in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/POSMapping/GetPrintReport", function (data) {
            if (data.msgType === 1) {
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html(`<center><object data="${url}" width="1100" height="600"></object></center>`);
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitUnitDDL: function (_selectedValue) {
        $.ajax({
            url: 'POSMapping/GetUnits',
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

                empr_POSMapping.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
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

    InitPartyCodeDDL: function (dataSource, selectedValue) {
        $('#PARTY_CODE').dxSelectBox({
            dataSource: dataSource,
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

    OpenGroupImage: function () {
        debugger;
        var baseUrl = "/Client/POSMapping/";
        var hdnUrl = $('#Group_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
            //const fileURL = window.location.origin + baseUrl + hdnUrl;
            //window.open(fileURL, '_blank');
        }
    },



    OpenItemImage: function () {

        var baseUrl = "/Client/ItemMaster/";
        var hdnUrl = $('#Item_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },
    UploadStickerImg: function () {
        debugger;
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('SImg').files;
        if (files.length > 0) {
            $('#S_IMG').val(files[0].name);
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
                debugger;
                if (data.msgType == '1') {
                    $("#S_IMG").val(data.data);
                    //empr_helper.notify("File uploaded successfully.", 1);
                } else {
                    $('#SImg').val("No File");
                    empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                }
                $('#saveAttempt').prop('disabled', false);
            }
        });
    },
    OpenStickerImg: function () {
        debugger;
        var baseUrl = "/Client/Docs/";
        var hdnUrl = $('#S_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },
    UploadLogoImg: function () {
        debugger;
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('POSPRINTL').files;
        if (files.length > 0) {
            $('#POS_PRINT_L').val(files[0].name);
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
                debugger;
                if (data.msgType == '1') {
                    $("#POS_PRINT_L").val(data.data);
                    //empr_helper.notify("File uploaded successfully.", 1);
                } else {
                    $('#POSPRINTL').val("No File");
                    empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                }
                $('#saveAttempt').prop('disabled', false);
            }
        });
    },
    OpenLogoImg: function () {
        debugger;
        var baseUrl = "/Client/Docs/";
        var hdnUrl = $('#POS_PRINT_L').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },

    //OpenLogoImage: function () {
    //    debugger;
    //    var baseUrl = "/Client/ItemMaster/";
    //    var hdnUrl = $('#POS_PRINT_L').val();
    //    if (hdnUrl == "" || hdnUrl == null) {
    //        empr_helper.notify("Please upload a file to view.", 2);
    //    }
    //    else {
    //        ShowImage(hdnUrl);
    //    }
    //},
    OpenWaiterImage: function () {

        var baseUrl = "/Client/Company/";
        var hdnUrl = $('#Waiter_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },

    OpenTableImage: function () {
        var baseUrl = "/Client/Company/";
        var hdnUrl = $('#Table_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },
    SaveImage(imageName) {
        debugger;
        $('#BtnSave').prop('disabled', true);
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob, imageName);
        formData.append('imageName', imageName);
        $.ajax({
            url: `/POSMapping/SaveImage`,
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                debugger;
                if (data.msgType == '1') {
                    if (imageName == "ItemGroup")
                        $("#Group_IMG").val(data.data);
                    if (imageName == "ItemMaster")
                        $("#Item_IMG").val(data.data);
                    if (imageName == "Waiter")
                        $("#Waiter_IMG").val(data.data);
                    if (imageName == "Table")
                        $("#Table_IMG").val(data.data);
                    if (imageName == "SImg")
                        $("#S_IMG").val(data.data);
                    if (imageName == "PosLogo")
                        $("#POS_PRINT_L").val(data.data);
                }
                else {
                    console.log(data);
                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                }
                $('#BtnSave').prop('disabled', false);
            }
        });
    },

}