var empr_LotRegistration = {
    lotNo: 0,
    initEvents: function () {
        $(document).ready(function () {
            console.log('PartyType', PartyType);
            empr_LotRegistration.InitPartyType();
            empr_LotRegistration.InitQuickSearch();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_LotRegistration.validateForm()) {
                            empr_LotRegistration.saveAttempt();
                        }
                    }
                } else {
                    if (empr_LotRegistration.validateForm()) {
                        empr_LotRegistration.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_LotRegistration.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_LotRegistration.GetLotRegistrationByID(reportid);
            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var date = $(this).attr("reportdate");
                console.log(date);
                var lotNo = $(this).attr("reportlotno");
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
                    $('#updatedDated').val(date);
                    $('#updatedLotNo').val(lotNo);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModal_LR').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDated').val(), item_NAME: $('#updatedLotNo').val() }, "/LotRegistration/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_LotRegistration.GetLotRegistrationByID(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_LotRegistration.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_LotRegistration.DeleteRecord();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/LotRegistration/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_LotRegistration.resetForm();
                    empr_LotRegistration.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        empr_LotRegistration.InitPartyType();
        $('#PARTY_CODE').dxSelectBox('instance').option('value', '');
        $("#LOT_NO").val(empr_LotRegistration.lotNo);
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
    },
    validateForm: function () {

        var valid = true;
        var LOT_NO = $("#LOT_NO").val().trim();
        var PARTY_CODE = $("#partyhidden").val();

        if (LOT_NO == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        if (PARTY_CODE == '') {
            empr_helper.notify("Please select Party Type.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    GetDataToSave: function () {
        var ID = $("#Code").val();
        var LOT_NO = $("#LOT_NO").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');        
        var V_DATE = $("#V_DATE").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var modelRecord = {
            CODE: ID,
            LOT_NO: LOT_NO,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_LotRegistration.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/LotRegistration/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_LotRegistration.resetForm();
                empr_LotRegistration.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_LotRegistration.GetAllLotRegistrations();
    },
    GetAllLotRegistrations: function () {
        ajaxHelper.ajaxGetJson('/LotRegistration/QuickSearch', function (data) {
            empr_LotRegistration.CreateGrid(data.data);
            var allLots = data.data;
            if (allLots.length > 0) {
                if (!isNaN(parseInt(allLots[0].loT_NO))) {
                    empr_LotRegistration.lotNo = parseInt(allLots[0].loT_NO) + 1;
                } else {
                    empr_LotRegistration.lotNo = allLots[0].loT_NO;
                }
            } else {
                empr_LotRegistration.lotNo = 0;
            }
            $("#LOT_NO").val(empr_LotRegistration.lotNo);
        }, false, true);
    },
    GetLotRegistrationByID: function (id) {
        ajaxHelper.ajaxGetJson('/LotRegistration/GetLotRegistrationByID?id=' + id, function (data) {
            empr_LotRegistration.resetForm();
            if (data.msgType == 1) {

                var record = data.data;
                var filteredData = $.grep(PartyType, function (item) {
                    return item.partyCode === record.partY_CODE && item.accountCode === record.acT_CODE.toString();
                });
                $("#Code").val(record.code);
                $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#LOT_NO").val(record.loT_NO);
                $('#V_DATE').val(record.v_DATE);

                $('.modal').modal('hide');
                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
                    }
                    if (Permissions.r_ADD) {
                        $('#BtnNew').show();
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
                    $('#BtnNew').show();
                }
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc);
        var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.gpic != null && options.data.gpic != '' && options.data.gpic != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.gpic}')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += `<a href="javascript:;" class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>`;
                    html += `<a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportlotno=${options.data.loT_NO} reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>`;
                    html += '</div>';                
                    $(html).appendTo(container);
                }
        },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'loT_NO', caption: 'Lot' },
            { dataField: 'partY_NAME', caption: 'Party' },
            { dataField: 'astatus', caption: 'Active' },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false },
            { dataField: 'adD_DATE', caption: 'Created Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Created PostalCode', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false },
            { dataField: 'ediT_DATE', caption: 'Updated Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'ediT_POSTALCODE', caption: 'Updated PostalCode', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "LotRegistration");
    },
    InitPartyType: function () {
        empr_LotRegistration.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
            if (d.value == null || d.value == '') {
                $('#partyhidden').val('');
                $('#acthidden').val('');
            }
            else {
                var filteredData = $.grep(PartyType, function (item) {
                    return item.key === d.value;
                });
                $('#partyhidden').val(filteredData[0].partyCode)
                $('#acthidden').val(filteredData[0].accountCode)
            }

        });
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },
}