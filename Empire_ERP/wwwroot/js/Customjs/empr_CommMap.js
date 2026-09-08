var empr_commMap = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    InitEvents: function () {
        $(document).ready(function () {
            //console.log('account', Accounts);
            empr_commMap.ResetForm();

            //empr_commMap.InitSalesManDDL();
            empr_commMap.InitSalesManDDL(null, 'SalesMan');
            empr_commMap.InitQuickSearchGrid();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_commMap.GetCommisionMapByCode(data.traN_ID);
                }
            });
            $('body').on('click', '#BtnQuickSearch', function () {
                empr_commMap.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSave', function () {
                console.log('dataClear', dataClear);
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        empr_commMap.ValidateAndPrepareDataForSave();
                    }
                } else {
                    empr_commMap.ValidateAndPrepareDataForSave();
                }
            });

           

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_commMap.GetCommisionMapByCode(id);
            });

            $('body').on('click', '.elm_copy', function () {
                //debugger
                var id = $(this).attr("reportid");
                var selected_value = $(this).attr("salesman");
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
                    //debugger
                    empr_commMap.InitSalesManDDL(selected_value, 'SalesManModal')
                    empr_helper.selectedBill = id;
                    $('#CopySalesMan').modal('show');
                });
            });
         
            $('body').on('click', '#saveCopiedRecord', function () {
                var selectedSalesman = $('#SalesManModal').dxSelectBox('instance').option('value');
                ajaxHelper.ajaxPostJsonData({ grouP_CODE: empr_helper.selectedBill, salesman: selectedSalesman }, "/CommMap/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_commMap.GetCommisionMapByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_commMap.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_commMap.ResetForm();
                $('#SalesMan').dxSelectBox('instance').option('value', null);

                
            });

            //$('body').on('click', '.btn-print,#BtnGenerateReport', function () {
            //    empr_commMap.GeneratePrintReport();
            //});

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },

    ResetForm: function () {
        empr_commMap.CreateGrid([{ __KEY__: empr_commMap.GenerateKey(36), dC_TYPE: empr_commMap.DC_TYPE }]);
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE, #hdnDOC').val('');
 
        $('#BtnDelete').hide();
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        //$('#SalesMan').dxSelectBox('instance').option('placeholder', 'Select Salesman');
        //$('#SalesMan').dxSelectBox('instance').option('value', null);
   
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

    CreateGrid: function (dataSrc) {
        //debugger;
        console.log('CreateGrid', dataSrc);
        if (dataSrc.length > 0) {
            empr_commMap.rowsCount = dataSrc.length - 1;
           
        }
        //helper
        var commisionType = empr_helper.commType;
        console.log(commisionType);
        console.log("ITEMS" ,ItemMaster);
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
                    console.log('options', options);
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_commMap.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_commMap.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_commMap.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        let costCenterAction = '';
                        let knockOffAction = '';
                        if ($('#Code').val() != '') {
                            if (options.data.dC_TYPE === "D" && options.data.partY_CODE == 0 && options.data.dT_CODE) {
                                costCenterAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 8px; color:#FFD700" onclick="empr_commMap.ShowCostCenterModal(${options.data.grouP_CODE},${options.data.dT_CODE},'${options.data.dT_DESC}',${options.data.amt},${options.data.partY_CODE})"><i class="fa fa-coins"></i></a>`;
                            }
                            if (options.data.partY_CODE > 0 && options.data.dT_CODE) {
                                knockOffAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 8px;" onclick="empr_commMap.ShowKnockOffModal(${options.data.grouP_CODE},${options.data.dT_CODE},${options.data.amt},${options.data.partY_CODE},${options.data.acT_CODE})"><i class="fa fa-link"></i></a>`;
                            }
                        }
                        $(`<div class="btn-group btn-group-sm">
                       <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_commMap.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                       <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_commMap.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                       <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_commMap.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
                       ${costCenterAction}
                       ${knockOffAction}
                       </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'grouP_CODE',
                caption: 'Code',
                width: 100,
                visible: false,
            },
            {
                dataField: 'dT_CODE',
                caption: 'dtcode',
                visible: false,
            },
            {
                dataField: 'iteM_CODE',
                caption: 'Items',
                //width:200,
                lookup: {
                    dataSource: ItemMaster,
                    displayExpr: 'value', 
                    valueExpr: 'key'     
                },
                editCellTemplate: function (cellElement, cellInfo) {
                    $('<div>').dxSelectBox({
                        dataSource: ItemMaster,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: cellInfo.value,
                        searchEnabled: true,
                        onValueChanged: function (e) {
                            cellInfo.setValue(e.value);

         
                            cellInfo.row.data.ITEM_KEY = e.value;
                        }
                    }).appendTo(cellElement);
                }
            },


            {
                dataField: 'sacT_CODE',
                visible: false
            },


            {
                dataField: 'sacT_CODE',
                visible: false
            },

          
            {
                dataField: 'comM_UNIT',
                caption: 'Type',
                width: 120,
                allowSorting: false,
                lookup: {
                    dataSource: commisionType,
                    displayExpr: 'value',
                    valueExpr: 'key'
                },
                setCellValue: function (newData, value) {
                    // Update the correct field
                    newData.comM_UNIT = value;

                    // Also set to hidden field if needed
                    $("#UNITTYPE").val(value);
                }
            },

            {   
                dataField: 'comM_VALUE',
                caption: 'Value',
                dataType: 'number',
                width: 120,
                format: { type: 'fixedPoint', precision: 0 },
                
            },
       
        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "CashReceiptVoucher", "custoM_ACT_CODE");
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
        //debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        let dataSource = gridInstance.option("dataSource") || [];

        if (dataSource.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add " + Limit + " records.", 2);
            return;
        }


        if (gridInstance.hasEditData()) {
            gridInstance.saveEditData().done(() => {
                cloneRowAtIndex(index);
            });
        } else {
            cloneRowAtIndex(index);
        }

        function cloneRowAtIndex(idx) {
            let dataSource = gridInstance.option("dataSource") || [];
            if (!dataSource[idx]) return;

            let clonedRowData = $.extend(true, {}, dataSource[idx]);

            if (clonedRowData.hasOwnProperty('dT_CODE')) delete clonedRowData.dT_CODE;

            clonedRowData.__KEY__ = empr_commMap.GenerateKey(36);

            let newDataSource = [clonedRowData, ...dataSource];
            gridInstance.option("dataSource", newDataSource);

            gridInstance.refresh();

            // Update row count if needed
            empr_commMap.rowsCount += 1;
        }
    },

    AddRow: function () {
        //debugger;
        const gridIns = $('#DetailContainer').dxDataGrid('instance');
        const dataSrc = gridIns.option("dataSource");

        if (dataSrc.length >= Limit && Limit != 0) {
            empr_helper.notify("You can only add  " + Limit + " records.", 2);
            return;
        }
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_commMap.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_commMap.GenerateKey(36), grouP_CODE: empr_commMap.grouP_CODE });
                gridInstance.option("dataSource", dataSource);
                console.log("dtaaa", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_commMap.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_commMap.GenerateKey(36), dC_TYPE: empr_commMap.DC_TYPE });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    //AddRow: function () {
    //    const gridIns = $('#DetailContainer').dxDataGrid('instance');
    //    const dataSrc = gridIns.option("dataSource");

    //    if (dataSrc.length >= Limit && Limit != 0) {
    //        empr_helper.notify("You can only add " + Limit + " records.", 2);
    //        return;
    //    }

    //    const gridInstance = $('#DetailContainer').dxDataGrid('instance');
    //    const dataSource = gridInstance.option("dataSource");

      
    //    const groupCode = $('#MasterContainer').dxForm('instance').option('formData').GROUP_CODE || 0;


    //    const newRow = {
    //        __KEY__: empr_commMap.GenerateKey(36),
    //        DT_CODE: empr_commMap.dT_CODE,  // backend can generate new DT_CODE if 0
    //        GROUP_CODE: grouP_CODE            // link to existing master
    //        // DC_TYPE removed
    //    };

    //    dataSource.unshift(newRow);
    //    gridInstance.option("dataSource", dataSource);
    //    gridInstance.refresh();
    //},


    DeleteRow: function (index, dtCode) {
        //debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        console.log("datasourcesss", dataSource);
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_commMap.rowsCount -= 1;
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
                            //debugger;
                            ajaxHelper.ajaxPostJsonData({ gcode: $('#Code').val(), code: dtCode }, "/CommMap/DeleteCommisionMapDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_commMap.rowsCount -= 1;
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
                var row = dataSource[index];
                if (dtCode != '' && dtCode != null && dtCode != undefined) {
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
                            ajaxHelper.ajaxPostJsonData({ gcode: $('#Code').val(), code: dtCode }, "/CommMap/DeleteCommisionMapDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    empr_commMap.CreateGrid([{ __KEY__: empr_commMap.GenerateKey(36), dC_TYPE: empr_commMap.DC_TYPE }]);
                                }
                            }, false, true);
                        });
                    } else {
                        empr_helper.notify("You are not allowed to delete the last row.", 2);
                    }
                } else {
                    empr_commMap.CreateGrid([{ __KEY__: empr_commMap.GenerateKey(36), dC_TYPE: empr_commMap.DC_TYPE }]);
                    empr_helper.notify("You are not allowed to delete the last row.", 2);
                }
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


    //InitSalesManDDL: function (_selectedValue) {

    //    $.ajax({
    //        url: 'CommMap/GetSalesMan',
    //        method: 'GET',
    //        success: function (data) {

    //            console.log("salesman ", data);

    //            if (data.msgType == 1) {
    //                $('#SalesMan').dxSelectBox({
    //                    dataSource: data.data,
    //                    displayExpr: 'value',
    //                    valueExpr: 'key',
    //                    value: _selectedValue,
    //                    searchEnabled: true,
    //                    width: '100%',
    //                    placeholder: 'Search',
    //                    showClearButton: true,
    //                    dropDownOptions: {
    //                        height: 'auto'
    //                    },
    //                    pagingEnabled: true,
    //                    searchTimeout: 500,

    //                    onValueChanged: function (e) {
    //                        //debugger;

    //                        if (e.value != '' && e.value != null) {
    //                            var items = e.component._dataSource._items;
    //                            var item = items.find(i => i.key == e.value);

    //                            if (item) {
    //                                $('#Rate').val(item.rate);
    //                                $('#SACT_CODE').val(item.code); 
    //                            }
    //                        } else {
    //                            $('#Rate').val('');
    //                            $('#SACT_CODE').val(''); 
    //                        }
    //                    }
    //                });
    //            } else {
    //                empr_helper.notify(data.data, data.msgType);
    //            }
    //        },
    //        error: function (error) {
    //            console.error('Error fetching data:', error);
    //        }
    //    });
    //},

    InitSalesManDDL: function (_selectedValue, targetId) {
        $.ajax({
            url: 'CommMap/GetSalesMan',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#' + targetId).dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
                        searchEnabled: true,
                        width: '100%',
                        placeholder: 'Search',
                        showClearButton: true,
                        dropDownOptions: {
                            height: 'auto'
                        },
                        pagingEnabled: true,
                        searchTimeout: 500,
                        onValueChanged: function (e) {
                            if (e.value) {
                                var items = e.component._dataSource._items;
                                var item = items.find(i => i.key == e.value);
                                if (item) {
                                    $('#Rate').val(item.rate);
                                    $('#SACT_CODE').val(item.code);
                                }
                            } else {
                                $('#Rate').val('');
                                $('#SACT_CODE').val('');
                            }
                        }
                    });
                } else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

  

    InitQuickSearchGrid: function () {
        empr_commMap.GetCommisionMap();
    },

    GetCommisionMap: function () {
        ajaxHelper.ajaxGetJson('/CommMap/GetCommisionMap', function (data) {
            if (data.msgType == 1) {
                empr_commMap.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {

        console.log("datass",dataSrc)
        //debugger;
        var columns = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                //<a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>

                if (Permissions != "Admin" && !Permissions.r_PRINT) {
                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>
                               </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                               <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>
                               <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.salesman} reportid=${options.data.grouP_CODE} title="COPY"><i class="fa fa-copy"></i></a>
                               </div>`).appendTo(container);
                }
            }
        },

            { dataField: 'grouP_CODE', caption: 'Code', },
            { dataField: 'dT_CODE', caption: 'DetailCode', visible: false },
            { dataField: 'sacT_CODE', caption: 'Sact Code ', visible: false },

            { dataField: 'salesmaN_NAME', caption: 'Salesman Name' },

            //{ dataField: 'grouP_CODE', caption: 'Code',  },
            //{ dataField: 'salesmaN_NAME', caption: 'Salesman', },
            { dataField: 'salesman', caption: 'Salesman', visible: false },
   

        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "CashReceiptVoucherQS", "single");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/CashReceiptVoucher/GetCommisionMap", "dT_CODE", "CashReceiptVoucher", "multiple");
    },
    ValidateAndPrepareDataForSave: function () {
        //debugger;
        const gridInstance = $('#DetailContainer').dxDataGrid('instance');

        if (gridInstance.hasEditData()) {
            // Save any pending edits first
            gridInstance.saveEditData().done(function () {
                let detailRecords = gridInstance.option("dataSource");

                // Flatten grouped data
                if (Array.isArray(detailRecords) && detailRecords.some(item => item.key !== undefined)) {
                    detailRecords = detailRecords.flatMap(group => group.items || []);
                }

                
                if (!Array.isArray(detailRecords) || detailRecords.length === 0) {
                    empr_helper.notify("No detail records to save.", 2);
                    return;
                }

                
                if (!$("#Code").val()) {
                    detailRecords.reverse();
                }

                debugger;
                for (let obj of detailRecords) {
                    obj.GROUP_CODE = $("#Code").val();
                    obj.ITEM_CODE = obj.iteM_CODE || obj.ITEM_KEY;
                    obj.SACT_CODE = obj.sacT_CODE || $("#SACT_CODE").val();
                    obj.COMM_UNIT = obj.comM_UNIT || $("#UNITTYPE").val();
                    obj.COMM_VALUE = obj.comM_VALUE || $("#COMM_VAL").val();
                    obj.SALESMAN = obj.SALESMAN || $('#SalesMan').dxSelectBox('option', 'value');
                    obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
            

                    if (!obj.SALESMAN) {
                        empr_helper.notify("Salesman is required for all records.", 2);
                        return;
                    }
                    if (!obj.ITEM_CODE) {
                        empr_helper.notify("Item is required for all records.", 2);
                        return;
                    }
                    if (!obj.COMM_UNIT) {
                        empr_helper.notify("Commission unit is required for all records.", 2);
                        return;
                    }
                  
                }

                // After validation, save
                empr_commMap.SaveInfo(detailRecords);
            });
        } else {
            // When no pending edits
            let detailRecords = gridInstance.option("dataSource");

            if (Array.isArray(detailRecords) && detailRecords.some(item => item.key !== undefined)) {
                detailRecords = detailRecords.flatMap(group => group.items || []);
            }

            if (!Array.isArray(detailRecords) || detailRecords.length === 0) {
                empr_helper.notify("No detail records to save.", 2);
                return;
            }

            if (!$("#Code").val()) {
                detailRecords.reverse();
            }

            for (let obj of detailRecords) {
                obj.GROUP_CODE = obj.grouP_CODE || 0;
                obj.ITEM_CODE = obj.iteM_CODE || obj.ITEM_KEY;
                obj.SACT_CODE = obj.SACT_CODE || $("#SACT_CODE").val();
                obj.COMM_UNIT = obj.comM_UNIT || $("#UNITTYPE").val();
                obj.COMM_VALUE = obj.comM_VALUE || $("#COMM_VAL").val();
                obj.SALESMAN = $('#SalesMan').dxSelectBox('option', 'value');
                obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
                obj.GROUP_CODE = $("#Code").val();
                obj.REMARKS = $("#REMARKS").val();
                obj.DOC = $("#hdnDOC").val();

                if (!obj.SALESMAN) {
                    empr_helper.notify("Salesman is required for all records.", 2);
                    return;
                }
                if (!obj.ITEM_CODE) {
                    empr_helper.notify("Item is required for all records.", 2);
                    return;
                }
              
                if (!obj.COMM_VALUE || obj.COMM_VALUE <= 0) {
                    empr_helper.notify("Commission value must be greater than 0.", 2);
                    return;
                }
            }

            empr_commMap.SaveInfo(detailRecords);
        }
    },

    SaveInfo: function (detailRecords) {
        //debugger;
        console.log(detailRecords)
        ajaxHelper.ajaxPostJsonData({ modelRecord: detailRecords }, "/CommMap/Save", function (data) {
            console.log('SaveInfo Responce', data);
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgError != null) {
                empr_helper.notify(data.msgError, 2);
            }
            if (data.msgType == 1) {
                debugger;
                if ($("#Code").val() == 0
                    || $("#Code").val() == null
                    || $("#Code").val() == undefined
                    || $("#Code").val() == "") {
                    $('#Code').val(data.data.code);
                    empr_helper.selectedBill = data.data.code;
                    //empr_commMap.ResetForm();
                //    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                if (dataClear == 1) {
                    empr_commMap.GetCommisionMapByCode($('#Code').val());
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                    } else {
                        $('#BtnDelete').show();
                    }
                }
                else {
                    empr_commMap.ResetForm();
                    $('#SalesMan').dxSelectBox('instance').option('value', null);
                        

                }


            }
        }, false, true);
    },

    GetCommisionMapByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/CommMap/GetCommisionMapByCode?code=' + code, function (data) {
            //debugger;
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                console.log("new data ", masterData);
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.grouP_CODE);
                    $('#SACT_CODE').val(response.sacT_CODE);
               
                    $('#SalesMan').dxSelectBox('instance').option('value', response.salesman);
                   
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);

                 
                   
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
                    empr_commMap.CreateGrid(data.detail.data);
                    $('.card-body').addClass('customHighlightForModifiedCells');
                    $("#Loader").hide();
                }
                else {
                    empr_helper.notify("2" + data.msg, data.msgType);
                    $("#Loader").hide();
                }
            }
            else {
                empr_helper.notify("1" + data.msg, data.msgType);
                $("#Loader").hide();
            }
        }, false, true);
    },

    GetCommisionMapDetailsByCode: function (code) {
        console.log('edit call',code);
        debugger;
        ajaxHelper.ajaxGetJson('/CommMap/GetCommisionMapDetailsByCode?code=' + code, function (data) {
            //console.log(data)
            if (data.msgType == 1) {
                empr_commMap.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/CommMap/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_commMap.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

 
}