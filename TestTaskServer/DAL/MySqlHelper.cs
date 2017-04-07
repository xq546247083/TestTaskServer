using System;
using System.Data;

namespace TestTaskServer
{
    using MySql.Data.MySqlClient;

    /// <summary>
    ///操作数据库类
    /// </summary>
    public abstract class MySqlHelper
    {
        //数据库连接字符串
        public static string Conn = System.Configuration.ConfigurationManager.AppSettings["ConnStr"];

        /// <summary>
        ///  给数据库用假设参数执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);
                try
                {
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return val;
                }
                catch (Exception ex)
                {
                    throw ex;
                } 
            }
        }

        
        /// <summary>
        ///使用事务执行多个sql命令，且读取数据限制
        /// </summary>
        /// <param name="cmdTextArray">sql命令语句集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteTranNonQuery(params string[] cmdTextArray)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction sqlTransaction =null;

            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                //设置命令
                foreach (string cmdText in cmdTextArray)
                {
                    cmd.CommandText += cmdText + " ";
                }
                try
                {
                    sqlTransaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Connection = conn;
                    cmd.Transaction = sqlTransaction;
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    sqlTransaction.Commit();
                    return val;
                }
                catch (Exception ex)
                {
                    if (sqlTransaction!=null)
                    {
                        sqlTransaction.Rollback();
                    }

                    throw ex;
                }
            } 
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public static DataSet GetDataSet( CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand();
            //创建一个MySqlConnection对象
            MySqlConnection conn = new MySqlConnection(Conn);

            try
            {
                //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
                PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);
                //调用 MySqlCommand  的 ExecuteReader 方法
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();

                adapter.Fill(ds);
                //清除参数
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
        /// <param name="cmdText">命令文本,例如:Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

    }
}