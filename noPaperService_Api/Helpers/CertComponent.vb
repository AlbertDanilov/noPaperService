Namespace Helpers
    Public Class CertComponent
        Inherits Models.CertComponent

        Public Shared Function [New](cert As System.Security.Cryptography.X509Certificates.X509Certificate2) As Models.CertComponent
            Dim CC As New Models.CertComponent
            If cert IsNot Nothing Then
                CC.cert = cert
                CC.thumbprint = cert.Thumbprint
                CC.issued = GetValue(cert.Issuer, "CN")
                CC.organization = GetValue(cert.Subject, "CN")
                CC.valid = "с " & cert.NotBefore.ToString("dd.MM.yyyy") & " по " & cert.NotAfter.ToString("dd.MM.yyyy")
                CC.subject = GetValue(cert.Subject, "SN") & " " & GetValue(cert.Subject, "G")
                CC.serialNumber = cert.SerialNumber
                CC.fullSubject = GetFullValue(cert.Subject)
                CC.fullIssued = GetFullValue(cert.Issuer)
                CC.subjectPost = GetValue(cert.Subject, "T")
                CC.subjectOrg = GetNumApt(cert.Subject, "OU=", CC.subjectPost)
            End If
            Return CC
        End Function
        Private Shared Function GetNumApt(sender As String, name As String, subjectPost As String) As String
            Dim arrayToComare() As String = {"аптек", "аптеч", "цра"}
            Dim arrayEnding() As String = {"и", "ного пункта", ""}
            'Dim stringToComare As String = "цра"
            Dim apt As String = Nothing
            Dim flag As Boolean = False
            Dim findValue As String = String.Empty
            Dim i As Byte = 0

            Dim listObjects As String() = sender.Split(",")

            For Each item As String In listObjects
                i = 0
                If item.Contains(name) AndAlso item.Contains("№") Then
                    flag = True

                    For Each compare In arrayToComare
                        If i < 2 AndAlso item.ToLower.Contains(compare) Then
                            apt = item.Substring(item.IndexOf("№")).Replace("""", "")
                            findValue = compare
                            GoTo LoopEnd
                        End If
                        i += 1
                    Next

                    If item.ToLower.Contains(arrayToComare(2)) Then
                        apt = item.Substring(item.IndexOf("№")).Replace("""", "")
                        findValue = arrayToComare(2)
                    End If
                ElseIf flag AndAlso item.Contains("№") Then
                    For Each compare In arrayToComare
                        If i < 2 AndAlso item.ToLower.Contains(compare) Then
                            apt = item.Substring(item.IndexOf("№")).Replace("""", "")
                            findValue = compare
                            GoTo LoopEnd
                        End If
                        i += 1
                    Next

                    If item.ToLower.Contains(arrayToComare(2)) Then
                        apt = item.Substring(item.IndexOf("№")).Replace("""", "")
                        findValue = arrayToComare(2)
                    End If
                End If
            Next

LoopEnd:    For Each compare In arrayToComare
                If subjectPost.ToLower.Contains(compare) Then
                    Return apt
                End If
            Next
            i = 0
            For Each compare In arrayToComare
                If findValue.ToLower.Contains(compare) Then
                    apt = compare & $"{arrayEnding(i)} " & apt
                    Return apt
                End If
                i += 1
            Next

            Return apt
        End Function

        Private Shared Function GetOrg(item As String, ByRef findValue As String)
            Dim arrayToComare() As String = {"аптек", "аптеч"}
            Dim stringToComare As String = "цра"
            Dim apt As String = Nothing
            Dim flag As Boolean = False
            Dim i As Byte

            For Each compare In arrayToComare
                If i < 2 AndAlso item.ToLower.Contains(compare) Then
                    apt = item.Substring(item.IndexOf("№")).Replace("""", "")
                    findValue = compare
                    Return apt
                End If
                i += 1
            Next

            If item.Contains(stringToComare) Then
                apt = item.Substring(item.IndexOf("№")).Replace("""", "")
                findValue = stringToComare
            End If

            Return apt
        End Function

        Private Shared Function GetValue(sender As String, name As String) As String
            Dim listObjects As String() = sender.Split(",")
            For Each item As String In listObjects
                Dim nv As New NameValue(item)
                If nv.name = name Then Return nv.value
            Next
            Return Nothing
        End Function
        Private Shared Function GetFullValue(ByVal sender As String) As String
            Dim listObjects As String() = sender.Split(",")
            Dim fullSubject As String = Nothing
            Dim comma As String = Nothing
            For Each item As String In listObjects
                Dim nv As New NameValue(item)
                fullSubject += comma & nv.value
                comma = ", "
            Next
            Return fullSubject
        End Function
        Private Class NameValue
            Public Property name As String = ""
            Public Property value As String = ""

            Public Sub New(sender As String)
                Dim indexOfChar As Integer = sender.IndexOf("=")
                Try
                    Me.name = Trim(sender.Substring(0, indexOfChar))
                    Me.value = sender.Substring(indexOfChar + 1)
                Catch
                    Me.name = ""
                    Me.value = sender
                End Try
            End Sub
        End Class
    End Class
End Namespace
