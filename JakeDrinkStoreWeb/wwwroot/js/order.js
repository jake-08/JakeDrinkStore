var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    } else if (url.includes("completed")) {
        loadDataTable("completed");
    } else if (url.includes("pending")) {
        loadDataTable("pending");
    } else if (url.includes("approved")) {
        loadDataTable("approved");
    } else {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    dataTable = $("#tblData").DataTable({
        ajax: {
            url: "/Admin/Order/GetAll?status=" + status,
        },
        columns: [
            { data: "id", "className": "align-middle" },
            { data: "name", "className": "align-middle" },
            { data: "phoneNumber", "className": "align-middle" },
            { data: "applicationUser.email", "className": "align-middle" },
            { data: "orderStatus", "className": "align-middle" },
            { data: "paymentStatus", "className": "align-middle" },
            {
                data: "orderTotal", "className": "align-middle",
                render: function (orderTotal) {
                    return '$ ' + orderTotal.toFixed(2)
                }
            },
            {
                data: "id",
                render: function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a class="btn btn-primary mx-2" href="/Admin/Order/Details?orderHeaderId=${data}">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                     `;
                },
                width: "5%",
            },
            {
                data: "id",
                render: function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a class="btn btn-danger mx-2" onClick="Delete('/Admin/Order/Delete?orderHeaderId=${data}')">
                                <i class="bi bi-trash3"></i>
                            </a>
                        </div>
                     `;
                },
                width: "5%",
            },
        ],
        "rowCallback": function (row, data, dataIndex) {
            if (data.orderStatus == "Cancelled") {
                $(row).addClass('text-danger');
            }
            if (data.orderStatus == "Completed") {
                $(row).addClass('text-success');
            }
            if (data.orderStatus == "Pending") {
                $(row).addClass('text-warning');
            }
            if (data.orderStatus == "Processing") {
                $(row).addClass('text-secondary');
            }
            if (data.orderStatus == "Approved") {
                $(row).addClass('text-primary');
            }
        },
        "order": [[0, "desc"]]
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