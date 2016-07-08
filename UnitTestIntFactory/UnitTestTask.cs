using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntFactory.Sdk;
namespace UnitTestIntFactory
{
    [TestClass]
    public class UnitTestTask
    {
        string userID = "BC6802E9-285C-471C-8172-3867C87803E2";
        string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466";

        [TestMethod]
        public void GetTasks()
        {
            var result= TaskBusiness.BaseBusiness.GetTasks(new FilterTasks(),userID, agentID);
            Assert.IsTrue(result.error_code==0);
        }

        [TestMethod]
        public void GetTaskDetail()
        {
            var taskID = "8e266ab4-0ff6-499b-89ce-d2e3454be123";
            var result = TaskBusiness.BaseBusiness.GetTaskDetail(taskID, userID, agentID);
            Assert.IsTrue(result.error_code == 0);
        }

        [TestMethod]
        public void GetOrderInfo()
        {
            var orderID = "f9c9f321-e0d3-4458-b875-27a1821d552e";
            var result = TaskBusiness.BaseBusiness.GetOrderInfo(orderID, userID, agentID);
            Assert.IsTrue(result.error_code == 0);
        }
    }
}
