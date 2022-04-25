Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Http
Imports noPaperService_Api.Entities
Imports noPaperService_ecpWorker

Namespace Controllers
    Public Class ECPController
        Inherits ApiController

        Dim mainPath = "D:\Rsklad.Documents"
        Dim userPath = Environment.SpecialFolder.Personal
        Dim savePath = "D:\Rsklad.Documents" '$"{userPath}\Downloads"

        'Верификация подписи QRCode
        <HttpGet>
        <Route("api/GetEcp")>
        Function GetEcp(pv_id As String)
            Dim str As String() = pv_id.ToString().Split("-")
            pv_id = str(0)
            Dim jsonFileNamePath = $"{mainPath}\JSON\{pv_id}.json"
            Dim signFileNamePath = $"{mainPath}\P7S\{pv_id}.p7s"
            Dim response As New HttpResponseMessage()
            Dim imgBytes As Byte() = My.Resources.shtamp_png
            Dim headerText As String = String.Empty
            Dim headerColor As String = String.Empty
            Dim fileBytes As Byte()

            If File.Exists(jsonFileNamePath) And File.Exists(signFileNamePath) Then
                If File.Exists(jsonFileNamePath) Then
                    fileBytes = File.ReadAllBytes(jsonFileNamePath)
                End If

                'читаем подписанный файл
                Dim signedFile As Byte() = File.ReadAllBytes(jsonFileNamePath)

                'читаем подпись в массив байтов
                Dim sign As Byte() = File.ReadAllBytes(signFileNamePath)

                'проверяем валидность подписи
                Dim valid As Boolean = X509.PKCS_7.Detached.Verify(signedFile, sign)

                'полная првоерка валидности подписи
                Dim ver As Integer = X509.PKCS_7.Detached.fullVreify(signedFile, sign)

                If (ver = 0) Then
                    headerText = "ДОКУМЕНТ НЕ ПОДПИСАН"
                    headerColor = "black"
                ElseIf (ver = 1) Then
                    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ ПОДТВЕРЖДЕНА"
                    headerColor = "019D69"
                ElseIf (ver = -1) Then
                    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ НЕ ПОДТВЕРЖДЕНА"
                    headerColor = "red"
                End If

                For Each signComponent As SignComponent In CreateStamps.CreateStamps.GetSigners(sign)

                    response.Content = New StringContent("<html>
                                                          <head>
                                                           <title>Проверка выполнена</title>
                                                          </head>
                                                          <body>
                                                            <center> <b><font color=""" & headerColor & """>" & headerText & "</font></b></center>
                                                        <p><b><big>Статус сертификата подписи:</big></b> ДЕЙСТВИТЕЛЕН, сертификат выдан аккредитованным удостоверяющим центром</p>
                                                        <p><b><big>Владелец:</big></b> " & signComponent.SignCer.fullSubject & "</p>
                                                        <p><b><big>Издатель:</big></b> " & signComponent.SignCer.fullIssued & "</h2></p>
                                                        <p><b><big>Действителен:</big></b> " & signComponent.SignCer.valid & "</p>
                                                        <p><b><big>Место хранения:</big></b><a href=""https://etpzakaz.ru/""> etpzakaz.ru</a></p>
                                                        <body style=""background:url(data:image/png;base64," & Convert.ToBase64String(imgBytes) & ") no-repeat  60% 10%"">
                                                        <p><b><big>Дата подписи: </big></b>" & signComponent.SignDateTimeUtc.ToLocalTime.ToString("yyyy.MM.dd HH:mm") & "</p>
                                                         </body>
                                                      </html>", Encoding.UTF8)
                    response.Content.Headers.ContentType = New MediaTypeHeaderValue("text/html")
                    response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName
                Next
            End If
            Return response
        End Function

        'Печать ПДФ со штампами
        <HttpGet>
        <Route("api/GetInvoice")>
        Sub PrintPDF(pv_id As Integer)
            Try
                Dim jsonFileNamePath = $"{mainPath}\JSON\{pv_id}.json" 'C:\Rsklad.Documents
                Dim sign As Byte() = File.ReadAllBytes($"{mainPath}\P7S\{pv_id}.p7s")
                Dim absoluteUrl = HttpContext.Current.Request.Url.Authority
                'Dim signIden As String = $"{absoluteUrl}-"
                Dim signIden As String = $"http://{absoluteUrl}/ECP_API/api/GetEcp?pv_id={pv_id}-"

                Dim signedFileByte As Byte() = File.ReadAllBytes(jsonFileNamePath)

                Dim docTemplateFileNamePath As String = $"{mainPath}\Накладная.docx"
                Dim docFileNamePathExtension As String = String.Empty
                Dim docFileNamePath As String = String.Empty
                Dim docFileName As String = String.Empty

                PrintDoc.Print(mainPath, jsonFileNamePath, docFileName, docFileNamePath, docTemplateFileNamePath, docFileNamePathExtension)
                Dim pdfPath = LayoutStamps.LayoutStamps(savePath, docFileName, sign, docFileNamePathExtension, signIden)

                'Dim homeController As New HomeController()
                'homeController.Download(pdfPath)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
    End Class
End Namespace