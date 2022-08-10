Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Security.Cryptography.Pkcs
Imports Gma.QrCodeNet.Encoding
Imports Gma.QrCodeNet.Encoding.Windows.Render
Imports noPaperService_Api.Models

Namespace CreateStamps
    Public Class CreateStamps
        'Public Shared Function GetSigners(ByVal sign As Byte()) As List(Of SignComponent)
        '    ' Объект, в котором будут происходить декодирование и проверка.
        '    Dim signedCms As SignedCms = New SignedCms()
        '    ' Декодируем сообщение.
        '    signedCms.Decode(sign)
        '    Dim signComponentList As New List(Of SignComponent)
        '    For Each signer As SignerInfo In signedCms.SignerInfos
        '        Dim signingTime As Pkcs9SigningTime = Nothing
        '        For Each s In signer.SignedAttributes
        '            If TypeOf s.Values(0) Is Pkcs9SigningTime Then
        '                signingTime = s.Values(0)
        '                Exit For
        '            End If
        '        Next
        '        signComponentList.Add(New SignComponent() With {.SignDateTimeUtc = signingTime.SigningTime, .SignCer = New CertComponent(signer.Certificate)})
        '    Next

        '    signComponentList.Sort(Function(v1, v2) v1.SignDateTimeUtc.CompareTo(v2.SignDateTimeUtc))
        '    Return signComponentList
        'End Function

        Public Shared Function GetStamps(sign As Byte(), signIden As String, Optional ByRef stampList As List(Of Bitmap) = Nothing, Optional ByVal i As Integer = 1, Optional pvOtrDate As String = Nothing) As List(Of Bitmap)
            If stampList Is Nothing Then
                stampList = New List(Of Bitmap)
            End If
            For Each signComponent As SignComponent In GetSigners(sign)
                stampList.Add(GetStamp(signComponent, signIden & i.ToString, pvOtrDate))
                i += 1
            Next
            Return stampList
        End Function

        Public Shared Function _GetStamps(printExcel As PrintExcel, Optional ByRef stampList As List(Of Bitmap) = Nothing) As List(Of Bitmap)
            If stampList Is Nothing Then
                stampList = New List(Of Bitmap)
            End If
            stampList.Add(_GetStamp(printExcel))
            Return stampList
        End Function

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
                signComponentList.Add(Helpers.SignComponent.[New](signingTime.SigningTime, Helpers.CertComponent.[New](signer.Certificate)))
            Next

            signComponentList.Sort(Function(v1, v2) v1.SignDateTimeUtc.CompareTo(v2.SignDateTimeUtc))
            Return signComponentList
        End Function

        'Получение штампа
        Public Shared Function GetStamp(ByVal signComponent As Models.SignComponent, ByVal qrText As String, pvOtrDate As String) As Image
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
                g.DrawString(If(pvOtrDate Is Nothing, signComponent.SignDateTimeUtc.ToLocalTime.ToString("yyyy.MM.dd HH:mm"), pvOtrDate), New Font("Arial", 80), myBrush, New Rectangle(1000, myBMPHeight - 250, 2450, 150))
                'g.DrawString(signComponent.SignDateTimeUtc.ToLocalTime.ToString("yyyy.MM.dd HH:mm"), New Font("Arial", 80), myBrush, New Rectangle(1000, myBMPHeight - 250, 2450, 150))
            End Using
            'Возвращаем холст
            Return myBMP
            Return Nothing
        End Function

        'Получение штампа
        Public Shared Function _GetStamp(printExcel As PrintExcel) As Image
            'Кисть
            Dim myBrush As System.Drawing.Brush = New SolidBrush(Drawing.Color.FromArgb(43, 87, 154))
            'Высота холста
            Dim myBMPHeight As Integer = 1500
            'Создаем холст для рисования
            Dim myBMP As New Bitmap(3500, myBMPHeight, Imaging.PixelFormat.Format32bppArgb)

            Dim sf As New StringFormat With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }

            Using g As Graphics = Graphics.FromImage(myBMP)
                'Рисуем рамку штампа
                g.DrawRectangle(New Pen(myBrush, 80), New Rectangle(0, 0, 3500, myBMPHeight))

                'Настройки прорисовки текста
                g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                'Рисуем заголовку
                Dim emSize As Integer = 90
                Dim str As String = "ШТАМП ПРИЕМКИ"
                Dim font = New Font("Arial", emSize, FontStyle.Bold)
                Dim newX As Decimal = 150.0
                Dim newY As Decimal = 100.0
                Dim newWidth As Decimal = 3300.0
                Dim sizeF As SizeF = g.MeasureString(str, font)
                g.DrawString(str, font, myBrush, New Rectangle(newX, newY, newWidth, sizeF.Height), sf)

                str = "ГУП «Таттехмедфарм»"
                font = New Font("Arial", emSize - 10, FontStyle.Underline)
                newY += sizeF.Height
                sizeF = g.MeasureString(str, font)
                g.DrawString(str, font, myBrush, New Rectangle(newX, newY, newWidth, sizeF.Height), sf)

                str = "(наименование юридического лица)"
                font = New Font("Arial", emSize - 40, FontStyle.Regular)
                newY += sizeF.Height
                sizeF = g.MeasureString(str, font)
                g.DrawString(str, font, myBrush, New Rectangle(newX, newY, newWidth, sizeF.Height), sf)

                str = printExcel.pvAgentPrintname
                font = New Font("Arial", emSize - 10, FontStyle.Underline)
                newY += sizeF.Height
                sizeF = g.MeasureString(str, font)
                Dim heightLev As Double = Math.Ceiling(sizeF.Width / newWidth)
                sizeF.Height *= heightLev
                g.DrawString(str, font, myBrush, New Rectangle(newX, newY, newWidth, sizeF.Height), sf)

                str = "(номер аптечной организации, адрес)"
                font = New Font("Arial", emSize - 40, FontStyle.Regular)
                newY += sizeF.Height
                sizeF = g.MeasureString(str, font)
                g.DrawString(str, font, myBrush, New Rectangle(newX, newY, newWidth, sizeF.Height), sf)

                str = "Принятый товар соответствует данным, указанным в сопроводительных документах"
                font = New Font("Arial", emSize, FontStyle.Regular)
                sizeF = g.MeasureString(str, font)
                heightLev = Math.Ceiling(sizeF.Width / newWidth)
                'heightLev = Math.Ceiling(sizeF.Width / 2700.0)
                'sizeF.Height *= heightLev
                newY += sizeF.Height
                sizeF.Height *= heightLev
                g.DrawString(str, font, myBrush, New Rectangle(100, newY, newWidth, sizeF.Height), sf) '1150
            End Using
            'Возвращаем холст
            Return myBMP
            Return Nothing
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

        Public Shared Function ImageToBytes(ByRef image As Bitmap) As Byte()
            Dim imageByte As Byte()
            Using stream = New MemoryStream()
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png)
                imageByte = stream.ToArray()
            End Using
            Return imageByte
        End Function

        Public Shared Function GetDataStamp(ByVal data As String) As System.Drawing.Image
            Return GetDataStamp(data, 80)
        End Function
        Private Shared Function GetDataStamp(ByVal data As String, fontSize As Integer) As System.Drawing.Image
            'Кисть
            Dim myBrush As System.Drawing.Brush = New SolidBrush(System.Drawing.Color.FromArgb(43, 87, 154))
            'Высота холста
            Dim myBMPHeight As Integer = 160
            'Ширина холста
            Dim myBMPWidth As Integer = 620
            'Создаем холст для рисования
            Dim myBMP As New Bitmap(myBMPWidth, myBMPHeight, Imaging.PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(myBMP)
                'Рисуем рамку штампа
                g.DrawRectangle(New Pen(myBrush, 30), New System.Drawing.Rectangle(0, 0, myBMPWidth, myBMPHeight))
                'Настройки прорисовки текста
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                'Рисуем дату подписи
                g.DrawString(data, New System.Drawing.Font("Arial", fontSize, FontStyle.Bold), myBrush, New System.Drawing.Rectangle(20, 20, myBMPWidth, myBMPHeight))
            End Using
            'Возвращаем холст
            Return myBMP
        End Function

        Public Shared Function GetStringStamp(ByVal data As String, fontSize As Integer) As System.Drawing.Image
            'Кисть
            Dim myBrush As System.Drawing.Brush = New SolidBrush(System.Drawing.Color.FromArgb(43, 87, 154))
            'Высота холста
            Dim myBMPHeight As Integer = 80
            'Ширина холста
            Dim myBMPWidth As Integer = 620
            'Создаем холст для рисования
            Dim myBMP As New Bitmap(myBMPWidth, myBMPHeight, Imaging.PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(myBMP)
                'Рисуем рамку штампа
                g.DrawRectangle(New Pen(myBrush, 15), New System.Drawing.Rectangle(0, 0, myBMPWidth, myBMPHeight))
                'Настройки прорисовки текста
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault
                g.SmoothingMode = Drawing2D.SmoothingMode.Default
                'Рисуем текст
                g.DrawString(data, New System.Drawing.Font("Calibri", fontSize, FontStyle.Bold), myBrush, New System.Drawing.Rectangle(20, 20, myBMPWidth, myBMPHeight))
            End Using
            'Возвращаем холст
            Return myBMP
        End Function
    End Class
End Namespace
