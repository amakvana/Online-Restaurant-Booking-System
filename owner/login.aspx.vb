Imports System.Data
Imports clsSecurity

Partial Class owner_login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        ' lets make a connection to the DB 
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")
        Dim dt As New DataTable
        Dim email As String = StripTags(txtEmail.Text)  ' strip html and store email

        lblError.Text = ""
        db.AddParameter("@email", email)
        If db.Execute("sp_Owner_GetPassword_From_Email") Then   ' has the query ran successfully?
            dt = db.QueryResults    ' store data query has returned

            ' if there is a password returned
            If dt.Rows.Count = 1 Then
                Dim pass As String = StripTags(txtPassword.Text)    ' strip html tags and store pass
                Dim hash As String = dt.Rows(0)("Password").ToString    ' store password from db 

                ' do passwords match?
                If BCrypt.Net.BCrypt.Verify(pass, hash) Then
                    ' create a new session for this owner and redirect them 
                    Dim owner As New clsOwner(email, hash)
                    Session("owner") = owner
                    dt = Nothing  ' empty the datatable 
                    Response.Redirect("menu.aspx")
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
