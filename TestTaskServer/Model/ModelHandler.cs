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
            //如果没有数据，返回null，否则返回其第一个表对应的List
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return FillModel(ds.Tables[0]);

        }

        /// <summary>
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>
        /// <param name="dt">表</param>
        /// <returns>数据集</returns>
        public static List<T> FillModel(DataTable dt)
        {
            //如果表为空或者没有数据，返回null
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            //将DataTable数据转换为List
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T model = new T();
                for (Int32 i = 0; i < dr.Table.Columns.Count; i++)
                {
                    //把表格的数据设置到List
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }

            return modelList;
        }

        #endregion

        #region 实体类转换List

        /// <summary>
        /// 转换list到DataTable
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="items">数据</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ToDataTable(List<T> items)
        {
            //创建T类型对应的表格
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            //为表格添加数据
            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (Int32 i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// 判断该类型不为值类型
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns>返回值</returns>
        public static Boolean IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 返回对应的type
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回类型</returns>
        public static Type GetCoreType(Type type)
        {
            if (type != null && IsNullable(type))
            {
                return !type.IsValueType ? type : Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        #endregion
    }
}