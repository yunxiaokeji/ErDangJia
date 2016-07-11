using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk.Business
{
    public class ClientBusiness
    {
        public static ClientBusiness BaseBusiness = new ClientBusiness();

        public ClientResult GetClientInfo(string zngcClientID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("zngcClientID", zngcClientID);

            return HttpRequest.RequestServer<ClientResult>(ApiOption.GetClientInfo, paras);
        }
    }
}
