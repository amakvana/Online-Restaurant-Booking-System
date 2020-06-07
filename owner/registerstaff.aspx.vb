Imports clsSecurity
Imports System.Data

Partial Class owner_registerstaff
    Inherits System.Web.UI.Page

    Dim owner As clsOwner

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("owner") IsNot Nothing Then
            owner = Session("owner")
            lblHeader.Text = "Welcome " & owner.FirstName
            PreventBackButtonCaching(Response)
            Form.DefaultButton = btnRegister.UniqueID
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Session("owner") IsNot Nothing Then
            Session("owner") = owner
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Dim dtRestaurantNames As DataTable = owner.GetRestaurantNames(owner.ID)

        ' sort the datatable
        dtRestaurantNames.DefaultView.Sort = "Name ASC"
        dtRestaurantNames = dtRestaurantNames.DefaultView.ToTable

        ' populate list of restaurants
        ddlRestaurants.DataSource = dtRestaurantNames
        ddlRestaurants.DataTextField = "Name"
        ddlRestaurants.DataValueField = "RestaurantID"
        ddlRestaurants.DataBind()
    End Sub

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        If ddlRestaurants.SelectedIndex <> -1 Then
            Dim restID As Integer = StripTags(ddlRestaurants.SelectedValue)
            Dim firstName As String = StripTags(txtFirstname.Text)
            Dim surname As String = StripTags(txtSurname.Text)
            Dim email As String = StripTags(txtEmail.Text)
            Dim password As String = StripTags(txtPassword.Text)
            Dim registerAs As String = StripTags(rblRegisterAs.SelectedItem.Text)
            Dim errors As New List(Of String)   ' store error messages 

            ' error checking & validation 
            If String.IsNullOrWhiteSpace(restID) Then errors.Add("Please select a restaurant")
            If String.IsNullOrWhiteSpace(firstName) Then errors.Add("Please enter your firstname")
            If String.IsNullOrWhiteSpace(surname) Then errors.Add("Please enter your surname")
            If Not isValidEmail(email) Then errors.Add("Please enter a valid email address")
            If String.IsNullOrWhiteSpace(password) Then errors.Add("Please enter a password")

            password = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(10)) ' hash the pass

            ' if no errors then continue otherwise display errors
            lblSuccess.Text = ""
            lblError.Text = ""
            If errors.Count = 0 Then
                ' lets insert it into the DB now then redirect back to the login page 
                owner.RegisterStaff(restID, firstName, surname, email, password, registerAs)
                lblSuccess.Text = "Success! You've registered a new staff"
            Else
                ' otherwise print all errors out
                Dim totalErrors As Integer = errors.Count
                For i = 0 To totalErrors - 1
                    lblError.Text &= errors(i).ToString & "<br>"
                Next
            End If
        Else
            lblError.Text = "Please select a restaurant"
        End If
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        owner.LogOut() ' fires the LogOut method 
        Session("owner") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

End Class
