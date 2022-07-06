Imports System.Drawing
Imports System.IO
Imports DevExpress.Spreadsheet
Imports DevExpress.XtraRichEdit
Imports iTextSharp.text.pdf
Imports noPaperService_Api.Models

Namespace Helpers
    Public Class ConvertToPDF
        Public Shared Function ConvertToPDFExcelBook(savePath As String, layoutStamps As Models.LayoutStamps, printExcel As PrintExcel, responseData As ResponseData) 'Книжная ориентация
            layoutStamps.pdfFileNamePathExtension = $"{savePath}\{printExcel.docFileName}.pdf"

            Dim listAppPDF As String

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
                                End Using
                            End Using
                        End Using

                        listAppPDF = layoutStamps.pdfFileNamePathExtension

                        'Dim rn As Cell = "DATE1"
                        If File.Exists(printExcel.docFileNamePathExtension) Then
                            File.Delete(printExcel.docFileNamePathExtension)
                        End If
                    Catch ex As Exception
                        If File.Exists(printExcel.docFileNamePathExtension) Then
                            File.Delete(printExcel.docFileNamePathExtension)
                        End If
                        If File.Exists(layoutStamps.pdfFileNamePathExtension) Then
                            File.Delete(layoutStamps.pdfFileNamePathExtension)
                        End If
                        responseData.IsError = True
                        responseData.ErrorText = CSKLAD.noPaperAPIException.ConertToPDF
                        Throw New Exception()
                    End Try
                End Using
            End Using
            Return listAppPDF
        End Function
    End Class
End Namespace