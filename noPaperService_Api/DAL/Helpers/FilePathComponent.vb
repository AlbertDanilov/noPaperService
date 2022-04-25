Namespace Helpers
    Public Class FilePathComponent
        Inherits Entities.FilePathComponent
        Public Shared Function [New](ByVal name As String,
                                     ByVal etype As Integer,
                                     ByVal eid As Int64,
                                     ByVal fid As Int64,
                                     Optional ByVal pid As Int64 = 0,
                                     Optional ByVal pid2 As Int64 = 0,
                                     Optional ByVal ref As String = Nothing) As Entities.FilePathComponent
            Dim FPC As New Entities.FilePathComponent
            FPC.name = name
            FPC.etype = etype
            FPC.eid = eid
            FPC.fid = fid
            FPC.pid = pid
            FPC.pid2 = pid2
            FPC.ref = ref

            Return FPC
        End Function

    End Class
End Namespace
