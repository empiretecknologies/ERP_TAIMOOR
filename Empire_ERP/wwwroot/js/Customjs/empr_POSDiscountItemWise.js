var empr_POSDiscountItemWise = {
    isValueAssigned: false,
    InitEvents: function () {
        $(document).ready(function () {
            empr_POSDiscountItemWise.ResetForm();

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_POSDiscountItemWise.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSave', function () {
                if (empr_POSDiscountItemWise.ValidateInfo()) {
                    empr_POSDiscountItemWise.SaveInfo();
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid")
                empr_POSDiscountItemWise.GetPOSDiscountItemWiseByCode(id);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_POSDiscountItemWise.Delete($('#Code').val());
            });

            $('body').on('click', '#BtnNew', function () {
                empr_POSDiscountItemWise.ResetForm();
            });
        });
    },
    InitQuickSearchGrid: function () {
        empr_POSDiscountItemWise.QuickSearch();
    },

    QuickSearch: function () {
        ajaxHelper.ajaxGetJson('/POSDiscountItemWise/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_POSDiscountItemWise.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    detailGrid: function () {
        ajaxHelper.ajaxGetJson('/POSDiscountItemWise/detailGrid', function (data) {
            if (data.msgType == 1) {
                empr_POSDiscountItemWise.CreatedetailGrid(data.data);

                setTimeout(function () {
                    let grid = $('#detailgridContainer').dxDataGrid('instance');
                    grid.refresh(); // Grid ko refresh karega

                    grid.clearSelection(); 
                }, 500);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger
                if (Permissions != "Admin") {
                    const editAction = !Permissions.r_EDIT
                        ? ''
                        : `<a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>`;
                    const actions = `<div class="btn-group btn-group-sm">${editAction}</div>`;
                    $(actions).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                            <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                            </div>`).appendTo(container);
                }

            }
        },
        { dataField: 'id', caption: 'Code', },
        { dataField: 'astatus', caption: 'Status', },
        { dataField: 'descr', caption: 'Description', },
        /*{ dataField: 'b_NAME', caption: 'Branch Name', },*/
        { dataField: 'iteM_NAME', caption: 'Item Name', },
        { dataField: 'grouP_NAME', caption: 'Group Name', },
        { dataField: 'fdate', caption: 'From Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'tdate', caption: 'To Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'disc', caption: 'Discount%', },
        { dataField: 'disC_EXP', caption: 'DiscountExp.', },
        { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
        { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
        { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
        { dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
        { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
        { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
        { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        { dataField: 'ediT_POSTALCODE', caption: 'Edited Postal Code', visible: false, },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "POSDiscountItemWise" , "single");
    },
    CreatedetailGrid: function (dataSrc) {
        console.log(dataSrc);
        var col = [
            { dataField: 'id', caption: 'Code', },
            { dataField: 'dt_code', caption: 'DT_Code', visible: false },
            { dataField: 'itemName', caption: 'Item Name', },
            { dataField: 'remark', caption: 'Remark', },
            { dataField: 'groupName', caption: 'Group Name', },
            { dataField: 'catName', caption: 'Category Name', },
            { dataField: 'subCatName', caption: 'Sub Category Name', },
        ];
        empr_helper.dxGridbindingVouchers('#detailgridContainer', col, dataSrc, "POSDiscountItemWiseDetail");

    },
    CreateBranchesGrid: function (dataSrc) {
        var col = [
            { dataField: 'key', caption: 'Code', width:100 },
            { dataField: 'value', caption: 'Branch' },
        ];
        empr_helper.dxGridbindingVouchers('#BranchesGridContainer', col, dataSrc, "POSDiscountItemWiseKeyValue");

    },
    GetData: function () {
        var CODE = $("#Code").val();
        var description = $("#Description").val();
        var status = $("#ASTATUS").dxSelectBox('instance').option('value');
        /*var SelectedBranches = $("#branches_hidden").val();*/
        //var SelectedItems = $("#design_hidden").val();
        //var SelectedItemGroups = $("#group_hidden").val();
        var FromDate = $("#FromDate").val();
        var ToDate = $("#ToDate").val();
        var DiscountPercent = $("#DiscountPercent").val();
        var DiscountExpired = $("#DiscountExpired").prop('checked') ? "1" : "0";

        var masterRecord = {
            CODE: CODE,
            DESCR: description,
            ASTATUS: status,
            /*SELECTEDBRANCHES: SelectedBranches,*/
            //SELECTEDITEMS: SelectedItems,
            //SELECTEDITEMGROUPS: SelectedItemGroups,
            FDATE: FromDate,
            TDATE: ToDate,
            DISC: DiscountPercent,
            DISC_EXP: DiscountExpired
        }

        var detailRecords = [];
        if ($('#detailgridContainer').dxDataGrid('instance').hasEditData()) {
            detailRecords = $('#detailgridContainer').dxDataGrid('instance').getSelectedRowsData();
        }
        else {
            detailRecords = $('#detailgridContainer').dxDataGrid('instance').getSelectedRowsData();
        }
        var branchRecords = [];
        if ($('#BranchesGridContainer').dxDataGrid('instance').hasEditData()) {
            branchRecords = $('#BranchesGridContainer').dxDataGrid('instance').getSelectedRowsData();
        }
        else {
            branchRecords = $('#BranchesGridContainer').dxDataGrid('instance').getSelectedRowsData();
        }

        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords,
            Branch: branchRecords
        };
        return modelRecord;
        console.log(modelRecord);
    },
    ValidateInfo: function () {

        var valid = true;
        var Modeldata = empr_POSDiscountItemWise.GetData();
        var data = Modeldata.Master;

        if (data.DESCR == '' || data.DESCR == null || data.DESCR == undefined) {
            empr_helper.notify("Please enter the description.", 2);
            valid = false;
            return valid;
        }

        //if (data.SELECTEDBRANCHES == '' || data.SELECTEDBRANCHES == null || data.SELECTEDBRANCHES == undefined) {
        //    empr_helper.notify("Please select at least one branch.", 2);
        //    valid = false;
        //    return valid;
        //}

        //if ((data.SELECTEDITEMS == '' || data.SELECTEDITEMS == null || data.SELECTEDITEMS == undefined) && (data.SELECTEDITEMGROUPS == '' || data.SELECTEDITEMGROUPS == null || data.SELECTEDITEMGROUPS == undefined)) {
        //    empr_helper.notify("Please select at least one item or one item group.", 2);
        //    valid = false;
        //    return valid;
        //}

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
        var data = empr_POSDiscountItemWise.GetData();
        ajaxHelper.ajaxPostJsonData(data, "/POSDiscountItemWise/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_POSDiscountItemWise.ResetForm();
            }
        }, false, true);
    },
    ResetForm: function () {
        debugger;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #FromDate').val('');
        $('#DiscountExpired').prop('checked', false);
        //empr_POSDiscountItemWise.InitQuickSearchGrid();
        empr_POSDiscountItemWise.detailGrid();
        empr_POSDiscountItemWise.InitBranchesDDL();
        //empr_POSDiscountItemWise.InitDesignNoDDL();
        //empr_POSDiscountItemWise.InitItemGroupDDL();
        $('#BtnDelete, #BtnNew').hide();
        //let grid = $('#detailgridContainer').dxDataGrid('instance');
        //grid.refresh();
        //grid.clearSelection(); 
    },
    GetPOSDiscountItemWiseByCode: function (id) {

        ajaxHelper.ajaxGetJson('/POSDiscountItemWise/GetPOSDiscountItemWiseByCode?code=' + id, function (data) {
            debugger;
            empr_POSDiscountItemWise.ResetForm();
            if (data.master.msgType == 1) {
                console.log(data);
                var response = data.master.data[0];
                
                $("#Code").val(response.code);
                $("#Description").val(response.descr);
                $("#ASTATUS").dxSelectBox('instance').option('value', response.astatus);
                /*$("#branches_hidden").val(response.bcode);*/
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
                //setTimeout(function () {
                //    var branchRow = $('#Branches_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.bcode);
                //    if (branchRow[0] != null) {
                //        var selectedRow = branchRow[0];
                //        $('#Branches_grid').dxDataGrid('instance').selectRows(selectedRow);
                //    }

                //}, 1000);

                var detail = data.detail.data;
                setTimeout(function () {
                    var gridInstance = $('#detailgridContainer').dxDataGrid('instance');

                    var dataSource = gridInstance.option('dataSource');

                    var itemCodes = detail.map(d => d.itemcode);

                    var selectedRows = dataSource.filter(row => itemCodes.includes(row.id));

                    if (selectedRows.length > 0) {
                        gridInstance.selectRows(selectedRows);

                        //selectedRows.forEach(row => {
                        //    var matchingDetail = detail.find(d => d.itemcode === row.id); // Match karne ke liye
                        //    if (matchingDetail) {
                        //        row.dt_code = matchingDetail.dtcode; // dt_code update
                        //    }
                        //});

                        // Grid ko update karo
                        gridInstance.option('dataSource', dataSource);
                        gridInstance.refresh();
                    }
                    var barnchgridInstance = $('#BranchesGridContainer').dxDataGrid('instance');

                    dataSource = barnchgridInstance.option('dataSource');

                    var barnchCode = detail.map(d => d.bcode);

                    selectedRows = dataSource.filter(row => barnchCode.includes(row.key));

                    if (selectedRows.length > 0) {
                        barnchgridInstance.selectRows(selectedRows);

                        //selectedRows.forEach(row => {
                        //    matchingDetail = detail.find(d => d.bcode === row.key); // Match karne ke liye
                        //    if (matchingDetail) {
                        //        row.key = matchingDetail.bcode; // dt_code update
                        //    }
                        //});

                        // Grid ko update karo
                        barnchgridInstance.option('dataSource', dataSource);
                        barnchgridInstance.refresh();
                    }
                }, 2000);

                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#BtnDelete').show();
                    }
                    if (Permissions.r_ADD) {
                        $('#BtnNew').show();
                    }
                    else {
                        $('#BtnSave').hide();
                    }
                } else {
                    $('#BtnSave').show();
                    $('#BtnDelete').show();
                    $('#BtnNew').show();
                }

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
            ajaxHelper.ajaxPostJsonData({ code: _id }, "/POSDiscountItemWise/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    window.location.reload();
                }
            }, false, true);
        });
    },
    InitBranchesDDL: function () {
        debugger;
        $.ajax({
            url: 'POSDiscountItemWise/GetBranches',
            method: 'GET',
            success: function (data) {
                console.log(data);
                empr_POSDiscountItemWise.CreateBranchesGrid(data);

                //empr_POSDiscountItemWise.MultipleDxGridBoxDropdown('#Branches', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_branches", function (selectedvalue, hidden) {
                //    if (selectedvalue.selectedRowsData.length > 0) {
                //        debugger;
                //        var array = selectedvalue.selectedRowsData;
                //        var keys = array.map(item => item.key).join(',');
                //        var values = array.map(item => item.value).join(',');
                //        $('#branches_hidden').val(keys);
                //        $('#displayExpr_branches').val(values);
                //        empr_POSDiscountItemWise.isValueAssigned = false;
                //    }
                //    else {
                //        $('#branches_hidden').val('');
                //        $('#displayExpr_branches').val('');
                //    }
                //}, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitDesignNoDDL: function (_selectedValue) {
        $.ajax({
            url: 'POSDiscountItemWise/GetDesignNos',
            method: 'GET',
            success: function (data) {
                empr_POSDiscountItemWise.MultipleDxGridBoxDropdown('#DesignNo', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_design", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#design_hidden').val(keys);
                        $('#displayExpr_design').val(values);
                        empr_POSDiscountItemWise.isValueAssigned = false;
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
            url: 'POSDiscountItemWise/GetItemGroups',
            method: 'GET',
            success: function (data) {
                empr_POSDiscountItemWise.MultipleDxGridBoxDropdown('#ItemGroup', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_group", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#group_hidden').val(keys);
                        $('#displayExpr_group').val(values);
                        empr_POSDiscountItemWise.isValueAssigned = false;
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