using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using  YXERP.Common;
namespace YXERP.Areas.Api.Controllers
{
    public class BaseAPIController : Controller
    {
        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected Dictionary<string, object> JsonDictionary = new Dictionary<string, object>()
        {
          {"error_code",0}, {"error_msg",""}
        }; 
        protected int PageSize = 10;

        public static string ApiUrl = Common.Common.GetXmlNodeValue("root/ZNGCApi", "ApiUrl") ?? "http://localhost:8888";
        public static string AppKey = Common.Common.GetXmlNodeValue("root/ZNGCApi", "AppKey");
        public static string AppSecret = Common.Common.GetXmlNodeValue("root/ZNGCApi", "AppSecret");
        public static string CallBackUrl = Common.Common.GetXmlNodeValue("root/ZNGCApi", "CallBackUrl");

        /// <summary>
        /// 登录IP
        /// </summary>
        protected string OperateIP
        {
            get
            {
                return string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
            catch (InvalidOperationException ieox)
            {
                ViewData["error"] = "Unknown Action: \"" + Server.HtmlEncode(actionName) + "\"";
                ViewData["exMessage"] = ieox.Message;
                ViewData["urlReferrer"] = Request != null && Request.Url!=null ? Request.Url.ToString() : "";
                this.View("Error").ExecuteResult(this.ControllerContext);
            }
        }
    }
}
