var empr_HRJobPost = {
    initEvents: function () {

        $(document).ready(function () {
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_HRJobPost.GetHRJobPostByID(data.traN_ID);
                }
            });
            //empr_HRJobPost.InitCardTypeDDL();

            empr_HRJobPost.InitDepartmentDDL();
            empr_HRJobPost.InitBranchDDL();
            empr_HRJobPost.InitEducationDDL();
            empr_HRJobPost.InitShiftDDL();
            empr_HRJobPost.resetForm();

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_HRJobPost.validateForm()) {
                            empr_HRJobPost.saveAttempt();
                            swal({
                                title: 'Want to send Mail?',
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
                                ajaxHelper.ajaxGetJson('/HRJobPost/GetEmpMails', function (data) {
                                    if (data.msgType == 1) {
                                        ajaxHelper.ajaxGetJson('/HRJobPost/GetEmpMails', function (data) {
                                            if (data.msgType == 1) {
                                                empr_HRJobPost.sendEmailsSequentially(data.data);
                                            } else {
                                                empr_helper.notify(data.msg, data.msgType);
                                            }
                                        }, false, true);
                                    }
                                    else {
                                        empr_helper.notify(data.msg, data.msgType);
                                    }
                                }, false, true);
                            });
                        }
                    }
                } else {
                    if (empr_HRJobPost.validateForm()) {
                        empr_HRJobPost.saveAttempt();
                        swal({
                            title: 'Do you want to send Email??',
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
                            ajaxHelper.ajaxGetJson('/HRJobPost/GetEmpMails', function (data) {
                                if (data.msgType == 1) {
                                    ajaxHelper.ajaxGetJson('/HRJobPost/GetEmpMails', function (data) {
                                        if (data.msgType == 1) {
                                            empr_HRJobPost.sendEmailsSequentially(data.data);
                                        } else {
                                            empr_helper.notify(data.msg, data.msgType);
                                        }
                                    }, false, true);
                                }
                                else {
                                    empr_helper.notify(data.msg, data.msgType);
                                }
                            }, false, true);
                        });
                    }
                }
            })

            $('#generateCode').click(function () {
                empr_HRJobPost.GenerateCardNo();
            })

            $('body').on('click', '#quicksearch', function () {
                empr_HRJobPost.InintQuickSearch();
            })

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_HRJobPost.GeneratePrintReport();
            });

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_HRJobPost.GetHRJobPostByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_HRJobPost.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_HRJobPost.DeleteRecord();
            })

            $('body').on('click', '#BtnSodaPick', function () {
                empr_HRJobPost.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_HRJobPost.AddSodaToDelivery();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_HRJobPost.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }
        });

    },
    InitShiftDDL: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "HRJobPost/GetShifts",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.code == _selectedValue }) || [];
                    if (selectedObject.length > 0) {

                        selectedValue = selectedObject[0].name;
                        $('#SHIFT_hidden').val(selectedObject[0].code);
                        $('#displayExprShift').val(selectedObject[0].name);

                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#JOB_TYPE").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "name",
                    displayExpr: function (item) {
                        return item ? `${item.name}` : "Select a value...";
                    },
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.name === e.value);
                            if (selectedData) {
                                $('#SHIFT_hidden').val(selectedData.code);
                                $('#displayExprShift').val(selectedData.name);

                            }
                        } else {
                            $('#SHIFT_hidden').val('');
                            $('#displayExprShift').val('');
                        }
                    },
                    onFocusIn: function (e) {
                        if (!isProgrammaticOpen) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 100);
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
                                    dataField: "name",
                                    caption: "Name",
                                    width: '60px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "timeIn",
                                    caption: "Time In",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "timeOut",
                                    caption: "Time Out",
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
                                    e.component.option("value", selected.name);
                                    e.component.close();

                                    // Update hidden and display fields
                                    $('#SHIFT_hidden').val(selected.code);
                                    $('#displayExprShift').val(selected.name);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].name], false);
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
                            ["name", "contains", searchTerm],
                            "or",
                            ["timeIn", "contains", searchTerm],
                            "or",
                            ["timeOut", "contains", searchTerm]
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

                // Handle clear button
                $(document).on("dxclick", "#InitShiftDDL .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#SHIFT_hidden').val('');
                    $('#displayExprShift').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
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
                    empr_HRJobPost.resetForm();
                    empr_HRJobPost.InitTree();
                    empr_HRJobPost.reFreshTree();
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
        /*var TRAN_ID = $("#Code").val();*/
        var V_DATE = $("#V_DATE").val();
        var JOB_TITLE = $("#JOB_TITLE").val();
        var DEPARTMENT = $('#DEPARTMENT').dxSelectBox('instance').option('value');
        var JOB_TYPE = $("#SHIFT_hidden").val();
        var EDUCATION = $('#EDUCATION').dxSelectBox('instance').option('value');
        var JOB_LOCATION = $("#JOB_LOCATION").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var MINIMUM_EXPERIENCE = $("#MINIMUM_EXPERIENCE").val().trim();
        var MAXIMUM_EXPERIENCE = $("#MAXIMUM_EXPERIENCE").val();
        var MINIMUM_SALARY = $("#MINIMUM_SALARY").val();
        var MAXIMUM_SALARY = $("#MAXIMUM_SALARY").val();
        var SKILL = $("#SKILL").val();
        var DUE_DATE = $("#DUE_DATE").val();
        var BCODE = $('#BCODE').dxSelectBox('instance').option('value');
        var JOB_DESCRIPTION = $("#JOB_DESCRIPTION").val();
        var DOCUMENT = $("#DOC").val();
        var JOB_RESPONSIBILITY = $("#JOB_RESPONSIBILITY").val();

        // Validation helper function
        function checkRequired(value, fieldName) {
            if (!value || value === "" || value === null) {
                empr_helper.notify("Please enter/select " + fieldName + ".", 2);
                valid = false;
            }
        }

        // Required validations
        checkRequired(TRAN_ID, "Transaction ID");
        checkRequired(V_DATE, "Date");
        checkRequired(JOB_TITLE, "Job Title");
        checkRequired(DEPARTMENT, "Department");
        checkRequired(JOB_TYPE, "Job Type");
        checkRequired(EDUCATION, "Education");
        checkRequired(JOB_LOCATION, "Job Location");
        checkRequired(ASTATUS, "Status");
        checkRequired(MINIMUM_EXPERIENCE, "Minimum Experience");
        checkRequired(MAXIMUM_EXPERIENCE, "Maximum Experience");
        checkRequired(MINIMUM_SALARY, "Minimum Salary");
        checkRequired(MAXIMUM_SALARY, "Maximum Salary");
        checkRequired(SKILL, "Skill");
        checkRequired(DUE_DATE, "Due Date");
        checkRequired(BCODE, "Branch Code");
        checkRequired(JOB_DESCRIPTION, "Job Description");
        checkRequired(DOCUMENT, "Document");
        checkRequired(JOB_RESPONSIBILITY, "Job Responsibility");

        return valid;
    },
    getDataToSave: function () {
        var TRAN_ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val()
        var JOB_TITLE = $("#JOB_TITLE").val()
        var DEPARTMENT = $('#DEPARTMENT').dxSelectBox('instance').option('value');
        var JOB_TYPE = $("#SHIFT_hidden").val();
        var EDUCATION = $('#EDUCATION').dxSelectBox('instance').option('value');
        var JOB_LOCATION = $("#JOB_LOCATION").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var MINIMUM_EXPERIENCE = $("#MINIMUM_EXPERIENCE").val().trim();
        var MAXIMUM_EXPERIENCE = $("#MAXIMUM_EXPERIENCE").val();
        var MINIMUM_SALARY = $("#MINIMUM_SALARY").val();
        var MAXIMUM_SALARY = $("#MAXIMUM_SALARY").val();
        var SKILL = $("#SKILL").val();
        var DUE_DATE = $("#DUE_DATE").val();
        var BCODE = $('#BCODE').dxSelectBox('instance').option('value');
        var JOB_DESCRIPTION = $("#JOB_DESCRIPTION").val();
        var DOCUMENT = $("#hdnDOC").val();
        var JOB_RESPONSIBILITY = $("#JOB_RESPONSIBILITY").val();
        var WEB_PUBLISH = $("#WEB_PUBLISH").is(":checked") ? 1 : 0;

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            V_DATE: V_DATE,
            JOB_TITLE: JOB_TITLE,
            DEPARTMENT: DEPARTMENT,
            JOB_TYPE: JOB_TYPE,
            EDUCATION: EDUCATION,
            JOB_LOCATION: JOB_LOCATION,
            ASTATUS: ASTATUS,
            MINIMUM_EXPERIENCE: MINIMUM_EXPERIENCE,
            MAXIMUM_EXPERIENCE: MAXIMUM_EXPERIENCE,
            MINIMUM_SALARY: MINIMUM_SALARY,
            MAXIMUM_SALARY: MAXIMUM_SALARY,
            SKILL: SKILL,
            DUE_DATE: DUE_DATE,
            BCODE: BCODE,
            JOB_DESCRIPTION: JOB_DESCRIPTION,
            DOCUMENT: DOCUMENT,
            JOB_RESPONSIBILITY: JOB_RESPONSIBILITY,
            WEB_PUBLISH: WEB_PUBLISH
        };
        return modelRecord;
    },
    saveAttempt: function () {
        var obj = empr_HRJobPost.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/HRJobPost/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#Code').val(data.data);
                $('#optmodal').modal('hide');
                $('.btn-delete').show();
                /*$('.btn-print').show();*/
                //empr_HRJobPost.resetForm();
                empr_HRJobPost.GetHRJobPostByID(data.data);
            }
        }, false, true);
    },
    resetForm: function () {
        // Text / input fields
        $("#Code").val('');
        /*$("#V_DATE").val('');*/
        $("#JOB_TITLE").val('');
        /*$("#JOB_TYPE").val('');*/
        $("#JOB_LOCATION").val('');
        $("#MINIMUM_EXPERIENCE").val('');
        $("#MAXIMUM_EXPERIENCE").val('');
        $("#MINIMUM_SALARY").val('');
        $("#MAXIMUM_SALARY").val('');
        $("#SKILL").val('');
        $("#DUE_DATE").val('');
        $("#JOB_DESCRIPTION").val('');
        $("#DOC").val('');
        $("#JOB_RESPONSIBILITY").val('');
        $("#WEB_PUBLISH").val('');

        // DevExtreme SelectBoxes
        $('#DEPARTMENT').dxSelectBox('instance').option('value', null);
        $('#EDUCATION').dxSelectBox('instance').option('value', null);
        /*$('#ASTATUS').dxSelectBox('instance').option('value', null);*/
        $('#BCODE').dxSelectBox('instance').option('value', null);
        /*$('#JOB_TYPE').dxDropDownBox('instance').option('value', null);*/
        $("#SHIFT_hidden").val('');

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
        empr_HRJobPost.GetQuickSearch();
    },
    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/HRJobPost/QuickSearch', function (data) {
            if (data.msgType == 1) {
                console.log(data.data)
                empr_HRJobPost.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetAllHRJobPosts: function () {

        var xhr = ajaxHelper.ajaxGetJson('/HRJobPost/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_HRJobPost.CreateGrid(data.data);

        }, false, true);

    },
    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },
    GetHRJobPostByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/HRJobPost/HRJobPostByid?id=' + id, function (data) {
            console.log('editData', data.data);
            //$('.btn-delete').show();
            //$('.btn-print').show();
            //$('#resetall').show();

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
                /*$('.btn-print').show();*/
                $('#resetall').show();
            }
            $('#generateCode').hide();

            $('.modal').modal('hide')

            $('#Code').val(data.data.traN_ID);
            $("#V_DATE").val(data.data.v_DATE);
            $("#JOB_TITLE").val(data.data.joB_TITLE);
            $("#JOB_LOCATION").val(data.data.joB_LOCATION);
            $("#MINIMUM_EXPERIENCE").val(data.data.miN_EXP);
            $("#MAXIMUM_EXPERIENCE").val(data.data.maX_EXP);
            $("#MINIMUM_SALARY").val(data.data.miN_SALARY);
            $("#MAXIMUM_SALARY").val(data.data.maX_SALARY);
            $("#SKILL").val(data.data.skill);
            $("#JOB_DESCRIPTION").val(data.data.joB_DESC);
            $("#JOB_RESPONSIBILITY").val(data.data.joB_RESP);
            $("#DUE_DATE").val(data.data.duE_DATE);
            //$("#DOC").val(data.data.doc);
            $("#hdnDOC").val(data.data.doc);
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.status);
            $('#EDUCATION').dxSelectBox('instance').option("value", data.data.education);
            empr_HRJobPost.InitDepartmentDDL(data.data.deP_ID);
            $('#BCODE').dxSelectBox('instance').option("value", data.data.bcode);
            empr_HRJobPost.InitShiftDDL(data.data.joB_TYPE);
            $("#SHIFT_hidden").val(data.data.joB_TYPE);
            if (data.data.weB_PUBLISH == '1') {
                $("#WEB_PUBLISH").prop("checked", true);
            } else {
                $("#WEB_PUBLISH").prop("checked", false);
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
                debugger

                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);

                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);


            }
        },
        { dataField: 'v_DATE', caption: 'Date' },
        { dataField: 'joB_TITLE', caption: 'Job Title' },
        { dataField: 'DeP_ID', caption: 'Department' },
        { dataField: 'joB_TYPE', caption: 'Job Type' },
        { dataField: 'joB_LOCATION', caption: 'Job Location' },
        { dataField: 'miN_EXP', caption: 'Minimum Experience' },
        { dataField: 'maX_EXP', caption: 'Maximum Experience' },
        { dataField: 'miN_SALARY', caption: 'Minimum Salary' },
        { dataField: 'maX_SALARY', caption: 'Maximum Salary' },
        { dataField: 'education', caption: 'Education' },
        { dataField: 'skill', caption: 'Skill' },
        { dataField: 'duE_DATE', caption: 'Due Date' },
        { dataField: 'bcode', caption: 'Branch' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "HRJobPostQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/HRJobPost/QuickSearchLazyLoading", "traN_ID", "HRJobPosts");
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
                        empr_HRJobPost.GetHRJobPostByID(clickedRowData.acT_CODE);
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
    InitCardTypeDDL: function (_selectedValue) {
        console.log(CardType);

        $('#CARD_TYPE').dxSelectBox({
            dataSource: CardType,
            displayExpr: "value",    // ✅ quotes mein hona chahiye
            valueExpr: "key",        // ✅ quotes mein hona chahiye
            value: _selectedValue,   // ✅ aapka function param
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
    InitDepartmentDDL: function (_selectedValue) {
        console.log(_selectedValue);
        $('#DEPARTMENT').dxSelectBox({
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
    InitBranchDDL: function (_selectedValue) {
        $('#BCODE').dxSelectBox({
            dataSource: Branch,
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
        empr_HRJobPost.InitReportTypeDDL();
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
    GenerateCardNo: function () {
        ajaxHelper.ajaxGetJson("/HRJobPost/GenerateCardNo", function (data) {
            $('#CARD_NO').val(data);
        }, false, true);
    },
    InitSodaPickGrid: function () {
        debugger
        var data = empr_HRJobPost.getDataToSave();
        console.log(data.S_DATE)
        if (data.S_DATE == "" || data.S_DATE == null || data.S_DATE == undefined) {
            empr_helper.notify("Please select the soda date first.", 2);
        }
        else {
            empr_HRJobPost.GetSodaBookFeedingDetailBySodaDate(data.S_DATE);
        }
    },
    GetSodaBookFeedingDetailBySodaDate: function (sodaDate) {
        debugger
        ajaxHelper.ajaxGetJson('/DeliveryFeeding/GetSodaBookFeedingDetailBySodaDate?sodaDate=' + sodaDate, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_HRJobPost.CreateSodaPickGrid(data.data);
                    $('#SodaPickModal').modal('show');
                } else {
                    empr_helper.notify("No soda found.", 2);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateSodaPickGrid: function (dataSrc) {
        debugger
        console.log(dataSrc)
        var col = [
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
            { dataField: 'seller', caption: 'Seller', allowEditing: false, },
            { dataField: 'buyer', caption: 'Buyer', allowEditing: false, },
            { dataField: 'broker', caption: 'Broker', allowEditing: false, },
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
            { dataField: 'unit', caption: 'Unit', allowEditing: false, visible: false },
            { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false },
            { dataField: 'qty', caption: 'S. QTY', allowEditing: false, },
            { dataField: 'dqty', caption: 'I. QTY', allowEditing: false, },
            { dataField: 'baL_QTY', caption: 'Balance Quantity', allowEditing: false, },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, },
            { dataField: 'amt', caption: 'Amount', allowEditing: false, },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "HRJobPostPick", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_HRJobPost.getDataToSave();
                var IsDataAvailableInGrid = false;

                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    $('#S_NO').val(selectedSodas[0].voucheR_NO);
                    $('#TBAG').val(selectedSodas[0].qty);
                    empr_HRJobPost.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].iteM_CODE);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
            if (selectedSodas.length > 0) {
                $('#S_NO').val(selectedSodas[0].voucheR_NO);
                $('#TBAG').val(selectedSodas[0].qty);
                empr_HRJobPost.InitDropdownsWithValue(selectedSodas[0].buyeR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].iteM_CODE, selectedSodas[0].unit);
            }
            $('.modal').hide();
            $('#V_DATE').focus();
        }
    },
    InitUnitDDL: function (_selectedValue) {

        $.ajax({
            url: 'HRJobPost/GetUnits',
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

                empr_HRJobPost.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
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
                    empr_HRJobPost.sendEmailsSequentially(employees, index + 1);
                } else {
                    empr_HRJobPost.sendEmailsSequentially(employees, index + 1);
                }
            }, false, true);
        }, 200);
        
    }
}