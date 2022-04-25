Namespace Entities
    Public Class FilePathComponent
        Public Property name As String
        Public Property etype As EtpDocTypes
        Public Property eid As Int64
        Public Property fid As Int64
        Public Property pid As Int64 = 0
        Public Property pid2 As Int64 = 0
        Public Property ref As String = Nothing
    End Class

    Public Enum EtpDocTypes
        Zakaz = 1
        ZakazZayav = 3
        ContractCntr = 9
        Protocol = 14
        ContractEtp = 16
        VschetVozvrat = 18
    End Enum
End Namespace