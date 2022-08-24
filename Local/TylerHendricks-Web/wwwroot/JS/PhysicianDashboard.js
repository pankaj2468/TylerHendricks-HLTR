var tableRecent;
var tableNoService;
var _recordType = 1;
$(document).ready(function () {
    var __RecordTypeQueryString = getUrlVars()["r"];
    if (__RecordTypeQueryString === undefined) {
        _recordType = 1;
    } else {
        _recordType = parseInt(__RecordTypeQueryString) == NaN ? 1 : parseInt(__RecordTypeQueryString);
    }

    $('.tabrecordtype').each(function () {
        $(this).children().first().removeClass();
        $(this).children().first().addClass('k1');
    });

    $('.tabrecordtype[recordtype="' + _recordType + '"]').children().first().removeClass('k1');
    $('.tabrecordtype[recordtype="' + _recordType + '"]').children().first().addClass('k3');
    if (_recordType != 8) {
        dataTableRecent();
        $('#tblPatientNotifyDashboard').hide();
        $('#tblPatientNotifyDashboard_wrapper').hide();
        $('#tblPatientDashboard').show();
        $('#tblPatientDashboard_wrapper').show();
    } else {
        dataTableNotServiceAvailable();
        $('#tblPatientNotifyDashboard').show();
        $('#tblPatientNotifyDashboard_wrapper').show();
        $('#tblPatientDashboard').hide();
        $('#tblPatientDashboard_wrapper').hide();
    }

    $('#txtSearch').keyup(function () {
        if (_recordType == 8) {
            tableNoService.search($(this).val()).draw();
        } else {
            tableRecent.search($(this).val()).draw();
        }
    });
    $('.tabrecordtype').click(function () {
        $('.tabrecordtype').each(function () {
            $(this).children().first().removeClass();
            $(this).children().first().addClass('k1');
        });
        $(this).children().first().removeClass('k1');
        $(this).children().first().addClass('k3');
        _recordType = $(this).attr('recordtype');
        $('#Loader').show();
        window.location.href = window.location.origin + window.location.pathname + '?r=' + _recordType;
    });
});

function dataTableRecent() {
    tableRecent = $('#tblPatientDashboard').DataTable({
        "dom": '<"top"iflp<"clear">>rt',
        "pageLength": 50,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "responsive": true,
        "autowidth": false,
        "filter": true, // this is for disable filter (search box)
        "order": [],
        "ajax": {
            "url": '' + PhysicianPortalUrl.PhysicianDashbordRecord + '',
            "type": "POST",
            "datatype": "json",
            "data": { RecordType: _recordType },
            "error": function (data) { }
        },
        "columns": [
            { "data": "rowId", "name": "RowId", "autoWidth": false },
            {
                "render": function (data, type, row) {
                    return '<a href = "' + PhysicianPortalUrl.PhysicianPatientChart + '?'+row.queryString+'" > ' + row.name + '</a >';
                },
                "data": "name",
                "name":"Name",
                "sortable": true
            },
            { "data": "consultationCateory", "name": "ConsultationCateory", "autoWidth": false },
            { "data": "dob", "name": "DOB", "autoWidth": false },
            { "data": "stateCode", "name": "StateCode", "autoWidth": false },
            { "data": "submitted", "name": "Submitted", "autoWidth": false },
            { "data": "requestedRx", "name": "RequestedRx", "autoWidth": false },
            { "data": "refills", "name": "Refills", "autoWidth": false },
            { "data": "tabStatus", "name": "TabStatus", "autoWidth": false },
            { "data": "waitInQuene", "name": "WaitInQuene", "autoWidth": false }
        ],
        "filterOptions": { searchButton: "Search", clearSearchButton: "ClearSearch", searchContainer: "SearchContainer" }
    });
    $('#tblPatientDashboard_filter').hide();
    $('#tblPatientDashboard_length').hide();
}
function dataTableNotServiceAvailable() {
    tableNoService = $('#tblPatientNotifyDashboard').DataTable({
        "dom": '<"top"iflp<"clear">>rt',
        "pageLength": 50,
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "responsive": true,
        "autowidth": false,
        "filter": true, // this is for disable filter (search box)
        "order": [],
        "ajax": {
            "url": '' + PhysicianPortalUrl.GetNotifyRecordsUrl + '',
            "type": "POST",
            "datatype": "json",          
            "error": function (data) { }
        },
        "columns": [
            { "data": "id", "name": "Id", "autoWidth": false },
            { "data": "email", "name": "Email", "autoWidth": false },
            { "data": "stateCode", "name": "StateCode", "autoWidth": false },       
        ],
        "filterOptions": { searchButton: "Search", clearSearchButton: "ClearSearch", searchContainer: "SearchContainer" }
    });
    $('#tblPatientNotifyDashboard_filter').hide();
    $('#tblPatientNotifyDashboard_length').hide();
    $('#tblPatientNotifyDashboard_wrapper').hide();
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}


