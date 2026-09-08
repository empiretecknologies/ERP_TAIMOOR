var empr_MpoLayout = {
    totalCount: 0,
    rowsCount: 0,
    parties: [],
    PartiesData: [],
    pickId: 0,
    PickQty: 0,
    //BtnBatchPick
    //BtnAddBatch
    InitEvents: function () {
        $(document).ready(function () {
            console.log('ClientPO', ClientPO);
            console.log('Items', Items);
            empr_MpoLayout.InitReportTypeDDL();
            empr_MpoLayout.InitPOJobGridDDL(null);
            empr_MpoLayout.ResetForm();
            ajaxHelper.ajaxGetJson("/DeliveryFeeding/GetParties", function (data) {
                if (data.msgType == 1) {
                    empr_MpoLayout.PartiesData = data.data;
                    parties = data.data;
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            }, false, true);

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_MpoLayout.InitQuickSearchGrid();
            });

            //$('body').on('click', '.my-griddoc-browse', function () {
            //    $('#gridDOC').val('');
            //    $('#gridDOCName').val('');
            //    $('#gridDOC').click();
            //});

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.vHide').show();
                $('.modal').modal('hide');
                empr_MpoLayout.GetMpoLayoutByCode(id);

            });

            $('body').on('click', '.elm_copy', function () {
                var id = $(this).attr("reportid");
                var date = $(this).attr("reportdate");
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
                    $('#updatedDate').val(date);
                    empr_helper.selectedBill = id;
                    $('#CopyViewModal').modal('show');
                });
            });

            $('body').on('click', '#saveCopiedRecord', function () {
                ajaxHelper.ajaxPostJsonData({ traN_ID: empr_helper.selectedBill, v_DATE: $('#updatedDate').val() }, "/MpoLayout/CopyRecord", function (data) {
                    empr_helper.notify(data.msg, data.msgType);
                    if (data.msgType == 1) {
                        $('.modal').modal('hide');
                        empr_MpoLayout.GetMpoLayoutByCode(data.data.code);
                    }
                }, false, true);
            });


            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_MpoLayout.ValidateInfo()) {
                            empr_MpoLayout.SaveInfo();
                        }
                    }
                } else {
                    if (empr_MpoLayout.ValidateInfo()) {
                        empr_MpoLayout.SaveInfo();
                    }
                }
            });

            $('body').on('click', '#BtnNew', function () {
                empr_MpoLayout.ResetForm();
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_MpoLayout.Delete();
            });

            $('body').on('click', '#BtnPrint, #BtnGenerateReport', function () {
                empr_MpoLayout.GeneratePrintReport();
            });

            $('body').on('click', '.elm_print', function () {
                empr_helper.selectedBill = $(this).attr("reportid");
                empr_MpoLayout.GeneratePrintReport();
            });

            //$('body').on('click', '#BtnBatchPick', function () { // first  
            //    empr_MpoLayout.InitBatchPickGrid();
            //});

            $('body').on('click', '#BtnAddBatch', function () { // second
                //debugger;
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();

                    empr_MpoLayout.pickId = selectedSodas[0].picK_ID;
                    empr_MpoLayout.PickQty = selectedSodas[0].qty;
                    //empr_MpoLayout.InitSupplierDDL(Supplier.value.data, selectedSodas[0].spartY_CODE);
                    empr_MpoLayout.CreateGrid([selectedSodas[0]]);

                    //$('.modal').hide();
                    // Model double clicks
                    var modalEl = document.getElementById('SodaPickModal');
                    var modalInstance = bootstrap.Modal.getInstance(modalEl);
                    if (modalInstance) {
                        modalInstance.hide(); // proper close
                    }

                    //$('#CLIENT_PO').prop('disabled', true);
                    $('#V_DATE').focus();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('.btn-print').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }

        });
    },
    GeneratePrintReport: function () {

        empr_MpoLayout.InitReportTypeDDL();
        let TRAN_ID = empr_helper.selectedBill;
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/MpoLayout/GetPrintReport", function (data) {
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
    },
    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/MpoLayout/GetReportTypes", function (data) {
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
    ResetForm: function () {
        empr_MpoLayout.CreateGrid([{ __KEY__: empr_MpoLayout.GenerateKey(36) }]);
        empr_MpoLayout.pickId = 0;
        $('.Record input').not('#ASTATUS, .dx-texteditor-input, #V_DATE').val('');
        $('#REMARKS').val('');
        $('#BtnPrint').hide();
        $('#key_hidden').val('');
        $('#BtnDelete').hide();
        $('#CLIENT_PO').val('');
        $('#PARTY_CODE').val('');
        empr_MpoLayout.InitPOJobGridDDL(null);
        //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
        $('.card-body').removeClass('customHighlightForModifiedCells');
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
        empr_MpoLayout.CreateGrid([{ __KEY__: empr_MpoLayout.GenerateKey(36) }]);
        if ($('#FinishItem').data('dxSelectBox') != null) {
            $('#FinishItem').dxSelectBox('instance').dispose();
        }
        //empr_MpoLayout.InitPartyCodeDDL();
        //$('#CLIENT_PO').prop('disabled', false);
    },
    CreateGrid: function (dataSrc) {
        console.log('CreateGrid', dataSrc);

        if (dataSrc.length > 0) {
            empr_MpoLayout.rowsCount = dataSrc.length - 1;
            //dataSrc.forEach(item => {
            //    if (
            //        item.shiP_DATE == '1900-01-01' || item.shiP_DATE == '01-01-1900' || item.shiP_DATE == '01-Jan-1900' || item.shiP_DATE == '1/1/1900 12:00:00 AM' || item.shiP_DATE == '01/01/1900 12:00:00 AM' || item.shiP_DATE == '1/1/1900' ||
            //        item.shiP_DATE == '2000-01-01' || item.shiP_DATE == '01-01-2000' || item.shiP_DATE == '01-Jan-2000' || item.shiP_DATE == '1/1/2000 12:00:00 AM' || item.shiP_DATE == '01/01/2000 12:00:00 AM' || item.shiP_DATE == '1/1/2000' ||
            //        item.shiP_DATE == '00-01-01' || item.shiP_DATE == '01-01-00' || item.shiP_DATE == '01-Jan-00' || item.shiP_DATE == '1/1/00 12:00:00 AM' || item.shiP_DATE == '01/01/00 12:00:00 AM' || item.shiP_DATE == '1/1/00' || item.shiP_DATE == '01-Jan-00 12:00:00 AM'
            //    ) {
            //        item.shiP_DATE = null;
            //    }
            //    if (
            //        item.bookinG_DATE == '1900-01-01' || item.bookinG_DATE == '01-01-1900' || item.bookinG_DATE == '01-Jan-1900' || item.bookinG_DATE == '1/1/1900 12:00:00 AM' || item.bookinG_DATE == '01/01/1900 12:00:00 AM' || item.bookinG_DATE == '1/1/1900' ||
            //        item.bookinG_DATE == '2000-01-01' || item.bookinG_DATE == '01-01-2000' || item.bookinG_DATE == '01-Jan-2000' || item.bookinG_DATE == '1/1/2000 12:00:00 AM' || item.bookinG_DATE == '01/01/2000 12:00:00 AM' || item.bookinG_DATE == '1/1/2000' ||
            //        item.bookinG_DATE == '00-01-01' || item.bookinG_DATE == '01-01-00' || item.bookinG_DATE == '01-Jan-00' || item.bookinG_DATE == '1/1/00 12:00:00 AM' || item.bookinG_DATE == '01/01/00 12:00:00 AM' || item.bookinG_DATE == '1/1/00' || item.bookinG_DATE == '01-Jan-00 12:00:00 AM'
            //    ) {
            //        item.bookinG_DATE = null;
            //    }
            //    if (
            //        item.handoveR_DATE == '1900-01-01' || item.handoveR_DATE == '01-01-1900' || item.handoveR_DATE == '01-Jan-1900' || item.handoveR_DATE == '1/1/1900 12:00:00 AM' || item.handoveR_DATE == '01/01/1900 12:00:00 AM' || item.handoveR_DATE == '1/1/1900' ||
            //        item.handoveR_DATE == '2000-01-01' || item.handoveR_DATE == '01-01-2000' || item.handoveR_DATE == '01-Jan-2000' || item.handoveR_DATE == '1/1/2000 12:00:00 AM' || item.handoveR_DATE == '01/01/2000 12:00:00 AM' || item.handoveR_DATE == '1/1/2000' ||
            //        item.handoveR_DATE == '00-01-01' || item.handoveR_DATE == '01-01-00' || item.handoveR_DATE == '01-Jan-00' || item.handoveR_DATE == '1/1/00 12:00:00 AM' || item.handoveR_DATE == '01/01/00 12:00:00 AM' || item.handoveR_DATE == '1/1/00' || item.handoveR_DATE == '01-Jan-00 12:00:00 AM'
            //    ) {
            //        item.handoveR_DATE = null;
            //    }
            //});
        }

        //var maxSNoFromDB = 0;
        //if (dataSrc && dataSrc.length > 0) {
        //    maxSNoFromDB = Math.max.apply(Math, dataSrc.map(o => o.s_NO || 0));
        //}

        var col = [
            {
                dataField: "Action",
                width: 120,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY ? '' : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_MpoLayout.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT) ? '' : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_MpoLayout.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT ? '' : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_MpoLayout.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                   <a href="javascript:;" class="grid-action-icon" onclick="empr_MpoLayout.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                   <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_MpoLayout.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                   <a href="javascript:;" class="grid-action-icon" style="margin-left: 8px" onclick="empr_MpoLayout.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                   </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: "reV_STATUS",
                caption: "Rev Status",
                width: 140,
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (options.data.reV_STATUS === undefined || options.data.reV_STATUS === null) {
                        options.data.reV_STATUS = "N";
                    }
                    const currentValue = options.data.reV_STATUS;
                    const isDisabled = (options.data.dT_CODE == null);
                    const btnGroupHTML = `
                            <div class="btn-group nrcBtnCommon nrc_btns${options.rowIndex}" role="group">
                                <button type="button" class="btn btn-sm btn_n${options.rowIndex}" data-value="N" ${isDisabled ? 'disabled' : ''}>N</button>
                                <button type="button" class="btn btn-sm btn_r${options.rowIndex}" data-value="R" ${isDisabled ? 'disabled' : ''}>R</button>
                                <button type="button" class="btn btn-sm btn_c${options.rowIndex}" data-value="C" ${isDisabled ? 'disabled' : ''}>C</button>
                            </div>
                            `;
                    $(btnGroupHTML).appendTo(container);
                    const color = options.data.reV_COLOR === "green" ? "#51bb25" : options.data.reV_COLOR === "red" ? "#dc3545" : "#055a87";
                    if (currentValue === "N") {
                        $(container).find(".btn_n" + options.rowIndex).css({
                            "background-color": "#055a87",
                            "color": "white"
                        });
                    } else if (currentValue === "R") {
                        $(container).find(".btn_r" + options.rowIndex).css({
                            "background-color": color,
                            "color": "white"
                        });
                    } else if (currentValue === "C") {
                        $(container).find(".btn_c" + options.rowIndex).css({
                            "background-color": color,
                            "color": "white"
                        });
                    }

                    if (!isDisabled) {
                        $(container).find('button').on('click', function () {
                            const newValue = $(this).data('value');
                            options.data.reV_STATUS = newValue;
                            if (newValue === "N") empr_MpoLayout.handleN(options.data, options.rowIndex);
                            else if (newValue === "R") empr_MpoLayout.handleR(options.data, options.rowIndex);
                            else if (newValue === "C") empr_MpoLayout.handleC(options.data, options.rowIndex);
                        });
                    }
                }
            },
            {
                dataField: "reV_TOGGLE",
                caption: "Rev Allow",
                width: 100,
                allowEditing: false,
                alignment: "center",
                cellTemplate: function (container, options) {

                    let isChecked = options.data.reV_TOGGLE === true;

                    let checkbox = $("<input>")
                        .addClass("form-check-input")
                        .attr("type", "checkbox")
                        .attr("id", "nrc_toggle" + options.rowIndex)
                        .prop("checked", isChecked)
                        .prop("disabled", true);

                    $("<div>")
                        .addClass("form-check form-switch toggle-rev-status")
                        .append(checkbox)
                        .appendTo(container);
                }
            },
            {
                dataField: 'reV_COLOR',
                visible: false,
            },
            {
                dataField: 'reV_REF',
                visible: false,
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'carD_FILE_DATE',
                caption: 'CAD File',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.carD_FILE_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.carD_FILE_DATE = null;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear();
                        $dateCell.text(day + '-' + month + '-' + year);
                    }
                },
            },
            {
                dataField: 'caD_FILE_DOC',
                caption: 'CAD File Doc',
                width: 250,
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

                    if (options.data.caD_FILE_DOC && options.data.caD_FILE_DOC !== "") {
                        const onlyName = options.data.caD_FILE_DOC.split('/').pop();
                        txtFileName.val(onlyName);
                    }

                    const fileInput = $('<input>')
                        .attr({
                            type: 'file',
                            accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*',
                            id: 'gridDOC'
                        })
                        .css("display", "none");

                    const browseBtn = $('<button>')
                        .addClass('btn btn-primary')
                        .addClass('my-griddoc-browse')
                        .text('Browse')
                        .on('click', function () {
                            fileInput.val('');
                            //txtFileName.val('');
                            fileInput.click();
                        });

                    const eyeBtn = $('<a>')
                        .addClass('my-eye-btn')
                        .append($('<i>').addClass('fa fa-eye'))
                        .on('click', function () {
                            if (options.data.caD_FILE_DOC) {
                                window.open(options.data.caD_FILE_DOC, '_blank');
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
                            url: '/MpoLayout/SaveImage',
                            type: "POST",
                            data: formData,
                            processData: false,
                            contentType: false,
                            success: function (data) {

                                if (data.msgType == '1') {
                                    let grid = options.component;
                                    grid.cellValue(options.rowIndex, "caD_FILE_DOC", data.data);
                                    grid.refresh();
                                    txtFileName.val(file.name);
                                    //empr_helper.notify("File uploaded successfully.", 1);
                                }
                                else {
                                    txtFileName.val("No File");
                                    //empr_helper.notify("Something went wrong while saving the file.", 2);
                                }
                            },
                            error: function () {
                                empr_helper.notify("File upload failed.", 2);
                            }
                        });
                    });

                    wrapper.append(txtFileName, browseBtn, eyeBtn, fileInput);
                    $(container).append(wrapper);
                }
            },
            {
                dataField: 'laY_SUBM_DATE',
                caption: 'Layout Subm',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.laY_SUBM_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.laY_SUBM_DATE = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },
            {
                dataField: 'laY_SUBD_DATE',
                caption: 'Layout Subd',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.laY_SUBD_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.laY_SUBD_DATE = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },
            {
                dataField: 'laY_SUBD_DOC',
                caption: 'Layout Submitted Doc',
                width: 250,
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

                    if (options.data.laY_SUBD_DOC && options.data.laY_SUBD_DOC !== "") {
                        const onlyName = options.data.laY_SUBD_DOC.split('/').pop();
                        txtFileName.val(onlyName);
                    }

                    const fileInput = $('<input>')
                        .attr({
                            type: 'file',
                            accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*',
                            id: 'gridDOC'
                        })
                        .css("display", "none");

                    const browseBtn = $('<button>')
                        .addClass('btn btn-primary')
                        .addClass('my-griddoc-browse')
                        .text('Browse')
                        .on('click', function () {
                            fileInput.val('');
                            //txtFileName.val('');
                            fileInput.click();
                        });

                    const eyeBtn = $('<a>')
                        .addClass('my-eye-btn')
                        .append($('<i>').addClass('fa fa-eye'))
                        .on('click', function () {
                            if (options.data.laY_SUBD_DOC) {
                                window.open(options.data.laY_SUBD_DOC, '_blank');
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
                            url: '/MpoLayout/SaveImage',
                            type: "POST",
                            data: formData,
                            processData: false,
                            contentType: false,
                            success: function (data) {

                                if (data.msgType == '1') {
                                    let grid = options.component;
                                    grid.cellValue(options.rowIndex, "laY_SUBD_DOC", data.data);
                                    grid.refresh();
                                    txtFileName.val(file.name);
                                    //empr_helper.notify("File uploaded successfully.", 1);
                                }
                                else {
                                    txtFileName.val("No File");
                                    //empr_helper.notify("Something went wrong while saving the file.", 2);
                                }
                            },
                            error: function () {
                                empr_helper.notify("File upload failed.", 2);
                            }
                        });
                    });

                    wrapper.append(txtFileName, browseBtn, eyeBtn, fileInput);
                    $(container).append(wrapper);
                }
            },
            {
                dataField: 'laY_APPR_DATE',
                caption: 'Layout Appr',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.laY_APPR_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.laY_APPR_DATE = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },
            {
                dataField: 'stofF_SUBM_DATE',
                caption: 'StrikeOff Subm',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.stofF_SUBM_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.stofF_SUBM_DATE = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },
            {
                dataField: 'stofF_SUBD_DATE',
                caption: 'StrikeOff Subd',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.stofF_SUBD_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.stofF_SUBD_DATE = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },
            {
                dataField: 'stofF_SUBD_DOC',
                caption: 'StrikeOff Submitted Doc',
                width: 250,
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

                    if (options.data.stofF_SUBD_DOC && options.data.stofF_SUBD_DOC !== "") {
                        const onlyName = options.data.stofF_SUBD_DOC.split('/').pop();
                        txtFileName.val(onlyName);
                    }

                    const fileInput = $('<input>')
                        .attr({
                            type: 'file',
                            accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*',
                            id: 'gridDOC'
                        })
                        .css("display", "none");

                    const browseBtn = $('<button>')
                        .addClass('btn btn-primary')
                        .addClass('my-griddoc-browse')
                        .text('Browse')
                        .on('click', function () {
                            fileInput.val('');
                            //txtFileName.val('');
                            fileInput.click();
                        });

                    const eyeBtn = $('<a>')
                        .addClass('my-eye-btn')
                        .append($('<i>').addClass('fa fa-eye'))
                        .on('click', function () {
                            if (options.data.stofF_SUBD_DOC) {
                                window.open(options.data.stofF_SUBD_DOC, '_blank');
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
                            url: '/MpoLayout/SaveImage',
                            type: "POST",
                            data: formData,
                            processData: false,
                            contentType: false,
                            success: function (data) {
                                if (data.msgType == '1') {
                                    let grid = options.component;
                                    grid.cellValue(options.rowIndex, "stofF_SUBD_DOC", data.data);
                                    grid.refresh();
                                    txtFileName.val(file.name);
                                    //empr_helper.notify("File uploaded successfully.", 1);
                                }
                                else {
                                    txtFileName.val("No File");
                                    //empr_helper.notify("Something went wrong while saving the file.", 2);
                                }
                            },
                            error: function () {
                                empr_helper.notify("File upload failed.", 2);
                            }
                        });
                    });

                    wrapper.append(txtFileName, browseBtn, eyeBtn, fileInput);
                    $(container).append(wrapper);
                }
            },
            {
                dataField: 'stofF_APPR_DATE',
                caption: 'StrikeOff Appr',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                alignment: 'center',
                width: 130,
                setCellValue: function (newData, value) {
                    if (value) {
                        var date = new Date(value);
                        var year = date.getFullYear();
                        if (year < 100) year += 2000;
                        newData.stofF_APPR_DATE = new Date(year, date.getMonth(), date.getDate());
                    } else {
                        newData.stofF_APPR_DATE = value;
                    }
                },
                cellTemplate: function (container, options) {
                    var $dateCell = $('<div>').appendTo(container);
                    var dateValue = options.value;
                    if (dateValue) {
                        var date = new Date(dateValue);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear()
                        var formattedDate = day + '-' + month + '-' + year;
                        $dateCell.text(formattedDate);
                    }
                },
            },

































            //{
            //    dataField: 'shiP_DATE',
            //    caption: 'Ship Date',
            //    dataType: 'date',
            //    format: 'dd-MM-yyyy',
            //    alignment: 'center',
            //    width: 100,
            //    setCellValue: function (newData, value) {
            //        if (value) {
            //            var date = new Date(value);
            //            var year = date.getFullYear();
            //            if (year < 100) year += 2000;

            //            var shipDate = new Date(year, date.getMonth(), date.getDate());
            //            newData.shiP_DATE = shipDate;

            //            // ✅ Auto set Booking Date = 15 days before Ship Date
            //            var bookingDate = new Date(shipDate);
            //            bookingDate.setDate(shipDate.getDate() - 15);
            //            newData.bookinG_DATE = bookingDate;

            //        } else {
            //            newData.shiP_DATE = null;
            //            newData.bookinG_DATE = null;
            //        }
            //    },
            //    cellTemplate: function (container, options) {
            //        var $dateCell = $('<div>').appendTo(container);
            //        var dateValue = options.value;
            //        if (dateValue) {
            //            var date = new Date(dateValue);
            //            var day = ("0" + date.getDate()).slice(-2);
            //            var month = ("0" + (date.getMonth() + 1)).slice(-2);
            //            var year = date.getFullYear();
            //            $dateCell.text(day + '-' + month + '-' + year);
            //        }
            //    },
            //},
            //{
            //    dataField: 'port',
            //    caption: 'Port',
            //    lookup: {
            //        dataSource: Ports,
            //        displayExpr: 'value',
            //        valueExpr: 'key'
            //    },
            //    width: 100,
            //    alignment: 'center'
            //},
            //{
            //    dataField: 'seasoN_CODE',
            //    caption: 'Season',
            //    lookup: {
            //        dataSource: Season,
            //        displayExpr: 'value',
            //        valueExpr: 'key',
            //        allowClearing: true
            //    },
            //    width: 130,
            //    alignment: 'center',
            //},
            //{
            //    dataField: 'dT_DESC',
            //    caption: 'Comment',
            //},
            //{
            //    dataField: 'doc',
            //    caption: 'Document',
            //    width: 250,
            //    allowEditing: false,
            //    cellTemplate: function (container, options) {
            //        const inputGroup = $('<div>').addClass('input-group');

            //        const fileInput = $('<input>')
            //            .attr({
            //                type: 'file',
            //                accept: '.pdf, .doc, .docx, .xls, .xlsx, image/*'
            //            })
            //            .addClass('form-control')
            //            .css({ "display": "block" });

            //        const fileLabel = $('<span>')
            //            .css({ "margin-left": "10px", "font-size": "12px", "font-style": "italic" });

            //        fileInput.on('change', function (event) {
            //            const file = event.target.files[0];
            //            if (file) {
            //                fileLabel.text(file.name);

            //                let formData = new FormData();
            //                formData.append('model', file, file.name);

            //                $.ajax({
            //                    url: '/MpoLayout/SaveImage',
            //                    data: formData,
            //                    processData: false,
            //                    contentType: false,
            //                    type: "POST",
            //                    success: function (data) {
            //                        if (data.msgType == '1') {
            //                            let grid = options.component;
            //                            let rowIndex = options.rowIndex;
            //                            let dataSource = grid.option("dataSource");

            //                            dataSource[rowIndex].doc = data.data;
            //                            grid.repaint();
            //                        } else {
            //                            empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
            //                        }
            //                    },
            //                    error: function (error) {
            //                        empr_helper.notify("File upload failed. Please try again.", 2);
            //                    }
            //                });
            //            } else {
            //                fileLabel.text('');
            //            }
            //        });

            //        inputGroup.append(fileInput).append(fileLabel);

            //        if (options.data.doc != null && options.data.doc !== '') {
            //            const viewButton = $('<div>')
            //                .addClass('input-group-append')
            //                .append(
            //                    $('<a>')
            //                        .attr('href', 'javascript:;')
            //                        .addClass('input-group-text')
            //                        .on('click', function () {
            //                            const fileUrl = options.data.doc;
            //                            if (fileUrl) {
            //                                window.open(fileUrl, '_blank');
            //                            } else {
            //                                empr_helper.notify('No document available to view.', 2);
            //                            }
            //                        })
            //                        .append($('<i>').addClass('fa fa-eye'))
            //                );

            //            inputGroup.append(viewButton);
            //        }

            //        $(container).append(inputGroup);
            //    }
            //},
            
            //{
            //    dataField: 'picK_ID',
            //    caption: 'Pick Id',
            //    visible: false
            //},
            //{ dataField: "acT_CODE", visible: false },
            //{ dataField: "clienT_NAME", visible: false },
            //{ dataField: "deP_ID", visible: false },
            //{ dataField: "deP_NAME", visible: false },
            //{ dataField: "emP_ID", visible: false },
            //{ dataField: "ename", visible: false },
            //{ dataField: "iteM_NAME", visible: false },
            //{ dataField: "joB_NO", visible: false },
            //{ dataField: "partY_CODE", visible: false },
            //{ dataField: "ref", visible: false },
            //{ dataField: "remarks", visible: false },
            //{ dataField: "sacT_CODE", visible: false },
            //{ dataField: "spartY_CODE", visible: false },
            //{ dataField: "supplieR_NAME", visible: false },
            //{ dataField: "traN_ID", visible: false },
            //{ dataField: "uniT_NAME", visible: false },
            //{ dataField: "v_DATE", visible: false },
            //{ dataField: "voucheR_NO", visible: false }

        ];
        empr_helper.editableDxGridbindingForTransactions('#DetailContainer', col, dataSrc, "MerchantPurchaseOrderDetail", "ordeR_NO", '');
        if (dataSrc.length == 0) {
            $('#DetailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#DetailContainer').dxDataGrid('instance').saveEditData();
            });
        }

    },

    CloneRow: function (index) {
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_MpoLayout.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    clonedRowData.__KEY__ = empr_MpoLayout.GenerateKey(36);

                    clonedRowData.reV_STATUS = "N";
                    clonedRowData.reV_TOGGLE = false;

                    let newDataSource = [clonedRowData].concat(dataSource);
                    gridInstance.option("dataSource", newDataSource);
                    gridInstance.refresh();
                }
            });
        }
        else {
            empr_MpoLayout.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                let clonedRowData = $.extend(true, {}, dataSource[index]);
                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_MpoLayout.GenerateKey(36);

                clonedRowData.reV_STATUS = "N";
                clonedRowData.reV_TOGGLE = false;

                let newDataSource = [clonedRowData].concat(dataSource);
                gridInstance.option("dataSource", newDataSource);
                gridInstance.refresh();
            }
        }
    },
    AddRow: function () {

        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_MpoLayout.rowsCount += 1;
                const gridInstance = $('#DetailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");

                dataSource.unshift({ __KEY__: empr_MpoLayout.GenerateKey(36) });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_MpoLayout.rowsCount += 1;
            const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_MpoLayout.GenerateKey(36) });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },
    DeleteRow: function (index) {

        const gridInstance = $('#DetailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_MpoLayout.rowsCount -= 1;
                    gridInstance.saveEditData();
                }
                else {
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
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/MpoLayout/Delete", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_MpoLayout.rowsCount -= 1;
                                gridInstance.saveEditData();
                            }
                        }, false, true);
                    });
                }
            }
            else {
                empr_helper.notify("You are not allowed to delete the last row.", 2);
            }
        }
    },
    GenerateKey: function (keyLength) {

        var key = "";
        var characters = "abcdef0123456789";
        for (var i = 0; i < keyLength; i++) {
            if (i === 8 || i === 13 || i === 18 || i === 23) {
                key += "-";
            } else {
                key += characters.charAt(Math.floor(Math.random() * characters.length));
            }
        }
        return key;
    },
    //InitFinishItemsDDL: function (selectedValue) {
    //    $.ajax({
    //        url: "DailyProduction/GetFinishItems",
    //        type: "GET",
    //        success: function (response) {
    //            empr_MpoLayout.BindDxDDL("FinishItem", response.data, selectedValue, "key", "value", "Select", function (d) {
    //                $('#FinishItem_Hidden').val(d.value)
    //                if (d.value == null) {
    //                    $('#FinishItem_Hidden').val('');
    //                }
    //            });
    //        }
    //    });
    //},
    //InitSupplierDDL: function (dataSource, selectedValue) {
    //    $('#SUPPLIER').dxSelectBox({
    //        dataSource: {
    //            store: dataSource,
    //            paginate: true,
    //            pageSize: 50
    //        },
    //        paging: {
    //            enabled: true,
    //            pageSize: 50,
    //        },
    //        displayExpr: 'value',
    //        valueExpr: 'customizedKey',
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
    //        disabled: true
    //    });
    //},
    BindDxDDL: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },
    InitQuickSearchGrid: function () {
        empr_MpoLayout.GetQuickSearchData();
    },
    GetQuickSearchData: function () {
        ajaxHelper.ajaxGetJson('/MpoLayout/QuickSearch', function (data) {
            if (data.msgType == 1) {
                empr_MpoLayout.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },
    CreateQuickSearchGrid: function (dataSrc) {
        console.log('Quick', dataSrc);
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                if (Permissions != "Admin" && !Permissions.r_PRINT) {
                    $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                } else {
                    $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                           <a href="javascript:;"  class="grid-action-icon elm_copy" style="margin-left: 8px" reportdate=${options.data.v_DATE} reportid=${options.data.traN_ID} title="COPY"><i class="fa fa-copy"></i></a>
                           </div>`).appendTo(container);
                }
            }
        },
        //<a href="javascript:;"  class="grid-action-icon elm_print" style="margin-left: 8px" reportid=${options.data.traN_ID} title="PRINT"><i class="fa fa-print"></i></a>
        { dataField: 'v_DATE', caption: 'Tran Date', dataType: 'date', width: 100, alignment: 'center' },
        { dataField: 'astatus', caption: 'Status', width: 100, alignment: 'center' },
        { dataField: 'voucheR_NO', caption: 'Transaction#', alignment: 'center', width: 150 },
        { dataField: 'clienT_PO', caption: 'Model# / PO#', alignment: 'center', width: 150 },
        { dataField: 'joB_NO', caption: 'Job No#', alignment: 'center', width: 100 },
        { dataField: 'remarks', caption: 'Remarks', alignment: 'center' },


        //{ dataField: 'carD_FILE_DATE', caption: 'Card File', dataType: 'date', width: 120, alignment: 'center' },
        //{ dataField: 'laY_SUBM_DATE', caption: 'Layout Subm', dataType: 'date', width: 120, alignment: 'center' },
        //{ dataField: 'laY_SUBD_DATE', caption: 'Layout Subd', dataType: 'date', width: 120, alignment: 'center' },
        //{ dataField: 'laY_APPR_DATE', caption: 'Layout Appr', dataType: 'date', width: 120, alignment: 'center' },
        //{ dataField: 'stofF_SUBM_DATE', caption: 'StrikeOff Subm', dataType: 'date', width: 120, alignment: 'center' },
        //{ dataField: 'stofF_SUBD_DATE', caption: 'StrikeOff Subd', dataType: 'date', width: 120, alignment: 'center' },
        //{ dataField: 'stofF_APPR_DATE', caption: 'StrikeOff Appr', dataType: 'date', width: 120, alignment: 'center' },


        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "DailyProductionQS");
    },
    GetDataToSave: function () {

        var CODE = $("#Code").val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        //var REF = $("#REF").val();
        var REMARKS = $("#REMARKS").val();
        var JOB_NO = $("#key_hidden").val();
        //var SPARTY_CODE = $('#SUPPLIER').dxSelectBox('option', 'value');
        //var FABRIC = $('#FABRIC').dxSelectBox('option', 'value');
        //var GSM = $('#GSM').dxSelectBox('option', 'value');
        //var COMM = $("#COMM").val();
        //var COMM_AMT = $("#COMM_AMT").val();
        //var COMM_VAL = $("#COMM_VAL").val();

        var masterRecord = {
            TRAN_ID: CODE,
            ASTATUS: ASTATUS,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            REMARKS: REMARKS,
            //REF: REF,
            //FABRIC: FABRIC,
            //GSM: GSM,
            //COMM: COMM,
            //COMM_AMT: COMM_AMT,
            //COMM_VAL: COMM_VAL,
            //SPARTY_CODE: SPARTY_CODE,
            JOB_NO: JOB_NO,
            //PICK_ID: empr_MpoLayout.pickId,
        }

        // validate data
        var detailRecords = [];
        debugger;
        if ($('#DetailContainer').dxDataGrid('instance').hasEditData()) {
            $('#DetailContainer').dxDataGrid('instance').saveEditData().done(function () {
                detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
            });
        }
        else {
            detailRecords = $('#DetailContainer').dxDataGrid('instance').option("dataSource");
        }

        $.each(detailRecords, function (index, item) {
            if (!(item.carD_FILE_DATE == "" || item.carD_FILE_DATE == null || item.carD_FILE_DATE == undefined)) {
                item.carD_FILE_DATE = empr_helper.PrepareDate(item.carD_FILE_DATE);
            }

            if (!(item.laY_SUBM_DATE == "" || item.laY_SUBM_DATE == null || item.laY_SUBM_DATE == undefined)) {
                item.laY_SUBM_DATE = empr_helper.PrepareDate(item.laY_SUBM_DATE);
            }

            if (!(item.laY_SUBD_DATE == "" || item.laY_SUBD_DATE == null || item.laY_SUBD_DATE == undefined)) {
                item.laY_SUBD_DATE = empr_helper.PrepareDate(item.laY_SUBD_DATE);
            }

            if (!(item.laY_APPR_DATE == "" || item.laY_APPR_DATE == null || item.laY_APPR_DATE == undefined)) {
                item.laY_APPR_DATE = empr_helper.PrepareDate(item.laY_APPR_DATE);
            }

            if (!(item.stofF_SUBM_DATE == "" || item.stofF_SUBM_DATE == null || item.stofF_SUBM_DATE == undefined)) {
                item.stofF_SUBM_DATE = empr_helper.PrepareDate(item.stofF_SUBM_DATE);
            }

            if (!(item.stofF_SUBD_DATE == "" || item.stofF_SUBD_DATE == null || item.stofF_SUBD_DATE == undefined)) {
                item.stofF_SUBD_DATE = empr_helper.PrepareDate(item.stofF_SUBD_DATE);
            } 

            if (!(item.stofF_APPR_DATE == "" || item.stofF_APPR_DATE == null || item.stofF_APPR_DATE == undefined)) {
                item.stofF_APPR_DATE = empr_helper.PrepareDate(item.stofF_APPR_DATE);
            }

        });

        if (empr_MpoLayout.rowsCount == detailRecords.length) {
            var modelRecord = {
                Master: masterRecord,
                Detail: detailRecords
            };
            return modelRecord;
        }
        else {
            var modelRecord = {
                Master: masterRecord,
                Detail: $('#DetailContainer').dxDataGrid('instance').option("dataSource")
            };
            return modelRecord;
        }
    },
    ValidateInfo: function () {
        //debugger;
        var valid = true;
        var data = empr_MpoLayout.GetDataToSave();

        data.Detail = $('#DetailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add Record.", 2);
            valid = false;
            return valid;
        }
        //$.each(data.Detail, function (index, item) {
        //    debugger;
        //    if (item.ordeR_NO == "" || item.ordeR_NO == null || item.ordeR_NO == undefined) {
        //        empr_helper.notify("Please select Order at Line no " + (index + 1), 2);
        //        valid = false;
        //        return valid;
        //    }

        //    if (item.reV_STATUS == "R") {
        //        if (item.reV_REF == "" || item.reV_REF == null || item.reV_REF == undefined || item.reV_REF <= 0) {
        //            empr_helper.notify("You must need to select S-NO# of Revised entry at Line no " + (index + 1), 2);
        //            valid = false;
        //            return valid;
        //        }
        //    }

        //    if (item.qty == "" || item.qty == null || item.qty == undefined) {
        //        empr_helper.notify("Please enter quantity at Line no " + (index + 1), 2);
        //        valid = false;
        //        return valid;
        //    }

        //    if (item.qty <= 0) {
        //        empr_helper.notify("Please enter correct item quantity at Line no " + (index + 1), 2);
        //        valid = false;
        //        return valid;
        //    }

        //    if (item.picK_ID <= 0 || item.picK_ID == "" || item.picK_ID == null || item.picK_ID == undefined) {
        //        empr_helper.notify("You Cannot add record without pick", 2);
        //        valid = false;
        //        return valid;
        //    }

        //    if (item.doc == "" || item.doc == null || item.doc == undefined) {
        //        empr_helper.notify("Please select Document Line no " + (index + 1), 2);
        //        valid = false;
        //        return valid;
        //    }
        //});

        return valid;
    },
    SaveInfo: function () {
        //debugger;
        var dataModel = empr_MpoLayout.GetDataToSave();

        //var totalQty = (dataModel.Detail || []).reduce(function (sum, item) {
        //    return sum + (parseFloat(item.qty) || 0);
        //}, 0);

        //var totalQty = (dataModel.Detail || []).reduce(function (sum, item) {
        //    if (item.reV_STATUS !== "C") {
        //        return sum + (parseFloat(item.qty) || 0);
        //    } else {
        //        return sum;
        //    }
        //}, 0);


        //if (totalQty != empr_MpoLayout.PickQty) {
        //    empr_helper.notify(`Quantity does not match. It should be equal to ${empr_MpoLayout.PickQty}`, 2);
        //    valid = false;
        //    return valid;
        //}

        console.log('SaveInfo', dataModel.Detail);
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }

        ajaxHelper.ajaxPostJsonData(dataModel, "/MpoLayout/Save", function (data) {
            console.log('Save', data);
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_helper.selectedBill = data.data.code;
                if (dataModel.Master.TRAN_ID == 0
                    || dataModel.Master.TRAN_ID == null
                    || dataModel.Master.TRAN_ID == undefined) {
                    $('#Code').val(data.data.code);
                    $('#VOUCHER_NO').val(data.data.voucherNo);
                }
                empr_MpoLayout.GetMpoLayoutByCode(data.data.code);
                $('#BtnDelete').show();
            }
        }, false, true);
    },
    GetMpoLayoutByCode: function (code) {
        ajaxHelper.ajaxGetJson('/MpoLayout/GetMpoLayoutByCode?code=' + code, function (data) {
            console.log('edit', data);
            if (data.master.msgType == 1) {

                var masterData = data.master.data;
                var detailData = data.detail.data;

                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#Code').val(response.traN_ID);
                    empr_helper.selectedBill = response.traN_ID;
                    $('#ASTATUS').dxSelectBox('instance').option('value', response.astatus);
                    empr_MpoLayout.InitPOJobGridDDL(response.joB_NO);
                    //$('#JOB_NO').val(response.joB_NO);
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);

                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                        if (Permissions.r_EDIT) {
                            $('#BtnSave').show();
                        }
                        else {
                            $('#BtnSave').hide();
                        }
                    } else {
                        $('#BtnSave').show();
                        $('#BtnDelete').show();
                    }
                }

                if (data.detail.msgType == 1) {
                    empr_MpoLayout.CreateGrid(detailData);
                    $('.card-body').addClass('customHighlightForModifiedCells');
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
                $('#BtnPrint').show();
            }
            else {
                empr_helper.notify(data.master.msg, data.master.msgType);
            }
        }, false, true);
    },
    //GetDailyProductionDetailsByCode: function (code) {
    //    ajaxHelper.ajaxGetJson('/MpoLayout/GetDailyProductionDetailByCode?code=' + code, function (data) {
    //        if (data.msgType == 1) {
    //            empr_MpoLayout.CreateGrid(data.data);
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},
    Delete: function () {

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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/MpoLayout/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_MpoLayout.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },
    //InitBatchPickGrid: function () {
    //    var jobId = $("#key_hidden").val();
    //    if (jobId == "" || jobId == null || jobId == undefined) {
    //        empr_helper.notify("Please select Job# first.", 2);
    //    }
    //    else {
    //        empr_MpoLayout.GetBatchDetailByProcess(jobId);
    //    }
    //},
    //GetBatchDetailByProcess: function (clientPO) {
    //    console.log('jobId', clientPO);
    //    ajaxHelper.ajaxGetJson('/MpoLayout/GetBatchDetailByProcess?process=' + clientPO, function (data) {
    //        if (data.msgType == 1) {
    //            if (data.data.length > 0) {
    //                if ($('#SodaPickGridContainer').data('dxDataGrid') != undefined) {
    //                    $('#SodaPickGridContainer').data('dxDataGrid').dispose();
    //                }
    //                empr_MpoLayout.CreatePickGrid(data.data);
    //                $('#SodaPickModal').modal('show');

    //            } else {
    //                empr_helper.notify("No Purchase Order found for this Client PO.", 2);
    //            }
    //        }
    //        else {
    //            empr_helper.notify(data.msg, data.msgType);
    //        }
    //    }, false, true);
    //},
    //CreatePickGrid: function (dataSrc) {
    //    var col = [
    //        { dataField: 'v_DATE', caption: 'Date', dataType: 'date', allowEditing: false, format: 'dd-MM-yyy' }, // pick grid
    //        { dataField: 'voucheR_NO', caption: 'Voucher', allowEditing: false, },
    //        { dataField: 'iteM_NAME', caption: 'Item', allowEditing: false, width: 150, },
    //        { dataField: 'clienT_NAME', caption: 'Client', allowEditing: false, },
    //        { dataField: 'ref', caption: 'Ref', allowEditing: false, },
    //        { dataField: 'joB_NO', caption: 'Job #', allowEditing: false, },
    //        { dataField: 'ename', caption: 'Employee', allowEditing: false, },
    //        { dataField: 'deP_NAME', caption: 'Department', allowEditing: false, },
    //        { dataField: 'supplieR_NAME', caption: 'Supplier', allowEditing: false, width: 150, },
    //        { dataField: 'qty', caption: 'Quantity', allowEditing: false, },
    //        { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false, },
    //        { dataField: 'rate', caption: 'Rate', allowEditing: false, },
    //        { dataField: 'amt', caption: 'Amount', allowEditing: false, },
    //        { dataField: 'remarks', caption: 'Remarks', allowEditing: false, },
    //        { dataField: 'comM_TYPE', allowEditing: false, visible: false, },
    //        { dataField: 'comM_RATE', allowEditing: false, visible: false, },
    //        { dataField: 'comM_AMT', allowEditing: false, visible: false, },
    //        { dataField: 'status', allowEditing: false, visible: false, },
    //    ];
    //    empr_helper.dxGridbindingVouchers('#SodaPickGridContainer', col, dataSrc, "PurchaseBillPick", "single");
    //    setTimeout(function () {
    //        $('#SodaPickGridContainer').dxDataGrid('instance').resize();
    //    }, 500);
    //},
    //CreateBatchGrid: function (dataSrc) {
    //    var col = [
    //        { dataField: 'traN_ID', caption: 'Code', visible: false, },
    //        { dataField: 'process', caption: 'Process', allowEditing: false, },
    //        { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, },
    //        { dataField: 'batch', caption: 'Batch', allowEditing: false, },
    //        { dataField: 'uniT_NAME', caption: 'Unit', allowEditing: false },
    //        { dataField: 'iteM_CODE', caption: 'Item Code', visible: false, },
    //        { dataField: 'qty', caption: 'Batch Qty', allowEditing: false, },
    //        { dataField: 'baL_QTY', caption: 'B.Qty', allowEditing: false, visible: false },
    //        { dataField: 'unit', caption: 'Unit', allowEditing: false, visible: false },
    //        { dataField: 'mfG_DATE', caption: 'MFG Date', dataType: 'date', allowEditing: false, format: 'MM-yyy' },
    //        { dataField: 'exP_DATE', caption: 'EXP Date', dataType: 'date', allowEditing: false, format: 'MM-yyy' },
    //    ];
    //    empr_helper.editableDxGridbindingForTransactionsVouchers('#SodaPickGridContainer', col, dataSrc, "DailyProductionPick", "v_DATE", 'multiple');
    //    setTimeout(function () {
    //        $('#SodaPickGridContainer').dxDataGrid('instance').resize();
    //    }, 500);
    //},
    //AddBatchToDailyProduction: function () {
    //    if ($('#SodaPickGridContainer').dxDataGrid('instance').hasEditData()) {
    //        $('#SodaPickGridContainer').dxDataGrid('instance').saveEditData().done(function () {
    //            var data = empr_MpoLayout.GetDataToSave();
    //            var IsDataAvailableInGrid = false;
    //            $.each(data.Detail, function (index, item) {
    //                if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
    //                    IsDataAvailableInGrid = true;
    //                }
    //            });

    //            if (IsDataAvailableInGrid) {
    //                var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
    //                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //                var finalData = existingData.concat(selectedSodas);
    //                $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
    //            }
    //            else {
    //                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //                $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
    //            }
    //            $('.modal').hide();
    //            $('#V_DATE').focus();
    //        });
    //    }
    //    else {
    //        var data = empr_MpoLayout.GetDataToSave();
    //        var IsDataAvailableInGrid = false;
    //        $.each(data.Detail, function (index, item) {
    //            if (item.iteM_CODE != "" && item.iteM_CODE != null && item.iteM_CODE != undefined) {
    //                IsDataAvailableInGrid = true;
    //            }
    //        });

    //        if (IsDataAvailableInGrid) {
    //            var existingData = $('#DetailContainer').dxDataGrid('instance').option('dataSource');
    //            var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //            var finalData = existingData.concat(selectedSodas);
    //            $('#DetailContainer').dxDataGrid('instance').option('dataSource', finalData);
    //        }
    //        else {
    //            var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    //            $('#DetailContainer').dxDataGrid('instance').option('dataSource', selectedSodas);
    //        }

    //        $('.modal').hide();
    //        $('#V_DATE').focus();
    //    }
    //},
    //SetData: function (dataSource) {
    //    $.each(dataSource, function (index, item) {
    //        item.chK1 = false;
    //        item.chk = "0";
    //        if (item.unit != '') {
    //            var selectedUnit = Units.filter(u => u.key == item.unit);
    //            if (selectedUnit.length > 0) {
    //                item.qtY2 = selectedUnit[0].qty;
    //            }
    //        }
    //        else {
    //            item.qtY2 = 0;
    //        }

    //        var qty = parseFloat(item.qty) || 0;
    //        var qtY2 = parseFloat(item.qtY2) || 1;
    //        var rate = parseFloat(item.rate) || 0;
    //        var rT_TYPE = parseFloat(item.rT_TYPE) || 0;
    //        if (!isNaN(qty) && !isNaN(qtY2)) {
    //            if (item.chK1) {
    //                item.baL_QTY = qty * qtY2;
    //                if (empr_MpoLayout.Branch_RT_TYPE == 'Y') {
    //                    item.amt = item.baL_QTY * rate;
    //                    var perRate = parseFloat(rate / rT_TYPE) || 0;
    //                    if (perRate > -1 && perRate != 'Infinity') {
    //                        item.amt = (item.baL_QTY * perRate).toFixed(2);
    //                    }
    //                }

    //                if (empr_MpoLayout.Branch_RT_TYPE == 'N') {
    //                    item.amt = qty * rate;
    //                }
    //                item.neT_AMT = item.amt;
    //            }
    //            else {
    //                item.baL_QTY = qty;
    //                if (empr_MpoLayout.Branch_RT_TYPE == 'Y') {
    //                    var perRate = parseFloat(rate / rT_TYPE) || 0;
    //                    if (perRate > -1 && perRate != 'Infinity') {
    //                        item.amt = (item.baL_QTY * perRate).toFixed(2);
    //                    }
    //                }

    //                if (empr_MpoLayout.Branch_RT_TYPE == 'N') {
    //                    item.amt = qty * rate;
    //                }
    //                item.neT_AMT = item.amt;
    //            }
    //        }

    //        item.__KEY__ = empr_MpoLayout.GenerateKey(36);
    //    });

    //    return dataSource;
    //},

    //InitFabricDDL: function (selectedValue) {

    //    $('#FABRIC').dxSelectBox({
    //        dataSource: Fabric,
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

    //InitGSMDDL: function (selectedValue) {

    //    $('#GSM').dxSelectBox({
    //        dataSource: GSMData,
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

    InitPOJobGridDDL: function (_selectedValue) {
        console.log(_selectedValue);

        ajaxHelper.ajaxGetJson('/MpoLayout/POJobsDropdown', function (data) {
            console.log('InitPOJobGridDDL', data);
            if (data.msgType == 1) {
                var Datasource = data.data;
                console.log('Datasource', Datasource);
                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.key == _selectedValue }) || [];
                    if (selectedObject.length > 0) {
                        selectedValue = selectedObject[0].key;

                        $('#key_hidden').val(selectedObject[0].key);
                        $('#CLIENT_PO').val(selectedObject[0].clientPo);
                        $('#PARTY_CODE').val(selectedObject[0].partyName);
                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#JOB_NO").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "key",
                    displayExpr: "value",
                    placeholder: "Select a value...",
                    dataSource: Datasource,
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.key === e.value);
                            if (selectedData) {
                                $('#key_hidden').val(selectedData.key);
                                $('#CLIENT_PO').val(selectedData.clientPo);
                                $('#PARTY_CODE').val(selectedData.partyName);
                            }
                        } else {
                            $('#key_hidden').val('');
                            $('#CLIENT_PO').val('');
                            $('#PARTY_CODE').val('');
                        }
                    },
                    onOpened: function (e) {
                        if (!currentSearchTerm) {
                            if (gridInstance) {
                                gridInstance.getDataSource().filter(null);
                                gridInstance.refresh();
                            }
                        }

                        setTimeout(() => {
                            const input = e.component._$element.find(".dx-texteditor-input").first();
                            input.focus();
                            if (input.val()) {
                                //input.select();
                            }
                        }, 50);
                    },
                    onInput: function (e) {
                        currentSearchTerm = e.event.target.value;

                        if (!e.component.option("opened")) {
                            isProgrammaticOpen = true;
                            e.component.open();
                            setTimeout(() => { isProgrammaticOpen = false; }, 100);
                        }

                        if (gridInstance) {
                            applyGridFilter(gridInstance, currentSearchTerm);
                        }
                    },
                    contentTemplate: function (e) {
                        gridInstance = $("<div>").dxDataGrid({
                            dataSource: new DevExpress.data.DataSource({
                                store: Datasource,
                                key: "key"
                            }),
                            columns: [

                                {
                                    dataField: "value",
                                    caption: "Job No",
                                    width: 200,
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "clientPo",
                                    caption: "Model# / PO#",
                                    width: 200,
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "partyName",
                                    caption: "Client",
                                    width: 370,
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                }
                            ],
                            selection: {
                                mode: "single",
                                showCheckBoxesMode: "always"
                            },
                            hoverStateEnabled: true,
                            height: 300,
                            keyboardNavigation: {
                                enabled: true,
                                enterKeyAction: "select",
                                editOnKeyPress: true
                            },
                            onSelectionChanged: function (selectedItems) {
                                const selected = selectedItems.selectedRowsData[0];
                                if (selected) {
                                    e.component.option("value", selected.key);

                                    e.component.close();

                                    $('#key_hidden').val(selected.key);
                                    $('#CLIENT_PO').val(selected.clientPo);
                                    $('#PARTY_CODE').val(selected.partyName);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].key], false);
                                    }
                                }
                            }
                        }).dxDataGrid("instance");

                        gridInstance.element().on('click', function (event) {
                            event.stopPropagation();
                        });

                        return gridInstance.element();
                    }
                });

                function applyGridFilter(grid, searchTerm) {
                    const dataSource = grid.getDataSource();
                    if (searchTerm) {
                        dataSource.filter([
                            ["value", "contains", searchTerm],
                            "or",
                            ["clientPo", "contains", searchTerm],
                            "or",
                            ["partyName", "contains", searchTerm]
                        ]);
                    } else {
                        dataSource.filter(null);
                    }
                    dataSource.load();
                }

                function highlightText(container, value) {
                    if (!value) return;

                    const text = value.toString();
                    if (!currentSearchTerm || !text.toLowerCase().includes(currentSearchTerm.toLowerCase())) {
                        container.text(text);
                        return;
                    }

                    const regex = new RegExp(currentSearchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), "gi");
                    const highlighted = text.replace(regex, match =>
                        `<span style="background-color: #ffeb3b; font-weight: bold;">${match}</span>`
                    );
                    container.html(highlighted);
                }

                $(document).on("dxclick", "#JOB_NO .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#ACT_PARENT_CODE_hidden').val('');
                    $('#displayExprAccParent').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });

            }
            else {
                empr_helper.notify('error', 2);
            }
        }, false, true);

    },

    handleN: function (rowData, rowIndex) {
        console.log("N clicked", rowData);

        // Get the row element
        const rowElement = $("#gridContainer").dxDataGrid("instance")
            .getRowElement(rowData);

        // Remove background from all buttons in this row
        $(document).find(".nrc_btns" + rowIndex + " button").css({
            "background-color": "",
            "color": ""
        });
        
        // Set background for clicked button
        $(document).find(".btn_n" + rowIndex).css({
            "background-color": "#055a87",
            "color": "white",
        });

        $(document).find("#nrc_toggle" + rowIndex).prop("checked", false);

        rowData.reV_TOGGLE = false;
    },

    handleR: function (rowData, rowIndex) {

        swal({
            title: 'Are you sure you want to Revised this record?',
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
            $(document).find(".nrc_btns" + rowIndex + " button").css({
                "background-color": "",
                "color": ""
            });

            $(document).find(".btn_r" + rowIndex).css({
                "background-color": "#51bb25",
                "color": "white",
            });

            $(document).find("#nrc_toggle" + rowIndex).prop("checked", true)

            rowData.reV_TOGGLE = true;
            rowData.reV_COLOR = "green";


            //const gridInstance = $('#DetailContainer').dxDataGrid('instance');
            //gridInstance.refresh();
        });
    },

    handleC: function (rowData, rowIndex) {
        swal({
            title: 'Are you sure you want to Cancle this record?',
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
            $(document).find(".nrc_btns" + rowIndex + " button").css({
                "background-color": "",
                "color": ""
            });

            $(document).find(".btn_c" + rowIndex).css({
                "background-color": "##dc3545",
                "color": "white",
            });

            $(document).find("#nrc_toggle" + rowIndex).prop("checked", true)

            rowData.reV_TOGGLE = true;
            rowData.reV_COLOR = "red";
        }); 
    },

}