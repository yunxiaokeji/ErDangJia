using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using CloudSalesEnum;
//using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.POIFS.NIO;
using NPOI.SS.Util;

namespace CloudSalesBusiness
{
   
    public class ExcelWriter
    {
        public ExcelWriter()
        {
            Maps = new List<ExcelPopertyMap>();
        }

        public List<ExcelPopertyMap> Maps { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        /// <param name="key"> DataTable中的列名 此处不区分大小写</param>
        /// <param name="colName">Excel的列名</param>
        public void Map(string key, string colName)
        {
            string info = key;           
            Maps.Add(new ExcelPopertyMap(info)
            {
                Name = colName
            });
        }
        /// <summary>
        /// 导出Excel List 
        /// </summary> 
        /// <param name="records"></param>    
        /// <returns></returns>
        public byte[] Write(IEnumerable<Dictionary<string, string>> records)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var workBook = new XSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            var sheet = workBook.CreateSheet();
            var headerRow = sheet.CreateRow(0);
            var cellIndex = 0;
            foreach (var map in Maps)
            {
                headerRow.CreateCell(cellIndex, CellType.String).SetCellValue(map.Name);
                cellIndex++;
            }
            var rowIndex = 1;
            foreach (var record in records)
            {
                var dr = sheet.CreateRow(rowIndex);
                for (int i = 0; i < Maps.Count; i++)
                {
                    var cellValue = record[Maps[i].Info];
                    dr.CreateCell(i).SetCellValue(cellValue ?? "");
                }
                rowIndex++;
            }
            workBook.Write(ms);
            byte[] buffer = ms.ToArray();
            ms.Close();
            sw.Stop();
            return buffer;
        }
        /// <summary>
        /// 导出Excel 运行时解析 
        /// </summary>
        /// <param name="records">records必须都为字符串</param>   
        /// <returns></returns>
        public byte[] Write(IEnumerable<dynamic> records)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var workBook = new XSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            var sheet = workBook.CreateSheet();
            var headerRow = sheet.CreateRow(0);
            var cellIndex = 0;
            foreach (var map in Maps)
            {
                headerRow.CreateCell(cellIndex, CellType.String).SetCellValue(map.Name);
                cellIndex++;
            }
            if (records.Any())
            {
                var rowIndex = 1;
                var first = records.First();
                Type t = first.GetType();
                PropertyInfo[] propertysInfos = t.GetProperties();
                foreach (var record in records)
                {
                    var dr = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < Maps.Count; i++)
                    {
                        var propertysInfo = propertysInfos.Where(x => x.Name == Maps[i].Info).FirstOrDefault();
                        if (propertysInfo == null)
                        {
                            continue;
                        }
                        var cellValue = propertysInfo.GetValue(record);
                        dr.CreateCell(i).SetCellValue(cellValue ?? "");
                    }
                    rowIndex++;
                }
            }
            workBook.Write(ms);
            byte[] buffer = ms.ToArray();
            ms.Close();
            sw.Stop();
            return buffer;
        }

        /// <summary>
        /// 导出Excel DataTable
        /// </summary> 
        /// <param name="records">records必须都为DataTable</param>   
        /// <param name="formatter">Dictionary key:DataTable中的列明此处必须小写 value:EnumColumnTrans</param>
        /// <returns></returns>
        public byte[] Write(DataTable records, Dictionary<string, ExcelFormatter> formatter = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var workBook = new XSSFWorkbook(); 
            MemoryStream ms = new MemoryStream();
            var sheet = workBook.CreateSheet();
            var headerRow = sheet.CreateRow(0);
            var cellIndex = 0;
            ICellStyle cstyle = workBook.CreateCellStyle();
            cstyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;
            foreach (var map in Maps)
            {
                var hcell = headerRow.CreateCell(cellIndex, CellType.String);
                hcell.CellStyle = cstyle;
                hcell.SetCellValue(map.Name);
                cellIndex++;
            }
            IDataValidationHelper dvHelper = sheet.GetDataValidationHelper();
            var rowIndex = 1;
            foreach (DataRow record in records.Rows)
            {
                var dr = sheet.CreateRow(rowIndex);
                for (int i = 0; i < Maps.Count; i++)
                {
                    string drValue = record[Maps[i].Info.ToString()].ToString();
                    ICell cell= dr.CreateCell(i);
                    if (formatter.Any() && formatter.ContainsKey(Maps[i].Info.ToLower()) && formatter[Maps[i].Info.ToLower()]!=null)
                    {
                        ExcelFormatter excelFormatter = formatter[Maps[i].Info.ToLower()];
                        if (!string.IsNullOrEmpty(drValue))
                        {
                            if (excelFormatter!=null && excelFormatter.ColumnTrans == EnumColumnTrans.ConvertDownList)
                            {
                                cell.SetCellValue(drValue);
                                XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateExplicitListConstraint(excelFormatter.DropSource.Split(','));
                                CellRangeAddressList regions = new CellRangeAddressList(1, 65535, i, i);
                                XSSFDataValidation dataValidate = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, regions);
                                sheet.AddValidationData(dataValidate);
                            }
                            else
                            {
                                cell.SetCellValue(FormatterCoulumn(drValue, excelFormatter.ColumnTrans));
                            }
                        }
                        else { cell.SetCellValue(drValue); }
                    }
                    else
                    {
                        switch (records.Columns[Maps[i].Info].DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                cell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                cell.SetCellValue(dateV);
                                //cell.CellStyle =
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                cell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                cell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                cell.SetCellValue(doubV);
                                break;
                            case "System.DBNull"://空值处理
                                cell.SetCellValue("");
                                break;
                            default:
                                cell.SetCellValue("");
                                break;
                        }
                    }
                }
                rowIndex++;
            }
            workBook.Write(ms);
            byte[] buffer = ms.ToArray();
            ms.Close();
            sw.Stop();
            return buffer;
        }
        public string FormatterCoulumn(string coulumnValue, EnumColumnTrans eumnType)
        {
            switch (eumnType)
            {
                case EnumColumnTrans.ConvertTime:
                    return Convert.ToDateTime(coulumnValue).ToString("yyyy-MM-dd");
                case EnumColumnTrans.ConvertStatus:
                    return CommonBusiness.GetEnumDesc<EnumClientOrderStatus>((EnumClientOrderStatus)Enum.Parse(typeof(EnumClientOrderStatus), coulumnValue));
                case EnumColumnTrans.ConvertOrderStatus:
                    return CommonBusiness.GetEnumDesc<EnumOrderStatus>((EnumOrderStatus)Enum.Parse(typeof(EnumOrderStatus), coulumnValue));
                case EnumColumnTrans.ConvertSendStatus:
                    return CommonBusiness.GetEnumDesc<EnumSendStatus>((EnumSendStatus)Enum.Parse(typeof(EnumSendStatus), coulumnValue));
                case EnumColumnTrans.ConvertOrderPay:
                    return CommonBusiness.GetEnumDesc<EnumClientOrderPay>((EnumClientOrderPay)Enum.Parse(typeof(EnumClientOrderPay), coulumnValue));
                case EnumColumnTrans.ConvertReturnStatus:
                    return CommonBusiness.GetEnumDesc<EnumReturnStatus>((EnumReturnStatus)Enum.Parse(typeof(EnumReturnStatus), coulumnValue));
                case EnumColumnTrans.ConvertExpressType:
                    return CommonBusiness.GetEnumDesc<EnumExpressType>((EnumExpressType)Enum.Parse(typeof(EnumExpressType), coulumnValue));
                case EnumColumnTrans.ConvertCustomerExtent:
                    return  CommonBusiness.GetEnumDesc<EnumCustomerExtend>((EnumCustomerExtend) Enum.Parse(typeof (EnumCustomerExtend), coulumnValue));
                    case EnumColumnTrans.ConvertIndustry:
                    return CommonBusiness.GetIndustryID(coulumnValue);
                default:
                    return coulumnValue;
            }
        }
    }
    /// <summary>
    /// Column特殊列格式化方法选择 Status PayStatus OrderStatus CreateTime 等
    /// </summary>
    public enum EnumColumnTrans
    {
        /// <summary>
        /// 转为日期
        /// </summary>
        ConvertTime =0,
        /// <summary>
        /// 状态转为文字
        /// </summary>
        ConvertStatus = 1,
        /// <summary>
        /// 订单状态转换
        /// </summary>
        ConvertOrderStatus = 2,
        /// <summary>
        /// 出库状态
        /// </summary>
        ConvertSendStatus = 3,
        /// <summary>
        /// 付款状态
        /// </summary>
        ConvertOrderPay=4,
        /// <summary>
        /// 退货状态
        /// </summary>
        ConvertReturnStatus = 5,
        /// <summary>
        /// 配送状态
        /// </summary>
        ConvertExpressType=6,
        /// <summary>
        /// 下拉框
        /// </summary>
        ConvertDownList=7,
        /// <summary>
        /// 公司规模
        /// </summary>
        ConvertCustomerExtent = 8,
        /// <summary>
        /// 公司规模
        /// </summary>
        ConvertIndustry = 9
    }

    public enum DropSourceList
    {
        /// <summary>
        /// 行业
        /// </summary>
        Industry = 1,
        /// <summary>
        /// 省
        /// </summary>
        Province = 2
    }

    public class ExcelFormatter
    {
        public EnumColumnTrans ColumnTrans { get; set; }
        public string DropSource { get; set; }
    }

    public class ExcelPopertyMap
    {
        public readonly string Info;
        public ExcelPopertyMap(string propertyInfo)
        {
            Info = propertyInfo;
        }
        public string Name { get; set; }
    }
    public class CommonUntil {

        public static List<Dictionary<string, string>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> rowItem = new Dictionary<string, string>();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    rowItem.Add(dt.Columns[i].ColumnName, row.ItemArray[i].ToString());
                }
                list.Add(rowItem);
            }
            return list;
        }
    }
}
