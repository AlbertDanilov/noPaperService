Namespace Helpers
    Public Class SignComponent
        Inherits Models.SignComponent
        Public Shared Function [New](ByVal signDateTime As Date, ByVal signCert As Models.CertComponent) As Models.SignComponent
            Dim signComponent As New Models.SignComponent
            signComponent.SignDateTimeUtc = signDateTime
            signComponent.SignCer = signCert
            Return signComponent
        End Function
    End Class
End Namespace