var empr_LoginDetails = {
    InitEvents: function () {
        AllPeriods = [];
        $(document).ready(function () {
            empr_LoginDetails.LoadAllDDL();
            //empr_LoginDetails.InitCompaniesDDL();
            //empr_LoginDetails.InitBranchesDDL();
            //empr_LoginDetails.InitPeriodsDDL();

            $('#nextmove').click(function () {
                debugger;
                var company = $('#companydxddl').dxSelectBox('instance').option('value');
                var branch = $('#branchdxddl').dxSelectBox('instance').option('value');
                var period = $('#perioddxddl').dxSelectBox('instance').option('value');
                var isvalid = true;
                if (company == '' || company == null || company == undefined) {
                    isvalid = false;
                    empr_helper.notify('Please select company.', 2);
                }
                if (branch == '' || branch == null || branch == undefined) {
                    isvalid = false;
                    empr_helper.notify('Please select branch.', 2);
                }
                if (period == '' || period == null || period == undefined) {
                    isvalid = false;
                    empr_helper.notify('Please select period.', 2);
                }

                if (!isvalid) {
                    return;
                }

                //if (window.location.protocol == 'https:') {
                //    if (navigator.geolocation) {
                //        navigator.geolocation.getCurrentPosition(empr_LoginDetails.ShowPosition, empr_LoginDetails.ShowError);
                //    } else {
                //        empr_helper.notify("Geolocation is not supported by this browser.", 2);
                //    }
                //}
                //else {

                //}
                var xhr = ajaxHelper.ajaxGetJson('/Login/SaveLoginUserDetails?company=' + company + "&branch=" + branch + "&period=" + period, function (data) {
                    debugger;
                    if (data.msgType == 1) {
                        empr_helper.notify(data.msg, 1);
                        window.location.href = "/Home";
                    }
                    else {
                        empr_helper.notify(data.msg, 2);
                    }
                }, false, true);
            });
        });
    },
    ShowPosition: function (position) {
        debugger;
        if (position != null || position != [] || position != undefined) {
            debugger;
            var company = $('#companydxddl').dxSelectBox('instance').option('value');
            var branch = $('#branchdxddl').dxSelectBox('instance').option('value');
            var period = $('#perioddxddl').dxSelectBox('instance').option('value');
            var xhr = ajaxHelper.ajaxGetJson('/Login/SaveLoginUserDetails?company=' + company + "&branch=" + branch + "&period=" + period + "&lat=" + position.coords.latitude + "&lon=" + position.coords.longitude, function (data) {
                debugger;
                if (data.msgType == 1) {
                    empr_helper.notify(data.msg, 1);
                    window.location.href = "/Home";
                }
                else {
                    empr_helper.notify(data.msg, 2);
                }
            }, false, true);
        }
    },
    ShowError: function (error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                empr_helper.notify("Please allow the location permission.", 2);
                break;
            case error.POSITION_UNAVAILABLE:
                empr_helper.notify("Please allow the location permission.", 2);
                break;
            case error.TIMEOUT:
                empr_helper.notify("Please allow the location permission.", 2);
                break;
            case error.UNKNOWN_ERROR:
                empr_helper.notify("Please allow the location permission.", 2);
                break;
        }
    },
    //InitCompaniesDDL: function (selectedValue) {
    //    $.ajax({
    //        url: '/Company/GetAllCompanies',
    //        method: 'GET',
    //        success: function (data) {
    //            $('#companydxddl').dxSelectBox({
    //                dataSource: data.data.table,
    //                displayExpr: 'c_NAME',
    //                valueExpr: 'ccode',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    empr_LoginDetails.InitBranchesDDL(e.value);
    //                },
    //                onInitialized: function (e) {
    //                    //if (data.data.table.length > 0) {
    //                    //    e.component.option('value', data.data.table[0].ccode);
    //                    //}
    //                }
    //            });
    //            setTimeout(function () {
    //                if (data.data.table.length > 0) {
    //                    $('#companydxddl').dxSelectBox('instance').option('value', data.data.table[0].ccode);
    //                }
    //            },100);
    //        },
    //        error: function (error) {
    //            console.error('Error fetching data:', error);
    //        }
    //    });
    //},
    //InitBranchesDDL: function (companyID, selectedValue) {
    //    $.ajax({
    //        url: '/Branch/GetBranchesByCompany?id=' + companyID,
    //        method: 'GET',
    //        success: function (data) {
    //            $('#branchdxddl').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'b_NAME',
    //                valueExpr: 'bcode',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onValueChanged: function (e) {
    //                    empr_LoginDetails.InitPeriodsDDL(e.value);
    //                },
    //                onInitialized: function (e) {
    //                    //if (data.data.table.length > 0) {
    //                    //    e.component.option('value', data.data.table[0].bcode);
    //                    //}
    //                }
    //            });

    //            setTimeout(function () {
    //                if (data.data.length > 0) {
    //                    $('#branchdxddl').dxSelectBox('instance').option('value', data.data[0].bcode);
    //                }
    //            }, 100);
    //        },
    //        error: function (error) {
    //            console.error('Error fetching data:', error);
    //        }
    //    });
    //},
    //InitPeriodsDDL: function (branchID, selectedValue) {
    //    $.ajax({
    //        url: '/Period/GetPeriodsByBranch?branchid=' + branchID,
    //        method: 'GET',
    //        success: function (data) {
    //            $('#perioddxddl').dxSelectBox({
    //                dataSource: data.data,
    //                displayExpr: 'descr',
    //                valueExpr: 'pid',
    //                value: selectedValue,
    //                searchEnabled: true,
    //                width: '100%',
    //                placeholder: 'Search',
    //                showClearButton: true,
    //                dropDownOptions: {
    //                    height: 'auto',
    //                },
    //                pagingEnabled: true,
    //                searchTimeout: 500,
    //                onInitialized: function (e) {
    //                    //if (data.data.table.length > 0) {
    //                    //    e.component.option('value', data.data.table[0].pid);
    //                    //}
    //                }
    //            });

    //            setTimeout(function () {
    //                if (data.data.length > 0) {
    //                    $('#perioddxddl').dxSelectBox('instance').option('value', data.data[0].pid);
    //                }
    //            }, 100);
    //        },
    //        error: function (error) {
    //            console.error('Error fetching data:', error);
    //        }
    //    });
    //},
    LoadAllDDL: function () {
        $.ajax({
            url: '/Login/GetAllDDL',
            method: 'GET',
            success: function (data) {
                console.log(data);
                empr_LoginDetails.AllPeriods = data.data.allPeriods;
                var selectedValue = null;
                console.log(`company : ${data.data.companies}`);
                $('#companydxddl').dxSelectBox({
                    dataSource: data.data.companies,
                    displayExpr: 'c_NAME',
                    valueExpr: 'ccode',
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
                        //empr_LoginDetails.InitBranchesDDL(e.value);
                    },
                    onInitialized: function (e) {
                        //if (data.data.table.length > 0) {
                        //    e.component.option('value', data.data.table[0].ccode);
                        //}
                    }
                });
                $('#branchdxddl').dxSelectBox({
                    dataSource: data.data.branches,
                    displayExpr: 'b_NAME',
                    valueExpr: 'bcode',
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
                        var Period = empr_LoginDetails.AllPeriods.filter(x => x.bcode == e.value);
                        var instance = $("#perioddxddl").dxSelectBox("instance");
                        instance.option("dataSource", Period);

                        setTimeout(function () {
                            if (Period.length > 0) {
                                $('#perioddxddl').dxSelectBox('instance').option('value', Period[0].pid);
                            }
                        }, 100);
                        //empr_LoginDetails.InitPeriodsDDL(e.value);
                    },
                    onInitialized: function (e) {
                        //if (data.data.table.length > 0) {
                        //    e.component.option('value', data.data.table[0].bcode);
                        //}
                    }
                });
                $('#perioddxddl').dxSelectBox({
                    dataSource: data.data.periods,
                    displayExpr: 'descr',
                    valueExpr: 'pid',
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
                    onInitialized: function (e) {
                        //if (data.data.table.length > 0) {
                        //    e.component.option('value', data.data.table[0].pid);
                        //}
                    }
                });
                setTimeout(function () {
                    if (data.data.companies.length > 0) {
                        $('#companydxddl').dxSelectBox('instance').option('value', data.data.companies[0].ccode);
                    }

                    if (data.data.branches.length > 0) {
                        $('#branchdxddl').dxSelectBox('instance').option('value', data.data.branches[0].bcode);
                    }

                    if (data.data.periods.length > 0) {
                        $('#perioddxddl').dxSelectBox('instance').option('value', data.data.periods[0].pid);
                    }
                }, 100);

            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    }
}