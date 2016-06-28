using CloudSalesBusiness;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    [YXERP.Common.UserAuthorize]
    public class BaseController : Controller
    {

        /// <summary>
        /// 默认分页Size
        /// </summary>
        protected int PageSize = 20;

        /// <summary>
        /// 登录IP
        /// </summary>
        protected string OperateIP
        {
            get 
            {
                return string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
            }
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        protected CloudSalesEntity.Users CurrentUser
        {
            get
            {
                if (Session["ClientManager"] == null)
                {
                    return null;
                }
                else
                {
                    return (CloudSalesEntity.Users)Session["ClientManager"];
                }
            }
            set { Session["ClientManager"] = value; }
        }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        public string GetCityCode(DataRow dr)
        {
            string cityCode = GetCityCode(dr, 0);
            cityCode = string.IsNullOrEmpty(cityCode) ? GetCityCode(dr, 2) : cityCode;
            return cityCode;
        }

        public string GetCityCode(DataRow dr, int sublen=0)
        {
            string cityCode="";
            if (dr["省"]==null  || string.IsNullOrEmpty(dr["省"].ToString()))
            {
                return "";
            }
            string province = dr["省"].ToString();
            if (sublen == 0)
            {
                sublen = province.Length;
            }
            else if( sublen > province.Length)
            {
                sublen = province.Length;
            }
            province = province.Substring(0, sublen);
            var sheng = CommonBusiness.Citys.Where(x => x.Level == 1 && x.Name.Contains(province)).ToList();
            if (sheng.Any())
            {
                int shilen = dr["市"].ToString().Length > 2 ? 2 : dr["市"].ToString().Length;
                int qulen = dr["区"].ToString().Length > 2 ? 2 : dr["区"].ToString().Length;
                cityCode = sheng.FirstOrDefault().CityCode; 
                var shi =
                    CommonBusiness.Citys.Where(
                        x => x.Level == 2 && x.PCode == cityCode && x.Name.Contains(dr["市"].ToString().Substring(0, shilen)))
                        .ToList();
                if (shi.Any())
                {
                    cityCode = shi.FirstOrDefault().CityCode;
                    if (!string.IsNullOrEmpty(dr["区"].ToString()))
                    {
                        var qu = CommonBusiness.Citys.Where(
                            x =>
                                x.Level == 3 && x.PCode == cityCode &&
                                x.Name.Contains(dr["区"].ToString().Substring(0, qulen))).ToList();
                        if (qu.Any())
                        {
                            cityCode = qu.FirstOrDefault().CityCode;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(dr["区"].ToString()))
                    {
                        var qu = CommonBusiness.Citys.Where(
                            x =>
                                x.Level == 3 && x.Province == province &&
                                x.Name.Contains(dr["区"].ToString().Substring(0, qulen))).ToList();
                        if (qu.Any())
                        {
                            cityCode = qu.FirstOrDefault().CityCode;
                        }
                    }
                }
            }
            return cityCode;
        }

        /// <summary>
        /// Excel转table 导入 Michaux 20160531
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public DataTable ImportExcelToDataTable(HttpPostedFileBase file,Dictionary<string,ExcelFormatter> formatColumn=null)
        {
            var datatable = new DataTable();
            if (file.FileName.IndexOf("xlsx") > -1)
            {
                NPOI.XSSF.UserModel.XSSFWorkbook Upfile = new NPOI.XSSF.UserModel.XSSFWorkbook(file.InputStream);
                var sheet = Upfile.GetSheetAt(0);
                var firstRow = sheet.GetRow(0);
                var buffer = new byte[file.ContentLength];
                for (int cellIndex = firstRow.FirstCellNum; cellIndex < firstRow.LastCellNum; cellIndex++)
                {
                    datatable.Columns.Add(firstRow.GetCell(cellIndex).StringCellValue, typeof(string));
                }
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow datarow = datatable.NewRow();
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    bool con = true;
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        //if (formatColumn!=null && formatColumn.ContainsKey(firstRow.GetCell(j).StringCellValue))
                        //{
                           
                        //}
                        var cell = row.GetCell(j);
                        if (cell == null)
                        {
                            datarow[j] = "";
                            continue;
                        }
                        switch (cell.CellType)
                        {
                            case CellType.Numeric:
                                datarow[j] = cell.NumericCellValue;
                                break;
                            case CellType.String:
                                datarow[j] = cell.StringCellValue;
                                break;
                            case CellType.Blank:
                                datarow[j] = "";
                                break;
                            case CellType.Formula:
                                switch (row.GetCell(j).CachedFormulaResultType)
                                {
                                    case CellType.String:
                                        string strFORMULA = row.GetCell(j).StringCellValue;
                                        if (strFORMULA != null && strFORMULA.Length > 0)
                                        {
                                            datarow[j] = strFORMULA.ToString();
                                        }
                                        else
                                        {
                                            datarow[j] = null;
                                        }
                                        break;
                                    case CellType.Numeric:
                                        datarow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                        break;
                                    case CellType.Boolean:
                                        datarow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                        break;
                                    case CellType.Error:
                                        datarow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                        break;
                                    default:
                                        datarow[j] = "";
                                        break;
                                }
                                break;
                            default:
                                con = false;
                                break;
                        }
                        if (!con)
                        {
                            break;
                        }
                    }
                    if (con)
                    {
                        datatable.Rows.Add(datarow);
                    }
                }
            }
            else
            {
                NPOI.HSSF.UserModel.HSSFWorkbook Upfile = new NPOI.HSSF.UserModel.HSSFWorkbook(file.InputStream);
                var sheet = Upfile.GetSheetAt(0);
                var firstRow = sheet.GetRow(0);
                var buffer = new byte[file.ContentLength];
                for (int cellIndex = firstRow.FirstCellNum; cellIndex < firstRow.LastCellNum; cellIndex++)
                {
                    datatable.Columns.Add(firstRow.GetCell(cellIndex).StringCellValue, typeof(string));
                }
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow datarow = datatable.NewRow();
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    bool con = true;
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)
                        {
                            datarow[j] = "";
                            continue;
                        }
                        switch (cell.CellType)
                        {
                            case CellType.Numeric:
                                datarow[j] = cell.NumericCellValue;
                                break;
                            case CellType.String:
                                datarow[j] = cell.StringCellValue;
                                break;
                            case CellType.Blank:
                                datarow[j] = "";
                                break;
                            case CellType.Formula:
                                switch (row.GetCell(j).CachedFormulaResultType)
                                {
                                    case CellType.String:
                                        string strFORMULA = row.GetCell(j).StringCellValue;
                                        if (strFORMULA != null && strFORMULA.Length > 0)
                                        {
                                            datarow[j] = strFORMULA.ToString();
                                        }
                                        else
                                        {
                                            datarow[j] = null;
                                        }
                                        break;
                                    case CellType.Numeric:
                                        datarow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                        break;
                                    case CellType.Boolean:
                                        datarow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                        break;
                                    case CellType.Error:
                                        datarow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                        break;
                                    default:
                                        datarow[j] = "";
                                        break;
                                }
                                break;
                            default:
                                con = false;
                                break;
                        }
                        if (!con)
                        {
                            break;
                        }
                    }
                    if (con)
                    {
                        datatable.Rows.Add(datarow);
                    }
                }
            }
            #region 清除最后的空行
            for (int i = datatable.Rows.Count - 1; i > 0; i--)
            {
                bool isnull = true;
                for (int j = 0; j < datatable.Columns.Count; j++)
                {
                    if (datatable.Rows[i][j] != null)
                    {
                        if (datatable.Rows[i][j].ToString() != "")
                        {
                            isnull = false;
                            break;
                        }
                    }
                }
                if (isnull)
                {
                    datatable.Rows[i].Delete();
                }
            }
            #endregion
            return datatable;
        }

        public string GetExcelModel(string modelName)
        {
            string path = Server.MapPath("~/") +"modules/excelmodel/"+ modelName+".json";
            string jsonStr = "";
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    jsonStr += line.ToString();
                }
            }
            catch (Exception)
            {
                jsonStr = "{\"Error\":\"未找到模板也\",\"Item\":{}}";
                    throw;
            }
            return jsonStr;
        }
        public string GetJsonValue(JEnumerable<JToken> jToken, string key)
        {
            IEnumerator enumerator = jToken.GetEnumerator();
            while (enumerator.MoveNext())
            {
                JToken jc = (JToken)enumerator.Current;
                if (jc is JObject || ((JProperty)jc).Value is JObject)
                {
                    return GetJsonValue(jc.Children(), key);
                }
                else
                {
                    if (((JProperty)jc).Name == key)
                    {
                        return ((JProperty)jc).Value.ToString();
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 导出模版获取
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="formatColumn"></param>
        /// <param name="key"></param>
        /// <param name="test"></param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public Dictionary<string, ExcelModel> GetColumnForJson(string modelName, ref Dictionary<string, ExcelFormatter> formatColumn, string key = "", string test = "", string clientID = "")
        {
            Dictionary<string, ExcelModel> dic = new Dictionary<string, ExcelModel>();
            string path = Server.MapPath("~/") + "modules/excelmodel/" + modelName + ".json";
            string jsonStr = "";
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = "Item";
                }
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    jsonStr += line.ToString();
                }
                JObject jObject = (JObject) JsonConvert.DeserializeObject(jsonStr);
                Dictionary<string, object> jDic =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(jObject[key].ToString());
                foreach (var keyvalue in jDic)
                {
                    JObject jChild = (JObject) JsonConvert.DeserializeObject(keyvalue.Value.ToString());
                    ExcelModel excelModel = GetExcelModel(keyvalue.Key.ToLower(),jChild);
                    if (!excelModel.IsHide)
                    {
                        dic.Add(keyvalue.Key.ToLower(), excelModel);
                        int columnType = Convert.ToInt32(jChild[test+"type"]);
                        if (excelModel.IsfFomat && columnType > 0)
                        {
                            string dataList = excelModel.DataSource;
                            string[] distType = dataList.Split('|');
                            string dropSource = "";
                            if (distType.Length > 1)
                            {
                                if (distType[0] == "List")
                                {
                                    dropSource = distType[1];
                                }
                                else
                                {
                                    if (Convert.ToInt32(distType[1]) > 0)
                                    {
                                        dropSource =
                                            CommonBusiness.GetDropList(
                                                (DropSourceList) Enum.Parse(typeof (DropSourceList), distType[1]),clientID);
                                    }
                                }
                            }
                            formatColumn.Add(keyvalue.Key.ToLower(), new ExcelFormatter()
                            {
                                ColumnTrans =
                                    (EnumColumnTrans)
                                        Enum.Parse(typeof(EnumColumnTrans), columnType.ToString()),
                                DropSource = dropSource
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                dic.Add("Error", new ExcelModel(){Title = "未找到模版"});
                throw;
            }
            return dic;
        }
        /// <summary>
        /// 导入对比模版获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jChild"></param>
        /// <returns></returns>
        public ExcelModel GetExcelModel(string key,JObject jChild)
        {
            ExcelModel excelModel = new ExcelModel();
            excelModel.Title = jChild["title"].ToString();
            excelModel.ColumnName = key;
            excelModel.IsHide = Convert.ToBoolean(jChild["hide"]);
            excelModel.IsfFomat = Convert.ToBoolean(jChild["format"]);
            excelModel.Type = Convert.ToInt32(jChild["type"]);
            excelModel.TestType = Convert.ToInt32(jChild["testtype"]);
            excelModel.DataSource = jChild["datasource"].ToString();
            excelModel.DefaultText = jChild["defaulttext"].ToString();
            return excelModel;
        }

        public Dictionary<string, string> GetColumnForJson(string modelName, string key = "Item")
        {
             Dictionary<string, string> dic = new Dictionary<string, string>();
            string path = Server.MapPath("~/") + "modules/excelmodel/" + modelName + ".json";
            string jsonStr = "";
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = "Item";
                }
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    jsonStr += line.ToString();
                }
                JObject jObject = (JObject) JsonConvert.DeserializeObject(jsonStr);
                Dictionary<string, object> jDic =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(jObject[key].ToString());
                foreach (var keyvalue in jDic)
                {
                    JObject jChild = (JObject)JsonConvert.DeserializeObject(keyvalue.Value.ToString());
                    if (!Convert.ToBoolean(jChild["hide"]))
                    {
                        dic.Add(jChild["title"].ToString(), keyvalue.Key.ToLower());
                    }
                }
            }
            catch (Exception)
            {
                dic.Add("Error", "未找到模板");
                throw;
            }
            return dic;
        }
    }
}
