var dataTable;
$(document).ready(function () {
    loadDataTabel();
});

function loadDataTabel() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/User/GetAll'},
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'company.name', "width": "15%" },
            //{ data: '', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                            <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary">Edit</a>
                        `}
            },
        ]
    });
}