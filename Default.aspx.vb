Imports System.Data

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' connect to the database and load the cities of restaurants into the system 
        ' also hyperlink them so user can go to the link 
        Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
        Dim dtRegions As New DataTable

        If db.Execute("sp_GetRestaurantRegions") Then
            dtRegions = db.QueryResults
            Dim totalRegions As Integer = dtRegions.Rows.Count

            ' check if there ae any rows 
            If totalRegions > 0 Then
                Dim currRestCity As String
                Dim sb As New StringBuilder

                'loop through each row and add it to the form 
                For i = 0 To totalRegions - 1
                    ' store current city 
                    currRestCity = dtRegions.Rows(i)("City").ToString
                    sb.Append("<a href=""restaurants/view.aspx?city=" & Server.HtmlEncode(currRestCity) & """>" & currRestCity & "</a><br>")
                Next
                restaurantRegions.InnerHtml = sb.ToString
            End If
        End If

        ' cleanup resources
        dtRegions.Dispose()
        dtRegions = Nothing
        db = Nothing
    End Sub

    Protected Sub btnAdvancedSearch_Click(sender As Object, e As EventArgs) Handles btnAdvancedSearch.Click
        Response.Redirect("advancedsearch.aspx")
    End Sub
End Class
