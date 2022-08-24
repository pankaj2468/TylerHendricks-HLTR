$(function () {
    $('#EditEmail').on('click', function () {
        $('.step1').find('input[type=submit]').parent().removeClass('hide-select');
        $('.step1').find('input[type=email]').removeAttr('readonly');
        $('.step1').find('#EditEmail').addClass('hide-select');
        $('.step2').addClass('hide-select');
        $('#OTP').val('');
    });
    $('#UpdateEmailForm').submit(function () {
        if ($('.step1:nth-child(2)').hasClass('hide-select') === false && $('.step2:nth-child(1)').hasClass('hide-select') === false) {
            if ($('#Email').val() == '') {
                return false;
            }
        }
    });
});

function Numeric(input) {
    input.value = input.value.replace(/[^\d]/, '')
}

function RequestCompleteEmail(response) {
   
}

function RequestSuccessEmail(response) {
    if (response.succeeded) {
        var jsonObject = response.data;
        if (jsonObject.isValidEmail == true && (jsonObject.otp == null || jsonObject.otp == '')) {
            Swal.fire({
                title: 'Message',
                text: 'A verification message is successfully sent to your email address',
                icon: 'success',
                showClass: {
                    backdrop: 'swal2-noanimation',
                    popup: '',
                    icon: ''
                },
                hideClass: {
                    popup: '',
                }
            });
            $('.step1').find('input[type=submit]').parent().addClass('hide-select');
            $('.step1').find('input[type=email]').attr('readonly', 'readonly');
            $('.step1').find('#EditEmail').removeClass('hide-select');
            $('.step2').removeClass('hide-select');
        }
        else if (jsonObject.isOTPValid == true && jsonObject.otp != '' && jsonObject.error.status != '') {
            Swal.fire({
                title: 'Message',
                text: 'Your email address is successfully updated.',
                icon: 'success',
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
                    window.location.href = '' + PatientPortalUrl.LoginUrl + '';
                }
            });

            setTimeout(function () { window.location.href = '' + PatientPortalUrl.LoginUrl + ''; }, 10000);
        }

        if (jsonObject.error.email != '') {
            $('#Email').next('span').show();
            $('#Email').next('span').html(jsonObject.error.email);
        }
        if (jsonObject.error.otp != '') {
            $('#OTP').next('span').show();
            $('#OTP').next('span').html(jsonObject.error.otp);
        }
    }
}

function RequestFailureEmail(response) {

}
