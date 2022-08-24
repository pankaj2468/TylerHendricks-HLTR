$(document).ready(function () {

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

    $('#formQuestionaire').submit(function () {
        $('#Loader').show();
        if ($("#DivAddDynamicMedication").length > 0) {
            var medicineArr = new Array();
            var thisMedFlag = true;
            $("#DivAddDynamicMedication").children().each(function (key, element) {
                let _drugname = $(this).find('.md-drugname').val();
                let _dose = $(this).find('.md-dose').val();
                let _unitId = $(this).find('.md-unit').val();
                let _formId = $(this).find('.md-form').val();
                let _frequencyId = $(this).find('.md-frequency').val();
                let _medicationCondition = $(this).find('.md-medical-condition').val();
                if (_drugname == '' || _dose == '' || _medicationCondition == '' || _unitId == '0' || _formId == '0' || _frequencyId == '0') {
                    if (_drugname == '')
                        $(this).find('.md-drugname').next('span').show();
                    if (_dose == '')
                        $(this).find('.md-dose').next('span').show();
                    if (_medicationCondition == '')
                        $(this).find('.md-medical-condition').next('span').show();
                    if (_unitId == '0')
                        $(this).find('.md-unit').parent().next('span').show();
                    if (_formId == '0')
                        $(this).find('.md-form').parent().next('span').show();
                    if (_frequencyId == '0')
                        $(this).find('.md-frequency').parent().next('span').show();
                    if (key == 0) {
                        thisMedFlag = false;
                    }
                }
                var medicine = {
                    DrugName: _drugname, Dose: _dose, MedicationUnitId: _unitId, MedicationFormId: _formId,
                    MedicationFrequencyId: _frequencyId, MedicalCondition: _medicationCondition
                };
                medicineArr.push(medicine);
            });

            if (thisMedFlag == false) {
                $('#ErrorMessgeMedi').show();
                $('#Loader').hide();
                return false;
            }
            else {
                $('#ErrorMessgeMedi').hide();
                if (medicineArr.length > 0) {
                    $.ajax({
                        type: 'POST',
                        url: '' + PatientPortalUrl.MedicineDosePostUrl + '',
                        data: { medicines: medicineArr, isMedicine: true },
                        dataType: 'json',
                        beforeSend: function () { $('#Loader').show(); },
                        complete: function () {  },
                        async: false,
                        success: function (response) {

                        },
                        error: function (response) {
                            console.log(response);
                            $('#Loader').hide();
                            return false;
                        },
                        failure: function (response) {
                            console.log(response);
                            $('#Loader').hide();
                            return false;
                        }
                    });
                }
                else {
                    $('#ErrorMessgeMedi').show();
                    $('#Loader').hide();
                    return false;
                }
            }
        }
        if ($('#txtsurgries_textarea').length > 0) {
            $('#hdAnswer').val($('#txtsurgries_textarea').val());
        }
        if ($('#txtallergies_textarea').length > 0) {
            $('#hdAnswer').val($('#txtallergies_textarea').val());
        }
        if ($('#txthairlossallergies_textarea').length > 0) {
            $('#hdAnswer').val($('#txthairlossallergies_textarea').val());
        }
        if ($('#fuploadMedicine').length > 0) {
            if ($('#fuploadMedicine')[0].files.length == 0) {
                $(document).find('#SelfiErrorSpan').show();
                $('#Loader').hide();
                return false;
            } else {
                var data = new FormData();
                $(document).find('.sliderImg').each(function (i, index) {
                    var file = DataURIToBlob($(this).attr('src'));
                    data.append('file-' + i, file);
                });
                jQuery.ajax({
                    url: '' + PatientPortalUrl.MedicineUploadUrl + '',
                    type: 'POST',
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    async: false,
                    beforeSend: function () { $('#Loader').show() },
                    complete: function () { },
                    success: function (data) {
                        console.log(data);
                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }
        }
        if ($('#fuploadSelfie').length > 0) {
            $('#Loader').show();
            if ($('#fuploadSelfie')[0].files.length == 0 || $('#fuploadSelfiePhotoId')[0].files.length == 0) {
                $(document).find('#ErrorPhotoId').show();
                $('#Loader').hide();
                return false;
            } else {
                $(document).find('#ErrorPhotoId').hide();
                var data = new FormData();
                jQuery.each(jQuery('#fuploadSelfie')[0].files, function (i, file) {
                    data.append('file-selfie', file);
                });
                jQuery.each(jQuery('#fuploadSelfiePhotoId')[0].files, function (i, file) {
                    data.append('file-photo-id', file);
                });
                jQuery.ajax({
                    url: '' + PatientPortalUrl.PhotoIdUploadUrl + '',
                    type: 'POST',
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    async: false,
                    beforeSend: function () { $('#Loader').show(); },
                    complete: function () { },
                    success: function (data) {
                        window.location.reload();
                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }
        }
        if ($('#DivAddDynamicMedicationInDose').length > 0) {
            var thisPageFlag = true;
            var $medicationCategoryDropdown = $('#ddlmedicationcategory');
            var $medicationDropdown = $('#ddlselectmedication');
            var $doseInput = $(document).find('.md-dose');
            var $unitDropdown = $(document).find('select.md-unit');
            var $formDropdown = $(document).find('select.md-form');
            var $frequencyDropdown = $(document).find('select.md-frequency');
            if ($medicationCategoryDropdown.val() == '0' || $medicationCategoryDropdown.val() == undefined) {
                $medicationCategoryDropdown.parent().next('span').show();
                thisPageFlag = false;
            } else {
                $medicationCategoryDropdown.parent().next('span').hide();
            }
            if ($medicationDropdown.val() == '0' || $medicationDropdown.val() == undefined) {
                $medicationDropdown.parent().next('span').show();
                thisPageFlag = false;
            } else {
                $medicationDropdown.parent().next('span').hide();
            }
            if ($doseInput.val() == '' || $doseInput.val() == undefined) {
                $doseInput.next('span').show();
                thisPageFlag = false;
            } else {
                $doseInput.next('span').hide();
            }
            if ($unitDropdown.val() == '0' || $unitDropdown.val() == undefined) {
                $unitDropdown.parent().next('span').show();
                thisPageFlag = false;
            } else {
                $unitDropdown.parent().next('span').hide();
            }
            if ($formDropdown.val() == '0' || $formDropdown.val() == undefined) {
                $formDropdown.parent().next('span').show();
                thisPageFlag = false;
            } else {
                $formDropdown.parent().next('span').hide();
            }
            if ($frequencyDropdown.val() == '0' || $frequencyDropdown.val() == undefined) {
                $frequencyDropdown.parent().next('span').show();
                thisPageFlag = false;
            } else {
                $frequencyDropdown.parent().next('span').hide();
            }

            if (thisPageFlag == false) {
                $('#Loader').hide();
                return false;
            } else {
                var medicineArr = new Array();
                let _drugname = $medicationDropdown.find('option:selected').text();
                let _dose = $doseInput.val();
                let _unitId = $unitDropdown.val();
                let _formId = $formDropdown.val();
                let _frequencyId = $frequencyDropdown.val();
                let _medicationCondition = $medicationCategoryDropdown.find('option:selected').text();
                var medicine = {
                    DrugName: _drugname, Dose: _dose, MedicationUnitId: _unitId, MedicationFormId: _formId,
                    MedicationFrequencyId: _frequencyId, MedicalCondition: _medicationCondition
                };
                medicineArr.push(medicine);
                if (medicineArr.length > 0) {
                    $.ajax({
                        type: 'POST',
                        url: '' + PatientPortalUrl.MedicineDosePostUrl+ '',
                        data: { medicines: medicineArr, isMedicine: false },
                        dataType: 'json',
                        beforeSend: function () { $('#Loader').show(); },
                        complete: function () {  },
                        async: false,
                        success: function (response) {
                           
                        },
                        error: function (response) {
                            console.log(response);
                            $('#Loader').hide();
                            return false;
                        },
                        failure: function (response) {
                            console.log(response);
                            $('#Loader').hide();
                            return false;
                        }
                    });
                }
            }
        }
        if ($('input[name="hairlosstype"]').length > 0) {
            if ($('input[name="hairlosstype"]:checked').length == 0 || $('input[name="hairlosstype"]:checked').val() == undefined) {
                $(document).find('#ErrorHairLossArea').show();
                $('#Loader').hide();
                return false
            }
            else {
                $('#hdAnswer').val($('input[name="hairlosstype"]:checked').val());
            }
        }
        if ($('input[name="hairloss"]').length > 0) {
            if ($('input[name="hairloss"]:checked').length == 0 || $('input[name="hairloss"]:checked').val() == undefined) {
                $(document).find('#Errorhairlossnotice').show();
                $('#Loader').hide();
                return false;
            }
            else {
                $('#hdAnswer').val($('input[name="hairloss"]:checked').attr('def'));
            }
        }
        if ($('#medSelection1').length > 0) {
            $('#Loader').show();
            if ($('#fuploadMed1')[0].files.length > 0 || $('#fuploadMed2')[0].files.length > 0 || $('#fuploadMed3')[0].files.length > 0) {
                $(document).find('#ErrorMessgeMedicationUpload').hide();
                var data = new FormData();
                if ($('#fuploadMed1')[0].files.length > 0) {
                    $.each($('#fuploadMed1')[0].files, function (i, file) {
                        data.append('FILE-1', file);
                    });
                }
                if ($('#fuploadMed2')[0].files.length > 0) {
                    $.each($('#fuploadMed2')[0].files, function (i, file) {
                        data.append('FILE-2', file);
                    });
                }
                if ($('#fuploadMed3')[0].files.length > 0) {
                    $.each($('#fuploadMed3')[0].files, function (i, file) {
                        data.append('FILE-3', file);
                    });
                }
                $.ajax({
                    url: '' + PatientPortalUrl.MedicineUploadUrl + '',
                    type: 'POST',
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    async: false,
                    beforeSend: function () { $('#Loader').show(); },
                    complete: function () { },
                    success: function (data) {
                        console.log(data);
                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            } else {
                $(document).find('#ErrorMessgeMedicationUpload').show();
                $('#Loader').hide();
                return false;
            }
        }
    });

    GetMedicationCategory();

    $(document).on('change', '#ddlmedicationcategory', function () {
        if ($(this).val() != "0") {
            $(document).find('#DivSecondDropdownMedication').show();
        }
        GetMedication($('#ddlmedicationcategory').val());
    });

    $(document).on('change', '#DivAddDynamicMedication select', function () {
        if ($(this).val() != "0") {
            $(this).parent().next('span').hide();
        } else {
            $(this).parent().next('span').show(); 
        }
    });

    $(document).on('change', '#DivAddDynamicMedicationInDose select', function () {
        if ($(this).val() != "0") {
            $(this).parent().next('span').hide();
        } else {
            $(this).parent().next('span').show();
        }
    });

    $('#ddlselectmedication').change(function () {
        $(document).find('#ErrorMedicCateShort').hide();
        $('#hdAnswer').val($('#ddlselectmedication').val());
    });

    $('#PharmacyForm').submit(function () {
        if ($('#State').val() == '0') {
            $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-valid').addClass('field-validation-error');
            $(document).find('span[data-valmsg-for="State"]').html('<span id="State-error" class="">Required</span>');
            return false;
        } else {
            $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-error').addClass('field-validation-valid');
            $(document).find('span[data-valmsg-for="State"]').html('');
        }
    });

    $('#FinishAccountSetupForm').submit(function () {
        if ($('#State').val() == '0') {
            $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-valid').addClass('field-validation-error');
            $(document).find('span[data-valmsg-for="State"]').html('<span id="State-error" class="">Required</span>');
            return false;
        } else {
            $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-error').addClass('field-validation-valid');
            $(document).find('span[data-valmsg-for="State"]').html('');
        }
    });

    $('#FinishAccountSetupFormhairloss').submit(function () {
        if ($('#State').val() == '0') {
            $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-valid').addClass('field-validation-error');
            $(document).find('span[data-valmsg-for="State"]').html('<span id="State-error" class="">Required</span>');
            return false;
        } else {
            $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-error').addClass('field-validation-valid');
            $(document).find('span[data-valmsg-for="State"]').html('');
        }
    });
});


function openFileDialog(element) {
    $(element).click();
}

function Numeric(input) {
    input.value = input.value.replace(/[^\d]/, '')
}

/**************************Upload Medicine Photo Start*********************************/
$(function () {

    $('#BtnMedicineUpload').click(function () {
        $('#fuploadMedicine').click();
    });

    $('#btnuploadselfie').click(function () {
        $('#fuploadSelfie').click();
    });

    $('#btnuploadphotoid').click(function () {
        $('#fuploadSelfiePhotoId').click();
    });

    $('#fuploadSelfie').change(function () {
        $(document).find('#ErrorPhotoId').hide();
        $(this).parent().prev().css({ 'border': '1px solid #9d9d9d' });
        var returnFlag = fileUploadValidation(this);
        if (returnFlag) {
            filePreview(this, 'ImgSelfie');
        }
    });

    $('#fuploadSelfiePhotoId').change(function () {
        $(document).find('#ErrorPhotoId').hide();
        $(this).parent().prev().css({ 'border': '1px solid #9d9d9d' });
        var returnFlag = fileUploadValidation(this);
        if (returnFlag) {
            filePreview(this, 'ImgPhotoId');
        }
    });

    $('#fuploadMedicine').change(function () {
        var fileFlag = true;
        $(document).find('#SelfiErrorSpan').hide();
        if (this.files) {
            if (this.files.length <= 3) {
                $(this.files).each(function (key, value) {
                    let fsize = value.size / 1024;
                    if (fsize > 10240) {
                        Swal.fire({
                            title: 'File size',
                            text: 'File size is too large for destination',
                            icon: 'warning',
                            showClass: {
                                backdrop: 'swal2-noanimation',
                                popup: '',
                                icon: ''
                            },
                            hideClass: {
                                popup: '',
                            }
                        });
                        fileFlag = false;

                        return false;
                    }
                    let mimeType = value.type;
                    if (mimeType.split('/')[0] != 'image') {
                        Swal.fire({
                            title: 'File type not accepted',
                            text: 'Accept only all image formats',
                            icon: 'warning',
                            showClass: {
                                backdrop: 'swal2-noanimation',
                                popup: '',
                                icon: ''
                            },
                            hideClass: {
                                popup: '',
                            }
                        });
                        fileFlag = false;
                        return false;
                    }
                });
                if (fileFlag) {
                    if (this.files && this.files[0]) {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            $(document).find('.imgActive').attr('src', e.target.result);
                        };
                        reader.readAsDataURL(this.files[0]);
                    }
                }
            } else {
                Swal.fire({
                    title: 'Alert',
                    text: 'You are only allowed to upload a maximum of 3 files',
                    icon: 'warning',
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
    });
});

function fileUploadValidation(fileElement) {
    var flag = true;
    if (fileElement.files) {
        $(fileElement.files).each(function (key, value) {
            let fileSize = value.size / 1024;
            let mimeType = value.type;
            if (fileSize > 10240) {
                Swal.fire({
                    title: 'File size',
                    text: 'File size is too large for destination',
                    icon: 'warning',
                    showClass: {
                        backdrop: 'swal2-noanimation',
                        popup: '',
                        icon: ''
                    },
                    hideClass: {
                        popup: '',
                    }
                });
                flag = false;
            }
            if (mimeType.split('/')[0] != 'image') {
                Swal.fire({
                    title: 'File type not accepted',
                    text: 'Accept only all image formats',
                    icon: 'warning',
                    showClass: {
                        backdrop: 'swal2-noanimation',
                        popup: '',
                        icon: ''
                    },
                    hideClass: {
                        popup: '',
                    }
                });
                flag = false;
            }
        });
        return flag;
    }
}

function fileValidateMed(fileElement, preview, thumnail) {
    var flag = true;
    if (fileElement.files) {
        $(fileElement.files).each(function (key, value) {
            let fileSize = value.size / 1024;
            let mimeType = value.type;
            if (fileSize > 10240) {
                Swal.fire({
                    title: 'File size',
                    text: 'File size is too large for destination',
                    icon: 'warning',
                    showClass: {
                        backdrop: 'swal2-noanimation',
                        popup: '',
                        icon: ''
                    },
                    hideClass: {
                        popup: '',
                    }
                });
                flag = false;
            }
            if (mimeType.split('/')[0] != 'image') {
                Swal.fire({
                    title: 'File type not accepted',
                    text: 'Accept only all image formats',
                    icon: 'warning',
                    showClass: {
                        backdrop: 'swal2-noanimation',
                        popup: '',
                        icon: ''
                    },
                    hideClass: {
                        popup: '',
                    }
                });
                flag = false;
            }
        });
        if (flag) {
            filePreview(fileElement, preview);
            $('#' + preview).removeClass('hide-select');
            $('#' + thumnail).addClass('hide-select');
        }
        return flag;
    }
}

function UploadMedicineFile() {
    var data = new FormData();
    $('.sliderImg').each(function (i, index) {
        var file = DataURIToBlob($(this).attr('src'));
        data.append('file-' + i, file);
    });
    jQuery.ajax({
        url: '' + PatientPortalUrl.MedicineUploadUrl + '',
        type: 'POST',
        data: data,
        cache: false,
        contentType: false,
        processData: false,
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (data) {

        },
        error: function (data) {

        }
    });
}

function UploadPhotoIdFile() {
    var data = new FormData();
    jQuery.each(jQuery('#fuploadSelfie')[0].files, function (i, file) {
        data.append('file-selfie', file);
    });
    jQuery.each(jQuery('#fuploadSelfiePhotoId')[0].files, function (i, file) {
        data.append('file-photo-id', file);
    });
    jQuery.ajax({
        url: '' + PatientPortalUrl.PhotoIdUploadUrl + '',
        type: 'POST',
        data: data,
        cache: false,
        contentType: false,
        processData: false,
        beforeSend: function () { $('#Loader').show(); },
        success: function (data) {

        }
    });
}

function DataURIToBlob(dataURI) {
    const splitDataURI = dataURI.split(',')
    const byteString = splitDataURI[0].indexOf('base64') >= 0 ? atob(splitDataURI[1]) : decodeURI(splitDataURI[1])
    const mimeString = splitDataURI[0].split(':')[1].split(';')[0]

    const ia = new Uint8Array(byteString.length)
    for (let i = 0; i < byteString.length; i++)
        ia[i] = byteString.charCodeAt(i)

    return new Blob([ia], { type: mimeString })
}

function filePreview(input, preview) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById(preview).src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

/**************************Upload Medicine Photo End***********************************/

/**************************Medicine Dose Start*****************************************/
$(function () {
    if ($('#DivAddDynamicMedication').length > 0) {
        GetMedicationDropdown();
    }
    if ($('#DivAddDynamicMedicationInDose').length > 0) {
        GetMedicationDropdown();
    }
});

function checkEmpty(element) {
    if ($(element).val() == '') {
        $(element).next('span').show();
    } else {
        $(element).next('span').hide();
    }
}

function GetMedicationDropdown($mmunit, $mmform, $mmfrequency) {
    var $munit = $mmunit;
    var $mform = $mmform;
    var $mfrequency = $mmfrequency;
    $.ajax({
        type: "GET",
        url: '' + PatientPortalUrl.MedicationGetDropdownUrl + '',
        data: {},
        async: true,
        success: function (result) {
            var jsonObj = JSON.parse(result);
            $('#hdMedicineUnitList').val(JSON.stringify(jsonObj.units));
            $('#hdMedicineFormList').val(JSON.stringify(jsonObj.forms));
            $('#hdMedicineFrequencyList').val(JSON.stringify(jsonObj.frequency));

            if ($('#DivAddDynamicMedication').length > 0) {
                bindMedicationDropDown($munit, $mform, $mfrequency);
            }
            if ($('#DivAddDynamicMedicationInDose').length > 0) {
                bindMedicationDropDownInDose();
            }
        },
        error: function (error) {
        }
    });
}

function GetMedicationCategory() {
    if ($('#ddlmedicationcategory').length > 0) {
        $.ajax({
            type: "GET",
            url: '' + PatientPortalUrl.GetMedicationCategoryUrl + '',
            data: {},
            async: true,
            success: function (result) {
                var jsonobj = JSON.parse(result);
                $('#ddlmedicationcategory').html('<option value = "0" >Select a medication category</option>');
                $(jsonobj).each(function (key, value) {
                    $('#ddlmedicationcategory').append('<option value="' + jsonobj[key].Id + '">' + jsonobj[key].Name + '</option>');
                });
                GetMedication($('#ddlmedicationcategory').val());
                NewStyleDropdownForMedicationCategory();
            },
            error: function (error) {

            }
        });
    }
}

function GetMedication(__medicationId) {
    $.ajax({
        type: "GET",
        url: '' + PatientPortalUrl.GetMedicationUrl + '',
        data: { medicationcatId: __medicationId },
        async: true,
        success: function (result) {
            var $selectStateDropdown = $('#ddlselectmedication');
            if ($selectStateDropdown.parent().is('div.select')) {
                $selectStateDropdown.unwrap();
                $selectStateDropdown.removeClass('hide-select');
                $selectStateDropdown.find('option').removeAttr('selected');
                $selectStateDropdown.nextAll().remove();
            }
            var jsonobj = JSON.parse(result);
            $('#ddlselectmedication').html('<option value = "0" >Select a medication</option>');
            $(jsonobj).each(function (key, value) {
                $('#ddlselectmedication').append('<option value="' + jsonobj[key].Id + '">' + jsonobj[key].Name + '</option>');
            });
            NewStyleDropdownForMedication();
            $('<span class="text-danger" style="display:none">Required</span>').insertAfter($selectStateDropdown.parent());
        },
        error: function (error) {

        }
    });
}

function bindMedicationDropDownInDose(unit, form, frequency) {
    if (unit == undefined) {
        unit = $('#DivAddDynamicMedicationInDose').children().first().find('select.md-unit');
    }
    if (form == undefined) {
        form = $('#DivAddDynamicMedicationInDose').children().first().find('select.md-form');
    }
    if (frequency == undefined) {
        frequency = $('#DivAddDynamicMedicationInDose').children().first().find('select.md-frequency');
    }
    $(unit).empty();
    var unitArr = JSON.parse($('#hdMedicineUnitList').val());
    $(unit).append('<option value="0">Unit</option>');
    $(unitArr).each(function (key, value) {
        $(unit).append('<option value="' + unitArr[key].Id + '">' + unitArr[key].Name + '</option>');
    });

    $(form).empty();
    var formArr = JSON.parse($('#hdMedicineFormList').val());
    $(form).append('<option value="0">Form</option>');
    $(formArr).each(function (key, value) {
        $(form).append('<option value="' + formArr[key].Id + '">' + formArr[key].Name + '</option>');
    });

    $(frequency).empty();
    var frequrencyArr = JSON.parse($('#hdMedicineFrequencyList').val());
    $(frequency).append('<option value="0">Frequency</option>');
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

    $(form).each(function () {
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

    $(frequency).each(function () {

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
}

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
    $(unit).empty();
    var unitArr = JSON.parse($('#hdMedicineUnitList').val());
    $(unit).append('<option value="0">Unit</option>');
    $(unitArr).each(function (key, value) {
        $(unit).append('<option value="' + unitArr[key].Id + '">' + unitArr[key].Name + '</option>');
    });

    $(form).empty();
    var formArr = JSON.parse($('#hdMedicineFormList').val());
    $(form).append('<option value="0">Form</option>');
    $(formArr).each(function (key, value) {
        $(form).append('<option value="' + formArr[key].Id + '">' + formArr[key].Name + '</option>');
    });

    $(frequency).empty();
    var frequrencyArr = JSON.parse($('#hdMedicineFrequencyList').val());
    $(frequency).append('<option value="0">Frequency</option>');
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

    $(form).each(function () {
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

    $(frequency).each(function () {

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
}

function MedicationDoseRow(current) {
    var btnText = current.innerHTML.trim().toLowerCase();
    if (btnText == 'add') {
        var flag = true;
        var $container;
        if ($(current).attr('id') == 'btnstatic') {
            $container = $(document).find('#DivAddDynamicMedication').last();
        } else {
            $container = $(current).parent();
        }
        var $drugname = $container.find('.md-drugname');
        var $dose = $container.find('.md-dose');
        var $medication = $container.find('.md-medical-condition');
        var $unit = $container.find('.md-unit');
        var $form = $container.find('.md-form');
        var $frequency = $container.find('.md-frequency');

        if ($unit.val() == '0') {
            flag = false;
            $unit.parent().next('span').show();
        } else {
            $unit.parent().next('span').hide();
        }

        if ($form.val() == '0') {
            flag = false;
            $form.parent().next('span').show();
        } else {
            $form.parent().next('span').hide();
        }

        if ($frequency.val() == '0') {
            flag = false;
            $frequency.parent().next('span').show();
        } else {
            $frequency.parent().next('span').hide();
        }

        if ($drugname.val() == '' && $dose.val() == '' && $medication.val() == '') {
            $drugname.next('span').show();
            $dose.next('span').show();
            $medication.next('span').show();
            flag = false;
        }
        else if ($drugname.val() == '' && $dose.val() != '' && $medication.val() != '') {
            $drugname.next('span').show();
            $dose.next('span').hide();
            $medication.next('span').hide();
            flag = false;
        }
        else if ($drugname.val() != '' && $dose.val() == '' && $medication.val() != '') {
            $drugname.next('span').hide();
            $dose.next('span').show();
            $medication.next('span').hide();
            flag = false;
        }
        else if ($drugname.val() != '' && $dose.val() != '' && $medication.val() == '') {
            $drugname.next('span').hide();
            $dose.next('span').hide();
            $medication.next('span').show();
            flag = false;
        }
        else if ($drugname.val() == '' && $dose.val() == '' && $medication.val() != '') {
            $drugname.next('span').show();
            $dose.next('span').show();
            $medication.next('span').hide();
            flag = false;
        } else if ($drugname.val() == '' && $dose.val() != '' && $medication.val() == '') {
            $drugname.next('span').show();
            $dose.next('span').hide();
            $medication.next('span').show();
            flag = false;
        } else if ($drugname.val() != '' && $dose.val() == '' && $medication.val() == '') {
            $drugname.next('span').hide();
            $dose.next('span').show();
            $medication.next('span').show();
            flag = false;
        }

        if (flag) {
            var dynamicString = '<div class="d-flex align-items-start justify-content-around feilds mb-2 medicationdiv flex-wrap flex-lg-nowrap">';
            if ($(current).attr('id') == 'btnstatic') {
                dynamicString += '<button type="button" class="btn priBtn btndoseaddrow mt-0 new-add-btn-medicine remove-btn-medicine" onclick="MedicationDoseRow(this)">Remove</button>';
            } else {
                dynamicString += '<button type="button" class="btn priBtn btndoseaddrow mt-0 new-add-btn-medicine" onclick="MedicationDoseRow(this)">Add</button>';
            }
            dynamicString += '<div class="formWrap">'
                + '<input class="form-control md-drugname" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter drug name"/>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '<input class="form-control md-dose" maxlength="200" type="text" oninput="checkEmpty(this)" placeholder="Enter dosage"/>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '    <select class="form-control md-unit list-medication hide-select"></select>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '     <select class="form-control md-form list-medication hide-select"></select>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '     <select class="form-control md-frequency list-medication hide-select"></select>'
                + '<span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '<div class="formWrap">'
                + '    <input class="form-control md-medical-condition" maxlength="500" type="text" oninput="checkEmpty(this)" placeholder="Enter medical condition"/>'
                + '    <span class="text-danger" style="display:none;">Required</span>'
                + '</div>'
                + '</div>';

            $('#DivAddDynamicMedication').append(dynamicString);
            if ($(current).attr('id') != 'btnstatic') {
                current.innerHTML = 'Remove';
            }
            $(current).parent().find('.md-drugname').attr('readonly', 'readonly');
            $(current).parent().find('.md-dose').attr('readonly', 'readonly');
            $(current).parent().find('.md-unit').prop('disabled', true);
            $(current).parent().find('.md-form').prop('disabled', true);
            $(current).parent().find('.md-frequency').prop('disabled', true);
            $(current).parent().find('.md-medical-condition').attr('readonly', 'readonly');

            var $mform = $('#DivAddDynamicMedication').children().last().find('select.md-form');
            var $munit = $('#DivAddDynamicMedication').children().last().find('select.md-unit');
            var $mfrequency = $('#DivAddDynamicMedication').children().last().find('select.md-frequency');
            GetMedicationDropdown($munit, $mform, $mfrequency);
        }
    } else if (btnText == 'remove') {
        $(current).parent().remove();
    }
}

/**************************Medicine Dose End*****************************************/

function SetMedicationCT(response) {
    $('#hdMedicationCond1').val(response);
}

/**************************Patient Index Start***************************************/

function SetResponse(current, response, nextQId) {
    $('#hdnResponse').val(response);
    $('#hdNextQuestion').val(nextQId);
    $('#hdAnswer').val(current.innerHTML);
    var btntype = current.getAttribute('type');
    var pophtml = '';
    if (response == true) {
        pophtml = $('#hdModalPopup').val();
    } else {
        pophtml = $('#hdModalPopup1').val();
    }
    if (btntype == 'button') {
        Swal.fire({
            title: 'Alert',
            text: "" + pophtml + "",
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
                $('#formQuestionaire').submit();
            }
        });
    }
}

function sethairlossradio(current) {
    $(document).find('input:radio[name=hairlosstype][value="' + current.innerText + '"]').attr('checked', true);
    $('button.btn-hairloss-hairtype').removeClass('hairlossbtnactive');
    $(current).addClass('hairlossbtnactive');
}

function sethairlossnotice(current, val) {
    $(document).find('input:radio[name=hairloss][value=' + val + ']').attr('checked', true);
    $(document).find('input:radio[name=hairloss][value=' + val + ']').attr('def', $(current).html());
    $('button.hairlossnotice').removeClass('hairlossbtnactive');
    $(current).addClass('hairlossbtnactive');
}

/**************************Patient Index End*****************************************/

/**************************Finish Account Setup Start********************************/
$(function () {
    //$('#FinishAccountSetupForm').submit(function () {
    //    var $currentForm = $(this);
    //    var flag = 0;
    //    $currentForm.find('input[type=radio].chooseMedication').each(function () {
    //        if ($(this).prop("checked")) {
    //            flag = 1;
    //        }
    //    });
    //    if (flag == 0) {
    //        $('#ErrorFinishAccountSetup').show();
    //        return false;
    //    } else {
    //        var medicationDetail = $currentForm.find('#hdnFinishDetailId').val();
    //        if (medicationDetail == "" || medicationDetail == undefined || medicationDetail == "0") {
    //            $('#ErrorFinishAccountSetup').show();
    //            return false;
    //        }
    //    }
    //});

    $('#FinishAccountSetupForm').find('input[type=radio].chooseMedication').click(function () {
        var $current = $(this);
        if ($(this).prop("checked")) {
            $(document).find('#hdnFinishDetailId').val($(this).attr('data-value'));
            $(document).find('[data-valmsg-for=DetailId]').empty();
            bindMedicine($current.attr('data-price'), $current.attr('data-refill'), JSON.parse($current.attr('data-delivery').toLowerCase()))
        }
    });

    $('#formConsultationOption').submit(function () {
        var $currentForm = $(this);
        var flag = 0;
        $currentForm.find('input[type=radio].chooseMedication').each(function () {
            if ($(this).prop("checked")) {
                flag = 1;
            }
        });
        if (flag == 0) {
            $('#ChooseYourMedicationError').show();
            return false;
        } else {
            var medicationDetail = $currentForm.find('#hidMedicationDetailForHairLoss').val();
            if (medicationDetail == "" || medicationDetail == undefined || medicationDetail == "0") {
                $('#ChooseYourMedicationError').show();
                return false;
            }
        }
    });

    $('#formConsultationOption').find('input[type=radio].chooseMedication').click(function () {
        if ($(this).prop("checked")) {
            $(document).find('#hidMedicationDetailForHairLoss').val($(this).attr('data-value'));
        }
    });
});

function bindMedicine(price, time,ishomedelivery) {
    $('#MediSummary').children().not(':first').remove();
    if (parseInt(time) == 0) {
        if (ishomedelivery) {
            $('#MediSummary').append('<p class="mb-3"> Physician Consultation <br/> 1 prescription (30 tablets)<br/> </p>');
        } else {
            $('#MediSummary').append('<p class="mb-3"> Physician Consultation <br/> 1 prescription (30 day supply)<br/> </p>');
        }
    } else if (parseInt(time) == 1) {
        if (ishomedelivery) {
            $('#MediSummary').append('<p class="mb-3"> Physician Consultation <br/> 1 prescription (60 tablets)</p>');
        }
        else {
            $('#MediSummary').append('<p class="mb-3"> Physician Consultation <br/> 1 prescription (30 day supply)<br/> 1 refill (30 day supply)</p>');
        }
    } else if (parseInt(time) == 2) {
        if (ishomedelivery) {
            $('#MediSummary').append('<p class="mb-3"> Physician Consultation <br/> 1 prescription (90 tablets)</p>');
        }
        else {
            $('#MediSummary').append('<p class="mb-3"> Physician Consultation <br/> 1 prescription (30 day supply)<br/> 2 refills (30 day supply each)</p>');
        }
    }
    $('#TotalPrice').html(parseInt(price));
    $('#TotalPrice').parent().removeClass('hide-select');
    $('#ErrorFinishAccountSetup').hide();
}

/**************************Finish Account Setup End***********************************/

/**************************Choose Your Medication Start*******************************/
$(function () {
    //$(document).on('submit', '.formChooseMedicationStep1', function (event) {
    //    var $currentForm = $(this);
    //    var flag = 0;
    //    $currentForm.find('input[type=radio].chooseMedication').each(function () {
    //        if ($(this).prop("checked")) {
    //            flag = 1;
    //        }
    //    });
    //    if (flag == 0) {
    //        $currentForm.find('#ChooseYourMedicationError').show();
    //        return false;
    //    } else {
    //        let medicationId = $currentForm.find('#hdnMedicationId').val();
    //        if (medicationId == "" || medicationId == undefined || medicationId == "0") {
    //            $currentForm.find('#ChooseYourMedicationError').show();
    //            return false;
    //        }
    //    }
    //});

    $(document).on('click', '.formChooseMedicationStep1 input[type=radio].chooseMedication', function (event) {
        if ($(this).prop("checked")) {
            $(document).find('#hdnMedicationId').val($(this).attr('data-value'));
            $(document).find('[data-valmsg-for=Id]').empty();
        }
    });

    $('#formChooseYourMedication2').submit(function () {
        if ($('#hidMedicationName').val() == '') {
            $('#ChooseYourMedicationError').show();
            return false;
        } else {
            $('#ChooseYourMedicationError').hide();
        }
    });
});
function ChooseYourMedication(name, unit, quantity) {
    $('#hidMedicationName').val(name);
    $('#hidMedicationUnit').val(unit);
    $('#hidMedicationQuantity').val(quantity);

    $('#MediSummary').children().not(':first').remove();
    $('#MediSummary').append('<p>' + name + '</p>');
    $('#TotalPrice').html(unit);
    $('#ErrorFinishAccountSetup').hide();
}

function consulationOptions(name, unit, quantity, refill, price, description) {
    $('#hidMedicationName').val(name);
    $('#hidMedicationUnit').val(unit);
    $('#hidMedicationQuantity').val(quantity);
    $('#hidMedicationRefills').val(refill);
    $('#hidMedicationDescription').val(description);
    $('#hidMedicationTotal').val(price);
}

/****************************Choose Your Medication End*****************************/

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

function validate(evt) {
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    //var regex = /[0-9]|\./;
    var regex = /[0-9]/;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function onlyLetter(e) {
    var regex = new RegExp("^[a-zA-Z \s]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (regex.test(str)) {
        return true;
    }
    else {
        e.preventDefault();
        return false;
    }
}

$.fn.resetValidation = function () {

    var $form = this.closest('form');

    //reset jQuery Validate's internals
    $form.validate().resetForm();

    //reset unobtrusive validation summary, if it exists
    $form.find("[data-valmsg-summary=true]")
        .removeClass("validation-summary-errors")
        .addClass("validation-summary-valid")
        .find("ul").empty();

    //reset unobtrusive field level, if it exists
    $form.find("[data-valmsg-replace]")
        .removeClass("field-validation-error")
        .addClass("field-validation-valid")
        .empty();

    return $form;
};

$.fn.formReset = function (resetValidation) {
    var $form = this.closest('form');

    $form[0].reset();

    if (resetValidation == undefined || resetValidation) {
        $form.resetValidation();
    }

    return $form;
}

$(document).ready(function () {
    $(document).on('click', '.next', function () {
        $(".sliderImg.one_").removeClass("imgActive");
        $(".sliderImg.two_").addClass("imgActive");
        $(".sliderImg.three_").removeClass("imgActive");
        $(".next").addClass("two").removeClass("next");
        $(".prev").addClass("prevOne").removeClass("prev");
        $(".count").html("2 of 3");
    });
    $(document).on('click', '.two', function () {
        if ($(".count").html().trim() != '3 of 3') {

        }
        $(".sliderImg.one_").removeClass("imgActive");
        $(".sliderImg.three_").addClass("imgActive");
        $(".sliderImg.two_").removeClass("imgActive");
        $(".two").addClass("three").removeClass("two");
        $(".prevOne").addClass("prev").removeClass("prevOne");
        $(".count").html("3 of 3");
    });
    $(document).on('click', '.prev', function () {
        if ($(".count").html().trim() != '1 of 3') {
            $(".sliderImg.one_").removeClass("imgActive");
            $(".sliderImg.two_").addClass("imgActive");
            $(".sliderImg.three_").removeClass("imgActive");
            $(".prev").addClass("prevOne").removeClass("prev");
            $(".three").addClass("two").removeClass("three");
            $(".count").html("2 of 3");
        }
    });
    $(document).on('click', '.prevOne', function () {
        $(".sliderImg.one_").addClass("imgActive");
        $(".sliderImg.two_").removeClass("imgActive");
        $(".sliderImg.three_").removeClass("imgActive");
        $(".prevOne").addClass("prev").removeClass("prevOne");
        $(".two").addClass("next").removeClass("two");
        $(".count").html("1 of 3");
    });
});

// Start Consulation Page

function SetConsulation(element) {
    $(document).find('#hdnConsultationId').val($(element).attr('data-value'));
    $(document).find('#hdnIsStarted').val($(element).attr('is-started'));
    let date = new Date();
    let timeZone = date.getTimezoneOffset();
    $('#hdnTimeZone').val(timeZone);
    $(element).closest('form').submit();
}

function NewStyleDropdownForMedicationCategory() {
    $('#ddlmedicationcategory').each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        $customSelect.text($this.children('option').eq(0).text());

        var $optionlist = $('<ul />', { 'class': 'select-options', 'style': 'display:none' }).insertAfter($customSelect);

        for (var i = 0; i < selectOptions; i++) {
            $('<li/>', {
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
            GetMedication($(this).attr('rel'));
        });


        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });

    });
}
function NewStyleDropdownForMedication() {
    $('#ddlselectmedication').each(function () {
        var $this = $(this), selectOptions = $(this).children('option').length;
        $this.addClass('hide-select');
        $this.wrap('<div class="select"></div>');
        $this.after('<div class="custom-select"></div>');

        var $customSelect = $this.next('div.custom-select');
        $customSelect.text($this.children('option').eq(0).text());

        var $optionlist = $('<ul />', { 'class': 'select-options', 'style': 'display:none' }).insertAfter($customSelect);

        for (var i = 0; i < selectOptions; i++) {
            $('<li/>', {
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

            if ($(this).attr('rel') != '0') {
                $(document).find('#ErrorMedicCateShort').hide();
                $('#hdAnswer').val($(this).attr('rel'));
            }
        });

        $(document).click(function () {
            $customSelect.removeClass('active');
            $optionlist.hide();
        });

    });
}


// State Handler
function PharmacyDropdownChange(element) {
    if ($(element).val() != '0') {
        $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-error').addClass('field-validation-valid');
        $(document).find('span[data-valmsg-for="State"]').html('');
    } else {
        $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-valid').addClass('field-validation-error');
        $(document).find('span[data-valmsg-for="State"]').html('<span id="State-error" class="">Required</span>');
    }
}

function FinishAccountDropdownChange(element) {
    if ($(element).val() != '0') {
        $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-error').addClass('field-validation-valid');
        $(document).find('span[data-valmsg-for="State"]').html('');
    } else {
        $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-valid').addClass('field-validation-error');
        $(document).find('span[data-valmsg-for="State"]').html('<span id="State-error" class="">Required</span>');
    }
}

function FinishAccountDropdownChangeHairLoss(element) {
    if ($(element).val() != '0') {
        $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-error').addClass('field-validation-valid');
        $(document).find('span[data-valmsg-for="State"]').html('');
    } else {
        $(document).find('span[data-valmsg-for="State"]').removeClass('field-validation-valid').addClass('field-validation-error');
        $(document).find('span[data-valmsg-for="State"]').html('<span id="State-error" class="">Required</span>');
    }
}

$(function () {

    $(document).on('input', '#State', function () {
        var selectState = $(this).find('option:selected').text();
        $(this).siblings('.custom-select').html(selectState);
    });

    $(document).on('input', '#finishAccountSetupModel_State', function () {
        var selectState = $(this).find('option:selected').text();
        $(this).siblings('.custom-select').html(selectState);
    });

    let pathName = window.location.pathname.toLowerCase();
    if (pathName.indexOf('paymentsuccesschat') != -1) {
        $('body').addClass('payment-background');
    }
});




