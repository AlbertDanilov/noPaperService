Imports System.Drawing
Imports System.IO
Imports DevExpress.Spreadsheet
Imports DevExpress.XtraRichEdit
Imports iTextSharp.text.pdf
Imports noPaperService_Api.Models

Namespace Helpers
    Public Class LayoutStamps
        Public Shared Function LayoutStamps(savePath As String, docFileName As String, sign As Byte(), docFileNamePathExtension As String, signIden As String)
            Dim pdfFile As Byte() = Nothing

            Dim pdfFileNamePathExtension = $"{savePath}\{docFileName}.pdf"

            'Список для штампов
            Dim stampList As List(Of Bitmap)
            stampList = CreateStamps.CreateStamps.GetStamps(sign, signIden)

            Using inputPdfStream As New MemoryStream
                Dim mybytes As Byte()

                Dim richEdit = New RichEditDocumentServer()
                richEdit.LoadDocument(docFileNamePathExtension)
                richEdit.ExportToPdf(inputPdfStream)

                mybytes = inputPdfStream.ToArray

                Try
                    Using outputPdfStream As New FileStream(pdfFileNamePathExtension, FileMode.Create, FileAccess.Write, FileShare.None)
                        Using reader = New PdfReader(mybytes)
                            Using stamper = New PdfStamper(reader, outputPdfStream)
                                If stampList.Count > 0 Then
#Region "дата справа сверху"
                                    Dim pdfContentByteFirst As PdfContentByte = stamper.GetOverContent(1)
                                    Dim imageData As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(CreateStamps.CreateStamps.GetDataStamp(Date.Now.ToString("dd.MM.yyyy"))))
                                    'Позиция изображения
                                    imageData.SetAbsolutePosition(reader.GetPageSize(1).Width - (100 + 17), reader.GetPageSize(1).Height - (25 + 17))
                                    'Размер изображения
                                    imageData.ScaleAbsolute(100, 25)
                                    pdfContentByteFirst.AddImage(imageData)

#End Region

#Region "подпись на каждой странице(кроме последней) только для контракта"
                                    Dim imagetext As iTextSharp.text.Image
                                    Try
                                        imagetext = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(CreateStamps.CreateStamps.GetStringStamp("Электронный документ подписан ЭП на электронной площадке", 26)))
                                        imagetext.SetAbsolutePosition(220, 17)
                                        imagetext.ScaleAbsolute(330, 15)
                                        For index = 1 To reader.NumberOfPages - 1
                                            Dim pdfcontent = stamper.GetOverContent(index)
                                            pdfcontent.AddImage(imagetext)
                                        Next
                                    Catch ex As Exception
                                        Throw ex
                                    End Try
#End Region

#Region "печать эцп"
                                    Dim image1 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(stampList(0)))
                                    Dim parser = New parser.PdfReaderContentParser(reader)
                                    Dim finder = parser.ProcessContent(reader.NumberOfPages, New parser.TextMarginFinder())
                                    Dim lastPage As iTextSharp.text.Rectangle = reader.GetPageSize(reader.NumberOfPages)
                                    Dim maxHeightPage = lastPage.Height
                                    Dim maxVerticalHeightPage = 792
                                    Dim lastElemHeight As Integer
                                    Try
                                        lastElemHeight = finder.GetHeight()
                                    Catch ex As Exception
                                        Throw ex
                                    End Try

                                    Dim newWidth As Integer
                                    Dim newHeight As Integer
                                    Dim isNewPage As Boolean
                                    If maxHeightPage > lastElemHeight + 280 Then
                                        newWidth = 17
                                        newHeight = 17
                                        isNewPage = False
                                    Else
                                        isNewPage = True
                                        Dim rectangle = reader.GetPageSize(1)
                                        stamper.InsertPage(reader.NumberOfPages + 1, rectangle)
                                        Try
                                            stamper.GetOverContent(reader.NumberOfPages - 1).AddImage(imagetext)
                                        Catch ex As Exception
                                            Throw ex
                                        End Try
                                        newWidth = 17
                                        newHeight = maxVerticalHeightPage - (10 + 120)
                                    End If
                                    Dim pdfContentByte As PdfContentByte = stamper.GetOverContent(reader.NumberOfPages)
                                    'Позиция изображения
                                    image1.SetAbsolutePosition(newWidth, newHeight)
                                    'Размер изображения
                                    image1.ScaleAbsolute(280, 120)
                                    pdfContentByte.AddImage(image1)
                                    If stampList.Count > 1 Then
                                        Dim image2 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(stampList(stampList.Count - 1)))
                                        'Позиция изображения
                                        If isNewPage Then
                                            image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), maxVerticalHeightPage - (10 + 120))
                                        Else
                                            image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), 17)
                                        End If
                                        'Размер изображения
                                        image2.ScaleAbsolute(280, 120)
                                        pdfContentByte.AddImage(image2)
                                    End If
                                End If
#End Region
                            End Using
                        End Using
                    End Using
                    pdfFile = File.ReadAllBytes(pdfFileNamePathExtension)

                    If File.Exists(docFileNamePathExtension) Then
                        File.Delete(docFileNamePathExtension)
                    End If
                    If File.Exists(pdfFileNamePathExtension) Then
                        File.Delete(pdfFileNamePathExtension)
                    End If
                Catch ex As Exception
                    If File.Exists(docFileNamePathExtension) Then
                        File.Delete(docFileNamePathExtension)
                    End If
                    If File.Exists(pdfFileNamePathExtension) Then
                        pdfFile = File.ReadAllBytes(pdfFileNamePathExtension)
                        File.Delete(pdfFileNamePathExtension)
                    End If
                    Throw ex
                End Try

                Return pdfFile
            End Using
        End Function

        Public Shared Function LayoutStampsExcelBook(savePath As String, layoutStamps As Models.LayoutStamps, printExcel As PrintExcel, responseData As ResponseData) 'Книжная ориентация
            layoutStamps.pdfFileNamePathExtension = $"{savePath}\{printExcel.docFileName}.pdf"

            'Список для штампов
            Dim stampList As List(Of Bitmap)
            stampList = CreateStamps.CreateStamps.GetStamps(layoutStamps.sign, layoutStamps.signIden)

            Dim _stampList As List(Of Bitmap)

            If layoutStamps.signApt IsNot Nothing Then
                CreateStamps.CreateStamps.GetStamps(layoutStamps.signApt, layoutStamps.signIden, stampList, 2)
                _stampList = CreateStamps.CreateStamps._GetStamps(printExcel)
            End If

            Using workbook As New Workbook()
                Using inputPdfStream As New MemoryStream
                    Dim mybytes As Byte()

                    workbook.LoadDocument(printExcel.docFileNamePathExtension)
                    workbook.ExportToPdf(inputPdfStream)

                    mybytes = inputPdfStream.ToArray

                    Try
                        Using outputPdfStream As New FileStream(layoutStamps.pdfFileNamePathExtension, FileMode.Create, FileAccess.Write, FileShare.None)
                            Using reader = New PdfReader(mybytes)
                                Using stamper = New PdfStamper(reader, outputPdfStream)
                                    If stampList.Count > 0 Then
#Region "дата справа сверху"
                                        Dim pdfContentByteFirst As PdfContentByte = stamper.GetOverContent(1)
                                        Dim imageData As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(CreateStamps.CreateStamps.GetDataStamp(Date.Now.ToString("dd.MM.yyyy"))))
                                        'Позиция изображения
                                        imageData.SetAbsolutePosition(reader.GetPageSize(1).Width - (100 + 17), reader.GetPageSize(1).Height - (25 + 17))
                                        'Размер изображения
                                        imageData.ScaleAbsolute(100, 25)
                                        pdfContentByteFirst.AddImage(imageData)

#End Region

#Region "подпись на каждой странице(кроме последней) только для контракта"
                                        Dim imagetext As iTextSharp.text.Image
                                        imagetext = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(CreateStamps.CreateStamps.GetStringStamp("Электронный документ подписан ЭП", 26)))
                                        imagetext.SetAbsolutePosition(440, 17) '220
                                        imagetext.ScaleAbsolute(140, 15) '330
                                        For index = 1 To reader.NumberOfPages - 1
                                            Dim pdfcontent = stamper.GetOverContent(index)
                                            pdfcontent.AddImage(imagetext)
                                        Next
#End Region

#Region "печать эцп"
                                        Dim image1 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(stampList(0)))
                                        Dim parser = New parser.PdfReaderContentParser(reader)
                                        Dim finder = parser.ProcessContent(reader.NumberOfPages, New parser.TextMarginFinder())
                                        Dim lastPage As iTextSharp.text.Rectangle = reader.GetPageSize(reader.NumberOfPages)
                                        Dim maxHeightPage = lastPage.Height
                                        Dim maxVerticalHeightPage = 792
                                        Dim lastElemHeight As Integer
                                        lastElemHeight = finder.GetHeight()

                                        Dim newWidth As Integer
                                        Dim newHeight As Integer
                                        Dim isNewPage As Boolean
                                        If maxHeightPage > lastElemHeight + 280 - 100 Then
                                            newWidth = 17
                                            newHeight = 17
                                            isNewPage = False
                                        Else
                                            isNewPage = True
                                            Dim rectangle = reader.GetPageSize(1)
                                            stamper.InsertPage(reader.NumberOfPages + 1, rectangle)
                                            stamper.GetOverContent(reader.NumberOfPages - 1).AddImage(imagetext)
                                            newWidth = 17
                                            newHeight = maxVerticalHeightPage - (10 + 120)
                                        End If
                                        Dim pdfContentByte As PdfContentByte = stamper.GetOverContent(reader.NumberOfPages)
                                        'Позиция изображения
                                        image1.SetAbsolutePosition(newWidth, newHeight)
                                        'Размер изображения
                                        image1.ScaleAbsolute(280, 120)
                                        pdfContentByte.AddImage(image1)
                                        If stampList.Count > 1 Then
#Region "печать приемки"
                                            Dim _image1 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(_stampList(0)))
                                            Dim _maxVerticalHeightPage = 792 - 122
                                            Dim _newWidth As Integer
                                            Dim _newHeight As Integer
                                            Dim _isNewPage As Boolean

                                            _newWidth = 17
                                            _newHeight = 17
                                            _isNewPage = False

                                            'If maxHeightPage > lastElemHeight + 280 Then
                                            '    _newWidth = 17
                                            '    _newHeight = 17
                                            '    _isNewPage = False
                                            'Else
                                            '    _isNewPage = True
                                            '    Dim rectangle = reader.GetPageSize(1)
                                            '    stamper.InsertPage(reader.NumberOfPages + 1, rectangle)
                                            '    Try
                                            '        stamper.GetOverContent(reader.NumberOfPages - 1).AddImage(imagetext)
                                            '    Catch ex As Exception
                                            '        Throw ex
                                            '    End Try
                                            '    _newWidth = 17
                                            '    _newHeight = _maxVerticalHeightPage - (10 + 120)
                                            'End If

                                            Dim _pdfContentByte As PdfContentByte = stamper.GetOverContent(reader.NumberOfPages)
                                            Dim _image2 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(_stampList(_stampList.Count - 1)))
                                            'Позиция изображения
                                            If _isNewPage Then
                                                '_image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), _maxVerticalHeightPage - (10 + 120) + 122)
                                            Else
                                                _image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), 17 + 122)
                                            End If
                                            'Размер изображения
                                            _image2.ScaleAbsolute(280, 120)
                                            _pdfContentByte.AddImage(_image2)
#End Region
                                            Dim image2 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(stampList(stampList.Count - 1)))
                                            'Позиция изображения
                                            If isNewPage Then
                                                image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), maxVerticalHeightPage - (10 + 120))
                                            Else
                                                image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), 17)
                                            End If
                                            'Размер изображения
                                            image2.ScaleAbsolute(280, 120)
                                            pdfContentByte.AddImage(image2)
                                        End If
                                    End If
#End Region
                                End Using
                            End Using
                        End Using

                        'If printExcel.pvId = 1657790 Then
                        'Dim rn As Cell = "DATE1"
                        'End If

                        layoutStamps.pdfFiles.Add(layoutStamps.pdfFileNamePathExtension)

                        If File.Exists(printExcel.docFileNamePathExtension) Then
                            File.Delete(printExcel.docFileNamePathExtension)
                        End If
                    Catch ex As Exception
                        If File.Exists(printExcel.docFileNamePathExtension) Then
                            File.Delete(printExcel.docFileNamePathExtension)
                        End If
                        If File.Exists(layoutStamps.pdfFileNamePathExtension) Then
                            'pdfFile = File.ReadAllBytes(pdfFileNamePathExtension)
                            File.Delete(layoutStamps.pdfFileNamePathExtension)
                        End If
                        responseData.IsError = True
                        responseData.ErrorText = CSKLAD.noPaperAPIException.LayoutStamp
                        Throw New Exception()
                    End Try
                End Using
            End Using
            Return layoutStamps.pdfFiles
        End Function

        Public Shared Function LayoutStampsExcel(savePath As String, layoutStamps As Models.LayoutStamps, printExcel As PrintExcel, responseData As ResponseData) 'Альбомная ориентация
            Dim pdfFileNamePathExtension = $"{savePath}\{printExcel.docFileName}.pdf"

            'Список для штампов
            Dim stampList As List(Of Bitmap)
            stampList = CreateStamps.CreateStamps.GetStamps(layoutStamps.sign, layoutStamps.signIden)

            If layoutStamps.signApt IsNot Nothing Then
                CreateStamps.CreateStamps.GetStamps(layoutStamps.signApt, layoutStamps.signIden, stampList, 2)
            End If

            Using workbook As New Workbook()
                Using inputPdfStream As New MemoryStream
                    Dim mybytes As Byte()

                    workbook.LoadDocument(printExcel.docFileNamePathExtension)
                    workbook.ExportToPdf(inputPdfStream)

                    mybytes = inputPdfStream.ToArray

                    Try
                        Using outputPdfStream As New FileStream(pdfFileNamePathExtension, FileMode.Create, FileAccess.Write, FileShare.None)
                            Using reader = New PdfReader(mybytes)
                                Using stamper = New PdfStamper(reader, outputPdfStream)
                                    If stampList.Count > 0 Then
#Region "дата справа сверху"
                                        Dim pdfContentByteFirst As PdfContentByte = stamper.GetOverContent(1)
                                        Dim imageData As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(CreateStamps.CreateStamps.GetDataStamp(Date.Now.ToString("dd.MM.yyyy"))))
                                        'Позиция изображения
                                        imageData.SetAbsolutePosition(reader.GetPageSize(1).Width - (100 + 17), reader.GetPageSize(1).Height - (25 + 17))
                                        'Размер изображения
                                        imageData.ScaleAbsolute(100, 25)
                                        pdfContentByteFirst.AddImage(imageData)

#End Region

#Region "подпись на каждой странице(кроме последней) только для контракта"
                                        Dim imagetext As iTextSharp.text.Image
                                        imagetext = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(CreateStamps.CreateStamps.GetStringStamp("Электронный документ подписан ЭП", 26)))
                                        imagetext.SetAbsolutePosition(685, 17) '220
                                        imagetext.ScaleAbsolute(140, 15) '330
                                        For index = 1 To reader.NumberOfPages - 1
                                            Dim pdfcontent = stamper.GetOverContent(index)
                                            pdfcontent.AddImage(imagetext)
                                        Next
#End Region

#Region "печать эцп"
                                        Dim image1 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(stampList(0)))
                                        Dim parser = New parser.PdfReaderContentParser(reader)
                                        Dim finder = parser.ProcessContent(reader.NumberOfPages, New parser.TextMarginFinder())
                                        Dim lastPage As iTextSharp.text.Rectangle = reader.GetPageSize(reader.NumberOfPages)
                                        Dim maxHeightPage = lastPage.Height
                                        Dim maxVerticalHeightPage = 592
                                        Dim lastElemHeight As Integer
                                        lastElemHeight = finder.GetHeight()

                                        Dim newWidth As Integer
                                        Dim newHeight As Integer
                                        Dim isNewPage As Boolean
                                        If maxHeightPage > lastElemHeight + 280 - 100 Then
                                            newWidth = 17
                                            newHeight = 17
                                            isNewPage = False
                                        Else
                                            isNewPage = True
                                            Dim rectangle = reader.GetPageSize(1)
                                            stamper.InsertPage(reader.NumberOfPages + 1, rectangle)
                                            stamper.GetOverContent(reader.NumberOfPages - 1).AddImage(imagetext)
                                            newWidth = 17
                                            'newHeight = maxHeightPage - (10 + 120)
                                            newHeight = maxVerticalHeightPage - (10 + 120)
                                        End If
                                        Dim pdfContentByte As PdfContentByte = stamper.GetOverContent(reader.NumberOfPages)
                                        'Позиция изображения
                                        image1.SetAbsolutePosition(newWidth, newHeight - 10)
                                        'Размер изображения
                                        image1.ScaleAbsolute(280, 120)
                                        pdfContentByte.AddImage(image1)
                                        If stampList.Count > 1 Then
                                            Dim image2 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(CreateStamps.CreateStamps.ImageToBytes(stampList(stampList.Count - 1)))
                                            'Позиция изображения
                                            If isNewPage Then
                                                image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), maxVerticalHeightPage - (10 + 120) - 10)
                                            Else
                                                image2.SetAbsolutePosition(reader.GetPageSize(reader.NumberOfPages).Width - (280 + 17), 17 - 10)
                                            End If
                                            'Размер изображения
                                            image2.ScaleAbsolute(280, 120)
                                            pdfContentByte.AddImage(image2)
                                        End If
                                    End If
#End Region
                                End Using
                            End Using
                        End Using
                        'pdfFile = File.ReadAllBytes(pdfFileNamePathExtension)
                        layoutStamps.pdfFiles.Add(pdfFileNamePathExtension)

                        If File.Exists(printExcel.docFileNamePathExtension) Then
                            File.Delete(printExcel.docFileNamePathExtension)
                        End If
                        'If File.Exists(pdfFileNamePathExtension) Then
                        '    File.Delete(pdfFileNamePathExtension)
                        'End If
                        'Dim rn As Cell = "DATE1"
                    Catch ex As Exception
                        If File.Exists(printExcel.docFileNamePathExtension) Then
                            File.Delete(printExcel.docFileNamePathExtension)
                        End If
                        If File.Exists(pdfFileNamePathExtension) Then
                            'pdfFile = File.ReadAllBytes(pdfFileNamePathExtension)
                            File.Delete(pdfFileNamePathExtension)
                        End If
                        responseData.IsError = True
                        responseData.ErrorText = CSKLAD.noPaperAPIException.LayoutStamp
                        Throw New Exception()
                    End Try
                End Using
            End Using
            Return layoutStamps.pdfFiles
        End Function
    End Class
End Namespace