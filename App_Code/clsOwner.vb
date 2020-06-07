Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic
Imports System.Data

Public Class clsOwner
    ' member variables
    Private dtOwner As New DataTable
    Private db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 

    ' member variables to store user data into 
    Private _id As Integer       ' store current id
    Private _firstName As String       ' store current firstname
    Private _lastName As String       ' store current lastname
    Private _email As String       ' store current email
    Private _password As String       ' store current password
    Private _authenticated As Boolean       ' store if they're authenticated or not

    Public Sub New(email As String, password As String)
        ' pass the parameters required in the stored procedure & their values 
        db.AddParameter("@Email", email)
        db.AddParameter("@Password", password)
        db.Execute("sp_Owner_GetLoginDetails")       ' execute the stored procedure
        dtOwner = db.QueryResults   ' query the database & return results
        If dtOwner.Rows.Count = 1 Then      ' if there is a user 
            _authenticated = True   ' authenticate them 
            ' store their details inside the member variables
            _id = CInt(dtOwner.Rows(0)("OwnerID"))
            _firstName = CStr(dtOwner.Rows(0)("Firstname"))
            _lastName = CStr(dtOwner.Rows(0)("Surname"))
            _email = CStr(dtOwner.Rows(0)("Email"))
            _password = CStr(dtOwner.Rows(0)("Password"))
        Else    ' otherwise
            _authenticated = False  ' set authentication as false
        End If
        dtOwner.Dispose()
        dtOwner = Nothing       ' empty the datatable 
    End Sub

    Public ReadOnly Property ID() As Integer
        Get
            Return _id
        End Get
    End Property

    Public ReadOnly Property FirstName() As String
        Get
            Return _firstName
        End Get
    End Property

    Public ReadOnly Property LastName() As String
        Get
            Return _lastName
        End Get
    End Property

    Public ReadOnly Property FullName() As String
        Get
            Return _firstName & " " & _lastName
        End Get
    End Property

    Public ReadOnly Property Email() As String
        Get
            Return _email
        End Get
    End Property

    Public ReadOnly Property Password() As String
        Get
            Return _password
        End Get
    End Property

    Public ReadOnly Property IsAuthenticated() As Boolean
        Get
            Return _authenticated
        End Get
    End Property

    Public Sub LogOut()
        _authenticated = False
        _email = ""
        _firstName = ""
        _lastName = ""
        _password = ""
    End Sub

    Public Function AddNewRestaurant(ownerID As Integer, name As String, address As String, city As String, openTime As DateTime, closeTime As DateTime, postCode As String, email As String, phoneNo As String, foodType As String, tables As List(Of String), restaurantURL As String) As Boolean
        Dim newRestaurantID As Integer
        Dim totalTables As Integer = tables.Count
        Dim tblID As String
        Dim tblIDQty As String
        Dim tblInfo As String()

        ' first we want to insert the restaurant into the database itself
        restaurantURL = restaurantURL.Replace("\", "/")
        db.AddParameter("@ownerID", ownerID.ToString)
        db.AddParameter("@name", name)
        db.AddParameter("@addr", address)
        db.AddParameter("@city", city)
        db.AddParameter("@openTime", openTime.ToString("HH:mm"))
        db.AddParameter("@closeTime", closeTime.ToString("HH:mm"))
        db.AddParameter("@postCode", postCode)
        db.AddParameter("@email", email)
        db.AddParameter("@phoneNo", phoneNo)
        db.AddParameter("@foodType", foodType)
        db.AddParameter("@url", restaurantURL)
        If db.Execute("sp_Owner_AddNewRestaurant") Then
            ' we then need to get the ID of the latest restaurant added
            db.AddParameter("@ownerID", ownerID.ToString)
            db.Execute("sp_Owner_GetLastRestaurantID")
            newRestaurantID = Convert.ToInt32(db.QueryResults.Rows(0)("RestaurantID"))

            ' we then loop through the tables list we have, for each one insert it into the restauranttable table 
            For i = 0 To totalTables - 1
                tblInfo = tables(i).Split(","c)
                tblID = tblInfo(0).ToString
                tblIDQty = tblInfo(1).ToString
                db.AddParameter("@restID", newRestaurantID.ToString)
                db.AddParameter("@tblID", tblID)
                db.AddParameter("@tblIDQty", tblIDQty)
                db.Execute("sp_Owner_AddNewRestaurant_TableInformation")
            Next
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetRestaurantNames(ownerID As Integer) As DataTable
        db.AddParameter("@ownerID", ownerID.ToString)
        db.Execute("sp_Owner_GetRestaurants")
        Return db.QueryResults
    End Function

    Public Sub RegisterStaff(restaurantID As Integer, firstName As String, surname As String, email As String, password As String, registerAs As String)
        db.AddParameter("@restID", restaurantID.ToString)
        db.AddParameter("@firstname", firstName)
        db.AddParameter("@surname", surname)
        db.AddParameter("@email", email)
        db.AddParameter("@password", password)

        ' check what type of staff is to be registered.. 
        Select Case registerAs
            Case "Register as Waiter"
                db.Execute("sp_Owner_RegisterWaiter")
            Case "Register as Chef"
                db.Execute("sp_Owner_RegisterChef")
        End Select

    End Sub

    Public Function GetRestaurantIDs(ownerID As Integer) As DataTable
        db.AddParameter("@ownerID", ownerID.ToString)
        db.Execute("sp_Owner_GetRestaurantIDs")
        Return db.QueryResults
    End Function


End Class
