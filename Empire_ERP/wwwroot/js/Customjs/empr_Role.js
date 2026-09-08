var empr_Role = {
    dataGridInstance: "",
    selectedModule: 1,

    //for Role Management
    rolesData: [],
    approval: [],
    filtered: [],
    filteredApproval: [],
    selectedId: 0,

    //for Report Format
    reportsData: [],

    //for Chart of Account
    accountsData: [],

    //for Item Group
    itemsData: [],

    //for party Types
    partyData: [],

    //Roles By ID
    rolesByID: [],

    initEvents: function () {
        $(document).ready(function () {

            empr_Role.InitRoleTypeDDL();
            empr_Role.InitModuleDDL(1);
            empr_Role.InitBranchDDL();
            empr_Role.InitGetAll();
            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Role.validateForm()) {
                            empr_Role.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Role.validateForm()) {
                        empr_Role.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_Role.InitQuickSearch();
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_Role.resetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Role.DeleteRecord();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_Role.GetRolesByCode(id);
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },

    InitGetAll: function () {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        empr_Role.InitMainMenue();
        empr_Role.rolesData = [];
        empr_Role.approval = [];
        empr_Role.reportsData = [];
        empr_Role.accountsData = [];
        empr_Role.itemsData = [];
        empr_Role.partyData = [];
        empr_Role.filtered = [];
        empr_Role.filteredApproval = [];
        ajaxHelper.ajaxGetJson('/Role/GetAllPermissions', function (data) {
            empr_Role.rolesData = data.data.filter(data => data.modulE_ID == 1);
            empr_Role.approval = data.data.filter(data => data.modulE_ID == 5);
            empr_Role.reportsData = data.data.filter(data => data.modulE_ID == 2);
            empr_Role.accountsData = data.data.filter(data => data.modulE_ID == 3);
            empr_Role.itemsData = data.data.filter(data => data.modulE_ID == 4);
            empr_Role.partyData = data.data.filter(data => data.modulE_ID == 6);
            console.log(empr_Role.partyData);

            if (empr_Role.rolesByID.length > 0) {
                empr_Role.updatePermissions();
            }
            $('#roleManagement').show();
            $('#approval').hide();
            $('#reportFormat').hide();
            $('#itemGroup').hide();
            $('#chartOfAcc').hide();
            $('#partyType').hide();

            empr_Role.LoadMenues(1, 1);

            $('#MODULE_ID').dxSelectBox('instance').option('value', 1);

            $("#Loader").hide();
        }, false, true);
    },

    InitQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/Role/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_Role.CreateGridQuickSearch(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGridQuickSearch: function (dataSrc) {
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
                                   <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.rolE_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   </div>`).appendTo(container);
                }
            },
            { dataField: 'rolE_ID', caption: 'Id', visible: false },
            { dataField: 'rolE_NAME', caption: 'Name' },
            { dataField: 'rolE_TYPE', caption: 'Type' }
        ];
        empr_helper.dxGridbindingVouchers('#gridContainerQuickSearch', col, dataSrc, "Roles", "single");
    },

    GetRolesByCode: function (code) {
        ajaxHelper.ajaxGetJson('/Role/GetRoleById?id=' + code, function (data) {
            if (data.msgType == 1) {
                empr_Role.rolesByID = [];
                empr_Role.rolesByID = data.data;
                const name = empr_Role.rolesByID.length > 0 ? empr_Role.rolesByID[0].rolE_NAME : "";
                const showSelected = empr_Role.rolesByID.length > 0 ? empr_Role.rolesByID[0].shoW_SELECTED : "";
                const type = empr_Role.rolesByID.length > 0 ? empr_Role.rolesByID[0].rolE_TYPE : "";
                $('#ROLE_NAME').val(name);
                $("#SHOW_SELECTED").prop("checked", showSelected == 0 ? false : true);
                $('#ROLE_TYPE').dxSelectBox('instance').option('value', type);
                const distinctBranchIds = [...new Set(empr_Role.rolesByID.map(row => row.branch))];
                empr_Role.InitBranchDDL(distinctBranchIds);
                $('#branch_hidden').val(distinctBranchIds.join(','));

                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
                    }
                    if (Permissions.r_ADD) {
                        $('#BtnNew').show();
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
                    $('#BtnNew').show();
                }
                $('#moduleDiv').show();
                $('#ROLE_NAME').attr('readonly', true);
                $('#ROLE_TYPE').dxSelectBox('instance').option('disabled', true);

                empr_Role.InitGetAll();
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    LoadMenues(grcode, pageid) {
        var selectedType = $("#ROLE_TYPE").dxSelectBox('instance').option('value');
        if (selectedType == "U") {
            empr_Role.selectedId = grcode;
            empr_Role.GetMenues();
            empr_Role.GetMenuesForApproval();
        } else {
            empr_Role.selectedId = pageid;
            empr_Role.GetMenues();
            empr_Role.GetMenuesForApproval();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Role/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Role.resetForm();
                    empr_Role.InitMainMenue();
                    $('#BtnDelete').hide();
                    $('#moduleDiv').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);
        });

    },

    resetForm: function () {
        $("#Code").val('');
        $("#ROLE_NAME").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $('#ROLE_TYPE').dxSelectBox('instance').option('value', "U");
        $('#MODULE_ID').dxSelectBox('instance').option('value', 1);
        $('#moduleDiv').hide();
        empr_Role.rolesByID = [];
        empr_Role.InitGetAll();

        $('#ROLE_NAME').attr('readonly', false);
        $('#ROLE_TYPE').dxSelectBox('instance').option('disabled', false);

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
        var ROLE_NAME = $("#ROLE_NAME").val();
        var BRANCH = $('#branch_hidden').val();

        if (ROLE_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        if (BRANCH == '' || BRANCH == null) {
            valid = false;
            empr_helper.notify("Please select branch.", 2);
        }

        const PERMISSIONS = empr_Role.rolesData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy);
        if (PERMISSIONS.length == 0) {
            valid = false;
            empr_helper.notify("Please select atleast one permission.", 2);
        }

        return valid;
    },

    GetDataToSave: function () {

        var branches = $('#branch_hidden').val();

        var ID = $("#Code").val();
        var ROLE_NAME = $("#ROLE_NAME").val().trim();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var ROLE_TYPE = $("#ROLE_TYPE").dxSelectBox('instance').option('value');
        var BRANCH = branches.split(',').map(Number);
        var SHOW_SELECTED = $('#SHOW_SELECTED').is(':checked') ? 1 : 0;
        const PERMISSIONS = empr_Role.rolesData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy);
        const APPROVAL = empr_Role.approval.filter(record => record.add);
        const REPORTPERMISSIONS = empr_Role.reportsData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy);
        const ITEMPERMISSIONS = empr_Role.itemsData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy);
        const ACCPERMISSIONS = empr_Role.accountsData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy);
        const PARTYPERMISSIONS = empr_Role.partyData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy);
        const mergedPermissions = [...PERMISSIONS, ...APPROVAL, ...REPORTPERMISSIONS, ...ITEMPERMISSIONS, ...ACCPERMISSIONS, ...PARTYPERMISSIONS];
        //const mergedPermissions = [...PERMISSIONS, ...REPORTPERMISSIONS, ...ITEMPERMISSIONS, ...ACCPERMISSIONS, ...PARTYPERMISSIONS.slice(0, 945), ...PARTYPERMISSIONS.slice(946)];
        //console.log(empr_Role.partyData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy)[945])
        //console.log(empr_Role.partyData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy)[944])
        //console.log(empr_Role.partyData.filter(record => record.add || record.edit || record.view || record.delete || record.print || record.copy)[946])
        var modelRecord = {
            ROLE_ID: ID,
            ROLE_NAME: ROLE_NAME,
            ROLE_TYPE: ROLE_TYPE,
            ASTATUS: ASTATUS,
            BRANCH: BRANCH,
            SHOW_SELECTED: SHOW_SELECTED,
            PERMISSIONS: mergedPermissions
        }
        return modelRecord;
    },

    saveAttempt: function () {
        $("#Loader").css('display', 'flex').show();

        setTimeout(function () {
            var obj = empr_Role.GetDataToSave();
            ajaxHelper.ajaxPostJsonData(obj, "/Role/Save", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    $("#Code").val(parseInt(data.data));
                    $('#moduleDiv').show();
                    $('#ROLE_NAME').attr('readonly', true);
                    $('#ROLE_TYPE').dxSelectBox('instance').option('disabled', true);

                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                        if (Permissions.r_ADD) {
                            $('#BtnNew').show();
                        }
                        if (Permissions.r_EDIT) {
                            $('#BtnSave').show();
                        } else {
                            $('#BtnSave').hide();
                        }
                    } else {
                        $('#BtnSave').show();
                        $('#BtnDelete').show();
                        $('#BtnNew').show();
                    }
                }
                $("#Loader").hide();
            }, false, true);
        }, 10);
    },

    InitMainMenue: function () {
        empr_Role.GetMainMenue();
    },

    GetMainMenue: function () {
        ajaxHelper.ajaxGetJson('/Role/GetMainMenue', function (data) {
            empr_Role.CreateGrid(data.data);
        }, false, true);
    },

    CreateGrid: function (dataSrc) {
        var col = [
            { dataField: 'menU_NAME', caption: 'Module Wise Data' }
        ];
        empr_helper.dxGridbindingWithoutFeatures('#menuContainer', col, dataSrc, "SetupType", 'single');
        empr_helper.dxGridbindingWithoutFeatures('#approvalmenuContainer', col, dataSrc, "SetupType", 'single');
    },

    GetAllReportFormats: function () {
        empr_Role.CreatePermissionGrid(empr_Role.reportsData, "#reportFormatContainer", false);
    },

    GetAllItems: function () {
        empr_Role.CreatePermissionGrid(empr_Role.itemsData, "#itemGroupContainer", false);
    },

    GetAllPartyTypes: function () {
        empr_Role.CreatePermissionGrid(empr_Role.partyData, "#partyTypeContainer", false);
    },

    GetAllChartOfAcc: function () {
        empr_Role.CreatePermissionGrid(empr_Role.accountsData, "#chartOfAccContainer", true);
    },

    GetMenues: function () {
        if ($("#ROLE_TYPE").dxSelectBox('instance').option('value') == "U") {
            const prefix = empr_Role.selectedId;
          //empr_Role.filtered = empr_Role.rolesData.filter(record => record.grcode.startsWith(prefix));
            empr_Role.filtered = empr_Role.rolesData.filter(record => record.grcode.startsWith(prefix)).sort((a, b) => a.grcode.localeCompare(b.grcode));
        }
        else {
            empr_Role.filtered = empr_Role.rolesData.filter(record => record.menU_PARENT_CODE == empr_Role.selectedId);
        }
        console.log('filtered data == 1', empr_Role.filtered);
        empr_Role.CreatePermissionGrid(empr_Role.filtered, "#gridContainer", false);
    },

    GetMenuesForApproval: function () {
        if ($("#ROLE_TYPE").dxSelectBox('instance').option('value') == "U") {
            const prefix = empr_Role.selectedId;
            empr_Role.filteredApproval = empr_Role.approval.filter(record => record.grcode.startsWith(prefix)).sort((a, b) => a.grcode.localeCompare(b.grcode));
        }
        else {
            empr_Role.filteredApproval = empr_Role.approval.filter(record => record.menU_PARENT_CODE == empr_Role.selectedId);
        }
        console.log('filtered data == 5', empr_Role.filteredApproval);
        empr_Role.CreatePermissionGrid(empr_Role.filteredApproval, "#approvalgridContainer", false);
    },

    updatePermissions: function () {
        empr_Role.rolesData = empr_Role.rolesData.map(record => {
            const filteredPage = empr_Role.rolesByID.find(item => item.id === record.id && item.modulE_ID === record.modulE_ID);
            if (filteredPage) {
                return {
                    ...record,
                    add: filteredPage.add,
                    edit: filteredPage.edit,
                    view: filteredPage.view,
                    delete: filteredPage.delete,
                    copy: filteredPage.copy,
                    print: filteredPage.print,
                    rowselection: filteredPage.rowselection,
                };
            }
            return record;
        });
        empr_Role.approval = empr_Role.approval.map(record => {
            const filteredPage = empr_Role.rolesByID.find(item => item.id === record.id && item.modulE_ID === record.modulE_ID);
            if (filteredPage) {
                return {
                    ...record,
                    add: filteredPage.add,
                    //edit: filteredPage.edit,
                    //view: filteredPage.view,
                    //delete: filteredPage.delete,
                    //copy: filteredPage.copy,
                    //print: filteredPage.print,
                    rowselection: filteredPage.rowselection,
                };
            }
            return record;
        });

        empr_Role.reportsData = empr_Role.reportsData.map(record => {
            const filteredReport = empr_Role.rolesByID.find(item => item.id === record.id && item.modulE_ID === record.modulE_ID);
            if (filteredReport) {
                return {
                    ...record,
                    add: filteredReport.add,
                    edit: filteredReport.edit,
                    view: filteredReport.view,
                    delete: filteredReport.delete,
                    copy: filteredReport.copy,
                    print: filteredReport.print,
                    rowselection: filteredReport.rowselection,
                };
            }
            return record;
        });

        empr_Role.accountsData = empr_Role.accountsData.map(record => {
            const filteredAcc = empr_Role.rolesByID.find(item => item.id === record.id && item.modulE_ID === record.modulE_ID);
            if (filteredAcc) {
                return {
                    ...record,
                    add: filteredAcc.add,
                    edit: filteredAcc.edit,
                    view: filteredAcc.view,
                    delete: filteredAcc.delete,
                    copy: filteredAcc.copy,
                    print: filteredAcc.print,
                    rowselection: filteredAcc.rowselection,
                };
            }
            return record;
        });

        empr_Role.itemsData = empr_Role.itemsData.map(record => {
            const filteredItem = empr_Role.rolesByID.find(item => item.id === record.id && item.modulE_ID === record.modulE_ID);
            if (filteredItem) {
                return {
                    ...record,
                    add: filteredItem.add,
                    edit: filteredItem.edit,
                    view: filteredItem.view,
                    delete: filteredItem.delete,
                    copy: filteredItem.copy,
                    print: filteredItem.print,
                    rowselection: filteredItem.rowselection,
                };
            }
            return record;
        });

        empr_Role.partyData = empr_Role.partyData.map(record => {
            const filteredParty = empr_Role.rolesByID.find(item => item.id === record.id && item.modulE_ID === record.modulE_ID && item.acT_CODE === record.acT_CODE);
            if (filteredParty) {
                return {
                    ...record,
                    add: filteredParty.add,
                    edit: filteredParty.edit,
                    view: filteredParty.view,
                    delete: filteredParty.delete,
                    copy: filteredParty.copy,
                    print: filteredParty.print,
                    rowselection: filteredParty.rowselection,
                };
            }
            return record;
        });
    },

    

    CreatePermissionGrid: function (dataSrc, div, isTree, limitedColumns) {
        
        var col = [
            { dataField: 'id', visible: false },
            {
                dataField: 'rowselection',
                caption: '',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });

                    $('<input class="RowCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            empr_Role.HandleRowSelectionCheckBox(isChecked, options.data.id, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="RowCheckboxH checkbox_animated" type="checkbox">');
                    $header.append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        $(div).find('.RowCheckbox').prop('checked', $('.RowCheckboxH').is(':checked'));
                        var isChecked = $('.RowCheckboxH').is(':checked');
                        empr_Role.HandleRowSelectionCheckBoxHeader(isChecked);
                    });
                },
            },
            { dataField: 'menU_NAME', caption: 'Menu' },
            {
                dataField: 'add',
                caption: 'Add',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });

                    $('<input class="AddCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            var rowSelection = isChecked && options.data.edit && options.data.view && options.data.delete && options.data.copy && options.data.print;
                            empr_Role.HandleCheckboxChange('add', isChecked, options.data.id, rowSelection, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="AddCheckboxH checkbox_animated" type="checkbox">');
                    if (empr_Role.selectedModule == 5) {
                        $header.append('<span>Approval </span>').append($headerCheckbox);
                    } else {
                        $header.append('<span>Add </span>').append($headerCheckbox);
                    }
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        var isChecked = $('.AddCheckboxH').is(':checked');
                        $(div).find('.AddCheckbox').prop('checked', isChecked);
                        empr_Role.HandleHeaderCheckboxChange('add', isChecked, ['edit', 'view', 'delete', 'copy', 'print']);
                    });
                },
            },
            {
                dataField: 'edit',
                caption: 'Edit',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });
                    $('<input class="EditCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            var rowSelection = isChecked && options.data.add && options.data.view && options.data.delete && options.data.copy && options.data.print;
                            empr_Role.HandleCheckboxChange('edit', isChecked, options.data.id, rowSelection, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="EditCheckboxH checkbox_animated" type="checkbox">');
                    $header.append('<span>Edit </span>').append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        var isChecked = $('.EditCheckboxH').is(':checked');
                        $(div).find('.EditCheckbox').prop('checked', isChecked);
                        empr_Role.HandleHeaderCheckboxChange('edit', isChecked, ['add', 'view', 'delete', 'copy', 'print']);
                    });
                },
            },
            {
                dataField: 'view',
                caption: 'View',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });
                    $('<input class="ViewCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            var rowSelection = isChecked && options.data.add && options.data.edit && options.data.delete && options.data.copy && options.data.print;
                            empr_Role.HandleCheckboxChange('view', isChecked, options.data.id, rowSelection, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="ViewCheckboxH checkbox_animated" type="checkbox">');
                    $header.append('<span>View </span>').append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        var isChecked = $('.ViewCheckboxH').is(':checked');
                        $(div).find('.ViewCheckbox').prop('checked', isChecked);
                        empr_Role.HandleHeaderCheckboxChange('view', isChecked, ['add', 'edit', 'delete', 'copy', 'print']);
                    });
                },
            },
            {
                dataField: 'delete',
                caption: 'Delete',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });
                    $('<input class="DeleteCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            var rowSelection = isChecked && options.data.add && options.data.edit && options.data.view && options.data.copy && options.data.print;
                            empr_Role.HandleCheckboxChange('delete', isChecked, options.data.id, rowSelection, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="DeleteCheckboxH checkbox_animated" type="checkbox">');
                    $header.append('<span>Delete </span>').append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        var isChecked = $('.DeleteCheckboxH').is(':checked');
                        $(div).find('.DeleteCheckbox').prop('checked', isChecked);
                        empr_Role.HandleHeaderCheckboxChange('delete', isChecked, ['edit', 'view', 'add', 'copy', 'print']);
                    });
                },
            },
            {
                dataField: 'print',
                caption: 'Print',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });
                    $('<input class="PrintCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            var rowSelection = isChecked && options.data.add && options.data.edit && options.data.view && options.data.copy && options.data.delete;
                            empr_Role.HandleCheckboxChange('print', isChecked, options.data.id, rowSelection, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="PrintCheckboxH checkbox_animated" type="checkbox">');
                    $header.append('<span>Print </span>').append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        var isChecked = $('.PrintCheckboxH').is(':checked');
                        $(div).find('.PrintCheckbox').prop('checked', isChecked);
                        empr_Role.HandleHeaderCheckboxChange('print', isChecked, ['edit', 'view', 'delete', 'copy', 'add']);
                    });
                },
            },
            {
                dataField: 'copy',
                caption: 'Copy',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });
                    $('<input class="CopyCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            var rowSelection = isChecked && options.data.add && options.data.edit && options.data.view && options.data.print && options.data.delete;
                            empr_Role.HandleCheckboxChange('copy', isChecked, options.data.id, rowSelection, options.data.menU_PARENT_CODE, options.data.acT_CODE);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="CopyCheckboxH checkbox_animated" type="checkbox">');
                    $header.append('<span>Copy </span>').append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        var isChecked = $('.CopyCheckboxH').is(':checked');
                        $(div).find('.CopyCheckbox').prop('checked', isChecked);
                        empr_Role.HandleHeaderCheckboxChange('copy', isChecked, ['edit', 'view', 'delete', 'add', 'print']);
                    });
                },
            }
        ];

        if (div === '#approvalgridContainer') {
            col = col.filter((item, index) => [0, 2, 3].includes(index));
        }

        if (isTree) {
            empr_Role.InitTree(div, col, dataSrc, "Permissions");
        } else {
            empr_Role.dxGridbindingForReports(div, col, dataSrc, "Permissions");
        }
    },

    dxGridbindingForReports: function (div, columns, datasrc, fileName) {
        dataGridInstance = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "rowAlternationEnabled": true,
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": "none"
            },
            "editing": {
                "mode": "row",
                "allowUpdating": false,
                "allowAdding": false,
                "allowDeleting": false,
            },
            onContentReady: function (e) {
                if (empr_Role.selectedModule == 1) {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.filtered);
                }
                else if (empr_Role.selectedModule == 5) {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.filteredApproval);
                }
                else if (empr_Role.selectedModule == 2) {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.reportsData);
                }
                else if (empr_Role.selectedModule == 3) {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.accountsData);
                }
                else if (empr_Role.selectedModule == 6) {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.partyData);
                } else {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.itemsData);
                }

                if ($(div + ' .RowCheckboxH').is(':checked')) {
                    $(div).find('.RowCheckbox').prop('checked', $('.RowCheckboxH').is(':checked'));
                }
                if ($(div + ' .AddCheckboxH').is(':checked')) {
                    $(div).find('.AddCheckbox').prop('checked', $('.AddCheckboxH').is(':checked'));
                }
                if ($(div + ' .EditCheckboxH').is(':checked')) {
                    $(div).find('.EditCheckbox').prop('checked', $('.EditCheckboxH').is(':checked'));
                }
                if ($(div + ' .ViewCheckboxH').is(':checked')) {
                    $(div).find('.ViewCheckbox').prop('checked', $('.ViewCheckboxH').is(':checked'));
                }
                if ($(div + ' .DeleteCheckboxH').is(':checked')) {
                    $(div).find('.DeleteCheckbox').prop('checked', $('.DeleteCheckboxH').is(':checked'));
                }
                if ($(div + ' .PrintCheckboxH').is(':checked')) {
                    $(div).find('.PrintCheckbox').prop('checked', $('.PrintCheckboxH').is(':checked'));
                }
                if ($(div + ' .CopyCheckboxH').is(':checked')) {
                    $(div).find('.CopyCheckbox').prop('checked', $('.CopyCheckboxH').is(':checked'));
                }
            },
            onRowClick: function (e) {
                e.event.stopPropagation();
            },
            onCellClick: function (e) {
                e.event.stopPropagation();
            },
        }).dxDataGrid('instance');
    },

    InitTree: function (div, columns, datasrc, fileName) {
        dataGridInstance = $(div).dxTreeList({
            "dataSource": datasrc,
            "keyExpr": 'id',
            "parentIdExpr": 'menU_PARENT_CODE',
            "columns": columns,
            "remoteOperations": false,
            "rowAlternationEnabled": true,
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": "none"
            },
            "editing": {
                "mode": "row",
                "allowUpdating": false,
                "allowAdding": false,
                "allowDeleting": false,
            },
            onContentReady: function (e) {

                if (empr_Role.selectedModule == 3) {
                    empr_Role.HeaderCheckBoxes(div, empr_Role.accountsData);
                }

                if ($(div + ' .RowCheckboxH').is(':checked')) {
                    $(div).find('.RowCheckbox').prop('checked', $('.RowCheckboxH').is(':checked'));
                }
                if ($(div + ' .AddCheckboxH').is(':checked')) {
                    $(div).find('.AddCheckbox').prop('checked', $('.AddCheckboxH').is(':checked'));
                }
                if ($(div + ' .EditCheckboxH').is(':checked')) {
                    $(div).find('.EditCheckbox').prop('checked', $('.EditCheckboxH').is(':checked'));
                }
                if ($(div + ' .ViewCheckboxH').is(':checked')) {
                    $(div).find('.ViewCheckbox').prop('checked', $('.ViewCheckboxH').is(':checked'));
                }
                if ($(div + ' .DeleteCheckboxH').is(':checked')) {
                    $(div).find('.DeleteCheckbox').prop('checked', $('.DeleteCheckboxH').is(':checked'));
                }
                if ($(div + ' .PrintCheckboxH').is(':checked')) {
                    $(div).find('.PrintCheckbox').prop('checked', $('.PrintCheckboxH').is(':checked'));
                }
                if ($(div + ' .CopyCheckboxH').is(':checked')) {
                    $(div).find('.CopyCheckbox').prop('checked', $('.CopyCheckboxH').is(':checked'));
                }
            },
        }).dxTreeList('instance');
    },

    HeaderCheckBoxes(div, dataSrc) {
        $(div).find('.RowCheckboxH').prop('checked', empr_Role.checkEachRow('rowselection', dataSrc))
        $(div).find('.AddCheckboxH').prop('checked', empr_Role.checkEachRow('add', dataSrc))
        $(div).find('.EditCheckboxH').prop('checked', empr_Role.checkEachRow('edit', dataSrc))
        $(div).find('.ViewCheckboxH').prop('checked', empr_Role.checkEachRow('view', dataSrc))
        $(div).find('.DeleteCheckboxH').prop('checked', empr_Role.checkEachRow('delete', dataSrc))
        $(div).find('.PrintCheckboxH').prop('checked', empr_Role.checkEachRow('print', dataSrc))
        $(div).find('.CopyCheckboxH').prop('checked', empr_Role.checkEachRow('copy', dataSrc))
    },

    checkEachRow(feild, dataSrc) {
        var allTrue = true;
        $.each(dataSrc, function (index, obj) {
            if (obj[feild] !== true) {
                allTrue = false;
                return false;
            }
        });
        return allTrue;
    },

    MultipleDxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.MultipleDxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    HandleCheckboxChange(columnName, isChecked, recordId, rowSelection, parentCode, actCODE) {
        if (empr_Role.selectedModule == 1) {
            empr_Role.HandleChildPageCheckboxChange(columnName, isChecked, recordId);
            empr_Role.rolesData = empr_Role.rolesData.map(record => {
                if (record.id === recordId) {
                    const updatedRecord = { ...record, [columnName]: isChecked, rowselection: rowSelection && isChecked };
                    return updatedRecord;
                }
                return record;
            });
            empr_Role.CheckPageCheckBoxesByColumnName(columnName, parentCode);
            empr_Role.GetMenues();
        }
        else if (empr_Role.selectedModule == 5) {
            empr_Role.HandleChildPageCheckboxChange(columnName, isChecked, recordId);
            empr_Role.approval = empr_Role.approval.map(record => {
                if (record.id === recordId) {
                    const updatedRecord = { ...record, [columnName]: isChecked, rowselection: rowSelection && isChecked };
                    return updatedRecord;
                }
                return record;
            });
            empr_Role.CheckPageCheckBoxesByColumnName(columnName, parentCode);
            empr_Role.GetMenuesForApproval();
        }
        else if (empr_Role.selectedModule == 2) {
            empr_Role.reportsData = empr_Role.reportsData.map(record => {
                if (record.id === recordId) {
                    const updatedRecord = { ...record, [columnName]: isChecked, rowselection: rowSelection && isChecked };
                    return updatedRecord;
                }
                return record;
            });
            empr_Role.GetAllReportFormats();
        }
        else if (empr_Role.selectedModule == 3) {
            empr_Role.HandleChildAccountsCheckboxChange(columnName, isChecked, recordId);
            empr_Role.accountsData = empr_Role.accountsData.map(record => {
                if (record.id === recordId) {
                    const updatedRecord = { ...record, [columnName]: isChecked, rowselection: rowSelection && isChecked };
                    return updatedRecord;
                }
                return record;
            });
            empr_Role.CheckAccCheckBoxesByColumnName(columnName, parentCode);
            empr_Role.GetAllChartOfAcc();
        }
        else if (empr_Role.selectedModule == 4) {
            empr_Role.itemsData = empr_Role.itemsData.map(record => {
                if (record.id === recordId) {
                    const updatedRecord = { ...record, [columnName]: isChecked, rowselection: rowSelection && isChecked };
                    return updatedRecord;
                }
                return record;
            });
            empr_Role.GetAllItems();
        }
        else if (empr_Role.selectedModule == 6) {
            empr_Role.partyData = empr_Role.partyData.map(record => {
                if (record.id === recordId && record.acT_CODE === actCODE) {
                    const updatedRecord = { ...record, [columnName]: isChecked, rowselection: rowSelection && isChecked };
                    return updatedRecord;
                }
                return record;
            });
            console.log(empr_Role.partyData)
            empr_Role.GetAllPartyTypes();
        }
    },

    HandleHeaderCheckboxChange(columnName, isChecked, otherColumns) {
        if (empr_Role.selectedModule == 1) {
            const idsToUpdate = empr_Role.filtered.map(record => record.id);
            empr_Role.rolesData = empr_Role.rolesData.map(record => {
                if (idsToUpdate.includes(record.id)) {
                    const othersChecked = otherColumns.every(col => record[col]);
                    if (isChecked && othersChecked) {
                        return { ...record, [columnName]: isChecked, rowselection: isChecked };
                    } else {
                        return { ...record, [columnName]: isChecked, rowselection: false };
                    }
                }
                return record;
            });
            empr_Role.GetMenues();
        }
        else if (empr_Role.selectedModule == 5) {
            const idsToUpdate = empr_Role.filteredApproval.map(record => record.id);
            empr_Role.approval = empr_Role.approval.map(record => {
                if (idsToUpdate.includes(record.id)) {
                    const othersChecked = otherColumns.every(col => record[col]);
                    if (isChecked && othersChecked) {
                        return { ...record, [columnName]: isChecked, rowselection: isChecked };
                    } else {
                        return { ...record, [columnName]: isChecked, rowselection: false };
                    }
                }
                return record;
            });
            empr_Role.GetMenuesForApproval();
        }
        else if (empr_Role.selectedModule == 2) {
            empr_Role.reportsData = empr_Role.reportsData.map(record => {
                const othersChecked = otherColumns.every(col => record[col]);
                if (isChecked && othersChecked) {
                    return { ...record, [columnName]: isChecked, rowselection: isChecked };
                } else {
                    return { ...record, [columnName]: isChecked, rowselection: false };
                }
            });
            empr_Role.GetAllReportFormats();
        }
        else if (empr_Role.selectedModule == 3) {
            empr_Role.accountsData = empr_Role.accountsData.map(record => {
                const othersChecked = otherColumns.every(col => record[col]);
                if (isChecked && othersChecked) {
                    return { ...record, [columnName]: isChecked, rowselection: isChecked };
                } else {
                    return { ...record, [columnName]: isChecked, rowselection: false };
                }
            });
            empr_Role.GetAllChartOfAcc();
        }
        else if (empr_Role.selectedModule == 4) {
            empr_Role.itemsData = empr_Role.itemsData.map(record => {
                const othersChecked = otherColumns.every(col => record[col]);
                if (isChecked && othersChecked) {
                    return { ...record, [columnName]: isChecked, rowselection: isChecked };
                } else {
                    return { ...record, [columnName]: isChecked, rowselection: false };
                }
            });
            empr_Role.GetAllItems();
        }
        else if (empr_Role.selectedModule == 6) {
            empr_Role.partyData = empr_Role.partyData.map(record => {
                const othersChecked = otherColumns.every(col => record[col]);
                if (isChecked && othersChecked) {
                    return { ...record, [columnName]: isChecked, rowselection: isChecked };
                } else {
                    return { ...record, [columnName]: isChecked, rowselection: false };
                }
            });
            empr_Role.GetAllPartyTypes();
        }
    },

    CheckAccCheckBoxesByColumnName(columnName, parentId) {
        const anyChecked = empr_Role.accountsData
            .filter(record => record.menU_PARENT_CODE === parentId)
            .some(record => record[columnName] === true);

        const rowChecked = empr_Role.accountsData
            .filter(record => record.menU_PARENT_CODE === parentId)
            .some(record => record.rowselection === true);

        var newParentId = 0;
        empr_Role.accountsData = empr_Role.accountsData.map(record => {
            if (record.id === parentId) {
                newParentId = record.menU_PARENT_CODE;
                return { ...record, [columnName]: anyChecked, rowselection: rowChecked };
            }
            return record;
        });
        if (newParentId > 0) {
            empr_Role.CheckAccCheckBoxesByColumnName(columnName, newParentId);
        }
    },

    HandleChildAccountsCheckboxChange(columnName, isChecked, id) {
        let newIds = [];

        empr_Role.accountsData = empr_Role.accountsData.map(record => {
            if (record.menU_PARENT_CODE === id) {
                newIds.push(record.id);
                return { ...record, [columnName]: isChecked };
            }
            return record;
        });

        if (newIds.length > 0) {
            newIds.forEach(newId => {
                empr_Role.HandleChildAccountsCheckboxChange(columnName, isChecked, newId);
            });
        }
    },

    CheckPageCheckBoxesByColumnName(columnName, parentId) { // first column header ka checkbox
        if (empr_Role.selectedModule == 1) {
            const anyChecked = empr_Role.rolesData
                .filter(record => record.menU_PARENT_CODE === parentId)
                .some(record => record[columnName] === true);

            const rowChecked = empr_Role.rolesData
                .filter(record => record.menU_PARENT_CODE === parentId)
                .some(record => record.rowselection === true);

            var newParentId = 0;
            empr_Role.rolesData = empr_Role.rolesData.map(record => {
                if (record.id === parentId) {
                    newParentId = record.menU_PARENT_CODE;
                    return { ...record, [columnName]: anyChecked, rowselection: rowChecked };
                }
                return record;
            });
        }
        else if (empr_Role.selectedModule == 5) {
            const anyChecked = empr_Role.approval
                .filter(record => record.menU_PARENT_CODE === parentId)
                .some(record => record[columnName] === true);

            const rowChecked = empr_Role.approval
                .filter(record => record.menU_PARENT_CODE === parentId)
                .some(record => record.rowselection === true);

            var newParentId = 0;
            empr_Role.approval = empr_Role.approval.map(record => {
                if (record.id === parentId) {
                    newParentId = record.menU_PARENT_CODE;
                    return { ...record, [columnName]: anyChecked, rowselection: rowChecked };
                }
                return record;
            });
        }
        
        if (newParentId > 0) {
            empr_Role.CheckPageCheckBoxesByColumnName(columnName, newParentId);
        }
    },

    HandleChildPageCheckboxChange(columnName, isChecked, id) {
        let newIds = [];

        if (empr_Role.selectedModule == 1) {
            empr_Role.rolesData = empr_Role.rolesData.map(record => {
                if (record.menU_PARENT_CODE === id) {
                    newIds.push(record.id);
                    return { ...record, [columnName]: isChecked };
                }
                return record;
            });
        }
        else if (empr_Role.selectedModule == 5) {
            empr_Role.approval = empr_Role.approval.map(record => {
                if (record.menU_PARENT_CODE === id) {
                    newIds.push(record.id);
                    return { ...record, [columnName]: isChecked };
                }
                return record;
            });
        }
        

        if (newIds.length > 0) {
            newIds.forEach(newId => {
                empr_Role.HandleChildPageCheckboxChange(columnName, isChecked, newId);
            });
        }
    },

    HandleRowSelectionCheckBox(isChecked, id, menU_PARENT_CODE, actCode) {
        if (empr_Role.selectedModule == 1) {
            empr_Role.rolesData = empr_Role.rolesData.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                return record;
            });
            empr_Role.CheckPageCheckBoxesByColumnName('rowselection', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('add', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('edit', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('view', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('delete', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('print', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('copy', menU_PARENT_CODE);

            empr_Role.HandleChildPageCheckboxChange('rowselection', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('add', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('edit', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('view', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('delete', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('print', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('copy', isChecked, id);
            empr_Role.GetMenues();
        }
        else if (empr_Role.selectedModule == 5) {
            debugger;
            empr_Role.approval = empr_Role.approval.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked, add: isChecked};
                }
                return record;
            });
            empr_Role.CheckPageCheckBoxesByColumnName('rowselection', menU_PARENT_CODE);
            empr_Role.CheckPageCheckBoxesByColumnName('add', menU_PARENT_CODE);

            empr_Role.HandleChildPageCheckboxChange('rowselection', isChecked, id);
            empr_Role.HandleChildPageCheckboxChange('add', isChecked, id);
            empr_Role.GetMenuesForApproval();
        }
        else if (empr_Role.selectedModule == 2) {
            empr_Role.reportsData = empr_Role.reportsData.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                if (record.id === menU_PARENT_CODE) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                return record;
            });
            empr_Role.GetAllReportFormats();
        }
        else if (empr_Role.selectedModule == 3) {
            empr_Role.accountsData = empr_Role.accountsData.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                return record;
            });
            empr_Role.CheckAccCheckBoxesByColumnName('rowselection', menU_PARENT_CODE);
            empr_Role.CheckAccCheckBoxesByColumnName('add', menU_PARENT_CODE);
            empr_Role.CheckAccCheckBoxesByColumnName('edit', menU_PARENT_CODE);
            empr_Role.CheckAccCheckBoxesByColumnName('view', menU_PARENT_CODE);
            empr_Role.CheckAccCheckBoxesByColumnName('delete', menU_PARENT_CODE);
            empr_Role.CheckAccCheckBoxesByColumnName('print', menU_PARENT_CODE);
            empr_Role.CheckAccCheckBoxesByColumnName('copy', menU_PARENT_CODE);

            empr_Role.HandleChildAccountsCheckboxChange('rowselection', isChecked, id);
            empr_Role.HandleChildAccountsCheckboxChange('add', isChecked, id);
            empr_Role.HandleChildAccountsCheckboxChange('edit', isChecked, id);
            empr_Role.HandleChildAccountsCheckboxChange('view', isChecked, id);
            empr_Role.HandleChildAccountsCheckboxChange('delete', isChecked, id);
            empr_Role.HandleChildAccountsCheckboxChange('print', isChecked, id);
            empr_Role.HandleChildAccountsCheckboxChange('copy', isChecked, id);
            empr_Role.GetAllChartOfAcc();
        }
        else if (empr_Role.selectedModule == 4) {
            empr_Role.itemsData = empr_Role.itemsData.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                if (record.id === menU_PARENT_CODE) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                return record;
            });
            empr_Role.GetAllItems();
        }
        else if (empr_Role.selectedModule == 6) {
            empr_Role.partyData = empr_Role.partyData.map(record => {
                if (record.id === id && record.acT_CODE === actCode) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                return record;
            });
            empr_Role.GetAllPartyTypes();
        }
    },

    HandleRowSelectionCheckBoxHeader(isChecked) {
        debugger;
        if (empr_Role.selectedModule == 1) {
            const idsToUpdate = empr_Role.filtered.map(record => record.id);
            empr_Role.rolesData = empr_Role.rolesData.map(record => {
                if (idsToUpdate.includes(record.id)) {
                    return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
                }
                return record;
            });
            empr_Role.GetMenues();
        }
        else if (empr_Role.selectedModule == 5) {
            const idsToUpdate = empr_Role.filtered.map(record => record.id);
            empr_Role.approval = empr_Role.approval.map(record => {
                if (idsToUpdate.includes(record.id)) {
                    return { ...record, rowselection: isChecked, add: isChecked };
                }
                return record;
            });
            empr_Role.GetMenuesForApproval();
        }
        else if (empr_Role.selectedModule == 2) {
            empr_Role.reportsData = empr_Role.reportsData.map(record => {
                return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
            });
            empr_Role.GetAllReportFormats();
        }
        else if (empr_Role.selectedModule == 3) {
            empr_Role.accountsData = empr_Role.accountsData.map(record => {
                return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
            });
            empr_Role.GetAllChartOfAcc();
        }
        else if (empr_Role.selectedModule == 4) {
            empr_Role.itemsData = empr_Role.itemsData.map(record => {
                return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
            });
            empr_Role.GetAllItems();
        }
        else if (empr_Role.selectedModule == 6) {
            empr_Role.partyData = empr_Role.partyData.map(record => {
                return { ...record, rowselection: isChecked, add: isChecked, edit: isChecked, view: isChecked, delete: isChecked, copy: isChecked, print: isChecked };
            });
            empr_Role.GetAllPartyTypes();
        }
    },

    InitRoleTypeDDL: function () {
        $('#ROLE_TYPE').dxSelectBox({
            dataSource: [
                { value: 'U', text: 'User' },
                { value: 'M', text: 'Manager' }
            ],
            valueExpr: 'value',
            displayExpr: 'text',
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onInitialized: function (e) {
                e.component.option('value', 'U');
            }
        });
    },

    InitModuleDDL: function (selectedValue) {

        $.ajax({
            url: 'Role/GetModules',
            method: 'GET',
            data: null,
            success: function (data) {
                $('#MODULE_ID').dxSelectBox({
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
                    onValueChanged: function (e) {
                        empr_Role.selectedModule = e.value;
                        $('#roleManagement').hide();
                        $('#approval').hide();
                        $('#reportFormat').hide();
                        $('#itemGroup').hide();
                        $('#chartOfAcc').hide();
                        $('#partyType').hide();
                        if (e.value == 1) {
                            $('#roleManagement').show();
                            $('#showHide').hide();
                        }
                        else if (e.value == 2) {
                            empr_Role.GetAllReportFormats();
                            $('#reportFormat').show();
                            $('#showHide').hide();
                        }
                        else if (e.value == 4) {
                            empr_Role.GetAllItems();
                            $('#itemGroup').show();
                            $('#showHide').show();
                        }
                        if (e.value == 5) {//1
                            $('#approval').show();
                            $('#showHide').hide();
                        }
                        else if (e.value == 6) {
                            empr_Role.GetAllPartyTypes();
                            $('#partyType').show();
                            $('#showHide').show();
                        }
                        else if (e.value == 3) {
                            empr_Role.GetAllChartOfAcc();
                            $('#chartOfAcc').show();
                            $('#showHide').show();
                        }
                        else {
                        }
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitBranchDDL: function (selectedBranch) {
        $.ajax({
            url: 'Role/GetBranches',
            method: 'GET',
            success: function (data) {
                var filteredData = [];
                if (selectedBranch != null) {
                    filteredData = data.filter(item => selectedBranch.includes(item.key));
                }
                empr_Role.MultipleDxGridBoxDropdown('#BRANCH', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', filteredData, selectedBranch, 'key', 'value', "#displayExpr_branch", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(', ');
                        $('#branch_hidden').val(keys);
                        $('#displayExpr_branch').val(values);
                        empr_Role.isValueAssigned = false;
                    }
                    else {
                        $('#branch_hidden').val('');
                        $('#displayExpr_branch').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
}