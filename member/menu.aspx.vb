Imports clsSecurity
Imports System.Data
Imports System.Web.Services

Partial Class member_menu
    Inherits System.Web.UI.Page

    Dim member As clsMembers

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("member") IsNot Nothing Then
            member = Session("member")
            lblHeader.Text = "Welcome " & member.FirstName
            PreventBackButtonCaching(Response)
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        ' save globals
        If Session("member") IsNot Nothing Then
            Session("member") = member
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        btnRefreshBookings.Text = "↻"
        mmid.Value = Encrypt(member.ID)
        GetBookings()
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        member.LogOut() ' fires the LogOut method 
        Session("member") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

    Private Sub GetBookings()
        Dim dtBookings As DataTable = member.GetBookings(member.ID)
        Dim totalBookings As Integer = dtBookings.Rows.Count

        ' if there are bookings 
        BookingsTable.Rows.Clear()
        If totalBookings > 0 Then
            ' output the bookings to the screen ..
            Dim thc1, thc2, thc3, thc4, thc5, thc6, thc7 As New TableHeaderCell
            Dim tblRow As New TableRow

            ' prepare headers
            thc1.Text = "Booking ID"
            thc2.Text = "Restaurant Name"
            thc3.Text = "City"
            thc4.Text = "Booking Date"
            thc5.Text = "Booking Time"
            thc6.Text = "Table Size"
            thc7.Text = " "
            tblRow.TableSection = TableRowSection.TableHeader
            ' add header cells to row
            tblRow.Cells.Add(thc1)
            tblRow.Cells.Add(thc2)
            tblRow.Cells.Add(thc3)
            tblRow.Cells.Add(thc4)
            tblRow.Cells.Add(thc5)
            tblRow.Cells.Add(thc6)
            tblRow.Cells.Add(thc7)
            ' add row to table
            BookingsTable.Rows.Add(tblRow)

            ' for each booking 
            For i = 0 To totalBookings - 1
                Dim tc1, tc2, tc3, tc4, tc5, tc6, tc7 As New TableCell
                tblRow = New TableRow
                ' get the data
                tc1.Text = dtBookings.Rows(i)("BookingID").ToString
                tc2.Text = dtBookings.Rows(i)("Name").ToString
                tc3.Text = dtBookings.Rows(i)("City").ToString
                tc4.Text = dtBookings.Rows(i)("Date").ToString.Substring(0, 10)  ' shorten to dd/mm/yyyy
                tc5.Text = dtBookings.Rows(i)("Time").ToString.Substring(0, 5)  ' shorten to HH:mm
                tc6.Text = dtBookings.Rows(i)("NoOfSeats").ToString & " Seater"
                tc7.Text = "<button class=""btn btn-xs btn-danger staff-delete""><i class=""glyphicon glyphicon-remove""></i> Delete</button>"
                '    restTableID = dtBookingInfo.Rows(i)("RestaurantTableID").ToString
                ' add the table cells into the table row
                tblRow.Attributes.Add("data-id", Encrypt(dtBookings.Rows(i)("BookingID").ToString))
                tblRow.Cells.Add(tc1)
                tblRow.Cells.Add(tc2)
                tblRow.Cells.Add(tc3)
                tblRow.Cells.Add(tc4)
                tblRow.Cells.Add(tc5)
                tblRow.Cells.Add(tc6)
                tblRow.Cells.Add(tc7)
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

    Protected Sub btnRefreshBookings_Click(sender As Object, e As EventArgs) Handles btnRefreshBookings.Click
        GetBookings()
    End Sub

    <WebMethod>
    Public Shared Function DeleteBookingAjax(mmid As String, bid As String) As String
        ' unfortunately since this is using AJAX, we cannot use our typical member object as it's been constructed outside of this function :(
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim memberID As Integer = Convert.ToInt32(StripTags(Decrypt(mmid)))
        Dim bookingID As Integer = Convert.ToInt32(StripTags(Decrypt(bid)))
        Dim htmlOutput As String = ""
        Dim sb As New StringBuilder
        Dim dtBookings As New DataTable
        Dim totalBookings As Integer

        ' delete the record using the paramter data 
        db.AddParameter("@memberID", memberID)
        db.AddParameter("@bookingID", bookingID)

        ' if the record has been deleted successfully
        If db.Execute("sp_Members_RemoveBooking") Then
            ' get the new set of bookings 
            db.AddParameter("@memberID", memberID)
            db.Execute("sp_Members_GetMemberBookings")

            dtBookings = db.QueryResults
            totalBookings = dtBookings.Rows.Count

            ' if there are bookings 
            If totalBookings > 0 Then
                ' prepare headers
                sb.Append("<thead><th>Waiter ID</th><th>Firstname</th><th>Surname</th><th>Email</th><th> </th></thead>")
                sb.Append("<tbody>")

                ' for each restaurant 
                For i = 0 To totalBookings - 1
                    sb.Append("<tr data-id=" & Encrypt(dtBookings.Rows(i)("BookingID").ToString) & ">")
                    sb.Append("<td>" & dtBookings.Rows(i)("BookingID").ToString & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("Name").ToString & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("City").ToString & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("Date").ToString.Substring(0, 10) & "</td>")
                    sb.Append("<td>" & dtBookings.Rows(i)("Time").ToString.Substring(0, 5) & "</td>")
                    sb.Append("<td><button class=""btn btn-xs btn-danger staff-delete""><i class=""glyphicon glyphicon-remove""></i> Delete</button></td>")
                    sb.Append("</tr>")
                Next
                sb.Append("</tbody>")
            Else
                ' no restaurants added
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

End Class
