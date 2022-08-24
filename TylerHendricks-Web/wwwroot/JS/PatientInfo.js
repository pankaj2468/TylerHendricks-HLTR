var FilterMode = 0;
$(function () {
    dataTablePatient(FilterMode, '', '');

    var start = moment().subtract(6, 'days');
    var end = moment();

    $('input[name=filter]').change(function () {
        FilterMode = $(this).val();
        $('#tablePatientInfo').dataTable().fnDestroy();
        dataTablePatient(FilterMode,'','');
    });


    $('input[name=paymentFilter]').change(function () {
        FilterMode = $(this).val();
        var href = $(this).closest('#PaymentApproved').find('.btn-export-to-excel').attr('href').split('?')[0];
        $(this).closest('#PaymentApproved').find('.btn-export-to-excel').attr('href', href + '?FilterMode=' + FilterMode);
        if (FilterMode == "3") {
            $('#FilterControls1').hide();
            $('#FilterControls2').show();
        } else {
            $('input[name=filter]').eq(0).prop('checked', true);
            $('#FilterControls1').show();
            $('#FilterControls2').hide();
        }
        $('#tablePatientInfo').dataTable().fnDestroy();
        dataTablePatient(FilterMode, start.format('MMMM D, YYYY'), end.format('MMMM D, YYYY'));
    });

    $('#reportrange').daterangepicker({
        maxDate: new Date(),
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, weeklyCb);

    $('#reportrangePayment').daterangepicker({
        maxDate: new Date(),
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, paymentCb);

    weeklyCb(start, end);

    paymentCb(start, end);

    function weeklyCb(start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        dataTablePatientInitialRegistration(start.format('MMMM D, YYYY'), end.format('MMMM D, YYYY'));
    }

    function paymentCb(start, end) {
        $('#reportrangePayment span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        dataTablePatient(FilterMode, start.format('MMMM D, YYYY'), end.format('MMMM D, YYYY'));
    }
});

function dataTablePatient(filterId,startDate,endDate) {
    $('#tablePatientInfo').DataTable({
        "destroy": true,
        "pageLength": 50,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "responsive": true,
        "autowidth": false,
        "filter": true, // this is for disable filter (search box)
        "order": [],
        "ajax": {
            "url": '' + AdminPortalUrl.GetPatientRecordUrl + '?FilterMode=' + parseInt(filterId) + '&StartDate=' + startDate + '&EndDate=' + endDate,
            "type": "POST",
            "data": { "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            "datatype": "json",
            "error": function (data) { }
        },
        "columns": [
            {
                "render": function (data, type, row) {
                    if (row.isMDToolbox) {
                        return '<input cid="' + row.consultationId + '" type="checkbox" name="chkbox" class="patient-info-checkbox-md-tool" onclick="IsMDToolBox(this)" checked="checked">';
                    } else {
                        return '<input cid="' + row.consultationId + '" type="checkbox" name="chkbox" class="patient-info-checkbox-md-tool" onclick="IsMDToolBox(this)">';
                    }
                },
                "sortable": false
            },
            { "data": "firstName", "name": "FirstName", "autoWidth": false },
            { "data": "lastName", "name": "LastName", "autoWidth": false },
            { "data": "email", "name": "Email", "autoWidth": false },
            { "data": "registeredDate", "name": "RegisteredDate", "autoWidth": false },
            { "data": "submittedDate", "name": "SubmittedDate", "autoWidth": false },
            { "data": "address1", "name": "Address1", "autoWidth": false },
            { "data": "address2", "name": "Address2", "autoWidth": false },
            { "data": "city", "name": "City", "autoWidth": false },
            { "data": "state", "name": "State", "autoWidth": false },
            { "data": "zip", "name": "Zip", "autoWidth": false },
            { "data": "formatDobString", "name": "FormatDobString", "autoWidth": false },
            { "data": "patientPhone", "name": "PatientPhone", "autoWidth": false },
            { "data": "pharmacyName", "name": "PharmacyName", "autoWidth": false },
            { "data": "pharmacyAddress1", "name": "PharmacyAddress1", "autoWidth": false },
            { "data": "pharmacyAddress2", "name": "PharmacyAddress2", "autoWidth": false },
            { "data": "pharmacyCity", "name": "PharmacyCity", "autoWidth": false },
            { "data": "pharmacyState", "name": "PharmacyState", "autoWidth": false },
            { "data": "pharmacyZip", "name": "PharmacyZip", "autoWidth": false },
            { "data": "pharmacyPhone", "name": "PharmacyPhone", "autoWidth": false },
            { "data": "productPrice", "name": "ProductPrice", "autoWidth": false },
        ],
        "filterOptions": { searchButton: "Search", clearSearchButton: "ClearSearch", searchContainer: "SearchContainer" }
    });
    $('#tablePatientInfo_length').hide();
    $('#tablePatientInfo_filter').hide();
}

function dataTablePatientInitialRegistration(startDate,endDate) {
    $('#tablePatientInitialRegistration').DataTable({
        "destroy": true,
        "pageLength": 50,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "responsive": true,
        "autowidth": false,
        "filter": true, // this is for disable filter (search box)
        "order": [],
        "ajax": {
            "url": '' + AdminPortalUrl.GetPatientRecordInitialUrl + '?StartDate=' + startDate + '&EndDate=' + endDate,
            "type": "POST",
            "data": { "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            "datatype": "json",
            "error": function (data) { console.log(data); }
        },
        "columns": [
            { "data": "firstName", "name": "FirstName", "autoWidth": false },
            { "data": "lastName", "name": "LastName", "autoWidth": false },
            { "data": "email", "name": "Email", "autoWidth": false },
            { "data": "registrationDate", "name": "RegistrationDate", "autoWidth": false },
            { "data": "questionNaire", "name": "QuestionNaire", "autoWidth": false },


        ],
        "filterOptions": { searchButton: "Search", clearSearchButton: "ClearSearch", searchContainer: "SearchContainer" }
    });
    $('#tablePatientInitialRegistration_length').hide();
    $('#tablePatientInitialRegistration_filter').hide();
}

function IsMDToolBox(elements) {
    var consultationIdd = $(elements).attr('cid');
    $.ajax({
        url: '' + AdminPortalUrl.ChangeMDToolBoxUrl + '',
        type: "POST",
        data: { consultationId: consultationIdd, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
        beforeSend: function () { $('#Loader').show(); },
        complete: function () { $('#Loader').hide(); },
        success: function (response) {
            if (response.succeeded) {
                $('#tablePatientInfo').dataTable().fnDestroy();
                dataTablePatient(FilterMode);
            }
        },
        error: function (response) {

        }
    });
}

function openCity(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
}