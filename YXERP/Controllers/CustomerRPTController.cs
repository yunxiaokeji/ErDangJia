using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesEnum;

namespace YXERP.Controllers
{

    public class CustomerRPTController : BaseController
    {
        //
        // GET: /CustomerRPT/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SourceReport()
        {
            return View();
        }

        public ActionResult StageReport()
        {
            return View();
        }

        public ActionResult CustomerReport()
        {
            return View();
        }

        public ActionResult UserReport()
        {
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetOpportunityStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }


        #region Ajax 客户来源统计

        public JsonResult GetCustomerSourceScale(string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetCustomerSourceScale(beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult() 
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerSourceDate(EnumDateType dateType, string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetCustomerSourceDate(dateType, beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Ajax 客户分布统计

        public JsonResult GetCustomerReport(int type, string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetCustomerReport(type, beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            if (type == 1)
                list.Sort( (g1, g2) => { return Comparer<int>.Default.Compare(g2.value, g1.value); });
            else if (type == 3)
                list.Sort((g1, g2) => { return Comparer<int>.Default.Compare(int.Parse( g2.name),int.Parse( g1.name));});
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 客户转化率

        public JsonResult GetCustomerStageRate(int type, string beginTime, string endTime,string ownerid)
        {
            if (type<2)
            {
                var list = CustomerRPTBusiness.BaseBusiness.GetCustomerStageRate(beginTime, endTime, type, CurrentUser.ClientID, ownerid);
                JsonDictionary.Add("items", list);
            }
            else if (type == 2 || type == 3)
            {
                var list = CustomerRPTBusiness.BaseBusiness.GetUserCustomers("", "", beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
                JsonDictionary.Add("items", list);
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 销售客户统计

        public JsonResult GetUserCustomers(string UserID, string TeamID, string beginTime, string endTime,int type=0)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetUserCustomers(UserID, TeamID, beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);

            if (type == 6)
            {
                Dictionary<string, List<StageCustomerEntity>> customerlist =
                          new Dictionary<string, List<StageCustomerEntity>>();
                if (!string.IsNullOrEmpty(UserID))
                {
                    customerlist.Add("TotalList", list);
                    customerlist.Add("SCSRList", list);
                    customerlist.Add("OCSRList", list);
                    customerlist.Add("NCSRList", list);
                }
                else
                {
                    List<StageCustomerEntity> listcustomer = new List<StageCustomerEntity>();
                    list.ForEach(x => listcustomer.AddRange(x.ChildItems));
                    customerlist.Add("TotalList", listcustomer.OrderByDescending(x => x.TotalNum).Take(15).ToList());
                    customerlist.Add("SCSRList", listcustomer.OrderByDescending(x => x.SCSRNum).Take(15).ToList());
                    customerlist.Add("OCSRList", listcustomer.OrderByDescending(x => x.OCSRNum).Take(15).ToList());
                    customerlist.Add("NCSRList", listcustomer.OrderByDescending(x => x.NCSRNum).Take(15).ToList());
                } 
                JsonDictionary.Add("items", customerlist);
            }
            else
            {
                JsonDictionary.Add("items", list);
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
