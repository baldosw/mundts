

$(document).ready(function () {
    let tableId = "#customerDatatable";
    let table;

    function initializeTable() {
        table = $(tableId).DataTable({
            "processing": true,
            "serverSide": true,
            "filter": true,
            "pageLength": 10, // Number of rows per page
            "ajax": {
                "url": "/api/customer",
                "type": "POST",
                "datatype": "json",
                "data": function (d) {
                    return $.extend({}, d, {
                        "start": d.start,
                        "length": d.length
                    });
                }
            },
            "columnDefs": [{
                "targets": [0],
                "visible": false,
                "searchable": true
            }],
            "columns": [
                { "data": "id", "name": "Id", "autoWidth": true },
                { "data": "firstName", "name": "First Name", "autoWidth": true },
                { "data": "lastName", "name": "Last Name", "autoWidth": true },
                { "data": "contact", "name": "Contact", "autoWidth": true },
                { "data": "email", "name": "Email", "autoWidth": true },
                { "data": "dateOfBirth", "name": "Date Of Birth", "autoWidth": true },
                {
                    "data": null,
                    "render": function (data, type, row) {
                        return "<a href='#' class='btn btn-danger' onclick=DeleteCustomer('" + row.id + "'); >Delete</a>";
                    },
                    "autoWidth": true
                }
            ],
            "initComplete": function (settings, json) {
                console.log(json)
                let itemCount = json.recordsTotal; // Total number of items retrieved
                console.log("Number of items retrieved: " + itemCount);
            }
        });
    }

    function refreshTableData() {
        table.ajax.reload(null, false); // false to keep the current page
    }

    if ($.fn.dataTable.isDataTable(tableId)) {
        refreshTableData();
    } else {
        initializeTable();
    }
});