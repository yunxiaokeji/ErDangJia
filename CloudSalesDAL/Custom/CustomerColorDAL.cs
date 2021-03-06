﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL.Custom
{
    public class CustomerColorDAL : BaseDAL
    {
        public static CustomerColorDAL BaseProvider = new CustomerColorDAL();

        public DataTable GetCustomerColors(string clientid, int colorid=0)
        {
            string sqlText = "select  *  from CustomerColor where status <>9 and ClientID=@ClientID ";
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid), };
            if (colorid>0)
            {
                paras.SetValue(new SqlParameter("@ColorID", colorid), paras.Length);
                sqlText += " and ColorID=@ColorID";
            }
            sqlText += "   order by ColorID asc ";
            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public int InsertCustomerColor(string colorName,  string colorValue,string agentid, string clientid, string userid ,int status=0 )
        {
            int result = 0;
            SqlParameter[] paras = {  new SqlParameter("@Result",result),
                                     new SqlParameter("@ColorName",colorName),
                                     new SqlParameter("@ColorValue",colorValue), 
                                     new SqlParameter("@CreateUserID" , userid), 
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_InsertCustomerColor", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result;
        }
        

        public bool UpdateCustomerColor (string agentid, string clientid, int colorid, string colorName, string colorValue,string updateUserId)
        {
            SqlParameter[] paras = {  
                                     new SqlParameter("@ColorID",colorid),
                                     new SqlParameter("@ColorName",colorName),
                                     new SqlParameter("@ColorValue" , colorValue),
                                     new SqlParameter("@UpdateTime" , DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")), 
                                     new SqlParameter("@UpdateUserID" , updateUserId),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            string updateSql =
                @"update CustomerColor set ColorName=@ColorName,ColorValue=@ColorValue,UpdateUserID=@UpdateUserID,UpdateTime=@UpdateTime
                   where  AgentID=@AgentID and ClientID=@ClientID and ColorID=@ColorID";
            return ExecuteNonQuery(updateSql, paras, CommandType.Text) > 0; 
        }

        public bool DeleteCustomColor(int colorid, string agentid, string clientid, string userid)
        {
            SqlParameter[] paras = {  
                                     new SqlParameter("@ColorID",colorid),
                                     new SqlParameter("@UpdateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteCustomColor", paras, CommandType.StoredProcedure) > 0;
        }
    }
}
