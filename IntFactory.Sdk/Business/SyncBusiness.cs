﻿using IntFactory.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntFactory.Sdk
{
    public class SyncBusiness
    {

        public AddResult SyncProduct(string agentid, string clientid,string userid)
        {
            ///1.获取供应商
            var list = CloudSalesBusiness.ProductsBusiness.BaseBusiness.GetProviders(clientid);
            var providers = "";
            list.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.CMClientID))
                {
                    providers += "''" + x.CMClientID + "'',";
                }
            });
            if (!string.IsNullOrEmpty(providers))
            {
                providers = providers.Substring(1, providers.Length - 3);
            }
            //2.获取智能工厂已公开款式
            int totalcount = 1;
            for (int i = 1; i <= totalcount; i++)
            {
                var orderlist = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(providers, 20, i, clientid, "", "",
                    "", "", "");
                if (orderlist.orders.Any())
                {
                    totalcount = orderlist.pageCount;
                    orderlist.orders.ForEach(x =>
                    {
                        x.details=new List<ProductDetailEntity>();
                        x.OrderAttrs.Where(y => y.AttrType==2).ToList().ForEach(y =>
                        {
                            x.OrderAttrs.Where(z => z.AttrType == 2).ToList().ForEach(z =>
                            {
                                
                            }); 
                            
                        });
                        try
                        {
                            var dids = "";
                            OrderBusiness.BaseBusiness.ZNGCAddProduct(x, agentid, clientid, userid, ref dids);

                        }
                        catch (Exception ex)
                        {
                            CloudSalesBusiness.CommonBusiness.WriteLog(
                                string.Format("同步添加产品失败，原因{0}", ex.ToString()), 2, "Error");
                        }
                    });
                }
            }
            //3.循环同步数据

           
            //返回错误
            return new AddResult()
            {
                error_code=0
            }; 
        }
    }
}
