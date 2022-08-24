var autoRefreshInterval;
var intervalTime = 10000;

$(function () {

    var $consultationDropdown = $(document).find('#ddlConsultations');

    AutoRefresh();

    $(document).on('click', '#BtnSaveChat', function () {
        var chatMessage = $(document).find('#txtChatArea').val();
        if (chatMessage != '') {
            $.ajax({
                type: 'POST',
                url: '' + PatientPortalUrl.PatientMessageUrl + '',
                data: { message: chatMessage, ConsultationCategoryId: $consultationDropdown.val() },
                beforeSend: function () { $(document).find('#Loader').show(); },
                complete: function () { $(document).find('#Loader').hide(); },
                success: function (response) {
                    if (response.succeeded) {
                        $.when(GetMessages($consultationDropdown.val())).then(function () {
                            AutoRefresh();
                        });
                    }
                    else {
                        alert('Please try again!!');
                    }
                },
                error: function (response) {
                    alert('Please try again!!');
                }
            });
        } else {
            Swal.fire({
                title: 'Chat box is empty!',
                text: 'Please enter any message.',
                icon: 'warning',
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
    });

    $(document).on('change', '#ddlConsultations', function () {
        $.when(GetMessages($(this).val())).then(function () {
            AutoRefresh();
        });
    });

    $(document).find('#ddlConsultations').each(function () {
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

    /********* Clear Interval On Document Element Events *******/
    $('body').bind('scroll click change touchstart input', function (e) {
        if (e.originalEvent === undefined) {

        }
        else {
            clearInterval(autoRefreshInterval);
        }
    });

    function AutoRefresh() {
        autoRefreshInterval = setInterval(GetMessages($(document).find('#ddlConsultations').val()), intervalTime);
    }

    function GetMessages(consultationCategoryId) {
        $.ajax({
            type: "GET",
            url: '' + PatientPortalUrl.BindMessagesUrl + '',
            data: { ConsultationCategoryId: consultationCategoryId },
            success: function (response) {
                $(document).find('#divMessageContent').html(response);
                return 1;
            },
            error: function (response) {
                return 0;
            }
        });
    }
});

function fileValidateMessagePage(fileElement,fileCount=1) {
    var flag = true;
    var data = new FormData();
    if (fileElement.files) {
        if (fileCount < fileElement.files.length) {
            Swal.fire({
                title: 'Maximum File Count',
                text: 'Maximum select 3 file(s)',
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
            return false;
        }
        $(fileElement.files).each(function (key, file) {
            let fileSize = file.size / 1024;
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
            let mimeType = file.type;
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
            $(fileElement.files).each(function (i, index) {
                data.append($(fileElement).attr('title') + '-' + i, index);
            });
            $.ajax({
                url: '' + PatientPortalUrl.MessageUploadUrl + '?ConsultationCategoryId=' + $(document).find('#ddlConsultations').val(),
                type: 'POST',
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () { $('#Loader').show(); },
                complete: function () { $('#Loader').hide(); },
                success: function (data) {
                    window.location.reload();
                }
            });
        } 
        return flag;
    }
}

 
