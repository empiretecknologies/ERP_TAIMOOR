var empr_helper = {
    debitAverage: 0,
    creditAverage: 0,
    sellerAverage: 0,
    buyerAverage: 0,
    selectedBills: [],
    partyTypeCode: [],
    companyName: '',
    reportName: '',
    invoiceNo: '',
    fromDate: '',
    dcType: '',
    toDate: '',
    balanceAmount: '',
    lastAmt: 0,
    typeOneTotal: 0,
    typeTwoTotal: 0,
    typeThreeTotal: 0,
    typeFourTotal: 0,
    lastDate: '',

    conditions: [
        { key: 'Adv', value: 'Advance' },
        { key: 'Cash', value: 'Cash' },
        { key: 'Cr', value: 'Credit Days' },
        { key: 'CrD', value: 'Credit Date' },
        { key: 'Ad', value: 'Advance Days' },
        { key: 'Lc', value: 'LC' },
        { key: 'Cad', value: 'CAD' },
        { key: 'Da', value: 'DA' },
        { key: 'Con', value: 'Consignment' },
    ],

    priority: [
        { key: 'N', value: 'Normal' },
        { key: 'U', value: 'Urgent' },
        { key: 'M', value: 'Most Urgent' },
    ],

    gender: [
        { value: 'M', key: 'Male' },
        { value: 'F', key: 'Female' },
    ],

    maritalStatus: [
        { value: 'M', key: 'Married' },
        { value: 'UM', key: 'UnMarried' },
        { value: 'W', key: 'Widow' },
        { value: 'D', key: 'Divorced' },
    ],

    SBF_TYPE: [
        { value: 'LOC', text: 'Local' },
        { value: 'IMP', text: 'Import' },
        { value: 'EXP', text: 'Export' },
        { value: 'IND', text: 'Indent' }
    ],

    hrRecommendation: [
        { key: 'Hire', value: 'Hire' },
        { key: 'Hold', value: 'Hold' },
        { key: 'Reject', value: 'Reject' }
    ],

    hrStatus: [
        { key: 'Draft', value: 'Draft' },
        { key: 'Sent', value: 'Sent' },
        { key: 'Accepted', value: 'Accepted' },
        { key: 'Rejected', value: 'Rejected' }
    ],

    commType: [
        { key: 'PR', value: 'Percent' },
        { key: 'RS', value: 'Value' }
    ],

    bindSingleSelectDropDown: function (pControlID, Datasource, hiddenId, valueField, displayField, IsDisabled, FinalizeSelecteditems) {

        ati_dxHelper.LoadDDSearchControl(pControlID, Datasource, hiddenId, valueField, displayField, IsDisabled, FinalizeSelecteditems);

    },

    getTodayDate: function () {

        const local = new Date();
        local.setMinutes(local.getMinutes() - local.getTimezoneOffset());
        return local.toJSON().slice(0, 10);


    },

    notify: function (msg, type) {
        //https://www.jqueryscript.net/other/Highly-Customizable-jQuery-Toast-Message-Plugin-Toastr.html
        if (type == 1) {
            toastr.success(msg, 'Success');

        } if (type == 2) {
            toastr.error(msg, 'Error');

        } if (type == 3) {
            toastr.warning(msg, 'Warning');
        } if (type == 4) {
            toastr.info(msg, 'Info');
        }

    },

    strongPasswordValidation: function (elm) {

        var p = document.getElementById(elm).value,
            errors = [];
        if (p.length < 8) {
            errors.push("Your password must be at least 8 characters");
        }
        if (p.search(/[a-z]/i) < 0) {
            errors.push("Your password must contain at least one letter.");
        }
        if (p.search(/[0-9]/) < 0) {
            errors.push("Your password must contain at least one digit.");
        }
        if (errors.length > 0) {

            empr_helper.notify(errors.join("\n"), 2);

            return false;
        }
        return true;


    },

    matchConformPassword: function (p, cp) {

        var p = document.getElementById(p).value;
        var cp = document.getElementById(cp).value;

        if (p != cp) {

            empr_helper.notify("Password and confirm password does not match.", 2);
            return false;

        }
        return true;

    },

    isEmail: function (emailAdress) {


        let regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

        if (emailAdress.match(regex))
            return true;

        else
            return false;


    },

    DecimalFieldValidation: function (element) {
        //var value = $(element).val();
        //if ($.isNumeric(value) && parseFloat(value) >= 0 && parseFloat(value) <= 100) {
        //    $(element).get(0).setCustomValidity('');
        //} else {
        //    $(element).get(0).setCustomValidity("Please enter a valid number between 0 and 100.");
        //}

        var value = $(element).val();
        /*value = value.replace(/[^0-9.]/g, ''); // very close and working fine */
        value = value.replace(/[^0-9.]/g, '');
        value = value.replace(/\.(?=.*\.)/g, '');
        // Allow up to three decimal digits
        value = value.replace(/(\.\d{3})\d+/g, '$1');
        if (value === '' || (parseFloat(value) >= 0 && parseFloat(value) <= 100)) {
            $(element).val(value);
        } else {
            $(element).val($(element).data('lastValid') || '');
        }

        $(element).data('lastValid', $(element).val());
    },

    EnhancedDecimalFieldValidation: function (element) {
        var value = $(element).val();
        value = value.replace(/[^0-9.]/g, '');
        value = value.replace(/\.(?=.*\.)/g, '');
        value = value.replace(/(\.\d{3})\d+/g, '$1');
        //if (value === '' || (parseFloat(value) >= 0 && !/\.\d{3,}/.test(value))) {
        if (value === '' || parseFloat(value) >= 0) {
            $(element).val(value);
        } else {
            $(element).val($(element).data('lastValid') || '');
        }

        $(element).data('lastValid', $(element).val());
    },

    OnlyNumberAndHyphen: function (element) {

        var inputValue = $(element).val();
        var sanitizedValue = '';
        var hyphenCount = 0;

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 12) {
                if (char === '-' && hyphenCount === 0) {
                    sanitizedValue += char;
                    hyphenCount++;
                } else if (/[\d]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);
    },

    OnlyNumberAndHyphen20: function (element) {

        var inputValue = $(element).val();
        var sanitizedValue = '';
        var hyphenCount = 0;

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 20) {
                if (char === '-' && hyphenCount === 0) {
                    sanitizedValue += char;
                    hyphenCount++;
                } else if (/[\d]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);
    },

    OnlyNumber: function (element) {

        var inputValue = $(element).val();
        var sanitizedValue = '';

        for (var i = 0; i < inputValue.length; i++) {
            var char = inputValue.charAt(i);
            if (sanitizedValue.length < 12) {
                if (/[\d.]/.test(char)) {
                    sanitizedValue += char;
                }
            }
        }

        $(element).val(sanitizedValue);
    },

    getCode: function () {

        return $('#code').val();

    },

    dxGridbinding: function (div, columns, datasrc, fileName, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple";
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            //"allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0 || selectionMode == 'single') {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        let AllRows = true;
                        if (selectionMode == 'single') {
                            AllRows = false;
                        }
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: AllRows,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0 || selectionMode == 'single') {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);
                        let AllRows = true;
                        if (selectionMode == 'single') {
                            AllRows = false;
                        }
                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: AllRows
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            //customizeText(cellInfo) {
            //    const column = cellInfo.column;
            //    const value = cellInfo.value;

            //    // Check if the column name contains "date" and value is default
            //    if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        return '';
            //    }

            //    // If not a date column or not the default date value, return the original value
            //    return value;
            //}
            //onCellPrepared: function (e) {
            //    if (e.rowType === "data" && e.column.dataField.toLower().includes('data')) {

            //    }
            //},

            //onEditorPreparing: function (e) {
            //    if (e.dataField.toLower().includes('date') && e.parentType === "dataRow") {
            //        const defaultValueChangeHandler = e.editorOptions.onValueChanged;
            //        e.editorOptions.onValueChanged = function (args) { // Override the default handler
            //            // ...
            //            // Custom commands go here
            //            // ...
            //            // If you want to modify the editor value, call the setValue function:
            //            // e.setValue(newValue);
            //            // Otherwise, call the default handler:
            //            defaultValueChangeHandler(args);
            //        }
            //    }
            //cellTemplate(container, options) {
            //    const column = options.column;
            //    const value = options.value;

            //    // Check if the column name contains "date"
            //    if (value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        container.innerText = '';
            //    } else {
            //        container.innerText = value;
            //    }
            //}
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');



    },

    editableDxGridbinding_ItemImageUpload: function (div, columns, datasrc, fileName, isLandscape = false) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": 'multiple',
                "selectAllMode": 'page',
                "showCheckBoxesMode": 'always',
            },
            "stateStoring": {
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            editing: {
                mode: 'batch',
                allowUpdating: true,
                startEditAction: 'click',
                selectOnRowClick: false,
                onRowUpdating: function (e) {
                    debugger;
                    var editedRow = e.newData;
                    if (editedRow) {
                        var rowIndex = e.rowIndex;
                        empr_AccountOpening.editedRows[rowIndex] = editedRow;
                    }
                }
            },
            onExporting(e) {
                const fileName = empr_helper.reportName || "Report";

                if (e.format === 'pdf') {
                    $("#Loader").show();
                    $("#Loader").css('display', 'flex');
                    const layout = isLandscape;
                    const doc = new jsPDF({
                        orientation: layout,
                        unit: 'pt',
                        format: 'a4'
                    });

                    function addHeader() {
                        const pageWidth = doc.internal.pageSize.getWidth() - 27;

                        doc.setFont("helvetica", "bold");
                        doc.setFontSize(12);
                        doc.setTextColor(5, 90, 135);
                        doc.text(empr_helper.companyName, 20, 20);

                        doc.setFont("helvetica", "normal");
                        doc.setFontSize(10);
                        doc.text(empr_helper.reportName, 20, 40);
                        doc.setFillColor(5, 90, 135);
                        doc.rect(18, 47, pageWidth, 39, 'F');
                        doc.setFillColor(255, 255, 255);
                        doc.rect(18, 66, pageWidth, 0.5, 'F');

                        doc.setFontSize(8);
                        doc.setTextColor(255, 255, 255);
                        doc.text(`From : ${empr_helper.formatDate(empr_helper.fromDate)}       To: ${empr_helper.formatDate(empr_helper.toDate)}`, 20, 60);

                        const now = new Date();
                        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
                        const currentDate = now.toLocaleDateString('en-GB', options);
                        const currentTime = now.toLocaleTimeString();
                        doc.text(`Printed Date : ${currentDate}       Time : ${currentTime}`, 20, 80);
                        doc.setTextColor(0, 0, 0);
                    }
                    addHeader();


                    // ---------- GRID INSTANCES ----------
                    const grids = [];
                    const grid1 = $("#gridContainer").dxDataGrid("instance");
                    if (grid1) grids.push({ instance: grid1, title: empr_helper.reportName });

                    const isPartyLedger = empr_helper.reportName === 'Party Ledger';
                    //if (isPartyLedger) {
                    //    const grid2 = $("#AgingReportGridContainer").dxDataGrid("instance");
                    //    if (grid2) grids.push({ instance: grid2, title: empr_helper.reportName + " Aging" });
                    //}

                    // ---------- PREPARE GRIDS DATA ----------
                    const requestGrids = grids.map(g => {
                        const grid = g.instance;

                        const visibleCols = grid.getVisibleColumns().map(c => ({
                            dataField: c.dataField,
                            caption: c.caption || c.dataField
                        }));
                        const docIndex = visibleCols.findIndex(c => c.dataField === 'doc' || c.caption === 'doc');
                        if (docIndex > -1) {
                            const [docCol] = visibleCols.splice(docIndex, 1);
                            visibleCols.unshift(docCol);
                        }

                        const groupCols = grid.option("columns")
                            .filter(c => c.groupIndex !== undefined && c.groupIndex >= 0)
                            .sort((a, b) => a.groupIndex - b.groupIndex)
                            .map(c => c.dataField);

                        //const groupColsCap = grid.option("columns")
                        //    .filter(c => c.groupIndex !== undefined && c.groupIndex >= 0)
                        //    .sort((a, b) => a.groupIndex - b.groupIndex)
                        //    .map(c => c.caption || c.dataField);

                        // CURRENT GROUPS GET
                        const groupColsCap = grid.getVisibleColumns()
                            .filter(c => c.groupIndex !== undefined && c.groupIndex >= 0)
                            .sort((a, b) => a.groupIndex - b.groupIndex)
                            .map(c => c.caption || c.dataField);

                        const totals = {};
                        (grid.option("summary.totalItems") || []).forEach(s => {
                            totals[s.column] = grid.getTotalSummaryValue(s.column);
                        });

                        const data = (grid.option("dataSource") || []).map(row => {
                            const filteredRow = {};
                            visibleCols.forEach(col => {
                                filteredRow[col.caption] = row[col.dataField];
                            });
                            return filteredRow;
                        });
                        debugger;
                        return {
                            GridTitle: g.title,
                            GridData: data,
                            GroupColumnsCap: groupColsCap,
                            GroupColumns: groupCols,
                            Totals: totals,
                            IsLandscape: isLandscape
                        };
                    });

                    // AJAX call shuru hone se theek PEHLE hi tab khol lein (isey browser block nahi karega)
                    const pdfWindow = window.open("", "_blank");
                    if (pdfWindow) {
                        pdfWindow.document.write("<p style='font-family:sans-serif; text-align:center; margin-top:20%;'>Generating Your PDF, please wait...</p>");
                    }

                    $.ajax({
                        url: '/Report/GeneratePDF',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            CompanyName: empr_helper.companyName,
                            ReportName: empr_helper.reportName,
                            lastDate: empr_helper.lastDate || null,
                            lastAmount: empr_helper.lastAmt || null,
                            From: empr_helper.formatDateTopdf(empr_helper.fromDate),
                            To: empr_helper.formatDateTopdf(empr_helper.toDate),
                            IsLandscape: isLandscape,
                            Grids: requestGrids
                        }),
                        xhrFields: { responseType: 'blob' },
                        timeout: 300000,
                        success: function (result) {
                            console.log('PDF GENERATED');
                            const url = URL.createObjectURL(result);

                            // Agar tab successfully khula tha, toh usme PDF load kar dein
                            if (pdfWindow && !pdfWindow.closed) {
                                pdfWindow.location.href = url;
                            } else {
                                // Fallback: Agar phir bhi user ne pehle wala close kar diya ho
                                window.open(url, '_blank');
                            }

                            $("#Loader").hide();
                        },
                        error: function (xhr, status, error) {
                            console.error("Status: ", status);
                            // Agar error aaye toh khule hue blank tab ko band kar dein
                            if (pdfWindow) pdfWindow.close();

                            alert("PDF generation failed!");
                            $("#Loader").hide();
                        }
                    });

                } else {
                    // ---------- EXCEL EXPORT ----------
                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet(fileName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                        customizeCell: function (options) {
                            const { gridCell, excelCell } = options;
                            const targetFields = [
                                'debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc',
                                'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt',
                                'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales',
                                'cash', 'cardType', 'party'
                            ];

                            if (gridCell.rowType !== 'header' && targetFields.includes(gridCell.column.dataField)) {
                                if (typeof gridCell.value === 'number') {
                                    excelCell.numFmt = '#,##0';
                                }
                            }
                        }
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then(buffer => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                        });
                    });
                }
            },


            onEditorPreparing: function (e) {
                if (e.parentType === "dataRow" && e.dataField === "doc") {
                    const oldValue = e.value;
                    const standardHandler = e.setValue;

                    e.setValue = function (newValue) {
                        standardHandler(newValue);
                        if (newValue && newValue !== oldValue) {
                            const gridInstance = $(div).dxDataGrid('instance');
                            const rowKey = e.row.key;
                            gridInstance.selectRows([rowKey], true);
                        }
                    }
                }
            },
            showBorders: true,
            onToolbarPreparing: function (e) {
                var toolbarItems = e.toolbarOptions.items;
                for (var i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton") {
                        toolbarItems.splice(i, 1);
                        i--;
                    }
                }
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
        }).dxDataGrid('instance');
    },

    getTextPixelWidth: function (text, font = "14px Segoe UI") {
        const canvas = document.createElement("canvas");
        const ctx = canvas.getContext("2d");
        ctx.font = font;
        return ctx.measureText(text).width;
    },

    setLookupColumnWidths: function (columns) {
        console.log('Run');

        columns.forEach(col => {
            if (col.lookup && Array.isArray(col.lookup.dataSource)) {

                let displayExpr = col.lookup.displayExpr || "";
                let items = col.lookup.dataSource;

                let maxPx = 0;

                items.forEach(x => {
                    let text = x[displayExpr] ? String(x[displayExpr]).trim() : "";
                    let px = empr_helper.getTextPixelWidth(text);
                    if (px > maxPx)
                        maxPx = px;
                });

                maxPx += 10;

                if (maxPx < 80) maxPx = 80;

                col.width = maxPx;
            }
        });

        return columns;
    },

    dxGridbindingVouchers: function (div, columns, datasrc, fileName, selectionMode, extraOptions) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple"
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },

            stateStoring: {
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            "scrolling": {
                "mode": "both"
            },
            "rowDragging": {
                allowReordering: true,
                showDragIcons: true,
                onReorder: function (e) {
                    const data = e.component.option("dataSource");
                    const item = data.splice(e.fromIndex, 1)[0];
                    data.splice(e.toIndex, 0, item);
                    e.component.option("dataSource", data);
                }
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
                showCheckBoxsMode: 'always'
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            ...extraOptions,

            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
            onContentReady: function (e) {
                e.component.getView('headerPanel').element()
                    .find('.dx-datagrid-column-chooser-button')
                    .off('click')
                    .on('click', function () {
                        e.component.showColumnChooser();
                    });
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0 || selectionMode == 'single') {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        let AllRows = true;
                        if (selectionMode == 'single') {
                            AllRows = false;
                        }
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: AllRows,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0 || selectionMode == 'single') {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);
                        let AllRows = true;
                        if (selectionMode == 'single') {
                            AllRows = false;
                        }
                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: AllRows
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        },
                        name: "ActionCount"
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "discount",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "ActionCount") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue++;
                        }
                    }
                }
            }
        }).dxDataGrid('instance');



    },

    dxGridbindingVouchersForPrint: function (div, columns, datasrc, fileName, selectionMode, extraOptions) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple"
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": false,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },

            stateStoring: {
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            "scrolling": {
                "mode": "both"
            },
            "rowDragging": {
                allowReordering: true,
                showDragIcons: true,
                onReorder: function (e) {
                    const data = e.component.option("dataSource");
                    const item = data.splice(e.fromIndex, 1)[0];
                    data.splice(e.toIndex, 0, item);
                    e.component.option("dataSource", data);
                }
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
                showCheckBoxsMode: 'always'
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            ...extraOptions,

            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
            onContentReady: function (e) {
                e.component.getView('headerPanel').element()
                    .find('.dx-datagrid-column-chooser-button')
                    .off('click')
                    .on('click', function () {
                        e.component.showColumnChooser();
                    });
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0 || selectionMode == 'single') {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        let AllRows = true;
                        if (selectionMode == 'single') {
                            AllRows = false;
                        }
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: AllRows,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0 || selectionMode == 'single') {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);
                        let AllRows = true;
                        if (selectionMode == 'single') {
                            AllRows = false;
                        }
                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: AllRows
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        },
                        name: "ActionCount"
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "discount",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "ActionCount") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue++;
                        }
                    }
                }
            }
        }).dxDataGrid('instance');



    },

    dxGridbindingDetailCharges: function (div, columns, datasrc, fileName, selectionMode) {
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "rowAlternationEnabled": true,
            "paging": { "pageSize": 10 },
            editing: {
                mode: "cell",
                allowUpdating: true,
                useIcons: false,
                texts: { confirmDeleteMessage: '' }
            },
            "scrolling": { "mode": "both" },

            onRowUpdated: function (e) {
                console.log("Data changed, auto-saving...", e.data);

                empr_CashReceiptVoucher.SaveDetailCharges();
            }

        }).dxDataGrid('instance');
    },

    dxGridbindingKnockOff: function (div, columns, datasrc, fileName, selectionMode) {
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "rowAlternationEnabled": true,
            "paging": {
                "pageSize": 8,
            },
            "pager": {
                "visible": true,
                "showPageSizeSelector": false,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            editing: {
                mode: "cell",
                allowUpdating: true,
                useIcons: false,
                texts: {
                    confirmDeleteMessage: ''
                }
            },
            "scrolling": {
                "mode": "both"
            },
            summary: {
                totalItems: [
                    {
                        column: "qty",
                        summaryType: "custom", // ✅ corrected spelling
                        customizeText: function (data) {
                            return "Total : ";
                        },
                        name: "ActionCount" // ✅ optional but recommended when using custom
                    },
                    {
                        column: "whT_NET_AMT",
                        summaryType: "sum",
                        // Option 1: Using precision format
                        valueFormat: { type: "fixedPoint", precision: 2 },
                        displayFormat: "{0}"
                    },
                    {
                        column: "whT_AMT",
                        summaryType: "sum",
                        // Option 2: Using a format string directly in displayFormat
                        displayFormat: "{0}",
                        valueFormat: "#,##0.00"
                    },
                    {
                        column: "picK_AMT",
                        summaryType: "sum",
                        valueFormat: { type: "fixedPoint", precision: 2 },
                        displayFormat: "{0}"
                    },
                    {
                        column: "kO_AMT",
                        summaryType: "sum",
                        valueFormat: { type: "fixedPoint", precision: 2 },
                        displayFormat: "{0}"
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        valueFormat: { type: "fixedPoint", precision: 2 },
                        displayFormat: "{0}"
                    }
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "ActionCount") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue++;
                        }
                    }
                }
            }
        }).dxDataGrid('instance');



    },

    ExpensedxGridbinding: function (div, columns, datasrc, fileName, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple";
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            //"allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            //customizeText(cellInfo) {
            //    const column = cellInfo.column;
            //    const value = cellInfo.value;

            //    // Check if the column name contains "date" and value is default
            //    if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        return '';
            //    }

            //    // If not a date column or not the default date value, return the original value
            //    return value;
            //}
            //onCellPrepared: function (e) {
            //    if (e.rowType === "data" && e.column.dataField.toLower().includes('data')) {

            //    }
            //},

            //onEditorPreparing: function (e) {
            //    if (e.dataField.toLower().includes('date') && e.parentType === "dataRow") {
            //        const defaultValueChangeHandler = e.editorOptions.onValueChanged;
            //        e.editorOptions.onValueChanged = function (args) { // Override the default handler
            //            // ...
            //            // Custom commands go here
            //            // ...
            //            // If you want to modify the editor value, call the setValue function:
            //            // e.setValue(newValue);
            //            // Otherwise, call the default handler:
            //            defaultValueChangeHandler(args);
            //        }
            //    }
            //cellTemplate(container, options) {
            //    const column = options.column;
            //    const value = options.value;

            //    // Check if the column name contains "date"
            //    if (value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        container.innerText = '';
            //    } else {
            //        container.innerText = value;
            //    }
            //}
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');



    },
    dxGridbindingForMultiBillPrint: function (div, columns, datasrc, fileName, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple";
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    var img = new Image();
                                    img.src = options.gridCell.value;
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            //customizeText(cellInfo) {
            //    const column = cellInfo.column;
            //    const value = cellInfo.value;

            //    // Check if the column name contains "date" and value is default
            //    if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        return '';
            //    }

            //    // If not a date column or not the default date value, return the original value
            //    return value;
            //}
            //onCellPrepared: function (e) {
            //    if (e.rowType === "data" && e.column.dataField.toLower().includes('data')) {

            //    }
            //},

            //onEditorPreparing: function (e) {
            //    if (e.dataField.toLower().includes('date') && e.parentType === "dataRow") {
            //        const defaultValueChangeHandler = e.editorOptions.onValueChanged;
            //        e.editorOptions.onValueChanged = function (args) { // Override the default handler
            //            // ...
            //            // Custom commands go here
            //            // ...
            //            // If you want to modify the editor value, call the setValue function:
            //            // e.setValue(newValue);
            //            // Otherwise, call the default handler:
            //            defaultValueChangeHandler(args);
            //        }
            //    }
            //cellTemplate(container, options) {
            //    const column = options.column;
            //    const value = options.value;

            //    // Check if the column name contains "date"
            //    if (value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        container.innerText = '';
            //    } else {
            //        container.innerText = value;
            //    }
            //}
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            onToolbarPreparing: function (e) {
                let toolbarItems = e.toolbarOptions.items;

                toolbarItems.unshift({
                    widget: 'dxButton',
                    options: {
                        icon: 'print',
                        text: '',
                        onClick: function () {
                            if (e.component.getSelectedRowsData().length > 0) {
                                empr_helper.selectedBills = e.component.getSelectedRowsData().map(x => x.traN_ID);
                                empr_helper.printMultipleBills();
                                //let TRAN_IDs = e.component.getSelectedRowsData().map(x => x.traN_ID);
                                //console.log(TRAN_IDs)
                                //let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
                                //var dataModel = {
                                //    TRAN_IDs: TRAN_IDs,
                                //    MD_ID: MD_ID,
                                //}
                                //ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseSaleFormat/GetMultiBillPrintReport", function (data) {
                                //    if (data.msgType === 1) {
                                //        const byteCharacters = atob(data.data);
                                //        const byteNumbers = Array.from(byteCharacters, char => char.charCodeAt(0));
                                //        const byteArray = new Uint8Array(byteNumbers);
                                //        const blob = new Blob([byteArray], { type: 'application/pdf' });
                                //        const url = URL.createObjectURL(blob);
                                //        $('#ModalBody').empty();
                                //        setTimeout(function () {
                                //            $('#ModalBody').html(`<center><object data="${url}" width="1100" height="600"></object></center>`);
                                //            $('#ShowReportModal').show();
                                //            $('#ShowReportModal').modal('show');
                                //        }, 100);
                                //    }
                                //    else {
                                //        empr_helper.notify(data.msg, data.msgType);
                                //    }
                                //}, false, true);
                            } else {
                                empr_helper.notify("Please select the bill first.", 2);
                            }
                        }
                    },
                    location: 'after'
                });

                toolbarItems.unshift({
                    location: 'after',
                    template: function () {
                        return $("<div>").addClass("dx-toolbar-separator");
                    }
                });
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');



    },

    printMultipleBills: function () {
        let TRAN_IDs = empr_helper.selectedBills;
        if (TRAN_IDs.length > 0) {
            let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
            var dataModel = {
                TRAN_IDs: TRAN_IDs,
                MD_ID: MD_ID,
            }
            ajaxHelper.ajaxPostJsonData(dataModel, "/PurchaseSaleFormat/GetMultiBillPrintReport", function (data) {
                console.log(data);
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
        } else {
            empr_helper.notify("Please select the bill first.", 2);
        }
    },

    dxGridbindingWithoutFeatures: function (div, columns, datasrc, fileName, selectionMode) {
        var mode = "multiple";
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "rowAlternationEnabled": true,
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            selection: {
                mode: mode,
                //showCheckBoxesMode: "always",
                //selectAllMode: "allPages"
            },

            onContentReady: function (e) {

                if (div === "#menuPageContainer") {
                    debugger;

                    e.component.selectAll();

                    var allRows = e.component.getDataSource().items();

                    allRows.forEach(function (row) {
                        row.IsSelected = true;
                    });

                    console.log("All menu rows set to IsSelected = true");
                }
            },

            onCellClick(e) {
                if (e.column.dataField === 'menU_NAME') {
                    console.log(e.data)
                    empr_Role.LoadMenues(e.data.menU_GRCODE, e.data.id);

                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    $(e.cellElement).text('')
                }
            }
        }).dxDataGrid('instance');
    },

    dxGridbindingForReports: function (div, columns, datasrc, fileName, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple";
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            //"allowColumnReordering": true,
            "rowAlternationEnabled": true,
            //"groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
            },
            onSelectionChanged: function (e) {
                var selectedData = e.selectedRowsData;
                if (fileName == "ReportTypesList") {
                    if (selectedData.length > 0) {
                        if (selectedData[0].reporT_NAME == "Party Aging Report - Vendor" || selectedData[0].reporT_NAME == "Accounts Payable Recovery Report") {
                            $('#NATURE').dxSelectBox('instance').option('value', 3);
                        }
                        else if (selectedData[0].reporT_NAME == "Party Aging Report - Customer" || selectedData[0].reporT_NAME == "Customer Recovery Report") {
                            $('#NATURE').dxSelectBox('instance').option('value', 4);
                        }
                        else {
                            $('#NATURE').dxSelectBox('instance').option('value', '');
                        }
                    }
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    $(e.cellElement).text('')
                }
            },
            summary: {
                groupItems: [
                    {
                        column: "balance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '##0',
                    },
                ],
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "rate",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');
    },

    dxGridbindingLazyLoading: function (div, columns, Url, IdKey, fileName, selectionMode) {
        function isNotEmpty(value) {
            return value !== undefined && value !== null && value !== '';
        }
        var totalRowCount;
        const store = new DevExpress.data.CustomStore({
            key: IdKey,
            load(loadOptions) {
                const deferred = $.Deferred();

                const paramNames = [
                    'skip', 'take', 'requireTotalCount', 'requireGroupCount',
                    'sort', 'filter', 'totalSummary', 'group', 'groupSummary',
                ];

                const args = {};
                paramNames.filter((paramName) => isNotEmpty(loadOptions[paramName])).forEach((paramName) => {
                    args[paramName] = JSON.stringify(loadOptions[paramName]);
                });
                console.log(args);
                $.ajax({
                    type: 'POST',
                    url: Url,
                    dataType: 'json',
                    data: args,
                    success(result) {
                        console.log(result)
                        totalRowCount = result.data.totalCount;
                        deferred.resolve(result.data, {
                            totalCount: result.data.totalCount,
                            //summary: result.summary,
                            //groupCount: result.groupCount,
                        });
                    },
                    error() {
                        deferred.reject('Data Loading Error');
                    },
                    timeout: 5000,
                });
                return deferred.promise();
            },
        });

        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        var mode = "multiple";
        if (selectionMode != '' && selectionMode != null && selectionMode != undefined) {
            mode = selectionMode;
        }
        const dataGrid = $(div).dxDataGrid({
            "dataSource": store,
            "columns": columns,
            //"remoteOperations": true,
            "remoteOperations": {
                "sorting": true,
                "paging": true,
                "filtering": true,
                "grouping": true,
                "summary": true
            },
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 12,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [6, 12],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": mode,
                "deferred": false
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const doc = new jsPDF({
                            orientation: 'portrait',
                            unit: 'pt',
                            format: 'a1'
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 20,
                                right: 10,
                                bottom: 20,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {

                                    var img = new Image();
                                    img.src = options.gridCell.value;
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    $(e.cellElement).text('')
                }
            },
            summary: {
                recalculateWhileEditing: true,
                groupItems: [
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '##0',
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '##0',
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '##0',
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '##0',
                    },
                ],
                totalItems: [
                    //{
                    //    column: "Action",
                    //    summaryType: "custom",
                    //    customizeText: function (data) {
                    //        return "Count: " + totalRowCount;
                    //    }
                    //},
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                ]
            }
        }).dxDataGrid('instance');
    },

    editableDxGridbinding: function (div, columns, datasrc, fileName) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": 'multiple',
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            editing: {
                mode: 'batch',
                allowUpdating: true,
                startEditAction: 'click',
                onRowUpdating: function (e) {
                    var editedRow = e.newData;
                    if (editedRow) {
                        var rowIndex = e.rowIndex;
                        empr_AccountOpening.editedRows[rowIndex] = editedRow;
                    }
                }
            },
            showBorders: true,
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            onEditorPreparing: function (e) {
                if (e.dataType === "number") {
                    //e.editorOptions.onInput = function (e) {
                    //    // EnhancedDecimalFieldValidation logic
                    //    var element = e.element;
                    //    var value = $(element).val();
                    //    value = value.replace(/[^0-9.]/g, '');
                    //    value = value.replace(/\.(?=.*\.)/g, '');
                    //    value = value.replace(/(\.\d{3})\d+/g, '$1');
                    //    if (value === '' || parseFloat(value) >= 0) {
                    //        $(element).val(value);
                    //    } else {
                    //        $(element).val($(element).data('lastValid') || '');
                    //    }
                    //    $(element).data('lastValid', $(element).val());
                    //};
                    //e.editorOptions.onKeyDown = function (e) {
                    //    // EnhancedDecimalFieldValidation logic
                    //    var element = e.element;
                    //    var value = $(element).val();
                    //    if (e.key.length === 1 && /[^0-9.]/.test(e.key)) {
                    //        e.preventDefault();
                    //    }
                    //};
                    //e.editorOptions.onKeyPress = function (e) {
                    //    // EnhancedDecimalFieldValidation logic
                    //    var element = e.element;
                    //    var value = $(element).val();
                    //    if (e.key.length === 1 && /[^0-9.]/.test(e.key)) {
                    //        e.preventDefault();
                    //    }
                    //};

                    //e.editorOptions.onKeyDown = function (e) {
                    //    // EnhancedDecimalFieldValidation logic
                    //    var element = e.element;
                    //    var value = $(element).val();
                    //    if (!empr_helper.isValidKey(e)) {
                    //        return false;
                    //    }
                    //};
                    //e.editorOptions.onKeyPress = function (e) {
                    //    // EnhancedDecimalFieldValidation logic
                    //    var element = e.element;
                    //    var value = $(element).val();
                    //    if (!empr_helper.isValidKey(e)) {
                    //        return false;
                    //    }
                    //};

                    e.editorOptions.onInput = function (e) {
                        var element = e.element;
                        var value = $(element).find('input').val();
                        value = value.replace(/[^0-9.]/g, '');
                        value = value.replace(/\.(?=.*\.)/g, '');
                        value = value.replace(/(\.\d{3})\d+/g, '$1');
                        if (value === '' || parseFloat(value) >= 0) {
                            $(element).val(value);
                        } else {
                            $(element).val($(element).data('lastValid') || '');
                        }
                        $(element).data('lastValid', $(element).val());
                        //$(element).val(value);
                    };
                }

                //if (e.parentType === "dataRow") {
                //    if (e.dataField === "credit") {
                //        // Disable editing for editableColumn2
                //        e.editorOptions.disabled = true;
                //    } else if (e.dataField === "debit") {
                //        // Disable editing for editableColumn1
                //        e.editorOptions.disabled = true;
                //    }
                //}

                //if (e.parentType === "dataRow") {
                //    if (e.dataField === "credit") {
                //        // Disable editing for debit column if credit is being edited
                //        e.row.data.debit = 0;
                //        e.editorOptions.disabled = !!e.row.data.debit;
                //    } else if (e.dataField === "debit") {
                //        // Disable editing for credit column if debit is being edited
                //        e.row.data.credit = 0;
                //        e.editorOptions.disabled = !!e.row.data.credit;
                //    }
                //}

                //if (e.parentType === "dataRow" && (e.dataField === "credit" || e.dataField === "debit")) {
                //    e.editorOptions.onValueChanged = function (args) {
                //        if (args.value < 0) {
                //            args.component.option("value", 0); // Set value to 0 if it's negative
                //        }
                //    };
                //}

            },
            onRowUpdating: function (e) {
                //// Track the edited rows
                var editedRow = e.newData;
                if (editedRow.credit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.debit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0 && editedRow.debit > 0) {
                    e.cancel = true;
                    empr_helper.notify("Please enter either Credit or Debit.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0) {
                    if (e.oldData.debit > 0 && editedRow.debit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }

                if (editedRow.debit > 0) {
                    if (e.oldData.credit > 0 && editedRow.credit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }
            },
            onRowUpdated: function (e) {

                if ($('#IsValidate').val() == 'true') {
                    if (e.data != null) {
                        if (empr_AccountOpening.editedRows.filter(r => r.acT_CODE == e.data.acT_CODE).length > 0) {
                            var rowIndex = empr_AccountOpening.editedRows.findIndex(function (row) {
                                return row.acT_CODE === e.data.acT_CODE; // Assuming "ID" is the unique identifier of your row
                            });

                            if (rowIndex !== -1) {
                                empr_AccountOpening.editedRows.splice(rowIndex, 1);
                                empr_AccountOpening.editedRows.push(e.data);
                            }
                        }
                        else {
                            empr_AccountOpening.editedRows.push(e.data);
                        }
                    }
                }
            },
            onToolbarPreparing: function (e) {
                var toolbarItems = e.toolbarOptions.items;
                for (var i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton") {
                        toolbarItems.splice(i, 1);
                        i--;
                    }
                }
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
            //onRowPrepared: function (e) {
            //    if (e.rowType === "data" && e.data.acT_TYPE === "C") {
            //        e.rowElement.addClass("editingNotAllowed");
            //    }
            //},
            //allowUpdating: function (e) {
            //    var rowIndex = e.rowIndex;
            //    var rows = e.component.getVisibleRows();
            //    var isRowDisabled = false;
            //    rows.forEach(function (row) {
            //        if (row.rowIndex === rowIndex && $(row.rowElement[0]).hasClass("editingNotAllowed")) {
            //            isRowDisabled = true;
            //        }
            //    });
            //    if (!!isRowDisabled) {
            //        empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
            //    }
            //    return !isRowDisabled;
            //},
            onEditingStart: function (e) {
                //var rowIndex = e.rowIndex;
                //var rows = e.component.getVisibleRows();
                //var isRowDisabled = false;
                //rows.forEach(function (row) {
                //    if (row.rowIndex === rowIndex && $(row.rowElement[0]).hasClass("editingNotAllowed")) {
                //        isRowDisabled = true;
                //    }
                //});
                //if (isRowDisabled) {
                //    e.cancel = true; 
                //    empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
                //}
                if (e.data.acT_TYPE === "C") {
                    e.cancel = true;
                    empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "GR Code",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "dqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "sqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');



    },

    getTextPixelWidth: function (text, font = "14px Segoe UI") {
        const canvas = document.createElement("canvas");
        const ctx = canvas.getContext("2d");
        ctx.font = font;
        return ctx.measureText(text).width;
    },

    setLookupColumnWidths: function (columns) {
        console.log('Run');

        columns.forEach(col => {
            if (col.lookup && Array.isArray(col.lookup.dataSource)) {

                let displayExpr = col.lookup.displayExpr || "";
                let items = col.lookup.dataSource;

                let maxPx = 0;

                items.forEach(x => {
                    let text = x[displayExpr] ? String(x[displayExpr]).trim() : "";
                    let px = empr_helper.getTextPixelWidth(text);
                    if (px > maxPx)
                        maxPx = px;
                });

                maxPx += 10;

                if (maxPx < 80) maxPx = 80;

                col.width = maxPx;
            }
        });

        return columns;
    },


    editableDxGridbindingForTransactions: function (div, columns, datasrc, fileName, firstColumn, selectionMode, isEditable = true, height = 300) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];

        columns = empr_helper.setLookupColumnWidths(columns);


        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": height,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
                "enabled": false
            },
            "pager": {
                "visible": false,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": false,
                "showInfo": false,
                "showNavigationButtons": false,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            //stateStoring: {
            //    enabled: true,
            //    type: 'custom',

            //    customLoad: function () {
            //        const savedState = localStorage.getItem(fileName);
            //        if (!savedState) return null;

            //        const state = JSON.parse(savedState);

            //        return {
            //            columns: state.columns || []
            //        };
            //    },

            //    customSave: function (state) {
            //        localStorage.setItem(fileName, JSON.stringify({
            //            columns: state.columns.map(c => ({
            //                dataField: c.dataField,
            //                visible: c.visible,
            //                visibleIndex: c.visibleIndex,
            //                groupIndex: c.groupIndex,
            //                width: c.width
            //            }))
            //        }));
            //    }
            //},
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },

            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": false
            },
            "selection": {
                "mode": selectionMode == '' || selectionMode == null || selectionMode == undefined ? '' : selectionMode,
                "showCheckBoxesMode": 'always',
            },
            editing: {
                mode: 'batch',
                allowUpdating: isEditable,
                allowAdding: isEditable,
                allowDeleting: false,
                selectTextOnEditStart: true,
                startEditAction: 'click',
                //saveAllChanges: true,
                useIcons: true,
                newRowPosition: 'first',
            },
            showBorders: true,
            //onFocusedCellChanged: function (e) {
            //    if (e.column.dataField === 'custoM_ACT_CODE') {
            //        setTimeout(function () {
            //            e.component.editCell(e.rowIndex, e.columnIndex);
            //            var cellElement = e.component.getCellElement(e.rowIndex, e.columnIndex);

            //            if (cellElement) {
            //                var $editorInput = $(cellElement).find('.dx-dropdowneditor-input');
            //                if ($editorInput.length > 0) {
            //                    $editorInput.trigger('dxclick');
            //                }
            //            }
            //        }, 1700);
            //    }
            //},
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    var img = new Image();
                                    img.src = options.gridCell.value;
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            //onCellClick: function (e) {
            //    if (e.column.dataField === "insrancE_STATUS") {
            //        let data = e.row.data;
            //        data.insrancE_STATUS = !data.insrancE_STATUS;
            //        e.component.option("dataSource")[e.rowIndex] = data;
            //        e.component.repaintRows([e.rowIndex]);
            //    }
            //},
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
                    value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
                    value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
                    $(e.cellElement).text('')
                }

                /*                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
                    value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
                    value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
                    console.log("In It")
                    // Set the value to undefined to indicate invalid/missing data
                    if (rowData && column.dataField) {
                        rowData[column.dataField] = undefined; // Update the data source
                    }
                }*/

                if ($(e.element).attr('id') == 'StockDetailContainer') {
                    if (Type == 'B') {
                        empr_StockTransfer.IsBLabelValid = true;
                        if (e.rowType == 'data') {
                            var rowIndexesToColor = empr_StockTransfer.FindNonMatchingIndexes(e.component.option('dataSource'));
                            if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
                                empr_StockTransfer.IsBLabelValid = false;
                                e.cellElement.css({ "color": "white", "background-color": "red" });
                            }
                        }
                    }
                }

                if ($(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
                    if (Type == 'B') {
                        empr_StockAdjustment.IsBLabelValid = true;
                        if (e.rowType == 'data') {
                            var rowIndexesToColor = empr_StockAdjustment.FindNonMatchingIndexes(e.component.option('dataSource'));
                            if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
                                empr_StockAdjustment.IsBLabelValid = false;
                                e.cellElement.css({ "color": "white", "background-color": "red" });
                            }
                        }
                    }
                }

                if (div == '#deliveryFeedingDetailContainer') {
                    var totalAmt = e.component.getTotalSummaryValue('AmountTotal');
                    $('#Sel_AMT').val(totalAmt);
                    empr_DeliveryFeeding.calculateBuyerNetAmount();
                    empr_DeliveryFeeding.calculateSellerNetAmount();
                }
            },
            onEditorPreparing: function (e) {
                if (fileName == 'PurchaseOrderr' && empr_helper.dcType == 'SO') {
                    if (e.dataField === 'iteM_CODE') {
                        debugger;
                        //var PARTY_CODE = $("#partyhidden").val();
                        var PARTY_CODE = $('#PARTY_CODE').dxSelectBox('option', 'value');

                        if (!PARTY_CODE) {
                            e.editorOptions.disabled = true;
                            empr_helper.notify("Please select Party first.", 2);
                            //e.editorOptions.placeholder = "Please select Party first";

                            //e.editorOptions.onFocusIn = function () {
                            //    DevExpress.ui.notify(
                            //        "Please select Party first",
                            //        "warning",
                            //        2000
                            //    );
                            //};

                            return;
                        }

                        $(e.editorElement).on('keydown', function (event) {
                            if (event.key === 'Tab' && event.shiftKey) {
                                event.preventDefault();
                                $('#REMARKS').focus();
                            }
                        });
                    }

                }

                if (e.dataField === "coveR_NO" || e.dataField === "inS_DATE") {
                    var rowData = e.row.data;
                    if (rowData && rowData.insrancE_STATUS) {
                        e.editorOptions.disabled = true;
                    }
                }
                if (e.dataField === 'rate' && div === '#deliveryFeedingDetailContainer' && e.row.data.picK_ID > 0) { // Replace 'someCondition' with your condition
                    e.editorOptions.readOnly = true;
                }
                //if (e.dataField === 'iteM_CODE') {
                //    $(e.editorElement).on('keydown', function (event) {
                //        if (event.key === 'Tab' && event.shiftKey) {
                //            event.preventDefault();
                //            if (event.shiftKey) {
                //                $('#REMARKS').focus();
                //            }
                //        }
                //    });
                //}
                if (e.dataType === "number") {
                    e.editorOptions.onInput = function (e) {
                        var element = e.element;
                        var value = $(element).find('input').val();
                        value = value.replace(/[^0-9.]/g, '');
                        value = value.replace(/\.(?=.*\.)/g, '');
                        value = value.replace(/(\.\d{3})\d+/g, '$1');
                        if (value === '' || parseFloat(value) >= 0) {
                            $(element).val(value);
                        } else {
                            $(element).val($(element).data('lastValid') || '');
                        }
                        $(element).data('lastValid', $(element).val());
                    };
                }
            },
            onRowUpdating: function (e) {
                //// Track the edited rows
                var editedRow = e.newData;
                if (editedRow.credit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.debit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0 && editedRow.debit > 0) {
                    e.cancel = true;
                    empr_helper.notify("Please enter either Credit or Debit.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0) {
                    if (e.oldData.debit > 0 && editedRow.debit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }

                if (editedRow.debit > 0) {
                    if (e.oldData.credit > 0 && editedRow.credit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }
            },
            onRowUpdated: function (e) {

                if ($('#IsValidate').val() == 'true') {
                    if (e.data != null) {
                        if (empr_AccountOpening.editedRows.filter(r => r.acT_CODE == e.data.acT_CODE).length > 0) {
                            var rowIndex = empr_AccountOpening.editedRows.findIndex(function (row) {
                                return row.acT_CODE === e.data.acT_CODE; // Assuming "ID" is the unique identifier of your row
                            });

                            if (rowIndex !== -1) {
                                empr_AccountOpening.editedRows.splice(rowIndex, 1);
                                empr_AccountOpening.editedRows.push(e.data);
                            }
                        }
                        else {
                            empr_AccountOpening.editedRows.push(e.data);
                        }
                    }
                }
            },
            //onToolbarPreparing: function (e) {
            //    //debugger;
            //    var toolbarItems = e.toolbarOptions.items;
            //    for (var i = 0; i < toolbarItems.length; i++) {
            //        if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton" || toolbarItems[i].name === "addRowButton" || toolbarItems[i].name === "deleteRowButton") {
            //            toolbarItems.splice(i, 1);
            //            i--;
            //        }
            //    }
            //},
            onToolbarPreparing: function (e) {
                var toolbarItems = e.toolbarOptions.items;
                for (var i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton" || toolbarItems[i].name === "addRowButton" || toolbarItems[i].name === "deleteRowButton") {
                        toolbarItems.splice(i, 1);
                        i--;
                    }
                }
                toolbarItems.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        hint: 'Refresh',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            //localStorage.setItem(fileName, '');
                            location.reload();
                        }
                    }
                });
            },
            onEditingStart: function (e) {
                if (e.data.acT_TYPE === "C") {
                    e.cancel = true;
                    empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
                }

                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
                    if (e.column.dataField == 'iteM_CODE') {
                        if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                            e.cancel = true;
                            //empr_helper.notify("You are not allowed to change this item", 2);
                        }
                    }

                    //if (e.column.dataField == 'rate') {
                    //    if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                    //        e.cancel = true;
                    //        //empr_helper.notify("You are not allowed to change the rate of this item", 2);
                    //    }
                    //}
                }
            },
            onKeyDown: function (e) {
                if ($(e.element).attr('id') == 'detailContainer') {
                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        console.log(columnIndex);
                        if (columnIndex === 5) {
                            e.event.preventDefault();
                            $('#SETT').focus();
                        }
                    }
                };
                if ($(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        if (columnIndex === 10) {
                            e.event.preventDefault();
                            $('#BrSeller').focus();
                        }
                    }
                };
                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");
                    if (focusedRowIndex !== undefined) {
                        var $focusedCell = $(dataGrid.getCellElement(focusedRowIndex, "Action")); // Assuming "Action" is the dataField of your action column
                        // Handle Enter key
                        if (keyCode === 13) {
                            // Check if the focused cell contains the specified class
                            console.log("Enter is pressed");

                            // To Save Value Of Current Cell
                            var nextColumnIndex = dataGrid.option("focusedColumnIndex") + 1;
                            if (nextColumnIndex < dataGrid.columnCount()) {
                                var nextCell = dataGrid.getCellElement(dataGrid.option("focusedRowIndex"), nextColumnIndex);
                                dataGrid.focus(nextCell);
                            }

                            var $actionButton = $focusedCell.find(".Add"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, firstColumn);
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                                // Set focus to the desired column in edit mode
                                dataGrid.editCell(0, firstColumn);
                            }, 500);
                        }
                        if (keyCode === 13) {
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, firstColumn);
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                                dataGrid.editCell(0, firstColumn);
                            }, 500);
                        }
                        //else if (e.event.ctrlKey && keyCode === 67) {
                        //    console.log("Ctrl+C is pressed");
                        //    var $actionButton = $focusedCell.find(".Clone"); // Replace "Add" with your desired class name
                        //    $($actionButton).click();
                        //    setTimeout(function () {
                        //        var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                        //        $(nextElement).click();
                        //        dataGrid.focus(nextElement);
                        //    }, 1500);
                        //}
                        else if (keyCode === 46) {
                            // Ctrl+C logic
                            console.log("Delete is pressed");
                            var $actionButton = $focusedCell.find(".Delete"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                            }, 1500);
                        }
                    }

                }

                if ($(e.element).attr('id') == 'SodaPickGridContainer' || $(e.element).attr('id') == 'BarcodePickGridContainer') {

                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");

                    // Check if a row is focused
                    if (focusedRowIndex !== undefined) {
                        // Handle Enter key
                        if (keyCode === 13) {
                            // Check if the focused cell contains the specified class
                            console.log("Enter is pressed");
                            dataGrid.selectRowsByIndexes([focusedRowIndex]);
                        }

                        //if (e.event.key == 'Control' && (keyCode == 65 || keyCode == 97)) {
                        if (e.event.ctrlKey && e.event.which === 65) {
                            console.log('CTRL+A is pressed');
                            $('#BtnAddSodaToDelivery').click();
                        }
                    }
                }
            },
            summary: {
                recalculateWhileEditing: true,
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        },
                        //name: "ActionCount"
                    },
                    {
                        column: "GR Code",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "sqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bamt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "samt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    //{
                    //    column: "rate",
                    //    summaryType: "sum",
                    //    displayFormat: "Total: {0}"
                    //},
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "AmountTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "adV_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "bR_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "bR_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "NetTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "buyer",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.buyerAverage > 0) {
                                return "Average : " + empr_helper.buyerAverage.toLocaleString('en-US');
                            }
                        }
                    },
                    {
                        column: "seller",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.sellerAverage > 0) {
                                return "Average : " + empr_helper.sellerAverage.toLocaleString('en-US');
                            }
                        }
                    }
                ],
            }
        }).dxDataGrid('instance');
    },

    editableDxGridbindingForTransactionsVouchers: function (div, columns, datasrc, fileName, firstColumn, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];

        //columns = empr_helper.setLookupColumnWidths(columns);

        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 300,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
                "enabled": false
            },
            "pager": {
                "visible": false,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": false,
                "showInfo": false,
                "showNavigationButtons": false,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            //"export": {
            //    "enabled": false
            //},
            "export": {
                "enabled": true,
                "formats": ['excel'],
                allowExportSelectedData: false,
            },
            "selection": {
                "mode": selectionMode == '' || selectionMode == null || selectionMode == undefined ? '' : selectionMode,
                "showCheckBoxesMode": 'always',
            },
            editing: {
                mode: 'batch',
                allowUpdating: true,
                allowAdding: true,
                allowDeleting: false,
                selectTextOnEditStart: true,
                startEditAction: 'click',
                //saveAllChanges: true,
                useIcons: true,
                newRowPosition: 'first',
            },
            showBorders: true,
            //onFocusedCellChanged: function (e) {
            //    if (e.column.dataField === 'custoM_ACT_CODE') {
            //        setTimeout(function () {
            //            e.component.editCell(e.rowIndex, e.columnIndex);
            //            var cellElement = e.component.getCellElement(e.rowIndex, e.columnIndex);

            //            if (cellElement) {
            //                var $editorInput = $(cellElement).find('.dx-dropdowneditor-input');
            //                if ($editorInput.length > 0) {
            //                    $editorInput.trigger('dxclick');
            //                }
            //            }
            //        }, 1700);
            //    }
            //},
            onExporting(e) {
                debugger;

                var fileNewName = fileName +'_'+ empr_helper.invoiceNo;
                // ---------- EXCEL EXPORT ----------
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet(fileName);

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                    customizeCell: function (options) {
                        const { gridCell, excelCell } = options;
                        const targetFields = [
                            'debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc',
                            'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt',
                            'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales',
                            'cash', 'cardType', 'party'
                        ];

                        if (gridCell.rowType !== 'header' && targetFields.includes(gridCell.column.dataField)) {
                            if (typeof gridCell.value === 'number') {
                                excelCell.numFmt = '#,##0';
                            }
                        }
                    }
                }).then(() => {
                    workbook.xlsx.writeBuffer().then(buffer => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileNewName + '.xlsx');
                    });
                });

            },
            onCellClick: function (e) {
                if (e.column.dataField === "insrancE_STATUS") {
                    let data = e.row.data;
                    data.insrancE_STATUS = !data.insrancE_STATUS;
                    e.component.option("dataSource")[e.rowIndex] = data;
                    e.component.repaintRows([e.rowIndex]);
                }
            },
            onRowPrepared: function (e) {
                if (e.rowType === "data") {
                    var cc_status = parseFloat((e.data.cosT_CENTER_STATUS || "").toString().trim());
                    var ko_status = parseFloat((e.data.knockofF_STATUS || "").toString().trim());

                    if (cc_status === 1 || ko_status === 1) {
                        // Row background-color
                        e.rowElement.css("background-color", "red");
                        // Cells background-color
                        e.rowElement.find('td').css("background-color", "#ED7777");
                    }
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
                    value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
                    value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
                    $(e.cellElement).text('')
                }

                /*                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
                    value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
                    value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
                    console.log("In It")
                    // Set the value to undefined to indicate invalid/missing data
                    if (rowData && column.dataField) {
                        rowData[column.dataField] = undefined; // Update the data source
                    }
                }*/

                if ($(e.element).attr('id') == 'StockDetailContainer') {
                    if (Type == 'B') {
                        empr_StockTransfer.IsBLabelValid = true;
                        if (e.rowType == 'data') {
                            var rowIndexesToColor = empr_StockTransfer.FindNonMatchingIndexes(e.component.option('dataSource'));
                            if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
                                empr_StockTransfer.IsBLabelValid = false;
                                e.cellElement.css({ "color": "white", "background-color": "red" });
                            }
                        }
                    }
                }

                if ($(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
                    if (Type == 'B') {
                        empr_StockAdjustment.IsBLabelValid = true;
                        if (e.rowType == 'data') {
                            var rowIndexesToColor = empr_StockAdjustment.FindNonMatchingIndexes(e.component.option('dataSource'));
                            if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
                                empr_StockAdjustment.IsBLabelValid = false;
                                e.cellElement.css({ "color": "white", "background-color": "red" });
                            }
                        }
                    }
                }

                if (div == '#deliveryFeedingDetailContainer') {
                    var totalAmt = e.component.getTotalSummaryValue('AmountTotal');
                    $('#Sel_AMT').val(totalAmt);
                    empr_DeliveryFeeding.calculateBuyerNetAmount();
                    empr_DeliveryFeeding.calculateSellerNetAmount();
                }

                if (div == '#fdeliveryFeedingDetailContainer') {
                    var totalAmt = e.component.getTotalSummaryValue('AmountTotal');
                    $('#Sel_AMT').val(totalAmt);
                    empr_FDeliveryFeeding.calculateBuyerNetAmount();
                    empr_FDeliveryFeeding.calculateSellerNetAmount();
                }
            },
            onEditorPreparing: function (e) {

                if (fileName == 'PurchaseBill' && (empr_helper.dcType == 'SB' || empr_helper.dcType == 'SO' || empr_helper.dcType == 'DC')) {
                    if (e.dataField === 'iteM_CODE') {
                        debugger;
                        var PARTY_CODE = $("#partyhidden").val();

                        if (!PARTY_CODE) {
                            e.editorOptions.disabled = true;
                            empr_helper.notify("Please select Party first.", 2);
                            //e.editorOptions.placeholder = "Please select Party first";

                            //e.editorOptions.onFocusIn = function () {
                            //    DevExpress.ui.notify(
                            //        "Please select Party first",
                            //        "warning",
                            //        2000
                            //    );
                            //};

                            return;
                        }

                        $(e.editorElement).on('keydown', function (event) {
                            if (event.key === 'Tab' && event.shiftKey) {
                                event.preventDefault();
                                $('#REMARKS').focus();
                            }
                        });
                    }

                }


                if (e.dataField === "coveR_NO" || e.dataField === "inS_DATE") {
                    var rowData = e.row.data;
                    if (rowData && rowData.insrancE_STATUS) {
                        e.editorOptions.disabled = true;
                    }
                }
                if (e.dataField === 'rate' && div === '#deliveryFeedingDetailContainer' && (e.row.data.picK_ID > 0 && Permissions != "Admin")) { // Replace 'someCondition' with your condition
                    e.editorOptions.readOnly = true;
                }
                //if (e.dataField === 'diS_TIME') {
                //    e.editorOptions.valueFormat = 'HH:mm';
                //}
                if (e.dataField === 'iteM_CODE') {
                    $(e.editorElement).on('keydown', function (event) {
                        if (event.key === 'Tab' && event.shiftKey) {
                            event.preventDefault();
                            if (event.shiftKey) {
                                $('#REMARKS').focus();
                            }
                        }
                    });
                }
                if (e.dataType === "number") {
                    e.editorOptions.onInput = function (e) {
                        var element = e.element;
                        var value = $(element).find('input').val();
                        value = value.replace(/[^0-9.]/g, '');
                        value = value.replace(/\.(?=.*\.)/g, '');
                        value = value.replace(/(\.\d{3})\d+/g, '$1');
                        if (value === '' || parseFloat(value) >= 0) {
                            $(element).val(value);
                        } else {
                            $(element).val($(element).data('lastValid') || '');
                        }
                        $(element).data('lastValid', $(element).val());
                    };
                }
            },
            onRowUpdating: function (e) {
                //// Track the edited rows
                var editedRow = e.newData;
                if (editedRow.credit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.debit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0 && editedRow.debit > 0) {
                    e.cancel = true;
                    empr_helper.notify("Please enter either Credit or Debit.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0) {
                    if (e.oldData.debit > 0 && editedRow.debit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }

                if (editedRow.debit > 0) {
                    if (e.oldData.credit > 0 && editedRow.credit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }
            },
            onRowUpdated: function (e) {

                if ($('#IsValidate').val() == 'true') {
                    if (e.data != null) {
                        if (empr_AccountOpening.editedRows.filter(r => r.acT_CODE == e.data.acT_CODE).length > 0) {
                            var rowIndex = empr_AccountOpening.editedRows.findIndex(function (row) {
                                return row.acT_CODE === e.data.acT_CODE; // Assuming "ID" is the unique identifier of your row
                            });

                            if (rowIndex !== -1) {
                                empr_AccountOpening.editedRows.splice(rowIndex, 1);
                                empr_AccountOpening.editedRows.push(e.data);
                            }
                        }
                        else {
                            empr_AccountOpening.editedRows.push(e.data);
                        }
                    }
                }
            },
            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            //localStorage.setItem(fileName, '');
                            location.reload();
                        }
                    }
                });
                var toolbarItems = e.toolbarOptions.items;
                for (var i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton" || toolbarItems[i].name === "addRowButton" || toolbarItems[i].name === "deleteRowButton") {
                        toolbarItems.splice(i, 1);
                        i--;
                    }
                }
            },
            onEditingStart: function (e) {
                if (e.data.acT_TYPE === "C") {
                    e.cancel = true;
                    empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
                }

                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
                    if (e.column.dataField == 'iteM_CODE') {
                        if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                            e.cancel = true;
                            //empr_helper.notify("You are not allowed to change this item", 2);
                        }
                    }

                    //if (e.column.dataField == 'rate') {
                    //    if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                    //        e.cancel = true;
                    //        //empr_helper.notify("You are not allowed to change the rate of this item", 2);
                    //    }
                    //}
                }
            },
            onKeyDown: function (e) {
                if ($(e.element).attr('id') == 'detailContainer') {
                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        console.log(columnIndex);
                        if (columnIndex === 5) {
                            e.event.preventDefault();
                            $('#SETT').focus();
                        }
                    }
                };
                if ($(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        if (columnIndex === 10) {
                            e.event.preventDefault();
                            $('#BrSeller').focus();
                        }
                    }
                };
                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");
                    if (focusedRowIndex !== undefined) {
                        var $focusedCell = $(dataGrid.getCellElement(focusedRowIndex, "Action")); // Assuming "Action" is the dataField of your action column
                        // Handle Enter key
                        if (keyCode === 13) {
                            // Check if the focused cell contains the specified class
                            console.log("Enter is pressed");

                            // To Save Value Of Current Cell
                            var nextColumnIndex = dataGrid.option("focusedColumnIndex") + 1;
                            if (nextColumnIndex < dataGrid.columnCount()) {
                                var nextCell = dataGrid.getCellElement(dataGrid.option("focusedRowIndex"), nextColumnIndex);
                                dataGrid.focus(nextCell);
                            }

                            var $actionButton = $focusedCell.find(".Add"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, firstColumn);
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                                // Set focus to the desired column in edit mode
                                dataGrid.editCell(0, firstColumn);
                            }, 500);
                        }
                        if (keyCode === 13) {
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, firstColumn);
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                                dataGrid.editCell(0, firstColumn);
                            }, 500);
                        }
                        //else if (e.event.ctrlKey && keyCode === 67) {
                        //    console.log("Ctrl+C is pressed");
                        //    var $actionButton = $focusedCell.find(".Clone"); // Replace "Add" with your desired class name
                        //    $($actionButton).click();
                        //    setTimeout(function () {
                        //        var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                        //        $(nextElement).click();
                        //        dataGrid.focus(nextElement);
                        //    }, 1500);
                        //}
                        //else if (keyCode === 46) {
                        //    // Ctrl+C logic
                        //    console.log("Delete is pressed");
                        //    var $actionButton = $focusedCell.find(".Delete"); // Replace "Add" with your desired class name
                        //    $($actionButton).click();
                        //    setTimeout(function () {
                        //        var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                        //        $(nextElement).click();
                        //        dataGrid.focus(nextElement);
                        //    }, 1500);
                        //}
                    }

                }

                if ($(e.element).attr('id') == 'SodaPickGridContainer' || $(e.element).attr('id') == 'BarcodePickGridContainer') {

                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");

                    // Check if a row is focused
                    if (focusedRowIndex !== undefined) {
                        // Handle Enter key
                        if (keyCode === 13) {
                            // Check if the focused cell contains the specified class
                            console.log("Enter is pressed");
                            dataGrid.selectRowsByIndexes([focusedRowIndex]);
                        }

                        //if (e.event.key == 'Control' && (keyCode == 65 || keyCode == 97)) {
                        if (e.event.ctrlKey && e.event.which === 65) {
                            console.log('CTRL+A is pressed');
                            $('#BtnAddSodaToDelivery').click();
                        }
                    }
                }
            },
            onCellValueChanged: function (e) {
                debugger;
                if (e.dataField === "net_amt") {
                    console.log("net_amt changed:", e.value);
                }
            },
            summary: {
                recalculateWhileEditing: true,
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        },
                        name: "ActionCount"
                    },
                    {
                        column: "GR Code",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "totaL_PACK",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "sqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bamt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "samt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    //{
                    //    column: "rate",
                    //    summaryType: "sum",
                    //    displayFormat: "Total: {0}"
                    //},
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "AmountTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "adV_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "bR_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "bR_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "NetTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "comM_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "buyer",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.buyerAverage > 0) {
                                return "Average : " + empr_helper.buyerAverage.toLocaleString('en-US');
                            }
                        }
                    },
                    {
                        column: "seller",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.sellerAverage > 0) {
                                return "Average : " + empr_helper.sellerAverage.toLocaleString('en-US');
                            }
                        }
                    }
                ],
            }
        }).dxDataGrid('instance');
    },

    dxGridbindingForStockReceive: function (div, columns, datasrc, fileName, firstColumn, selectionMode) {
        //const columnChooserModes = [{
        //    "key": 'dragAndDrop',
        //    "name": 'Drag and drop',
        //}, {
        //    "key": 'select',
        //    "name": 'Select',
        //}];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 300,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            //"groupPanel": { visible: true },
            //"columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
                "enabled": false
            },
            //"pager": {
            //    "visible": false,
            //    "allowedPageSizes": [100, 200, 300, 'all'],
            //    "showPageSizeSelector": false,
            //    "showInfo": false,
            //    "showNavigationButtons": false,
            //},
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            //"filterRow": {
            //    "visible": false,
            //    "applyFilter": 'auto',
            //},
            //"stateStoring": {
            //    //"enabled": true,
            //    //"type": 'localStorage',
            //    //"storageKey": fileName
            //    enabled: true,
            //    type: 'custom',
            //    customLoad: function () {
            //        const savedState = localStorage.getItem(fileName);
            //        if (savedState) {
            //            const state = JSON.parse(savedState);
            //            return {
            //                columns: state.columns || []
            //            };
            //        }
            //        return null;
            //    },
            //    customSave: function (state) {
            //        const stateToSave = {
            //            columns: state.columns.map(col => ({
            //                dataField: col.dataField,
            //                visible: col.visible,
            //                visibleIndex: col.visibleIndex,
            //                groupIndex: col.groupIndex,
            //                width: col.width
            //            }))
            //        };
            //        localStorage.setItem(fileName, JSON.stringify(stateToSave));
            //    }
            //},
            //"columnChooser": {
            //    "enabled": true,
            //    "mode": columnChooserModes[1].key,
            //    "position": {
            //        "my": 'right top',
            //        "at": 'right bottom',
            //        "of": '.dx-datagrid-column-chooser-button',
            //    },
            //    "search": {
            //        "enabled": true,
            //        "editorOptions": { placeholder: 'Search column' },
            //    },
            //    "selection": {
            //        "recursive": true,
            //        "selectByClick": true,
            //        "allowSelectAll": true,
            //    },
            //},
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            //"export": {
            //    "enabled": false
            //},
            //"selection": {
            //    "mode": selectionMode == '' || selectionMode == null || selectionMode == undefined ? '' : selectionMode,
            //    "showCheckBoxesMode": 'always',
            //},
            //editing: {
            //    mode: 'batch',
            //    allowUpdating: true,
            //    allowAdding: true,
            //    allowDeleting: false,
            //    selectTextOnEditStart: true,
            //    startEditAction: 'click',
            //    //saveAllChanges: true,
            //    useIcons: true,
            //    newRowPosition: 'first',
            //},
            showBorders: true,
            //onExporting(e) {
            //    if (e.format === 'pdf') {
            //        if (e.component.getSelectedRowsData().length > 0) {
            //            //const doc = new jsPDF({ orientation: 'landscape' });
            //            const doc = new jsPDF({
            //                orientation: 'portrait', // or 'landscape' depending on your requirement
            //                unit: 'pt', // units of measurement: points
            //                format: 'a1' // set the page size to A3
            //            });

            //            DevExpress.pdfExporter.exportDataGrid({
            //                "jsPDFDocument": doc,
            //                "component": e.component,
            //                indent: 5,
            //                selectedRowsOnly: true,
            //                margin: {
            //                    top: 10,
            //                    right: 10,
            //                    bottom: 10,
            //                    left: 10,
            //                },
            //                topLeft: { x: 5, y: 5 },
            //                onExporting: function (e) {
            //                    e.component.beginUpdate();
            //                },
            //                onExported: function (e) {
            //                    e.component.endUpdate();
            //                },
            //                customizeCell: function (options) {
            //                    if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
            //                        var img = new Image();
            //                        img.src = options.gridCell.value;
            //                        doc.addImage(img, 'PNG', 5, 5, 50, 50);
            //                        return false;
            //                    }
            //                }
            //            }).then(function () {
            //                doc.save(fileName + '.pdf');
            //            });
            //        } else {
            //            empr_helper.notify('Please first the select rows.', 2);
            //        }
            //    }
            //    else {
            //        if (e.component.getSelectedRowsData().length > 0) {
            //            const workbook = new ExcelJS.Workbook();
            //            const worksheet = workbook.addWorksheet(fileName);

            //            DevExpress.excelExporter.exportDataGrid({
            //                component: e.component,
            //                worksheet,
            //                autoFilterEnabled: true,
            //                selectedRowsOnly: true
            //            }).then(() => {
            //                workbook.xlsx.writeBuffer().then((buffer) => {
            //                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
            //                });
            //            });
            //        } else {
            //            empr_helper.notify('Please first the select rows.', 2);
            //        }
            //    }
            //},
            //onCellClick: function (e) {
            //    if (e.column.dataField === "insrancE_STATUS") {
            //        let data = e.row.data;
            //        data.insrancE_STATUS = !data.insrancE_STATUS;
            //        e.component.option("dataSource")[e.rowIndex] = data;
            //        e.component.repaintRows([e.rowIndex]);
            //    }
            //},
            //onRowPrepared: function (e) {
            //    if (e.rowType === "data") {
            //        var cc_status = parseFloat((e.data.cosT_CENTER_STATUS || "").toString().trim());

            //        if (cc_status === 1) {
            //            // Row background-color
            //            e.rowElement.css("background-color", "red");
            //            // Cells background-color
            //            e.rowElement.find('td').css("background-color", "#ED7777");
            //        }
            //    }
            //},
            //onCellPrepared(e) {
            //    const column = e.column;
            //    const rowData = e.data;
            //    const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
            //    if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
            //        value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
            //        value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
            //        $(e.cellElement).text('')
            //    }

            //    /*                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
            //        value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
            //        value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
            //        console.log("In It")
            //        // Set the value to undefined to indicate invalid/missing data
            //        if (rowData && column.dataField) {
            //            rowData[column.dataField] = undefined; // Update the data source
            //        }
            //    }*/

            //    if ($(e.element).attr('id') == 'StockDetailContainer') {
            //        if (Type == 'B') {
            //            empr_StockTransfer.IsBLabelValid = true;
            //            if (e.rowType == 'data') {
            //                var rowIndexesToColor = empr_StockTransfer.FindNonMatchingIndexes(e.component.option('dataSource'));
            //                if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
            //                    empr_StockTransfer.IsBLabelValid = false;
            //                    e.cellElement.css({ "color": "white", "background-color": "red" });
            //                }
            //            }
            //        }
            //    }

            //    if ($(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
            //        if (Type == 'B') {
            //            empr_StockAdjustment.IsBLabelValid = true;
            //            if (e.rowType == 'data') {
            //                var rowIndexesToColor = empr_StockAdjustment.FindNonMatchingIndexes(e.component.option('dataSource'));
            //                if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
            //                    empr_StockAdjustment.IsBLabelValid = false;
            //                    e.cellElement.css({ "color": "white", "background-color": "red" });
            //                }
            //            }
            //        }
            //    }

            //    if (div == '#deliveryFeedingDetailContainer') {
            //        var totalAmt = e.component.getTotalSummaryValue('AmountTotal');
            //        $('#Sel_AMT').val(totalAmt);
            //        empr_DeliveryFeeding.calculateBuyerNetAmount();
            //        empr_DeliveryFeeding.calculateSellerNetAmount();
            //    }
            //},
            //onEditorPreparing: function (e) {
            //    if (e.dataField === "coveR_NO" || e.dataField === "inS_DATE") {
            //        var rowData = e.row.data;
            //        if (rowData && rowData.insrancE_STATUS) {
            //            e.editorOptions.disabled = true;
            //        }
            //    }
            //    if (e.dataField === 'rate' && div === '#deliveryFeedingDetailContainer' && (e.row.data.picK_ID > 0 && Permissions != "Admin")) { // Replace 'someCondition' with your condition
            //        e.editorOptions.readOnly = true;
            //    }
            //    //if (e.dataField === 'diS_TIME') {
            //    //    e.editorOptions.valueFormat = 'HH:mm';
            //    //}
            //    if (e.dataField === 'iteM_CODE') {
            //        $(e.editorElement).on('keydown', function (event) {
            //            if (event.key === 'Tab' && event.shiftKey) {
            //                event.preventDefault();
            //                if (event.shiftKey) {
            //                    $('#REMARKS').focus();
            //                }
            //            }
            //        });
            //    }
            //    if (e.dataType === "number") {
            //        e.editorOptions.onInput = function (e) {
            //            var element = e.element;
            //            var value = $(element).find('input').val();
            //            value = value.replace(/[^0-9.]/g, '');
            //            value = value.replace(/\.(?=.*\.)/g, '');
            //            value = value.replace(/(\.\d{3})\d+/g, '$1');
            //            if (value === '' || parseFloat(value) >= 0) {
            //                $(element).val(value);
            //            } else {
            //                $(element).val($(element).data('lastValid') || '');
            //            }
            //            $(element).data('lastValid', $(element).val());
            //        };
            //    }
            //},
            //onRowUpdating: function (e) {
            //    //// Track the edited rows
            //    var editedRow = e.newData;
            //    if (editedRow.credit < 0) {
            //        e.cancel = true;
            //        empr_helper.notify("Amount must be greater than 0.", 2);
            //        $('#IsValidate').val('false');
            //    }

            //    if (editedRow.debit < 0) {
            //        e.cancel = true;
            //        empr_helper.notify("Amount must be greater than 0.", 2);
            //        $('#IsValidate').val('false');
            //    }

            //    if (editedRow.credit > 0 && editedRow.debit > 0) {
            //        e.cancel = true;
            //        empr_helper.notify("Please enter either Credit or Debit.", 2);
            //        $('#IsValidate').val('false');
            //    }

            //    if (editedRow.credit > 0) {
            //        if (e.oldData.debit > 0 && editedRow.debit != 0) {
            //            e.cancel = true;
            //            empr_helper.notify("Please enter either Credit or Debit.", 2);
            //            $('#IsValidate').val('false');
            //        }
            //    }

            //    if (editedRow.debit > 0) {
            //        if (e.oldData.credit > 0 && editedRow.credit != 0) {
            //            e.cancel = true;
            //            empr_helper.notify("Please enter either Credit or Debit.", 2);
            //            $('#IsValidate').val('false');
            //        }
            //    }
            //},
            //onRowUpdated: function (e) {

            //    if ($('#IsValidate').val() == 'true') {
            //        if (e.data != null) {
            //            if (empr_AccountOpening.editedRows.filter(r => r.acT_CODE == e.data.acT_CODE).length > 0) {
            //                var rowIndex = empr_AccountOpening.editedRows.findIndex(function (row) {
            //                    return row.acT_CODE === e.data.acT_CODE; // Assuming "ID" is the unique identifier of your row
            //                });

            //                if (rowIndex !== -1) {
            //                    empr_AccountOpening.editedRows.splice(rowIndex, 1);
            //                    empr_AccountOpening.editedRows.push(e.data);
            //                }
            //            }
            //            else {
            //                empr_AccountOpening.editedRows.push(e.data);
            //            }
            //        }
            //    }
            //},
            //onToolbarPreparing: function (e) {
            //    e.toolbarOptions.items.unshift({
            //        location: 'after',
            //        widget: 'dxButton',
            //        options: {
            //            icon: 'refresh',
            //            text: '',
            //            onClick: function () {
            //                localStorage.removeItem(fileName);
            //                location.reload();
            //            }
            //        }
            //    });
            //    var toolbarItems = e.toolbarOptions.items;
            //    for (var i = 0; i < toolbarItems.length; i++) {
            //        if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton" || toolbarItems[i].name === "addRowButton" || toolbarItems[i].name === "deleteRowButton") {
            //            toolbarItems.splice(i, 1);
            //            i--;
            //        }
            //    }
            //},
            //onEditingStart: function (e) {
            //    if (e.data.acT_TYPE === "C") {
            //        e.cancel = true;
            //        empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
            //    }

            //    if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
            //        if (e.column.dataField == 'iteM_CODE') {
            //            if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
            //                e.cancel = true;
            //                //empr_helper.notify("You are not allowed to change this item", 2);
            //            }
            //        }

            //        //if (e.column.dataField == 'rate') {
            //        //    if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
            //        //        e.cancel = true;
            //        //        //empr_helper.notify("You are not allowed to change the rate of this item", 2);
            //        //    }
            //        //}
            //    }
            //},
            //onKeyDown: function (e) {
            //    if ($(e.element).attr('id') == 'detailContainer') {
            //        if (e.event.key === "Tab") {
            //            var columnIndex = dataGrid.option("focusedColumnIndex");
            //            console.log(columnIndex);
            //            if (columnIndex === 5) {
            //                e.event.preventDefault();
            //                $('#SETT').focus();
            //            }
            //        }
            //    };
            //    if ($(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
            //        if (e.event.key === "Tab") {
            //            var columnIndex = dataGrid.option("focusedColumnIndex");
            //            if (columnIndex === 10) {
            //                e.event.preventDefault();
            //                $('#BrSeller').focus();
            //            }
            //        }
            //    };
            //    if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
            //        var keyCode = e.event.keyCode;
            //        var focusedRowIndex = dataGrid.option("focusedRowIndex");
            //        if (focusedRowIndex !== undefined) {
            //            var $focusedCell = $(dataGrid.getCellElement(focusedRowIndex, "Action")); // Assuming "Action" is the dataField of your action column
            //            // Handle Enter key
            //            if (keyCode === 13) {
            //                // Check if the focused cell contains the specified class
            //                console.log("Enter is pressed");

            //                // To Save Value Of Current Cell
            //                var nextColumnIndex = dataGrid.option("focusedColumnIndex") + 1;
            //                if (nextColumnIndex < dataGrid.columnCount()) {
            //                    var nextCell = dataGrid.getCellElement(dataGrid.option("focusedRowIndex"), nextColumnIndex);
            //                    dataGrid.focus(nextCell);
            //                }

            //                var $actionButton = $focusedCell.find(".Add"); // Replace "Add" with your desired class name
            //                $($actionButton).click();
            //                setTimeout(function () {
            //                    var nextElement = dataGrid.getCellElement(0, firstColumn);
            //                    $(nextElement).click();
            //                    dataGrid.focus(nextElement);
            //                    // Set focus to the desired column in edit mode
            //                    dataGrid.editCell(0, firstColumn);
            //                }, 500);
            //            }
            //            if (keyCode === 13) {
            //                setTimeout(function () {
            //                    var nextElement = dataGrid.getCellElement(0, firstColumn);
            //                    $(nextElement).click();
            //                    dataGrid.focus(nextElement);
            //                    dataGrid.editCell(0, firstColumn);
            //                }, 500);
            //            }
            //            //else if (e.event.ctrlKey && keyCode === 67) {
            //            //    console.log("Ctrl+C is pressed");
            //            //    var $actionButton = $focusedCell.find(".Clone"); // Replace "Add" with your desired class name
            //            //    $($actionButton).click();
            //            //    setTimeout(function () {
            //            //        var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
            //            //        $(nextElement).click();
            //            //        dataGrid.focus(nextElement);
            //            //    }, 1500);
            //            //}
            //            else if (keyCode === 46) {
            //                // Ctrl+C logic
            //                console.log("Delete is pressed");
            //                var $actionButton = $focusedCell.find(".Delete"); // Replace "Add" with your desired class name
            //                $($actionButton).click();
            //                setTimeout(function () {
            //                    var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
            //                    $(nextElement).click();
            //                    dataGrid.focus(nextElement);
            //                }, 1500);
            //            }
            //        }

            //    }

            //    if ($(e.element).attr('id') == 'SodaPickGridContainer' || $(e.element).attr('id') == 'BarcodePickGridContainer') {

            //        var keyCode = e.event.keyCode;
            //        var focusedRowIndex = dataGrid.option("focusedRowIndex");

            //        // Check if a row is focused
            //        if (focusedRowIndex !== undefined) {
            //            // Handle Enter key
            //            if (keyCode === 13) {
            //                // Check if the focused cell contains the specified class
            //                console.log("Enter is pressed");
            //                dataGrid.selectRowsByIndexes([focusedRowIndex]);
            //            }

            //            //if (e.event.key == 'Control' && (keyCode == 65 || keyCode == 97)) {
            //            if (e.event.ctrlKey && e.event.which === 65) {
            //                console.log('CTRL+A is pressed');
            //                $('#BtnAddSodaToDelivery').click();
            //            }
            //        }
            //    }
            //},
            summary: {
                recalculateWhileEditing: true,
                totalItems: [
                    {
                        column: "GR Code",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "sqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bamt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "samt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    //{
                    //    column: "rate",
                    //    summaryType: "sum",
                    //    displayFormat: "Total: {0}"
                    //},
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "AmountTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "adV_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "bR_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "bR_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "NetTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "buyer",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.buyerAverage > 0) {
                                return "Average : " + empr_helper.buyerAverage.toLocaleString('en-US');
                            }
                        }
                    },
                    {
                        column: "seller",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.sellerAverage > 0) {
                                return "Average : " + empr_helper.sellerAverage.toLocaleString('en-US');
                            }
                        }
                    }
                ],
            }
        }).dxDataGrid('instance');
    },

    editableDxGridbindingForPurchaseSale: function (div, columns, datasrc, fileName, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 300,
            //"allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
                "enabled": false
            },
            "pager": {
                "visible": false,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": false,
                "showInfo": false,
                "showNavigationButtons": false,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": false
            },
            "selection": {
                "mode": selectionMode == '' || selectionMode == null || selectionMode == undefined ? '' : selectionMode,
                "showCheckBoxesMode": 'always',
            },
            editing: {
                mode: 'batch',
                allowUpdating: true,
                allowAdding: true,
                allowDeleting: false,
                selectTextOnEditStart: true,
                startEditAction: 'click',
                //saveAllChanges: true,
                useIcons: true,
                newRowPosition: 'first',
            },
            showBorders: true,
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    var img = new Image();
                                    img.src = options.gridCell.value;
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
                    value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
                    value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
                    $(e.cellElement).text('')
                }

                /*                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '01/01/1900 12:00:00 AM' || value == '1/1/1900' ||
                    value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '01/01/2000 12:00:00 AM' || value == '1/1/2000' ||
                    value == '00-01-01' || value == '01-01-00' || value == '01-Jan-00' || value == '1/1/00 12:00:00 AM' || value == '01/01/00 12:00:00 AM' || value == '1/1/00')) {
                    console.log("In It")
                    // Set the value to undefined to indicate invalid/missing data
                    if (rowData && column.dataField) {
                        rowData[column.dataField] = undefined; // Update the data source
                    }
                }*/

                if ($(e.element).attr('id') == 'StockDetailContainer') {
                    if (Type == 'B') {
                        empr_StockTransfer.IsBLabelValid = true;
                        if (e.rowType == 'data') {
                            var rowIndexesToColor = empr_StockTransfer.FindNonMatchingIndexes(e.component.option('dataSource'));
                            if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
                                empr_StockTransfer.IsBLabelValid = false;
                                e.cellElement.css({ "color": "white", "background-color": "red" });
                            }
                        }
                    }
                }

                if ($(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
                    if (Type == 'B') {
                        empr_StockAdjustment.IsBLabelValid = true;
                        if (e.rowType == 'data') {
                            var rowIndexesToColor = empr_StockAdjustment.FindNonMatchingIndexes(e.component.option('dataSource'));
                            if (rowIndexesToColor.indexOf(e.rowIndex) !== -1) {
                                empr_StockAdjustment.IsBLabelValid = false;
                                e.cellElement.css({ "color": "white", "background-color": "red" });
                            }
                        }
                    }
                }

                if (div == '#deliveryFeedingDetailContainer') {
                    var totalAmt = e.component.getTotalSummaryValue('AmountTotal');
                    $('#Sel_AMT').val(totalAmt);
                    empr_DeliveryFeeding.calculateBuyerNetAmount();
                    empr_DeliveryFeeding.calculateSellerNetAmount();
                }
            },
            onEditorPreparing: function (e) {
                if (e.dataField === 'iteM_CODE') {
                    $(e.editorElement).on('keydown', function (event) {
                        if (event.key === 'Tab' && event.shiftKey) {
                            event.preventDefault();
                            if (event.shiftKey) {
                                $('#REMARKS').focus();
                            }
                        }
                    });
                }
                if (e.dataType === "number") {
                    e.editorOptions.onInput = function (e) {
                        var element = e.element;
                        var value = $(element).find('input').val();
                        value = value.replace(/[^0-9.]/g, '');
                        value = value.replace(/\.(?=.*\.)/g, '');
                        value = value.replace(/(\.\d{3})\d+/g, '$1');
                        if (value === '' || parseFloat(value) >= 0) {
                            $(element).val(value);
                        } else {
                            $(element).val($(element).data('lastValid') || '');
                        }
                        $(element).data('lastValid', $(element).val());
                    };
                }
            },
            onRowUpdating: function (e) {
                //// Track the edited rows
                var editedRow = e.newData;
                if (editedRow.credit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.debit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0 && editedRow.debit > 0) {
                    e.cancel = true;
                    empr_helper.notify("Please enter either Credit or Debit.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0) {
                    if (e.oldData.debit > 0 && editedRow.debit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }

                if (editedRow.debit > 0) {
                    if (e.oldData.credit > 0 && editedRow.credit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }
            },
            onRowUpdated: function (e) {

                if ($('#IsValidate').val() == 'true') {
                    if (e.data != null) {
                        if (empr_AccountOpening.editedRows.filter(r => r.acT_CODE == e.data.acT_CODE).length > 0) {
                            var rowIndex = empr_AccountOpening.editedRows.findIndex(function (row) {
                                return row.acT_CODE === e.data.acT_CODE; // Assuming "ID" is the unique identifier of your row
                            });

                            if (rowIndex !== -1) {
                                empr_AccountOpening.editedRows.splice(rowIndex, 1);
                                empr_AccountOpening.editedRows.push(e.data);
                            }
                        }
                        else {
                            empr_AccountOpening.editedRows.push(e.data);
                        }
                    }
                }
            },
            onToolbarPreparing: function (e) {
                //debugger;
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
                var toolbarItems = e.toolbarOptions.items;
                for (var i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton" || toolbarItems[i].name === "addRowButton" || toolbarItems[i].name === "deleteRowButton") {
                        toolbarItems.splice(i, 1);
                        i--;
                    }
                }
            },
            onEditingStart: function (e) {
                if (e.data.acT_TYPE === "C") {
                    e.cancel = true;
                    empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
                }

                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer') {
                    if (e.column.dataField == 'iteM_CODE') {
                        if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                            e.cancel = true;
                            //empr_helper.notify("You are not allowed to change this item", 2);
                        }
                    }

                    if (e.column.dataField == 'rate') {
                        if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                            e.cancel = true;
                            //empr_helper.notify("You are not allowed to change the rate of this item", 2);
                        }
                    }
                }
            },
            onKeyDown: function (e) {
                if ($(e.element).attr('id') == 'detailContainer') {
                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        console.log(columnIndex);
                        if (columnIndex === 5) {
                            e.event.preventDefault();
                            $('#SETT').focus();
                        }
                    }
                };
                if ($(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        if (columnIndex === 10) {
                            e.event.preventDefault();
                            $('#BrSeller').focus();
                        }
                    }
                };
                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'StockDetailContainer' || $(e.element).attr('id') == 'StockAdjustmentDetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");
                    if (focusedRowIndex !== undefined) {
                        var $focusedCell = $(dataGrid.getCellElement(focusedRowIndex, "Action")); // Assuming "Action" is the dataField of your action column
                        // Handle Enter key
                        if (keyCode === 13) {
                            // Check if the focused cell contains the specified class
                            console.log("Enter is pressed");
                            var $actionButton = $focusedCell.find(".Add"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                                // Set focus to the desired column in edit mode
                                dataGrid.editCell(0, 'iteM_CODE');
                            }, 1500);
                        }
                        else if (e.event.ctrlKey && keyCode === 67) {
                            console.log("Ctrl+C is pressed");
                            var $actionButton = $focusedCell.find(".Clone"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                            }, 1500);
                        }
                        else if (keyCode === 46) {
                            // Ctrl+C logic
                            console.log("Delete is pressed");
                            var $actionButton = $focusedCell.find(".Delete"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                            }, 1500);
                        }
                    }

                }

                if ($(e.element).attr('id') == 'SodaPickGridContainer' || $(e.element).attr('id') == 'BarcodePickGridContainer') {

                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");

                    // Check if a row is focused
                    if (focusedRowIndex !== undefined) {
                        // Handle Enter key
                        if (keyCode === 13) {
                            // Check if the focused cell contains the specified class
                            console.log("Enter is pressed");
                            dataGrid.selectRowsByIndexes([focusedRowIndex]);
                        }

                        //if (e.event.key == 'Control' && (keyCode == 65 || keyCode == 97)) {
                        if (e.event.ctrlKey && e.event.which === 65) {
                            console.log('CTRL+A is pressed');
                            $('#BtnAddSodaToDelivery').click();
                        }
                    }
                }
            },
            summary: {
                recalculateWhileEditing: true,
                totalItems: [
                    {
                        column: "GR Code",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "sqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "bamt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "samt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    //{
                    //    column: "rate",
                    //    summaryType: "sum",
                    //    displayFormat: "Total: {0}"
                    //},
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "AmountTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "bR_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "bR_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_BUYER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "wT_AMOUNT_SELLER",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "NetTotal",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "buyer",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.buyerAverage > 0) {
                                return "Average : " + empr_helper.buyerAverage.toLocaleString('en-US');
                            }
                        }
                    },
                    {
                        column: "seller",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.sellerAverage > 0) {
                                return "Average : " + empr_helper.sellerAverage.toLocaleString('en-US');
                            }
                        }
                    }
                ],
            }
        }).dxDataGrid('instance');
    },

    DxGridBindingForReports: function (div, columns, datasrc, fileName) {
        debugger;
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            // Your existing configuration
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 500,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "allowColumnReordering": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 300,
                "enabled": true
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            grouping: {
                autoExpandAll: true
            },
            showBorders: true,
            onExporting(e) {
                if (e.format === 'pdf') {
                    var layout;
                    if (fileName == 'POS Report') {
                        layout = 'landscape';
                    }
                    else {
                        layout = 'portrait';
                    }
                    const doc = new jsPDF({
                        orientation: layout, // or 'landscape' depending on your requirement
                        unit: 'pt', // units of measurement: points
                        format: 'a4' // set the page size to A3
                    });

                    function addHeader() {
                        const pageWidth = doc.internal.pageSize.getWidth() - 27;
                        const headerHeight = 60;

                        doc.setFont("helvetica", "bold");
                        doc.setFontSize(15);
                        doc.setTextColor(5, 90, 135);
                        doc.text(empr_helper.companyName, 20, 20);

                        doc.setFont("helvetica", "normal");
                        doc.setFontSize(12);
                        doc.text(empr_helper.reportName, 20, 40);
                        doc.setFillColor(5, 90, 135);
                        doc.rect(18, 47, pageWidth, 39, 'F');
                        doc.setFillColor(255, 255, 255);
                        doc.rect(18, 66, pageWidth, 0.5, 'F');

                        doc.setFontSize(10);
                        doc.setTextColor(255, 255, 255);
                        doc.text(`From : ${empr_helper.formatDate(empr_helper.fromDate)}       To: ${empr_helper.formatDate(empr_helper.toDate)}`, 20, 60);
                        const now = new Date();
                        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
                        const currentDate = now.toLocaleDateString('en-GB', options);
                        const currentTime = now.toLocaleTimeString();
                        doc.text(`Printed Date : ${currentDate}       Time : ${currentTime}`, 20, 80);
                        doc.setTextColor(0, 0, 0);
                    }
                    addHeader();

                    DevExpress.pdfExporter.exportDataGrid({
                        jsPDFDocument: doc,
                        component: e.component,
                        indent: 5,
                        margin: {
                            //top: 40,
                            top: 100,
                            right: 10,
                            bottom: 40,
                            left: 10,
                        },
                        topLeft: { x: 5, y: 0 },
                        customizeCell: function (cellInfo) {
                            if (cellInfo && cellInfo.gridCell && cellInfo.gridCell.column) {


                                if (cellInfo.gridCell.value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.wordWrapEnabled = true;
                                cellInfo.pdfCell.text = cellInfo.pdfCell.text || '';
                                cellInfo.pdfCell.font = {
                                    size: 9
                                };

                                cellInfo.pdfCell.padding = {
                                    top: 2,
                                    right: 2,
                                    bottom: 2,
                                    left: 2
                                };

                                let value = cellInfo.gridCell.value;
                                const column = cellInfo.gridCell.column;

                                if (column.caption.toLowerCase().includes('trans')) {
                                    cellInfo.pdfCell.textColor = '#055a87';
                                }

                                if (cellInfo.gridCell.rowType === 'header') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 9 };
                                    cellInfo.pdfCell.textColor = 'black';
                                }

                                if (cellInfo.gridCell.rowType === 'group') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 9 };
                                }

                                if (cellInfo.gridCell.rowType === 'groupFooter' || cellInfo.gridCell.rowType === 'totalFooter') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 9 };
                                }

                                if (value instanceof Date) {
                                    const year = value.getFullYear();
                                    const month = String(value.getMonth() + 1).padStart(2, '0');
                                    const day = String(value.getDate()).padStart(2, '0');
                                    value = `${year}-${month}-${day}`;
                                }

                                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) &&
                                    (value === '1900-01-01' || value === '01-01-1900' || value === '01-Jan-1900' || value === '1/1/1900 12:00:00 AM' || value === '1/1/1900' || value === '01/01/1900')) {
                                    cellInfo.pdfCell.text = '';
                                }

                                if (['debit', 'recv', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party'].includes(column.dataField)) {
                                    if (cellInfo.gridCell.rowType !== 'header') {
                                        if (value < 0) {
                                            cellInfo.pdfCell.text = `(${Math.abs(value)})`;
                                            cellInfo.pdfCell.textColor = 'red';
                                        } else {
                                            cellInfo.pdfCell.text = value;
                                            cellInfo.pdfCell.textColor = 'black';
                                        }
                                    }
                                }

                                if (['debit', 'credit', 'recv', 'balance'].includes(column.dataField) && value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.borderColor = 'black';
                            }
                        }
                    }).then(function () {
                        const pageCount = doc.internal.getNumberOfPages();
                        for (let i = 1; i <= pageCount; i++) {
                            doc.setPage(i);

                            addHeader();

                            doc.setFontSize(10);
                            doc.text(`Page ${i} of ${pageCount}`, doc.internal.pageSize.getWidth() / 2, doc.internal.pageSize.getHeight() - 20, { align: 'center' });
                        }
                        const pdfBlob = doc.output('blob');
                        const blobUrl = URL.createObjectURL(pdfBlob);
                        window.open(blobUrl);
                    });
                } else {
                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet(fileName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then((buffer) => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                        });
                    });
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (e.rowType === 'group' && (e.column.dataField === 'accountName' || e.column.dataField === 'itemId' || e.column.dataField === 'bTransfer' || e.column.dataField === 'vDate'
                    || e.column.dataField === 'itemName' || e.column.dataField === 'category' || e.column.dataField === 'subCategory' || e.column.dataField === 'barcode' || e.column.dataField === ''
                )) {
                    e.cellElement.css({
                        'font-weight': '650',
                    });
                }
                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900' || value == '01/01/1900' || value == '28/01/2000')) {
                    $(e.cellElement).text('')
                }

                if (['debit', 'credit', 'recv', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party'].includes(column.dataField)) {
                    if (value === 0) {
                        $(e.cellElement).text('');
                    } else if (value !== null && value !== undefined && !isNaN(value)) {
                        if (value < 0) {
                            $(e.cellElement).text(`(${Math.abs(value).toLocaleString('en-US')})`);
                            $(e.cellElement).css('color', 'red');
                        } else {
                            $(e.cellElement).text(Number(value).toLocaleString('en-US'));
                            $(e.cellElement).css('color', 'black');
                        }
                    }
                }

            },
            summary: {
                recalculateWhileEditing: true,
                groupItems: [
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    //{
                    //    column: "disc",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    showInGroupFooter: true,
                    //    valueFormat: {
                    //        type: "fixedPoint",
                    //        precision: 0,
                    //    }
                    //},
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        name: "customPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    }
                ],
                totalItems: [
                    {
                        name: "customPriceTotal",
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "buyerName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.debitAverage > 0) {
                                return "Stock In Avg : " + empr_helper.debitAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "itemName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.creditAverage > 0) {
                                return "Stock Out Avg : " + empr_helper.creditAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "recv",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "voucherDate",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Date : ${empr_helper.lastDate}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "voucherNo",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Amount : ${empr_helper.lastAmt.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "accountDescription",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.balanceAmount != "") {
                                if (empr_helper.balanceAmount > 0) {
                                    return "Debit Balance : " + empr_helper.balanceAmount.toLocaleString('en-US');
                                } else {
                                    return `Credit Balance : (${new Intl.NumberFormat().format(Math.abs(empr_helper.balanceAmount))})`;
                                }
                            }
                            return "";
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "total",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    //{
                    //    column: "qty",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}"
                    //},
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "oqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "iqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalSales",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cash",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cardType",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "party",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "cset",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    //{
                    //    column: "disc",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    valueFormat: {
                    //        type: "fixedPoint",
                    //        precision: 0,\
                    //    }
                    //},
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "customPriceTotal" || options.name === "customPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.price * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customWPriceTotal" || options.name === "customWPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.wholePrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customSPriceTotal" || options.name === "customSPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.shopPrice * options.value.totalBalance;
                        }
                    }
                }
            }
        }).dxDataGrid('instance');
    },

    DxGridBindingForReportsWithSetting: function (div, columns, datasrc, fileName, isLandscape = false) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            // Your existing configuration
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 500,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "allowColumnReordering": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 300,
                "enabled": true
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            grouping: {
                autoExpandAll: true
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            showBorders: true,
            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    var layout;
                    if (isLandscape) {
                        layout = 'landscape';
                    } else {
                        layout = 'portrait';
                    }
                    const doc = new jsPDF({
                        orientation: layout, // or 'landscape' depending on your requirement
                        unit: 'pt', // units of measurement: points
                        format: 'a4' // set the page size to A3
                    });

                    function addHeader() {
                        const pageWidth = doc.internal.pageSize.getWidth() - 27;
                        const headerHeight = 60;

                        doc.setFont("helvetica", "bold");
                        doc.setFontSize(12);
                        doc.setTextColor(5, 90, 135);
                        doc.text(empr_helper.companyName, 20, 20);

                        doc.setFont("helvetica", "normal");
                        doc.setFontSize(10);
                        doc.text(empr_helper.reportName, 20, 40);
                        doc.setFillColor(5, 90, 135);
                        doc.rect(18, 47, pageWidth, 39, 'F');
                        doc.setFillColor(255, 255, 255);
                        doc.rect(18, 66, pageWidth, 0.5, 'F');

                        doc.setFontSize(8);
                        doc.setTextColor(255, 255, 255);
                        doc.text(`From : ${empr_helper.formatDate(empr_helper.fromDate)}       To: ${empr_helper.formatDate(empr_helper.toDate)}`, 20, 60);
                        const now = new Date();
                        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
                        const currentDate = now.toLocaleDateString('en-GB', options);
                        const currentTime = now.toLocaleTimeString();
                        doc.text(`Printed Date : ${currentDate}       Time : ${currentTime}`, 20, 80);
                        doc.setTextColor(0, 0, 0);
                    }
                    addHeader();

                    DevExpress.pdfExporter.exportDataGrid({
                        jsPDFDocument: doc,
                        component: e.component,
                        indent: 5,
                        margin: {
                            //top: 40,
                            top: 100,
                            right: 10,
                            bottom: 40,
                            left: 10,
                        },
                        topLeft: { x: 10, y: 0 },
                        customizeCell: function (cellInfo) {
                            if (cellInfo && cellInfo.gridCell && cellInfo.gridCell.column) {


                                if (cellInfo.gridCell.value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.wordWrapEnabled = true;
                                cellInfo.pdfCell.text = cellInfo.pdfCell.text || '';
                                cellInfo.pdfCell.font = {
                                    size: 7
                                };

                                cellInfo.pdfCell.padding = {
                                    top: 2,
                                    right: 2,
                                    bottom: 2,
                                    left: 2
                                };

                                let value = cellInfo.gridCell.value;
                                const column = cellInfo.gridCell.column;

                                if (column.caption.toLowerCase().includes('trans')) {
                                    cellInfo.pdfCell.textColor = '#055a87';
                                }

                                if (cellInfo.gridCell.rowType === 'header') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 8 };
                                    cellInfo.pdfCell.textColor = 'black';
                                }

                                if (cellInfo.gridCell.rowType === 'group') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 8 };
                                }

                                if (cellInfo.gridCell.rowType === 'groupFooter' || cellInfo.gridCell.rowType === 'totalFooter') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 7 };
                                }

                                if (value instanceof Date) {
                                    const year = value.getFullYear();
                                    const month = String(value.getMonth() + 1).padStart(2, '0');
                                    const day = String(value.getDate()).padStart(2, '0');
                                    value = `${year}-${month}-${day}`;
                                }

                                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) &&
                                    (value === '1900-01-01' || value === '01-01-1900' || value === '01-Jan-1900' || value === '1/1/1900 12:00:00 AM' || value === '1/1/1900' || value === '01/01/1900' || value === '2000-01-01' || value === '01-01-2000' || value === '01-Jan-2000' || value === '1/1/2000 12:00:00 AM' || value === '1/1/2000' || value === '01/01/2000' || value === '28/01/2000')) {
                                    cellInfo.pdfCell.text = '';
                                }

                                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party', 'tax'].includes(column.dataField)) {
                                    if (cellInfo.gridCell.rowType !== 'header') {

                                        const formattedValue = Number(Math.abs(value)).toLocaleString(); // comma separated

                                        if (value < 0) {
                                            cellInfo.pdfCell.text = `(${Math.abs(value)})`;
                                            cellInfo.pdfCell.textColor = 'red';
                                        } else {
                                            cellInfo.pdfCell.text = formattedValue;
                                            cellInfo.pdfCell.textColor = 'black';
                                        }


                                    }
                                }

                                if (['debit', 'credit', 'balance'].includes(column.dataField) && value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.borderColor = 'black';
                            }
                        }
                    }).then(function () {
                        const pageCount = doc.internal.getNumberOfPages();
                        for (let i = 1; i <= pageCount; i++) {
                            doc.setPage(i);

                            addHeader();

                            doc.setFontSize(7);
                            doc.text(`Page ${i} of ${pageCount}`, doc.internal.pageSize.getWidth() / 2, doc.internal.pageSize.getHeight() - 20, { align: 'center' });
                        }
                        const pdfBlob = doc.output('blob');
                        const blobUrl = URL.createObjectURL(pdfBlob);
                        window.open(blobUrl);
                    });
                } else {
                    //const workbook = new ExcelJS.Workbook();
                    //const worksheet = workbook.addWorksheet(fileName);

                    //DevExpress.excelExporter.exportDataGrid({
                    //    component: e.component,
                    //    worksheet,
                    //    autoFilterEnabled: true,
                    //}).then(() => {
                    //    workbook.xlsx.writeBuffer().then((buffer) => {
                    //        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                    //    });
                    //});

                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet(fileName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                        customizeCell: function (options) {
                            const { gridCell, excelCell } = options;

                            // Target columns for formatting
                            const targetFields = [
                                'debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc',
                                'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt',
                                'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales',
                                'cash', 'cardType', 'party'
                            ];

                            if (gridCell.rowType !== 'header' && targetFields.includes(gridCell.column.dataField)) {
                                if (typeof gridCell.value === 'number') {
                                    // Apply comma formatting with 2 decimals
                                    excelCell.numFmt = '#,##0';

                                }
                            }
                        }
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then((buffer) => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                        });
                    });

                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (e.rowType === 'group' && (e.column.dataField === 'accountName' || e.column.dataField === 'itemId' || e.column.dataField === 'bTransfer' || e.column.dataField === 'vDate'
                    || e.column.dataField === 'itemName' || e.column.dataField === 'category' || e.column.dataField === 'subCategory' || e.column.dataField === 'barcode' || e.column.dataField === ''
                    || e.column.dataField === 'bType')) {
                    e.cellElement.css({
                        'font-weight': '650',
                        position: "sticky",
                        top: "0px",
                        background: "#fff",
                        zIndex: 5,
                        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
                    });
                }
                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900' || value == '01/01/1900' || value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '1/1/2000' || value == '01/01/2000')) {
                    $(e.cellElement).text('')
                }

                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party', 'qty', 'rate', 'disc', 'balanced'].includes(column.dataField)) {
                    if (value === 0) {
                        $(e.cellElement).text('');
                    } else if (value !== null && value !== undefined && !isNaN(value)) {
                        if (value < 0) {
                            $(e.cellElement).text(`(${Math.abs(value).toLocaleString('en-US')})`);
                            $(e.cellElement).css('color', 'red');
                        } else {
                            $(e.cellElement).text(Number(value).toLocaleString('en-US'));
                            $(e.cellElement).css('color', 'black');
                        }
                    }
                }
                if (e.rowType === 'groupFooter' && e.column.dataField === 'debit') {
                    const groupIndex = e.row.groupIndex;
                    if (groupIndex !== 0) {
                        e.cellElement.text('');
                    }
                }
            },
            summary: {
                recalculateWhileEditing: true,
                groupItems: [
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "commission",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return Math.round(e.value).toLocaleString("en-US");
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    //{
                    //    column: "balance2",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    showInGroupFooter: true,
                    //    valueFormat: '#,##0',
                    //    customizeText: function (e) {
                    //        return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                    //    }
                    //},
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balance",
                        summaryType: "sum",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balanced",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },

                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "deliveryCharges",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        name: "customPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "TotalmaountAndLabel",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "balanced",  // exact field name
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (e.value == null || e.value === 0) return "";

                            // Final text already assigned in finalize stage
                            return e.text || e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customAvgSummary",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "itemName", // show result in 'balanced' column footer 
                        displayFormat: "Avg Total: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                ],
                totalItems: [
                    {
                        name: "customPriceTotal",
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "accountName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.typeOneTotal != 0 || empr_helper.typeTwoTotal != 0) {
                                var grossSales = empr_helper.typeOneTotal + empr_helper.typeTwoTotal;
                                var grossProfit = grossSales - empr_helper.typeThreeTotal;
                                var netProfit = grossProfit - empr_helper.typeFourTotal;
                                return `Gross Sales : ${grossSales.toLocaleString('en-US')} --------------- Gross Profit : ${grossProfit.toLocaleString('en-US')} --------------- Net Profit : ${netProfit.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "buyerName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.debitAverage > 0) {
                                return "Stock In Avg : " + empr_helper.debitAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "itemName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.creditAverage > 0) {
                                return "Stock Out Avg : " + empr_helper.creditAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (empr_helper.typeOneTotal == 0 || empr_helper.typeTwoTotal == 0) {
                                return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "voucherDate",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Date : ${empr_helper.lastDate}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "voucherNo",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Amount : ${empr_helper.lastAmt.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "accountDescription",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.balanceAmount != "") {
                                if (empr_helper.balanceAmount > 0) {
                                    return "Debit Balance : " + empr_helper.balanceAmount.toLocaleString('en-US');
                                } else {
                                    return `Credit Balance : (${new Intl.NumberFormat().format(Math.abs(empr_helper.balanceAmount))})`;
                                }
                            }
                            return "";
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "total",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "deliveryCharges",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    //{
                    //    column: "qty",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}"
                    //},
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "oqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "iqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        customizeText: function (e) {
                            if (!e.value || e.value === 0) return "";
                            return e.value < 0
                                ? `(${Math.abs(e.value).toLocaleString('en-US')})`
                                : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "avg",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        customizeText: function (e) {
                            if (!e.value || e.value === 0) return "";
                            return e.value < 0
                                ? `(${Math.abs(e.value).toLocaleString('en-US')})`
                                : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalSales",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cash",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cardType",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "party",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "amt", // ammar2
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "taxAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "inclTax",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "cset",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "disc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "commission",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return Math.round(e.value).toLocaleString("en-US");
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        name: "customAvgSummary",
                        showInColumn: "itemNameWithAvg", // show result in 'balanced' column footer 
                        displayFormat: "Avg Total: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                    {
                        name: "customBalanceSummary",
                        showInColumn: "balanced", // show result in 'balanced' column footer
                        displayFormat: "Closing Balance: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                    {
                        name: "netTotal",
                        showInColumn: "balances",
                        summaryType: "custom",
                        displayFormat: "Net Profit: {0}",
                        valueFormat: '#,##0'
                    }
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "customPriceTotal" || options.name === "customPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.price * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customWPriceTotal" || options.name === "customWPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.wholePrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customSPriceTotal" || options.name === "customSPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.shopPrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customBalanceSummary") {
                        if (options.summaryProcess === "start") {
                            options.totalA = 0;
                            options.totalS = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            const vcType = options.value.vctype;
                            const balance = parseFloat(options.value.balanced) || 0;

                            if (vcType === "A") {
                                options.totalA += balance;
                            }
                            if (vcType === "S") {
                                options.totalS += balance;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            options.totalValue = options.totalA - options.totalS;
                            empr_ClosingShop.ClosingBalance = options.totalA - options.totalS;
                        }
                    }
                    if (options.name === "customAvgSummary") {
                        if (options.summaryProcess === "start") {
                            options.totalAmt = 0;
                            options.totalQty = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            const amt = parseFloat(options.value.amt) || 0;
                            const qty = parseFloat(options.value.oqty) || 0;

                            options.totalAmt += amt;
                            options.totalQty += qty;
                        }
                        if (options.summaryProcess === "finalize") {
                            if (options.totalQty !== 0) {
                                options.totalValue = options.totalAmt / options.totalQty;
                            } else {
                                options.totalValue = 0;
                            }

                            // Format the value to 2 decimal places
                            options.totalValue = parseFloat(options.totalValue.toFixed(2));
                            console.log(options.totalValue);
                        }
                    }
                    if (options.name === "netTotal") {
                        if (options.summaryProcess === "start") {
                            options.revenueTotal = 0;
                            options.expenseTotal = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            console.log("accountCode:" + options.value.accountCode);
                            if (options.value.accountCode === 4) {
                                options.revenueTotal += options.value.balances || 0;
                            }
                            else if (options.value.accountCode === 5) {
                                options.expenseTotal += options.value.balances || 0;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            var Profit = options.revenueTotal - options.expenseTotal;
                            options.totalValue = Profit;
                        }
                    }
                    if (options.name === "TotalmaountAndLabel") {
                        if (options.summaryProcess === "start") {
                            options.assetsTotal = 0;
                            options.liabilitiesTotal = 0;
                        }

                        if (options.summaryProcess === "calculate") {
                            // Row-wise calculation
                            if (options.value.accountCode === 1) { // 1 = Assets
                                options.assetsTotal += options.value.balance || 0;
                            }
                            else if (options.value.accountCode === 2) { // 2 = Liabilities
                                options.liabilitiesTotal += options.value.balance || 0;
                            }
                        }

                        if (options.summaryProcess === "finalize") {
                            // Final totals ke liye text assign karo
                            if (options.summaryIndex === 0) { // first summary row
                                options.totalValue = options.assetsTotal;
                                options.text = 'Assets: ' + options.assetsTotal.toLocaleString('en-US');
                            }
                            else if (options.summaryIndex === 1) { // second summary row
                                options.totalValue = options.liabilitiesTotal;
                                options.text = 'Liabilities: ' + options.liabilitiesTotal.toLocaleString('en-US');
                            }
                        }
                    }
                }
            },
            onCellPrepared(e) {
                if (
                    e.rowType === "groupFooter" &&
                    e.row.groupIndex === 0 &&
                    e.column.dataField === "balance" // LEFT SIDE column
                ) {
                    var groupValue = e.row.data.key;
                    var totalText = e.cellElement.text();

                    if (!totalText || totalText === "0") {
                        e.cellElement.html("");
                    } else {
                        e.cellElement.html(
                            'Group Total : ' + totalText
                        );
                    }
                }
            }
        }).dxDataGrid('instance');
    },

    DxGridBindingForReportsWithSetting_withoutGroup: function (div, columns, datasrc, fileName, isLandscape = false) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            // Your existing configuration
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 500,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            //"groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "allowColumnReordering": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 300,
                "enabled": true
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            grouping: {
                autoExpandAll: true
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            showBorders: true,
            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    var layout;
                    if (isLandscape) {
                        layout = 'landscape';
                    } else {
                        layout = 'portrait';
                    }
                    const doc = new jsPDF({
                        orientation: layout, // or 'landscape' depending on your requirement
                        unit: 'pt', // units of measurement: points
                        format: 'a4' // set the page size to A3
                    });

                    function addHeader() {
                        const pageWidth = doc.internal.pageSize.getWidth() - 27;
                        const headerHeight = 60;

                        doc.setFont("helvetica", "bold");
                        doc.setFontSize(12);
                        doc.setTextColor(5, 90, 135);
                        doc.text(empr_helper.companyName, 20, 20);

                        doc.setFont("helvetica", "normal");
                        doc.setFontSize(10);
                        doc.text(empr_helper.reportName, 20, 40);
                        doc.setFillColor(5, 90, 135);
                        doc.rect(18, 47, pageWidth, 39, 'F');
                        doc.setFillColor(255, 255, 255);
                        doc.rect(18, 66, pageWidth, 0.5, 'F');

                        doc.setFontSize(8);
                        doc.setTextColor(255, 255, 255);
                        doc.text(`From : ${empr_helper.formatDate(empr_helper.fromDate)}       To: ${empr_helper.formatDate(empr_helper.toDate)}`, 20, 60);
                        const now = new Date();
                        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
                        const currentDate = now.toLocaleDateString('en-GB', options);
                        const currentTime = now.toLocaleTimeString();
                        doc.text(`Printed Date : ${currentDate}       Time : ${currentTime}`, 20, 80);
                        doc.setTextColor(0, 0, 0);
                    }
                    addHeader();

                    DevExpress.pdfExporter.exportDataGrid({
                        jsPDFDocument: doc,
                        component: e.component,
                        indent: 5,
                        margin: {
                            //top: 40,
                            top: 100,
                            right: 10,
                            bottom: 40,
                            left: 10,
                        },
                        topLeft: { x: 10, y: 0 },
                        customizeCell: function (cellInfo) {
                            if (cellInfo && cellInfo.gridCell && cellInfo.gridCell.column) {


                                if (cellInfo.gridCell.value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.wordWrapEnabled = true;
                                cellInfo.pdfCell.text = cellInfo.pdfCell.text || '';
                                cellInfo.pdfCell.font = {
                                    size: 7
                                };

                                cellInfo.pdfCell.padding = {
                                    top: 2,
                                    right: 2,
                                    bottom: 2,
                                    left: 2
                                };

                                let value = cellInfo.gridCell.value;
                                const column = cellInfo.gridCell.column;

                                if (column.caption.toLowerCase().includes('trans')) {
                                    cellInfo.pdfCell.textColor = '#055a87';
                                }

                                if (cellInfo.gridCell.rowType === 'header') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 8 };
                                    cellInfo.pdfCell.textColor = 'black';
                                }

                                if (cellInfo.gridCell.rowType === 'group') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 8 };
                                }

                                if (cellInfo.gridCell.rowType === 'groupFooter' || cellInfo.gridCell.rowType === 'totalFooter') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 7 };
                                }

                                if (value instanceof Date) {
                                    const year = value.getFullYear();
                                    const month = String(value.getMonth() + 1).padStart(2, '0');
                                    const day = String(value.getDate()).padStart(2, '0');
                                    value = `${year}-${month}-${day}`;
                                }

                                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) &&
                                    (value === '1900-01-01' || value === '01-01-1900' || value === '01-Jan-1900' || value === '1/1/1900 12:00:00 AM' || value === '1/1/1900' || value === '01/01/1900' || value === '2000-01-01' || value === '01-01-2000' || value === '01-Jan-2000' || value === '1/1/2000 12:00:00 AM' || value === '1/1/2000' || value === '01/01/2000' || value === '28/01/2000')) {
                                    cellInfo.pdfCell.text = '';
                                }

                                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party', 'tax'].includes(column.dataField)) {
                                    if (cellInfo.gridCell.rowType !== 'header') {

                                        const formattedValue = Number(Math.abs(value)).toLocaleString(); // comma separated

                                        if (value < 0) {
                                            cellInfo.pdfCell.text = `(${Math.abs(value)})`;
                                            cellInfo.pdfCell.textColor = 'red';
                                        } else {
                                            cellInfo.pdfCell.text = formattedValue;
                                            cellInfo.pdfCell.textColor = 'black';
                                        }


                                    }
                                }

                                if (['debit', 'credit', 'balance'].includes(column.dataField) && value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.borderColor = 'black';
                            }
                        }
                    }).then(function () {
                        const pageCount = doc.internal.getNumberOfPages();
                        for (let i = 1; i <= pageCount; i++) {
                            doc.setPage(i);

                            addHeader();

                            doc.setFontSize(7);
                            doc.text(`Page ${i} of ${pageCount}`, doc.internal.pageSize.getWidth() / 2, doc.internal.pageSize.getHeight() - 20, { align: 'center' });
                        }
                        const pdfBlob = doc.output('blob');
                        const blobUrl = URL.createObjectURL(pdfBlob);
                        window.open(blobUrl);
                    });
                } else {
                    //const workbook = new ExcelJS.Workbook();
                    //const worksheet = workbook.addWorksheet(fileName);

                    //DevExpress.excelExporter.exportDataGrid({
                    //    component: e.component,
                    //    worksheet,
                    //    autoFilterEnabled: true,
                    //}).then(() => {
                    //    workbook.xlsx.writeBuffer().then((buffer) => {
                    //        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                    //    });
                    //});

                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet(fileName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                        customizeCell: function (options) {
                            const { gridCell, excelCell } = options;

                            // Target columns for formatting
                            const targetFields = [
                                'debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc',
                                'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt',
                                'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales',
                                'cash', 'cardType', 'party'
                            ];

                            if (gridCell.rowType !== 'header' && targetFields.includes(gridCell.column.dataField)) {
                                if (typeof gridCell.value === 'number') {
                                    // Apply comma formatting with 2 decimals
                                    excelCell.numFmt = '#,##0';

                                }
                            }
                        }
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then((buffer) => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                        });
                    });

                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (e.rowType === 'group' && (e.column.dataField === 'accountName' || e.column.dataField === 'itemId' || e.column.dataField === 'bTransfer' || e.column.dataField === 'vDate'
                    || e.column.dataField === 'itemName' || e.column.dataField === 'category' || e.column.dataField === 'subCategory' || e.column.dataField === 'barcode' || e.column.dataField === ''
                    || e.column.dataField === 'bType')) {
                    e.cellElement.css({
                        'font-weight': '650',
                        position: "sticky",
                        top: "0px",
                        background: "#fff",
                        zIndex: 5,
                        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
                    });
                }
                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900' || value == '01/01/1900' || value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '1/1/2000' || value == '01/01/2000')) {
                    $(e.cellElement).text('')
                }

                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party', 'qty', 'rate', 'disc', 'balanced'].includes(column.dataField)) {
                    if (value === 0) {
                        $(e.cellElement).text('');
                    } else if (value !== null && value !== undefined && !isNaN(value)) {
                        if (value < 0) {
                            $(e.cellElement).text(`(${Math.abs(value).toLocaleString('en-US')})`);
                            $(e.cellElement).css('color', 'red');
                        } else {
                            $(e.cellElement).text(Number(value).toLocaleString('en-US'));
                            $(e.cellElement).css('color', 'black');
                        }
                    }
                }
                if (e.rowType === 'groupFooter' && e.column.dataField === 'debit') {
                    const groupIndex = e.row.groupIndex;
                    if (groupIndex !== 0) {
                        e.cellElement.text('');
                    }
                }
            },
            summary: {
                recalculateWhileEditing: true,
                groupItems: [
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "commission",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return Math.round(e.value).toLocaleString("en-US");
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    //{
                    //    column: "balance2",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    showInGroupFooter: true,
                    //    valueFormat: '#,##0',
                    //    customizeText: function (e) {
                    //        return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                    //    }
                    //},
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balance",
                        summaryType: "sum",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balanced",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },

                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "deliveryCharges",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        name: "customPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "TotalmaountAndLabel",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "balanced",  // exact field name
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (e.value == null || e.value === 0) return "";

                            // Final text already assigned in finalize stage
                            return e.text || e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customAvgSummary",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "itemName", // show result in 'balanced' column footer 
                        displayFormat: "Avg Total: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                ],
                totalItems: [
                    {
                        name: "customPriceTotal",
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "accountName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.typeOneTotal != 0 || empr_helper.typeTwoTotal != 0) {
                                var grossSales = empr_helper.typeOneTotal + empr_helper.typeTwoTotal;
                                var grossProfit = grossSales - empr_helper.typeThreeTotal;
                                var netProfit = grossProfit - empr_helper.typeFourTotal;
                                return `Gross Sales : ${grossSales.toLocaleString('en-US')} --------------- Gross Profit : ${grossProfit.toLocaleString('en-US')} --------------- Net Profit : ${netProfit.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "buyerName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.debitAverage > 0) {
                                return "Stock In Avg : " + empr_helper.debitAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "itemName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.creditAverage > 0) {
                                return "Stock Out Avg : " + empr_helper.creditAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (empr_helper.typeOneTotal == 0 || empr_helper.typeTwoTotal == 0) {
                                return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "voucherDate",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Date : ${empr_helper.lastDate}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "voucherNo",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Amount : ${empr_helper.lastAmt.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "accountDescription",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.balanceAmount != "") {
                                if (empr_helper.balanceAmount > 0) {
                                    return "Debit Balance : " + empr_helper.balanceAmount.toLocaleString('en-US');
                                } else {
                                    return `Credit Balance : (${new Intl.NumberFormat().format(Math.abs(empr_helper.balanceAmount))})`;
                                }
                            }
                            return "";
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "total",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "deliveryCharges",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    //{
                    //    column: "qty",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}"
                    //},
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "oqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "iqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        customizeText: function (e) {
                            if (!e.value || e.value === 0) return "";
                            return e.value < 0
                                ? `(${Math.abs(e.value).toLocaleString('en-US')})`
                                : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "avg",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        customizeText: function (e) {
                            if (!e.value || e.value === 0) return "";
                            return e.value < 0
                                ? `(${Math.abs(e.value).toLocaleString('en-US')})`
                                : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalSales",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cash",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cardType",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "party",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "amt", // ammar2
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "taxAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "inclTax",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "cset",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "disc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "commission",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return Math.round(e.value).toLocaleString("en-US");
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        name: "customAvgSummary",
                        showInColumn: "itemNameWithAvg", // show result in 'balanced' column footer 
                        displayFormat: "Avg Total: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                    {
                        name: "customBalanceSummary",
                        showInColumn: "balanced", // show result in 'balanced' column footer
                        displayFormat: "Closing Balance: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                    {
                        name: "netTotal",
                        showInColumn: "balances",
                        summaryType: "custom",
                        displayFormat: "Net Profit: {0}",
                        valueFormat: '#,##0'
                    }
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "customPriceTotal" || options.name === "customPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.price * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customWPriceTotal" || options.name === "customWPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.wholePrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customSPriceTotal" || options.name === "customSPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.shopPrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customBalanceSummary") {
                        if (options.summaryProcess === "start") {
                            options.totalA = 0;
                            options.totalS = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            const vcType = options.value.vctype;
                            const balance = parseFloat(options.value.balanced) || 0;

                            if (vcType === "A") {
                                options.totalA += balance;
                            }
                            if (vcType === "S") {
                                options.totalS += balance;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            options.totalValue = options.totalA - options.totalS;
                            empr_ClosingShop.ClosingBalance = options.totalA - options.totalS;
                        }
                    }
                    if (options.name === "customAvgSummary") {
                        if (options.summaryProcess === "start") {
                            options.totalAmt = 0;
                            options.totalQty = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            const amt = parseFloat(options.value.amt) || 0;
                            const qty = parseFloat(options.value.oqty) || 0;

                            options.totalAmt += amt;
                            options.totalQty += qty;
                        }
                        if (options.summaryProcess === "finalize") {
                            if (options.totalQty !== 0) {
                                options.totalValue = options.totalAmt / options.totalQty;
                            } else {
                                options.totalValue = 0;
                            }

                            // Format the value to 2 decimal places
                            options.totalValue = parseFloat(options.totalValue.toFixed(2));
                            console.log(options.totalValue);
                        }
                    }
                    if (options.name === "netTotal") {
                        if (options.summaryProcess === "start") {
                            options.revenueTotal = 0;
                            options.expenseTotal = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            console.log("accountCode:" + options.value.accountCode);
                            if (options.value.accountCode === 4) {
                                options.revenueTotal += options.value.balances || 0;
                            }
                            else if (options.value.accountCode === 5) {
                                options.expenseTotal += options.value.balances || 0;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            var Profit = options.revenueTotal - options.expenseTotal;
                            options.totalValue = Profit;
                        }
                    }
                    if (options.name === "TotalmaountAndLabel") {
                        if (options.summaryProcess === "start") {
                            options.assetsTotal = 0;
                            options.liabilitiesTotal = 0;
                        }

                        if (options.summaryProcess === "calculate") {
                            // Row-wise calculation
                            if (options.value.accountCode === 1) { // 1 = Assets
                                options.assetsTotal += options.value.balance || 0;
                            }
                            else if (options.value.accountCode === 2) { // 2 = Liabilities
                                options.liabilitiesTotal += options.value.balance || 0;
                            }
                        }

                        if (options.summaryProcess === "finalize") {
                            // Final totals ke liye text assign karo
                            if (options.summaryIndex === 0) { // first summary row
                                options.totalValue = options.assetsTotal;
                                options.text = 'Assets: ' + options.assetsTotal.toLocaleString('en-US');
                            }
                            else if (options.summaryIndex === 1) { // second summary row
                                options.totalValue = options.liabilitiesTotal;
                                options.text = 'Liabilities: ' + options.liabilitiesTotal.toLocaleString('en-US');
                            }
                        }
                    }
                }
            },
            onCellPrepared(e) {
                if (
                    e.rowType === "groupFooter" &&
                    e.row.groupIndex === 0 &&
                    e.column.dataField === "balance" // LEFT SIDE column
                ) {
                    var groupValue = e.row.data.key;
                    var totalText = e.cellElement.text();

                    if (!totalText || totalText === "0") {
                        e.cellElement.html("");
                    } else {
                        e.cellElement.html(
                            '' + groupValue + ' Total :' + totalText
                        );
                    }
                }
            }
        }).dxDataGrid('instance');
    },

    DxGridBindingForReportsWithSetting_Aging: function (div, columns, datasrc, fileName, isLandscape = false) {
        debugger;
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            // Your existing configuration
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 500,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "allowColumnReordering": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 300,
                "enabled": true
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            grouping: {
                autoExpandAll: true
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            showBorders: true,
            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },

            onExporting(e) {
                debugger;
                const fileName = empr_helper.reportName || "Report";
                debugger;
                if (e.format === 'pdf') {
                    $("#Loader").show();
                    const layout = isLandscape;
                    const doc = new jsPDF({
                        orientation: layout,
                        unit: 'pt',
                        format: 'a4'
                    });

                    function addHeader() {
                        const pageWidth = doc.internal.pageSize.getWidth() - 27;

                        doc.setFont("helvetica", "bold");
                        doc.setFontSize(12);
                        doc.setTextColor(5, 90, 135);
                        doc.text(empr_helper.companyName, 20, 20);

                        doc.setFont("helvetica", "normal");
                        doc.setFontSize(10);
                        doc.text(empr_helper.reportName, 20, 40);
                        doc.setFillColor(5, 90, 135);
                        doc.rect(18, 47, pageWidth, 39, 'F');
                        doc.setFillColor(255, 255, 255);
                        doc.rect(18, 66, pageWidth, 0.5, 'F');

                        doc.setFontSize(8);
                        doc.setTextColor(255, 255, 255);
                        doc.text(`From : ${empr_helper.formatDate(empr_helper.fromDate)}       To: ${empr_helper.formatDate(empr_helper.toDate)}`, 20, 60);

                        const now = new Date();
                        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
                        const currentDate = now.toLocaleDateString('en-GB', options);
                        const currentTime = now.toLocaleTimeString();
                        doc.text(`Printed Date : ${currentDate}       Time : ${currentTime}`, 20, 80);
                        doc.setTextColor(0, 0, 0);
                    }
                    addHeader();

                    debugger;
                    // ---------- GRID INSTANCES ----------
                    const grids = [];
                    const grid1 = $("#ReportGridContainer").dxDataGrid("instance");
                    if (grid1) grids.push({ instance: grid1, title: empr_helper.reportName });

                    const isPartyLedger = empr_helper.reportName === "Master Plaining Sheet";
                    if (isPartyLedger) {
                        const grid2 = $("#SubGridContainer").dxDataGrid("instance");
                        if (grid2) grids.push({ instance: grid2, title: "Mpo Layout Summary" });
                    }

                    // ---------- PREPARE GRIDS DATA ----------
                    const requestGrids = grids.map(g => {
                        const grid = g.instance;

                        const visibleCols = grid.getVisibleColumns().map(c => ({
                            dataField: c.dataField,
                            caption: c.caption || c.dataField
                        }));

                        const groupCols = grid.option("columns")
                            .filter(c => c.groupIndex !== undefined && c.groupIndex >= 0)
                            .sort((a, b) => a.groupIndex - b.groupIndex)
                            .map(c => c.dataField);

                        //const groupColsCap = grid.option("columns")
                        //    .filter(c => c.groupIndex !== undefined && c.groupIndex >= 0)
                        //    .sort((a, b) => a.groupIndex - b.groupIndex)
                        //    .map(c => c.caption || c.dataField);

                        const groupColsCap = grid.getVisibleColumns()
                            .filter(c => c.groupIndex !== undefined && c.groupIndex >= 0)
                            .sort((a, b) => a.groupIndex - b.groupIndex)
                            .map(c => c.caption || c.dataField);
                        debugger;
                        const totals = {};
                        (grid.option("summary.totalItems") || []).forEach(s => {
                            totals[s.column] = grid.getTotalSummaryValue(s.column);
                        });

                        const data = (grid.option("dataSource") || []).map(row => {
                            const filteredRow = {};
                            visibleCols.forEach(col => {
                                filteredRow[col.caption] = row[col.dataField];
                            });
                            return filteredRow;
                        });

                        debugger;
                        return {
                            GridTitle: g.title,
                            GridData: data,
                            GroupColumnsCap: groupColsCap,
                            GroupColumns: groupCols,
                            Totals: totals,
                            IsLandscape: isLandscape
                        };
                    });

                    // AJAX call shuru hone se theek PEHLE hi tab khol lein (isey browser block nahi karega)
                    const pdfWindow = window.open("", "_blank");
                    if (pdfWindow) {
                        pdfWindow.document.write("<p style='font-family:sans-serif; text-align:center; margin-top:20%;'>Generating Your PDF, please wait...</p>");
                    }

                    $.ajax({
                        url: '/Report/GeneratePDF',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            CompanyName: empr_helper.companyName,
                            ReportName: empr_helper.reportName,
                            lastDate: empr_helper.lastDate || null,
                            lastAmount: empr_helper.lastAmt || null,
                            From: empr_helper.formatDateTopdf(empr_helper.fromDate),
                            To: empr_helper.formatDateTopdf(empr_helper.toDate),
                            IsLandscape: isLandscape,
                            Grids: requestGrids
                        }),
                        xhrFields: { responseType: 'blob' },
                        timeout: 300000,
                        success: function (result) {
                            console.log('PDF GENERATED');
                            const url = URL.createObjectURL(result);

                            // Agar tab successfully khula tha, toh usme PDF load kar dein
                            if (pdfWindow && !pdfWindow.closed) {
                                pdfWindow.location.href = url;
                            } else {
                                // Fallback: Agar phir bhi user ne pehle wala close kar diya ho
                                window.open(url, '_blank');
                            }

                            $("#Loader").hide();
                        },
                        error: function (xhr, status, error) {
                            console.error("Status: ", status);
                            // Agar error aaye toh khule hue blank tab ko band kar dein
                            if (pdfWindow) pdfWindow.close();

                            alert("PDF generation failed!");
                            $("#Loader").hide();
                        }
                    });

                } else {
                    // ---------- EXCEL EXPORT ----------
                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet(fileName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                        customizeCell: function (options) {
                            const { gridCell, excelCell } = options;
                            const targetFields = [
                                'debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc',
                                'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt',
                                'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales',
                                'cash', 'cardType', 'party'
                            ];

                            if (gridCell.rowType !== 'header' && targetFields.includes(gridCell.column.dataField)) {
                                if (typeof gridCell.value === 'number') {
                                    excelCell.numFmt = '#,##0';
                                }
                            }
                        }
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then(buffer => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                        });
                    });
                }
            },

            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (e.rowType === 'group' && (e.column.dataField === 'accountName' || e.column.dataField === 'itemId' || e.column.dataField === 'bTransfer' || e.column.dataField === 'vDate'
                    || e.column.dataField === 'itemName' || e.column.dataField === 'category' || e.column.dataField === 'subCategory' || e.column.dataField === 'barcode' || e.column.dataField === ''
                    || e.column.dataField === 'bType')) {
                    e.cellElement.css({
                        'font-weight': '650',
                        position: "sticky",
                        top: "0px",
                        background: "#fff",
                        zIndex: 5,
                        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
                    });
                }
                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900' || value == '01/01/1900' || value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '1/1/2000' || value == '01/01/2000')) {
                    $(e.cellElement).text('')
                }

                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'tax', 'taxAmt', 'adV_TAX', 'adV_TAX_AMT', 'netAmt', 'totalBalance',
                    'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party', 'qty', 'rate', 'disc',
                    'balanced', 'totaL_QTY', 'totaL_R_PRICE', 'totaL_DISC', 'totaL_PRICE', 'totaL_WS_PRICE', 'stkT_WS_PRICE', 'totaL_EXP', 'factorY_TRANSFER', 'casH_SALE', 'banK_W_CH',
                    'neT_PROFIT1', 'totaL_COST', 'neT_PROFIT2'].includes(column.dataField)) {
                    if (value === 0) {
                        $(e.cellElement).text('');
                    } else if (value !== null && value !== undefined && !isNaN(value)) {
                        if (value < 0) {
                            $(e.cellElement).text(`(${Math.abs(value).toLocaleString('en-US')})`);
                            $(e.cellElement).css('color', 'red');
                        } else {
                            $(e.cellElement).text(Number(value).toLocaleString('en-US'));
                            $(e.cellElement).css('color', 'black');
                        }
                    }
                }
                if (e.rowType === 'groupFooter' && e.column.dataField === 'debit') {
                    const groupIndex = e.row.groupIndex;
                    if (groupIndex !== 0) {
                        e.cellElement.text('');
                    }
                }

            },

            summary: {
                recalculateWhileEditing: true,
                groupItems: [
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "aging_0_15",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_16_30",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_31_45",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_46_60",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_61_90",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_91_120",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_120_Plus",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "balanceWithTotal",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "commission",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return Math.round(e.value).toLocaleString("en-US");
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    //{
                    //    column: "balance2",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    showInGroupFooter: true,
                    //    valueFormat: '#,##0',
                    //    customizeText: function (e) {
                    //        return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                    //    }
                    //},
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balanced",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },

                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    //{
                    //    column: "disc",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    showInGroupFooter: true,
                    //    valueFormat: {
                    //        type: "fixedPoint",
                    //        precision: 0,
                    //    }
                    //},
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "netAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "deliveryCharges",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "pack",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "totalPack",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "weight",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'

                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },

                    {
                        column: "taxAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },
                    {
                        column: "adV_TAX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0'
                    },


                    {
                        name: "customPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "TotalmaountAndLabel",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "balanced",  // exact field name
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (e.value == null || e.value === 0) return "";

                            // Final text already assigned in finalize stage
                            return e.text || e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customAvgSummary",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "itemName", // show result in 'balanced' column footer 
                        displayFormat: "Avg Total: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                ],
                totalItems: [
                    {
                        name: "customPriceTotal",
                        showInColumn: "price",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "aging_0_15",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_16_30",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_31_45",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_46_60",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_61_90",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_91_120",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "aging_120_Plus",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "balanceWithTotal",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        valueFormat: '#,##0.00',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                        }
                    },
                    {
                        column: "accountName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.typeOneTotal != 0 || empr_helper.typeTwoTotal != 0) {
                                var grossSales = empr_helper.typeOneTotal + empr_helper.typeTwoTotal;
                                var grossProfit = grossSales - empr_helper.typeThreeTotal;
                                var netProfit = grossProfit - empr_helper.typeFourTotal;
                                return `Gross Sales : ${grossSales.toLocaleString('en-US')} --------------- Gross Profit : ${grossProfit.toLocaleString('en-US')} --------------- Net Profit : ${netProfit.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        name: "customWPriceTotal",
                        showInColumn: "wholePrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        name: "customSPriceTotal",
                        showInColumn: "shopPrice",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "buyerName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.debitAverage > 0) {
                                return "Stock In Avg : " + empr_helper.debitAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "itemName",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.creditAverage > 0) {
                                return "Stock Out Avg : " + empr_helper.creditAverage.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "totaL_STOCK_IN",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totaL_STOCK_OUT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "sPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wPrice",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "debit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (empr_helper.typeOneTotal == 0 || empr_helper.typeTwoTotal == 0) {
                                return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                            }
                            return "";
                        }
                    },
                    {
                        column: "credit",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "amount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "balance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "dueAmount",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "voucherDate",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Date : ${empr_helper.lastDate}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "voucherNo",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.lastAmt > 0 || empr_helper.lastDate != '') {
                                return `Last Amount : ${empr_helper.lastAmt.toLocaleString('en-US')}`;
                            }
                            return "";
                        }
                    },
                    {
                        column: "accountDescription",
                        summaryType: "custom",
                        customizeText: function (e) {
                            if (empr_helper.balanceAmount != "") {
                                if (empr_helper.balanceAmount > 0) {
                                    return "Debit Balance : " + empr_helper.balanceAmount.toLocaleString('en-US');
                                } else {
                                    return `Credit Balance : (${new Intl.NumberFormat().format(Math.abs(empr_helper.balanceAmount))})`;
                                }
                            }
                            return "";
                        }
                    },
                    {
                        column: "stockIn",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "stockOut",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    //{
                    //    column: "netAmt",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    valueFormat: '#,##0',
                    //    customizeText: function (e) {
                    //        return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                    //    }
                    //},
                    {
                        column: "total",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "deliveryCharges",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "pAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "ins",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    //{
                    //    column: "qty",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}"
                    //},
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "oqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "iqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "bqty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        customizeText: function (e) {
                            if (!e.value || e.value === 0) return "";
                            return e.value < 0
                                ? `(${Math.abs(e.value).toLocaleString('en-US')})`
                                : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "avg",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        customizeText: function (e) {
                            if (!e.value || e.value === 0) return "";
                            return e.value < 0
                                ? `(${Math.abs(e.value).toLocaleString('en-US')})`
                                : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "posQty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "totalSales",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cash",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "cardType",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "party",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "wAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString('en-US');
                        }
                    },
                    {
                        column: "mDisc_Amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "taxAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    //{
                    //    column: "taxAmt",
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    valueFormat: "#,##0",
                    //    //customizeText: function (e) {
                    //    //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                    //    //}
                    //    customizeText: function (e) {
                    //        return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                    //    }
                    //},
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "inclTax",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        //customizeText: function (e) {
                        //    return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        //}
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US", { maximumFractionDigits: 0 });
                        }
                    },
                    {
                        column: "cset",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            return e.value === 0 ? "" : e.value.toLocaleString("en-US");
                        }
                    },
                    //{
                    //    column: "disc", //disc footer total
                    //    summaryType: "sum",
                    //    displayFormat: "{0}",
                    //    valueFormat: {
                    //        type: "fixedPoint",
                    //        precision: 0,
                    //    }
                    //},
                    {
                        column: "adV_TAX_AMT", //adV_TAX_AMT footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    //============================ totaL_DISC
                    {
                        column: "totaL_QTY", //totaL_QTY footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "totaL_R_PRICE", //totaL_R_PRICE footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "totaL_DISC", //totaL_QTY footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "totaL_PRICE", //totaL_R_PRICE footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "totaL_WS_PRICE", //totaL_QTY footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "stkT_WS_PRICE", //totaL_R_PRICE footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "totaL_EXP", //totaL_QTY footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "factorY_TRANSFER", //totaL_R_PRICE footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "casH_SALE", //totaL_QTY footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "banK_W_CH", //totaL_R_PRICE footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "neT_PROFIT1",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0
                        },
                        customizeText: function (e) {
                            if (e.value < 0) {
                                return "(" + Math.abs(e.value) + ")";
                            }
                            return e.valueText;
                        }
                    },
                    {
                        column: "totaL_COST", //totaL_QTY footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "neT_PROFIT2", //totaL_R_PRICE footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },







                    {
                        column: "netAmt", //netAmt footer total
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },

                    {
                        column: "returnAmt",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "returnDisc",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        column: "disC_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}"
                    },
                    {
                        column: "taX_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "pack",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "totalPack",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "weight",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 2,
                        }
                    },
                    {
                        column: "commission",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: "#,##0",
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return Math.round(e.value).toLocaleString("en-US");
                        }
                    },
                    {
                        column: "totalBalance",
                        summaryType: "sum",
                        displayFormat: "{0}",
                        valueFormat: {
                            type: "fixedPoint",
                            precision: 0,
                        }
                    },
                    {
                        name: "customAvgSummary",
                        showInColumn: "itemNameWithAvg", // show result in 'balanced' column footer 
                        displayFormat: "Avg Total: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                    {
                        name: "customBalanceSummary",
                        showInColumn: "balanced", // show result in 'balanced' column footer
                        displayFormat: "Closing Balance: {0}",
                        valueFormat: "#,##0.##",
                        summaryType: "custom"
                    },
                    {
                        name: "netTotal",
                        showInColumn: "balances",
                        summaryType: "custom",
                        displayFormat: "Net Profit: {0}",
                        valueFormat: '#,##0'
                    }
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "customPriceTotal" || options.name === "customPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.price * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customWPriceTotal" || options.name === "customWPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.wholePrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customSPriceTotal" || options.name === "customSPriceTotalGroup") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            options.totalValue += options.value.shopPrice * options.value.totalBalance;
                        }
                    }
                    if (options.name === "customBalanceSummary") {
                        if (options.summaryProcess === "start") {
                            options.totalA = 0;
                            options.totalS = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            const vcType = options.value.vctype;
                            const balance = parseFloat(options.value.balanced) || 0;

                            if (vcType === "A") {
                                options.totalA += balance;
                            }
                            if (vcType === "S") {
                                options.totalS += balance;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            options.totalValue = options.totalA - options.totalS;
                            empr_ClosingShop.ClosingBalance = options.totalA - options.totalS;
                        }
                    }
                    if (options.name === "customAvgSummary") {
                        if (options.summaryProcess === "start") {
                            options.totalAmt = 0;
                            options.totalQty = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            const amt = parseFloat(options.value.amt) || 0;
                            const qty = parseFloat(options.value.oqty) || 0;

                            options.totalAmt += amt;
                            options.totalQty += qty;
                        }
                        if (options.summaryProcess === "finalize") {
                            if (options.totalQty !== 0) {
                                options.totalValue = options.totalAmt / options.totalQty;
                            } else {
                                options.totalValue = 0;
                            }

                            // Format the value to 2 decimal places
                            options.totalValue = parseFloat(options.totalValue.toFixed(2));
                            console.log(options.totalValue);
                        }
                    }
                    if (options.name === "netTotal") {
                        if (options.summaryProcess === "start") {
                            options.revenueTotal = 0;
                            options.expenseTotal = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            console.log("accountCode:" + options.value.accountCode);
                            if (options.value.accountCode === 4) {
                                options.revenueTotal += options.value.balances || 0;
                            }
                            else if (options.value.accountCode === 5) {
                                options.expenseTotal += options.value.balances || 0;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            var Profit = options.revenueTotal - options.expenseTotal;
                            options.totalValue = Profit;
                        }
                    }
                    if (options.name === "TotalmaountAndLabel") {
                        if (options.summaryProcess === "start") {
                            options.assetsTotal = 0;
                            options.liabilitiesTotal = 0;
                        }

                        if (options.summaryProcess === "calculate") {
                            // Row-wise calculation
                            if (options.value.accountCode === 1) { // 1 = Assets
                                options.assetsTotal += options.value.balance || 0;
                            }
                            else if (options.value.accountCode === 2) { // 2 = Liabilities
                                options.liabilitiesTotal += options.value.balance || 0;
                            }
                        }

                        if (options.summaryProcess === "finalize") {
                            // Final totals ke liye text assign karo
                            if (options.summaryIndex === 0) { // first summary row
                                options.totalValue = options.assetsTotal;
                                options.text = 'Assets: ' + options.assetsTotal.toLocaleString('en-US');
                            }
                            else if (options.summaryIndex === 1) { // second summary row
                                options.totalValue = options.liabilitiesTotal;
                                options.text = 'Liabilities: ' + options.liabilitiesTotal.toLocaleString('en-US');
                            }
                        }
                    }
                }
            }
        }).dxDataGrid('instance');
    },

    DxGridBindingForReportsWithSetting_IncomeReport: function (div, columns, datasrc, fileName, isLandscape = false) {
        debugger;
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            // Your existing configuration
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 500,
            "allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "allowColumnReordering": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 300,
                "enabled": true
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            grouping: {
                autoExpandAll: true
            },
            "stateStoring": {
                //"enabled": true,
                //"type": 'localStorage',
                //"storageKey": fileName
                enabled: true,
                type: 'custom',
                customLoad: function () {
                    const savedState = localStorage.getItem(fileName);
                    if (savedState) {
                        const state = JSON.parse(savedState);
                        return {
                            columns: state.columns || []
                        };
                    }
                    return null;
                },
                customSave: function (state) {
                    const stateToSave = {
                        columns: state.columns.map(col => ({
                            dataField: col.dataField,
                            visible: col.visible,
                            visibleIndex: col.visibleIndex,
                            groupIndex: col.groupIndex,
                            width: col.width
                        }))
                    };
                    localStorage.setItem(fileName, JSON.stringify(stateToSave));
                }
            },
            showBorders: true,
            onToolbarPreparing: function (e) {
                e.toolbarOptions.items.unshift({
                    location: 'after',
                    widget: 'dxButton',
                    options: {
                        icon: 'refresh',
                        text: '',
                        onClick: function () {
                            localStorage.removeItem(fileName);
                            location.reload();
                        }
                    }
                });
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    var layout;
                    if (isLandscape) {
                        layout = 'landscape';
                    } else {
                        layout = 'portrait';
                    }
                    const doc = new jsPDF({
                        orientation: layout, // or 'landscape' depending on your requirement
                        unit: 'pt', // units of measurement: points
                        format: 'a4' // set the page size to A3
                    });

                    function addHeader() {
                        const pageWidth = doc.internal.pageSize.getWidth() - 27;
                        const headerHeight = 60;

                        doc.setFont("helvetica", "bold");
                        doc.setFontSize(15);
                        doc.setTextColor(5, 90, 135);
                        doc.text(empr_helper.companyName, 20, 20);

                        doc.setFont("helvetica", "normal");
                        doc.setFontSize(12);
                        doc.text(empr_helper.reportName, 20, 40);
                        doc.setFillColor(5, 90, 135);
                        doc.rect(18, 47, pageWidth, 39, 'F');
                        doc.setFillColor(255, 255, 255);
                        doc.rect(18, 66, pageWidth, 0.5, 'F');

                        doc.setFontSize(10);
                        doc.setTextColor(255, 255, 255);
                        doc.text(`From : ${empr_helper.formatDate(empr_helper.fromDate)}       To: ${empr_helper.formatDate(empr_helper.toDate)}`, 20, 60);
                        const now = new Date();
                        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
                        const currentDate = now.toLocaleDateString('en-GB', options);
                        const currentTime = now.toLocaleTimeString();
                        doc.text(`Printed Date : ${currentDate}       Time : ${currentTime}`, 20, 80);
                        doc.setTextColor(0, 0, 0);
                    }
                    addHeader();

                    DevExpress.pdfExporter.exportDataGrid({
                        jsPDFDocument: doc,
                        component: e.component,
                        indent: 5,
                        margin: {
                            //top: 40,
                            top: 100,
                            right: 10,
                            bottom: 40,
                            left: 10,
                        },
                        topLeft: { x: 5, y: 0 },
                        customizeCell: function (cellInfo) {
                            if (cellInfo && cellInfo.gridCell && cellInfo.gridCell.column) {


                                if (cellInfo.gridCell.value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.wordWrapEnabled = true;
                                cellInfo.pdfCell.text = cellInfo.pdfCell.text || '';
                                cellInfo.pdfCell.font = {
                                    size: 9
                                };

                                cellInfo.pdfCell.padding = {
                                    top: 2,
                                    right: 2,
                                    bottom: 2,
                                    left: 2
                                };

                                let value = cellInfo.gridCell.value;
                                const column = cellInfo.gridCell.column;

                                if (column.dataField === "accountNature" && value != null) {
                                    const map = {
                                        1: "CASH A/C",
                                        2: "BANK A/C",
                                        3: "VENDOR",
                                        4: "CUSTOMER",
                                        5: "SALES",
                                        6: "SALES RETURN",
                                        7: "PURCHASE",
                                        7.2: "Gross Total",
                                        8: "PURCHASE RETURN",
                                        9: "SALES PERSON",
                                        10: "TRANSPOTERS",
                                        11: "Fixed Assets",
                                        15: "Stiching Unit",
                                        20: "OTHERS",
                                        21: "LABOUR",
                                        22: "Cash Sales",
                                        23: "Cash Sales Return",
                                        24: "Advance",
                                        25: "Transfer Shop",
                                        26: "Expenses",
                                        27: "Shop Expenses"
                                    };

                                    cellInfo.pdfCell.text = map[value] || value;
                                    //cellInfo.pdfCell.textAlign = 'center';
                                    cellInfo.pdfCell.horizontalAlign = 'center';
                                }

                                if (column.caption.toLowerCase().includes('trans')) {
                                    cellInfo.pdfCell.textColor = '#055a87';
                                }

                                if (cellInfo.gridCell.rowType === 'header') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 9 };
                                    cellInfo.pdfCell.textColor = 'black';
                                }

                                if (cellInfo.gridCell.rowType === 'group') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 9 };
                                }

                                if (cellInfo.gridCell.rowType === 'groupFooter' || cellInfo.gridCell.rowType === 'totalFooter') {
                                    cellInfo.pdfCell.font = { style: 'bold', size: 9 };
                                }

                                if (value instanceof Date) {
                                    const year = value.getFullYear();
                                    const month = String(value.getMonth() + 1).padStart(2, '0');
                                    const day = String(value.getDate()).padStart(2, '0');
                                    value = `${year}-${month}-${day}`;
                                }

                                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) &&
                                    (value === '1900-01-01' || value === '01-01-1900' || value === '01-Jan-1900' || value === '1/1/1900 12:00:00 AM' || value === '1/1/1900' || value === '01/01/1900' || value === '2000-01-01' || value === '01-01-2000' || value === '01-Jan-2000' || value === '1/1/2000 12:00:00 AM' || value === '1/1/2000' || value === '01/01/2000' || value === '28/01/2000')) {
                                    cellInfo.pdfCell.text = '';
                                }

                                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party'].includes(column.dataField)) {
                                    if (cellInfo.gridCell.rowType !== 'header') {

                                        const formattedValue = Number(Math.abs(value)).toLocaleString(); // comma separated

                                        //if (value < 0) {
                                        //    cellInfo.pdfCell.text = `(${Math.abs(value)})`;
                                        //    cellInfo.pdfCell.textColor = 'red';
                                        //} else {
                                        //    cellInfo.pdfCell.text = formattedValue;
                                        //    cellInfo.pdfCell.textColor = 'black';
                                        //}
                                        if (value == null || value === '' || isNaN(value)) {
                                            cellInfo.pdfCell.text = '';
                                        } else if (value < 0) {
                                            const formattedNegative = Math.abs(value).toLocaleString(); // comma separated
                                            cellInfo.pdfCell.text = `(${formattedNegative})`;
                                            cellInfo.pdfCell.textColor = 'red';
                                        } else {
                                            const formattedPositive = Number(value).toLocaleString();
                                            cellInfo.pdfCell.text = formattedPositive;
                                            cellInfo.pdfCell.textColor = 'black';
                                        }



                                    }
                                }

                                if (['debit', 'credit', 'balance'].includes(column.dataField) && value === 0) {
                                    cellInfo.pdfCell.text = '';
                                }

                                cellInfo.pdfCell.borderColor = 'black';
                            }
                        }
                    }).then(function () {
                        const pageCount = doc.internal.getNumberOfPages();
                        for (let i = 1; i <= pageCount; i++) {
                            doc.setPage(i);

                            addHeader();

                            doc.setFontSize(10);
                            doc.text(`Page ${i} of ${pageCount}`, doc.internal.pageSize.getWidth() / 2, doc.internal.pageSize.getHeight() - 20, { align: 'center' });
                        }
                        const pdfBlob = doc.output('blob');
                        const blobUrl = URL.createObjectURL(pdfBlob);
                        window.open(blobUrl);
                    });
                } else {
                    //const workbook = new ExcelJS.Workbook();
                    //const worksheet = workbook.addWorksheet(fileName);

                    //DevExpress.excelExporter.exportDataGrid({
                    //    component: e.component,
                    //    worksheet,
                    //    autoFilterEnabled: true,
                    //}).then(() => {
                    //    workbook.xlsx.writeBuffer().then((buffer) => {
                    //        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                    //    });
                    //});

                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet(fileName);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                        customizeCell: function (options) {
                            const { gridCell, excelCell } = options;

                            // Target columns for formatting
                            const targetFields = [
                                'debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc',
                                'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt',
                                'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales',
                                'cash', 'cardType', 'party'
                            ];

                            if (gridCell.rowType !== 'header' && targetFields.includes(gridCell.column.dataField)) {
                                if (typeof gridCell.value === 'number') {
                                    // Apply comma formatting with 2 decimals
                                    excelCell.numFmt = '#,##0';

                                }
                            }
                        }
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then((buffer) => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                        });
                    });

                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;
                if (e.rowType === 'group' && (e.column.dataField === 'accountName' || e.column.dataField === 'itemId' || e.column.dataField === 'bTransfer' || e.column.dataField === 'vDate'
                    || e.column.dataField === 'itemName' || e.column.dataField === 'category' || e.column.dataField === 'subCategory' || e.column.dataField === 'barcode' || e.column.dataField === ''
                    || e.column.dataField === 'bType')) {
                    e.cellElement.css({
                        'font-weight': '650',
                        position: "sticky",
                        top: "0px",
                        background: "#fff",
                        zIndex: 5,
                        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
                    });
                }
                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900' || value == '01/01/1900' || value == '2000-01-01' || value == '01-01-2000' || value == '01-Jan-2000' || value == '1/1/2000 12:00:00 AM' || value == '1/1/2000' || value == '01/01/2000')) {
                    $(e.cellElement).text('')
                }

                if (['debit', 'credit', 'balance', 'balance2', 'amt', 'rate', 'posQty', 'disc', 'mDisc_Amt', 'netAmt', 'totalBalance', 'stock', 'profitAndLoss', 'pAmt', 'pbRate', 'wRate', 'wAmt', 'cashTax', 'bankTax', 'partyTax', 'totalSales', 'cash', 'cardType', 'party', 'qty', 'rate', 'disc', 'balanced'].includes(column.dataField)) {
                    if (value === 0) {
                        $(e.cellElement).text('');
                    } else if (value !== null && value !== undefined && !isNaN(value)) {
                        if (value < 0) {
                            $(e.cellElement).text(`(${Math.abs(value).toLocaleString('en-US')})`);
                            $(e.cellElement).css('color', 'red');
                        } else {
                            $(e.cellElement).text(Number(value).toLocaleString('en-US'));
                            $(e.cellElement).css('color', 'black');
                        }
                    }
                }
                if (e.rowType === 'groupFooter' && e.column.dataField === 'debit') {
                    const groupIndex = e.row.groupIndex;
                    if (groupIndex !== 0) {
                        e.cellElement.text('');
                    }
                }

            },
            summary: {
                recalculateWhileEditing: true,
                groupItems: [
                    {
                        name: "customWPriceTotal",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        showInColumn: "credit",
                        displayFormat: "{0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            var name = e.totalDisplayName || "Total: ";
                            return e.value === 0 ? "" : name + e.value.toLocaleString('en-US');
                        }
                    },
                ],
                totalItems: [
                    //{
                    //    name: "customNetProfitTotal",
                    //    alignByColumn: true,
                    //    showInColumn: "credit",
                    //    displayFormat: "Net Profit: {0}",
                    //    summaryType: "custom",
                    //    valueFormat: '#,##0',
                    //    //customizeText: function (e) {
                    //    //    var name = "Net Profit: ";
                    //    //    return e.value === 0 ? "" : name + e.value.toLocaleString('en-US');
                    //    //}
                    //},
                    {
                        name: "customNetProfitTotal",
                        alignByColumn: true,
                        showInColumn: "credit",
                        displayFormat: "Net Profit: {0}",
                        summaryType: "custom",
                        valueFormat: '#,##0',
                        customizeText: function (e) {
                            if (e.value === 0) return "";
                            return "Net Profit: " +
                                (e.value < 0
                                    ? "(" + Math.abs(e.value).toLocaleString('en-US') + ")"
                                    : e.value.toLocaleString('en-US'));
                        }
                    },
                ],
                calculateCustomSummary: function (options) {
                    if (options.name === "customWPriceTotal") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                            options.totalDisplayName = "";
                        }
                        if (options.summaryProcess === "calculate") {
                            var credit = options.value.credit || 0;
                            var debit = options.value.debit || 0;
                            // accountNature mapping: 5 = SALES, 6 = SALES RETURN
                            if (options.value.accountNature === 5) {
                                options.totalValue += credit - debit;
                                options.totalDisplayName = "TOTAL SALES:";
                            } else if (options.value.accountNature === 6) {
                                options.totalValue += debit - credit;
                                options.totalDisplayName = "TOTAL SALES RETURN:";
                            } else if (options.value.accountNature === 7) {
                                options.totalValue += debit - credit;
                                options.totalDisplayName = "TOTAL PURCHASE:";
                            } else if (options.value.accountNature === 26) {
                                options.totalValue += debit;
                                options.totalDisplayName = "TOTAL EXPENSES:";
                            }

                        }
                        if (options.summaryProcess === "finalize") {
                            options.totalValue = Number(options.totalValue || 0);
                            options.totalDisplayName = options.totalDisplayName || "Total: ";
                        }
                    }
                    if (options.name === "customNetProfitTotal") {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                            // initialize accumulators
                            options._sale = 0;
                            options._saleReturn = 0;
                            options._purchase = 0;
                            options._expenses = 0;
                        }
                        if (options.summaryProcess === "calculate") {
                            var credit = options.value.credit || 0;
                            var debit = options.value.debit || 0;

                            if (options.value.accountNature === 5) {
                                options._sale += credit - debit;
                            }
                            if (options.value.accountNature === 6) {
                                options._saleReturn += debit - credit;
                            }
                            if (options.value.accountNature === 7) {
                                options._purchase += debit - credit;
                            }
                            if (options.value.accountNature === 26) {
                                options._expenses += debit;
                            }
                        }
                        if (options.summaryProcess === "finalize") {
                            var grossTotal = (options._sale - options._saleReturn) - options._purchase;
                            options.totalValue = grossTotal - options._expenses;
                        }
                    }
                }

            }
        }).dxDataGrid('instance');
    },

    formatDate(date) {
        const options = { year: 'numeric', month: 'numeric', day: 'numeric' };
        return new Date(date).toLocaleDateString('en-GB', options);
    },

    formatDateTopdf(d) {
        if (!d || d === "1900-01-01") return "";
        let x = new Date(d);
        return isNaN(x) ? "" :
            String(x.getDate()).padStart(2, '0') + "-" +
            String(x.getMonth() + 1).padStart(2, '0') + "-" +
            x.getFullYear();
    },


    isValidKey(e) {
        var keyCode = e.keyCode || e.which;
        // Allow numeric characters (0-9), backspace (8), delete (46), and dot (190 or 110)
        return (keyCode >= 48 && keyCode <= 57) || keyCode === 8 || keyCode === 46 || keyCode === 190 || keyCode === 110;
    },

    dxGridBindingWithSearch: function (div, columns, datasrc, fileName) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            //"allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": 'multiple',
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            //customizeText(cellInfo) {
            //    const column = cellInfo.column;
            //    const value = cellInfo.value;

            //    // Check if the column name contains "date" and value is default
            //    if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        return '';
            //    }

            //    // If not a date column or not the default date value, return the original value
            //    return value;
            //}
            //onCellPrepared: function (e) {
            //    if (e.rowType === "data" && e.column.dataField.toLower().includes('data')) {

            //    }
            //},

            //onEditorPreparing: function (e) {
            //    if (e.dataField.toLower().includes('date') && e.parentType === "dataRow") {
            //        const defaultValueChangeHandler = e.editorOptions.onValueChanged;
            //        e.editorOptions.onValueChanged = function (args) { // Override the default handler
            //            // ...
            //            // Custom commands go here
            //            // ...
            //            // If you want to modify the editor value, call the setValue function:
            //            // e.setValue(newValue);
            //            // Otherwise, call the default handler:
            //            defaultValueChangeHandler(args);
            //        }
            //    }
            //cellTemplate(container, options) {
            //    const column = options.column;
            //    const value = options.value;

            //    // Check if the column name contains "date"
            //    if (value && value.getTime() === new Date('01-01-1900').getTime()) {
            //        container.innerText = '';
            //    } else {
            //        container.innerText = value;
            //    }
            //}
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');



    },

    GetCurrentDate: function () {
        var today = new Date();
        var day = today.getDate();
        var month = today.getMonth() + 1;
        var year = today.getFullYear();

        day = (day < 10) ? '0' + day : day;
        month = (month < 10) ? '0' + month : month;
        var formatted_date = year + '-' + month + '-' + day;
        //console.log("Today's date is:", formatted_date);
        return formatted_date;
    },

    PrepareDate: function (date) {
        var today = new Date(date);
        var day = today.getDate();
        var month = today.getMonth() + 1;
        var year = today.getFullYear();

        day = (day < 10) ? '0' + day : day;
        month = (month < 10) ? '0' + month : month;
        var formatted_date = year + '-' + month + '-' + day;
        //console.log("Today's date is:", formatted_date);
        return formatted_date;
    },

    PrepareTime: function (date) {
        var time = new Date(date);
        var hours = time.getHours();
        var minutes = time.getMinutes();
        var seconds = time.getSeconds();

        // Pad with leading zero if needed
        hours = (hours < 10) ? '0' + hours : hours;
        minutes = (minutes < 10) ? '0' + minutes : minutes;
        seconds = (seconds < 10) ? '0' + seconds : seconds;

        var formatted_time = hours + ':' + minutes + ':' + seconds;
        console.log("Time is:", formatted_time);
        return formatted_time;
    },

    HandleTabFocus: function (event, target) {
        if (event.key === 'Tab') {
            event.preventDefault();
            $(target).focus();
        }
    },

    HandleTabFocusToSelectBox: function (event, element) {
        if (event.key === 'Tab') {
            event.preventDefault();
            $(element).data('dxSelectBox').focus()
        }
    },

    FormatYear: function (input) {
        const dateValue = input.value;
        if (dateValue) {
            const parts = dateValue.split("-");
            let year = parseInt(parts[0], 10);
            if (year < 100) {
                year += 2000;
                input.value = `${year}-${parts[1]}-${parts[2]}`;
            }
        }
    },

    MoveFocusToGrid: function (event, gridElement, rowIndex, dataField) {
        if (event.key === 'Tab') {
            event.preventDefault();
            var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
            $(nextElement).click();
            $(gridElement).dxDataGrid('instance').focus(nextElement);
        }
    },

    MoveFocusToGridShiftTab: function (event, gridElement, rowIndex, dataField) {
        if (event.key === 'Tab') {
            event.preventDefault();
            // Check if Shift is pressed
            if (event.shiftKey) {
                $('#Condition').data('dxSelectBox').focus(); // Modify as needed for reverse navigation
            } else {
                var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
                $(nextElement).click();
                $(gridElement).dxDataGrid('instance').focus(nextElement);
            }
        }
    },

    MoveFocusToGridWithouTab: function (gridElement, rowIndex, dataField) {
        var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
        $(nextElement).click();
        $(gridElement).dxDataGrid('instance').focus(nextElement);
    },

    EnableShortCutKeys: function (saveElement, newElement, deleteElement, quickSearchElement, idElement, gridElement, rowIndex, dataField, sodaPickModalElement, sodaPickModalOpenButtonElement, sodaPickGridElement, sodaPickGridRowIndex, sodaPickGridDataField, sodaPickButtonElement) {
        $(document).keydown(function (e) {
            //if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
            //    console.log('CTRL+N is pressed');
            //    e.preventDefault();
            //    $(newElement).click();
            //    return false;
            //}

            if ((e.altKey || e.metaKey) && e.key === 'n') {
                console.log('ALT+N is pressed');
                e.preventDefault();
                $(newElement).click();
                return false;
            }

            if ((e.ctrlKey || e.metaKey) && e.key === 's') {
                console.log('CTRL+S is pressed');
                e.preventDefault();
                $(saveElement).click();
                return false;
            }

            if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
                console.log('CTRL+D is pressed');
                e.preventDefault();
                if ($(idElement).val() != '' && $(idElement).val() != null && $(idElement).val() != undefined) {
                    $(deleteElement).click();
                }
                return false;
            }

            if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
                console.log('CTRL+F is pressed');
                e.preventDefault();
                $(quickSearchElement).click();
                return false;
            }

            //if ((e.altKey || e.metaKey) && e.key === 'n') {
            //    console.log('ALT+N is pressed');
            //    e.preventDefault();
            //    $(newElement).click();
            //    return false;
            //}

            //if ((e.altKey || e.metaKey) && e.key === 's') {
            //    console.log('ALT+S is pressed');
            //    e.preventDefault();
            //    $(saveElement).click();
            //    return false;
            //}

            //if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
            //    console.log('ALT+D is pressed');
            //    e.preventDefault();
            //    if ($(idElement).val() != '' || $(idElement).val() != null || $(idElement).val() != undefined) {
            //        $(deleteElement).click();
            //    }
            //    return false;
            //}

            //if ((e.altKey || e.metaKey) && e.key === 'f') {
            //    console.log('ALT+F is pressed');
            //    e.preventDefault();
            //    $(quickSearchElement).click();
            //    return false;
            //}

            //if ((e.ctrlKey || e.metaKey) && e.key === 'c') {
            //    console.log('CTRL+C is pressed');
            //    e.preventDefault();

            //    return false;
            //}

            if (e.key === 'Enter') {
                console.log('Enter is pressed');
                e.preventDefault();
                $($('a.grid-action-icon[title="Add"]')[0]).click();
                setTimeout(function () {
                    var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
                    $(nextElement).click();
                    $(gridElement).dxDataGrid('instance').focus(nextElement);
                    // Set focus to the desired column in edit mode
                    //$(gridElement).dxDataGrid('instance').editCell(rowIndex, dataField);
                }, 1500);
                return false;
            }

            //if (e.key === 'Delete') {
            //    console.log('Delete is pressed');
            //    e.preventDefault();

            //    return false;
            //}

            if (sodaPickModalElement != '' && sodaPickModalElement != null && sodaPickModalElement != undefined) {
                if (e.key === 'F1') {
                    console.log('F1 is pressed');
                    e.preventDefault();
                    $(sodaPickModalOpenButtonElement).click();
                    setTimeout(function () {
                        var nextElement = $(sodaPickGridElement).dxDataGrid('instance').getCellElement(sodaPickGridRowIndex, sodaPickGridDataField);
                        $(nextElement).click();
                        $(sodaPickGridElement).dxDataGrid('instance').focus(nextElement);
                    }, 1500);
                    return false;
                }
            }

            //if (sodaPickModalElement != '' && sodaPickModalElement != null && sodaPickModalElement != undefined) {
            //    if ((e.ctrlKey || e.metaKey) && e.key === 'a') {
            //        console.log('CTRL+A is pressed');
            //        e.preventDefault();
            //        $(sodaPickButtonElement).click();
            //        return false;
            //    }
            //}
        });
    },

    MasterDetailDxGridBinding: function (div, columns, detailColumns, datasrc, fileName) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            //"height": 320,
            //"allowColumnReordering": true,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": true,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
            },
            "pager": {
                "visible": true,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": true,
                "showInfo": true,
                "showNavigationButtons": true,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": true,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "selection": {
                "mode": 'multiple',
            },
            "export": {
                "enabled": true,
                "formats": ['excel', 'pdf'],
                allowExportSelectedData: false,
            },
            masterDetail: {
                enabled: true,
                template: function (container, options) {
                    var detailData = datasrc.filter(i => i.traN_ID == options.data.traN_ID);
                    if (detailData.length > 0) {
                        //$("<div>").text(detailData[0].voucheR_NO + " Details:").appendTo(container);
                        $("<div id='" + div + "_detail' style='margin-left: 170px;'>").appendTo(container).dxDataGrid({
                            columnAutoWidth: true,
                            searchPanel: { text: $(div).dxDataGrid("instance").option("searchPanel.text") },
                            columns: detailColumns,
                            dataSource: detailData[0].detail,
                            summary: {
                                totalItems: detailColumns
                                    .filter(col => col.dataType === 'number' || col.dataType === 'decimal' || col.dataType === 'float')
                                    .map(col => ({
                                        column: col.dataField,
                                        summaryType: "sum",
                                        displayFormat: "{0}",
                                        valueFormat: col.format || undefined
                                    }))
                                    .concat([{
                                        column: detailColumns[0].dataField,
                                        summaryType: "count",
                                        displayFormat: "Count: {0}"
                                    }])
                            }
                        });
                    }
                    else {
                        //$("<div>").text("Details:").appendTo(container);
                        $("<div id='" + div + "_detail' style='margin-left: 170px;'>").appendTo(container).dxDataGrid({
                            columnAutoWidth: true,
                            searchPanel: { text: $(div).dxDataGrid("instance").option("searchPanel.text") },
                            columns: detailColumns,
                            dataSource: []
                        });
                    }
                }
            },
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        //const doc = new jsPDF({ orientation: 'landscape' });
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            //columnWidths: [30, 30, 30, 30, 30, 30],
                            //onRowExporting: (e) => {
                            //    const isHeader = e.rowCells[0].text === 'Image';
                            //    if (!isHeader) {
                            //        e.rowHeight = 40;
                            //    }

                            //    //e.component.beginUpdate();
                            //    //e.options.exportOptions.pdf.customizeCell = function (options) {
                            //    //    if (options.rowType === "data" && options.column.caption === "Image") {
                            //    //        return {
                            //    //            image: options.value, // Assuming options.value contains the URL of the image
                            //    //            imageWidth: 50, // Adjust width as needed
                            //    //            imageHeight: 50 // Adjust height as needed
                            //    //        };
                            //    //    }
                            //    //};
                            //    //e.component.endUpdate();

                            //    if (e.rowType === "data" && e.rowValues[0] === "Image") {
                            //        e.rowValues.forEach((value, index) => {
                            //            if (e.columnInfos[index].caption === "Image") {
                            //                console.log(e.rowValues[index]);
                            //                e.rowValues[index] = {
                            //                    image: '/Client/SetupSubTypeFiles/' + value, // Assuming value contains the URL of the image
                            //                    imageWidth: 50, // Adjust width as needed
                            //                    imageHeight: 50 // Adjust height as needed
                            //                };
                            //            }
                            //        });
                            //    }
                            //},
                            //customDrawCell: (e) => {
                            //    if (e.gridCell.rowType === 'data' && e.gridCell.column.caption === 'Image') {
                            //        doc.addImage(e.gridCell.value, 'PNG', e.rect.x, e.rect.y, e.rect.w, e.rect.h);
                            //        e.cancel = true;
                            //    }
                            //},

                            //onExporting: (e) => {
                            //    e.component.beginUpdate();

                            //    e.options.exportOptions.pdf.customizeCell = function (options) {
                            //        if (options.rowType === "data" && options.column.caption === "Image") {
                            //            return {
                            //                image: options.value, // Assuming options.value contains the URL of the image
                            //                imageWidth: 50, // Adjust width as needed
                            //                imageHeight: 50 // Adjust height as needed
                            //            };
                            //        }
                            //    };

                            //    e.component.endUpdate();
                            //},

                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    //var img = new Image();
                                    //img.src = options.gridCell.value; // Assuming options.value contains the URL of the image
                                    //var cellRect = options.cellRect;
                                    //var imgWidth = cellRect[2] - cellRect[0]; // Adjust width as needed
                                    //var imgHeight = cellRect[3] - cellRect[1]; // Adjust height as needed
                                    //doc.addImage(img, 'PNG', cellRect[0], cellRect[1], imgWidth, imgHeight);
                                    //return false; // Prevent default cell rendering


                                    var img = new Image();
                                    img.src = options.gridCell.value; // Assuming options.value contains the URL of the image

                                    //var imgWidth = options.rowHeight - 10; // Adjust width as needed
                                    //var imgHeight = options.rowHeight - 10; // Adjust height as needed

                                    //// Calculate position based on column width
                                    //var cellWidth = e.component.columnOption(options.column.index, "width");
                                    //var cellLeft = options.cellElement.getBoundingClientRect().left;
                                    //var cellTop = options.cellElement.getBoundingClientRect().top;

                                    //var imgLeft = cellLeft + (cellWidth - imgWidth) / 2;
                                    //var imgTop = cellTop + (options.rowHeight - imgHeight) / 2;

                                    //doc.addImage(img, 'PNG', imgLeft, imgTop, imgWidth, imgHeight);
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                ////const column = e.column;
                ////const value = e.value;

                ////// Check if the column name contains "date"
                ////if (value == '01-Jan-1900') {
                ////    e.cellElement.innerText = '';
                ////}

                //const column = e.column;
                //const dataField = column.dataField;
                //const rowData = e.data;
                ///*const value = rowData["'"+dataField+"'"];*/
                //const value = rowData && column ? rowData[column.dataField] : null;

                //// Check if the column name contains "date"
                ////if (column.dataType === 'date' && value && value.getTime() === new Date('01-01-1900').getTime()) {
                ////    e.cellElement.innerText = 'null';
                ////}

                //if (value == '01-Jan-1900') {
                //    e.cellElement.innerText = '';
                //}

                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                // Check if the column is defined, and if the value is a date matching the default date
                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    //e.cellElement.innerText = '';
                    $(e.cellElement).text('')
                }
            },
            rowExpanding(e) {
                var masterGrid = $(div).dxDataGrid("instance");
                var detailGrid = $(div + "_detail").dxDataGrid("instance");

                // Get the specific column value from the master grid
                var specificColumnValue = e.data["Action"];

                // Depending on the specific column value, adjust the detail grid's settings
                if (specificColumnValue != null) {
                    // Example: Align the detail grid's first column with the specific column
                    detailGrid.columnOption(0, "visibleIndex", e.component.columnOption("Action", "visibleIndex"));
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "Action",
                        summaryType: "custom",
                        customizeText: function (data) {
                            return "Count: " + datasrc.length;
                        }
                    },
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "qtY2",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "baL_QTY",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    }
                ]
            }
        }).dxDataGrid('instance');
    },

    CreateCustomPermissionGrid: function (dataSrc, flatData, div, isTree, type) {
        var col = [
            { dataField: 'id', visible: false },
            {
                dataField: 'rowselection',
                caption: '',
                width: 80,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    container.css({
                        'text-align': 'center'
                    });

                    $('<input class="RowCheckbox checkbox_animated" type="checkbox">')
                        .prop('checked', options.value)
                        .on('change', function () {
                            var isChecked = $(this).is(':checked');
                            empr_Binaries.HandleRowSelectionCheckBox(isChecked, options.data.id, options.data.parentId, type);
                        })
                        .appendTo(container);
                },
                headerCellTemplate: function (container) {
                    var $header = $("<div>");
                    var $headerCheckbox = $('<input class="RowCheckboxH checkbox_animated" type="checkbox">');
                    $header.append($headerCheckbox);
                    container.append($header);
                    $headerCheckbox.on('change', function () {
                        $(div).find('.RowCheckbox').prop('checked', $('.RowCheckboxH').is(':checked'));
                        var isChecked = $('.RowCheckboxH').is(':checked');
                        empr_Binaries.HandleRowSelectionCheckBox(isChecked, options.data.id, options.data.parentId, type);
                    });
                },
            },
            { dataField: 'name', caption: 'Menu' }
        ];

        empr_Binaries.InitTree(div, col, flatData, "Permissions", type);
    },



    validateDate: function () {
        const input = document.getElementById('TO_DATE');
        const date = new Date(input.value);
        if (empr_helper.isValidDate(date)) {
        } else {
            empr_helper.notify("Invalid date: Please enter a valid date.", 2);
            input.value = new Date().toISOString().split('T')[0];
        }
    },

    validateVDate: function () {
        const input = document.getElementById('V_DATE');
        const date = new Date(input.value);
        if (empr_helper.isValidDate(date)) {
        } else {
            empr_helper.notify("Invalid date: Please enter a valid date.", 2);
            input.value = new Date().toISOString().split('T')[0];
        }
    },

    isValidDate: function (date) {
        const year = date.getFullYear();
        const month = date.getMonth();
        const day = date.getDate();
        const daysInMonth = [31, empr_helper.isLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
        return day > 0 && day <= daysInMonth[month];
    },

    isLeapYear: function (year) {
        return (year % 4 === 0 && year % 100 !== 0) || (year % 400 === 0);
    },

    validateDateRange: function (input, min, max) {
        let minDate = new Date(min);
        let maxDate = new Date(max);
        let selectedDate = new Date(input);
        let formattedMinDate = new Date(minDate).toLocaleDateString();
        let formattedMaxDate = new Date(maxDate).toLocaleDateString();

        if (selectedDate < minDate || selectedDate > maxDate) {
            empr_helper.notify("Transaction date must be between " + formattedMinDate + " and " + formattedMaxDate, 2);
            return false;
        } else {
            return true;
        }
    },
}