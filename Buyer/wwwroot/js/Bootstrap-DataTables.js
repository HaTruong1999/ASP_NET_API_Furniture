var mSortingString = [];
var disableSortingColumn = 2;
mSortingString.push({ "bSortable": false, "aTargets": [disableSortingColumn] });

$(function() {
        var table = $('#example').dataTable({
            "paging": true,
            "ordering": true,
            "info": true,
            "aaSorting": [],
            "orderMulti": true,
            "aoColumnDefs": mSortingString

        });
});