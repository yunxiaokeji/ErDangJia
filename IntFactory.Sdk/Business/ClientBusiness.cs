using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntFactory.Sdk.Entity.Client;

namespace IntFactory.Sdk
{
    public class ClientBusiness
    {
        public static ClientBusiness BaseBusiness = new ClientBusiness();

        public static List<CategoryEntity>  CategoryList;
        public static Nullable<DateTime> refreshTime { get; set; } 

        public ClientResult GetClientInfo(string zngcClientID,string userid="")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("clientID", zngcClientID);

            return HttpRequest.RequestServer<ClientResult>(ApiOption.GetClientInfo, paras);
        }

        public List<ProcessCategoryEntity> GetProcessCategorys(string zngcClientID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("clientID", zngcClientID);

            ProcessCategoryResult result= HttpRequest.RequestServer<ProcessCategoryResult>(ApiOption.GetProcessCategorys, paras);
            if (result.error_code == 0)
            {
                return result.result;
            }
            else
            {
                return new List<ProcessCategoryEntity>();
            }
        }

        public CategoryEntity GetCategoryByID(string categoryID)
        {
            var list = GetAllCategory();
            return list.Where(x => x.CategoryID == categoryID).FirstOrDefault();
        }

        public List<CategoryEntity> GetCategoryByPID(string pid)
        {
            var list = GetAllCategory();
            return list.Where(x => x.PID == pid).ToList();
        }

        public List<CategoryEntity> GetAllCategory()
        {
            if (CategoryList != null && CategoryList.Count != 0)
            {
                return CategoryList;
            }
            var list = new List<CategoryEntity>();

            var paras = new Dictionary<string, object>();
            paras.Add("layerid", -1);
            paras.Add("type", EnumCategoryType.Order);
            CategorysResult result = HttpRequest.RequestServer<CategorysResult>(ApiOption.GetAllCategorys, paras);
            if (result.error_code == 0)
            {
                CategoryList = result.result;
                list = CategoryList;
            }
            return list;
            
        }

        public List<CategoryEntity> GetClientCategorys(string categoryid, EnumCategoryType type)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("categoryID", categoryid);
            paras.Add("type", type); 
            CategorysResult result= HttpRequest.RequestServer<CategorysResult>(ApiOption.GetClientCategorys, paras);
            if (result.error_code == 0)
            {
                return result.result;
            }
            else
            {
                return new List<CategoryEntity>();
            }
        }
    }
}
