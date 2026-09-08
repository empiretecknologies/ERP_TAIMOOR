var empr_Login = {
    TotalTime: 120000,
    ips: [],
    ipAddress:'',
    InitEvents: function () {
        $(document).ready(function () {

            var hostname = location.hostname;
            var storedData = localStorage.getItem("empr_" + hostname);

            if (storedData) {
                var userData = JSON.parse(storedData);

                $('#username').val(userData.username || '');
                $('#password').val(userData.password || '');
                $('#checkbox1').prop('checked', userData.remember === true);
            }

            //window.exampleModalInstance = new bootstrap.Modal(document.getElementById('exampleModal'), {
            //    backdrop: 'static',
            //    keyboard: false
            //});

            //window.newModalInstance = new bootstrap.Modal(document.getElementById('newModal'), {
            //    backdrop: 'static',
            //    keyboard: false
            //});

            $('#loginattempt').click(function () {
                empr_Login.Login();
            });

            history.pushState(null, document.title, location.href);

            $('#BtnForgotPassword').click(function () {

                var username = $('#username').val().trim();
                if (username == '' || username == null || username == undefined) {
                    empr_helper.notify("Please enter the username.", 2);
                    return;
                }

                $("#Loader").show();
                $("#Loader").css('display', 'flex');
                setTimeout(function () {
                    $("#Loader").hide();
                    $('#ForgotUsername').val(username);
                    $("#ForgetFormSubmit").css('display', 'block');
                    $(".theme-form").hide();
                }, 500);
            });

            $('#BtnBack').click(function () {
                $("#Loader").show();
                $("#Loader").css('display', 'flex');
                setTimeout(function () {
                    $("#Loader").hide();
                    $("#ForgetFormSubmit").hide();
                    $(".theme-form").show();
                }, 500);
            });

            $('#BtnResetPassword').click(function () {
                $("#exampleModal").modal('hide');
                $('.otpclass').val('');
                $('#OTPCode').val('');
                $("#Loader").show();
                $("#Loader").css('display', 'flex');
                ajaxHelper.ajaxGetJson('/Login/UsernameVerification?username=' + $('#username').val().trim(), function (data) {
                    $("#Loader").hide();
                    if (data.msgType == 1) {
                        $('#message').html("We've sent you an email <b>" + data.data + "</b> containing your one-time password (OTP) for verification. </br>");
                        $("#exampleModal").modal('show');
                        empr_Login.StartTimer();
                        setTimeout(function () {
                            document.querySelectorAll(".otpclass")[0].focus();
                        }, 500);
                    } else {
                        empr_helper.notify(data.msg, 2);
                    }
                }, false, true);
            });

            $('#BtnVerify').click(function () {
                empr_Login.OTPVerification();
            });

            $('#password').on('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault(); // Prevent default behavior (like tab navigation)
                    $('#loginattempt').click();
                }
            });

            /* Adding script to toggle OTP inputs Start */
            var otpInputs = document.querySelectorAll(".otpclass");

            empr_Login.SetupOTPInputListeners(otpInputs);
            otpInputs[0].focus(); // Set focus on the first OTP input field
            otpInputs[5].addEventListener("input", function () {
                empr_Login.SetOTPValue(otpInputs);
            });
            /* Adding script to toggle OTP inputs End */

            $('#newpassword').keyup(function () {
                empr_Login.validateNewPassword();
            });
            $('#Conpassword').keyup(function () {
                debugger;
                empr_Login.validateConfirmPassword();
            });

            $('#BtnChangePassword').click(function () {
                if (empr_Login.validateNewPassword() && empr_Login.validateConfirmPassword()) {
                    empr_Login.UpdatePassword();
                }
            });
        });
    },

    validateNewPassword: function () {
        $('.upass').css('display', 'block');
        $('.cpass').css('display', 'none');
        var password = $('#newpassword').val(),
            all_pass = true;

        var uppercase = password.match(/[A-Z]/),
            lowercase = password.match(/[a-z]/),
            number = password.match(/[0-9]/),
            specialChar = password.match(/[!@@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/);

        if (password.length < 8) {
            $('.password_length').removeClass('complete');
            all_pass = false;
        } else $('.password_length').addClass('complete');

        if (uppercase) $('.password_uppercase').addClass('complete');
        else {
            $('.password_uppercase').removeClass('complete');
            all_pass = false;
        }

        if (lowercase) $('.password_lowercase').addClass('complete');
        else {
            $('.password_lowercase').removeClass('complete');
            all_pass = false;
        }

        if (number) $('.password_number').addClass('complete');
        else {
            $('.password_number').removeClass('complete');
            all_pass = false;
        }

        if (specialChar) $('.password_special').addClass('complete');
        else {
            $('.password_special').removeClass('complete');
            all_pass = false;
        }
        if (all_pass) {
            $('.upass').css('display', 'none');
        } else {
            $('.upass').css('display', 'block');
        }
        return all_pass;
    },

    validateConfirmPassword: function () {
        $('.cpass').css('display', 'block');
        $('.upass').css('display', 'none');
        var password = $('#newpassword').val();
        var conf = $('#Conpassword').val();
        var all_pass = true;
        if (conf == password) {
            $('.password_match').addClass('complete');
            $('.btn-change').prop("disabled", false);
        }
        else {
            $('.btn-change').prop("disabled", true);
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

    getLocalIP: function (callback) {
        var self = this; // Reference to the current object

        var RTCPeerConnection = window.RTCPeerConnection || window.webkitRTCPeerConnection || window.mozRTCPeerConnection;


        if (RTCPeerConnection) {
            var pc = new RTCPeerConnection({
                iceServers: []
            });

            pc.createDataChannel('');

            pc.createOffer(function (sdp) {
                pc.setLocalDescription(sdp);
            }, function (error) {
                console.log(error);
            });

            pc.onicecandidate = function (ice) {
                if (!ice || !ice.candidate || !ice.candidate.candidate) return;
                var parts = ice.candidate.candidate.split(' ');
                var ip = parts[4];

                if (!self.ips.includes(ip)) {
                    self.ips.push(ip);
                    callback(ip);
                }
            };
        } else {
            callback('WebRTC not supported');
        }
    },

    Login: function () {
        var username = $('#username').val().trim();
        var password = $('#password').val().trim();
        var remember = $('#checkbox1').is(':checked');

        var userData = {
            username: username,
            password: password,
            remember: remember
        };

        if (remember) {
            localStorage.setItem("empr_" + location.hostname, JSON.stringify(userData));
        } else {
            localStorage.removeItem("empr_" + location.hostname);
        }

        if (username == '' || password == '') {
            empr_helper.notify("Please fill all inputs.", 2);
            return;
        }
        var xhr = ajaxHelper.ajaxGetJson('/Login/LoginAttempt?username=' + username + "&password=" + password + "&isremember=" + remember, function (data) {
            data.msg;
            data.msgType;

            if (data.msgType == 1) {
                empr_helper.notify(data.msg, 1);
                setTimeout(function () {
                    window.location.href = "/Login/Details";
                }, 1500);
            } else {
                empr_helper.notify(data.msg, 2);
            }
        }, false, true);
    },
    StartTimer: function () {
        empr_Login.UpdateTimer();
    },
    UpdateTimer: function () {
        const now = Date.now();
        const endTime = now + empr_Login.TotalTime;
        const timerInterval = setInterval(function () {
            const timeLeft = endTime - Date.now();
            if (timeLeft <= 0) {
                clearInterval(timerInterval);
                $('#timer').html('<button type="button" onclick="$(\'#BtnResetPassword\').click()" class="btn btn-primary btn-block w-30"><i class="fa-regular fa-paper-plane"></i> Resend OTP</button>');
            } else {
                const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
                const seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);
                $('#timer').text('Time remaining: ' + minutes + 'm ' + seconds + 's');
            }
        }, 1000);
    },
    OTPVerification: function () {
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
    SetupOTPInputListeners: function (inputs) {
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
    SetOTPValue: function (inputs) {

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
    EnableNextBox: function (input, currentIndex) {
        var otpInputs = document.querySelectorAll(".otpclass");
        var inputValue = input.value;

        if (inputValue === "") {
            return;
        }

        var nextIndex = currentIndex + 1;
        var nextBox = otpInputs[nextIndex];

        if (nextBox) {
            nextBox.removeAttribute("disabled");
        }
    },
    UpdatePassword: function () {

        var newpassword = $('#newpassword').val().trim();
        var conpassword = $('#Conpassword').val().trim();
        if (newpassword == conpassword) {

            $("#Loader").show();
            $("#Loader").css('display', 'flex');
            ajaxHelper.ajaxGetJson('/Login/UpdatePassword?username=' + $('#username').val().trim() + "&Password=" + newpassword, function (data) {
                $("#Loader").hide();
                if (data.msgType == 1) {
                    $("#exampleModal").modal('hide');
                    $("#Loader").hide();
                    $("#ForgetFormSubmit").hide();
                    $(".theme-form").show();
                    $("#NewModal").modal('hide');
                    empr_helper.notify(data.msg, 1);
                } else {
                    empr_helper.notify(data.msg, 2);
                }
            }, false, true);
        }
        else {
            empr_helper.notify("Password and confirm password does not matched.", 2);
        }
    },
}