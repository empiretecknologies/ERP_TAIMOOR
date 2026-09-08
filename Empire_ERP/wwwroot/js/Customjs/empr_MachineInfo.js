var empr_MachineInfo = {
    $uploadCrop: null,
    rawImg: null,
    initEvents: function () {
        $(document).ready(function () {
            empr_MachineInfo.InintQuickSearch();
            empr_MachineInfo.InitDropdowns();
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                if (data.traN_ID != 0) {
                    $('#Code').val(data.traN_ID);
                    empr_MachineInfo.GetMachineInfoByID(data.traN_ID);
                }
            });
            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_MachineInfo.validateForm()) {
                            empr_MachineInfo.saveAttempt();
                            empr_MachineInfo.resetForm();
                        }
                    }
                } else {
                    if (empr_MachineInfo.validateForm()) {
                        empr_MachineInfo.saveAttempt();
                        empr_MachineInfo.resetForm();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_MachineInfo.InintQuickSearch();
            })

            $('body').on('click', '.elm_edit', function () {
                empr_MachineInfo.resetForm();
                var rportid = $(this).attr("rportid")
                empr_MachineInfo.GetMachineInfoByID(rportid);

            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_MachineInfo.resetForm();

            })

            $('.btn-delete').click(function () {
                empr_MachineInfo.DeleteRecord();
            })

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_MachineInfo.GeneratePrintReport();
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#quicksearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/MachineInfo/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    $('.btn-delete').hide();
                    $('.btn-print').hide();
                    $('#resetall').hide();
                    empr_MachineInfo.InintQuickSearch();
                    empr_MachineInfo.resetForm();
                }
            }, false, true);
        });
    },

    resetForm: function () {
        $("#Code").val('');
        $("#Machine_Code").val('');
        $("#Machine_Name").val('');
        $("#IP_ADDRESS").val('');
        $("#IP_PORT").val('');
        $("#SERVER_NAME").val('');
        $('#BRANCH').dxSelectBox('option', '');
        $("#S_USER_ID").val('');
        $("#S_PASSWORD").val('');
        $("#DB_NAME").val('');
        $("#QUERY").val('');
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        } else {
            $('#saveAttempt').show();
        }
    },

    validateForm: function () {
        var valid = true;

        var ModalData = empr_MachineInfo.getDataToSave();

        if (ModalData.Machine_Code == '' || ModalData.Machine_Code == null) {
            valid = false;
            empr_helper.notify("Please Enter Machine Code.", 2);
        }
        if (ModalData.Machine_Name == '' || ModalData.Machine_Name == null) {
            valid = false;
            empr_helper.notify("Please Enter Machine Name.", 2);
        }
        if (ModalData.BRANCH == '' || ModalData.BRANCH == null) {
            valid = false;
            empr_helper.notify("Please Select Branch.", 2);
        }
        if (ModalData.IP_ADDRESS == '' || ModalData.IP_ADDRESS == null) {
            valid = false;
            empr_helper.notify("Please Enter IP Address", 2);
        }
        if (ModalData.IP_PORT == '' || ModalData.IP_PORT == null) {
            valid = false;
            empr_helper.notify("Please Enter IP PORT", 2);
        }
        if (ModalData.SERVER_NAME == '' || ModalData.SERVER_NAME == null) {
            valid = false;
            empr_helper.notify("Please Enter SERVER NAME", 2);
        }
        if (ModalData.S_USER_ID == '' || ModalData.S_USER_ID == null) {
            valid = false;
            empr_helper.notify("Please Enter SERVER USER ID", 2);
        }
        if (ModalData.S_PASSWORD == '' || ModalData.S_PASSWORD == null) {
            valid = false;
            empr_helper.notify("Please Enter SERVER USER PASSWORD", 2);
        }
        if (ModalData.DB_NAME == '' || ModalData.DB_NAME == null) {
            valid = false;
            empr_helper.notify("Please Enter Database Name", 2);
        }
        if (ModalData.QUERY == '' || ModalData.QUERY == null) {
            valid = false;
            empr_helper.notify("Please Enter Query", 2);
        }
        return valid;
    },

    getDataToSave: function () {
        var TRAN_ID = $("#Code").val().trim();
        var Machine_Code = $("#Machine_Code").val();
        var Machine_Name = $("#Machine_Name").val();
        var IP_ADDRESS = $("#IP_ADDRESS").val();
        var IP_PORT = $("#IP_PORT").val();
        var SERVER_NAME = $("#SERVER_NAME").val();
        var BRANCH = $('#BRANCH').dxSelectBox('option', 'value');
        var S_USER_ID = $("#S_USER_ID").val();
        var S_PASSWORD = $("#S_PASSWORD").val();
        var DB_NAME = $("#DB_NAME").val();
        var QUERY = $("#QUERY").val();

        var modelRecord = {
            TRAN_ID: TRAN_ID,
            Machine_Code: Machine_Code,
            Machine_Name: Machine_Name,
            IP_ADDRESS: IP_ADDRESS,
            IP_PORT: IP_PORT,
            SERVER_NAME: SERVER_NAME,
            BRANCH: BRANCH,
            S_USER_ID: S_USER_ID,
            S_PASSWORD: S_PASSWORD,
            DB_NAME: DB_NAME,
            QUERY: QUERY
        };
        return modelRecord;
    },

    saveAttempt: function () {
        var obj = empr_MachineInfo.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/MachineInfo/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('.btn-delete').hide();
                $('.btn-print').hide();
                $('#resetall').hide();
                empr_MachineInfo.InintQuickSearch();
            }
        }, false, true);
    },

    InintQuickSearch: function () {
        empr_MachineInfo.GetQuickSearch();
    },

    GetQuickSearch: function () {
        ajaxHelper.ajaxGetJson('/MachineInfo/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_MachineInfo.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GetAllMachineInfos: function () {
        var xhr = ajaxHelper.ajaxGetJson('/MachineInfo/QuickSearch?menuid=' + empr_helper.getCode(), function (data) {
            empr_MachineInfo.CreateGrid(data.data);
        }, false, true);
    },

    makeReadOnly: function (isreadonly, type) {
        $("#VOUCHER_NO, input[type='radio']").prop("disabled", isreadonly);
    },

    GetMachineInfoByID: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/MachineInfo/MachineInfoByid?id=' + id, function (data) {
            debugger;
            console.log(Permissions);
            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
                }
                if (Permissions.r_PRINT) {
                    $('.btn-print').show();
                }
                if (Permissions.r_ADD) {
                    $('#resetall').show();
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
                $('#resetall').show();
            }
            $('#Code').val(data.data[0].tranid);
            $("#Machine_Code").val(data.data[0].machineCode);
            $("#Machine_Name").val(data.data[0].machineName);
            $('#BRANCH').dxSelectBox('instance').option("value", data.data[0].branch);
            $("#IP_ADDRESS").val(data.data[0].ipAddress);
            $("#IP_PORT").val(data.data[0].ipport);
            $("#SERVER_NAME").val(data.data[0].serverName);
            $("#S_USER_ID").val(data.data[0].sUserId);
            $('#S_PASSWORD').val(data.data[0].sPassword);
            $('#DB_NAME').val(data.data[0].dbName);
            $('#QUERY').val(data.data[0].query);
            

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
                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);
                if (Permissions != "Admin") {
                    const editAction = !Permissions.r_EDIT
                        ? ''
                        : `<a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.tranid} title="Edit"><i class="fa fa-edit"></i></a>`;
                    const actions = `<div class="btn-group btn-group-sm">${editAction}</div>`;
                    $(actions).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.tranid} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);
                }

            }
        },
        { dataField: 'tranid', caption: 'Code', width: 80, alignment: "center", visible: false, },
        { dataField: 'machineCode', caption: 'Machine Code' },
        { dataField: 'machineName', caption: 'Machine Name' },
        { dataField: 'branch', caption: 'Branch' },
        { dataField: 'ipAddress', caption: 'IP Address' },
        { dataField: 'ipport', caption: 'IP Port' },
        { dataField: 'serverName', caption: 'Server Name' },
        { dataField: 'sUserId', caption: 'S.User Id' },
        { dataField: 'sPassword', caption: 'S.Password' },
        { dataField: 'dbName', caption: 'Database Name' },
        { dataField: 'query', caption: 'Query', width: 150 }
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "MachineInfoQS" , "single");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    InitDropdowns: function () {
        debugger;
        empr_MachineInfo.InitCashAccount("");
        empr_MachineInfo.InitBankAccount("");
        empr_MachineInfo.InitPartyAccount("");
        empr_MachineInfo.InitBranch("");

    },

    InitDropdownsWithValue: function (lot, iCode, unit, pCode) {
        debugger
        empr_MachineInfo.InitUnitDDL(unit);
        empr_MachineInfo.InitReportTypeDDL();
        ajaxHelper.ajaxGetJson("/MachineInfo/GetItems", function (data) {
            if (data.msgType == 1) {
                empr_MachineInfo.InitItemCodeDDL(data.data, iCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/MachineInfo/GetLots", function (data) {
            if (data.msgType == 1) {
                empr_MachineInfo.InitLotDDL(data.data, lot);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
        ajaxHelper.ajaxGetJson("/MachineInfo/GetParties", function (data) {
            if (data.msgType == 1) {
                empr_MachineInfo.InitPartyCodeDDL(data.data, pCode);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitCashAccount: function (selectedValue) {
        debugger;
        $('#CACCOUNT').dxSelectBox({
            dataSource: CashAccount,
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
    },

    InitBankAccount: function (selectedValue) {
        $('#BACCOUNT').dxSelectBox({
            dataSource: BankAccount,
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
    },

    InitPartyAccount: function (selectedValue) {
        $('#PAY_ACTCODE').dxSelectBox({
            dataSource: PartyAccount,
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
    },

    InitBranch: function (selectedValue) {
        $('#BRANCH').dxSelectBox({
            dataSource: Branch,
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
    },

    InitLotDDL: function (dataSource, selectedValue) {
        $('#LOT').dxSelectBox({
            dataSource: dataSource,
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
                if (e.value != '' && e.value != null) {
                    var items = e.component._dataSource._items;
                    var item = items.filter(i => i.key == e.value);
                    if (item && item.length > 0) {
                        $('#PARTY_CODE').dxSelectBox('instance').option('value', item[0].customizedKey);
                    }
                }
            },
        });
    },

    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/MachineInfo/GetReportTypes", function (data) {
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
        let TRAN_ID = $("#Code").val();
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the delivery in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/MachineInfo/GetPrintReport", function (data) {
            if (data.msgType === 1) {
                const byteCharacters = atob(data.data);
                const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                const byteArray = new Uint8Array(byteNumbers);
                const blob = new Blob([byteArray], { type: 'application/pdf' });
                const url = URL.createObjectURL(blob);
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html(`<center><object data="${url}" width="1100" height="600"></object></center>`);
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitUnitDDL: function (_selectedValue) {
        $.ajax({
            url: 'MachineInfo/GetUnits',
            method: 'GET',
            data: null,
            success: function (data) {

                var selectedvalue = 0;
                var selectedobj = [];
                if (_selectedValue != null) {
                    selectedobj = data.filter(x => x.key == _selectedValue);
                    if (selectedobj.length > 0) {
                        selectedvalue = _selectedValue;
                        $("#unitcode_hidden").val(selectedvalue);
                        $("#displayExpr_unitcode").val(selectedobj[0].value);
                    }
                }

                empr_MachineInfo.bindDxGridBoxDdl('#UNIT', data, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedobj, selectedvalue, 'id', 'name', '#displayExpr_unitcode', function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var key = selectedvalue.selectedRowsData[0]['key'];
                        var value = selectedvalue.selectedRowsData[0]['value'];
                        $('#unitcode_hidden').val(key);
                        $('#displayExpr_unitcode').val(value);
                    }
                    else {
                        $('#unitcode_hidden').val('');
                        $('#displayExpr_unitcode').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitPartyCodeDDL: function (dataSource, selectedValue) {
        $('#PARTY_CODE').dxSelectBox({
            dataSource: dataSource,
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

    OpenGroupImage: function () {
        debugger;
        var baseUrl = "/Client/MachineInfo/";
        var hdnUrl = $('#Group_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
            //const fileURL = window.location.origin + baseUrl + hdnUrl;
            //window.open(fileURL, '_blank');
        }
    },

    SaveImage(imageName) {
        debugger;
        $('#BtnSave').prop('disabled', true);
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob, imageName);
        formData.append('imageName', imageName); // Important for backend
        $.ajax({
            url: `/MachineInfo/SaveImage`,
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    if (imageName == "ItemGroup")
                        $("#Group_IMG").val(data.data);
                    if (imageName == "ItemMaster")
                        $("#Item_IMG").val(data.data);
                    if (imageName == "Waiter")
                        $("#Waiter_IMG").val(data.data);
                    if (imageName == "Table")
                        $("#Table_IMG").val(data.data);
                }
                else {
                    console.log(data);
                    empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                }
                $('#BtnSave').prop('disabled', false);
            }
        });
    },

    OpenItemImage: function () {
        var baseUrl = "/Client/ItemMaster/";
        var hdnUrl = $('#Item_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },

    OpenWaiterImage: function () {
        var baseUrl = "/Client/Company/";
        var hdnUrl = $('#Waiter_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },

    OpenTableImage: function () {
        var baseUrl = "/Client/Company/";
        var hdnUrl = $('#Table_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage(hdnUrl);
        }
    },

}