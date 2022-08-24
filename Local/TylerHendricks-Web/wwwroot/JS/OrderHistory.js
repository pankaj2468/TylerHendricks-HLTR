var tableRecent;
$(document).ready(function () {
    dataTableRecent();
    $('#txtSearch').keyup(function () {
        tableRecent.search($(this).val()).draw();
    });
});

function dataTableRecent() {
    tableRecent = $('#tblOrderHistory').DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "responsive": true,
        "autowidth": false,
        "filter": true, // this is for disable filter (search box)
        "order": [],
        "ajax": {
            "url": '' + PatientPortalUrl.GetOrderHistoryUrl + '',
            "type": "POST",
            "datatype": "json",
            "error": function (data) { }
        },
        "columns": [
            {
                "render": function (data, type, row) {
                    console.log(row)
                    if (row.requestMessage) {
                        return '<a href="' + PatientPortalUrl.MessagesPageUrl + '" class="new-msg-txt">New Message</a>'
                    } else {
                        return '';
                    }
                },
                "sortable": true
            },
            { "data": "request", "name": "Request", "autoWidth": false },
            { "data": "status", "name": "Status", "autoWidth": false },
            { "data": "requestedDate", "name": "RequestedDate", "autoWidth": false },
            { "data": "prescribeDate", "name": "PrescribeDate", "autoWidth": false },
            { "data": "refills", "name": "Refills", "autoWidth": false },
            {
                "render": function (data, type, row) {
                    return '$' + row.payments;
                }, "data": "payments", "name": "Payments", "autoWidth": false
            },
            {
                "render": function (data, type, row) {
                    let result;
                    result = '<a href="#!" class="light-grey-btn">Not applicable</a>'
                    return result;

                },
                "sortable": false
            }

        ],
        "filterOptions": { searchButton: "Search", clearSearchButton: "ClearSearch", searchContainer: "SearchContainer" }

    });

    $('#tblOrderHistory_length').hide();
    $('#tblOrderHistory_filter').hide();
}