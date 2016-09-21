using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudSalesBusiness;
using CloudSalesEnum;
using Newtonsoft.Json;
using CloudSalesEntity;

using CloudSalesBusiness.Manage;
using CloudSalesEntity.Manage;
using YunXiaoService;

namespace YXERP.Areas.Mall.Controllers
{
    public class StoreController : YXERP.Controllers.BaseController
    {

        public ActionResult Index(string id,string categoryid="")
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Home/login");
            }

            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
            //非智能工厂暂不开通店铺
            var agent = CloudSalesBusiness.AgentsBusiness.GetAgentDetail(client.AgentID);
            if (string.IsNullOrEmpty(agent.CMClientID))
            {
                return Redirect("/Home/login");
            }
            if (!ProductService.IsExistsProvider(id, CurrentUser.ClientID) && id.ToLower() != CurrentUser.ClientID.ToLower())
            {
                
                string providerID = ProductService.AddProviders(client.CompanyName, client.ContactName,
                    client.MobilePhone, "", client.CityCode, client.Address,
                    "", id, client.ClientCode, "", CurrentUser.Client.AgentID, CurrentUser.Client.ClientID, 2);
            }
            if (YXERP.Common.Common.IsMobileDevice())
            {
                return Redirect("/M/Home/Index?providerID=" + id);
            }
            else
            {
                ViewBag.Url = GetbaseUrl();
                ViewBag.ClientID = id; ;
                ViewBag.CategoryID = categoryid;
                ViewBag.Client = client == null ? CurrentUser.Client : client;
                return View("Goods");
            }
        }

        /// <summary>
        /// 打样单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Goods(string id, string categoryid = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Home/login");
            }

            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
            //非智能工厂暂不开通店铺
            var agent = CloudSalesBusiness.AgentsBusiness.GetAgentDetail(client.AgentID);
            if (string.IsNullOrEmpty(agent.CMClientID))
            {
                return Redirect("/Home/login");
            }
            if (!ProductService.IsExistsProvider(id, CurrentUser.ClientID) && id.ToLower() != CurrentUser.ClientID.ToLower())
            {
                string providerID = ProductService.AddProviders(client.CompanyName, client.ContactName,
                    client.MobilePhone, "", client.CityCode, client.Address,
                    "", id, client.ClientCode, "", CurrentUser.Client.AgentID, CurrentUser.Client.ClientID, 2);
            }
            if (YXERP.Common.Common.IsMobileDevice())
            {
                return Redirect("/M/Home/Index?providerID=" + id);
            }
            ViewBag.Url = GetbaseUrl();
            ViewBag.ClientID = id;;
            ViewBag.CategoryID = categoryid;
            ViewBag.Client = client == null ? CurrentUser.Client : client;
            return View();
        }


        public ActionResult CallBackView(string sign, string uid = "", string aid = "")
        {
            //非智能工厂暂不开通店铺
            if (string.IsNullOrEmpty(CurrentUser.Agents.CMClientID))
            {
                return Redirect("/Home/login");
            }
            return Redirect("/Mall/Store/Goods");
        } 

        /// <summary>
        /// 打样单详情
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public ActionResult GoodsDetail(string orderid, string clientid = "", string categoryid = "")
        {
            if (string.IsNullOrEmpty(orderid))
            {
                return Redirect("/Home/login");
            }

            string ccode, address, mphone,cname;
            ccode = address = mphone = cname = "";
            if (CurrentUser != null)
            {
                ccode = CurrentUser.Client.CityCode;
                address = CurrentUser.Client.Address;
                mphone = CurrentUser.Client.MobilePhone;
                cname = CurrentUser.Client.ContactName;
            }
            ViewBag.CategoryID = categoryid;
            ViewBag.Url = GetbaseUrl();
            ViewBag.OrderID = orderid;
            ViewBag.CityCode = ccode;
            ViewBag.ContactName = cname;
            ViewBag.MobilePhone = mphone;
            ViewBag.Address = address; 
            var obj = new ProductsBusiness().GetProductByIDForDetails(orderid);
            ViewBag.Model = obj;
            ViewBag.ClientID = obj.ClientID;
            return View();
        }

        #region

        public JsonResult GetClientDetail(string clientid)
        {
            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(clientid);
            if (client!=null)
            {
                JsonDictionary.Add("items", client);
            }
            return new JsonResult()
            {
                Data = client,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 
        public JsonResult GetEdjCateGory(string clientid)
        {
            if (string.IsNullOrEmpty(CurrentUser.CurrentCMClientID))
            {
                var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(clientid);
                CurrentUser.CurrentClientID = clientid;
                var agent = AgentsBusiness.GetAgentDetail(client.AgentID);
                CurrentUser.CurrentCMClientID = agent.CMClientID;
            }
            return GetAllCateGory(CurrentUser.CurrentCMClientID);
            var result = new ProductsBusiness().GetCategorys(clientid); 
            JsonDictionary.Add("items", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetAllCateGory(string clientid)
        {
            var result = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetAllCategory();

            JsonDictionary.Add("items", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProduct(string clientid, string keyWords, int pageSize, int pageIndex, bool isAsc, string categoryID = "", string orderby = "", string beginPrice = "", string endPrice = "")
        {
            int pageCount = 0;
            int totalCount = 0;
            if (clientid.Equals(CurrentUser.Agents.CMClientID))
            {
                clientid = CurrentUser.ClientID;
            }
            if (string.IsNullOrEmpty(CurrentUser.CurrentCMClientID))
            {
                var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(clientid);
                CurrentUser.CurrentClientID = clientid;
                var agent = AgentsBusiness.GetAgentDetail(client.AgentID);
                CurrentUser.CurrentCMClientID = agent.CMClientID;
            }
            //暂读取智能工厂产品
            return  this.GetProductList
            (CurrentUser.CurrentCMClientID, keyWords, pageSize, pageIndex, categoryID, orderby,  isAsc, beginPrice, endPrice);

            List<Products> list = new ProductsBusiness().GetProductList(categoryID, beginPrice, endPrice, keyWords,
                   orderby, isAsc, pageSize, pageIndex, ref totalCount, ref pageCount, clientid, 1);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetProductList(string clientid, string keyWords, int pageSize, int pageIndex, string categoryID = "", string orderby = "", bool isAsc=false, string beginPrice = "", string endPrice = "")
        {
            IntFactory.Sdk.OrderListResult item = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(keyWords, clientid, pageSize, pageIndex, categoryID, beginPrice, endPrice, isAsc, orderby);
            JsonDictionary.Add("items", item.orders);
            JsonDictionary.Add("totalCount", item.totalCount);
            JsonDictionary.Add("pageCount", item.pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreatePurchaseOrder(string productid, decimal price, string parentprid,
            string personname, string mobiletele, string citycode, string address, string dids, decimal totalFee = 0)
        {
            int result = 0;
            string error = string.Empty;
            if (CurrentUser == null)
            {
                result = -9;
                error = "未登录请登陆后再提交";
            }
            else
            {

                string purid = StockBusiness.AddPurchaseDoc(productid, dids.TrimEnd(','), parentprid, totalFee, "", "", 2, CurrentUser.UserID,
                    CurrentUser.AgentID, CurrentUser.ClientID, personname, mobiletele, address, citycode);

                if (string.IsNullOrEmpty(purid))
                {
                    result = -9;
                    error = "采购单生成失败，请稍后重试";
                }
                else
                {
                    result = 1;
                    JsonDictionary.Add("PurchaseID", purid);
                }
            }
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errMsg", error);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
