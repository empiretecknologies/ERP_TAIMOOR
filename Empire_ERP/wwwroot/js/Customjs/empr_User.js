var empr_User = {
    TotalTime: 120000,

    initEvents() {
        $(document).ready(function () {

            empr_User.toggleResetButton();

            $('#btnClearImage').click(function () {

                if (empr_User.croppieInstance) {
                    empr_User.croppieInstance.croppie('destroy');
                    empr_User.croppieInstance = null;
                    $("#EMP_IMG").val('');
                }

                $('#croppieContainer').hide().empty();
                $('#empProfileImg').hide().attr('src', '');
                $('#btnTransImg').hide();
                $('#btnTakeCapture').show();
                $('#btnPickImage').show();
                $('#cameraPreview').show();

                empr_User.startCamera();

                $('#btnClearImage').hide();
            });


            //$('#btnResetImage').click(function () {
            //    empr_User.resetImageProcess();
            //});

            $('#btnPickImage').click(function () {
                empr_User.pickImage();
            });

            $('#empImgUpload').change(function () {
                empr_User.onImagePicked(this);
            });

            $('#btnCaptureImage').click(function () {
                empr_User.startCamera();
            });

            $('#btnTakeCapture').click(function () {

                $('#btnPickImage').hide();
                empr_User.captureImage();
            });

            $('#btnOpenUploadModal').click(function () {
                $('#uploadImageModal').modal('show');
                empr_User.resetImageProcess();
                if (empr_User.croppieInstance != null) {
                    $('#btnPickImage').hide();
                    $('#btnTakeCapture').hide();
                } else {
                    $('#btnPickImage').show();
                    $('#btnTakeCapture').show();
                    $('#btnClearImage').hide();
                }



                empr_User.startCamera();
            });

            $('body').on('click', '#btnTransImg', function () {
                debugger;
                if (empr_User.croppieInstance) {
                    empr_User.croppieInstance.croppie('result', {
                        type: 'base64',
                        size: 'viewport',
                        format: 'png',
                        quality: 1
                    }).then(function (croppedImage) {
                        $('#empProfileImgFinal').attr('src', croppedImage).show();
                        empr_User.SaveImage();

                        $('#empCroppedImage').val(croppedImage);

                        empr_User.toggleResetButton();
                        $('#uploadImageModal').modal('hide');

                        empr_User.croppieInstance.croppie('destroy');
                        empr_User.croppieInstance = null;
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
                        empr_User.SaveImage();
                        $('#empCroppedImage').val(imgData);
                        $('#uploadImageModal').modal('hide');
                    }
                }
            });












            const today = new Date();
            const formattedDate = today.toISOString().split('T')[0];
            $('#USTART_DATE').val(formattedDate);

            empr_User.InitQuickSearch();

            empr_User.InitBranchDDL();

            empr_User.InitRoleTypeDDL();

            $('body').on('click', '#BtnNew', function () {
                $('#BtnDelete').hide();
                empr_User.AddNew();
            });

            $('#BtnVerify').click(function () {
                empr_User.OTPVerification();
            });

            var otpInputs = document.querySelectorAll(".otpclass");

            empr_User.SetupOTPInputListeners(otpInputs);

            otpInputs[0].focus();

            otpInputs[5].addEventListener("input", function () {
                empr_User.SetOTPValue(otpInputs);
            });

            $('#BtnSave').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_User.validateForm()) {
                            if ($('#Code').val() > 0) {
                                empr_User.saveAttempt();
                            } else {
                                empr_User.SendOTP();
                            }
                        }
                    }
                } else {
                    if (empr_User.validateForm()) {
                        if ($('#Code').val() > 0) {
                            empr_User.saveAttempt();
                        } else {
                            empr_User.SendOTP();
                        }
                    }
                }
            });

            $('body').on('click', '#QuickSearch', function () {
                empr_User.InitQuickSearch();
            });

            $('body').on('click', '.elm_edit', function () {
                var reportid = $(this).attr("reportid");
                empr_User.GetUserByID(reportid);
            });

            $('#BtnDelete').click(function () {
                empr_User.DeleteRecord();
            });

            $('#EMAIL').on('input', function () {
                var email = $(this).val();
                if (!empr_User.validateEmail(email)) {
                    $(this).css('border-color', 'red');
                } else {
                    $(this).css('border-color', '');
                }
            });

            $('#CELL_NO').on('input', function () {
                var number = $(this).val();
                if (!empr_User.validateNumber(number)) {
                    $(this).css('border-color', 'red');
                } else {
                    $(this).css('border-color', '');
                }
            });
            
            $('#UPASS').on('input', function () {
                var upass = $(this).val();
                empr_User.validatePassword(upass)                
            });
            $('#CPASS').on('input', function () {
                var cpass = $(this).val();
                empr_User.validateCPassword(cpass)
            });

            if (Permissions != "Admin") {
                !Permissions.r_VIEW && $('#gridContainer').hide();
                !Permissions.r_ADD && $('#BtnNew').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
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
        empr_User.stopCamera();

        if (empr_User.croppieInstance) {
            empr_User.croppieInstance.croppie('destroy');
            empr_User.croppieInstance = null;
            $("#EMP_IMG").val('');
        }

        $('#empProfileImgFinal').attr('src', '/Client/Company/default.jpg').show();
        $('#empProfileImg').attr('src', '').hide();
        $('#cameraPreview').hide();

        $('#croppieContainer').hide().empty();

        $('#btnTakeCapture').hide();
        $('#btnTransImg').hide();

        $('#empImgUpload').val('');

        empr_User.toggleResetButton();
    },

    initCroppie: function (imageSrc) {
        if (empr_User.croppieInstance) {
            empr_User.croppieInstance.croppie('destroy');
        }

        $('#croppieContainer').show();
        $('#empProfileImg').hide();

        const $finalImg = $('#empProfileImgFinal');
        const finalWidth = $finalImg.width() || 200;
        const finalHeight = $finalImg.height() || 200;

        empr_User.croppieInstance = $('#croppieContainer').croppie({
            viewport: { width: finalWidth, height: finalHeight, type: 'square' },
            boundary: { width: finalWidth + 100, height: finalHeight + 100 },
            enableOrientation: true
        });

        empr_User.croppieInstance.croppie('bind', {
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
                empr_User.stopCamera();
                empr_User.initCroppie(e.target.result);
            };
            reader.readAsDataURL(file);
        }
    },

    startCamera: function () {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(s => {
                empr_User.stream = s;
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

        empr_User.stopCamera();

        $('#cameraPreview').hide();
        $('#btnTakeCapture').hide();
        $('#btnClearImage').show();

        empr_User.initCroppie(imgData);

    },

    stopCamera: function () {
        if (empr_User.stream) {
            empr_User.stream.getTracks().forEach(track => track.stop());
        }
    },

    DeleteRecord() {

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/User/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_User.AddNew();
                    empr_User.InitQuickSearch();
                    $('#optmodal').modal('hide');
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });

    },

    resetForm() {
        debugger
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0];
        $('#empProfileImgFinal').attr('src', '/Client/Company/default.jpg');
        $("#branchhidden").val('');
        $("#rolehidden").val('');
        $("#Code").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $("#FULLNAME").val('');
        $("#USERNAME").val('');
        $("#EMAIL").val('');
        $("#CELL_NO").val('');
        //$('#ROLE').dxSelectBox('instance').option('value', null);
        $('#BRANCH').dxSelectBox('instance').option('value', null);
        $("#USTART_DATE").val(formattedDate);
        $("#ESTART_DATE").val('');
        $("#Image").val('');
        $("#GROUP_PIC").val('');
        $("#UPASS").val('');
        $("#CPASS").val('');
        $("#USERNAME").val('').prop('readonly', true);
        $("#EMAIL").val('').prop('readonly', true);
        $("#userPass").val('').addClass('d-none');
        $("#userconfirmPass").val('').addClass('d-none');

        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
    },

    AddNew() {
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0];
        $("#branchhidden").val('');
        $("#rolehidden").val('');
        $("#Code").val('');
        $('#ASTATUS').dxSelectBox('instance').option('value', "Y");
        $("#FULLNAME").val('');
        $("#USERNAME").val('');
        $("#EMAIL").val('');
        $("#CELL_NO").val('');
        $('#ROLE').dxSelectBox('instance').option('value', null);
        $('#BRANCH').dxSelectBox('instance').option('value', null);
        $("#USTART_DATE").val(formattedDate);
        $("#ESTART_DATE").val('');
        $("#Image").val('');
        $("#GROUP_PIC").val('');
        $("#UPASS").val('');
        $("#CPASS").val('');
        $("#USERNAME").val('').prop('readonly', false);
        $("#EMAIL").val('').prop('readonly', false);
        $("#userPass").val('').removeClass('d-none');
        $("#userconfirmPass").val('').removeClass('d-none');
        $('#empProfileImgFinal').attr('src', '/Client/Company/default.jpg');
    },

    validateForm() {

        var valid = true;
        var FULLNAME = $("#FULLNAME").val();
        var USERNAME = $("#USERNAME").val().trim();
        var EMAIL = $("#EMAIL").val();
        var CELL_NO = $("#CELL_NO").val();
        var ROLE = $("#ROLE").val();
        var BRANCH = $("#BRANCH").val();
        var USTART_DATE = $("#USTART_DATE").val();
        var ESTART_DATE = $("#ESTART_DATE").val();
        var PICTURES = $("#PICTURES").val();
        var UPASS = $("#UPASS").val();
        var CPASS = $("#CPASS").val();

        if (USERNAME == '') {
            valid = false;
            empr_helper.notify("Please enter Username.", 2);
        }

        if (!empr_User.validateEmail(EMAIL)) {
            valid = false;
            $('#EMAIL').css('border-color', 'red');
            empr_helper.notify("Please enter valid email address.", 2);
        }
        else {
            $('#EMAIL').css('border-color', '');
        }

        if ((!empr_User.validatePassword(UPASS) || !empr_User.validateCPassword(CPASS)) && ($('#Code').val() == 0 || $('#Code').val() == null || $('#Code').val() == "")) {
            valid = false;
            empr_helper.notify("Confirm Password must be similar to Password.", 2);
        }
        else {
            $('#UPASS').css('border-color', '');
            $('#CPASS').css('border-color', '');
        }

        if (!empr_User.validateNumber(CELL_NO)) {
            valid = false;
            $('#CELL_NO').css('border-color', 'red');
            empr_helper.notify("Please enter valid Cell No..", 2);
        }
        else {
            $('#CELL_NO').css('border-color', '');
        }

/*        if (new Date(ESTART_DATE) < new Date(USTART_DATE) || ESTART_DATE == '') {
            valid = false;
            $('#ESTART_DATE').css('border-color', 'red');
            empr_helper.notify("End Date must be greater then Start Date.", 2);
        } else {
            $('#ESTART_DATE').css('border-color', '');
        }*/

        return valid;
    },

    validateEmail(email) {
        const re = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return re.test(String(email).toLowerCase());
    },

    validateNumber(number) {
        const re = /^[0-9]+$/;
        return re.test(String(number));
    },

    validatePassword(UPASS) {

        var valid = false;

        $('.upass').css('display', 'block');
        $('.cpass').css('display', 'none');
        var password = UPASS,
            valid = true;

        var uppercase = password.match(/[A-Z]/),
            lowercase = password.match(/[a-z]/),
            number = password.match(/[0-9]/),
            specialChar = password.match(/[!@@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/);

        if (password.length < 8) {
            $('.password_length').removeClass('complete');
            valid = false;
        } else $('.password_length').addClass('complete');

        if (uppercase) $('.password_uppercase').addClass('complete');
        else {
            $('.password_uppercase').removeClass('complete');
            valid = false;
        }

        if (lowercase) $('.password_lowercase').addClass('complete');
        else {
            $('.password_lowercase').removeClass('complete');
            valid = false;
        }

        if (number) $('.password_number').addClass('complete');
        else {
            $('.password_number').removeClass('complete');
            valid = false;
        }

        if (specialChar) $('.password_special').addClass('complete');
        else {
            $('.password_special').removeClass('complete');
            valid = false;
        }
        if (valid) {
            $('.upass').css('display', 'none');
        } else {
            $('.upass').css('display', 'block');
        }
        return valid;



    },

    validateCPassword(CPASS) {
        debugger
        $('.cpass').css('display', 'block');
        $('.upass').css('display', 'none');
        var password = $('#UPASS').val();
        var conf = CPASS;
        var all_pass = true;
        if (conf == password) {
            $('.password_match').addClass('complete');
            $('#BtnSave').prop("disabled", false);
        }
        else {
            $('#BtnSave').prop("disabled", true);
            $('.password_match').removeClass('complete')
            all_pass = false;
        }
        if (all_pass) {
            $('.cpass').css('display', 'none');
        } else {
            $('.cpass').css('display', 'block');
        }

        return all_pass;
    },

    GetDataToSave() {

        var U_ID = $("#Code").val();
        var USERNAME = $("#USERNAME").val();
        var CELL_NO = $("#CELL_NO").val();
        var EMAIL = $("#EMAIL").val();
        var UPASS = $("#UPASS").val();
        var CPASS = $("#CPASS").val();
        var USTART_DATE = $("#USTART_DATE").val();
        var ESTART_DATE = $("#ESTART_DATE").val();
        var BRANCH = $("#branchhidden").val();
        var FULLNAME = $("#FULLNAME").val();
        var PICTURES = $("#GROUP_PIC").val();
        console.log('Pic',PICTURES);
        var DLT = $("#DLT").val();
        var ROLEID = $("#rolehidden").val();
        var ASTATUS = $("#ASTATUS").dxSelectBox('instance').option('value');
        var ROLE_TYPE = $("#ROLE_TYPE").dxSelectBox('instance').option('value');
        var MULTIPLE_LOGIN = $('#flexSwitchCheckDefault').is(':checked') ? 1 : 0;

        var modelRecord = {
            U_ID: U_ID,
            USERNAME: USERNAME,
            CELL_NO: CELL_NO,
            EMAIL: EMAIL,
            UPASS: UPASS,
            CPASS: CPASS,
            USTART_DATE: USTART_DATE,
            ESTART_DATE: ESTART_DATE,
            BRANCH: BRANCH,
            FULLNAME: FULLNAME,
            PICTURES: PICTURES,
            DLT: DLT,
            ROLEID: ROLEID,
            MAC_ID: "",
            ASTATUS: ASTATUS,
            ROLE_TYPE: ROLE_TYPE,
            MULTIPLE_LOGIN: MULTIPLE_LOGIN
        }

        return modelRecord;
    },

    saveAttempt() {

        var obj = empr_User.GetDataToSave();
        ajaxHelper.ajaxPostJsonData(obj, "/User/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_User.AddNew();
                empr_User.InitQuickSearch();
                $('#BtnDelete').hide();
            }
        }, false, true);
    },

    InitQuickSearch() {
        empr_User.GetAllUsers();
    },

    GetAllUsers() {
        ajaxHelper.ajaxGetJson('/User/QuickSearch', function (data) {
            empr_User.CreateGrid(data.data);
        }, false, true);
    },

    GetUserByID(id) {
        ajaxHelper.ajaxGetJson('/User/GetUserByID?id=' + id, function (data) {
            empr_User.resetForm();
            if (data.msgType == 1) {

                var record = data.data;
                var startDate = empr_User.FormatDate(record.ustarT_DATE);
                var endDate = empr_User.FormatDate(record.estarT_DATE);

                $("#branchhidden").val(record.branch);
                $("#rolehidden").val(record.roleid);
                $("#Code").val(record.u_ID);
                if (record.astatus == 'Active') {
                    //$('#ASTATUS').dxSelectBox('instance').option('value', 'Y');
                } else {
                    $('#ASTATUS').dxSelectBox('instance').option('value', 'N');
                }
                $("#FULLNAME").val(record.fullname);
                $("#USERNAME").val(record.username);
                $("#EMAIL").val(record.email);
                $("#CELL_NO").val(record.celL_NO);
                $("#flexSwitchCheckDefault").prop("checked", record.multiplE_LOGIN == 0 ? false : true);
                $('#BRANCH').dxSelectBox('instance').option('value', record.branch);
                $('#ROLE_TYPE').dxSelectBox('instance').option('value', record.rolE_TYPE);
                $("#USTART_DATE").val(startDate);
                $("#ESTART_DATE").val(endDate);
                $("#GROUP_PIC").val(record.pictures);
                $("#UPASS").val(record.upass);
                $("#CPASS").val(record.cpass);

                var fullPath = '/images/upload/users/' + record.pictures;
                $('#empProfileImgFinal').attr('src', fullPath);

                $('.modal').modal('hide');
                //$('#BtnDelete').show();
                $('#BtnNew').show();
                empr_User.InitRoleDDL(record.rolE_TYPE, record.roleid);
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateGrid(dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                var html = '<div class="btn-group btn-group-sm">';
                if (options.data.pictures != null && options.data.pictures != '' && options.data.pictures != undefined) {
                    html += `<a href="javascript:;" class="grid-action-icon" title="View Pic" onclick="ShowImage('/images/upload/users/${options.data.pictures}')"><i class="fa fa-eye"></i></a>`;
                }
                html += `<a href="javascript:;"  class="grid-action-icon elm_edit" style="padding-left: 6px;" reportid=${options.data.u_ID} title="Edit"><i class="fa fa-edit"></i></a>`;
                html += '</div>';
                $(html).appendTo(container);
            }
        },
        { dataField: 'fullname', caption: 'Name' },
        { dataField: 'username', caption: 'Username' },
        { dataField: 'email', caption: 'Email' },
        { dataField: 'rolE_NAME', caption: 'Role' },
        { dataField: 'brancH_NAME', caption: 'Branch' },
        { dataField: 'astatus', caption: 'Active' },
        { dataField: 'adD_USER_ID', caption: 'Created By', visible: false },
        { dataField: 'adD_DATE', caption: 'Created Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false },
        { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false },
        { dataField: 'adD_POSTALCODE', caption: 'Created PostalCode', visible: false },
        { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false },
        { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false },
        { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false },
        { dataField: 'ediT_DATE', caption: 'Updated Date', visible: false, dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'ediT_POSTALCODE', caption: 'Updated PostalCode', visible: false },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "UserQS");
    },

    InitBranchDDL() {
        empr_User.bindDxDdl("BRANCH", Branch, null, "key", "value", "Select", function (d) {
            $('#branchhidden').val(d.value)
            if (d.value == null) {
                $('#branchhidden').val('');
            }

        });
    },

    InitRoleDDL(roleType, selectedValue) {
        console.log(roleType == "A")
        console.log(roleType)
        if (roleType == "A") {
            $('#roleDDL').hide();
        } else {
            $('#roleDDL').show();
        }

        $.ajax({
            url: 'User/GetRolesByType',
            method: 'GET',
            data: { type: roleType },
            success: function (data) {
                console.log(data)
                $('#ROLE').dxSelectBox({
                    dataSource: data,
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
                        $('#rolehidden').val(e.value)
                        if (e.value == null) {
                            $('#rolehidden').val('');
                        }
                    }
                });
                if (selectedValue == null || selectedValue == 0) {
                    $('#ROLE').dxSelectBox('instance').option('value', 0);
                    $('#rolehidden').val('');
                }
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });

/*        empr_User.bindDxDdl("ROLE", Role, null, "key", "value", "Select", function (d) {

            $('#rolehidden').val(d.value)
            if (d.value == null) {
                $('#rolehidden').val('');
            }

        });*/
    },

    bindDxDdl(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },

    SendOTP() {
        $("#exampleModal").modal('hide');
        $('.otpclass').val('');
        $('#OTPCode').val('');
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/User/UserEmailVerification?email=' + $('#EMAIL').val(), function (data) {
            $("#Loader").hide();
            if (data.msgType == 1) {
                $('#message').html("We've sent you an email <b>" + data.data + "</b> containing your one-time password (OTP) for verification. </br>");
                $("#exampleModal").modal('show');
                empr_User.StartTimer();
                setTimeout(function () {
                    document.querySelectorAll(".otpclass")[0].focus();
                }, 500);
            } else {
                empr_helper.notify(data.msg, 2);
            }
        }, false, true);
    },

    StartTimer() {
        empr_User.UpdateTimer();
    },

    UpdateTimer() {
        const now = Date.now();
        const endTime = now + empr_User.TotalTime;
        const timerInterval = setInterval(function () {
            const timeLeft = endTime - Date.now();
            if (timeLeft <= 0) {
                clearInterval(timerInterval);
                $('#timer').html('<button type="button" onclick="$(\'#BtnSave\').click()" class="btn btn-primary btn-block w-30"><i class="fa-regular fa-paper-plane"></i> Resend OTP</button>');
            } else {
                const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
                const seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);
                $('#timer').text('Time remaining: ' + minutes + 'm ' + seconds + 's');
            }
        }, 1000);
    },

    OTPVerification() {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/Login/OTPVerification?username=' + $('#username').val().trim() + "&OTP=" + $('#OTPCode').val(), function (data) {
            $("#Loader").hide();
            if (data.msgType == 1) {
                $("#exampleModal").modal('hide');
                $("#Loader").hide();
                $("#NewModal").modal('show');
            } else {
                empr_helper.notify(data.msg, 2);
            }
        }, false, true);
    },

    SetupOTPInputListeners(inputs) {
        inputs.forEach(function (input, index) {
            input.addEventListener("paste", function (ev) {
                var clip = ev.clipboardData.getData('text').trim();
                if (!/^\d{6}$/.test(clip)) {
                    ev.preventDefault();
                    return;
                }

                var characters = clip.split("");
                inputs.forEach(function (otpInput, i) {
                    otpInput.value = characters[i] || "";
                });

                empr_Login.EnableNextBox(inputs[0], 0);
                inputs.forEach(function (input) {
                    input.removeAttribute("disabled");
                });
                inputs[5].focus();
                empr_Login.SetOTPValue(inputs);
                ev.preventDefault();
                return;
            });

            input.addEventListener("input", function () {
                var currentIndex = Array.from(inputs).indexOf(this);
                var inputValue = this.value.trim();

                if (!/^\d$/.test(inputValue)) {
                    this.value = "";
                    return;
                }

                if (inputValue && currentIndex < 5) {
                    inputs[currentIndex + 1].removeAttribute("disabled");
                    inputs[currentIndex + 1].focus();
                }

                if (currentIndex === 4 && inputValue) {
                    inputs[5].removeAttribute("disabled");
                    inputs[5].focus();
                }

                empr_Login.SetOTPValue(inputs);
            });

            input.addEventListener("keydown", function (ev) {
                var currentIndex = Array.from(inputs).indexOf(this);
                if ((ev.key == "Backspace" || ev.key == "Delete")) {
                    $('#BtnVerify').attr('disabled', true);
                }
                if (!this.value && ev.key === "Backspace" && currentIndex > 0) {
                    inputs[currentIndex - 1].focus();
                }
            });
        });
    },

    SetOTPValue(inputs) {

        var otpInputs = document.querySelectorAll(".otpclass");
        var otpValue = "";

        inputs.forEach(function (input) {
            otpValue += input.value;
        });

        if (otpValue.length == 6) {
            document.getElementById("OTPCode").value = otpValue;
            $('#BtnVerify').attr('disabled', false);
        }
        else {
            $('#BtnVerify').attr('disabled', true);
        }
    },

    OTPVerification() {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/User/OTPVerification?email=' + $('#EMAIL').val() + "&OTP=" + $('#OTPCode').val(), function (data) {
            $("#Loader").hide();
            if (data.msgType == 1) {
                $("#exampleModal").modal('hide');
                $("#Loader").hide();
                empr_User.saveAttempt();
            } else {
                empr_helper.notify(data.msg, 2);
            }
        }, false, true);
    },

    FormatDate(dateToSet) {
        var date = new Date(dateToSet);
        var year = date.getFullYear();
        var month = ('0' + (date.getMonth() + 1)).slice(-2);
        var day = ('0' + date.getDate()).slice(-2);
        var formattedDate = year + '-' + month + '-' + day;
        return formattedDate;
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
    //        url: "/User/SaveImage",
    //        data: formData,
    //        processData: false,
    //        contentType: false,
    //        type: "POST",
    //        success: function (data) {
    //            if (data.msgType == '1') {
    //                $("#GROUP_PIC").val(data.data);
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
            url: "/User/SaveImage",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                console.log('pic res',data);
                if (data.msgType == '1') {
                    $("#GROUP_PIC").val(data.data);
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

    InitRoleTypeDDL: function () {
        $('#ROLE_TYPE').dxSelectBox({
            dataSource: [
                { value: 'A', text: 'Admin' },
                { value: 'U', text: 'User' },
                { value: 'M', text: 'Manager' }
            ],
            valueExpr: 'value',
            displayExpr: 'text',
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search ......!',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
            onValueChanged: function (e) {
                empr_User.InitRoleDDL(e.value, 0);
            }
        });
    },
}