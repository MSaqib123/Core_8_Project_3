var dataTable;
$(document).ready(function () {
    loadDataTabel();

    function loadDataTabel() {
        dataTable = $('#tblData').DataTable({
            "ajax": { url: '/Admin/Order/GetAll' },
            "columns": [
                { data: 'id' , "width":"10%"},
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

});