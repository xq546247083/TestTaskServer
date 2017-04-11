using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace TestTaskServer
{
    using MySql.Data.MySqlClient;

    /// <summary>
    ///操作数据库类
    /// </summary>
    public abstract class MySqlHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static String Conn = ConfigurationManager.AppSettings["ConnStr"];

        /// <summary>
        ///  给数据库用假设参数执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static Int32 ExecuteNonQuery(CommandType cmdType, String cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                Int32 val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                return val;
            }
        }

        ///  <summary>
        /// 使用事务执行多个sql命令，且读取数据限制
        ///  </summary>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="cmdTextArray">sql命令语句集合</param>
        ///  <returns>执行命令所影响的行数</returns>
        public static Int32 ExecuteTranNonQuery(MySqlParameter[] commandParameters, params String[] cmdTextArray)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction sqlTransaction = null;

            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                //设置命令
                foreach (String cmdText in cmdTextArray)
                {
                    cmd.CommandText += cmdText + " ";
                }

                if (commandParameters != null)
                {
                    foreach (MySqlParameter parm in commandParameters)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }

                try
                {
                    sqlTransaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Connection = conn;
                    cmd.Transaction = sqlTransaction;

                    //执行操作
                    Int32 val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    sqlTransaction.Commit();

                    return val;
                }
                catch (Exception)
                {
                    //捕获到错误后，先回滚错误，并将错误消息抛出给顶层显示
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///使用现有的SQL事务执行一个sql命令（不返回数据集）
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="trans">一个现有的事务</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static Int32 ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return val;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public static DataSet GetDataSet(CommandType cmdType, String cmdText, params MySqlParameter[] commandParameters)
        {
            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter { SelectCommand = cmd };
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                //清除参数
                cmd.Parameters.Clear();

                return ds;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public static List<T> GetDataList<T>(CommandType cmdType, String cmdText, params MySqlParameter[] commandParameters) where T : new()
        {
            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                //清除参数
                cmd.Parameters.Clear();

                return ModelHandler<T>.FillModel(ds);
            }
        }

        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">数据库连接</param>
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
        /// <param name="cmdText">命令文本,例如:Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, String cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //准备cmd命令的参数
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (trans != null)
            {
                cmd.Transaction = trans;

            }

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}