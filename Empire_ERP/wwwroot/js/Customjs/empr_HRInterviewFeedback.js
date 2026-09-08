var empr_HRInterviewFeedback = {
    canMailCheck: 0,
    isValueAssigned: false,
    InterviewId: 0,
    HRMail: 0,
    initEvents: function () {
        $(document).ready(function () {
            $('#resetall').show();
            empr_HRInterviewFeedback.resetForm();


            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_HRInterviewFeedback.validateForm()) {
                            empr_HRInterviewFeedback.saveAttempt();
                            swal({
                                title: 'Do you want to send email to HR?',
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
                                empr_HRInterviewFeedback.HRMail = 1;
                                ajaxHelper.ajaxGetJson('/HRInterviewFeedback/GetHRMails', function (data) {
                                    console.log('HRMails', data);
                                    if (data.msgType == 1) {
                                        empr_HRInterviewFeedback.sendEmailsSequentially(data.data);
                                    }
                                    else {
                                        empr_helper.notify(data.msg, data.msgType);
                                    }
                                }, false, true);
                            });
                        }
                    }
                } else {
                    if (empr_HRInterviewFeedback.validateForm()) {
                        empr_HRInterviewFeedback.saveAttempt();
                        swal({
                            title: 'Do you want to send email to HR?',
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
                            empr_HRInterviewFeedback.HRMail = 1;
                            ajaxHelper.ajaxGetJson('/HRInterviewFeedback/GetHRMails', function (data) {
                                console.log('HRMails', data);
                                if (data.msgType == 1) {
                                    empr_HRInterviewFeedback.sendEmailsSequentially(data.data);
                                }
                                else {
                                    empr_helper.notify(data.msg, data.msgType);
                                }
                            }, false, true);
                        });
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_HRInterviewFeedback.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_HRInterviewFeedback.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_HRInterviewFeedback.GetHRJobPostByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').show();
                empr_HRInterviewFeedback.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_HRInterviewFeedback.DeleteRecord();
            })


            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_HRInterviewFeedback.GeneratePrintReport();
            });

            $(document).on('click', '.swal2-cancel', function () {
                empr_HRInterviewFeedback.HRMail = 0;
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }
        });

    },
    InitRatingStars: function (csSelected, tsSelected, psSelected, conSelected, cfSelected) {
        empr_HRInterviewFeedback.CreateRatingStar('cs_star', 'cs_selectedRating', 'cs_emoji', csSelected);
        empr_HRInterviewFeedback.CreateRatingStar('ts_star', 'ts_selectedRating', 'ts_emoji', tsSelected);
        empr_HRInterviewFeedback.CreateRatingStar('ps_star', 'ps_selectedRating', 'ps_emoji', psSelected);
        empr_HRInterviewFeedback.CreateRatingStar('con_star', 'con_selectedRating', 'con_emoji', conSelected);
        empr_HRInterviewFeedback.CreateRatingStar('cf_star', 'cf_selectedRating', 'cf_emoji', cfSelected);
    },
    CreateRatingStar: function (starClass, selectedRatingClass, emojiClass, initialRating) {
        var $element = $('.' + starClass);
        var $selectedRating = $('.' + selectedRatingClass);
        var $emoji = $('.' + emojiClass);

        var rating = 0;
        var firstStarClickCount = 0;

        var emojis = {
            0.5: "😡",
            1: "😠",
            1.5: "😕",
            2: "🙁",
            2.5: "😐",
            3: "🙂",
            3.5: "😊",
            4: "😁",
            4.5: "🤩",
            5: "🔥"
        };

        var selectedRating = initialRating;
        if (selectedRating > 0) {
            rating = selectedRating;
            updateStars(rating);
            $selectedRating.text(rating);
            $emoji.text(emojis[rating] || "🙂");
        }

        $element.on('click', function () {
            var value = parseInt($(this).data('value'));

            if (value === 1 && rating > 1) {
                rating = 0;
                resetStars();
                $selectedRating.text(rating);
                $emoji.text("🙂");
                if (starClass === 'cs_star') $('#communicationRating').val(rating);
                else if (starClass === 'ts_star') $('#technicalRating').val(rating);
                else if (starClass === 'ps_star') $('#problemRating').val(rating);
                else if (starClass === 'con_star') $('#confidenceRating').val(rating);
                else if (starClass === 'cf_star') $('#culturalRating').val(rating);
                return;
            }

            if (value === 1) {
                firstStarClickCount++;
                if (firstStarClickCount === 3) {
                    rating = 0;
                    firstStarClickCount = 0;
                    resetStars();
                    $selectedRating.text(rating);
                    $emoji.text("🙂");
                    if (starClass === 'cs_star') $('#communicationRating').val(rating);
                    else if (starClass === 'ts_star') $('#technicalRating').val(rating);
                    else if (starClass === 'ps_star') $('#problemRating').val(rating);
                    else if (starClass === 'con_star') $('#confidenceRating').val(rating);
                    else if (starClass === 'cf_star') $('#culturalRating').val(rating);
                    return;
                }
            } else {
                firstStarClickCount = 0;
            }

            if (rating === value - 0.5) {
                rating = value;
            } else {
                rating = value - 0.5;
            }

            updateStars(rating);
            $selectedRating.text(rating);
            $emoji.text(emojis[rating] || "🙂");
            if (starClass === 'cs_star') $('#communicationRating').val(rating);
            else if (starClass === 'ts_star') $('#technicalRating').val(rating);
            else if (starClass === 'ps_star') $('#problemRating').val(rating);
            else if (starClass === 'con_star') $('#confidenceRating').val(rating);
            else if (starClass === 'cf_star') $('#culturalRating').val(rating);
        });

        function updateStars(r) {
            $element.each(function (index) {
                $(this).removeClass('half full');
                if (index < Math.floor(r)) {
                    $(this).addClass('full');
                }
            });

            if (r % 1 !== 0) {
                $($element[Math.floor(r)]).addClass('half');
            } else if (r > 0) {
                $($element[Math.floor(r) - 1]).addClass('full');
            }
        }

        function resetStars() {
            $element.removeClass('half full');
        }
    },
    ResetRatingStars: function () {
        $('.cs_star').removeClass('half full');
        $('.cs_selectedRating').text(0);
        $('.cs_emoji').text('🙂');
        $('#communicationRating').val(0);

        // Technical Skills
        $('.ts_star').removeClass('half full');
        $('.ts_selectedRating').text(0);
        $('.ts_emoji').text('🙂');
        $('#technicalRating').val(0);

        // Problem Solving
        $('.ps_star').removeClass('half full');
        $('.ps_selectedRating').text(0);
        $('.ps_emoji').text('🙂');
        $('#problemRating').val(0);

        // Confidence Skills
        $('.con_star').removeClass('half full');
        $('.con_selectedRating').text(0);
        $('.con_emoji').text('🙂');
        $('#confidenceRating').val(0);

        // Cultural Fit
        $('.cf_star').removeClass('half full');
        $('.cf_selectedRating').text(0);
        $('.cf_emoji').text('🙂');
        $('#culturalRating').val(0);
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/HRInterviewFeedback/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_HRInterviewFeedback.resetForm();
                    empr_HRInterviewFeedback.InitTree();
                    empr_HRInterviewFeedback.reFreshTree();
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
        //var CANDIDATE = $('#CANDIDATE').dxSelectBox('instance').option('value');
        var FINAL_RECOM = $('#FINAL_RECOM').dxSelectBox('instance').option('value');
        var COMM_SKILL = $('#communicationRating').val();
        var TECH_SKILL = $('#technicalRating').val();
        var PROBLEM_SOLVED = $('#problemRating').val();
        var CONF_SKILL = $('#confidenceRating').val();
        var CUL_FIT = $('#culturalRating').val();
        var INTERVIEW = $('#INTERVIEW').dxSelectBox('instance').option('value');
        
        var OVERALL_REMARKS = $("#OVERALL_REMARKS").val();
        var EMP_ID = $('#ACT_GROUP_hidden').val();

        function checkRequired(value, fieldName) {
            if (!value || value === "" || value === null) {
                empr_helper.notify("Please enter/select " + fieldName + ".", 2);
                valid = false;
            }
        }
        function checkNum(value, field) {
            value = Number(value);
            if (value === 0 || value === null || value == undefined) {
                empr_helper.notify("Please select " + field + " rating stars .", 2);
                valid = false;
            }
        }

        checkRequired(ASTATUS, "Status");
        checkRequired(FINAL_RECOM, "Final Recommendation");
        checkRequired(OVERALL_REMARKS, "Overall Remarks");
        if (FINAL_RECOM != 'Reject') {
            checkNum(CUL_FIT, "Cultural Fit / Team Fit");
            checkNum(CONF_SKILL, "Confidence Skills");
            checkNum(PROBLEM_SOLVED, "Problem Solving");
            checkNum(TECH_SKILL, "Technical Skill");
            checkNum(COMM_SKILL, "Communication Skill");
        }
        //checkRequired(REMARKS, "Comments");
        
        //checkRequired(CANDIDATE, "Candidate");
        checkRequired(INTERVIEW, "Interview");
        checkRequired(EMP_ID, "Employee");

        return valid;
    },
    getDataToSave: function () {
        var TRAN_ID = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var EMP_ID = $('#ACT_GROUP_hidden').val();
        var INT_ID = $('#INTERVIEW').dxSelectBox('instance').option('value');
        //var CON_ID = $('#CANDIDATE').dxSelectBox('instance').option('value');
        var FINAL_RECOM = $('#FINAL_RECOM').dxSelectBox('instance').option('value');
        debugger;
        if (FINAL_RECOM != 'Reject') {
            var COMM_SKILL = $('#communicationRating').val();
            var TECH_SKILL = $('#technicalRating').val();
            var PROBLEM_SOLVED = $('#problemRating').val();
            var CONF_SKILL = $('#confidenceRating').val();
            var CUL_FIT = $('#culturalRating').val();
        }
        else {
            var COMM_SKILL = 0;
            var TECH_SKILL = 0;
            var PROBLEM_SOLVED = 0;
            var CONF_SKILL = 0;
            var CUL_FIT = 0;
        }
        var OVERALL_REMARKS = $("#OVERALL_REMARKS").val();
        var HR_MAIL = empr_HRInterviewFeedback.HRMail;

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            ASTATUS: ASTATUS,
            EMP_ID: EMP_ID,
            INT_ID: INT_ID,
            //CON_ID: CON_ID,
            COMM_SKILL: COMM_SKILL,
            TECH_SKILL: TECH_SKILL,
            PROBLEM_SOLVED: PROBLEM_SOLVED,
            CONF_SKILL: CONF_SKILL,
            CUL_FIT: CUL_FIT,
            OVERALL_REMARKS: OVERALL_REMARKS,
            FINAL_RECOM: FINAL_RECOM,
            HR_MAIL: HR_MAIL,
        };

        return modelRecord;
    },
    saveAttempt: function () {
        var obj = empr_HRInterviewFeedback.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/HRInterviewFeedback/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#optmodal').modal('hide');
                //$('.btn-delete').show();
                /*$('.btn-print').show();*/
                empr_HRInterviewFeedback.resetForm();
            }
        }, false, true);
    },
    resetForm: function () {
        $("#Code").val('');
        $('#communicationRating').val();
        $('#technicalRating').val();
        $('#problemRating').val();
        $('#confidenceRating').val();
        $('#culturalRating').val();
        $("#OVERALL_REMARKS").val('');
        $('#ACT_GROUP_hidden').val();
        //empr_HRInterviewFeedback.InitCandidateDDL();
        empr_HRInterviewFeedback.InitAccountGroupGridBox();
        empr_HRInterviewFeedback.InitInterviewDDL();
        empr_HRInterviewFeedback.ResetRatingStars();
        empr_HRInterviewFeedback.InitRatingStars();
        empr_HRInterviewFeedback.InitRecommendationDDL();


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
        empr_HRInterviewFeedback.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/HRInterviewFeedback/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_HRInterviewFeedback.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllHRJobPosts: function () {

        var xhr = ajaxHelper.ajaxGetJson('/HRJobPost/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_HRInterviewFeedback.CreateGrid(data.data);

        }, false, true);

    },
    GetHRJobPostByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/HRInterviewFeedback/HRJobPostByid?id=' + id, function (data) {
            console.log('Edit Data', data);
            empr_HRInterviewFeedback.InterviewId = data.data.inT_ID;
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
            $('.modal').modal('hide');

            $('#Code').val(data.data.traN_ID);
            $("#OVERALL_REMARKS").val(data.data.oveR_ALL_REMARKS);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);

            $('#communicationRating').val(data.data.comM_SKILL);
            $('#technicalRating').val(data.data.tecH_SKILL);
            $('#problemRating').val(data.data.probleM_SOLVED);
            $('#confidenceRating').val(data.data.conF_SKILL);
            $('#culturalRating').val(data.data.cuL_FIT);

            empr_HRInterviewFeedback.InitAccountGroupGridBox(data.data.emP_ID);
            //empr_HRInterviewFeedback.InitCandidateDDL(data.data.coN_ID);
            empr_HRInterviewFeedback.InitRatingStars(data.data.comM_SKILL, data.data.tecH_SKILL, data.data.probleM_SOLVED, data.data.conF_SKILL, data.data.cuL_FIT);
            empr_HRInterviewFeedback.InitRecommendationDDL(data.data.finaL_RECOM);
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
                                </div>`).appendTo(container);


            }
        },
        { dataField: 'astatus', caption: 'Status' },
        { dataField: 'caN_NAME', caption: 'Candidate Name' },
            {//1
                dataField: 'comM_SKILL',
                caption: 'Communication Skills',
                cellTemplate: function (container, options) {
                    let rating = options.value || 0; // Value from your data, e.g. 1.5, 3, 4.5, etc.
                    let maxStars = 5;

                    for (let i = 1; i <= maxStars; i++) {
                        let $star = $('<span>★</span>').css({
                            fontSize: '18px',
                            marginRight: '2px',
                            color: '#ddd' // default grey
                        });

                        if (rating >= i) {
                            // Full star
                            $star.css('color', 'steelblue');
                        } else if (rating >= i - 0.5) {
                            // Half star
                            $star.css({
                                background: 'linear-gradient(90deg, steelblue 50%, #ddd 50%)',
                                '-webkit-background-clip': 'text',
                                '-webkit-text-fill-color': 'transparent',
                                color: 'steelblue'
                            });
                        }
                        container.append($star);
                    }
                }
            },
            {//2
                dataField: 'tecH_SKILL',
                caption: 'Technical Skills',
                cellTemplate: function (container, options) {
                    let rating = options.value || 0; // Value from your data, e.g. 1.5, 3, 4.5, etc.
                    let maxStars = 5;

                    for (let i = 1; i <= maxStars; i++) {
                        let $star = $('<span>★</span>').css({
                            fontSize: '18px',
                            marginRight: '2px',
                            color: '#ddd' // default grey
                        });

                        if (rating >= i) {
                            // Full star
                            $star.css('color', 'steelblue');
                        } else if (rating >= i - 0.5) {
                            // Half star
                            $star.css({
                                background: 'linear-gradient(90deg, steelblue 50%, #ddd 50%)',
                                '-webkit-background-clip': 'text',
                                '-webkit-text-fill-color': 'transparent',
                                color: 'steelblue'
                            });
                        }
                        container.append($star);
                    }
                }
            },
            {//3
                dataField: 'probleM_SOLVED',
                caption: 'Problem Solving',
                cellTemplate: function (container, options) {
                    let rating = options.value || 0; // Value from your data, e.g. 1.5, 3, 4.5, etc.
                    let maxStars = 5;

                    for (let i = 1; i <= maxStars; i++) {
                        let $star = $('<span>★</span>').css({
                            fontSize: '18px',
                            marginRight: '2px',
                            color: '#ddd' // default grey
                        });

                        if (rating >= i) {
                            // Full star
                            $star.css('color', 'steelblue');
                        } else if (rating >= i - 0.5) {
                            // Half star
                            $star.css({
                                background: 'linear-gradient(90deg, steelblue 50%, #ddd 50%)',
                                '-webkit-background-clip': 'text',
                                '-webkit-text-fill-color': 'transparent',
                                color: 'steelblue'
                            });
                        }
                        container.append($star);
                    }
                }
            },
            {//4
                dataField: 'conF_SKILL',
                caption: 'Confidence Skills',
                cellTemplate: function (container, options) {
                    let rating = options.value || 0; // Value from your data, e.g. 1.5, 3, 4.5, etc.
                    let maxStars = 5;

                    for (let i = 1; i <= maxStars; i++) {
                        let $star = $('<span>★</span>').css({
                            fontSize: '18px',
                            marginRight: '2px',
                            color: '#ddd' // default grey
                        });

                        if (rating >= i) {
                            // Full star
                            $star.css('color', 'steelblue');
                        } else if (rating >= i - 0.5) {
                            // Half star
                            $star.css({
                                background: 'linear-gradient(90deg, steelblue 50%, #ddd 50%)',
                                '-webkit-background-clip': 'text',
                                '-webkit-text-fill-color': 'transparent',
                                color: 'steelblue'
                            });
                        }
                        container.append($star);
                    }
                }
            },
            {//5
                dataField: 'cuL_FIT',
                caption: 'Cultural Fit / Team Fit',
                cellTemplate: function (container, options) {
                    let rating = options.value || 0; // Value from your data, e.g. 1.5, 3, 4.5, etc.
                    let maxStars = 5;

                    for (let i = 1; i <= maxStars; i++) {
                        let $star = $('<span>★</span>').css({
                            fontSize: '18px',
                            marginRight: '2px',
                            color: '#ddd' // default grey
                        });

                        if (rating >= i) {
                            // Full star
                            $star.css('color', 'steelblue');
                        } else if (rating >= i - 0.5) {
                            // Half star
                            $star.css({
                                background: 'linear-gradient(90deg, steelblue 50%, #ddd 50%)',
                                '-webkit-background-clip': 'text',
                                '-webkit-text-fill-color': 'transparent',
                                color: 'steelblue'
                            });
                        }
                        container.append($star);
                    }
                }
            },
        { dataField: 'overalL_REMARKS', caption: 'Overall Remarks' },
        { dataField: 'finaL_RECOM', caption: 'Final Recommendation' },
        { dataField: 'emP_NAME', caption: 'Emp Name' },
        { dataField: 'traN_ID', caption: 'Code', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "HRJobPostQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    //InitCandidateDDL: function (_selectedValue) {
    //    $('#CANDIDATE').dxSelectBox({
    //        dataSource: Candidate,
    //        displayExpr: 'value',
    //        valueExpr: 'key',
    //        value: _selectedValue,
    //        searchEnabled: true,
    //        width: '100%',
    //        placeholder: 'Search',
    //        showClearButton: true,
    //        dropDownOptions: {
    //            height: 'auto',
    //        },
    //        pagingEnabled: true,
    //        searchTimeout: 500,
    //    });
    //},
    InitInterviewDDL: function (empId, _selectedValue) {
        ajaxHelper.ajaxGetJson('/HRInterviewFeedback/InterviewsByEmpDropdown?id=' + empId, function (data) {
            $('#INTERVIEW').dxSelectBox({
                dataSource: data,
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
        }, false, true);


    },
    InitRecommendationDDL: function (selectedValue) {

        $('#FINAL_RECOM').dxSelectBox({
            dataSource: empr_helper.hrRecommendation,
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
                if (e.value == 'Reject') {
                    $(".hideOnReject").hide();

                    //$('#communicationRating').val(0);
                    //$('#technicalRating').val(0);
                    //$('#problemRating').val(0);
                    //$('#confidenceRating').val(0);
                    //$('#culturalRating').val(0);
                }
                else {
                    $(".hideOnReject").show();
                }
            },
        });
    },
    InitAccountGroupGridBox: function (_selectedValue) {
        $.ajax({
            url: "EmpLeaves/GetEmployeeGroups",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.code == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].code;
                        $('#ACT_GROUP_hidden').val(selectedObject[0].code);
                        $('#displayExprAccGroup').val(selectedObject[0].ename);
                    }
                }
                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#ENAME").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "code",
                    displayExpr: function (item) {
                        return item ? `${item.ename}` : "Select a value...";
                    },
                    dataSource: Datasource,
                    placeholder: 'Select a value...',
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            if (selectedValue === null || selectedValue === 0 || selectedValue === undefined) {
                                const selectedData = gridInstance.getDataSource().items().find(item => item.code === e.value);
                                empr_HRInterviewFeedback.InitInterviewDDL(selectedData.code);

                                if (selectedData) {
                                    $('#ACT_GROUP_hidden').val(selectedData.code);
                                    $('#displayExprAccGroup').val(selectedData.ename);
                                }
                            }
                            else {
                                const selectedData = gridInstance.getDataSource().items().find(item => item.code === e.value);
                                empr_HRInterviewFeedback.InitInterviewDDL(selectedData.code, empr_HRInterviewFeedback.InterviewId);

                                if (selectedData) {
                                    $('#ACT_GROUP_hidden').val(selectedData.code);
                                    $('#displayExprAccGroup').val(selectedData.ename);
                                }
                            }
                        } else {
                            $('#ACT_GROUP_hidden').val('');
                            $('#displayExprAccGroup').val('');
                        }
                    },
                    onOpened: function (e) {
                        if (!currentSearchTerm) {
                            if (gridInstance) {
                                gridInstance.getDataSource().filter(null);
                                gridInstance.refresh();
                            }
                        }

                        setTimeout(() => {
                            const input = e.component._$element.find(".dx-texteditor-input").first();
                            input.focus();
                            if (input.val()) {
                                input.select();
                            }
                        }, 50);
                    },
                    onInput: function (e) {
                        currentSearchTerm = e.event.target.value;

                        if (!e.component.option("opened")) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 100);
                        }

                        if (gridInstance) {
                            applyGridFilter(gridInstance, currentSearchTerm);
                        }
                    },
                    contentTemplate: function (e) {
                        gridInstance = $("<div>").dxDataGrid({
                            dataSource: new DevExpress.data.DataSource({
                                store: Datasource,
                                key: "code"
                            }),
                            columns: [
                                {
                                    dataField: "emP_ID",
                                    caption: "ID",
                                    width: '100px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "ename",
                                    caption: "Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "designatioN_NAME",
                                    caption: "Designation",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                }
                            ],
                            selection: {
                                mode: "single",
                                showCheckBoxesMode: "always"
                            },
                            hoverStateEnabled: true,
                            height: 300,
                            keyboardNavigation: {
                                enabled: true,
                                enterKeyAction: "select",
                                editOnKeyPress: true
                            },
                            onSelectionChanged: function (selectedItems) {
                                const selected = selectedItems.selectedRowsData[0];
                                if (selected) {
                                    e.component.option("value", selected.code);
                                    e.component.close();

                                    $('#ACT_GROUP_hidden').val(selected.code);
                                    $('#displayExprAccGroup').val(selected.ename);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].code], false);
                                    }
                                }
                            }
                        }).dxDataGrid("instance");

                        gridInstance.element().on('click', function (event) {
                            event.stopPropagation();
                        });

                        return gridInstance.element();
                    }

                });

                function applyGridFilter(grid, searchTerm) {
                    const dataSource = grid.getDataSource();
                    if (searchTerm) {
                        dataSource.filter([
                            ["ename", "contains", searchTerm],
                            "or",
                            ["code", "contains", searchTerm],
                            "or",
                            ["designatioN_NAME", "contains", searchTerm]
                        ]);
                    } else {
                        dataSource.filter(null);
                    }
                    dataSource.load();
                }

                function highlightText(container, value) {
                    if (!value) return;

                    const text = value.toString();
                    if (!currentSearchTerm || !text.toLowerCase().includes(currentSearchTerm.toLowerCase())) {
                        container.text(text);
                        return;
                    }

                    const regex = new RegExp(currentSearchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), "gi");
                    const highlighted = text.replace(regex, match =>
                        `<span style="background-color: #ffeb3b; font-weight: bold;">${match}</span>`
                    );
                    container.html(highlighted);
                }

                $(document).on("dxclick", "#ENAME .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#ACT_GROUP_hidden').val('');
                    $('#displayExprAccGroup').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });

                $("#ENAME").find(".dx-texteditor-input").off("mousedown.preventOpen").on("mousedown.preventOpen", function (e) {
                    var dropDown = $("#ENAME").dxDropDownBox("instance");
                    if (dropDown.option("opened")) return; 
                    e.stopPropagation();
                });

            }
        });

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

            var emp = mails[index];

            var emailModel = {
                To: emp.email,
                Subject: "Test Email Subject",
                Message: "This is a test mail"
            };

            console.log('emailModel', emailModel);

            ajaxHelper.ajaxPostJsonData(emailModel, "/MailBox/SendMail", function (data) {
                empr_helper.notify(data.data, data.msgType);

                if (data.msgType == 1) {
                    empr_HRInterviewFeedback.sendEmailsSequentially(mails, forType, index + 1);
                } else {
                    empr_HRInterviewFeedback.sendEmailsSequentially(mails, forType, index + 1);
                }
            }, false, true);
        }, 200);

    }
}