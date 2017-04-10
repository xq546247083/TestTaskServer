using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace TestTaskServer
{
    /// <summary>
    /// DataTable与实体类互相转换
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    public class ModelHandler<T> where T : new()
    {
        #region DataTable转换成实体类

        /// <summary>
        /// 填充对象列表：用DataSet的第一个表填充实体类
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>数据集</returns>
        public static List<T> FillModel(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FillModel(ds.Tables[0]);
            }
        }

        /// <summary>
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>
        /// <param name="dt">表</param>
        /// <returns>数据集</returns>
        public static List<T> FillModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T model = new T();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }

            return modelList;
        }

        #endregion
    }
}