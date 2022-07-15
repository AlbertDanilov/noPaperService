Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Http
Imports DevExpress.Pdf
Imports noPaperService_Api.Models
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

            Dim signNumber As String = str(1)

            Dim jsonFileNamePath As String = $"{mainPath}\JSON\{pv_id}.json"
            Dim signFileNamePath As String

            Select Case signNumber
                Case "1"
                    signFileNamePath = $"{mainPath}\P7S\{pv_id}.p7s"
                Case "2"
                    signFileNamePath = $"{mainPath}\P7S_APT\{pv_id}.p7s"
                Case Else
                    signFileNamePath = $"{mainPath}\P7S\{pv_id}.p7s"
            End Select

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

                headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ ПОДТВЕРЖДЕНА"
                headerColor = "019D69"

                'If (valid = True) Then
                '    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ ПОДТВЕРЖДЕНА"
                '    headerColor = "019D69"
                'ElseIf (valid = False) Then
                '    headerText = "ПОДЛИННОСТЬ ЭЛЕКТРОННОЙ ЦИФРОВОЙ ПОДПИСИ НЕ ПОДТВЕРЖДЕНА"
                '    headerColor = "red"
                'End If

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

                For Each signComponent As Models.SignComponent In CreateStamps.CreateStamps.GetSigners(sign)

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
                                                        <p><b><big>Место хранения:</big></b><a href=""https://farm.tatarstan.ru/""> ttmf.ru</a></p>
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
                Dim pdfByte = Helpers.LayoutStamps.LayoutStamps(savePath, docFileName, sign, docFileNamePathExtension, signIden)

                Dim response As HttpResponseMessage = New HttpResponseMessage(HttpStatusCode.OK) With {
                    .Content = New ByteArrayContent(pdfByte)
                }
                Return response
            Catch ex As Exception
                Throw ex
            End Try
        End Function

        'Печать ПДФ со штампами ЭКСЕЛЬ (Накаладная)
        <HttpGet>
        <Route("api/GetInvoiceExcel")>
        Function PrintExcelPDF_Invoice(jsonPV As String)
            Try
                Dim absoluteUrl

                Dim listPV = Utf8Json.JsonSerializer.Deserialize(Of List(Of Integer))(jsonPV)
                Dim pdfByte = Nothing
                Dim pdfFiles As New List(Of String)
                Dim endFile As String = String.Empty
                Dim okPV As New List(Of Integer)
                Dim errorPV As New List(Of Integer)
                Dim responseData As New ResponseData
                Dim invoice As New Invoice
                Dim printExcel As New PrintExcel
                Dim layoutStamps As New Models.LayoutStamps
                Dim appPDF As String = String.Empty
                Dim i = 0

                Using pdfDocumentProcessor As New PdfDocumentProcessor()
                    For Each pv_id As Integer In listPV
                        Try
                            Dim signedFileByte As Byte()
                            printExcel.nameFile = "Накладная"
                            printExcel.docTemplateFileNamePath = $"{mainPath}\{printExcel.nameFile}.xlsx"

                            Try
                                printExcel.jsonFileNamePath = $"{mainPath}\JSON\{pv_id}.json"
                                layoutStamps.sign = File.ReadAllBytes($"{mainPath}\P7S\{pv_id}.p7s")

                                If File.Exists($"{mainPath}\P7S_APT\{pv_id}.p7s") Then
                                    layoutStamps.signApt = File.ReadAllBytes($"{mainPath}\P7S_APT\{pv_id}.p7s")
                                End If

                                absoluteUrl = HttpContext.Current.Request.Url.Authority
                                layoutStamps.signIden = $"https://{absoluteUrl}/ECP_API/api/GetEcp?pv_id={pv_id}-"

                                signedFileByte = File.ReadAllBytes(printExcel.jsonFileNamePath)
                            Catch ex As Exception
                                responseData.IsError = True
                                responseData.ErrorText = CSKLAD.noPaperAPIException.Json
                                Throw New Exception()
                            End Try

                            Print.PrintExcel_Invoice(mainPath, printExcel, layoutStamps, responseData)
                            Helpers.LayoutStamps.LayoutStampsExcelBook(savePath, layoutStamps, printExcel, responseData)
                            endFile = $"{savePath}\{printExcel.nameFile} {jsonPV}.pdf"

                            printExcel.nameFile = "Приложение"
                            printExcel.docTemplateFileNamePath = $"{mainPath}\{printExcel.nameFile}.xlsx"

                            If listPV.Count = 1 Then
                                pdfDocumentProcessor.CreateEmptyDocument(endFile)
                                pdfDocumentProcessor.AppendDocument(layoutStamps.pdfFiles(i))

                                If File.Exists(layoutStamps.pdfFiles(i)) Then
                                    File.Delete(layoutStamps.pdfFiles(i))
                                End If
                            Else
                                If i = 0 Then
                                    pdfDocumentProcessor.CreateEmptyDocument(endFile)
                                    pdfDocumentProcessor.AppendDocument(layoutStamps.pdfFiles(i))
                                Else
                                    pdfDocumentProcessor.AppendDocument(layoutStamps.pdfFiles(i))
                                End If

                                If File.Exists(layoutStamps.pdfFiles(i)) Then
                                    File.Delete(layoutStamps.pdfFiles(i))
                                End If
                            End If


                            Dim app As Boolean = Print.PrintExcel_InvoiceApplication(mainPath, printExcel, responseData)
                            If app Then
                                appPDF = ConvertToPDF.ConvertToPDFExcelBook(mainPath, layoutStamps, printExcel, responseData)
                            End If

                            If app Then
                                pdfDocumentProcessor.AppendDocument(appPDF)

                                If File.Exists(appPDF) Then
                                    File.Delete(appPDF)
                                End If
                            End If

                            okPV.Add(pv_id)

                            i += 1
                        Catch ex As Exception
                            If responseData.IsError Then
                                If responseData.ErrorText = CSKLAD.noPaperAPIException.PrintExcel Then
                                    invoice.ErrorText = "Ошибка в Excel"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.LayoutStamp Then
                                    invoice.ErrorText = "Не удается проштамповать документ"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                    'errorPV.Clear()
                                    'Exit For
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.Json Then
                                    invoice.ErrorText = "Электронный документ в процессе формирования"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.PrintExcelApp Then
                                    invoice.ErrorText = "Ошибка в Excel при печати приложения"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                    i += 1
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.ConertToPDF Then
                                    invoice.ErrorText = "Ошибка при создании PDF приложения"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                    i += 1
                                End If
                            Else
                                errorPV.Add(pv_id)
                                invoice.ErrorText &= ex.Message & vbNewLine
                            End If
                        End Try
                    Next
                End Using

                'If endFile IsNot String.Empty Then pdfByte = File.ReadAllBytes(endFile)

                If File.Exists(endFile) Then
                    pdfByte = File.ReadAllBytes(endFile)
                    File.Delete(endFile)
                End If
                If File.Exists(layoutStamps.pdfFileNamePathExtension) Then
                    File.Delete(layoutStamps.pdfFileNamePathExtension)
                End If

                invoice.OkPV = okPV
                invoice.ErrorPV = errorPV
                invoice.PdfByte = pdfByte

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

        'Печать ПДФ со штампами ЭКСЕЛЬ (Протокол согласования цен)
        <HttpGet>
        <Route("api/GetPriceApprovalProtocolExcel")>
        Function PrintExcelPDF_PriceApprovalProtocol(jsonPV As String)
            Try
                Dim absoluteUrl

                Dim listPV = Utf8Json.JsonSerializer.Deserialize(Of List(Of Integer))(jsonPV)
                Dim pdfByte = Nothing
                Dim endFile As String = String.Empty
                Dim okPV As New List(Of Integer)
                Dim errorPV As New List(Of Integer)
                Dim responseData As New ResponseData
                Dim invoice As New Invoice
                Dim printExcel As New PrintExcel
                Dim layoutStamps As New Models.LayoutStamps

                printExcel.nameFile = "Протокол согласования цен"

                Using pdfDocumentProcessor As New PdfDocumentProcessor()
                    Dim i = 0

                    For Each pv_id As Integer In listPV
                        Try
                            Dim signedFileByte As Byte()
                            printExcel.docTemplateFileNamePath = $"{mainPath}\{printExcel.nameFile}.xlsx"

                            Try
                                printExcel.jsonFileNamePath = $"{mainPath}\JSON\{pv_id}.json"
                                layoutStamps.sign = File.ReadAllBytes($"{mainPath}\P7S\{pv_id}.p7s")

                                If File.Exists($"{mainPath}\P7S_APT\{pv_id}.p7s") Then
                                    layoutStamps.signApt = File.ReadAllBytes($"{mainPath}\P7S_APT\{pv_id}.p7s")
                                End If

                                absoluteUrl = HttpContext.Current.Request.Url.Authority
                                layoutStamps.signIden = $"https://{absoluteUrl}/ECP_API/api/GetEcp?pv_id={pv_id}-"

                                signedFileByte = File.ReadAllBytes(printExcel.jsonFileNamePath)
                            Catch ex As Exception
                                responseData.IsError = True
                                responseData.ErrorText = CSKLAD.noPaperAPIException.Json
                                Throw New Exception()
                            End Try

                            Print.PrintExcel_PriceApprovalProtocol(mainPath, printExcel, responseData)
                            Helpers.LayoutStamps.LayoutStampsExcel(savePath, layoutStamps, printExcel, responseData)

                            endFile = $"{savePath}\{printExcel.nameFile} {jsonPV}.pdf"

                            If listPV.Count = 1 Then
                                endFile = layoutStamps.pdfFiles(i)
                            Else
                                If i = 0 Then
                                    pdfDocumentProcessor.CreateEmptyDocument(endFile)
                                    pdfDocumentProcessor.AppendDocument(layoutStamps.pdfFiles(i))
                                Else
                                    pdfDocumentProcessor.AppendDocument(layoutStamps.pdfFiles(i))
                                End If

                                If File.Exists(layoutStamps.pdfFiles(i)) Then
                                    File.Delete(layoutStamps.pdfFiles(i))
                                End If
                            End If

                            okPV.Add(pv_id)

                            i += 1
                        Catch ex As Exception
                            If responseData.IsError Then
                                If responseData.ErrorText = CSKLAD.noPaperAPIException.PrintExcel Then
                                    invoice.ErrorText = "Ошибка в Excel"
                                    invoice.IsError = True
                                    errorPV.Clear()
                                    Exit For
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.LayoutStamp Then
                                    invoice.ErrorText = "Не удается проштамповать документ"
                                    invoice.IsError = True
                                    errorPV.Clear()
                                    Exit For
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.Json Then
                                    invoice.ErrorText = "Электронный документ в процессе формирования"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                ElseIf responseData.ErrorText = CSKLAD.noPaperAPIException.Jnvls Then
                                    invoice.ErrorText = "Нет товаров ЖНВЛС"
                                    invoice.IsError = True
                                    errorPV.Add(pv_id)
                                End If
                            Else
                                errorPV.Add(pv_id)
                                invoice.ErrorText &= ex.Message & vbNewLine
                            End If
                        End Try
                    Next
                End Using

                If endFile IsNot String.Empty Then pdfByte = File.ReadAllBytes(endFile)

                invoice.OkPV = okPV
                invoice.ErrorPV = errorPV
                invoice.PdfByte = pdfByte

                If File.Exists(endFile) Then
                    File.Delete(endFile)
                ElseIf File.Exists(layoutStamps.pdfFileNamePathExtension) Then
                    File.Delete(layoutStamps.pdfFileNamePathExtension)
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