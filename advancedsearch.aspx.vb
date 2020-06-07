Imports clsSecurity
Imports System.Data

Partial Class advancedsearch
    Inherits System.Web.UI.Page

    Dim restaurants As New clsRestaurants

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' get food types of retaurants and populate them into the first listbox 
        If Not IsPostBack Then
            restaurants = New clsRestaurants
        Else
            restaurants = Session("restaurants")
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        ' populate the dropdown list with food types
        PopulateDropDownList()
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim typeOfFood As String
        Dim location As String

        ' if an option is chosen
        lblError.Text = ""
        If ddlTypeOfFood.SelectedIndex <> -1 Then
            typeOfFood = ddlTypeOfFood.SelectedValue
            location = StripTagsAndContent(txtLocation.Text.Trim)
            GetRestaurants(typeOfFood, location)
        Else
            lblError.Text = "Select a food type"
        End If
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        ' save globals
        Session("restaurants") = restaurants
    End Sub

    Private Sub PopulateDropDownList()
        ' first get all the types from the database
        ' loop through each record fetched, store it into an array, if there are multiple values in one record, split the string and insert them into the array too.
        ' remove duplicate data from array & alphabetically sort it 
        ' assign that to the dropdownlist 
        Using dtFoodTypes As DataTable = restaurants.GetFoodTypes()
            Dim totalFoodTypes As Integer = dtFoodTypes.Rows.Count
            Dim foodTypes As New List(Of String)
            Dim currFoodType As String
            Dim multipleFoodTypesPerRow() As String

            ' loop through each record 
            ddlTypeOfFood.Items.Clear()
            For i = 0 To totalFoodTypes - 1
                currFoodType = dtFoodTypes.Rows(i)("FoodType")

                ' if there are multiple types of food in the current record 
                If currFoodType.Contains(",") Then
                    ' split the string & add it into the temp array
                    multipleFoodTypesPerRow = currFoodType.Split(","c)
                    For j = 0 To multipleFoodTypesPerRow.Count - 1
                        foodTypes.Add(multipleFoodTypesPerRow(j).Trim)
                    Next
                Else
                    foodTypes.Add(currFoodType.Trim)
                End If
            Next

            ' remove duplicates & sort
            foodTypes = foodTypes.Distinct.ToList
            foodTypes.Sort()
            For Each ft As String In foodTypes
                ddlTypeOfFood.Items.Add(New ListItem(ft, ft))
            Next
        End Using
    End Sub

    Private Sub GetRestaurants(typeOfFood As String, location As String)
        ' query the database for all restaurants and their data based on 2 data provided
        Dim dtRestaurants As DataTable
        Dim sb As New StringBuilder

        If String.IsNullOrWhiteSpace(location) Then
            dtRestaurants = restaurants.AdvancedSearch(typeOfFood)
        Else
            dtRestaurants = restaurants.AdvancedSearch(typeOfFood, location)
        End If

        lblError.Text = ""
        lblError.Attributes.CssStyle.Add("display", "none")
        ' if the data is contains something
        If dtRestaurants IsNot Nothing Then
            ' process it 
            Dim totalRestaurantsReturned As Integer = dtRestaurants.Rows.Count
            If totalRestaurantsReturned > 0 Then
                SearchResults.Attributes.CssStyle.Add("display", "block")
                With dtRestaurants
                    For i = 0 To totalRestaurantsReturned - 1
                        sb.Append("<div class=""restaurant-container"">")
                        sb.Append("<div class=""restaurant-location"">")
                        sb.Append("<h3>" & .Rows(i)("Name").ToString & "</h3>")
                        sb.Append("<p>" & String.Format("{0}, {1} {2}", .Rows(i)("Address").ToString, .Rows(i)("City").ToString, .Rows(i)("PostCode").ToString) & "</p>")
                        sb.Append("<a class=""btn btn-sm btn-info"" href=""" & .Rows(i)("URL").ToString & """><i class=""glyphicon glyphicon-chevron-right""></i> View</a>")
                        sb.Append("</div>")
                        sb.Append("<div class=""restaurant-details clearfix"">")
                        sb.Append("<p><strong>Type Of Food</strong><br>" & .Rows(i)("FoodType").ToString & "</p>")
                        ' check if restaurant is open (based on current time)
                        If isRestaurantOpen(.Rows(i)("OpenTime").ToString, .Rows(i)("CloseTime").ToString) Then
                            sb.Append("<p><strong>Restaurant is open</strong><br>Closes at " & .Rows(i)("CloseTime").ToString.Substring(0, 5) & "</p>")
                        Else
                            sb.Append("<p><strong>Opens at</strong><br>" & .Rows(i)("OpenTime").ToString.Substring(0, 5) & "</p>")
                        End If
                        sb.Append("</div>")
                        sb.Append("</div>")
                    Next
                End With
                SearchResults.InnerHtml = sb.ToString
            Else
                ' no restaurants 
                SearchResults.Attributes.CssStyle.Add("display", "none")
                SearchResults.InnerHtml = ""
                lblError.Attributes.CssStyle.Add("display", "block")
                lblError.Text = "No restaurants found!"
            End If
        End If

        ' cleanup 
        dtRestaurants.Dispose()
        dtRestaurants = Nothing
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Response.Redirect("Default.aspx")
    End Sub

    Private Function isRestaurantOpen(openTime As DateTime, closeTime As DateTime) As Boolean
        Return DateTime.Now.TimeOfDay >= openTime.TimeOfDay AndAlso DateTime.Now.TimeOfDay <= closeTime.TimeOfDay
    End Function

End Class
