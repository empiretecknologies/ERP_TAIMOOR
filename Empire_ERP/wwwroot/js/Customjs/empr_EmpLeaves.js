var empr_EmpLeaves = {
    TableName: '',
    MasterId: 0,
    rowsCount: 0,
    initEvents: function () {

        $(document).ready(function () {

            //empr_EmpLeaves.InitQuickSearch();

            /*empr_EmpLeaves.InitDDLEducation();*/
            empr_EmpLeaves.InitAccountGroupGridBox();
            /*empr_EmpLeaves.InitHRTables();*/
            empr_EmpLeaves.FieldHideAndShow('');
            empr_EmpLeaves.resetForm();


            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_EmpLeaves.validateForm()) {
                            empr_EmpLeaves.saveAttempt();
                        }
                    }
                } else {
                    if (empr_EmpLeaves.validateForm()) {
                        empr_EmpLeaves.saveAttempt();
                    }
                }
            });

            $('#BtnNew').click(function () {
                debugger;
                $("#ENAME").dxDropDownBox("instance").option("disabled", true);
                $('.Record').show();
                //empr_EmpLeaves.CreateGrid([{ priority: 'N' }]);
                empr_EmpLeaves.InitQuickSearchGrid();
            });

            $('#resetForm').click(function () {
                debugger;
                empr_EmpLeaves.resetForm();

            });

            $('#refresh').click(function () {
                empr_EmpLeaves.FieldHideAndShow('');
                empr_EmpLeaves.resetForm();
                $('#BtnNew').show();
                $('#ENAME').dxDropDownBox("instance").option("disabled", false);
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_EmpLeaves.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_EmpLeaves.GetEmpLeavesByID(reportid, empr_EmpLeaves.TableName);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_EmpLeaves.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_EmpLeaves.DeleteRecord();
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

    InitQuickSearchGrid: function () {
        empr_EmpLeaves.GetPurchaseOrders();
    },

    GetPurchaseOrders: function () {
        debugger;
        var EmpId = parseInt($("#ENAME").dxDropDownBox("instance").option("value")) || 0;
        ajaxHelper.ajaxGetJson('/EmpLeaves/GetEmpLeavesRecord?id=' + EmpId, function (data) {
            console.log(data);
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    empr_EmpLeaves.CreateGrid(data.data);
                }
                else {
                    empr_EmpLeaves.CreateGrid([{ priority: 'N' }]);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_EmpLeaves.rowsCount = dataSrc.length - 1;

        }

        var col = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    $(`<div class="btn-group btn-group-sm">
                            <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_EmpLeaves.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                            <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_EmpLeaves.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                        </div>`).appendTo(container);
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false
            },
            {
                dataField: 'date',
                caption: 'Date',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy',
                validationRules: [
                    { type: "required", message: "Date is required" }
                ]
            },
            {
                dataField: 'leavE_TYPE',
                caption: 'Bank Name',
                width: 200,
                allowSorting: false,
                lookup: {
                    dataSource: {
                        store: LeaveTypes,
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    showClearButton: true,
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    }
                },
                validationRules: [
                    { type: "required", message: "Leave Type is required" }
                ]
            },
            {
                dataField: 'lfrom',
                caption: 'From Date',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy',
                validationRules: [
                    { type: "required", message: "From Date is required" }
                ]
            },
            {
                dataField: 'lto',
                caption: 'To Date',
                dataType: 'date',
                width: 100,
                format: 'dd-MM-yyyy',
                validationRules: [
                    { type: "required", message: "To Date is required" },
                    {
                        type: "custom",
                        validationCallback: function (e) {
                            const from = e.data.lFrom;
                            const to = e.value;
                            return !from || !to || new Date(to) >= new Date(from);
                        },
                        message: "'To Date' must be greater than or equal to 'From Date'"
                    }
                ]
            },
            {
                dataField: 'nol',
                caption: '# Of Leaves',
                width: 70,
                allowEditing: false,
                dataType: "number",
                calculateCellValue: function (rowData) {
                    if (rowData.lfrom && rowData.lto) {
                        let fromDate = new Date(rowData.lfrom);
                        let toDate = new Date(rowData.lto);

                        // Normalize both dates to remove time
                        fromDate.setHours(0, 0, 0, 0);
                        toDate.setHours(0, 0, 0, 0);

                        let diffTime = toDate - fromDate;
                        let diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24)) + 1;

                        // Set the value in the row for saving
                        rowData.nol = diffDays > 0 ? diffDays : 0;

                        return rowData.nol;
                    }

                    // Ensure nol is zero if dates not provided
                    rowData.nol = 0;
                    return 0;
                }
            },
            {
                dataField: 'purpose',
                caption: 'Purpose',
                validationRules: [
                    { type: "required", message: "Purpose is required" }
                ]
            },
            {
                dataField: 'doc',
                caption: 'Original Doc',
                //width: 250,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    const inputGroup = $('<div>').addClass('input-group');

                    const fileInput = $('<input>')
                        .attr({
                            type: 'file',
                            accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*'
                        })
                        .addClass('form-control')
                        .css({ "display": "block" });


                    fileInput.on('change', function (event) {
                        const file = event.target.files[0];

                        if (file) {
                            let formData = new FormData();
                            formData.append('model', file, file.name);

                            $.ajax({
                                url: '/EmpLeaves/SaveImage',
                                data: formData,
                                processData: false,
                                contentType: false,
                                type: "POST",
                                success: function (data) {
                                    if (data.msgType == '1') {
                                        let grid = options.component;
                                        let rowIndex = options.rowIndex;
                                        let dataSource = grid.option("dataSource");

                                        dataSource[rowIndex].doc = data.data;
                                        grid.repaint(); // Ensures the UI updates correctly
                                    } else {
                                        console.error("Upload Error:", data);
                                        empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                                    }
                                },
                                error: function (error) {
                                    console.error("File upload failed:", error);
                                    empr_helper.notify("File upload failed. Please try again.", 2);
                                }
                            });
                        }
                    });

                    inputGroup.append(fileInput);
                    if (options.data.doc != null && options.data.doc !== '') {
                        const viewButton = $('<div>')
                            .addClass('input-group-append')
                            .append(
                                $('<a>')
                                    .attr('href', 'javascript:;')
                                    .addClass('input-group-text')
                                    .on('click', function () {
                                        const fileUrl = options.data.doc;
                                        if (fileUrl) {
                                            window.open(fileUrl, '_blank');
                                        } else {
                                            alert('No document available to view.');
                                        }
                                    })
                                    .append($('<i>').addClass('fa fa-eye'))
                            );

                        inputGroup.append(viewButton);
                    }
                    $(container).append(inputGroup);
                }
            },
            {
                dataField: 'emP_ID',
                caption: 'Employee Id',
                visible: false,
                calculateCellValue: function (rowData) {

                    rowData.emP_ID = $('#ACT_GROUP_hidden').val();

                    return rowData.emP_ID;
                }
            }

        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "BankDetails", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        setTimeout(function () {
            var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
            $('#DetailContainer').dxDataGrid('instance').focus(nextElement);
        }, 1500);
    },



    GenerateKey: function (keyLength) {

        var key = "";
        var characters = "abcdef0123456789";
        for (var i = 0; i < keyLength; i++) {
            if (i === 8 || i === 13 || i === 18 || i === 23) {
                key += "-";
            } else {
                key += characters.charAt(Math.floor(Math.random() * characters.length));
            }
        }
        return key;
    },

    DeleteRow: function (index, traN_ID) {
        debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            var row = dataSource[index];
            if (traN_ID == '' || traN_ID == null || traN_ID == undefined) {
                gridInstance.deleteRow(index);
                empr_EmpLeaves.rowsCount -= 1;
                gridInstance.saveEditData();
            }
            else {
                var availableRows = dataSource.filter(x => x.traN_ID > 0);
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
                    ajaxHelper.ajaxGetJson('/EmpLeaves/Delete?id=' + traN_ID, function (data) {
                        empr_helper.notify(data.msg, data.msgType);
                        if (data.msgType == 1) {
                            gridInstance.deleteRow(index);
                            empr_EmpLeaves.rowsCount -= 1;
                            gridInstance.saveEditData();
                        }
                    }, false, true);
                });
            }

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val(), TableName: empr_EmpLeaves.TableName }, "/EmpLeaves/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_EmpLeaves.resetForm();
                    empr_EmpLeaves.InitQuickSearch(empr_EmpLeaves.TableName);
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {

       
        //empr_EmpLeaves.CreateGrid([{ priority: 'N' }]);
        //$('#Code').val('');
        //$('#StartD').val('');
        //$('#EndD').val('');
        //$('#CName').val('');
        //$('#DecStart').val('');
        //$('#DecEnd').val('');
        //$('#InitialSalary').val('');
        //$('#FinalSalary').val('');
        //$('#CellNo').val('');
        //$('#LReason').val('');
        //$('#WADD').val('');
        //$('#MSubject').val('');
        //$('#EYear').val('');
        //$('#GBatch').val('');
        //$('#Institute').val('');
        //$('#G_CGPA').val('');
        //$('#EduDoc').val('');
        //$('#WDate').val('');
        //$('#Title').val('');
        //$('#WorkRemark').val('');
        //$('#WorkDoc').val('');
        //$('#BasicSalary').val('');
        //$('#HouseRent').val('');
        //$('#UTILITY').val('');
        //$('#COLA').val('');
        //$('#EFFDate').val('');
        //$('#EmpRemark').val('');
        //$('#EmpDoc').val('');
        //$("#EducatioID").dxSelectBox('instance').reset();
        //$('#ASTATUS').dxSelectBox('instance').option('value', "Y");


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
            url: "EmpLeaves/GetEmployeeGroups",
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

                $("#ENAME").find(".dx-texteditor-input").off("mousedown.preventOpen").on("mousedown.preventOpen", function (e) {
                    var dropDown = $("#ENAME").dxDropDownBox("instance");
                    if (dropDown.option("opened")) return; // agar already open hai to chhodo
                    e.stopPropagation(); // click event ko propagate hone se roko
                });

            }
        });

    },

    validateForm: function () {

        var valid = true;

        //function notifyIfEmpty(selector, message) {
        //    if ($(selector).val().trim() === '') {
        //        empr_helper.notify(message, 2);
        //        valid = false;
        //    }
        //}

        //notifyIfEmpty("#Code", "Please enter Group Code.");
        //notifyIfEmpty("#StartD", "Please enter Start Date.");
        //notifyIfEmpty("#EndD", "Please enter End Date.");
        //notifyIfEmpty("#CName", "Please enter Company Name.");
        //notifyIfEmpty("#DecStart", "Please enter Description Start.");
        //notifyIfEmpty("#DecEnd", "Please enter Description End.");
        //notifyIfEmpty("#InitialSalary", "Please enter Initial Salary.");
        //notifyIfEmpty("#FinalSalary", "Please enter Final Salary.");
        //notifyIfEmpty("#CellNos", "Please enter Cell Number.");
        //notifyIfEmpty("#LReason", "Please enter Leaving Reason.");
        //notifyIfEmpty("#WADD", "Please enter Work Address.");
        //notifyIfEmpty("#MSubject", "Please enter Major Subject.");
        //notifyIfEmpty("#EndYear", "Please enter End Year.");
        //notifyIfEmpty("#GBatch", "Please enter Batch.");
        //notifyIfEmpty("#Institute", "Please enter Institute.");
        //notifyIfEmpty("#G_CGPA", "Please enter CGPA.");
        //notifyIfEmpty("#EduDoc", "Please attach Education Document.");

        //var eduVal = $("#EducatioID").dxSelectBox('instance').option('value');
        //if (!eduVal) {
        //    empr_helper.notify("Please select Education.", 2);
        //    valid = false;
        //}

        //notifyIfEmpty("#WDate", "Please enter Work Date.");
        //notifyIfEmpty("#Title", "Please enter Job Title.");
        //notifyIfEmpty("#WorkRemark", "Please enter Work Remarks.");
        //notifyIfEmpty("#WorkDoc", "Please attach Work Document.");
        //notifyIfEmpty("#BasicSalary", "Please enter Basic Salary.");
        //notifyIfEmpty("#HouseRents", "Please enter House Rent.");
        //notifyIfEmpty("#UTILITY", "Please enter Utility.");
        //notifyIfEmpty("#COLA", "Please enter COLA.");
        //notifyIfEmpty("#EFFDate", "Please enter Effective Date.");
        //notifyIfEmpty("#EmpRemark", "Please enter Employee Remarks.");
        //notifyIfEmpty("#EmpDoc", "Please attach Employee Document.");

        //var statusVal = $("#ASTATUS").dxSelectBox('instance').option('value');
        //if (!statusVal) {
        //    empr_helper.notify("Please select Application Status.", 2);
        //    valid = false;
        //}

        return valid;
    },

    GetDataToSave: function () {
        debugger;
        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData();
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }
        detailRecords.forEach(x => {
            x.Date = formatDate(x.Date);
            x.lFrom = formatDate(x.lFrom);
            x.lTo = formatDate(x.lTo);
        });

        function formatDate(date) {
            if (!date) return null;
            const d = new Date(date);
            return `${d.getDate().toString().padStart(2, '0')}-${(d.getMonth() + 1).toString().padStart(2, '0')}-${d.getFullYear()}`;
        }

        var modelRecord = {
            Master: detailRecords
        };
        return modelRecord;
    },
    saveAttempt: function () {
        debugger;
        var obj = empr_EmpLeaves.GetDataToSave();
        console.log(obj);
        ajaxHelper.ajaxPostJsonData(obj, "/EmpLeaves/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_EmpLeaves.resetForm();
                empr_EmpLeaves.InitQuickSearch(empr_EmpLeaves.TableName);
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function (TableName) {
        empr_EmpLeaves.GetAllEmpLeavess(TableName);
    },
    InitHRTables: function (selectedValue) {
        console.log(EmpLeavesTables);
        $('#TNAME').dxSelectBox({
            dataSource: EmpLeavesTables,
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
                var selectedItem = EmpLeavesTables.find(x => x.key === selectedKey);
                empr_EmpLeaves.TableName = selectedItem.tname;
                empr_EmpLeaves.MasterId = selectedItem.key;
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
            //    var selectedItem = EmpLeavesTables.find(x => x.key === selectedKey);
            //    empr_EmpLeaves.TableName = selectedItem.tname;
            //    empr_EmpLeaves.MasterId =  selectedItem.key;
            //}
        });
    },
    FieldHideAndShow(TableName) {
        // Hide all fields first
        $('.Record').hide();
        $('#BtnSave').hide();

        $('#refresh').show();
    },
    GetAllEmpLeavess: function (TableName) {
        ajaxHelper.ajaxGetJson('/EmpLeaves/QuickSearch?TableName=' + TableName, function (data) {
            empr_EmpLeaves.CreateGrid(data.data, TableName);
        }, false, true);
    },
    GetEmpLeavesByID: function (id, tableName) {
        ajaxHelper.ajaxGetJson('/EmpLeaves/GetEmpLeavesByID?id=' + id + '&TableName=' + tableName, function (data) {
            empr_EmpLeaves.resetForm();
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
                empr_EmpLeaves.InitAccountGroupGridBox(record.emP_ID);

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
    AddRow: function () {
        debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        //if (dataSrc.length >= Limit && Limit != 0) {
        //    empr_helper.notify("You can only add  " + Limit + " records.", 2);
        //    return;
        //}
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_EmpLeaves.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_EmpLeaves.GenerateKey(36), priority: 'N' });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_BankDetail.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_EmpLeaves.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_EmpLeaves.GenerateKey(36), priority: 'N' });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
}