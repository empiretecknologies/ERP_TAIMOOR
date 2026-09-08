var empr_Hawla = {
    editedRows: [],
    PickId: 0,
    pageType: 0,
    rbcode: 0,
    rperiod_id: 0,
    TJV_TRANID: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_Hawla.InitBranchTo(null, null);
            empr_Hawla.InitCreditDDL();
            empr_Hawla.InitDebitDDL();
            empr_Hawla.InitGrid();
        });

        $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr("href");
            if (target === '#pills-warninghome') {
                $('#tab1Wrapper').show();
                $('#tab2Wrapper').hide();
            }
            if (target === '#pills-warningcontact') {
                $('#tab1Wrapper').hide();
                $('#tab2Wrapper').show();
            }
        });


        $('body').on('click', '#BtnSave', function () {
            empr_Hawla.InitGrid();
            //empr_Hawla.Save();
        }); 

        $('body').on('click', '#ChangeModalCloseBtn', function () {
            empr_Hawla.InitDebitDDL();
            empr_Hawla.InitCreditDDL();
            $("#REMARKS").val('');
        }); 

        $('body').on('click', '#elm_print', function () {

            var row = $(this).closest('tr');
            var amount = row.find('td').eq(6).text().trim();

            var Brnach = $("#TBCODE").dxSelectBox('option', 'value');

            ajaxHelper.ajaxGetJson(`/Hawla/PrintModal?amount=${amount}&&branch=${Brnach}`, function (data) {
                if (data.msgType == 1) {

                    $('#PrintModals').html(data.slipHtml);

                    $('#PrintModal').modal('show');
                    //empr_Hawla.CreateGrid(data.data);
                } else {
                    //empr_helper.notify(data.msg, data.msgType);
                }
            }, false, true);

        });

        $('body').on('click', '#updateBtn', function () {
            var $modal = $('#ChangeModal');

            $modal.find('.modal-dialog').addClass('fade-out');

            setTimeout(function () {
                $modal.modal('hide');
                $modal.find('.modal-dialog').removeClass('fade-out'); 
            }, 400); 

            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            setTimeout(function () {
                if (empr_Hawla.ValidateInfo()) {
                    empr_Hawla.Save();

                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 500);
                }
            }, 200);
            
        });

        $('#Screenshot').click(function () {
            html2canvas(document.querySelector('#PrintModals')).then(function (canvas) {
                // Convert to Base64 image
                var imageData = canvas.toDataURL("image/png");

                // Create temporary download link
                var link = document.createElement('a');
                link.href = imageData;
                link.download = 'screenshot.png';

                // Trigger download
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            });
        });

        if (Permissions != "Admin") {
            (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            !Permissions.r_VIEW && $('#gridContainer').hide();
        }
    },
    InitGrid: function () {
        var Brnach = $("#TBCODE").dxSelectBox('option', 'value');
        if (Brnach != '' || Brnach != null) {
            empr_Hawla.GetHawlas(Brnach);
        }
        else {
            empr_Hawla.CreateGrid("");
        }

    },
    GetHawlas: function (Branch) {
        ajaxHelper.ajaxGetJson(`/Hawla/GetHawlas?Branch=${Branch}`, function (data) {
            if (data.msgType == 1) {
                console.log('GetHawlas',data);
                empr_Hawla.CreateGrid(data.data);
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGrid: function (dataSrc) {
        console.log('CreateGrid', dataSrc[0].withoutPickId);
        console.log('CreateGrid with pick id', dataSrc[0].withPickId);


        var col = [
            {
                caption: 'Change',
                dataField: 'change',

                width: 90,
                alignment: 'center',
                cellTemplate: function (container, options) {
                    const tranId = options.data.tranId;
                    const pageType = options.data.pagE_TYPE;
                    const pickId = options.data.pickId;
                    
                    var iconColor = (pickId != null && pickId != 0) ? '#7CFC00' : '#055a87';

                    $("<div style='text-align:center; width:100%;'>")
                        .append(`<i class='fa-solid fa-arrows-rotate' style='cursor:pointer; color: ${iconColor}; font-size:18px;' id='rotateIcon'></i>`)
                        .appendTo(container)
                        .find('i')
                        .on('click', function () {
                            //debugger;
                            empr_Hawla.TJV_TRANID = options.data.tjV_TRANID;
                            empr_Hawla.rbcode = options.data.bcode;
                            empr_Hawla.rperiod_id = options.data.perioD_ID;

                            const $icon = $(this);
                            $icon.addClass('spin');

                            const $modal = $('#ChangeModal');
                            $modal.find('.modalRecord').hide(); 
                            $modal.find('.modal-body').prepend(`
                            <div class="modal-loader d-flex justify-content-center py-4">
                                <div class="spinner-border" style="color: #055a87;" role="status">
                                    <span class="visually-hidden">Loading...</span>
                                </div>
                            </div>
                        `);

                            // Show modal
                            //debugger;
                            if (options.data.tjV_TRANID > 0) {
                                empr_Hawla.GetTJVRecord(options.data.tjV_TRANID);
                            }
                            $modal.modal('show');

                            setTimeout(function () {
                                $modal.find('.modal-loader').remove();
                                $modal.find('.modalRecord').fadeIn();
                            }, 1500); 

                            empr_Hawla.PickId = tranId;
                            empr_Hawla.pageType = pageType;
                        });
                }
            },
            {
                caption: 'Receive',
                dataField: 'select',
                width: 130, // slightly wider to fit both
                alignment: 'center',
                cellTemplate: function (container, options) {
                    var isChecked = options.data.pickId != null && options.data.pickId != 0;

                    // Container with flex layout
                    var $wrapper = $("<div style='display: flex; align-items: center; justify-content: center; gap: 10px;'></div>");

                    // Checkbox
                    var $checkbox = $("<input type='checkbox' class='form-check-input' style='transform: scale(1.5);'>")
                        .prop('checked', isChecked)
                        .prop('disabled', true);

                    // Print icon (Font Awesome)
                    var $printIcon = $("<i class='fa fa-print' id='elm_print' style='font-size: 13px; color: #055a87;'></i>");

                    // Append both
                    $wrapper.append($checkbox).append($printIcon).appendTo(container);
                }
            },
            { dataField: 'voucherDate', caption: 'Date', width: 100 },
            { dataField: 'voucherNo', caption: 'Transaction #', width: 120 },
            { dataField: 'bookType', caption: 'Debit Account', width: 200 },
            { dataField: 'actName', caption: 'Credit Account', width: 200 },
            { dataField: 'amount', caption: 'Amount', width: 80 },
            { dataField: 'descr', caption: 'Description' },
            { dataField: 'pickId', caption: 'Pick Id', visible: false },
            { dataField: 'tranId', caption: 'TRAN ID', visible: false },
            { dataField: 'bcode', caption: 'BCODE', visible: false },
            { dataField: 'perioD_ID', caption: 'PERIOD_ID', visible: false },
            { dataField: 'pagE_TYPE', caption: 'pagE_TYPE', visible: false }
        ];

        ;
        empr_helper.DxGridBindingForReportsWithSetting_withoutGroup('#gridContainer', col, dataSrc[0].withoutPickId, empr_helper.reportName);
        empr_helper.DxGridBindingForReportsWithSetting_withoutGroup('#ToBranchgridContainer', col, dataSrc[0].withPickId, empr_helper.reportName);
    },
    InitBranchTo: function (selected1, selected2) {
        debugger;
        empr_Hawla.bindDxDdl("TBCODE", BranchTo, null, "key", "value", "Select", function (d) {
            //$('#branchtohidden').val(d.value)
            //if (d.value == null) {
            //    $('#branchtohidden').val('');
            //}

        });
        //empr_Hawla.bindDxDdl("bookType1", BookTypes, selected1, "key", "value", "Select", function (d) {

        //});
        //empr_Hawla.bindDxDdl("bookType2", BookTypes, selected2, "key", "value", "Select", function (d) {

        //});

    },
    GetEditedRows: function () {
        var originalArray = empr_Hawla.editedRows.filter(function (row) {
            return row !== undefined;
        });

        var keyMap = {
            "acT_CODE": "ACT_CODE",
            "acT_GR_CODE": "ACT_GR_CODE",
            "acT_NAME": "ACT_NAME",
            "astatus": "ASTATUS",
            "controL_NAME": "CONTROL_NAME",
            "credit": "CREDIT",
            "debit": "DEBIT",
            "oP_ID": "OP_ID"
        };

        //return empr_Hawla.RenameKeys(originalArray, keyMap);

        const newArray = originalArray.map(({
            acT_CODE: ACT_CODE,
            acT_GR_CODE: ACT_GR_CODE,
            acT_NAME: ACT_NAME,
            astatus: ASTATUS,
            controL_NAME: CONTROL_NAME,
            credit: CREDIT,
            debit: DEBIT,
            oP_ID: OP_ID
        }) => ({
            ACT_CODE,
            ACT_GR_CODE,
            ACT_NAME,
            ASTATUS,
            CONTROL_NAME,
            CREDIT,
            DEBIT,
            OP_ID
        }));

        return newArray;
    },
    RenameKeys: function (originalArray, keyMap) {
        //var renamedArray = {};
        //for (var key in originalArray) {
        //    if (keyMap.hasOwnProperty(key)) {
        //        renamedArray[keyMap[key]] = originalArray[key];
        //    } else {
        //        renamedArray[key] = originalArray[key];
        //    }
        //}
        //return renamedArray;

        var renamedArray = {};
        $.each(originalArray, function (key, value) {
            var newKey = keyMap[key] || key; // Use the mapped key if exists, otherwise keep the original key
            renamedArray[newKey] = value;
        });
        return renamedArray;
    },
    ValidateInfo: function () {

        var valid = true;
        var CreditAcc = $("#bookType2").dxSelectBox('option', 'value'); 
        var DebitAcc = $("#bookType1").dxSelectBox('option', 'value'); 

        if (CreditAcc == "" || CreditAcc == null || CreditAcc == undefined) {
            empr_helper.notify("Please select Credit Account.", 2);
            valid = false;
        }
        if (DebitAcc == "" || DebitAcc == null || DebitAcc == undefined) {
            empr_helper.notify("Please select Debit Account.", 2);
            valid = false;
        }

        $("#Loader").hide();
        return valid;
    },

    GetData: function () {
        var Branch = $("#TBCODE").dxSelectBox('option', 'value');
        var CreditAcc = $("#bookType2").dxSelectBox('option', 'value');
        var DebitAcc = $("#bookType1").dxSelectBox('option', 'value');
        var REMARKS = $("#REMARKS").val();

        var modelRecord = {
            Branch: Branch,
            CreditAccount: CreditAcc,
            debitAccount: DebitAcc,
            PickId: empr_Hawla.PickId,
            PAGE_TYPE: empr_Hawla.pageType,
            TJV_TRANID: empr_Hawla.TJV_TRANID,
            RBCODE: empr_Hawla.rbcode,
            RPERIOD_ID: empr_Hawla.rperiod_id,
            REMARKS: REMARKS,
        }

        return modelRecord;
    },

    Save: function () {
        var data = empr_Hawla.GetData();
        console.log('Save',data);
        ajaxHelper.ajaxPostJsonData(data, "/Hawla/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Hawla.InitGrid();
                $("#bookType2").dxSelectBox('option', 'value', null);
                $("#bookType1").dxSelectBox('option', 'value', null);
                $('#REMARKS').val('');
            }
        }, false, true);
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    InitDebitDDL: function (selectedValue) {

        $('#bookType1').dxSelectBox({
            dataSource: BookTypes,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue,
            searchEnabled: true,
            placeholder: 'Select',
            showClearButton: true,
            pagingEnabled: true,
            searchTimeout: 500,
        });
    },

    InitCreditDDL: function (selectedValue) {

        $('#bookType2').dxSelectBox({
            dataSource: BookTypes,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue,
            searchEnabled: true,
            placeholder: 'Select',
            showClearButton: true,
            pagingEnabled: true,
            searchTimeout: 500,
        });
    },

    GetTJVRecord: function (code) {
        ajaxHelper.ajaxGetJson('/Hawla/GetTJVRecord?code=' + code, function (data) {
            //debugger;
            if (data.data.msgType == 1) {
                var data = data.data.data[0];
                console.log('data', data);
                //empr_Hawla.InitBranchTo(data.debiT_AC, data.crediT_AC);
                empr_Hawla.InitDebitDDL(data.debiT_AC);
                empr_Hawla.InitCreditDDL(data.crediT_AC);
                $("#REMARKS").val(data.ddesc);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
}