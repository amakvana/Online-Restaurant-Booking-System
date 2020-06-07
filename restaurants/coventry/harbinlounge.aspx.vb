Imports System.Data
Imports clsSecurity

Partial Class restaurant_harbinlounge
    Inherits System.Web.UI.Page

    Private Shared prevPage As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                prevPage = Request.UrlReferrer.ToString
            Catch ex As Exception
                prevPage = "../../Default.aspx"
            End Try
        End If
    End Sub

    Protected Sub btnBookNow_Click(sender As Object, e As EventArgs) Handles btnBookNow.Click
        Response.Redirect("../../member/login.aspx")
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Response.Redirect(prevPage)
    End Sub

End Class