Imports clsSecurity
Imports System.Data

Partial Class member_addbooking
    Inherits System.Web.UI.Page

    Dim member As clsMembers
    Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 

    Const PERSON_QTY_LENIENCY As Integer = 2

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it & for non-refreshes on form  populate it, otherwise redirect back to login form 
        If Session("member") IsNot Nothing Then
            member = Session("member")
            lblHeader.Text = "Welcome " & member.FirstName
            PreventBackButtonCaching(Response)
            Form.DefaultButton = btnAdd.UniqueID
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Not IsPostBack Then
            PopulateForm()
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Session("member") IsNot Nothing Then
            Session("member") = member
        End If
    End Sub

    Sub PopulateForm()
        ' first lets populate the cities of the restaurants
        If db.Execute("sp_GetRestaurantRegions") Then
            ddlRestCity.DataSource = db.QueryResults
            ddlRestCity.DataTextField = "City"
            ddlRestCity.DataBind()
        End If

        ' get the data for the first city loaded into the dropdownlist 
        db.AddParameter("@city", ddlRestCity.SelectedValue)
        If db.Execute("sp_Restaurants_GetFromCity") Then
            ddlRestName.DataSource = db.QueryResults
            ddlRestName.DataTextField = "Name"
            ddlRestName.DataValueField = "RestaurantID"
            ddlRestName.DataBind()
        End If

        GetTableAmount()

        ' now we need to get available times, these are based on the opening time-closing time of each restaurant..
        GetTimes(ddlRestName.SelectedValue)
    End Sub

    Sub GetTableAmount()
        Dim dtTables As New DataTable

        ' we then need to get the table size
        lblError.Text = ""
        db.AddParameter("@restID", ddlRestName.SelectedValue)
        If db.Execute("sp_Table_GetAmount_AtID") Then
            dtTables = db.QueryResults
            dtTables.DefaultView.Sort = "NoOfSeats ASC"
            dtTables = dtTables.DefaultView.ToTable
            ddlTableSize.DataSource = dtTables
            ddlTableSize.DataTextField = "NoOfSeats"
            ddlTableSize.DataValueField = "TableID"
            ddlTableSize.DataBind()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim bookingDate As DateTime

        ' check if date entered is in correct format 
        lblError.Text = ""
        lblSuccess.Text = ""
        If DateTime.TryParse(txtDate.Text.Trim, Nothing) Then
            bookingDate = DateTime.Parse(txtDate.Text.Trim)

            ' store & validate data 
            Dim memberID As Integer = StripTags(member.ID)
            Dim restaurantID As String = StripTags(ddlRestName.SelectedValue.Trim)
            Dim quantity As Integer = StripTags(txtNoOfPeople.Text.Trim)
            Dim tableID As Integer = StripTags(ddlTableSize.SelectedValue.Trim)
            Dim tableSize As Integer = StripTags(ddlTableSize.SelectedItem.Text.Trim)
            Dim time As DateTime
            If DateTime.TryParse(ddlTime.SelectedValue.Trim, Nothing) Then
                ' if the time is set in HH:mm format, then a length check is required pad it out so it becomes HH:mm:ss with ss defaulting to 00
                time = DateTime.Parse(ddlTime.SelectedValue.Trim)
                bookingDate = bookingDate.ToString("dd/MM/yyyy").Trim  ' force british standard

                ' check if the date selected is today or in the future 
                If bookingDate >= DateTime.Today Then

                    ' check if the time is also at the current time or in future 
                    If bookingDate <> DateTime.Today Or time.Hour >= DateTime.Now.TimeOfDay.Hours Then

                        ' if table size selected is:(totalPeople = tblSize) OR (tblSize >= (totalPeople+2)
                        If ((tableSize - quantity) <= PERSON_QTY_LENIENCY) And ((tableSize - quantity) >= 0) Then

                            'check if there is an existing booking at the same time
                            ' if not, then we can make a booking otherwise tell the user that the slot has been booked
                            If member.BookingAvailable(member.ID, bookingDate, time, tableID) Then
                                'now we need to get the restauranttableid from the tb to pass into the addbooking method
                                Dim restTableID As Integer
                                Dim dtIDs As New DataTable

                                'so we query the db based on the restaurantID picked, tableID (selected value from dropdown list) and return the restTableID to use later 
                                db.AddParameter("@restID", restaurantID)
                                db.AddParameter("@tableID", tableID)
                                db.Execute("sp_Members_GetRestaurantTableID")
                                dtIDs = db.QueryResults

                                ' if there is a record returned 
                                If dtIDs.Rows.Count > 0 Then
                                    restTableID = dtIDs.Rows(0)("RestaurantTableID")
                                    dtIDs.Dispose()
                                    dtIDs = Nothing

                                    'okay so we have an available slot.. Book it
                                    If member.AddNewBooking(restTableID, member.ID, bookingDate, time) Then
                                        lblSuccess.Text = "Success!"
                                    Else
                                        'if non of the requirements are made then the user cannot book a 4 seater table.
                                        lblError.Text = "Check all data has been filled correctly."
                                    End If
                                End If
                            Else
                                lblError.Text = "There are no more tables available at this time."
                            End If
                        Else
                            lblError.Text = "You can only reserve tables that are the same size as number of people or 2 higher."
                        End If
                    Else
                        lblError.Text = "You cannot pick a time that has already gone."
                    End If
                Else
                    lblError.Text = "You cannot pick a date that is before today."
                End If
            Else
                lblError.Text = "Please select a valid time."
            End If
        Else
            lblError.Text = "Please select a valid date (DD/MM/YYYY)."
        End If
    End Sub

    Protected Sub ddlRestCity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRestCity.SelectedIndexChanged
        ' when the city is changed, we update the restaurant names for the listbox below
        db.AddParameter("@city", ddlRestCity.SelectedValue)
        If db.Execute("sp_Restaurants_GetFromCity") Then
            ddlRestName.DataSource = db.QueryResults
            ddlRestName.DataTextField = "Name"
            ddlRestName.DataValueField = "RestaurantID"
            ddlRestName.DataBind()
        End If
        GetTableAmount()
        GetTimes(ddlRestName.SelectedValue)
    End Sub

    Protected Sub ddlRestName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRestName.SelectedIndexChanged
        GetTableAmount()
        GetTimes(ddlRestName.SelectedValue)
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
        ddlTime.Items.Clear()
        For i = openHour To closeHour - 1
            ' check if opening hour 1 character long, if so we need to prepend a 0
            currHour = i
            If currHour.ToString.Length = 1 Then currHour = "0" & currHour
            ddlTime.Items.Add(New ListItem(currHour & ":00", currHour & ":00")) ' add to dropdownlist
        Next
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        member.LogOut() ' fires the LogOut method 
        Session("member") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

End Class
