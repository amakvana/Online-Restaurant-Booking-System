Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic
Imports System.Data

Public Class clsMembers

    ' member variables
    Private dtMember As New DataTable
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
        db.Execute("sp_Members_GetLoginDetails")       ' execute the stored procedure
        dtMember = db.QueryResults   ' query the database & return results
        If dtMember.Rows.Count = 1 Then      ' if there is a user 
            _authenticated = True   ' authenticate them 
            ' store their details inside the member variables
            _id = CInt(dtMember.Rows(0)("MemberID"))
            _firstName = CStr(dtMember.Rows(0)("Firstname"))
            _lastName = CStr(dtMember.Rows(0)("Surname"))
            _email = CStr(dtMember.Rows(0)("Email"))
            _password = CStr(dtMember.Rows(0)("Password"))
        Else    ' otherwise
            _authenticated = False  ' set authentication as false
        End If
        ' empty the datatable 
        dtMember.Dispose()
        dtMember = Nothing
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

    Public Function AddNewBooking(restTableID As Integer, memberID As Integer, bookingDate As String, time As DateTime) As Boolean
        bookingDate = If(bookingDate.Length = 10, bookingDate & " 00:00:00", bookingDate)

        ' add the booking
        db.AddParameter("@restTableID", restTableID.ToString)
        db.AddParameter("@memberID", memberID.ToString)
        db.AddParameter("@bookingDate", bookingDate)
        db.AddParameter("@time", time.ToString("HH:mm"))
        Return db.Execute("sp_Bookings_AddNew")
    End Function

    Public Function BookingAvailable(memberID As Integer, bookingDate As Date, time As DateTime, tableID As Integer) As Boolean
        ' we need to check if the bookingdate, time and table size is avilable on the current day 
        ' if its' fine, we need to check if that table size is available..
        ' so if there is 4 2seater tables, we need to grab the table size requested, get the quanitty of that.. check if the amount of bookings made in the db with that tableid is less then the quantity, which means that there is a table available so we can make a booking 
        ' so we are comparing the amount of records there are returned from the booking available fucntion with the quanity of tables..
        ' if the about satisfies then all good 
        Dim totalBookings As Integer
        Dim tableIDQty As Integer
        Dim dtBookings As New DataTable
        Dim dtMemberBookings As New DataTable
        Dim totalMemberBookings As Integer

        ' first we need to check if the member has made a booking on that date & time 
        db.AddParameter("@memberID", memberID.ToString)
        db.AddParameter("@bookingDate", bookingDate.ToString("dd/MM/yyyy"))
        db.AddParameter("@time", time.ToString("HH:mm"))
        db.Execute("sp_Members_HasBooking")
        dtMemberBookings = db.QueryResults
        totalMemberBookings = dtMemberBookings.Rows.Count

        ' if the member hasn't made a booking on the current date/time then they can make a booking 
        If totalMemberBookings = 0 Then
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
                dtMemberBookings.Dispose()
                dtBookings = Nothing
                dtMemberBookings = Nothing

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
                dtMemberBookings.Dispose()
                dtBookings = Nothing
                dtMemberBookings = Nothing
                Return True
            End If
        Else
            ' otherwise, they cnanot make a booking as they currently have one..
            ' safer to have one booking per person over the internet at that time and date 
            ' free resources 
            dtBookings.Dispose()
            dtMemberBookings.Dispose()
            dtBookings = Nothing
            dtMemberBookings = Nothing
            Return False
        End If
    End Function

    Public Function GetBookings(memberID As Integer) As DataTable
        db.AddParameter("@memberID", memberID.ToString)
        db.Execute("sp_Members_GetMemberBookings")
        Return db.QueryResults
    End Function

End Class
