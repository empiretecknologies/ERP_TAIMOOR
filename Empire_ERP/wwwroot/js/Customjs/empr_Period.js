var empr_Period = {
    initEvents: function () {

        $(document).ready(function () {

            empr_Period.InitQuickSearch();
            empr_Period.InitBranch();

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Period.validateForm()) {
                            empr_Period.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Period.validateForm()) {
                        empr_Period.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_Period.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_Period.GetPeriodByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_Period.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Period.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Period/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Period.resetForm();
                    empr_Period.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#DESCR").val('');
        $("#START_D").val('');
        $("#START_E").val('');
        $('#flexSwitchCheckDefault').prop('checked', false);
        $("#branchhidden").val('');
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('#BCODE').dxSelectBox('instance').option('value', null);

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
        var DESCR = $("#DESCR").val().trim();
        
        if (DESCR == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        return valid;
    },
    GetDataToSave: function () {
        debugger
        var ID = $("#Code").val();
        var START_D = $("#START_D").val();
        var START_E = $("#START_E").val();
        var DESCR = $("#DESCR").val();
        var costcenter = $('#flexSwitchCheckDefault').is(':checked') ? 1 : 0;
        var BCODE = $("#BCODE").dxSelectBox('instance').option('value')
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var modelRecord = {
            PID: ID,
            START_D: START_D,
            START_E: START_E,
            DESCR: DESCR,
            BCODE: BCODE,
            CLOSING: costcenter,
            ASTATUS: ASTATUS,
        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_Period.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/Period/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Period.resetForm();
                empr_Period.InitQuickSearch();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_Period.GetAllPeriodTypes();
    },
    GetAllPeriodTypes: function () {
        ajaxHelper.ajaxGetJson('/Period/QuickSearch', function (data) {
            empr_Period.CreateGrid(data.data);
        }, false, true);
    },
    GetPeriodByID: function (id) {
        ajaxHelper.ajaxGetJson('/Period/GetPeriodById?id=' + id, function (data) {
            empr_Period.resetForm();
            if (data.msgType == 1) {
                debugger
                var record = data.data;
                console.log(record)
                $("#Code").val(record.pid);
                $('#branchhidden').val(data.data.bcode);
                $('#BCODE').dxSelectBox('instance').option("value", parseInt(data.data.bcode));
                $("#flexSwitchCheckDefault").prop("checked", data.data.closing == 0 ? false : true);
                $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
                $("#DESCR").val(record.descr);
                $("#START_D").val(record.starT_D);
                $("#START_E").val(record.starT_E);
                $('.modal').modal('hide');
                //$('#BtnDelete').show();
                //$('#BtnNew').show();

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
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.pid} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
            { dataField: 'pid', caption: 'Code' },
            { dataField: 'starT_D', caption: 'Start Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'starT_E', caption: 'End Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'descr', caption: 'Description' },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Period");
    },

    InitBranch: function () {

        empr_Period.bindDxDdl("BCODE", Branch, null, "key", "value", "Select", function (d) {

            $('#branchhidden').val(d.value)
            if (d.value == null) {
                $('#branchhidden').val('');
            }

        });

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
}