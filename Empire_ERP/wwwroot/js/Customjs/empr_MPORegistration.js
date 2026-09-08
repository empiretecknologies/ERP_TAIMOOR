var empr_MPORegistration = {
    lotNo: 0,
    initEvents: function () {
        $(document).ready(function () {
            console.log('Clients', Clients);
            empr_MPORegistration.InitPartyType();
            empr_MPORegistration.InitQuickSearch();
            empr_MPORegistration.InitFabricDDL();
            empr_MPORegistration.InitGSMDDL();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_MPORegistration.validateForm()) {
                            empr_MPORegistration.saveAttempt();
                        }
                    }
                } else {
                    if (empr_MPORegistration.validateForm()) {
                        empr_MPORegistration.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_MPORegistration.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_MPORegistration.GetMPORegistrationById(reportid);
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
                    $('#updatedDatedMPOR').val(date);
                    $('#updatedclientPO').val(lotNo);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModal_MPOR').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDatedMPOR').val(), clienT_PO: $('#updatedclientPO').val() }, "/MPORegistration/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_MPORegistration.GetMPORegistrationById(data.data);
                        empr_MPORegistration.InitQuickSearch();
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_MPORegistration.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_MPORegistration.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/MPORegistration/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_MPORegistration.resetForm();
                    empr_MPORegistration.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        empr_MPORegistration.InitPartyType();
        $('#PARTY_CODE').dxSelectBox('instance').option('value', '');
        $('#FABRIC').dxSelectBox('instance').option('value', '');
        $('#GSM').dxSelectBox('instance').option('value', '');
        $("#CLIENT_PO").val('');
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
        //var LOT_NO = $("#LOT_NO").val().trim();
        var PARTY_CODE = $("#partyhidden").val();

        //if (LOT_NO == '') {
        //    valid = false;
        //    empr_helper.notify("Please enter name.", 2);
        //}

        if (PARTY_CODE == '') {
            empr_helper.notify("Please select Party Type.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    GetDataToSave: function () {
        var ID = $("#Code").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');        
        var V_DATE = $("#V_DATE").val();
        var CLIENT_PO = $("#CLIENT_PO").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var FABRIC = $("#FABRIC").dxSelectBox('instance').option('value');        
        var GSM = $("#GSM").dxSelectBox('instance').option('value');        
        var modelRecord = {
            CODE: ID,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            CLIENT_PO: CLIENT_PO,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            FABRIC: FABRIC,
            GSM: GSM,
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_MPORegistration.GetDataToSave();
        debugger;
        ajaxHelper.ajaxPostJsonData(obj, "/MPORegistration/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_MPORegistration.resetForm();
                empr_MPORegistration.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_MPORegistration.GetAllLotRegistrations();
    },
    GetAllLotRegistrations: function () {
        ajaxHelper.ajaxGetJson('/MPORegistration/QuickSearch', function (data) {
            empr_MPORegistration.CreateGrid(data.data);
            //var allLots = data.data;
            //if (allLots.length > 0) {
            //    if (!isNaN(parseInt(allLots[0].loT_NO))) {
            //        empr_MPORegistration.lotNo = parseInt(allLots[0].loT_NO) + 1;
            //    } else {
            //        empr_MPORegistration.lotNo = allLots[0].loT_NO;
            //    }
            //} else {
            //    empr_MPORegistration.lotNo = 0;
            //}
            //$("#LOT_NO").val(empr_MPORegistration.lotNo);
        }, false, true);
    },
    GetMPORegistrationById: function (id) {
        ajaxHelper.ajaxGetJson('/MPORegistration/GetMPORegistrationById?id=' + id, function (data) {
            empr_MPORegistration.resetForm();
            if (data.msgType == 1) {
                debugger;
                var record = data.data;
                var filteredData = $.grep(Clients, function (item) {
                    return item.key === record.partY_CODE && item.accountCode === record.acT_CODE;
                });
                $("#Code").val(record.code);
                $('#FABRIC').dxSelectBox('instance').option('value', record.fabric);
                $('#GSM').dxSelectBox('instance').option('value', record.gsm);
                $('#V_DATE').val(record.v_DATE);
                $("#CLIENT_PO").val(record.clienT_PO);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);

                $('#PARTY_CODE').dxSelectBox('instance').option('value', filteredData[0].key);

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
                    html += `<a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportlotno=${options.data.clienT_PO} reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>`;
                    html += '</div>';                
                    $(html).appendTo(container);
                }
        },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'astatus', caption: 'Active' },
            { dataField: 'clienT_PO', caption: 'Client PO#' },
            { dataField: 'partY_NAME', caption: 'Party' },
            { dataField: 'fabric', caption: 'Fabric' },
            { dataField: 'gsm', caption: 'GSM' },
            //{ dataField: 'adD_USER_ID', caption: 'Created By', visible: false },
            //{ dataField: 'adD_DATE', caption: 'Created Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            //{ dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false },
            //{ dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false },
            //{ dataField: 'adD_POSTALCODE', caption: 'Created PostalCode', visible: false },
            //{ dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false },
            //{ dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false },
            //{ dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false },
            //{ dataField: 'ediT_DATE', caption: 'Updated Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            //{ dataField: 'ediT_POSTALCODE', caption: 'Updated PostalCode', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "MPORegistration");
    },
    InitPartyType: function () {
        empr_MPORegistration.bindDxDdl("PARTY_CODE", Clients, null, "key", "value", "Select", function (d) {
            if (d.value == null || d.value == '') {
                $('#partyhidden').val('');
                $('#acthidden').val('');
            }
            else {
                var filteredData = $.grep(Clients, function (item) {
                    return item.key === d.value;
                });
                $('#partyhidden').val(filteredData[0].key)
                $('#acthidden').val(filteredData[0].accountCode)
            }

        });
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },

    InitFabricDDL: function (selectedValue) {

        $('#FABRIC').dxSelectBox({
            dataSource: Fabric,
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

    InitGSMDDL: function (selectedValue) {

        $('#GSM').dxSelectBox({
            dataSource: GSMData,
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
}