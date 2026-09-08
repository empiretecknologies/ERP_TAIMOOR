var empr_ReportType = {
    initEvents: function () {

        $(document).ready(function () {
            empr_ReportType.InitMenu();
            empr_ReportType.InitQuickSearch();
            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ReportType.validateForm()) {
                            empr_ReportType.saveAttempt();
                        }
                    }
                } else {
                    if (empr_ReportType.validateForm()) {
                        empr_ReportType.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_ReportType.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_ReportType.GetReportTypeByID(reportid);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_ReportType.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_ReportType.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/ReportType/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ReportType.resetForm();
                    empr_ReportType.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });
    },

    resetForm: function () {
        $("#Code").val('');
        $("#SNO").val('');
        $("#REPORT_NAME").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $("#menuhidden").val('');
        $('#M_ID').dxSelectBox('instance').option('value', null);
        empr_ReportType.InitQuickSearch();
        empr_ReportType.InitMenu();

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
        var SNO = $("#SNO").val().trim();
        var REPORT_NAME = $("#REPORT_NAME").val().trim();
        
        if (SNO == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        if (REPORT_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter report name.", 2);
        }

        return valid;
    },

    GetDataToSave: function () {
        var ID = $("#Code").val();
        var SNO = $("#SNO").val().trim();
        var REPORT_NAME = $("#REPORT_NAME").val().trim();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var M_ID = $("#M_ID").dxSelectBox('instance').option('value');
        var modelRecord = {
            R_ID: ID,
            SNO: SNO,
            ASTATUS: ASTATUS,
            REPORT_NAME: REPORT_NAME,
            M_ID: M_ID
        }
        return modelRecord;
    },

    saveAttempt: function () {
        var obj = empr_ReportType.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/ReportType/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_ReportType.resetForm();
                
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },

    InitQuickSearch: function () {
        empr_ReportType.GetAllReportTypes();
    },

    GetAllReportTypes: function () {
        ajaxHelper.ajaxGetJson('/ReportType/QuickSearch', function (data) {
            empr_ReportType.CreateGrid(data.data);
        }, false, true);
    },

    GetReportTypeByID: function (id) {
        ajaxHelper.ajaxGetJson('/ReportType/GetReportTypeByID?id=' + id, function (data) {
            empr_ReportType.resetForm();
            if (data.msgType == 1) {

                var record = data.data;
                $("#Code").val(record.r_ID);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#REPORT_NAME").val(record.reporT_NAME);
                $('#menuhidden').val(record.m_ID);
                $('#M_ID').dxSelectBox('instance').option("value", parseInt(record.m_ID));
                $("#SNO").val(record.sno);

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
        console.log(dataSrc)
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
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.r_ID} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
            { dataField: 'sno', caption: 'SNO' },
            { dataField: 'reporT_NAME', caption: 'Report Name' },
            { dataField: 'menU_NAME', caption: 'Menu Name' },
            { dataField: 'astatus', caption: 'Active' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ReportType");
    },

    InitMenu: function () {
        ajaxHelper.ajaxGetJson('/ReportType/MenuList', function (data) {
            empr_ReportType.bindDxDdl("M_ID", data, null, "key", "value", "Select", function (d) {
                debugger;
                $('#menuhidden').val(d.value)
                let newArray = data;
                let filteredItem = newArray.filter(m => m.key === d.value);
                if (filteredItem.length > 0) {
                    $('#SNO').val(filteredItem[0].sno)
                    if (d.value == null) {
                        $('#menuhidden').val('');
                    }
                }
            });
        }, false, true);
       
    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },
}