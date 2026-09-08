var empr_Warehouse = {
    InitEvents: function () {
        $(document).ready(function () {
            empr_Warehouse.InitTree();
            empr_Warehouse.InitAccountType();
            empr_Warehouse.InitControlsGridBox(null);
            
            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Warehouse.ValidateForm()) {
                            empr_Warehouse.SaveAttempt();
                        }
                    }
                } else {
                    if (empr_Warehouse.ValidateForm()) {
                        empr_Warehouse.SaveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_Warehouse.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                empr_Warehouse.GetWarehouseAccountById($(this).attr("reportid"));
            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var name = $(this).attr("reportname");
                swal({
                    title: 'Are you sure you want to Copy this record?',
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
                    $('#updatedName').val(name);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModalName').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, iteM_NAME: $('#updatedName').val() }, "/Warehouse/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_Warehouse.GetWarehouseAccountById(data.data);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                $('#BtnNew').hide();
                empr_Warehouse.ResetForm();
            });

            $('#BtnDelete').click(function () {
                empr_Warehouse.DeleteRecord();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#QuickSearch').hide();
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/Warehouse/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Warehouse.ResetForm();
                    empr_Warehouse.InitTree();
                    empr_Warehouse.RefreshTree();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                    $('#BtnNew').hide();
                }
            }, false, true);

        });
    },
    ResetForm: function () {


        $("#Code").val('');
        $("#DESCR").val('');
        $("#ACT_PARENT_CODE_hidden").val('');
        $("#displayExprcontrol").val(''); 
        $("#CONTACT_PERSON").val('');
        $("#CELL").val('');
        $("#TELL").val('');
        $("#ADDR").val('');
        $('#ASTATUS').dxSelectBox('instance').option("value", 'Y');
        $('#GROUP_TYPE').dxSelectBox('instance').option("value", 'C');
        $('#GROUP_TYPE').dxSelectBox('instance').option('disabled', false);
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }

        empr_Warehouse.HideShow('C');
        empr_Warehouse.InitTree();
        empr_Warehouse.RefreshTree();
        empr_Warehouse.InitControlsGridBox();
    },
    ValidateForm: function () {

        var valid = true;
        var model = empr_Warehouse.GetDataToSave();

        if (model.DESCR == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        if (model.GROUP_TYPE != 'C') {
            if (model.PARENT_CODE == '') {
                valid = false;
                empr_helper.notify("Please select control name.", 2);
            }

/*            if (model.CONTACT_PERSON == '') {
                valid = false;
                empr_helper.notify("Please enter contact person.", 2);
            }

            if (model.CELL == '') {
                valid = false;
                empr_helper.notify("Please enter cell number.", 2);
            }*/
        }
        return valid;
    },
    GetDataToSave: function () {

        var code = $("#Code").val().trim();
        var descr = $("#DESCR").val().trim();
        var groupType = $("#GROUP_TYPE").dxSelectBox('instance').option('value');
        var astatus = $("#ASTATUS").dxSelectBox('instance').option('value');
        var parentCode = $("#ACT_PARENT_CODE_hidden").val();
        var contactPerson = $("#CONTACT_PERSON").val().trim();
        var cell = $("#CELL").val().trim();
        var tell = $("#TELL").val().trim();
        var address = $("#ADDR").val().trim();
        var modelRecord = {
            CODE: code,
            DESCR: descr,
            GROUP_TYPE: groupType,
            PARENT_CODE: parentCode,
            CONTACT_PERSON: contactPerson,
            CELL: cell,
            TELL: tell,
            ADDR: address,
            ASTATUS: astatus
        }
        return modelRecord;
    },
    SaveAttempt: function () {
        var obj = empr_Warehouse.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/Warehouse/save", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Warehouse.ResetForm();
                empr_Warehouse.InitTree();
                empr_Warehouse.RefreshTree();
                $('#optmodal').modal('hide');
                $('#BtnDelete').hide();
                $('BtnNew').hide();
            }
        }, false, true);
    },
    InitQuickSearch: function () {
        empr_Warehouse.GetAllAccounts();
    },
    GetAllAccounts: function () {
        ajaxHelper.ajaxGetJson('/Warehouse/QuickSearch', function (data) {
            empr_Warehouse.CreateGrid(data.data);
        }, false, true);
    },
    GetWarehouseAccountById: function (id) {
        ajaxHelper.ajaxGetJson('/Warehouse/GetWarehouseAccountById?code=' + id, function (data) {

            if (data.msgType == 1) {
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
                
                $('.modal').modal('hide')
                empr_Warehouse.HideShow(data.data.grouP_TYPE);
                $('#GROUP_TYPE').dxSelectBox('instance').option("value", data.data.grouP_TYPE);
                $('#Code').val(data.data.code);
                $('#DESCR').val(data.data.descr);
                $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
                $("#CONTACT_PERSON").val(data.data.contacT_PERSON);
                $("#CELL").val(data.data.cell);
                $("#TELL").val(data.data.tell);
                $("#ADDR").val(data.data.addr);
                $('#GROUP_TYPE').dxSelectBox('instance').option('disabled', true);

                empr_Warehouse.InitControlsGridBox(data.data.parenT_CODE);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc);
        var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    debugger

                    $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
                                <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportname="${options.data.descr}" reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>
                                </div>`).appendTo(container);
                }
            },
            { dataField: 'descr', caption: 'Name' },
            { dataField: 'gR_CODE', caption: 'GR CODE', visible: false },
            { dataField: 'parenT_CODE', caption: 'Parent' },
            {
                dataField: "astatus",
                caption: "Active",
                width: 100,
                visible: true,
                alignment: "center",
                calculateCellValue: function (rowData) {
                    if (rowData.astatus == 'Y') {
                        return "Active";
                    } else {
                        return "In-Active";
                    }
                }
            },  
            {
                dataField: "grouP_TYPE",
                caption: "Type",
                width: 100,
                visible: true,
                alignment: "center",
                calculateCellValue: function (rowData) {
                    if (rowData.grouP_TYPE == 'C') {
                        return "Control";
                    } else if (rowData.grouP_TYPE == 'S') {
                        return "Subsidiarity";
                    }
                }
            },
            { dataField: 'contacT_PERSON', caption: 'Contact Person' },
            { dataField: 'tell', caption: 'Tel #.' },
            { dataField: 'cell', caption: 'Cell #.' },
            { dataField: 'addr', caption: 'Address' },
            { dataField: 'adD_USER_ID', caption: 'Add USER', visible: false },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Add COMPUTER NAME', visible: false },
            { dataField: 'adD_DATE', caption: 'Add DATE', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'adD_IP_ADDRESS', caption: 'Add IP ADDRESS', visible: false },
            { dataField: 'adD_POSTALCODE', caption: 'Add POSTALCODE', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Edit USER', visible: false },
            { dataField: 'ediT_DATE', caption: 'Edit DATE', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Edit COMPUTER NAME', visible: false },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Edit IP ADDRESS', visible: false },
            { dataField: 'ediT_POSTALCODE', caption: 'Edit POSTALCODE', visible: false },                  
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "WarehouseQS");
    },
    InitTree: function () {

        $.ajax({
            url: 'Warehouse/GetWarehouseAccountsForTreeView',
            method: 'GET',
            data: { Code: empr_helper.getCode() },
            success: function (data) {
                console.log(data);
                $('#treeListContainer').dxTreeList({
                    dataSource: data.data,
                    keyExpr: 'code',
                    parentIdExpr: 'parenT_CODE',
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
                            dataField: 'descr',
                            caption: 'Name'
                        }
                    ],
                    expandedRowKeys: [0],
                    showRowLines: true,
                    onRowDblClick: function (info) {
                        empr_Warehouse.GetWarehouseAccountById(info.data.code);
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    RefreshTree: function () {
        if ($('#treeListContainer').dxTreeList('instance') != undefined) {
            $('#treeListContainer').dxTreeList('instance').clearFilter();
        }
        $("#treeListContainer").dxTreeList("instance").refresh();
    },
    InitAccountType: function () {

        var Datasource = [
            { Value: 'C', Key: 'Control' },
            { Value: 'S', Key: 'Subsidiarity' }
        ];
        empr_Warehouse.BindDxDdl("GROUP_TYPE", Datasource, "C", "Value", "Key", "Select", function (d) {

            $('#typehidden').val(d.value)
            if (d.value == null) {
                $('#typehidden').val('');
            }
            empr_Warehouse.HideShow(d.value); 
        });
    },
    BindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitControlsGridBox: function (_selectedValue) {
        $.ajax({
            url: "Warehouse/GetAccounts",
            type: "GET",
            success: function (response) {
                var Datasource = response;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.key == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].key;
                        $('#ACT_PARENT_CODE_hidden').val(selectedObject[0].key);
                        $('#displayExprcontrol').val(selectedObject[0].value);
                    }
                }

                empr_Warehouse.BindDxGridBoxDdl('#PARENT_CODE', Datasource, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }], 'hidden', selectedObject, selectedValue, 'key', 'value', '#displayExprcontrol', function (selectedvalue, hidden) {
                    debugger
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var id = selectedvalue.selectedRowsData[0]['key'];
                        var name = selectedvalue.selectedRowsData[0]['value'];
                        $('#ACT_PARENT_CODE_hidden').val(id);
                        $('#displayExprcontrol').val(name);
                    }
                    else {
                        $('#ACT_PARENT_CODE_hidden').val('');
                        $('#displayExprcontrol').val('');
                    }
                });
            }
        });
    },
    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {

        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);

    },
    HideShow: function (value) {
        if (value == "S") {
            $("#CONTACT_PERSON").val('');
            $("#CELL").val('');
            $("#TELL").val('');
            $("#ADDR").val('');
            $('.Dynamic').show();
        } else {
            $("#CONTACT_PERSON").val('');
            $("#CELL").val('');
            $("#TELL").val('');
            $("#ADDR").val('');
            $('.Dynamic').hide();
        }
    }
}