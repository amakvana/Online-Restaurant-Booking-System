' ╔════════════════════════════════════════════════════════════════════════════════════════════════════════╗
' ║ Class name: clsSQLServer                                                                               ║
' ║ Original Author: Tugrul Essendal                                                                       ║
' ║ Optimizations & Improvments: Akash Makvana                                                             ║
' ║ This code is free to use/modify so long as full credit is given to all authors/contributors i.e. us!   ║
' ╚════════════════════════════════════════════════════════════════════════════════════════════════════════╝

Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web

'============================================
'   Data Access Level
'
'   Represents the BOTTOM tier of
'   OO 3-tier software development
'============================================

Public Class clsSQLServer

    'data adapter transfers data to and from the database
    Dim da As New SqlDataAdapter
    'Dim params As New ArrayList
    Dim params As New List(Of SqlParameter) ' upto 4x faster then arraylist 
    Dim dtResults As New DataTable
    Dim newRec As DataRow
    Dim connString As String

    Public ReadOnly Property QueryResults As DataTable
        Get
            Return dtResults
        End Get
    End Property

    Public ReadOnly Property NewRecord() As DataRow
        'Provides access to the new record as a single data row
        Get
            Return newRec
        End Get
    End Property

    Public Sub New(dbPath As String)
        params.Clear()

        connString = "Data Source=(LocalDB)\v11.0;AttachDbFilename=" + dbPath + ";Integrated Security=True;Connect Timeout=30"

    End Sub

    Private Function GetHomeAddr() As String
        'Folder where Default.aspx is located
        Return System.AppDomain.CurrentDomain.BaseDirectory
    End Function

    Public Sub AddParameter(paramName As String, paramValue As String)
        'public method allowing the addition of an sql parameter to the list of parameters
        'it accepts two parameters the name of the parameter and its value

        'create a new instance of the sql parameter object
        Dim aParam As New SqlParameter(paramName, paramValue)
        'add the parameter to the list
        params.Add(aParam)
    End Sub

    Public Function Execute(spName As String) As Boolean
        'public method used to execute the named stored procedure
        'accepts one parameter which is the name of the stored procedure to use
        'open the stored procedure
        dtResults.Rows.Clear() 'otherwise, the next SELECT appends its records to existing records

        Try
            Using conn As New SqlConnection(connString)
                conn.Open() ' open the connection
                Using cmd As New SqlCommand(spName, conn)
                    cmd.CommandType = CommandType.StoredProcedure    ' tell the data command we're working with stored procedures
                    ' build the command builder with parameters
                    If params.Count > 0 Then
                        Dim totalParams As Integer = params.Count   ' cache total params (makes for loop more efficient)
                        For i = 0 To totalParams - 1
                            cmd.Parameters.Add(params(i))
                        Next
                    End If

                    da = New SqlDataAdapter(spName, conn)   ' initialise data adapter
                    da.SelectCommand = cmd   ' select the command property
                    da.Fill(dtResults)     ' fill the data adapter
                    newRec = dtResults.NewRow()   ' get structure of single record
                    params.Clear()  ' clear params array
                End Using
            End Using
            Return True
        Catch ex As Exception
            params.Clear()  ' clear params array
            Return False
        End Try
    End Function

    Public Sub WriteToDatabase()
        'Updates the changes to the data adapter, thus changing the database
        da.Update(dtResults)
    End Sub

    Public Sub RemoveRecord(index As Integer)
        'Removes the record at the specified index position in the query results
        dtResults.Rows(index).Delete()
    End Sub

    Public Sub AddToDataTable()
        'Adds the new record to the table
        dtResults.Rows.Add(newRec)
        're initialise the new record
        newRec = dtResults.NewRow()
    End Sub

    Public Function Count() As Integer
        'Returns the number of records in the query results
        Return dtResults.Rows.Count
    End Function

    Public Sub Dispose()
        ' disposes any methods that might not have been done automatically 
        da.Dispose()
        dtResults.Dispose()
    End Sub

End Class
