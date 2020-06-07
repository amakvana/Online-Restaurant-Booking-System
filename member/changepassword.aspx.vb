Imports clsSecurity
Imports System.Data

Partial Class member_changepassword
    Inherits System.Web.UI.Page

    Dim member As clsMembers

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("member") IsNot Nothing Then
            member = Session("member")
            lblHeader.Text = "Welcome " & member.FirstName
            PreventBackButtonCaching(Response)
            Form.DefaultButton = btnChange.UniqueID
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub btnChange_Click(sender As Object, e As EventArgs) Handles btnChange.Click
        ' do some validation and sanitise the input strings
        ' we then check to see if the current password entered is correct
        ' if so, check if new pass & verify match 
        ' if so, hash & replace old pass with new one 
        Dim currentPass As String
        Dim newPass As String
        Dim verifyPass As String

        lblError.Text = ""
        lblSuccess.Text = ""
        ' check if strings are empty, if so strip html and store it
        If Not String.IsNullOrWhiteSpace(txtCurrentPass.Text) Then
            currentPass = StripTags(txtCurrentPass.Text.Trim)

            If Not String.IsNullOrWhiteSpace(txtNewPass.Text) Then
                newPass = StripTags(txtNewPass.Text.Trim)

                If Not String.IsNullOrWhiteSpace(txtVerifyNewPass.Text) Then
                    verifyPass = StripTags(txtVerifyNewPass.Text.Trim)

                    ' ok so strings have passed validation checks, let's check to make sure current password is correct 
                    Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database
                    Dim dtPassword As New DataTable

                    db.AddParameter("@id", member.ID)
                    db.Execute("sp_Members_GetPassword_From_ID")
                    dtPassword = db.QueryResults

                    If dtPassword.Rows.Count = 1 Then
                        Dim hash As String = dtPassword.Rows(0)("Password").ToString

                        dtPassword.Dispose()
                        dtPassword = Nothing
                        'okay user has entered their current password correct 
                        If BCrypt.Net.BCrypt.Verify(currentPass, hash) Then
                            ' check the new pass matches verify
                            ' if so, hash & store it
                            If (newPass = verifyPass) And (verifyPass = newPass) Then
                                newPass = BCrypt.Net.BCrypt.HashPassword(newPass, BCrypt.Net.BCrypt.GenerateSalt(10))
                                db.AddParameter("@id", member.ID)
                                db.AddParameter("@password", newPass)

                                If db.Execute("sp_Members_ChangePassword") Then
                                    Dim newSession As New clsMembers(member.Email, newPass)
                                    Session("member") = newSession    ' override old session with updated one
                                    lblSuccess.Text = "Password has been updated successfully"
                                End If
                            Else
                                lblError.Text = "New password doesn't match verification"
                            End If
                        Else
                            lblError.Text = "Current password entered is incorrect"
                        End If
                    End If
                Else
                    lblError.Text = "Please verify the new password"
                End If
            Else
                lblError.Text = "Please enter a new password"
            End If
        Else
            lblError.Text = "Please enter your current password"
        End If
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        member.LogOut() ' fires the LogOut method 
        Session("member") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

End Class
