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
        private String _flag;
        private String _msg;
        private DataTable _dataTable;
        private string _data;

        public BaseMsg(String flag, String msg,DataTable dataTable)
        {
            this._flag = flag;
            this._msg = msg;
            this._dataTable = dataTable;
        }

        /// <summary>
        /// 是否成功标志
        /// </summary>
        [DataMember(Order = 0)]
        public String Flag
        {
            get 
            { 
                return _flag; 
            }
            set 
            { 
                _flag = value;
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
                return _msg;
            }
            set 
            {
                _msg = value;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        [DataMember(Order = 2)]
        public string Data
        {
            get 
            {
                //将datatable转化为json数组对象
                if (_dataTable==null||_dataTable.Rows.Count == 0)
                {
                    return "";
                }

                StringBuilder jsonBuilder = new StringBuilder();
                jsonBuilder.Append("[");
                for (int i = 0; i < _dataTable.Rows.Count; i++)
                {
                    jsonBuilder.Append("{");
                    for (int j = 0; j < _dataTable.Columns.Count; j++)
                    {
                        jsonBuilder.Append("'");
                        jsonBuilder.Append(_dataTable.Columns[j].ColumnName);
                        jsonBuilder.Append("':'");
                        jsonBuilder.Append(_dataTable.Rows[i][j].ToString());
                        jsonBuilder.Append("',");
                    }

                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                    jsonBuilder.Append("},");
                }

                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]");

                return jsonBuilder.ToString();
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// 数据表
        /// </summary>
        public DataTable DataTable
        {
            get 
            {
                return _dataTable; 
            }
            set 
            { 
                _dataTable = value; 
            }
        }
    }
}