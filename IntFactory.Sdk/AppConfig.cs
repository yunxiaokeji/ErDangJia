using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class AppConfig
    {
        public static string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["CMApiUrl"] ?? "http://localhost:9999/";
        //public static string AppKey = "BC6802E9-285C-471C-8172-3867C87803E2";
        //public static string AppSecret = "9F8AF979-8A3B-4E23-B19C-AB8702988466";

        public static string AppKey = System.Configuration.ConfigurationManager.AppSettings["CMAppKey"] ?? "f64bd98d-dc45-4d3b-891d-7601f4ee71b4";
        public static string AppSecret = System.Configuration.ConfigurationManager.AppSettings["CMAppSecret"] ?? "ff6a2c17-2937-4a21-a7f1-ae74c4836315";
        public static string CallBackUrl = System.Configuration.ConfigurationManager.AppSettings["CMCallBackUrl"] ?? "http://localhost:8888/Home/CMCallBack";
    }
}
