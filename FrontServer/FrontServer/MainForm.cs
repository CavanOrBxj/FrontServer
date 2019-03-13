using Apache.NMS;
using System;
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
using System.Windows.Forms;

namespace FrontServer
{
    public partial class MainForm : Form
    {
        public static IniFiles ini;
        private MQ m_mq = null;
        
        private IMessageConsumer m_consumer;
        private bool isConn = false; //是否已与MQ服务器正常连接
        private string MQIP = "";
        private string MQPORT = "";
        private string MQUSER = "";
        private string MQWD = "";
        private string TopicName1 = "";
        private string TopicName2 = "";

        public delegate void LogAppendDelegate(string text);

        private System.Timers.Timer tm;

        private IoServer iocp = new IoServer(10, 2048);
        private int TcpReceivePort = 0;

        public FrontServerDataHelper dataHelperreal;


        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            CheckIniConfig();
            LogHelper.WriteLog(typeof(Program), "程序启动！","");
            LogMessage("程序启动！");

            DealMqConnection();


            dataHelperreal = new FrontServerDataHelper();
        }

     


        /// <summary>
        /// 启动TCP服务
        /// </summary>
        private void InitTCPServer()
        {
            iocp.Start(TcpReceivePort);
            iocp.mainForm = this;
        }



        private void DealMqConnection()
        {

            if (OpenMQ(MQIP, MQPORT, MQUSER, MQWD))
            {
                Open_consumer(TopicName1);             //创建消息消费者
                m_mq.CreateProducer(false, TopicName2);//创建消息生产者   //Queue
            }
            else
            {
                LogMessage("MQ连接失败！");
            }
        }

        /// <summary> 
        /// 追加显示文本 
        /// </summary> 
        /// <param name="color">文本颜色</param> 
        /// <param name="text">显示文本</param> 
        public void LogAppend(string text)
        {
            richTextRebackMsg.AppendText("\n");
            richTextRebackMsg.AppendText(text);
        }


        public void LogMessage(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            richTextRebackMsg.Invoke(la,text);
        } 

        
        private bool CheckIniConfig()
        {
            try
            {
                string iniPath = Path.Combine(Application.StartupPath, "FrontServer.ini");
                ini = new IniFiles(iniPath);
                MQIP = ini.ReadValue("MQ", "MQIP");
                MQPORT = ini.ReadValue("MQ", "MQPORT");
                MQUSER = ini.ReadValue("MQ", "MQUSER");
                MQWD = ini.ReadValue("MQ", "MQPWD");
                TopicName1 = ini.ReadValue("MQ", "RECTOPIC");
                TopicName2 = ini.ReadValue("MQ", "SENDTOPIC");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(MainForm), "配置文件打开失败", "Debug");//日志测试  20180319
                return false;
            }
            return true;
        }

        private bool OpenMQ(string _MQIP, string _MQPort, string _MQUser, string _MQPWD)
        {
            string txtURI = "";
              txtURI = "failover:tcp://" + _MQIP + ":" + _MQPort;//带断线重连  failover
            try
            {
                m_mq = new MQ();
                m_mq.uri = txtURI;
                m_mq.username = _MQUser;
                m_mq.password = _MQPWD;
                m_mq.Start();
                isConn = true;
            }
            catch (System.Exception ex)
            {
                isConn = false;
                LogHelper.WriteLog(typeof(MainForm), "连接MQ服务器异常，请检查端口号、IP地址、用户名及密码是否正确！", "Debug");//日志测试  20180319
            }
            return isConn;
        }

        private void Open_consumer(string _MQRecTopic)
        {
            try
            {
                if (m_consumer != null)
                {
                    m_consumer.Close();
                    m_consumer = null;
                    GC.Collect();
                }
                m_consumer = m_mq.CreateConsumer(false, _MQRecTopic);
                m_consumer.Listener += new MessageListener(consumer_listener);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(typeof(MainForm), "MQ生产者、消费者初始化失败！", "Debug");//日志测试  20180319
            }
        }

        
        private void consumer_listener(IMessage message)
        {
            string strMsg;

            try
            {
                ITextMessage msg = (ITextMessage)message;
                strMsg = msg.Text;
                LogHelper.WriteLog(typeof(Program), "MQ接收信息打印：" + strMsg, "Debug");
                LogMessage("MQ接收信息打印：" + strMsg);

                LogHelper.WriteLog(typeof(EBMMain), "收到MQ指令！");
                Serialize(message.Properties);


                string contenettmp = "";
                string file = "";

                if (strMsg.Contains("PACKTYPE") && strMsg.Contains("CONTENT") && strMsg.Contains("FILE"))//防止误收
                {
                    string[] commandsection = strMsg.Split('|');
                    foreach (string item in commandsection)
                    {
                        if (item.Contains("CONTENT"))
                        {
                            contenettmp = item.Split('~')[1];
                        }
                        if (item.Contains("FILE"))
                        {
                           // file = _path + "\\" + item.Split('~')[1];
                        }
                    }
                   
                }
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
                   // AnalysisEBMIndexData(MsgMap);
                    break;
                case "3":
                 //   AnalysisEBMConfigureData(MsgMap);
                    break;
                case "4":
                 //   AnalysisDailyBroadcastData(MsgMap);
                    break;
                case "5":
                 //   AnalysisEBContentData(MsgMap);
                    break;
                case "6":
                    ChangeInputChannel(MsgMap);
                    break;
            }
        }


        private void ChangeInputChannel(IPrimitiveMap map)
        {
            string packetype = map["PACKETTYPE"].ToString();
            OperatorData op = new OperatorData();
            op.OperatorType = packetype;
            switch (packetype)
            {
                case "ChangeInutChannel":
                    CountyplatformChangechannel changechannelinfo = new CountyplatformChangechannel();
                    changechannelinfo.inputchannel = map["inputchannel"].ToString().Trim();
                    changechannelinfo.PhysicalCode = map["PhysicalCode"].ToString().Trim();
                    changechannelinfo.ResourceCode = map["ResourceCode"].ToString().Trim();
                    op.Data = (object)changechannelinfo;
                    break;
            }
            DealChangeInputChannel(op);
        }


        #region  广西县平台切换通道数据处理
        private void DealChangeInputChannel(OperatorData op)
        {

            CountyplatformChangechannel changechannelinfo = (CountyplatformChangechannel)op.Data;

            string ip = changechannelinfo.PhysicalCode.Split(':')[0];
            int port = Convert.ToInt32(changechannelinfo.PhysicalCode.Split(':')[1]);

            int channelId = Convert.ToInt32(changechannelinfo.inputchannel);

            string resourcecode = changechannelinfo.ResourceCode;


            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port);  //临时方案
            OnorOFFResponse res = SwitchChannel(channelId, resourcecode, ie);
            //if (SingletonInfo.GetInstance().PhysicalCode2IPDic.ContainsKey(changechannelinfo.PhysicalCode))
            //{
            //    ie = SingletonInfo.GetInstance().PhysicalCode2IPDic[changechannelinfo.PhysicalCode];
            //          OnorOFFResponse res = SwitchChannel(1,);
            //}
        }
        #endregion


        private OnorOFFResponse SwitchChannel(int channelID, string ResourceCode, IPEndPoint ie)
        {
            OnorOFFBroadcast tt = new OnorOFFBroadcast();
            tt.ebm_class = "4";
            tt.ebm_id = BBSHelper.CreateEBM_ID();
            tt.ebm_level = "2";
            tt.ebm_type = "00000";
            tt.end_time = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
            tt.start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tt.power_switch = "3";// 1开播  2停播   3切换通道
            tt.volume = "80";
            tt.resource_code_type = "1";
            tt.resource_codeList = new List<string>();
            tt.resource_codeList.Add(ResourceCode);
            tt.input_channel_id = channelID;

            OnorOFFResponse resopnse = (OnorOFFResponse)SingletonInfo.GetInstance().tcpsend.SendTCPCommnand(tt, 0x04, ie);
            return resopnse;

        }
        
    }
}
