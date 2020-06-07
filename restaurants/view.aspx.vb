Imports System.Data
Imports clsSecurity

Partial Class restaurants_view
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' connect to the database and load the cities of restaurants into the system 
        ' also hyperlink them so user can go to the link 

        ' check if the user has come from the Default.aspx page 
        If Request.QueryString("city") IsNot Nothing Then
            ' strip all html elements & content (xss) & store cleaned value 
            Dim cityName As String = StripTagsAndContent(Server.HtmlDecode(Request.QueryString("city"))).Trim
            Dim db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 
            Dim dtRestaurants As New DataTable

            db.AddParameter("@city", cityName)
            If db.Execute("sp_Restaurants_GetFromCity") Then
                dtRestaurants = db.QueryResults
                Dim totalRegions As Integer = dtRestaurants.Rows.Count

                ' check if there are any rows 
                If totalRegions > 0 Then
                    Dim cityNameShort As String = cityName.Trim.ToLower.Replace(" ", "")
                    Dim currRestName As String
                    Dim currRestNameShort As String
                    Dim sb As New StringBuilder

                    'loop through each row and add it to the form 
                    For i = 0 To totalRegions - 1
                        ' store current city 
                        currRestName = dtRestaurants.Rows(i)("Name")
                        currRestNameShort = currRestName.Trim.ToLower.Replace(" ", "")
                        sb.Append("<a href=""" & cityNameShort & "/" & currRestNameShort & ".aspx"">" & currRestName & "</a><br>")
                    Next
                    restaurantNames.InnerHtml = sb.ToString
                End If
            End If
        Else
            Response.Redirect("../Default.aspx")
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Response.Redirect("../Default.aspx")
    End Sub
End Class
