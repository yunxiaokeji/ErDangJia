using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.StyleCenter.Controllers
{
    public class BaseController : Controller
    {
        protected Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
        /// <summary>
        /// 当前登录用户
        /// </summary>
        protected CloudSalesEntity.Users CurrentUser
        {
            get
            {
                if (Session["KSManager"] == null)
                {
                    return null;
                }
                else
                {
                    return (CloudSalesEntity.Users)Session["KSManager"];
                }
            }
            set { Session["KSManager"] = value; }
        }
        public string GetbaseUrl()
        {
            string port = HttpContext.Request.Url.Port.ToString();
            return HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host +
                         (string.IsNullOrEmpty(port) || port == "80" ? "" : ":" + port);
        }
    }
}
