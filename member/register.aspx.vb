Imports System.Data
Imports clsSecurity

Partial Class member_register
    Inherits System.Web.UI.Page

    Protected Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        ' Sanitise the imput first, we remove any HTML from the fields
        Dim firstname As String = StripTags(txtFirstname.Text.Trim)
        Dim surname As String = StripTags(txtSurname.Text.Trim)
        Dim email As String = StripTags(txtEmail.Text.Trim)
        Dim password As String = StripTags(txtPassword.Text)
        Dim verifyPass As String = StripTags(txtVerifyPass.Text)
        Dim errors As New List(Of String) ' this is where we wil store the errors caused by validation.

        ' check if the data entered is valid, if not we store the error messages 
        lblError.Text = ""
        lblError.Attributes.CssStyle.Add("display", "none")
        If String.IsNullOrWhiteSpace(firstname) Then errors.Add("Please enter your firstname")
        If String.IsNullOrWhiteSpace(surname) Then errors.Add("Please enter your surname")
        If Not isValidEmail(email) Then errors.Add("Please enter a valid email address")
        If String.IsNullOrWhiteSpace(password) Then errors.Add("Please enter a password")
        ' check passwords match 
        If (password = verifyPass) And (verifyPass = password) Then
            password = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(10))
        Else
            errors.Add("Please make sure your passwords match")
        End If

        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")

        ' then we need to check if a user with the same unique credentials has already registered..
        ' so we can check the email field....
        db.AddParameter("@email", email)
        If db.Execute("sp_Members_CheckIfExists") Then
            ' has a record been returned? If not, register otherwise error message 
            If db.QueryResults.Rows.Count <> 0 Then
                errors.Add("A user with that email address already exists.")
            End If
        End If

        ' if the error array contains nothing, then all data is valid 
        If errors.Count = 0 Then
            ' lets insert it into the DB now then redirect back to the login page 
            db.AddParameter("@firstname", firstname)
            db.AddParameter("@surname", surname)
            db.AddParameter("@email", email)
            db.AddParameter("@password", password)
            ' if the query was successfully executed 
            If db.Execute("sp_Members_AddNew") Then
                db = Nothing
                ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "Success", "alert(""Success! You will now be redirected to the login screen""); location.href='login.aspx';", True)
            End If
        Else
            ' otherwise print all errors out
            Dim totalErrors As Integer = errors.Count
            lblError.Attributes.CssStyle.Add("display", "block")
            For i = 0 To totalErrors - 1
                lblError.Text &= errors(i) & "<br>"
            Next
            db = Nothing
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Response.Redirect("login.aspx")
    End Sub
End Class
