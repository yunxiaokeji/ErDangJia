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
                        order.details = new List<ProductDetailEntity>();
                    }

                    orderlist.orders.ForEach(x =>
                    {
                        x.details = new List<ProductDetailEntity>();
                        x.OrderAttrs.Where(y => y.AttrType == 2).ToList().ForEach(y =>
                        {
                            x.OrderAttrs.Where(z => z.AttrType == 1).ToList().ForEach(z =>
                            {
                                string xremark = z.AttrName.Replace("【", "[").Replace("】", "]");
                                string yremark = y.AttrName.Replace("【", "[").Replace("】", "]");
                                x.details.Add(new ProductDetailEntity()
                                {
                                    xRemark = xremark,
                                    yRemark = yremark,
                                    xYRemark = yremark + xremark,
                                    remark = yremark + xremark
                                });
                            });
                        });
                        try
                        {
                            var dids = "";
                            //插入分类
                            string categoryid = SyncCateGory(x.categoryID, clientid, ref cateList);
                            //同步插入产品
                            OrderBusiness.BaseBusiness.ZNGCAddProduct(x, agentid, clientid, userid, ref dids, categoryid);
                        }
                        catch (Exception ex)
                        {
                            CloudSalesBusiness.CommonBusiness.WriteLog(
                                string.Format("同步添加产品失败，原因{0}", ex.ToString()), 2, "Error");
                        }
                    });
                }
                break;
            }
            //返回错误
            return new AddResult()
            {
                error_code=0,
                error_message = "同步成功"
            }; 
        }

        public string SyncCateGory(string categoryid, string clientid, ref List<CategoryEntity> cateList)
        {
            var category = cateList.Where(x => x.CategoryID == categoryid).FirstOrDefault();
            if (category == null)
            {
                cateList = ClientBusiness.BaseBusiness.GetAllCategory(); 
            }
            string pid = "";
            Category pcate=null;
            //获取父级分类
            if (!string.IsNullOrEmpty(category.PID))
            {
                var pcategory = cateList.Where(x => x.CategoryID == category.PID).FirstOrDefault();
                pcate = ProductsBusiness.BaseBusiness.GetCategoryByName(pcategory.CategoryName, clientid);
                if (string.IsNullOrEmpty(pcate.CategoryID))
                {
                    //判断规格是否存在并插入新的分类
                    pcate = GetCateGory(pcategory, clientid);
                    var newcate = GetCateGory(category, clientid, pcate == null ? "" : pcate.CategoryID);
                    return newcate == null ? "" : newcate.CategoryID;
                }
                else
                { 
                   string saleattr= CheckAttr(pcategory, clientid);
                   UpdateCategoryAttr(pcate, saleattr);
                }
                pid = pcate == null ? "" : pcate.CategoryID;
            }
            var localcate = ProductsBusiness.BaseBusiness.GetCategoryByName(category.CategoryName, clientid, pid);
            if (string.IsNullOrEmpty(localcate.CategoryID))
            {
                var newcate = GetCateGory(category, clientid, pid);
                if (newcate != null)
                {
                    pid = newcate.CategoryID;
                }
            }
            else
            {
               string saleattr= CheckAttr(category, clientid);
               UpdateCategoryAttr(localcate, saleattr);
            }
            return pid;
        }

        public CloudSalesEntity.Category GetCateGory(CategoryEntity category, string clientid,string pid="")
        {
            var result = 0;
             //判断规格是否存在
            string salesAttrs = CheckAttr(category, clientid);
           
            //category.SaleAttrs.ForEach(x =>
            //{
            //    var saleList = ProductsBusiness.BaseBusiness.GetAttrs(clientid);
            //    saleList = saleList != null ? saleList : new List<ProductAttr>();
            //    string attrid = "";
            //    var saleAttr = saleList.Where(y => y.AttrName == x.AttrName).FirstOrDefault();
            //    if (saleAttr == null)
            //    {
            //        attrid = ProductsBusiness.BaseBusiness.AddProductAttr(x.AttrName, "", "", 2, "", clientid);
            //        if (!string.IsNullOrEmpty(attrid))
            //        {
            //            saleAttr = ProductsBusiness.BaseBusiness.GetProductAttrByID(attrid, clientid);
            //        }
            //    }
            //    else
            //    {
            //        attrid = saleAttr.AttrID;
            //    }
            //    //比对规格值
            //    if (!string.IsNullOrEmpty(attrid))
            //    {
            //        x.AttrValues.ForEach(y =>
            //        {
            //            //不存在规格值
            //            if (!saleAttr.AttrValues.Where(z => z.ValueName == y.ValueName).Any())
            //            {
            //                ProductsBusiness.BaseBusiness.AddAttrValue(y.ValueName, attrid, "", clientid);
            //            }
            //        });
            //        salesAttrs += attrid + ",";
            //    }
            //});
            //salesAttrs = salesAttrs.TrimEnd(',');
            //插入新的分类
            string code = string.IsNullOrEmpty(category.CategoryCode)
                ? GenerateRandomNumber(10)
                : category.CategoryCode;
            var cate = ProductsBusiness.BaseBusiness.AddCategory(code, category.CategoryName, pid, 1, "", salesAttrs, "", "", clientid, out result);
            return cate;
        }

        public string CheckAttr(CategoryEntity category, string clientid)
        {
            string salesAttrs = "";
            category.SaleAttrs.ForEach(x =>
            {
                var saleList = ProductsBusiness.BaseBusiness.GetAttrs(clientid);
                saleList = saleList != null ? saleList : new List<ProductAttr>();
                string attrid = "";
                var saleAttr = saleList.Where(y => y.AttrName == x.AttrName).FirstOrDefault();
                if (saleAttr == null)
                {
                    attrid = ProductsBusiness.BaseBusiness.AddProductAttr(x.AttrName, "", "", 2, "", clientid);
                    if (!string.IsNullOrEmpty(attrid))
                    {
                        saleAttr = ProductsBusiness.BaseBusiness.GetProductAttrByID(attrid, clientid);
                    }
                }
                else
                {
                    attrid = saleAttr.AttrID;
                }
                //比对规格值
                if (!string.IsNullOrEmpty(attrid))
                {
                    x.AttrValues.ForEach(y =>
                    {
                        //不存在规格值
                        if (!saleAttr.AttrValues.Where(z => z.ValueName == y.ValueName).Any())
                        {
                            ProductsBusiness.BaseBusiness.AddAttrValue(y.ValueName, attrid, "", clientid);
                        }
                    });
                    salesAttrs += attrid + ",";
                }
            });
            return salesAttrs.TrimEnd(',');
        }

        public void UpdateCategoryAttr(Category category, string saleattr)
        {
            int result = 0;
            ProductsBusiness.BaseBusiness.UpdateCategory(category.CategoryID, category.CategoryName,
                category.CategoryCode, category.Status.Value, category.AttrList, saleattr, category.Description, category.OperateIP,
                category.ClientID, out result);
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
