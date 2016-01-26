using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CloudSalesEnum;
using CloudSalesEntity;

namespace YXERP.Models
{
    [Serializable]
    public class FilterUser
    {
        public string Keywords { get; set; }

        public string DepartID { get; set; }

        public string RoleID { get; set; }

        public int PageIndex { get; set; }

    }
}