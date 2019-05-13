using System.Threading;
using System.Collections.Generic;
using System.Data;
using EBSignature;
using System.Net;
using FrontServer.StructClass;
using FrontServer.Utils;
using System;

namespace FrontServer
{
    public class SingletonInfo
    {
        private static SingletonInfo _singleton;

        public int scramblernum;//记录当前所使用的是哪种CA 江南天安-1 江南科友-2     内置CA-5

        public bool ischecksignature;//记录当前是否需要签名检测

        public int OpenScramblerReturn; //天安密码器打开情况   0表示成功 

        public int InlayCAType;//内置CA的类型  1表示EbMSGCASignature  2表示EbMSGPLSignature

        public bool IsUseCAInfo;//表明是否启用CA(签名)  true表示启用  false表示不启用
       

        public EbmSignature InlayCA;

        public bool IsStartSend;//是否已经启动发送

        public string cramblertype;

        public int OriginalNetworkId;//应急广播原始网络标识符 0-65535

        public bool IsGXProtocol;//表明是否是广西协议

        public bool IsUseAddCert;//是否使用增加的证书
        //public string Cert_SN;//增加的证书编号
        //public string PriKey;//增加证书的私钥
        //public string PubKey;//增加证书的公钥

    //    public int Cert_Index;//证书索引
        public string LocalHost;//本机IP
       
        public string ebm_id_front;
        public string ebm_id_behind;
        public int ebm_id_count;

        public TcpHelper tcpsend;

        public Dictionary<string,IPEndPoint> PhysicalCode2IPDic;//记录适配器硬件上传的IP及端口


        public int TimerInterval;//定时器执行周期

        public List<IndexItemIData> IndexItemList;

        public string HttpServer;
        public HttpHelper post;

        public string Interstitial_prlId;//插播prlID;
        public string pid;//httpPlayID

        public bool IsTaskUpload;//任务上报标志位
        public string PhoneBindResourceCode;//

        public Dictionary<string, int> DicCertIndex2CerSN;//增加的证书索引与证书编号的字典

        public string SQLServerIP;
        public string SQLDataBaseName;
        public string SQLUserName;
        public string SQLPassword;

        public string FrontCode;//主动发送给县适配器的FrontCode;

        public bool SignatureType;//签名方式  true表示软签名  false表示硬件签名

        public USBE usb;// = new USBE();
        public IntPtr phDeviceHandle;// = (IntPtr)1;

        private SingletonInfo()                                                                 
        {
            scramblernum = 0;
            ischecksignature = false;
            OpenScramblerReturn = 2;
            InlayCAType = 0;
            IsUseCAInfo = true; //默认启用CA
            InlayCA=new EbmSignature();
            IsStartSend = false;
            cramblertype = "";
            OriginalNetworkId = 0;//是否需要保存？   20180328

            IsGXProtocol = false;

            IsUseAddCert = false;
            LocalHost = "";
            ebm_id_front = "";
            ebm_id_behind = "";
            ebm_id_count = 0;
            PhysicalCode2IPDic = new Dictionary<string, IPEndPoint>();

            tcpsend = new TcpHelper();
            TimerInterval = 0;
            IndexItemList = new List<IndexItemIData>();
            HttpServer = "";
            post = new HttpHelper();

            Interstitial_prlId = "";
            pid = "";
            IsTaskUpload = false;
            PhoneBindResourceCode = "";

            DicCertIndex2CerSN = new Dictionary<string, int>();

            SQLPassword = "";
            SQLDataBaseName = "";
            SQLUserName = "";
            SQLPassword = "";
            FrontCode = "";

            SignatureType = true;

            usb = new USBE();
            phDeviceHandle = (IntPtr)1;

        }

        public static SingletonInfo GetInstance()
        {
            if (_singleton == null)
            {
                Interlocked.CompareExchange(ref _singleton, new SingletonInfo(), null);
            }
            return _singleton;
        }
    }
}