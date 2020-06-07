$(function () {
    var $staffTable = $('#MyStaffTable');
    var $restaurantsTable = $('#MyRestaurantsTable');
    var $ddlFilter = $('#ddlFilter');
    var oid = $('#oid').val();
    var rid = $('#rid').val();

    // delete restaurant button 
    $('button.restaurants-delete').click(function (e) {
        e.preventDefault();
        var rrid = $(this).closest('tr').data('id');
        if (confirm("Deleting this restaurant will also delete the following:\n\n\u2022 Restaurant Webpage.\n\u2022 Bookings & Staff tied with this restaurant.\n\u2022 Restaurant Table information.\n\nClick OK to continue. To quit, click Cancel.")) {
            // delete the restaurant chosen 
            $.ajax({
                async: false,
                type: "POST",
                url: "menu.aspx/DeleteRestaurantAjax",
                data: "{'oid':'" + oid + "', 'rid':'" + rrid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $restaurantsTable.html((result.hasOwnProperty("d")) ? result.d : result);
                }
            });

            // refresh the filter
            $.ajax({
                async: false,
                type: "POST",
                url: "menu.aspx/RefreshRestIDs",
                data: "{'oid':'" + oid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $ddlFilter.html((result.hasOwnProperty("d")) ? result.d : result);
                }
            });
            $staffTable.css("display", "none");
        }
        return false;
    });

    // delete staff button
    $('button.staff-delete').click(function (e) {
        e.preventDefault();
        var wid = $(this).closest('tr').data('id');
        if (confirm("You are about to delete a member of Staff.\n\nClick OK to continue. To quit, click Cancel.")) {
            $.ajax({
                type: "POST",
                url: "menu.aspx/DeleteMemberAjax",
                data: "{'oid':'" + oid + "', 'rid':'" + rid + "', 'wid':'" + wid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $staffTable.html((result.hasOwnProperty("d")) ? result.d : result);
                }
            });
            $staffTable.css("display", "block");
        }
        return false;
    });
});