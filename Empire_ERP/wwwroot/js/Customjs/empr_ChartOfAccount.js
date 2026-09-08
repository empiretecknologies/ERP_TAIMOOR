var empr_ChartOfAccount = {
    selectedAccount: "",
    initEvents: function () {

        $(document).ready(function () {
            console.log('ChartType', ChartType);
            $('#PassDiv').hide();
            //Chart Type
            empr_ChartOfAccount.InitChartTypeDDL();

            empr_ChartOfAccount.InitTree();
            empr_ChartOfAccount.InitChequeDDL();
            empr_ChartOfAccount.InitAccountType();
            empr_ChartOfAccount.InitCurrency();


            empr_ChartOfAccount.InitAccountGroupGridBox(null);
            empr_ChartOfAccount.InitAccountNatureGridBox(null);
            empr_ChartOfAccount.InitControlsGridBox(null);

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ChartOfAccount.validateForm()) {
                            empr_ChartOfAccount.saveAttempt();
                        }
                    }
                } else {
                    if (empr_ChartOfAccount.validateForm()) {
                        empr_ChartOfAccount.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {

                empr_ChartOfAccount.InintQuickSearch();

            })

            $('body').on('click', '.elm_edit', function () {

                var rportid = $(this).attr("rportid")
                empr_ChartOfAccount.GetChartOfAccountByID(rportid);

            })

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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, acT_NAME: $('#updatedName').val() }, "/ChartOfAccount/CopyRecord", function (data) {
                    console.log(data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_ChartOfAccount.GetChartOfAccountByID(data.data.acT_CODE);
                    }
                }, false, true);
            });

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                //$('#resetall').hide();
                empr_ChartOfAccount.resetForm();
                $('#CHART_TYPE').dxSelectBox('instance').reset();

            })

            $('.btn-delete').click(function () {


                empr_ChartOfAccount.DeleteRecord();

            })

            //$('').click(function () {

            //    empr_ChartOfAccount.InitControlsGridBox();

            //})

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/ChartOfAccount/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ChartOfAccount.resetForm();
                    empr_ChartOfAccount.InitTree();
                    empr_ChartOfAccount.reFreshTree();
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    //$('#resetall').hide();
                    $('#CHART_TYPE').dxSelectBox('instance').reset();


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
        $("#ACT_SNAME").val('');
        $("#ACT_PARENT_CODE_hidden").val('');
        $("#ACT_GROUP_hidden").val('');
        $("#ACT_nature_hidden").val('');
        $('#ASTATUS').dxSelectBox('option', 'value');
        $("#PREFIX").val('');
        $("#ACCOUNT_NO").val('');
        $("#TITTLE").val('');
        $("#SWIFT").val('');
        $('#flexSwitchCheckDefault').prop('checked', false);
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $('#ACT_TYPE').dxSelectBox('instance').option('value', "C");
        $("#CHQ_ID").dxSelectBox('instance').option('value', null);
        $("#CURRENCY").dxSelectBox('instance').option('value', null);
        $("#PREFIX,  #SWIFT, #CHQ_ID, #CURRENCY, #ACT_NATURE,  input[type='radio']").prop("disabled", false);
        $('#ASTATUS').dxSelectBox('instance').option('disabled', false);
        $('#ACT_TYPE').dxSelectBox('instance').option('disabled', false);
        $('#accountnaturegridBox').dxDropDownBox('instance').option('disabled', false);
        $('#controlgridBox').dxDropDownBox('instance').option('disabled', false);
        $("#CURRENCY").dxSelectBox('instance').option('disabled', false);
        $('#flexSwitchCheckDefault').attr('disabled', false);



        //chart type
        $('#CHART_TYPE').dxSelectBox('instance').option('disabled', false);


        if ($('#treeListContainer').dxTreeList('instance') != undefined) {
            $('#treeListContainer').dxTreeList('instance').clearFilter();
        }
        empr_ChartOfAccount.InitAccountGroupGridBox();
        empr_ChartOfAccount.InitAccountNatureGridBox();
        empr_ChartOfAccount.InitControlsGridBox();

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        }
    },

    resetAfterSaveForm: function () {
        $("#Code").val('');
        $("#PASS").val('');
        $("#ACT_NAME").val('');
        $("#ACT_SNAME").val('');
        empr_ChartOfAccount.resetForm();

        $('#controlgridBox').dxDropDownBox('instance').reset();

        $('#CHART_TYPE').dxSelectBox('instance').option('value', null);
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

        //var ID = $("#ID").val().trim();
        //var ADD_USER_ID = $("#ADD_USER_ID").val()
        //var ADD_DATE = $("#ADD_DATE").val()
        //var ADD_COMPUTER_NAME = $("#ADD_COMPUTER_NAME").val();
        //var ADD_IP_ADDRESS = $("#ADD_IP_ADDRESS").val();
        //var ADD_POSTALCODE = $("#ADD_POSTALCODE").val();
        //var MENU_ID = $("#MENU_ID").val()
        //var ACT_GR_CODE = $("#ACT_GR_CODE").val();
        var ACT_NAME = $("#ACT_NAME").val().trim();
        var ACT_SNAME = $("#ACT_SNAME").val().trim();
        var ACT_PARENT_CODE = $("#ACT_PARENT_CODE_hidden").val().trim();
        var ACT_GROUP = $("#ACT_GROUP_hidden").val().trim();
        var ACT_NATURE = $("#ACT_nature_hidden").val().trim();
        var PREFIX = $("#PREFIX").val().trim();
        var ACCOUNT_NO = $("#ACCOUNT_NO").val();
        var PASS = $("#PASS").val();
        var TITTLE = $("#TITTLE").val().trim();
        var SWIFT = $("#SWIFT").val().trim();

        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');;
        var ACT_TYPE = $('#ACT_TYPE').dxSelectBox('option', 'value');
        var CHQ_ID = $("#CHQ_ID").dxSelectBox('instance').option('value');
        var CURRENCY = $("#CURRENCY").dxSelectBox('instance').option('value');

        //Chart Type
        var CHART_TYPE = $('#CHART_TYPE').dxSelectBox('instance').option('value');

        if (!CHART_TYPE) {
            valid = false;
            empr_helper.notify("Please enter chart Type", 2)
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
            empr_helper.notify("Please enter account name.", 2);
        }


        if (ACT_GROUP == '') {
            valid = false;
            empr_helper.notify("Please enter account group.", 2);
        }

        if (PASS && PASS.length < 8) {
            valid = false;
            empr_helper.notify("Password must be at least 8 characters long or should be left empty.", 2);
        }

        if (ACT_NATURE == '') {
            valid = false;
            empr_helper.notify("Please enter account nature.", 2);
        }

        if (ACT_NATURE == 2) {
            // if  nature is bank.
            if (PREFIX == '') {
                valid = false;
                empr_helper.notify("Please enter prefix.", 2);
            }

            if (TITTLE == '') {
                valid = false;
                empr_helper.notify("Please enter title.", 2);
            }

            if (ACCOUNT_NO == '') {
                valid = false;
                empr_helper.notify("Please enter account number.", 2);
            }

            if (CURRENCY == '') {
                valid = false;
                empr_helper.notify("Please enter currency.", 2);
            }

            //if (CHQ_ID == '') {
            //    valid = false;
            //    empr_helper.notify("Please enter cheque.", 2);
            //}

            if (SWIFT == '') {
                valid = false;
                empr_helper.notify("Please enter swift.", 2);
            }
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
        var ACT_SNAME = $("#ACT_SNAME").val()
        var ACT_PARENT_CODE = $("#ACT_PARENT_CODE_hidden").val();
        var ACT_GROUP = $("#ACT_GROUP_hidden").val();
        var ACT_NATURE = $("#ACT_nature_hidden").val()
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value')
        var PREFIX = $("#PREFIX").val()
        var ACCOUNT_NO = $("#ACCOUNT_NO").val()
        var TITTLE = $("#TITTLE").val()
        var SWIFT = $("#SWIFT").val()
        var ACT_TYPE = $('#ACT_TYPE').dxSelectBox('option', 'value')
        var CHQ_ID = $("#CHQ_ID").dxSelectBox('instance').option('value')
        var CURRENCY = $("#CURRENCY").dxSelectBox('instance').option('value')
        var Code = empr_helper.getCode();
        var ACT_CODE = $("#Code").val().trim();
        var PASS = $("#PASS").val();
        var costcenter = $('#flexSwitchCheckDefault').is(':checked') ? 1 : 0;
        //chart type save 
        var chartType = $('#CHART_TYPE').dxSelectBox('instance').option('value')
        var modelRecord = {
            ID: ID,
            ADD_USER_ID: ADD_USER_ID,
            ADD_DATE: ADD_DATE,
            ADD_COMPUTER_NAME: ADD_COMPUTER_NAME,
            ADD_IP_ADDRESS: ADD_IP_ADDRESS,
            ADD_POSTALCODE: ADD_POSTALCODE,
            MENU_ID: MENU_ID,
            ACT_GR_CODE: ACT_GR_CODE,
            ACT_NAME: ACT_NAME,
            ACT_SNAME: ACT_SNAME,
            ACT_TYPE: ACT_TYPE,
            ACT_PARENT_CODE: ACT_PARENT_CODE,
            ACT_GROUP: ACT_GROUP,
            ACT_NATURE: ACT_NATURE,
            ASTATUS: ASTATUS,
            PREFIX: PREFIX,
            ACCOUNT_NO: ACCOUNT_NO,
            TITTLE: TITTLE,
            SWIFT: SWIFT,
            CHQ_ID: CHQ_ID,
            Code: Code,
            CURRENCY: CURRENCY,
            ACT_CODE: ACT_CODE,
            PASS: PASS,
            //chart type
            CHART_TYPE: chartType,
            COSTCENTER: costcenter
        }
        return modelRecord;

    },
    saveAttempt: function () {

        var obj = empr_ChartOfAccount.getDataToSave();

        empr_ChartOfAccount.selectedAccount = obj.ACT_PARENT_CODE;
        console.log(obj)
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/ChartOfAccount/save", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_ChartOfAccount.resetAfterSaveForm();
                empr_ChartOfAccount.InitTree();
                empr_ChartOfAccount.reFreshTree();

                empr_ChartOfAccount.InitControlsGridBox(empr_ChartOfAccount.selectedAccount);
                $('#optmodal').modal('hide');
                $('.btn-delete').hide();
                //$('#resetall').hide();
            }
        }, false, true);

    },
    InintQuickSearch: function () {
        //empr_ChartOfAccount.CreateGrid();
        empr_ChartOfAccount.GetAllChartOfAccounts();
    },
    GetAllChartOfAccounts: function () {

        var xhr = ajaxHelper.ajaxGetJson('/ChartOfAccount/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {

            empr_ChartOfAccount.CreateGrid(data.data);

        }, false, true);

    },
    makeReadOnly: function (isreadonly, type) {
        $("#PREFIX, #CHQ_ID, #CURRENCY, #ACT_NATURE,  input[type='radio']").prop("disabled", isreadonly);
        $('#ACT_TYPE').dxSelectBox('instance').option('disabled', isreadonly);
        $('#accountnaturegridBox').dxDropDownBox('instance').option('disabled', isreadonly);
        $("#CURRENCY").dxSelectBox('instance').option('disabled', isreadonly);
        $('#controlgridBox').dxDropDownBox('instance').option('disabled', isreadonly);
        $('#flexSwitchCheckDefault').attr('disabled', isreadonly);
        $('#CHART_TYPE').dxSelectBox('instance').option('disabled', isreadonly);


    },
    GetChartOfAccountByID: function (id) {

        var xhr = ajaxHelper.ajaxGetJson('/ChartOfAccount/ChartOfAccountByid?id=' + id, function (data) {
            console.log('data', data);
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
            Permissions.r_ADD && $('#resetall').show();
            $('.modal').modal('hide')
            empr_ChartOfAccount.makeReadOnly(true);
            $('#Code').val(data.data.id);
            $('#ACT_GR_CODE').val(data.data.acT_GR_CODE);

            $('#ACT_NAME').val(data.data.acT_NAME);
            $('#ACT_SNAME').val(data.data.acT_SNAME);
            $('#PREFIX').val(data.data.prefix);
            $('#ACCOUNT_NO').val(data.data.accounT_NO);
            $('#TITTLE').val(data.data.tittle);
            $('#SWIFT').val(data.data.swift);

            $('#activestatushidden').val(data.data.astatus);
            $('#typehidden').val(data.data.acT_TYPE);
            $('#currencyhidden').val(data.data.currency);
            $('#chequehidden').val(data.data.chQ_ID);
            $('#PASS').val(data.data.pass);

            $("#flexSwitchCheckDefault").prop("checked", data.data.costcenter == 0 ? false : true);
            $('#CURRENCY').dxSelectBox('instance').option("value", parseInt(data.data.currency));
            $('#CHQ_ID').dxSelectBox('instance').option("value", parseInt(data.data.chQ_ID));
            $('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            $('#ACT_TYPE').dxSelectBox('instance').option("value", data.data.acT_TYPE);

            //add chart type
            //$('#CHART_TYPE').dxSelectBox('instance').option("value", data.data.charT_TYPE);
            empr_ChartOfAccount.InitChartTypeDDL(data.data.charT_TYPE);
            console.log('Chart Typesss:', data.data.charT_TYPE);


            if (data.data.acT_TYPE == "C") {
                $('#PassDiv').hide();
            } else {
                $('#PassDiv').show();
            }

            if (data.data.acT_NATURE == 2) {
                empr_ChartOfAccount.hideshow(true);
            } else {
                empr_ChartOfAccount.hideshow(false);
            }

            empr_ChartOfAccount.InitControlsGridBox(data.data.acT_PARENT_CODE);
            empr_ChartOfAccount.InitAccountGroupGridBox(data.data.acT_GROUP);
            empr_ChartOfAccount.InitAccountNatureGridBox(data.data.acT_NATURE);



        }, false, true);

    },
    CreateGrid: function (dataSrc) {
        console.log("datttaa:", dataSrc);
        console.log("chartType", dataSrc.charT_TYPE)
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);
                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                                <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportname=${options.data.acT_NAME} reportid=${options.data.id} title="COPY"><i class="fa fa-copy"></i></a>
                                </div>`).appendTo(container);
            }
        },

        { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'acT_GR_CODE', caption: 'Account GR Code' },
        { dataField: 'acT_NAME', caption: 'Account Name' },
        { dataField: 'acT_SNAME', caption: 'Account Short Name' },
        { dataField: 'acT_NATURE_NAME', caption: 'Account Nature' },
        { dataField: 'acT_STATUS', caption: 'Active' },
        { dataField: 'accounT_TYPE', caption: 'Account Type' },
        { dataField: 'chQ_ID', caption: 'Cheque ID' },
        { dataField: 'accounT_NO', caption: 'Account No' },
        { dataField: 'currency', caption: 'Currency' },
        //add chart type
        { dataField: 'charT_TYPE', caption: 'Chart Type' },






        { dataField: 'tittle', caption: 'Title' },
        { dataField: 'parenT_NAME', caption: 'Parent Name', visible: false },
        { dataField: 'acT_GROUP_NAME', caption: 'Group Name', visible: false },
        { dataField: 'prefix', caption: 'Prefix', visible: false },
        { dataField: 'swift', caption: 'Swift', visible: false },

        //{ dataField: 'adD_USER_ID', caption: 'Add USER', visible: false },
        //{ dataField: 'adD_DATE', caption: 'Add DATE', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
        //{ dataField: 'adD_COMPUTER_NAME', caption: 'Add COMPUTER NAME', visible: false },
        //{ dataField: 'adD_IP_ADDRESS', caption: 'Add IP ADDRESS', visible: false },
        //{ dataField: 'ediT_USER_ID', caption: 'Edit USER', visible: false },
        //{ dataField: 'ediT_DATE', caption: 'Edit DATE', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
        //{ dataField: 'ediT_COMPUTER_NAME', caption: 'Edit COMPUTER NAME', visible: false },
        //{ dataField: 'ediT_IP_ADDRESS', caption: 'Edit IP ADDRESS', visible: false },
        //{ dataField: 'adD_POSTALCODE', caption: 'Add POSTALCODE', visible: false },
        //{ dataField: 'ediT_POSTALCODE', caption: 'Edit POSTALCODE', visible: false },
        {
            dataField: "costcenter",
            caption: "Cost Center",
            width: 60,
            visible: true,
            alignment: "center",
            calculateCellValue: function (rowData) {
                if (rowData.costcenter == '0') {
                    return "No"
                } else {
                    return "Yes"
                }
            }
        },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ChartOfAccounts");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', col, "/ChartOfAccount/QuickSearchLazyLoading", "id", "ChartOfAccounts");
    },
    InitTree: function () {
        $.ajax({
            url: 'ChartOfAccount/GetAccountsForTreeView',
            method: 'GET',
            data: { Code: empr_helper.getCode() },
            success: function (data) {
                $('#treeListContainer').dxTreeList({
                    dataSource: data.data,
                    keyExpr: 'acT_CODE',
                    parentIdExpr: 'acT_PARENT_CODE',
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
                            dataField: 'acT_NAME',
                            caption: 'Name'
                        }
                    ],
                    expandedRowKeys: [0],
                    showRowLines: true,
                    onRowDblClick: function (info) {
                        const clickedRowData = info.data;
                        empr_ChartOfAccount.GetChartOfAccountByID(clickedRowData.acT_CODE);
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
    InitChequeDDL: function () {

        empr_ChartOfAccount.bindDxDdl("CHQ_ID", JSON.parse(Cheque), null, "Key", "Value", "Select", function (d) {

            $('#chequehidden').val(d.value)
            if (d.value == null) {
                $('#chequehidden').val('');
            }

        });

    },
    InitAccountType: function () {

        var Datasource = [
            { Value: 'C', Key: 'Control' },
            { Value: 'S', Key: 'Subsidiarity' }
        ];
        empr_ChartOfAccount.bindDxDdl("ACT_TYPE", Datasource, "C", "Value", "Key", "Select", function (d) {

            $('#typehidden').val(d.value)
            if (d.value == null) {
                $('#typehidden').val('');
            }

            if (d.value == "C") {
                $('#PassDiv').hide();
            } else {
                $('#PassDiv').show();
            }

        });

    },
    InitCurrency: function () {

        empr_ChartOfAccount.bindDxDdl("CURRENCY", Currency, null, "key", "value", "Select", function (d) {

            $('#currencyhidden').val(d.value)
            if (d.value == null) {
                $('#currencyhidden').val('');
            }

        });

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitAccountGroupGridBox: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "AccountGroup/GetAccountGroups",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.code == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].code;
                        $('#ACT_GROUP_hidden').val(selectedObject[0].code);
                        $('#displayExprAccGroup').val(selectedObject[0].descr);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#accountgroupgrdiBox").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "code",
                    //displayExpr: function (item) {
                    //    return item ? `${item.descr}` : "Select a value...";
                    //},
                    displayExpr: "descr",
                    placeholder: "Select a value...",
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: true,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.code === e.value);
                            if (selectedData) {
                                $('#ACT_GROUP_hidden').val(selectedData.code);
                                $('#displayExprAccGroup').val(selectedData.descr);
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

                        //if (!e.component.option("opened")) {
                        //    isProgrammaticOpen = true;
                        //    e.component.open();
                        //    setTimeout(() => { isProgrammaticOpen = false; }, 100);
                        //}
                        if (!e.component.option("opened") && !isProgrammaticOpen) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 150);
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
                                    dataField: "code",
                                    caption: "Code",
                                    width: '60px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "descr",
                                    caption: "Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "grouP_TYPE",
                                    caption: "Control Name",
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
                                    $('#displayExprAccGroup').val(selected.descr);
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
                            ["descr", "contains", searchTerm],
                            "or",
                            ["code", "contains", searchTerm],
                            "or",
                            ["grouP_TYPE", "contains", searchTerm]
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


                $(document).on("dxclick", "#accountgroupgrdiBox .dx-clear-button-area", function (e) {
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



    InitAccountNatureGridBox: function (_selectedValue) {
        console.log(_selectedValue)

        $.ajax({
            url: "AccountNature/GetAccountNature",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.grouP_CODE == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].grouP_CODE;
                        $('#ACT_nature_hidden').val(selectedObject[0].grouP_CODE);
                        $('#displayExprAccnature').val(selectedObject[0].grouP_NAME);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#accountnaturegridBox").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "grouP_CODE",
                    //displayExpr: function (item) {
                    //    return item ? `${item.grouP_NAME}` : "Select a value...";
                    //},
                    displayExpr: "grouP_NAME",
                    placeholder: "Select a value...",
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: true,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.grouP_CODE === e.value);
                            if (selectedData) {
                                $('#ACT_nature_hidden').val(selectedData.grouP_CODE);
                                $('#displayExprAccnature').val(selectedData.grouP_NAME);

                                if (selectedData.grouP_CODE == 2) {
                                    empr_ChartOfAccount.hideshow(true);
                                } else {
                                    empr_ChartOfAccount.hideshow(false);
                                }
                            }
                        } else {
                            $('#ACT_nature_hidden').val('');
                            $('#displayExprAccnature').val('');
                            empr_ChartOfAccount.hideshow(false);
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
                                key: "grouP_CODE"
                            }),
                            columns: [
                                {
                                    dataField: "grouP_CODE",
                                    caption: "Code",
                                    width: '60px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "grouP_NAME",
                                    caption: "Name",
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
                                    e.component.option("value", selected.grouP_CODE);
                                    e.component.close();

                                    // Update hidden and display fields
                                    $('#ACT_nature_hidden').val(selected.grouP_CODE);
                                    $('#displayExprAccnature').val(selected.grouP_NAME);

                                    // Show/hide based on selection
                                    if (selected.grouP_CODE == 2) {
                                        empr_ChartOfAccount.hideshow(true);
                                    } else {
                                        empr_ChartOfAccount.hideshow(false);
                                    }
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].grouP_CODE], false);
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
                            ["grouP_NAME", "contains", searchTerm],
                            "or",
                            ["grouP_CODE", "contains", searchTerm]
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

                $(document).on("dxclick", "#accountnaturegridBox .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#ACT_nature_hidden').val('');
                    $('#displayExprAccnature').val('');
                    empr_ChartOfAccount.hideshow(false);
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
            }
        });
    },
    InitChartTypeDDL: function (selectedValue) {
        
        $.ajax({
            url: '/ChartOfAccount/GetChartType',
            method: 'GET',
            success: function (data) {
                $('#CHART_TYPE').dxSelectBox({
                    dataSource: data,
                    displayExpr: 'value',
                    valueExpr: 'key',
                    value: selectedValue,
                    searchEnabled: true,
                    width: '100%',
                    placeholder: "Select Chart Type",
                    showClearButton: true,
                    dropDownOptions: {
                        height: 'auto',
                    },
                    pagingEnabled: true,
                    searchTimeout: 300,
                });
            },
            error: function (error) {
                console.error('Error fetching chart types:', error);
            }
        });
    },


    InitControlsGridBox: function (_selectedValue) {
        //console.log(_selectedValue)
        $.ajax({
            url: "ChartOfAccount/GetChartOfAccounts",
            type: "GET",
            success: function (response) {
                debugger;
                var Datasource = response.data;
                console.log("dataaaaa:", Datasource);

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.acT_CODE == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].acT_CODE;
                        $('#ACT_PARENT_CODE_hidden').val(selectedObject[0].acT_CODE);
                        $('#displayExprcontrol').val(selectedObject[0].acT_NAME);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#controlgridBox").dxDropDownBox({

                    value: selectedValue,
                    valueExpr: "acT_CODE",
                    //displayExpr: function (item) {
                    //    return item ? `${item.acT_NAME}` : "Select a value...";
                    //},
                    displayExpr: "acT_NAME",
                    placeholder: "Select a value...",
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: true,
                    onValueChanged: function (e) {
                        debugger;
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.acT_CODE === e.value);
                            if (selectedData) {
                                $('#ACT_PARENT_CODE_hidden').val(selectedData.acT_CODE);
                                $('#displayExprAccParent').val(selectedData.acT_NAME);
                                //let chartType = selectedData.charT_TYPE; edit error
                                let chartType = selectedData.charT_NAME;
                                let chartTypeInstance = $("#CHART_TYPE").dxSelectBox('instance');
                                let chartTypes = chartTypeInstance.option('dataSource');

                                if (Array.isArray(chartTypes)) {
                                    let matched = chartTypes.find(ct => ct.value == chartType);
                                    chartTypeInstance.option('value', matched ? matched.key : null);
                                    chartTypeInstance.option('disabled', true);

                                }
                            }
                        } else {
                            $('#ACT_PARENT_CODE_hidden').val('');
                            $('#displayExprAccParent').val('');
                            let chartTypeInstance = $("#CHART_TYPE").dxSelectBox('instance');
                            if (chartTypeInstance) {
                                chartTypeInstance.option('value', null);
                                chartTypeInstance.option('disabled', false);

                            }
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
                                key: "acT_CODE"
                            }),
                            columns: [
                                {
                                    dataField: "acT_CODE",
                                    caption: "Code",
                                    width: 80,
                                    alignment: "center",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "acT_NAME",
                                    caption: "Account Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "acT_SNAME",
                                    caption: "Account Control Name",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },

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
                                    e.component.option("value", selected.acT_CODE);
                                    e.component.close();

                                    $('#ACT_PARENT_CODE_hidden').val(selected.acT_CODE);
                                    $('#displayExprAccParent').val(selected.acT_NAME);

                                    let chartType = selected.charT_NAME;
                                    let chartTypeInstance = $('#CHART_TYPE').dxSelectBox('instance');
                                    let chartTypes = chartTypeInstance.option('dataSource');

                                    if (Array.isArray(chartTypes)) {
                                        let matched = chartTypes.find(ct => ct.value === chartType);
                                        chartTypeInstance.option('value', matched ? matched.key : null);
                                    }
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].acT_CODE], false);
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
                            ["acT_NAME", "contains", searchTerm],
                            "or",
                            ["acT_CODE", "contains", searchTerm],
                            "or",
                            ["acT_SNAME", "contains", searchTerm]
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

                $(document).on("dxclick", "#controlgridBox .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#ACT_PARENT_CODE_hidden').val('');
                    $('#displayExprAccParent').val('');
                    const chartTypeInstance = $('#CHART_TYPE').dxSelectBox('instance');
                    console.log("chartttttt:", chartTypeInstance);
                    if (chartTypeInstance) {
                        chartTypeInstance.option('value', null);
                        chartTypeInstance.option('disabled', false);

                    }
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });

            }
        });
    },
    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {

        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);

    },
    hideshow: function (show) {

        if (show) {

            $('#preifex').show();
            $('#accounttitle').show();
            $('#bankaccount').show();
            $('#accountswift').show();
            $('#Cheqformat').show();
            $('#accountcurrecny').show();
        } else {

            $('#preifex').hide();
            $('#accounttitle').hide();
            $('#bankaccount').hide();
            $('#accountswift').hide();
            $('#Cheqformat').hide();
            $('#accountcurrecny').hide();

            $("#PREFIX").val('');
            $("#ACCOUNT_NO").val('');
            $("#TITTLE").val('');
            $("#SWIFT").val('');
            $("#CHQ_ID").dxSelectBox('instance').option('value', null);
            $("#CURRENCY").dxSelectBox('instance').option('value', null);
            //$("#CHART_TYPE").dxSelectBox('instance').option('value', null);
        }
    },



}