using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesEntity;
using YXERP.Models;
using CloudSalesBusiness;
using CloudSalesEnum;

namespace YXERP.Controllers
{
    public class OpportunitysController : BaseController
    {
        //
        // GET: /Orders/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyOpportunity()
        {
            ViewBag.Title = "我的机会";
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetOpportunityStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Type = (int)EnumSearchType.Myself;
            return View("Opportunitys");
        }

        public ActionResult BranchOpportunity()
        {
            ViewBag.Title = "下属机会";
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetOpportunityStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Type = (int)EnumSearchType.Branch;
            return View("Opportunitys");
        }

        public ActionResult Opportunitys()
        {
            ViewBag.Title = "所有机会";
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetOpportunityStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Type = (int)EnumSearchType.All;
            return View("Opportunitys");
        }

        public ActionResult ChooseProducts(string id)
        {
            ViewBag.Type = (int)EnumDocType.Order;
            ViewBag.GUID = id;
            ViewBag.Title = "销售机会选择产品";
            return View("FilterProducts");
        }

        public ActionResult Detail(string id)
        {
            var model = OpportunityBusiness.BaseBusiness.GetOpportunityByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OpportunityID))
            {
                return Redirect("/Opportunitys/MyOpportunity");
            }

            ViewBag.Model = model;
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetOpportunityStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.OrderTypes = SystemBusiness.BaseBusiness.GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        


        #region Ajax

        public JsonResult Create(string customerid, string typeid)
        {
            string id = OpportunityBusiness.BaseBusiness.CreateOpportunity(customerid, typeid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOpportunitys(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterOpportunity model = serializer.Deserialize<FilterOpportunity>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = OpportunityBusiness.BaseBusiness.GetOpportunitys(model.SearchType, model.TypeID, model.Status, model.StageID, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, model.Keywords, model.OrderBy, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOpportunityByCustomerID(string customerid, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OpportunityBusiness.BaseBusiness.GetOpportunityaByCustomerID(customerid, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOpportunityOwner(string ids, string userid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && OpportunityBusiness.BaseBusiness.UpdateOpportunityOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitOrder(string orderid)
        {
            var bl = OrdersBusiness.BaseBusiness.SubmitOrder(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOpportunity(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OpportunityEntity model = serializer.Deserialize<OpportunityEntity>(entity);


            var bl = OpportunityBusiness.BaseBusiness.UpdateOpportunity(model.OpportunityID, model.PersonName, model.MobileTele, model.CityCode, model.Address, model.PostalCode, model.TypeID, model.Remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderPrice(string orderid, string autoid, string name, decimal price)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderPrice(orderid, autoid, name, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CloseOpportunity(string opportunityid)
        {
            var bl = OpportunityBusiness.BaseBusiness.CloseOpportunity(opportunityid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOpportunityLogs(string opportunityid, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(opportunityid, EnumLogObjectType.Opportunity, 10, pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOpportunityStage(string ids, string stageid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && OpportunityBusiness.BaseBusiness.UpdateOpportunityStage(id, stageid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetReplys(string guid, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            var list = OpportunityBusiness.GetReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaReply(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReplyEntity model = serializer.Deserialize<ReplyEntity>(entity);

            string replyID = "";
            replyID = OpportunityBusiness.CreateReply(model.GUID, model.Content, CurrentUser.UserID, CurrentUser.AgentID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);

            List<ReplyEntity> list = new List<ReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {
                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = CurrentUser;
                model.CreateUserID = CurrentUser.UserID;
                model.AgentID = CurrentUser.AgentID;
                if (!string.IsNullOrEmpty(model.FromReplyUserID) && !string.IsNullOrEmpty(model.FromReplyAgentID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
