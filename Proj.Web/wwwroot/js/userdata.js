var dataTable;
$(document).ready(function () {
    loadDataTabel();
});

function loadDataTabel() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/User/GetAll'},
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'company.name', "width": "10%" },
            { data: 'role', "width": "10%" },
            {
                data: { id:'id',lockoutEnd : "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getDate();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                            <a onclick="LockUnlock('${data.id}')" class="btn btn-sm btn-success"><i class="bi bi-unlock-fill"></i> UnLock</a> | 
                            <a href="#" class="btn btn-sm btn-primary">Permission</a>
                        `
                    }
                    else {
                        return `
                            <a onclick="LockUnlock('${data.id}')" class="btn btn-sm btn-danger"><i class="bi bi-lock-fill"></i> Lock</a> | 
                            <a class="btn btn-sm btn-primary">Permission</a>
                        `
                    }
                    
                }
            },
        ]
    });
}


function LockUnlock(id) {
    alert(id);
    //Swal.fire({
    //    title: 'Are you sure?',
    //    text: "You won't be able to revert this!",
    //    icon: 'warning',
    //    showCancelButton: true,
    //    confirmButtonColor: '#3085d6',
    //    cancelButtonColor: '#d33',
    //    confirmButtonText: 'Yes, delete it!'
    //}).then((result) => {
    //    if (result.isConfirmed) {
    //        $.ajax({
    //            url: url,
    //            type: 'delete',
    //            success: function (response) {
    //                dataTable.ajax.reload();
    //                toastr.success(response.message);
    //            },
    //            error: function (response) {
    //                toastr.error(response.message);
    //            }

    //        })

    //    }
    //})
}