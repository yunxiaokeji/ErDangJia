using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            Config.Init(); 
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(bucket, 3600);

            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            JsonDictionary.Add("uptoken", upToken);
            JsonDictionary.Add("bucket", bucket);
            JsonDictionary.Add("imgurl", imgurl);
            return JsonDictionary;
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
    }

    public class PwdErrorUserEntity
    {
        public int ErrorCount;
        public DateTime ForbidTime;
    }

}