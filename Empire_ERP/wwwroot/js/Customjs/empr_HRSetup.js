var empr_HRSetup = {
    rowsCount:0,
    initEvents: function () {

        $(document).ready(function () {

            empr_HRSetup.resetForm();

            $('.saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_HRSetup.validateMainInfo()) {
                            empr_HRSetup.saveAttempt();
                        }
                    }
                } else {
                    if (empr_HRSetup.validateMainInfo()) {
                        empr_HRSetup.saveAttempt();
                    }
                }
            });

            $('#DocumentFile').change(function () {
                empr_HRSetup.uploadFile();
            })

            $('#CnicFile').change(function () {
                empr_HRSetup.uploadCNIC();
            })

            $('.delete').click(function () {
                var partytypecode = $('#Code').val();
                empr_HRSetup.delete(partytypecode);
            });

            $('body').on('click', '#quicksearch', function () {
                empr_HRSetup.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var rportid = $(this).attr("rportid")
                empr_HRSetup.GetHRSetupByHRSetupCode(rportid);
            });

            $('body').on('click', '#resetForm', function () {
                debugger;
                empr_HRSetup.resetForm();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#resetForm').hide();
                !Permissions.r_VIEW && $('#quicksearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('.saveAttempt').hide();
            }
        });

    },

    validURl: function (url) {
        //var pattern = /^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$/;
        var pattern = /^[a-zA-Z0-9.-]+\.(com)$/i;
        return pattern.test(url);
    },

    validateMainInfo: function () {

        var valid = true;
        var data = empr_HRSetup.getDataToSave();

        if (data.EMP_ID.trim() == '') {
            empr_helper.notify("HRSetup ID is required.", 2);
            valid = false;
        }

        if (data.MACHINE_CODE.trim() == '') {
            empr_helper.notify("Machine Code is required.", 2);
            valid = false;
        }

        if (data.ENAME == '') {
            empr_helper.notify("HRSetup Name is required.", 2);
            valid = false;
        }

        if (data.FATHER_NAME == '') {
            empr_helper.notify("Father Name is required.", 2);
            valid = false;
        }

        if (data.DEP_ID == '' || data.DEP_ID == null) {
            empr_helper.notify("Department is required.", 2);
            valid = false;
        }

        if (data.DESIG == '' || data.DESIG == null) {
            empr_helper.notify("Designation is required.", 2);
            valid = false;
        }

        if (data.GENDER == '' || data.GENDER == null) {
            empr_helper.notify("Gender is required.", 2);
            valid = false;
        }

        if (data.BCODE == '' || data.BCODE == null) {
            empr_helper.notify("Branch is required.", 2);
            valid = false;
        }

        if (data.SHIFT_T == '' || data.SHIFT_T == null) {
            empr_helper.notify("Shift is required.", 2);
            valid = false;
        }

        if (data.EMP_TYPE == '' || data.EMP_TYPE == null) {
            empr_helper.notify("Type is required.", 2);
            valid = false;
        }

        if (data.CELL_NO == '' || data.CELL_NO == null) {
            empr_helper.notify("Cell No. is required.", 2);
            valid = false;
        }

        if (data.REG == '' || data.REG == null) {
            empr_helper.notify("Religion No. is required.", 2);
            valid = false;
        }

        if (data.PAY_MODE == '' || data.PAY_MODE == null) {
            empr_helper.notify("Pay Mode No. is required.", 2);
            valid = false;
        }

        if (data.JOIN_DATE == '' || data.JOIN_DATE == null) {
            empr_helper.notify("Joining Date is required.", 2);
            valid = false;
        }

        if (data.PARM_DATE == '' || data.PARM_DATE == null) {
            empr_helper.notify("Permanent Date is required.", 2);
            valid = false;
        }

        //if (data.FAMILY_NUM == '' || data.FAMILY_NUM == null) {
        //    empr_helper.notify("Family No. is required.", 2);
        //    valid = false;
        //}

        if (data.EMP_ADD == '' || data.EMP_ADD == null) {
            empr_helper.notify("Address is required.", 2);
            valid = false;
        }

        //if (data.DOB == '' || data.DOB == null) {
        //    empr_helper.notify("DOB is required.", 2);
        //    valid = false;
        //}

        //if (data.RSTATUS == '' || data.RSTATUS == null) {
        //    empr_helper.notify("Roster Status is required.", 2);
        //    valid = false;
        //}

        //if (data.EMAIL != '') {
        //    if (!empr_helper.isEmail(data.EMAIL)) {
        //        empr_helper.notify("Please add valid email address.", 2);
        //        valid = false;
        //    }
        //}

        return valid;
    },

    InitQuickSearch: function () {

        empr_HRSetup.GetDataForQuickSeachGrid();
    },

    GetDataForQuickSeachGrid: function () {

        var xhr = ajaxHelper.ajaxGetJson('/HRSetup/QuickSearchHRSetup', function (data) {
            empr_HRSetup.CreateGrid(data.data);
        }, false, true);

    },

    CreateGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_HRSetup.rowsCount = dataSrc.length - 1;

        }
        const invalidDates = [
            '1900-01-01', '01-01-1900', '01-Jan-1900', '1/1/1900', '01/01/1900',
            '1/1/1900 12:00:00 AM', '01/01/1900 12:00:00 AM',
            '2000-01-01', '01-01-2000', '01-Jan-2000', '1/1/2000', '01/01/2000',
            '1/1/2000 12:00:00 AM', '01/01/2000 12:00:00 AM',
            '00-01-01', '01-01-00', '01-Jan-00', '1/1/00', '01/01/00',
            '1/1/00 12:00:00 AM', '01/01/00 12:00:00 AM', '01-Jan-00 12:00:00 AM'
        ];

        //dataSrc.forEach(item => {
        //    if (invalidDates.includes(item.eta)) {
        //        item.eta = null;
        //    }
        //    if (item.fI_REQ == '1') {
        //        item.fI_REQ = true;
        //    } else {
        //        item.fI_REQ = false;
        //    }
        //});

        var col = [
            {
                dataField: 'grouP_CODE',
                caption: 'Seq #',
                alignment: "center" ,
                allowEditing: false,
                width: 100,
            },
            {
                dataField: 'astatus',
                caption: 'Status',
                alignment: "center" ,
                width: 100,
                allowEditing: false,
                dataType: "number",
                cellTemplate: function (container, options) {
                    var $checkBoxContainer = $("<div>")
                        .addClass("custom-checkbox-container")
                        .css("margin-right", "0px");

                    $("<div>")
                        .dxCheckBox({
                            value: options.value === "1",   // triple equals check
                            onValueChanged: function (e) {
                                var gridInstance = $("#DetailContainer").dxDataGrid("instance");
                                gridInstance.cellValue(options.rowIndex, "astatus", e.value ? "1" : "0");
                            }
                        })
                        .appendTo($checkBoxContainer);

                    $(container).append($checkBoxContainer);
                }
            },
            {
                dataField: 'grouP_NAME',
                caption: 'Holidays',
                alignment: "center" ,
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "HRSetup", "iteM_CODE");
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

    GetHRSetupByHRSetupCode: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/HRSetup/GetHRSetupByHRSetupCode?code=' + id, function (data) {
            var HRSetupdata = data.HRSetupData.data;

            empr_HRSetup.InitGenderDDL(HRSetupdata.gender);
            empr_HRSetup.InitReligionDDL(HRSetupdata.reg);
            empr_HRSetup.InitDepartmentDDL(HRSetupdata.deP_ID);
            empr_HRSetup.InitDesignationDDL(HRSetupdata.desig);
            empr_HRSetup.InitEmploymentTypeDDL(HRSetupdata.emP_TYPE);
            empr_HRSetup.InitShiftDDL(HRSetupdata.shifT_T);
            empr_HRSetup.InitBranchDDL(HRSetupdata.bcode);
            empr_HRSetup.InitPaymentModeDDL(HRSetupdata.paY_MODE);
            empr_HRSetup.InitMaritalStatusDDL(HRSetupdata.mstatus);
            empr_HRSetup.InitRoasterStatusDDL(HRSetupdata.rstatus);
            empr_HRSetup.InitEducationDDL(HRSetupdata.education);

            $("#Code").val(HRSetupdata.emP_CODE);


            $('#EMP_ID').val(HRSetupdata.emP_ID);
            $('#MACHINE_CODE').val(HRSetupdata.machinE_CODE);
            $('#ENAME').val(HRSetupdata.ename);
            $('#FATHER_NAME').val(HRSetupdata.fatheR_NAME);
            $('#CELL_NO').val(HRSetupdata.celL_NO);
            $('#EMAIL').val(HRSetupdata.email);
            $('#CHILD').val(HRSetupdata.child);
            $('#JOIN_DATE').val(HRSetupdata.joiN_DATE);
            $('#PARM_DATE').val(HRSetupdata.parM_DATE);
            $("#SALARY_HOLD").prop("checked", HRSetupdata.salarY_HOLD == 'Y' ? true : false);
            $('#CNIC').val(HRSetupdata.cnic);
            $('#FAMILY_NUM').val(HRSetupdata.familY_NUM);
            $('#BANK_ACC').val(HRSetupdata.banK_ACC);
            $('#CNIC_IDATE').val(HRSetupdata.cniC_IDATE);
            $('#CNIC_EDATE').val(HRSetupdata.cniC_EDATE);
            $('#BANK_NAME').val(HRSetupdata.banK_NAME);
            $('#NTN_NO').val(HRSetupdata.ntN_NO);
            $('#FILE_NO').val(HRSetupdata.filE_NO);
            $('#EMP_ADD').val(HRSetupdata.emP_ADD);
            $('#NATION').val(HRSetupdata.nation);
            $('#POB').val(HRSetupdata.pob);
            $('#DOB').val(HRSetupdata.dob);
            $("#MSTAFF").prop("checked", HRSetupdata.mstaff == 'Y' ? true : false);
            $("#OT").prop("checked", HRSetupdata.ot == 'Y' ? true : false);
            $('#EMP_CAST').val(HRSetupdata.emP_CAST);
            $('#VEH_NUMBER').val(HRSetupdata.veH_NUMBER);
            $('#LTYPE').val(HRSetupdata.ltype);
            $('#LNUMBER').val(HRSetupdata.lnumber);
            $('#L_IDATE').val(HRSetupdata.l_IDATE);
            $('#L_EDATE').val(HRSetupdata.l_EDATE);
            $('#LEFT_DATE').val(HRSetupdata.lefT_DATE);
            $('#REASON_L').val(HRSetupdata.reasoN_L);
            $('#ASTATUS').dxSelectBox('instance').option("value", HRSetupdata.astatus);
            $('#hdnTHUMB').val(HRSetupdata.signa);
            $('#hdnSIGNA').val(HRSetupdata.thumb);
            $('#EMP_IMG').val(HRSetupdata.emP_IMG);
            
            $('#pills-warningprofile-tab').show();

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
                }
                if (Permissions.r_EDIT) {
                    $('.saveAttempt').show();
                    $('#saveBranchInfo').show();
                }
                else {
                    $('.saveAttempt').hide();
                    $('#saveBranchInfo').hide();
                }
            } else {
                $('.saveAttempt').show();
                $('#saveBranchInfo').show();
                $('.btn-delete').show();
            }
            $('.modal').modal('hide');

        }, false, true);
    },

    delete: function (_id) {
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
            var ACT_CODE = $("#chartofaccount_hidden").val();
            var xhr = ajaxHelper.ajaxPostJsonData({ code: _id, actCode: ACT_CODE }, "/HRSetup/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {

                    empr_HRSetup.resetForm();
                }
            }, false, true);
        });
        //var xhr = ajaxHelper.ajaxPostJsonData({ partycode: _id }, "/HRSetup/Delete", function (data) {
        //    empr_helper.notify(data.msg, data.msgType);
        //    if (data.msgType == 1) {

        //        empr_HRSetup.resetForm();
        //    }
        //}, false, true);

    },

    getDataToSave: function () {

        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData();
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }

        var modelRecord = {
            Holiday: detailRecords
        };
        return modelRecord;
    },

    saveAttempt: function () {
        debugger;
        var dataModel = empr_HRSetup.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(dataModel, "/HRSetup/SaveMainOtherInfo", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {

                $('#Code').val(data.data);
                $('#pills-warningprofile-tab').show();
                $('.btn-delete').show();

                //if (dataModel.PARTY_CODE > 0) {
                //    empr_HRSetup.resetForm();
                //}
            }
        }, false, true);
    },

    resetForm: function () {
        var xhr = ajaxHelper.ajaxGetJson('/HRSetup/GetHolidayRecords', function (data) {
            console.log(data);
            if (data.data.rowsCount > 0) {
                mpr_HRSetup.CreateGrid(data.data);
            }
            else {
                empr_HRSetup.CreateGrid([{ priority: 'N' }]);
            }
            
        }, false, true);
        
        /*$('#pills-warningprofile-tab').hide();*/
        $('#myTab li:first-child a').click();
        $('.tab-pane').removeClass('fade');
        $('.btn-delete').hide();

        $("#Code").val('');
        $('#EMP_ID').val('');
        $('#MACHINE_CODE').val('');
        $('#ENAME').val('');
        $('#FATHER_NAME').val('');
        $('#CELL_NO').val('');
        $('#EMAIL').val('');
        $('#CHILD').val('');
        $('#JOIN_DATE').val('');
        $('#PARM_DATE').val('');
        $("#SALARY_HOLD").prop("checked", true);
        $('#CNIC').val('');
        $('#FAMILY_NUM').val('');
        $('#BANK_ACC').val('');
        $('#CNIC_IDATE').val('');
        $('#CNIC_EDATE').val('');
        $('#BANK_NAME').val('');
        $('#NTN_NO').val('');
        $('#FILE_NO').val('');
        $('#EMP_ADD').val('');
        $('#NATION').val('');
        $('#POB').val('');
        $('#DOB').val('');
        $("#MSTAFF").prop("checked", true);
        $("#OT").prop("checked", true);
        $('#EMP_CAST').val('');
        $('#VEH_NUMBER').val('');
        $('#LTYPE').val('');
        $('#LNUMBER').val('');
        $('#L_IDATE').val('');
        $('#L_EDATE').val('');
        $('#LEFT_DATE').val('');
        $('#REASON_L').val('');
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $('#hdnTHUMB').val('');
        $('#hdnSIGNA').val('');
        $('#EMP_IMG').val('');

        empr_HRSetup.InitGenderDDL();
        empr_HRSetup.InitReligionDDL();
        empr_HRSetup.InitDepartmentDDL();
        empr_HRSetup.InitDesignationDDL();
        empr_HRSetup.InitEmploymentTypeDDL();
        empr_HRSetup.InitShiftDDL();
        empr_HRSetup.InitBranchDDL();
        empr_HRSetup.InitPaymentModeDDL();
        empr_HRSetup.InitMaritalStatusDDL();
        empr_HRSetup.InitRoasterStatusDDL();
        empr_HRSetup.InitEducationDDL();

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('.saveAttempt').show();
            } else {
                $('.saveAttempt').hide();
            }
        }
    },

    uploadFile: function () {

        var input = document.getElementById('DocumentFile');
        var files = input.files;
        var formData = new FormData();

        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax(
            {
                url: "/HRSetup/UploadImage",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#PartytypesDocs_hidden").val(data.data);
                    }
                }
            }
        );

    },

    uploadCNIC: function () {

        var input = document.getElementById('CnicFile');
        var files = input.files;
        var formData = new FormData();

        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax(
            {
                url: "/HRSetup/UploadImage",
                //url: "/User/uploadfiles",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {

                    if (data.msgType == '1') {

                        $("#PartytypesCNIC_hidden").val(data.data);

                    }

                }
            }
        );

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

    InitReligionDDL: function (_selectedValue) {
        $.ajax({
            url: 'HRSetup/GetReligions',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#REG').dxSelectBox({
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

    InitDepartmentDDL: function (_selectedValue) {
        $.ajax({
            url: 'HRSetup/GetDepartments',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#DEP_ID').dxSelectBox({
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

    InitDesignationDDL: function (_selectedValue) {
        $.ajax({
            url: 'HRSetup/GetDesignations',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#DESIG').dxSelectBox({
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

    InitEmploymentTypeDDL: function (_selectedValue) {
        $.ajax({
            url: 'HRSetup/GetEmploymentTypes',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#EMP_TYPE').dxSelectBox({
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

    InitShiftDDL: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "HRSetup/GetShifts",
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

                $("#SHIFT_T").dxDropDownBox({
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

    InitBranchDDL: function (_selectedValue) {
        $.ajax({
            url: 'HRSetup/GetBranches',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#BCODE').dxSelectBox({
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

    InitPaymentModeDDL: function (_selectedValue) {
        var dataSource = [
            { value: 'C', key: 'Cash' },
            { value: 'B', key: 'Bank' },
            { value: 'CHQ', key: 'Cheque' },
        ];

        $('#PAY_MODE').dxSelectBox({
            dataSource: dataSource,
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

    InitRoasterStatusDDL: function (_selectedValue) {
        var dataSource = [
            { value: 'RE', key: 'Roaster/Non-Roaster HRSetup' },
            { value: 'DS', key: 'Direct Salary' },
        ];

        $('#RSTATUS').dxSelectBox({
            dataSource: dataSource,
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

    InitEducationDDL: function (_selectedValue) {
        $.ajax({
            url: 'HRSetup/GetEducations',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#EDUCTION').dxSelectBox({
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

    UploadThumb: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('THUMB').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/HRSetup/UploadThumbs",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnTHUMB").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },

    OpenThumb: function () {
        var hdnUrl = $('#hdnTHUMB').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    UploadSign: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('SIGNA').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/HRSetup/UploadSigns",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnSIGNA").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },

    OpenSign: function () {
        var hdnUrl = $('#hdnSIGNA').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
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
            url: "/HRSetup/SaveImage",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    $("#EMP_IMG").val(data.data);
                }
                else {
                    console.log(data);
                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                }
                $('#BtnSave').prop('disabled', false);
            }
        });
    },

    OpenImage: function () {
        var baseUrl = "/images/upload/HRSetups/";
        var hdnUrl = $('#EMP_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage('/images/upload/HRSetups/' + hdnUrl)
            //const fileURL = window.location.origin + baseUrl + hdnUrl;
            //window.open(fileURL, '_blank');
        }
    },

    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
}