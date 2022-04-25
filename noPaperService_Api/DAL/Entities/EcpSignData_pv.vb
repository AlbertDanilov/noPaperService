
Public Class EcpSignData_pv

    Public Property pv_id As Long
    Public Property pv_nom As Integer?
    Public Property pv_num As String
    Public Property pv_create_date As Date?
    Public Property pv_date As Date?
    Public Property pv_otr_date As Date?
    Public Property pv_agent_id As Integer?
    Public Property pv_agent_agnabbr As String
    Public Property pv_sklad_id As Integer?
    Public Property pv_sklad_name As String
    Public Property pv_otg_date As Date?
    Public Property pv_plat_id As Integer?
    Public Property pv_plat_agnabbr As String
    Public Property pv_opl_type_id As Short?
    Public Property pv_opl_type As String
    Public Property pv_otr_user_id As Integer?
    Public Property pv_zayav_id As Integer?
    Public Property pv_dlo_zayav_id As Integer?
    Public Property pv_doing_date As Date?
    Public Property pv_is_mark As Byte?
    Public Property pvsList As List(Of EcpSignData_pvs)
End Class