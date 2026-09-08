var DisableView = function () {
    $('form').find("input[type='text'],input[type='password'],textarea,input[type='checkbox'],input[type='file'],input[type='email'],input[type='number'],input[type='date'],input[type='radio'],select").not('.hideShowPass').each(function () {
        $(this).attr("disabled", "disabled");
    });
    $('form').removeAttr("action");
}
var EnableDisableArea = function (area, object_type) {
    var main_context = $('form');
    if (area != "") {
        main_context = $(area);
    }
    if (object_type == "Disable") {
        main_context.find("submit,button,input[type='checkbox'],input[type='file'],input[type='email'],input[type='number'],input[type='date'],input[type='radio'],select").each(function () {
            $(this).attr("disabled", "disabled");
        });
        main_context.find("input[type='text'],input[type='password'],textarea").each(function () {
            $(this).attr("readonly", "readonly");
        });
        main_context.find("a").each(function () {
            $(this).addClass("disabled");
        });
    } else {
        main_context.find("input[type='text'],input[type='password'],textarea").each(function () {
            $(this).removeAttr("readonly");
        });
        main_context.find("a").each(function () {
            $(this).removeClass("disabled");
        });
        main_context.find("submit,button,input[type='checkbox'],input[type='file'],input[type='email'],input[type='number'],input[type='date'],input[type='radio'],select").each(function () {
            $(this).removeAttr("disabled");
        });
    }
}
//var formLoader = "#Loader";
onFormSubmitBegin = function () {
    EnableDisableArea("", "Disable");
}
onFormSubmitComplete = function () {

}
onFormFailure = function (response) {
    EnableDisableArea("", "Enable");
    toastr.error(response.message, "Error", {
        timeOut: "5000"
    });
}
onFormSuccess = function (response) {
    var responseTypeArray = response.type.split('-');
    if (response.success == true) {
        className = "text-success";
    } else {
        className = "text-danger";
    }
    $.each(responseTypeArray, function (key, value) {
        if (value == "M") {
            toastr.success(response.message, "Success", {
                timeOut: "5000"
            });
        } else if (value == "R") {
            $('form')[0].reset();
            $(".hidea-area").hide();
            if ($(".select2-zero").length > 0) {
                $(".select2-zero").select2("val", "0");
                $(".select2-zero").trigger('change.select2');
            } else if ($(".select2-hidden-accessible").length > 0) {
                $(".select2-hidden-accessible").select2("val", "");
                $(".select2-hidden-accessible").trigger('change.select2');
            }
        } else if (value == "T") {
            //window.location = response.targetURL;
        } else if (value == "TD") {
            setTimeout(function () {
                //window.location = response.targetURL;
            }, 2000);
        } else if (value == "RL") {
            //window.location.reload();
        } else if (value == "RLD") {
            setTimeout(function () {
               // window.location.reload();
            }, 2000);
        } else if (value == "D") {
            toastr.success(response.message, "Success", {
                timeOut: "5000"
            });
        } else if (value == "MN") {
            var active = $('.nav-tabs-setup li.active');
            if (response.success == true) {
                if (response.fieldName != "Exit") {
                    var nextActive = active.next().removeClass('disabled').addClass('active');
                    nextTab(nextActive);
                    toastr.success(response.message, "Success", {
                        timeOut: "5000"
                    });
                } else {
                    toastr.success(response.message, "Success", {
                        timeOut: "5000"
                    });
                    setTimeout(function () {
                        //window.location.reload();
                    }, 1000);
                }
                $('.nav-tabs-setup li[aria-expanded="false"]').addClass('active');
            } else {
                $('#btnSubmit').show();
                toastr.error(response.message, "Error", {
                    timeOut: "5000"
                });
            }
        }
    });
    EnableDisableArea("", "Enable");
}
jQuery(function ($) {
    //$(".CnicFormat").mask("99999-9999999-9");
    $("#date").mask("99/99/9999");
    $(".ContactNo").mask("9999-9999999");
    $(".OTP").mask("9-9-9-9-9");
    $(".cnic").mask("99999-9999999-9");
    $(".pay").mask("9999");
    $(".Telephone").mask("+92 999 999-999");
    $("#ext").mask("(999) 999-9999? Ext.99999");
    $("#mobile").mask("+99 999 999 999");
    $("#percent").mask("99%");
    $("#productkey").mask("a*-999-a999");
    $("#orderno").mask("PO: aaa-999-***");
    $("#date2").mask("99/99/9999", { autoclear: false });
    $("#date3").mask("99/99/9999", {
        autoclear: false,
        completed: function () {
            alert("Completed!");
        }
    });
    $("#mobile2").mask("+1 999 999 999");
    $('input').on('keypress', function (event) {
        const restrictedCharacters = ["'", ":"];
        const enteredChar = String.fromCharCode(event.which);

        if (restrictedCharacters.includes(enteredChar)) {
            event.preventDefault();
        }
    });
});
$(function () {
    $('select').select2({ width: '100%' });
    $('.select2-search__field').focus();
});
$(document).on('select2:open', () => {
    document.querySelector('.select2-search__field').focus();
});
$(function () {
    $(".datepicker").datepicker({
        dateFormat: "dd-MM-yy",
        altFormat: "DD, d MM, yy"
    });
});
//$(document).on('select2:open', () => {
//    document.querySelector('.select2-search__field').focus();
//});
$(function () {
    statusApp = (statusApp || '').toString().trim().toUpperCase();
    console.log('statusApp :', statusApp);
    $('#ASTATUS').dxSelectBox({
        dataSource: [
            { value: 'Y', text: 'Active' },
            { value: 'N', text: 'In-Active' }
        ],
        valueExpr: 'value',
        displayExpr: 'text',
        searchEnabled: true,
        width: '100%',
        placeholder: 'Search ......!',
        showClearButton: true,
        disabled: statusApp === 'N',
        dropDownOptions: {
            height: 'auto',
        },
        pagingEnabled: true,
        searchTimeout: 500,
        onInitialized: function (e) {
            e.component.option('value', statusApp);
        }
    });
    $('#AM').dxSelectBox({
        dataSource: [
            { value: 'A', text: 'Auto' },
            { value: 'M', text: 'Manual' }
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
        onInitialized: function (e) {
            e.component.option('value', 'A');
        }
    });
    $('#STYPE').dxSelectBox({
        dataSource: [
            { value: 'I', text: 'Stock In' },
            { value: 'O', text: 'Stock Out' }
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
        onInitialized: function (e) {
            e.component.option('value', 'I');
        }
    });
    $('#BTYPE').dxSelectBox({
        dataSource: [
            { value: 'A', text: 'Advance' },
            { value: 'C', text: 'Cash' },
            { value: 'CR', text: 'Credit' }
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
        onInitialized: function (e) {
            e.component.option('value', 'CR');
        }
    });
});
function USDConvertPKR(USDRate, USDAmount) {
    let PKR = parseFloat(parseFloat(USDRate) * parseFloat(USDAmount)).toFixed(2);
    return isNaN(PKR) ? 0 : PKR;
}
$('#AmountUsd').keyup(function () {
    let PKR = USDConvertPKR($('#UsdRate').val(), $('#AmountUsd').val());
    $('#AmountPkr').val(PKR);
    $('#ConversionAmount').val(PKR);
});
$('#UsdRate').keyup(function () {
    if ($(this).val() > 0) {
        $('#AmountUsd').attr('readonly', false);
    }
    else {
        $('#AmountUsd').attr('readonly', true);
    }
    if ($('#AmountUsd').val() > 0) {
        let PKR = USDConvertPKR($('#UsdRate').val(), $('#AmountUsd').val());
        $('#AmountPkr').val(PKR);
        $('#ConversionAmount').val(PKR);
    }
});
function ShowPassword(elem, cur) {
    if (elem.attr("type") === "password") {
        $(cur).find("i").toggleClass("fa-eye-slash fa-eye");
        elem.attr("type", "text");
    }
    else {
        $(cur).find("i").toggleClass("fa-eye fa-eye-slash");
        elem.attr("type", "password");
    }
}
$('.decimal').on('input', function () {
    let inputValue = $(this).val().replace(/[^0-9.]/g, '');
    let floatValue = parseFloat(inputValue).toFixed(2);
    $(this).val(floatValue);
});
function limitNumberLength(input, maxLength) {
    debugger
    if (input.value.length > maxLength) {
        input.value = input.value.slice(0, maxLength);
    }
}