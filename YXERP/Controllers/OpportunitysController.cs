﻿using System;
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
            ViewBag.Type = (int)EnumDocType.Opportunity;
            ViewBag.GUID = id;
            ViewBag.Title = "添加意向产品";
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

        public JsonResult SubmitOrder(string opportunityid)
        {
            var bl = OpportunityBusiness.BaseBusiness.SubmitOrder(opportunityid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        public JsonResult GetStageItems(string stageid)
        {
            var list = SystemBusiness.BaseBusiness.GetOpportunityStageByID(stageid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list.StageItem);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOpportunityProductPrice(string opportunityid, string productid, string name, decimal price)
        {
            var bl = OpportunityBusiness.BaseBusiness.UpdateOpportunityProductPrice(opportunityid, productid, name, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOpportunityProductQuantity(string opportunityid, string productid, string name, int quantity)
        {
            var bl = OpportunityBusiness.BaseBusiness.UpdateOpportunityProductQuantity(opportunityid, productid, name, quantity, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        #endregion

    }
}
