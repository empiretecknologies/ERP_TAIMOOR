var empr_Region = {
    initEvents: function () {

        $(document).ready(function () {

            empr_Region.InitTree();
            empr_Region.InitAccountType();
            empr_Region.InitControlsGridBox(null);

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Region.validateForm()) {
                            empr_Region.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Region.validateForm()) {
                        empr_Region.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {


                empr_Region.InintQuickSearch();


            })

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_Region.GetRegionByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('#resetall').hide();
                empr_Region.resetForm();

            })

            $('.btn-delete').click(function () {

                empr_Region.DeleteRecord();

            })

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#resetall').hide();
                !Permissions.r_VIEW && $('#quicksearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Region/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Region.resetForm();
                    empr_Region.InitTree();
                    empr_Region.reFreshTree();
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('#resetall').hide();
                }
            }, false, true);

        });

    },
    resetForm: function () {


        $("#Code").val('')
        $("#ADD_USER_ID").val('');
        $("#MENU_ID").val();
        $("#ACT_GR_CODE").val('');
        $("#ACT_NAME").val('');
        $("#DEP_TYPE").val('');
        $("#ADDRESS").val('');
        //$("#DEP_TYPE").prop("disabled", false);
        //$("#ADDRESS").prop("disabled", false);
        $("#ACT_PARENT_CODE_hidden").val('');
        $('#ASTATUS').dxSelectBox('option', 'value');
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $('#ACT_TYPE').dxSelectBox('instance').option('value', "C"); // yaha 1
        $("input[type='radio']").prop("disabled", false);
        $('#ASTATUS').dxSelectBox('instance').option('disabled', false);
        $('#ACT_TYPE').dxSelectBox('instance').option('disabled', false);
        $('#controlgridBox').dxDropDownBox('instance').option('disabled', false);
        if ($('#treeListContainer').dxTreeList('instance') != undefined) {
            $('#treeListContainer').dxTreeList('instance').clearFilter();
        }
        empr_Region.InitControlsGridBox();

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        }
    },
    validateForm: function () {

        var valid = true;

        var ACT_NAME = $("#ACT_NAME").val().trim();
        var ACT_PARENT_CODE = $("#ACT_PARENT_CODE_hidden").val().trim();

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var ACT_TYPE = $('#ACT_TYPE').dxSelectBox('option', 'value')

        if (ACT_TYPE == 'S') {
            var DEP_TYPE = $("#DEP_TYPE").val();
            var ADDRESS = $("#ADDRESS").val();
            //if (DEP_TYPE == '') {
            //    valid = false;
            //    empr_helper.notify("Please enter Short Name.", 2);
            //}

            //if (ADDRESS == '') {
            //    valid = false;
            //    empr_helper.notify("Please enter Address.", 2);
            //}
        }

        if (ASTATUS == '') {
            valid = false;
            empr_helper.notify("Please select active.", 2);
        }


        if (ACT_TYPE != 'C') {
            if (ACT_PARENT_CODE == '') {
                valid = false;
                empr_helper.notify("Please select control name.", 2);
            }
        }


        if (ACT_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter region name.", 2);
        }


        return valid;

    },
    getDataToSave: function () {

        var ID = $("#ID").val().trim();
        var ADD_USER_ID = $("#ADD_USER_ID").val()
        var ADD_DATE = $("#ADD_DATE").val()
        var ADD_COMPUTER_NAME = $("#ADD_COMPUTER_NAME").val();
        var ADD_IP_ADDRESS = $("#ADD_IP_ADDRESS").val();
        var ADD_POSTALCODE = $("#ADD_POSTALCODE").val();
        var MENU_ID = $("#MENU_ID").val()
        var ACT_GR_CODE = $("#ACT_GR_CODE").val();
        var ACT_NAME = $("#ACT_NAME").val();
        var ACT_PARENT_CODE = $("#ACT_PARENT_CODE_hidden").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var ACT_TYPE = $('#ACT_TYPE').dxSelectBox('option', 'value')
        var Code = empr_helper.getCode();
        var ACT_CODE = $("#Code").val().trim();
        var DEP_TYPE = $("#DEP_TYPE").val();
        var ADDRESS = $("#ADDRESS").val();
        var modelRecord = {
            ID: ID,
            ADD_USER_ID: ADD_USER_ID,
            ADD_DATE: ADD_DATE,
            ADD_COMPUTER_NAME: ADD_COMPUTER_NAME,
            ADD_IP_ADDRESS: ADD_IP_ADDRESS,
            ADD_POSTALCODE: ADD_POSTALCODE,
            MENU_ID: MENU_ID,
            GR_CODE: ACT_GR_CODE,
            DESCR: ACT_NAME,
            GROUP_TYPE: ACT_TYPE,
            PARENT_CODE: ACT_PARENT_CODE,
            ASTATUS: ASTATUS,
            Code: Code,
            CODE: ACT_CODE,
            ADDRESS: ADDRESS,
            DEP_TYPE: DEP_TYPE
        }
        return modelRecord;

    },
    saveAttempt: function () {

        var obj = empr_Region.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/Region/save", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Region.resetForm();
                empr_Region.InitTree();
                empr_Region.reFreshTree();
                $('#optmodal').modal('hide');
                $('.btn-delete').hide();
                $('#resetall').hide();
            }


        }, false, true);


    },
    InintQuickSearch: function () {


        empr_Region.GetAllRegions();

    },
    GetAllRegions: function () {

        var xhr = ajaxHelper.ajaxGetJson('/Region/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_Region.CreateGrid(data.data);

        }, false, true);

    },
    makeReadOnly: function (isreadonly, type) {
        $('#ACT_TYPE').dxSelectBox('instance').option('disabled', isreadonly);
        $('#controlgridBox').dxDropDownBox('instance').option('disabled', isreadonly);
        //$('#DEP_TYPE').prop('disabled', true);
        //$('#ADDRESS').prop('disabled', true);
    },
    GetRegionByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/Region/RegionByid?id=' + id, function (data) {
            //$('.btn-delete').show();
            //$('#resetall').show();

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
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
            }

            $('.modal').modal('hide')
            empr_Region.makeReadOnly(true);
            $('#Code').val(data.data.id);
            $('#ACT_GR_CODE').val(data.data.acT_GR_CODE);
            $('#DEP_TYPE').val(data.data.deP_TYPE);
            $('#ADDRESS').val(data.data.address);

            $('#ACT_NAME').val(data.data.descr);

            $('#ACT_PARENT_CODE_hidden').val(data.data.parenT_CODE);
            $('#activestatushidden').val(data.data.astatus);
            $('#typehidden').val(data.data.acT_TYPE);

            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            $('#ACT_TYPE').dxSelectBox('instance').option("value", data.data.acT_TYPE);

            empr_Region.InitControlsGridBox(data.data.parenT_CODE);

            $('#resetall').show();

        }, false, true);

    },
    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
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
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);


            }
        },

        { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'acT_GR_CODE', caption: 'GR Code' },
        { dataField: 'descr', caption: 'Name' },
        { dataField: 'deP_TYPE', caption: 'Dep.Type', width: 70, },
        { dataField: 'acT_STATUS', caption: 'Active' },
        { dataField: 'accounT_TYPE', caption: 'Account Type' },
        { dataField: 'parenT_NAME', caption: 'Parent Name' }
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Regions");
    },
    InitTree: function () {

        $.ajax({
            url: 'Region/GetAccountsForTreeView',
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
                        const clickedRowData = info.data;
                        empr_Region.GetRegionByID(clickedRowData.code);
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
    InitAccountType: function () {

        var Datasource = [
            { Value: 'C', Key: 'Control' },
            { Value: 'S', Key: 'Subsidiarity' }
        ];
        empr_Region.bindDxDdl("ACT_TYPE", Datasource, "C", "Value", "Key", "Select", function (d) {

            $('#typehidden').val(d.value)
            if (d.value == null) {
                $('#typehidden').val('');
            }

            if (d.value == 'S') {
                $(".showOnS").show();
            } else {
                $(".showOnS").hide();
            }

        });

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingleForAccountGroup(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitControlsGridBox: function (_selectedValue) {

        debugger;

        if (true) {
            $.ajax({
                url: "Region/GetRegions",
                type: "GET",
                success: function (response) {
                    var Datasource = response.data;

                    selectedObject = [];
                    selectedValue = _selectedValue;

                    if (_selectedValue != null) {
                        selectedObject = Datasource.filter(x => { return x.acT_CODE == _selectedValue }) || [];
                        if (selectedObject.length > 0) {
                            selectedValue = selectedObject[0].acT_CODE;// Datasource.findIndex(x => { return x.acT_CODE == _selectedValue }) == -1 ? 0 : Datasource.findIndex(x => { return x.acT_CODE == _selectedValue })
                            $('#ACT_PARENT_CODE_hidden').val(selectedObject[0].acT_CODE);
                            $('#displayExprcontrol').val(selectedObject[0].acT_NAME);
                        }
                    }

                    empr_Region.bindDxGridBoxDdl('#controlgridBox', Datasource, [{ dataField: 'code', caption: 'Code', width: '60px' }, { dataField: 'descr', caption: 'Name' }, { dataField: 'acT_SNAME', caption: 'Control Name' }], 'hidden', selectedObject, selectedValue, 'code', 'descr', '#displayExprAccParent', function (selectedvalue, hidden) {
                        debugger
                        if (selectedvalue.selectedRowsData.length > 0) {

                            var id = selectedvalue.selectedRowsData[0]['code'];
                            var name = selectedvalue.selectedRowsData[0]['descr'];
                            $('#ACT_PARENT_CODE_hidden').val(id);
                            $('#displayExprAccParent').val(name);

                        }
                        else {
                            $('#ACT_PARENT_CODE_hidden').val('');
                            $('#displayExprAccParent').val('');
                        }

                    });

                }
            });
        }



    },
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {

        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);

    },
    hideshow: function (show) {

        if (show) {

            $('#accounttitle').show();
        } else {

            $('#accounttitle').hide();
        }
    }
}