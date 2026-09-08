

var ati_dxHelper = {
    createDataGridLazyLoad: function (divId, dataApi, columns) {
        var data = new DevExpress.data.CustomStore({
            load: function (loadOptions) {
                var deferred = $.Deferred(),
                    args = {};
                if (loadOptions.filter) {
                    args.filter = [];
                    var arr = loadOptions.filter;
                    var index = 0;
                    $.each(arr, function (i, e) {
                        if (Array.isArray(e)) {
                            args.filter[index] = {};  //
                            args.filter[index].columnName = e[0];
                            args.filter[index].oprt = e[1];
                            args.filter[index].value = e[2];
                            args.filter[index].columnIndex = e.columnIndex;
                            index++;
                        }
                    });
                }
                if (loadOptions.sort) {
                    args.orderby = loadOptions.sort[0].selector;
                    if (loadOptions.sort[0].desc)
                        args.orderby += "desc"
                }
                if (loadOptions.select) {
                    args.select = JSON.stringify(loadOptions.select);
                }
                if (loadOptions.group) {
                    args.group = JSON.stringify(loadOptions.group);
                }
                args.skip = loadOptions.skip || 0;
                args.take = loadOptions.take || 10;
                $.ajax({
                    url: dataApi,
                    data: args,
                    type: 'post',
                    async: false,
                    success: function (result) {
                        deferred.resolve($.parseJSON(result.items), { totalCount: result.totalCount }, { summary: result.summaries });
                    },
                    error: function () {
                        deferred.reject("Data Loading Error");
                    }
                });
                return deferred.promise();
            }
        });
        $("#" + divId).dxDataGrid({
            dataSource: {
                store: data
            },
            remoteOperations: { paging: true, sorting: true, filtering: true, selection: true, allowSearch: true },
            filterRow: { visibility: true, allowFilter: true },
            searchPanel: { visible: true, placeholder: "Search...", allowSearch: true },
            headerfilter: { visible: true },
            selection: { allowSelectAll: true, mode: "AllPages" },
            paging: {
                pageSize: 10,
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 25, 50, 100],
                showInfo: true
            },
            columns: columns, //["Quarter", "Month", "Region", "Satate", "Manager", "Salesperson", "Category", "Subcategory", "Quantity"]
            allowColumnResizing: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            showBorders: true
        }).dxDataGrid("instance");
    },
    createTreeList: function (divId, data, keyExp, dataField, parentId) {
        $("#" + divId).dxTreeList({
            dataSource: data,
            dataStructure: "plain",
            keyExpr: keyExp,
            filterRow: { visible: true },
            searchPanel: {
                visible: true,
                placeholder: "Search data here..."
            },
            showRowLines: true,
            selection: {
                mode: "single",
                recursive: false
            },
            parentIdExpr: parentId,
            //selection: { allowSelectAll: false, mode: "single" },
            columns: [
                {
                    dataField: dataField,
                    cellTemplate: function (element, info) {
                        if (info.data.isFolder == true) {
                            element.append('<div class="customTreelistItem"><i class="fa fa-folder folder-icon" style="color: ' + info.data.folderColor + '"></i><div class="listText">' + info.text + '</div></div>');
                        }
                        else {
                            var recordid = info.data.folderid.toString().split('_')[1];
                            element.append('<div class="customTreelistItem"><i style="color: #175bf5" class="fa fa-file-text file-icon"></i><div class="listText"><a style="cursor: pointer" onclick="ati_common.openRecordViewSlide(' + recordid + ');">' + info.text + '</a></div></div>');
                        }
                    }
                }
            ],
            expandedRowKeys: [1],
            showBorders: true,
            columnAutoWidth: true
        });
    },
    createDataGrid: function (divId, url, columns) {
        
        var jsonData;
        ajaxHelper.ajaxGetJson(url, function (data) {
            
            jsonData = data;
        }, false, false);
        $("#" + divId).dxDataGrid({
            dataSource: jsonData,
            remoteOperations: { paging: true, sorting: true, filtering: true, selection: true, allowSearch: true },
            filterRow: { visibility: true, allowFilter: true },
            searchPanel: { visible: true, placeholder: "Search...", allowSearch: true },
            headerfilter: { visible: true },
            selection: { allowSelectAll: true, mode: "AllPages" },
            paging: {
                pageSize: 10
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 25, 50, 100],
                showInfo: true
            },
            columns: columns, //["Quarter", "Month", "Region", "Satate", "Manager", "Salesperson", "Category", "Subcategory", "Quantity"]
            allowColumnResizing: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            showBorders: true
        }).dxDataGrid("instance");
    },
    createDropdownList: function (divId, data, hiddenId, keyExp, dataField, extraOptions, IsDisabled, SelectedValue) {
         
        var treeList;
        var newSelectionMade = false;
        var objTreeList = {
            height: 300,
            dataSource: data,
            keyExpr: keyExp,
            showColumnHeaders: true,
            filterRow: { visible: true },
            showRowLines: true,
            selection: { allowSelectAll: true, mode: "multiple" },
            columns: [{ dataField: dataField, caption: "Select All" }],
            disabled: IsDisabled,
            value: SelectedValue,
            //showRowLines: false,
            columnAutoWidth: false,
            onSelectionChanged: function (e) { newSelectionMade = true; handleSelection(e, hiddenId, keyExp, dataField); },
            onContentReady: function (e) {

                e.component.option("selectedRowKeys", SelectedValue);
                //if ($('#' + hiddenId).val() != "" && $('#' + hiddenId).val() != 0 && !newSelectionMade) {

                //    var list = $('#' + hiddenId).val().split(',');
                //    //list.splice($.inArray("", list), 1);
                //    e.component.option("selectedRowKeys", list);
                //}
            }
        };

        // Overwrite any properties of options obj that are supplied by extraOptions parameter
        if (typeof extraOptions === 'object' && Object.keys(extraOptions).length > 0) {
            var keys = Object.keys(extraOptions);

            for (var i = 0; i < keys.length; i++) {
                var currKey = keys[i].toString();
                objTreeList[currKey] = extraOptions[currKey];
            }
        }

        $("#" + divId).dxDropDownBox({
            placeholder: "Select a value...",
            showClearButton: false,
            focusStateEnabled: false,
            height: 32,
            dataSource: data,
            valueExpr: keyExp,
            displayExpr: dataField,
            value: CreateSelectedArray(CreateDxTagBoxJSON(data))[1],
            onInitialized: function () {
                if ($('#' + hiddenId).val() != null && $('#' + hiddenId).val() != 0 && $('#' + hiddenId).val() != "") {
                    var list = $('#' + hiddenId).val().split(',').map(Number);
                    $("#" + divId).dxDropDownBox('instance').option('value', list);
                }
            },
            contentTemplate: function (e) {
                rolesDropDown = e;
                var value = e.component.option("value"),
                    $treeList = $("<div id='" + divId + "_list'>").dxTreeList(objTreeList);
                treeList = $treeList.dxTreeList("instance");
                return $treeList;
            }
        });

        function handleSelection(e, hiddenId, keyExp, dataField) {
            var count = e.component.getSelectedRowsData().length;
            if (count == 0) {
                rolesDropDown.component.option("value", "");
            }
            else {
                if (count > 6) {
                    rolesDropDown.component.option("value", count + " items selected");
                }
                else {
                    var selectedRoles = "";
                    for (var i = 0; i < count; i++) {
                        if (i > 0) selectedRoles = selectedRoles + " , " + e.component.getSelectedRowsData()[i][dataField.toString()];
                        else selectedRoles = e.component.getSelectedRowsData()[i][dataField.toString()];
                    }
                    rolesDropDown.component.option("value", selectedRoles);
                }

                var selectedRolesIDs = "";
                for (var z = 0; z < count; z++) {
                    if (z > 0) selectedRolesIDs = selectedRolesIDs + " , " + e.component.getSelectedRowsData()[z][keyExp.toString()];
                    else selectedRolesIDs = e.component.getSelectedRowsData()[z][keyExp.toString()];
                }
                $("#" + hiddenId).val(selectedRolesIDs);
                var obj = {};
                obj.array = selectedRolesIDs;
                //ati_dxHelper.dxSelectedIndexChangeSingle(divId, JSON.stringify(obj));
            }
        }
    },    
    createDropdownSingle: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onValueChangedFun) {
        
        $("#" + divId).dxSelectBox({
            items: data,
            valueExpr: keyExp,
            displayExpr: dataField,
            placeholder: placeholder,
            showClearButton: true,
            searchEnabled: true,
            onValueChanged: function (e) {

                onValueChangedFun(e);

               
            },
            onInitialized: function () {
                if (selectedvalues != null && selectedvalues != "") {
                    
                        $("#" + divId).dxSelectBox({
                            value: selectedvalues
                        });
                }
            }
        });
    },
    createDropdownSingle_New: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onValueChangedFun) {
        
        $("#" + divId).dxSelectBox({
            items: data,
            valueExpr: dataField,
            displayExpr: dataField,
            placeholder: placeholder,
            showClearButton: true,
            searchEnabled: true,
            onValueChanged: function (e) {

                onValueChangedFun(e);

               
            },
            onInitialized: function () {
                if (selectedvalues != null && selectedvalues != "") {
                    
                        $("#" + divId).dxSelectBox({
                            value: selectedvalues
                        });
                }
            }
        });
    },
    createDropdownSingleForAccountGroup: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onValueChangedFun) {

        $("#" + divId).dxSelectBox({
            items: data,
            valueExpr: keyExp,
            displayExpr: dataField,
            placeholder: placeholder,
            showClearButton: true,
            searchEnabled: true,
            onValueChanged: function (e) {

                onValueChangedFun(e);


            },
            onInitialized: function (e) {
                if (selectedvalues != null && selectedvalues != "") {

                    $("#" + divId).dxSelectBox({
                        value: selectedvalues
                    });
                }

                var currentValue = e.component.option("value");
                if (currentValue == 'S') {
                    $(".showOnS").show();
                } else {
                    $(".showOnS").hide();
                }
            }
        });
    },
    createHtmlEditor: function (divId) {
        $("#" + divId).dxHtmlEditor({
            toolbar: {
                items: [
                    "undo", "redo", "separator",
                    {
                        formatName: "size",
                        formatValues: ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"]
                    },
                    {
                        formatName: "font",
                        formatValues: ["Arial", "Courier New", "Georgia", "Impact", "Lucida Console", "Tahoma", "Times New Roman", "Verdana"]
                    },
                    "separator", "bold", "italic", "strike", "underline", "separator",
                    "alignLeft", "alignCenter", "alignRight", "alignJustify", "separator",
                    {
                        formatName: "header",
                        formatValues: [false, 1, 2, 3, 4, 5]
                    }, "separator",
                    "orderedList", "bulletList", "separator",
                    "color", "background", "separator",
                    "link", "image", "separator",
                    "clear", "codeBlock", "blockquote"
                ]
            },
            mediaResizing: {
                enabled: true
            }
        });
    },
    createColorBox: function (divId, value, extraOptions) {
        var options = {
            value: value,
            editAlphaChannel: true,
            height: 33,
        };
        // Overwrite any properties of options obj that are supplied by extraOptions parameter
        if (typeof extraOptions === 'object' && Object.keys(extraOptions).length > 0) {
            var keys = Object.keys(extraOptions);

            for (var i = 0; i < keys.length; i++) {
                var currKey = keys[i].toString();
                options[currKey] = extraOptions[currKey];
            }
        }
        return $("#" + divId).dxColorBox(options);
    },
    getColorBoxValue: function (divId) {
        return $("#" + divId).dxColorBox("instance").option("value");
    },
    createGrid: function (divId, columns, jsonData, _filename, _columnChooser, _getId) {
        //handling the celltemplate
        for (var i = 0; i < columns.length; i++) {
            if (columns[i].hyperlinkColumn) {
                columns[i].cellTemplate = function (container, options) {
                    var elm;
                  
                    if (columns.length == options.row.cells.length)
                        currentColumn = columns[options.columnIndex] || {};
                    else
                        currentColumn = columns.filter(function (x) { return x.caption == options.column.caption })[0];

                    if (currentColumn.hyperlinkColumn) {
                        var hyperlinkColumnValue = options.data[currentColumn.hyperlinkColumn] || "";
                        elm = $("<span>").html("<a href='javascript:void(0);' rel='noopener' class='drillLink' style='cursor:pointer;'>" + options.text + "</a>");
                        $('a', elm).click(function () {
                            _navigateToColumnHyperlink(hyperlinkColumnValue);
                           // _navigateToColumnHyperlink("http://www.google.com");
                        });
                    }
                    else {
                        //elm = $("<span title='" + options.text + "'>").html(options.text);
                        elm = $("<span title='" + options.text + "'>").text(options.text);
                    }


                    $(elm).appendTo(container);
                };
            }
        }

        var _exportEnabled;
        if (_filename != "") {
            _exportEnabled = true;
        }
        $("#" + divId).dxDataGrid({
            dataSource: jsonData,
            showBorders: false,
            remoteOperations: { paging: true, sorting: true, filtering: true, selection: true, allowSearch: true },
            filterRow: { visibility: true, allowFilter: true },
            searchPanel: { visible: false, placeholder: "Search...", allowSearch: true },
            headerfilter: { visible: true },
            selection: { allowSelectAll: true, mode: "AllPages" },

            paging: {
                pageSize: 10
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 25, 50, 100],
                showInfo: true
            },
            columns: columns, //["Quarter", "Month", "Region", "Satate", "Manager", "Salesperson", "Category", "Subcategory", "Quantity"]
            allowColumnResizing: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            showBorders: true,
            columnChooser: {
                enabled: _columnChooser,
                allowSearch: true,
                mode: "select",
                //width: 250,
                //height: 260
            },
            export: {
                enabled: _exportEnabled,
                fileName: _filename,
            },
            columnAutoWidth: true,
            onCellClick: function (e) {
                if (_getId != "") {
                    //  girdDrill = "table_Level1"
                    //   drillType = 1;
                    //   Level = 1;
                    $("#Id").val(e.data[_getId].trim());
                    //   common_Dashboard.Grid(Url, common_Dashboard.Param(FromDate, ToDate, FromPriorDate, ToPriorDate, e.data[_getId].trim(), Agent, Aduser, Level, 0, drillWidgetId, "drill", drillType, Value, User), girdDrill, _filename, true, "", false);
                }
            },
            //,
            //scrolling: {
            //    mode: "virtual"
            //}

        }).dxDataGrid("instance");

        NoData_Helper.Nodata_createGrid(divId, jsonData);
        //console.log($("#" + divId).dxDataGrid("instance").option('dataSource'));
    },
    createGridForDynamicDashboard: function (divId, drillType, jsonData, drillColumns,_columnChooser, _getId,_queryId) {

        function getFormat(format) {
            if (format == "percent") {
                format = "#0.##'%'";
            } return format;
        }

        //var _exportEnabled;
        //if (_filename != "") {
        //    _exportEnabled = true;
        //}
        // Get Drill Column Formatting
        var ajaxParam = {
            queryId: _queryId,
            drillNo: drillType
        };
        var drillColumnsFormatting = [];
        ajaxHelper.ajaxPostJsonData(JSON.stringify(ajaxParam), '/Dashboard/GetQueryDrillColumns', function (response) {
            var dd = response;
            $(dd).each(function (i, e) {
                drillColumnsFormatting.push({
                    colid: e.drillColumnId,
                    columnName: e.dataField,
                    alignment: e.alignment,
                    datatype: e.dataType,
                    format:  e.format,
                    precision: e.precision
                });
            });

        }, false);
        // Get Drill Column Formatting
        var columns = [];
        var drillcol = drillColumns.split(',');
        if (drillType == 1) {
            /*columns = ["Branch", "Unit", "Volume"]*/
            for (var i = 0; i < drillcol.length; i++) {
                var formatting = drillColumnsFormatting.filter(x => x.columnName == drillcol[i])[0];
                var format = null;
                var precision = formatting.precision;
                if (formatting.format != 'none') {
                    format = getFormat(formatting.format)//formatting.format;
                    precision = 0;
                }
                if (drillcol[i] == "Branch" || drillcol[i] == "BranchName" || drillcol[i] == "Branch Name") {
                    columns.push({
                        alignment: formatting.alignment, caption: drillcol[i], cssClass: "drill2", dataField: drillcol[i], dataType: formatting.datatype, format: getFormat(format),//format,
                        precision: precision
                    });
                }
                else {

                    if (formatting.datatype == "number")
                    {
                        if (formatting.format == 'none')
                        {
                            columns.push({
                                alignment: formatting.alignment,
                                caption: drillcol[i],
                                cssClass: "",
                                dataField: drillcol[i],
                                dataType: formatting.datatype,
                                
                            });
                        }
                        else
                        {
                            columns.push({
                                alignment: formatting.alignment,
                                caption: drillcol[i],
                                cssClass: "",
                                dataField: drillcol[i],
                                dataType: formatting.datatype,
                                format: {
                                    type: getFormat(formatting.format),//formatting.format,
                                    precision: formatting.precision
                                },
                            });
                        }
                        
                    }
                    else
                    {
                        columns.push({
                            alignment: formatting.alignment, caption: drillcol[i], cssClass: "", dataField: drillcol[i], dataType: formatting.datatype, format: getFormat(format),//format,
                            precision: precision
                        });
                    }
                }
            }
        }
        else if (drillType == 2) {
            for (var i = 0; i < drillcol.length; i++) {
                var formatting = drillColumnsFormatting.filter(x => x.columnName == drillcol[i])[0];
                var format = null;
                var precision = formatting.precision;
                if (formatting.format != 'none') {
                    format = getFormat(formatting.format),//formatting.format;
                    precision = formatting.precision;
                }
                if (drillcol[i] == "LoanOfficer" || drillcol[i] == "LoanOfficerName") {
                    columns.push({
                        alignment: formatting.alignment, caption: drillcol[i], cssClass: "drill3", dataField: drillcol[i], dataType: formatting.datatype, format: getFormat(format),//format,
                        precision: precision
                    });
                }
                else if (drillcol[i] == "Branch" || drillcol[i] == "BranchName" || drillcol[i] == "Branch Name") {
                    columns.push({
                        alignment: formatting.alignment, caption: drillcol[i], cssClass: "classBranch", dataField: drillcol[i], dataType: formatting.datatype, format: getFormat(format),// format,
                        precision: precision
                    });
                }
                else {

                    if (formatting.datatype == "number") {
                        if (formatting.format == 'none') {
                            columns.push({
                                alignment: formatting.alignment,
                                caption: drillcol[i],
                                cssClass: "",
                                dataField: drillcol[i],
                                dataType: formatting.datatype,

                            });
                        }
                        else {
                            columns.push({
                                alignment: formatting.alignment,
                                caption: drillcol[i],
                                cssClass: "",
                                dataField: drillcol[i],
                                dataType: formatting.datatype,
                                format: {
                                    type: getFormat(formatting.format),//formatting.format,
                                    precision: formatting.precision
                                },
                            });
                        }

                    }
                    else
                    {
                        columns.push({
                            alignment: formatting.alignment, caption: drillcol[i], cssClass: "", dataField: drillcol[i], dataType: formatting.datatype, format: getFormat(format),// format,
                            precision: precision
                        });
                    }
                }

            }
        }
        else if (drillType == 3) {
            for (var i = 0; i < drillcol.length; i++) {
                var formatting = drillColumnsFormatting.filter(x => x.columnName == drillcol[i])[0];
                var format = null;
                var precision = formatting.precision;
                if (formatting.format != 'none') {
                    format = getFormat(formatting.format)//formatting.format;
                    precision = formatting.precision;
                    columns.push({
                        alignment: formatting.alignment,
                        caption: drillcol[i],
                        cssClass: "",
                        dataField: drillcol[i],
                        dataType: formatting.datatype,
                        format: {
                            type: getFormat(formatting.format),//formatting.format,
                            precision: formatting.precision
                        },
                    });
                }
                else {
                    columns.push({
                        alignment: formatting.alignment,
                        caption: drillcol[i],
                        cssClass: "",
                        dataField: drillcol[i],
                        dataType: formatting.datatype
                    });
                }
                
            }
        }
        $("#" + divId).dxDataGrid({
            dataSource: jsonData,
            showBorders: false,
            remoteOperations: { paging: true, sorting: true, filtering: true, selection: true, allowSearch: true },
            filterRow: { visibility: true, allowFilter: true },
            searchPanel: { visible: false, placeholder: "Search...", allowSearch: true },
            headerfilter: { visible: true },
            selection: { allowSelectAll: true, mode: "AllPages" },

            paging: {
                pageSize: 10
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 25, 50, 100],
                showInfo: true
            },
            columns: columns, //["Quarter", "Month", "Region", "Satate", "Manager", "Salesperson", "Category", "Subcategory", "Quantity"]
            allowColumnResizing: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            showBorders: true,
            columnChooser: {
                enabled: _columnChooser,
                allowSearch: true,
                mode: "select",
                //width: 250,
                //height: 260
            },
            export: {
                enabled: false,
                /*fileName: _filename,*/
            },
            columnAutoWidth: true,
            onCellClick: function (e) {
                if (_getId != "") {
                    //  girdDrill = "table_Level1"
                    //   drillType = 1;
                    //   Level = 1;
                    $("#Id").val(e.data[_getId].trim());
                    //   common_Dashboard.Grid(Url, common_Dashboard.Param(FromDate, ToDate, FromPriorDate, ToPriorDate, e.data[_getId].trim(), Agent, Aduser, Level, 0, drillWidgetId, "drill", drillType, Value, User), girdDrill, _filename, true, "", false);
                }
            },
            //,
            //scrolling: {
            //    mode: "virtual"
            //}

        }).dxDataGrid("instance");

        NoData_Helper.Nodata_createGrid(divId, jsonData);
        //console.log($("#" + divId).dxDataGrid("instance").option('dataSource'));
        
    },
    LoadDDSearchControl: function (pControlID, DataSource, hiddenId, valueField, displayField, IsDisabled, SelectedValue) {
     
         
        $("#" + pControlID).dxTagBox({
            searchEnabled: true,
            items: DataSource,
            valueExpr: valueField,
            displayExpr: displayField,
            showSelectionControls: true,
            value: SelectedValue,
            disabled: IsDisabled,
            maxDisplayedTags: 2,
            onValueChanged: function (e) {
                $('#btnSave').show();
                let EventValue = e.value.length > 0 ? e.value.join() : e.value;
                if (e.value.length > 0) {
                    if (e.element[0].id == "cmbChannel") {
                        window.localStorage.setItem("Channel", EventValue);
                        GetData_Post('/User/FilterDivisionByChannel', '{"array":"' + EventValue + '"}', function (result, status) {

                            if (result.msgType == 1) {
                                var JSON = result.data;
                                let Datasource = CreateDxTagBoxJSONSelection(JSON, "Division")
                                $('#cmbDivision').dxTagBox('instance').option("dataSource", Datasource);
                            }
                        })
                    }
                    else if (e.element[0].id == "cmbDivision") {
                        let previousChannel = window.localStorage.getItem("Channel");
                        window.localStorage.setItem("Division", EventValue);
                        GetData_Post('/User/FilterRegionByDivision', '{"channel":"' + previousChannel + '","divison":"' + EventValue + '"}', function (result, status) {

                            if (result.msgType == 1) {
                                var JSON = result.data;
                                let Datasource = CreateDxTagBoxJSONSelection(JSON, "Region")
                                $('#cmbRegion').dxTagBox('instance').option("dataSource", Datasource);
                            }
                        })
                    }
                    else if (e.element[0].id == "cmbRegion") {
                        let previousChannel = window.localStorage.getItem("Channel");
                        let previousDivision = window.localStorage.getItem("Division");
                        window.localStorage.setItem("Region", EventValue);
                        GetData_Post('/User/FilterBranchByRegion', '{"channel":"' + previousChannel + '","Division":"' + previousDivision + '","Region":"' + EventValue + '"}', function (result, status) {

                            if (result.msgType == 1) {
                                var JSON = result.data;

                                let Datasource = CreateDxTagBoxJSONSelection(JSON,"Branch")
                                $('#cmbBranch').dxTagBox('instance').option("dataSource", Datasource);
                            }
                        })
                    }
                    else if (e.element[0].id == "cmbBranch") {
                        let previousChannel =  window.localStorage.getItem("Channel");
                        let previousDivision = window.localStorage.getItem("Division");
                        let previousRegion =   window.localStorage.getItem("Region");

                        GetData_Post('/User/FilterSubBranchByBranch', '{"channel":"' + previousChannel + '","Division":"' + previousDivision + '","Region":"' + previousRegion + '","Branch":"' + EventValue + '"}', function (result, status) {

                            if (result.msgType == 1) {
                                var JSON = result.data;

                                let Datasource = CreateDxTagBoxJSONSelection(JSON, "SubBranch")
                                $('#cmbSubBranch').dxTagBox('instance').option("dataSource", Datasource);
                            }
                        })
                    }
                }
            },
            onMultiTagPreparing: function (args) {
                const selectedItemsLength = args.selectedItems.length;
                const totalCount = DataSource.length;
                if (selectedItemsLength < totalCount) {
                    args.cancel = true;
                } else {
                    args.text = "All selected (" + selectedItemsLength + ")";
                    args.multiTagElement.addClass("red");
                }
            }
           
        });
        window.localStorage.removeItem("Channel");
        window.localStorage.removeItem("Division");
        window.localStorage.removeItem("Region");

    },
    ValidateDropdownSource: function (Array1, Array2, DropdownFOR, pControlID, hiddenId, valueField, displayField, IsDisabled) {

        var FinalizeSelecteditems = [];
        var FinalizeDatasource = [];

        //Get Common element
        var array1 = GetFineArray(Array1, DropdownFOR),
            array2 = GetFineArray(Array2, DropdownFOR);
        //Get Finalize Datasource
        if (Array1[0][DropdownFOR] != null) {
            FinalizeDatasource = $.unique($.merge($.merge([], array1), array2));
        }
        else {
            FinalizeDatasource = array2;
        }
        FinalizeSelecteditems = getDuplicateArrayElements($.merge($.merge([], array1), array2));
        //Transform Datasource for DD Search control
        var Datasource = CreateDxTagBoxJSON(FinalizeDatasource)

        //Bind DropDown
        ati_dxHelper.LoadDDSearchControl(pControlID, Datasource, hiddenId, valueField, displayField, IsDisabled, FinalizeSelecteditems)
    }, 
    DxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden,onchangeFun ) {

        $(divid).dxDropDownBox({
                value: selectval,
                valueExpr: valueExpr,
                //keyExpr: 'BCODE',
                deferRendering: false,
                placeholder: 'Select a value...',
                inputAttr: { 'aria-label': 'Owner' },
                displayExpr(item) {
                if (item != undefined) {

                    return item[displayExpr];

                } else {
                    
                    return $(displayExprHidden).val();

                }

                },
                showClearButton: true,
                dataSource: datasrc,
            contentTemplate(e) {
                    const value = e.component.option('value');
                const $dataGrid = $('<div id=' + divid.replace('#', "")+'_grid>').dxDataGrid({
                        dataSource: datasrc,
                        columns: col,//['CompanyName', 'City', 'Phone'],
                        hoverStateEnabled: true,
                        //keyExpr:'BCODE',
                        paging: { enabled: true, pageSize: 10 },
                        filterRow: { visible: true },
                        scrolling: { mode: 'virtual' },
                        selection: { mode: 'single' },
                        selectedRowKeys: selectedOjb,
                        height: '100%',
                        showBorders: true,
                        onSelectionChanged(selectedItems) {

                            const keys = selectedItems.selectedRowKeys;
                            const hasSelection = keys.length;
                            onchangeFun(selectedItems, hiddenid);
                            // $("#" + divId).dxDropDownBox("instance").close();
                            //$(divId).dxDropDownBox('instance').deselectAll();
                            e.component.option('value', hasSelection ? selectedItems.selectedRowsData : null);
                            
                        },
                    });

                    dataGrid = $dataGrid.dxDataGrid('instance');

                    e.component.on('valueChanged', (args) => {
                        if (args.value == '' || args.value == null) {
                            //dataGrid.deselectRows(args.previousValue);
                            //dataGrid.deselectAll();
                            var instance = $(divid + '_grid').dxDataGrid('instance');
                            instance.deselectRows(instance.getSelectedRowKeys());
                        }
                        else {
                            var instance = $(divid + '_grid').dxDataGrid('instance');
                            instance.selectRows(args.value);
                            //dataGrid.selectRows(args.value, false);
                            e.component.close();
                        }
                    });

                    return $dataGrid;
                },
            });
   
    },
    MultipleDxGridBoxDropdown: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        $(divid).dxDropDownBox({
            value: selectval,
            valueExpr: valueExpr,
            //keyExpr: 'BCODE',
            deferRendering: false,
            placeholder: 'Select a value...',
            inputAttr: { 'aria-label': 'Owner' },
            displayExpr(item) {
                if (item != undefined) {
                    return item[displayExpr];
                } else {
                    if (window.location.href.includes("ItemMaster")) {
                        if (empr_ItemMaster.isValueAssigned == false) {
                            empr_ItemMaster.isValueAssigned = true;
                            return $(displayExprHidden).val();
                        }
                    }

                    if (window.location.href.includes("HRInterviewSchedule")) {
                        if (empr_HRInterviewSchedule.isValueAssigned == false) {
                            empr_HRInterviewSchedule.isValueAssigned = true;
                            return $(displayExprHidden).val();
                        }
                    }

                    if (window.location.href.includes("Role")) {
                        if (empr_Role.isValueAssigned == false) {
                            empr_Role.isValueAssigned = true;
                            return $(displayExprHidden).val();
                        }
                    }

                    if (window.location.href.includes("POSDiscountItemWise")) {
                        if (empr_POSDiscountItemWise.isValueAssigned == false) {
                            empr_POSDiscountItemWise.isValueAssigned = true;
                            console.log(displayExprHidden);
                            return $(displayExprHidden).val();
                        }
                    }

                    if (window.location.href.includes("POSDiscount")) {
                        if (empr_POSDiscount.isValueAssigned == false) {
                            empr_POSDiscount.isValueAssigned = true;
                            return $(displayExprHidden).val();
                        }
                    }

                }
            },
            showClearButton: true,
            dataSource: datasrc,
            contentTemplate(e) {
                const value = e.component.option('value');
                const $dataGrid = $('<div id=' + divid.replace('#', "") + '_grid>').dxDataGrid({
                    dataSource: datasrc,
                    columns: col,//['CompanyName', 'City', 'Phone'],
                    hoverStateEnabled: true,
                    //keyExpr:'BCODE',
                    paging: { enabled: true, pageSize: 10 },
                    filterRow: { visible: true },
                    scrolling: { mode: 'virtual' },
                    selection: { mode: 'multiple' },
                    selectedRowKeys: selectedOjb,
                    height: '100%',
                    showBorders: true,
                    onSelectionChanged(selectedItems) {

                        const keys = selectedItems.selectedRowKeys;
                        const hasSelection = keys.length;
                        onchangeFun(selectedItems, hiddenid);
                        // $("#" + divId).dxDropDownBox("instance").close();
                        //$(divId).dxDropDownBox('instance').deselectAll();
                        e.component.option('value', hasSelection ? selectedItems.selectedRowsData : null);

                    },
                });

                dataGrid = $dataGrid.dxDataGrid('instance');

                e.component.on('valueChanged', (args) => {
                    if (args.value == '' || args.value == null) {
                        //dataGrid.deselectRows(args.previousValue);
                        //dataGrid.deselectAll();
                        var instance = $(divid + '_grid').dxDataGrid('instance');
                        //instance.selectRows(instance.getSelectedRowKeys());
                    }
                    else {
                        var instance = $(divid + '_grid').dxDataGrid('instance');
                        instance.selectRows(args.value);
                        //dataGrid.selectRows(args.value, false);
                        //e.component.close();
                    }
                });

                return $dataGrid;
            },
        });
        //$(divid).dxDropDownBox({
        //    value: [1],
        //    valueExpr: 'key',
        //    placeholder: 'Select a value...',
        //    displayExpr: 'value',
        //    showClearButton: true,
        //    inputAttr: { 'aria-label': 'Owner' },
        //    dataSource: datasrc,// makeAsyncDataSource('customers.json'),
        //    contentTemplate(e) {
        //        const v = e.component.option('value');
        //        const $dataGrid = $('<div>').dxDataGrid({
        //            dataSource: e.component.getDataSource(),
        //            columns: col,
        //            hoverStateEnabled: true,
        //            paging: { enabled: true, pageSize: 10 },
        //            filterRow: { visible: true },
        //            scrolling: { mode: 'virtual' },
        //            height: 345,
        //            selection: { mode: 'multiple' },
        //            selectedRowKeys: v,
        //            onSelectionChanged(selectedItems) {
        //                const keys = selectedItems.selectedRowKeys;
        //                e.component.option('value', keys);
        //            },
        //        });

        //        dataGrid = $dataGrid.dxDataGrid('instance');

        //        e.component.on('valueChanged', (args) => {
        //            const { value } = args;
        //            dataGrid.selectRows(value, false);
        //        });

        //        return $dataGrid;
        //    },
        //});
    },
    DxGridBoxDropdownWithSearch: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        let dataGrid,
            searchTimer,
            dataSource = new DevExpress.data.DataSource({
                store: datasrc,
                searchExpr: ["value"]
            });
        $(divid).dxDropDownBox({
            value: selectval,
            valueExpr: valueExpr,
            //keyExpr: 'BCODE',
            deferRendering: false,
            placeholder: 'Select a value...',
            inputAttr: { 'aria-label': 'Owner' },
            displayExpr(item) {
                if (item != undefined) {

                    return item[displayExpr];

                } else {

                    return $(displayExprHidden).val();

                }
                //return (
                //    item && `${item.value}`
                //);
            },
            acceptCustomValue: true,
            openOnFieldClick: false,
            valueChangeEvent: "",
            showClearButton: true,
            dataSource: datasrc,
            onInput: function (e) {
                clearTimeout(searchTimer);
                searchTimer = setTimeout(function () {
                    let text = e.component.option("text"),
                        opened = e.component.option("opened");

                    dataSource.searchValue(text);
                    if (opened && ati_dxHelper.isSearchIncomplete(e.component)) {
                        dataSource.load().done((items) => {
                            if (items.length > 0 && dataGrid)
                                dataGrid.option("focusedRowKey", items[0])
                        });
                    } else {
                        e.component.open();
                    }
                }, 500);
            },
            onOpened: function (e) {
                let ddbInstance = e.component;
                if (ddbInstance.isKeyDown) {
                    var contentReadyHandler = args => {
                        let gridInstance = args.component;
                        gridInstance.focus();
                        gridInstance.off("contentReady", contentReadyHandler);
                    };
                    if (!dataGrid.isNotFirstLoad)
                        dataGrid.on("contentReady", contentReadyHandler);
                    else {
                        var optionChangedHandler = (args) => {
                            let gridInstance = args.component;
                            if (args.name === 'focusedRowKey' || args.name === 'focusedColumnIndex') {
                                gridInstance.off('optionChanged', optionChangedHandler);
                                gridInstance.focus();
                            }
                        }
                        dataGrid.on('optionChanged', optionChangedHandler);
                        dataGrid.option("focusedRowIndex", 0)
                    }
                    ddbInstance.isKeyDown = false;
                } else if (dataGrid.isNotFirstLoad && ati_dxHelper.isSearchIncomplete(ddbInstance)) {
                    dataSource.load().done(items => {
                        if (items.length > 0) {
                            dataGrid.option("focusedRowKey", items[0]);
                            //dataGrid.close();
                            $('#BQTY').click();
                        }
                            
                        ddbInstance.focus();
                    });
                }
            },
            onClosed: function (e) {
                let ddbInstance = e.component,
                    value = ddbInstance.option("value"),
                    searchValue = dataSource.searchValue();
                if (ati_dxHelper.isSearchIncomplete(ddbInstance)) {
                    ddbInstance.option("value", value === "" ? null : "");
                }
                if (searchValue) {
                    dataSource.searchValue(null);
                }
            },
            onKeyDown: function (e) {
                let ddbInstance = e.component;
                if (e.event.keyCode !== 40) return; //not arrow down
                if (!ddbInstance.option("opened")) {
                    ddbInstance.isKeyDown = true;
                    ddbInstance.open();
                } else dataGrid && dataGrid.focus();
            },
            contentTemplate(e) {
                const value = e.component.option('value');
                ddbInstance = e.component;
                const $dataGrid = $('<div id=' + divid.replace('#', "") + '_grid>').dxDataGrid({
                    dataSource: dataSource,
                    columns: col,//['CompanyName', 'City', 'Phone'],
                    hoverStateEnabled: true,
                    //keyExpr:'BCODE',
                    paging: { enabled: true, pageSize: 10 },
                    focusedRowIndex: -1,
                    focusedRowEnabled: true,
                    autoNavigateToFocusedRow: false,
                    onContentReady: function (e) {
                        if (!e.component.isNotFirstLoad) {
                            e.component.isNotFirstLoad = true;
                            ddbInstance.focus();
                        }
                    },
                    remoteOperations: true,
                    filterRow: { visible: true },
                    scrolling: { mode: 'virtual' },
                    selection: { mode: 'single' },
                    selectedRowKeys: selectedOjb,
                    height: '100%',
                    //columnWidth: 100,
                    showBorders: true,
                    onKeyDown: function (e) {
                        let gridInstance = e.component;
                        if (e.event.keyCode === 13) // Enter press
                            gridInstance.selectRows(
                                [gridInstance.option("focusedRowKey")],
                                false
                            );
                    },
                    onSelectionChanged(selectedItems) {

                        const keys = selectedItems.selectedRowKeys;
                        const hasSelection = keys.length;
                        onchangeFun(selectedItems, hiddenid);
                        // $("#" + divId).dxDropDownBox("instance").close();
                        //$(divId).dxDropDownBox('instance').deselectAll();
                        ddbInstance.option('value', hasSelection ? selectedItems.selectedRowsData : null);

                    },
                });

                dataGrid = $dataGrid.dxDataGrid('instance');

                ddbInstance.on('valueChanged', (args) => {
                    clearTimeout(searchTimer);
                    dataGrid.option("selectedRowKeys", args.value ? [args.value] : []);
                    ddbInstance.close();
                    e.component.close();
                });

                return $dataGrid;
            },
        });

    },
    isSearchIncomplete: function (dropDownBox) {
        // compare the last displayed value and the current real text in the input field
        let displayValue = dropDownBox.option("displayValue"),
        text = dropDownBox.option("text");
        text = text && text.length && text; //text[0];
        displayValue = displayValue && displayValue.length && displayValue[0];
        return text !== displayValue;
    }
};