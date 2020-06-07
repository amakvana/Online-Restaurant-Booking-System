$(function () {
    var INTERVAL = 5000;  // ms 
    var $wid = $('#wid').val(),
        $wrid = $('#wrid').val(),
        $filter = $('#ddlFilter'),
        $bookingsTable = $('#BookingsTable');

    // function to pull the latest bookings at the interval set 
    setInterval(function () {
        $.ajax({
            type: "POST",
            url: "menu.aspx/GetBookingsAjax",
            data: "{'wid':'" + $wid + "', 'wrid':'" + $wrid + "', 'filter':'" + $filter.val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $bookingsTable.html((result.hasOwnProperty("d")) ? result.d : result);
            }
        });
    }, INTERVAL);

    // mark off booking button
    $('button.mark-off').click(function (e) {
        e.preventDefault();
        var bid = $(this).closest('tr').data('id');
        $.ajax({
            type: "POST",
            url: "menu.aspx/MarkOffBookingAjax",
            data: "{'bid':'" + bid + "', 'wid':'" + $wid + "', 'wrid':'" + $wrid + "', 'filter':'" + $filter.val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $bookingsTable.html((result.hasOwnProperty("d")) ? result.d : result);
            }
        });
        return false;
    });

    // cancel bookng button
    $('button.booking-cancel').click(function (e) {
        e.preventDefault();
        var bid = $(this).closest('tr').data('id');
        if (confirm("You are about to cancel this members booking.\nClick OK to continue. To keep this booking, click Cancel.")) {
            $.ajax({
                type: "POST",
                url: "menu.aspx/DeleteBookingAjax",
                data: "{'bid':'" + bid + "', 'wid':'" + $wid + "', 'wrid':'" + $wrid + "', 'filter':'" + $filter.val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $bookingsTable.html((result.hasOwnProperty("d")) ? result.d : result);
                }
            });
        }
        return false;
    });
});