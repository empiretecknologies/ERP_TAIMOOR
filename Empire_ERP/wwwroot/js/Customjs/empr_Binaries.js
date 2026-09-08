var empr_Binaries = {
    sourceDbObject: [],
    destinationDbObject: [],
    sourceFlatTreeData: [],
    destinationFlatTreeData: [],
    sourceGridInstance: null,
    destinationGridInstance: null,

    InitEvents: function () {
        $(document).ready(function () {
            empr_Binaries.InitSourceDatabaseDDL('data');

            empr_Binaries.InitDestinationDatabaseDDL('data');

            $('body').on('click', '#s_Test_Btn', function () {
                if (empr_Binaries.ValidateSourceInfo()) {
                    var data = empr_Binaries.GetDataSourceDDL();
                    ajaxHelper.ajaxPostJsonData(data, "/Binaries/GetDatabaseObjects", function (responce) {

                        if (responce.msgType == 1) {
                            empr_Binaries.sourceDbObject = responce.data;
                            empr_Binaries.sourceFlatTreeData = empr_Binaries.flattenTree(empr_Binaries.sourceDbObject);
                            empr_helper.notify(responce.msg, responce.msgType);
                            $('#toDbName').text(data.S_DATABASE);
                        }
                        else if (responce.msgType == 2) {
                            empr_helper.notify(responce.msg, responce.msgType);
                        }
                        
                    }, false, true);
                }
            });

            $('body').on('click', '#d_Test_Btn', function () {
                if (empr_Binaries.ValidateDestinationInfo()) {
                    var data = empr_Binaries.GetDataDestinationDDL();
                    ajaxHelper.ajaxPostJsonData(data, "/Binaries/GetDatabaseObjects", function (responce) {

                        if (responce.msgType == 1) {
                            empr_Binaries.destinationDbObject = responce.data;
                            empr_Binaries.destinationFlatTreeData = empr_Binaries.flattenTree(empr_Binaries.destinationDbObject);
                            empr_helper.notify(responce.msg, responce.msgType);
                            $('#fromDbName').text(data.D_DATABASE);
                        }
                        else if (responce.msgType == 2) {
                            empr_helper.notify(responce.msg, responce.msgType);
                        }

                        
                    }, false, true);
                }
            });

            $('body').on('click', '#btn_Options', function () {
                empr_helper.CreateCustomPermissionGrid(empr_Binaries.sourceDbObject, empr_Binaries.sourceFlatTreeData, "#sourceTreeViewContainer", false, 'source');
                empr_helper.CreateCustomPermissionGrid(empr_Binaries.destinationDbObject, empr_Binaries.destinationFlatTreeData, "#destinationTreeViewContainer", false, 'destination');
                var modal = new bootstrap.Modal(document.getElementById('optionsModal'));
                modal.show();
            });
            
            $('#s_Auth').change(function () {
                var S_AUTH = $("#s_Auth").val();
                if (S_AUTH == 'Windows') {
                    $('#s_user').prop('disabled', true);
                    $('#s_pass').prop('disabled', true);
                } else {
                    $('#s_user').prop('disabled', false);
                    $('#s_pass').prop('disabled', false);
                }
            });

            $('#d_Auth').change(function () {
                console.log('change d auth');
                var D_AUTH = $("#d_Auth").val();
                console.log(D_AUTH);
                if (D_AUTH == 'Windows') {
                    $('#d_user').prop('disabled', true);
                    $('#d_pass').prop('disabled', true);
                } else {
                    $('#d_Username').prop('disabled', false);
                    $('#d_Pass').prop('disabled', false);
                }
            });

            // Done
            $('#s_Database .dx-texteditor-container').on('click', function () {
                empr_Binaries.SourceDatabaseDDL();
            });

            // Done
            $('#d_Database .dx-texteditor-container').on('click', function () {
                empr_Binaries.DestinationDatabaseDDL();
            });

            //$('#btn_Compare').on('click', function () {
            //        // parent child both
            //    //var selectedSourceData = empr_Binaries.sourceFlatTreeData.filter(function (item) {
            //    //    return item.rowselection === true;
            //    //});

            //        // just childs
            //    var selectedSourceData = empr_Binaries.sourceFlatTreeData.filter(function (item) {
            //        return item.rowselection === true &&
            //            !empr_Binaries.sourceFlatTreeData.some(child => child.parentId === item.id);
            //    });

            //    var selectedDestData = empr_Binaries.destinationFlatTreeData.filter(function (item) {
            //        return item.rowselection === true;
            //    });

            //    var result = {
            //        source: selectedSourceData,
            //        destination: selectedDestData 
            //    };

            //    console.log('Selected Data:', result);

            //});

            $('#btn_Compare').on('click', function () {
                // ✅ Source aur Destination data arrays
                var source = empr_Binaries.sourceFlatTreeData || [];
                var destination = empr_Binaries.destinationFlatTreeData || [];

                // ✅ Extract names/ids from destination for quick lookup
                var destinationNames = destination.map(item => item.name.toLowerCase());

                // ✅ Find source items jo destination me nahi hain
                var notInDestination = source.filter(item => {
                    return !destinationNames.includes(item.name.toLowerCase());
                });

                // ✅ Result object bana lo
                var compareResult = {
                    missingInDestination: notInDestination
                };

                console.log("Compare Result:", compareResult);

                // Example: agar UI me dikhana ho
                var html = "<h3>Missing in Destination:</h3><ul>";
                notInDestination.forEach(item => {
                    html += "<li>" + item.name + "</li>";
                });
                html += "</ul>";

                $("#compareResultContainer").html(html);
            });



            $('#btn_GenerateScripts').on('click', function () {
                var missing = compareResult.missingInDestination;

                var payload = {
                    IsSource: true,  // ya false — tum decide karna hai
                    MissingObjects: missing.map(x => {
                        return { Name: x.name, Type: x.id.split('_')[0] };
                    })
                };

                ajaxHelper.ajaxPostJsonData(payload, "/Binaries/GetMissingCreateScripts", function (res) {
                    if (res.msgType == 1) {
                        var html = "<h3>Generated SQL Scripts:</h3><pre>";
                        res.data.forEach(script => {
                            html += script + "\n";
                        });
                        html += "</pre>";
                        $("#scriptsContainer").html(html);
                    } else {
                        empr_helper.notify(res.msg, res.msgType);
                    }
                });
            });




        });
    },

    ValidateSourceInfo: function () {

        var valid = true;
        var data = empr_Binaries.GetDataSourceDDL();
        if (data.S_SERVER_HOST == '' || data.S_SERVER_HOST == null) {
            empr_helper.notify("Source Server Host is required.", 2);
            valid = false;
            return valid;
        }

        if (data.S_AUTH == '' || data.S_AUTH == null) {
            empr_helper.notify("Source Authentication is required.", 2);
            valid = false;
            return valid;
        }

        if (data.S_AUTH == 'SQL') {
            if (data.S_USERNAME == '' || data.S_USERNAME == null) {
                empr_helper.notify("Source Username is required.", 2);
                valid = false;
                return valid;
            }

            if (data.S_PASS == '' || data.S_PASS == null) {
                empr_helper.notify("Source Password is required.", 2);
                valid = false;
                return valid;
            }
        }

        if (data.S_DATABASE == '' || data.S_DATABASE == null) {
            empr_helper.notify("Source Database is required.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },

    ValidateDestinationInfo: function () {

        var valid = true;
        var data = empr_Binaries.GetDataDestinationDDL();
        if (data.D_SERVER_HOST == '' || data.D_SERVER_HOST == null) {
            empr_helper.notify("Destination Server Host is required.", 2);
            valid = false;
            return valid;
        }

        if (data.D_AUTH == '' || data.D_AUTH == null) {
            empr_helper.notify("Destination Authentication is required.", 2);
            valid = false;
            return valid;
        }

        if (data.D_AUTH == 'SQL') {

            if (data.D_USERNAME == '' || data.D_USERNAME == null) {
                empr_helper.notify("Destination Username is required.", 2);
                valid = false;
                return valid;
            }

            if (data.D_PASS == '' || data.D_PASS == null) {
                empr_helper.notify("Destination Password is required.", 2);
                valid = false;
                return valid;
            }
        }

        if (data.D_DATABASE == '' || data.D_DATABASE == null) {
            empr_helper.notify("Destination Database is required.", 2);
            valid = false;
            return valid;
        }

        return valid;
    },

    GetDataSourceDDL: function () {

        var S_SERVER_HOST = $("#s_Server_Host").val();
        var S_AUTH = $("#s_Auth").val();

        if (S_AUTH == "SQL")
        {
            var S_USERNAME = $('#s_user').val();
            var S_PASS = $("#s_pass").val();
        }

        var S_DATABASE = $('#s_Database').dxSelectBox('instance').option('value');

        var sourceRecord = {
            S_SERVER_HOST: S_SERVER_HOST,
            S_AUTH: S_AUTH,
            S_USERNAME: S_USERNAME,
            S_PASS: S_PASS,
            S_DATABASE: S_DATABASE,
            DB_TYPE: 'source',
        }

        return sourceRecord;
    },

    GetDataDestinationDDL: function () {

        var D_SERVER_HOST = $("#d_Server_Host").val();
        var D_AUTH = $("#d_Auth").val();

        if (D_AUTH == "SQL") {
            var D_USERNAME = $('#d_user').val();
            var D_PASS = $("#d_pass").val();
        }

        var D_DATABASE = $('#d_Database').dxSelectBox('instance').option('value');

        var destRecord = {
            D_SERVER_HOST: D_SERVER_HOST,
            D_AUTH: D_AUTH,
            D_USERNAME: D_USERNAME,
            D_PASS: D_PASS,
            D_DATABASE: D_DATABASE,
            DB_TYPE: 'destination',
        }

        return destRecord;
    },

    SourceDatabaseDDL: function () {
        var dataModel = empr_Binaries.GetDataSourceDDL();

        if (dataModel.S_SERVER_HOST != '' && dataModel.S_USERNAME != '' && dataModel.S_PASS != '') {
            ajaxHelper.ajaxPostJsonData(dataModel, "/Binaries/SourceDatabaseDDL", function (data) {
                empr_Binaries.InitSourceDatabaseDDL(data);
                empr_helper.notify(data.msg, data.msgType);
            }, false, true);
        }
    },

    DestinationDatabaseDDL: function () {
        var dataModel = empr_Binaries.GetDataDestinationDDL();

        ajaxHelper.ajaxPostJsonData(dataModel, "/Binaries/DestinationDatabaseDDL", function (data) {
            empr_Binaries.InitDestinationDatabaseDDL(data);
            empr_helper.notify(data.msg, data.msgType);
        }, false, true);
    },

    InitSourceDatabaseDDL: function (data) {
            $('#s_Database').dxSelectBox({
                dataSource: data.data,
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
    },

    InitDestinationDatabaseDDL: function (data) {
        $('#d_Database').dxSelectBox({
            dataSource: data.data,
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
    },

    InitTree: function (div, columns, datasrc, fileName, type) {
        let grid = $(div).dxTreeList({
            "dataSource": datasrc,
            "keyExpr": 'id',
            "parentIdExpr": 'parentId',
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
            "selection": {
                "mode": "none"
            },
            "editing": {
                "mode": "row",
                "allowUpdating": false,
                "allowAdding": false,
                "allowDeleting": false,
            },
            
        }).dxTreeList('instance');

        if (type === 'source') {
            empr_Binaries.sourceGridInstance = grid;
        } else if (type === 'destination') {
            empr_Binaries.destinationGridInstance = grid;
        }
    },

    flattenTree: function (data, parentId = null) {
        let flat = [];
        data.forEach(item => {
            flat.push({
                id: item.id,
                name: item.name,
                parentId: parentId
            });
            if (item.children && item.children.length > 0) {
                flat = flat.concat(empr_Binaries.flattenTree(item.children, item.id));
            }
        });
        return flat;
    },

    HandleRowSelectionCheckBox: function (isChecked, id, parentId, type) {

        if (type === 'source') {
            empr_Binaries.sourceFlatTreeData = empr_Binaries.sourceFlatTreeData.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked };
                }
                return record; // ✅ har record ka return zaroori hai
            });
        }
        else if (type === 'destination') {
            empr_Binaries.destinationFlatTreeData = empr_Binaries.destinationFlatTreeData.map(record => {
                if (record.id === id) {
                    return { ...record, rowselection: isChecked };
                }
                return record; // ✅ yahan bhi
            });
        }

        // 2. Children update karo (recursive)
        empr_Binaries.HandleChildPageCheckboxChange('rowselection', isChecked, id, type);

        // 3. Parent ko update karo (upar propagate)
        empr_Binaries.CheckPageCheckBoxesByColumnName('rowselection', parentId, type);
        empr_Binaries.refreshTree(type);
    },

    CheckPageCheckBoxesByColumnName: function (columnName, parentId, type) {
        if (!parentId) return; // agar parent hi nahi hai to stop
        let newParentId = null;

        if (type === 'source') {
            const anyChecked = empr_Binaries.sourceFlatTreeData
                .filter(record => record.parentId === parentId)
                .some(record => record[columnName] === true);

            const rowChecked = empr_Binaries.sourceFlatTreeData
                .filter(record => record.parentId === parentId)
                .some(record => record.rowselection === true);

            empr_Binaries.sourceFlatTreeData = empr_Binaries.sourceFlatTreeData.map(record => {
                if (record.id === parentId) {
                    newParentId = record.parentId;
                    return { ...record, [columnName]: anyChecked, rowselection: rowChecked };
                }
                return record;
            });
        }
        else if (type === 'destination') {
            const anyChecked = empr_Binaries.destinationFlatTreeData
                .filter(record => record.parentId === parentId)
                .some(record => record[columnName] === true);

            const rowChecked = empr_Binaries.destinationFlatTreeData
                .filter(record => record.parentId === parentId)
                .some(record => record.rowselection === true);

            empr_Binaries.destinationFlatTreeData = empr_Binaries.destinationFlatTreeData.map(record => {
                if (record.id === parentId) {
                    newParentId = record.parentId;
                    return { ...record, [columnName]: anyChecked, rowselection: rowChecked };
                }
                return record;
            });
        }

        if (newParentId) {
            empr_Binaries.CheckPageCheckBoxesByColumnName(columnName, newParentId);
        }
    },

    HandleChildPageCheckboxChange: function (columnName, isChecked, id, type) {
        let newIds = [];

        if (type === 'source') {
            empr_Binaries.sourceFlatTreeData = empr_Binaries.sourceFlatTreeData.map(record => {
                if (record.parentId === id) {
                    newIds.push(record.id);
                    return { ...record, [columnName]: isChecked };
                }
                return record;
            });
        }
        else if (type === 'destination') {
            empr_Binaries.destinationFlatTreeData = empr_Binaries.destinationFlatTreeData.map(record => {
                if (record.parentId === id) {
                    newIds.push(record.id);
                    return { ...record, [columnName]: isChecked };
                }
                return record;
            });
        }

        if (newIds.length > 0) {
            newIds.forEach(newId => {
                empr_Binaries.HandleChildPageCheckboxChange(columnName, isChecked, newId, type);
            });
        }
    },

    refreshTree: function (type) {

        if (type === 'source' && empr_Binaries.sourceGridInstance) {
            empr_Binaries.sourceGridInstance.option("dataSource", empr_Binaries.sourceFlatTreeData);
        }
        else if (type === 'destination' && empr_Binaries.destinationGridInstance) {
            empr_Binaries.destinationGridInstance.option("dataSource", empr_Binaries.destinationFlatTreeData);
        }
    }


}