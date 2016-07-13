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

        public CustomerResult GetCustomerByID(string zngcCustomerID, string zngcClientID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("customerID", zngcCustomerID);
            paras.Add("clientID", zngcClientID);

            return HttpRequest.RequestServer<CustomerResult>(ApiOption.GetCustomerByID, paras);
        }

        public CustomerResult GetCustomerByMobilePhone(string mobilePhone, string name, string zngcClientID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("mobilePhone", mobilePhone);
            paras.Add("clientID", zngcClientID);
            paras.Add("name", name);

            return HttpRequest.RequestServer<CustomerResult>(ApiOption.GetCustomerByMobilePhone, paras);
        }

        public UpdateResult SetCustomerYXinfo(string customerID, string name, string mobilePhone, string zngcClientID, string yxAgentID, string yxClientID, string yxClientCode)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("customerID", customerID);
            paras.Add("name", name);
            paras.Add("mobilePhone", mobilePhone);
            paras.Add("clientID", zngcClientID);
            paras.Add("yxAgentID", yxAgentID);
            paras.Add("yxClientID", yxClientID);
            paras.Add("yxClientCode", yxClientCode);

            return HttpRequest.RequestServer<UpdateResult>(ApiOption.SetCustomerYXinfo, paras);
        }
    }
}
