Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        ViewData("Title") = "Home Page"

        Return View()
    End Function

    Public Function Download(ByRef pdfFile As String)
        Return File(pdfFile, "application/octet-stream", "Согласие " + ".docx")
    End Function
End Class
