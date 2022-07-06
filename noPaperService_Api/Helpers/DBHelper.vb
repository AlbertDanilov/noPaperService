Imports System.Data.SqlClient

Namespace Helpers
    Public Class DBHelper
        Public Shared Function GetTableByCommand(commandText As String, connectionString As String, Optional params As SqlParameter() = Nothing, Optional commandType As CommandType = CommandType.Text, Optional timeout As ULong = 60000) As DataTable
            Dim table As New DataTable
            Using con As New SqlConnection(connectionString)
                con.Open()
                Using da As New SqlDataAdapter(New SqlCommand(commandText, con))
                    da.SelectCommand.CommandTimeout = timeout
                    da.SelectCommand.CommandType = commandType
                    If Not IsNothing(params) Then
                        da.SelectCommand.Parameters.AddRange(params)
                    End If
                    da.Fill(table)
                End Using
            End Using
            Return table
        End Function
    End Class
End Namespace
