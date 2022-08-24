$(function () {
    $('.btn-delivery-option').click(function (event) {
        $('[data-valmsg-for=isHomeDelivery]').empty();
        $('.btn-delivery-option').removeClass('theme-bg-14').addClass('theme-bg-13');
        $(this).removeClass('theme-bg-13').addClass('theme-bg-14');
        $('#hdnHomeDelivery').val($(this).attr('data-value'));
        $(this).css("outline", "none");
    });
    //$('#frmSelectDelivery').submit(function (event) {
    //    var deliveryOption = $('#hdnHomeDelivery').val();
    //    if (deliveryOption == undefined || deliveryOption == '' || deliveryOption == null) {
    //        $('#lblRequiredFieldForDeliveryOption').html('Please select an option.');
    //        return false;
    //    } else {
    //        $('#lblRequiredFieldForDeliveryOption').empty();
    //    }
    //});
});