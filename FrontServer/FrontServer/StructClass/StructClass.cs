using EBMTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontServer.StructClass
{
    public class PrintData
    {
        /// <summary>
        /// 数据源  表明数据来源  1表示来自系统  2表示来自MQ  3表示主动发出tcp数据   4表示接收的tcp数据
        /// </summary>
        public string source { get; set; }

        public string MessageInfo { get; set; }

    }


    public class IndexItemIData
    {
       /// <summary>
       /// 播放消息ID
       /// </summary>
        public string IndexItemID { get; set; }
        /// <summary>
        /// ebmID
        /// </summary>
        public string ebm_id { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_code_List { get; set; }

    }

    #region 内建类  配置表

    public class TimeService_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureTimeServiceTag; } }
        public EBConfigureTimeService Configure { get; set; }
        //public string TimeSer
        //{
        //    get { return Configure.Real_time.ToString(); }
        //    set { Configure.Real_time = Convert.ToDateTime(value); }
        //}
        public bool GetSystemTime { get; set; }
        private int sendTick = 60;
        public int SendTick
        {
            get { return sendTick; }
            set { sendTick = value; }
        }

    }

    public class SetAddress_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureSetAddressTag; } }
        public EBConfigureSetAddress Configure { get; set; }
        //public string S_Logic_address
        //{
        //    get { return Configure.S_Logic_address; }
        //    set { Configure.S_Logic_address = value; }
        //}
        //public string S_Phisical_address
        //{
        //    get { return Configure.S_Phisical_address; }
        //    set { Configure.S_Phisical_address = value; }
        //}

    }

    public class WorkMode_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureWorkModeTag; } }
        public EBConfigureWorkMode Configure { get; set; }
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        //public byte B_Terminal_wordmode
        //{
        //    get { return Configure.B_Terminal_wordmode; }
        //    set { Configure.B_Terminal_wordmode = value; }
        //}

    }

    public class MainFrequency_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureMainFrequencyTag; } }
        public EBConfigureMainFrequency Configure { get; set; }
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        //public int Freq
        //{
        //    get { return Configure.Freq; }
        //    set { Configure.Freq = value; }
        //}
        //public short QAM
        //{
        //    get { return Configure.QAM; }
        //    set { Configure.QAM = value; }
        //}
        //public int SymbolRate
        //{
        //    get { return Configure.SymbolRate; }
        //    set { Configure.SymbolRate = value; }
        //}

    }

    /// <summary>
    /// 广西版的回传参数设置
    /// </summary>
    public class Reback_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureRebackTag; } }
        public EBConfigureRebackGX Configure { get; set; }
    }

    /// <summary>
    /// 国标版的回传参数设置
    /// </summary>
    public class Reback_Nation : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureRebackTag; } }
        public EBConfigureRebackGX Configure { get; set; }
    }

    public class Reback_Nation_add : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureRebackTag; } }
        public EBConfigureRebackGX Configure { get; set; }

        public List<string> query_code_List { get; set; }
    }

    public class DefaltVolume_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureDefaltVolumeTag; } }
        public EBConigureDefaltVolume Configure { get; set; }
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        //public short Column
        //{
        //    get { return Configure.Column; }
        //    set { Configure.Column = value; }
        //}

    }

    public class RebackPeriod_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureRebackPeriodTag; } }

        public EBConfigureRebackPeriod Configure { get; set; }
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        //public int reback_period
        //{
        //    get { return Configure.reback_period; }
        //    set { Configure.reback_period = value; }
        //}

    }

    public class ContentMoniterRetback_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureContentMoniterRetbackTag; } }
        public EBConfigureContentMoniterRetbackGX Configure { get; set; }


        //public int Start_package_index
        //{
        //    get { return Configure.Start_package_index; }
        //    set { Configure.Start_package_index = value; }
        //}
        //public string S_Reback_serverIP
        //{
        //    get { return Configure.S_Audio_reback_serverip; }
        //    set { Configure.S_Audio_reback_serverip = value; }
        //}

        //public int I_Reback_PORT
        //{
        //    get { return Configure.I_Audio_reback_port; }
        //    set { Configure.I_Audio_reback_port = value; }
        //}

        //public string S_File_id
        //{
        //    get { return Configure.S_File_id; }
        //    set { Configure.S_File_id = value; }
        //}
        ////public byte B_AudioRetback_mode
        ////{
        ////    get { return Configure.B_Audio_reback_mod; }
        ////    set { Configure.B_Audio_reback_mod = value; }
        ////}

        //public int B_AudioRetback_mode
        //{
        //    get { return (int)Configure.B_Audio_reback_mod; }
        //    set { Configure.B_Audio_reback_mod = (byte)value; }
        //}
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}

    }

    public class ContentRealMoniter_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureContentRealMoniterTag; } }
        public EBConfigureContentRealMoniter Configure { get; set; }
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        //public string S_EBM_id
        //{
        //    get { return Configure.S_EBM_id; }
        //    set { Configure.S_EBM_id = value; }
        //}
        //public string S_Server_addr
        //{
        //    get { return Configure.S_Server_addr; }
        //    set { Configure.S_Server_addr = value; }
        //}
        //public short Retback_mode
        //{
        //    get { return Configure.Retback_mode; }
        //    set { Configure.Retback_mode = value; }
        //}
        //public int Moniter_time_duration
        //{
        //    get { return Configure.Moniter_time_duration; }
        //    set { Configure.Moniter_time_duration = value; }
        //}

    }


    public class ContentRealMoniterGX_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureContentRealMoniterTag; } }
        public EBConfigureContentRealMoniterGX Configure { get; set; }
    }

    public class StatusRetback_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.ConfigureStatusRetbackTag; } }
        public EBConfigureStatusRetback Configure { get; set; }
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}

    }

    public class SoftwareUpGrade_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag
        {
            get
            {
                return Utils.ComboBoxHelper.ConfigureSoftwareUpGradeTag;
            }
        }
        public EBConfigureSoftwareUpGrade Configure { get; set; }
        //public byte B_CarrMode
        //{
        //    get { return Configure.B_CarrMode; }
        //    set { Configure.B_CarrMode = value; }
        //}
        //public byte B_FHMode
        //{
        //    get { return Configure.B_FHMode; }
        //    set { Configure.B_FHMode = value; }
        //}
        //public byte B_ILMode
        //{
        //    get { return Configure.B_ILMode; }
        //    set { Configure.B_ILMode = value; }
        //}
        //public byte B_Mode
        //{
        //    get { return Configure.B_Mode; }
        //    set { Configure.B_Mode = value; }
        //}
        //public byte B_ModType
        //{
        //    get { return Configure.B_ModType; }
        //    set { Configure.B_ModType = value; }
        //}
        //public int B_Pid
        //{
        //    get { return Configure.B_Pid; }
        //    set { Configure.B_Pid = value; }
        //}
        //public int I_DeviceType
        //{
        //    get { return Configure.I_DeviceType; }
        //    set { Configure.I_DeviceType = value; }
        //}
        //public int I_Freq
        //{
        //    get { return Configure.I_Freq; }
        //    set { Configure.I_Freq = value; }
        //}
        //public int I_Rate
        //{
        //    get { return Configure.I_Rate; }
        //    set { Configure.I_Rate = value; }
        //}
        //public string S_NewVersion
        //{
        //    get { return Configure.S_NewVersion; }
        //    set { Configure.S_NewVersion = value; }
        //}
        //public string S_OldVersion
        //{
        //    get { return Configure.S_OldVersion; }
        //    set { Configure.S_OldVersion = value; }
        //}
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        public Enums.DeviceOrderType DeviceOrderType { get; set; }

    }

    public class RdsConfig_ : Configure
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag
        {
            get
            {
                return Utils.ComboBoxHelper.ConfigureRdsConfigTag;
            }
        }
        public EBConfigureRdsConfig Configure { get; set; }
        //public byte B_Rds_terminal_type
        //{
        //    get { return Configure.B_Rds_terminal_type; }
        //    set { Configure.B_Rds_terminal_type = value; }
        //}
        //public byte B_Address_type
        //{
        //    get { return Configure.B_Address_type; }
        //    set { Configure.B_Address_type = value; }
        //}
        public string RdsDataText
        {
            get { return Utils.ArrayHelper.Bytes2String(Configure.Br_Rds_data); }
            set
            {
                var bytes = Utils.ArrayHelper.String2Bytes(value);
                if (bytes == null)
                {

                }
                else
                {
                    Configure.Br_Rds_data = bytes;
                }
            }
        }
    }

    public abstract class Configure
    {
        public abstract byte B_Daily_cmd_tag { get; }
        // public bool SendState { get; set; }
    }

    [Serializable]
    public class OutSwitch_ : DailyProgram
    {
        public string ItemID { get; set; }
        public override byte B_Daily_cmd_tag { get { return Utils.ComboBoxHelper.OutSwitchTag; } }
        public DailyCmdOutSwitch Program { get; set; }

    }

    [Serializable]
    public abstract class DailyProgram
    {
        // public abstract string Summary { get; }
        public abstract byte B_Daily_cmd_tag { get; }
        //  public bool SendState { get; set; }
    }
    #endregion
}
