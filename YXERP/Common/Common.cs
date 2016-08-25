using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Qiniu.Conf;
using Qiniu.IO;
using Qiniu.RS;

namespace YXERP.Common
{
    public class Common
    {
        //云销客户端的ClientID、AgentID
        public static string YXClientID = System.Configuration.ConfigurationManager.AppSettings["YXClientID"] ?? string.Empty;
        public static string YXAgentID = System.Configuration.ConfigurationManager.AppSettings["YXAgentID"] ?? string.Empty;

        //支付宝对接页面
        public static string AlipaySuccessPage = System.Configuration.ConfigurationManager.AppSettings["AlipaySuccessPage"] ?? string.Empty;
        public static string AlipayNotifyPage = System.Configuration.ConfigurationManager.AppSettings["AlipayNotifyPage"] ?? string.Empty;

        //七牛
        public static String bucket = System.Configuration.ConfigurationManager.AppSettings["QN_Bucket"] ?? "test-yunxiaokeji";
        public static string imgurl = System.Configuration.ConfigurationManager.AppSettings["QN_ImgUrl"] ?? "http://obo9ophyw.bkt.clouddn.com/";
        public static Dictionary<string, object> QnDictionary = new Dictionary<string, object>();
        public static object QNtokent=new object();

       /// <summary>
       /// 获取请求方ip
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        public static string GetRequestIP()
        {
            return string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers.Get("X-Real-IP")) ? System.Web.HttpContext.Current.Request.UserHostAddress : System.Web.HttpContext.Current.Request.Headers["X-Real-IP"];
        }


        public static string GetXmlNodeValue(string strNodeName, string strValueName)
        {
            try
            {
                string pathurl = System.Web.HttpContext.Current.Server.MapPath("~/Common/ApiSetting.xml");
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(pathurl);
                System.Xml.XmlNode xNode = xmlDoc.SelectSingleNode("//" + strNodeName + "");
                string strValue = xNode.Attributes[strValueName].Value;
                return strValue;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        /// <summary>
        /// 写支付宝文本日志
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool WriteAlipayLog(string content) {
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"C:\WebLog\Alipay");
                //string path = HttpContext.Current.Server.MapPath("~/Log/Alipay");
                if (!Directory.Exists(path))//判断是否有该文件  
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";//生成日志文件  
                if (!File.Exists(logFileName))//判断日志文件是否为当天  
                    File.Create(logFileName);//创建文件  
                StreamWriter writer = File.AppendText(logFileName);//文件中添加文件流  
                writer.WriteLine(DateTime.Now.ToString() + " " + content);
                writer.Flush();
                writer.Dispose();
                writer.Close();
                
                return true;
            }
            catch
            {
                return false;
            };
        }

        /// <summary>
        /// 存入手机验证码会话
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        public static void SetCodeSession(string mobilePhone, string code) 
        {
            HttpContext.Current.Session[mobilePhone] = code;
        }

        /// <summary>
        /// 验证手机验证码
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool ValidateMobilePhoneCode(string mobilePhone, string code)
        {
            if (HttpContext.Current.Session[mobilePhone] != null)
            {
              return  HttpContext.Current.Session[mobilePhone].ToString() == code;
            }

            return false;
        }

        /// <summary>
        /// 清除手机验证码会话
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static void ClearMobilePhoneCode(string mobilePhone)
        {
            if (HttpContext.Current.Session[mobilePhone] != null)
            {
                 HttpContext.Current.Session.Remove(mobilePhone);
            }
        }

        public static Dictionary<string, object> GetQNToken()
        {
            lock (QNtokent)
            { 
                if (QnDictionary.ContainsKey("uptoken"))
                {
                    TimeSpan ts =
                        new TimeSpan(Convert.ToDateTime(QnDictionary["ctime"]).Ticks).Subtract(
                            new TimeSpan(DateTime.Now.Ticks)).Duration();
                    if (ts.TotalMinutes > 55)
                    {
                        Config.Init();
                        PutPolicy put = new PutPolicy(bucket, 3600);
                        QnDictionary["uptoken"] = put.Token();
                    }
                    return QnDictionary;
                }
                else
                {
                    Config.Init();
                    //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
                    PutPolicy put = new PutPolicy(bucket, 3600);
                    //调用Token()方法生成上传的Token
                    string upToken = put.Token();
                    QnDictionary.Add("uptoken", upToken);
                    QnDictionary.Add("ctime", DateTime.Now);
                    QnDictionary.Add("bucket", bucket);
                    QnDictionary.Add("imgurl", imgurl);
                }
                return QnDictionary;
            }
        } 
        public static string UploadAttachment(string filepath, string files = "orders")
        {
            string allFilePath = "";
            Config.Init();
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            Dictionary<string, object> param=GetQNToken(); 
            //调用Token()方法生成上传的Token
            string upToken = param["uptoken"].ToString();
            //上传文件的路径
            if (!string.IsNullOrEmpty(filepath))
            {
                string[] filepaths = filepath.Split(',');
                foreach (string file in filepaths)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        var fileExtension = file.Substring(file.LastIndexOf(".") + 1).ToLower();
                        var key = files + (DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "/") + GetTimeStamp() + "." + fileExtension;
                        //调用PutFile()方法上传
                        PutRet ret = target.PutFile(upToken, key, file, extra);
                        if (ret.OK)
                        {
                            allFilePath += param["imgurl"] + ret.key + ",";
                        }
                    }
                }
            }
            return allFilePath.TrimEnd(',');
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        } 
        #region 缓存

        #region 用户登录密码错误缓存
        private static Dictionary<string, PwdErrorUserEntity> _cachePwdErrorUsers;
        public static Dictionary<string, PwdErrorUserEntity> CachePwdErrorUsers
        {
            set { _cachePwdErrorUsers = value; }

            get { 

                if(_cachePwdErrorUsers==null)
                {
                    _cachePwdErrorUsers= new Dictionary<string, PwdErrorUserEntity>();
                }

                return _cachePwdErrorUsers;
            }
        }

        #endregion


        #endregion


        /// <summary>
        /// 获取URL特定参数的值
        /// </summary>
        /// <param name="qname">参数名称</param>
        /// <param name="queryString">URl</param>
        /// <returns></returns>
        public static string GetQueryString(string qname,string url)
        { 
            NameValueCollection col = GetQueryString(url);
            try
            {
                return col[qname];
            }
            catch (Exception ex)
            { 
                return "";
            }
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string url)
        {
            return GetQueryString(url, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncoded"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string url, Encoding encoding, bool isEncoded)
        {
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(url))
            {
                return result;
            }
            string queryString = url;
            int ind = url.IndexOf("?");
            if (ind > -1)
            {
                result["baseurl"] = MyUrlDeCode(url.Substring(0, ind), encoding);
                queryString = url.Substring(ind + 1, url.Length - ind-1);
            }
            
            queryString = queryString.Replace("?", ""); 
            
            if (!string.IsNullOrEmpty(queryString))
            {
                int count = queryString.Length;
                for (int i = 0; i < count; i++)
                {
                    int startIndex = i;
                    int index = -1;
                    while (i < count)
                    {
                        char item = queryString[i];
                        if (item == '=')
                        {
                            if (index < 0)
                            {
                                index = i;
                            }
                        }
                        else if (item == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (index >= 0)
                    {
                        key = queryString.Substring(startIndex, index - startIndex);
                        value = queryString.Substring(index + 1, (i - index) - 1);
                    }
                    else
                    {
                        key = queryString.Substring(startIndex, i - startIndex);
                    }
                    if (isEncoded)
                    {
                        result[MyUrlDeCode(key, encoding)] = MyUrlDeCode(value, encoding);
                    }
                    else
                    {
                        result[key] = value;
                    }
                    if ((i == (count - 1)) && (queryString[i] == '&'))
                    {
                        result[key] = string.Empty;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 解码URL.
        /// </summary>
        /// <param name="encoding">null为自动选择编码</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                     
                string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return HttpUtility.UrlDecode(str, encoding);
        }

    }

    public class PwdErrorUserEntity
    {
        public int ErrorCount;
        public DateTime ForbidTime;
    }

}