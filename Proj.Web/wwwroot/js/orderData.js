var dataTable;
$(document).ready(function () {
    loadDataTabel();

    function loadDataTabel() {
        dataTable = $('#tblData').DataTable({
            "ajax": { url: '/Admin/Company/GetAll' },
            "columns": [
                { data: 'id' , "width":"10%"},
                { data: 'name', "width": "20%" },
                { data: 'phonenumber', "width": "25%" },
                { data: 'applicationuser.email', "width": "20%" },
                { data: 'orderstatus', "width": "15%" },
                { data: 'ordertotal', "width": "10%" },
                {
                    data: 'id',
                    "render": function (data) {
                        return `
                            <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary">Edit</a>
                        `}
                },
            ]
        });
    }

});