var empr_POSDiscount = {
    isValueAssigned: false,
    InitEvents: function () {
        $(document).ready(function () {
            empr_POSDiscount.ResetForm();

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_POSDiscount.InitQuickSearchGrid();
            });

            $('body').on('click', '#BtnSave', function () {
                if (empr_POSDiscount.ValidateInfo()) {
                    empr_POSDiscount.SaveInfo();
                }
            });

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid")
                empr_POSDiscount.GetPOSDiscountByCode(id);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_POSDiscount.Delete($('#Code').val());
            });

            $('body').on('click', '#BtnNew', function () {
                empr_POSDiscount.ResetForm();
            });
        });
    },
    InitQuickSearchGrid: function () {
        empr_POSDiscount.QuickSearch();
    },
    QuickSearch: function () {
        ajaxHelper.ajaxGetJson('/POSDiscount/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_POSDiscount.CreateQuickSearchGrid(data.data);
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
            { dataField: 'b_NAME', caption: 'Branch Name', },
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
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "POSDiscount");
    },
    GetData: function () {

        var CODE = $("#Code").val();
        var description = $("#Description").val().trim();
        var status = $("#ASTATUS").dxSelectBox('instance').option('value');
        var SelectedBranches = $("#branches_hidden").val();
        //var SelectedItems = $("#design_hidden").val();
        //var SelectedItemGroups = $("#group_hidden").val();
        var FromDate = $("#FromDate").val();
        var ToDate = $("#ToDate").val();
        var DiscountPercent = $("#DiscountPercent").val();
        var DiscountExpired = $("#DiscountExpired").prop('checked') ? "1" : "0";

        var modelRecord = {
            CODE: CODE,
            DESCR: description,
            ASTATUS: status,
            SELECTEDBRANCHES: SelectedBranches,
            //SELECTEDITEMS: SelectedItems,
            //SELECTEDITEMGROUPS: SelectedItemGroups,
            FDATE: FromDate,
            TDATE: ToDate,
            DISC: DiscountPercent,
            DISC_EXP: DiscountExpired
        }

        return modelRecord;
    },
    ValidateInfo: function () {

        var valid = true;
        var data = empr_POSDiscount.GetData();

        if (data.DESCR == '' || data.DESCR == null || data.DESCR == undefined) {
            empr_helper.notify("Please enter the description.", 2);
            valid = false;
            return valid;
        }

        if (data.SELECTEDBRANCHES == '' || data.SELECTEDBRANCHES == null || data.SELECTEDBRANCHES == undefined) {
            empr_helper.notify("Please select at least one branch.", 2);
            valid = false;
            return valid;
        }

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
        var data = empr_POSDiscount.GetData();
        ajaxHelper.ajaxPostJsonData(data, "/POSDiscount/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_POSDiscount.ResetForm();
                empr_POSDiscount.InitQuickSearchGrid();
            }
        }, false, true);
    },
    ResetForm: function () {
        debugger;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #FromDate').val('');
        $('#DiscountExpired').prop('checked', false);
        empr_POSDiscount.InitQuickSearchGrid();
        empr_POSDiscount.InitBranchesDDL();
        //empr_POSDiscount.InitDesignNoDDL();
        //empr_POSDiscount.InitItemGroupDDL();
        $('#BtnDelete, #BtnNew').hide();
    },
    GetPOSDiscountByCode: function (id) {

        ajaxHelper.ajaxGetJson('/POSDiscount/GetPOSDiscountByCode?code=' + id, function (data) {
            debugger;
            empr_POSDiscount.ResetForm();
            if (data.msgType == 1) {
                var response = data.data[0];

                $("#Code").val(response.code);
                $("#Description").val(response.descr);
                $("#ASTATUS").dxSelectBox('instance').option('value', response.astatus);
                $("#branches_hidden").val(response.bcode);
                //$("#design_hidden").val(response.iteM_CODE);
                //$("#group_hidden").val(response.iteM_GROUP);
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

                setTimeout(function () {
                    var branchRow = $('#Branches_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.bcode);
                    if (branchRow[0] != null) {
                        var selectedRow = branchRow[0];
                        $('#Branches_grid').dxDataGrid('instance').selectRows(selectedRow);
                    }

                    //var designRow = $('#DesignNo_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.iteM_CODE);
                    //if (designRow[0] != null) {
                    //    var selectedRow = designRow[0];
                    //    $('#DesignNo_grid').dxDataGrid('instance').selectRows(selectedRow);
                    //}

                    //var groupRow = $('#ItemGroup_grid').dxDataGrid('instance').option('dataSource').filter(x => x.key == response.iteM_GROUP);
                    //if (groupRow[0] != null) {
                    //    var selectedRow = groupRow[0];
                    //    $('#ItemGroup_grid').dxDataGrid('instance').selectRows(selectedRow);
                    //}

                }, 1000);

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
            ajaxHelper.ajaxPostJsonData({ code: _id }, "/POSDiscount/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    window.location.reload();
                }
            }, false, true);
        });
    },
    InitBranchesDDL: function () {
        $.ajax({
            url: 'POSDiscount/GetBranches',
            method: 'GET',
            success: function (data) {
                empr_POSDiscount.MultipleDxGridBoxDropdown('#Branches', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_branches", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#branches_hidden').val(keys);
                        $('#displayExpr_branches').val(values);
                        empr_POSDiscount.isValueAssigned = false;
                    }
                    else {
                        $('#branches_hidden').val('');
                        $('#displayExpr_branches').val('');
                    }
                }, 'multiple');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitDesignNoDDL: function (_selectedValue) {
        $.ajax({
            url: 'POSDiscount/GetDesignNos',
            method: 'GET',
            success: function (data) {
                empr_POSDiscount.MultipleDxGridBoxDropdown('#DesignNo', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_design", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#design_hidden').val(keys);
                        $('#displayExpr_design').val(values);
                        empr_POSDiscount.isValueAssigned = false;
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
            url: 'POSDiscount/GetItemGroups',
            method: 'GET',
            success: function (data) {
                empr_POSDiscount.MultipleDxGridBoxDropdown('#ItemGroup', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', null, null, 'key', 'value', "#displayExpr_group", function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var array = selectedvalue.selectedRowsData;
                        var keys = array.map(item => item.key).join(',');
                        var values = array.map(item => item.value).join(',');
                        $('#group_hidden').val(keys);
                        $('#displayExpr_group').val(values);
                        empr_POSDiscount.isValueAssigned = false;
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