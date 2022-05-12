Imports System.Drawing
Imports System.IO
Imports DevExpress.Spreadsheet
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native
Imports Newtonsoft.Json
Imports noPaperService_Api.Helpers

Public Class Print
    Public Shared Function PrintDoc(mainPath As String, jsonFileNamePath As String, ByRef docFileName As String, ByRef docFileNamePath As String, docTemplateFileNamePath As String, ByRef docFileNamePathExtension As String)
        Dim docFile As Byte() = Nothing

        Dim json As String = File.ReadAllText(jsonFileNamePath)

        Dim document1 As noPaperService_common.Entities.EcpSignData_pv = JsonConvert.DeserializeObject(Of noPaperService_common.Entities.EcpSignData_pv)(json)

        docFileName = $"Накладная {document1.pv_nom} от ({Date.Now:dd.MM.yyyy HH.mm.ss})"
        docFileNamePath = $"{mainPath}\{docFileName}"
        docFileNamePathExtension = $"{docFileNamePath}.docx"

        Dim server As New RichEditDocumentServer()
        Dim document = server.Document
        server.LoadDocument(docTemplateFileNamePath)

        Dim nameBookmark As Bookmark = document.Bookmarks("document_num")
        document.Replace(nameBookmark.Range, document1.pv_num)
        nameBookmark = document.Bookmarks("pv_agent_agnabbr")
        document.Replace(nameBookmark.Range, document1.pv_agent_agnabbr)
        nameBookmark = document.Bookmarks("pv_plat_agnabbr")
        document.Replace(nameBookmark.Range, document1.pv_plat_agnabbr)
        nameBookmark = document.Bookmarks("create_date")
        document.Replace(nameBookmark.Range, document1.pv_create_date.Value.ToString())
        nameBookmark = document.Bookmarks("otgr_date")
        document.Replace(nameBookmark.Range, document1.pv_otg_date.Value.ToString())

        For Each i As noPaperService_common.Entities.EcpSignData_pvs In document1.pvsList
            nameBookmark = document.Bookmarks("ttns_shifr")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_shifr)
            nameBookmark = document.Bookmarks("ttns_nommodif")
            document.Replace(nameBookmark.Range, If(i.ttnsInfo.ttns_p_name_s, i.ttnsInfo.ttns_nommodif))
            nameBookmark = document.Bookmarks("ttns_prcena_bnds")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_prcena_bnds)
            nameBookmark = document.Bookmarks("ttns_ocena_nds")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_ocena_nds)
            nameBookmark = document.Bookmarks("ttns_rcena_nds")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_rcena_nds)
            nameBookmark = document.Bookmarks("copy_paste")
            document.Copy(nameBookmark.Range)
            'nameBookmark = document.Bookmarks("paste")
            'document.Paste(nameBookmark.Range)
            Exit For
        Next

        server.SaveDocument(docFileNamePathExtension, DevExpress.XtraRichEdit.DocumentFormat.OpenXml)

        docFile = File.ReadAllBytes(docFileNamePathExtension)
        Return docFile
    End Function

    Public Shared Sub PrintExcel(mainPath As String, jsonFileNamePath As String, ByRef docFileName As String, ByRef docFileNamePath As String, docTemplateFileNamePath As String, ByRef docFileNamePathExtension As String)
        Dim docFile As Byte() = Nothing
        Dim json As String = File.ReadAllText(jsonFileNamePath)
        Dim pv As noPaperService_common.Entities.EcpSignData_pv = JsonConvert.DeserializeObject(Of noPaperService_common.Entities.EcpSignData_pv)(json)

        docFileName = $"Накладная {pv.pv_nom} от ({Date.Now:dd.MM.yyyy HH.mm.ss})"
        docFileNamePath = $"{mainPath}\{docFileName}"
        docFileNamePathExtension = $"{docFileNamePath}.xlsx"

        Using wb As New Workbook()
            wb.LoadDocument(docTemplateFileNamePath)
            Dim ws As Worksheet = wb.Worksheets(0)
            Dim rowIndexPaste As Integer = 27
            Dim rowIndexFormat As Integer = 31
            Dim rowIndexSum As Integer = 4
            Dim pageBreak As Integer = 35
            Dim pageLenght As Integer = 88
            Dim pageLenghtSum As Integer = 88
            Dim pageLenghtRow As Integer = 79

            Dim allSumOptBnds As Decimal = 0
            Dim allSumRoznNds As Decimal = 0
            Dim allSumNdsRozn As Decimal = 0
            Dim ndsSumOpt As Decimal = 0
            Dim ndsSumRozn As Decimal = 0

            Dim ks As String
            Dim rs As Long
            Dim pme_WPROG As New pme.WPROG

            Dim zayTypeS As String = String.Empty
            Dim osnName As String = String.Empty
            Dim prim As String = String.Empty

            If pv.pv_work_program_id = CSKLAD.c_WORK_PROG_ROZN Then
                zayTypeS = "Сводная заявка № "
                prim = ""
            ElseIf pv.pv_work_program_id = CSKLAD.c_WORK_PROG_RODSERT Then
                zayTypeS = "Заявка № "
                prim = pv.pv_zay_lpu
            ElseIf pv.pv_work_program_id = CSKLAD.c_WORK_PROG_ONLS Then
                zayTypeS = "Заявка № "
                prim = ""
            ElseIf pv.pv_work_program_id = CSKLAD.c_WORK_PROG_7NOZ Then
                zayTypeS = "Заявка № "
                prim = ""
            ElseIf pv.pv_work_program_id = CSKLAD.c_WORK_PROG_SPEC_PROG Then
                zayTypeS = ""
                prim = pv.pv_zay_lpu
            ElseIf pv.pv_work_program_id = CSKLAD.c_WORK_PROG_10ST Then
                zayTypeS = "Заявка № "
                If pv.pv_sklad_iname = "МЗ РФ 3" Then
                    prim = "Гос. контракт № 12-216 от 14.08.2012 г."
                Else
                    prim = pv.pv_zay_lpu
                End If
            Else
                zayTypeS = "Заявка № "
                prim = pv.pv_zay_lpu
            End If

            If pv.pv_zay_zname IsNot String.Empty Then
                osnName = zayTypeS & pv.pv_zay_zname & " от " & pv.pv_zay_cdate.Value.ToString("dd.MM.yyyy")
            Else
                osnName = pv.pv_reason
            End If

            wb.Unit = DevExpress.Office.DocumentUnit.Point
            wb.BeginUpdate()

            Try
                Dim k = 1
                Dim rng As CellRange

                ws.Range("I1").Value = pv.pv_agent_printname

                If pv.pv_is_mark.Value > 0I Then
                    rng = ws.Range("AZ11:BH12")
                    rng.Borders.SetOutsideBorders(Color.Black, BorderLineStyle.Thin)
                    rng.Value = "Маркировка"
                End If

                If pv.pv_sklad_name.ToUpper.Contains("ЛПУ2 МО2") Then
                    rng = ws.Range("AZ13:BH14")
                    rng.Borders.SetOutsideBorders(Color.Black, BorderLineStyle.Thin)
                    rng.Value = "Медикаменты МО"
                End If

                rng = ws.Range("AZ17:BH18")
                rng.Value = pv.pv_work_program_name
                rng.FillColor = Color.Gray
                rng.Font.Color = Color.White

                ws.Range("T19").Value = $"{pv.pv_nom}/ {pv.pv_sklad_name}"
                ws.Range("AB19").Value = pv.pv_otr_date.Value.ToString("dd.MM.yyyy")
                ws.Range("AH19").Value = pv.pv_otg_date.Value.ToString("dd.MM.yyyy")

                ws.Range("I12").Value = osnName

                ws.Range("G21").Value = pv.pv_otv_fio
                ws.Range("G22").Value = prim

                For Each pvs As noPaperService_common.Entities.EcpSignData_pvs In pv.pvsList
                    ws.Range($"A{rowIndexPaste}").Value = k

                    ws.Range($"D{rowIndexPaste}").Value = pvs.ttnsInfo.ttns_shifr
                    ws.Range($"I{rowIndexPaste}").Value = pvs.pvs_dg_num

                    ws.Range($"D{rowIndexPaste + 1}").Value = $"{pvs.ttnsInfo.ttns_sert_num}, {pvs.ttnsInfo.ttns_sert_date_po.Value:dd.MM.yyyy}"

                    ws.Range($"W{rowIndexPaste}").Value = If(pvs.ttnsInfo.ttns_p_name_s, pvs.ttnsInfo.ttns_nommodif)

                    ws.Range($"W{rowIndexPaste + 2}").Value = pvs.ttnsInfo.ttns_seria
                    ws.Range($"W{rowIndexPaste + 3}").Value = pvs.ttnsInfo.ttns_sgod.Value.ToString("dd.MM.yyyy")

                    ws.Range($"AI{rowIndexPaste + 2}").Value = Decimal.Round(pvs.pvs_kol_tov.Value, 2)
                    ws.Range($"AI{rowIndexPaste + 3}").Value = pvs.ttnsInfo.ttns_ed_shortname.ToString

                    ws.Range($"AM{rowIndexPaste + 2}").Value = pvs.ttnsInfo.ttns_temp_regim_name
                    ws.Range($"AW{rowIndexPaste + 2}").Value = Decimal.Round(pvs.ttnsInfo.ttns_rcena_nds.Value, 2)
                    ws.Range($"BD{rowIndexPaste + 2}").Value = Decimal.Round(pvs.ttnsInfo.ttns_rcena_nds.Value * pvs.pvs_kol_tov.Value, 2)

                    Dim s = 0
                    s = pvs.ttnsInfo.ttns_nds_i_val + 100
                    s /= 100

                    If pvs.pvs_psum_bnds.HasValue Then
                        allSumOptBnds += Decimal.Round(pvs.pvs_psum_bnds.Value)
                    End If
                    ndsSumOpt = If(pvs.pvs_psum_nds.HasValue, Decimal.Round(pvs.pvs_psum_nds.Value, 2), 0) - If(pvs.pvs_psum_bnds.HasValue, Decimal.Round(pvs.pvs_psum_bnds.Value, 2), 0)
                    ndsSumOpt = Decimal.Round(ndsSumOpt, 2, MidpointRounding.AwayFromZero)
                    ndsSumRozn = If(pvs.pvs_rsum_nds.HasValue, Decimal.Round(pvs.pvs_rsum_nds.Value, 2), 0) - If(pvs.pvs_rsum_nds.HasValue, (Decimal.Round(pvs.pvs_rsum_nds.Value, 2) / Decimal.Round(s, 2)), 0)
                    allSumRoznNds += If(pvs.pvs_rsum_nds.HasValue, Decimal.Round(pvs.pvs_rsum_nds.Value, 2), 0)
                    allSumNdsRozn += Decimal.Round(ndsSumRozn, 2)

                    If k < pv.pvsList.Count Then
                        ws.Rows.Insert(rowIndexFormat, 4)
                        ws.Range($"A{rowIndexFormat}").CopyFrom(ws.Range($"A{rowIndexPaste}:BI{rowIndexFormat - 1}"), PasteSpecial.Formats)
                        rowIndexFormat += rowIndexSum
                        rowIndexPaste += rowIndexSum
                        k += 1

                        If rowIndexPaste + rowIndexSum > pageLenght Then
                            pageLenght += pageLenghtSum
                            pageLenghtRow += pageLenghtSum
                            ws.HorizontalPageBreaks.Add(rowIndexPaste - 1)
                        End If
                    End If
                Next

                Dim cellrng As CellRange = ws.Range("ROW_LIST")
                If cellrng.BottomRowIndex >= pageLenghtRow Then
                    ws.HorizontalPageBreaks.Add(cellrng.BottomRowIndex - pageBreak)
                End If

                ws.Range("OTPUSK_PRODUCE").Value = pv.pv_sklad_mol

                Dim d = cellrng.BottomRowIndex - ws.Range("ITOGO").BottomRowIndex + 1 'pageBreak

                ws.Range("ITOGO").Value = $"ИТОГО ПО ТТН № {pv.pv_nom}/ {pv.pv_sklad_name} ОТ {pv.pv_otr_date.Value:dd.MM.yyyy} отгр {pv.pv_otg_date.Value:dd.MM.yyyy}"


                allSumOptBnds = Decimal.Round(allSumOptBnds, 2, MidpointRounding.AwayFromZero)
                allSumRoznNds = Decimal.Round(allSumRoznNds, 2, MidpointRounding.AwayFromZero)
                allSumNdsRozn = Decimal.Round(allSumNdsRozn, 2, MidpointRounding.AwayFromZero)

                If allSumOptBnds.ToString.Replace(",", ".").Contains(".") Then
                    ks = allSumOptBnds.ToString.Replace(",", ".").Split(".")(1)
                    rs = CLng(allSumOptBnds.ToString.Replace(",", ".").Split(".")(0))
                Else
                    ks = "0"
                    rs = CLng(allSumOptBnds)
                End If

                If ks.Length = 1 Then
                    ks &= "0"
                End If
                ws.Range("SUM_OPT_TEXT").Value = pme_WPROG.sum_to_string(rs, CByte(ks))

                If allSumRoznNds.ToString.Replace(",", ".").Contains(".") Then
                    ks = allSumRoznNds.ToString.Replace(",", ".").Split(".")(1)
                    rs = CLng(allSumRoznNds.ToString.Replace(",", ".").Split(".")(0))
                Else
                    ks = "0"
                    rs = CLng(allSumRoznNds)
                End If

                If ks.Length = 1 Then
                    ks &= "0"
                End If
                ws.Range("SUM_ROZN_TEXT").Value = pme_WPROG.sum_to_string(rs, CByte(ks))

                If allSumNdsRozn.ToString.Replace(",", ".").Contains(".") Then
                    ks = allSumNdsRozn.ToString.Replace(",", ".").Split(".")(1)
                    rs = CLng(allSumNdsRozn.ToString.Replace(",", ".").Split(".")(0))
                Else
                    ks = "0"
                    rs = CLng(allSumNdsRozn)
                End If

                If ks.Length = 1 Then
                    ks &= "0"
                End If
                ws.Range("SUM_NDS_TEXT").Value = pme_WPROG.sum_to_string(rs, CByte(ks))

                ws.Range("SUM_OPT").Value = allSumOptBnds
                ws.Range("SUM_ROZN").Value = allSumRoznNds
                ws.Range("SUM_NDS").Value = allSumNdsRozn

            Catch ex As Exception
                Throw ex
            Finally
                wb.EndUpdate()
            End Try

            wb.Calculate()

            wb.SaveDocument(docFileNamePathExtension, DevExpress.Spreadsheet.DocumentFormat.OpenXml)
        End Using
    End Sub
End Class
