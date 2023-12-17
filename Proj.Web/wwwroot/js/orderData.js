var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTabel("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTabel("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTabel("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTabel("approved");
                }
                else {
                    loadDataTabel("all");
                }
            }
        }
    }
    
});

function loadDataTabel(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Order/GetAll?status=' + status },
        "columns": [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                            <a href="/Admin/Order/Upsert?id=${data}" class="btn btn-primary">Edit</a>
                        `}
            },
        ]
    });
}