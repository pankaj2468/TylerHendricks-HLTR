$(function () {
    
    $('[rel="tooltip"]').tooltip();

    $(document).find('#ddlManualConsultationType').each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        $customSelect.text($this.children('option').eq(0).text());

        var $optionlist = $('<ul />', { 'class': 'select-options', 'style': 'display:none' }).insertAfter($customSelect);

        for (var i = 0; i < selectOptions; i++) {
            $('<li />', {
                text: $this.children('option').eq(i).text(),
                rel: $this.children('option').eq(i).val()
            }).appendTo($optionlist);
        }

        var $optionlistItems = $optionlist.children('li');

        $customSelect.click(function (e) {
            e.stopPropagation();
            $('div.custom-select.active').not(this).each(function () {
                $(this).removeClass('active').next('ul.select-options').hide();
            });
            $(this).toggleClass('active').next('ul.select-options').slideToggle();
        });

        $optionlistItems.click(function (e) {
            e.stopPropagation();
            $customSelect.text($(this).text()).removeClass('active');
            $this.val($(this).attr('rel'));
            $optionlist.hide();
            $this.change();
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });

    $(document).on('click touchstart', '.custom-select-consultation', function (event) {
        $(this).next('ul').toggle();
    });  

    $(document).on('click touchstart', '#btnManualQuestionNaireContinue', function (event) {
        var IsValid = true;
        var $emailField = $(document).find('#txtManualEmailAddress');
        var $consultationField = $(document).find('#ddlManualConsultationType');
        var $errorField = $(document).find('#spanErrorManualQuestionNaire');
        if ($emailField.val() == '' || $consultationField.val() == '') {
            IsValid = false;
            $errorField.html('All field are mandatory!');
        }
        else if (!validateEmail($emailField.val())) {
            IsValid = false;
            $errorField.html('Enter valid email adress!');
        }
        else {
            $errorField.empty();
        }
        if (IsValid) {
            if ($consultationField.val() == "1") {
                window.location.href = '' + PhysicianPortalUrl.ManualEDUrl + '?email=' + encodeURI($emailField.val());
            } else if ($consultationField.val() == "2") {
                window.location.href = '' + PhysicianPortalUrl.ManualMedicalRefillUrl + '?email=' + encodeURI($emailField.val());
            } else if ($consultationField.val() == "3") {
                window.location.href = '' + PhysicianPortalUrl.ManualHairLossUrl + '?email=' + encodeURI($emailField.val());
            } else {
                $errorField.html('First select consultation type!');
            }
        }
    });
});

function validateEmail($email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test($email);
}