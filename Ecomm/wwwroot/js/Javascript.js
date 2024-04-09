var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#productData").DataTable({
        "ajax": {
            "url": "Product/GetAll"
        },
        "columns": [
            {"data":"title", "width":"40%"},
            {
                "data": "description",
                "width": "50%",
                "render": function (data) {
                    // Limit description to 100 characters
                    return data.length > 100 ? data.substr(0, 50) + '...' : data;
                }
            },
            { "data":"price", "width":"10%"},
            { "data":"quantity", "width":"10%"},
            { "data":"author", "width":"10%"},
            { "data":"category.name", "width":"10%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="w-75 btn-group" role="group">
                    <a href="Product/Upsert?id=${data}"
                    class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>Edit</a>
                    <a onClick=Delete('Product/Delete/'+${data})
                    class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                    </div>
                    `
                }
            },
        ]
    });
}
function Delete(url) {
    Swal.fire({
        title: "Do you want to Delete This Product?",
        showCancelButton: true,
        confirmButtonText: "Delete",
        confirmButtonColor: '#3085d6',
        cancelButtonColor:'#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error("An error occurred while deleting the product.");
                    console.error(xhr.responseText);
                }
            });
        } else {
            toastr.info("Deletion canceled.");
        }
    });
}
