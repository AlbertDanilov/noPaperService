Imports System.IO
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native
Imports Newtonsoft.Json
Public Class PrintDoc
    Public Shared Function Print(mainPath As String, jsonFileNamePath As String, ByRef docFileName As String, ByRef docFileNamePath As String, docTemplateFileNamePath As String, ByRef docFileNamePathExtension As String)
        Dim docFile As Byte() = Nothing

        Dim json As String = File.ReadAllText(jsonFileNamePath)

        Dim document1 As EcpSignData_pv = JsonConvert.DeserializeObject(Of EcpSignData_pv)(json)
        Dim document2 As EcpSignData_pvs = JsonConvert.DeserializeObject(Of EcpSignData_pvs)(json)
        Dim document3 As EcpSignData_ttns = JsonConvert.DeserializeObject(Of EcpSignData_ttns)(json)

        docFileName = $"Накладная {document1.pv_nom} от ({Date.Now.ToString("dd.MM.yyyy HH.mm.ss")})"
        docFileNamePath = $"{mainPath}\{docFileName}"
        docFileNamePathExtension = $"{docFileNamePath}.docx"

        Dim server As New RichEditDocumentServer()
        Dim document = server.Document
        server.LoadDocument(docTemplateFileNamePath)

        Dim nameBookmark As Bookmark = document.Bookmarks("document_num")
        document.Replace(nameBookmark.Range, document1.pv_num)
        nameBookmark = document.Bookmarks("agent_recipient")
        document.Replace(nameBookmark.Range, document1.pv_agent_agnabbr)
        nameBookmark = document.Bookmarks("create_date")
        document.Replace(nameBookmark.Range, document1.pv_create_date.Value.ToString())
        nameBookmark = document.Bookmarks("otgr_date")
        document.Replace(nameBookmark.Range, document1.pv_otg_date.Value.ToString())

        server.SaveDocument(docFileNamePathExtension, DocumentFormat.OpenXml)

        docFile = File.ReadAllBytes(docFileNamePathExtension)
        Return docFile
    End Function
End Class
