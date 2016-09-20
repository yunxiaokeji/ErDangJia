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

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Home/login");
            }
            if (!ProductService.IsExistsProvider(id, CurrentUser.ClientID) && id.ToLower() != CurrentUser.ClientID.ToLower())
            {
                var client = ClientBusiness.GetClientDetail(id);
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
                var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
                ViewBag.Client = client == null ? CurrentUser.Client : client;
                return View("Goods");
            }
        }

        /// <summary>
        /// 打样单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Goods(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Home/login");
            }
            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
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
            
            ViewBag.Client = client == null ? CurrentUser.Client : client;
            return View();
        }

        //
        // GET: /IntFactoryOrder/
        /// <summary>
        /// 需求单下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DownOrder(string id)
        {
            ViewBag.ClientID = id;
            ViewBag.Items = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetClientCategorys("", IntFactory.Sdk.EnumCategoryType.Order);
            ViewBag.Categorys = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetProcessCategorys(id);
            var clientid = "";
            if (CurrentUser != null)
            {
                clientid = CurrentUser.ClientID;
            }
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(clientid);
            return View();
        }

        public ActionResult CallBackView(string sign, string uid = "", string aid = "")
        { 
            return Redirect("/Mall/Store/Goods");
        } 

        /// <summary>
        /// 打样单详情
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public ActionResult GoodsDetail(string orderid, string clientid="")
        {
            if (string.IsNullOrEmpty(orderid))
            {
                Response.Write(
                    "<script type='text/javascript'>location.href='/Home/login';</script>");
                Response.End();
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
        public JsonResult GetOrderAttrsList(string goodsid)
        {
            var result = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrdersAttrsList(goodsid);
            JsonDictionary.Add("items", result.error_code == 0 ? result.attrList : new List<IntFactory.Sdk.OrderAttrEntity>());
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetProcessCategorys(string zngcclientid)
        {
            var result = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetProcessCategorys(zngcclientid);
            JsonDictionary.Add("items", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrder(string entity,string clientid,string userid="")
        {
            IntFactory.Sdk.AddResult result = IntFactory.Sdk.OrderBusiness.BaseBusiness.CreateOrder(entity, clientid, userid);
            JsonDictionary.Add("id", result.id);
            JsonDictionary.Add("err_msg", result.error_message);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult LoginCallBack(string sign, string uid = "", string aid = "")
        { 
            JsonDictionary.Add("uid", !string.IsNullOrEmpty(uid));
            JsonDictionary.Add("ccode", CurrentUser.Client.CityCode);
            JsonDictionary.Add("address", CurrentUser.Client.Address);
            JsonDictionary.Add("mphone", CurrentUser.Client.MobilePhone);
            JsonDictionary.Add("cname", CurrentUser.Client.ContactName); 
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
         
        public JsonResult GetEdjCateGory(string clientid,string categoryid="")
        {
            var result = new ProductsBusiness().GetCategorys(clientid);
            result = result.Where(x => x.PID == categoryid).ToList();
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
            List<Products> list = new ProductsBusiness().GetProductList(categoryID, beginPrice, endPrice, keyWords,
                   orderby, isAsc, pageSize, pageIndex, ref totalCount, ref pageCount, clientid,1);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetZNGCCategorys(string categoryid)
        {
            var obj = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetCategoryByID(categoryid);
            JsonDictionary.Add("items", obj); 
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
    }
}
