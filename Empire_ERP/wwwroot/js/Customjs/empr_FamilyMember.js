var empr_FamilyMember = {
    TotalTime: 120000,

    initEvents() {
        $(document).ready(function () {

            empr_FamilyMember.InitEmployeeGridBox();
            empr_FamilyMember.InitMaritalStatusDDL();
            empr_FamilyMember.InitGenderDDL();
            empr_FamilyMember.InitRelationDDL();

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                empr_FamilyMember.resetForm();
            });

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_FamilyMember.validateForm()) {
                            empr_FamilyMember.saveAttempt();
                        }
                    }
                } else {
                    if (empr_FamilyMember.validateForm()) {
                        empr_FamilyMember.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_FamilyMember.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_FamilyMember.GetFamilyMemberByID(reportid);
            });

            $('#BtnDelete').click(function () {
                empr_FamilyMember.DeleteRecord();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
                !Permissions.r_ADD && $('#BtnNew').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },

    DeleteRecord() {

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/FamilyMember/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_FamilyMember.AddNew();
                    empr_FamilyMember.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });

    },

    resetForm() {
        $("#Code").val('');
        $("#Employee_hidden").val('');
        $("#displayExprEmployee").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $('#RELATION').dxSelectBox('instance').option('value', null);
        $("#F_NAME").val('');
        $("#DOB").val('');
        $('#GENDER').dxSelectBox('instance').option('value', null);
        $('#MSTATUS').dxSelectBox('instance').option('value', null);
        $("#CNIC").val('');
        $("#CNIC_EXP").val('');
        $("#INSTITUTE").val('');
        $("#EDUCATION").val('');
        $("#REMARKS").val('');
        empr_FamilyMember.InitEmployeeGridBox();
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

    AddNew() {
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0];
        $("#branchhidden").val('');
        $("#rolehidden").val('');
        $("#Code").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $("#FULLNAME").val('');
        $("#USERNAME").val('');
        $("#EMAIL").val('');
        $("#CELL_NO").val('');
        $('#ROLE').dxSelectBox('instance').option('value', null);
        $('#BRANCH').dxSelectBox('instance').option('value', null);
        $("#USTART_DATE").val(formattedDate);
        $("#ESTART_DATE").val('');
        $("#Image").val('');
        $("#GROUP_PIC").val('');
        $("#UPASS").val('');
        $("#CPASS").val('');
        $("#USERNAME").val('').prop('readonly', false);
        $("#EMAIL").val('').prop('readonly', false);
        $("#userPass").val('').removeClass('d-none');
        $("#userconfirmPass").val('').removeClass('d-none');
    },

    validateForm() {
        var valid = true;
        var TRAN_ID = $("#Code").val();
        var EMP_ID = $("#Employee_hidden").val();
        var F_NAME = $("#F_NAME").val();
        var RELATION = $("#RELATION").dxSelectBox('instance').option('value');
        var DOB = $("#DOB").val();
        var GENDER = $("#GENDER").dxSelectBox('instance').option('value');
        var CNIC = $("#CNIC").val();
        var CNIC_EXP = $("#CNIC_EXP").val();
        var MSTATUS = $("#MSTATUS").dxSelectBox('instance').option('value');
        var INSTITUTE = $("#INSTITUTE").val();
        var EDUCATION = $("#EDUCATION").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');

        if (EMP_ID == '' || EMP_ID == 0 || EMP_ID == null) {
            valid = false;
            empr_helper.notify("Please select employee.", 2);
        }

        if (F_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter Name.", 2);
        }

        if (DOB == '') {
            valid = false;
            empr_helper.notify("Please enter DOB.", 2);
        }

        if (CNIC_EXP == '') {
            valid = false;
            empr_helper.notify("Please enter CNIC Expiry.", 2);
        }

        if (CNIC == '') {
            valid = false;
            empr_helper.notify("Please enter CNIC.", 2);
        }

        if (GENDER == '') {
            valid = false;
            empr_helper.notify("Please select Gender.", 2);
        }

        return valid;
    },

    GetDataToSave() {

        var TRAN_ID = $("#Code").val();
        var EMP_ID = $("#Employee_hidden").val();
        var F_NAME = $("#F_NAME").val();
        var RELATION = $("#RELATION").dxSelectBox('instance').option('value');
        var DOB = $("#DOB").val();
        var GENDER = $("#GENDER").dxSelectBox('instance').option('value');
        var CNIC = $("#CNIC").val();
        var CNIC_EXP = $("#CNIC_EXP").val();
        var MSTATUS = $("#MSTATUS").dxSelectBox('instance').option('value');
        var INSTITUTE = $("#INSTITUTE").val();
        var EDUCATION = $("#EDUCATION").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            EMP_ID: EMP_ID,
            F_NAME: F_NAME,
            RELATION: RELATION,
            DOB: DOB,
            GENDER: GENDER,
            CNIC: CNIC,
            CNIC_EXP: CNIC_EXP,
            MSTATUS: MSTATUS,
            INSTITUTE: INSTITUTE,
            EDUCATION: EDUCATION,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS
        }

        return modelRecord;
    },

    saveAttempt() {
        var EMP_ID = $("#Employee_hidden").val();
        var obj = empr_FamilyMember.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/FamilyMember/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                //empr_FamilyMember.resetForm();
                if ($("#Code").val() == 0
                    || $("#Code").val() == null
                    || $("#Code").val() == undefined
                    || $("#Code").val() == "") {
                    $('#Code').val(data.data);
                    empr_helper.selectedBill = data.data;
                }
                empr_FamilyMember.InitQuickSearch(EMP_ID);
                $('#BtnDelete').hide();
            }
        }, false, true);
    },

    InitQuickSearch(employeeId) {
        empr_FamilyMember.GetAllFamilyMembers(employeeId);
    },

    GetAllFamilyMembers(employeeId) {
        ajaxHelper.ajaxGetJson('/FamilyMember/QuickSearch?employeeId=' + employeeId, function (data) {
            empr_FamilyMember.CreateGrid(data.data);
        }, false, true);
    },

    GetFamilyMemberByID(id) {
        ajaxHelper.ajaxGetJson('/FamilyMember/GetFamilyMemberByID?id=' + id, function (data) {
            //empr_FamilyMember.resetForm();
            if (data.msgType == 1) {

                var record = data.data;
                $("#Code").val(record.traN_ID);
                $("#Employee_hidden").val(record.emP_ID);
                $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $('#RELATION').dxSelectBox('instance').option('value', record.relation);
                $("#F_NAME").val(record.f_NAME);
                $("#DOB").val(record.dob);
                $('#GENDER').dxSelectBox('instance').option('value', record.gender);
                $('#MSTATUS').dxSelectBox('instance').option('value', record.mstatus);
                $("#CNIC").val(record.cnic);
                $("#CNIC_EXP").val(record.cniC_EXP);
                $("#INSTITUTE").val(record.institute);
                $("#EDUCATION").val(record.education);
                $("#REMARKS").val(record.remarks);
                empr_FamilyMember.InitEmployeeGridBox(record.emP_ID);

                $('#BtnNew').show();
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGrid(dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        { dataField: 'f_NAME', caption: 'Name' },
        { dataField: 'gender', caption: 'Relation' },
        { dataField: 'dob', caption: 'DOB' },
        { dataField: 'cnic', caption: 'CNIC' },
        { dataField: 'astatus', caption: 'Active' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "FamilyMemberQS");
    },

    bindDxDdl(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },

    SaveImage() {
        $('#BtnSave').prop('disabled', true);
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob);
        $.ajax({
            url: "/FamilyMember/SaveImage",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    $("#GROUP_PIC").val(data.data);
                }
                else {
                    console.log(data);
                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                }
                $('#BtnSave').prop('disabled', false);
            }
        });
    },

    InitEmployeeGridBox: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "FamilyMember/GetEmployees",
            type: "GET",
            success: function (response) {
                console.log(response)
                var Datasource = response;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.key == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].key;
                        $('#Employee_hidden').val(selectedObject[0].key);
                        $('#displayExprEmployee').val(selectedObject[0].value);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#EMP_ID").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "key",
                    displayExpr: function (item) {
                        return item ? `${item.value}` : "Select a value...";
                    },
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.key === e.value);
                            if (selectedData) {
                                $('#Employee_hidden').val(selectedData.key);
                                $('#displayExprEmployee').val(selectedData.value);

                                empr_FamilyMember.InitQuickSearch(selectedData.key);
                            }
                        } else {
                            $('#Employee_hidden').val('');
                            $('#displayExprEmployee').val('');
                            empr_FamilyMember.InitQuickSearch(0);
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
                                key: "key"
                            }),
                            columns: [
                                {
                                    dataField: "key",
                                    caption: "Code",
                                    width: '60px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "empId",
                                    caption: "Employee ID",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "value",
                                    caption: "Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "dep",
                                    caption: "Department",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "desig",
                                    caption: "Designation",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "branch",
                                    caption: "Branch",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "father",
                                    caption: "Father Name",
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
                                    e.component.option("value", selected.key);
                                    e.component.close();

                                    // Update hidden and display fields
                                    $('#Employee_hidden').val(selected.key);
                                    $('#displayExprEmployee').val(selected.value);
                                    empr_FamilyMember.InitQuickSearch(selected.key);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].key], false);
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
                            ["value", "contains", searchTerm],
                            "or",
                            ["key", "contains", searchTerm],
                            "or",
                            ["empId", "contains", searchTerm],
                            "or",
                            ["dep", "contains", searchTerm],
                            "or",
                            ["desig", "contains", searchTerm],
                            "or",
                            ["branch", "contains", searchTerm],
                            "or",
                            ["father", "contains", searchTerm]
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

                $(document).on("dxclick", "#EMP_ID .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#Employee_hidden').val('');
                    $('#displayExprEmployee').val('');
                    empr_FamilyMember.InitQuickSearch(0);
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
            }
        });
    },

    InitMaritalStatusDDL: function (_selectedValue) {
        $('#MSTATUS').dxSelectBox({
            dataSource: empr_helper.maritalStatus,
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

    InitRelationDDL: function (_selectedValue) {
        $.ajax({
            url: 'FamilyMember/GetRelations',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#RELATION').dxSelectBox({
                        dataSource: data.data,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
}