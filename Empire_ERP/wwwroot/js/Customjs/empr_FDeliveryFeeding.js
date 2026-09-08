var empr_FDeliveryFeeding = {
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
            empr_FDeliveryFeeding.InitAccountDDL();

            console.log('PICK_DATA', pickData);
            $('.coBroker').hide();
            empr_FDeliveryFeeding.ResetForm();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_FDeliveryFeeding.GetDeliveryFeedingByCode(data.traN_ID);
                }
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_FDeliveryFeeding.InitQuickSearchGrid();
            });

            $('body').on('change', '#Sel_AMT', function () {
                empr_FDeliveryFeeding.calculateBuyerNetAmount();
                empr_FDeliveryFeeding.calculateSellerNetAmount();
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_FDeliveryFeeding.GeneratePrintReport();
            });

            $('body').on('click', '#BtnSave', function () {
                debugger;
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_FDeliveryFeeding.ValidateInfo()) {
                            empr_FDeliveryFeeding.SaveInfo();
                        }
                    }
                } else {
                    if (empr_FDeliveryFeeding.ValidateInfo()) {
                        empr_FDeliveryFeeding.SaveInfo();
                    }
                }
            });

            $('#BrSeller, #WtSeller, #SBardana, #SGOD_CHARGES, #SLABOUR, #SFRIEGHT, #SFUMIGATION').on('input', function () {
                empr_FDeliveryFeeding.calculateSellerNetAmount();
            });

            $('#BrBuyer, #WtBuyer, #Bardana, #BGOD_CHARGES, #BLABOUR, #BFRIEGHT, #BFUMIGATION').on('input', function () {
                empr_FDeliveryFeeding.calculateBuyerNetAmount();
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_FDeliveryFeeding.GetDeliveryFeedingByCode(id);
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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/FDeliveryFeeding/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_FDeliveryFeeding.GetDeliveryFeedingByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_FDeliveryFeeding.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_FDeliveryFeeding.ResetForm();
            });

            $('body').on('click', '#BtnSodaPick', function () {
                empr_FDeliveryFeeding.InitSodaPickGrid();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                debugger;
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_FDeliveryFeeding.AddSodaToDelivery();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_FDeliveryFeeding.GeneratePrintReport();
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
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                !Permissions.r_ADD && $('#BtnSodaPick').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    toggleSignSeller: function(sign, id) {
        document.getElementById(id + 'Sign').value = sign;
        let parent = document.getElementById(id).parentElement;
        let minusBtn = parent.querySelector(".minus");
        let plusBtn = parent.querySelector(".plus");

        if (sign === "+") {
            plusBtn.classList.add("active");
            minusBtn.classList.remove("active");
        } else {
            minusBtn.classList.add("active");
            plusBtn.classList.remove("active");
        }

        empr_FDeliveryFeeding.calculateSellerNetAmount();
    },
    toggleSignBuyer: function(sign, id) {
        document.getElementById(id + 'Sign').value = sign;
        let parent = document.getElementById(id).parentElement;
        let minusBtn = parent.querySelector(".minus");
        let plusBtn = parent.querySelector(".plus");

        if (sign === "+") {
            plusBtn.classList.add("active");
            minusBtn.classList.remove("active");
        } else {
            minusBtn.classList.add("active");
            plusBtn.classList.remove("active");
        }

        empr_FDeliveryFeeding.calculateBuyerNetAmount();
    },
    toggleOperatorsOnEdit: function () {
        $(".number-input").each(function () {
            let hiddenSignField = $(this).find("input[type='hidden']");
            let minusBtn = this.querySelector(".minus");
            let plusBtn = this.querySelector(".plus");

            if (hiddenSignField.val() === "+") {
                plusBtn.classList.add("active");
                minusBtn.classList.remove("active");
            } else {
                minusBtn.classList.add("active");
                plusBtn.classList.remove("active");
            }
        });
    },

    InitAccountDDL: function (selectedValue) {
        debugger;
        ajaxHelper.ajaxGetJson("/FDeliveryFeeding/GetCOA", function (data) {
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
        empr_FDeliveryFeeding.CreateGrid([{ __KEY__: empr_FDeliveryFeeding.GenerateKey(36), chK1: false, chk: "0" }]);
        empr_FDeliveryFeeding.InitTypeDDL();
        empr_FDeliveryFeeding.InitAccountDDL();

        empr_FDeliveryFeeding.InitSodaTypeDDL();
        empr_FDeliveryFeeding.InitUnitDDL();
        empr_FDeliveryFeeding.InitCurrencyDDL();
        empr_FDeliveryFeeding.InitCoBrokerCodeDDL();
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #SDATE').val('');
        $('#REMARKS').val('');
        $(".ConditionDiv").hide();
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        $('#V_DATE').val(todayDate);
        $('#SDATE').val(todayDate);
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
        $(".number-input").each(function () {
            let hiddenSignField = $(this).find("input[type='hidden']");
            hiddenSignField.val("+");
            let plusBtn = this.querySelector(".plus");
            let minusBtn = this.querySelector(".minus");
            plusBtn.classList.add("active");
            minusBtn.classList.remove("active");
        });
        empr_FDeliveryFeeding.GetDeliveryFeedingDefaultOperators();
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
        empr_FDeliveryFeeding.InitDropdowns();
        empr_FDeliveryFeeding.InitReportTypeDDL();
        $('#SDATE').focus();
    },
    CreateGrid: function (dataSrc) {
        console.log('CreateGrid Old Link', dataSrc);
        if (dataSrc.length > 0) {
            empr_FDeliveryFeeding.rowsCount = dataSrc.length - 1;
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
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_FDeliveryFeeding.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_FDeliveryFeeding.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_FDeliveryFeeding.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_FDeliveryFeeding.CloneRow(`+ options.rowIndex +`)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_FDeliveryFeeding.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_FDeliveryFeeding.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                width: 150,
                allowSorting: false,
                fixed: true,
                fixedPosition: "left",
                lookup: {
                    dataSource: Items,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qty = value;
                    var qty = parseFloat(newData.qty) || 0;
                    var qtY2 = parseFloat(currentRowData.qtY2) || 1;
                    var orate = parseFloat(currentRowData.rate) || 0;
                    var crate = parseFloat(currentRowData.crate) || 0;
                    var rate = orate - crate;
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
                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }

                    var brAmountBuyer = parseFloat(currentRowData.bR_AMOUNT_BUYER) || 0;
                    var brAmountSeller = parseFloat(currentRowData.bR_AMOUNT_SELLER) || 0;
                    var wtAmountBuyer = parseFloat(currentRowData.wT_AMOUNT_BUYER) || 0;
                    var wtAmountSeller = parseFloat(currentRowData.wT_AMOUNT_SELLER) || 0;
                    newData.neT_AMT = (parseFloat(newData.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
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
                    }
                    else {
                        newData.qtY2 = 0;
                    }

                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 1;
                    var orate = parseFloat(currentRowData.rate) || 0;
                    var crate = parseFloat(currentRowData.crate) || 0;
                    var rate = orate - crate;
                    if (isNaN(qty)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }
                    if (isNaN(qtY2)) {
                        empr_helper.notify("Please enter the correct quantity2.", 2);
                    }
                    if (!isNaN(qty) && !isNaN(qtY2)) {
                        if (currentRowData.chK1) {
                            newData.baL_QTY = qty * qtY2;
                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                newData.amt = newData.baL_QTY * rate;
                            }

                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                newData.amt = newData.baL_QTY * rate;
                            }

                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                }
            },
            {
                dataField: 'qtY2',
                caption: 'Bags',
                setCellValue: function (newData, value, currentRowData) {
                    newData.qtY2 = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var qtY2 = parseFloat(newData.qtY2) || 1;
                    var orate = parseFloat(currentRowData.rate) || 0;
                    var crate = parseFloat(currentRowData.crate) || 0;
                    var rate = orate - crate;
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
                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                        else {
                            newData.baL_QTY = qty;
                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                var perRate = parseFloat(rate / rT_TYPE) || 0;
                                if (perRate > -1 && perRate != 'Infinity') {
                                    newData.amt = (newData.baL_QTY * perRate).toFixed(2);
                                }
                            }

                            if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                newData.amt = qty * rate;
                            }
                        }
                    }
                    var brAmountBuyer = parseFloat(currentRowData.bR_AMOUNT_BUYER) || 0;
                    var brAmountSeller = parseFloat(currentRowData.bR_AMOUNT_SELLER) || 0;
                    var wtAmountBuyer = parseFloat(currentRowData.wT_AMOUNT_BUYER) || 0;
                    var wtAmountSeller = parseFloat(currentRowData.wT_AMOUNT_SELLER) || 0;
                    newData.neT_AMT = (parseFloat(newData.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
                }
            },
            {
                dataField: 'baL_QTY',
                caption: 'Balance Quantity',
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var $cell = $("<div>").addClass("custom-cell");
                    var isChecked = options.data.chK1;
                    var quantity = options.data.baL_QTY;
                    if (quantity == 0 || quantity == '' || quantity == null || quantity == undefined) {
                        quantity = '';
                    }
                    var $checkbox = $("<input type='checkbox'>")
                        .addClass("chkCell")
                        .prop('checked', isChecked)
                        .on('change', function () {
                            var item = options.data;
                            var qty = parseFloat(item.qty) || 0;
                            var qtY2 = parseFloat(item.qtY2) || 1;
                            var orate = parseFloat(item.rate) || 0;
                            var crate = parseFloat(item.crate) || 0;
                            var rate = orate - crate;
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
                                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                                        if (perRate > -1 && perRate != 'Infinity') {
                                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                                        }
                                    }

                                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                        item.amt = qty * rate;
                                    }
                                }
                                else {
                                    item.baL_QTY = qty;
                                    item.chk = "0";
                                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                                        if (perRate > -1 && perRate != 'Infinity') {
                                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                                        }
                                    }

                                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                                        item.amt = qty * rate;
                                    }
                                }

                                var brAmountBuyer = parseFloat(item.bR_AMOUNT_BUYER) || 0;
                                var brAmountSeller = parseFloat(item.bR_AMOUNT_SELLER) || 0;
                                var wtAmountBuyer = parseFloat(item.wT_AMOUNT_BUYER) || 0;
                                var wtAmountSeller = parseFloat(item.wT_AMOUNT_SELLER) || 0;
                                item.neT_AMT = (parseFloat(item.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
                                item.chK1 = this.checked;
                                container.find('span').text(item.baL_QTY);
                            }


                            var gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
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
                    var orate = parseFloat(newData.rate) || 0;
                    var crate = parseFloat(currentRowData.crate) || 0;
                    var rate = orate - crate;
                    var balancedQTY = parseFloat(currentRowData.baL_QTY) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    if (isNaN(newData.rate)) {
                        empr_helper.notify("Please enter the correct rate.", 2);
                    }
                    if (isNaN(balancedQTY)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }

                    if (!isNaN(balancedQTY) && !isNaN(newData.rate)) {
                        if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                            var perRate = parseFloat(rate / rT_TYPE) || 0;
                            if (perRate > -1 && perRate != 'Infinity') {
                                newData.amt = (balancedQTY * perRate).toFixed(2);
                            }
                        }

                        if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                            newData.amt = qty * rate;
                        }
                    }

                    var brAmountBuyer = parseFloat(currentRowData.bR_AMOUNT_BUYER) || 0;
                    var brAmountSeller = parseFloat(currentRowData.bR_AMOUNT_SELLER) || 0;
                    var wtAmountBuyer = parseFloat(currentRowData.wT_AMOUNT_BUYER) || 0;
                    var wtAmountSeller = parseFloat(currentRowData.wT_AMOUNT_SELLER) || 0;
                    newData.neT_AMT = (parseFloat(newData.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
                }
            },
            {
                dataField: 'crate',
                caption: 'C.Rate',
                setCellValue: function (newData, value, currentRowData) {
                    newData.crate = value;
                    var qty = parseFloat(currentRowData.qty) || 0;
                    var orate = parseFloat(currentRowData.rate) || 0;
                    var crate = parseFloat(newData.crate) || 0;
                    var rate = orate - crate;
                    var balancedQTY = parseFloat(currentRowData.baL_QTY) || 0;
                    var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
                    if (isNaN(currentRowData.rate)) {
                        empr_helper.notify("Please enter the correct rate.", 2);
                    }
                    if (isNaN(balancedQTY)) {
                        empr_helper.notify("Please enter the correct quantity.", 2);
                    }

                    if (!isNaN(balancedQTY) && !isNaN(currentRowData.rate)) {
                        if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                            var perRate = parseFloat(rate / rT_TYPE) || 0;
                            if (perRate > -1 && perRate != 'Infinity') {
                                newData.amt = (balancedQTY * perRate).toFixed(2);
                            }
                        }

                        if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                            newData.amt = qty * rate;
                        }
                    }

                    var brAmountBuyer = parseFloat(currentRowData.bR_AMOUNT_BUYER) || 0;
                    var brAmountSeller = parseFloat(currentRowData.bR_AMOUNT_SELLER) || 0;
                    var wtAmountBuyer = parseFloat(currentRowData.wT_AMOUNT_BUYER) || 0;
                    var wtAmountSeller = parseFloat(currentRowData.wT_AMOUNT_SELLER) || 0;
                    newData.neT_AMT = (parseFloat(newData.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
                }
            },
            {
                dataField: 'rT_TYPE',
                caption: 'RT Type',
                lookup: {
                    dataSource: empr_FDeliveryFeeding.RT_TYPES,
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
                                empr_FDeliveryFeeding.RT_TYPES.insert(newItem);
                                //args.CustomItem = newItem;
                                args.customItem = newItem;
                                setTimeout(function () {
                                    $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance')
                                        .columnOption("rT_TYPE", "lookup", {
                                            dataSource: empr_FDeliveryFeeding.RT_TYPES,
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
                        if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                            var perRate = parseFloat(rate / newData.rT_TYPE) || 0;
                            if (perRate > -1 && perRate != 'Infinity') {
                                newData.amt = (balancedQTY * perRate).toFixed(2);
                            }
                        }

                        if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                            newData.amt = qty * rate;
                        }
                    }

                    var brAmountBuyer = parseFloat(currentRowData.bR_AMOUNT_BUYER) || 0;
                    var brAmountSeller = parseFloat(currentRowData.bR_AMOUNT_SELLER) || 0;
                    var wtAmountBuyer = parseFloat(currentRowData.wT_AMOUNT_BUYER) || 0;
                    var wtAmountSeller = parseFloat(currentRowData.wT_AMOUNT_SELLER) || 0;
                    newData.neT_AMT = (parseFloat(newData.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
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
            {
                dataField: 'deL_DATE',
                caption: 'Del Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                visible: false,
                showInColumnChooser: false 
            },
            {
                dataField: 'trucK_NO',
                caption: 'Truck No',
            },
            {
                dataField: 'conT_NO',
                caption: 'Cont No',
            },
            {
                dataField: 'scomp',
                caption: 'SComp',
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var $cell = $("<div>").addClass("custom-cell");
                    var isChecked = options.data.scomP1;
                    var $checkbox = $("<input type='checkbox'>")
                        .prop('checked', isChecked)
                        .on('change', function () {
                            var item = options.data;
                            if (this.checked) {
                                item.scomp = "1";
                            }
                            else {
                                item.scomp = "0";
                            }
                            item.scomP1 = this.checked;

                            var gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
                            var dataSource = gridInstance.option("dataSource");
                            dataSource[options.rowIndex] = item;
                            gridInstance.option("dataSource", dataSource);
                        });

                    $cell.append($checkbox);
                    container.append($cell);
                }
            }, 
            //{
            //    dataField: 'voucheR_NO',
            //    caption: 'Voucher No',
            //},
            {
                dataField: 'voucheR_NO', caption: 'Voucher No',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_FDeliveryFeeding.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.traN_ID) + ')')
                        .appendTo(container);
                }
            },
            {
                dataField: 'ins',
                caption: 'Insurance',
            },
            {
                dataField: 'loT_NO',
                caption: 'Lot No',
            },
            //{
            //    dataField: 'warehouse',
            //    caption: 'Warehouse',
            //    width: 150,
            //    allowSorting: false,
            //    //lookup: {
            //    //    dataSource: Warehouse,
            //    //    displayExpr: 'value',
            //    //    valueExpr: 'key'
            //    //},
            //    lookup: {
            //        dataSource: Warehouse,
            //        valueExpr: "key",
            //        displayExpr: function (item) {
            //            if (!item) return "";
            //            return item.value + " - " + item.name;
            //        }
            //    },
            //},
            {
                dataField: "warehouse",
                caption: "Warehouse",
                calculateCellValue: function (rowData) {
                    let item = Warehouse.find(x => x.key === rowData.warehouse);
                    return item ? item.value : "";
                },
                editorType: "dxDropDownBox",
                editorOptions: {
                    dataSource: Warehouse,
                    valueExpr: "key",
                    displayExpr: "value",
                    contentTemplate: function (e, cellInfo) {
                        let $grid = $("<div>").dxDataGrid({
                            dataSource: Warehouse,
                            keyExpr: "key",
                            columns: [
                                { dataField: "value", caption: "As Name", width: 250, },
                                { dataField: "name", caption: "Control Name", width: 150, }
                            ],
                            selection: { mode: "single" },
                            hoverStateEnabled: true,
                            height: 200,
                            searchPanel: {
                                visible: true,
                                width: 380,
                                placeholder: "Search..."
                            },
                            //filterRow: { visible: true },   // optional
                            onSelectionChanged: function (selectedItems) {
                                let selectedKey = selectedItems.selectedRowKeys[0];
                                e.component.option("value", selectedKey);   // dropdown ke liye
                                cellInfo.setValue = selectedKey;             // parent grid ke liye
                                e.component.close();
                            }
                        });

                        return $grid;
                    }
                },
                width: 400,
            }

        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#fdeliveryFeedingDetailContainer', col, dataSrc, "DeliveryFeeding", "iteM_CODE");
        if (dataSrc.length == 0) {
            $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

        //setTimeout(function () {
        //    var nextElement = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
        //    $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').focus(nextElement);  
        //}, 1500);
    },
    CloneRow: function (index) {
        debugger;
        const gridIns = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_FDeliveryFeeding.rowsCount += 1;
                const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
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
                    clonedRowData.__KEY__ = empr_FDeliveryFeeding.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_FDeliveryFeeding.rowsCount += 1;
            const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
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
                clonedRowData.__KEY__ = empr_FDeliveryFeeding.GenerateKey(36);
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
        const gridIns = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_FDeliveryFeeding.rowsCount += 1;
                //const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
                //gridInstance.addRow();
                const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_FDeliveryFeeding.GenerateKey(36), chK1: true, chk: "1" });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            //empr_FDeliveryFeeding.rowsCount += 1;
            //const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            empr_FDeliveryFeeding.rowsCount += 1;
            //const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
            //gridInstance.addRow();
            const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_FDeliveryFeeding.GenerateKey(36), chK1: true, chk: "1" });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    DeleteRow: function (index, dtCode) {
        debugger;
        const gridInstance = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_FDeliveryFeeding.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ code: dtCode }, "/FDeliveryFeeding/DeleteDeliveryFeedingDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_FDeliveryFeeding.rowsCount -= 1;
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
    InitQuickSearchGrid: function () {
        empr_FDeliveryFeeding.GetDeliveryFeedings();
    },
    GetDeliveryFeedings: function () {
        ajaxHelper.ajaxGetJson('/FDeliveryFeeding/GetDeliveryFeedings', function (data) {
            if (data.msgType == 1) {
                empr_FDeliveryFeeding.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        console.log('My Link', dataSrc);
        var columns = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin" && !Permissions.r_PRINT) {
                        $(`<div class="btn-group btn-group-sm">
                                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
                                       </div>`).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                                       <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.code} title="Edit"><i class="fa fa-edit"></i></a>
                                       <a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.code} title="PRINT"><i class="fa fa-print"></i></a>
                                       <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.code} title="COPY"><i class="fa fa-copy"></i></a>
                                       </div>`).appendTo(container);
                    }
                }
            },
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Voucher Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'astatus', caption: 'Status', },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            //{ dataField: 'sodA_VOUCHER', caption: 'Soda No', },
            {
                dataField: 'sodA_VOUCHER', caption: 'Soda No',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_FDeliveryFeeding.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.ptraN_ID) + ')')
                        .appendTo(container);
                }
            },
            { dataField: 'ref', caption: 'Reference No', },
            { dataField: 'selleR_CODE', caption: 'Seller', },
            { dataField: 'coa', caption: 'COA', },

            { dataField: 'sacT_CODE', caption: 'Seller Code', visible: false },
            //{ dataField: 'buyeR_CODE', caption: 'Buyer', },
            //{ dataField: 'bacT_CODE', caption: 'Buyer Code', visible: false },
            { dataField: 'brokeR_CODE', caption: 'Broker', },
            { dataField: 'bD_ACT_CODE', caption: 'Broker Code', visible: false },
            { dataField: 'cond', caption: 'Condition', },
            { dataField: 'crediT_DAYS', caption: 'Credit Days', },
            { dataField: 'sdate', caption: 'Soda Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'duE_DATE', caption: 'Due Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'deL_DATE', caption: 'Del Date', dataType: 'date', format: 'dd-MM-yyy', visible: false, showInColumnChooser: false },
            { dataField: 'remarks', caption: 'Remarks', },

            { dataField: 'iteM_CODE', caption: 'Item', },
            { dataField: 'qty', caption: 'Quantity', },
            { dataField: 'unit', caption: 'Unit', },
            { dataField: 'qtY2', caption: 'Quantity 2', },
            { dataField: 'baL_QTY', caption: 'Balance Quantity', },
            { dataField: 'rate', caption: 'Rate', },
            { dataField: 'rT_TYPE', caption: 'RT Type', },
            { dataField: 'amt', caption: 'Amount', },

            //{ dataField: 'bR_AMOUNT_BUYER' , caption: 'Brokery Buyer', },
            //{ dataField: 'wT_AMOUNT_BUYER' , caption: 'Weight Buyer', },
            { dataField: 'bardana', caption: 'Bardana', },
            { dataField: 'sbardana', caption: 'Seller Bardana', },
            { dataField: 'bR_AMOUNT_SELLER' , caption: 'Brokery Seller', },
            { dataField: 'wT_AMOUNT_SELLER' , caption: 'Weight Seller', },
            { dataField: 's_NET_AMOUNT' , caption: 'Seller Net', },
            //{ dataField: 'b_NET_AMOUNT', caption: 'Buyer Net', },
            //{ dataField: 'bgoD_CHARGES', caption: 'Buyer Godown', },
            { dataField: 'sgoD_CHARGES', caption: 'Seller Godown', },
            //{ dataField: 'blabour', caption: 'Buyer Labour', },
            { dataField: 'slabour', caption: 'Seller Labour', },
            //{ dataField: 'bfrieght', caption: 'Buyer Frieght', },
            //{ dataField: 'sfrieght', caption: 'Buyer Seller Frieght', },
            //{ dataField: 'bfumigation', caption: 'Buyer Fumigation', },
            { dataField: 'sfumigation', caption: 'Seller Fumigation', },
            //{ dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            //{ dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            //{ dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            //{ dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
            //{ dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            //{ dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            //{ dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            //{ dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            //{ dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
            //{ dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, },
            //{
            //    dataField: 'detail', calculateFilterExpression: function (value) {
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
            //    }
            //}
        ];
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/FDeliveryFeeding/GetDeliveryFeeding", "traN_ID", "DeliveryFeeding");
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
        //    { dataField: 'bR_AMOUNT_SELLER', caption: 'Brk Amt (Sellr)', },
        //    { dataField: 'bR_AMOUNT_BUYER', caption: 'Brk Amt (Buyr)', },
        //    { dataField: 'wT_AMOUNT_SELLER', caption: 'Wt Amt (Sellr)', },
        //    { dataField: 'wT_AMOUNT_BUYER', caption: 'Wt Amt (Buyr)', },
        //    { dataField: 'neT_AMT', caption: 'Net Amount', },
        //    { dataField: 'trucK_NO', caption: 'Truck No', visible: false },
        //    { dataField: 'conT_NO', caption: 'Cont No', visible: false },
        //    { dataField: 'scomp', caption: 'SComp', },
        //];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "DeliveryFeedingQS");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.MasterDetailDxGridBinding('#gridContainer', columns, detailColumns, dataSrc, "DeliveryFeeding");
    },
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var SELLER_CODE = $('#SellerCode').dxSelectBox('option', 'value');
        //var BUYER_CODE = $('#BuyerCode').dxSelectBox('option', 'value');
        var BROKER_CODE = $('#BrokerCode').dxSelectBox('option', 'value');
        var COND = $('#Condition').dxSelectBox('option', 'value');
        var REF = $("#REF").val();
        var D_NAME = $("#D_NAME").val();
        var D_NUM = $("#D_NUM").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var SDATE = $("#SDATE").val();
        var CREDIT_DAYS = $('#CreditDays').val();
        var DUE_DATE = $('#DueDate').val();
        var SHIP_STATUS = $('#SHIP_STATUS').is(":checked") == true ? 1 : 0;

        var totalAmount = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').getTotalSummaryValue('AmountTotal');
        $('#Sel_AMT').val(totalAmount);
        empr_FDeliveryFeeding.calculateSellerNetAmount();
        empr_FDeliveryFeeding.calculateBuyerNetAmount();

        var BR_AMT_BUYER = $('#BrBuyer').val();
        var BR_AMT_SELLER = $('#BrSeller').val();
        var WT_AMT_BUYER = $('#WtBuyer').val();
        var BARDANA = $('#Bardana').val();
        var SBARDANA = $('#SBardana').val();
        var WT_AMT_SELLER = $('#WtSeller').val();
        var B_NET_AMOUNT = $('#BuyNetAMT').val();
        var S_NET_AMOUNT = $('#SelNetAMT').val();

        var SGOD_CHARGES = $('#SGOD_CHARGES').val();
        var SLABOUR = $('#SLABOUR').val();
        var SFRIEGHT = $('#SFRIEGHT').val();
        var SFUMIGATION = $('#SFUMIGATION').val();
        var BGOD_CHARGES = $('#BGOD_CHARGES').val();
        var BLABOUR = $('#BLABOUR').val();
        var BFRIEGHT = $('#BFRIEGHT').val();
        var BFUMIGATION = $('#BFUMIGATION').val();

        var BR_AMOUNT_BUYER_ST = $('#BrBuyerSign').val() === "+" ? "P" : "M"; 
        var BR_AMOUNT_SELLER_ST = $('#BrSellerSign').val() === "+" ? "P" : "M"; 
        var WT_AMOUNT_BUYER_ST = $('#WtBuyerSign').val() === "+" ? "P" : "M"; 
        var WT_AMOUNT_SELLER_ST = $('#WtSellerSign').val() === "+" ? "P" : "M"; 
        var BARDANA_ST = $('#BardanaSign').val() === "+" ? "P" : "M"; 
        var SBARDANA_ST = $('#SBardanaSign').val() === "+" ? "P" : "M"; 
        var BGOD_CHARGES_ST = $('#BGOD_CHARGESSign').val() === "+" ? "P" : "M"; 
        var SGOD_CHARGES_ST = $('#SGOD_CHARGESSign').val() === "+" ? "P" : "M"; 
        var BLABOUR_ST = $('#BLABOURSign').val() === "+" ? "P" : "M"; 
        var SLABOUR_ST = $('#SLABOURSign').val() === "+" ? "P" : "M"; 
        var BFRIEGHT_ST = $('#BFRIEGHTSign').val() === "+" ? "P" : "M"; 
        var SFRIEGHT_ST = $('#SFRIEGHTSign').val() === "+" ? "P" : "M"; 
        var BFUMIGATION_ST = $('#BFUMIGATIONSign').val() === "+" ? "P" : "M"; 
        var SFUMIGATION_ST = $('#SFUMIGATIONSign').val() === "+" ? "P" : "M"; 

        var COB_CODE = $('#coBrokerCode').dxSelectBox('option', 'value');
        var CURR_CODE = $('#CURR_CODE').dxSelectBox('option', 'value');
        var UNIT = $('#UNIT').dxSelectBox('option', 'value');
        var CRATE = $('#CRATE').val();
        var SHIP_DATE = $('#SHIP_DATE').val();
        var SODA_TYPE = $('#SODA_TYPE').dxSelectBox('option', 'value');
        var SBF_TYPE = $('#SBF_TYPE').dxSelectBox('option', 'value');
        var COA = $('#ACCOUNT_NAME').dxSelectBox('option', 'value');


        var masterRecord = {
            TRAN_ID: CODE,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            SELLER_CODE: SELLER_CODE,
            //BUYER_CODE: BUYER_CODE,
            BROKER_CODE: BROKER_CODE,
            COND: COND,
            COB_CODE: COB_CODE,
            CURR_CODE: CURR_CODE,
            UNIT: UNIT,
            CRATE: CRATE,
            SHIP_DATE: SHIP_DATE,
            SODA_TYPE: SODA_TYPE,
            SBF_TYPE: SBF_TYPE,
            SHIP_STATUS: SHIP_STATUS,
            REF: REF,
            D_NAME: D_NAME,
            D_NUM: D_NUM,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS,
            SDATE: SDATE,
            DUE_DATE: DUE_DATE,
            CREDIT_DAYS: CREDIT_DAYS,
            BR_AMOUNT_BUYER: BR_AMT_BUYER,
            BR_AMOUNT_SELLER: BR_AMT_SELLER,
            WT_AMOUNT_SELLER: WT_AMT_SELLER,
            WT_AMOUNT_BUYER: WT_AMT_BUYER,
            B_NET_AMOUNT: B_NET_AMOUNT,
            S_NET_AMOUNT: S_NET_AMOUNT,
            BARDANA: BARDANA,
            SBARDANA: SBARDANA,
            SGOD_CHARGES: SGOD_CHARGES,
            SLABOUR: SLABOUR,
            SFRIEGHT: SFRIEGHT,
            SFUMIGATION: SFUMIGATION,
            BGOD_CHARGES: BGOD_CHARGES,
            BLABOUR: BLABOUR,
            BFRIEGHT: BFRIEGHT,
            BFUMIGATION: BFUMIGATION,
            BR_AMOUNT_BUYER_ST: BR_AMOUNT_BUYER_ST,
            BR_AMOUNT_SELLER_ST: BR_AMOUNT_SELLER_ST,
            WT_AMOUNT_BUYER_ST: WT_AMOUNT_BUYER_ST,
            WT_AMOUNT_SELLER_ST: WT_AMOUNT_SELLER_ST,
            BARDANA_ST: BARDANA_ST,
            SBARDANA_ST: SBARDANA_ST,
            BGOD_CHARGES_ST: BGOD_CHARGES_ST,
            SGOD_CHARGES_ST: SGOD_CHARGES_ST,
            BLABOUR_ST: BLABOUR_ST,
            SLABOUR_ST: SLABOUR_ST,
            BFRIEGHT_ST: BFRIEGHT_ST,
            SFRIEGHT_ST: SFRIEGHT_ST,
            BFUMIGATION_ST: BFUMIGATION_ST,
            SFUMIGATION_ST: SFUMIGATION_ST,
            COA:COA,
        }

        var detailRecords = [];
        if ($('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option("dataSource");
        }

        if (empr_FDeliveryFeeding.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },
    GetGridData: async function () {
        return await $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option("dataSource");
    },
    ValidateInfo: function () {

        var valid = true;
        var data = empr_FDeliveryFeeding.GetDataToSave();

        if (data.Master.SELLER_CODE == "" || data.Master.SELLER_CODE == null || data.Master.SELLER_CODE == undefined) {
            empr_helper.notify("Please select seller.", 2);
            valid = false;
            return valid;
        }

        //if (data.Master.BUYER_CODE == "" || data.Master.BUYER_CODE == null || data.Master.BUYER_CODE == undefined) {
        //    empr_helper.notify("Please select buyer.", 2);
        //    valid = false;
        //    return valid;
        //}

        data.Detail = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option("dataSource");

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

            //if (item.deL_DATE == "" || item.deL_DATE == null || item.deL_DATE == undefined) {
            //    empr_helper.notify("Please select DelDate at index " + index, 2);
            //    valid = false;
            //    return valid;
            //    console.log("Item at index " + index + " has empty DelDate.");
            //}
            //else {
            //    item.deL_DATE = empr_helper.notify(item.deL_DATE);
            //}
        });

        if (!valid) return;

        return empr_helper.validateDateRange($("#V_DATE").val(), minDate, maxDate);

        return valid;
    },
    SaveInfo: function () {
        debugger;
        var dataModel = empr_FDeliveryFeeding.GetDataToSave();
        console.log('dataModel.detail ......', dataModel.Detail);
        if (pickData == "Y") {
            if (!dataModel.Detail[0].hasOwnProperty('picK_ID') || dataModel.Detail[0].picK_ID == null || dataModel.Detail[0].picK_ID == undefined) {
                empr_helper.notify('Soda pick is required.', 2);
                return;
            }
        }


        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/FDeliveryFeeding/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                    empr_helper.selectedBill = data.data.code;
                }
                //empr_FDeliveryFeeding.GetDeliveryFeedingDetailsByCode($('#Code').val());
                //$('#BtnDelete').show();
                //empr_FDeliveryFeeding.ResetForm();
                if (dataClear == 1) {
                    empr_FDeliveryFeeding.GetDeliveryFeedingByCode($('#Code').val());
                }
                else {
                    empr_FDeliveryFeeding.ResetForm();
                }
                
                setTimeout(function () {
                    var nextElement = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').getCellElement(0, 'iteM_CODE');
                    $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').focus(nextElement);
                    $('#SDATE').focus();
                }, 400);
            }
        }, false, true);
    },
    GetDeliveryFeedingByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/FDeliveryFeeding/GetDeliveryFeedingByCode?code=' + code, function (data) {
            debugger;
            console.log('data',data);
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    $('#REF').val(response.ref);
                    $('#D_NAME').val(response.d_NAME);
                    $('#D_NUM').val(response.d_NUM);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);
                    $('#Condition').val(response.cond);
                    $('#SDATE').val(response.sdate);
                    $('#DueDate').val(response.duE_DATE);
                    $('#CreditDays').val(response.crediT_DAYS);
                    $('#BuyNetAMT').val(response.b_NET_AMOUNT);
                    $('#SelNetAMT').val(response.s_NET_AMOUNT);

                    $('#ACCOUNT_NAME').dxSelectBox('instance').option('value', response.coa);

                    $('#BrSeller').val(response.bR_AMOUNT_SELLER);
                    $('#WtSeller').val(response.wT_AMOUNT_SELLER);
                    $('#BrBuyer').val(response.bR_AMOUNT_BUYER);
                    $('#WtBuyer').val(response.wT_AMOUNT_BUYER);
                    $('#Bardana').val(response.bardana);
                    $('#SBardana').val(response.sbardana);

                    $('#SGOD_CHARGES').val(response.sgoD_CHARGES);
                    $('#SLABOUR').val(response.slabour);
                    $('#SFRIEGHT').val(response.sfrieght);
                    $('#SFUMIGATION').val(response.sfumigation);
                    $('#BGOD_CHARGES').val(response.bgoD_CHARGES);
                    $('#BLABOUR').val(response.blabour);
                    $('#BFRIEGHT').val(response.bfrieght);
                    $('#BFUMIGATION').val(response.bfumigation);

                    $('#BrSellerSign').val(response.bR_AMOUNT_SELLER_ST === "P" ? "+" : "-");
                    $('#WtSellerSign').val(response.wT_AMOUNT_SELLER_ST === "P" ? "+" : "-");
                    $('#BrBuyerSign').val(response.bR_AMOUNT_BUYER_ST === "P" ? "+" : "-");
                    $('#WtBuyerSign').val(response.wT_AMOUNT_BUYER_ST === "P" ? "+" : "-");
                    $('#BardanaSign').val(response.bardanA_ST === "P" ? "+" : "-");
                    $('#SBardanaSign').val(response.sbardanA_ST === "P" ? "+" : "-");

                    $('#SGOD_CHARGESSign').val(response.sgoD_CHARGES_ST === "P" ? "+" : "-");
                    $('#SLABOURSign').val(response.slabouR_ST === "P" ? "+" : "-");
                    $('#SFRIEGHTSign').val(response.sfrieghT_ST === "P" ? "+" : "-");
                    $('#SFUMIGATIONSign').val(response.sfumigatioN_ST === "P" ? "+" : "-");
                    $('#BGOD_CHARGESSign').val(response.bgoD_CHARGES_ST === "P" ? "+" : "-");
                    $('#BLABOURSign').val(response.blabouR_ST === "P" ? "+" : "-");
                    $('#BFRIEGHTSign').val(response.bfrieghT_ST === "P" ? "+" : "-");
                    $('#BFUMIGATIONSign').val(response.bfumigatioN_ST === "P" ? "+" : "-");
                    $('#CRATE').val(response.crate);


                    const invalidDate = "1900-01-01";
                    if (response.shiP_DATE !== invalidDate) {
                        if (response.shiP_STATUS == 0) {
                            $("#SHIP_DATE").attr("type", "date");
                            $("#SHIP_STATUS").prop("checked", false);
                        } else {
                            $("#SHIP_DATE").attr("type", "month");
                        }
                        $('#SHIP_DATE').val(response.shiP_DATE);
                        console.log("Shipment date is valid.");
                    } else {
                        $('#SHIP_DATE').val("");
                        console.log("Shipment date is invalid.");
                    }

                    empr_FDeliveryFeeding.InitDropdownsWithValue(response.selleR_CODE, response.brokeR_CODE, response.cond, response.sbF_TYPE, response.coB_CODE, response.curR_CODE, response.unit, response.sodA_TYPE, response.coa);
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

                    empr_FDeliveryFeeding.toggleOperatorsOnEdit();
                }
                if (data.detail.msgType == 1) {
                    const detailData = data.detail.data; 
                    let totalAmt = 0;
                    $.each(detailData, function (index, row) {
                        totalAmt += parseFloat(row.amt);
                    });
                    $('#Sel_AMT').val(totalAmt);
                    empr_FDeliveryFeeding.CreateGrid(data.detail.data);
                    $('.card-body').addClass('customHighlightForModifiedCells');
                    $("#Loader").hide();
                }
                else {
                    $("#Loader").hide();
                    empr_helper.notify(data.msg, data.msgType);
                }
            }
            else {
                $("#Loader").hide();
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    GetDeliveryFeedingDefaultOperators: function () {
        ajaxHelper.ajaxGetJson('/FDeliveryFeeding/GetDeliveryFeedingDefaultOperators', function (data) {
            if (data.msgType == 1) {
                var masterData = data.data.data;
                var response = masterData[0];
                if (masterData.length == 1) {
                    $('#BrSellerSign').val(response.bR_AMOUNT_SELLER_ST === "P" ? "+" : "-");
                    $('#WtSellerSign').val(response.wT_AMOUNT_SELLER_ST === "P" ? "+" : "-");
                    $('#BrBuyerSign').val(response.bR_AMOUNT_BUYER_ST === "P" ? "+" : "-");
                    $('#WtBuyerSign').val(response.wT_AMOUNT_BUYER_ST === "P" ? "+" : "-");
                    $('#BardanaSign').val(response.bardanA_ST === "P" ? "+" : "-");
                    $('#SBardanaSign').val(response.sbardanA_ST === "P" ? "+" : "-");
                    $('#SGOD_CHARGESSign').val(response.sgoD_CHARGES_ST === "P" ? "+" : "-");
                    $('#SLABOURSign').val(response.slabouR_ST === "P" ? "+" : "-");
                    $('#SFRIEGHTSign').val(response.sfrieghT_ST === "P" ? "+" : "-");
                    $('#SFUMIGATIONSign').val(response.sfumigatioN_ST === "P" ? "+" : "-");
                    $('#BGOD_CHARGESSign').val(response.bgoD_CHARGES_ST === "P" ? "+" : "-");
                    $('#BLABOURSign').val(response.blabouR_ST === "P" ? "+" : "-");
                    $('#BFRIEGHTSign').val(response.bfrieghT_ST === "P" ? "+" : "-");
                    $('#BFUMIGATIONSign').val(response.bfumigatioN_ST === "P" ? "+" : "-");

                    empr_FDeliveryFeeding.toggleOperatorsOnEdit();
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    calculateSellerNetAmount: function () {
        var totalAmount = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').getTotalSummaryValue('AmountTotal');
        $('#Sel_AMT').val(totalAmount);
        let amtSeller = parseFloat($('#Sel_AMT').val()) || 0;

        $(".sellerCalculations .number-input").each(function () {
            let inputField = $(this).find("input[type='text']");
            let hiddenSignField = $(this).find("input[type='hidden']");

            let value = parseFloat(inputField.val()) || 0;
            let sign = hiddenSignField.val();
            if (sign === "-") {
                amtSeller -= value;
            } else {
                amtSeller += value;
            }
        });

        $('#SelNetAMT').val(amtSeller);
    },
    calculateBuyerNetAmount: function () {
        var totalAmount = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').getTotalSummaryValue('AmountTotal');
        $('#Sel_AMT').val(totalAmount);
        let amtBuyer = parseFloat($('#Sel_AMT').val()) || 0;

        $(".buyerCalculations .number-input").each(function () {
            let inputField = $(this).find("input[type='text']");
            let hiddenSignField = $(this).find("input[type='hidden']");

            let value = parseFloat(inputField.val()) || 0;
            let sign = hiddenSignField.val();
            if (sign === "-") {
                amtBuyer -= value;
            } else {
                amtBuyer += value;
            }
        });

        $('#BuyNetAMT').val(amtBuyer);
    },
    GetDeliveryFeedingDetailsByCode: function (code) {
        ajaxHelper.ajaxGetJson('/FDeliveryFeeding/GetDeliveryFeedingDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_FDeliveryFeeding.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/FDeliveryFeeding/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_FDeliveryFeeding.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    InitSodaPickGrid: function () {
        var data = empr_FDeliveryFeeding.GetDataToSave();
        console.log(data.Master.SDATE);
        if (data.Master.SDATE == "" || data.Master.SDATE == null || data.Master.SDATE == undefined) {
            empr_helper.notify("Please select the soda date first.", 2); 
        }
        else {
            empr_FDeliveryFeeding.GetSodaBookFeedingDetailBySodaDate(data.Master.SDATE);
        }
    },
    GetSodaBookFeedingDetailBySodaDate: function (sodaDate) {
        debugger;

        ajaxHelper.ajaxGetJson('/FDeliveryFeeding/GetSodaBookFeedingDetailBySodaDate?sodaDate=' + sodaDate, function (data) {
            console.log('sodaData',data);
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
                        $('#SodaPickGridContainer').data('dxDataGrid').dispose();
                    }
                    empr_FDeliveryFeeding.CreateSodaPickGrid(data.data);
                    $('#SodaPickModal').modal('show');
                } else {
                    empr_helper.notify("No soda found.", 2); 
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateSodaPickGrid: function (dataSrc) {
        var col = [
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'v_DATE', caption: 'Transaction Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' },
            { dataField: 'qty', caption: 'S.Qty', allowEditing: false, },
            { dataField: 'dqty', caption: 'I.Qty', allowEditing: false, },
            { dataField: 'baL_QTY', caption: 'B.Qty', allowEditing: false, },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
            { dataField: 'rate', caption: 'Rate', allowEditing: false, },
            { dataField: 'cond', caption: 'Condition', allowEditing: false, },
            { dataField: 'seller', caption: 'Seller', allowEditing: false, },
            { dataField: 'buyer', caption: 'COA', allowEditing: false, },
            { dataField: 'broker', caption: 'Broker', allowEditing: false, },
            { dataField: 'voucheR_NO', caption: 'Voucher No', allowEditing: false, },
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
            { dataField: 'sbF_TYPE', caption: 'SBF_TYPE', visible: false, },
            { dataField: 'coB_CODE', caption: 'COB_CODE', visible: false, },
            { dataField: 'shiP_DATE', caption: 'SHIP_DATE', visible: false, },
            { dataField: 'sodA_TYPE', caption: 'SODA_TYPE', visible: false, },

            //{ dataField: 'qty', caption: 'D. QTY', },
            { dataField: 'amt', caption: 'Amount', allowEditing: false, },
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "DeliveryFeedingPick", "v_DATE", 'multiple');
        setTimeout(function () {
            $('#SodaPickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },
    AddSodaToDelivery: function () {
        if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
            $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
                var data = empr_FDeliveryFeeding.GetDataToSave();
                var IsDataAvailableInGrid = false;
                $.each(data.Detail, function (index, item) {
                    if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                        IsDataAvailableInGrid = true;
                    }
                });

                if (IsDataAvailableInGrid) {
                    var existingData = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option('dataSource');
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    console.log('1', selectedSodas);
                    selectedSodas = empr_FDeliveryFeeding.SetData(selectedSodas);
                    var brokerValue = '';
                    if ($('#BrokerCode').data('dxSelectBox') != undefined) {
                        brokerValue = $('#BrokerCode').data('dxSelectBox').option('value');
                    }
                    if (selectedSodas.length > 0) {
                        empr_FDeliveryFeeding.InitDropdownsWithValue(selectedSodas[0].selleR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].cond, selectedSodas[0].sbF_TYPE, selectedSodas[0].coB_CODE, selectedSodas[0].curR_CODE, selectedSodas[0].unit, selectedSodas[0].sodA_TYPE);
                        console.log(selectedSodas[0])
                        $('#REF').val(selectedSodas[0].ref);
                        $('#CRATE').val(selectedSodas[0].rate);
                        $('#REMARKS').val(selectedSodas[0].remarks);
                        $('#CreditDays').val(selectedSodas[0].crediT_DAYS);
                        $('#DueDate').val(empr_helper.PrepareDate(selectedSodas[0].duE_DATE));

                        const invalidDate = "1900-01-01";
                        if (selectedSodas[0].shiP_DATE !== invalidDate) {
                            if (selectedSodas[0].shiP_STATUS == 0) {
                                $("#SHIP_DATE").attr("type", "date");
                                $("#SHIP_STATUS").prop("checked", false);
                            } else {
                                $("#SHIP_DATE").attr("type", "month");
                            }
                            $('#SHIP_DATE').val(selectedSodas[0].shiP_DATE);
                            console.log("Shipment date is valid.");
                        } else {
                            $('#SHIP_DATE').val("");
                            console.log("Shipment date is invalid.");
                        }
                    }
                    var finalData = existingData.concat(selectedSodas);
                    $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option('dataSource', finalData);
                }
                else {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                    selectedSodas = empr_FDeliveryFeeding.SetData(selectedSodas);
                    console.log('2', selectedSodas);
                    var brokerValue = '';
                    if ($('#BrokerCode').data('dxSelectBox') != undefined) {
                        brokerValue = $('#BrokerCode').data('dxSelectBox').option('value');
                    }
                    if (selectedSodas.length > 0) {
                        empr_FDeliveryFeeding.InitDropdownsWithValue(selectedSodas[0].selleR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].cond, selectedSodas[0].sbF_TYPE, selectedSodas[0].coB_CODE, selectedSodas[0].curR_CODE, selectedSodas[0].unit, selectedSodas[0].sodA_TYPE);
                        $('#REF').val(selectedSodas[0].ref);
                        $('#CRATE').val(selectedSodas[0].rate);
                        $('#REMARKS').val(selectedSodas[0].remarks);
                        console.log(selectedSodas[0])
                        $('#CreditDays').val(selectedSodas[0].crediT_DAYS);
                        $('#DueDate').val(empr_helper.PrepareDate(selectedSodas[0].duE_DATE));

                        const invalidDate = "1900-01-01";
                        if (selectedSodas[0].shiP_DATE !== invalidDate) {
                            if (selectedSodas[0].shiP_STATUS == 0) {
                                $("#SHIP_DATE").attr("type", "date");
                                $("#SHIP_STATUS").prop("checked", false);
                            } else {
                                $("#SHIP_DATE").attr("type", "month");
                            }
                            $('#SHIP_DATE').val(selectedSodas[0].shiP_DATE);
                            console.log("Shipment date is valid.");
                        } else {
                            $('#SHIP_DATE').val("");
                            console.log("Shipment date is invalid.");
                        }
                    }
                    $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
                }
                $('.modal').hide();
                $('#V_DATE').focus();
            });
        }
        else {
            debugger;
            var data = empr_FDeliveryFeeding.GetDataToSave();
            var IsDataAvailableInGrid = false;
            $.each(data.Detail, function (index, item) {
                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
                    IsDataAvailableInGrid = true;
                }
            });

            if (IsDataAvailableInGrid) {
                var existingData = $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option('dataSource');
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                console.log('3', selectedSodas);
                selectedSodas = empr_FDeliveryFeeding.SetData(selectedSodas);
                var brokerValue = '';
                if ($('#BrokerCode').data('dxSelectBox') != undefined) {
                    brokerValue = $('#BrokerCode').data('dxSelectBox').option('value');
                }
                if (selectedSodas.length > 0) {
                    empr_FDeliveryFeeding.InitDropdownsWithValue(selectedSodas[0].selleR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].cond, selectedSodas[0].sbF_TYPE, selectedSodas[0].coB_CODE, selectedSodas[0].curR_CODE, selectedSodas[0].unit, selectedSodas[0].sodA_TYPE, selectedSodas[0].coA_CODE);
                    console.log(selectedSodas[0])
                    $('#REF').val(selectedSodas[0].ref);
                    $('#CRATE').val(selectedSodas[0].rate);
                    $('#REMARKS').val(selectedSodas[0].remarks);
                    $('#CreditDays').val(selectedSodas[0].crediT_DAYS);
                    $('#DueDate').val(empr_helper.PrepareDate(selectedSodas[0].duE_DATE));

                    const invalidDate = "1900-01-01";
                    if (selectedSodas[0].shiP_DATE !== invalidDate) {
                        if (selectedSodas[0].shiP_STATUS == 0) {
                            $("#SHIP_DATE").attr("type", "date");
                            $("#SHIP_STATUS").prop("checked", false);
                        } else {
                            $("#SHIP_DATE").attr("type", "month");
                        }
                        $('#SHIP_DATE').val(selectedSodas[0].shiP_DATE);
                        console.log("Shipment date is valid.");
                    } else {
                        $('#SHIP_DATE').val("");
                        console.log("Shipment date is invalid.");
                    }
                }
                var finalData = existingData.concat(selectedSodas);
                $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option('dataSource', finalData);
            }
            else {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                console.log('4', selectedSodas);
                selectedSodas = empr_FDeliveryFeeding.SetData(selectedSodas);
                var brokerValue = '';
                if ($('#BrokerCode').data('dxSelectBox') != undefined) {
                    brokerValue = $('#BrokerCode').data('dxSelectBox').option('value');
                }
                if (selectedSodas.length > 0) {
                    empr_FDeliveryFeeding.InitDropdownsWithValue(selectedSodas[0].selleR_CODE, selectedSodas[0].brokeR_CODE, selectedSodas[0].cond, selectedSodas[0].sbF_TYPE, selectedSodas[0].coB_CODE, selectedSodas[0].curR_CODE, selectedSodas[0].unit, selectedSodas[0].sodA_TYPE, selectedSodas[0].coA_CODE);
                        console.log(selectedSodas[0])
                    $('#CreditDays').val(selectedSodas[0].crediT_DAYS);
                    $('#CRATE').val(selectedSodas[0].rate);

                    const invalidDate = "1900-01-01";
                    if (selectedSodas[0].shiP_DATE !== invalidDate) {
                        if (selectedSodas[0].shiP_STATUS == 0) {
                            $("#SHIP_DATE").attr("type", "date");
                            $("#SHIP_STATUS").prop("checked", false);
                        } else {
                            $("#SHIP_DATE").attr("type", "month");
                        }
                        $('#SHIP_DATE').val(selectedSodas[0].shiP_DATE);
                        console.log("Shipment date is valid.");
                    } else {
                        $('#SHIP_DATE').val("");
                        console.log("Shipment date is invalid.");
                    }

                    //$('#SHIP_DATE').val(selectedSodas[0].shiP_DATE); 
                    $('#DueDate').val(empr_helper.PrepareDate(selectedSodas[0].duE_DATE));
                    $('#REF').val(selectedSodas[0].ref);
                    $('#REMARKS').val(selectedSodas[0].remarks);
                }
                $('#fdeliveryFeedingDetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
            }

            $('.modal').hide();
            $('#V_DATE').focus();
        }
    }, 
    SetData: function (dataSource) {
        $.each(dataSource, function (index, item) {
            item.chK1 = false;
            item.chk = "0";
            if (item.unit != '') {
                var selectedUnit = Units.filter(u => u.key == item.unit);
                if (selectedUnit.length > 0) {
                    item.qtY2 = selectedUnit[0].qty;
                } 
            }
            else {
                item.qtY2 = 0;
            }

            var qty = parseFloat(item.qty) || 0;
            var qtY2 = parseFloat(item.qtY2) || 1;
            var rate = parseFloat(item.rate) || 0;
            var rT_TYPE = parseFloat(item.rT_TYPE) || 0;
            if (!isNaN(qty) && !isNaN(qtY2)) {
                if (item.chK1) {
                    item.baL_QTY = qty * qtY2;
                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                        item.amt = item.baL_QTY * rate;
                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                        if (perRate > -1 && perRate != 'Infinity') {
                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                        }
                    }

                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                        item.amt = qty * rate;
                    }
                    item.neT_AMT = item.amt;
                }
                else {
                    item.baL_QTY = qty;
                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'Y') {
                        var perRate = parseFloat(rate / rT_TYPE) || 0;
                        if (perRate > -1 && perRate != 'Infinity') {
                            item.amt = (item.baL_QTY * perRate).toFixed(2);
                        }
                    }

                    if (empr_FDeliveryFeeding.Branch_RT_TYPE == 'N') {
                        item.amt = qty * rate;
                    }
                    item.neT_AMT = item.amt;
                }
            }

            item.__KEY__ = empr_FDeliveryFeeding.GenerateKey(36);
            //item.duE_DATE = empr_helper.GetCurrentDate();
            //item.deL_DATE = empr_helper.GetCurrentDate()

            //newData.qty = value;
            //var qty = parseFloat(newData.qty) || 0;
            //var qtY2 = parseFloat(currentRowData.qtY2) || 1;
            //var rate = parseFloat(currentRowData.rate) || 0;
            //var rT_TYPE = parseFloat(currentRowData.rT_TYPE) || 0;
            //if (isNaN(qty)) {
            //    empr_helper.notify("Please enter the correct quantity.", 2);
            //}
            //if (isNaN(qtY2)) {
            //    empr_helper.notify("Please enter the correct quantity2.", 2);
            //}
            //if (!isNaN(qty) && !isNaN(qtY2)) {
            //    if (currentRowData.chK1) {
            //        newData.baL_QTY = qty * qtY2;
            //        var perRate = parseFloat(rate / rT_TYPE) || 0;
            //        if (perRate > -1 && perRate != 'Infinity') {
            //            newData.amt = (newData.baL_QTY * perRate).toFixed(2);
            //        }
            //    }
            //    else {
            //        newData.baL_QTY = qty;
            //        var perRate = parseFloat(rate / rT_TYPE) || 0;
            //        if (perRate > -1 && perRate != 'Infinity') {
            //            newData.amt = (newData.baL_QTY * perRate).toFixed(2);
            //        }
            //    }
            //}

            //var brAmountBuyer = parseFloat(currentRowData.bR_AMOUNT_BUYER) || 0;
            //var brAmountSeller = parseFloat(currentRowData.bR_AMOUNT_SELLER) || 0;
            //var wtAmountBuyer = parseFloat(currentRowData.wT_AMOUNT_BUYER) || 0;
            //var wtAmountSeller = parseFloat(currentRowData.wT_AMOUNT_SELLER) || 0;
            //newData.neT_AMT = (parseFloat(newData.amt) + parseFloat(brAmountBuyer) - parseFloat(brAmountSeller) + parseFloat(wtAmountBuyer) - parseFloat(wtAmountSeller));
        });

        return dataSource;
    },
    InitDropdowns: function () {
        debugger;
        empr_FDeliveryFeeding.InitConditionDDL();
        ajaxHelper.ajaxGetJson("/FDeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_FDeliveryFeeding.InitSellerCodeDDL(data.data);
                //empr_FDeliveryFeeding.InitBuyerCodeDDL(coa);
                empr_FDeliveryFeeding.InitBrokerCodeDDL(data.data);
                empr_FDeliveryFeeding.InitCoBrokerCodeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    InitDropdownsWithValue: function (sCode, brCode, condition, sbfType, coBro, curr, unit, sodaType,coa) {
        debugger;
        empr_FDeliveryFeeding.InitConditionDDL(condition);
        ajaxHelper.ajaxGetJson("/FDeliveryFeeding/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_FDeliveryFeeding.InitSellerCodeDDL(data.data, sCode);
                //empr_FDeliveryFeeding.InitBuyerCodeDDL(data.data, bCode);
                empr_FDeliveryFeeding.InitBrokerCodeDDL(data.data, brCode);
                empr_FDeliveryFeeding.InitCoBrokerCodeDDL(data.data, coBro);
                empr_FDeliveryFeeding.InitCurrencyDDL(curr);
                empr_FDeliveryFeeding.InitAccountDDL(coa);

                empr_FDeliveryFeeding.InitUnitDDL(unit);
                empr_FDeliveryFeeding.InitSodaTypeDDL(sodaType);
                empr_FDeliveryFeeding.InitTypeDDL(sbfType);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },
    InitSellerCodeDDL: function (dataSource, selectedValue) {
        console.log(selectedValue);
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
    InitConditionDDL: function (selectedValue) {

        //var dataSource = [
        //    { key: 'Adv', value: 'Advance' },
        //    { key: 'Cash', value: 'Cash' },
        //    { key: 'Cr', value: 'Credit Days' },
        //    { key: 'CrD', value: 'Credit Date' }
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
                console.log('1',e);
                console.log('2',e.value);
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
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/FDeliveryFeeding/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    selectedValue = data.data[0].mD_ID;
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
    GeneratePrintReport: function () {
        empr_FDeliveryFeeding.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the delivery in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/FDeliveryFeeding/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    InitTypeDDL: function (_selectedValue) {
        $('#SBF_TYPE').dxSelectBox({
            dataSource: [
                { value: 'LOC', text: 'Local' },
                { value: 'IMP', text: 'Import' },
                { value: 'EXP', text: 'Export' },
                { value: 'IND', text: 'Indent' }
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
            searchTimeout: 500,
            onInitialized: function (e) {
                if (!_selectedValue) {
                    e.component.option('value', 'LOC');
                }

                // delay trigger to avoid E0009
                setTimeout(function () {
                    var currentVal = e.component.option("value");
                    ApplyHideShowLogic(currentVal, e.component);
                }, 200);  // 200ms safe delay
            },
            onValueChanged: function (e) {
                ApplyHideShowLogic(e.value, e.component);
            },
        });

        // common logic in one function
        function ApplyHideShowLogic(value, component) {
            if (value && value !== '') {
                if (value === "LOC") {
                    $('.ImpExp').hide();
                    $('.coBroker').hide();
                    //$('#CRATE').val('');
                    //$('#SHIP_DATE').val('');
                    //$('#CURR_CODE').dxSelectBox('instance').option('value', '');
                    //$('#UNIT').dxSelectBox('instance').option('value', '');
                    //$('#SODA_TYPE').dxSelectBox('instance').option('value', '');
                }
                else if (value === "IND") {
                    $('.ImpExp').show();
                    $('.coBroker').show();
                    $('.IndHide').hide();
                }
                else if (value === "IMP" || value === "EXP") {  // yahan aapka case "Exp" vs "EXP" ka tha
                    $('.ImpExp').show();
                    $('.coBroker').hide();
                }
                else {
                    $('.ImpExp').show();
                    $('.coBroker').hide();
                }
            }
            else {
                $('.ImpExp').hide();
                $('.coBroker').hide();
                //$('#CRATE').val('');
                //$('#SHIP_DATE').val('');
                //$('#CURR_CODE').dxSelectBox('instance').option('value', '');
                //$('#UNIT').dxSelectBox('instance').option('value', '');
                //$('#SODA_TYPE').dxSelectBox('instance').option('value', '');
            }
        }
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
    //MoveFocusToGridShiftTab: function (event, gridElement, rowIndex, dataField) {
    //    if (event.key === 'Tab') {
    //        var SBF_TYPE = $('#SBF_TYPE').dxSelectBox('option', 'value');
    //        event.preventDefault();
    //        if (event.shiftKey) {
    //            $('#REMARKS').focus();
    //        } else {
    //            debugger
    //            if (SBF_TYPE == "LOC") {
    //                var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
    //                $(nextElement).click();
    //                $(gridElement).dxDataGrid('instance').focus(nextElement);
    //            } else {
    //                $('#CURR_CODE').data('dxSelectBox').focus();
    //            }
    //        }
    //    }
    //},
    openVoucherPage(link, tran_Id) {
        debugger
        console.log(link)
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: tran_Id }, '*');
            }, 1000);
        });
    },
}