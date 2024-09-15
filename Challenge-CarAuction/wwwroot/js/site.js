$(document).ready(function () {
    $('table thead th').each(function (i) {
        var title = $(this).text();
        $(this).html(
            '<input type="text" placeholder="' + title + '" data-index="' + i + '" />'
        );
    });

    // Initialize DataTables with FixedColumns
    var table = $('table').DataTable({
        paging: false,
    });

    // Add search functionality to header inputs
    $('table thead input').on('keyup', function () {
        var index = $(this).data('index');
        var value = $(this).val();
        table.column(index).search(value).draw();
    });
});