var empr_EmpMasterInfo = {
    TableName: '',
    MasterId: 0 ,
    initEvents: function () {

        $(document).ready(function () {

            //empr_EmpMasterInfo.InitQuickSearch();
            empr_EmpMasterInfo.InitDDLEducation();
            empr_EmpMasterInfo.InitAccountGroupGridBox();
            empr_EmpMasterInfo.InitHRTables();
            empr_EmpMasterInfo.FieldHideAndShow('');

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_EmpMasterInfo.validateForm()) {
                            empr_EmpMasterInfo.saveAttempt(empr_EmpMasterInfo.TableName);
                        }
                    }
                } else {
                    if (empr_EmpMasterInfo.validateForm()) {
                        empr_EmpMasterInfo.saveAttempt(empr_EmpMasterInfo.TableName);
                    }
                }
            });

            $('#BtnNew').click(function () {
                debugger;
                if (empr_EmpMasterInfo.validateForm()) {
                    debugger;
                    empr_EmpMasterInfo.FieldHideAndShow(empr_EmpMasterInfo.TableName);
                    $("#ENAME").dxDropDownBox("instance").option("disabled", true);
                    $('#BtnDelete').hide();
                        $('#BtnNew').hide();
                        empr_EmpMasterInfo.resetForm();
                }
            });

            $('#resetForm').click(function () {
                debugger;
                empr_EmpMasterInfo.resetForm();

            });

            $('#refresh').click(function () {
                empr_EmpMasterInfo.FieldHideAndShow('');
                empr_EmpMasterInfo.resetForm();
                $('#BtnNew').show();
                $('#ENAME').dxDropDownBox("instance").option("disabled", false);
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_EmpMasterInfo.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_EmpMasterInfo.GetEmpMasterInfoByID(reportid, empr_EmpMasterInfo.TableName);
            });

            //$('body').on('click', '#BtnNew', function () {
            //    $('#BtnDelete').hide();
            //    $('#BtnNew').hide();
            //    empr_EmpMasterInfo.resetForm();
            //});

            $('#BtnDelete').click(function () {
                empr_EmpMasterInfo.DeleteRecord();
            });

            // When user changes AnnualPct
            $("#AnnualPct").change(function () {
                var grossSalary = parseFloat($("#GSalary").val()) || 0;
                var annualPct = parseFloat($(this).val()) || 0;

                var annualAmt = Math.round((grossSalary * annualPct) / 100);
                $("#Annualamt").val(annualAmt);
            });

            // When user changes Annualamt and AnnualPct is blank
            $("#Annualamt").change(function () {
                var grossSalary = parseFloat($("#GSalary").val()) || 0;
                var annualAmt = parseFloat($(this).val()) || 0;

                var annualPct = 0;
                if (grossSalary > 0) {
                    annualPct = Math.round((annualAmt / grossSalary) * 100);
                }
                $("#AnnualPct").val(annualPct);
                
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val(), TableName: empr_EmpMasterInfo.TableName }, "/EmpMasterInfo/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_EmpMasterInfo.resetForm();
                    empr_EmpMasterInfo.InitQuickSearch(empr_EmpMasterInfo.TableName);
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $('#Code').val('');
        $('#StartD').val('');
        $('#EndD').val('');
        $('#CName').val('');
        $('#DecStart').val('');
        $('#DecEnd').val('');
        $('#InitialSalary').val('');
        $('#FinalSalary').val('');
        $('#CellNo').val('');
        $('#LReason').val('');
        $('#WADD').val('');
        $('#MSubject').val('');
        $('#EYear').val('');
        $('#GBatch').val('');
        $('#Institute').val('');
        $('#G_CGPA').val('');
        $('#EduDoc').val('');
        $('#WDate').val('');
        $('#Title').val('');
        $('#WorkRemark').val('');
        $('#WorkDoc').val('');
        $('#BasicSalary').val('');
        $('#HouseRent').val('');
        $('#UTILITY').val('');
        $('#COLA').val('');
        $('#EFFDate').val('');
        $('#EmpRemark').val('');
        $('#EmpDoc').val('');
        $("#EducatioID").dxSelectBox('instance').reset();
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

    InitAccountGroupGridBox: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "EmpMasterInfo/GetEmployeeGroups",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;
                console.log(Datasource);

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
                debugger;
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
                            const selectedData = gridInstance.getDataSource().items().find(item => item.code === e.value);
                            if (selectedData) {
                                $('#ACT_GROUP_hidden').val(selectedData.code);
                                $('#displayExprAccGroup').val(selectedData.ename);
                            }
                        } else {
                            $('#ACT_GROUP_hidden').val('');
                            $('#displayExprAccGroup').val('');
                        }
                    },
                    //onFocusIn: function (e) {
                    //    if (!isProgrammaticOpen) {
                    //        isProgrammaticOpen = true;
                    //        e.component.open();
                    //        setTimeout(() => { isProgrammaticOpen = false; }, 100);
                    //    }
                    //},
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
                                    caption: "Emp ID",
                                    width: '60px',
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

                                    // Update hidden and display fields
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

                // Handle clear button
                $(document).on("dxclick", "#ENAME .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#ACT_GROUP_hidden').val('');
                    $('#displayExprAccGroup').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
            }
        });
    },

    validateForm: function () {
        debugger;
        var valid = true;
        var Ename = $("#ENAME").dxDropDownBox('instance').option('value');
        var Tname = $("#TNAME").dxSelectBox('instance').option('value')

        if (Ename == '' || Ename == undefined || Ename == null) {
            empr_helper.notify("Employee Name is required.", 2);
            valid = false;
        }
        if (Tname == '' || Tname == undefined || Tname == null) {
            empr_helper.notify("Experiensce is required.", 2);
            valid = false;
        }
        

        return valid;
    },

    GetDataToSave: function (TableName) {

        var modelRecord = {
            GROUP_CODE: parseInt($("#Code").val()) || null,
            START_D: $('#StartD').val(),
            END_D: $('#EndD').val(),
            CompanyName: $('#CName').val(),
            DESC_S: $('#DecStart').val(),
            DESC_E: $('#DecEnd').val(),
            InitialSalary: parseInt($('#InitialSalary').val()) || null,
            FinalSalary: parseInt($('#FinalSalary').val()) || null,
            CellNo: $('#CellNo').val(),
            LREASON: $('#LReason').val(),
            WADD: $('#WADD').val(),
            MSubject: $('#MSubject').val(),
            EYear: $('#EYear').val(),              // FIXED: previously WADD
            GBatch: $('#GBatch').val(),
            Institute: $('#Institute').val(),        // make sure this input exists
            GCGPA: $('#G_CGPA').val(),
            EduDoc: $('#EduDoc').val(),
            EducationID: parseInt($("#EducatioID").dxSelectBox('instance').option('value')) || null,
            WDate: $('#WDate').val(),
            Title: $('#Title').val(),
            WorkRemark: $('#WorkRemark').val(),
            WorkDoc: $('#WorkDoc').val(),
            BasicSalary: parseInt($('#BasicSalary').val()) || null,
            HouseRent: parseInt($('#HouseRent').val()) || null,
            Utility: $('#UTILITY').val(),
            Cola: $('#COLA').val(),
            EFFDate: $('#EFFDate').val(),
            EmpRemark: $('#EmpRemark').val(),
            EmpDoc: $('#EmpDoc').val(),
            TableName: TableName,
            MasterId: empr_EmpMasterInfo.MasterId,
            ASTATUS: $("#ASTATUS").dxSelectBox('instance').option('value'),
            EMP_ID: $("#ENAME").dxDropDownBox('instance').option('value')
        };

        return modelRecord;
    },
    saveAttempt: function (TableName) {
        debugger;
        var obj = empr_EmpMasterInfo.GetDataToSave(TableName);

        ajaxHelper.ajaxPostJsonData(obj, "/EmpMasterInfo/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_EmpMasterInfo.resetForm();
                empr_EmpMasterInfo.InitQuickSearch(empr_EmpMasterInfo.TableName);
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function (TableName) {
        empr_EmpMasterInfo.GetAllEmpMasterInfos(TableName);
    },
    InitHRTables: function (selectedValue) {
        console.log(EmpMasterInfoTables);
        $('#TNAME').dxSelectBox({
            dataSource: EmpMasterInfoTables,
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
                var selectedKey = e.value;
                var selectedItem = EmpMasterInfoTables.find(x => x.key === selectedKey);
                empr_EmpMasterInfo.TableName = selectedItem.tname;
                empr_EmpMasterInfo.MasterId =  selectedItem.key;
            }
        });
    },
    InitDDLEducation: function (selectedValue) {
        console.log(EducationRecords);
        $('#EducatioID').dxSelectBox({
            dataSource: EducationRecords,
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
            //onValueChanged: function (e) {
            //    var selectedKey = e.value;
            //    var selectedItem = EmpMasterInfoTables.find(x => x.key === selectedKey);
            //    empr_EmpMasterInfo.TableName = selectedItem.tname;
            //    empr_EmpMasterInfo.MasterId =  selectedItem.key;
            //}
        });
    },
    FieldHideAndShow(TableName) {
        // Hide all fields first
        $('.col-xl-2').hide();
        $('.col-xl-3').hide();
        $('.col-xl-4').hide();
        $('#refresh').show();
        // Show fields based on selected table
        switch (TableName) {
            case 'TBL_WORK_EXP':
                $('#StartDs').show();
                $('#EndDs').show();
                $('#CNames').show();
                $('#DecStarts').show();
                $('#DecEnds').show();
                $('#InitialSalarys').show();
                $('#FinalSalarys').show();
                $('#CellNos').show();
                $('#LReasons').show();
                $('#WADDs').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_EmpMasterInfo.InitQuickSearch(TableName);
                break;
            case 'TBL_EMP_EDUCATION':
                $('#EducatioIDs').show();
                $('#MSubjects').show();
                $('#EYears').show();
                $('#GBatchs').show();
                $('#Institutes').show();
                $('#G_CGPAS').show();
                $('#EduDocs').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_EmpMasterInfo.InitQuickSearch(TableName);
                break;
            case 'TBL_EMP_WORKSHOP':
                $('#WDates').show();
                $('#Titles').show();
                $('#WorkRemarks').show();
                $('#WorkDocs').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_EmpMasterInfo.InitQuickSearch(TableName);
                break;
            case 'TBL_EMP_SALARY_HISTORY':
                $('#BasicSalarys').show();
                $('#HouseRents').show();
                $('#UTILITYS').show();
                $('#COLAS').show();
                $('#EFFDates').show();
                $('#EmpRemarks').show();
                $('#EmpDocs').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_EmpMasterInfo.InitQuickSearch(TableName);
                break;
            default:
                $('#BtnSave').hide();
                $('.card-body').hide();
                break;
        }
    },
    GetAllEmpMasterInfos: function (TableName) {
        ajaxHelper.ajaxGetJson('/EmpMasterInfo/QuickSearch?TableName=' + TableName, function (data) {
            empr_EmpMasterInfo.CreateGrid(data.data, TableName);
        }, false, true);
    },
    GetEmpMasterInfoByID: function (id, tableName) {
        ajaxHelper.ajaxGetJson('/EmpMasterInfo/GetEmpMasterInfoByID?id=' + id + '&TableName=' + tableName, function (data) {
            empr_EmpMasterInfo.resetForm();
            if (data.msgType == 1) {

                var record = data.data[0];
                console.log(record);
                $("#Code").val(record.grouP_CODE);
                $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
                $('#StartD').val(record.starT_D ?? '');
                $('#EndD').val(record.enD_D ?? '');
                $('#CName').val(record.companyName ?? '');
                $('#DecStart').val(record.desC_S ?? '');
                $('#DecEnd').val(record.desC_E ?? '');
                $('#InitialSalary').val(record.initialSalary ?? '');
                $('#FinalSalary').val(record.finalSalary ?? '');
                $('#CellNo').val(record.cellNo ?? '');
                $('#LReason').val(record.lreason ?? '');
                $('#WADD').val(record.wadd ?? '');
                $('#MSubject').val(record.mSubject ?? '');
                $('#EYear').val(record.eYear ?? '');
                $('#GBatch').val(record.gBatch ?? '');
                $('#Institute').val(record.institute ?? '');
                $('#G_CGPA').val(record.gcgpa ?? '');
                $('#EduDoc').val(record.eduDoc ?? '');
                $('#WDate').val(record.wDate ?? '');
                $('#Title').val(record.title ?? '');
                $('#WorkRemark').val(record.workRemark ?? '');
                $('#WorkDoc').val(record.workDoc ?? '');
                $('#BasicSalary').val(record.basicSalary ?? '');
                $('#HouseRent').val(record.houseRent ?? '');
                $('#UTILITY').val(record.utility ?? '');
                $('#COLA').val(record.cola ?? '');
                $('#EFFDate').val(record.effDate ?? '');
                $('#EmpRemark').val(record.empRemark ?? '');
                $('#EmpDoc').val(record.empDoc ?? '');
                $('#EducatioID').dxSelectBox('instance').option('value', record.educationID);
                empr_EmpMasterInfo.InitAccountGroupGridBox(record.emP_ID);

                $('.modal').modal('hide');

                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
                    }
                    if (Permissions.r_ADD) {
                        //$('#BtnNew').show();
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
                    //$('#BtnNew').show();
                }
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc, TableName) {
        console.log(dataSrc);
        if (TableName == 'TBL_WORK_EXP') {
            var col = [
                {
                    dataField: "Action",
                    width: 100,
                    alignment: 'center',
                    fixed: true,
                    fixedPosition: "left",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger
                        var html = '<div class="btn-group btn-group-sm">';
                        html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                        html += '</div>';
                        $(html).appendTo(container);
                    }
                },
                { dataField: 'grouP_CODE', caption: 'Code', sortOrder: 'desc' },
                { dataField: 'starT_D', caption: 'Start Date' },
                { dataField: 'enD_D', caption: 'End Date' },
                { dataField: 'companyName', caption: 'Company Name' },
                { dataField: 'desC_S', caption: 'Joining Description' },
                { dataField: 'desC_E', caption: 'Left Description' },
                { dataField: 'initialSalary', caption: 'Initial Salary' },
                { dataField: 'finalSalary', caption: 'Final Salary' },
                { dataField: 'cellNo', caption: 'Cell No' },
                { dataField: 'wadd', caption: 'Work Address' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        else if (TableName == 'TBL_EMP_EDUCATION') {
            var col = [
                {
                    dataField: "Action",
                    width: 100,
                    alignment: 'center',
                    fixed: true,
                    fixedPosition: "left",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger
                        var html = '<div class="btn-group btn-group-sm">';
                        html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                        html += '</div>';
                        $(html).appendTo(container);
                    }
                },
                { dataField: 'grouP_CODE', caption: 'Code', sortOrder: 'desc' },
                { dataField: 'educationID', caption: 'Eduction' },
                { dataField: 'mSubject', caption: 'Major Subject' },
                { dataField: 'eYear', caption: 'Ending Year' },
                { dataField: 'gBatch', caption: 'Group Batch' },
                { dataField: 'institute', caption: 'Institute' },
                { dataField: 'gcgpa', caption: 'Grande / CGPA' },
                { dataField: 'eduDoc', caption: 'Document' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        else if (TableName == 'TBL_EMP_WORKSHOP') {
            var col = [
                {
                    dataField: "Action",
                    width: 100,
                    alignment: 'center',
                    fixed: true,
                    fixedPosition: "left",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger
                        var html = '<div class="btn-group btn-group-sm">';
                        html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                        html += '</div>';
                        $(html).appendTo(container);
                    }
                },
                { dataField: 'grouP_CODE', caption: 'Code', sortOrder: 'desc' },
                { dataField: 'wDate', caption: 'Work Date' },
                { dataField: 'title', caption: 'Title' },
                { dataField: 'workRemark', caption: 'Remarks' },
                { dataField: 'workDoc', caption: 'Document' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        else if (TableName == 'TBL_EMP_SALARY_HISTORY') {
            var col = [
                {
                    dataField: "Action",
                    width: 100,
                    alignment: 'center',
                    fixed: true,
                    fixedPosition: "left",
                    allowExporting: false,
                    cellTemplate: function (container, options) {
                        debugger
                        var html = '<div class="btn-group btn-group-sm">';
                        html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                        html += '</div>';
                        $(html).appendTo(container);
                    }
                },
                { dataField: 'grouP_CODE', caption: 'Code', sortOrder: 'desc' },
                { dataField: 'basicSalary', caption: 'Basic Salary' },
                { dataField: 'houseRent', caption: 'House Rent' },
                { dataField: 'utility', caption: 'UTILITY' },
                { dataField: 'effDate', caption: 'EFF Date' },
                { dataField: 'empRemark', caption: 'Remarks' },
                { dataField: 'empDoc', caption: 'Document' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "EmpMasterInfo" , 'single');
    },
}