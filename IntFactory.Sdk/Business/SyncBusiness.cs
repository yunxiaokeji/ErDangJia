using IntFactory.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CloudSalesBusiness;
using CloudSalesEntity;

namespace IntFactory.Sdk
{
    public class SyncBusiness
    {
        public static SyncBusiness BaseBusiness = new SyncBusiness();

        public AddResult SyncProduct(string cmClientID, string userid, string agentid, string clientid)
        {
            List<CategoryEntity> cateList = ClientBusiness.BaseBusiness.GetAllCategory();
            try
            {
                string provideid = ProductsBusiness.BaseBusiness.GetCMProviderID(clientid);
                //2.获取智能工厂已公开款式
                int totalcount = 1;
                for (int i = 1; i <= totalcount; i++)
                {
                    var orderlist = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(clientid, 20, i, cmClientID, "", "", "", "", "");
                    if (orderlist.orders.Any())
                    {
                        totalcount = orderlist.pageCount;

                        foreach (var order in orderlist.orders)
                        {
                            var categortModel = new Category();
                            if (categortModel != null && !string.IsNullOrEmpty(categortModel.CategoryID))
                            {
                                string[] attrs = categortModel.SaleAttr.Split(',');
                                order.details = new List<ProductDetailEntity>();
                                order.OrderAttrs.Where(y => y.AttrType == 2).ToList().ForEach(y =>
                                {
                                    order.OrderAttrs.Where(z => z.AttrType == 1).ToList().ForEach(z =>
                                    {
                                        string xremark = z.AttrName.Replace("【", "[").Replace("】", "]");
                                        string yremark = y.AttrName.Replace("【", "[").Replace("】", "]");
                                        order.details.Add(new ProductDetailEntity()
                                        {
                                            saleAttr = attrs[0] + "," + attrs[1],
                                            attrValue = y.AttrName.Replace("【", "").Replace("】", "") + "," + z.AttrName.Replace("【", "").Replace("】", ""),
                                            saleAttrValue = attrs[0] + ":" + y.AttrName.Replace("【", "").Replace("】", "") + "," +
                                                            attrs[1] + ":" + z.AttrName.Replace("【", "").Replace("】", ""),
                                            xRemark = xremark,
                                            yRemark = yremark,
                                            xYRemark = yremark + xremark,
                                            remark = yremark + xremark
                                        });
                                    });
                                });
                            }

                            //同步插入产品
                            OrderBusiness.BaseBusiness.ZNGCAddProduct(order, categortModel.CategoryID, provideid, agentid, clientid, userid);
                        }
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                CloudSalesBusiness.CommonBusiness.WriteLog(
                    string.Format("同步添加产品失败，原因{0}", ex.ToString()), 2, "Error");
            }
            //返回错误
            return new AddResult()
            {
                error_code = 0,
                error_message = "同步成功"
            };
        }

        private static char[] constant ={   
        '0','1','2','3','4','5','6','7','8','9','-',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',   
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'   
      };
        public static string GenerateRandomNumber(int length=10)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }

    }
}
