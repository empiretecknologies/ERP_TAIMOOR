var empr_UploadItemImages = {
    editedRows: [],
    gridData: [],



    InitEvents: function () {
        $(document).ready(function () {
            empr_UploadItemImages.InitGrid();
            empr_UploadItemImages.InitItemGroupDDL();
            empr_UploadItemImages.InitStockStatusDDL();
            console.log("item groups name ", ItemGroups)
        });

        //$('body').on('click', '#BtnGet', function () {
        //    var STOCK_STATUS = $('#STOCK_STATUS').dxSelectBox('option', 'value');
        //    var CODE = $('#GROUP_CODE').dxSelectBox('option', 'value');

        //    var allData = empr_UploadItemImages.gridData || [];
        //    debugger;

        //    var filteredData = allData.filter(x => x.grouP_CODE == CODE);

        //    if (CODE == null || CODE == '0' || CODE == 0 || CODE == undefined) {

        //        empr_UploadItemImages.CreateGrid(allData);

        //    } else {
        //        empr_UploadItemImages.CreateGrid(filteredData);
        //    }
        //});

        //$('body').on('click', '#BtnGet', function () {
        //    var STOCK_STATUS = $('#STOCK_STATUS').dxSelectBox('option', 'value');
        //    var CODE = $('#GROUP_CODE').dxSelectBox('option', 'value');

        //    var allData = empr_UploadItemImages.gridData || [];
        //    debugger;

        //    var filteredData = allData.filter(function (x) {

        //        var matchCode = true;
        //        if (CODE !== null && CODE !== '0' && CODE !== 0 && CODE !== undefined && CODE !== "") {
        //            matchCode = (x.grouP_CODE == CODE);
        //        }

        //        var matchStatus = true;
        //        if (STOCK_STATUS !== null && STOCK_STATUS !== undefined && STOCK_STATUS !== "") {
        //            if (STOCK_STATUS === 'P') {
        //                matchStatus = (x.balance > 0);
        //            } else if (STOCK_STATUS === 'N') {
        //                matchStatus = (x.balance < 0);
        //            } else if (STOCK_STATUS === 'Z') {
        //                matchStatus = (x.balance == 0);
        //            }
        //        }

        //        return matchCode && matchStatus;
        //    });

        //    empr_UploadItemImages.CreateGrid(filteredData);
        //});
        $('body').on('click', '#BtnGet', function () {
            // 1. Dono dropdowns ki values get karein
            // STOCK_STATUS ab ek array hoga, jaise: ['N', 'Z'] ya []
            var STOCK_STATUS = $('#STOCK_STATUS').dxTagBox('option', 'value') || [];
            var CODE = $('#GROUP_CODE').dxSelectBox('option', 'value');

            var allData = empr_UploadItemImages.gridData || [];
            debugger;

            // 2. Data filtering
            var filteredData = allData.filter(function (x) {

                // --- Group Code Filter ---
                var matchCode = true;
                if (CODE !== null && CODE !== '0' && CODE !== 0 && CODE !== undefined && CODE !== "") {
                    matchCode = (x.grouP_CODE == CODE);
                }

                // --- Multiple Stock Status Filter ---
                var matchStatus = true;

                // Agar user ne kam az kam ek status select kiya hai (array khali nahi hai)
                if (STOCK_STATUS.length > 0) {

                    // Row ka status check karein ke kya hai
                    var currentStatus = '';
                    if (x.balance > 0) {
                        currentStatus = 'P';
                    } else if (x.balance < 0) {
                        currentStatus = 'N';
                    } else if (x.balance == 0) {
                        currentStatus = 'Z';
                    }

                    // Agar row ka status selected array ke andar majood HAI, to true warna false
                    matchStatus = STOCK_STATUS.includes(currentStatus);
                }

                // Dono conditions check karein
                return matchCode && matchStatus;
            });

            // 3. Grid load karein
            empr_UploadItemImages.CreateGrid(filteredData);
        });

        $('body').on('click', '#BtnSave', function () {
            empr_UploadItemImages.Save();
        });

        if (Permissions != "Admin") {
            (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            !Permissions.r_VIEW && $('#gridContainer').hide();
        }
    },

    InitGrid: function () {
        empr_UploadItemImages.GetItemMaster();
    },

    GetItemMaster: function () {
        ajaxHelper.ajaxGetJson('/UploadItemImages/GetItemMaster', function (data) {
            if (data.msgType == 1) {
                empr_UploadItemImages.CreateGrid(data.data);
                empr_UploadItemImages.gridData = data.data;
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGrid: function (dataSrc) {
        console.log(dataSrc)
        var col = [
            { dataField: 'iteM_CODE', caption: 'Item Code', visible: false },
            { dataField: 'iteM_NAME', caption: 'Items', allowEditing: false },
            {
                dataField: 'grouP_NAME', caption: 'Group Name', width: 200, allowEditing: false
            },
            { dataField: 'salE_RATE', caption: 'Sale Rate', allowEditing: false, width: 150, alignment: 'center' },
            {
                dataField: 'doc',
                caption: 'Doc',
                width: 300,
                allowEditing: false,
                cellTemplate: function (container, options) {

                    const wrapper = $('<div>').addClass('input-group');

                    const txtFileName = $('<input>')
                        .attr({
                            type: 'text',
                            readonly: true,
                            placeholder: 'No File',
                            id: 'gridDOCName'
                        })
                        .addClass('form-control');

                    if (options.data.doc && options.data.doc !== "") {
                        const onlyName = options.data.doc.split('/').pop();
                        txtFileName.val(onlyName);
                    }

                    const fileInput = $('<input>')
                        .attr({
                            type: 'file',
                            accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*',
                            id: 'gridDOC'
                        })
                        .css("display", "none");

                    //const browseBtn = $('<button>')
                    //    .addClass('btn btn-primary')
                    //    .addClass('my-griddoc-browse')
                    //    .text('Browse')
                    //    .on('click', function () {
                    //        fileInput.val('');
                    //        //txtFileName.val('');
                    //        fileInput.click();
                    //    });
                    const browseBtn = $('<button>')
                        .addClass('btn btn-primary')
                        .addClass('my-griddoc-browse')
                        .text('Browse')
                        .on('click', function (e) {
                            // 1. Event ko grid tak jaane se rokein (Row focus nahi hogi)
                            e.stopPropagation();
                            e.preventDefault();

                            fileInput.val('');
                            fileInput.click();
                        });

                    const eyeBtn = $('<a>')
                        .addClass('my-eye-btn')
                        .append($('<i>').addClass('fa fa-eye'))
                        .on('click', function () {
                            if (options.data.doc) {
                                window.open(options.data.doc, '_blank');
                            } else {
                                empr_helper.notify("No document available to view.", 2);
                            }
                        });

                    fileInput.on('change', function (e) {
                        const file = e.target.files[0];

                        if (!file) return;

                        let formData = new FormData();
                        formData.append('model', file, file.name);

                        $.ajax({
                            url: '/UploadItemImages/SaveImage',
                            type: "POST",
                            data: formData,
                            processData: false,
                            contentType: false,
                            //success: function (data) {

                            //    if (data.msgType == '1') {
                            //        let grid = options.component;
                            //        grid.cellValue(options.rowIndex, "doc", data.data);
                            //        grid.refresh();
                            //        txtFileName.val(file.name);
                            //    }
                            //    else {
                            //        txtFileName.val("No File");
                            //    }
                            //},
                            //error: function () {
                            //    empr_helper.notify("File upload failed.", 2);
                            //}
                            success: function (data) {
                                if (data.msgType == '1') {
                                    let grid = options.component;
                                    let rowIndex = options.rowIndex;
                                    let rowKey = options.key;

                                    // 1. Pehle cell ki value change karein (Isse batch edit mode me item highlight ho jayega)
                                    grid.cellValue(rowIndex, "doc", data.data);

                                    // 2. DevExtreme ke internal batch-render cycle ke baad selection apply karne ke liye setTimeout lagayein
                                    setTimeout(function () {
                                        // Pehle se selected saari keys ka array nikaalein
                                        let currentSelectedKeys = grid.getSelectedRowKeys();

                                        // Agar current row pehle se selected nahi hai, to array me push karein
                                        if (currentSelectedKeys.indexOf(rowKey) === -1) {
                                            currentSelectedKeys.push(rowKey);
                                        }

                                        // Saari keys ko ek sath select karwayein (false matlab purane select hataye bina)
                                        grid.selectRows(currentSelectedKeys, false);

                                        // UI par file name display set karein
                                        txtFileName.val(file.name);
                                    }, 50); // 50ms ka delay DevExtreme ke focus cycle ko bypass kar dega
                                }
                                else {
                                    txtFileName.val("No File");
                                }
                            },
                        });
                    });

                    wrapper.append(txtFileName, browseBtn, eyeBtn, fileInput);
                    $(container).append(wrapper);
                }
            },
            {
                dataField: 'balance',
                caption: 'Current Stock',
                width: 150,
                allowEditing: false,
                format: {
                    type: 'fixedPoint',
                    precision: 2
                },
                cellTemplate: function (container, options) {
                    var val = options.value;
                    //if (val === undefined || val === null || val === 0) {
                    //    container.text('');
                    //    return;
                    //}
                    var formattedValue = Math.abs(val).toFixed(0);
                    if (val < 0) {
                        $('<span>')
                            .text('(' + formattedValue + ')')
                            .css('color', 'red')
                            .appendTo(container);
                    }
                    else {
                        container.text(formattedValue);
                    }
                }

            }
        ];
        empr_helper.editableDxGridbinding_ItemImageUpload('#gridContainer', col, dataSrc, "ItemImageUpload", false);
    },

    GetEditedRows: function () {
        debugger;
        var originalArray = empr_UploadItemImages.editedRows.filter(function (row) {
            return row !== undefined;
        });

        var keyMap = {
            "acT_CODE": "ACT_CODE",
            "acT_GR_CODE": "ACT_GR_CODE",
            "acT_NAME": "ACT_NAME",
            "astatus": "ASTATUS",
            "controL_NAME": "CONTROL_NAME",
            "credit": "CREDIT",
            "debit": "DEBIT",
            "oP_ID": "OP_ID"
        };

        //return empr_UploadItemImages.RenameKeys(originalArray, keyMap);

        const newArray = originalArray.map(({
            acT_CODE: ACT_CODE,
            acT_GR_CODE: ACT_GR_CODE,
            acT_NAME: ACT_NAME,
            astatus: ASTATUS,
            controL_NAME: CONTROL_NAME,
            credit: CREDIT,
            debit: DEBIT,
            oP_ID: OP_ID
        }) => ({
            ACT_CODE,
            ACT_GR_CODE,
            ACT_NAME,
            ASTATUS,
            CONTROL_NAME,
            CREDIT,
            DEBIT,
            OP_ID
        }));

        return newArray;
    },

    RenameKeys: function (originalArray, keyMap) {
        //var renamedArray = {};
        //for (var key in originalArray) {
        //    if (keyMap.hasOwnProperty(key)) {
        //        renamedArray[keyMap[key]] = originalArray[key];
        //    } else {
        //        renamedArray[key] = originalArray[key];
        //    }
        //}
        //return renamedArray;

        var renamedArray = {};
        $.each(originalArray, function (key, value) {
            var newKey = keyMap[key] || key; // Use the mapped key if exists, otherwise keep the original key
            renamedArray[newKey] = value;
        });
        return renamedArray;
    },
    //Save: function () {
    //    debugger;
    //    if ($('#gridContainer').dxDataGrid('instance').hasEditData()) {
    //        $('#gridContainer').dxDataGrid('instance').saveEditData().done(function () {

    //            var dataModel = empr_UploadItemImages.GetEditedRows();
    //            if (dataModel.length > 0) {
    //                ajaxHelper.ajaxPostJsonData({ accountOpenings: dataModel }, "/UploadItemImages/Save", function (data) {
    //                    empr_helper.notify(data.msg, data.msgType);
    //                    if (data.msgType == 1) {
    //                        $('#IsValidate').val('true');
    //                        empr_UploadItemImages.editedRows = [];
    //                    }
    //                }, false, true);
    //            }
    //            console.log("Data saved successfully");
    //        }).fail(function (error) {
    //            console.error("Error occurred while saving the data:", error);
    //            empr_helper.notify("Error occurred while saving the data.", 2);
    //            $('#gridContainer').dxDataGrid('instance').cancelEditData();
    //        });
    //    } else {
    //        empr_helper.notify("Please update the opening first.", 2);
    //    }
    //},
    Save: function () {
        debugger;

        var gridInstance = $('#gridContainer').dxDataGrid('instance');
        var selectedData = gridInstance.getSelectedRowsData();

        if (selectedData.length > 0) {

            gridInstance.saveEditData().done(function () {

                ajaxHelper.ajaxPostJsonData({ UploadItemImages: selectedData }, "/UploadItemImages/Save", function (data) {
                    empr_helper.notify(data.msg, data.msgType);

                    if (data.msgType == 1) {
                        $('#IsValidate').val('true');
                        empr_UploadItemImages.editedRows = [];
                        gridInstance.clearSelection();
                    }
                }, false, true);

                console.log("Data saved successfully");

            }).fail(function (error) {
                console.error("Error occurred while saving the data:", error);
                empr_helper.notify("Error occurred while saving the data.", 2);
                gridInstance.cancelEditData();
            });

        } else {
            empr_helper.notify("Please select at least one row/item to save.", 2);
        }
    },

    InitItemGroupDDL: function (selectedValue) {

        //$.ajax({
        //    url: 'ItemMaster/GetItemGroups',
        //    method: 'GET',
        //    data: null,
        //    success: function (data) {
        $('#GROUP_CODE').dxSelectBox({
            dataSource: ItemGroups,
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
        });
        //    },
        //    error: function (error) {
        //        console.error('Error fetching data:', error);
        //    }
        //});
    },

    //InitStockStatusDDL: function (selectedValue) {
    //    var data = [
    //        { key: 'P', value: 'Positive' },
    //        { key: 'N', value: 'Negative' },
    //        { key: 'Z', value: 'Zero' },
    //    ];

    //    $('#STOCK_STATUS').dxSelectBox({
    //        dataSource: data,
    //        displayExpr: 'value',
    //        valueExpr: 'key',
    //        value: selectedValue,
    //        searchEnabled: true,
    //        width: '100%',
    //        placeholder: 'Search',
    //        showClearButton: true,
    //        dropDownOptions: {
    //            height: 'auto',
    //        },
    //        pagingEnabled: true,
    //        searchTimeout: 500,
    //    });

    //},
    InitStockStatusDDL: function (selectedValue) {
        var data = [
            { key: 'P', value: 'Positive' },
            { key: 'N', value: 'Negative' },
            { key: 'Z', value: 'Zero' },
        ];

        $('#STOCK_STATUS').dxTagBox({
            dataSource: data,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue || [],
            searchEnabled: true,
            width: '100%',
            placeholder: 'Select Statuses',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            showSelectionControls: true,
            applyValueMode: "instantly"
        });
    }
}