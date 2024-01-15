var dataTable;

$(document).ready(function () {
    loadDataTable();
})
function loadDataTable(){
    dataTable = $('#tblData').DataTable({
        pageLength:3,
        lengthMenu: [3, 6, 9, 12, 15],
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            {"data":"title","width":"15%"},
            {"data":"description","width":"15%"},
            {"data":"author","width":"15%"},
            {"data":"isbn","width":"15%"},
            { "data": "price", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                    <a href="/Admin/Product/Upsert/${data}" class="btn btn-info">
                    <i class="fas fa-edit"></i>
                    </a>
                    <a class="btn btn-danger" onclick=Delete("/Admin/Product/Delete/${data}")>
                    <i class="fas fa-trash-alt"></i>
                    </a>
                    </div>
                    `;
                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Want To Delete Data ?",
        text: "Data will be permanently deleted",
        icon: "info",
        buttons: true,
        dangerModel:true
    }).then((willDelete)=> {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
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