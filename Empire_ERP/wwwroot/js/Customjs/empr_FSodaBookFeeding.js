var empr_FSodaBookFeeding = {
    totalCount: 0,
    rowsCount: 0,
    RT_TYPES: new DevExpress.data.ArrayStore({
        key: "key",
        data: [
            { key: 1, value: '1 KG' },
            { key: 40, value: '40 KG' },
            { key: 1000, value: '1000 KG' },
        ]
    }),
    Branch_RT_TYPE: '',
    InitEvents: function () {


        $(document).ready(function () {

            empr_FSodaBookFeeding.InitAccountDDL();

            $('.coBroker').hide();
            empr_FSodaBookFeeding.ResetForm();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_FSodaBookFeeding.GetSodaBookFeedingByCode(data.traN_ID);
                }
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_FSodaBookFeeding.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_FSodaBookFeeding.ResetForm();
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_FSodaBookFeeding.ValidateInfo()) {
                            empr_FSodaBookFeeding.SaveInfo();
                        }
                    }
                } else {
                    if (empr_FSodaBookFeeding.ValidateInfo()) {
                        empr_FSodaBookFeeding.SaveInfo();
                    }
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_FSodaBookFeeding.GetSodaBookFeedingByCode(id);
            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var date = $(this).attr("reportdate");
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
                    $('#updatedDate').val(date);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModal').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/FSodaBookFeeding/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_FSodaBookFeeding.GetSodaBookFeedingByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_FSodaBookFeeding.Delete();
            });

            $('body').on('change', '#SHIP_STATUS', function () {
                if (!$(this).is(":checked")) {
                    $("#SHIP_DATE").attr("type", "date");
                    let today = new Date().toISOString().split("T")[0];
                    $("#SHIP_DATE").val(today);
                } else {
                    $("#SHIP_DATE").attr("type", "month");
                    $("#SHIP_DATE").val(new Date().toISOString().slice(0, 7));
                }
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },



    InitAccountDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/FSodaBookFeeding/GetCOA", function (data) {
            if (data.msgType == 1) {
                $('#ACCOUNT_NAME').dxSelectBox({
                    dataSource: data.data,
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
                    searchTimeout: 500
                });
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    ResetForm: function () {
        empr_FSodaBookFeeding.CreateGrid([{ __KEY__: empr_FSodaBookFeeding.GenerateKey(36), chK1: true, chk: "1" }]);
        empr_FSodaBookFeeding.InitTypeDDL();
        //empr_FSodaBookFeeding.InitPurchaseAndSaleDDL();
        empr_FSodaBookFeeding.InitSodaTypeDDL();
        empr_FSodaBookFeeding.InitCurrencyDDL();
        empr_FSodaBookFeeding.InitUnitDDL();
        empr_FSodaBookFeeding.InitAccountDDL();

        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        $(".ConditionDiv").hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        //$('#ACCOUNT_NAME').dxSelectBox('instance').reset();
        $('.card-body').removeClass('customHighlightForModifiedCells');
        $('#V_DATE').val(todayDate);
        $("#P_S").prop("checked", true);
        $("#P_SHIP").prop('checked', false);
        $("#TRANS_PS").prop('checked', false);
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
        var dataSource = [
            {
                "Action": "",
                "dT_CODE": "abc123",
                "iteM_CODE": "xyz456",
                "qty": 42,
                "unit": "kg",
                "qtY2": 24,
                "baL_QTY": 168,
                "dT_DESC": "Lorem ipsum",
                "rate": 5.99,
                "amt": 251.58,
                "duE_DATE": "2024-04-15"
            },
            {
                "Action": "",
                "dT_CODE": "def789",
                "iteM_CODE": "uvw012",
                "qty": 73,
                "unit": "lbs",
                "qtY2": 12,
                "baL_QTY": 876,
                "dT_DESC": "Dolor sit amet",
                "rate": 8.49,
                "amt": 741.72,
                "duE_DATE": "2024-05-03"
            },
            {
                "Action": "",
                "dT_CODE": "ghi345",
                "iteM_CODE": "rst678",
                "qty": 91,
                "unit": "pcs",
                "qtY2": 36,
                "baL_QTY": 3276,
                "dT_DESC": "Consectetur adipiscing elit",
                "rate": 12.99,
                "amt": 4252.04,
                "duE_DATE": "2024-06-21"
            }
        ];
        empr_FSodaBookFeeding.InitDropdowns();

        $('#V_DATE').focus();
    },
    CreateGrid: function (dataSrc) {
        if (dataSrc.length > 0) {
            empr_FSodaBookFeeding.rowsCount = dataSrc.length - 1;
        }
        var col = [
            {
                dataField: "Action",
                width: 120,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    console.log('yahaaaaaaaa', options.data);
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_FSodaBookFeeding.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_FSodaBookFeeding.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_FSodaBookFeeding.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        let extraAction = '';
                        if ($('#Code').val() != '') {
                            extraAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 8px; color:#FFD700" onclick="empr_FSodaBookFeeding.ShowCostCenterModal(${options.data.traN_ID},${options.data.dT_CODE},'${options.data.dT_DESC}',${options.data.amt})"><i class="fa fa-coins"></i></a>`;
                        }
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_FSodaBookFeeding.CloneRow(`+ options.rowIndex +`)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_FSodaBookFeeding.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_FSodaBookFeeding.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                           ${extraAction}
                           </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'iteM_CODE',
                caption: 'Item',
                //width: 250,
                allowSorting: false,
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                //width: 100,
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 1;
                    var rate = parseFloat(currentRowData.rate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                }
            },
            {
                dataField: 'unit',
                caption: 'Unit',
                lookup: {
                    dataSource: Units,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value, currentRowData) {

                    if (value != '') {
                        newData.unit = value;
                        var selectedUnit = Units.filter(u => u.key == value);
                        if (selectedUnit.length > 0) {
                            newData.qtY2 = selectedUnit[0].qty;
                        }
                        //ajaxHelper.ajaxGetJson('/SodaBookFeeding/GetQuantityByUnit?id=' + value, function (data) {
                        //    if (data.msgType == 1) {
                        //        newData.qtY2 = data.data;
                        //    }
                        //    else {
                        //        empr_helper.notify(data.msg, data.msgType);
                        //    }
                        //}, false, true);
                    }
                    else {
                        newData.qtY2 = 0;
                    }

                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 1;
                    var rate = parseFloat(currentRowData.rate) || 0;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                newData.amt = newData.baL_QTY * rate;
                            }

                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                newData.amt = newData.baL_QTY * rate;
                            }

                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                }
            },
            {
                dataField: 'qtY2',
                caption: 'Qunatity 2',
                visible: false,
                setCellValue: function (newData, value, currentRowData) {
                    newData.qtY2 = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 1;
                    var rate = parseFloat(currentRowData.rate) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;

                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                }
            },
            {
                dataField: 'baL_QTY',
                caption: 'Balance Quantity',
                allowEditing: false,
                visible: false,
                cellTemplate: function (container, options) {
                    var $cell = $("<div>").addClass("custom-cell");
                    var isChecked = options.data.chK1;
                    var quantity = options.data.baL_QTY;
                    if (quantity == 0 || quantity == '' || quantity == null || quantity == undefined) {
                        quantity = '';
                    }
                    console.log("isChecked:", isChecked); 
                    var $checkbox = $("<input type='checkbox'>")
                        .addClass("chkCell")
                        .prop('checked', isChecked)
                        .on('change', function () {
                            var item = options.data;
                            var qty = parseFloat(item.qty) || 0;
                            var qtY2 = parseFloat(item.qtY2) || 1;
                            var rate = parseFloat(item.rate) || 0;
                            var rT_TYPE = parseFloat(item.rT_TYPE) || 0;

                            if (isNaN(qty)) {
                                empr_helper.notify("Please enter the correct quantity.", 2);
                            }
                            if (isNaN(qtY2)) {
                                empr_helper.notify("Please enter the correct quantity2.", 2);
                            }
                            if (!isNaN(qty) && !isNaN(qtY2)) {
                                if (this.checked) {
                                    item.baL_QTY = qty * qtY2;
                                    item.chk = "1";
                                    if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                                        if (perRate > -1 && perRate != 'Infinity') {
                                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                                        }
                                    }

                                    if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                        item.amt = qty * rate;
                                    }
                                }
                                else {
                                    item.baL_QTY = qty;
                                    item.chk = "0";
                                    if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                                        if (perRate > -1 && perRate != 'Infinity') {
                                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                                        }
                                    }

                                    if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                                        item.amt = qty * rate;
                                    }
                                }
                                item.chK1 = this.checked;
                                container.find('span').text(item.baL_QTY);
                            }


                            var gridInstance = $('#DetailContainer').dxDataGrid('instance');
                            var dataSource = gridInstance.option("dataSource");
                            dataSource[options.rowIndex] = item;
                            gridInstance.option("dataSource", dataSource);
                        });

                    var $span = $('<span>' + quantity + '</span>');
                    $cell.append($checkbox).append($span);
                    container.append($cell);
                }
            },
            {
                dataField: 'dT_DESC',
                caption: 'Description',
                visible: false,
            },

            {
                dataField: 'rate',
                caption: 'Rate',
                setCellValue: function (newData, value, currentRowData) {
                    newData.rate = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var rate = parseFloat(newData.rate) || 0;
                    var balancedQTY = parseFloat(currentRowData.baL_QTY) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;

                    if (isNaN(newData.rate)) {
                        empr_helper.notify("Please enter the correct rate.", 2);
                    }
                    if (isNaN(balancedQTY)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }

                    if (!isNaN(balancedQTY) && !isNaN(newData.rate)) {
                        if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                            var perRate = parseFloat(newData.rate / rT_TYPE) || 0;
                            if (perRate > -1 && perRate != 'Infinity') {
                                newData.amt = (balancedQTY * perRate).toFixed(2);
                            }
                        }

                        if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                            newData.amt = qty * rate;
                        }
                    }
                }
            },
            {
                dataField: 'rT_TYPE',
                caption: 'RT Type',
                lookup: {
                    dataSource: empr_FSodaBookFeeding.RT_TYPES,
                    //"acceptCustomValue": "true",
                    //"searchEnabled": "true",
                    //"onCustomItemCreating": function (args) {
                    //    //set the value on the row that is edited  
                    //    var newItem = new ProductContentTypeModel();
                    //    newItem.Code = data.text;
                    //    newItem.ID = this.id++;


                    //    this.productContentTypes.push(newItem)
                    //    data.customItem = newItem;
                    //    this.cdr.detectChanges();
                    //    return newItem;
                    //}, 
                    //"onValueChanged": function (args) {
                    //    //set the value on the row that is edited 
                    //}, 
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                editorOptions: {
                    acceptCustomValue: true,
                    searchEnabled: true,
                    onCustomItemCreating: function (args) {
                        if (args.text != undefined) {
                            var newItem = {};
                            newItem.key = parseInt(args.text);
                            if (isNaN(newItem.key)) {
                                empr_helper.notify("Please enter only number in RT Type.", 2);
                                return;
                            }
                            else {
                                if (args.text.toLowerCase().indexOf('kg') != -1) {
                                    newItem.value = args.text;
                                }
                                else {
                                    newItem.value = args.text + ' KG';
                                }
                                empr_FSodaBookFeeding.RT_TYPES.insert(newItem);
                                //args.CustomItem = newItem;
                                args.customItem = newItem;
                                setTimeout(function () {
                                    $('#DetailContainer').dxDataGrid('instance')
                                        .columnOption("rT_TYPE", "lookup", {
                                            dataSource: empr_FSodaBookFeeding.RT_TYPES,
                                            displayExpr: 'value',
                                            valueExpr: 'key'
                                        });
                                });
                            }
                            //return newItem;
                        }
                    }
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.rT_TYPE = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var balancedQTY = parseFloat(currentRowData.baL_QTY) || 0;
                    var rate = parseFloat(currentRowData.rate) || 0;

                    if (!isNaN(balancedQTY) && !isNaN(rate)) {
                        if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'Y') {
                            var perRate = parseFloat(rate / newData.rT_TYPE) || 0;
                            if (perRate > -1 && perRate != 'Infinity') {
                                newData.amt = (balancedQTY * perRate).toFixed(2);
                            }
                        }

                        if (empr_FSodaBookFeeding.Branch_RT_TYPE == 'N') {
                            newData.amt = qty * rate;
                        }
                    }
                }
            },
            {
                dataField: 'amt',
                caption: 'Amount',
                allowEditing: false,
            },
            {
                dataField: 'duE_DATE',
                caption: 'Due Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                visible: false,
                showInColumnChooser: false 
            },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "SodaBookFeeding", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        //setTimeout(function () {
        //    var nextElement = $('#DetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#DetailContainer').dxDataGrid('instance').focus(nextElement);
        //    //$('#V_DATE').focus();
        //}, 1500);
    },
    CloneRow: function (index) {
        debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_FSodaBookFeeding.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    //let clonedRowData = dataSource[index];
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    //gridInstance.addRow();
                    //$.each(clonedRowData, function (key, value) {
                    //    if (key == 'dT_CODE') {
                    //        gridInstance.cellValue(0, 'dT_CODE', '');
                    //    }
                    //    else {
                    //        gridInstance.cellValue(0, key, value);
                    //    }
                    //});
                    // After adding the row, insert it at the first position
                    //gridInstance.insertRow(clonedRowData, 0); // Assuming you want to insert at the first position
                    //dataSource.unshift(clonedRowData); // Add the cloned row data at the beginning of the dataSource array
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    //delete clonedRowData.dT_CODE;
                    clonedRowData.__KEY__ = empr_FSodaBookFeeding.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_FSodaBookFeeding.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                //let clonedRowData = dataSource[index];
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                //gridInstance.addRow();
                //$.each(clonedRowData, function (key, value) {
                //    if (key == 'dT_CODE') {
                //        gridInstance.cellValue(0, 'dT_CODE', '');
                //    }
                //    else {
                //        gridInstance.cellValue(0, key, value);
                //    }
                //});
                // After adding the row, insert it at the first position
                //gridInstance.insertRow(clonedRowData, 0); // Assuming you want to insert at the first position
                //dataSource.unshift(clonedRowData); // Add the cloned row data at the beginning of the dataSource array
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_FSodaBookFeeding.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
            }
        }
    },
    AddRow: function () {
        debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_FSodaBookFeeding.rowsCount += 1;
                //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_FSodaBookFeeding.GenerateKey(36), chK1: true, chk: "1" });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_FSodaBookFeeding.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_FSodaBookFeeding.rowsCount += 1;
            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_FSodaBookFeeding.GenerateKey(36), chK1: true, chk: "1" });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    DeleteRow: function (index, dtCode) {
        debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_FSodaBookFeeding.rowsCount -= 1;
                    gridInstance.saveEditData();
                }
                else {
                    var availableRows = dataSource.filter(x => x.dT_CODE > 0);
                    if (availableRows.length > 1) {
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
                        ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/FSodaBookFeeding/DeleteSodaBookFeedingDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_FSodaBookFeeding.rowsCount -= 1;
                                gridInstance.saveEditData();
                            }
                        }, false, true);
                    });
                    } else {
                        empr_helper.notify("You are not allowed to delete the last row.", 2);
                    }
                }
            }
            else {
                empr_helper.notify("You are not allowed to delete the last row.", 2);
            }
        }
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
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var SELLER_CODE = $('#SellerCode').dxSelectBox('option', 'value');
        //var BUYER_CODE = $('#BuyerCode').dxSelectBox('option', 'value');
        var BROKER_CODE = $('#BrokerCode').dxSelectBox('option', 'value');
        var COB_CODE = $('#coBrokerCode').dxSelectBox('option', 'value');
        var COND = $('#Condition').dxSelectBox('option', 'value');
        var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');        
        var CREDIT_DAYS = $('#CreditDays').val();
        var DUE_DATE = $('#DueDate').val();
        var SBF_TYPE = $('#SBF_TYPE').dxSelectBox('option', 'value');
        var CURR_CODE = $('#CURR_CODE').dxSelectBox('option', 'value');
        var UNIT = $('#UNIT').dxSelectBox('option', 'value');
        var CRATE = $('#CRATE').val();
        var SHIP_DATE = $('#SHIP_DATE').val();
        var SHIP_STATUS = $('#SHIP_STATUS').is(":checked") == true ? 1 : 0;
        //var P_S = $('#TYPE').dxSelectBox('option', 'value');
        var SODA_TYPE = $('#SODA_TYPE').dxSelectBox('option', 'value');  
        var P_SHIP = $("#P_SHIP").is(":checked") ? 1 : 0;
        var TRANS_PS = $("#TRANS_PS").is(":checked") ? 1 : 0;
        var COA = $('#ACCOUNT_NAME').dxSelectBox('option', 'value');


        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            SELLER_CODE: SELLER_CODE,
            //BUYER_CODE: BUYER_CODE,
            BROKER_CODE: BROKER_CODE,
            COB_CODE: COB_CODE,
            COND: COND,
            REF: REF,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS,
            CREDIT_DAYS: CREDIT_DAYS,
            DUE_DATE: DUE_DATE,
            SBF_TYPE: SBF_TYPE,
            CURR_CODE: CURR_CODE,
            UNIT: UNIT,
            CRATE: CRATE,
            SHIP_STATUS: SHIP_STATUS,
            SHIP_DATE: SHIP_DATE,
            //P_S: P_S,
            SODA_TYPE: SODA_TYPE,
            P_SHIP: P_SHIP,
            TRANS_PS: TRANS_PS,
            COA:COA

        }

        var detailRecords = [];
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
                //var response = empr_FSodaBookFeeding.GetGridData();
                //response.then((data) => {
                //    debugger;
                //    console.log(data);
                //    var modelRecord = {
                //        Master: masterRecord,
                //        Detail: data
                //    };
                //    return modelRecord;
                //});
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            //var modelRecord = {
            //    Master: masterRecord,
            //    Detail: detailRecords
            //};
            //return modelRecord;
        }

        if (empr_FSodaBookFeeding.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },
    GetGridData: async function () {
        return await $('#DetailContainer').dxDataGrid('instance').option("dataSource");
    },
    ValidateInfo: function () {

        var valid = true;
        var data = empr_FSodaBookFeeding.GetDataToSave();

        if (data.Master.SELLER_CODE == "" || data.Master.SELLER_CODE == null || data.Master.SELLER_CODE == undefined) {
            empr_helper.notify("Please select Party Name.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.COA == "" || data.Master.COA == null || data.Master.COA == undefined) {
            empr_helper.notify("Please select COA.", 2);
            valid = false;
            return valid;
        }
        
        //if (data.Master.BUYER_CODE == "" || data.Master.BUYER_CODE == null || data.Master.BUYER_CODE == undefined) {
        //    empr_helper.notify("Please select buyer.", 2);
        //    valid = false;
        //    return valid;
        //}

        //if (data.Master.P_S == "" || data.Master.P_S == null || data.Master.P_S == undefined) {
        //    empr_helper.notify("Please select type.", 2);
        //    valid = false;
        //    return valid;
        //}

        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add items.", 2);
            valid = false;
            return valid;
        }

        $.each(data.Detail, function (index, item) {
            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
                empr_helper.notify("Please select item at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty ItemCode.");
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
            }

            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
            }

            if (item.qtY2 != "" && item.qtY2 != null && item.qtY2 != undefined && item.qtY2 < 0) {
                empr_helper.notify("Please enter correct item quantity2 at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity2.");
            }

            if (item.rate != "" && item.rate != null && item.rate != undefined && item.rate <= 0) {
                empr_helper.notify("Please enter correct rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty rate.");
            }

            //if (item.duE_DATE == "" || item.duE_DATE == null || item.duE_DATE == undefined) {
            //    empr_helper.notify("Please select DueDate at index " + index, 2);
            //    valid = false;
            //    return valid;
            //    console.log("Item at index " + index + " has empty DueDate.");
            //}
        });

        return valid;
    },
    SaveInfo: function () {
        debugger;
        var dataModel = empr_FSodaBookFeeding.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/FSodaBookFeeding/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }

                if (dataClear == 1) {
                    empr_FSodaBookFeeding.GetSodaBookFeedingDetailsByCode($('#Code').val());
                    $('#BtnDelete').show();
                }
                else {
                    empr_FSodaBookFeeding.ResetForm();
                } 
            }
        }, false, true);
    },
    InitQuickSearchGrid: function () {
        empr_FSodaBookFeeding.GetSodaBookFeedings();
    },
    GetSodaBookFeedings: function () {
        ajaxHelper.ajaxGetJson('/FSodaBookFeeding/GetSodaBookFeedings', function (data) {
            if (data.msgType == 1) {
                empr_FSodaBookFeeding.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        var columns = [{
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
                               <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>
                               </div>`).appendTo(container);
                }
            },
            { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'astatus', caption: 'Status', },
            { dataField: 'iteM_CODE', caption: 'Item', },
            { dataField: 'qty', caption: 'Quantity',  },
            { dataField: 'qtY2', caption: 'Quantity 2', },
            { dataField: 'baL_QTY', caption: 'Bal.Qty', },
            { dataField: 'unit', caption: 'Unit', },
            { dataField: 'masterunit', caption: 'M.Unit', },
            { dataField: 'rate', caption: 'Rate', },
            { dataField: 'amt', caption: 'Amount', },
            { dataField: 'cond', caption: 'Condition', },
            { dataField: 'brokeR_CODE', caption: 'Broker', },
            { dataField: 'selleR_CODE', caption: 'Seller', },
            { dataField: 'coa', caption: 'COA', },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'crediT_DAYS', caption: 'Credit Days', },
            { dataField: 'duE_DATE', caption: 'Due Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'rT_TYPE', caption: 'RT Type', },
            { dataField: 'remarks', caption: 'Remarks', },
            { dataField: 'traN_ID', caption: 'Code', visible: false },
            { dataField: 'sacT_CODE', caption: 'Seller Code', visible: false },
            { dataField: 'bD_ACT_CODE', caption: 'Broker Code', visible: false },


            //{ dataField: 'detail', calculateFilterExpression: function (value) {
            //        return [function (data) {
            //            var details = data.detail,
            //                detail;

            //            for (var i = 0; i < details.length; i++) {
            //                detail = details[i];
            //                for (var fieldName in detail) {
            //                    if (detail[fieldName] && detail[fieldName].toString().toLowerCase().indexOf(value.toLowerCase()) >= 0) {
            //                        return true;
            //                    }
            //                }
            //            }
            //            return false;
            //        }, "=", true]
            //    },
            //    visible: false,
            //    showInColumnChooser: false 
            //}
        ];
        //var detailColumns = [
        //    { dataField: 'iteM_CODE', caption: 'Item Name' },
        //    { dataField: 'qty', caption: 'Qty', },
        //    { dataField: 'unit', caption: 'Unit', },
        //    { dataField: 'qtY2', caption: 'Qty2', },
        //    { dataField: 'baL_QTY', caption: 'Balance Quantity', },
        //    { dataField: 'rate', caption: 'Rate', },
        //    { dataField: 'rT_TYPE', caption: 'RT Type', },
        //    { dataField: 'amt', caption: 'Amount', },
        //    { dataField: 'dT_DESC', caption: 'Description', visible: false },
        //];
        //empr_helper.MasterDetailDxGridBinding('#gridContainer', columns, detailColumns, dataSrc, "SodaBookFeeding");
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/SodaBookFeeding/GetSodaBookFeedings", "traN_ID", "SodaBookFeeding");
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "SodaBookFeedingQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    GetSodaBookFeedingByCode: function (code) {
        ajaxHelper.ajaxGetJson('/FSodaBookFeeding/GetSodaBookFeedingByCode?code=' + code, function (data) {
            console.log('data ...............',data);
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#REF').val(response.ref);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#CreditDays').val(response.crediT_DAYS);
                    $('#DueDate').val(response.duE_DATE);
                    $('#ACCOUNT_NAME').dxSelectBox('instance').option('value', response.coa);

                    //$('#TYPE').dxSelectBox('instance').option('value', response.p_S);
                    /*$("#P_S").prop("checked", response.p_S == "P" ? true : false);*/
                    const invalidDate = "1900-01-01";
                    if (response.shiP_DATE !== invalidDate) {
                        if (response.shiP_STATUS == 0) {
                            $("#SHIP_DATE").attr("type", "date");
                        } else {
                            $("#SHIP_DATE").attr("type", "month");
                        }
                        $('#SHIP_DATE').val(response.shiP_DATE);
                        console.log("Shipment date is valid.");
                    } else {
                        $('#SHIP_DATE').val("");
                        console.log("Shipment date is invalid.");
                    }
                    $('#CRATE').val(response.crate);
                    if (response.p_SHIP == '1') {
                        $("#P_SHIP").prop("checked", true);
                    } else {
                        $("#P_SHIP").prop("checked", false);
                    }
                    if (response.tranS_PS == '1') {
                        $("#TRANS_PS").prop("checked", true);
                    } else {
                        $("#TRANS_PS").prop("checked", false);
                    }
                    empr_FSodaBookFeeding.InitDropdownsWithValue(response.selleR_CODE, response.buyeR_CODE, response.brokeR_CODE, response.cond, response.sbF_TYPE, response.curR_CODE, response.unit, response.sodA_TYPE);
                    if (response.sbF_TYPE == "LOC") {
                        $('.ImpExp').hide();
                        $('#CRATE').val('');
                        $('#SHIP_DATE').val('');
                        $('#CURR_CODE').dxSelectBox('instance').option('value', '');
                        $('#UNIT').dxSelectBox('instance').option('value', '');
                    } else {
                        $('.ImpExp').show();
                    }
                    //$('#BtnDelete').show();
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
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
                    }
                }

                if (data.detail.msgType == 1) {
                    empr_FSodaBookFeeding.CreateGrid(data.detail.data);
                    $('.card-body').addClass('customHighlightForModifiedCells');
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetSodaBookFeedingDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/FSodaBookFeeding/GetSodaBookFeedingDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_FSodaBookFeeding.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    Delete: function () {

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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/FSodaBookFeeding/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_FSodaBookFeeding.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    InitDropdowns: function () {
        debugger;
        empr_FSodaBookFeeding.InitConditionDDL();
        ajaxHelper.ajaxGetJson("/FSodaBookFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_FSodaBookFeeding.InitSellerCodeDDL(data.data);
                //empr_FSodaBookFeeding.InitBuyerCodeDDL(data.data);
                empr_FSodaBookFeeding.InitBrokerCodeDDL(data.data);
                empr_FSodaBookFeeding.InitCoBrokerCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    InitDropdownsWithValue: function (sCode, bCode, brCode, condition, type, currCode, unit, sodaType) {
        debugger;
        empr_FSodaBookFeeding.InitConditionDDL(condition);
        empr_FSodaBookFeeding.InitCurrencyDDL(currCode);
        empr_FSodaBookFeeding.InitUnitDDL(unit);
        empr_FSodaBookFeeding.InitTypeDDL(type);
        empr_FSodaBookFeeding.InitSodaTypeDDL(sodaType);
        ajaxHelper.ajaxGetJson("/FSodaBookFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_FSodaBookFeeding.InitSellerCodeDDL(data.data, sCode);
                //empr_FSodaBookFeeding.InitBuyerCodeDDL(data.data, bCode);
                empr_FSodaBookFeeding.InitBrokerCodeDDL(data.data, brCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    InitSellerCodeDDL: function (dataSource, selectedValue) {
        $('#SellerCode').dxSelectBox({
            dataSource: {
                store: dataSource,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
            displayExpr: 'value',
            valueExpr: 'customizedKey',
            value: selectedValue,
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
    },
    //InitBuyerCodeDDL: function (dataSource, selectedValue) {
    //    $('#BuyerCode').dxSelectBox({
    //        dataSource: {
    //            store: dataSource,
    //            paginate: true,
    //            pageSize: 50
    //        },
    //        paging: {
    //            enabled: true,
    //            pageSize: 50,
    //        },
    //        displayExpr: 'value',
    //        valueExpr: 'customizedKey',
    //        value: selectedValue,
    //        searchEnabled: true,
    //        width: '100%',
    //        placeholder: 'Search',
    //        showClearButton: true,
    //        dropDownOptions: {
    //            height: 'auto',
    //        },
    //        pagingEnabled: true,
    //        searchTimeout: 500
    //    });
    //},
    InitBrokerCodeDDL: function (dataSource, selectedValue) {
        $('#BrokerCode').dxSelectBox({
            dataSource: {
                store: dataSource,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
            displayExpr: 'value',
            valueExpr: 'customizedKey',
            value: selectedValue,
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
    },
    InitCoBrokerCodeDDL: function (dataSource, selectedValue) {
        $('#coBrokerCode').dxSelectBox({
            dataSource: {
                store: dataSource,
                paginate: true,
                pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
            displayExpr: 'value',
            valueExpr: 'customizedKey',
            value: selectedValue,
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
    },
    InitConditionDDL: function (selectedValue) {

        //var dataSource = [
        //    { key: 'Adv', value: 'Advance' },
        //    { key: 'Cash', value: 'Cash' },
        //    { key: 'Cr', value: 'Credit Days' },
        //    { key: 'CrD', value: 'Credit Date' },
        //    { key: 'Lc', value: 'LC' },
        //    { key: 'Cad', value: 'CAD' },
        //    { key: 'Da', value: 'DA' },
        //    { key: 'Con', value: 'Consignment' },
        //];

        $('#Condition').dxSelectBox({
            dataSource: empr_helper.conditions,
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
                if (e.value == 'Cr' || e.value == 'CrD' || e.value == 'Ad') {
                    $(".ConditionDiv").show();
                }
                else {
                    $(".ConditionDiv").hide();
                    $("#CreditDays").val('');
                    $('#DueDate').val('');
                }
            },
        });
    },
    AutoCalculateDays: function (element) {
        var transactionDate = new Date($("#V_DATE").val());
        var dueDate = new Date($(element).val());
        var daysDifference = Math.ceil((dueDate - transactionDate) / (1000 * 60 * 60 * 24));
        console.log('transactionDate: ' + transactionDate);
        console.log('dueDate: ' + dueDate);
        console.log('daysDifference: ' + daysDifference);
        $("#CreditDays").val(daysDifference);
    },
    AutoCalculateDueDate: function (element) {
        var transactionDate = new Date($("#V_DATE").val());
        var days = parseInt($(element).val());
        if (isNaN(days)) {
            $("#DueDate").val('');
            return;
        }
        var dueDate = new Date(transactionDate.getTime() + days * 24 * 60 * 60 * 1000);
        var dueDateString = dueDate.toISOString().split('T')[0];
        console.log('transactionDate: ' + transactionDate);
        console.log('days: ' + days);
        console.log('dueDate: ' + dueDate);
        console.log('dueDateString: ' + dueDateString);
        $("#DueDate").val(dueDateString);
    },
    InitUnitDDL: function (_selectedValue) {
        $('#UNIT').dxSelectBox({
            dataSource: Units,
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
            searchTimeout: 500,
            onValueChanged: function (e) {
            },
        });
    },
    InitCurrencyDDL: function (_selectedValue) {
        $.ajax({
            url: 'SodaBookFeeding/GetCurrencies',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#CURR_CODE').dxSelectBox({
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
                        searchTimeout: 500,
                        onValueChanged: function (e) {
                            if (e.value != '' && e.value != null) {
                                var items = e.component._dataSource._items;
                                var item = items.filter(i => i.key == e.value);
                                if (item && item.length > 0) {
                                    $('#CRATE').val(item[0].rate);
                                }
                            }
                            else {
                                $('#CRATE').val('');
                            }
                        },
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
    InitTypeDDL: function (_selectedValue) {
        $('#SBF_TYPE').dxSelectBox({
            dataSource: empr_helper.SBF_TYPE,
            valueExpr: 'value',
            displayExpr: 'text',
            searchEnabled: true,
            value: _selectedValue,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onInitialized: function (e) {
                e.component.option('value', 'LOC');
                $('.ImpExp').hide();
            },
            onValueChanged: function (e) {
                if (e.value != '' && e.value != null) {
                    var items = e.component._dataSource._items;
                    var item = items.filter(i => i.value == e.value);
                    if (item.length > 0 && item[0].value === "LOC") {
                        $('.ImpExp').hide();
                        $('.coBroker').hide();
                        $('#CRATE').val('');
                        $('#SHIP_DATE').val('');
                        $('#CURR_CODE').dxSelectBox('instance').option('value', '');
                        $('#UNIT').dxSelectBox('instance').option('value', '');
                        $('#SODA_TYPE').dxSelectBox('instance').option('value', '');
                    }
                    else if (item.length > 0 && item[0].value === "IND")
                    {
                        $('.ImpExp').show();
                        $('.coBroker').show();
                        $('.IndHide').hide();
                    }
                    else if (item.length > 0 && item[0].value === "Exp") {
                        $('.coBroker').hide();
                    }
                    else
                    {
                        $('.ImpExp').show();
                        $('.coBroker').hide();
                    }
                }
                else {
                    $('.ImpExp').hide();
                    $('.coBroker').hide();
                    $('#CRATE').val('');
                    $('#SHIP_DATE').val('');
                    $('#CURR_CODE').dxSelectBox('instance').option('value', '');
                    $('#UNIT').dxSelectBox('instance').option('value', '');
                    $('#SODA_TYPE').dxSelectBox('instance').option('value', '');
                }
            },
        });
    },
    InitSodaTypeDDL: function (_selectedValue) {
        $('#SODA_TYPE').dxSelectBox({
            dataSource: [
                { value: 'CNF', text: 'CNF' },
                { value: 'FOB', text: 'FOB' },
                { value: 'CON', text: 'Consignment' }
            ],
            valueExpr: 'value',
            displayExpr: 'text',
            searchEnabled: true,
            value: _selectedValue,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500
        });
    },
    //InitPurchaseAndSaleDDL: function (_selectedValue) {
    //    $('#TYPE').dxSelectBox({
    //        dataSource: [
    //            { value: 'P', text: 'Purchase' },
    //            { value: 'S', text: 'Sale' }
    //        ],
    //        valueExpr: 'value',
    //        displayExpr: 'text',
    //        searchEnabled: true,
    //        value: _selectedValue,
    //        width: '100%',
    //        placeholder: 'Search',
    //        showClearButton: true,
    //        dropDownOptions: {
    //            height: 'auto',
    //        },
    //        pagingEnabled: true,
    //        searchTimeout: 500
    //    });
    //},
    MoveFocusToGridShiftTab: function (event, gridElement, rowIndex, dataField) {
        if (event.key === 'Tab') {
            var SBF_TYPE = $('#SBF_TYPE').dxSelectBox('option', 'value');
            event.preventDefault();
            if (event.shiftKey) {
                $('#REMARKS').focus();
            } else {
                debugger
                if (SBF_TYPE == "LOC") {
                    var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
                    $(nextElement).click();
                    $(gridElement).dxDataGrid('instance').focus(nextElement);
                } else {
                    $('#CURR_CODE').data('dxSelectBox').focus();
                }
            }
        }
    },

    ShowCostCenterModal: function (tranId, dtCode, desc, amt) {
        if (amt == null || amt === undefined || amt <= 0) {
            empr_helper.notify('Amount should be greater than zero.', 2);
            return;
        }

        var obj = {
            TranId: tranId,
            DtCode: dtCode,
            Desc: desc,
            Amt: amt
        };

        $.ajax({
            url: 'CostCenter/Index',
            method: 'GET',
            data: obj,
            success: function (result) {
                $('#costCenterModalBody').html(result);
                $('#costCenterModal').modal('show');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    }
}