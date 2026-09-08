var empr_HRInterviewSchedule = {
    canMailCheck : 0,
    isValueAssigned: false,
    initEvents: function () {
        $(document).ready(function () {
            $('#resetall').show();
            empr_HRInterviewSchedule.InitJobDDL();
            empr_HRInterviewSchedule.InitCandidateDDL();
            empr_HRInterviewSchedule.InitEmpDDL();
            empr_HRInterviewSchedule.resetForm();
            empr_HRInterviewSchedule.InitMultipleEmpDDL();

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        empr_HRInterviewSchedule.SendMailOrsaveAttempt();
                    }
                } else {
                    empr_HRInterviewSchedule.SendMailOrsaveAttempt();
                }
            })


            $('body').on('click', '#quicksearch', function () {
                empr_HRInterviewSchedule.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_HRInterviewSchedule.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                console.log('rportid', rportid);
                empr_HRInterviewSchedule.GetHRJobPostByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').show();
                empr_HRInterviewSchedule.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_HRInterviewSchedule.DeleteRecord();
            })


            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_HRInterviewSchedule.GeneratePrintReport();
            });

            $(document).on('click', '.swal2-cancel', function () {
                empr_HRInterviewSchedule.saveAttempt();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }
        });

    },
    SendMailOrsaveAttempt: function () {
        if (empr_HRInterviewSchedule.validateForm()) {
            var data = empr_HRInterviewSchedule.getDataToSave();
            if (data.CAN_EMAIL == 1) {
                empr_HRInterviewSchedule.canMailCheck = 1;
                if (CandidateMails) {

                    var candidateId = $('#CANDIDATE').dxSelectBox('instance').option('value');

                    if (candidateId) {
                        var candidateMail = CandidateMails.filter(function (x) {
                            return x.key === candidateId;
                        }).map(function (x) {
                            return { email: x.value };
                        });

                        console.log("Selected Candidate:", candidateMail);

                        empr_HRInterviewSchedule.sendEmailsSequentially(candidateMail, 'candidate');
                    }

                }
            }
            setTimeout(() => {
                swal({
                    title: 'Want to send Mail to Employees?',
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
                }).then(function (result) {
                    $('#EMP_EMAIL').val(1);

                    if (EmpMails) {
                        var singleEmpId = [];
                        var selectedEmployees = $("#color_hidden").val();

                        if (selectedEmployees) {
                            singleEmpId = selectedEmployees.split(',').map(function (item) {
                                return parseInt(item.trim());
                            });
                        }

                        if (singleEmpId.length > 0) {
                            var selectedMails = EmpMails.filter(function (x) {
                                return singleEmpId.includes(x.key);
                            }).map(function (x) {
                                return x.value;
                            });

                            console.log("Selected Emails:", selectedMails);

                            empr_HRInterviewSchedule.sendEmailsSequentially(selectedMails, 'employees');
                            empr_HRInterviewSchedule.saveAttempt();
                        }

                    } else {
                        empr_helper.notify(data.msg, data.msgType);
                    }
                })     
            }, empr_HRInterviewSchedule.canMailCheck == 0 ? 0 : 5000);

            
        }
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/HRInterviewSchedule/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_HRInterviewSchedule.resetForm();
                    empr_HRInterviewSchedule.InitTree();
                    empr_HRInterviewSchedule.reFreshTree();
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
        var CANDIDATE = $('#CANDIDATE').dxSelectBox('instance').option('value');
        var CAN_EMAIL = $("#CAN_EMAIL").val();
        var JOB = $('#JOB').dxSelectBox('instance').option('value');
        var INT_DATE = $("#INT_DATE").val();
        var INT_TIME = $("#INT_TIME").val();
        var INT_LOCATION = $("#INT_LOCATION").val();
        var INT_ROUND = $("#INT_ROUND").val();
        var REMARKS = $("#REMARKS").val();

        function checkRequired(value, fieldName) {
            if (!value || value === "" || value === null) {
                empr_helper.notify("Please enter/select " + fieldName + ".", 2);
                valid = false;
            }
        }

        function checkEmail(value) {
            var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            if (value && !emailPattern.test(value)) {
                empr_helper.notify("Please enter a valid Email address.", 2);
                valid = false;
            }
        }


        // Required validations
        checkRequired(ASTATUS, "Status");
        checkRequired(CANDIDATE, "Candidate");
        checkRequired(CAN_EMAIL, "Candidate Email");
        checkRequired(JOB, "Job");
        checkRequired(INT_DATE, "Interview Date");
        checkRequired(INT_TIME, "Interview Time");
        checkRequired(INT_LOCATION, "Interview Location");
        checkRequired(INT_ROUND, "Interview Round");
        checkRequired(REMARKS, "Remarks");

        checkEmail(CAN_EMAIL);

        return valid;
    },
    getDataToSave: function () {
        var TRAN_ID = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var CANDIDATE = $('#CANDIDATE').dxSelectBox('instance').option('value');
        var JOB = $('#JOB').dxSelectBox('instance').option('value');
        var INT_DATE = $("#INT_DATE").val();
        var INT_TIME = $("#INT_TIME").val();
        var INT_LOCATION = $("#INT_LOCATION").val();
        var INT_ROUND = $("#INT_ROUND").val();
        var REMARKS = $("#REMARKS").val();
        var EMP = $("#color_hidden").val();
        var EMP_EMAIL = $("#EMP_EMAIL").val();
        var CAN_EMAIL = $("#CAND_MAIL_SEND").is(":checked") ? 1 : 0;

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            ASTATUS: ASTATUS,
            CANDIDATE: CANDIDATE,
            CAN_EMAIL: CAN_EMAIL,
            JOB: JOB,
            INT_DATE: INT_DATE,
            INT_TIME: INT_TIME,
            INT_LOCATION: INT_LOCATION,
            INT_ROUND: INT_ROUND,
            REMARKS: REMARKS,
            EMP: EMP,
            EMP_EMAIL: EMP_EMAIL,
        };
        console.log('safeAttempt', modelRecord);
        return modelRecord;
    },
    saveAttempt: function () {
        var obj = empr_HRInterviewSchedule.getDataToSave();
        console.log(obj);
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/HRInterviewSchedule/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#optmodal').modal('hide');
                //$('.btn-delete').show();
                /*$('.btn-print').show();*/
                empr_HRInterviewSchedule.resetForm();
                //empr_HRInterviewSchedule.GetHRJobPostByID(data.data);
            }
        }, false, true);
    },
    resetForm: function () {
        $("#Code").val('');
        $("#JOB").val('');
        $("#INT_DATE").val('');
        $("#INT_LOCATION").val('');
        $("#INT_TIME").val('');
        $("#INT_ROUND").val('');
        $("#CAN_EMAIL").val('');
        $("#REMARKS").val('');
        $("#CAND_MAIL_SEND").prop('checked', false);


        empr_HRInterviewSchedule.InitJobDDL();
        empr_HRInterviewSchedule.InitCandidateDDL();
        empr_HRInterviewSchedule.InitMultipleEmpDDL();

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
        empr_HRInterviewSchedule.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/HRInterviewSchedule/QuickSearch', function (data) {
            if (data.msgType == 1) {
                console.log('quick grid data', data.data)
                empr_HRInterviewSchedule.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllHRJobPosts: function () {

        var xhr = ajaxHelper.ajaxGetJson('/HRJobPost/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_HRInterviewSchedule.CreateGrid(data.data);

        }, false, true);

    },
    GetHRJobPostByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/HRInterviewSchedule/HRJobPostByid?id=' + id, function (data) {
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
            $("#INT_DATE").val(data.data.inT_DATE);
            $("#INT_TIME").val(data.data.inT_TIME);
            $("#INT_LOCATION").val(data.data.inT_LOCATION);
            $("#INT_ROUND").val(data.data.inT_ROUND);
            $("#REMARKS").val(data.data.remarks);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            empr_HRInterviewSchedule.InitJobDDL(data.data.joB_ID);
            //var selectedVal = data.data.emP_ID ? [data.data.emP_ID] : [];
            //empr_HRInterviewSchedule.InitMultipleEmpDDL(selectedVal);

            var selectedVal = [];
            if (data.data.emP_ID) {
                selectedVal = data.data.emP_ID.split(',').map(function (item) {
                    return parseInt(item.trim()); // number me convert karna
                });
            }

            empr_HRInterviewSchedule.InitMultipleEmpDDL(selectedVal);

            console.log('..............', data.data.coN_ID);
            empr_HRInterviewSchedule.InitCandidateDDL(data.data.coN_ID);
            //$("#CAN_EMAIL").val(data.data.coN_EMAIL);
            //$("#EMP_EMAIL").val(data.data.emP_EMAIL);
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

                var data = JSON.stringify(options.data);

                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);


            }
        },
        //{ dataField: 'v_DATE', caption: 'Voucher Date' },
        { dataField: 'astatus', caption: 'Status' },
        { dataField: 'caN_NAME', caption: 'Candidate Name' },
        //{ dataField: 'coN_EMAIL', caption: 'Candidate Email' },
        { dataField: 'inT_DATE', caption: 'Int Date' },
        { dataField: 'inT_TIME', caption: 'Int Time' },
        { dataField: 'inT_LOCATION', caption: 'Int Location' },
        { dataField: 'inT_ROUND', caption: 'Int Round' },
        { dataField: 'job', caption: 'Job' },
        //{ dataField: 'emP_NAME', caption: 'Emp Name' },
        //{ dataField: 'emP_EMAIL', caption: 'Emp Email' },
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
                        empr_HRInterviewSchedule.GetHRJobPostByID(clickedRowData.acT_CODE);
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
    InitCandidateDDL: function (_selectedValue) {
        $('#CANDIDATE').dxSelectBox({
            dataSource: Candidate,
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
            onValueChanged: function (e) {
                if (e.value) {
                    let selected = CandidateMails.find(x => x.key === e.value);
                    console.log('Selected Candidate ki Mail', selected.value);
                    if (selected) {
                        $("#CAN_EMAIL").val(selected.value);
                    }
                } else {
                    $("#CAN_EMAIL").val("");
                }
            }
        });
    },
    InitEmpDDL: function (_selectedValue) {
        $('#EMP').dxSelectBox({
            dataSource: Employee,
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

            //onValueChanged: function (e) {
            //    if (e.value) {
            //        let selected = EmpMails.find(x => x.key === e.value);
            //        if (selected) {
            //            $("#EMP_EMAIL").val(selected.value);
            //        }
            //    } else {
            //        $("#EMAIL").val("");
            //    }
            //}
        });
    },
    InitMultipleEmpDDL: function (selectedVal) {

        empr_HRInterviewSchedule.MultipleDxGridBoxDropdown('#Color', Employee.data, [{ dataField: 'emP_ID', caption: 'ID', width: '60px' }, { dataField: 'ename', caption: 'Name' }, { dataField: 'designatioN_NAME', caption: 'Designation' }], 'hidden', null, selectedVal, 'code', 'ename', "#displayExpr_color", function (selectedvalue, hidden) {

            //console.log(selectedvalue.selectedRowsData); 
            if (selectedvalue.selectedRowsData.length > 0) {
                var array = selectedvalue.selectedRowsData;
                var keys = array.map(item => item.code).join(',');
                var values = array.map(item => item.ename).join(',');
                $('#color_hidden').val(keys);
                $('#displayExpr_color').val(values);
                empr_HRInterviewSchedule.isValueAssigned = false;
            }
            else {
                $('#color_hidden').val('');
                $('#displayExpr_color').val('');
            }
        }, 'multiple');
    },
    MultipleDxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.MultipleDxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
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
        empr_HRInterviewSchedule.InitReportTypeDDL();
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
    sendEmailsSequentially: function (mails, forType, index = 0) {
        $('#Loader').appendTo('body');
        $("#Loader").css({
            display: 'flex'
        });

        setTimeout(function () {
            if (index >= mails.length) {
                setTimeout(function () {
                    $("#Loader").hide();
                }, 1000);
                return;
            }

            var emp = mails[index];
            if (forType === "candidate") {
                var emailModel = {
                    To: emp.email,
                    Subject: "Test Email Subject to Candidate",
                    Message: "This is a test mail to Candidate"
                };
            }
            else if (forType === "employees") {
                var emailModel = {
                    To: emp,
                    Subject: "Test Email Subject to Employees",
                    Message: "This is a test mail to Employees"
                };
            }

            ajaxHelper.ajaxPostJsonData(emailModel, "/MailBox/SendMail", function (data) {
                empr_helper.notify(data.data + ' to \n' + data.name, data.msgType);

                if (data.msgType == 1) {
                    empr_HRInterviewSchedule.sendEmailsSequentially(mails, forType, index + 1);
                } else {
                    empr_HRInterviewSchedule.sendEmailsSequentially(mails, forType, index + 1);
                }
            }, false, true);
        }, 200);

    }
}