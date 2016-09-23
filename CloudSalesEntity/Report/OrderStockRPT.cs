using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class OrderStockRPT
    {
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string ProductCode { get; set; }
        public string Remark { get; set; }
        public int Quantity { get; set; }
        public int QCQuantity { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int JYQuantity { get; set; }
        public int InQuantity { get; set; }
        public int OutQuantity { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal TotalMoney { get; set; }
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
