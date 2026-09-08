var empr_DeleteAttendance = {
    isValueAssigned: false,
    GetTime: "",
    DeleteAttendanceData: [],
    InitEvents: function () {
        $(document).ready(function () {
            empr_DeleteAttendance.CreateQuickSearchGrid("");
            empr_DeleteAttendance.InitEmployees("");
            //empr_DeleteAttendance.InitAllAttendanceGrid();
            empr_DeleteAttendance.SetTime();
            //empr_DeleteAttendance.InitReportType();

            $('#BtnGet').click(function () {
                debugger;
                empr_DeleteAttendance.InitQuickSearchGrid();
            });
            $('#quicksearch, #GetAttendance').click(function () {
                debugger;
                if (empr_DeleteAttendance.ValidateInfo()) {
                    empr_DeleteAttendance.InitAllAttendanceGrid();
                }
                
            });
            $('body').on('click', '#printModalClose', function () {
                debugger;
                // Clear previous data from modal to prevent duplication
                $('#printModal').modal('hide');

            });
            $('body').on('input', '#EmpId', function () {
                debugger;
                var value = $(this).val(); // Corrected value retrieval
                var Emp = Employee.find(x => x.empId == value); // Find employee by empId

                if (Emp) {
                    $('#employee').dxSelectBox('option', 'value', Emp.key); // Set the value correctly
                }
                else {
                    $('#employee').dxSelectBox('option', 'value', ''); // Set the value correctly
                }
            });

            document.querySelectorAll(".close").forEach(function (closeBtn) {
                closeBtn.addEventListener("click", function () {
                    $('#printModal').modal('hide');
                });
            });
        });
    },
    SetTime() {
        $("#Time").on("input", function () {
            debugger;
            var value = $(this).val();

            // Agar sirf ek digit likhi gai ho
            if (value.length === 1) {
                if (value === "2") {
                    $(this).val("12:00 AM"); // 2 likhne pe direct 12:00 AM set karein
                    return;
                }
            }

            // Agar user `13` likhta hai toh usay `01` bana dein
            if (value.length === 2 && parseInt(value) > 12) {
                $(this).val("0" + (parseInt(value) - 12) + ":00 PM");
                return;
            }

            // Regular expression to match hh or hh:mm format
            var regex = /^(\d{1,2})(:\d{1,2})?$/;

            if (regex.test(value)) {
                var match = value.match(regex);
                var hours = match[1]; // Extract hours
                var minutes = match[2] ? match[2].slice(1) : "00"; // Default minutes to :00 if not provided

                // Agar sirf ek digit hai jo `2` nahi hai toh doosri digit likhne dein
                if ((hours.length === 1 && hours !== "2")) {
                    if (hours.length === 2) {
                        $(this).val(hours + ":");
                    }
                    return;
                }

                // Format hours to be two digits
                hours = hours.padStart(2, "0");

                // Determine AM or PM
                var period = parseInt(hours) >= 12 ? "PM" : "AM";

                // Adjust hours to 12-hour format
                if (parseInt(hours) > 12) {
                    hours = (parseInt(hours) - 12).toString().padStart(2, "0");
                } else if (hours === "00" || hours === "0") {
                    hours = "12"; // 00 or 0 should be displayed as 12
                }

                if (minutes.length === 2) {
                    // Update field value with formatted time
                    $(this).val(hours + ":" + minutes + " " + period);
                } else {
                    $(this).val(hours + ":" + minutes);
                    return;
                }
            }
        });

        $("#Time").on("change", function () {
            empr_DeleteAttendance.GetTime = $(this).val();
            console.log("Entered Time: " + $(this).val());
        });
    },

    InitEmployees: function (selectedValue) {
        debugger;
        $('#employee').dxSelectBox({
            dataSource: Employee,
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
                if (e.value) {
                    var Emp = Employee.find(x => x.key == e.value); // Corrected key comparison
                    if (Emp) {
                        console.log(Emp);
                        $('#EmpId').val(Emp.empId); // EmpId correctly assigned
                    }
                } else {
                    $('#EmpId').val(''); // Clear if no selection
                }
            }
        });
    },

    PrintModal() {
        var dataModel = empr_DeleteAttendance.GetDataToSave();
        debugger;
        ajaxHelper.ajaxPostJsonData(dataModel, "/ClosingShop/PrintModal", function (data) {
            if (true) {
                debugger;
                $('#slipPreview').html(data.slipHtml);
                $('#printModal').modal('show');

                $('#printModal').on('shown.bs.modal', function () {
                    setTimeout(function () {
                        var modalBody = $('#printModal .modal-body'); // Scroll the modal body, not the footer
                        console.log(modalBody[0].scrollHeight); // Check if scrollHeight is valid
                        if (modalBody[0].scrollHeight > modalBody.height()) {
                            modalBody.animate({
                                scrollTop: modalBody[0].scrollHeight
                            }, 200); // Scroll to the bottom of the modal body
                        } // Scroll to the bottom of the modal body
                    }, 100); // Delay to ensure the modal is fully rendered
                });
            }
        }, false, true);
    },
    GetDataToSave: function () {
        debugger;
        var FromDate = empr_DeleteAttendance.formatDate($('#FromDate').val());
        var ToDate = empr_DeleteAttendance.formatDate($('#ToDate').val());

        var masterRecord = {
            FromDate: FromDate,
            ToDate: ToDate
        }

        var detailRecords = empr_DeleteAttendance.ClosingData.data;
        console.log(detailRecords);
        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords
        };
        return modelRecord;
    },

    formatDate(dateString) {
        var date = new Date(dateString);
        var day = String(date.getDate()).padStart(2, '0');
        var month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
        var year = date.getFullYear();
        return `${day}-${month}-${year}`;
    },
    InitQuickSearchGrid: function () {
        empr_DeleteAttendance.QuickSearch();
    },
    InitAllAttendanceGrid: function () {
        empr_DeleteAttendance.AllAttendance();
    },
    InitReportType: function () {
        ajaxHelper.ajaxGetJson("/ClosingShop/GetReportTypes", function (data) {
            debugger
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    var selectedValue = data.data[0].mD_ID;
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
    UpdateClosedData: function () {
        debugger;
        var FromDate = $('#FromDate').val();
        var ToDate = $('#ToDate').val();

        ajaxHelper.ajaxGetJson('/ClosingShop/UpdateClosedData?FromDate=' + FromDate + '&ToDate=' + ToDate, function (data) {
            if (data.msgType == 1) {
                empr_DeleteAttendance.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);

    },
    QuickSearch: function () {
        debugger;
        ajaxHelper.ajaxGetJson('/DeleteAttendance/GetDeleteAttendanceData', function (data) {
            empr_DeleteAttendance.DeleteAttendanceData = data;
            if (data.msgType == 1) {
                empr_DeleteAttendance.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    SendDatatoFilter: function () {
        debugger;
        var FromDate = $('#FromDate').val();
        var ToDate = $('#ToDate').val();
        var Time = $('#Time').val();
        var EmpId = $('#EmpId').val();
        var Employee = $('#employee').dxSelectBox('option', 'value');

        var modelRecord = {
            FromDate: FromDate,
            ToDate: ToDate,
            Time: Time,
            EmpId: EmpId,
            Employee: Employee
        };
        return modelRecord;
    },

    AllAttendance: function () {
        debugger;
        var obj = empr_DeleteAttendance.SendDatatoFilter();

        ajaxHelper.ajaxPostJsonData(obj, '/DeleteAttendance/GetAllAttendance', function (data) {
            empr_DeleteAttendance.DeleteAttendanceData = data;
            if (data.msgType == 1) {
                empr_DeleteAttendance.CreateAllAttendanceGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        debugger;

        var col = [
            {
                dataField: "Action",
                width: 90,
                alignment: 'center',
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" style="margin-left: 8px;" class="grid-action-icon Delete" title="Delete" onclick="empr_DeleteAttendance.DeleteRow(${options.rowIndex}, '${options.data.time}','${options.data.date}')" ><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;" style="margin-left: 8px;" class="grid-action-icon Delete" title="Delete" onclick="empr_DeleteAttendance.DeleteRow(${options.rowIndex}, '${options.data.time}','${options.data.date}')" ><i class="fa fa-trash"></i></a>
                                   </div>`).appendTo(container);
                    }
                }
            },
            { dataField: 'machinecode', caption: 'M.Code', width: 70 },
            { dataField: 'checktype', caption: 'Check', },
            { dataField: 'date', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy', width: 80 },
            { dataField: 'time', caption: 'Time', width: 80 },
            { dataField: 'empid', caption: 'Emp.ID', width: 70 },
            { dataField: 'ename', caption: 'Employee Name', width: 150 },
            { dataField: 'fathername', caption: 'Father Name', width: 150 },
            { dataField: 'depname', caption: 'Department', width: 150 },
            { dataField: 'shiftt', caption: 'Shift', width: 80 },
            { dataField: 'email', caption: 'Email', width: 150 },
            { dataField: 'cellno', caption: 'Cell No', width: 100 },
            { dataField: 'reg', caption: 'Religion', width: 80 },
            { dataField: 'joindate', caption: 'J.Date', dataType: 'date', format: 'dd-MM-yyy', width: 80 },
            { dataField: 'parmdate', caption: 'Perm.Date', dataType: 'date', format: 'dd-MM-yyy', width: 100 },
            { dataField: 'salaryhold', caption: 'Salary', visible: true, width: 70 },
            { dataField: 'mstatus', caption: 'Emp.Status', visible: true, width: 100 },
        ];
        empr_helper.dxGridbindingVouchers('#AllAttendanceGrid', col, dataSrc, "DeleteAttendanceQS", "single");
    },

    CreateAllAttendanceGrid: function (dataSrc) {
        debugger;
        console.log(Permissions);
        var col = [
            {
                dataField: "Action",
                width: 90,
                alignment: 'center',
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" style="margin-left: 8px;" class="grid-action-icon Delete" title="Delete" onclick="empr_DeleteAttendance.DeleteRow(${options.rowIndex}, '${options.data.time}','${options.data.date}')" ><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;" style="margin-left: 8px;" class="grid-action-icon Delete" title="Delete" onclick="empr_DeleteAttendance.DeleteRow(${options.rowIndex}, '${options.data.time}','${options.data.date}')" ><i class="fa fa-trash"></i></a>
                                   </div>`).appendTo(container);
                    }
                }
            },
            { dataField: 'machinecode', caption: 'M.Code', width: 70 },
            { dataField: 'checktype', caption: 'Check', },
            { dataField: 'date', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy', width: 80 },
            { dataField: 'time', caption: 'Time', width: 80 },
            { dataField: 'empid', caption: 'Emp.ID', width: 70 },
            { dataField: 'ename', caption: 'Employee Name', width: 150 },
            { dataField: 'fathername', caption: 'Father Name', width: 150 },
            { dataField: 'depname', caption: 'Department', width: 150 },
            { dataField: 'shiftt', caption: 'Shift', width: 80 },
            { dataField: 'email', caption: 'Email', width: 150 },
            { dataField: 'cellno', caption: 'Cell No', width: 100 },
            { dataField: 'reg', caption: 'Religion', width: 80 },
            { dataField: 'joindate', caption: 'J.Date', dataType: 'date', format: 'dd-MM-yyy', width: 80 },
            { dataField: 'parmdate', caption: 'Perm.Date', dataType: 'date', format: 'dd-MM-yyy', width: 100 },
            { dataField: 'salaryhold', caption: 'Salary', visible: true, width: 70 },
            { dataField: 'mstatus', caption: 'Emp.Status', visible: true, width: 100 },
        ];
        empr_helper.dxGridbindingVouchers('#AllAttendanceGrid', col, dataSrc, "DeleteAttendance", "single");
        /*empr_helper.DxGridBindingForReports('#AllAttendanceGrid', col, dataSrc, "DeleteAttendance");*/
    },

    DeleteRow: function (index, time , date) {
        const gridInstance = $('#AllAttendanceGrid').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            var row = dataSource[index];
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
                ajaxHelper.ajaxPostJsonData({ Time: time, Date: date }, "/DeleteAttendance/DeleteattendanceByTime", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        empr_DeleteAttendance.InitAllAttendanceGrid();
                        //gridInstance.getDataSource().load(); 
                        //gridInstance.refresh();
                        //gridInstance.saveEditData();
                    }
                }, false, true);
            });
        }
    },

    GetData: function () {
        var CODE = $("#Code").val();
        var description = $("#Description").val().trim();
        var status = $("#ASTATUS").dxSelectBox('instance').option('value');
        var SelectedBranches = $("#branches_hidden").val();
        var SelectedItems = $("#design_hidden").val();
        var SelectedItemGroups = $("#group_hidden").val();
        var FromDate = $("#FromDate").val();
        var ToDate = $("#ToDate").val();
        var DiscountPercent = $("#DiscountPercent").val();
        var DiscountExpired = $("#DiscountExpired").prop('checked') ? "1" : "0";

        var modelRecord = {
            CODE: CODE,
            DESCR: description,
            ASTATUS: status,
            SELECTEDBRANCHES: SelectedBranches,
            SELECTEDITEMS: SelectedItems,
            SELECTEDITEMGROUPS: SelectedItemGroups,
            FDATE: FromDate,
            TDATE: ToDate,
            DISC: DiscountPercent,
            DISC_EXP: DiscountExpired
        }

        return modelRecord;
    },
    ValidateInfo: function () {

        var valid = true;
        var data = empr_DeleteAttendance.SendDatatoFilter();

        if (data.EmpId == '' || data.EmpId == null || data.EmpId == undefined) {
            empr_helper.notify("Please Enter Employee ID.", 2);
            valid = false;
            return valid;
        }
        //if (data.EmpId != '' || data.EmpId != null || data.EmpId != undefined || data.Employee == '' || data.Employee == null || data.Employee == undefined) {
        //    empr_helper.notify("Please Enter correct Employee ID.", 2);
        //    valid = false;
        //    return valid;
        //}

        if (data.Employee == '' || data.Employee == null || data.Employee == undefined) {
            empr_helper.notify("Please Select Employee.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },
    SaveInfo: function () {
        debugger;
        var data = empr_DeleteAttendance.GetData();
        ajaxHelper.ajaxPostJsonData(data, "/ClosingShop/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_DeleteAttendance.ResetForm();
            }
        }, false, true);
    },
    ResetForm: function () {
        debugger;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #FromDate').val('');
        $('#DiscountExpired').prop('checked', false);
        empr_DeleteAttendance.InitQuickSearchGrid();
        empr_DeleteAttendance.InitBranchesDDL();
        empr_DeleteAttendance.InitDesignNoDDL();
        empr_DeleteAttendance.InitItemGroupDDL();
        $('#BtnDelete, #BtnNew').hide();
    },
    GetClosingShopByCode: function (id) {

        ajaxHelper.ajaxGetJson('/ClosingShop/GetClosingShopByCode?code=' + id, function (data) {
            debugger;
            empr_DeleteAttendance.ResetForm();
            if (data.msgType == 1) {
                var response = data.data[0];

                $("#Code").val(response.code);
                $("#Description").val(response.descr);
                $("#ASTATUS").dxSelectBox('instance').option('value', response.astatus);
                $("#branches_hidden").val(response.bcode);
                $("#design_hidden").val(response.iteM_CODE);
                $("#group_hidden").val(response.iteM_GROUP);
                if (response.fdate != '1900-01-01') {
                    $("#FromDate").val(response.fdate);
                }

                if (response.tdate != '1900-01-01') {
                    $("#ToDate").val(response.tdate);
                }

                $("#DiscountPercent").val(response.disc);
                if (response.disC_EXP == "1") {
                    $("#DiscountExpired").prop('checked', true);
                }

                setTimeout(function () {
                    var branchRow = $('#Branches_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.bcode);
                    if (branchRow[0] != null) {
                        var selectedRow = branchRow[0];
                        $('#Branches_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                    var designRow = $('#DesignNo_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.iteM_CODE);
                    if (designRow[0] != null) {
                        var selectedRow = designRow[0];
                        $('#DesignNo_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                    var groupRow = $('#ItemGroup_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.iteM_GROUP);
                    if (groupRow[0] != null) {
                        var selectedRow = groupRow[0];
                        $('#ItemGroup_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                }, 1000);
                $('#BtnDelete').show();
                $('#BtnNew').show();

                $('.modal').modal('hide');
            }
        }, false, true);
    },
    Delete: function (_id) {

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
            ajaxHelper.ajaxPostJsonData({ code: _id }, "/ClosingShop/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    window.location.reload();
                }
            }, false, true);
        });
    },
    InitBranchesDDL: function () {
        $.ajax({
            url: 'ClosingShop/GetBranches',
            method: 'GET',
            success: function (data) {
                empr_DeleteAttendance.MultipleDxGridBoxDropdown('#Branches', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_branches", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#branches_hidden').val(keys);
                        $('#displayExpr_branches').val(values);
                        empr_DeleteAttendance.isValueAssigned = false;
                    }
                    else {
                        $('#branches_hidden').val('');
                        $('#displayExpr_branches').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitDesignNoDDL: function (_selectedValue) {
        $.ajax({
            url: 'ClosingShop/GetDesignNos',
            method: 'GET',
            success: function (data) {
                empr_DeleteAttendance.MultipleDxGridBoxDropdown('#DesignNo', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_design", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#design_hidden').val(keys);
                        $('#displayExpr_design').val(values);
                        empr_DeleteAttendance.isValueAssigned = false;
                        $('#group_hidden').val('');
                        $('#displayExpr_group').val('');
                        $('#ItemGroup_grid').dxDataGrid('instance').selectRows([]);
                    }
                    else {
                        $('#design_hidden').val('');
                        $('#displayExpr_design').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitItemGroupDDL: function (_selectedValue) {
        $.ajax({
            url: 'ClosingShop/GetItemGroups',
            method: 'GET',
            success: function (data) {
                empr_DeleteAttendance.MultipleDxGridBoxDropdown('#ItemGroup', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_group", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#group_hidden').val(keys);
                        $('#displayExpr_group').val(values);
                        empr_DeleteAttendance.isValueAssigned = false;
                        $('#design_hidden').val('');
                        $('#displayExpr_design').val('');
                        $('#DesignNo_grid').dxDataGrid('instance').selectRows([]);
                    }
                    else {
                        $('#group_hidden').val('');
                        $('#displayExpr_group').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    MultipleDxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.MultipleDxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
}