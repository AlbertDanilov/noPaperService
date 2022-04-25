Imports System.IO
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native
Imports Newtonsoft.Json
Public Class PrintDoc
    Public Shared Function Print(mainPath As String, jsonFileNamePath As String, ByRef docFileName As String, ByRef docFileNamePath As String, docTemplateFileNamePath As String, ByRef docFileNamePathExtension As String)
        Dim docFile As Byte() = Nothing

        Dim json As String = File.ReadAllText(jsonFileNamePath)

        Dim document1 As EcpSignData_pv = JsonConvert.DeserializeObject(Of EcpSignData_pv)(json)

        docFileName = $"Накладная {document1.pv_nom} от ({Date.Now.ToString("dd.MM.yyyy HH.mm.ss")})"
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
        nameBookmark = document.Bookmarks("ttns_shifr")

        For Each i As EcpSignData_pvs In document1.pvsList
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_shifr)
            nameBookmark = document.Bookmarks("ttns_nommodif")
            document.Replace(nameBookmark.Range, If(i.ttnsInfo.ttns_p_name_s, i.ttnsInfo.ttns_nommodif))
            nameBookmark = document.Bookmarks("ttns_prcena_bnds")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_prcena_bnds)
            nameBookmark = document.Bookmarks("ttns_ocena_nds")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_ocena_nds)
            nameBookmark = document.Bookmarks("ttns_rcena_nds")
            document.Replace(nameBookmark.Range, i.ttnsInfo.ttns_rcena_nds)
        Next

        server.SaveDocument(docFileNamePathExtension, DocumentFormat.OpenXml)

        docFile = File.ReadAllBytes(docFileNamePathExtension)
        Return docFile
    End Function
End Class
