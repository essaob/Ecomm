var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Order/GetAll"
        },
        "columns": [
            {"data":"id", "width":"40%"},
            { "data":"name", "width":"50%"},
            { "data":"phoneNumber", "width":"10%"},
            { "data":"applicationUser.email", "width":"10%"},
            { "data":"orderStatus", "width":"10%"},
            { "data":"orderTotal", "width":"10%"},
            
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
