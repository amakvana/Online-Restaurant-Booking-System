Option Explicit On
Option Strict On

Imports clsSecurity
Imports Microsoft.VisualBasic
Imports System.Data

Public Class clsRestaurants

    ' member variables
    Private db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 

    Public Sub New()
    End Sub

    Public Function GetFoodTypes() As DataTable
        db.Execute("sp_Restaurants_GetFoodTypes")
        Return db.QueryResults
    End Function

    Public Function AdvancedSearch(foodType As String) As DataTable
        ' clean the string 
        foodType = StripTags(foodType.Trim)

        db.AddParameter("@foodType", foodType)
        If db.Execute("sp_Restaurants_AdvSearch_NoLocation") Then
            Return db.QueryResults
        Else
            Return Nothing
        End If
    End Function

    Public Function AdvancedSearch(foodType As String, location As String) As DataTable
        ' clean the string 
        foodType = StripTags(foodType.Trim)
        location = StripTags(location.Trim)

        db.AddParameter("@foodType", foodType)
        db.AddParameter("@location", location)
        If db.Execute("sp_Restaurants_AdvSearch") Then
            Return db.QueryResults
        Else
            Return Nothing
        End If
    End Function

    Public Function GetTableSizes() As DataTable
        db.Execute("sp_Table_GetAmount")
        Return db.QueryResults
    End Function

    Public Function GetTimes(restaurantID As Integer) As DataTable
        db.AddParameter("@id", restaurantID.ToString)
        db.Execute("sp_Restaurants_GetTimesFromID")
        Return db.QueryResults
    End Function

End Class
