Imports System.Text.RegularExpressions
Imports clsSecurity
Imports System.Data

Partial Class login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        ' lets make a connection to the DB 
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")
        Dim dt As New DataTable
        Dim email As String = StripTags(txtEmail.Text.Trim) ' strip the HTML out of the 

        ' check if email provided returns a password (means account is valid)
        db.AddParameter("@email", email)
        If db.Execute("sp_Members_GetPassword_From_Email") Then
            dt = db.QueryResults
            ' if there is a record 
            If dt.Rows.Count = 1 Then
                ' compare the hash and password and see if they match 
                Dim pass As String = StripTags(txtPassword.Text)
                Dim hash As String = dt.Rows(0)("password").ToString

                If BCrypt.Net.BCrypt.Verify(pass, hash) Then    ' if password matches hash
                    Dim member As New clsMembers(email, hash)   ' create new session for the member
                    Session("member") = member  ' store it in a session object 
                    dt = Nothing  ' empty the datatable 
                    Response.Redirect("menu.aspx")   ' redirect 
                Else
                    lblError.Text = "Login failed!"
                End If
            Else
                lblError.Text = "Login failed!"
            End If
        Else
            lblError.Text = "Login failed!"
        End If
    End Sub

End Class
