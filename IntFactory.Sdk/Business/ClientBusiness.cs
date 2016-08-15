using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntFactory.Sdk.Entity.Client;

namespace IntFactory.Sdk.Business
{
    public class ClientBusiness
    {
        public static ClientBusiness BaseBusiness = new ClientBusiness();

        public ClientResult GetClientInfo(string zngcClientID)
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
            var paras = new Dictionary<string, object>();
            paras.Add("categoryID", categoryID);

            CategoryResult result = HttpRequest.RequestServer<CategoryResult>(ApiOption.GetCategoryByID, paras);
            if (result.error_code == 0)
            {
                return result.result;
            }
            else
            {
                return new CategoryEntity();
            }
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
