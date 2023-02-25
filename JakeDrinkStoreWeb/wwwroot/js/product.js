var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "name", "className": "align-middle" }, // the data value should match the API Json key
            { "data": "brand", "className": "align-middle" },
            {
                "data": "listPrice", "className": "align-middle",
                render: function (listPrice) {
                    return '$ ' + listPrice.toFixed(2)
                }
            },
            {
                "data": "casePrize", "className": "align-middle",
                render: function (casePrize) {
                    return '$ ' + casePrize.toFixed(2)
                }
            },
            {
                "data": "bulkCasePrice", "className": "align-middle",
                render: function (bulkCasePrice) {
                    return '$ ' + bulkCasePrice.toFixed(2)
                }
            },
            { "data": "drinkType.name", "className": "align-middle" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a class="btn btn-primary mx-2" href="/Admin/Product/Upsert?id=${data}">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a class="btn btn-danger mx-2" onClick="Delete('/Admin/Product/Delete/${data}')">
                                <i class="bi bi-trash3"></i>
                            </a>
                        </div>
                     `
                }
            },
        ],
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