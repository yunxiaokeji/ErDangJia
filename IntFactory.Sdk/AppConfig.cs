using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class AppConfig
    {
        public static string ApiUrl =System.Configuration.ConfigurationManager.AppSettings["ApiUrl"]?? "http://90cloudsales.com";
        public static string AppKey = "BC6802E9-285C-471C-8172-3867C87803E2";
        public static string AppSecret = "9F8AF979-8A3B-4E23-B19C-AB8702988466";
        public static string CallBackUrl = "http://gyl.movedbuy.com";
    }
}
