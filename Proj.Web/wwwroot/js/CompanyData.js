var dataTable;
$(document).ready(function () {
    loadDataTabel();

    function loadDataTabel() {
        dataTable = $('#tblData').DataTable({
            "ajax": { url: '/Admin/Company/GetAll' },
            "columns": [
                { data: 'id' },
                { data: 'name' },
                { data: 'city' },
                { data: 'state' },
                { data: 'postalCode' },
                {
                    data: 'id',
                    "render": function (data) {
                        return `
                            <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary">Edit</a>
                            <a onClick=Delete("/Admin/Company/DeleteRecord?id=${data}") class="btn btn-danger">Delete</a>
                        `}
                },
            ]
        });
    }

});



function Delete(url)
{
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
                type: 'delete',
                success: function (response) {
                    dataTable.ajax.reload();
                    toastr.success(response.message);
                },
                error: function (response) {
                    toastr.error(response.message);
                }

            })
            
        }
    })
}