Namespace Models
    Public Class CertComponent
        Public Property cert As Security.Cryptography.X509Certificates.X509Certificate2 = Nothing
        Public Property thumbprint As String = Nothing
        Public Property valid As String = Nothing
        Public Property issued As String = Nothing
        Public Property fullIssued As String = Nothing
        Public Property organization As String = Nothing
        Public Property subject As String = Nothing
        Public Property fullSubject As String = Nothing
        Public Property serialNumber As String = Nothing
        Public Property subjectPost As String = Nothing
    End Class
End Namespace