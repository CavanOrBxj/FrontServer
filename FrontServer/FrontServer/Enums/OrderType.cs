using System.ComponentModel;

namespace FrontServer.Enums
{
    public enum OrderType
    {
        [Description("TS方案中的特殊指令")]
        TSSpi = 0x01,

        [Description("RDS方案中的特殊指令")]
        RDDSpi = 0x02,

        [Description("IP方案中的特殊指令")]
        IPSpi = 0x03,

        [Description("开关机指令")]
        OnorOFFBroadcast = 0x04,

        [Description("通用更新区域码指令")]
        UpdateArea = 0x05,

        [Description("通用音量设置指令")]
        VolumeSet = 0x06,

        [Description("通用回传参数设置指令")]
        GRPSI = 0x07,

        [Description("通用被动回传查询指令")]
        GPRQI = 0x08,

        [Description("通用时钟校准指令")]
        UCCI = 0x09,

        [Description("通用终端IP设置指令")]
        GTIPSI = 0x0A,
        
        [Description("通用回传周期设置")]
        GRCS = 0x0B,

        [Description("白名单更新")]
        WIU = 0x0C,

        [Description("回传参数设置")]
        RPS = 0x0D,

        [Description("输出通道查询")]
        OCQ = 0x0E,

        [Description("输入通道查询")]
        ICQ = 0x0F,

        [Description("播发记录查询")]
        BRQ = 0x10,

        [Description("故障详情查询")]
        FDE = 0x11,

        [Description("通用回复")]
        GR = 0x12,

        [Description("查询输出通道回复")]
        QOCR = 0x13,

        [Description("查询输入通道回复")]
        QICR = 0x14,

        [Description("查询播发记录回复")]
        QBRR = 0x15,

        [Description("查询故障详情回复")]
        QFDR = 0x16,

        [Description("开停播请求回复")]
        OnOrOffReply = 0x17,

        [Description("任务开始上报")]
        TaskBeg = 0x18,


        [Description("任务结束上报")]
        TaskOverReply = 0x19,

        [Description("心跳")]
        HeartBeat = 0x20,

        [Description("通用回复-平台回复至大喇叭适配器")]
        Greponse = 0x21,

        [Description("任务开始上报回复")]
        TaskBegReply = 0x22,

        [Description("终端开关指令")]
        TerminalSwitch = 0x3F,

        [Description("通用证书更新指令")]
        CerUpdate = 0x40,
    }
}
