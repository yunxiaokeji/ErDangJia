using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntFactory.Sdk;
namespace UnitTestIntFactory
{
    [TestClass]
    public class UnitTestCustomer
    {
        string userID = "BC6802E9-285C-471C-8172-3867C87803E2";
        string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466";
        string customerid = "";

        [TestMethod]
        public void GetTasks()
        {
            var result = CustomerBusiness.BaseBusiness.GetCustomerByID(customerid, agentID);
            Assert.IsTrue(result.error_code==0);
        }

        [TestMethod]
        public void GetTaskDetail()
        {
            var result = CustomerBusiness.BaseBusiness.GetCustomerByMobilePhone("111111111111", agentID, "name");
            Assert.IsTrue(result.error_code == 0);
        }

        [TestMethod]
        public void GetOrderInfo()
        {
            var result = CustomerBusiness.BaseBusiness.SetCustomerYXinfo(customerid,agentID,"", "","");
            Assert.IsTrue(result.error_code == 0);
        }
    }
}
