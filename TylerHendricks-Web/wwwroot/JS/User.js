$(function () {

    GetTermsAndCondition('TermsAndConditions');

    GetTermsAndCondition('PrivacyPolicy');

    $('#checkbox').prop('checked', false);
    $('#btnSave').attr("disabled", true);
    $('#btnSave').addClass("btn btn-theme");
    $('#btnSave').removeClass("btn-theme2");

    $('#SignUpForm').submit(function () {
        if ($(this).valid() == false) {
            $('#checkbox').prop('checked', false);
            $('#btnSave').attr("disabled", true);
            $('#btnSave').addClass("btn btn-theme");
            $('#btnSave').removeClass("btn-theme2");
            if ($('#password').attr('aria-invalid') == 'true') {
                $('#password').val('');
                $('#confirm_password').val('');
            }
            if ($('#Email').val() == '') {
                $('#errorEmail').show();
            }
            if ($('#password').val() == '') {
                $('#errorPassword').children().first().html('Required');
                $('#errorPassword').show();
            }
            if ($('#confirm_password').val() == '') {
                $('#errorConfirmPassword').children().first().html('Required');
                $('#errorConfirmPassword').show();
            }
            $(this).find('.validation-summary-errors').find('ul').children(':contains(Required)').remove();
        } 
    });

    $(document).on('input', '#Email', function () {
        if ($(this).val() == '') {
            $('#errorEmail').show();
        } else {
            $('#errorEmail').hide();
        }
    });

    $(document).on('input', '#password', function () {
        if ($(this).val() == '') {
            $('#errorPassword').show();
        } else {
            $('#errorPassword').hide();
        }
    });

    $(document).on('input', '#confirm_password', function () {
        if ($(this).val() == '') {
            $('#errorConfirmPassword').show();
        } else {
            $('#errorConfirmPassword').hide();
        }
    });

});
/***********SignUp*****************/
function temCondHandler(event) {
    if (event.target.checked) {
        $('#btnSave').attr("disabled", false);
        $('#btnSave').removeClass("btn btn-theme");
        $('#btnSave').addClass("btn-theme2");
    }
    else {
        $('#btnSave').attr("disabled", true);
        $('#btnSave').removeClass("btn-theme2");
        $('#btnSave').addClass("btn btn-theme");
    }
}
function Validate() {
    var password = document.getElementById("password").value;
    var confirmPassword = document.getElementById("confirm_password").value;
    if (password != confirm_password) {
        Swal.fire({
            title: '',
            text: 'You first Passwords is not similar with 2nd password. Please enter same password in both',
            showClass: {
                backdrop: 'swal2-noanimation',
                popup: '',
                icon: ''
            },
            hideClass: {
                popup: '',
            }
        });
        return false;
    }
    return true;
}
function modalpopup() {

    $('#TermConditionModel').modal({
        backdrop: 'static'
    });
}
function modalpopup1() {
    $('#PrivacyPoliciesModel').modal({
        backdrop: 'static'
    });
}
function GetTermsAndCondition(doctype) {
    $.ajax({
        type: "GET",
        url: '' + PatientPortalUrl.TermsAndConditionUrl+'',
        data: { DocType: doctype},
        success: function (response) {
            if (response.succeeded) {
                if (doctype == 'TermsAndConditions') {
                    $('#TermsModelBody').html(response.data);
                }
                else if (doctype == 'PrivacyPolicy') {
                    $('#PrivacyModelBody').html(response.data);
                }
            }
        },
        error: function (error) {
        }
    });
}
/**********************************/

/************ChangePasswordStart********/

function RequestCompleteChangePass(data) {

}
function RequestSuccessChangePass(data) {
    if (data.succeeded) {
        document.getElementById("ChangesPasswordForm").reset();
        Swal.fire({
            title: 'success',
            icon: 'success',
            text: 'Password Changed!',
            showClass: {
                backdrop: 'swal2-noanimation',
                popup: '',
                icon: ''
            },
            hideClass: {
                popup: '',
            },
            confirmButtonText: `OK`,
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '' + PatientPortalUrl.PatientProfileUrl + '';
            }
        });
    }
    else {
        Swal.fire({
            title: '',
            icon: 'error',
            text: 'Current password is incorrect!',
            showClass: {
                backdrop: 'swal2-noanimation',
                popup: '',
                icon: ''
            },
            hideClass: {
                popup: '',
            }
        });
    }
}
function RequestFailureChangePass(data) {

}

/************ChangePasswordEnd**********/

function SetTimeZone() {
    let date = new Date();
    let timeZone = date.getTimezoneOffset();
    $('#hdnTimeZone').val(timeZone);
}
