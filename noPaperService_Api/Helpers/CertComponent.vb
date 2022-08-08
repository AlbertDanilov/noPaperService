Namespace Helpers
    Public Class CertComponent
        Inherits Models.CertComponent

        Public Shared Function [New](cert As System.Security.Cryptography.X509Certificates.X509Certificate2) As Models.CertComponent
            Dim CC As New Models.CertComponent
            If cert IsNot Nothing Then
                CC.cert = cert
                CC.thumbprint = cert.Thumbprint
                CC.issued = GetValue(cert.Issuer, "CN")
                CC.organization = GetValue(cert.Subject, "CN")
                CC.valid = "с " & cert.NotBefore.ToString("dd.MM.yyyy") & " по " & cert.NotAfter.ToString("dd.MM.yyyy")
                CC.subject = GetValue(cert.Subject, "SN") & " " & GetValue(cert.Subject, "G")
                CC.serialNumber = cert.SerialNumber
                CC.fullSubject = GetFullValue(cert.Subject)
                CC.fullIssued = GetFullValue(cert.Issuer)
                CC.subjectPost = GetValue(cert.Subject, "T")
                CC.subjectOrg = GetValue(cert.Subject, "OU")
            End If
            Return CC
        End Function
        Private Shared Function GetValue(ByVal sender As String, ByVal name As String) As String
            Dim listObjects As String() = sender.Split(",")
            For Each item As String In listObjects
                Dim nv As New NameValue(item)
                If nv.name = name Then Return nv.value
            Next
            Return Nothing
        End Function
        Private Shared Function GetFullValue(ByVal sender As String) As String
            Dim listObjects As String() = sender.Split(",")
            Dim fullSubject As String = Nothing
            Dim comma As String = Nothing
            For Each item As String In listObjects
                Dim nv As New NameValue(item)
                fullSubject += comma & nv.value
                comma = ", "
            Next
            Return fullSubject
        End Function
        Private Class NameValue
            Public Property name As String = ""
            Public Property value As String = ""

            Public Sub New(sender As String)
                Dim indexOfChar As Integer = sender.IndexOf("=")
                Try
                    Me.name = Trim(sender.Substring(0, indexOfChar))
                    Me.value = sender.Substring(indexOfChar + 1)
                Catch
                    Me.name = ""
                    Me.value = sender
                End Try
            End Sub
        End Class
    End Class
End Namespace
