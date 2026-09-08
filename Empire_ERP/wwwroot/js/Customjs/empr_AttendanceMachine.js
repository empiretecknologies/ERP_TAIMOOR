var empr_AttendanceMachine = {
    isValueAssigned: false,
    GetTime:"",
    AttendanceMachineData: [],
    InitEvents: function () {
        $(document).ready(function () {
            empr_AttendanceMachine.CreateQuickSearchGrid();
            empr_AttendanceMachine.SetTime();
            //empr_AttendanceMachine.InitReportType();

            $('#BtnGet').click(function () {
                debugger;
                empr_AttendanceMachine.InitQuickSearchGrid();
            });
            $('#quicksearch, #GetAttendance').click(function () {
                debugger;
                empr_AttendanceMachine.InitAllAttendanceGrid();
            });
            $('body').on('click', '#printModalClose', function () {
                debugger;
                // Clear previous data from modal to prevent duplication
                $('#printModal').modal('hide');

            });

            document.querySelectorAll(".close").forEach(function (closeBtn) {
                closeBtn.addEventListener("click", function () {
                    $('#printModal').modal('hide');
                });
            });

            $('#printButton').on('click', function () {
                debugger;

                // Get the content to print
                const printContent = document.getElementById('slipPreview').innerHTML;

                // Create a temporary iframe for printing
                const printFrame = document.createElement('iframe');
                printFrame.style.position = 'absolute';
                printFrame.style.top = '-10000px'; // Hide the iframe
                document.body.appendChild(printFrame);

                // Write the content into the iframe's document
                const frameDoc = printFrame.contentWindow || printFrame.contentDocument;
                frameDoc.document.open();
                frameDoc.document.write(`
                                        <html>  <head>
                                                    <style>
                                                        body {
                                                            width: 80mm;
                                                        }
                                                        table {
                                                            width: 100%;
                                                            border-collapse: collapse;
                                                        }
                                                        th, td {
                                                            padding: 3px;
                                                        }
                                                        tbody tr:last-child {
                                                            border-bottom: 0.5px solid black !important;
                                                        }
                                                    </style>
                                                </head>
                                            <body>
                                            ${printContent}
                                            </body>
                                        </html>
                                    `);
                frameDoc.document.close();

                empr_AttendanceMachine.UpdateClosedData();
                $('#printModal').modal('hide');

                // Trigger print in the iframe
                frameDoc.focus(); // Focus on the iframe
                frameDoc.print(); // Trigger the print

                // Remove the iframe after printing
                setTimeout(() => {
                    document.body.removeChild(printFrame);
                }, 1000);

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
            empr_AttendanceMachine.GetTime = $(this).val();
            console.log("Entered Time: " + $(this).val());
        });
    },
    PrintModal() {
        var dataModel = empr_AttendanceMachine.GetDataToSave();
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
        var FromDate = empr_AttendanceMachine.formatDate($('#FromDate').val());
        var ToDate = empr_AttendanceMachine.formatDate($('#ToDate').val());

        var masterRecord = {
            FromDate: FromDate,
            ToDate: ToDate
        }

        var detailRecords = empr_AttendanceMachine.ClosingData.data;
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
        empr_AttendanceMachine.QuickSearch();
    },
    InitAllAttendanceGrid: function () {
        empr_AttendanceMachine.AllAttendance();
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
                empr_AttendanceMachine.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);

    },
    QuickSearch: function () {
        debugger;
        ajaxHelper.ajaxGetJson('/AttendanceMachine/GetAttendanceMachineData', function (data) {
            empr_AttendanceMachine.AttendanceMachineData = data;
            if (data.msgType == 1) {
                empr_AttendanceMachine.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    SendDatatoFilter: function () {
        var FromDate = $('#FromDate').val();
        var ToDate = $('#ToDate').val();
        var Time = $('#Time').val();
        var EmpId = $('#EmpId').val();
        var Employee = $('#Employee').dxSelectBox('option', 'value');

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
        var obj = empr_AttendanceMachine.SendDatatoFilter();

        ajaxHelper.ajaxPostJsonData(obj, '/DeleteAttendance/GetAllAttendance', function (data) {
            empr_AttendanceMachine.AttendanceMachineData = data;
            if (data.msgType == 1) {
                empr_AttendanceMachine.CreateAllAttendanceGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        debugger;

        var col = [
            { dataField: 'machinecode', caption: 'M.Code', width: 70 },
            { dataField: 'checktype', caption: 'Check', visible: true },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "AttendanceMachineQS", "single");
    },
    CreateAllAttendanceGrid: function (dataSrc) {
        debugger;

        var col = [
            { dataField: 'machinecode', caption: 'M.Code', width: 70 },
            { dataField: 'checktype', caption: 'Check', width: 70 },
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
            { dataField: 'joindate', caption: 'J.Date', dataType: 'date', format: 'dd-MM-yyy',  width: 80 },
            { dataField: 'parmdate', caption: 'Perm.Date', dataType: 'date', format: 'dd-MM-yyy', width: 100 },
            { dataField: 'salaryhold', caption: 'Salary', visible: true,  width: 70 },
            { dataField: 'mstatus', caption: 'Emp.Status', visible: true, width: 100 },
        ];
        empr_helper.dxGridbindingVouchers('#AllAttendanceGrid', col, dataSrc, "AttendanceMachine", "single");
        /*empr_helper.DxGridBindingForReports('#AllAttendanceGrid', col, dataSrc, "AttendanceMachine");*/
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
        var data = empr_AttendanceMachine.GetData();

        if (data.DESCR == '' || data.DESCR == null || data.DESCR == undefined) {
            empr_helper.notify("Please enter the description.", 2);
            valid = false;
            return valid;
        }

        if (data.SELECTEDBRANCHES == '' || data.SELECTEDBRANCHES == null || data.SELECTEDBRANCHES == undefined) {
            empr_helper.notify("Please select at least one branch.", 2);
            valid = false;
            return valid;
        }

        if ((data.SELECTEDITEMS == '' || data.SELECTEDITEMS == null || data.SELECTEDITEMS == undefined) && (data.SELECTEDITEMGROUPS == '' || data.SELECTEDITEMGROUPS == null || data.SELECTEDITEMGROUPS == undefined)) {
            empr_helper.notify("Please select at least one item or one item group.", 2);
            valid = false;
            return valid;
        }

        if (data.DISC == '' || data.DISC == null || data.DISC == undefined) {
            empr_helper.notify("Please enter the discount percentage.", 2);
            valid = false;
            return valid;
        }

        if (data.FDATE == '' || data.FDATE == null || data.FDATE == undefined) {
            empr_helper.notify("Please enter the from date.", 2);
            valid = false;
            return valid;
        }

        if (data.TDATE != null && data.FDATE != null) {
            var fromDate = new Date(data.FDATE);
            var toDate = new Date(data.TDATE);

            if (toDate < fromDate) {
                empr_helper.notify("please select valid ToDate.", 2);
                valid = false;
                return valid;
            }
        }

        return valid;
    },
    SaveInfo: function () {
        debugger;
        var data = empr_AttendanceMachine.GetData();
        ajaxHelper.ajaxPostJsonData(data, "/ClosingShop/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_AttendanceMachine.ResetForm();
            }
        }, false, true);
    },
    ResetForm: function () {
        debugger;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #FromDate').val('');
        $('#DiscountExpired').prop('checked', false);
        empr_AttendanceMachine.InitQuickSearchGrid();
        empr_AttendanceMachine.InitBranchesDDL();
        empr_AttendanceMachine.InitDesignNoDDL();
        empr_AttendanceMachine.InitItemGroupDDL();
        $('#BtnDelete, #BtnNew').hide();
    },
    GetClosingShopByCode: function (id) {

        ajaxHelper.ajaxGetJson('/ClosingShop/GetClosingShopByCode?code=' + id, function (data) {
            debugger;
            empr_AttendanceMachine.ResetForm();
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
                empr_AttendanceMachine.MultipleDxGridBoxDropdown('#Branches', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_branches", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#branches_hidden').val(keys);
                        $('#displayExpr_branches').val(values);
                        empr_AttendanceMachine.isValueAssigned = false;
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
                empr_AttendanceMachine.MultipleDxGridBoxDropdown('#DesignNo', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_design", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#design_hidden').val(keys);
                        $('#displayExpr_design').val(values);
                        empr_AttendanceMachine.isValueAssigned = false;
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
                empr_AttendanceMachine.MultipleDxGridBoxDropdown('#ItemGroup', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_group", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#group_hidden').val(keys);
                        $('#displayExpr_group').val(values);
                        empr_AttendanceMachine.isValueAssigned = false;
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