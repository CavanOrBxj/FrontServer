using Apache.NMS;
using EBMTable;
using EBSignature;
using FrontServer.Enums;
using FrontServer.StructClass;
using FrontServer.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FrontServer
{
    public partial class MainForm : Form
    {
        public static IniFiles ini;
        private MQ m_mq = null;
        private IMessageConsumer m_consumer;
        private bool isConn = false; //是否已与MQ服务器正常连接

        public delegate void LogAppendDelegate(string text, string type);
        private IoServer iocp = new IoServer(10, 2048);
        private int TcpReceivePort = 0;

        public FrontServerDataHelper dataHelperreal;

        public string RemoteSeverIP;//县适配器的IP地址
        public int RemoteServerPort;//县适配器的数据接收端口

       

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            CheckIniConfig();

            ConnectMQServer();

            ConnectSQLServer();

            InitUsbPwsSupport();

            LogHelper.WriteLog(typeof(Program), "程序启动！", "Info");
            LogMessage("程序启动！", "1");
            dataHelperreal = new FrontServerDataHelper();
            InitTCPServer();
            FrontServerDataHelper.MyEvent += new FrontServerDataHelper.MyDelegate(PrintMain);
            FrontServerDataHelper.JumpEvent += new FrontServerDataHelper.MyDelegate(GlobalDataDeal);
            TcpHelper.MyEvent += new TcpHelper.MyDelegate(PrintMain);

            //string CalculateSignaturestr = "AAAIgAAANgEAAjnxm8VBCE7PZxdF1oBrlRYmMvzi3/F/xCJp04AOxgni3D1OjzJO41SwRR3qCefLkcR4povzenHpvzna0joeaPI=";
            //byte[] signdata = Convert.FromBase64String(CalculateSignaturestr);
            //int qwq = signdata.Length;
        }

        private void GlobalDataDeal(object obj)
        {
            try
            {
                ParamObject ReceiveObject = (ParamObject)obj;
                switch (ReceiveObject.commandcode)
                {
                    case 0x18: //任务上报

                        TaskUploadBegin op = (TaskUploadBegin)ReceiveObject.paramobj;
                        //开
                        OnlineAllStart("1");
                        break;

                    case 0x19: //任务上报结束

                        //TaskUploadBegin op = (TaskUploadBegin)ReceiveObject.paramobj;
                        //开
                        OnlineSelectedStop();
                        break;
                    case 0x20:

                        RecvHeartBeat receiveheartbeat = (RecvHeartBeat)ReceiveObject.paramobj;
                      
                        break;
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(typeof(MainForm), ex.ToString());
            }
        }

        private void OnlineAllStart(object obj)
        {
            string broadcastType = (string)obj;
            SendPlayInfo palyinfo = new SendPlayInfo();
            palyinfo.broadcastType = broadcastType;//0：日常  1：应急
            palyinfo.organization_List = new List<organizationdata>();
            Generalresponse response = (Generalresponse)SingletonInfo.GetInstance().post.PostCommnand(palyinfo, "播放");
            if (response.code == 0)
            {
                Thread.Sleep(10000);
                broadcastrecord broadcastrecordresponse = (broadcastrecord)SingletonInfo.GetInstance().post.PostCommnand(null, "直播列表");
                if (broadcastrecordresponse.data.Count > 0)
                {
                    SingletonInfo.GetInstance().Interstitial_prlId = broadcastrecordresponse.data[0].prlId.ToString();
                }
            }
        }


        private void OnlineSelectedStop()
        {
            List<string> STOP_ID_List = new List<string>();
            STOP_ID_List.Add(SingletonInfo.GetInstance().Interstitial_prlId);
            if (STOP_ID_List.Count > 0)
            {
                Generalresponse stopresponse = (Generalresponse)SingletonInfo.GetInstance().post.PostCommnand(STOP_ID_List, "停止");           
            }
        }


        public void NetErrorDeal()
        {
            this.Invoke(new Action(() =>
            {
                SingletonInfo.GetInstance().OpenScramblerReturn = 2;//表示打开密码器的预制状态
                                                                    //  MenuItemStopStream_Click(null, null);
                MessageBox.Show("网络异常,数据发送停止！");
            }));

        }
        /// <summary>
        /// 启动TCP服务
        /// </summary>
        private void InitTCPServer()
        {
            iocp.Start(TcpReceivePort);
            iocp.mainForm = this;
        }
        
        #region  打印显示
        public void LogAppend(string text, string type)
        {
            switch (type)
            {
                case "1":
                    SystemMsg.AppendText("\n\r");
                    SystemMsg.AppendText(text);
                    break;
                case "2":
                    MQRecvMsg.AppendText("\n\r");
                    MQRecvMsg.AppendText(text);
                    break;
                case "3":
                    TCPSendMsg.AppendText("\n\r");
                    TCPSendMsg.AppendText(text);
                    break;
                case "4":
                    TCPRecvMsg.AppendText("\n\r");
                    TCPRecvMsg.AppendText(text);
                    break;

            }

        }
        public void LogMessage(string text, string type)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            switch (type)
            {
                case "1":
                    SystemMsg.Invoke(la, text, "1");
                    break;
                case "2":
                    MQRecvMsg.Invoke(la, text, "2");
                    break;
                case "3":
                    TCPSendMsg.Invoke(la, text, "3");
                    break;
                case "4":
                    TCPRecvMsg.Invoke(la, text, "4");
                    break;

            }
        }

        private void PrintMain(object data)
        {
            PrintData showinfo = (PrintData)data;
            LogMessage(showinfo.MessageInfo, showinfo.source);
        }
        #endregion


        private bool CheckIniConfig()
        {
            try
            {
                string iniPath = Path.Combine(Application.StartupPath, "FrontServer.ini");
                ini = new IniFiles(iniPath);
                SingletonInfo.GetInstance().LocalHost = ini.ReadValue("AddressInfo", "LocalHost");
               
                SingletonInfo.GetInstance().ebm_id_front = ini.ReadValue("EBM", "ebm_id_front");
                SingletonInfo.GetInstance().ebm_id_behind = ini.ReadValue("EBM", "ebm_id_behind");
                SingletonInfo.GetInstance().ebm_id_count = Convert.ToInt32(ini.ReadValue("EBM", "ebm_id_count"));


                #region AddCertInfo
                SingletonInfo.GetInstance().IsUseAddCert = ini.ReadValue("AddCertInfo", "IsUseAddCert") == "1" ? true : false;//“1”表示使用增加的证书 2表示不使用增加证书信息
              

                EBCert tmp1 = new EBCert();
                tmp1.Cert_sn = ini.ReadValue("AddCertInfo", "Cert_SN1");
                tmp1.PriKey = ini.ReadValue("AddCertInfo", "PriKey1");
                tmp1.PubKey = ini.ReadValue("AddCertInfo", "PubKey1");
                SingletonInfo.GetInstance().DicCertIndex2CerSN.Add(tmp1.Cert_sn,SingletonInfo.GetInstance().InlayCA.AddEBCert(tmp1));

                EBCert tmp2 = new EBCert();
                tmp2.Cert_sn = ini.ReadValue("AddCertInfo", "Cert_SN2");
                tmp2.PriKey = ini.ReadValue("AddCertInfo", "PriKey2");
                tmp2.PubKey = ini.ReadValue("AddCertInfo", "PubKey2");
                SingletonInfo.GetInstance().DicCertIndex2CerSN.Add(tmp2.Cert_sn,SingletonInfo.GetInstance().InlayCA.AddEBCert(tmp2));


                EBCert tmp3 = new EBCert();
                tmp3.Cert_sn = ini.ReadValue("AddCertInfo", "Cert_SN3");
                tmp3.PriKey = ini.ReadValue("AddCertInfo", "PriKey3");
                tmp3.PubKey = ini.ReadValue("AddCertInfo", "PubKey3");
                SingletonInfo.GetInstance().DicCertIndex2CerSN.Add(tmp3.Cert_sn,SingletonInfo.GetInstance().InlayCA.AddEBCert(tmp3));


                EBCert tmp4 = new EBCert();
                tmp4.Cert_sn = ini.ReadValue("AddCertInfo", "Cert_SN4");
                tmp4.PriKey = ini.ReadValue("AddCertInfo", "PriKey4");
                tmp4.PubKey = ini.ReadValue("AddCertInfo", "PubKey4");
                SingletonInfo.GetInstance().DicCertIndex2CerSN.Add(tmp4.Cert_sn, SingletonInfo.GetInstance().InlayCA.AddEBCert(tmp4));



                EBCert tmp5 = new EBCert();
                tmp5.Cert_sn = ini.ReadValue("AddCertInfo", "Cert_SN5");
                tmp5.PriKey = ini.ReadValue("AddCertInfo", "PriKey5");
                tmp5.PubKey = ini.ReadValue("AddCertInfo", "PubKey5");
                SingletonInfo.GetInstance().DicCertIndex2CerSN.Add(tmp5.Cert_sn, SingletonInfo.GetInstance().InlayCA.AddEBCert(tmp5));
                #endregion

                RemoteSeverIP = ini.ReadValue("AddressInfo", "RemoteServerIP");
                RemoteServerPort = Convert.ToInt32(ini.ReadValue("AddressInfo", "RemoteServerPort"));
                TcpReceivePort = Convert.ToInt32(ini.ReadValue("AddressInfo", "RecvPort"));

                SingletonInfo.GetInstance().HttpServer = ini.ReadValue("HttpURL", "HttpServer");
                SingletonInfo.GetInstance().pid = ini.ReadValue("PlayInfo", "playPID");


                SingletonInfo.GetInstance().SQLServerIP = ini.ReadValue("Database", "ServerName");
                SingletonInfo.GetInstance().SQLDataBaseName = ini.ReadValue("Database", "DataBase");
                SingletonInfo.GetInstance().SQLUserName = ini.ReadValue("Database", "LogID");
                SingletonInfo.GetInstance().SQLPassword = ini.ReadValue("Database", "LogPass");


                SingletonInfo.GetInstance().FrontCode= ini.ReadValue("FrontInfo", "front_code");

                SingletonInfo.GetInstance().SignatureType= ini.ReadValue("Scrambler", "SignatureType") == "0" ? false : true;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(MainForm), "配置文件打开失败", "Debug");//日志测试  20180319
                return false;
            }
            return true;
        }


        /// <summary>
        /// 初始化MQ信息
        /// </summary>
        private void ConnectMQServer()
        {
            try
            {
                m_mq = new MQ();
                m_mq.uri = "failover:tcp://" + ini.ReadValue("MQ", "MQIP") + ":" + ini.ReadValue("MQ", "MQPORT");
                m_mq.username = "admin";
                m_mq.password = "admin";
                m_mq.Start();
                isConn = true;
                m_consumer = m_mq.CreateConsumer(true, ini.ReadValue("MQ", "TopicName"));
                m_consumer.Listener += new MessageListener(consumer_listener);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(MainForm), ex.ToString(),"Debug");
            }
        }


        private void ConnectSQLServer()
        {
            DbHelperSQL.connectionString= GetConnectString();
        }


        private void InitUsbPwsSupport()
        {
            if (!SingletonInfo.GetInstance().SignatureType)
            {
                try
                {
                    int nReturn = SingletonInfo.GetInstance().usb.USB_OpenDevice(ref SingletonInfo.GetInstance().phDeviceHandle);
                    if (nReturn != 0)
                    {
                        MessageBox.Show("密码器打开失败！");
                        MessageBox.Show(nReturn.ToString());
                    }
                }
                catch (Exception em)
                {
                    MessageBox.Show("密码器打开失败：" + em.Message);
                }
            }

        }

        #region
        //获取数据库连接字符串
        private String GetConnectString()
        {
            string sReturn;
            string sPass;
            sPass = SingletonInfo.GetInstance().SQLPassword;//用户登录密码  
            if (sPass == "")
                sPass = "tuners2012";
            sReturn = "server = " + SingletonInfo.GetInstance().SQLServerIP +//数据库地址
                   ";UID = " + SingletonInfo.GetInstance().SQLUserName +//用户名
                    ";Password = " + sPass +
                     ";DataBase = " +SingletonInfo.GetInstance().SQLDataBaseName + ";"//数据库名
                     + "MultipleActiveResultSets=True";

            return sReturn;
        }
        #endregion


        private void consumer_listener(IMessage message)
        {
            string strMsg;
            try
            {
                ITextMessage msg = (ITextMessage)message;
                strMsg = msg.Text;
                LogHelper.WriteLog(typeof(Program), "MQ接收信息打印：" + strMsg, "Debug");
                LogMessage(DateTime.Now.ToString()+":" + strMsg, "2");
                Serialize(message.Properties);
            }
            catch (System.Exception ex)
            {
                m_consumer.Close();
                LogHelper.WriteLog(typeof(Program), "MQ数据处理异常：" + ex.ToString(), "Debug");
                GC.Collect();
            }
        }

     
        private void SendMQMessage(string str)
        {
            try
            {
                if (str != null)
                {
                    m_mq.SendMQMessage(str);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(Program), "MQ通讯异常", "Debug");
            }
        }

        public void Serialize(IPrimitiveMap MsgMap)
        {
            string commandId = MsgMap["CommandID"].ToString();

            switch (commandId)
            {

                case "1":
                    // AnalysisCertAuthData(MsgMap);
                    break;
                case "2":
                     AnalysisEBMIndexData(MsgMap);
                    break;
                case "3":
                     AnalysisEBMConfigureData(MsgMap);
                    break;
                case "4":
                      AnalysisDailyBroadcastData(MsgMap);
                    break;
                case "5":
                    //   AnalysisEBContentData(MsgMap);
                    break;
                case "6":
                    // ChangeInputChannel(MsgMap);
                    AnalysisFrontSetRDSScanFre(MsgMap);
                    break;
                case "7":
                    AnalysisFrontSetWhiteList(MsgMap);
                    break;

                case "8":
                  
                    break;

            }
        }

        private void AnalysisEBMIndexData(IPrimitiveMap map)
        {
            string packetype = map["PACKETTYPE"].ToString();
            OperatorData op = new OperatorData();
            op.OperatorType = packetype;

            switch (packetype)
            {
                case "AddEBMIndex":
                    EBMIndexTmp tmp = new EBMIndexTmp();

                    tmp.ExtraData= map["ExtraData"].ToString();
                    tmp.BL_details_channel_indicate = map["BL_details_channel_indicate"].ToString();
                    tmp.IndexItemID = map["IndexItemID"].ToString();

                    tmp.S_EBM_original_network_id = (SingletonInfo.GetInstance().OriginalNetworkId + 1).ToString();
                    tmp.S_EBM_start_time = map["S_EBM_start_time"].ToString();
                    tmp.S_EBM_end_time = map["S_EBM_end_time"].ToString();
                    tmp.S_EBM_type = map["S_EBM_type"].ToString();
                    tmp.S_EBM_class = map["S_EBM_class"].ToString();
                    tmp.S_EBM_level = map["S_EBM_level"].ToString();
                    tmp.List_EBM_resource_code = map["List_EBM_resource_code"].ToString();

                    tmp.DesFlag = map["DesFlag"].ToString();
                    tmp.S_details_channel_transport_stream_id = map["S_details_channel_transport_stream_id"].ToString();
                    tmp.S_details_channel_program_number = map["S_details_channel_program_number"].ToString();
                    tmp.S_details_channel_PCR_PID = map["S_details_channel_PCR_PID"].ToString();

                    tmp.DeliverySystemDescriptor = new object();
                    if (tmp.DesFlag == "true")
                    {
                        string data = map["DeliverySystemDescriptor"].ToString();
                        string pp1 = data.Substring(1);
                        string dd1 = pp1.Substring(0, pp1.Length - 1);
                        if (data.Contains("B_FEC_inner"))//有线传送系统描述符
                        {
                            tmp.DeliverySystemDescriptor = (object)JsonConvert.DeserializeObject<CableDeliverySystemDescriptortmp>(dd1);
                            tmp.descriptor_tag = 68;
                        }
                        else
                        {
                            //地面传送系统描述符
                            tmp.DeliverySystemDescriptor = (object)JsonConvert.DeserializeObject<TerristrialDeliverySystemDescriptortmp>(dd1);
                            tmp.descriptor_tag = 90;
                        }
                    }
                    tmp.List_ProgramStreamInfo = new List<ProgramStreamInfotmp>();

                    if (tmp.BL_details_channel_indicate == "true")
                    {
                        string data = map["List_ProgramStreamInfo"].ToString();


                        JavaScriptSerializer Serializer = new JavaScriptSerializer();

                        List<ProgramStreamInfotmp> objs = Serializer.Deserialize<List<ProgramStreamInfotmp>>(data);

                        foreach (ProgramStreamInfotmp item in objs)
                        {

                            //修改于20180530
                            item.Descriptor2 = null;
                            //测试注释20181227

                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                item.B_stream_type = "00";
                            }
                            else
                            {
                                if (item.B_stream_type == "84")
                                {
                                    item.B_stream_type = "03";
                                }
                            }
                        }
                        tmp.List_ProgramStreamInfo = objs;

                    }
                    OnorOFFResponse_RecvMQ(tmp);
                    break;
                case "AddAreaEBMIndex":
                case "DelAreaEBMIndex":
                   
                    break;
                case "DelEBMIndex":
                    //op.Data = map["IndexItemID"].ToString();

                    op.Data= map["IndexItemID"].ToString().Split('~')[0];
                    IndexItemIData delone = SingletonInfo.GetInstance().IndexItemList.Find(s => s.IndexItemID.Equals(op.Data));
                    if (delone != null)
                    {

                        if (!SingletonInfo.GetInstance().IsTaskUpload)
                        {
                            OnorOFFBroadcast playoff = new OnorOFFBroadcast();
                            playoff.ebm_id = delone.ebm_id;
                            playoff.power_switch = "2";
                            playoff.resource_code_type = delone.resource_code_type;

                            playoff.resource_codeList = delone.resource_code_List;
                            DealBroadcastPlay(playoff);
                            SingletonInfo.GetInstance().IndexItemList.Remove(delone);
                        }
                        else
                        {
                            SingletonInfo.GetInstance().IsTaskUpload = false;
                        }
                    }
                    break;
            }
        }

        private void AnalysisEBMConfigureData(IPrimitiveMap map)
        {
            string packetype = map["PACKETTYPE"].ToString();
            OperatorData op = new OperatorData();
            op.OperatorType = packetype;
            op.ModuleType = map["Cmdtag"].ToString();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();

            switch (packetype)
            {
                case "AddEBMConfigure":
                case "ModifyEBMConfigure":
                    string data = map["data"].ToString();
                    switch (op.ModuleType)
                    {
                        case "1"://时间校准
                            JsonstructureDeal(ref data);
                            //  List<TimeService_> listTS = Serializer.Deserialize<List<TimeService_>>(data);
                            //因为有ValueType缘故 不能反序列化
                            string[] dataArray = data.Split(',');
                            TimeService_ pp = new TimeService_();
                            foreach (var item in dataArray)
                            {
                                if (item.Contains("ItemID"))
                                {
                                    pp.ItemID = item.Split(':')[1];
                                }

                                if (item.Contains("B_Daily_cmd_tag"))
                                {
                                    // pp.B_Daily_cmd_tag = Convert.ToByte(item.Split(':')[1]);
                                }
                                if (item.Contains("Configure"))
                                {
                                    pp.Configure = new EBConfigureTimeService();

                                    string timestr = item.Substring(26);
                                    string time = timestr.Substring(0, timestr.Length - 2);
                                    DateTime dd = Convert.ToDateTime(time);
                                    pp.Configure.Real_time = dd;
                                    // pp.Configure.Real_time = item.Split(':')[1].Split(':')[1].TrimEnd('}');
                                }

                                if (item.Contains("GetSystemTime"))
                                {
                                    // pp.GetSystemTime = item.Split(':')[1] == "true" ? true : false;

                                    //按照陈良需求 修改为固定取系统时间  
                                    pp.GetSystemTime = true;
                                }

                                if (item.Contains("SendTick"))
                                {
                                    string qq = item.Split(':')[1];
                                    string ww = qq.Substring(0, qq.Length - 2);

                                    if (Convert.ToInt32(ww) < 60)
                                    {
                                        pp.SendTick = 60;

                                    }
                                    else
                                    {
                                        pp.SendTick = Convert.ToInt32(ww);
                                    }

                                }
                            }
                            List<TimeService_> listTS = new List<TimeService_>();

                            listTS.Add(pp);
                            ClockCalibration_RecvMQ(listTS);
                            //op.Data = listTS;
                            break;
                        case "2"://2区域码设置
                            JsonstructureDeal(ref data);

                            List<SetAddress_> listSA = Serializer.Deserialize<List<SetAddress_>>(data);
                            
                            ResourceCode_RecvMQ(listSA);
                            break;
                        case "3"://工作模式设置
                            JsonstructureDeal(ref data);
                            List<WorkMode_> listWM = Serializer.Deserialize<List<WorkMode_>>(data);

                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                for (int i = 0; i < listWM.Count; i++)
                                {
                                    for (int j = 0; j < listWM[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listWM[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listWM[i].Configure.list_Terminal_Address[j] = "F6" + listWM[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listWM.Count; i++)
                                {
                                    for (int j = 0; j < listWM[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listWM[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listWM[i].Configure.list_Terminal_Address[j] = "0612" + listWM[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                            }

                            op.Data = listWM;
                            break;
                        case "4"://锁定频率设置
                            JsonstructureDeal(ref data);
                            List<MainFrequency_> listMF = Serializer.Deserialize<List<MainFrequency_>>(data);
                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                for (int i = 0; i < listMF.Count; i++)
                                {
                                    for (int j = 0; j < listMF[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listMF[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listMF[i].Configure.list_Terminal_Address[j] = "F6" + listMF[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listMF.Count; i++)
                                {
                                    for (int j = 0; j < listMF[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listMF[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listMF[i].Configure.list_Terminal_Address[j] = "0612" + listMF[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                            }

                            op.Data = listMF;
                            break;
                        case "5"://回传方式设置
                            JsonstructureDeal(ref data);
                            //又要特殊处理

                            string tmp1 = data.Replace("\"S_reback_address_backup\":,", "\"S_reback_address_backup\":\"null\",");
                            string tmp2 = tmp1.Replace("\"I_reback_port_Backup\":,", "\"I_reback_port_Backup\":0,");
                            
                                List<Reback_Nation> listRB = Serializer.Deserialize<List<Reback_Nation>>(tmp2);
                                for (int i = 0; i < listRB.Count; i++)
                                {
                                    for (int j = 0; j < listRB[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listRB[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listRB[i].Configure.list_Terminal_Address[j] = "F6" + listRB[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                               // op.Data = listRB;
                            RebackSet_RecvMQ(listRB);

                            break;
                        case "6"://默认音量设置
                            JsonstructureDeal(ref data);
                            List<DefaltVolume_> listDV = Serializer.Deserialize<List<DefaltVolume_>>(data);

                            
                                for (int i = 0; i < listDV.Count; i++)
                                {
                                    for (int j = 0; j < listDV[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listDV[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listDV[i].Configure.list_Terminal_Address[j] = "F6" + listDV[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            

                           // op.Data = listDV;

                            VolumeSet_RecvMQ(listDV);
                            break;
                        case "7"://回传周期设置
                            JsonstructureDeal(ref data);
                            List<RebackPeriod_> listRP = Serializer.Deserialize<List<RebackPeriod_>>(data);
                          

                                for (int i = 0; i < listRP.Count; i++)
                                {
                                    for (int j = 0; j < listRP[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listRP[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listRP[i].Configure.list_Terminal_Address[j] = "F6" + listRP[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }


                            //  op.Data = listRP;

                            RebackPeriod_RecvMQ(listRP);
                            break;
                        case "104"://启动内容检测指令
                            JsonstructureDeal(ref data);
                            List<ContentMoniterRetback_> listCMR = Serializer.Deserialize<List<ContentMoniterRetback_>>(data);

                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                for (int i = 0; i < listCMR.Count; i++)
                                {
                                    for (int j = 0; j < listCMR[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listCMR[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listCMR[i].Configure.list_Terminal_Address[j] = "F6" + listCMR[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listCMR.Count; i++)
                                {
                                    for (int j = 0; j < listCMR[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listCMR[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listCMR[i].Configure.list_Terminal_Address[j] = "0612" + listCMR[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                            }

                            op.Data = listCMR;
                            break;
                        case "105"://启动内容监测实时监听指令
                            JsonstructureDeal(ref data);
                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                List<ContentRealMoniter_> listCRM = Serializer.Deserialize<List<ContentRealMoniter_>>(data);

                                for (int i = 0; i < listCRM.Count; i++)
                                {
                                    for (int j = 0; j < listCRM[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listCRM[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listCRM[i].Configure.list_Terminal_Address[j] = "F6" + listCRM[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                                op.Data = listCRM;
                            }
                            else
                            {
                                List<ContentRealMoniterGX_> listCRMGX = Serializer.Deserialize<List<ContentRealMoniterGX_>>(data);
                                for (int i = 0; i < listCRMGX.Count; i++)
                                {
                                    for (int j = 0; j < listCRMGX[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listCRMGX[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listCRMGX[i].Configure.list_Terminal_Address[j] = "0612" + listCRMGX[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                                op.Data = listCRMGX;
                            }
                            break;
                        case "106"://终端工作状态查询
                            JsonstructureDeal(ref data);
                            List<StatusRetback_> listSR = Serializer.Deserialize<List<StatusRetback_>>(data);

                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                for (int i = 0; i < listSR.Count; i++)
                                {
                                    for (int j = 0; j < listSR[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listSR[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listSR[i].Configure.list_Terminal_Address[j] = "F6" + listSR[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listSR.Count; i++)
                                {
                                    for (int j = 0; j < listSR[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listSR[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listSR[i].Configure.list_Terminal_Address[j] = "0612" + listSR[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                            }

                            op.Data = listSR;
                            break;
                        case "240"://终端固件升级
                            JsonstructureDeal(ref data);
                            List<SoftwareUpGrade_> listSUG = Serializer.Deserialize<List<SoftwareUpGrade_>>(data);

                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {

                                for (int i = 0; i < listSUG.Count; i++)
                                {
                                    for (int j = 0; j < listSUG[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listSUG[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listSUG[i].Configure.list_Terminal_Address[j] = "F6" + listSUG[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listSUG.Count; i++)
                                {
                                    for (int j = 0; j < listSUG[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listSUG[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listSUG[i].Configure.list_Terminal_Address[j] = "0612" + listSUG[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                            }


                            op.Data = listSUG;
                            break;
                        case "8"://RDS配置
                                 // JsonstructureDeal(ref data);
                            data = data.Substring(18, data.Length - 18);  //特殊处理  刘工发送的json字符异常
                            List<RdsConfig_> listRC = Serializer.Deserialize<List<RdsConfig_>>(data);
                            string textRdsData = "";
                            foreach (var item in data.Split(','))
                            {
                                if (item.Contains("RdsDataText"))
                                {
                                    textRdsData = item;
                                }
                            }
                            textRdsData = textRdsData.Substring(15);
                            textRdsData = textRdsData.Substring(0, textRdsData.Length - 4);
                            listRC[0].Configure.Br_Rds_data = Utils.ArrayHelper.String2Bytes(textRdsData);
                            listRC[0].RdsDataText = textRdsData;

                            if (!SingletonInfo.GetInstance().IsGXProtocol)
                            {
                                for (int i = 0; i < listRC.Count; i++)
                                {
                                    for (int j = 0; j < listRC[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listRC[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listRC[i].Configure.list_Terminal_Address[j] = "F6" + listRC[i].Configure.list_Terminal_Address[j] + "0314000000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listRC.Count; i++)
                                {
                                    for (int j = 0; j < listRC[i].Configure.list_Terminal_Address.Count; j++)
                                    {
                                        if (listRC[i].Configure.list_Terminal_Address[j].Length == 12)
                                        {
                                            listRC[i].Configure.list_Terminal_Address[j] = "0612" + listRC[i].Configure.list_Terminal_Address[j] + "00";
                                        }
                                    }
                                }
                            }

                            op.Data = listRC;
                            break;

                        case "300":
                            //状态查询指令

                            JsonstructureDeal(ref data);
                            //又要特殊处理

                            string tmp3 = data.Replace("\"S_reback_address_backup\":,", "\"S_reback_address_backup\":\"null\",");
                            string tmp4 = tmp3.Replace("\"I_reback_port_Backup\":,", "\"I_reback_port_Backup\":0,");

                            List<Reback_Nation_add> listRBa = Serializer.Deserialize<List<Reback_Nation_add>>(tmp4);
                            for (int i = 0; i < listRBa.Count; i++)
                            {
                                for (int j = 0; j < listRBa[i].Configure.list_Terminal_Address.Count; j++)
                                {
                                    if (listRBa[i].Configure.list_Terminal_Address[j].Length == 12)
                                    {
                                        listRBa[i].Configure.list_Terminal_Address[j] = "F6" + listRBa[i].Configure.list_Terminal_Address[j] + "0314000000";
                                    }
                                }
                            }
                            // op.Data = listRB;
                            listRBa[0].query_code_List = new List<string>();
                            //string ppo = "[{"ItemID":3,"B_Daily_cmd_tag":300,"Configure":{"B_reback_type":2,"S_reback_address":"192.168.100.100","S_reback_address_backup":"","I_reback_port":7202,"I_reback_port_Backup":,"B_Address_type":1,"list_Terminal_Address":["532625000000"],"query_code":["01,02,03,04,05,06,07,08,09,0A,0B,0C,0D,0E,0F,10,11,12"]}}]";

                            int local = data.IndexOf("query_code");
                           string  tmp22= data.Substring(local + 14);
                            string tmp33 = tmp22.Substring(0,tmp22.Length-5);
                            string[] query_codes = tmp33.Split(',');

                            #region 必须要查物理码 
                            bool exists = ((IList)query_codes).Contains("5");
                            if (!exists)
                            {
                                listRBa[0].query_code_List.Add("5");
                            }
                            #endregion
                            foreach (string item in query_codes)
                            {
                                listRBa[0].query_code_List.Add(item);
                            }
                            listRBa[0].query_code_List.Sort();
                            StatusInquiry_RecvMQ(listRBa);
                            break;
                    }

                    break;

                case "DelEBMConfigure":
                    string ItemList = map["ItemIDList"].ToString();
                    op.Data = ItemList;
                    break;
            }
          
        }



        private void AnalysisDailyBroadcastData(IPrimitiveMap map)
        {
            string packetype = map["PACKETTYPE"].ToString();
            OperatorData op = new OperatorData();
            op.OperatorType = packetype;
            op.ModuleType = map["Cmdtag"].ToString();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();

            switch (packetype)
            {
                case "AddDailyBroadcast":
                    string data = map["data"].ToString();
                    switch (op.ModuleType)
                    {
                        case "4"://终端功放开关
                            JsonstructureDeal(ref data);
                            List<OutSwitch_> listOS = Serializer.Deserialize<List<OutSwitch_>>(data);
                            DealPowerAmplifierSwitch(listOS);
                            break;
                    }
                    break;
                       
            }

        }


        private void AnalysisFrontSetRDSScanFre(IPrimitiveMap map)
        {
            
            string strFre = map["FreList"].ToString();
            string[] data = strFre.Split('~');

            RDSScanFreInfo rdsscanfre = new RDSScanFreInfo();
            rdsscanfre.RDSScanFreList = new List<RDSScanFre>();
            rdsscanfre.coverag_resource_List = new List<string>();
            int Frecount = Convert.ToInt32(data[0]);

            for (int i = 0; i < Frecount; i++)
            {
                RDSScanFre pp = new RDSScanFre();
                string[] info = data[1 + i].Split(',');
                pp.freqCount = info[0];
                pp.freqPri= info[1];
                pp.freq = info[2];
                rdsscanfre.RDSScanFreList.Add(pp);
            }

            string[] resourcecode = map["coverag_resource"].ToString().Split(',');
            foreach (string item in resourcecode)
            {
                rdsscanfre.coverag_resource_List.Add(item);
            }
            DealRDSScanFre(rdsscanfre);
        }

        private void AnalysisFrontSetWhiteList(IPrimitiveMap map)
        {

            string strwhil = map["WhiteList"].ToString();
            string[] whiteListdata = strwhil.Split('~');

            WhiteListInfo whitelist = new WhiteListInfo();
            whitelist.WhiteListsList = new List<WhiteList>();


            foreach (string item in whiteListdata)
            {
                string[] ww = item.Split(',');
                WhiteList wl = new WhiteList();
                wl.permission_area_codeList = new List<string>();
                wl.oper_type = ww[0];
                wl.phone_number = ww[1];
                wl.user_name = ww[2];
                wl.permission_type = ww[3];

                string[] codes = ww[4].Split('-');
                foreach (string tt in codes)
                {
                    wl.permission_area_codeList.Add(tt);
                }

                whitelist.WhiteListsList.Add(wl);
            }

          
            DealWhiteList(whitelist);
        }

        private void JsonstructureDeal(ref string data)
        {
            int loacal = data.IndexOf('[');
            data = data.Substring(loacal, data.Length - loacal - 2);
        }

        #region  国标大喇叭前端播放
        private void OnorOFFResponse_RecvMQ(EBMIndexTmp tmp)
        {

            if (!SingletonInfo.GetInstance().IsTaskUpload)
            {
                IndexItemIData addone = new IndexItemIData();

                //播放指令  引用TS数据界面获取
                string[] List_EBM_resource_code_judge = tmp.List_EBM_resource_code.Split(',');//如果是逻辑码 则不发送给县适配器  20190325
                int lengtmp_judge = List_EBM_resource_code_judge[0].Length;
                if (lengtmp_judge == 23)
                {
                    LogMessage(DateTime.Now.ToString() + ":" + "收到IP协议指令！", "1");
                    return;
                }
                OnorOFFBroadcast db = new OnorOFFBroadcast();
                db.ebm_id = BBSHelper.CreateEBM_ID();
                db.power_switch = "1";
                if (tmp.S_EBM_class == "0100")
                {
                    db.ebm_class = "0004";
                }
                else
                {
                    db.ebm_class = tmp.S_EBM_class;
                }
                // db.ebm_class = tmp.S_EBM_class;
                db.ebm_level = tmp.S_EBM_level;
                db.ebm_type = tmp.S_EBM_type;
                db.start_time = tmp.S_EBM_start_time;
                db.end_time = tmp.S_EBM_end_time;
                db.volume = "255";
                db.resource_code_type = "1";
                db.resource_codeList = new List<string>();


                string[] List_EBM_resource_code = tmp.List_EBM_resource_code.Split(',');
                int lengtmp = List_EBM_resource_code[0].Length;
                if (lengtmp == 23)
                {
                    foreach (string item in List_EBM_resource_code)
                    {
                        string tt = item.Substring(1);
                        string tt1 = tt.Substring(0, tt.Length - 4);
                        db.resource_codeList.Add(tt1);
                    }
                }
                else if (lengtmp == 12)
                {
                    foreach (string item in List_EBM_resource_code)
                    {
                        //应陈良要求  首位改为0  20190318
                        db.resource_codeList.Add("0" + item + "0314000000");
                    }
                }

                db.multilingual_contentList = new List<MultilingualContentInfo>();
                MultilingualContentInfo pp = new MultilingualContentInfo();
                pp.language_code = "zho";
                pp.coded_character_set = "0";
                pp.text_char = "test";
                pp.agency_name_char = "文山州马关县应急广播平台";
                pp.AuxiliaryInfoList = new List<AuxiliaryInfo>();
                AuxiliaryInfo auxone = new AuxiliaryInfo();
                auxone.auxiliary_data_type = 61;//辅助数据类型

                db.S_elementary_PID = tmp.List_ProgramStreamInfo[0].S_elementary_PID;

                string[] ExtraData = tmp.ExtraData.Split('|');

                string rtpurl = ExtraData[0].Split('~')[1];
                auxone.auxiliary_data = rtpurl;
                pp.AuxiliaryInfoList.Add(auxone);



                db.multilingual_contentList.Add(pp);

                //     db.input_channel_id =Convert.ToInt32(ExtraData[2].Split('~')[1]);

                db.input_channel_id = 0;


                // string CHANNELOUTtmp = ExtraData[3].Split('~')[1];
                //  string CHANNELOUTS = CHANNELOUTtmp.Substring(0, CHANNELOUTtmp.Length - 1);
                //  string[] CHANNELOUT = CHANNELOUTS.Split(',');
                string[] CHANNELOUT = new string[4] { "1", "2", "3", "4" };

                db.OutPut_Channel_IdList = new List<int>();
                foreach (string item in CHANNELOUT)
                {
                    db.OutPut_Channel_IdList.Add(Convert.ToInt32(item));
                }

                addone.IndexItemID = tmp.IndexItemID;
                addone.ebm_id = db.ebm_id;
                addone.resource_code_List = new List<string>();
                addone.resource_code_type = db.resource_code_type;
                addone.resource_code_List = db.resource_codeList;
                SingletonInfo.GetInstance().IndexItemList.Add(addone);
                DealBroadcastPlay(db);
            }
                
        }

        private OnorOFFResponse DealBroadcastPlay(OnorOFFBroadcast obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
           // TcpHelper tcpsend = new TcpHelper();
            OnorOFFResponse resopnse = (OnorOFFResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.OnorOFFBroadcast, ie);
            return resopnse;
        }
        #endregion

        #region 国标时钟校准
       private void  ClockCalibration_RecvMQ(List<TimeService_> tmp)
        {
            ClockCalibration ccb = new ClockCalibration();
            ccb.time = ((DateTime)(tmp[0].Configure.Real_time)).ToString("yyyy-MM-dd HH:mm:ss");

            DealClockCalibrationPlay(ccb);
        }

        private GeneralResponse DealClockCalibrationPlay(ClockCalibration obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
          //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.UCCI, ie);
            return resopnse;
        }
        #endregion

        #region  国标音量设置
        private void VolumeSet_RecvMQ(List<DefaltVolume_> tmp)
        {
            GeneralVolumn gv = new GeneralVolumn();
            gv.resource_code_type = tmp[0].Configure.B_Address_type.ToString();
            gv.volume = tmp[0].Configure.Column.ToString();
            gv.resource_codeList = new List<string>();
            int lengtmp = tmp[0].Configure.list_Terminal_Address[0].Length-1;
            if (lengtmp == 23)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    string tt = item.Substring(1);
                  //  string tt1 = tt.Substring(0, tt.Length - 4);
                    gv.resource_codeList.Add(tt);
                }
            }
            else if (lengtmp == 12)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    gv.resource_codeList.Add("6" + item + "0314000000");
                }
            }
            DealVolumeSetPlay(gv);
        }

        private GeneralResponse DealVolumeSetPlay(GeneralVolumn obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.VolumeSet, ie);
            return resopnse;
        }
        #endregion

        #region 通用回传周期设置
        private void RebackPeriod_RecvMQ(List<RebackPeriod_> tmp)
        {
            RebackPeriod rp = new RebackPeriod();
            rp.resource_code_type = tmp[0].Configure.B_Address_type.ToString();
            rp.reback_cycle = tmp[0].Configure.reback_period.ToString();
            rp.resource_codeList = new List<string>();
            int lengtmp = tmp[0].Configure.list_Terminal_Address[0].Length - 1;
            if (lengtmp == 23)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    string tt = item.Substring(1);
                    //  string tt1 = tt.Substring(0, tt.Length - 4);
                    rp.resource_codeList.Add(tt);
                }
            }
            else if (lengtmp == 12)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    rp.resource_codeList.Add("6" + item + "0314000000");
                }
            }
            DealRebackPeriod(rp);
        }

        private GeneralResponse DealRebackPeriod(RebackPeriod obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.GRCS, ie);
            return resopnse;
        }
        #endregion

        #region  国标通用资源编码设置
        private void ResourceCode_RecvMQ(List<SetAddress_> tmp)
        {
            //ResourceCodeInfo rci = new ResourceCodeInfo();
            List<ResourceCodeInfo> rciList = new List<ResourceCodeInfo>();
            foreach (SetAddress_ item in tmp)
            {
                ResourceCodeInfo pp = new ResourceCodeInfo();
                pp.physical_address = item.Configure.S_Phisical_address;
                pp.logic_address = item.Configure.S_Logic_address;
                rciList.Add(pp);
            }
            DealResourceCodeSetPlay(rciList);
        }

        private GeneralResponse DealResourceCodeSetPlay(List<ResourceCodeInfo> obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.UpdateArea, ie);
            return resopnse;
        }
        #endregion

        #region 通用回传参数设置
        private void RebackSet_RecvMQ(List<Reback_Nation> tmp)
        {

            RebackSet rbs = new RebackSet();

            switch (tmp[0].Configure.B_reback_type.ToString())
            {
                case "1":
                    //短信
                    rbs.reback_type = "1";
                    rbs.reback_address = tmp[0].Configure.S_reback_address;
                   
                    break;
                case "2":
                    //IP 地址和端口
                    rbs.reback_type = "2";
                    rbs.reback_address = tmp[0].Configure.S_reback_address + ":"+ tmp[0].Configure.I_reback_port;
                  
                    break;
                case "3":
                    //域名+“：”+端口号
                    rbs.reback_type = "3";
                    rbs.reback_address = tmp[0].Configure.S_reback_address + ":" + tmp[0].Configure.I_reback_port;
                    break;
            }
            rbs.resource_code_type = tmp[0].Configure.B_Address_type.ToString();
            rbs.resource_codeList = new List<string>();
            int lengtmp = tmp[0].Configure.list_Terminal_Address[0].Length - 1;
            if (lengtmp == 23)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    string tt = item.Substring(1);
                    //  string tt1 = tt.Substring(0, tt.Length - 4);
                    rbs.resource_codeList.Add(tt);
                }
            }
            else if (lengtmp == 12)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    rbs.resource_codeList.Add("6" + item + "0314000000");
                }
            }

            DealRebackSet(rbs);
        }

        private GeneralResponse DealRebackSet(RebackSet obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.GRPSI, ie);
            return resopnse;
        }
        #endregion

        #region 状态查询指令
        private void StatusInquiry_RecvMQ(List<Reback_Nation_add> tmp)
        {
            StatusInquiry si = new StatusInquiry();
            //string pp = "2";
            //StatusInquiry si = new StatusInquiry();

            switch (tmp[0].Configure.B_reback_type.ToString())
            {
                case "1":
                    //短信
                    si.reback_type = "1";
                    si.reback_address = tmp[0].Configure.S_reback_address;
                   // si.reback_address = "";
                    break;
                case "2":
                    //IP 地址和端口
                    si.reback_type = "2";
                    si.reback_address = tmp[0].Configure.S_reback_address + ":" + tmp[0].Configure.I_reback_port;
                   // si.reback_address = "192.168.100.254:7777";
                    break;
                case "3":
                    //域名+“：”+端口号
                    si.reback_type = "3";
                    si.reback_address = tmp[0].Configure.S_reback_address + ":" + tmp[0].Configure.I_reback_port;
                   // si.reback_address = "";
                    break;
            }
            si.resource_code_type = "1";
            si.resource_codeList = new List<string>();
            int lengtmp = 23;
            if (lengtmp == 23)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    string tt = item.Substring(1);
                    //  string tt1 = tt.Substring(0, tt.Length - 4);
                    si.resource_codeList.Add(tt);
                }
            }
            else if (lengtmp == 12)
            {
                foreach (string item in tmp[0].Configure.list_Terminal_Address)
                {
                    si.resource_codeList.Add("6" + item + "0314000000");
                }
            }

            si.query_code_codeList = new List<string>();
            si.query_code_codeList = tmp[0].query_code_List;
            DealStatusInquiry(si);
        }

        private GeneralResponse DealStatusInquiry(StatusInquiry obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.GPRQI, ie);
            return resopnse;
        }
        #endregion

        #region 回传参数设置  非通用
        private GeneralResponse DealRebackParam(RebackParam obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.RPS, ie);
            return resopnse;
        }
        #endregion

        #region 输入通道查询  非通用
        private InputChannelResponse DealInputChannel(InputChannelInquire obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            InputChannelResponse resopnse = (InputChannelResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.ICQ, ie);
            return resopnse;
        }
        #endregion


        #region  RDS扫描频点设置
     

        private GeneralResponse DealRDSScanFre(RDSScanFreInfo obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.RDDSpi, ie);
            return resopnse;
        }
        #endregion

        #region 终端功放开关

        private GeneralResponse DealPowerAmplifierSwitch(List<OutSwitch_> obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.TerminalSwitch, ie);
            return resopnse;
        }
        #endregion

        #region 
        private GeneralResponse DealWhiteList(WhiteListInfo obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            GeneralResponse resopnse = (GeneralResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.WIU, ie);
            return resopnse;
        }
        #endregion

        #region 输出通道查询  非通用
        private OutChannelResponse DealOutputChannel(OutChannelInquire obj)
        {
            string ip = RemoteSeverIP;
            int port = RemoteServerPort;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
                                                                        //  TcpHelper tcpsend = new TcpHelper();
            OutChannelResponse resopnse = (OutChannelResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(obj, OrderType.OCQ, ie);
            return resopnse;
        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (!SingletonInfo.GetInstance().SignatureType)
            {
                int nDeviceHandle = (int)SingletonInfo.GetInstance().phDeviceHandle;
                int nReturn = SingletonInfo.GetInstance().usb.USB_CloseDevice(ref nDeviceHandle);
            }

            ini.WriteValue("EBM", "ebm_id_behind", SingletonInfo.GetInstance().ebm_id_behind);
            ini.WriteValue("EBM", "ebm_id_count", SingletonInfo.GetInstance().ebm_id_count.ToString());
        }

        private void btn_reback_cycle_Click(object sender, EventArgs e)
        {
            try
            {
                RebackParam rp = new RebackParam();
                rp.reback_type = "2";//写死
                rp.reback_cycle = txt_reback_cycle.Text.Trim();
                rp.reback_address = txt_reback_address.Text.Trim();
              GeneralResponse  re=  DealRebackParam(rp);
                if (re.return_code=="0")
                {
                    MessageBox.Show("指令发送成功！");
                } 
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //  string pp = "b0 6d 01 f0 53 26 25 10 20 00 03 14 00 00 00 65 53 26 25 10 20 00 03 14 01 02 01 20 19 03 23 00 05 00 00 00 5a 5c 96 45 7f 00 00 00 00 00 08 ee 58 be c4 cb 60 c4 c4 1d 51 0d 82 1b 48 46 3d d8 37 bc 4c 9b 13 67 11 56 0c 87 b0 cb e5 b1 9a 2f d6 1e 91 34 de 99 af 8e 26 3c 1a 60 78 2f 76 bb f8 f4 73 b5 1d c1 56 3e 15 f5 66 ff 11 f5 54";
                string pp = "FE FD 01 00 00 00 00 06 01 01 00 C3 F0 53 26 25 00 00 00 03 14 02 01 02 00 01 F0 53 26 25 00 00 00 00 00 00 00 00 01 00 4A F0 53 26 25 00 00 00 00 00 00 00 00 20 19 03 23 00 06 05 01 30 30 30 30 30 FF 5C 99 91 FA 5C 9E 25 DA 02 3D 00 15 72 74 70 3A 2F 2F 32 32 34 2E 32 2E 32 2E 32 3A 31 37 30 33 36 F0 00 0C 01 1F FF 00 00 00 00 00 00 00 00 00 00 4A 5C 99 91 FD 00 00 00 00 00 01 38 C3 58 29 98 F8 75 0B EB EB 09 F5 6B 63 A1 D3 DF CF D1 21 EE DC 45 6B A1 B0 DA EA FC C2 02 88 FB 9D 4E 28 B6 EF AC 0B E0 9C 0F 20 0C 76 5B AC 29 8A 9A 3A 55 AA D1 67 A6 EC E0 08 C9 3A E3 17 69 F3 9B FE";
                string[] ppl = pp.Split(' ');

                List<byte> input = new List<byte>();
                foreach (string item in ppl)
                {
                   
                    input.Add((byte)Convert.ToInt32(item,16));
                }
                bool flag = VerifysignatureDeXin(input.ToArray());
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool VerifysignatureDeXin(byte[] input)
        {
            bool flag = false;
            // byte[] length = input.Skip(5).Take(4).ToArray();

            //  int datalenth = GetDataLenth(length) + 9;
            int datalenth = input.Length - 64 - 10-4-2;
            byte[] pdatabuf = input.Take(datalenth).ToArray();
            byte[] signature = input.Skip(datalenth+10+2).Take(64).ToArray();
            //int random = GetDataLenth(input.Skip(input.Length - 64 - 4 - 6 - 4).Take(4).ToArray());

            byte[] Cert_Sn = new byte[6] { 0, 0, 0, 0, 0,1 };

            List<byte> pp = new List<byte>();
            pp.AddRange(Cert_Sn);
            pp.AddRange(signature);

            flag = SingletonInfo.GetInstance().InlayCA.EbMsgSignVerifyWithoutUTC(pdatabuf, pdatabuf.Length, pp.ToArray());
            return flag;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OnlineAllStart("1");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OnlineSelectedStop();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                InputChannelInquire ICI = new InputChannelInquire();
                ICI.front_code = SingletonInfo.GetInstance().FrontCode;
                ICI.input_channel_id = "0";
                ICI.input_channel_state = "0";

                InputChannelResponse ICR = DealInputChannel(ICI);


                //Thread.Sleep(2000);


                //OutChannelInquire OCI = new OutChannelInquire();
                //OCI.front_code= SingletonInfo.GetInstance().FrontCode;
                //OCI.output_channel_id = "0";
                //OCI.output_channel_state = "0";
                //OutChannelResponse OCR = DealOutputChannel(OCI);
                MessageBox.Show("指令发送成功！");
        
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string sqlstr = "INSERT INTO In_Out_Channel(channel_class, channel_id, input_channel_name, input_channel_group,input_channel_state) VALUES('0', '1', 'IP', '0','1')";
            DbHelperSQL.ExecuteSql(sqlstr);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            InputChannelInquire ICI = new InputChannelInquire();
            ICI.front_code = SingletonInfo.GetInstance().FrontCode;
            ICI.input_channel_id = "0";
            ICI.input_channel_state = "0";

            InputChannelResponse ICR = DealInputChannel(ICI);
            MessageBox.Show("指令发送成功！");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OutChannelInquire OCI = new OutChannelInquire();
            OCI.front_code = SingletonInfo.GetInstance().FrontCode;
            OCI.output_channel_id = "0";
            OCI.output_channel_state = "0";
            OutChannelResponse OCR = DealOutputChannel(OCI);
            MessageBox.Show("指令发送成功！");
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                string pp1 = "41 41 41 41 43 77 41 41 4E 67 45 41 41 6B 43 70 49 2F 31 42 45 4F 7A 39 2F 5A 61 74 71 45 69 74 41 53 7A 6D 68 75 33 55 5A 62 36 69 55 4A 42 75 35 66 39 74 45 33 32 2B 50 79 34 42 79 63 49 6C 64 36 2F 6F 58 61 7A 6F 49 34 31 4A 36 6B 78 39 73 69 34 4D 5A 37 52 53 6B 4A 43 31 76 55 73 30 34 66 6F 3D";
                string pp2 = "00 00 00 0E 00 00 36 01 00 02 4C F1 20 75 1D 55 5C 23 2D 6C F9 ED 77 19 DE 3F B3 01 A5 AF 5D DF B4 AF 1E 20 C7 E5 BD 82 B8 A7 18 49 04 62 BE 3E 67 71 0C A0 90 0E DB F6 49 00 4B 25 40 90 E0 53 81 3B FB 08 A6 1D 05 F6 B6 B2";
                List<byte> pdatabuf = new List<byte>();
                List<byte> signature = new List<byte>();
                string[] ppList = pp1.Split(' ');
                string[] pplist2 = pp2.Split(' ');
                foreach (string item in ppList)
                {
                    pdatabuf.Add((byte)Convert.ToInt32(item,16));
                }

                foreach (string item in pplist2)
                {
                    signature.Add((byte)Convert.ToInt32(item, 16));
                }
                int random = 0;

               bool flag = SingletonInfo.GetInstance().InlayCA.EbMsgSignVerify(pdatabuf.ToArray(), pdatabuf.Count,ref random, signature.ToArray());

                if (flag)
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
