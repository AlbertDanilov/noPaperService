Imports noPaperService_common.Entities

Public Class ECP
    Public Shared Function Sign(thumbprint As String, signData As Byte()) As ReturnData
        Try
            Dim sertificate As ReturnData = X509.selectSingleCertificate(thumbprint)
            Dim signedData As ReturnData = X509.PKCS_7.Detached.Sign(signData, sertificate.data)

            Return signedData
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
