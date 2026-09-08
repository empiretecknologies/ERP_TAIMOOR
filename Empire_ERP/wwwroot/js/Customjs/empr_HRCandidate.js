var empr_HRCandidate = {
    initEvents: function () {
        $(document).ready(function () {
            $('#resetall').show();
            empr_HRCandidate.InitGenderDDL();
            empr_HRCandidate.InitJobDDL();
            empr_HRCandidate.InitEducationDDL();
            empr_HRCandidate.resetForm();

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_HRCandidate.validateForm()) {
                            empr_HRCandidate.saveAttempt();
                        }
                    }
                } else {
                    if (empr_HRCandidate.validateForm()) {
                        empr_HRCandidate.saveAttempt();
                    }
                }
            })


            $('body').on('click', '#quicksearch', function () {
                empr_HRCandidate.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_HRCandidate.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                console.log('rportid', rportid);
                empr_HRCandidate.GetHRJobPostByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').show();
                empr_HRCandidate.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_HRCandidate.DeleteRecord();
            })


            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_HRCandidate.GeneratePrintReport();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/HRJobPost/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_HRCandidate.resetForm();
                    empr_HRCandidate.InitTree();
                    empr_HRCandidate.reFreshTree();
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('.btn-print').hide();
                    $('#resetall').hide();
                }
            }, false, true);

        });

    },
    validateForm: function () {
        var valid = true;

        // Fields
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var V_DATE = $("#V_DATE").val();
        var APPLY_DATE = $("#APPLY_DATE").val();
        var JOB = $('#JOB').dxSelectBox('instance').option('value');
        var NAME = $("#NAME").val();
        var EMAIL = $("#EMAIL").val();
        var PHONE = $("#PHONE").val();
        var GENDER = $('#GENDER').dxSelectBox('instance').option('value');
        var DOB = $("#DOB").val();
        var EXP_YEAR = $("#EXP_YEAR").val();
        var EDUCATION = $('#EDUCATION').dxSelectBox('instance').option('value');
        var ADDRESS = $("#ADDRESS").val();
        var REMARKS = $("#REMARKS").val();
        var DOC = $("#hdnDOC").val();

        function checkRequired(value, fieldName) {
            if (!value || value === "" || value === null) {
                empr_helper.notify("Please enter/select " + fieldName + ".", 2);
                valid = false;
            }
        }

        function checkEmail(value) {
            // strict regex: only valid email formats allowed
            var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            if (value && !emailPattern.test(value)) {
                empr_helper.notify("Please enter a valid Email address.", 2);
                valid = false;
            }
        }


        // Required validations
        checkRequired(ASTATUS, "Status");
        checkRequired(V_DATE, "Date");
        checkRequired(APPLY_DATE, "Apply Date");
        checkRequired(JOB, "Job");
        checkRequired(NAME, "Name");
        checkRequired(EMAIL, "Email");
        checkRequired(PHONE, "Phone No");
        checkRequired(GENDER, "Gender");
        checkRequired(DOB, "DOB");
        checkRequired(EXP_YEAR, "Experience Years");
        checkRequired(EDUCATION, "Education");
        checkRequired(ADDRESS, "Address");
        checkRequired(REMARKS, "Remarks");
        checkRequired(DOC, "Document");

        checkEmail(EMAIL);

        return valid;
    },

    getDataToSave: function () {
        var TRAN_ID = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var V_DATE = $("#V_DATE").val();
        var APPLY_DATE = $("#APPLY_DATE").val();
        var JOB = $('#JOB').dxSelectBox('instance').option('value');
        var NAME = $("#NAME").val();
        var EMAIL = $("#EMAIL").val();
        var PHONE = $("#PHONE").val();
        var GENDER = $('#GENDER').dxSelectBox('instance').option('value');
        var DOB = $("#DOB").val();
        var EXP_YEAR = $("#EXP_YEAR").val();
        var EDUCATION = $('#EDUCATION').dxSelectBox('instance').option('value');
        var ADDRESS = $("#ADDRESS").val();
        var REMARKS = $("#REMARKS").val();
        var DOC = $("#hdnDOC").val();

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            APPLY_DATE: APPLY_DATE,
            JOB: JOB,
            NAME: NAME,
            EMAIL: EMAIL,
            PHONE: PHONE,
            GENDER: GENDER,
            DOB: DOB,
            EXP_YEAR: EXP_YEAR,
            EDUCATION: EDUCATION,
            ADDRESS: ADDRESS,
            REMARKS: REMARKS,
            DOC: DOC,
        };
        return modelRecord;
    },
    saveAttempt: function () {
        var obj = empr_HRCandidate.getDataToSave();
        console.log(obj);
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/HRCandidate/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#optmodal').modal('hide');
                //$('.btn-delete').show();
                /*$('.btn-print').show();*/
                empr_HRCandidate.resetForm();
                //empr_HRCandidate.GetHRJobPostByID(data.data);
            }
        }, false, true);
    },
    resetForm: function () {
        $("#Code").val('');
        $("#JOB").val('');
        $("#APPLY_DATE").val('');
        $("#NAME").val('');
        $("#EMAIL").val('');
        $("#PHONE").val('');
        $("#DOB").val('');
        $("#EXP_YEAR").val('');
        $("#ADDRESS").val('');
        $("#REMARKS").val('');
        $("#hdnDOC").val('');

        $('#EDUCATION').dxSelectBox('instance').option('value', null);
        empr_HRCandidate.InitGenderDDL();
        empr_HRCandidate.InitJobDDL();
        empr_HRCandidate.InitEducationDDL();

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
    InintQuickSearch: function () {
        empr_HRCandidate.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/HRCandidate/QuickSearch', function (data) {
            if (data.msgType == 1) {
                console.log('quick grid data', data.data)
                empr_HRCandidate.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllHRJobPosts: function () {

        var xhr = ajaxHelper.ajaxGetJson('/HRJobPost/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_HRCandidate.CreateGrid(data.data);

        }, false, true);

    },
    GetHRJobPostByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/HRCandidate/HRJobPostByid?id=' + id, function (data) {
            console.log('editData', data.data);

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
            $('#generateCode').hide();

            $('.modal').modal('hide')

            $('#Code').val(data.data.traN_ID);
            $("#V_DATE").val(data.data.v_DATE);
            $("#NAME").val(data.data.name);
            $("#EMAIL").val(data.data.email);
            $("#PHONE").val(data.data.phone);
            $("#GENDER").val(data.data.gender);
            $("#DOB").val(data.data.dob);
            $("#APPLY_DATE").val(data.data.applY_DATE);
            $("#EXP_YEAR").val(data.data.exP_YEAR);
            $("#ADDRESS").val(data.data.address);
            $("#REMARKS").val(data.data.remarks);
            $("#hdnDOC").val(data.data.doc);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            $('#EDUCATION').dxSelectBox('instance').option("value", data.data.education);
            empr_HRCandidate.InitJobDDL(data.data.job);
            empr_HRCandidate.InitGenderDDL(data.data.gender);
            empr_HRCandidate.InitEducationDDL(data.data.education);
        }, false, true);

    },
    CreateGrid: function (dataSrc) {
        console.log('yaha', dataSrc);
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

                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);


            }
        },
        { dataField: 'v_DATE', caption: 'Date' },
        { dataField: 'applY_DATE', caption: 'Apply Date' },
        { dataField: 'astatus', caption: 'Status' },
        { dataField: 'coN_ADDRESS', caption: 'Address' },
        { dataField: 'dob', caption: 'DOB' },
        { dataField: 'education', caption: 'Education' },
        { dataField: 'email', caption: 'Email' },
        { dataField: 'exP_YEAR', caption: 'Exp Years' },
        { dataField: 'fulL_NAME', caption: 'Name' },
        { dataField: 'gender', caption: 'Gender' },
        { dataField: 'job', caption: 'Job' },
        { dataField: 'phone', caption: 'Phone' },
        { dataField: 'resumE_PATH', caption: 'DOC' },
        { dataField: 'traN_ID', caption: 'Code', visible: false },
        { dataField: 'remarks', caption: 'Remarks' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "HRJobPostQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    InitTree: function () {

        $.ajax({
            url: 'HRJobPost/GetAccountsForTreeView',
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
                        empr_HRCandidate.GetHRJobPostByID(clickedRowData.acT_CODE);
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
    InitEducationDDL: function (_selectedValue) {
        $('#EDUCATION').dxSelectBox({
            dataSource: Education,
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
            searchTimeout: 500
        });
    },
    InitGenderDDL: function (_selectedValue) {
        $('#GENDER').dxSelectBox({
            dataSource: empr_helper.gender,
            displayExpr: 'key',
            valueExpr: 'value',
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
    },
    InitJobDDL: function (_selectedValue) {
        $('#JOB').dxSelectBox({
            dataSource: Job,
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
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/HRJobPost/GetReportTypes", function (data) {
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
        empr_HRCandidate.InitReportTypeDDL();
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


        ajaxHelper.ajaxPostJsonData(dataModel, "/HRJobPost/GetPrintReport", function (data) {
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
    UploadDoc: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/Common/UploadVoucherDocs",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnDOC").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },

    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },
    validateExperience: function (input) {
        const value = input.value;
        if (!/^\d{0,2}(\.\d{0,2})?$/.test(value)) {
            input.value = value.slice(0, -1);
        }
    },

    sendEmailsSequentially: function (employees, index = 0) {
        $('#Loader').appendTo('body');
        $("#Loader").css({
            display: 'flex'
        });

        setTimeout(function () {
            if (index >= employees.length) {
                setTimeout(function () {
                    $("#Loader").hide();
                }, 1000);
                return;
            }

            var emp = employees[index];
            var emailModel = {
                To: emp.email,
                Subject: "Test Email Subject",
                Message: "This is a test mail"
            };

            ajaxHelper.ajaxPostJsonData(emailModel, "/MailBox/SendMail", function (data) {
                empr_helper.notify(data.data, data.msgType);

                if (data.msgType == 1) {
                    empr_HRCandidate.sendEmailsSequentially(employees, index + 1);
                } else {
                    empr_HRCandidate.sendEmailsSequentially(employees, index + 1);
                }
            }, false, true);
        }, 200);

    }
}