var empr_ItemGroups = {
    initEvents: function () {

        $(document).ready(function () {
            empr_ItemGroups.InitTree();

            empr_ItemGroups.InitAccountType();

            empr_ItemGroups.InitAccountGridBox(null);
            empr_ItemGroups.InitItemTypesGridBox(null);
            empr_ItemGroups.InitControlGridBox(null);
            
            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_ItemGroups.validateForm()) {
                            empr_ItemGroups.saveAttempt();
                        }
                    }
                } else {
                    if (empr_ItemGroups.validateForm()) {
                        empr_ItemGroups.saveAttempt();
                    }
                }
            });

            $('#ProductImage').change(function () {
                empr_ItemGroups.UploadImage();
            })

            $('body').on('click', '#quicksearch', function () {


                empr_ItemGroups.InintQuickSearch();


            })

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_ItemGroups.GetItemGroupByID(reportid);
            });

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
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, partY_NAME: $('#updatedName').val() }, "/ItemGroups/CopyRecord", function (data) {
                    console.log(data.data);
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_ItemGroups.GetItemGroupByID(data.data);
                    }
                }, false, true);
            });


            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('#resetall').hide();
                empr_ItemGroups.resetForm();
                empr_ItemGroups.InitTree();
                empr_ItemGroups.reFreshTree();
                empr_ItemGroups.hideshow(false);
            })

            $('.btn-delete').click(function () {

                empr_ItemGroups.DeleteRecord();

            })

            if (Permissions != "Admin") {
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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/ItemGroups/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_ItemGroups.resetForm();
                    empr_ItemGroups.InitTree();
                    empr_ItemGroups.reFreshTree();
                    empr_ItemGroups.hideshow(false);
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('#resetall').hide();
                }
            }, false, true);

        });

    },
    resetForm: function () {

        $("#Code").val('');
        $("#GROUP_NAME").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $('#GROUP_TYPE').dxSelectBox('instance').option('value', "C");
        $("#GR_CODE").val('');
        $("#SALES_TAX").val('');

        empr_ItemGroups.hideshow(false);

        if ($('#treeListContainer').dxTreeList('instance') != undefined) {
            $('#treeListContainer').dxTreeList('instance').clearFilter();
        }

        $("#groupparent_hidden").val('');
        $("#displayExpr_groupparent").val('');
        $("#itemtype_hidden").val('');
        $("#displayExpr_itemtype").val('');
        $("#displayExpr_account").val('');
        $("#account_hidden").val('');
        $("#ProductImage").val('');
        $("#ProductImageHidden").val('');
        $('#Image').val('');
        document.getElementById('STICKER').checked = false;

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        }
        else {
            $('#saveAttempt').show();
        }
        $('#GROUP_TYPE').dxSelectBox('instance').option('disabled', false);
        $('#PARENT_CODE').dxDropDownBox('instance').option('disabled', false);
        empr_ItemGroups.InitAccountGridBox();
        empr_ItemGroups.InitItemTypesGridBox();
        empr_ItemGroups.InitControlGridBox();
    },
    validateForm: function () {

        var valid = true;
        var GROUP_NAME = $("#GROUP_NAME").val().trim();
        var GROUP_TYPE = $("#GROUP_TYPE").dxSelectBox('instance').option('value');
        var ACT_PARENT_CODE = $("#groupparent_hidden").val();
        var ITEM_TYPE = $("#itemtype_hidden").val();
        var ACCOUNT_TYPE = $("#account_hidden").val(); 

        if (GROUP_NAME == '') {
            valid = false;
            empr_helper.notify("Please enter name.", 2);
        }

        if (GROUP_TYPE != 'C') {
            if (ACT_PARENT_CODE == '') {
                valid = false;
                empr_helper.notify("Please select control name.", 2);
            }

            if (ACCOUNT_TYPE == '') {
                valid = false;
                empr_helper.notify("Please select acccount type.", 2);
            }

            if (ITEM_TYPE == '') {
                valid = false;
                empr_helper.notify("Please select item type.", 2);
            }
        }

        return valid;

    },
    SalesTaxValidation: function (element) {

        var value = $(element).val();
        value = value.replace(/[^0-9.]/g, '');
        value = value.replace(/(\.\d{3})\d+/g, '$1');
        if (value === '' || (parseFloat(value) >= 0 && parseFloat(value) <= 100)) {
            $(element).val(value);
        } else {
            $(element).val($(element).data('lastValid') || '');
        }

        $(element).data('lastValid', $(element).val());
    },
    GetDataToSave: function () {

        var ID = $("#Code").val();
        var GROUP_NAME = $("#GROUP_NAME").val();
        var GROUP_TYPE = $("#GROUP_TYPE").dxSelectBox('instance').option('value');
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var ASETUP = $("#account_hidden").val();
        var ITEM_TYPE = $("#itemtype_hidden").val();
        var K_PRINTER = $("#K_PRINTER").val();
        var STK_PRINTER = $("#STK_PRINTER").val();
        var SALES_TAX = $("#SALES_TAX").val();
        var GR_CODE = $("#GR_CODE").val();
        var PARENT_CODE = $("#groupparent_hidden").val(); 
        var ProductImage = $("#ProductImageHidden").val();
        var STICKER = document.getElementById('STICKER').checked ? 1 : 0;

        console.log(ProductImage);
        var modelRecord = {
            GROUP_CODE: ID,
            GROUP_NAME: GROUP_NAME,
            GROUP_TYPE: GROUP_TYPE,
            ASTATUS: ASTATUS,
            ASETUP: ASETUP,
            ITEM_TYPE: ITEM_TYPE,
            SALES_TAX: SALES_TAX,
            IPIC: ProductImage,
            GR_CODE: GR_CODE,
            Code: ID,
            PARENT_CODE: PARENT_CODE,
            STICKER: STICKER,
            K_PRINTER: K_PRINTER,
            STK_PRINTER: STK_PRINTER

        }
        return modelRecord;
    },
    saveAttempt: function () {

        var obj = empr_ItemGroups.GetDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/ItemGroups/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_ItemGroups.resetForm();
                empr_ItemGroups.InitTree();
                empr_ItemGroups.reFreshTree();
                $('#optmodal').modal('hide');
                $('.btn-delete').hide();
                $('#resetall').hide();
            }
        }, false, true);
    },
    InintQuickSearch: function () {


        empr_ItemGroups.GetAllItemGroups();

    },
    GetAllItemGroups: function () {

        var xhr = ajaxHelper.ajaxGetJson('/ItemGroups/QuickSearch', function (data) {

            empr_ItemGroups.CreateGrid(data.data);

        }, false, true);

    },
    makeReadOnly: function (isreadonly, type) {
        $("#PREFIX, #CHQ_ID, #CURRENCY, #ACT_NATURE,  input[type='radio']").prop("disabled", isreadonly);
        $('#ACT_TYPE').dxSelectBox('instance').option('disabled', isreadonly);
        $('#accountnaturegridBox').dxDropDownBox('instance').option('disabled', isreadonly);
        $("#CURRENCY").dxSelectBox('instance').option('disabled', isreadonly);
        $('#controlgridBox').dxDropDownBox('instance').option('disabled', isreadonly);
        $('#flexSwitchCheckDefault').attr('disabled', isreadonly);
    },
    GetItemGroupByID: function (id) {
        //debugger;
        var xhr = ajaxHelper.ajaxGetJson('/ItemGroups/GetItemGroupByID?id=' + id, function (data) {
            debugger;

            //$('.btn-delete').show();
            //$('#resetall').show();

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
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

            $('.modal').modal('hide');
            //empr_ItemGroups.makeReadOnly(true);
            //debugger;
            //$('#Code').val(data.data.id);
            //$('#ACT_GR_CODE').val(data.data.acT_GR_CODE);

            //$('#ACT_NAME').val(data.data.acT_NAME);
            //$('#ACT_SNAME').val(data.data.acT_SNAME);
            //$('#PREFIX').val(data.data.prefix);
            //$('#ACCOUNT_NO').val(data.data.accounT_NO);
            //$('#TITTLE').val(data.data.tittle);
            //$('#SWIFT').val(data.data.swift);

            //$('#activestatushidden').val(data.data.astatus);
            //$('#typehidden').val(data.data.acT_TYPE);
            //$('#currencyhidden').val(data.data.currency);
            //$('#chequehidden').val(data.data.chQ_ID);

            //$("#flexSwitchCheckDefault").prop("checked", data.data.costcenter == 0 ? false : true);

            //$('#CURRENCY').dxSelectBox('instance').option("value", parseInt(data.data.currency));
            //$('#CHQ_ID').dxSelectBox('instance').option("value", parseInt(data.data.chQ_ID));
            //$('#ASTATUS').dxSelectBox('instance').option("value", data.data.astatus);
            //$('#ACT_TYPE').dxSelectBox('instance').option("value", data.data.acT_TYPE);

            //if (data.data.acT_NATURE == 2) {
            //    empr_ItemGroups.hideshow(true);
            //} else {
            //    empr_ItemGroups.hideshow(false);
            //}
            var record = data.data;
            $('#GROUP_TYPE').dxSelectBox('instance').option('disabled', true);
            $('#PARENT_CODE').dxDropDownBox('instance').option('disabled', true);
            $("#Code").val(record.grouP_CODE);
            $('#GROUP_TYPE').dxSelectBox('instance').option('value', record.grouP_TYPE)
            $("#GR_CODE").val(record.gR_CODE);
            $('#ASTATUS').dxSelectBox('instance').option('value', record.astatus)
            $("#GROUP_NAME").val(record.grouP_NAME)
            debugger;

            $("#K_PRINTER").val(record.k_PRINTER)
            $("#STK_PRINTER").val(record.stk_PRINTER)


            $("#ProductImageHidden").val(record.ipic)
            if (record.sticker == "1")
                document.getElementById('STICKER').checked = true;
            if (record.grouP_TYPE == "S") {
                empr_ItemGroups.hideshow(true);
            } else {
                empr_ItemGroups.hideshow(false);
            }
            $("#SALES_TAX").val(record.saleS_TAX)
            $("#GR_CODE").val(record.gR_CODE)
            
            empr_ItemGroups.InitControlGridBox(record.parenT_CODE);
            empr_ItemGroups.InitAccountGridBox(record.asetup);
            empr_ItemGroups.InitItemTypesGridBox(record.iteM_TYPE);            

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
                debugger
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.ipic != null && options.data.ipic != '' && options.data.ipic != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('${options.data.ipic}')"><i class="fa fa-eye"></i></a>`;
                }
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += `<a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportname="${options.data.grouP_NAME2}"" reportid=${options.data.grouP_CODE} title="COPY"><i class="fa fa-copy"></i></a>`;
                html += '</div>';
                $(html).appendTo(container);

                //$(`<div class="btn-group btn-group-sm">
                //    <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.grouP_CODE} title="Edit"><i class="fa fa-edit"></i></a>
                //</div>`).appendTo(container);
            }
        },
        {
            dataField: 'ipic',
            caption: 'Image',
            width: 120,
            visible: false,
            cellTemplate(container, options) {
                if (options.value != null && options.value != '' && options.value != undefined) {
                    $('<div>')
                        .append($('<img>', { src: options.value, height: '100px', width: '100px' }))
                        .appendTo(container);
                }
            },
        },
        { dataField: 'grouP_CODE', caption: 'Group Code', alignment: "center" },
        { dataField: 'grouP_NAME', caption: 'Group Name' },
        { dataField: 'parenT_CODE', caption: 'Parent' },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "ItemGroups");
    },
    InitTree: function () {

        $.ajax({
            url: 'ItemGroups/GetItemGroupsForTreeView',
            method: 'GET',
            success: function (data) {
                console.log(data);
                $('#treeListContainer').dxTreeList({
                    dataSource: data.data,
                    keyExpr: 'grouP_CODE',
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
                            dataField: 'grouP_NAME',
                            caption: 'Name'
                        }
                    ],
                    expandedRowKeys: [0],
                    showRowLines: true,
                    onRowDblClick: function (info) {
                        const clickedRowData = info.data;
                        empr_ItemGroups.GetItemGroupByID(clickedRowData.grouP_CODE);
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
        empr_ItemGroups.bindDxDdl("GROUP_TYPE", Datasource, "C", "Value", "Key", "Select", function (d) {
            $('#typehidden').val(d.value)
            if (d.value == null) {
                $('#typehidden').val('');
            }
            
            if (d.value == "S") {
                empr_ItemGroups.hideshow(true); 
            } else {
                empr_ItemGroups.hideshow(false);
            }
        });
    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },
    InitAccountGridBox: function (_selectedValue) {

        $.ajax({
            url: "ItemGroups/GetAccounts",
            type: "GET",
            success: function (response) {
                var Datasource = response;
                if (!Datasource || Datasource.length === 0) return;

                var selectedObject = [];
                var selectedValue = 0;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => x.key == _selectedValue);
                }
                if (selectedObject.length > 0) {
                    selectedValue = selectedObject[0].key;
                } else {
                    selectedObject = [Datasource[0]]; 
                    selectedValue = Datasource[0].key;
                }
                $('#account_hidden').val(selectedValue);
                $('#displayExpr_account').val(selectedObject[0].value);
                empr_ItemGroups.bindDxGridBoxDdl('#ASETUP', Datasource, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedObject, selectedValue, 'key', 'value', '#displayExpr_account', function (selectedvalue, hidden) {
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var value = selectedvalue.selectedRowsData[0]['key'];
                        var branch = selectedvalue.selectedRowsData[0]['value'];
                        $('#account_hidden').val(value);
                        $('#displayExpr_account').val(branch);
                    }
                    else {
                        $('#account_hidden').val('');
                        $('#displayExpr_account').val('');
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },
    InitItemTypesGridBox: function (_selectedValue) {

        $.ajax({
            url: "ItemGroups/GetItemTypes",
            type: "GET",
            success: function (response) {
                var Datasource = response;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.key == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].key;// Datasource.findIndex(x => { return x.grouP_CODE == _selectedValue }) == -1 ? 0 : Datasource.findIndex(x => { return x.grouP_CODE == _selectedValue })
                        $('#itemtype_hidden').val(selectedObject[0].key);
                        $('#displayExpr_itemtype').val(selectedObject[0].value);
                    }
                }

                empr_ItemGroups.bindDxGridBoxDdl('#ITEM_TYPE', Datasource, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }], 'hidden', selectedObject, selectedValue, 'key', 'value', '#displayExpr_itemtype', function (selectedvalue, hidden) {
                    debugger;
                    if (selectedvalue.selectedRowsData.length > 0) {

                        var id = selectedvalue.selectedRowsData[0]['key'];
                        var name = selectedvalue.selectedRowsData[0]['value'];
                        $('#itemtype_hidden').val(id);
                        $('#displayExpr_itemtype').val(name);

                    }
                    else {
                        $('#itemtype_hidden').val('');
                        $('#displayExpr_itemtype').val('');
                    }
                });
            }
        });
    },
    InitControlGridBox: function (_selectedValue) {
        debugger;
        $.ajax({
            url: "ItemGroups/GetItemGroupParents",
            type: "GET",
            success: function (response) {
                var Datasource = response;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    debugger;
                    selectedObject = Datasource.filter(x => { return x.key == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].key;// Datasource.findIndex(x => { return x.acT_CODE == _selectedValue }) == -1 ? 0 : Datasource.findIndex(x => { return x.acT_CODE == _selectedValue })
                        $('#groupparent_hidden').val(selectedObject[0].key);
                        $('#displayExpr_groupparent').val(selectedObject[0].value);
                    }
                }

                empr_ItemGroups.bindDxGridBoxDdl('#PARENT_CODE', Datasource, [{ dataField: 'key', caption: 'Code', width: '60px' }, { dataField: 'value', caption: 'Name' }, { dataField: 'name', caption: 'Control Name' }], 'hidden', selectedObject, selectedValue, 'key', 'value', '#displayExpr_groupparent', function (selectedvalue, hidden) {
                    debugger
                    if (selectedvalue.selectedRowsData.length > 0) {
                        var id = selectedvalue.selectedRowsData[0]['key'];
                        var name = selectedvalue.selectedRowsData[0]['value'];
                        $('#groupparent_hidden').val(id);
                        $('#displayExpr_groupparent').val(name);
                    }
                    else {
                        $('#groupparent_hidden').val('');
                        $('#displayExpr_groupparent').val('');
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
            $("#account").show();
            $("#item").show();
            $("#tax").show();
            $("#stickerss").show();
            $("#KT_PRINTER").show();
            $("#ST_PRINTER").show();
        } else {
            $("#account").hide();
            $("#item").hide();
            $("#tax").hide();
            $("#stickerss").hide();
            $("#stickerss").css("display", "none !important");
            $("#KT_PRINTER").hide();
            $("#ST_PRINTER").hide();

        }
    },
    UploadImage: function () {
        $('#BtnSave').prop('disabled', true);
        var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
        var binaryData = atob(base64String);
        var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
            return char.charCodeAt(0);
        }))], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob);
        $.ajax(
            {
                url: "/ItemGroups/UploadImage",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#ProductImageHidden").val(data.data);
                    }
                    else {
                        console.log(data);
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },
    OpenDoc: function () {
        var hdnUrl = $('#ProductImageHidden').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a image to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            ShowImage(fileURL);
        }
    },
}