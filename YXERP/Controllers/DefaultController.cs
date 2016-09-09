using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;

using CloudSalesEntity.Manage;
using CloudSalesBusiness.Manage;

namespace YXERP.Controllers
{
    public class DefaultController : BaseController
    {
        //
        // GET: /Default/

        public ActionResult Index(string href = "",string name="")
        { 
            ViewBag.Herf = string.IsNullOrEmpty(href) ? "" : href + (string.IsNullOrEmpty(name) ? "" : "&name=" + name);
            string otherID =CurrentUser.Client.OtherSysID;
            ViewBag.OtherID =(string.IsNullOrEmpty(otherID) ? "" : otherID);
            ViewBag.RemainDay =Math.Ceiling((CurrentUser.Client.EndTime - DateTime.Now).TotalDays);
            ViewBag.RemainDate =  CurrentUser.Client.EndTime.Date.ToString("yyyy-MM-dd"); 
            return View();
        }

        public ActionResult LeftMenu(string id)
        {
            ViewBag.MenuCode = id;
            return PartialView();
        }
    
        public ActionResult Home()
        {
            ViewBag.UserCount = OrganizationBusiness.GetUsers(CurrentUser.AgentID).Count;
            var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);
            ViewBag.RemainderDays = (agent.EndTime - DateTime.Now).Days;
            ViewBag.UserQuantity = agent.UserQuantity;

            return View();
        }

        #region Ajax

        //待办列表
        public JsonResult GetClientUpcomings()
        {
            var list = LogBusiness.BaseBusiness.GetClientUpcomings(CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //授权信息
        public JsonResult GetAgentAuthorizes()
        {

            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            int remainderDays = 0;
            int authorizeType = 0;

            if (Session["ClientManager"] != null)
            {
                var CurrentUser = (CloudSalesEntity.Users)Session["ClientManager"];
                var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);

                remainderDays = (agent.EndTime - DateTime.Now).Days;
                authorizeType = agent.AuthorizeType;

            }

            JsonDictionary.Add("remainderDays", remainderDays);
            JsonDictionary.Add("authorizeType", authorizeType);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        //统计信息
        public JsonResult GetAgentActions()
        {
            CloudSalesEntity.Users CurrentUser = (CloudSalesEntity.Users)Session["ClientManager"];
            var model = LogBusiness.BaseBusiness.GetAgentActions(CurrentUser.AgentID);

            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("model", model);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //意见反馈
        public JsonResult SaveFeedBack(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FeedBack model = serializer.Deserialize<FeedBack>(entity);
            model.CreateUserID = CurrentUser.UserID;

            bool flag = FeedBackBusiness.InsertFeedBack(model);
            JsonDictionary.Add("Result", flag ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
