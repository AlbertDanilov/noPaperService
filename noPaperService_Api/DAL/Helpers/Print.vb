Imports System.Drawing
Imports System.IO
Imports DevExpress.Spreadsheet
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native
Imports Newtonsoft.Json
Imports noPaperService_Api.Entities

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

    Public Shared Sub PrintExcelInvoice(mainPath As String, printExcel As PrintExcel, responseData As ResponseData)
        Dim json As String = File.ReadAllText(printExcel.jsonFileNamePath)
        Dim pv As noPaperService_common.Entities.EcpSignData_pv = JsonConvert.DeserializeObject(Of noPaperService_common.Entities.EcpSignData_pv)(json)

        printExcel.docFileName = $"{printExcel.nameFile} {pv.pv_nom} от {Date.Now:dd.MM.yyyy HH.mm.ss}"
        printExcel.docFileNamePath = $"{mainPath}\{printExcel.docFileName}"
        printExcel.docFileNamePathExtension = $"{printExcel.docFileNamePath}.xlsx"

        Using wb As New Workbook()
            wb.LoadDocument(printExcel.docTemplateFileNamePath)
            Dim ws As Worksheet = wb.Worksheets(0)
            Dim rowIndexPaste As Integer = 27
            Dim rowIndexFormat As Integer = 32
            Dim rowIndexSum As Integer = 5
            Dim pageBreak As Integer = 35 'размер итоговой части
            'Dim pageLenght As Integer = 67 '88
            'Dim pageLenghtSum As Integer = 91 '88
            'Dim pageLenghtRow As Integer = 67

            Dim pageLenght As Integer = 14
            Dim pageLenghtSum As Integer = 17
            Dim pageLenghtBool As Boolean = False
            'Dim pageLenghtRow As Integer = 67
            If pv.pvsList.Count > 1 AndAlso pv.pvsList.Count < 14 Then
                pageLenghtBool = True
            End If

            Dim allSumOptBnds As Decimal = 0
            Dim allSumRoznNds As Decimal = 0
            Dim allSumNdsRozn As Decimal = 0
            Dim ndsSumOpt As Decimal = 0
            Dim ndsSumRozn As Decimal = 0

            Dim ks As String
            Dim rs As Long
            Dim sumToString As New noPaperService_common.Helpers.SumToString

            Dim zayTypeS As String = String.Empty
            Dim osnName As String = String.Empty
            Dim prim As String = String.Empty

            wb.Unit = DevExpress.Office.DocumentUnit.Point
            wb.BeginUpdate()

            'Dim rn As Cell = "DATE1"
            Try
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

                ws.Range("T19").Value = $"{pv.pv_nom}/ {pv.pv_sklad_iname}"
                ws.Range("AB19").Value = pv.pv_otr_date?.ToString("dd.MM.yyyy")
                ws.Range("AH19").Value = pv.pv_otg_date?.ToString("dd.MM.yyyy")

                ws.Range("I12").Value = osnName

                ws.Range("G21").Value = pv.pv_otv_fio
                ws.Range("G22").Value = prim

                printExcel.pvAgentPrintname = pv.pv_agent_printname

                For Each pvs As noPaperService_common.Entities.EcpSignData_pvs In pv.pvsList
                    ws.Range($"A{rowIndexPaste}").Value = k

                    ws.Range($"D{rowIndexPaste}").Value = pvs.ttnsInfo.ttns_shifr
                    ws.Range($"I{rowIndexPaste}").Value = pvs.pvs_dg_num

                    ws.Range($"D{rowIndexPaste + 2}").Value = $"{pvs.ttnsInfo.ttns_sert_num}, {pvs.ttnsInfo.ttns_sert_date_s?.ToString("dd.MM.yyyy")}"

                    ws.Range($"W{rowIndexPaste}").Value = If(pvs.ttnsInfo.ttns_p_name_s, pvs.ttnsInfo.ttns_nommodif)

                    ws.Range($"W{rowIndexPaste + 3}").Value = pvs.ttnsInfo.ttns_seria
                    ws.Range($"W{rowIndexPaste + 4}").Value = pvs.ttnsInfo.ttns_sgod?.ToString("dd.MM.yyyy")

                    ws.Range($"AI{rowIndexPaste + 3}").Value = Decimal.Round(pvs.pvs_kol_tov.Value, 2)
                    ws.Range($"AI{rowIndexPaste + 4}").Value = pvs.ttnsInfo.ttns_ed_shortname.ToString

                    ws.Range($"AM{rowIndexPaste + 3}").Value = pvs.ttnsInfo.ttns_temp_regim_name
                    ws.Range($"AW{rowIndexPaste + 3}").Value = Decimal.Round(pvs.ttnsInfo.ttns_rcena_nds.Value, 2)
                    ws.Range($"BD{rowIndexPaste + 3}").Value = Decimal.Round(pvs.ttnsInfo.ttns_rcena_nds.Value * pvs.pvs_kol_tov.Value, 2)

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
                        ws.Rows.Insert(rowIndexFormat, rowIndexSum)
                        ws.Range($"A{rowIndexFormat}").CopyFrom(ws.Range($"A{rowIndexPaste}:BI{rowIndexFormat - 1}"), PasteSpecial.Formats)
                        rowIndexFormat += rowIndexSum
                        rowIndexPaste += rowIndexSum
                        k += 1

                        'If rowIndexPaste + rowIndexSum >= pageLenght Then
                        '    pageLenght += pageLenghtSum
                        '    pageLenghtRow += pageLenghtSum
                        '    ws.HorizontalPageBreaks.Add(rowIndexPaste - 1) ' разрыв страницы, если превышает определенную длину
                        'End If

                        'If pv.pvsList.Count > 1 AndAlso pv.pvsList.Count < 14 Then
                        'If k = pv.pvsList.Count AndAlso pageLenghtBool Then
                        '    ws.HorizontalPageBreaks.Add(rowIndexPaste - 1) ' разрыв страницы, если превышает определенную длину
                        '    pageLenghtBool = False
                        'Else
                        If k = pageLenght Then
                            pageLenght += pageLenghtSum
                            ws.HorizontalPageBreaks.Add(rowIndexPaste - 1) ' разрыв страницы, если превышает определенную длину
                        End If

                        'If k >= i OrElse k > 1 AndAlso k < 15 Then
                        '    i += 20
                        '    ws.HorizontalPageBreaks.Add(rowIndexPaste - 1) ' разрыв страницы, если превышает определенную длину
                        'End If
                    End If
                Next

                Dim cellrng As CellRange = ws.Range("ROW_LIST")
                'If cellrng.BottomRowIndex >= pageLenghtRow Then
                '    ws.HorizontalPageBreaks.Add(cellrng.BottomRowIndex - pageBreak) 'разрыв страницы на итоговую часть
                'End If
                'i += 17
                If k = pv.pvsList.Count AndAlso pageLenghtBool Then
                    ws.HorizontalPageBreaks.Add(cellrng.BottomRowIndex - pageBreak) 'разрыв страницы на итоговую часть
                ElseIf pageLenght - k < 12 Then
                    ws.HorizontalPageBreaks.Add(cellrng.BottomRowIndex - pageBreak) 'разрыв страницы на итоговую часть
                End If

                ws.Range("OTPUSK_PRODUCE").Value = pv.pv_sklad_mol

                ws.Range("ITOGO").Value = $"ИТОГО ПО ТТН № {pv.pv_nom}/ {pv.pv_sklad_name} ОТ {pv.pv_otr_date?.ToString("dd.MM.yyyy")} отгр {pv.pv_otg_date?.ToString("dd.MM.yyyy")}"

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
                ws.Range("SUM_OPT_TEXT").Value = sumToString.sum_to_string(rs, CByte(ks))

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

                ws.Range("SUM_ROZN_TEXT").Value = sumToString.sum_to_string(rs, CByte(ks))

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
                ws.Range("SUM_NDS_TEXT").Value = sumToString.sum_to_string(rs, CByte(ks))

                ws.Range("SUM_OPT").Value = allSumOptBnds
                ws.Range("SUM_ROZN").Value = allSumRoznNds
                ws.Range("SUM_NDS").Value = allSumNdsRozn

                ws.Range("DATE1").Value = Date.Now.ToString("dd.MM.yyyy")
            Catch ex As Exception
                responseData.IsError = True
                responseData.ErrorText = CSKLAD.noPaperAPIException.PrintExcel
                Throw New Exception()
            Finally
                wb.EndUpdate()
            End Try

            wb.Calculate()

            wb.SaveDocument(printExcel.docFileNamePathExtension, DevExpress.Spreadsheet.DocumentFormat.OpenXml)
        End Using
    End Sub

    Public Shared Sub PrintExcel_PriceApprovalProtocol(mainPath As String, printExcel As PrintExcel, responseData As ResponseData)
        Dim json As String = File.ReadAllText(printExcel.jsonFileNamePath)
        Dim pv As noPaperService_common.Entities.EcpSignData_pv = JsonConvert.DeserializeObject(Of noPaperService_common.Entities.EcpSignData_pv)(json)

        printExcel.docFileName = $"{printExcel.nameFile} {pv.pv_nom} от {Date.Now:dd.MM.yyyy HH.mm.ss}"
        printExcel.docFileNamePath = $"{mainPath}\{printExcel.docFileName}"
        printExcel.docFileNamePathExtension = $"{printExcel.docFileNamePath}.xlsx"

        Dim jnvlCount = pv.pvsList.AsEnumerable.Where(Function(x) x.ttnsInfo.docs_p_jnvls = 1)

        If jnvlCount.Count > 0 Then
            Using wb As New Workbook()
                wb.LoadDocument(printExcel.docTemplateFileNamePath)
                Dim ws As Worksheet = wb.Worksheets(0)
                Dim rowIndexPaste As Integer = 39
                Dim rowIndexFormat As Integer = 43
                Dim rowIndexSum As Integer = 4
                Dim listPage As Integer = 0

                Dim pageBreak As Integer = 9 'размер итоговой части
                Dim pageLenght As Integer = 59
                Dim pageLenghtSum As Integer = 59
                Dim pageLenghtRow As Integer = 52 '49
                Dim pageNameLenght As Integer = 1
                Dim headTableLenght As Integer = 10
                'Dim pageLenghtRow As Integer = 79

                Dim allSumOptBnds As Decimal = 0
                Dim allSumRoznNds As Decimal = 0
                Dim allSumNdsRozn As Decimal = 0
                Dim ndsSumOpt As Decimal = 0
                Dim ndsSumRozn As Decimal = 0

                Dim sumToString As New noPaperService_common.Helpers.SumToString

                Dim zayTypeS As String = String.Empty
                Dim osnName As String = String.Empty
                Dim prim As String = String.Empty

                wb.Unit = DevExpress.Office.DocumentUnit.Point
                wb.BeginUpdate()

                Try
                    Dim k = 1
                    Dim listRng As New List(Of String)

                    listRng.Add("CL28")

                    ws.Range("A13").Value = $"Протокол к накладной № {pv.pv_num} от {pv.pv_otg_date?.ToString("dd.MM.yyyy")}"
                    ws.Range("G22").Value = pv.pv_agent_agnabbr
                    ws.Range("A28").Value = $"Дата отгрузки: {pv.pv_otg_date?.ToString("dd.MM.yyyy")}"

                    'printExcel.pvAgentPrintname = pv.pv_agent_printname

                    For Each pvs As noPaperService_common.Entities.EcpSignData_pvs In pv.pvsList
                        ws.Range($"A{rowIndexPaste}").Value = pvs.ttnsInfo.docs_p_mnn '1
                        ws.Range($"G{rowIndexPaste}").Value = pvs.ttnsInfo.docs_p_tn '2
                        ws.Range($"S{rowIndexPaste}").Value = pvs.ttnsInfo.ttns_seria '3
                        ws.Range($"X{rowIndexPaste}").Value = pvs.ttnsInfo.docs_p_proizv '4
                        ws.Range($"AF{rowIndexPaste}").Value = pvs.ttnsInfo.docs_p_prcena_proizv.Value '5
                        ws.Range($"AJ{rowIndexPaste}").Value = pvs.ttnsInfo.docs_prcena_bnds.Value '6
                        ws.Range($"AN{rowIndexPaste}").Value = pvs.ttnsInfo.docs_prcena_nds.Value '7

                        ws.Range($"AW{rowIndexPaste}").Value = pvs.pvs_pcena_bnds.Value '10
                        ws.Range($"AN{rowIndexPaste}").Value = pvs.pvs_pcena_nds.Value '11
                        ws.Range($"BA{rowIndexPaste}").Value = pvs.ttnsInfo.nac_prc_val_p.Value '12
                        ws.Range($"BE{rowIndexPaste}").Value = pvs.ttnsInfo.nac_sum_val_p2.Value '13
                        ws.Range($"BG{rowIndexPaste}").Value = pvs.ttnsInfo.docs_ocena_bnds.Value '15
                        ws.Range($"BQ{rowIndexPaste}").Value = pvs.ttnsInfo.ttns_ocena_nds.Value '16
                        ws.Range($"BM{rowIndexPaste}").Value = pvs.ttnsInfo.nac_prc_val.Value '17
                        ws.Range($"BW{rowIndexPaste}").Value = pvs.ttnsInfo.nac_sum_val.Value '18
                        ws.Range($"CA{rowIndexPaste}").Value = pvs.ttnsInfo.nac_prc_rozn_val.Value '19
                        ws.Range($"CC{rowIndexPaste}").Value = pvs.ttnsInfo.nac_sum_rozn_val.Value '20
                        ws.Range($"CI{rowIndexPaste}").Value = pvs.ttnsInfo.rcena_bnds.Value '22

                        If k < pv.pvsList.Count Then
                            Dim temprowIndexPaste = rowIndexPaste
                            temprowIndexPaste += rowIndexSum

                            If temprowIndexPaste + rowIndexSum > pageLenght Then
                                ws.Rows.Insert(rowIndexFormat, pageNameLenght) 'смещаем вниз на одну позицию, чтобы добавить пустую строку
                                ws.Range($"A{rowIndexFormat}").CopyFrom(ws.Range("A28:CL28"), PasteSpecial.All)
                                rowIndexFormat += pageNameLenght
                                rowIndexPaste += pageNameLenght
                                ws.Rows.Insert(rowIndexFormat, headTableLenght)
                                ws.Range($"A{rowIndexFormat}").CopyFrom(ws.Range("A29:CL38"), PasteSpecial.All)
                                rowIndexFormat += headTableLenght
                                rowIndexPaste += headTableLenght
                            End If

                            ws.Rows.Insert(rowIndexFormat, rowIndexSum)
                            ws.Range($"A{rowIndexFormat}").CopyFrom(ws.Range("A39:CL42"), PasteSpecial.Formats)
                            rowIndexFormat += rowIndexSum
                            rowIndexPaste += rowIndexSum
                            k += 1

                            If rowIndexPaste + rowIndexSum > pageLenght Then
                                pageLenght += pageLenghtSum
                                pageLenghtRow += pageLenghtSum
                                ws.HorizontalPageBreaks.Add(rowIndexPaste - headTableLenght - pageNameLenght - 1) ' разрыв страницы, если превышает определенную длину
                                listRng.Add($"CL{rowIndexPaste - headTableLenght - pageNameLenght}")
                            End If
                        End If

                    Next

                    Dim cellrng As CellRange = ws.Range("ROW_LIST")
                    If cellrng.BottomRowIndex >= pageLenghtRow - 1 Then
                        Dim row As Integer = cellrng.BottomRowIndex - pageBreak
                        ws.HorizontalPageBreaks.Add(row) 'разрыв страницы на итоговую часть
                        listRng.Add($"CL{row + 1}")
                    End If

                    Dim list As Short = 1
                    listPage += listRng.Count

                    For Each cRng As String In listRng
                        ws.Range(cRng).Font.Size = 11
                        ws.Range(cRng).Alignment.Vertical = SpreadsheetVerticalAlignment.Center
                        ws.Range(cRng).Alignment.Horizontal = SpreadsheetHorizontalAlignment.Right
                        ws.Range(cRng).Value = $"Страница {list} из {listPage}"
                        list += 1
                    Next

                    'Dim rn As Cell = "DATE1"
                Catch ex As Exception
                    responseData.IsError = True
                    responseData.ErrorText = CSKLAD.noPaperAPIException.PrintExcel
                    Throw New Exception()
                Finally
                    wb.EndUpdate()
                End Try

                wb.Calculate()

                wb.SaveDocument(printExcel.docFileNamePathExtension, DevExpress.Spreadsheet.DocumentFormat.OpenXml)
            End Using
        Else
            responseData.IsError = True
            responseData.ErrorText = CSKLAD.noPaperAPIException.Jnvls
            Throw New Exception()
        End If
    End Sub
End Class
