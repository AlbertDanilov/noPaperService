Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Http
Imports DevExpress.Pdf
Imports noPaperService_Api.Entities
Imports noPaperService_Api.Helpers
Imports noPaperService_ecpWorker

Namespace Controllers
    Public Class ECPController
        Inherits ApiController

        Dim mainPath = "C:\Rsklad.Documents"
        Dim userPath = Environment.SpecialFolder.Personal
        Dim savePath = "C:\Rsklad.Documents" '$"{userPath}\Downloads"

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

                If (valid = True) Then
                    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ ПОДТВЕРЖДЕНА"
                    headerColor = "019D69"
                ElseIf (valid = False) Then
                    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ НЕ ПОДТВЕРЖДЕНА"
                    headerColor = "red"
                End If

                'полная првоерка валидности подписи
                'Dim ver As Integer = X509.PKCS_7.Detached.fullVerify(signedFile, sign)

                'If (ver = 0) Then
                '    headerText = "ДОКУМЕНТ НЕ ПОДПИСАН"
                '    headerColor = "black"
                'ElseIf (ver = 1) Then
                '    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ ПОДТВЕРЖДЕНА"
                '    headerColor = "019D69"
                'ElseIf (ver = -1) Then
                '    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ НЕ ПОДТВЕРЖДЕНА"
                '    headerColor = "red"
                'End If

                For Each signComponent As Entities.SignComponent In CreateStamps.CreateStamps.GetSigners(sign)

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

        'Печать ПДФ со штампами ВОРД
        <HttpGet>
        <Route("api/GetInvoiceDoc")>
        Function PrintPDF(pv_id As Integer)
            Try
                Dim jsonFileNamePath = $"{mainPath}\JSON\{pv_id}.json"
                Dim sign As Byte() = File.ReadAllBytes($"{mainPath}\P7S\{pv_id}.p7s")
                Dim absoluteUrl = HttpContext.Current.Request.Url.Authority
                Dim signIden As String = $"https://{absoluteUrl}/ECP_API/api/GetEcp?pv_id={pv_id}-"

                Dim signedFileByte As Byte() = File.ReadAllBytes(jsonFileNamePath)

                Dim docTemplateFileNamePath As String = $"{mainPath}\Накладная.docx"
                Dim docFileNamePathExtension As String = String.Empty
                Dim docFileNamePath As String = String.Empty
                Dim docFileName As String = String.Empty

                Print.PrintDoc(mainPath, jsonFileNamePath, docFileName, docFileNamePath, docTemplateFileNamePath, docFileNamePathExtension)
                Dim pdfByte = LayoutStamps.LayoutStamps(savePath, docFileName, sign, docFileNamePathExtension, signIden)

                Dim response As HttpResponseMessage = New HttpResponseMessage(HttpStatusCode.OK) With {
                    .Content = New ByteArrayContent(pdfByte)
                }
                Return response
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        'Печать ПДФ со штампами ЭКСЕЛЬ
        <HttpGet>
        <Route("api/GetInvoiceExcel")>
        Function PrintExcelPDF(jsonStringPV As String)
            Try
                Dim jsonFileNamePath
                Dim sign As Byte()
                Dim absoluteUrl
                Dim signIden As String

                Dim listPV = Utf8Json.JsonSerializer.Deserialize(Of List(Of Integer))(jsonStringPV)
                Dim pdfByte = Nothing
                Dim pdfFiles As New List(Of String)
                Dim endFile As String = String.Empty
                Dim okPV As New List(Of Integer)
                Dim errorPV As New List(Of Integer)
                Dim responseData As New ResponseData
                Dim invoice As New Invoice

                Using pdfDocumentProcessor As New PdfDocumentProcessor()
                    Dim i = 0

                    For Each pv_id As Integer In listPV
                        Try
                            jsonFileNamePath = $"{mainPath}\JSON\{pv_id}.json"
                            sign = File.ReadAllBytes($"{mainPath}\P7S\{pv_id}.p7s")
                            absoluteUrl = HttpContext.Current.Request.Url.Authority
                            signIden = $"https://{absoluteUrl}/ECP_API/api/GetEcp?pv_id={pv_id}-"

                            Dim signedFileByte As Byte() = File.ReadAllBytes(jsonFileNamePath)
                            Dim docTemplateFileNamePath As String = $"{mainPath}\Накладная.xlsx"
                            Dim docFileNamePathExtension As String = String.Empty
                            Dim docFileNamePath As String = String.Empty
                            Dim docFileName As String = String.Empty

                            Print.PrintExcel(mainPath, jsonFileNamePath, docFileName, docFileNamePath, docTemplateFileNamePath, docFileNamePathExtension)
                            LayoutStamps.LayoutStampsExcel(savePath, docFileName, sign, docFileNamePathExtension, signIden, pdfFiles)

                            endFile = $"{savePath}\Накладные {jsonStringPV}.pdf"

                            If listPV.Count = 1 Then
                                endFile = pdfFiles(i)
                            Else
                                If i = 0 Then
                                    pdfDocumentProcessor.CreateEmptyDocument(endFile)
                                    pdfDocumentProcessor.AppendDocument(pdfFiles(i))
                                Else
                                    pdfDocumentProcessor.AppendDocument(pdfFiles(i))
                                End If

                                If File.Exists(pdfFiles(i)) Then
                                    File.Delete(pdfFiles(i))
                                End If
                            End If

                            okPV.Add(pv_id)

                            i += 1
                        Catch ex As Exception
                            Dim num As Integer
                            Dim isNum = Integer.TryParse(ex.Message, num)

                            If isNum Then
                                If ex.Message = CSKLAD.noPaperAPIException.PrintExcel Then
                                    invoice.ErrorText = "Ошибка в Excel"
                                    invoice.IsError = True
                                ElseIf ex.Message = CSKLAD.noPaperAPIException.LayoutStamp Then
                                    invoice.ErrorText = "Не удается проштамповать документ"
                                    invoice.IsError = True
                                End If
                            Else
                                errorPV.Add(pv_id)
                                invoice.ErrorText &= ex.Message & vbNewLine
                            End If

                            'responseData.IsError = True
                            'Throw New Exception(CSKLAD.EXCEPTION.Json)
                        End Try
                    Next
                End Using

                If endFile IsNot String.Empty Then pdfByte = File.ReadAllBytes(endFile)

                invoice.OkPV = okPV
                invoice.ErrorPV = errorPV
                invoice.PdfByte = pdfByte

                'responseData.Data = invoice

                If File.Exists(endFile) Then
                    File.Delete(endFile)
                End If

                Dim jsonResponse As String = Utf8Json.JsonSerializer.ToJsonString(invoice)

                Dim response As New HttpResponseMessage(HttpStatusCode.OK) With {
                    .Content = New StringContent(jsonResponse)
                }
                Return response
            Catch ex As Exception
                Dim response As New HttpResponseMessage(HttpStatusCode.InternalServerError) With {
                    .Content = New StringContent(ex.Message)
                }
                Dim r = New HttpResponseException(response)
                Throw r
            End Try
        End Function
    End Class

End Namespace