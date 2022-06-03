Imports noPaperService_common.Entities
Imports noPaperService_common.Helpers

Public Class ECP
    Public Shared Function Sign(thumbprint As String, signData As Byte()) As ReturnData
        Try
            Dim signedData As ReturnData
            Dim sertificate As ReturnData = X509.selectSingleCertificate(thumbprint)

            If sertificate.isSuccess Then
                If sertificate IsNot Nothing AndAlso sertificate.data IsNot Nothing Then
                    signedData = X509.PKCS_7.Detached.Sign(signData, sertificate.data)
                Else
                    Return Nothing
                End If

                If signedData.isSuccess Then
                    Return signedData
                Else
                    Console.WriteLine($"Ошибка: {signedData.errorText}")
                    LogHelper.WriteLog($"Ошибка: {signedData.errorText}")

                    Return Nothing
                End If
            Else
                Console.WriteLine($"Ошибка: {sertificate.errorText}")
                LogHelper.WriteLog($"Ошибка: {sertificate.errorText}")
            End If

        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
