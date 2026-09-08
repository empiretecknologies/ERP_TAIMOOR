var empr_CustomerPricing = {
    totalCount: 0,
    rowsCount: 0,
    DC_TYPE: '',
    act_code:'',
    InitEvents: function () {
        $(document).ready(function () {
            //console.log('account', Accounts);
            empr_CustomerPricing.ResetForm();
            //empr_CustomerPricing.InitSalesManDDL();
            //PartyType
            empr_CustomerPricing.InitPartyDDL(null, 'PARTY_CODE');
            empr_CustomerPricing.InitQuickSearchGrid();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    $('#Code').val(data.traN_ID);
                    empr_CustomerPricing.GetCommisionMapByCode(data.traN_ID);
                }
            });
            $('body').on('click', '#BtnQuickSearch', function () {
                empr_CustomerPricing.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        empr_CustomerPricing.ValidateAndPrepareDataForSave();
                    }
                } else {
                    empr_CustomerPricing.ValidateAndPrepareDataForSave();
                }
            });

           

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_helper.selectedBill = id;
                empr_CustomerPricing.GetCommisionMapByCode(id);
            });

            $('body').on('click', '.elm_copy', function () {
                debugger
                var id = $(this).attr("reportid");
                //var selected_value = $(this).attr("party");
                var selected_value = parseInt($(this).attr("party"), 10);

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
                    $('#CustomerPricingCopy').modal('show');
                    empr_CustomerPricing.InitPartyDDL(selected_value, 'PartyModel')
                    //PartyModel
                    empr_helper.selectedBill = id;
                });
            });
         
            $('body').on('click', '#saveCopiedRecord', function () {
                debugger;
                var selectedParty = $('#PartyModel').dxSelectBox('option', 'value');
                ajaxHelper.ajaxPostJsonData({ grouP_CODE: empr_helper.selectedBill, party: selectedParty, acT_CODE: empr_CustomerPricing.act_code }, "/CustomerPricing/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_CustomerPricing.GetCommisionMapByCode(data.data.code);
                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_CustomerPricing.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_CustomerPricing.ResetForm();
                $('#PARTY_CODE').dxSelectBox('instance').option('value', null);

                
            });

            //$('body').on('click', '.btn-print,#BtnGenerateReport', function () {
            //    empr_CustomerPricing.GeneratePrintReport();
            //});

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },
    InitPartyDDL: function (selectedValue, targetId) {
        $('#' + targetId).dxSelectBox({
            dataSource: {
                store: PartyType,
                paginate: true,
                //pageSize: 50
            },
            paging: {
                enabled: true,
                pageSize: 50,
            },
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
                debugger;

                if (!e.value) return
                var selectedItem = PartyType.find(function (x) {
                    return x.key == e.value;
                });
                debugger;
                if (selectedItem) {
                    $("#ACT_CODE").val(selectedItem.accountCode);
                    empr_CustomerPricing.act_code = selectedItem.accountCode;
                   
                }
            }

        
        });
    },
  //InitPartyType: function () {
  //      debugger;
  //      empr_PurchaseBill.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
  //          console.log(d)
  //          if (d.value == null || d.value == '') {
  //              $('#partyhidden').val('');
  //              $('#acthidden').val('');
  //              empr_PurchaseBill.InitSalesman(0);
  //          }
  //          else {
  //              debugger;
  //              var filteredData = $.grep(PartyType, function (item) {
  //                  return item.key === d.value;
  //              });
  //              console.log("Party:" + filteredData);
  //              $('#partyhidden').val(filteredData[0].partyCode)
  //              $('#acthidden').val(filteredData[0].accountCode)
  //              $('#DISC').val(filteredData[0].disc)
  //              if (filteredData[0].partyCode != "" && filteredData[0].partyCode != 0) {
  //                  empr_PurchaseBill.InitSalesman(filteredData[0].partyCode, parseInt(filteredData[0].scode));
  //              }
  //          }

  //      });

  //  },
    ResetForm: function () {

        empr_CustomerPricing.CreateGrid([{ __KEY__: empr_CustomerPricing.GenerateKey(36), dC_TYPE: empr_CustomerPricing.DC_TYPE }]);
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
        if (dataSrc.length > 0) {
            empr_CustomerPricing.rowsCount = dataSrc.length - 1;

        }
        var commisionType = empr_helper.commType;
        var col = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_CustomerPricing.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_CustomerPricing.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_CustomerPricing.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        let costCenterAction = '';
                        let knockOffAction = '';
                        if ($('#Code').val() != '') {
                            if (options.data.dC_TYPE === "D" && options.data.partY_CODE == 0 && options.data.dT_CODE) {
                                costCenterAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 8px; color:#FFD700" onclick="empr_CustomerPricing.ShowCostCenterModal(${options.data.grouP_CODE},${options.data.dT_CODE},'${options.data.dT_DESC}',${options.data.amt},${options.data.partY_CODE})"><i class="fa fa-coins"></i></a>`;
                            }
                            if (options.data.partY_CODE > 0 && options.data.dT_CODE) {
                                knockOffAction = `<a href="javascript:;" class="grid-action-icon" style="margin-left: 8px;" onclick="empr_CustomerPricing.ShowKnockOffModal(${options.data.grouP_CODE},${options.data.dT_CODE},${options.data.amt},${options.data.partY_CODE},${options.data.acT_CODE})"><i class="fa fa-link"></i></a>`;
                            }
                        }
                        $(`<div class="btn-group btn-group-sm">
                      <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_CustomerPricing.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                      <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_CustomerPricing.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                      <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_CustomerPricing.DeleteRow(${options.rowIndex},${options.data.dT_CODE})" title="Delete"><i class="fa fa-trash"></i></a>
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
                allowSorting: false,
                lookup: {
                    dataSource: ItemMaster,
                    displayExpr: 'value',
                    valueExpr: 'key',
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.iteM_CODE = value;
                    var selectedItem = ItemMaster.filter(u => u.key == value);
                    if (selectedItem.length > 0) {
                        newData.rate = selectedItem[0].rate;
                    } else {
                        newData.rate = 0;
                    }
                },

            },
            {
                dataField: 'rate',
                alignment: 'right',
                caption: 'Rate',
                width: 100,
            },
            {
                dataField: 'disc',
                alignment: 'right',
                caption: 'Disc%',
                width: 100,
            },
            {
                dataField: 'adv',
                caption: 'Barcode',
                alignment: 'center',
                width: 400,
            },

        ];
        empr_helper.editableDxGridbindingForTransactionsVouchers('#DetailContainer', col, dataSrc, "CashReceiptVoucher", "custoM_ACT_CODE");
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }
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

            clonedRowData.__KEY__ = empr_CustomerPricing.GenerateKey(36);

            let newDataSource = [clonedRowData, ...dataSource];
            gridInstance.option("dataSource", newDataSource);

            gridInstance.refresh();

            // Update row count if needed
            empr_CustomerPricing.rowsCount += 1;
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
                empr_CustomerPricing.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_CustomerPricing.GenerateKey(36), grouP_CODE: empr_CustomerPricing.grouP_CODE });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_CustomerPricing.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_CustomerPricing.GenerateKey(36), dC_TYPE: empr_CustomerPricing.DC_TYPE });
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
    //        __KEY__: empr_CustomerPricing.GenerateKey(36),
    //        DT_CODE: empr_CustomerPricing.dT_CODE,  // backend can generate new DT_CODE if 0
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
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (dtCode == '' || dtCode == null || dtCode == undefined) {
                    gridInstance.deleteRow(index);
                    empr_CustomerPricing.rowsCount -= 1;
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
                            ajaxHelper.ajaxPostJsonData({ gcode: $('#Code').val(), code: dtCode }, "/CustomerPricing/DeleteCommisionMapDetailByCode", function (data) {
                                empr_helper.notify(data.msg, data.msgType);
                                if (data.msgType == 1) {
                                    gridInstance.deleteRow(index);
                                    empr_CustomerPricing.rowsCount -= 1;
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
                                    empr_CustomerPricing.CreateGrid([{ __KEY__: empr_CustomerPricing.GenerateKey(36), dC_TYPE: empr_CustomerPricing.DC_TYPE }]);
                                }
                            }, false, true);
                        });
                    } else {
                        empr_helper.notify("You are not allowed to delete the last row.", 2);
                    }
                } else {
                    empr_CustomerPricing.CreateGrid([{ __KEY__: empr_CustomerPricing.GenerateKey(36), dC_TYPE: empr_CustomerPricing.DC_TYPE }]);
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
            }
        });
    },

  

    InitQuickSearchGrid: function () {
        empr_CustomerPricing.GetCommisionMap();
    },

    GetCommisionMap: function () {
        ajaxHelper.ajaxGetJson('/CustomerPricing/GetCommisionMap', function (data) {
            if (data.msgType == 1) {
                empr_CustomerPricing.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {

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
                               <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" party=${options.data.partY_CODE} reportid=${options.data.grouP_CODE} title="COPY"><i class="fa fa-copy"></i></a>
                               </div>`).appendTo(container);
                }
            }
        },

            { dataField: 'grouP_CODE', caption: 'Code', },
            { dataField: 'dT_CODE', caption: 'DetailCode', visible: false },
            //{ dataField: 'sacT_CODE', caption: 'Sact Code ', visible: false },

            { dataField: 'partY_NAME', caption: 'Party Name' },

            //{ dataField: 'grouP_CODE', caption: 'Code',  },
            //{ dataField: 'salesmaN_NAME', caption: 'Salesman', },
            //{ dataField: 'salesman', caption: 'Salesman', visible: false },
   

        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "CashReceiptVoucherQS", "single");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
        //empr_helper.dxGridbindingLazyLoading('#gridContainer', columns, "/CashReceiptVoucher/GetCommisionMap", "dT_CODE", "CashReceiptVoucher", "multiple");
    },
    ValidateAndPrepareDataForSave: function () {
        debugger;
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
                    obj.ACT_CODE =  $("#ACT_CODE").val();
                    obj.RATE = obj.rate || $("#rate").val();
                    //obj.COMM_VALUE = obj.comM_VALUE || $("#COMM_VAL").val();
                    obj.PARTY = obj.PARTY || $('#PARTY_CODE').dxSelectBox('option', 'value');
                    obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
            


                    if (!obj.PARTY) {
                        empr_helper.notify("Party is required for all records.", 2);
                        return;
                    }
                    if (!obj.ITEM_CODE) {
                        empr_helper.notify("Item is required for all records.", 2);
                        return;
                    }

                    if (!obj.RATE || obj.RATE == 0) {
                        empr_helper.notify("RATE  must be greater than 0.", 2);
                        return;
                    }
                  
                }

                // After validation, save
                empr_CustomerPricing.SaveInfo(detailRecords);
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
                obj.ACT_CODE = $("#ACT_CODE").val();
                obj.RATE = obj.rate || $("#RATE").val();
                //obj.COMM_VALUE = obj.comM_VALUE || $("#COMM_VAL").val();
                obj.PARTY = $('#PARTY_CODE').dxSelectBox('option', 'value');
                obj.ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
                obj.GROUP_CODE = $("#Code").val();
                obj.REMARKS = $("#REMARKS").val();
                obj.DOC = $("#hdnDOC").val();

                if (!obj.PARTY) {
                    empr_helper.notify("Party is required for all records.", 2);
                    return;
                }
                if (!obj.ITEM_CODE) {
                    empr_helper.notify("Item is required for all records.", 2);
                    return;
                }
              
                if (!obj.RATE || obj.RATE == 0 ) {
                    empr_helper.notify("RATE  must be greater than 0.", 2);
                    return;
                }
            }

            empr_CustomerPricing.SaveInfo(detailRecords);
        }
    },

    SaveInfo: function (detailRecords) {
        debugger;
        console.log('Safe Attempt',detailRecords)
        ajaxHelper.ajaxPostJsonData({ modelRecord: detailRecords }, "/CustomerPricing/Save", function (data) {
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
                    //empr_CustomerPricing.ResetForm();
                //    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                if (dataClear == 1) {
                    empr_CustomerPricing.GetCommisionMapByCode($('#Code').val());
                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                    } else {
                        $('#BtnDelete').show();
                    }
                }
                else {
                    empr_CustomerPricing.ResetForm();
                    $('#PARTY_CODE').dxSelectBox('instance').option('value', null);
                        

                }


            }
        }, false, true);
    },

    GetCommisionMapByCode: function (code) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/CustomerPricing/GetCommisionMapByCode?code=' + code, function (data) {
            //debugger;
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.grouP_CODE);
                    $('#ACT_CODE').val(response.acT_CODE);
               
                   $('#PARTY_CODE').dxSelectBox('instance').option('value', response.party);
                   
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
                    empr_CustomerPricing.CreateGrid(data.detail.data);
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
        debugger;
        ajaxHelper.ajaxGetJson('/CustomerPricing/GetCommisionMapDetailsByCode?code=' + code, function (data) {
            //console.log(data)
            if (data.msgType == 1) {
                empr_CustomerPricing.CreateGrid(data.data);
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/CustomerPricing/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_CustomerPricing.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

 
}