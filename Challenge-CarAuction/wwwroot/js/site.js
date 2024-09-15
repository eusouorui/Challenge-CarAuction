$(document).ready(function () {

    var totalHeaders = $('table thead th').length;

    $('table thead th').each(function (i) {
        if (i !== totalHeaders - 1) {
            var title = $(this).text();
            $(this).html(
                '<input type="text" placeholder="' + title + '" data-index="' + i + '" />'
            );
        }
    });

    var table = $('table').DataTable({
        paging: false,
    });

    $('table thead input').on('keyup', function () {
        var index = $(this).data('index');
        var value = $(this).val();
        table.column(index).search(value).draw();
    });
});