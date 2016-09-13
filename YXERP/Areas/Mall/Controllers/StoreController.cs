﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactory.Sdk;
using CloudSalesBusiness;
using CloudSalesEnum;
using Newtonsoft.Json;
using CloudSalesEntity;

namespace YXERP.Areas.Mall.Controllers
{
    public class StoreController : YXERP.Controllers.BaseController
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
        public ActionResult Goods(string id)
        {
            var Status = 0;
            var clientid = ""; 
            if (CurrentUser != null)
            {
                Status = 1;
                clientid = CurrentUser.ClientID; 
            }
            else
            {
                Response.Write(
                    "<script type='text/javascript'>alert('请登录后再操作.');location.href='/Home/login?Status=-1&BindAccountType=6&ReturnUrl=" +
                    GetbaseUrl() + "/Mall/Store/Goods?id=" + id + "';</script>");
                Response.End();
            }

            if (string.IsNullOrEmpty(id))
            {
                Response.Write(
                    "<script type='text/javascript'>location.href='/Home/login';</script>");
                Response.End();
            }
            ViewBag.Url = GetbaseUrl();
            ViewBag.ClientID = id;
            ViewBag.LogStatus=Status;
            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
            ViewBag.Client = client == null ? CurrentUser.Client : client;
            return View();
        }
        public ActionResult CallBackView(string sign, string uid = "", string aid = "")
        { 
            return Redirect("/Mall/Store/Goods");
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
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders("-1");
            return View();
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
            else
            {
                Response.Write(
                    "<script type='text/javascript'>alert('请登录后再操作.');location.href='/Home/login?Status=-1&BindAccountType=6&ReturnUrl=" +
                    GetbaseUrl() + "/Mall/Store/Goods?id=" + clientid + "';</script>");
                Response.End();
            } 
            ViewBag.Url = GetbaseUrl();
            ViewBag.ClientID = clientid;
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
        public JsonResult GetOrderAttrsList(string goodsid)
        {
            var result = OrderBusiness.BaseBusiness.GetOrdersAttrsList(goodsid);
            if (result.error_code == 0)
            {
                JsonDictionary.Add("items", result.attrList);
            }
            else
            {
                JsonDictionary.Add("items", "[]");
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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
        public JsonResult GetEdjCateGory()
        {
            var result = new ProductsBusiness().GetCategorys(CurrentUser.ClientID);
            result = result.Where(x => string.IsNullOrEmpty(x.PID)).ToList();
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
                            "", ord.clientID, ord.clientCode, "", CurrentUser.Client.AgentID, CurrentUser.ClientID,1);
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

        public JsonResult CreatePurchaseOrder(string productid, decimal price, string parentprid, string entity, string goodsid, string goodscode,string goodsname, decimal totalFee = 0)
        {
            if (CurrentUser == null)
            {
                JsonDictionary.Add("result", -9);
                JsonDictionary.Add("errMsg", "未登录请登陆后再提交");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            var details = JsonConvert.DeserializeObject<List<IntFactory.Sdk.ProductDetailEntity>>(entity);
            CloudSalesEntity.Users user = CurrentUser;

            //1.判断产品是否存ZNGCAddProduct在 与明细 不存在则插入 
            string dids = OrderBusiness.BaseBusiness.CheckProductDetail(productid, details, price, CurrentUser.ClientID, user.UserID, goodsid, goodscode, goodsname);
            if (string.IsNullOrEmpty(productid) || string.IsNullOrEmpty(dids))
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", "获取产品失败，请稍后重试");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            string provideid = ProductsBusiness.BaseBusiness.GetProviderIDByCMID(CurrentUser.ClientID, parentprid);
            //2.生成采购单据 
            string purid = StockBusiness.AddPurchaseDoc(productid, dids, provideid, totalFee, "", "", 2, user.UserID,
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
            JsonDictionary.Add("result", 1); 
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
