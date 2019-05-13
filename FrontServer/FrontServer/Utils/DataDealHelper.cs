
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace FrontServer.Utils
{
    /// <summary>
    /// 数据缓存处理
    /// </summary>
    class DataDealHelper : IDisposable
    {
     //   private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DataDealHelper));








        public void Dispose()
        {
            
        }


        //public void Serialize(string Msg)
        //{
        //    if (Msg.Contains("UPGRADE"))
        //    {
        //        UpdateCode tmp = new UpdateCode();
        //        string[] msgItem = Msg.Split('|');
        //        if (msgItem.Length > 0)
        //        {
        //            foreach (var item in msgItem)
        //            {
        //                switch (item.Split('~')[0])
        //                {
        //                    case "PACKETTYPE":
        //                        tmp.PACKETTYPE = item.Split('~')[1];
        //                        break;
        //                    case "NEWVERSION":
        //                        tmp.NEWVERSION = item.Split('~')[1];
        //                        break;
        //                    case "OLDVERSION":
        //                        tmp.OLDVERSION = item.Split('~')[1];
        //                        break;
        //                    case "UPGRADEMODE":
        //                        tmp.UPGRADEMODE = item.Split('~')[1];
        //                        break;
        //                    case "HTTPURL":
        //                        tmp.HTTPURL = item.Split('~')[1];
        //                        break;
        //                    case "LIST":
        //                        string[] str = item.Split('~')[1].Split('&');
        //                        tmp.LIST = new List<string>();
        //                        foreach (var code in str)
        //                        {
        //                            tmp.LIST.Add(code);
        //                        }
        //                        break;
        //                }
        //            }
        //            SendbackDetail sendbackDetail = new SendbackDetail();
        //            sendbackDetail.tag = Equipment.OnlineUpdate;
        //            sendbackDetail.Extras = tmp;
        //            DataDealHelper.MyEvent(sendbackDetail);
        //        }
        //    }
        //    else if (Msg.Contains("TRANSFER"))
        //    {
        //        TransferCode tmp = new TransferCode();
        //        string[] msgItem = Msg.Split('|');
        //        if (msgItem.Length > 0)
        //        {
        //            foreach (var item in msgItem)
        //            {
        //                switch (item.Split('~')[0])
        //                {
        //                    case "PACKETTYPE":
        //                        tmp.PACKETTYPE = item.Split('~')[1];
        //                        break;
        //                    case "FILENAME":
        //                        tmp.FILENAME = item.Split('~')[1];
        //                        break;
        //                    case "SRVPHYSICALCODE":
        //                        tmp.SRVPHYSICALCODE = item.Split('~')[1];
        //                        break;
        //                    case "PACKSTARTINDEX":
        //                        byte[] startIndex = new byte[4];
        //                        if (item.Split('~')[1].Length > 0)
        //                        {
        //                            for (int i = 0; i < item.Split('~')[1].Length / 2; i++)
        //                            {
        //                                startIndex[i] = (byte)Convert.ToInt32(item.Split('~')[1].Substring(2 * i, 2), 16);
        //                            }
        //                        }
        //                        tmp.PACKSTARTINDEX = startIndex;

        //                        break;
        //                    case "AUDIOREBACKSERVERIP":
        //                        tmp.AUDIOREBACKSERVERIP = item.Split('~')[1];
        //                        break;
        //                    case "AUDIOREBACKPORT":
        //                        tmp.AUDIOREBACKPORT = Convert.ToInt32(item.Split('~')[1]);
        //                        break;
        //                }
        //            }
        //            if (tmp.AUDIOREBACKSERVERIP != null && tmp.AUDIOREBACKPORT != null)
        //            {
        //                string name = Dns.GetHostName();
        //                IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
        //                foreach (var item in ipadrlist)
        //                {
        //                    if (item.ToString() == tmp.AUDIOREBACKSERVERIP)
        //                    {
        //                        OpenReceiveTool(tmp.AUDIOREBACKSERVERIP, Convert.ToInt32(tmp.AUDIOREBACKPORT));
        //                    }
        //                }


        //            }    
        //            tmp.Audio_reback_mode = 1;//1 UDP;2 TCP;3串口;其它值预留  现暂时默认设置为udp  20180129
        //            SendbackDetail sendbackDetail = new SendbackDetail();
        //            sendbackDetail.tag = Equipment.FileName;
        //            sendbackDetail.Extras = tmp;
        //            DataDealHelper.MyEvent(sendbackDetail);
        //        }
        //    }
        //    else if (Msg.Contains("TTS"))
        //    {
        //        TTSC tmp = new TTSC();
        //        string[] msgItem = Msg.Split('|');
        //        if (msgItem.Length > 0)
        //        {
        //            foreach (var item in msgItem)
        //            {
        //                switch (item.Split('~')[0])
        //                {
        //                    case "PACKETTYPE":
        //                        tmp.PACKETTYPE = item.Split('~')[1];
        //                        break;
        //                    case "TsCmd_Params":
        //                        tmp.TsCmd_Params = item.Split('~')[1];
        //                        break;
        //                    case "TsCmd_Mode":
        //                        tmp.TsCmd_Mode = item.Split('~')[1];
        //                        break;
        //                    case "TsCmd_ValueID":
        //                        tmp.DeviceIdList = new List<int>();
        //                        if(tmp.TsCmd_Mode=="区域")
        //                        {
        //                            string strAreaId = item.Split('~')[1];
        //                            string[] ids = strAreaId.Split(',');
        //                            List<int> tt = new List<int>();
        //                            for (int i = 0; i < ids.Length; i++)
        //                            {
        //                                tt.Add(Convert.ToInt32(ids[i]) );
        //                            }

        //                            tmp.DeviceIdList = db.AeraCode2DeviceID(tt);
        //                        }
        //                        else
        //                        {
        //                            string strDeviceID = item.Split('~')[1];
        //                            string[] ids = strDeviceID.Split(',');
                                  
        //                            foreach (var id in ids)
        //                            {
        //                                tmp.DeviceIdList.Add(Convert.ToInt32(id));
        //                            }
        //                        }
                                
        //                        break;
        //                    case "TsCmd_PlayCount":
        //                        tmp.TsCmd_PlayCount = Convert.ToInt32(item.Split('~')[1]);
        //                        break;
        //                }
        //            }
        //            tmp.FileID = (SingletonInfo.GetInstance().FileID + 1).ToString("X").PadLeft(4,'0');
        //            SingletonInfo.GetInstance().FileID += 1;
        //            SendbackDetail sendbackDetail = new SendbackDetail();
        //            sendbackDetail.tag = Equipment.TTS;
        //            sendbackDetail.Extras = tmp;

        //            log.Error("收到文本转语音MQ指令");
        //            DataDealHelper.MyEvent(sendbackDetail);
        //        }
        //    }
        //}
    }
}
