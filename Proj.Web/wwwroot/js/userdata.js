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
                            <a onclick="LockUnlock('${data.id}','unlock_kro')" class="btn btn-sm btn-danger"><i class="bi bi-lock-fill"></i> Lock</a> | 
                            <a href="#" class="btn btn-sm btn-primary">Permission</a>
                        `
                    }
                    else {
                        return `
                            <a onclick="LockUnlock('${data.id}','lock_kro')" class="btn btn-sm btn-success"><i class="bi bi-unlock-fill"></i> UnLock</a> | 
                            <a class="btn btn-sm btn-primary">Permission</a>
                        `
                    }
                    
                }
            },
        ]
    });
}

function LockUnlock(id, action) {
    var message = "";
    var lockStatus = "";
    if (action == 'unlock_kro') {
        message = `
            <i class="bi bi-unlock-fill"></i> You Want to Unlock User 
        `;
        lockStatus = "Unlock it!";
    }
    if (action == 'lock_kro') {
        message = `
            <i class="bi bi-lock-fill"></i> You Want to Lock User 
        `;
        lockStatus = "Lock it!";
    }

    Swal.fire({
        title: lockStatus,
        html: `${message}`,  // Use the `html` option instead of `text`
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `Yes, ${lockStatus}`,
        allowHtml: true  // Add this option to allow HTML
    }).then((result) => {
        // Your remaining code
    });
}
