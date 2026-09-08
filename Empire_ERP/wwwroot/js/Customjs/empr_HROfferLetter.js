var empr_HROfferLetter = {
    canMailCheck: 0,
    isValueAssigned: false,
    currentFormData: {},
    initEvents: function () {
        $(document).ready(function () {
            console.log('Department', Department);
            console.log('company', company);
            $('#resetall').show();
            empr_HROfferLetter.resetForm();

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {

                        if (empr_HROfferLetter.validateForm()) {
                            var data = empr_HROfferLetter.getDataToSave();
                            console.log('1', data);
                            if (data.ASTATUS == 'Sent') {
                                empr_HROfferLetter.currentFormData = data;
                                console.log('2', data.INT_ID);
                                empr_HROfferLetter.GetCandidateEmailById(data.INT_ID);
                            }
                            empr_HROfferLetter.saveAttempt();
                        }
                    }
                } else {

                    if (empr_HROfferLetter.validateForm()) {
                        var data = empr_HROfferLetter.getDataToSave();
                        console.log('1', data);
                        if (data.ASTATUS == 'Sent') {
                            empr_HROfferLetter.currentFormData = data;
                            console.log('2', data.INT_ID);
                            empr_HROfferLetter.GetCandidateEmailById(data.INT_ID);
                        }
                        empr_HROfferLetter.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_HROfferLetter.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_HROfferLetter.GeneratePrintReport();
            });

            $('body').on('click', '.btn-print', function () {
                empr_HROfferLetter.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid");
                empr_helper.selectedBill = rportid;
                empr_HROfferLetter.GetHRJobPostByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').show();
                empr_HROfferLetter.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_HROfferLetter.DeleteRecord();
            })

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_HROfferLetter.GeneratePrintReport();
            });

            $(document).on('click', '.swal2-cancel', function () {
                empr_HROfferLetter.HRMail = 0;
            });

            $('#BASIC_SALARY').change(function () {
                empr_HROfferLetter.calculateTotalPkg();
            });

            $('#ALLOWANCES').change(function () {
                empr_HROfferLetter.calculateTotalPkg();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }
        });
    },
    GetCandidateEmailById: function (canId) {
        ajaxHelper.ajaxPostJsonData({ canId: canId }, "/HROfferLetter/GetCandidateEmailById", function (data) {
            if (data.msgType == 1) {
                empr_HROfferLetter.sendEmailsSequentially(data.data);
            }
        }, false, true);
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/HROfferLetter/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_HROfferLetter.resetForm();
                    empr_HROfferLetter.InitTree();
                    empr_HROfferLetter.reFreshTree();
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
        var INTERVIEW = $('#INTERVIEW').dxSelectBox('instance').option('value');
        var JOB_TITLE = $('#JOB_TITLE').val();
        var DEP = $('#DEP').dxSelectBox('instance').option('value');
        var BASIC_SALARY = $('#BASIC_SALARY').val();
        var ALLOWANCES = $('#ALLOWANCES').val();
        var TOTAL_PACKAGE = $('#TOTAL_PACKAGE').val();
        var JOINING_DATE = $('#JOINING_DATE').val();
        var OFFER_DATE = $('#OFFER_DATE').val();
        var DUTY_START = $('#DUTY_START').val();
        var DUTY_END = $('#DUTY_END').val();
        var WEEKLY_OFF = $('#WEEKLY_OFF').val();
        var WEEKLY_DESC = $('#WEEKLY_DESC').val();
        var DOC = $("#hdnDOC").val();


        function checkRequired(value, fieldName) {
            if (!value || value === "" || value === null || value == undefined) {
                empr_helper.notify("Please enter/select " + fieldName + ".", 2);
                valid = false;
            }
        }

        checkRequired(ASTATUS, "Status");
        checkRequired(INTERVIEW, "Interview");
        checkRequired(JOB_TITLE, "Job Title");
        checkRequired(DEP, "Department");
        checkRequired(BASIC_SALARY, "Basic Salary");
        checkRequired(ALLOWANCES, "Allowances");
        checkRequired(TOTAL_PACKAGE, "Total Package");
        checkRequired(JOINING_DATE, "Joining Date");
        checkRequired(OFFER_DATE, "Offer Date");
        checkRequired(DUTY_START, "Duty Start");
        checkRequired(DUTY_END, "Duty End");
        checkRequired(WEEKLY_OFF, "Weekly Off");
        checkRequired(WEEKLY_DESC, "Weekly Desc");
        checkRequired(DOC, "Document");


        return valid;
    },
    getDataToSave: function () {
        var TRAN_ID = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var INTERVIEW = $('#INTERVIEW').dxSelectBox('instance').option('value');
        var JOB_TITLE = $('#JOB_TITLE').val();
        var DEP = $('#DEP').dxSelectBox('instance').option('value');
        var BASIC_SALARY = $('#BASIC_SALARY').val();
        var ALLOWANCES = $('#ALLOWANCES').val();
        var TOTAL_PACKAGE = $('#TOTAL_PACKAGE').val();
        var JOINING_DATE = $('#JOINING_DATE').val();
        var OFFER_DATE = $('#OFFER_DATE').val();
        var DUTY_START = $('#DUTY_START').val();
        var DUTY_END = $('#DUTY_END').val();
        var WEEKLY_OFF = $('#WEEKLY_OFF').val();
        var WEEKLY_DESC = $('#WEEKLY_DESC').val();
        var DOC = $("#hdnDOC").val();

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            ASTATUS: ASTATUS,
            INT_ID: INTERVIEW,
            JOB_TITLE: JOB_TITLE,
            DEP_ID: DEP,
            BASIC_SALARY: BASIC_SALARY,
            ALLOWANCES: ALLOWANCES,
            TOTAL_PACKAGE: TOTAL_PACKAGE,
            JOINING_DATE: JOINING_DATE,
            OFFER_DATE: OFFER_DATE,
            DUTY_START: DUTY_START,
            DUTY_END: DUTY_END,
            WEEKLY_OFF: WEEKLY_OFF,
            WEEKLY_DESC: WEEKLY_DESC,
            DOC: DOC,
        };

        return modelRecord;
    },
    saveAttempt: function () {
        var obj = empr_HROfferLetter.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/HROfferLetter/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#optmodal').modal('hide');
                //$('.btn-delete').show();
                /*$('.btn-print').show();*/
                empr_HROfferLetter.resetForm();
            }
        }, false, true);
    },
    resetForm: function () {
        $("#Code").val('');
        $('#JOB_TITLE').val('');
        $('#BASIC_SALARY').val('');
        $('#ALLOWANCES').val('');
        $('#TOTAL_PACKAGE').val('');
        $('#JOINING_DATE').val('');
        $('#OFFER_DATE').val('');
        $('#DUTY_START').val('');
        $('#DUTY_END').val('');
        $('#WEEKLY_OFF').val('');
        $('#WEEKLY_DESC').val('');
        $("#hdnDOC").val('');
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');

        empr_HROfferLetter.InitInterviewDDL();
        empr_HROfferLetter.InitDepartmentDDL();

        const draftSentStatuses = empr_helper.hrStatus.filter(status =>
            status.key === 'Draft' || status.key === 'Sent'
        );

        empr_HROfferLetter.InitStatusDDL(draftSentStatuses, null);
        $('.btn-print').hide();


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
        empr_HROfferLetter.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/HROfferLetter/QuickSearch', function (data) {
            console.log('QuickSearch', data);
            if (data.msgType == 1) {
                empr_HROfferLetter.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllHRJobPosts: function () {

        var xhr = ajaxHelper.ajaxGetJson('/HROfferLetter/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_HROfferLetter.CreateGrid(data.data);

        }, false, true);

    },
    GetHRJobPostByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/HROfferLetter/HRJobPostByid?id=' + id, function (data) {
            console.log('Edit Data', data);
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
                $('.btn-print').show();
                $('#resetall').show();
            }
            $('.modal').modal('hide');

            $('#Code').val(data.data.traN_ID);
            empr_HROfferLetter.InitStatusDDL(empr_helper.hrStatus, data.data.astatus);
            empr_HROfferLetter.InitInterviewDDL(data.data.inT_ID);
            $("#JOB_TITLE").val(data.data.joB_TITLE);
            empr_HROfferLetter.InitDepartmentDDL(data.data.deP_ID);



            $('#BASIC_SALARY').val(data.data.basiC_SALARY);
            $('#ALLOWANCES').val(data.data.allowances);
            $('#TOTAL_PACKAGE').val(data.data.totaL_PACKAGE);
            $('#JOINING_DATE').val(data.data.joininG_DATE);
            $('#OFFER_DATE').val(data.data.offeR_DATE);
            $('#DUTY_START').val(data.data.dutY_START);
            $('#DUTY_END').val(data.data.dutY_END);
            $('#WEEKLY_OFF').val(data.data.weeklY_OFF);
            $('#WEEKLY_DESC').val(data.data.weeklY_DESC);
            $('#hdnDOC').val(data.data.doc);

            //empr_HROfferLetter.InitRecommendationDDL(data.data.finaL_RECOM);
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

                var data = JSON.stringify(options.data);

                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
                                </div>`).appendTo(container);


            }
        },
        { dataField: 'astatus', caption: 'Status' },
        { dataField: 'caN_NAME', caption: 'Candidate Name' },
        { dataField: 'deP_NAME', caption: 'Depart Name' },
        { dataField: 'joB_TITLE', caption: 'Job Title' },
        { dataField: 'offeR_DATE', caption: 'Offer Date' },
        { dataField: 'joininG_DATE', caption: 'Joining Date' },
        { dataField: 'basiC_SALARY', caption: 'Basic Salary' },
        { dataField: 'allowances', caption: 'Allowances' },
        { dataField: 'totaL_PACKAGE', caption: 'Total Pkg' },
        { dataField: 'dutY_START', caption: 'Duty Start' },
        { dataField: 'dutY_END', caption: 'Duty End' },
        { dataField: 'weeklY_DESC', caption: 'Weekly Desc' },
        { dataField: 'weeklY_OFF', caption: 'Weekly Off' },
        { dataField: 'traN_ID', caption: 'Code', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "HRJobPostQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    InitInterviewDDL: function (_selectedValue) {
        $('#INTERVIEW').dxSelectBox({
            dataSource: InterviewsDropdown,
            displayExpr: 'caN_NAME',
            valueExpr: 'traN_ID',
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
                var selected = e.selectedItem || InterviewsDropdown.find(x => x.traN_ID === e.value);
                if (selected) {
                    $('#JOB_TITLE').val(selected.joB_TITLE);
                    empr_HROfferLetter.InitDepartmentDDL(selected.deP_ID);
                } else {
                    $('#JOB_TITLE').val('');
                    empr_HROfferLetter.InitDepartmentDDL();
                }
            },
        });
    },
    InitDepartmentDDL: function (_selectedValue) {
        $('#DEP').dxSelectBox({
            dataSource: Department,
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
    InitStatusDDL: function (data, selectedValue) {

        $('#ASTATUS').dxSelectBox({
            dataSource: data,
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
    formatTimeTo12Hour: function (timeStr) {
        if (!timeStr) return "";
        var parts = timeStr.split(':');
        var hours = parseInt(parts[0], 10);
        var minutes = parts[1];

        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // 0 ko 12 banana

        return hours + ':' + minutes + ' ' + ampm;
    },
    sendEmailsSequentially: function (mails, index = 0) {
        console.log('mails', mails);
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

            var data = empr_HROfferLetter.currentFormData;
            console.log('currentFormData', data);
            var emp = mails[index];
            var salaryLine = "";

            if (data.ALLOWANCES > 0) {
                salaryLine = `Your total monthly package will include a Basic Salary of ${data.BASIC_SALARY} plus Allowances of ${data.ALLOWANCES}, making it ${data.TOTAL_PACKAGE} in total.`;
            } else {
                salaryLine = `Your total monthly salary package will be ${data.TOTAL_PACKAGE}.`;
            }

            var deptValue = "";
            var deptObj = Department.find(d => d.key == data.DEP_ID);
            if (deptObj) {
                deptValue = deptObj.value;
            }

            var dutyStart = empr_HROfferLetter.formatTimeTo12Hour(data.DUTY_START);
            var dutyEnd = empr_HROfferLetter.formatTimeTo12Hour(data.DUTY_END);

            var offerMessage = `
                                Dear ${emp.fulL_NAME},

                                We are pleased to offer you the position of ${data.JOB_TITLE} in our ${deptValue}.  
                                ${salaryLine}
                                Your expected joining date is ${data.JOINING_DATE}.  

                                Your regular working hours will be from ${dutyStart} to ${dutyEnd}, 
                                with ${data.WEEKLY_OFF} as your weekly off.  

                                We look forward to having you as part of our team.

                                Sincerely,  
                                ${company}  
                                [HR Manager Name]
                            `;

            var emailModel = {
                To: emp.email,
                Subject: `Offer Letter – ${data.JOB_TITLE} at [Company Name]`,
                Message: offerMessage
            };

            //ajaxHelper.ajaxPostJsonData(emailModel, "/MailBox/SendMail", function (data) {
            //    empr_helper.notify(data.data, data.msgType);

            //    if (data.msgType == 1) {
            //        empr_HROfferLetter.sendEmailsSequentially(mails, forType, index + 1);
            //    } else {
            //        empr_HROfferLetter.sendEmailsSequentially(mails, forType, index + 1);
            //    }
            //}, false, true);
        }, 200);

    },
    calculateTotalPkg: function () {
        var SAL = parseFloat($("#BASIC_SALARY").val()) || 0;
        var ALL = parseFloat($("#ALLOWANCES").val()) || 0;

        var TotalPkg = SAL + ALL;
        $("#TOTAL_PACKAGE").val(TotalPkg);

    },
    GeneratePrintReport: function () {
        empr_HROfferLetter.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("TranId null", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/HROfferLetter/GetPrintReport", function (data) {
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
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/HROfferLetter/GetReportTypes", function (data) {
            console.log('InitReportTypeDDL', data.msgType);
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
    }

}