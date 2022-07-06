Namespace Models
    Public Class Invoice
        Public Property OkPV As List(Of Integer)
        Public Property ErrorPV As List(Of Integer)
        Public Property PdfByte As Byte() = Nothing
        Public Property ErrorText As String
        Public Property IsError As Boolean
    End Class
End Namespace