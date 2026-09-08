var empr_HRMaster = {
    TableName: '',
    MasterId: 0 ,
    initEvents: function () {

        $(document).ready(function () {

            //empr_HRMaster.InitQuickSearch();
            empr_HRMaster.InitHRTables();
            empr_HRMaster.FieldHideAndShow('');

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_HRMaster.validateForm()) {
                            empr_HRMaster.saveAttempt(empr_HRMaster.TableName);
                        }
                    }
                } else {
                    if (empr_HRMaster.validateForm()) {
                        empr_HRMaster.saveAttempt(empr_HRMaster.TableName);
                    }
                }
            });

            $('#BtnNew').click(function () {
                debugger;
                empr_HRMaster.FieldHideAndShow(empr_HRMaster.TableName);
                $('#TNAME').dxSelectBox("instance").option("disabled", true);

            });

            $('#resetForm').click(function () {
                debugger;
                empr_HRMaster.resetForm();

            });

            $('#refresh').click(function () {
                empr_HRMaster.FieldHideAndShow('');
                empr_HRMaster.resetForm();
                $('#BtnNew').show();
                $('#TNAME').dxSelectBox("instance").option("disabled", false);
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_HRMaster.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_HRMaster.GetHRMasterByID(reportid, empr_HRMaster.TableName);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_HRMaster.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_HRMaster.DeleteRecord();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val(), TableName: empr_HRMaster.TableName }, "/HRMaster/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_HRMaster.resetForm();
                    empr_HRMaster.InitQuickSearch(empr_HRMaster.TableName);
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },
    resetForm: function () {
        $("#Code").val('');
        $("#GROUP_NAME").val('');
        $("#Leaves").val('');
        $("#TimeIn").val('');
        $("#TimeOut").val('');
        $("#WHR").val('');
        $("#GTimeIn").val('');
        $("#GTimeOut").val('');
        $("#BTimeIn").val('');
        $("#BTimeOut").val('');
        $("#NightShift").val('');
        $("#Driver").val('');
        $("#Capacity").val('');
        $("#Vehicle").val('');
        $("#Route").val('');
        $("#BSalary").val('');
        $("#GSalary").val('');
        $("#AnnualPct").val('');
        $("#Annualamt").val('');
        $("#MaximumSalary").val('');
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
    validateForm: function () {

        var valid = true;

        var TableName = empr_HRMaster.TableName;
        var GROUP_NAME = $("#GROUP_NAME").val();
        var Leaves = $("#Leaves").val();
        var TimeIn = $("#TimeIn").val();
        var TimeOut = $("#TimeOut").val();
        var WHR = $("#WHR").val();
        var GTimeIn = $("#GTimeIn").val();
        var GTimeOut = $("#GTimeOut").val();
        var BTimeIn = $("#BTimeIn").val();
        var BTimeOut = $("#BTimeOut").val();
        var NightShift = $("#NightShift").val();
        var Driver = $("#Driver").val();
        var Capacity = $("#Capacity").val();
        var Vehicle = $("#Vehicle").val();
        var Route = $("#Route").val();
        var BSalary = $("#BSalary").val();
        var GSalary = $("#GSalary").val();
        var AnnualPct = $("#AnnualPct").val();
        var Annualamt = $("#Annualamt").val();
        var MaximumSalary = $("#MaximumSalary").val();

        if (TableName == 'TBL_DESIGNATION') {
            if (GROUP_NAME.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter name.", 2);
            }
        }
        else if (TableName == 'TBL_LEAVES') {
            if (GROUP_NAME.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter name.", 2);
            }
            else if (Leaves.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter leaves.", 2);
            }
        }
        else if (TableName == 'TBL_SHIFT') {
            if (GROUP_NAME.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter name.", 2);
            }
            else if (TimeIn.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Time In.", 2);
            }
            else if (TimeOut.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Time Out.", 2);
            }
            else if (WHR.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter WHR.", 2);
            }
            else if (GTimeIn.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter G-Time In.", 2);
            }
            else if (GTimeOut.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter G-Time Out.", 2);
            }
            else if (BTimeIn.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter B-Time In.", 2);
            }
            else if (BTimeOut.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter B-Time Out.", 2);
            }
            else if (NightShift.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Night Shift.", 2);
            }
        }
        else if (TableName == 'TBL_TRANSPORT') {
            if (GROUP_NAME.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter name.", 2);
            }
            else if (Driver.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Driver.", 2);
            }
            else if (Capacity.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Capacity.", 2);
            }
            else if (Vehicle.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Vehicle.", 2);
            }
            else if (Route.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Route.", 2);
            }
        }
        else if (TableName == 'TBL_PAY_SCALE') {
            if (GROUP_NAME.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter name.", 2);
            }
            else if (BSalary.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Basic Salary.", 2);
            }
            else if (GSalary.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Gross Salary.", 2);
            }
            else if (AnnualPct.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Annual %.", 2);
            }
            else if (Annualamt.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Annual Amount.", 2);
            }
            else if (MaximumSalary.trim() === '') {
                valid = false;
                empr_helper.notify("Please enter Maximum Salary.", 2);
            }
        }

        
        return valid;
    },

    GetDataToSave: function (TableName) {

        var ID = $("#Code").val();
        var GROUP_NAME = $("#GROUP_NAME").val().trim();
        var LEAVES =     $("#Leaves").val();
        var TIMEIN =     $("#TimeIn").val();
        var TIMEOUT =    $("#TimeOut").val();
        var WHR =        $("#WHR").val();
        var GTIMEIN =    $("#GTimeIn").val();
        var GTIMEOUT =   $("#GTimeOut").val();
        var BTIMEIN =    $("#BTimeIn").val();
        var BTIMEOUT =   $("#BTimeOut").val();
        var NIGHTSHIFT = $("#NightShift").val();
        var Driver =     $("#Driver").val();
        var Capacity =   $("#Capacity").val();
        var Vehicle = $("#Vehicle").val();
        var T_Route = $("#Route").val();
        var BSalary = $("#BSalary").val();
        var GSalary = $("#GSalary").val();
        var AnnualPct = $("#AnnualPct").val();
        var Annualamt = $("#Annualamt").val();
        var MaximumSalary = $("#MaximumSalary").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var modelRecord = {
            GROUP_CODE: ID,
            GROUP_NAME: GROUP_NAME,
            LEAVES: LEAVES,
            TIMEIN: TIMEIN,
            TIMEOUT: TIMEOUT,
            HOURS: WHR,
            GTIMEIN: GTIMEIN,
            GTIMEIN: GTIMEOUT,
            BTIMEIN: BTIMEIN,
            BTIMEOUT: BTIMEOUT,
            NIGHTSHIFT: NIGHTSHIFT,
            ASTATUS: ASTATUS,
            Driver: Driver,
            Capacity: Capacity,
            Vehicle: Vehicle,
            TableName: TableName,
            tRoute: T_Route,
            BSalary: BSalary,
            GSalary: GSalary,
            AnnualPct: AnnualPct,
            Annualamt: Annualamt,
            MaximumSalary: MaximumSalary,
            MasterId: empr_HRMaster.MasterId
        }
        return modelRecord;
    },
    saveAttempt: function (TableName) {
        debugger;
        var obj = empr_HRMaster.GetDataToSave(TableName);

        ajaxHelper.ajaxPostJsonData(obj, "/HRMaster/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_HRMaster.resetForm();
                empr_HRMaster.InitQuickSearch(empr_HRMaster.TableName);
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function (TableName) {
        empr_HRMaster.GetAllHRMasters(TableName);
    },
    InitHRTables: function (selectedValue) {
        console.log(HRMasterTables);
        $('#TNAME').dxSelectBox({
            dataSource: HRMasterTables,
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
                var selectedItem = HRMasterTables.find(x => x.key === selectedKey);
                empr_HRMaster.TableName = selectedItem.tname;
                empr_HRMaster.MasterId =  selectedItem.key;
            }
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
            case 'TBL_DESIGNATION':
                $('#GROUP_NAMES').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_HRMaster.InitQuickSearch(TableName);
                break;
            case 'TBL_LEAVES':
                $('#GROUP_NAMES').show();
                $('#Leavess').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_HRMaster.InitQuickSearch(TableName);
                break;
            case 'TBL_TRANSPORT':
                $('#GROUP_NAMES').show();
                $('#Capacitys').show();
                $('#Drivers').show();
                $('#Vehicles').show();
                $('#Routes').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_HRMaster.InitQuickSearch(TableName);
                break;
            case 'TBL_SHIFT':
                $('#GROUP_NAMES').show();
                $('#TimeIns').show();
                $('#TimeOuts').show();
                $('#WHRS').show();
                $('#GTimeIns').show();
                $('#GTimeOuts').show();
                $('#BTimeIns').show();
                $('#BTimeOuts').show();
                $('#NightShifts').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_HRMaster.InitQuickSearch(TableName);
                break;
            case 'TBL_PAY_SCALE':
                $('#GROUP_NAMES').show();
                $('#BSalarys').show();
                $('#GSalarys').show();
                $('#AnnualPcts').show();
                $('#Annualamts').show();
                $('#MaximumSalarys').show();
                $('.Code').show();
                $('.Status').show();
                $('.card-body').show();
                $('#BtnSave').show();
                empr_HRMaster.InitQuickSearch(TableName);
                break;
            default:
                $('#BtnSave').hide();
                $('.card-body').hide();
                break;
        }
    },
    GetAllHRMasters: function (TableName) {
        ajaxHelper.ajaxGetJson('/HRMaster/QuickSearch?TableName=' + TableName, function (data) {
            empr_HRMaster.CreateGrid(data.data, TableName);
        }, false, true);
    },
    GetHRMasterByID: function (id, tableName) {
        ajaxHelper.ajaxGetJson('/HRMaster/GetHRMasterByID?id=' + id + '&TableName=' + tableName, function (data) {
            empr_HRMaster.resetForm();
            if (data.msgType == 1) {

                var record = data.data;
                console.log(record);
                $("#Code").val(record[0].grouP_CODE);
                $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
                $("#GROUP_NAME").val(record[0].grouP_NAME);
                $("#Leaves").val(record[0].leaves);
                $("#TimeIn").val(record[0].timein);
                $("#TimeOut").val(record[0].timeout);
                $("#WHR").val(record[0].hours);
                $("#GTimeIn").val(record[0].gtimein);
                $("#GTimeOut").val(record[0].gtimeout);
                $("#BTimeIn").val(record[0].btimein);
                $("#BTimeOut").val(record[0].btimeout);
                $("#NightShift").val(record[0].nightshift);
                $("#Driver").val(record[0].driver);
                $("#Capacity").val(record[0].capacity);
                $("#Vehicle").val(record[0].vehicle);
                $("#Route").val(record[0].tRoute);
                $("#BSalary").val(record[0].bsalary);
                $("#GSalary").val(record[0].gsalary);
                $("#AnnualPct").val(record[0].annualPct);
                $("#Annualamt").val(record[0].annualamt);
                $("#MaximumSalary").val(record[0].maximumSalary);

                $('.modal').modal('hide');
                //$('#BtnDelete').show();
                //$('#BtnNew').show();

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
        if (TableName == 'TBL_DESIGNATION') {
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
                { dataField: 'grouP_NAME', caption: 'Name' },
                { dataField: 'astatus', caption: 'Active' },
            ];
            //var col = [
            //    {
            //        dataField: "Action",
            //        width: 100,
            //        alignment: 'center',
            //        fixed: true,
            //        fixedPosition: "left",
            //        allowExporting: false,
            //        cellTemplate: function (container, options) {
            //            debugger
            //            var html = '<div class="btn-group btn-group-sm">';
            //            html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
            //            html += '</div>';
            //            $(html).appendTo(container);
            //        }
            //    },
            //    { dataField: 'grouP_NAME', caption: 'Name' },
            //    { dataField: 'astatus', caption: 'Active' },
            //    { dataField: 'qty', caption: 'Quantity' },
            //    { dataField: 'adD_USER_ID', caption: 'Created By', visible: false },
            //    { dataField: 'adD_DATE', caption: 'Created Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            //    { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false },
            //    { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false },
            //    { dataField: 'adD_POSTALCODE', caption: 'Created PostalCode', visible: false },
            //    { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false },
            //    { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false },
            //    { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false },
            //    { dataField: 'ediT_DATE', caption: 'Updated Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            //    { dataField: 'ediT_POSTALCODE', caption: 'Updated PostalCode', visible: false },
            //];
        }
        else if (TableName == 'TBL_LEAVES') {
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
                { dataField: 'grouP_NAME', caption: 'Name' },
                { dataField: 'leaves', caption: 'Leaves' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        else if (TableName == 'TBL_SHIFT') {
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
                { dataField: 'grouP_NAME', caption: 'Name' },
                { dataField: 'timein', caption: 'Time In' },
                { dataField: 'timeout', caption: 'Time Out' },
                { dataField: 'hours', caption: 'Working Hours' },
                { dataField: 'gtimein', caption: 'Gross Time In' },
                { dataField: 'gtimeout', caption: 'Gross Time Out' },
                { dataField: 'btimein', caption: 'Break Time In' },
                { dataField: 'btimeout', caption: 'Break Time Out' },
                { dataField: 'nightshift', caption: 'Night shift' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        else if (TableName == 'TBL_TRANSPORT') {
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
                { dataField: 'grouP_NAME', caption: 'Name' },
                { dataField: 'capacity', caption: 'Capacity' },
                { dataField: 'driver', caption: 'Driver' },
                { dataField: 'vehicle', caption: 'Vehicle' },
                { dataField: 'tRoute', caption: 'Route' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        else if (TableName == 'TBL_PAY_SCALE') {
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
                { dataField: 'grouP_NAME', caption: 'Name' },
                { dataField: 'bsalary', caption: 'Basic Salary' },
                { dataField: 'gsalary', caption: 'Gross Salary' },
                { dataField: 'annualPct', caption: 'Annual Percentage' },
                { dataField: 'annualamt', caption: 'Annual Amount' },
                { dataField: 'maximumSalary', caption: 'Maximum Salary' },
                { dataField: 'astatus', caption: 'Active' },
            ];
        }
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "HRMaster", 'single');
    },
}