Namespace Models
    Public Class LayoutStamps
        Public Property sign As Byte()
        Public Property signApt As Byte()
        Public Property signIden As String
        Public Property pdfFiles As New List(Of String)
        Public Property pdfFileNamePathExtension As String = String.Empty
    End Class
End Namespace