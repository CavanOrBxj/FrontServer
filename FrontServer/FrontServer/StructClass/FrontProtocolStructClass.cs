using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace FrontServer
{
    public class OnorOFFBroadcast
    {
        public string ebm_id { get; set; }
        public string power_switch { get; set; }
        public string ebm_class { get; set; }

        public string ebm_type { get; set; }

        public string ebm_level { get; set; }

        public string start_time { get; set; }//UTC时间

        public string end_time { get; set; }//UTC时间

        public string volume { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度


        public List<MultilingualContentInfo> multilingual_contentList;//


        public int input_channel_id { get; set; }

        public List<int> OutPut_Channel_IdList;

        //图南私有  PID
        public string S_elementary_PID { get; set; }
    }


    public class MultilingualContentInfo
    {
        public string language_code { get; set; }

        public string coded_character_set { get; set; }

       // public int text_length { get; set; }

        public string text_char { get; set; }

     //   public int agency_name_length { get; set; }

        public string agency_name_char { get; set; }

        public List<AuxiliaryInfo> AuxiliaryInfoList;
    }

    public class AuxiliaryInfo
    {
        public int auxiliary_data_type { get; set; }
       // public int auxiliary_data_length { get; set; }
        public string auxiliary_data { get; set; }

    }


    public class OnorOFFResponse
    {
        public string front_code { get; set; }
        public string ebm_id { get; set; }

        //0表示成功  1表示失败
        public int result_code { get; set; }
        public int result_desc_length { get; set; }

        public string result_desc { get; set; }

        public int accept_stream_address_length { get; set; }

        public string accept_stream_address { get; set; }
    }

    public class PlayRecord_tcp_ts
    {
        public string resource_code { get; set; }

        public string IndexItemID { get; set; }

        public string prAreaName { get; set; }

        public string prEvnType { get; set; }

    }



    public class GeneralResponse
    {
        public string return_code { get; set; }
        public string return_data { get; set; }

    }

    /// <summary>
    /// 功放开关
    /// </summary>
    public class SwitchAmplifier
    {
        public string switch_option { get; set; }
        public string resource_code_type { get; set; }

        public List<string> resource_codeList;

    }


  

   

    public class WhiteListRecord
    {
        public string username { get; set; }

        public string phone_number { get; set; }
        public string Organizations { get; set; }

        public string gb_codes { get; set; }
    }

    #region  任务开始上报
    public class TaskUploadBegin
    {
        public string front_code { get; set; }
        public string ebm_id { get; set; }

        public string program_resource { get; set; }
        public string ebm_class { get; set; }
        public string ebm_level { get; set; }

        public string ebm_type { get; set; }

        public string input_channel_id { get; set; }

        public List<output_channel_struct> utput_channel_structList;
        public string tel_length { get; set; }
        public string tel_number { get; set; }

        public string volume { get; set; }

        public List<string> volumeList;

    }

    public class output_channel_struct
    {
        public string output_channel_type { get; set; }
        public string output_channel_id { get; set; }
    }

    public class TaskUploadOver
    {
        public string front_code { get; set; }
        public string ebm_id { get; set; }
    }


    #endregion


    #region  心跳结构类
    public class RecvHeartBeat
    {
        public string front_code { get; set; }
        public string front_State { get; set; }
    }
  
    #endregion

    public class GeneralRebackParam
    {
        public string reback_type { get; set; }
        public string reback_address { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度

    }

    public class GeneralRebackCycle
    {
        public string reback_cycle { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度

    }

    public class GeneralVolumn
    {
        public string volume { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度

    }

    /// <summary>
    /// 通用网络参数设置
    /// </summary>
    public class GeneralNetworkParam
    {
        public string ip { get; set; }
        public string subnet_mask { get; set; }
        public string gateway { get; set; }
        public string resource_code_type { get; set; }
        public string resource_code { get; set; }

    }

    /// <summary>
    /// 传递参数载体
    /// </summary>
    public class ParamObject
    {
        public byte commandcode { get; set; }

        public object paramobj { get; set; }
    
    }

    


    public class OperatorData
    {
        public string OperatorType { get; set; }

        public string ModuleType { get; set; }
        public object Data { get; set; }

    }

    /// <summary>
    /// 时钟校准
    /// </summary>
    public class ClockCalibration
    {
        public string time { get; set; }

    }


    public class ResourceCodeInfo
    {
        public string physical_address { get; set; }

        public string logic_address { get; set; }

    }


    public class RebackSet
    {
        public string reback_type { get; set; }

        public string reback_address { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度

    }


    public class StatusInquiry
    {
        public string reback_type { get; set; }

        public string reback_address { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度

        public List<string> query_code_codeList;//查询信息类型码  默认所有资源码都是同一长度

    }


    public class RebackPeriod
    {
        public string reback_cycle { get; set; }

        public string resource_code_type { get; set; }

        public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度

    }

    /// <summary>
    /// huican
    /// </summary>
    public class RebackParam
    {
        public string reback_type { get; set; }

        public string reback_cycle { get; set; }

        public string reback_address;

    }

    /// <summary>
    /// 输出通道查询
    /// </summary>
    public class OutChannelInquire
    {
        public string front_code { get; set; }

        /// <summary>
        /// 传输通道号：取 0 时不做为查询条件
        /// </summary>
        public string output_channel_id { get; set; }

        /// <summary>
        /// 根据传输通道状态查询 0：查询全部 1：查询空闲；2：查询占用 3：查询故障
        /// </summary>
        public string output_channel_state;

    }


    /// <summary>
    /// 输出通道查询回复
    /// </summary>
    public class OutChannelResponse
    {
        public string front_code { get; set; }

        public List<OutChannel> OutChannelList;

    }

    /// <summary>
    /// 输出通道信息
    /// </summary>
    public class OutChannel
    {
        public string output_channel_id { get; set; }
        public string out_channel_type { get; set; }

        public int sub_channel_number { get; set; }

        public List<sub_channel_Info_1> sub_channel_Info_1List;

        public List<sub_channel_Info_2or3> sub_channel_Info_2or3List;

    }


    /// <summary>
    /// out_channel_type==1时的sub_channel_Info
    /// </summary>
    public class sub_channel_Info_1
    {
        public string sub_channel_freq { get; set; }
        public string output_channel_state { get; set; }

        public string ebm_id { get; set; }//output_channel_state==2
    }


    /// <summary>
    /// out_channel_type==2或3时的sub_channel_Info
    /// </summary>
    public class sub_channel_Info_2or3
    {
        public string original_network_id { get; set; }
        public string details_channel_transport_stream_id { get; set; }

        public string details_channel_program_number { get; set; }

        public string details_channel_pcr_pid { get; set; }

        public int stream_number { get; set; }

        public List<stream> streamList;

        public string output_channel_state { get; set; }

        public string ebm_id { get; set; }
    }


    public class stream
    {
        public string stream_type { get; set; }//1字节
        public string elementary_pid { get; set; }//2字节
    }

    /// <summary>
    /// 输入通道查询
    /// </summary>
    public class InputChannelInquire
    {
        public string front_code { get; set; }

        /// <summary>
        /// 传输通道号：取 0 时不做为查询条件
        /// </summary>
        public string input_channel_id { get; set; }

        /// <summary>
        ///根据输入通道状态查询 0：查询全部 1：查询空闲；2：查询占用 3：查询故障
        /// </summary>
        public string input_channel_state;

    }

    /// <summary>
    /// 输出通道查询回复
    /// </summary>
    public class InputChannelResponse
    {
        public string front_code { get; set; }

        public List<InputChannel> InputChannelList;

    }

    public class InputChannel
    {
        public string input_channel_id { get; set; }

        public string input_channel_name { get; set; }

        public string input_channel_group { get; set; }

        public string input_channel_state { get; set; }
    }



    public class RDSScanFreInfo
    {
        public List<RDSScanFre> RDSScanFreList { get; set; }

        public List<string> coverag_resource_List { get; set; }
    }

    public class RDSScanFre
    {
        /// <summary>
        /// 频点序号
        /// </summary>
        public string freqCount { get; set; }
        /// <summary>
        /// 频点优先级
        /// </summary>
        public string freqPri { get; set; }
        /// <summary>
        /// 频率
        /// </summary>
        public string freq { get; set; }
    }

    //public class PowerAmplifierSwitch
    //{
    //    /// <summary>
    //    /// 1：表示关闭喇叭2：表示打开喇叭
    //    /// </summary>
    //    public string switch_option { get; set; }

    //    public string resource_code_type { get; set; }

    //    public List<string> resource_codeList;//资源码信息  默认所有资源码都是同一长度
    //}

    public class WhiteListInfo
    {
        public List<WhiteList> WhiteListsList { get; set; }
    }

    public class WhiteList
    {
        public string oper_type { get; set; }

        public string phone_number { get; set; }

        public string user_name { get; set; }

        public string permission_type { get; set; }

        public List<string> permission_area_codeList;//资源码信息  默认所有资源码都是同一长度

    }

    

  




}
