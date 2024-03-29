﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/ApplicationUser/GetAll"
        },
        "columns": [
            { "data": "name", "className": "align-middle" }, // the data value should match the API Json key
            { "data": "email", "className": "align-middle" },
            { "data": "phoneNumber", "className": "align-middle" },
            { "data": "streetAddress", "className": "align-middle" },
            { "data": "suburb", "className": "align-middle" },
            { "data": "state", "className": "align-middle" },
            { "data": "postcode", "className": "align-middle" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a class="btn btn-primary mx-2" href="/Admin/ApplicationUser/Edit?id=${data}">
                            <i class="bi bi-pencil-square"></i>
                        </a>
                     `
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a class="btn btn-danger mx-2" onClick="Delete('/Admin/ApplicationUser/Delete/${data}')">
                            <i class="bi bi-trash3"></i>
                        </a>
                     `
                }
            },
        ],
        order: [[1, "asc"]]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}