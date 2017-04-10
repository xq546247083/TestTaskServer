﻿using System;
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
        public static string Conn = ConfigurationManager.AppSettings["ConnStr"];

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

                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val; 
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

                    ///执行操作
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    sqlTransaction.Commit();
                    return val;
                }
                catch (Exception ex)
                {
                    //捕获到错误后，先回滚错误，并将错误消息抛出给顶层显示
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
            MySqlConnection conn = new MySqlConnection(Conn);

            //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
            PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            //清除参数
            cmd.Parameters.Clear();
            conn.Close();
            return ds;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public static List<T> GetDataList<T>(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) where T : new()
        {
            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(Conn);

            //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
            PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            //清除参数
            cmd.Parameters.Clear();
            conn.Close();
            return ModelHandler<T>.FillModel(ds); 
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