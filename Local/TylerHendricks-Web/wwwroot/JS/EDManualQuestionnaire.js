$(function () {
    $(document).find('#dropdownState').each(function () {
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
    $(document).find('#dropdownPharmacyState').each(function () {
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
    $(document).find('#dropdownAccountState').each(function () {
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
    $('.IsPhoneNumber').blur(function (e) {
        var x = e.target.value.replace(/\D/g, '').match(/(\d{3})(\d{3})(\d{4})/);
        if (x == null) {
            $(this).next('span').show();
        }
        else {
            e.target.value = '(' + x[1] + ') ' + x[2] + '-' + x[3];
            $(this).next('span').hide();
        }
    });
});

function checkEmpty(element) {
    if ($(element).val() == '') {
        $(element).next('span').show();
    } else {
        $(element).next('span').hide();
    }
}

function Numeric(input) {
    input.value = input.value.replace(/[^\d]/, '')
}