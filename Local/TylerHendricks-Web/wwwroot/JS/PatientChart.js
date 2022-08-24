var chatMessage = {
    RequestSelfie: "Hello. A new \"selfie\" photo is needed in order for us to proceed with your consultation. Providers need to see who they are treating, just as they would in an in-person doctor's office. Please upload a close-up, front view of your entire face including, your eyes (from the top of your forehead to the bottom of your chin). When we receive your updated photo we will proceed with your consultation. Thank you.",
    RequestPhoto: "We will need a clear photo of your government issued ID to complete your consultation. We are required to verify your identity in order to treat you. This is due to state telemedicine regulations. It’s similar to being asked for an ID at the front desk of an in-person practice. We take the privacy and security of your health and personal information seriously. All of your photos, including your ID, are stored safely and only used for your medical care here. Once you have provided this clear ID photo, we will proceed with your consultation.",
    RequestMedicine: "We are unable to complete your consultation with your currently submitted photos. Please provide an additional photo of your medication bottle / scalp to proceed with your consultation."
};

$(function () {
    var $consultationId = $(document).find('#ConsultationId');
    $(document).on('click', '#btnSubmitChat', function () {
        var $messageBox = $(document).find('#txtChatBox');
        var _message = $messageBox.val();
        var _patientid = $(document).find('#PatientId').val();
        var _consultationId = $(document).find('#ConsultationId').val();
        if (_message != '' && _message != undefined) {
            $.ajax({
                type: 'POST',
                url: '' + PhysicianPortalUrl.RetakeRequestUrl + '',
                data: { PatientId: _patientid, ConsultationId: _consultationId, Message: _message, Action: $(this).attr('do') },
                dataType: 'json',
                beforeSend: function () { $('#Loader').show(); },
                complete: function () { $('#Loader').hide(); },
                success: function (response) {
                    var obj = JSON.parse(response);
                    if (obj.status) {
                        window.location.href = window.location.origin + window.location.pathname + '?' + obj.url;
                    } else {
                        window.location.href = '' + PhysicianPortalUrl.PhysicianDashboardUrl + '';
                    }
                },
                error: function () { }
            });
        } else {
            $messageBox.css({ 'border': '1px solid red' });
            $messageBox.focus();
        }
    });
    $(document).on('input', '#txtChatBox', function () {
        if ($(this).val().length > 0) {
            $(this).css({ 'border': '' });
            $(this).focus();
        }
        else {
            $(this).css({ 'border': '1px solid red' });
            $(this).focus();
        }
    });
    $(document).on('click', '#BtnPrescribe', function () {
        Swal.fire({
            title: 'Are you sure?',
            text: '',
            icon: 'warning',
            showClass: {
                backdrop: 'swal2-noanimation',
                popup: '',
                icon: ''
            },
            hideClass: {
                popup: '',
            },
            showCancelButton: true,
            confirmButtonColor: '#0bc3bf',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.isConfirmed) {
                $(document).find('#BtnComplete').removeAttr('disabled');
                $(document).find('#BtnComplete').attr('status', '4');
                $(document).find('#BtnComplete').removeClass('complBtn').addClass('prescribeBtn');
                $(document).find('#DivAlert').find('div:first').html('<strong>Alerts:</strong> Prescribed');
            }
        });
        
    });
    $(document).on('click', '#BtnComplete', function () {
        var statusid = $(this).attr('status');
        if (statusid != '0' && statusid != '' && statusid != undefined) {
            var _patientid = $(document).find('#PatientId').val();
            var _consultationId = $(document).find('#ConsultationId').val();
            $.ajax({
                type: 'POST',
                url: '' + PhysicianPortalUrl.UpdateOrderStatusUrl + '',
                data: { PatientId: _patientid, ConsultationId: _consultationId, StatusId: parseInt(statusid) },
                dataType: 'json',
                beforeSend: function () { $('#Loader').show(); },
                complete: function () { $('#Loader').hide(); },
                success: function (response) {
                    var obj = JSON.parse(response);
                    if (obj.status) {
                        window.location.href = window.location.origin + window.location.pathname + '?' + obj.url;
                    } else {
                        window.location.href = '' + PhysicianPortalUrl.PhysicianDashboardUrl + '';
                    }
                },
                error: function () { }
            });
        } else {
            GenericSimpleBox('Please first click on Prescribe/Deny button!');
        }     
    });
    $(document).on('click', '#BtnDeny', function () {
        Swal.fire({
            title: 'Are you sure?',
            text: '',
            icon: 'warning',
            showClass: {
                backdrop: 'swal2-noanimation',
                popup: '',
                icon: ''
            },
            hideClass: {
                popup: '',
            },
            showCancelButton: true,
            confirmButtonColor: '#0bc3bf',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.isConfirmed) {
                $(document).find('#BtnComplete').removeAttr('disabled');
                $(document).find('#BtnComplete').attr('status', '5');
                $(document).find('#BtnComplete').removeClass('complBtn').addClass('prescribeBtn');
                $(document).find('#DivAlert').find('div:first').html('<strong>Alerts:</strong> Denied');
            }
        });
    });
    $(document).on('change', '#PhysicianNoteUpload', function () {
        var UserId = $(document).find('#PatientId').val();
        if (this.files && this.files[0]) {
            var data = new FormData();
            jQuery.each(this.files, function (i, file) {
                data.append(UserId, file);
            });
            jQuery.ajax({
                url: '' + PhysicianPortalUrl.UploadNoteUrl + '',
                type: 'POST',
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () { $('#Loader').show(); },
                complete: function () { $('#Loader').hide(); },
                success: function (data) {
                    NextPatient($consultationId.val());
                }
            });
        } else {

        }
    });
    $(document).on('click', '#btnSaveNotes', function () {
        var txtnotes = $(document).find('#txtNotes').val();
        var _patientid = $(document).find('#PatientId').val();
        $.ajax({
            type: 'POST',
            url: '' + PhysicianPortalUrl.SaveNoteUrl + '',
            data: { PatientId: _patientid, Notes: txtnotes },
            dataType: 'json',
            beforeSend: function () { $('#Loader').show(); },
            complete: function () { $('#Loader').hide(); },
            success: function (response) {
                NextPatient($consultationId.val());
            },
            error: function () { }
        });
    });
    $(document).on('click', '#btnSaveMedicine', function () {
        var medicines = new Array();
        $('#DivAddDynamicMedication').children().slice(1).each(function () {
            if ($(this).find('button').html().trim().toLowerCase() == 'remove') {
                var drugName = $(this).find('.md-drugname').val();
                var dose = $(this).find('.md-dose').val();
                var unit = parseInt($(this).find('.md-unit').val());
                var form = parseInt($(this).find('.md-form').val());
                var frequency = parseInt($(this).find('.md-frequency').val());
                var medicalCondition = $(this).find('.md-medical-condition').val();
                var obj = { DrugName: drugName, Dose: dose, UnitId: unit, FormId: form, FrequencyId: frequency, MedicalCondition: medicalCondition };
                medicines.push(obj);
            }
        });
        if (medicines.length > 0) {
            $.ajax({
                type: 'POST',
                url: '' + PhysicianPortalUrl.UpdateMedicineUrl + '',
                data: { MedicineList: medicines, patientId: $('#PatientId').val(), consultationId: $('#ConsultationId').val()},
                beforeSend: function () { $('#Loader').show(); },
                complete: function () { $('#Loader').hide(); },
                success: function (response) {
                    NextPatient($consultationId.val());
                    $("a[data-target='#MedicineModel']").click();
                },
                error: function (response) {
                }
            });
        }
    });
    $(document).on('click', '#btnMoveToAllPatient', function () {
        var token = $(document).find('#formPage').find('input[name="__RequestVerificationToken"]').val();
        var ConsultationId = $(document).find("#ConsultationId").val();
        $.ajax({
            type: 'POST',
            url: '' + PhysicianPortalUrl.MoveToAllUrl + '',
            data: {
                __RequestVerificationToken: token,
                consultationId: ConsultationId
            },
            dataType: 'JSON',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            beforeSend: function () { $('#Loader').show(); },
            complete: function () { $('#Loader').hide(); },
            success: function (response) {
                window.location.reload();
            },
            error: function (response) {
            }
        });
    });

    var swiper = new Swiper(".mySwiper", {
        pagination: {
            el: ".swiper-pagination",
            type: "fraction",
        },
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        },
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

    $(".IsPhoneNumber").keypress(function (event) {
        // Backspace, tab, enter, end, home, left, right
        // We don't support the del key in Opera because del == . == 46.
        if (isNaN($(event.target).prop('value')) == false) {
            if ($(event.target).prop('value').length >= 10) {
                if (event.keyCode != 32) { return false }
            }
        }
        var controlKeys = [8, 9, 13, 35, 36, 37];
        // IE doesn't support indexOf
        var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
        // Some browsers just don't raise events for control keys. Easy.
        // e.g. Safari backspace.
        if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
            (48 <= event.which && event.which <= 57) || // Always 1 through 9
            isControlKey) { // Opera assigns values for control keys.
            return;
        } else {
            event.preventDefault();
        }
    });

    getMedicationDropdown();
});

function LetterOnlyWithSpaces(input) {
    input.value = input.value.replace(/[^a-zA-Z\s]*$/, '')
}

function NumericWithDots(input) {
    input.value = input.value.replace(/[^\d]/, '')
}

function getMedicationDropdown($mmunit, $mmform, $mmfrequency) {
    var $munit = $mmunit;
    var $mform = $mmform;
    var $mfrequency = $mmfrequency;
    bindMedicationDropDown($munit, $mform, $mfrequency);
}

function NextPatient(consultationId) {
    $('#Loader').show();
    $('#PatientChart').load('' + PhysicianPortalUrl.GetPatientChartUrl + '?ConsultationId=' + consultationId, function (responseTxt, statusTxt, xhr) {
        if (statusTxt == "success")
            $('#Loader').hide();
        if (statusTxt == "error")
            $('#Loader').hide();
    });
}

function SetRequest(action) {
    $(document).find('#btnSubmitChat').attr('do', action);
    if (action == 'SE') {
        $(document).find('#txtChatBox').val(chatMessage.RequestSelfie);
    } else if (action == 'MI') {
        $(document).find('#txtChatBox').val(chatMessage.RequestMedicine);
    } else if (action == 'PI') {
        $(document).find('#txtChatBox').val(chatMessage.RequestPhoto);
    }
}

function AccountStatus(patientid, consultationid) {
    $.ajax({
        type: 'POST',
        url: '' + PhysicianPortalUrl.ChangeAccountStatusUrl + '',
        data: { PatientId: patientid },
        dataType: 'json',
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
            var consultationId = $(document).find('#ConsultationId').val();
            NextPatient(consultationId);
        },
        error: function () { }
    });
}

function ImgErrorVideo(source) {
    source.src = "/images/no-thumbnail.jpg";
}

function GenericSimpleBox(Title,Text,Icon) {
    Swal.fire({
        title: Title,
        text: Text,
        icon: Icon,
        showClass: {
            backdrop: 'swal2-noanimation',
            popup: '',
            icon: ''
        },
        hideClass: {
            popup: '',
        },
        confirmButtonColor: '#0bc3bf'
    });
}

function DeleteNote(ctrlId) {
    $.ajax({
        type: 'POST',
        url: '' + PhysicianPortalUrl.DeleteNoteUrl + '',
        data: { Id: ctrlId },
        dataType: 'json',
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
            var consultationId = $(document).find('#ConsultationId').val();
            NextPatient(consultationId);
        },
        error: function () { }
    });
}

function ViewRxImage(element) {
    $('#RxModel').find('img').attr('src',$(element).find('img').attr('src'));
}

function ViewChatImage(element) {
    $('#ChatBoxModel').find('img').attr('src', $(element).find('img').attr('src'));
}

function populatePatientData() {
    $.ajax({
        type: "GET",
        url: '' + PhysicianPortalUrl.PopulatePatientDataUrl + '',
        data: { patientId: $('#PatientId').val() },
        dataType: 'json',
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
            var item = JSON.parse(response);
            if (item != undefined && item != null) {
                $('#hdnPatientId').val($('#PatientId').val());
                $('#txtFirstName').val(item.FirstName);
                $('#txtLastName').val(item.LastName);
                $('#txtDob').val(item.DobToLocal);
                $('#txtPhoneNumber').val(item.PhoneNumber);
                $('#txtZipCode').val(item.ZipCode);
                $('#ddlState').val(item.StateId);
            }
            patientStateDropdown();
        },
        error: function (response) {

        }
    });
}

function populatePharmacyData() {
    $.ajax({
        type: "GET",
        url: '' + PhysicianPortalUrl.PopulatePharmacyDataUrl + '',
        data: { consultationId: $('#ConsultationId').val() },
        dataType: 'json',
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
            var item = JSON.parse(response);
            if (item != undefined && item != null) {
                $('#hdnPharmacyId').val(item.PharmacyId);
                $('#txtPharmacyName').val(item.PharmacyName);
                $('#txtPhPhoneNumber').val(item.PhoneNumber);
                $('#txtAddressLine1').val(item.AddressLine1);
                $('#txtAddressLine2').val(item.AddressLine2);
                $('#txtCity').val(item.City);
                $('#ddlPhState').val(item.StateId);
                $('#txtPhZip').val(item.ZipCode);
            }
            pharmacyStateDropdown();
        },
        error: function (response) {

        }
    });
}

function populateMedicineData() {
    $.ajax({
        type: "GET",
        url: '' + PhysicianPortalUrl.PopulateMedicineDataUrl + '',
        data: { consultationId: $('#ConsultationId').val() },
        dataType: 'json',
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
            fillOutMedicineData(response);
        },
        error: function (response) {

        }
    });
}

/**  Patient Update Functions  Start**/
function RequestCompletePatientUpdate(response) {

}

function RequestSuccessPatientUpdate(response) {
    Swal.fire({
        title: 'Alert',
        text: 'Patient details has been successfully update.',
        icon: 'success',
        showClass: {
            backdrop: 'swal2-noanimation',
            popup: '',
            icon: ''
        },
        hideClass: {
            popup: '',
        },
        confirmButtonColor: '#0bc3bf',
        confirmButtonText: 'Okay'
    }).then((result) => {
        if (result.isConfirmed) {
          
        }
    });
    var consultationId = $(document).find('#ConsultationId').val();
    NextPatient(consultationId);
    $("a[data-target='#PatientModel']").click();
}

function RequestFailurePatientUpdate(response) {

}
/**  Patient Update Functions  End**/

/**  Pharmacy Update Functions  Start**/
function RequestCompletePharmacyUpdate(response) {

}

function RequestSuccessPharmacyUpdate(response) {
    Swal.fire({
        title: 'Alert',
        text: 'Pharmacy details has been successfully update.',
        icon: 'success',
        showClass: {
            backdrop: 'swal2-noanimation',
            popup: '',
            icon: ''
        },
        hideClass: {
            popup: '',
        },
        confirmButtonColor: '#0bc3bf',
        confirmButtonText: 'Okay'
    }).then((result) => {
        if (result.isConfirmed) {
            
        }
    });
    var consultationId = $(document).find('#ConsultationId').val();
    NextPatient(consultationId);
    $("a[data-target='#PharmacyModel']").click();
}

function RequestFailurePharmacyUpdate(response) {

}
/**  Pharmacy Update Functions  End**/
function bindMedicationDropDown(unit, form, frequency) {
    if (unit == undefined) {
        unit = $('#DivAddDynamicMedication').children().first().find('select.md-unit');
    }
    if (form == undefined) {
        form = $('#DivAddDynamicMedication').children().first().find('select.md-form');
    }
    if (frequency == undefined) {
        frequency = $('#DivAddDynamicMedication').children().first().find('select.md-frequency');
    }
   
    var unitArr = JSON.parse($('#hdMedicineUnitList').val());
    $(unitArr).each(function (key, value) {
        $(unit).append('<option value="' + unitArr[key].Id + '">' + unitArr[key].Name + '</option>');
    });

    $(form).empty();
    var formArr = JSON.parse($('#hdMedicineFormList').val());
    $(formArr).each(function (key, value) {
        $(form).append('<option value="' + formArr[key].Id + '">' + formArr[key].Name + '</option>');
    });

    $(frequency).empty();
    var frequrencyArr = JSON.parse($('#hdMedicineFrequencyList').val());
    $(frequrencyArr).each(function (key, value) {
        $(frequency).append('<option value="' + frequrencyArr[key].Id + '">' + frequrencyArr[key].Name + '</option>');
    });

    $(unit).each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        $customSelect.text($this.children('option').eq(0).text());

        var $optionlist = $('<ul />', {
            'class': 'select-options'
        }).insertAfter($customSelect);

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
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });

    $(form).each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        $customSelect.text($this.children('option').eq(0).text());

        var $optionlist = $('<ul />', {
            'class': 'select-options'
        }).insertAfter($customSelect);

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
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });

    $(frequency).each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        $customSelect.text($this.children('option').eq(0).text());

        var $optionlist = $('<ul />', {
            'class': 'select-options'
        }).insertAfter($customSelect);

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
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });
}

function medicationDoseRow(current) {
    var btnText = current.innerHTML.trim();
    if (btnText == 'Add') {
        var $drugname = $(current).parent().find('.md-drugname');
        var $dose = $(current).parent().find('.md-dose');
        var $medication = $(current).parent().find('.md-medical-condition');
        if ($drugname.val() == '' && $dose.val() == '' && $medication.val() == '') {
            $drugname.next('span').show();
            $dose.next('span').show();
            $medication.next('span').show();
        }
        else if ($drugname.val() == '' && $dose.val() != '' && $medication.val() != '') {
            $drugname.next('span').show();
            $dose.next('span').hide();
            $medication.next('span').hide();
        }
        else if ($drugname.val() != '' && $dose.val() == '' && $medication.val() != '') {
            $drugname.next('span').hide();
            $dose.next('span').show();
            $medication.next('span').hide();
        }
        else if ($drugname.val() != '' && $dose.val() != '' && $medication.val() == '') {
            $drugname.next('span').hide();
            $dose.next('span').hide();
            $medication.next('span').show();
        }
        else if ($drugname.val() == '' && $dose.val() == '' && $medication.val() != '') {
            $drugname.next('span').show();
            $dose.next('span').show();
            $medication.next('span').hide();
        } else if ($drugname.val() == '' && $dose.val() != '' && $medication.val() == '') {
            $drugname.next('span').show();
            $dose.next('span').hide();
            $medication.next('span').show();
        } else if ($drugname.val() != '' && $dose.val() == '' && $medication.val() == '') {
            $drugname.next('span').hide();
            $dose.next('span').show();
            $medication.next('span').show();
        } else {
            $('#DivAddDynamicMedication').append('<div class="d-flex align-items-start justify-content-between flex-md-nowrap flex-wrap feilds mb-2 medicationdiv">'
                + '<button type="button" class="btn priBtn btndoseaddrow mt-0" onclick="medicationDoseRow(this)">Add</button>'
                + '<div class="formWrap">'
                + '<input class="form-control md-drugname" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter drug name"/>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '<input class="form-control md-dose" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter dosage"/>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '    <select class="form-control md-unit list-medication hide-select"></select>'
                + '</div>'
                + '<div class="formWrap">'
                + '     <select class="form-control md-form list-medication hide-select"></select>'
                + '</div>'
                + '<div class="formWrap">'
                + '     <select class="form-control md-frequency list-medication hide-select"></select>'
                + '</div>'
                + '<div class="formWrap">'
                + '    <input class="form-control md-medical-condition" maxlength="500" type="text" oninput="checkEmpty(this)" placeholder="Enter medical condition"/>'
                + '    <span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '</div>');
            current.innerHTML = 'Remove';
            var $mform = $('#DivAddDynamicMedication').children().last().find('select.md-form');
            var $munit = $('#DivAddDynamicMedication').children().last().find('select.md-unit');
            var $mfrequency = $('#DivAddDynamicMedication').children().last().find('select.md-frequency');
            getMedicationDropdown($munit, $mform, $mfrequency);
        }
    } else if (btnText == 'Remove') {
        Swal.fire({
            title: 'Are you sure?',
            text: 'Do you want to remove this record.',
            icon: 'warning',
            showClass: {
                backdrop: 'swal2-noanimation',
                popup: '',
                icon: ''
            },
            hideClass: {
                popup: '',
            },
            showCancelButton: true,
            confirmButtonColor: '#0bc3bf',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.isConfirmed) {
                $(current).parent().remove();
            }
        });
    }
}

function fillOutMedicineData(response) {
    var $medicineContainer = $('#DivAddDynamicMedication');
    $medicineContainer.children().not(':first').remove();
    if (response.succeeded) {
        if (response.data !== '[]') {
            var itemArray = JSON.parse(response.data);
            $(itemArray).each(function (key, value) {
                $('<div class="d-flex align-items-start flex-wrap justify-content-between flex-md-nowrap feilds mb-2 medicationdiv">'
                    + '<button type="button" class="btn priBtn btndoseaddrow mt-0" onclick="medicationDoseRow(this)">Remove</button>'
                    + '<div class="formWrap">'
                    + '    <input class="form-control md-drugname" value="' + itemArray[key].DrugName + '" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter drug name"/>'
                    + '    <span class="text-danger" style="display:none;">Required</span>'
                    + '</div>'
                    + '<div class="formWrap">'
                    + '    <input class="form-control md-dose" value="' + itemArray[key].Dose + '" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter dosage"/>'
                    + '    <span class="text-danger" style="display:none;">Required</span>'
                    + '</div>'
                    + '<div class="formWrap">'
                    + '      <div class="wrap"><select class="form-control md-unit list-medication"></select></div>'
                    + '</div>'
                    + '<div class="formWrap">'
                    + '     <div class="wrap"><select class="form-control md-form list-medication"></select></div>'
                    + '</div>'
                    + '<div class="formWrap">'
                    + '     <div class="wrap"><select class="form-control md-frequency list-medication"></select></div>'
                    + '</div>'
                    + '<div class="formWrap">'
                    + '    <input class="form-control md-medical-condition" value="' + itemArray[key].MedicalCondition + '" maxlength="500" type="text" oninput="checkEmpty(this)" placeholder="Enter medical condition"/>'
                    + '    <span class="text-danger" style="display:none;">Required</span>'
                    + '</div>'
                    + '</div>').insertAfter($medicineContainer.children().first());

                var $mform = $medicineContainer.children().eq(1).find('select.md-form');
                var $munit = $medicineContainer.children().eq(1).find('select.md-unit');
                var $mfrequency = $medicineContainer.children().eq(1).find('select.md-frequency');

                var unitArr = JSON.parse($('#hdMedicineUnitList').val());
                $(unitArr).each(function (key, value) {
                    $munit.append('<option value="' + unitArr[key].Id + '">' + unitArr[key].Name + '</option>');
                });

                var formArr = JSON.parse($('#hdMedicineFormList').val());
                $(formArr).each(function (key, value) {
                    $mform.append('<option value="' + formArr[key].Id + '">' + formArr[key].Name + '</option>');
                });

                var frequrencyArr = JSON.parse($('#hdMedicineFrequencyList').val());
                $(frequrencyArr).each(function (key, value) {
                    $mfrequency.append('<option value="' + frequrencyArr[key].Id + '">' + frequrencyArr[key].Name + '</option>');
                });

                $mform.find('option[value="' + itemArray[key].FormId + '"]').attr('selected', 'selected');
                $munit.find('option[value="' + itemArray[key].UnitId + '"]').attr('selected', 'selected');
                $mfrequency.find('option[value="' + itemArray[key].FrequencyId + '"]').attr('selected', 'selected');

                $munit.each(function () {
                    var $this = $(this), selectOptions = $(this).children('option').length;
                    $this.addClass('hide-select');
                    $this.wrap('<div class="select"></div>');
                    $this.after('<div class="custom-select"></div>');

                    var $customSelect = $this.next('div.custom-select');
                    var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
                    $customSelect.text(displayText);

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
                        $this.find('option').removeAttr('selected');
                        $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
                        $optionlist.hide();
                    });

                    $(document).click(function () {
                        $customSelect.removeClass('active');
                        $optionlist.hide();
                    });
                });

                $mform.each(function () {
                    var $this = $(this), selectOptions = $(this).children('option').length;
                    $this.addClass('hide-select');
                    $this.wrap('<div class="select"></div>');
                    $this.after('<div class="custom-select"></div>');

                    var $customSelect = $this.next('div.custom-select');
                    var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
                    $customSelect.text(displayText);

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
                        $this.find('option').removeAttr('selected');
                        $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
                        $optionlist.hide();
                    });

                    $(document).click(function () {
                        $customSelect.removeClass('active');
                        $optionlist.hide();
                    });
                });

                $mfrequency.each(function () {
                    var $this = $(this), selectOptions = $(this).children('option').length;
                    $this.addClass('hide-select');
                    $this.wrap('<div class="select"></div>');
                    $this.after('<div class="custom-select"></div>');

                    var $customSelect = $this.next('div.custom-select');
                    var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
                    $customSelect.text(displayText);

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
                        $this.find('option').removeAttr('selected');
                        $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
                        $optionlist.hide();
                    });

                    $(document).click(function () {
                        $customSelect.removeClass('active');
                        $optionlist.hide();
                    });
                });
            });
        }
    }

    $('<div class="d-flex align-items-end flex-wrap justify-content-between flex-md-nowrap feilds mb-2 medicationdiv">'
        + '<button type="button" class="btn priBtn btndoseaddrow mt-0" onclick="medicationDoseRow(this)">Add</button>'
        + '<div class="formWrap">'
        + '    <input class="form-control md-drugname"  maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter drug name"/>'
        + '    <span class="text-danger" style="display:none;">Required</span>'
        + '</div>'
        + '<div class="formWrap">'
        + '    <input class="form-control md-dose" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter dosage"/>'
        + '    <span class="text-danger" style="display:none;">Required</span>'
        + '</div>'
        + '<div class="formWrap">'
        + '      <div class="wrap"><select class="form-control md-unit list-medication"></select></div>'
        + '</div>'
        + '<div class="formWrap">'
        + '     <div class="wrap"><select class="form-control md-form list-medication"></select></div>'
        + '</div>'
        + '<div class="formWrap">'
        + '     <div class="wrap"><select class="form-control md-frequency list-medication"></select></div>'
        + '</div>'
        + '<div class="formWrap">'
        + '    <input class="form-control md-medical-condition"  maxlength="500" type="text" oninput="checkEmpty(this)" placeholder="Enter medical condition"/>'
        + '    <span class="text-danger" style="display:none;">Required</span>'
        + '</div>'
        + '</div>').insertAfter($medicineContainer.children().last());

    var $mform = $medicineContainer.children().last().find('select.md-form');
    var $munit = $medicineContainer.children().last().find('select.md-unit');
    var $mfrequency = $medicineContainer.children().last().find('select.md-frequency');

    var unitArr = JSON.parse($('#hdMedicineUnitList').val());
    $(unitArr).each(function (key, value) {
        $munit.append('<option value="' + unitArr[key].Id + '">' + unitArr[key].Name + '</option>');
    });

    var formArr = JSON.parse($('#hdMedicineFormList').val());
    $(formArr).each(function (key, value) {
        $mform.append('<option value="' + formArr[key].Id + '">' + formArr[key].Name + '</option>');
    });

    var frequrencyArr = JSON.parse($('#hdMedicineFrequencyList').val());
    $(frequrencyArr).each(function (key, value) {
        $mfrequency.append('<option value="' + frequrencyArr[key].Id + '">' + frequrencyArr[key].Name + '</option>');
    });

    $munit.each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
        $customSelect.text(displayText);

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
            $this.find('option').removeAttr('selected');
            $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
            $optionlist.hide();
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });

    $mform.each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
        $customSelect.text(displayText);

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
            $this.find('option').removeAttr('selected');
            $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
            $optionlist.hide();
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });

    $mfrequency.each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
        $customSelect.text(displayText);

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
            $this.find('option').removeAttr('selected');
            $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
            $optionlist.hide();
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });
}

function patientStateDropdown() {
    var $selectStateDropdown = $('#ddlState');
    if ($selectStateDropdown.parent().is('div.patient-state-dropdown')) {
        $selectStateDropdown.unwrap();
        $selectStateDropdown.removeClass('hide-select');
        $selectStateDropdown.find('option').removeAttr('selected');
        $selectStateDropdown.nextAll().remove();
    }
    $('#ddlState').each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select patient-state-dropdown"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
        $customSelect.text(displayText);

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
            $this.find('option').removeAttr('selected');
            $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
            $optionlist.hide();
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });

    });
}

function pharmacyStateDropdown() {
    var $selectStateDropdown = $('#ddlPhState');
    if ($selectStateDropdown.parent().is('div.pharmacy-state-dropdown')) {
        $selectStateDropdown.unwrap();
        $selectStateDropdown.removeClass('hide-select');
        $selectStateDropdown.find('option').removeAttr('selected');
        $selectStateDropdown.nextAll().remove();
    }
    $('#ddlPhState').each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select pharmacy-state-dropdown"></div>');
        $this.after('<div class="custom-select active"></div>');

        var $customSelect = $this.next('div.custom-select');
        var displayText = $this.children('option:selected').text() == undefined ? $this.children('option').eq(0).text() : $this.children('option:selected').text();
        $customSelect.text(displayText);

        var $optionlist = $('<ul />', {'class': 'select-options','style':'display:none' }).insertAfter($customSelect);

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
            $this.find('option').removeAttr('selected');
            $this.find('option[value="' + $(this).attr('rel') + '"]').attr('selected', 'selected');
            $optionlist.hide();
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });
    });   
}

function checkEmpty(element) {
    if ($(element).val() == '') {
        $(element).next('span').show();
    } else {
        $(element).next('span').hide();
    }
}

function MoveNextPrevPatient(recordType,rowNo) {
    $('#Loader').show();
    $('#PatientChart').load('' + PhysicianPortalUrl.GetPatientChartByRowNoUrl + '?recordType=' + recordType + '&rowNo=' + rowNo, function (responseTxt, statusTxt, xhr) {
        if (statusTxt == "success")
            $('#Loader').hide();
        if (statusTxt == "error")
            $('#Loader').hide();
    });
}