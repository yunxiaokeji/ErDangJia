using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntFactory.Sdk;
namespace UnitTestIntFactory
{
    [TestClass]
    public class UnitTestCustomer
    {
        string agentID = "a89cbb94-e32b-4f99-bab9-2db1d9cff607";
        string customerid = "adfce9f6-1b81-4bc2-a856-428ab6102050";

        [TestMethod]
        public void GetCustomerByID()
        {
            var result = CustomerBusiness.BaseBusiness.GetCustomerByID(customerid, agentID);
            Assert.IsNotNull(result.customer.customerID);
            //Assert.IsTrue(result.error_code==0);
        }

        //[TestMethod]
        public void GetCustomerByMobilePhone()
        {
            var result = CustomerBusiness.BaseBusiness.GetCustomerByMobilePhone("111111111111", agentID, "name");
            Assert.IsTrue(result.error_code == 0);
        }

        //[TestMethod]
        public void SetCustomerYXinfo()
        {
            var result = CustomerBusiness.BaseBusiness.SetCustomerYXinfo(customerid,"","",agentID,"", "","");
            Assert.IsTrue(result.error_code == 0);
        }
    }
}
