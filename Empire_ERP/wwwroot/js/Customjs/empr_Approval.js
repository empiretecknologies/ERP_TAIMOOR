var empr_Approval = {
    InitEvents: function () {
        $(document).ready(function () {

            empr_Approval.GetGridData();
            empr_Approval.GetSetupGridData();
        });

        $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr("href");
            if (target === '#pills-warninghome') {
                $('#tab1Wrapper').show();
                $('#tab2Wrapper').hide();
                $('#updateSetup').hide();
                $('#updateTran').show();


            }
            if (target === '#pills-warningcontact') {
                $('#tab1Wrapper').hide();
                $('#tab2Wrapper').show(); 
                $('#updateTran').hide();
                $('#updateSetup').show();



            }
        });


        $('body').on('click', '#BtnSave', function () {
            if (empr_Approval.ValidateInfo()) {
                empr_Approval.Save();
            }
        }); 
        $('body').on('click', '#BtnSaveSetup', function () {
            if (empr_Approval.ValidateInfo()) {
                empr_Approval.SaveSetup();
            }
        }); 

        //BtnSaveSetup

        //$('body').on('click', '#ChangeModalCloseBtn', function () {
        //    empr_Approval.InitDebitDDL();
        //    empr_Approval.InitCreditDDL();
        //    $("#REMARKS").val('');
        //}); 

        //$('body').on('click', '#elm_print', function () {

        //    var row = $(this).closest('tr');
        //    var amount = row.find('td').eq(6).text().trim();

        //    var Brnach = $("#TBCODE").dxSelectBox('option', 'value');

        //    ajaxHelper.ajaxGetJson(`/Hawla/PrintModal?amount=${amount}&&branch=${Brnach}`, function (data) {
        //        if (data.msgType == 1) {

        //            $('#PrintModals').html(data.slipHtml);

        //            $('#PrintModal').modal('show');
        //            //empr_Approval.CreateGrid(data.data);
        //        } else {
        //            //empr_helper.notify(data.msg, data.msgType);
        //        }
        //    }, false, true);

        //});

        //$('body').on('click', '#updateBtn', function () {
        //    var $modal = $('#ChangeModal');

        //    $modal.find('.modal-dialog').addClass('fade-out');

        //    setTimeout(function () {
        //        $modal.modal('hide');
        //        $modal.find('.modal-dialog').removeClass('fade-out'); 
        //    }, 400); 

        //    $("#Loader").show();
        //    $("#Loader").css('display', 'flex');
        //    setTimeout(function () {
        //        if (empr_Approval.ValidateInfo()) {
        //            empr_Approval.Save();

        //            setTimeout(function () {
        //                $("#Loader").hide();
        //            }, 500);
        //        }
        //    }, 200);
            
        //});

        //$('#Screenshot').click(function () {
        //    html2canvas(document.querySelector('#PrintModals')).then(function (canvas) {
        //        // Convert to Base64 image
        //        var imageData = canvas.toDataURL("image/png");

        //        // Create temporary download link
        //        var link = document.createElement('a');
        //        link.href = imageData;
        //        link.download = 'screenshot.png';

        //        // Trigger download
        //        document.body.appendChild(link);
        //        link.click();
        //        document.body.removeChild(link);
        //    });
        //});

        //if (Permissions != "Admin") {
        //    (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
        //    !Permissions.r_VIEW && $('#gridContainer').hide();
        //}
    },
    GetGridData: function () {
        ajaxHelper.ajaxGetJson('/Approval/GetApprovals', function (data) {
            debugger;

            dataSrc = data.data;
            if (data.msgType == 1) {
                empr_Approval.CreateGrid(dataSrc);
            }
        });
    },
    GetSetupGridData: function () {
        ajaxHelper.ajaxGetJson('/Approval/GetApprovalSetup', function (data) {
            debugger;
            dataSrc = data.data;
            if (data.msgType == 1) {
                empr_Approval.CreateSetupGrid(dataSrc);
            }
        });
    },
    CreateGrid: function (dataSrc) {


        var col = [
            { dataField: 'voucheR_DATE', caption: 'Date' },
            {
                dataField: 'voucherNo', caption: 'Transaction #',
                cellTemplate: function (container, options) {
                    $('<a>')
                        .addClass('dx-link')
                        .text(options.value)
                        .attr('href', '#')
                        .attr('onclick', 'empr_Approval.openVoucherPage(' + JSON.stringify(options.data.link) + ', ' + JSON.stringify(options.data.mtraN_ID) + ')')
                        .appendTo(container);
                }
            },
            //{ dataField: 'voucherNo', caption: 'Transaction #', width: 120 },
            { dataField: 'booK_TYPE', caption: 'Book Type', width: 200 },
            { dataField: 'acT_NAME', caption: 'Account Name', width: 200 },
            { dataField: 'amt', caption: 'Amount', width: 80 },
            { dataField: 'remarks', caption: 'Remarks' },
            { dataField: 'traN_ID', caption: 'Tran Id', visible: false },
            { dataField: 'tabler', caption: 'Table', visible: false },

            { dataField: 'bcode', caption: 'BCODE', visible: false },
            { dataField: 'perioD_ID', caption: 'PERIOD_ID', visible: false },
            { dataField: 'ediT_USER_ID', caption: 'Create User', visible: false },



        ];
        
        //empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, 'ApprovalS');

        //empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "MaterialRequisitionQS");
        //setTimeout(function () {
        //    $('#gridContainer').dxDataGrid('instance').resize();
        //}, 500);
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, 'ApprovalS', 'multiple', {
            keyExpr: "voucherNo",
            selection: {
                mode: 'multiple',
                showCheckBoxesMode: 'always'
            },
            
            onSelectionChanged: function (e) {
                var grid = e.component;
                var data = grid.getDataSource().items();

                grid.beginUpdate();

                // 🔥 SELECT ALL
                if (e.selectedRowKeys.length === data.length) {
                    data.forEach(function (row) {
                        grid.selectRows(row.__rowKey, true);
                    });

                    grid.endUpdate();
                    return;
                }

                // 🔥 DESELECT ALL
                if (e.selectedRowKeys.length === 0) {
                    data.forEach(function (row) {
                        grid.deselectRows(row.__rowKey);
                    });

                    grid.endUpdate();
                    return;
                }

                // 🔹 NORMAL ROW CLICK
                var changedKeys = e.currentSelectedRowKeys
                    .concat(e.currentDeselectedRowKeys);

                changedKeys.forEach(function (rowKey) {

                    var changedRow = data.find(r => r.__rowKey === rowKey);
                    if (!changedRow) return;

                    var voucherNo = changedRow.voucherNo;
                    var isSelected = e.selectedRowKeys.includes(rowKey);

                    data.forEach(function (row) {
                        if (row.voucherNo === voucherNo) {

                            var alreadySelected = grid.isRowSelected(row.__rowKey);

                            if (isSelected && !alreadySelected) {
                                grid.selectRows(row.__rowKey, true);
                            }
                            else if (!isSelected && alreadySelected) {
                                grid.deselectRows(row.__rowKey);
                            }
                        }
                    });
                });

                grid.endUpdate();
            }

        });

         

         

    },
    CreateSetupGrid: function (dataSrc) {


        var col = [
         
            { dataField: 'grouP_CODE', caption: 'Code', width:100, },
            { dataField: 'grouP_NAME', caption: 'Group Name' },
            { dataField: 'menU_NAME', caption: 'Menu Name' },
            { dataField: 'ediT_USER_ID', caption: 'Create User'},
            //{ dataField: 'traN_ID', caption: 'Tran Id', visible: false },
            { dataField: 'tabler', caption: 'Table', visible:false, },
         

        ];

     
        empr_helper.dxGridbindingVouchers('#TransactionContainer', col, dataSrc, 'ApprovalT', 'multiple');

    },
    ValidateInfo: function () {
        debugger;
        var valid = true;
        var activeTab = $('a[data-bs-toggle="tab"].active').attr("href");

        if (activeTab === '#pills-warninghome') {

            var grid = $("#gridContainer").dxDataGrid("instance");
            var data = grid.getSelectedRowsData();
            if (data.length == 0 || data == null || data == undefined) {
                empr_helper.notify("Please select at least one record to approve 1 .", 2);
                valid = false;
            }
        }

        if (activeTab === '#pills-warningcontact') {

            var grid1 = $("#TransactionContainer").dxDataGrid("instance");
            var data1 = grid1.getSelectedRowsData();
            if (data1.length == 0 || data1 == null || data1 == undefined) {
                empr_helper.notify("Please select at least one record to approve. 2", 2);
                valid = false;
            }
        }

        return valid;
    },
    Save: function () {
        debugger;

        var grid = $("#gridContainer").dxDataGrid("instance");
        var data =grid.getSelectedRowsData();
      
        var postData = $.param({ modelRecord: data });
        debugger;
        ajaxHelper.ajaxPostJsonData(postData, "/Approval/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Approval.GetSetupGridData();
            }
        }, false, true);
    },
    SaveSetup: function () {
        debugger;
        var grid = $("#TransactionContainer").dxDataGrid("instance");
        var data = grid.getSelectedRowsData();

        var postData = $.param({ modelRecord: data });
        debugger;
        ajaxHelper.ajaxPostJsonData(postData, "/Approval/SaveSetup", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Approval.GetGridData();
            }
        }, false, true);
    },
    openVoucherPage(link, mtraN_ID) {
        debugger
        console.log(link)
        var newWindow = window.open(link, '_blank');
        newWindow.addEventListener('load', function () {
            setTimeout(function () {
                newWindow.postMessage({ traN_ID: mtraN_ID }, '*');
            }, 1000);
        });
    },
   
}