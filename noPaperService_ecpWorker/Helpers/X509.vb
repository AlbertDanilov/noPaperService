Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography.Pkcs
Imports System.Text
Imports System.IO
Imports System
Imports System.Drawing
Imports Gma.QrCodeNet.Encoding
Imports Gma.QrCodeNet.Encoding.Windows.Render
Imports noPaperService_common.Entities
Imports System.Drawing.Imaging

Public Class X509
    Public Shared Function selectSingleCertificate() As ReturnData
        Try
            Dim store As New X509Store("My", StoreLocation.CurrentUser)
            store.Open(OpenFlags.OpenExistingOnly Or OpenFlags.ReadWrite)
            If store.Certificates.Count = 0 Then
                store.Close()
                store = New X509Store("My", StoreLocation.LocalMachine)
                store.Open(OpenFlags.OpenExistingOnly Or OpenFlags.ReadWrite)
            End If
            Dim fcoll As X509Certificate2Collection = store.Certificates
            'Dim fcollection As X509Certificate2Collection = CType(store.Certificates.Find(X509FindType.FindByKeyUsage,
            '                                                                          X509KeyUsageFlags.DataEncipherment.ToString(),
            '                                                                          True), X509Certificate2Collection)
            Dim ccoll As X509Certificate2Collection = X509Certificate2UI.SelectFromCollection(fcoll,
                                                                                          "Выберите сертификат",
                                                                                          "Выберите сертификат для подписи.",
                                                                                          X509SelectionFlag.SingleSelection)
            If ccoll.Count > 0 Then
                Return New ReturnData(True, ccoll(0), Nothing)
            Else
                Return New ReturnData(False, Nothing, "Сертификат не выбран")
            End If
        Catch ex As Exception
            Return New ReturnData(False, Nothing, ex.Message)
        End Try
    End Function
    Public Shared Function selectSingleCertificate(ByVal thumbprint As String) As ReturnData
        Try
            Dim store As New X509Store("My", StoreLocation.CurrentUser)
            store.Open(OpenFlags.OpenExistingOnly Or OpenFlags.ReadWrite)

            Dim fcollection As X509Certificate2Collection = CType(store.Certificates.Find(X509FindType.FindByThumbprint,
                                                                                          thumbprint,
                                                                                          True), X509Certificate2Collection)

            If fcollection.Count = 0 Then
                store.Close()
                store = New X509Store("My", StoreLocation.LocalMachine)
                store.Open(OpenFlags.OpenExistingOnly Or OpenFlags.ReadWrite)

                fcollection = CType(store.Certificates.Find(X509FindType.FindByThumbprint,
                                                            thumbprint,
                                                            True), X509Certificate2Collection)
            End If

            If fcollection.Count > 0 Then
                Return New ReturnData(True, fcollection.Item(0), Nothing)
            Else
                Return New ReturnData(False, Nothing, "Сертификат не выбран")
            End If
        Catch ex As Exception
            Return New ReturnData(False, Nothing, ex.Message)
        End Try
    End Function

    Public Shared Function selectSingleCertificateByFIO(ByVal FIO As String) As ReturnData
        Try
            Dim store As New X509Store("My", StoreLocation.CurrentUser)
            store.Open(OpenFlags.OpenExistingOnly Or OpenFlags.ReadWrite)

            Dim fcollection As X509Certificate2Collection = CType(store.Certificates, X509Certificate2Collection)
            Dim rez_fcollection As X509Certificate2Collection = New X509Certificate2Collection

            Dim fio_arr As List(Of String) = FIO.Trim.Split(" ").ToList()

            If fcollection.Count > 0 AndAlso fio_arr IsNot Nothing AndAlso fio_arr.Count > 0 Then
                For Each item In fcollection
                    Try
                        If item.NotAfter >= DateTime.Now Then
                            Dim k As Integer = 0

                            For Each fio_item In fio_arr
                                If item.SubjectName.Name.Contains(fio_item) Then
                                    k += 1
                                End If
                            Next

                            If k >= 3 Then
                                rez_fcollection.Add(item)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                Next
            End If

            If rez_fcollection.Count = 0 Then
                store.Close()
                store = New X509Store("My", StoreLocation.LocalMachine)
                store.Open(OpenFlags.OpenExistingOnly Or OpenFlags.ReadWrite)

                fcollection = CType(store.Certificates, X509Certificate2Collection)

                If fcollection.Count > 0 AndAlso fio_arr IsNot Nothing AndAlso fio_arr.Count > 0 Then
                    For Each item In fcollection
                        Try
                            If item.NotAfter >= DateTime.Now Then
                                Dim k As Integer = 0

                                For Each fio_item In fio_arr
                                    If item.SubjectName.Name.Contains(fio_item) Then
                                        k += 1
                                    End If
                                Next

                                If k >= 3 Then
                                    rez_fcollection.Add(item)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                End If
            End If

            If rez_fcollection.Count > 0 Then
                Return New ReturnData(True, rez_fcollection.Item(0), Nothing)
            Else
                Return New ReturnData(False, Nothing, "Сертификат не выбран")
            End If
        Catch ex As Exception
            Return New ReturnData(False, Nothing, ex.Message)
        End Try
    End Function

    Public Class PKCS_7
        Public Shared Function GetSigners(ByVal sign As Byte()) As List(Of SignComponent)
            ' Объект, в котором будут происходить декодирование и проверка.
            Dim signedCms As SignedCms = New SignedCms()
            ' Декодируем сообщение.
            signedCms.Decode(sign)
            Dim signComponentList As New List(Of SignComponent)
            For Each signer As SignerInfo In signedCms.SignerInfos
                Dim signingTime As Pkcs9SigningTime = Nothing
                For Each s In signer.SignedAttributes
                    If TypeOf s.Values(0) Is Pkcs9SigningTime Then
                        signingTime = s.Values(0)
                        Exit For
                    End If
                Next
                signComponentList.Add(New SignComponent() With {.SignDateTimeUtc = signingTime.SigningTime, .SignCer = New CertComponent(signer.Certificate)})
            Next

            signComponentList.Sort(Function(v1, v2) v1.SignDateTimeUtc.CompareTo(v2.SignDateTimeUtc))
            Return signComponentList
        End Function
        Public Shared Function GetSignersCertificatesInfos(ByVal sign As Byte()) As List(Of CertComponent)
            ' Объект, в котором будут происходить декодирование и проверка.
            Dim signedCms As SignedCms = New SignedCms()
            ' Декодируем сообщение.
            signedCms.Decode(sign)
            Dim certificatesInfos As New List(Of CertComponent)
            For Each signer As SignerInfo In signedCms.SignerInfos
                certificatesInfos.Add(New CertComponent(signer.Certificate))
                For Each s In signer.SignedAttributes
                    If TypeOf s.Values(0) Is Pkcs9SigningTime Then
                        Dim signingTime As Pkcs9SigningTime = s.Values(0)
                    End If
                Next
            Next
            Return certificatesInfos
        End Function
        Public Class Stamp
            Public Shared Function GetStamps(ByVal sign As Byte(), ByVal signIden As String, Optional ByRef stampList As List(Of Bitmap) = Nothing, Optional ByVal i As Integer = 1) As List(Of Bitmap)
                If stampList Is Nothing Then
                    stampList = New List(Of Bitmap)
                End If
                For Each signComponent As SignComponent In GetSigners(sign)
                    stampList.Add(GetStamp(signComponent, signIden & i.ToString))
                    i += 1
                Next
                Return stampList
            End Function
            'Получение штампа
            Private Shared Function GetStamp(ByVal signComponent As SignComponent, ByVal qrText As String) As Image
                'Кисть
                Dim myBrush As System.Drawing.Brush = New SolidBrush(Drawing.Color.FromArgb(43, 87, 154))
                'Высота холста
                Dim myBMPHeight As Integer = 1500
                'Меняем высоту если информация о владельце превышает ширину
                If signComponent.SignCer.subject.Length > 46 Then
                    myBMPHeight = 1600
                End If
                'Создаем холст для рисования
                Dim myBMP As New Bitmap(3500, myBMPHeight, Imaging.PixelFormat.Format32bppArgb)
                'Генерим QrCode
                Dim newImage As Bitmap = GetQrCode(qrText, 500, myBrush)
                Using g As Graphics = Graphics.FromImage(myBMP)
                    'Рисуем рамку штампа
                    g.DrawRectangle(New Pen(myBrush, 80), New Rectangle(0, 0, 3500, myBMPHeight))
                    'Рисуем полученный QrCode
                    g.DrawImage(newImage, 100, 100, 650, 650)
                    'Настройки прорисовки текста
                    g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
                    g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    'Рисуем заголовку
                    g.DrawString("ДОКУМЕНТ ПОДПИСАН", New Font("Arial", 90, FontStyle.Bold), myBrush, New Rectangle(1350, 200, 2200, 150))
                    g.DrawString("УСИЛЕННОЙ КВАЛИФИЦИРОВАННОЙ", New Font("Arial", 90, FontStyle.Bold), myBrush, New Rectangle(900, 350, 2700, 150))
                    g.DrawString("ЭЛЕКТРОННОЙ ПОДПИСЬЮ", New Font("Arial", 90, FontStyle.Bold), myBrush, New Rectangle(1230, 500, 2300, 150))
                    'Рисуем информацию о сертификате
                    g.DrawString("Сертификат:", New Font("Arial", 80, FontStyle.Bold), myBrush, New Rectangle(100, 800, 800, 150))
                    g.DrawString(signComponent.SignCer.serialNumber, New Font("Arial", 80), myBrush, New Rectangle(850, 800, 2800, 150))
                    'Рисуем информацию о владельце
                    g.DrawString("Владелец:", New Font("Arial", 80, FontStyle.Bold), myBrush, New Rectangle(100, 950, 700, 150))
                    g.DrawString(signComponent.SignCer.subject, New Font("Arial", 80), myBrush, New Rectangle(750, 950, 2700, 250))
                    'Рисуем информацию о действие
                    g.DrawString("Действителен:", New Font("Arial", 80, FontStyle.Bold), myBrush, New Rectangle(100, myBMPHeight - 400, 950, 150))
                    g.DrawString(signComponent.SignCer.valid, New Font("Arial", 80), myBrush, New Rectangle(1000, myBMPHeight - 400, 2450, 150))
                    'Рисуем дату подписи
                    g.DrawString("Дата подписи:", New Font("Arial", 80, FontStyle.Bold), myBrush, New Rectangle(100, myBMPHeight - 250, 950, 150))
                    g.DrawString(signComponent.SignDateTimeUtc.ToLocalTime.ToString("yyyy.MM.dd HH:mm"), New Font("Arial", 80), myBrush, New Rectangle(1000, myBMPHeight - 250, 2450, 150))
                End Using
                'Возвращаем холст
                Return myBMP
            End Function
            'Генерим QrCode
            Private Shared Function GetQrCode(ByVal content As String, ByVal size As Integer, ByVal Brush As System.Drawing.Brush) As Image
                Dim qrEncoder As New QrEncoder(ErrorCorrectionLevel.H)
                Dim qrCode As New QrCode()
                qrEncoder.TryEncode(content, qrCode)
                Dim renderer As GraphicsRenderer = New GraphicsRenderer(New FixedCodeSize(size, QuietZoneModules.Zero), Brush, Brushes.Transparent)
                Dim ms As New MemoryStream()
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms)
                Dim imageTemp = New Bitmap(ms)
                Dim image = New Bitmap(imageTemp, New Size(New Point(size, size)))
                Return image
            End Function
        End Class
        Public Class Detached
            ' Подписываем сообщение секретным ключем.
            Public Shared Function Sign(ByVal msg As Byte(),
                                        ByVal signerCert As X509Certificate2) As ReturnData
                Try

                    'MessageBox.Show($"msg.length = {msg.Length}, thumbprint = {signerCert.Thumbprint}")

                    If signerCert Is Nothing Then Return Nothing
                    ' Создаем объект ContentInfo по сообщению.
                    ' Это необходимо для создания объекта SignedCms.
                    Dim contentInfo As New ContentInfo(msg)
                    ' Создаем объект SignedCms по только что созданному
                    ' объекту ContentInfo.
                    ' SubjectIdentifierType установлен по умолчанию в 
                    ' IssuerAndSerialNumber.
                    ' Свойство Detached устанавливаем явно в true, таким 
                    ' образом сообщение будет отделено от подписи.
                    Dim signedCms As New SignedCms(contentInfo, True)
                    ' Определяем подписывающего, объектом CmsSigner.
                    Dim cmsSigner As New CmsSigner(signerCert)
                    ' Время подписи
                    cmsSigner.SignedAttributes.Add(New Pkcs9SigningTime(Date.UtcNow))
                    ' Подписываем CMS/PKCS #7 сообение.

                    signedCms.ComputeSignature(cmsSigner, False)

                    ' Кодируем CMS/PKCS #7 подпись сообщения.
                    Return New ReturnData(True, signedCms.Encode(), Nothing)
                Catch ex As Exception
                    'System.Windows.Forms.MessageBox.Show("Sign Error: " & ex.Message)
                    Return New ReturnData(False, ex.Message, ex.Message)
                End Try
            End Function
            ' Подписываем сообщение секретным ключом.
            Public Shared Function AddSign(ByVal msg As Byte(),
                                           ByVal signature As Byte(),
                                           ByVal signerCert As X509Certificate2) As ReturnData
                Try
                    If signerCert Is Nothing Then Return Nothing
                    ' Создаем объект ContentInfo по сообщению.
                    ' Это необходимо для создания объекта SignedCms.
                    Dim contentInfo As New ContentInfo(msg)
                    ' Создаем объект SignedCms 
                    ' Свойство Detached устанавливаем явно в true, таким 
                    ' образом сообщение будет отделено от подписи.
                    Dim SignedCms As New SignedCms(contentInfo, True)
                    ' Декодируем
                    SignedCms.Decode(signature)
                    ' В этом месте чаще всего стоит проверка 
                    ' предыдущих подписей, для простоты опускаем.
                    'Dim bl As Boolean = VerifyMsg(src, msg)
                    ' Определяем подписывающего, объектом CmsSigner.
                    Dim CmsSigner As New CmsSigner(signerCert)
                    ' Время подписи
                    CmsSigner.SignedAttributes.Add(New Pkcs9SigningTime(Date.UtcNow))
                    ' Подписываем CMS/PKCS #7 сообение.
                    SignedCms.ComputeSignature(CmsSigner, False)
                    ' Кодируем CMS/PKCS #7 сообщение.
                    Return New ReturnData(True, SignedCms.Encode(), Nothing)
                Catch ex As Exception
                    Return New ReturnData(False, Nothing, ex.Message)
                End Try
            End Function
            'Сравниваем алгоритмы подписи
            Public Shared Function CheckSignAlgorithm(ByVal msg As Byte(),
                                                      ByVal signature As Byte(),
                                                      ByVal signerCert As X509Certificate2) As ReturnData
                Try
                    If signerCert Is Nothing Then Return Nothing
                    ' Создаем объект ContentInfo по сообщению.
                    ' Это необходимо для создания объекта SignedCms.
                    Dim contentInfo As New ContentInfo(msg)
                    ' Создаем объект SignedCms 
                    ' Свойство Detached устанавливаем явно в true, таким 
                    ' образом сообщение будет отделено от подписи.
                    Dim SignedCms As New SignedCms(contentInfo, True)
                    ' Декодируем
                    SignedCms.Decode(signature)

                    Dim FirstSignCertificate As SignerInfo = SignedCms.SignerInfos.Item(0)
                    If (FirstSignCertificate.Certificate.SignatureAlgorithm.Value = signerCert.SignatureAlgorithm.Value) Then
                        Return New ReturnData(True, True, Nothing)
                    Else
                        Return New ReturnData(True, False, Nothing)
                    End If
                Catch ex As Exception
                    Return New ReturnData(False, Nothing, ex.Message)
                End Try
            End Function
            ' Проверяем SignedCms сообщение и возвращаем Boolean
            ' значение определяющее результат проверки.
            Public Shared Function Verify(ByVal msg As Byte(),
                                          ByVal signature As Byte()) As Boolean
                ' Создаем объект ContentInfo по сообщению.
                ' Это необходимо для создания объекта SignedCms.
                Dim contentInfo As New ContentInfo(msg)
                ' Создаем SignedCms для декодирования и проверки.
                Dim SignedCms As New SignedCms(contentInfo, True)
                ' Декодируем подпись
                SignedCms.Decode(signature)
                ' Перехватываем криптографические исключения, для 
                ' возврата о false значения при некорректности подписи.
                Try
                    ' Проверяем подпись. В данном примере не 
                    ' проверяется корректность сертификата подписавшего.
                    ' В рабочем коде, скорее всего потребуется построение
                    ' и проверка корректности цепочки сертификата.
                    ' true проверить только подпись, без сертификата
                    SignedCms.CheckSignature(True)
                Catch e As Exception
                    Return False
                End Try
                Return True
            End Function

            Public Shared Function fullVerify(ByVal msg As Byte(),
                                              ByVal signature As Byte()) As Integer
                ' Создаем объект ContentInfo по сообщению.
                ' Это необходимо для создания объекта SignedCms.
                Dim contentInfo As New ContentInfo(msg)
                ' Объект, в котором будут происходить декодирование и проверка.
                ' Свойство Detached устанавливаем явно в true, таким 
                ' образом сообщение будет отделено от подписи.
                Dim signedCms As New SignedCms(contentInfo, True)
                ' Декодируем сообщение.
                signedCms.Decode(signature)
                '  Проверяем число основных и дополнительных подписей.
                If signedCms.SignerInfos.Count = 0 Then Return 0
                Dim valid As Boolean = True
                Dim enumerator As SignerInfoEnumerator = signedCms.SignerInfos.GetEnumerator()
                While enumerator.MoveNext()
                    Dim current As SignerInfo = enumerator.Current
                    Try
                        ' Используем проверку подписи и стандартную 
                        ' процедуру проверки сертификата: построение цепочки, 
                        ' проверку цепочки, и необходимых расширений для данного 
                        ' сертификата.
                        current.CheckSignature(False)
                    Catch e As System.Security.Cryptography.CryptographicException
                        valid = False
                        Throw New ArgumentException(e.Message)
                    End Try
                    ' При наличии соподписей проверяем соподписи.
                    If current.CounterSignerInfos.Count > 0 Then
                        Dim coenumerator As SignerInfoEnumerator = current.CounterSignerInfos.GetEnumerator()
                        While coenumerator.MoveNext()
                            Dim cosigner As SignerInfo = coenumerator.Current
                            Try
                                ' Используем проверку подписи и стандартную 
                                ' процедуру проверки сертификата: построение цепочки, 
                                ' проверку цепочки, и необходимых расширений для данного 
                                ' сертификата.
                                ' False проверить подпись так и сертификат
                                cosigner.CheckSignature(False)
                            Catch e As System.Security.Cryptography.CryptographicException
                                valid = False
                                Throw New ArgumentException(e.Message)
                            End Try
                        End While
                    End If
                End While

                If valid Then
                    Return 1
                Else
                    Return -1
                End If
            End Function
            Public Shared Function DeleteSign(ByVal signature As Byte(),
                                              ByVal thumbprint As String) As ReturnData
                Try
                    If signature Is Nothing Then
                        Throw New Exception("Подпись пуст")
                    End If

                    Dim SignedCms As New SignedCms()
                    SignedCms.Decode(signature)

                    Dim SignCount As Integer = SignedCms.SignerInfos.Count

                    For i As Integer = SignedCms.SignerInfos.Count - 1 To 0 Step -1
                        If SignedCms.SignerInfos(i).Certificate.Thumbprint = thumbprint Then
                            SignedCms.RemoveSignature(SignedCms.SignerInfos(i))
                        End If
                    Next

                    If SignCount = SignedCms.SignerInfos.Count Then
                        Return New ReturnData(False, Nothing, "Этим подписем не подписывали!")
                    End If

                    If SignedCms.SignerInfos.Count = 0 Then
                        Return New ReturnData(True, Nothing, Nothing)
                    Else
                        Return New ReturnData(True, SignedCms.Encode(), Nothing)
                    End If

                Catch ex As Exception
                    Return New ReturnData(False, Nothing, ex.Message)
                End Try
            End Function
        End Class
        Public Class NotDetached
            ' Подписываем сообщение секретным ключем.
            Public Shared Function signData(ByVal data As Byte(),
                                            ByVal cert As X509Certificate2) As ReturnData

                Try
                    ' Создаем объект ContentInfo по сообщению.
                    ' Это необходимо для создания объекта SignedCms.
                    Dim contentInfo As ContentInfo = New ContentInfo(data)
                    ' Создаем объект SignedCms по только что созданному объекту ContentInfo.
                    ' SubjectIdentifierType установлен по умолчанию в IssuerAndSerialNumber.
                    ' Свойство Detached установлено по умолчанию в false, таким образом сообщение будет включено в SignedCms.
                    Dim signedCms As SignedCms = New SignedCms(SubjectIdentifierType.SubjectKeyIdentifier, contentInfo, False)
                    ' Определяем подписывающего, объектом CmsSigner.
                    Dim cmsSigner As CmsSigner = New CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, cert)
                    ' Подписываем CMS/PKCS #7 сообение.
                    signedCms.ComputeSignature(cmsSigner, False)
                    ' Кодируем CMS/PKCS #7 сообщение.
                    Return New ReturnData(True, signedCms.Encode(), Nothing)
                Catch ex As Exception
                    Return New ReturnData(False, Nothing, ex.Message())
                End Try
            End Function
            ' Подписываем сообщение секретным ключом.
            Public Shared Function addSignature(ByVal data As Byte(),
                                                ByVal cert As X509Certificate2) As ReturnData
                Try
                    ' Создаем объект SignedCms 
                    Dim SignedCms As SignedCms = New SignedCms()
                    ' Декодируем
                    SignedCms.Decode(data)
                    ' В этом месте чаще всего стоит проверка предыдущих подписей, для простоты опускаем.
                    ' Определяем подписывающего, объектом CmsSigner.
                    Dim CmsSigner As CmsSigner = New CmsSigner(cert)
                    ' Подписываем CMS/PKCS #7 сообение.
                    SignedCms.ComputeSignature(CmsSigner, False)
                    ' Кодируем CMS/PKCS #7 сообщение.
                    Return New ReturnData(True, SignedCms.Encode(), Nothing)
                Catch ex As Exception
                    Return New ReturnData(False, Nothing, ex.Message())
                End Try
            End Function
            Public Shared Function verifySignature(ByVal encodedSignedCms As Byte()) As ReturnData

                Dim msg As String = ""

                ' Проверка корректности переданных параметров.
                If encodedSignedCms Is Nothing Then Return New ReturnData(False, Nothing, "На входе Nothing.")
                ' Объект, в котором будут происходить декодирование и проверка.
                Dim signedCms As SignedCms = New SignedCms()
                ' Декодируем сообщение.
                signedCms.Decode(encodedSignedCms)
                ' Проверяем число основных и дополнительных подписей.
                If signedCms.SignerInfos.Count = 0 Then Return New ReturnData(False, Nothing, "Документ не подписан.")

                Dim valid As Boolean = True
                Dim enumerator As SignerInfoEnumerator = signedCms.SignerInfos.GetEnumerator()
                While enumerator.MoveNext()
                    Dim current As SignerInfo = enumerator.Current
                    If Not current.Certificate Is Nothing Then
                        Console.Write("Проверка подписи для подписавшего '{0}'...", current.Certificate.SubjectName.Name)
                    Else
                        Console.Write("Проверка подписи для подписавшего без сертификата...")
                    End If
                    Try
                        ' Используем проверку подписи и стандартную 
                        ' процедуру проверки сертификата: построение цепочки, 
                        ' проверку цепочки, и необходимых расширений для данного 
                        ' сертификата.
                        current.CheckSignature(False)
                        Console.WriteLine("Успешно.")
                    Catch e As System.Security.Cryptography.CryptographicException
                        Console.WriteLine("Ошибка:")
                        Console.WriteLine(ChrW(9) & e.Message)
                        valid = False
                    End Try

                    ' При наличии соподписей проверяем соподписи.
                    If current.CounterSignerInfos.Count > 0 Then
                        Console.WriteLine(ChrW(9) & "Количество соподписей:{0}",
                            current.CounterSignerInfos.Count)
                        Dim coenumerator As SignerInfoEnumerator =
                            current.CounterSignerInfos.GetEnumerator()
                        While coenumerator.MoveNext()
                            Dim cosigner As SignerInfo = coenumerator.Current
                            Console.Write(ChrW(9) & "Проверка соподписи для соподписавшего '{0}'...",
                                cosigner.Certificate.SubjectName.Name)
                            Try
                                ' Используем проверку подписи и стандартную 
                                ' процедуру проверки сертификата: построение цепочки, 
                                ' проверку цепочки, и необходимых расширений для данного 
                                ' сертификата.
                                cosigner.CheckSignature(False)
                                Console.WriteLine("Успешно.")
                            Catch e As System.Security.Cryptography.CryptographicException
                                Console.WriteLine("Ошибка:")
                                Console.WriteLine(ChrW(9) & ChrW(9) & e.Message)
                                valid = False
                            End Try
                        End While
                    End If
                End While

                Console.WriteLine()
                Dim rez As String = ""

                If valid Then
                    rez = "Проверка PKCS #7 сообщения завершилась успешно."
                    Console.WriteLine(rez)
                Else
                    rez = "Проверка PKCS #7 сообщения завершилась неудачно." &
                          "Возможно сообщение, одна из подписей, или соподписей " &
                          "модифицированы в процессе передачи или хранения. " &
                          "Возможно, что не корректен или подменен один из " &
                          "сертификатов подписывающих. Возможно подписывающий " &
                          "не имеет соответствующих атрибутов для подписи. " &
                          "Достоверность и/или целостность сообщения не гарантируется."
                    Console.WriteLine(rez)
                End If

                Return New ReturnData(valid, Nothing, rez)

            End Function
        End Class
    End Class
    Public Class CertWithBase64
        Public Shared Function base64ToCert(ByRef cert64 As String) As X509Certificate2
            'проверка на Nothing
            If (cert64 Is Nothing) Then Return Nothing
            Dim cert As New X509Certificate2()
            Try
                cert.Import(Convert.FromBase64String(cert64))
                Return cert
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Shared Function certToBase64(ByRef cert As X509Certificate) As String
            'проверка на Nothing
            If (cert Is Nothing) Then Return Nothing
            Dim stringBuilder As New StringBuilder()
            Try
                stringBuilder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks))
                Return stringBuilder.ToString()
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
    End Class
End Class
