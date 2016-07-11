using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class CustomerBusiness
    {
        public static CustomerBusiness BaseBusiness = new CustomerBusiness();

        public CustomerResult GetCustomerByID(string customerID, string clientID) {
            var paras = new Dictionary<string, object>();
            paras.Add("customerID", customerID);
            paras.Add("clientID", clientID);

            return HttpRequest.RequestServer<CustomerResult>(ApiOption.GetCustomerByID, paras);
        }

        public CustomerResult GetCustomerByMobilePhone(string mobilePhone, string clientID, string name)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("mobilePhone", mobilePhone);
            paras.Add("clientID", clientID);
            paras.Add("name", name);

            return HttpRequest.RequestServer<CustomerResult>(ApiOption.GetCustomerByMobilePhone, paras);
        }

        public UpdateResult SetCustomerYXinfo(string customerID, string name, string mobilePhone, string clientID, string yxAgentID, string yxClientID, string yxClientCode)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("customerID", customerID);
            paras.Add("name", name);
            paras.Add("mobilePhone", mobilePhone);
            paras.Add("clientID", clientID);
            paras.Add("yxAgentID", yxAgentID);
            paras.Add("yxClientID", yxClientID);
            paras.Add("yxClientCode", yxClientCode);

            return HttpRequest.RequestServer<UpdateResult>(ApiOption.SetCustomerYXinfo, paras);
        }
    }
}
