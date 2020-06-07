
Partial Class waiter_Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Response.Redirect("login.aspx")
    End Sub
End Class
