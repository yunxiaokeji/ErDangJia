using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactory.Sdk;
using IntFactory.Sdk.Business;
using CloudSalesBusiness;
using CloudSalesEnum;
using Newtonsoft.Json;
using CloudSalesEntity;

namespace YXERP.Areas.StyleCenter.Controllers
{
    public class StyleCenterController : BaseController
    {
        
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
            ViewBag.Items = ClientBusiness.BaseBusiness.GetClientCategorys("", EnumCategoryType.Order);
            ViewBag.Categorys = ClientBusiness.BaseBusiness.GetProcessCategorys(id);
            var clientid = "";
            if (CurrentUser != null)
            {
                clientid = CurrentUser.ClientID;
            }
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(clientid); 
            return View();
        }

        /// <summary>
        /// 打样单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StyleCenter(string id)
        {
            var Status = 0;
            var clientid = ""; 
            if (CurrentUser != null)
            {
                Status = 1;
                clientid = CurrentUser.ClientID;
            } 
            ViewBag.Url = GetbaseUrl();
            ViewBag.ClientID = id;
            ViewBag.LogStatus=Status;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(clientid); 
            return View();
        }
        public ActionResult CallBackView(string sign, string uid = "", string aid = "")
        {
            SetSession(uid, aid);
            return Redirect("/StyleCenter/StyleCenter/StyleCenter");
        }
        public void SetSession(string uid = "", string aid = "")
        {
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(aid) )
            {
                if (CurrentUser == null)
                {
                    var user = CloudSalesBusiness.OrganizationBusiness.GetUserByUserID(uid, aid);
                    user.Client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(user.ClientID);
                    Session["KSManager"] = user;
                    Session["Time"] = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
                    Session.Timeout = 60;
                }
                else
                {
                    if (Convert.ToDateTime(Session["Time"]).AddHours(1).CompareTo(DateTime.Now) > 1)
                    {
                        Session["KSManager"] = null;
                        Session["Time"] = null;
                    }
                }
            } 
        }

        /// <summary>
        /// 订购订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Type = (int)EnumDocType.RK;
            ViewBag.ZNGCID = "";
            ViewBag.SouceType = 2;
            //ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders("-1");
            return View();
        }

        /// <summary>
        /// 打样单详情
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public ActionResult StyleDetail(string orderid, string clientid)
        {
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
            ViewBag.ClientID = clientid;
            ViewBag.OrderID = orderid;
            ViewBag.CityCode = ccode;
            ViewBag.ContactName = cname;
            ViewBag.MobilePhone = mphone;
            ViewBag.Address = address;
            var obj= OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            if (obj.error_code == 0)
            {
                ViewBag.Model = obj.order;
                ViewBag.ClientID = obj.order.clientID;
            }
            else
            {
                ViewBag.Model=new IntFactory.Sdk.OrderEntity();
            }
            return View();
        }

        public JsonResult GetProcessCategorys(string zngcclientid)
        {
            var result = ClientBusiness.BaseBusiness.GetProcessCategorys(zngcclientid);
            JsonDictionary.Add("items", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrder(string entity,string clientid,string userid="")
        { 
            AddResult result = OrderBusiness.BaseBusiness.CreateOrder(entity, clientid, userid);
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
            SetSession(uid, aid);
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

        public JsonResult IsLogin()
        {
            JsonDictionary.Add("result", CurrentUser!=null);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
      
        public JsonResult GetAllCateGory()
        { 
            var result = ClientBusiness.BaseBusiness.GetAllCategory(1,EnumCategoryType.Order);
          
            JsonDictionary.Add("items", result); 
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetProductList(string clientid, string keyWords, int pageSize, int pageIndex,string categoryID="", string orderby = "", string beginPrice="",string endPrice="")
        { 
            OrderListResult item = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode("", pageSize, pageIndex, clientid, keyWords, categoryID, orderby, beginPrice, endPrice);
            JsonDictionary.Add("items", item.orders);
            JsonDictionary.Add("totalCount", item.totalCount);
            JsonDictionary.Add("pageCount", item.pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProduct(string clientid, string keyWords, int pageSize, int pageIndex, bool isAsc, string categoryID = "", string orderby = "", string beginPrice = "", string endPrice = "")
        {
            int pageCount = 0;
            int totalCount = 0;
            List<Products> list = new ProductsBusiness().GetProductList(categoryID, beginPrice, endPrice, keyWords, orderby, isAsc, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
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
            var obj = ClientBusiness.BaseBusiness.GetCategoryByID(categoryid);
            JsonDictionary.Add("items", obj); 
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderDetailByID(string clientid, string orderid, string categoryid)
        { 
            var obj = OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            if (obj.error_code == 0)
            {
                JsonDictionary.Add("items", obj.order); 
            }
            else
            {
                JsonDictionary.Add("items", ""); 
                JsonDictionary.Add("errMsg", obj.error_message);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderEDJ(string entity,decimal totalFee=0)
        { 
            if (CurrentUser == null)
            {
                JsonDictionary.Add("result",-9);
                JsonDictionary.Add("errMsg", "未登录请登陆后再提交");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            var ord= JsonConvert.DeserializeObject<IntFactory.Sdk.OrderEntity>(entity);
            CloudSalesEntity.Users user = CurrentUser;
            //0.判断供应商以及是否授权
            string provideid = ProductsBusiness.BaseBusiness.GetProviderIDByCMID(CurrentUser.ClientID, ord.clientID);
            if (string.IsNullOrEmpty(provideid))
            {
                 YunXiaoService.ProductService.AddProviders(ord.clientName,
                            ord.clientContactName,
                            ord.clientMobile, "", ord.clientCityCode, ord.clientAdress,
                            "", ord.clientID, ord.clientCode, "", CurrentUser.Client.AgentID, CurrentUser.ClientID);
            }
            if (string.IsNullOrEmpty(CurrentUser.Client.OtherSysID) || (!string.IsNullOrEmpty(CurrentUser.Client.OtherSysID) && CurrentUser.Client.OtherSysID.IndexOf(ord.clientID)==-1))
            {
                var zngcResult = IntFactory.Sdk.CustomerBusiness.BaseBusiness.SetCustomerYXinfo("", CurrentUser.Client.CompanyName,
                            CurrentUser.Client.MobilePhone, ord.clientID,
                             CurrentUser.Client.AgentID, CurrentUser.ClientID, CurrentUser.Client.ClientCode);
                CloudSalesBusiness.Manage.ClientBusiness.UpdateClientOtherid(ord.clientID, CurrentUser.ClientID);
                user.Client.OtherSysID = string.IsNullOrEmpty(user.Client.OtherSysID)
                    ? ord.clientID + ","
                    : user.Client.OtherSysID + ord.clientID + ",";
                Session["KSManager"] = user;
            }

            //1.判断产品是否存ZNGCAddProduct在 与明细 不存在则插入
            string dids = "";
           
            string pid = OrderBusiness.BaseBusiness.ZNGCAddProduct(ord, user.AgentID, user.ClientID, user.UserID, ref dids);
            if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(dids))
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", "获取产品失败，请稍后重试");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            } 
            //2.生成采购单据 
            string purid = StockBusiness.AddPurchaseDoc(pid, dids, ord.clientID, totalFee, "", "", 2, user.UserID,
                user.AgentID, user.ClientID);
            if (string.IsNullOrEmpty(purid))
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", "采购单生成失败，请稍后重试");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            JsonDictionary.Add("PurchaseID", purid);
            //3.生成智能工厂单据
            ord.details.ForEach(x =>
            { 
                x.remark = x.remark.Replace("[", "【").Replace("]", "】");
            });
            var result = OrderBusiness.BaseBusiness.CreateDHOrder(ord.orderID, ord.finalPrice, ord.details,
                    ord.clientID, purid, user.ClientID,ord.personName, ord.mobileTele, ord.cityCode, ord.address);
            if (!string.IsNullOrEmpty(result.id))
            {
                JsonDictionary.Add("OtherOrderID", result.id);
                JsonDictionary.Add("result", 1);
            } 
            else
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", result.error_message);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
