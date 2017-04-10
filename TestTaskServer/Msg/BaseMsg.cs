using System;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace TestTaskServer
{
    /// <summary>
    /// 消息类
    /// </summary>
    [Serializable]
    [DataContractAttribute]
    public class BaseMsg
    {     
        private String flag;
        private String msg;
        private DataTable dataTable;
        private String data;

        public BaseMsg(String flag, String msg,DataTable dataTable)
        {
            this.flag = flag;
            this.msg = msg;
            this.dataTable = dataTable;
        }

        /// <summary>
        /// 是否成功标志
        /// </summary>
        [DataMember(Order = 0)]
        public String Flag
        {
            get 
            { 
                return flag; 
            }
            set 
            { 
                flag = value;
            }
        }

        /// <summary>
        /// 消息
        /// </summary>
        [DataMember(Order = 1)]
        public String Msg
        {
            get 
            {
                return msg;
            }
            set 
            {
                msg = value;
            }
        }

        /// <summary>
        /// 转数据为字符串
        /// </summary>
        [DataMember(Order = 2)]
        public String Data
        {
            get 
            {
                //将datatable转化为json数组对象
                if (dataTable==null||dataTable.Rows.Count == 0)
                {
                    return "";
                }

                StringBuilder jsonBuilder = new StringBuilder();
                jsonBuilder.Append("[");
                for (Int32 i = 0; i < dataTable.Rows.Count; i++)
                {
                    jsonBuilder.Append("{");
                    for (Int32 j = 0; j < dataTable.Columns.Count; j++)
                    {
                        jsonBuilder.Append("'");
                        jsonBuilder.Append(dataTable.Columns[j].ColumnName);
                        jsonBuilder.Append("':'");
                        jsonBuilder.Append(dataTable.Rows[i][j].ToString());
                        jsonBuilder.Append("',");
                    }

                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                    jsonBuilder.Append("},");
                }

                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]");
                data=jsonBuilder.ToString();

                return data;
            }
            set
            {
                data = value;
            }
        }

        /// <summary>
        /// 数据表
        /// </summary>
        public DataTable DataTable
        {
            get 
            {
                return dataTable; 
            }
            set 
            { 
                dataTable = value; 
            }
        }
    }
}