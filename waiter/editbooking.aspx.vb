Imports clsSecurity
Imports System.Data

Partial Class waiter_editbooking
    Inherits System.Web.UI.Page

    Dim waiter As clsWaiter
    Dim selectedBookingID As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("waiter") IsNot Nothing Then
            waiter = Session("waiter")
            selectedBookingID = Session("selectedBookingID")
            lblHeader.Text = "Welcome " & waiter.FirstName
            lblDashboard.InnerText = waiter.GetRestaurantName(waiter.RestaurantID) & " Dashboard"
            PreventBackButtonCaching(Response)
            Form.DefaultButton = btnUpdate.UniqueID
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        ' if has been reloaded
        If IsPostBack Then
            ' get the data from session
            ddlBookingIDs = Session("ddlBookingIDs")
        Else    ' otherwise fetch new data 
            ' get the ID's & store them into temp datatable
            Dim dtBookingIDs As DataTable = waiter.GetBookings(waiter.RestaurantID, waiter.WaiterID)

            ' sort the ID's as the data is fetched from another query
            dtBookingIDs.DefaultView.Sort = "BookingID ASC"
            dtBookingIDs = dtBookingIDs.DefaultView.ToTable

            ' assign datatable & bind it
            ddlBookingIDs.DataSource = dtBookingIDs
            ddlBookingIDs.DataTextField = "BookingID"
            ddlBookingIDs.DataValueField = "BookingID"
            ddlBookingIDs.DataBind()
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Session("waiter") IsNot Nothing Then
            Session("waiter") = waiter
            Session("ddlBookingIDs") = ddlBookingIDs
            Session("selectedBookingID") = selectedBookingID
        End If
    End Sub

    Protected Sub btnGetBookingInfo_Click(sender As Object, e As EventArgs) Handles btnGetBookingInfo.Click
        Dim dtBookingInfo As New DataTable

        ' check if booking ID has been selected
        If ddlBookingIDs.SelectedIndex <> -1 Then
            ' get the booking information 
            selectedBookingID = Convert.ToInt32(ddlBookingIDs.SelectedValue.ToString.Trim)
            dtBookingInfo = waiter.GetBookingInformation(selectedBookingID)

            ' output the data
            With dtBookingInfo
                txtName.Text = .Rows(0)("Firstname").ToString & " " & .Rows(0)("Surname").ToString
                lblBookingDate.Text = "New Booking Date: (Current Booking Date: " & .Rows(0)("Date").ToString.Substring(0, 10) & ")"
                lblBookingTime.Text = "New Booking Time: (Current Booking Time: " & .Rows(0)("Time").ToString.Substring(0, 5) & ")"
                txtTableSize.Text = .Rows(0)("NoOfSeats").ToString & " Seater"
                tid.Value = Encrypt(Convert.ToInt32(.Rows(0)("TableID").ToString))

                ' we now need populate the booking times with times available for the restaurant 
                GetTimes(Convert.ToInt32(.Rows(0)("RestaurantID").ToString))
            End With
            btnUpdate.Enabled = True
        End If

        dtBookingInfo.Dispose()
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        ' first we grab the data stored
        Dim newBookingDate As DateTime
        Dim newBookingTime As DateTime
        Dim tableID As Integer = Convert.ToInt32(StripTags(Decrypt(tid.Value.ToString)))
        Dim errors As New List(Of String)   ' store error messages

        ' validate data like normal

        ' check if the table id is valid (hacking attempt...)
        lblError.Text = ""
        lblSuccess.Text = ""
        If IsNumeric(tableID) Then
            ' check if date chosen is valid 
            If DateTime.TryParse(txtBookingDate.Text.Trim, Nothing) Then
                ' finally, check if the date is >= today 
                If DateTime.Parse(txtBookingDate.Text.Trim) >= DateTime.Today Then
                    newBookingDate = txtBookingDate.Text.Trim
                    newBookingDate = newBookingDate.ToString("dd/MM/yyyy")  ' force british standard
                Else
                    errors.Add("You cannot pick a date that is before today.")
                End If
            Else
                errors.Add("Please select a new booking date.")
            End If

            ' check if time chosen is valid
            If ddlBookingTime.SelectedIndex <> -1 Then
                ' check if the time is corect 
                If DateTime.TryParseExact(ddlBookingTime.SelectedItem.Text, "HH:mm", Nothing, Globalization.DateTimeStyles.None, Nothing) Then
                    ' check if the time is >= the current hour 
                    If newBookingDate <> DateTime.Today Or DateTime.Parse(ddlBookingTime.SelectedItem.Text).Hour >= DateTime.Now.TimeOfDay.Hours Then
                        newBookingTime = DateTime.Parse(ddlBookingTime.SelectedItem.Text)
                    Else
                        errors.Add("You cannot pick a time that has already gone.")
                    End If
                Else
                    errors.Add("Please select valid booking time")
                End If
            Else
                errors.Add("Please select a new booking time.")
            End If
        End If

        ' check if validation checks successful 
        If errors.Count = 0 Then
            ' we then need to check if booking is available for the new date/time selected (get code from clsMember class)
            If waiter.UpdateMemberBookingAvailable(newBookingDate, newBookingTime, tableID) Then
                ' if all is good, amend the booking
                If waiter.UpdateMemberBooking(ddlBookingIDs.SelectedItem.Text, newBookingDate, newBookingTime) Then
                    lblSuccess.Text = "Success!"
                Else
                    lblError.Text = "Check all data has been filled correctly."
                End If
            Else
                lblError.Text = "Bookings at this date & time for this table are fully booked."
            End If
        Else
            ' otherwise print all errors out
            Dim totalErrors As Integer = errors.Count
            For i = 0 To totalErrors - 1
                lblError.Text &= errors(i) & "<br>"
            Next
        End If
    End Sub

    Sub GetTimes(restaurantID As Integer)
        Dim dtTimes As New DataTable
        Dim restaurant As New clsRestaurants
        Dim openTimeRaw As String
        Dim closeTimeRaw As String
        Dim currHour As String
        Dim openHour As Integer
        Dim closeHour As Integer

        ' first we get the opening/closing time of the restaurant 
        dtTimes = restaurant.GetTimes(restaurantID)
        openTimeRaw = dtTimes.Rows(0)("OpenTime").ToString
        closeTimeRaw = dtTimes.Rows(0)("CloseTime").ToString

        ' store only the hours so we can loop them and add times into the dropdownlist
        openHour = Integer.Parse(openTimeRaw.Trim.Substring(0, 2))
        closeHour = Integer.Parse(closeTimeRaw.Trim.Substring(0, 2))

        ' display them 
        ddlBookingTime.Items.Clear()
        For i = openHour To closeHour - 1
            ' check if opening hour 1 character long, if so we need to prepend a 0
            currHour = i
            If currHour.ToString.Length = 1 Then currHour = "0" & currHour
            ddlBookingTime.Items.Add(New ListItem(currHour & ":00", currHour & ":00")) ' add to dropdownlist
        Next
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        waiter.LogOut() ' fires the LogOut method 
        Session("waiter") = Nothing
        Session("ddlBookingIDs") = Nothing
        Session("selectedBookingID") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

End Class
