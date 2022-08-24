$(function () {

});

function changeToogleOnOff(elements) {
    if ($(elements).siblings('.onoffswitch-checkbox').attr('checked') !='checked') {
        $(elements).siblings('.onoffswitch-checkbox').attr('checked','checked');
        $(elements)
            .find('.onoffswitch-inner')
            .html('ACTIVE')
            .removeClass('switch-off')
            .addClass('switch-on');
        $(elements)
            .find('.onoffswitch-switch')
            .removeClass('switch-right-off')
            .addClass('switch-right-on');
    }
    else {
        $(elements).siblings('.onoffswitch-checkbox').removeAttr('checked');
        $(elements)
            .find('.onoffswitch-inner')
            .html('INACTIVE')
            .removeClass('switch-on')
            .addClass('switch-off');
        $(elements)
            .find('.onoffswitch-switch')
            .removeClass('switch-right-on')
            .addClass('switch-right-off');
    }
    activeInactiveState(parseInt($(elements).parent().attr('sid')));
}

function activeInactiveState(currentStateId) {
    $.ajax({
        type: "POST",
        url: '' + AdminPortalUrl.ActiveInActiveStateUrl + '',
        data: { stateId: currentStateId },
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
        },
        error: function (response) {
        }
    });
}