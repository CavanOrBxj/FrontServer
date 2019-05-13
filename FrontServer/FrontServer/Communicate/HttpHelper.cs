using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace FrontServer
{
    public class HttpHelper
    {

        public object PostCommnand( object o, string requesttype)
        {
            string sReturnString="";
            string paraUrlCoded = "";
            string strURL = "";
            object reback = new object();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            switch (requesttype)
            {
                case "播放":
                    SendPlayInfo playInfo = (SendPlayInfo)o;
                    string id = "";


                    paraUrlCoded = "pidValue";
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode(SingletonInfo.GetInstance().pid);
                    paraUrlCoded += "&" + System.Web.HttpUtility.UrlEncode("organization_id");
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode(id);

                    paraUrlCoded += "&" + System.Web.HttpUtility.UrlEncode("broadcastType");
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode(playInfo.broadcastType);

                    paraUrlCoded += "&" + System.Web.HttpUtility.UrlEncode("creditCode");
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode("123");
                    strURL = SingletonInfo.GetInstance().HttpServer + "broadcast/program/play.htm";
                    sReturnString = SendHttpData(strURL, paraUrlCoded);
                   // if (sReturnString!="")
                   if(true)
                    {
                        Generalresponse  response = Serializer.Deserialize<Generalresponse>(sReturnString);
                        reback = response;
                    }
                    break;
                case "直播列表":
                    strURL = SingletonInfo.GetInstance().HttpServer + "broadcast/program/record.htm";
                    paraUrlCoded = "creditCode";
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode("123");
                    sReturnString = SendHttpData(strURL, paraUrlCoded);
                    if (sReturnString != "")
                    {
                        broadcastrecord response = Serializer.Deserialize<broadcastrecord>(sReturnString);
                        reback = response;
                    }
                    break;

                case "停止":

                    List<string> Stop_id_List = (List<string>)o;
                    string stopid = "";
                    foreach (var item in Stop_id_List)
                    {
                        stopid += "," + item;
                    }

                    stopid = stopid.Substring(1, stopid.Length - 1);


                    paraUrlCoded = "creditCode";
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode("123");
                    paraUrlCoded += "&" + System.Web.HttpUtility.UrlEncode("prlId");
                    paraUrlCoded += "=" + System.Web.HttpUtility.UrlEncode(stopid);
                    strURL = SingletonInfo.GetInstance().HttpServer + "broadcast/program/stop.htm";
                    sReturnString = SendHttpData(strURL, paraUrlCoded);
                    if (sReturnString != "")
                    {
                        Generalresponse response = Serializer.Deserialize<Generalresponse>(sReturnString);
                        reback = response;
                    }
                    break;
            }
            return reback;
        }

        /// <summary>
        /// Http同步接收接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public string SendHttpData(string url,string para)
        {
            try
            {
                string strURL = url;
                System.Net.HttpWebRequest request;
                request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
                //Post请求方式
                request.Method = "POST";
                // 内容类型
                request.ContentType = "application/x-www-form-urlencoded";

                // 参数经过URL编码
                string paraUrlCoded = para;
                byte[] payload;
                //将URL编码后的字符串转化为字节
                payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
                //设置请求的 ContentLength 
                request.ContentLength = payload.Length;
                //获得请 求流
                System.IO.Stream writer = request.GetRequestStream();
                //将请求参数写入流
                writer.Write(payload, 0, payload.Length);
                // 关闭请求流
                writer.Close();

                System.Net.HttpWebResponse response;
                // 获得响应流
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string responseText = myreader.ReadToEnd();
                myreader.Close();
                return responseText;
            }
            catch (Exception)
            {
                return "";
            }
           
        }
    }
}
