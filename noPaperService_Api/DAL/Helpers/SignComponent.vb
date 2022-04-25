Namespace Helpers
    Public Class SignComponent
        Inherits Entities.SignComponent
        Public Shared Function [New](ByVal signDateTime As Date, ByVal signCert As Entities.CertComponent) As Entities.SignComponent
            Dim signComponent As New Entities.SignComponent
            signComponent.SignDateTimeUtc = signDateTime
            signComponent.SignCer = signCert
            Return signComponent
        End Function
    End Class
End Namespace