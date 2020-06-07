Imports clsSecurity
Imports System.Data
Imports System.Web.Services

Partial Class owner_menu
    Inherits System.Web.UI.Page

    Dim waiter As clsWaiter

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("waiter") IsNot Nothing Then
            waiter = Session("waiter")
            lblHeader.Text = "Welcome " & waiter.FirstName
            lblDashboard.InnerText = waiter.GetRestaurantName(waiter.RestaurantID) & " Dashboard"
            PreventBackButtonCaching(Response)
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        btnRefreshBookings.Text = "↻"
        GetBookings()

        ' encrypt the waiter & restaurant ID's and save them onto the webpage
        wid.Value = Encrypt(waiter.WaiterID)
        wrid.Value = Encrypt(waiter.RestaurantID)
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Session("waiter") IsNot Nothing Then
            Session("waiter") = waiter
        End If
    End Sub

    Private Sub GetBookings()
        Dim dtBookings As DataTable
        Dim totalBookings As Integer
        Dim todaysDate As String = DateTime.Now.ToString("dd/MM/yyyy").Trim

        ' get booking based on filter
        If ddlFilter.SelectedItem.Text = "Today" Then
            dtBookings = waiter.GetBookings(waiter.RestaurantID, waiter.WaiterID, todaysDate)
        Else
            dtBookings = waiter.GetBookings(waiter.RestaurantID, waiter.WaiterID)
        End If

        totalBookings = dtBookings.Rows.Count

        ' if there are bookings 
        BookingsTable.Rows.Clear()
        If totalBookings > 0 Then
            ' output the bookings to the screen ..
            Dim thc1, thc2, thc3, thc4, thc5 As New TableHeaderCell
            Dim tblRow As New TableRow

            ' prepare headers
            thc1.Text = "Booking ID"
            thc2.Text = "Customer Name"
            thc3.Text = "Date"
            thc4.Text = "Time"
            thc5.Text = "Table Size"
            tblRow.TableSection = TableRowSection.TableHeader
            ' add header cells to row
            tblRow.Cells.Add(thc1)
            tblRow.Cells.Add(thc2)
            tblRow.Cells.Add(thc3)
            tblRow.Cells.Add(thc4)
            tblRow.Cells.Add(thc5)
            ' add row to table
            BookingsTable.Rows.Add(tblRow)

            ' for each booking
            For i = 0 To totalBookings - 1
                Dim tc1, tc2, tc3, tc4, tc5, tc6 As New TableCell
                tblRow = New TableRow
                ' get the data
                tc1.Text = dtBookings.Rows(i)("BookingID").ToString
                tc2.Text = dtBookings.Rows(i)("Firstname").ToString & " " & dtBookings.Rows(i)("Surname").ToString
                tc3.Text = dtBookings.Rows(i)("Date").ToString.Substring(0, 10)  ' shorten to dd/mm/yyyy
                tc4.Text = dtBookings.Rows(i)("Time").ToString.Substring(0, 5)  ' shorten to HH:mm
                tc5.Text = dtBookings.Rows(i)("NoOfSeats").ToString & " Seater"
                tc6.Text = "<button class=""btn btn-xs btn-success mark-off""><i class=""glyphicon glyphicon-ok""></i> Mark Off</button> <button class=""btn btn-xs btn-danger booking-cancel""><i class=""glyphicon glyphicon-remove""></i> Cancel</button>"
                ' add the table cells into the table row
                tblRow.Attributes.Add("data-id", Encrypt(dtBookings.Rows(i)("BookingID").ToString))
                tblRow.Cells.Add(tc1)
                tblRow.Cells.Add(tc2)
                tblRow.Cells.Add(tc3)
                tblRow.Cells.Add(tc4)
                tblRow.Cells.Add(tc5)
                tblRow.Cells.Add(tc6)
                ' display it in the table
                BookingsTable.Rows.Add(tblRow)
            Next
        Else
            ' no bookings currently 
            Dim thc1 As New TableHeaderCell
            Dim tblRow As New TableRow

            thc1.Text = "No bookings available"
            tblRow.TableSection = TableRowSection.TableHeader
            tblRow.Cells.Add(thc1)
            BookingsTable.Rows.Add(tblRow)
        End If
    End Sub

    ' sub procedure must be public & shared in order for $.ajax call to work 
    <WebMethod>
    Public Shared Function GetBookingsAjax(wid As String, wrid As String, filter As String) As String
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim waiterID As Integer = Convert.ToInt32(StripTags(Decrypt(wid)))
        Dim restaurantID As Integer = Convert.ToInt32(StripTags(Decrypt(wrid)))
        Dim htmlOutput As String = ""
        Dim sb As New StringBuilder
        Dim dtBookings As New DataTable
        Dim totalBookings As Integer

        ' check if the values are still integer (only hacking attempts)
        If IsNumeric(waiterID) And IsNumeric(restaurantID) Then

            ' if the filter is set at today only otherwise get all
            If filter = "Today" Then
                Dim todaysDate As String = DateTime.Now.ToString("dd/MM/yyyy").Trim
                Dim bookingDate As String = If(todaysDate.Length = 10, todaysDate & " 00:00:00", todaysDate)
                db.AddParameter("@restaurantID", restaurantID.ToString)
                db.AddParameter("@waiterID", waiterID.ToString)
                db.AddParameter("@bookingDate", bookingDate)
                db.Execute("sp_Waiter_GetBookings_AtDate")
            Else
                db.AddParameter("@restaurantID", restaurantID.ToString)
                db.AddParameter("@waiterID", waiterID.ToString)
                db.Execute("sp_Waiter_GetBookings")
            End If

            dtBookings = db.QueryResults
            totalBookings = dtBookings.Rows.Count

            ' if there are bookings 
            If totalBookings > 0 Then
                ' prepare headers
                sb.Append("<thead><th>Booking ID</th><th>Customer Name</th><th>Date</th><th>Time</th><th>Table Size</th></thead>")
                sb.Append("<tbody>")

                ' for each booking
                For i = 0 To totalBookings - 1
                    sb.Append("<tr data-id=" & Encrypt(dtBookings.Rows(i)("BookingID").ToString) & ">")
                    sb.Append("<td>" & dtBookings.Rows(i)("BookingID").ToString & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("Firstname").ToString & " " & dtBookings.Rows(i)("Surname").ToString & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("Date").ToString.Substring(0, 10) & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("Time").ToString.Substring(0, 5) & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("NoOfSeats").ToString & " Seater</td>")
                    sb.Append("<td><button class=""btn btn-xs btn-success mark-off""><i class=""glyphicon glyphicon-ok""></i> Mark Off</button> <button class=""btn btn-xs btn-danger booking-cancel""><i class=""glyphicon glyphicon-remove""></i> Cancel</button></td>")
                    sb.Append("</tr>")
                Next
                sb.Append("</tbody>")
            Else
                ' no bookings currently 
                sb.Append("<thead><th>No bookings available</th></thead>")
            End If
            htmlOutput = sb.ToString
        End If

        ' cleanup resources
        dtBookings.Dispose()
        sb = Nothing
        db = Nothing
        dtBookings = Nothing

        ' return the output 
        Return htmlOutput
    End Function

    <WebMethod>
    Public Shared Function MarkOffBookingAjax(bid As String, wid As String, wrid As String, filter As String) As String
        ' unfortunately since this is using AJAX, we cannot use our typical member object as it's been constructed outside of this function :(
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim bookingID As Integer = Convert.ToInt32(StripTags(Decrypt(bid)))
        Dim htmlOutput As String = ""

        ' check if the values are still integer (only hacking attempts)
        If IsNumeric(bookingID) Then
            ' remove the booking 
            db.AddParameter("@bookingID", bookingID)
            If db.Execute("sp_Waiter_MarkOffBooking") Then
                ' get fresh booking data 
                htmlOutput = GetBookingsAjax(wid, wrid, filter)
            End If
        End If

        ' return the output 
        Return htmlOutput
    End Function

    <WebMethod>
    Public Shared Function DeleteBookingAjax(bid As String, wid As String, wrid As String, filter As String) As String
        ' unfortunately since this is using AJAX, we cannot use our typical member object as it's been constructed outside of this function :(
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim bookingID As Integer = Convert.ToInt32(StripTags(Decrypt(bid)))
        Dim htmlOutput As String = ""

        ' check if the values are still integer (only hacking attempts)
        If IsNumeric(bookingID) Then
            ' remove the booking 
            db.AddParameter("@bookingID", bookingID)
            If db.Execute("sp_Waiter_RemoveBooking") Then
                ' get fresh booking data 
                htmlOutput = GetBookingsAjax(wid, wrid, filter)
            End If
        End If

        ' return the output 
        Return htmlOutput
    End Function

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        waiter.LogOut() ' fires the LogOut method 
        Session("waiter") = Nothing
        Session("ddlBookingIDs") = Nothing
        Session("selectedBookingID") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

    Protected Sub btnRefreshBookings_Click(sender As Object, e As EventArgs) Handles btnRefreshBookings.Click
        GetBookings()
    End Sub

    Protected Sub ddlFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFilter.SelectedIndexChanged
        GetBookings()
    End Sub

End Class
