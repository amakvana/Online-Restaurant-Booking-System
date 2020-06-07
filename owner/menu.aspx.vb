Imports clsSecurity
Imports System.Data
Imports System.IO
Imports System.Web.Services

Partial Class owner_menu
    Inherits System.Web.UI.Page

    Dim owner As clsOwner

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("owner") IsNot Nothing Then
            owner = Session("owner")
            lblHeader.Text = "Welcome " & owner.FirstName
            PreventBackButtonCaching(Response)
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        btnRefreshRestaurants.Text = "↻"
        GetRestaurants()
        If IsPostBack Then
            ddlFilter = Session("filter")
        Else
            GetRestaurantIDs()
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Session("owner") IsNot Nothing Then
            Session("owner") = owner
            Session("filter") = ddlFilter
        End If
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        owner.LogOut() ' fires the LogOut method 
        Session("filter") = Nothing
        Session("owner") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

    Protected Sub btnRefreshRestaurants_Click(sender As Object, e As EventArgs) Handles btnRefreshRestaurants.Click
        GetRestaurants()
    End Sub

    Private Sub GetRestaurants()
        Dim dtRestaurantsInfo As New DataTable
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")
        Dim totalRestaurants As Integer

        db.AddParameter("@ownerID", owner.ID)
        db.Execute("sp_Owner_GetRestaurants")
        dtRestaurantsInfo = db.QueryResults
        totalRestaurants = dtRestaurantsInfo.Rows.Count

        ' if there are restaurants 
        MyRestaurantsTable.Rows.Clear()
        If totalRestaurants > 0 Then
            ' output the restaurants to the screen ..
            Dim thc1, thc2, thc3, thc4, thc5, thc6 As New TableHeaderCell
            Dim tblRow As New TableRow

            ' prepare headers
            thc1.Text = "Restaurant ID"
            thc2.Text = "Name"
            thc3.Text = "Address"
            thc4.Text = "City"
            thc5.Text = "Post Code"
            thc6.Text = " "
            tblRow.TableSection = TableRowSection.TableHeader
            ' add header cells to row
            tblRow.Cells.Add(thc1)
            tblRow.Cells.Add(thc2)
            tblRow.Cells.Add(thc3)
            tblRow.Cells.Add(thc4)
            tblRow.Cells.Add(thc5)
            tblRow.Cells.Add(thc6)
            ' add row to table
            MyRestaurantsTable.Rows.Add(tblRow)

            ' for each restaurant 
            For i = 0 To totalRestaurants - 1
                Dim tc1, tc2, tc3, tc4, tc5, tc6 As New TableCell
                tblRow = New TableRow
                ' get the data
                tc1.Text = dtRestaurantsInfo.Rows(i)("RestaurantID").ToString
                tc2.Text = dtRestaurantsInfo.Rows(i)("Name").ToString
                tc3.Text = dtRestaurantsInfo.Rows(i)("Address").ToString
                tc4.Text = dtRestaurantsInfo.Rows(i)("City").ToString
                tc5.Text = dtRestaurantsInfo.Rows(i)("PostCode").ToString
                tc6.Text = "<a href=""../" & dtRestaurantsInfo.Rows(i)("URL").ToString & """ class=""btn btn-xs btn-info"" target=""_blank""><i class=""glyphicon glyphicon-chevron-right""></i> View</a> <button class=""btn btn-xs btn-danger restaurants-delete""><i class=""glyphicon glyphicon-remove""></i> Delete</button>"
                ' add the table cells into the table row
                tblRow.Attributes.Add("data-id", Encrypt(dtRestaurantsInfo.Rows(i)("RestaurantID").ToString))
                tblRow.Cells.Add(tc1)
                tblRow.Cells.Add(tc2)
                tblRow.Cells.Add(tc3)
                tblRow.Cells.Add(tc4)
                tblRow.Cells.Add(tc5)
                tblRow.Cells.Add(tc6)
                ' display it in the table
                MyRestaurantsTable.Rows.Add(tblRow)
            Next
        Else
            ' no restaurants added
            Dim thc1 As New TableHeaderCell
            Dim tblRow As New TableRow

            thc1.Text = "No restaurants added"
            tblRow.TableSection = TableRowSection.TableHeader
            tblRow.Cells.Add(thc1)
            MyRestaurantsTable.Rows.Add(tblRow)
        End If

        ' output ID's to screen (for $.ajax)
        oid.Value = Encrypt(owner.ID)

        ' cleanup 
        dtRestaurantsInfo.Dispose()
        dtRestaurantsInfo = Nothing
        db = Nothing
    End Sub

    Private Sub GetRestaurantIDs()
        ddlFilter.DataSource = owner.GetRestaurantIDs(owner.ID)
        ddlFilter.DataTextField = "RestaurantID"
        ddlFilter.DataValueField = "RestaurantID"
        ddlFilter.DataBind()
    End Sub

    Private Sub GetStaff()
        Dim dtStaff As New DataTable
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")
        Dim totalStaff As Integer

        db.AddParameter("@ownerID", owner.ID)
        db.AddParameter("@restaurantID", ddlFilter.SelectedItem.Text)
        db.Execute("sp_Owner_GetWaiters")
        dtStaff = db.QueryResults
        totalStaff = dtStaff.Rows.Count

        ' if there are restaurants 
        MyStaffTable.Rows.Clear()
        If totalStaff > 0 Then
            ' output the restaurants to the screen ..
            Dim thc1, thc2, thc3, thc4, thc5 As New TableHeaderCell
            Dim tblRow As New TableRow

            ' prepare headers
            thc1.Text = "Waiter ID"
            thc2.Text = "Firstname"
            thc3.Text = "Surname"
            thc4.Text = "Email"
            thc5.Text = " "
            tblRow.TableSection = TableRowSection.TableHeader
            ' add header cells to row
            tblRow.Cells.Add(thc1)
            tblRow.Cells.Add(thc2)
            tblRow.Cells.Add(thc3)
            tblRow.Cells.Add(thc4)
            tblRow.Cells.Add(thc5)
            ' add row to table
            MyStaffTable.Rows.Add(tblRow)

            ' for each restaurant 
            For i = 0 To totalStaff - 1
                Dim tc1, tc2, tc3, tc4, tc5 As New TableCell
                tblRow = New TableRow
                ' get the data
                tc1.Text = dtStaff.Rows(i)("WaiterID").ToString
                tc2.Text = dtStaff.Rows(i)("Firstname").ToString
                tc3.Text = dtStaff.Rows(i)("Surname").ToString
                tc4.Text = dtStaff.Rows(i)("Email").ToString
                tc5.Text = "<button class=""btn btn-xs btn-danger staff-delete""><i class=""glyphicon glyphicon-remove""></i> Delete</button>"

                ' add the table cells into the table row
                tblRow.Attributes.Add("data-id", Encrypt(dtStaff.Rows(i)("WaiterID").ToString))
                tblRow.Cells.Add(tc1)
                tblRow.Cells.Add(tc2)
                tblRow.Cells.Add(tc3)
                tblRow.Cells.Add(tc4)
                tblRow.Cells.Add(tc5)
                ' display it in the table
                MyStaffTable.Rows.Add(tblRow)
            Next
        Else
            ' no restaurants added
            Dim thc1 As New TableHeaderCell
            Dim tblRow As New TableRow

            thc1.Text = "No waiters are signed up to this restaurant"
            tblRow.TableSection = TableRowSection.TableHeader
            tblRow.Cells.Add(thc1)
            MyStaffTable.Rows.Add(tblRow)
        End If

        ' output ID's to screen (for $.ajax)
        oid.Value = Encrypt(owner.ID)
        rid.Value = Encrypt(ddlFilter.SelectedItem.Text)

        ' cleanup 
        dtStaff.Dispose()
        dtStaff = Nothing
        db = Nothing
    End Sub

    Protected Sub btnGetStaff_Click(sender As Object, e As EventArgs) Handles btnGetStaff.Click
        GetStaff()
    End Sub

    ' handles deleting of the waiter using $.ajax
    <WebMethod>
    Public Shared Function DeleteMemberAjax(oid As String, rid As String, wid As String) As String
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim ownerID As Integer = Convert.ToInt32(StripTags(Decrypt(oid)))
        Dim restaurantID As Integer = Convert.ToInt32(StripTags(Decrypt(rid)))
        Dim waiterID As Integer = Convert.ToInt32(StripTags(Decrypt(wid)))
        Dim htmlOutput As String = ""
        Dim sb As New StringBuilder
        Dim dtStaff As New DataTable
        Dim totalStaff As Integer

        ' delete the record using the paramter data 
        db.AddParameter("@waiterID", waiterID)
        db.AddParameter("@restaurantID", restaurantID)

        ' if the record has been deleted successfully
        If db.Execute("sp_Owner_DeleteWaiter") Then
            ' get the new set of staff 
            db.AddParameter("@ownerID", ownerID)
            db.AddParameter("@restaurantID", restaurantID)
            db.Execute("sp_Owner_GetWaiters")
            dtStaff = db.QueryResults
            totalStaff = dtStaff.Rows.Count

            ' if there are bookings 
            If totalStaff > 0 Then
                ' prepare headers
                sb.Append("<thead><th>Waiter ID</th><th>Firstname</th><th>Surname</th><th>Email</th><th> </th></thead>")
                sb.Append("<tbody>")

                ' for each restaurant 
                For i = 0 To totalStaff - 1
                    sb.Append("<tr data-id=" & Encrypt(dtStaff.Rows(i)("WaiterID").ToString) & ">")
                    sb.Append("<td>" & dtStaff.Rows(i)("WaiterID").ToString & "</td>")
                    sb.Append("<td>" & dtStaff.Rows(i)("Firstname").ToString & "</td>")
                    sb.Append("<td>" & dtStaff.Rows(i)("Surname").ToString & "</td>")
                    sb.Append("<td>" & dtStaff.Rows(i)("Email").ToString & "</td>")
                    sb.Append("<td><button class=""btn btn-xs btn-danger staff-delete""><i class=""glyphicon glyphicon-remove""></i> Delete</button></td>")
                    sb.Append("</tr>")
                Next
                sb.Append("</tbody>")
            Else
                ' no restaurants added
                sb.Append("<thead><th>No staff available</th></thead>")
            End If
            htmlOutput = sb.ToString
        End If

        ' cleanup resources
        dtStaff.Dispose()
        sb = Nothing
        db = Nothing
        dtStaff = Nothing

        ' return the output 
        Return htmlOutput
    End Function

    ' handles deleting of the restaurant using $.ajax
    <WebMethod>
    Public Shared Function DeleteRestaurantAjax(oid As String, rid As String) As String
        Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
        Dim db As New clsSQLServer(baseDir & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim ownerID As Integer = Convert.ToInt32(StripTags(Decrypt(oid)))
        Dim restaurantID As Integer = Convert.ToInt32(StripTags(Decrypt(rid)))
        Dim htmlOutput As String = ""
        Dim sb As New StringBuilder
        Dim dtRestaurantsInfo As New DataTable
        Dim totalRestaurants As Integer
        Dim restaurantURLPath As String

        ' first, we want to delete the webpage that the restaurant has.. We get the URL information from the DB, extract it, find the paths and delete them 
        db.AddParameter("@ownerID", ownerID)
        db.AddParameter("@restaurantID", restaurantID)
        db.Execute("sp_Owner_GetRestaurantURLs")
        restaurantURLPath = db.QueryResults.Rows(0)("URL").ToString.Replace("/", "\")
        File.Delete(baseDir & restaurantURLPath)
        File.Delete(baseDir & restaurantURLPath & ".vb")

        ' second, we delete all bookings, staff & delete the table information that belongs to the restaurant   
        db.AddParameter("@restaurantID", restaurantID)
        db.Execute("sp_Owner_DeleteBookingsStaffAndTableInfo")

        ' third, we delete the actual restaurant record itself  
        db.AddParameter("@restaurantID", restaurantID)
        db.Execute("sp_Owner_DeleteRestaurant_FromID")

        ' finally, we want to refresh the restaurants list & populate it into a nice html format 
        db.AddParameter("@ownerID", ownerID)
        db.Execute("sp_Owner_GetRestaurants")
        dtRestaurantsInfo = db.QueryResults
        totalRestaurants = dtRestaurantsInfo.Rows.Count
        If totalRestaurants > 0 Then
            ' prepare headers
            sb.Append("<thead><th>Restaurant ID</th><th>Name</th><th>Address</th><th>City</th><th>Post Code</th><th> </th></thead>")
            sb.Append("<tbody>")

            ' for each restaurant 
            For i = 0 To totalRestaurants - 1
                sb.Append("<tr data-id=" & Encrypt(dtRestaurantsInfo.Rows(i)("RestaurantID").ToString) & ">")
                sb.Append("<td>" & dtRestaurantsInfo.Rows(i)("RestaurantID").ToString & "</td>")
                sb.Append("<td>" & dtRestaurantsInfo.Rows(i)("Name").ToString & "</td>")
                sb.Append("<td>" & dtRestaurantsInfo.Rows(i)("Address").ToString & "</td>")
                sb.Append("<td>" & dtRestaurantsInfo.Rows(i)("City").ToString & "</td>")
                sb.Append("<td>" & dtRestaurantsInfo.Rows(i)("PostCode").ToString & "</td>")
                sb.Append("<td><a href=""../" & dtRestaurantsInfo.Rows(i)("URL").ToString & """ class=""btn btn-xs btn-info"" target=""_blank""><i class=""glyphicon glyphicon-chevron-right""></i> View</a> <button class=""btn btn-xs btn-danger staff-delete""><i class=""glyphicon glyphicon-remove""></i> Delete</button></td>")
                sb.Append("</tr>")
            Next
            sb.Append("</tbody>")
        Else
            sb.Append("<thead><th>No restaurants available</th></thead>")
        End If
        htmlOutput = sb.ToString

        ' cleanup resources
        dtRestaurantsInfo.Dispose()
        sb = Nothing
        db = Nothing
        dtRestaurantsInfo = Nothing

        ' return the output 
        Return htmlOutput
    End Function

    ' handles refreshing dropdown filter using $.ajax
    <WebMethod>
    Public Shared Function RefreshRestIDs(oid As String) As String
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim ownerID As Integer = Convert.ToInt32(StripTags(Decrypt(oid)))
        Dim dtRestaurantIDs As New DataTable
        Dim totalRestaurantIDs As Integer
        Dim htmlOutput As String = ""
        Dim sb As New StringBuilder

        ' refresh the filter dropdownmenu
        db.AddParameter("@ownerID", ownerID)
        db.Execute("sp_Owner_GetRestaurantIDs")
        dtRestaurantIDs = db.QueryResults
        totalRestaurantIDs = dtRestaurantIDs.Rows.Count
        If totalRestaurantIDs > 0 Then
            For i = 0 To totalRestaurantIDs - 1
                sb.Append(String.Format("<option value=""{0}"">{0}</option>", dtRestaurantIDs.Rows(i)("RestaurantID").ToString))
            Next
            htmlOutput = sb.ToString
        End If

        ' cleanup resources
        dtRestaurantIDs.Dispose()
        sb = Nothing
        db = Nothing
        dtRestaurantIDs = Nothing

        ' return the output 
        Return htmlOutput
    End Function

End Class
