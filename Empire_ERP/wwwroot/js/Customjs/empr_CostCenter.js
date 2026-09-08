var empr_CostCenter = {
    pickTranId: 0,
    pickDtCode: 0,
    pickAmount: 0,
    edit_Id: '',
    gridData: [],
    initEvents: function () {
        $(document).ready(function () {
            console.log('PickData', PickData);
            empr_CostCenter.pickTranId = PickData.ptraN_ID;
            empr_CostCenter.pickDtCode = PickData.picK_ID;
            empr_CostCenter.pickAmount = PickData.amount;
            empr_CostCenter.InitQuickSearch(empr_CostCenter.pickTranId, empr_CostCenter.pickDtCode);
            var gridTotalAmt = empr_CostCenter.GetGridSum(empr_CostCenter.gridData, null);
            empr_CostCenter.InitCostCenterDDL();
            empr_CostCenter.InitStatusDDL();

            if (gridTotalAmt == 0) {
                empr_CostCenter.SetPickData();
            }
            else {
                $("#PTRAN_ID").val(PickData.ptraN_ID);
                $("#PICK_ID").val(PickData.picK_ID);
            }

            // make modal moveable
            $(function () {
                $("#costCenterModal .modal-content").draggable({
                    handle: ".modal-header",
                    containment: "window"
                });
            });

            $('#costCenterBtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_CostCenter.validateForm()) {
                            empr_CostCenter.saveAttempt();
                        }
                    }
                } else {
                    if (empr_CostCenter.validateForm()) {
                        empr_CostCenter.saveAttempt();
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_CostCenter.InitQuickSearch();
            });

            $('body').on('click', '.cc_elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_CostCenter.edit_Id = $(this).attr("reportid");
                empr_CostCenter.GetCostCenterByID(reportid);
            });

            $('body').on('click', '#costCenterBtnNew', function () {
                $('#costCenterBtnDelete').hide();
                $('#costCenterBtnNew').hide();
                empr_CostCenter.resetForm();
            });

            $('#costCenterBtnDelete').click(function () {
                empr_CostCenter.DeleteRecord();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#ccgridContainer').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#costCenterBtnSave').hide();
            }

            // modal close event
            //$('#costCenterModal').on('hide.bs.modal', function (e) {
            //    console.log('close hit');
            //    //var canClose = empr_CostCenter.canCloseModal; // true ya false

            //    if (empr_CostCenter.gridTotalAmt < empr_CostCenter.pickAmount) {
            //        e.preventDefault(); // stop modal to close 
            //        var leftAmt = empr_CostCenter.pickAmount - empr_CostCenter.gridTotalAmt;
            //        empr_helper.notify('First add all your cost center amount, Left Amount is ' + leftAmt, 2);
            //    }
            //});
        });
    },

    resetForm: function () {
        $("#CC_Code").val('');
        $("#DESCR").val('');
        $("#AMOUNT").val(''); 
        empr_CostCenter.InitCostCenterDDL();
        //$('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#costCenterBtnSave').show();
            } else {
                $('#costCenterBtnSave').hide();
            }
        } else {
            $('#costCenterBtnSave').show();
        }
    },

    validateForm: function () {

        var valid = true;

        var COST_CENTER_ID = $("#costcenter_hidden").val();
        var AMOUNT = $("#AMOUNT").val();

        if (COST_CENTER_ID == "" || COST_CENTER_ID == "0" || COST_CENTER_ID == null || COST_CENTER_ID == undefined) {
            empr_helper.notify("Please select Cost Center.", 2);
            valid = false;
            return valid;
        }

        if (AMOUNT == "" || AMOUNT == "0" || AMOUNT == null || AMOUNT == undefined) {
            empr_helper.notify("Please enter Amount.", 2);
            valid = false;
            return valid;
        }
        //var GROUP_NAME = $("#GROUP_NAME").val().trim();

        //if (GROUP_NAME == '') {
        //    valid = false;
        //    empr_helper.notify("Please enter name.", 2);
        //}


        return valid;
    },

    GetDataToSave: function () {
        var ID = $("#CC_Code").val();
        var ASTATUS = $("#CCASTATUS").dxSelectBox('instance').option('value');
        //var COST_CENTER_ID = $('#COST_CENTER_ID').dxSelectBox('instance').option('value');
        var COST_CENTER_ID = $("#costcenter_hidden").val();
        var DESCR = $("#DESCR").val();
        var AMOUNT = $("#AMOUNT").val();
        var PTRAN_ID = $("#PTRAN_ID").val();
        var PICK_ID = $("#PICK_ID").val();
        var modelRecord = {
            GROUP_CODE: ID,
            ASTATUS: ASTATUS,
            COST_CENTER_ID: COST_CENTER_ID,
            DESCR: DESCR,
            AMOUNT: AMOUNT,
            PTRAN_ID: PTRAN_ID,
            PICK_ID: PICK_ID
        }
        //console.log('safeRecord',modelRecord);
        return modelRecord;
    },

    saveAttempt: function () {
        debugger;
        var obj = empr_CostCenter.GetDataToSave();
        var gridSum = empr_CostCenter.GetGridSum(empr_CostCenter.gridData, empr_CostCenter.edit_Id);
        var totalAddingAmt = Number(obj.AMOUNT) + Number(gridSum);

        if (totalAddingAmt > empr_CostCenter.pickAmount) {
            empr_helper.notify('Cost center amount cannot be greater than the actual amount.\nActual amount is: ' + empr_CostCenter.pickAmount, 2);
        }
        else {
            console.log('cc SaveAttempt',obj);
            ajaxHelper.ajaxPostJsonData(obj, "/CostCenter/save", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_CostCenter.resetForm();
                    empr_CostCenter.InitQuickSearch(empr_CostCenter.pickTranId, empr_CostCenter.pickDtCode);
                    $('#optmodal').modal('hide');
                    $('#costCenterBtnDelete').hide();
                    $('#costCenterBtnNew').hide();
                    empr_CostCenter.edit_Id = '';
                }
            }, false, true);
        }
    },

    InitQuickSearch: function (tranId,dtCode) {
        empr_CostCenter.GetAllCostCenters(tranId, dtCode);
    },

    GetAllCostCenters: function (tranId, dtCode) {
        var obj = { PTRAN_ID: tranId, PICK_ID: dtCode };

        ajaxHelper.ajaxPostJsonData(obj, "/CostCenter/QuickSearch", function (data) {
            empr_CostCenter.gridData = data.data;

            //var totalAmount = data.data.reduce(function (sum, item) {
            //    return sum + (item.amount || 0);
            //}, 0);

            //empr_CostCenter.gridTotalAmt = totalAmount;

            //empr_CostCenter.pickAmount = empr_CostCenter.gridTotalAmt;

            empr_CostCenter.CreateGrid(data.data);
        }, false, true);
    },

    GetCostCenterByID: function (id) {
        ajaxHelper.ajaxGetJson('/CostCenter/GetCostCenterByID?id=' + id, function (data) {
            console.log('GetCostCenterByID', data.data);
            if (data.msgType == 1) {
                
                var record = data.data;

                $("#CC_Code").val(record.grouP_CODE);
                empr_CostCenter.InitCostCenterDDL(record.cosT_CENTER_ID);
                $("#costcenter_hidden").val(record.cosT_CENTER_ID);
                $('#CCASTATUS').dxSelectBox('instance').option('value', record.astatus);
                $("#AMOUNT").val(record.amount);
                $("#DESCR").val(record.descr);

                //$('.modal').modal('hide');
                //$('#costCenterBtnDelete').show();
                if (Permissions != "Admin") {
                    if (Permissions.r_DLT) {
                        $('#costCenterBtnDelete').show();
                    }
                    if (Permissions.r_ADD) {
                        $('#costCenterBtnNew').show();
                    }
                    if (Permissions.r_EDIT) {
                        $('#costCenterBtnSave').show();
                    }
                    else {
                        $('#costCenterBtnSave').hide();
                    }
                } else {
                    $('#costCenterBtnSave').show();
                    $('#costCenterBtnDelete').show();
                    $('#costCenterBtnNew').show();
                }
            } else {
                empr_helper.notify("3" +data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGrid: function (dataSrc) {
        //console.log('CreateGrid', dataSrc);
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.gpic != null && options.data.gpic != '' && options.data.gpic != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.gpic}')"><i class="fa fa-eye"></i></a>`;
                }
                html += `<a href="javascript:;" class="grid-action-icon cc_elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';                
                $(html).appendTo(container);
            }
        },
        { dataField: 'descr', caption: 'Description' },
        { dataField: 'amount', caption: 'Amount' },
        { dataField: 'cC_NAME', caption: 'Cost Center' },
        { dataField: 'astatus', caption: 'Active' },
        ];
        empr_helper.dxGridbindingVouchers('#ccgridContainer', col, dataSrc, "SetupSubType");
    },

    InitCostCenterDDL: function (_selectedValue) {
        var selectedvalue = 0;
        var selectedobj = [];
        if (_selectedValue != null) {
            console.log('CostCenter', CostCenter);
            selectedobj = CostCenter.filter(x => x.key == _selectedValue);
            if (selectedobj.length > 0) {
                selectedvalue = _selectedValue;
                $("#department_hidden").val(selectedvalue);
                $("#displayExpr_department").val(selectedobj[0].value);
            }
        }

        empr_CostCenter.BindDxGridBoxDdl('#COST_CENTER_ID', CostCenter, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'key', 'value', '#displayExpr_costcenter', function (selectedvalue, hidden) {

            if (selectedvalue.selectedRowsData.length > 0) {
                var key = selectedvalue.selectedRowsData[0]['key'];
                var value = selectedvalue.selectedRowsData[0]['value'];
                var rate = selectedvalue.selectedRowsData[0]['rate'];
                $('#costcenter_hidden').val(key);
                $('#displayExpr_costcenter').val(value);
            }
            else {
                $('#costcenter_hidden').val('');
                $('#displayExpr_costcenter').val('');
            }
        });
    },

    BindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    InitStatusDDL: function (_selectedValue) {
        $('#CCASTATUS').dxSelectBox({
            dataSource: [
                { value: 'Y', text: 'Active' },
                { value: 'N', text: 'In-Active' }
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
                e.component.option('value', 'Y');
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
            ajaxHelper.ajaxPostJsonData({ id: $('#CC_Code').val() }, "/CostCenter/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_CostCenter.resetForm();
                    empr_CostCenter.InitQuickSearch(empr_CostCenter.pickTranId, empr_CostCenter.pickDtCode);
                    $('#optmodal').modal('hide');
                    $('#costCenterBtnDelete').hide();
                    $('#costCenterBtnNew').hide();
                }
            }, false, true);
        });
    },

    SetPickData: function () {
        $("#DESCR").val(PickData.descr);
        $("#AMOUNT").val(PickData.amount);
        $("#PTRAN_ID").val(PickData.ptraN_ID);
        $("#PICK_ID").val(PickData.picK_ID);
    },


    GetGridSum: function (dataSrc, gridId) {
        return dataSrc.reduce(function (sum, row) {
            var amount = parseFloat(row.amount) || 0;
            var rowId = row.grouP_CODE;

            // Agar gridId diya gaya hai aur yehi row ignore karni hai
            if (gridId != null && String(rowId) === String(gridId)) {
                return sum; // skip this row
            }

            return sum + amount; // otherwise, add amount
        }, 0);
    },


 
}