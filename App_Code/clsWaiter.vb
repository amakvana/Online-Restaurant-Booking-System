Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic
Imports System.Data

Public Class clsWaiter
    ' member variables
    Private dtWaiter As New DataTable
    Private db As New clsSQLServer(AppDomain.CurrentDomain.BaseDirectory & "App_Data\RestaurantSystem.mdf")      ' connect to the database 

    ' member variables to store user data into 
    Private _waiterID As Integer       ' store current waiter id
    Private _restID As Integer       ' store current restaurant id that waiter is signed up to
    Private _firstName As String       ' store current firstname
    Private _lastName As String       ' store current lastname
    Private _email As String       ' store current email
    Private _password As String       ' store current password
    Private _authenticated As Boolean       ' store if they're authenticated or not

    Public Sub New(email As String, password As String)
        ' pass the parameters required in the stored procedure & their values 
        db.AddParameter("@Email", email)
        db.AddParameter("@Password", password)
        db.Execute("sp_Waiter_GetLoginDetails")       ' execute the stored procedure
        dtWaiter = db.QueryResults   ' query the database & return results
        If dtWaiter.Rows.Count = 1 Then      ' if there is a user 
            _authenticated = True   ' authenticate them 
            ' store their details inside the member variables
            _waiterID = CInt(dtWaiter.Rows(0)("WaiterID").ToString)
            _restID = CInt(dtWaiter.Rows(0)("RestaurantID").ToString)
            _firstName = CStr(dtWaiter.Rows(0)("Firstname").ToString)
            _lastName = CStr(dtWaiter.Rows(0)("Surname").ToString)
            _email = CStr(dtWaiter.Rows(0)("Email").ToString)
            _password = CStr(dtWaiter.Rows(0)("Password").ToString)
        Else    ' otherwise
            _authenticated = False  ' set authentication as false
        End If
        dtWaiter.Dispose()
        dtWaiter = Nothing       ' empty the datatable 
    End Sub

    Public ReadOnly Property WaiterID() As Integer
        Get
            Return _waiterID
        End Get
    End Property

    Public ReadOnly Property RestaurantID() As Integer
        Get
            Return _restID
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

    Public Function GetBookings(restaurantID As Integer, waiterID As Integer) As DataTable
        db.AddParameter("@restaurantID", restaurantID.ToString)
        db.AddParameter("@waiterID", waiterID.ToString)
        db.Execute("sp_Waiter_GetBookings")
        Return db.QueryResults
    End Function

    Public Function GetBookings(restaurantID As Integer, waiterID As Integer, bookingDate As String) As DataTable
        ' if bookingDate length =10, append 00:00:00 otherwise keep same value
        bookingDate = If(bookingDate.Length = 10, bookingDate & " 00:00:00", bookingDate)
        db.AddParameter("@restaurantID", restaurantID.ToString)
        db.AddParameter("@waiterID", waiterID.ToString)
        db.AddParameter("@bookingDate", bookingDate)
        db.Execute("sp_Waiter_GetBookings_AtDate")
        Return db.QueryResults
    End Function

    Public Function GetRestaurantName(restaurantID As Integer) As String
        db.AddParameter("@restaurantID", restaurantID.ToString)
        db.Execute("sp_Waiter_GetRestaurantName_FromRestID")
        Return db.QueryResults.Rows(0)("Name").ToString.Trim
    End Function

    Public Function GetBookingInformation(bookingID As Integer) As DataTable
        db.AddParameter("@bookingID", bookingID.ToString)
        db.Execute("sp_Waiter_GetBookingInformation")
        Return db.QueryResults
    End Function

    Public Function UpdateMemberBooking(bookingID As Integer, bookingDate As String, time As DateTime) As Boolean
        bookingDate = If(bookingDate.Length = 10, bookingDate & " 00:00:00", bookingDate)
        db.AddParameter("@bookingID", bookingID.ToString)
        db.AddParameter("@bookingDate", bookingDate)
        db.AddParameter("@time", time.ToString("HH:mm"))
        Return db.Execute("sp_Waiter_UpdateMemberBooking")
    End Function

    Public Function UpdateMemberBookingAvailable(bookingDate As Date, time As DateTime, tableID As Integer) As Boolean
        ' we need to check if the bookingdate, time and table size is avilable on the current day 
        ' if its' fine, we need to check if that table size is available..
        ' so if there is 4 2seater tables, we need to grab the table size requested, get the quanitty of that.. check if the amount of bookings made in the db with that tableid is less then the quantity, which means that there is a table available so we can make a booking 
        ' so we are comparing the amount of records there are returned from the booking available fucntion with the quanity of tables..
        ' if the about satisfies then all good 
        Dim totalBookings As Integer
        Dim tableIDQty As Integer
        Dim dtBookings As New DataTable

        ' now we need to check if the bookingdate, time and table size is avilable on the current day 
        db.AddParameter("@date", bookingDate.ToString)
        db.AddParameter("@time", time.ToString("HH:mm"))
        db.AddParameter("@tableID", tableID.ToString)
        db.Execute("sp_Bookings_CheckAvailability")
        dtBookings = db.QueryResults
        totalBookings = dtBookings.Rows.Count

        ' if there are bookings already made for the table 
        If totalBookings > 0 Then
            ' we need to check if the amout of bookings made on that day, time & table are less then the quantity of the table chosen 
            ' if that doesn't satisfy then all those tables at the chosen date/time are booked.
            tableIDQty = Convert.ToInt32(dtBookings.Rows(0)("QuantityOfTableID"))
            ' free resources 
            dtBookings.Dispose()
            dtBookings = Nothing

            ' if the amount of bookings made for that table is less then the total quantity
            If ((tableIDQty - totalBookings) > 0) Then
                ' we have a table free so we can book it 
                Return True
            Else
                ' table is fully booked 
                Return False
            End If
        Else
            ' there are no bookings made, so it's available
            ' free resources 
            dtBookings.Dispose()
            dtBookings = Nothing
            Return True
        End If
    End Function

End Class
