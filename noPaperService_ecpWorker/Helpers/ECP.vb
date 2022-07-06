Imports noPaperService_common.Entities
Imports noPaperService_common.Helpers

Public Class ECP
    Public Shared Function Sign(thumbprint As String, FIO As String, signData As Byte()) As ReturnData
        Try
            Dim signedData As ReturnData
            Dim sertificate As ReturnData = Nothing

            If Not String.IsNullOrEmpty(FIO) Then
                'поиск ЭЦП по ФИО
                sertificate = X509.selectSingleCertificateByFIO(FIO)
            End If

            'если ФИО нет или по ФИО ничего не найдено
            If sertificate Is Nothing OrElse Not sertificate.isSuccess Then
                sertificate = X509.selectSingleCertificate(thumbprint)
            End If

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

                Return Nothing
            End If

        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
