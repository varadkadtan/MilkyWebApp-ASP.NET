var dataTable;

$(document).ready(function () {
    var url = window.location.search; //get complete url

    //nested if statement
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("pending")) {
            loadDataTable("pending");
        }
        else {
            if (url.includes("readyforpickup")) {
                loadDataTable("readyforpickup");
            }
            else {
                if (url.includes("completed")) {
                    loadDataTable("completed");
                }
                else {
                    if (url.includes("approved")) {
                        loadDataTable("approved");
                    }
                    else {
                        loadDataTable("all");
                    }
                }
            }
        }
    }

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status},
            //dataSrc: 'data' // Specify the data source property
        
        "columns": [
            { data: 'id', "width": "5%" },
            {
                data: 'orderDate',
                "width": "15%",
                "render": function (data) {
                    // Convert the date string to a Date object
                    var date = new Date(data);
                    // Get the individual components of the date
                    var day = date.getDate();
                    var month = date.getMonth() + 1; // Months are zero-based
                    var year = date.getFullYear();
                    // Format the date as DD-MM-YYYY
                    return (day < 10 ? '0' : '') + day + '-' + (month < 10 ? '0' : '') + month + '-' + year;
                }
            },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'orderStatus', "width": "5%" },
            { data: 'orderTotal', "width": "10%" },
            { data: 'uniqueCode', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}

