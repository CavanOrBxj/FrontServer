using FrontServer.Enums;
using FrontServer.StructClass;
using FrontServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FrontServer
{
    public class TcpHelper
    {

        public delegate void MyDelegate(object data);

        public static event MyDelegate MyEvent; //注意须关键字 static  


        public Socket newclient;
        public Thread myThread;
        public object  SendTCPCommnand( object o, OrderType protocol_type,IPEndPoint ie)
        {
            byte[] backdata = null;
            object rebackobj = new object();
            switch (protocol_type)
            {
                case OrderType.TSSpi:
                    break;
                case OrderType.RDDSpi:
                    RDSScanFreInfo rds = (RDSScanFreInfo)o;
                    backdata = RDSScanFreInfoCommnand(rds);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;
                case OrderType.IPSpi:
                    break;
                case OrderType.OnorOFFBroadcast:
                    OnorOFFBroadcast onoroff = (OnorOFFBroadcast)o;
                    backdata = OnorOFFBroadcastCommnand(onoroff);
                    rebackobj = DealOnorOFFResponse(SendTcpData(backdata, ie)); 
                    break;
                case OrderType.UpdateArea:
                    List<ResourceCodeInfo> rciList = (List<ResourceCodeInfo>)o;
                    backdata = ResourceCodeSetCommnand(rciList);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;
                case OrderType.VolumeSet:
                    GeneralVolumn genv = (GeneralVolumn)o;
                    backdata = GeneralVolumnCommnand(genv);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;
                case OrderType.GRPSI:
                    RebackSet rbs = (RebackSet)o;
                    backdata = RebackSetCommnand(rbs);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;
                case OrderType.GPRQI:
                    StatusInquiry si = (StatusInquiry)o;
                    backdata = StatusInquiryCommnand(si);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;
                case OrderType.UCCI:
                    ClockCalibration ucci = (ClockCalibration)o;
                    backdata= ClockCalibrationCommnand(ucci);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;

                case OrderType.GRCS:
                    RebackPeriod rp = (RebackPeriod)o;
                    backdata = RebackPeriodCommnand(rp);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;

                case OrderType.WIU:
                    WhiteListInfo wli = (WhiteListInfo)o;
                    backdata = WhiteListInfoCommnand(wli);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;

                case OrderType.OCQ:
                    OutChannelInquire oci = (OutChannelInquire)o;
                    backdata = OutChannelInquireCommnand(oci);
                    rebackobj = DealOutChannelResponse(SendTcpData(backdata, ie));

                    #region 数据库处理
                    InsertOrUpdateOutputChannel((OutChannelResponse)rebackobj);
                    #endregion
                    break;


                case OrderType.ICQ:
                    InputChannelInquire ici = (InputChannelInquire)o;
                    backdata = InputChannelInquireCommnand(ici);  
                    rebackobj = DealInputChannelResponse(SendTcpData(backdata, ie));

                    #region 数据库处理
                    InsertOrUpdateInputChannel((InputChannelResponse)rebackobj);
                    #endregion
                    break;

                case OrderType.RPS:
                    RebackParam rparam = (RebackParam)o;
                    backdata = RebackParamCommnand(rparam);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;

                case OrderType.TerminalSwitch:
                    List<OutSwitch_> outs = (List<OutSwitch_>)o;
                    backdata = PowerAmplifierSwitchCommnand(outs);
                    rebackobj = GeneralResponse(SendTcpData(backdata, ie));
                    break;
            }
            return rebackobj;
        }

        #region 数据库处理
        public void InsertOrUpdateInputChannel(InputChannelResponse input)
        {
            foreach (InputChannel item in input.InputChannelList)
            {
                string sqlstr = string.Format("INSERT INTO In_Out_Channel(channel_class, channel_id, input_channel_name, input_channel_group,input_channel_state) VALUES('{0}', '{1}', '{2}', '{3}','{4}')","0",item.input_channel_id,item.input_channel_name,item.input_channel_group,item.input_channel_state); ;
                DbHelperSQL.ExecuteSql(sqlstr);
            }
        }

        public void InsertOrUpdateOutputChannel(OutChannelResponse input)
        {
            foreach (OutChannel item in input.OutChannelList)
            {
                string sqlstr = string.Format("INSERT INTO In_Out_Channel(channel_class, channel_id, out_channel_type) VALUES('{0}', '{1}', '{2}')", "1", item.output_channel_id, item.out_channel_type); 
                DbHelperSQL.ExecuteSql(sqlstr);
            }
        }

        #endregion

        #region 解析方法
        public OnorOFFResponse DealOnorOFFResponse(byte[] inputdata)
        {
            OnorOFFResponse tmp = new OnorOFFResponse(); ;
            if (inputdata == null || inputdata.Length <= 0)
            {
                LogHelper.WriteLog(typeof(TcpHelper), "适配器数据返回异常", "Debug");
            }
            else
            {
                tmp.front_code= bcd2Str(inputdata.Skip(9).Take(12).ToArray()).Substring(1); 
                tmp.ebm_id= bcd2Str(inputdata.Skip(21).Take(18).ToArray()).Substring(1);
                tmp.result_code = inputdata.Skip(39).Take(1).ToArray()[0];
                tmp.result_desc_length = GetDataLenth(inputdata.Skip(40).Take(4).ToArray());

                tmp.result_desc = System.Text.Encoding.UTF8.GetString(inputdata.Skip(44).Take(tmp.result_desc_length).ToArray());
                tmp.accept_stream_address_length = Byte2int(inputdata.Skip(44+ tmp.result_desc_length).Take(2).ToArray());
                tmp.accept_stream_address = System.Text.Encoding.UTF8.GetString(inputdata.Skip(44 + tmp.result_desc_length + 2).Take(tmp.accept_stream_address_length).ToArray()); 
            }
            return tmp;
        }

        public GeneralResponse GeneralResponse(byte[] inputdata)
        {
            GeneralResponse tmp = new GeneralResponse(); ;
            if (inputdata == null || inputdata.Length <= 0)
            {
                LogHelper.WriteLog(typeof(TcpHelper), "适配器数据返回异常", "Debug");
            }
            else
            {
               // tmp.return_code = GetDataLenth(inputdata.Skip(9).Take(4).ToArray()).ToString(); 
              //  tmp.return_data= System.Text.Encoding.UTF8.GetString(inputdata.Skip(17).Take(tmp.return_data_length).ToArray());
            }
            return tmp;
        }


        public InputChannelResponse DealInputChannelResponse(byte[] inputdata)
        {
            InputChannelResponse tmp = new InputChannelResponse(); 
            if (inputdata == null || inputdata.Length <= 0)
            {
                LogHelper.WriteLog(typeof(TcpHelper), "适配器数据返回异常", "Debug");
            }
            else
            {
                string str = "";
                foreach (byte item in inputdata)
                {
                    str += item.ToString("X2")+" ";
                }
                tmp.front_code = bcd2Str(inputdata.Skip(9).Take(12).ToArray()).Substring(1);

                int input_channel_number = inputdata.Skip(21).Take(1).ToArray()[0];
                tmp.InputChannelList = new List<InputChannel>();
                int interval = 0;

                for (int i = 0; i < input_channel_number; i++)
                {
                    InputChannel pp = new InputChannel();
                    pp.input_channel_id = inputdata.Skip(22+ interval).Take(1).ToArray()[0].ToString();
                    int input_channel_name_length = Byte2int(inputdata.Skip(23+ interval).Take(2).ToArray());
                    pp.input_channel_name= System.Text.Encoding.Default.GetString(inputdata.Skip(25+ interval).Take(input_channel_name_length).ToArray());
                    pp.input_channel_group = inputdata.Skip(25 + input_channel_name_length+ interval).Take(1).ToArray()[0].ToString();
                    pp.input_channel_state = inputdata.Skip(25 + input_channel_name_length+1+ interval).Take(1).ToArray()[0].ToString();
                    tmp.InputChannelList.Add(pp);

                    interval += 1+2+ input_channel_name_length+1+1;
                }
                
            }
            return tmp;
        }


        public OutChannelResponse DealOutChannelResponse(byte[] inputdata)
        {
            OutChannelResponse tmp = new OutChannelResponse();
            if (inputdata == null || inputdata.Length <= 0)
            {
                LogHelper.WriteLog(typeof(TcpHelper), "适配器数据返回异常", "Debug");
            }
            else
            {
                string str = "";
                foreach (byte item in inputdata)
                {
                    str += item.ToString("X2") + " ";
                }
                tmp.front_code = bcd2Str(inputdata.Skip(9).Take(12).ToArray()).Substring(1);

                int output_channel_number = inputdata.Skip(21).Take(1).ToArray()[0];
                tmp.OutChannelList = new List<OutChannel>();
                int interval = 0;

                for (int i = 0; i < output_channel_number; i++)
                {
                    OutChannel pp = new OutChannel();
                    pp.output_channel_id = inputdata.Skip(22+ interval).Take(1).ToArray()[0].ToString();
                    pp.out_channel_type= inputdata.Skip(23+ interval).Take(1).ToArray()[0].ToString();

                    int interval2 = 0;
                    switch (pp.out_channel_type)
                    {
                        case "1":

                            int sub_channel_number = inputdata.Skip(24+ interval).Take(1).ToArray()[0];
                            pp.sub_channel_Info_1List = new List<sub_channel_Info_1>();
                            int interval3 = 0;

                            for (int j = 0; j < sub_channel_number; j++)
                            {
                                sub_channel_Info_1 sci1 = new sub_channel_Info_1();
                                sci1.sub_channel_freq= bcd2Str(inputdata.Skip(25+ interval).Take(3).ToArray());
                                sci1.output_channel_state = inputdata.Skip(28+ interval).Take(1).ToArray()[0].ToString();

                                if (sci1.output_channel_state == "2")
                                {
                                    sci1.ebm_id = bcd2Str(inputdata.Skip(29 + interval).Take(18).ToArray()).Substring(1);
                                    interval3 += 22;
                                }
                                else
                                {
                                    interval3 += 4;
                                }

                                pp.sub_channel_Info_1List.Add(sci1);

                               
                            }
                            interval2 += 1 + interval3;
                            break;

                        case "2":
                        case "3":
                            int sub_channel_number2or3 = inputdata.Skip(24+ interval).Take(1).ToArray()[0];
                            pp.sub_channel_Info_2or3List = new List<sub_channel_Info_2or3>();

                            int interval4 = 0;

                            for (int l = 0; l < sub_channel_number2or3; l++)
                            {
                                sub_channel_Info_2or3 sci2or3 = new sub_channel_Info_2or3();
                                sci2or3.original_network_id = Byte2int(inputdata.Skip(25+ interval).Take(2).ToArray()).ToString();
                                sci2or3.details_channel_transport_stream_id = Byte2int(inputdata.Skip(27+ interval).Take(2).ToArray()).ToString();
                                sci2or3.details_channel_program_number = Byte2int(inputdata.Skip(29+ interval).Take(2).ToArray()).ToString();
                                sci2or3.details_channel_pcr_pid = Byte2int(inputdata.Skip(31+ interval).Take(2).ToArray()).ToString();


                                sci2or3.streamList = new List<stream>();

                                int stream_number = inputdata.Skip(33+ interval).Take(1).ToArray()[0];

                                int interval5 = 0;
                                for (int h = 0; h < stream_number; h++)
                                {
                                    stream streamtmp = new stream();
                                    streamtmp.stream_type = inputdata.Skip(34+ interval).Take(1).ToArray()[0].ToString();
                                    streamtmp.elementary_pid= Byte2int(inputdata.Skip(35+ interval).Take(2).ToArray()).ToString() ;

                                    sci2or3.streamList.Add(streamtmp);

                                    interval5 += 3;
                                }
                                sci2or3.output_channel_state = inputdata.Skip(37 + interval + interval5).Take(1).ToArray()[0].ToString();

                                if (sci2or3.output_channel_state== "2")
                                {
                                    sci2or3.ebm_id = bcd2Str(inputdata.Skip(38 + interval+ interval5).Take(18).ToArray()).Substring(1);
                                    interval4 += 9 + interval5 + 1+18;
                                }
                                else
                                {
                                    interval4 += 9 + interval5+1;
                                }

                                pp.sub_channel_Info_2or3List.Add(sci2or3);
                            }
                            interval2 += 1 + interval4;
                            break;

                       
                    }

                    interval += interval2+2;

                    tmp.OutChannelList.Add(pp);
                }

            }
            return tmp;
        }

        #endregion

        //public byte[] SendTCPCommnand(string inputdata)
        //{
        //    return SendTcpData(str2byte(inputdata));
        //}

        /// <summary>
        /// 改成同步接口 tcp发送接收数据
        /// </summary>
        /// <param name="inputdata"></param>
        public byte[] SendTcpData(byte[] inputdata,IPEndPoint ie)
        {
            Connect(ie);
            if (inputdata != null)
            {
                newclient.Send(inputdata);
            }
            
            return   ReceiveMsg();
        }

        /// <summary>
        /// 初始化TCP发送套接字
        /// </summary>
        public void Connect(IPEndPoint ie)
        {
            try
            {
                newclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newclient.Connect(ie);
            }
            catch (SocketException ex)
            {
                LogHelper.WriteLog(typeof(TcpHelper), "TCP客户端初始化失败", "Debug");
                return;
            }
         
        }

        /// <summary>
        /// TCP客户端的数据接收  与TCP服务端注意区分
        /// </summary>
        public byte[] ReceiveMsg()
        {
            try
            {
                byte[] data = new byte[1024];
                int recv = 0;
                int cyclecount=5;
                while (true)
                {
                    Thread.Sleep(100);
                    recv = newclient.Receive(data);

                    if (recv > 0)
                    {
                        data= data.Take(recv).ToArray();
                        break;
                    }
                    if (cyclecount > 0)
                    {

                        cyclecount--;
                    }
                    else
                    {
                        break;
                    }
                }
                newclient.Close();
                //  myThread.Abort();

                string datastr = DateTime.Now.ToString() + "->";
                foreach (byte item in data.ToArray())
                {
                    datastr += item.ToString("X2").PadLeft(2, '0').ToUpper() + " ";
                }
                PrintData pp = new PrintData();
                pp.source = "4";
                pp.MessageInfo = datastr;
                TcpHelper.MyEvent(pp);

                return data;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TcpHelper), "TCP数据回馈接收失败", "Debug");
                return null;

            }

        }


        public byte[] IPstr2byte(string input)
        {
            List<byte> tmp = new List<byte>();
           string[] ipp= input.Split('.');
            foreach (string item in ipp)
            {
                tmp.Add((byte)Convert.ToInt32(item));
            }
            return tmp.ToArray();
        }

        /// <summary>
        /// 把 FF AA DD AA 的字符串转byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] str2byte(string input)
        {
            string[] qwe = input.Split(' ');
            List<byte> bb = new List<byte>();

            foreach (var item in qwe)
            {
                bb.Add((byte)Convert.ToInt32(item, 16));
            }
            return bb.ToArray();
        }

        /// <summary>
        /// 把int32转4字节byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] Int32toByte(Int32 input)
        {

            byte[] bData = BitConverter.GetBytes(input);
            Array.Reverse(bData);

            return bData;
        }

        /// <summary>
        /// 把Int值转化成两个字节的byte[]  高位在前
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] Int2ByteArray(int data)
        {
            string str = Convert.ToString(data, 16).PadLeft(4, '0');
            List<byte> Array = new List<byte>();
            for (int i = 0; i < 2; i++)
            {

                Array.Add((byte)(Convert.ToInt32(str.Substring(0 + 2 * i, 2), 16)));

            }
            return Array.ToArray();
        }

        /// <summary>
        /// 把string转byte[] 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] BCD2Byte(string str)
        {
            List<byte> tmp = new List<byte>();

            bool check = str.Length % 2 == 0 ? true : false;//奇偶校验判断  偶数true 奇数false

            for (int i = 0; i < str.Length / 2; i++)
            {
                tmp.Add((byte)Convert.ToInt32(str.Substring(i * 2, 2), 16));
            }

            if (!check)
            {
                string last = str.Substring(str.Length - 1, 1);
                tmp.Add((byte)Convert.ToInt32(last));
            }
            return tmp.ToArray();

        }

        /// <summary>
        /// 把string转ascii  "00000"    byte[] tt=byte[]{48, 48, 48, 48, 48} 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] Str2ASCII(string str)
        {
            //List<byte> tmp = new List<byte>();
            //for (int i = 0; i < str.Length; i++)
            //{
            //    tmp.Add(System.Text.Encoding.ASCII.GetBytes(str.Substring(i, 1))[0]) ; 
            //}

            //return tmp.ToArray();

            return Encoding.Default.GetBytes(str);
        }


        /// <summary>
        /// 把4字节的byte[] 转int 高位在前
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private int GetDataLenth(byte[] input)
        {
            Array.Reverse(input);
            return BitConverter.ToInt32(input, 0);
        }


        /// <summary>
        /// BCD码转为10进制串(阿拉伯数据)   不支持0开头
        /// </summary>
        /// <param name="bytes">BCD码 </param>
        /// <returns>10进制串 </returns>
        public String bcd2Str(byte[] bytes)
        {
            string str = "";
            foreach (byte item in bytes)
            {
                str += Convert.ToString(item, 16).PadLeft(2, '0');
            }
            return str;
        }


        /// <summary>
        /// DateTime string 转4字节byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] DatetimeStr2Byte()
        {

            DateTime dd = DateTime.UtcNow;
            DateTime mmmm = DateTime.Parse(DateTime.Now.ToString("1970-01-01 00:00:00"));
            int sec = (int)(dd.Subtract(mmmm).TotalSeconds);

            byte[] time = BitConverter.GetBytes(sec);

            Array.Reverse(time);

            return time;
        }


        /// <summary>
        /// 把 2字节的byte[] 转int  高位在前 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int Byte2int(byte[] input)
        {
            string strlen = "";
            foreach (byte item in input)
            {
                strlen += Convert.ToString(item, 16).PadLeft(2, '0');
            }
            int datalen = Convert.ToInt32(strlen, 16);

            return datalen;

        }
        /// <summary>
        /// 产生头部数据
        /// </summary>
        /// <param name="protocoltype"></param>
        /// <returns></returns>
        public byte[] Headdata(byte protocoltype)
        {
            List<byte> headdata = new List<byte>();
            headdata.Add(0x49);
            headdata.Add(0);
            headdata.Add(0x01);
            headdata.Add(protocoltype);
            headdata.Add(0x01);

            return headdata.ToArray();
        }

        public byte[] Buidsenddata(List<byte> obj,byte protocoltype)
        {
            List<byte> senddata = new List<byte>();
            senddata.AddRange(Headdata(protocoltype));
            senddata.AddRange(Int32toByte(obj.Count));
            senddata.AddRange(obj);

            //接下去要计算签名信息
            int signaturelen = 4 + 6 + 64;
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int random = (Int32)Convert.ToInt64(ts.TotalSeconds);
            byte[] SN = new byte[6] { 0, 0, 0, 0, 0, 1 };//一号证书
            byte[] Signaturedata = new byte[74];
            Calcle.SignatureFunc(senddata.ToArray(), senddata.Count, ref random, ref Signaturedata);
            senddata.AddRange(Int2ByteArray(signaturelen));
            senddata.AddRange(Signaturedata.Take(4).ToArray());
            if (SingletonInfo.GetInstance().SignatureType)
            {
                //软签名
                senddata.AddRange(SN);
            }
            else
            {
                //硬件签名
                senddata.AddRange(Signaturedata.Skip(4).Take(6).ToArray());
            }
          
            senddata.AddRange(Signaturedata.Skip(10).Take(64).ToArray());

            byte[] crcdata = CRC32.GetCRC32(senddata.ToArray());
            senddata.AddRange(crcdata);

            string data =DateTime.Now.ToString()+"->";
            foreach (byte item in senddata.ToArray())
            {
                data += item.ToString("X2").PadLeft(2, '0').ToUpper() + " ";
            }
            PrintData pp = new PrintData();
            pp.source = "3";
            pp.MessageInfo = data;
            TcpHelper.MyEvent(pp);
            return senddata.ToArray();
        }

        /// <summary>
        /// 创建开关数据流
        /// </summary>
        /// <param name="onoroff"></param>
        /// <returns></returns>
        public byte[] OnorOFFBroadcastCommnand(OnorOFFBroadcast onoroff)
        {

            List<byte> classdata = new List<byte>();

            classdata.AddRange(BCD2Byte("F" + onoroff.ebm_id));

            classdata.Add((byte)Convert.ToInt32(onoroff.power_switch));

            if (onoroff.ebm_class != null)
            {
                //加特殊处理  开和关  需要增加的信息不同
                //开机指令

                classdata.Add((byte)Convert.ToInt32(onoroff.ebm_class.Substring(3)));

                classdata.AddRange(Str2ASCII(onoroff.ebm_type));

                classdata.Add((byte)Convert.ToInt32(onoroff.ebm_level.Substring(1)));

                classdata.AddRange(Int32toByte((Int32)(Convert.ToDateTime(onoroff.start_time).AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
               // classdata.AddRange(Int32toByte(0));//测试

                classdata.AddRange(Int32toByte((Int32)(Convert.ToDateTime(onoroff.end_time).AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));

                classdata.Add((byte)Convert.ToInt32(onoroff.volume));


                classdata.Add((byte)Convert.ToInt32(onoroff.resource_code_type));

                int resource_code_number = onoroff.resource_codeList.Count;

                classdata.Add((byte)resource_code_number);


                int resource_code_length = 0;
                if (resource_code_number > 0)
                {
                    byte[] dsadas = BCD2Byte("F" + onoroff.resource_codeList[0]);//特别注意 这个resource_code_length是指所占字节的长度  并非实际字符长度
                    resource_code_length = dsadas.Length;
                }

                classdata.Add((byte)resource_code_length);
                foreach (string resourcecode in onoroff.resource_codeList)
                {
                    classdata.AddRange(BCD2Byte("F" + resourcecode));
                }


                int multilingual_content_number = 0;
                if (onoroff.multilingual_contentList.Count != 0)
                {
                    multilingual_content_number = onoroff.multilingual_contentList.Count;
                }

                classdata.Add((byte)multilingual_content_number);


                if (onoroff.multilingual_contentList.Count != 0)
                {
                    foreach (MultilingualContentInfo item in onoroff.multilingual_contentList)
                    {
                        classdata.AddRange(Str2ASCII(item.language_code));
                        classdata.Add((byte)Convert.ToInt32(item.coded_character_set));
                        byte[] text_char = Str2ASCII(item.text_char);
                        classdata.AddRange(Int2ByteArray(item.text_char.Length));
                        classdata.AddRange(text_char);
                        byte[] agency_name_char = Str2ASCII(item.agency_name_char);
                        classdata.Add((byte)Convert.ToInt32(agency_name_char.Length));
                        classdata.AddRange(agency_name_char);

                        int auxiliary_number = 0;
                        if (item.AuxiliaryInfoList != null)
                        {
                            auxiliary_number = item.AuxiliaryInfoList.Count;
                        }
                        classdata.Add((byte)auxiliary_number);

                        if (item.AuxiliaryInfoList.Count != 0)
                        {
                            foreach (AuxiliaryInfo auxiliary in item.AuxiliaryInfoList)
                            {
                                classdata.Add((byte)Convert.ToInt32(auxiliary.auxiliary_data_type));
                                byte[] auxiliary_data = System.Text.Encoding.ASCII.GetBytes(auxiliary.auxiliary_data); //Str2ASCII(auxiliary.auxiliary_data);
                                classdata.AddRange(Int32toByte(auxiliary_data.Length));
                                classdata.AddRange(auxiliary_data);
                            }
                        }

                    }
                }
                classdata.Add((byte)onoroff.input_channel_id);
                int output_channel_number = 0;
                if (onoroff.OutPut_Channel_IdList != null)
                {
                    output_channel_number = onoroff.OutPut_Channel_IdList.Count;

                }
                classdata.Add((byte)output_channel_number);

                if (onoroff.OutPut_Channel_IdList != null)
                {
                    foreach (int output_channel_id in onoroff.OutPut_Channel_IdList)
                    {
                        classdata.Add((byte)output_channel_id);
                    }
                }
                #region private_data_length  

                classdata.AddRange(Int2ByteArray(0));//应该改为2个字节长度  
               // classdata.AddRange(Int2ByteArray(Convert.ToInt32(onoroff.S_elementary_PID)));
                #endregion
            }
            else
            {
                //关机指令

                classdata.Add((byte)Convert.ToInt32(onoroff.resource_code_type));

                int resource_code_number = onoroff.resource_codeList.Count;

                classdata.Add((byte)resource_code_number);


                int resource_code_length = 0;
                if (resource_code_number > 0)
                {
                    byte[] dsadas = BCD2Byte("F" + onoroff.resource_codeList[0]);//特别注意 这个resource_code_length是指所占字节的长度  并非实际字符长度
                    resource_code_length = dsadas.Length;
                }

                classdata.Add((byte)resource_code_length);
                foreach (string resourcecode in onoroff.resource_codeList)
                {
                    classdata.AddRange(BCD2Byte("F" + resourcecode));
                }
            }
            
            return Buidsenddata(classdata, 0x04);
        }

        /// <summary>
        /// 创建时钟校准数据流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] ClockCalibrationCommnand(ClockCalibration obj)
        {
            List<byte> classdata = new List<byte>();

            classdata.AddRange(Int32toByte((Int32)(Convert.ToDateTime(obj.time).AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
           
            return Buidsenddata(classdata, 0x09);
        }

        public byte[] GeneralVolumnCommnand(GeneralVolumn obj)
        {
            List<byte> classdata = new List<byte>();
            classdata.Add((byte)Convert.ToInt32(obj.volume));
            classdata.Add((byte)Convert.ToInt32(obj.resource_code_type));
            classdata.Add((byte)obj.resource_codeList.Count);

            int resource_code_length = 0;
            if (obj.resource_codeList.Count > 0)
            {
                byte[] dsadas = BCD2Byte("F" + obj.resource_codeList[0]);//特别注意 这个resource_code_length是指所占字节的长度  并非实际字符长度
                resource_code_length = dsadas.Length;
            }

            classdata.Add((byte)resource_code_length);
            foreach (string resourcecode in obj.resource_codeList)
            {
                classdata.AddRange(BCD2Byte("F" + resourcecode));
            }
            
            return Buidsenddata(classdata, 0x06);
        }

        public byte[] ResourceCodeSetCommnand(List<ResourceCodeInfo> rciList)
        {

            List<byte> classdata = new List<byte>();
            classdata.Add((byte)rciList.Count);
            foreach (ResourceCodeInfo item in rciList)
            {
                byte[] physical = BCD2Byte(item.physical_address);
                int length = physical.Length;
                classdata.Add((byte)length);
                classdata.AddRange(physical);

                byte[] logic_address = BCD2Byte("F"+item.logic_address);
                int length2 = 12;
                classdata.Add((byte)length2);
                classdata.AddRange(logic_address);

            }
            return Buidsenddata(classdata, 0x05);
        }

        public byte[] RebackSetCommnand(RebackSet obj)
        {

            List<byte> classdata = new List<byte>();
            classdata.Add((byte)Convert.ToInt32(obj.reback_type));

            switch (obj.reback_type)
            {
                case "1":
                    //11位手机
                    classdata.Add(11);//长度
                    foreach (char  item in obj.reback_address)
                    {
                        classdata.Add((byte)Convert.ToInt32(item));
                    }
                    break;
                case "2":
                    //IP 地址和端口
                    classdata.Add(6);//长度
                    string iptmp = obj.reback_address.Split(':')[0];
                    string porttmp = obj.reback_address.Split(':')[1];
                    classdata.AddRange(IPstr2byte(iptmp));
                    classdata.AddRange(Int2ByteArray(Convert.ToInt32(porttmp)));
                    break;
                case "3":
                    //域名+“：”+端口号
                    byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(obj.reback_address);
                    classdata.Add((byte)byteArray.Length);
                    classdata.AddRange(byteArray);
                  
                    break;

            }

            classdata.Add((byte)Convert.ToInt32(obj.resource_code_type));
            int resource_code_number = obj.resource_codeList.Count;

            classdata.Add((byte)resource_code_number);


            int resource_code_length = 0;
            if (resource_code_number > 0)
            {
                byte[] dsadas = BCD2Byte("F" + obj.resource_codeList[0]);//特别注意 这个resource_code_length是指所占字节的长度  并非实际字符长度
                resource_code_length = dsadas.Length;
            }

            classdata.Add((byte)resource_code_length);
            foreach (string resourcecode in obj.resource_codeList)
            {
                classdata.AddRange(BCD2Byte("F" + resourcecode));
            }

            return Buidsenddata(classdata, 0x07);
        }

        public byte[] StatusInquiryCommnand(StatusInquiry obj)
        {

            List<byte> classdata = new List<byte>();
            classdata.Add((byte)Convert.ToInt32(obj.reback_type));

            switch (obj.reback_type)
            {
                case "1":
                    //11位手机
                    classdata.Add(11);//长度
                    foreach (char item in obj.reback_address)
                    {
                        classdata.Add((byte)Convert.ToInt32(item));
                    }
                    break;
                case "2":
                    //IP 地址和端口
                    classdata.Add(6);//长度
                    string iptmp = obj.reback_address.Split(':')[0];
                    string porttmp = obj.reback_address.Split(':')[1];
                    classdata.AddRange(IPstr2byte(iptmp));
                    classdata.AddRange(Int2ByteArray(Convert.ToInt32(porttmp)));
                    break;
                case "3":
                    //域名+“：”+端口号
                    byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(obj.reback_address);
                    classdata.Add((byte)byteArray.Length);
                    classdata.AddRange(byteArray);

                    break;

            }

            classdata.Add((byte)Convert.ToInt32(obj.resource_code_type));
            int resource_code_number = obj.resource_codeList.Count;

            classdata.Add((byte)resource_code_number);


            int resource_code_length = 0;
            if (resource_code_number > 0)
            {
                byte[] dsadas = BCD2Byte("F" + obj.resource_codeList[0]);//特别注意 这个resource_code_length是指所占字节的长度  并非实际字符长度
                resource_code_length = dsadas.Length;
            }

            classdata.Add((byte)resource_code_length);
            foreach (string resourcecode in obj.resource_codeList)
            {
                classdata.AddRange(BCD2Byte("F" + resourcecode));
            }

            int ruery_code_number = obj.query_code_codeList.Count;
            classdata.Add((byte)ruery_code_number);
            foreach (string query_code in obj.query_code_codeList)
            {
                classdata.Add((byte)Convert.ToInt32(query_code,16));
            }
            return Buidsenddata(classdata, 0x08);
        }

        public byte[] RebackPeriodCommnand(RebackPeriod obj)
        {
            List<byte> classdata = new List<byte>();

            classdata.AddRange(Int32toByte(Convert.ToInt32(obj.reback_cycle)));
            classdata.Add((byte)Convert.ToInt32(obj.resource_code_type));
            int resource_code_number = obj.resource_codeList.Count;

            classdata.Add((byte)resource_code_number);


            int resource_code_length = 0;
            if (resource_code_number > 0)
            {
                byte[] dsadas = BCD2Byte("F" + obj.resource_codeList[0]);//特别注意 这个resource_code_length是指所占字节的长度  并非实际字符长度
                resource_code_length = dsadas.Length;
            }

            classdata.Add((byte)resource_code_length);
            foreach (string resourcecode in obj.resource_codeList)
            {
                classdata.AddRange(BCD2Byte("F" + resourcecode));
            }
            return Buidsenddata(classdata, 0x0B);
        }

        public byte[] WhiteListInfoCommnand(WhiteListInfo obj)
        {
            List<byte> classdata = new List<byte>();
            classdata.Add((byte)obj.WhiteListsList.Count);
            foreach (WhiteList item in obj.WhiteListsList)
            {
                classdata.Add((byte)Convert.ToInt32(item.oper_type));
                int phone_number_length = item.phone_number.Length;
                classdata.Add((byte)phone_number_length);
                classdata.AddRange(System.Text.Encoding.ASCII.GetBytes(item.phone_number));
                int user_name_length = System.Text.Encoding.UTF8.GetBytes(item.user_name).Length;
                classdata.Add((byte)user_name_length);
                classdata.AddRange(System.Text.Encoding.UTF8.GetBytes(item.user_name));
                classdata.Add( (byte)Convert.ToInt32(item.permission_type) );

                int permission_area_number = item.permission_area_codeList.Count;
                classdata.Add((byte)permission_area_number);

                classdata.Add(6);
                foreach (string i in item.permission_area_codeList)
                {
                    classdata.AddRange(BCD2Byte(i));
                }
            }
            return Buidsenddata(classdata, 0x0C);
        }

        public byte[] RebackParamCommnand(RebackParam obj)
        {
            List<byte> classdata = new List<byte>();
            classdata.Add((byte)Convert.ToInt32(obj.reback_type) );
            classdata.AddRange(Int32toByte(Convert.ToInt32(obj.reback_cycle)));

            string[] addresssinfo = obj.reback_address.Split(':');
            byte[] ipinfo = IPstr2byte(addresssinfo[0]);
            byte[] portinfo = Int2ByteArray(Convert.ToInt32(addresssinfo[1]));
            classdata.Add((byte)(ipinfo.Length+portinfo.Length));
            classdata.AddRange(ipinfo);
            classdata.AddRange(portinfo);
            return Buidsenddata(classdata, 0x0D);
        }

        public byte[] PowerAmplifierSwitchCommnand(List<OutSwitch_> obj)
        {
            List<byte> classdata = new List<byte>();

            classdata.Add(obj[0].Program.B_Switch_status);
            classdata.Add(1);
            int resource_code_number = obj[0].Program.list_Terminal_Address.Count;
            classdata.Add((byte)resource_code_number);
            classdata.Add(12);//resource_code_length 
            foreach (string resourcecode in obj[0].Program.list_Terminal_Address)
            {
                classdata.AddRange(BCD2Byte("F" + resourcecode));
            }
            return Buidsenddata(classdata, 0x3F);
        }


        public byte[] InputChannelInquireCommnand(InputChannelInquire obj)
        {
            List<byte> classdata = new List<byte>();
            classdata.AddRange(BCD2Byte("F" + obj.front_code));
            classdata.Add((byte)Convert.ToInt32(obj.input_channel_id));
            classdata.Add((byte)Convert.ToInt32(obj.input_channel_state));
            return Buidsenddata(classdata, 0x0F);
        }

        public byte[] OutChannelInquireCommnand(OutChannelInquire obj)
        {
            List<byte> classdata = new List<byte>();
            classdata.AddRange(BCD2Byte("F" + obj.front_code));
            classdata.Add((byte)Convert.ToInt32(obj.output_channel_id));
            classdata.Add((byte)Convert.ToInt32(obj.output_channel_state));
            return Buidsenddata(classdata, 0x0E);
        }

        public byte[] RDSScanFreInfoCommnand(RDSScanFreInfo obj)
        {
            List<byte> classdata = new List<byte>();
            classdata.Add(0);
            classdata.Add(0);
            classdata.Add(0xFF);
            classdata.Add(0x64);
            classdata.Add((byte)obj.coverag_resource_List.Count);
            foreach (string resourcecode in obj.coverag_resource_List)
            {
                string res = "";
                if (resourcecode.Length == 12)
                {
                    res = "F6" + resourcecode + "0314000000";
                }
                else
                {
                    res = resourcecode;
                }
                classdata.AddRange(BCD2Byte(res));
            }

            int data_length = 1 + 5 * obj.RDSScanFreList.Count;

            classdata.AddRange(Int32toByte(data_length));
            classdata.Add((byte)obj.RDSScanFreList.Count);//频点数
            foreach (RDSScanFre item in obj.RDSScanFreList)
            {
                classdata.Add((byte)Convert.ToInt32(item.freqCount));
                classdata.Add((byte)Convert.ToInt32(item.freqPri));
                string[] fres = item.freq.Split('.');
                if (fres.Length > 1)
                {
                  string  ww=  fres[0].PadLeft(4,'0');
                    classdata.AddRange(BCD2Byte(ww + fres[1]));
                }
                else
                {
                    string ww = fres[0].PadLeft(4, '0');
                    classdata.AddRange(BCD2Byte(ww + "00"));
                }
            }
            return Buidsenddata(classdata, 0x02);
        }
    }
}
