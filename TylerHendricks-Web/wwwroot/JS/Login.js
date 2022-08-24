$(document).ready(function () {
    alertMessage();
});

function SetTimeZone() {
    let date = new Date();
    let timeZone = date.getTimezoneOffset();
    $('#hdnTimeZone').val(timeZone);
    if (date.isDstObserved()) {
        $('#hdnDayLightSaving').val('true');
    }
    else {
        $('#hdnDayLightSaving').val('false');
    }
}

Date.prototype.stdTimezoneOffset = function () {
    var jan = new Date(this.getFullYear(), 0, 1);
    var jul = new Date(this.getFullYear(), 6, 1);
    return Math.min(jan.getTimezoneOffset(), jul.getTimezoneOffset());
}

Date.prototype.isDstObserved = function () {
    return this.getTimezoneOffset() > this.stdTimezoneOffset();
}

function alertMessage() {
    var message = $('#hiddenAlert').val();
    if (message=="1") {
        Swal.fire({
            title: 'Check your mail',
            text:'You should receive an email shortly with a link to reset your password',
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
    else
    if (message == "2")
    {
        Swal.fire({
            title: 'Password reset',
            text: 'Your password has been reset!',
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

function alertMedication() {
    var message = $('#hiddenAlert').val();
    if (message == "1") {
        Swal.fire({
            title: 'Choose Medication',
            text: 'Choose Your Medication!',
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
