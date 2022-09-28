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
            Dim listObjects As String() = sender.Split(",")
            Dim apt As String = String.Empty
            Dim flag As Boolean = False
            For Each item As String In listObjects
                If item.Contains(name) Then
                    flag = True
                    If item.Contains("=") Then
                        apt += item.Substring(item.IndexOf("=") + 1)
                    End If
                ElseIf flag Then
                    If item.Contains("=") Then
                        Exit For
                    End If
                    apt += item
                End If
            Next
            Return apt.Replace("""", "")

            '            'Dim arrayToComare() As String = {
            '            '    "^(\S)(\s)аптеч(\S)(\s)от(\S)(\s)аптек(\S)(\s)№(\S)(\s)$",
            '            '    "(\S)(\s)аптеч(\S)(\s)от(\S)(\s)цра(\S)(\s)№(\S)(\s)",
            '            '    "(\S)(\s)аптеч(\S)(\s)№(\S)(\s)",
            '            '    "(\S)(\s)аптек(\S)(\s)№(\S)(\s)",
            '            '    "цра(\S*)(\s*)№(\S*)(\s*)(\d*)",
            '            '    "(\S)(\s)от(\S)(\s)цра(\S)(\s)№(\S)(\s)"
            '            '}
            '            Dim arrayToComare() As String = {
            '                "(аптеч\S*\s*)|(пун\S*\s*)|(от\S*\s*)|(цра\S*\s*)|(№\S*\s*)|(аптек\S*\s*)",
            '                "(аптеч\S*\s*)|(цра\S*\s*)|(аптек\S*\s*)"
            '            }
            '            Dim arrayToCompareT() As String = {"от апт", "от цра", "аптеч", "аптек", "цра"}
            '            Dim arrayToCompareTT() As String = {"аптеч", "аптек", "цра", "№"}
            '            Dim arrayEnding() As String = {"а", "ный пункт", ""}
            '            Dim apt As String = Nothing
            '            Dim flag As Boolean = False
            '            Dim findValue As String = String.Empty
            '            Dim i As Byte

            '            Dim listObjects As String() = sender.Split(",") '{"OU= цра №65", "OU= аптечный пункт от цра №55", "OU= аптечный пункт от аптеки №65", "OU= аптека №54"} 'sender.Split(",")

            '            Dim str As String = String.Empty
            '            Dim regex As New Regex(arrayToComare(1))
            '            Dim matches As MatchCollection = regex.Matches(subjectPost.ToLower)
            '            'If matches.Count > 0 Then
            '            '    For Each match As Match In matches
            '            '        str += match.Value
            '            '    Next
            '            'End If
            '            Dim d = str

            '            For Each item As String In listObjects
            '                str = String.Empty
            '                If item.Contains(name) AndAlso item.Contains("№") Then
            '                    flag = True
            '                    regex = New Regex(arrayToComare(0))
            '                    matches = regex.Matches(item.ToLower)
            '                    If matches.Count > 0 Then
            '                        For Each match As Match In matches
            '                            For Each compare In arrayToCompareTT
            '                                If Not subjectPost.Contains(compare) AndAlso match.Value.Contains(compare) Then
            '                                    str += match.Value
            '                                End If
            '                            Next
            '                        Next
            '                    End If
            '                    d = str
            '                ElseIf flag AndAlso item.Contains("№") Then
            '                    regex = New Regex(arrayToComare(0))
            '                    matches = regex.Matches(item.ToLower)
            '                    If matches.Count > 0 Then
            '                        For Each match As Match In matches
            '                            For Each compare In arrayToCompareTT
            '                                If Not subjectPost.Contains(compare) AndAlso match.Value.Contains(compare) Then
            '                                    str += match.Value
            '                                End If
            '                            Next
            '                        Next
            '                    End If
            '                    d = str
            '                End If

            '                'If item.Contains(name) AndAlso item.Contains("№") Then
            '                '    flag = True

            '                '    For Each compare In arrayToComare
            '                '        Dim regex As New Regex(compare)
            '                '        Dim matches As MatchCollection = regex.Matches(item.ToLower)
            '                '        Dim str As String = String.Empty
            '                '        If matches.Count > 0 Then
            '                '            For Each match As Match In matches
            '                '                str += match.Value
            '                '            Next
            '                '        End If
            '                '    Next

            '                '    'Do While i < 2
            '                '    '    If item.ToLower.Contains(arrayToComare(i)) Then
            '                '    '        Dim str = item.Substring(item.ToLower.IndexOf(arrayToComare(i))).Replace("""", "")
            '                '    '        apt = str.Substring(str.IndexOf("№"))
            '                '    '        findValue = arrayToComare(i)
            '                '    '        GoTo LoopEnd
            '                '    '    End If
            '                '    '    i += 1
            '                '    'Loop

            '                '    'If item.ToLower.Contains(arrayToComare(i)) Then
            '                '    '    Dim strCompare = arrayToComare(i)

            '                '    '    i += 1
            '                '    '    If item.ToLower.Contains(arrayToComare(i)) Then
            '                '    '        strCompare = arrayToComare(i)
            '                '    '    End If

            '                '    '    Dim str = item.Substring(item.ToLower.IndexOf(strCompare)).Replace("""", "")
            '                '    '    apt = "от ЦРА " & str.Substring(str.IndexOf("№"))
            '                '    '    findValue = strCompare
            '                '    'End If
            '                'ElseIf flag AndAlso item.Contains("№") Then
            '                '    Do While i < 2
            '                '        If item.ToLower.Contains(arrayToComare(i)) Then
            '                '            Dim str = item.Substring(item.ToLower.IndexOf(arrayToComare(i))).Replace("""", "")
            '                '            apt = str.Substring(str.IndexOf("№"))
            '                '            findValue = arrayToComare(i)
            '                '            GoTo LoopEnd
            '                '        End If
            '                '        i += 1
            '                '    Loop

            '                '    If item.ToLower.Contains(arrayToComare(i)) Then
            '                '        Dim strCompare = arrayToComare(i)

            '                '        i += 1
            '                '        If item.ToLower.Contains(arrayToComare(i)) Then
            '                '            strCompare = arrayToComare(i)
            '                '        End If

            '                '        Dim str = item.Substring(item.ToLower.IndexOf(strCompare)).Replace("""", "")
            '                '        apt = "от ЦРА " & str.Substring(str.IndexOf("№"))
            '                '        findValue = strCompare
            '                '    End If
            '                'End If
            '            Next

            'LoopEnd:    For Each compare In arrayToComare
            '                If subjectPost.ToLower.Contains(compare) Then
            '                    Return apt
            '                End If
            '            Next
            '            i = 0
            '            For Each compare In arrayToComare
            '                If findValue.ToLower.Contains(compare) Then
            '                    apt = ", " & compare & $"{arrayEnding(i)} " & apt
            '                    Return apt
            '                End If
            '                i += 1
            '            Next

            'Return apt
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
