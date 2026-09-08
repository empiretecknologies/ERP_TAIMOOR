var empr_Employee = {
    croppieInstance: null,

    initEvents: function () {

        $(document).ready(function () {
            empr_Employee.toggleResetButton();

            $('#btnClearImage').click(function () {

                if (empr_Employee.croppieInstance) {
                    empr_Employee.croppieInstance.croppie('destroy');
                    empr_Employee.croppieInstance = null;
                    $("#EMP_IMG").val('');
                }

                $('#croppieContainer').hide().empty();
                $('#empProfileImg').hide().attr('src', '');
                $('#btnTransImg').hide();
                $('#btnTakeCapture').show();
                $('#btnPickImage').show();
                $('#cameraPreview').show();

                empr_Employee.startCamera();

                $('#btnClearImage').hide();
            });


            //$('#btnResetImage').click(function () {
            //    empr_Employee.resetImageProcess();
            //});

            $('#btnPickImage').click(function () {
                empr_Employee.pickImage();
            });

            $('#empImgUpload').change(function () {
                empr_Employee.onImagePicked(this);
            });

            $('#btnCaptureImage').click(function () {
                empr_Employee.startCamera();
            });

            $('#btnTakeCapture').click(function () {
                
                $('#btnPickImage').hide();
                empr_Employee.captureImage();
            });

            $('#btnOpenUploadModal').click(function () {
                $('#uploadImageModal').modal('show');
                empr_Employee.resetImageProcess();
                if (empr_Employee.croppieInstance != null) {
                    $('#btnPickImage').hide();
                    $('#btnTakeCapture').hide();
                } else {
                    $('#btnPickImage').show();
                    $('#btnTakeCapture').show();
                    $('#btnClearImage').hide();
                }
                

                
                empr_Employee.startCamera();
            });

            $('body').on('click', '#btnTransImg', function () {
                if (empr_Employee.croppieInstance) {
                    empr_Employee.croppieInstance.croppie('result', {
                        type: 'base64',
                        size: 'viewport',
                        format: 'png',
                        quality: 1
                    }).then(function (croppedImage) {
                        $('#empProfileImgFinal').attr('src', croppedImage).show();
                        empr_Employee.SaveImage();

                        $('#empCroppedImage').val(croppedImage);

                        empr_Employee.toggleResetButton();
                        $('#uploadImageModal').modal('hide');

                        empr_Employee.croppieInstance.croppie('destroy');
                        empr_Employee.croppieInstance = null;
                        $('#croppieContainer').hide();

                        $('#empProfileImg').hide();
                        $('#cameraPreview').hide();
                        $('#btnTakeCapture').hide();
                        $('#btnTransImg').hide();
                    });
                } else {
                    const imgData = $('#empProfileImg').attr('src');
                    if (imgData) {
                        $('#empProfileImgFinal').attr('src', imgData).show();
                        empr_Employee.SaveImage();
                        $('#empCroppedImage').val(imgData);
                        $('#uploadImageModal').modal('hide');
                    }
                }
            });


            console.log('EmpDataByOfferLetter', EmpDataByOfferLetter);
            empr_Employee.resetForm();

            $('.saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Employee.validateMainInfo()) {
                            empr_Employee.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Employee.validateMainInfo()) {
                        empr_Employee.saveAttempt();
                    }
                }
            });

            $('#DocumentFile').change(function () {
                empr_Employee.uploadFile();
            })

            $('#CnicFile').change(function () {
                empr_Employee.uploadCNIC();
            })

            $('.delete').click(function () {
                var partytypecode = $('#Code').val();
                empr_Employee.delete(partytypecode);
            });

            $('body').on('click', '#quicksearch', function () {
                empr_Employee.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var rportid = $(this).attr("rportid")
                empr_Employee.GetEmployeeByEmployeeCode(rportid);
            });

            $('body').on('click', '#resetForm', function () {
                debugger;
                empr_Employee.resetForm();
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#resetForm').hide();
                !Permissions.r_VIEW && $('#quicksearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('.saveAttempt').hide();
            }
        });

    },

    toggleResetButton: function () {
        const imgSrc = $('#empProfileImgFinal').attr('src');
        if (imgSrc && imgSrc.includes('/Client/Company/default.jpg')) {
            $('#btnResetImage').hide();
        } else {
            $('#btnResetImage').show();
        }
    },

    resetImageProcess: function () {
        empr_Employee.stopCamera();

        if (empr_Employee.croppieInstance) {
            empr_Employee.croppieInstance.croppie('destroy');
            empr_Employee.croppieInstance = null;
            $("#EMP_IMG").val('');
        }

        $('#empProfileImgFinal').attr('src', '/Client/Company/default.jpg').show();
        $('#empProfileImg').attr('src', '').hide();
        $('#cameraPreview').hide();

        $('#croppieContainer').hide().empty();

        $('#btnTakeCapture').hide();
        $('#btnTransImg').hide();

        $('#empImgUpload').val('');

        empr_Employee.toggleResetButton();
    },

    initCroppie: function (imageSrc) {
        if (empr_Employee.croppieInstance) {
            empr_Employee.croppieInstance.croppie('destroy');
        }

        $('#croppieContainer').show();
        $('#empProfileImg').hide();

        const $finalImg = $('#empProfileImgFinal');
        const finalWidth = $finalImg.width() || 200;
        const finalHeight = $finalImg.height() || 200;

        empr_Employee.croppieInstance = $('#croppieContainer').croppie({
            viewport: { width: finalWidth, height: finalHeight, type: 'square' },
            boundary: { width: finalWidth + 100, height: finalHeight + 100 },
            enableOrientation: true
        });

        empr_Employee.croppieInstance.croppie('bind', {
            url: imageSrc
        });

        $('#btnTransImg').show();
    },

    pickImage: function () {
        $('#empImgUpload').click();
    },

    onImagePicked: function (input) {
        const file = input.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                $('#cameraPreview').hide();
                $('#btnTakeCapture').hide();
                $('#btnPickImage').hide();
                $('#btnClearImage').show();
                empr_Employee.stopCamera();
                empr_Employee.initCroppie(e.target.result);
            };
            reader.readAsDataURL(file);
        }
    },

    startCamera: function () {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(s => {
                empr_Employee.stream = s;
                $('#cameraPreview').show().get(0).srcObject = s;
                $('#empProfileImg').hide();
            })
            .catch(err => {
                alert("Camera not accessible: " + err);
            });
    },

    captureImage: function () {
        const video = $('#cameraPreview').get(0);
        const canvas = document.createElement('canvas');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        //canvas.getContext('2d').drawImage(video, 0, 0);
        const ctx = canvas.getContext('2d');

        ctx.translate(canvas.width, 0);
        ctx.scale(-1, 1);
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);


        const imgData = canvas.toDataURL("image/png");

        empr_Employee.stopCamera();

        $('#cameraPreview').hide();
        $('#btnTakeCapture').hide();
        $('#btnClearImage').show();

        empr_Employee.initCroppie(imgData);

    },

    stopCamera: function () {
        if (empr_Employee.stream) {
            empr_Employee.stream.getTracks().forEach(track => track.stop());
        }
    },

    validURl: function (url) {
        var pattern = /^[a-zA-Z0-9.-]+\.(com)$/i;
        return pattern.test(url);
    },

    validateMainInfo: function () {

        var valid = true;
        var data = empr_Employee.getDataToSave();

        if (data.EMP_ID.trim() == '') {
            empr_helper.notify("Employee ID is required.", 2);
            valid = false;
        }

        if (data.MACHINE_CODE.trim() == '') {
            empr_helper.notify("Machine Code is required.", 2);
            valid = false;
        }

        if (data.ENAME == '') {
            empr_helper.notify("Employee Name is required.", 2);
            valid = false;
        }

        if (data.FATHER_NAME == '') {
            empr_helper.notify("Father Name is required.", 2);
            valid = false;
        }

        if (data.DEP_ID == '' || data.DEP_ID == null) {
            empr_helper.notify("Department is required.", 2);
            valid = false;
        }

        if (data.DESIG == '' || data.DESIG == null) {
            empr_helper.notify("Designation is required.", 2);
            valid = false;
        }

        if (data.GENDER == '' || data.GENDER == null) {
            empr_helper.notify("Gender is required.", 2);
            valid = false;
        }

        if (data.BCODE == '' || data.BCODE == null) {
            empr_helper.notify("Branch is required.", 2);
            valid = false;
        }

        if (data.SHIFT_T == '' || data.SHIFT_T == null) {
            empr_helper.notify("Shift is required.", 2);
            valid = false;
        }

        if (data.EMP_TYPE == '' || data.EMP_TYPE == null) {
            empr_helper.notify("Type is required.", 2);
            valid = false;
        }

        if (data.CELL_NO == '' || data.CELL_NO == null) {
            empr_helper.notify("Cell No. is required.", 2);
            valid = false;
        }

        if (data.REG == '' || data.REG == null) {
            empr_helper.notify("Religion No. is required.", 2);
            valid = false;
        }

        if (data.PAY_MODE == '' || data.PAY_MODE == null) {
            empr_helper.notify("Pay Mode No. is required.", 2);
            valid = false;
        }

        if (data.JOIN_DATE == '' || data.JOIN_DATE == null) {
            empr_helper.notify("Joining Date is required.", 2);
            valid = false;
        }

        if (data.PARM_DATE == '' || data.PARM_DATE == null) {
            empr_helper.notify("Permanent Date is required.", 2);
            valid = false;
        }

        //if (data.FAMILY_NUM == '' || data.FAMILY_NUM == null) {
        //    empr_helper.notify("Family No. is required.", 2);
        //    valid = false;
        //}

        if (data.EMP_ADD == '' || data.EMP_ADD == null) {
            empr_helper.notify("Address is required.", 2);
            valid = false;
        }

        //if (data.DOB == '' || data.DOB == null) {
        //    empr_helper.notify("DOB is required.", 2);
        //    valid = false;
        //}

        //if (data.RSTATUS == '' || data.RSTATUS == null) {
        //    empr_helper.notify("Roster Status is required.", 2);
        //    valid = false;
        //}

        //if (data.EMAIL != '') {
        //    if (!empr_helper.isEmail(data.EMAIL)) {
        //        empr_helper.notify("Please add valid email address.", 2);
        //        valid = false;
        //    }
        //}

        return valid;
    },

    InitQuickSearch: function () {

        empr_Employee.GetDataForQuickSeachGrid();
    },

    GetDataForQuickSeachGrid: function () {

        var xhr = ajaxHelper.ajaxGetJson('/Employee/QuickSearchEmployee', function (data) {
            empr_Employee.CreateGrid(data.data);
        }, false, true);

    },

    CreateGrid: function (dataSrc) {
        var col = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    var html = '<div class="btn-group btn-group-sm">';
                    if (options.data.emP_IMG != null && options.data.emP_IMG != '' && options.data.emP_IMG != undefined) {
                        html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('/images/upload/employees/${options.data.emP_IMG}')"><i class="fa fa-eye"></i></a>`;
                    }
                    html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" rportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>`;
                    html += '</div>';
                    $(html).appendTo(container);
                }
            },
            { dataField: 'machinE_CODE', caption: 'M.Code' },
            { dataField: 'id', caption: 'Code', visible: false },
            { dataField: 'emP_ID', caption: 'Emp.Id' },
            { dataField: 'ename', caption: 'Employee Name', width: 150 },
            { dataField: 'fatheR_NAME', caption: 'Father Name', width: 150 },
            { dataField: 'deP_ID', caption: 'Department', width: 150 },
            { dataField: 'desig', caption: 'Designation', visible: false },
            { dataField: 'gender', caption: 'Gender', visible: false },
            { dataField: 'bcode', caption: 'Branch', visible: false },
            { dataField: 'shifT_T', caption: 'Shift', width: 80 },
            { dataField: 'emP_TYPE', caption: 'Type', visible: false },
            { dataField: 'email', caption: 'Email', width: 150 },
            { dataField: 'celL_NO', caption: 'Cell No.', width: 100 },
            { dataField: 'reg', caption: 'Religion', width: 80 },
            { dataField: 'child', caption: 'Childs', visible: false },
            { dataField: 'paY_MODE', caption: 'Pay Mode', visible: false },
            { dataField: 'joiN_DATE', caption: 'J.Date', dataType: 'date', format: 'dd-MM-yyy', width: 80 },
            { dataField: 'parM_DATE', caption: 'Perm.Date', dataType: 'date', format: 'dd-MM-yyy', width: 100 },
            { dataField: 'lefT_DATE', caption: 'Left Date', dataType: 'date', format: 'dd-MM-yyy', visible: false, },
            { dataField: 'salarY_HOLD', caption: 'Salary', visible: true, width: 70 },
            { dataField: 'cnic', caption: 'CNIC', visible: false },
            { dataField: 'familY_NUM', caption: 'Family No.', visible: false },
            { dataField: 'banK_ACC', caption: 'Account. No.', visible: false },
            { dataField: 'cniC_IDATE', caption: 'CNIC Issue', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'cniC_EDATE', caption: 'CNIC Expiry', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'banK_NAME', caption: 'Bank', visible: false },
            { dataField: 'ntN_NO', caption: 'NTN No.', visible: false },
            { dataField: 'filE_NO', caption: 'File No.', visible: false },
            { dataField: 'emP_ADD', caption: 'Address', visible: false },
            { dataField: 'nation', caption: 'Nationality', visible: false },
            { dataField: 'pob', caption: 'POB', visible: false },
            { dataField: 'dob', caption: 'DOB', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'mstatus', caption: 'Emp.Status', visible: true, width: 100 },
            { dataField: 'mstaff', caption: 'MStaff', visible: false },
            { dataField: 'rstatus', caption: 'Rational Status', visible: false },
            { dataField: 'thumb', caption: 'Thumb', visible: false },
            { dataField: 'signa', caption: 'Signature', visible: false },
            { dataField: 'emP_IMG', caption: 'Image', visible: false },
            { dataField: 'ot', caption: 'OT', visible: false },
            { dataField: 'emP_CAST', caption: 'Cast', visible: false },
            { dataField: 'eduction', caption: 'Education', visible: false },
            { dataField: 'veH_NUMBER', caption: 'Vehicle No.', visible: false },
            { dataField: 'ltype', caption: 'License Type', visible: false },
            { dataField: 'lnumber', caption: 'License No.', visible: false },
            { dataField: 'l_IDATE', caption: 'License Issue', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'l_EDATE', caption: 'License Expiry', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'reasoN_L', caption: 'Reason of Leaving', visible: false },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            { dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
            { dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, },
            { dataField: 'astatus', caption: 'Updated Postal Code', visible: false, },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Employee");
        setTimeout(function () {
            $('#gridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    GetEmployeeByEmployeeCode: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/Employee/GetEmployeeByEmployeeCode?code=' + id, function (data) {
            var employeedata = data.employeeData.data;

            empr_Employee.InitGenderDDL(employeedata.gender);
            empr_Employee.InitReligionDDL(employeedata.reg);
            empr_Employee.InitOfferLetterDDL(employeedata.offeR_LETTER);
            empr_Employee.InitDepartmentDDL(employeedata.deP_ID);
            empr_Employee.InitDesignationDDL(employeedata.desig);
            empr_Employee.InitEmploymentTypeDDL(employeedata.emP_TYPE);
            empr_Employee.InitShiftDDL(employeedata.shifT_T);
            empr_Employee.InitBranchDDL(employeedata.bcode);
            empr_Employee.InitPaymentModeDDL(employeedata.paY_MODE);
            empr_Employee.InitMaritalStatusDDL(employeedata.mstatus);
            empr_Employee.InitRoasterStatusDDL(employeedata.rstatus);
            empr_Employee.InitEducationDDL(employeedata.education);

            $("#Code").val(employeedata.emP_CODE);


            $('#EMP_ID').val(employeedata.emP_ID);
            $('#MACHINE_CODE').val(employeedata.machinE_CODE);
            $('#ENAME').val(employeedata.ename);
            $('#FATHER_NAME').val(employeedata.fatheR_NAME);
            $('#CELL_NO').val(employeedata.celL_NO);
            $('#EMAIL').val(employeedata.email);
            $('#CHILD').val(employeedata.child);
            $('#JOIN_DATE').val(employeedata.joiN_DATE);
            $('#PARM_DATE').val(employeedata.parM_DATE);
            $("#SALARY_HOLD").prop("checked", employeedata.salarY_HOLD == 'Y' ? true : false);
            $('#CNIC').val(employeedata.cnic);
            $('#FAMILY_NUM').val(employeedata.familY_NUM);
            $('#BANK_ACC').val(employeedata.banK_ACC);
            $('#CNIC_IDATE').val(employeedata.cniC_IDATE);
            $('#CNIC_EDATE').val(employeedata.cniC_EDATE);
            $('#BANK_NAME').val(employeedata.banK_NAME);
            $('#NTN_NO').val(employeedata.ntN_NO);
            $('#FILE_NO').val(employeedata.filE_NO);
            $('#EMP_ADD').val(employeedata.emP_ADD);
            $('#NATION').val(employeedata.nation);
            $('#POB').val(employeedata.pob);
            $('#DOB').val(employeedata.dob);
            $("#MSTAFF").prop("checked", employeedata.mstaff == 'Y' ? true : false);
            $("#OT").prop("checked", employeedata.ot == 'Y' ? true : false);
            $('#EMP_CAST').val(employeedata.emP_CAST);
            $('#VEH_NUMBER').val(employeedata.veH_NUMBER);
            $('#LTYPE').val(employeedata.ltype);
            $('#LNUMBER').val(employeedata.lnumber);
            $('#L_IDATE').val(employeedata.l_IDATE);
            $('#L_EDATE').val(employeedata.l_EDATE);
            $('#LEFT_DATE').val(employeedata.lefT_DATE);
            $('#REASON_L').val(employeedata.reasoN_L);
            $('#ASTATUS').dxSelectBox('instance').option("value", employeedata.astatus);
            $('#hdnTHUMB').val(employeedata.signa);
            $('#hdnSIGNA').val(employeedata.thumb);
            $('#EMP_IMG').val(employeedata.emP_IMG);

            var fullPath = '/images/upload/employees/' + employeedata.emP_IMG;
            $('#empProfileImgFinal').attr('src', fullPath);

            $('#pills-warningprofile-tab').show();

            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
                }
                if (Permissions.r_EDIT) {
                    $('.saveAttempt').show();
                    $('#saveBranchInfo').show();
                }
                else {
                    $('.saveAttempt').hide();
                    $('#saveBranchInfo').hide();
                }
            } else {
                $('.saveAttempt').show();
                $('#saveBranchInfo').show();
                $('.btn-delete').show();
            }
            $('.modal').modal('hide');

        }, false, true);
    },

    delete: function (_id) {
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
            var ACT_CODE = $("#chartofaccount_hidden").val();
            var xhr = ajaxHelper.ajaxPostJsonData({ code: _id, actCode: ACT_CODE }, "/Employee/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {

                    empr_Employee.resetForm();
                }
            }, false, true);
        });
        //var xhr = ajaxHelper.ajaxPostJsonData({ partycode: _id }, "/Employee/Delete", function (data) {
        //    empr_helper.notify(data.msg, data.msgType);
        //    if (data.msgType == 1) {

        //        empr_Employee.resetForm();
        //    }
        //}, false, true);

    },

    getDataToSave: function () {

        var EMP_CODE = $('#Code').val();
        var EMP_ID = $('#EMP_ID').val();
        var MACHINE_CODE = $('#MACHINE_CODE').val();
        var ENAME = $('#ENAME').val();
        var DEP_ID = $('#DEP_ID').dxSelectBox('instance').option('value');
        var FATHER_NAME = $('#FATHER_NAME').val();
        var DESIG = $('#DESIG').dxSelectBox('instance').option('value');
        var GENDER = $('#GENDER').dxSelectBox('instance').option('value');
        var BCODE = $('#BCODE').dxSelectBox('instance').option('value');
        var SHIFT_T = $("#SHIFT_hidden").val();
        var EMP_TYPE = $('#EMP_TYPE').dxSelectBox('instance').option('value');
        var CELL_NO = $('#CELL_NO').val();
        var EMAIL = $('#EMAIL').val();
        var REG = $('#REG').dxSelectBox('instance').option('value');
        var OFFER_LETTER = $('#OFFER_LETTER').dxSelectBox('instance').option('value');
        var CHILD = $('#CHILD').val();
        var PAY_MODE = $('#PAY_MODE').dxSelectBox('instance').option('value');
        var JOIN_DATE = $('#JOIN_DATE').val();
        var PARM_DATE = $('#PARM_DATE').val();
        var SALARY_HOLD = $('#SALARY_HOLD').is(':checked') ? 'Y' : 'N';
        var CNIC = $('#CNIC').val();
        var FAMILY_NUM = $('#FAMILY_NUM').val();
        var BANK_ACC = $('#BANK_ACC').val();
        var CNIC_IDATE = $('#CNIC_IDATE').val();
        var CNIC_EDATE = $('#CNIC_EDATE').val();
        var BANK_NAME = $('#BANK_NAME').val();
        var NTN_NO = $('#NTN_NO').val();
        var FILE_NO = $('#FILE_NO').val();
        var EMP_ADD = $('#EMP_ADD').val();
        var NATION = $('#NATION').val();
        var POB = $('#POB').val();
        var DOB = $('#DOB').val();
        var MSTATUS = $('#MSTATUS').dxSelectBox('instance').option('value');
        var MSTAFF = $('#MSTAFF').is(':checked') ? 'Y' : 'N';
        var RSTATUS = $('#RSTATUS').dxSelectBox('instance').option('value');
        var THUMB = $('#hdnTHUMB').val();
        var SIGNA = $('#hdnSIGNA').val();
        var EMP_IMG = $('#EMP_IMG').val();
        var OT = $('#OT').is(':checked') ? 'Y' : 'N';
        var EMP_CAST = $('#EMP_CAST').val();
        var EDUCTION = $('#EDUCTION').dxSelectBox('instance').option('value');
        var VEH_NUMBER = $('#VEH_NUMBER').val();
        var LTYPE = $('#LTYPE').val();
        var LNUMBER = $('#LNUMBER').val();
        var L_IDATE = $('#L_IDATE').val();
        var L_EDATE = $('#L_EDATE').val();
        var LEFT_DATE = $('#LEFT_DATE').val();
        var REASON_L = $('#REASON_L').val();
        var ASTATUS = $('#ASTATUS').dxSelectBox('option', 'value');

        var modelRecord = {
            EMP_CODE: EMP_CODE,
            EMP_ID: EMP_ID,
            MACHINE_CODE: MACHINE_CODE,
            ENAME: ENAME,
            DEP_ID: DEP_ID,
            FATHER_NAME: FATHER_NAME,
            DESIG: DESIG,
            GENDER: GENDER,
            BCODE: BCODE,
            SHIFT_T: SHIFT_T,
            EMP_TYPE: EMP_TYPE,
            CELL_NO: CELL_NO,
            EMAIL: EMAIL,
            REG: REG,
            OFFER_LETTER: OFFER_LETTER,
            CHILD: CHILD,
            PAY_MODE: PAY_MODE,
            JOIN_DATE: JOIN_DATE,
            PARM_DATE: PARM_DATE,
            SALARY_HOLD: SALARY_HOLD,
            CNIC: CNIC,
            FAMILY_NUM: FAMILY_NUM,
            BANK_ACC: BANK_ACC,
            CNIC_IDATE: CNIC_IDATE,
            CNIC_EDATE: CNIC_EDATE,
            BANK_NAME: BANK_NAME,
            NTN_NO: NTN_NO,
            FILE_NO: FILE_NO,
            EMP_ADD: EMP_ADD,
            NATION: NATION,
            POB: POB,
            DOB: DOB,
            MSTATUS: MSTATUS,
            MSTAFF: MSTAFF,
            RSTATUS: RSTATUS,
            THUMB: THUMB,
            SIGNA: SIGNA,
            EMP_IMG: EMP_IMG,
            OT: OT,
            EMP_CAST: EMP_CAST,
            EDUCTION: EDUCTION,
            VEH_NUMBER: VEH_NUMBER,
            LTYPE: LTYPE,
            LNUMBER: LNUMBER,
            L_IDATE: L_IDATE,
            L_EDATE: L_EDATE,
            LEFT_DATE: LEFT_DATE,
            REASON_L: REASON_L,
            ASTATUS: ASTATUS
        }
        return modelRecord;
    },

    saveAttempt: function () {
        debugger;
        var dataModel = empr_Employee.getDataToSave();
        var xhr = ajaxHelper.ajaxPostJsonData(dataModel, "/Employee/SaveMainOtherInfo", function (data) {

            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {

                $('#Code').val(data.data);
                $('#pills-warningprofile-tab').show();
                $('.btn-delete').show();

                //if (dataModel.PARTY_CODE > 0) {
                //    empr_Employee.resetForm();
                //}
            }
        }, false, true);
    },

    resetForm: function () {


        $('#pills-warningprofile-tab').hide();
        $('#myTab li:first-child a').click();
        $('.tab-pane').removeClass('fade');
        $('.btn-delete').hide();

        $("#Code").val('');
        $('#EMP_ID').val('');
        $('#MACHINE_CODE').val('');
        $('#ENAME').val('');
        $('#FATHER_NAME').val('');
        $('#CELL_NO').val('');
        $('#EMAIL').val('');
        $('#CHILD').val('');
        $('#JOIN_DATE').val('');
        $('#PARM_DATE').val('');
        $("#SALARY_HOLD").prop("checked", true);
        $('#CNIC').val('');
        $('#FAMILY_NUM').val('');
        $('#BANK_ACC').val('');
        $('#CNIC_IDATE').val('');
        $('#CNIC_EDATE').val('');
        $('#BANK_NAME').val('');
        $('#NTN_NO').val('');
        $('#FILE_NO').val('');
        $('#EMP_ADD').val('');
        $('#NATION').val('');
        $('#POB').val('');
        $('#DOB').val('');
        $("#MSTAFF").prop("checked", true);
        $("#OT").prop("checked", true);
        $('#EMP_CAST').val('');
        $('#VEH_NUMBER').val('');
        $('#LTYPE').val('');
        $('#LNUMBER').val('');
        $('#L_IDATE').val('');
        $('#L_EDATE').val('');
        $('#LEFT_DATE').val('');
        $('#REASON_L').val('');
        //$('#ASTATUS').dxSelectBox('instance').option("value", "Y");
        $('#hdnTHUMB').val('');
        $('#hdnSIGNA').val('');
        $('#EMP_IMG').val('');
        $('#empProfileImgFinal').attr('src', '/Client/Company/default.jpg');

        empr_Employee.InitGenderDDL();
        empr_Employee.InitReligionDDL();
        empr_Employee.InitOfferLetterDDL();
        empr_Employee.InitDepartmentDDL();
        empr_Employee.InitDesignationDDL();
        empr_Employee.InitEmploymentTypeDDL();
        empr_Employee.InitShiftDDL();
        empr_Employee.InitBranchDDL();
        empr_Employee.InitPaymentModeDDL();
        empr_Employee.InitMaritalStatusDDL();
        empr_Employee.InitRoasterStatusDDL();
        empr_Employee.InitEducationDDL();

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('.saveAttempt').show();
            } else {
                $('.saveAttempt').hide();
            }
        }
    },

    uploadFile: function () {

        var input = document.getElementById('DocumentFile');
        var files = input.files;
        var formData = new FormData();

        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax(
            {
                url: "/Employee/UploadImage",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#PartytypesDocs_hidden").val(data.data);
                    }
                }
            }
        );

    },

    uploadCNIC: function () {

        var input = document.getElementById('CnicFile');
        var files = input.files;
        var formData = new FormData();

        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }

        $.ajax(
            {
                url: "/Employee/UploadImage",
                //url: "/User/uploadfiles",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {

                    if (data.msgType == '1') {

                        $("#PartytypesCNIC_hidden").val(data.data);

                    }

                }
            }
        );

    },

    InitGenderDDL: function (_selectedValue) {
        $('#GENDER').dxSelectBox({
            dataSource: empr_helper.gender,
            displayExpr: 'key',
            valueExpr: 'value',
            value: _selectedValue,
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
    },

    InitReligionDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetReligions',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#REG').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitOfferLetterDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetOfferLetters',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#OFFER_LETTER').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                            var selectedKey = e.value;
                            empr_Employee.SetValuesByOfferLetter(selectedKey);
                        }
                    });
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    SetValuesByOfferLetter: function (id) {
        var data = EmpDataByOfferLetter.find(x => x.traN_ID == id);
        console.log('selected Obj', data);
        $('#ENAME').val(data.ename);
        $('#EMAIL').val(data.email);
        $('#CELL_NO').val(data.celL_NO);
        $('#JOIN_DATE').val(data.joininG_DATE);
        $('#DOB').val(data.dob);
        $('#EMP_ADD').val(data.emP_ADD);
        empr_Employee.InitGenderDDL(data.gender);
        empr_Employee.InitDepartmentDDL(data.deP_ID);
        empr_Employee.InitBranchDDL(data.bcode);
    },

    InitDepartmentDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetDepartments',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#DEP_ID').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitDesignationDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetDesignations',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#DESIG').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitEmploymentTypeDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetEmploymentTypes',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#EMP_TYPE').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitShiftDDL: function (_selectedValue) {
        console.log(_selectedValue)
        $.ajax({
            url: "Employee/GetShifts",
            type: "GET",
            success: function (response) {
                var Datasource = response.data;

                selectedObject = [];
                selectedValue = _selectedValue;

                if (_selectedValue != null) {
                    selectedObject = Datasource.filter(x => { return x.code == _selectedValue }) || [];
                    if (selectedObject.length > 0) {

                        selectedValue = selectedObject[0].name;
                        $('#SHIFT_hidden').val(selectedObject[0].code);
                        $('#displayExprShift').val(selectedObject[0].name);

                    }
                }

                let gridInstance;
                let currentSearchTerm = "";
                let isProgrammaticOpen = false;

                $("#SHIFT_T").dxDropDownBox({
                    value: selectedValue,
                    valueExpr: "name",
                    displayExpr: function (item) {
                        return item ? `${item.name}` : "Select a value...";
                    },
                    dataSource: Datasource,
                    placeholder: 'Select a value...',
                    acceptCustomValue: true,
                    showClearButton: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onValueChanged: function (e) {
                        if (e.value && gridInstance) {
                            const selectedData = gridInstance.getDataSource().items().find(item => item.name === e.value);
                            if (selectedData) {
                                $('#SHIFT_hidden').val(selectedData.code);
                                $('#displayExprShift').val(selectedData.name);

                            }
                        } else {
                            $('#SHIFT_hidden').val('');
                            $('#displayExprShift').val('');
                        }
                    },
                    //onFocusIn: function (e) {
                    //    if (!isProgrammaticOpen) {
                    //        isProgrammaticOpen = true;
                    //        e.component.open();
                    //        setTimeout(() => { isProgrammaticOpen = false; }, 100);
                    //    }
                    //},
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
                                input.select();
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
                                key: "code"
                            }),
                            columns: [
                                {
                                    dataField: "name",
                                    caption: "Name",
                                    width: '60px',
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "timeIn",
                                    caption: "Time In",
                                    cellTemplate: function (container, options) {
                                        highlightText(container, options.value);
                                    }
                                },
                                {
                                    dataField: "timeOut",
                                    caption: "Time Out",
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
                                    e.component.option("value", selected.name);
                                    e.component.close();

                                    $('#SHIFT_hidden').val(selected.code);
                                    $('#displayExprShift').val(selected.name);
                                }
                            },
                            onContentReady: function (e) {
                                if (currentSearchTerm) {
                                    const items = e.component.getDataSource().items();
                                    if (items.length > 0) {
                                        e.component.selectRows([items[0].name], false);
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
                            ["name", "contains", searchTerm],
                            "or",
                            ["timeIn", "contains", searchTerm],
                            "or",
                            ["timeOut", "contains", searchTerm]
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

                // Handle clear button
                $(document).on("dxclick", "#InitShiftDDL .dx-clear-button-area", function (e) {
                    currentSearchTerm = "";
                    $('#SHIFT_hidden').val('');
                    $('#displayExprShift').val('');
                    if (gridInstance) {
                        gridInstance.getDataSource().filter(null);
                        gridInstance.refresh();
                    }
                });
            }
        });
    },

    InitBranchDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetBranches',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#BCODE').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    InitPaymentModeDDL: function (_selectedValue) {
        var dataSource = [
            { value: 'C', key: 'Cash' },
            { value: 'B', key: 'Bank' },
            { value: 'CHQ', key: 'Cheque' },
        ];

        $('#PAY_MODE').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'key',
            valueExpr: 'value',
            value: _selectedValue,
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
    },

    InitMaritalStatusDDL: function (_selectedValue) {
        $('#MSTATUS').dxSelectBox({
            dataSource: empr_helper.maritalStatus,
            displayExpr: 'key',
            valueExpr: 'value',
            value: _selectedValue,
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
    },

    InitRoasterStatusDDL: function (_selectedValue) {
        var dataSource = [
            { value: 'RE', key: 'Roaster/Non-Roaster Employee' },
            { value: 'DS', key: 'Direct Salary' },
        ];

        $('#RSTATUS').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'key',
            valueExpr: 'value',
            value: _selectedValue,
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
    },

    InitEducationDDL: function (_selectedValue) {
        $.ajax({
            url: 'Employee/GetEducations',
            method: 'GET',
            success: function (data) {
                if (data.msgType == 1) {
                    $('#EDUCTION').dxSelectBox({
                        dataSource: data.data,
                        displayExpr: 'value',
                        valueExpr: 'key',
                        value: _selectedValue,
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
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    },

    UploadThumb: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('THUMB').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/Employee/UploadThumbs",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnTHUMB").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },

    OpenThumb: function () {
        var hdnUrl = $('#hdnTHUMB').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    UploadSign: function () {
        $('#BtnSave').prop('disabled', true);
        var files = document.getElementById('SIGNA').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/Employee/UploadSigns",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnSIGNA").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#BtnSave').prop('disabled', false);

                }
            }
        );
    },

    OpenSign: function () {
        var hdnUrl = $('#hdnSIGNA').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },

    //SaveImage() {
    //    $('#BtnSave').prop('disabled', true);
    //    var base64String = $('#item-img-output').attr('src').replace('data:image/png;base64,', '');
    //    var binaryData = atob(base64String);
    //    var blob = new Blob([new Uint8Array(Array.prototype.map.call(binaryData, function (char) {
    //        return char.charCodeAt(0);
    //    }))], { type: 'image/png' });

    //    var formData = new FormData();
    //    formData.append('model', blob);
    //    $.ajax({
    //        url: "/Employee/SaveImage",
    //        data: formData,
    //        processData: false,
    //        contentType: false,
    //        type: "POST",
    //        success: function (data) {
    //            if (data.msgType == '1') {
    //                $("#EMP_IMG").val(data.data);
    //            }
    //            else {
    //                console.log(data);
    //                empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
    //            }
    //            $('#BtnSave').prop('disabled', false);
    //        }
    //    });
    //},

    SaveImage() {
        debugger;
        $('#BtnSave').prop('disabled', true);

        var imgTag = $('#empProfileImgFinal');
        var src = imgTag.attr('src');

        if (!src || src.includes('/Client/Company/default.jpg')) {
            $('#BtnSave').prop('disabled', false);
            return;
        }

        var base64String = src.replace('data:image/png;base64,', '');

        var binaryData = atob(base64String);
        var bytes = new Uint8Array([...binaryData].map(c => c.charCodeAt(0)));
        var blob = new Blob([bytes], { type: 'image/png' });

        var formData = new FormData();
        formData.append('model', blob);

        $.ajax({
            url: "/Employee/SaveImage",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                if (data.msgType == '1') {
                    console.log(data.data);
                    $("#EMP_IMG").val(data.data);
                } else {
                    console.log(data);
                    empr_helper.notify("Something went wrong while saving the file. Please re-upload.", 2);
                }
                $('#BtnSave').prop('disabled', false);
            },
            error: function (xhr, status, err) {
                console.error(err);
                empr_helper.notify("Image upload failed.", 2);
                $('#BtnSave').prop('disabled', false);
            }
        });
    },

    OpenImage: function () {
        var baseUrl = "/images/upload/employees/";
        var hdnUrl = $('#EMP_IMG').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            ShowImage('/images/upload/employees/' + hdnUrl)
            //const fileURL = window.location.origin + baseUrl + hdnUrl;
            //window.open(fileURL, '_blank');
        }
    },

    bindDxGridBoxDdl: function (divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun) {
        ati_dxHelper.DxGridBoxDropdown(divid, datasrc, col, hiddenid, selectedOjb, selectval, valueExpr, displayExpr, displayExprHidden, onchangeFun);
    },
}