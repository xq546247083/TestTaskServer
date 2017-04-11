using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace TestTaskServer
{
    /// <summary>
    /// 抽奖信息Dal类
    /// </summary>
    public class LotteryDrawDal
    {
        /// <summary>
        /// 抽奖类Dal的内部字段
        /// </summary>
        private static LotteryDrawDal mInstance = new LotteryDrawDal();

        /// <summary>
        /// 抽奖信息Dal实例
        /// </summary>
        public static LotteryDrawDal Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 插入初始化的用户抽奖数据
        /// </summary>
        /// <param name="lotteryDrawModel">抽奖数据</param>
        public void InsertInitLotteryDrawData(LotteryDrawModel lotteryDrawModel)
        {
            MySqlParameter[] mySqlParameter = new MySqlParameter[]
            {
                new MySqlParameter("@userName",MySqlDbType.VarChar,32),
                new MySqlParameter("@userflag",MySqlDbType.VarChar,32)
            };
            mySqlParameter[0].Value = lotteryDrawModel.UserName;
            mySqlParameter[1].Value = lotteryDrawModel.UserFlag;

            MySqlHelper.ExecuteNonQuery(CommandType.Text, "INSERT INTO lotterydraw (userName,userflag) VALUES (@userName,@userflag)", mySqlParameter);
        }

        /// <summary>
        /// 获取用户抽奖数据
        /// </summary>
        /// <returns>抽奖数据</returns>
        public List<LotteryDrawModel> GetLotteryDrawData()
        {
            return MySqlHelper.GetDataList<LotteryDrawModel>(CommandType.Text, "SELECT * FROM lotterydraw");
        }

        /// <summary>
        /// 更新用户抽奖信息
        /// </summary>
        /// <param name="mySqlTransaction">事务</param>
        /// <param name="lotteryDrawModel">抽奖信息</param>
        public Int32 UpdateLotteryDrawInfo(MySqlTransaction mySqlTransaction, LotteryDrawModel lotteryDrawModel)
        {
            //参数
            MySqlParameter[] mySqlParameter = new MySqlParameter[]
            {
                new MySqlParameter("@userFlag",MySqlDbType.VarChar,32),
                new MySqlParameter("@pointsStr",MySqlDbType.VarChar,32),
                new MySqlParameter("@lastLotteryDrawTime",MySqlDbType.DateTime)
            };
            mySqlParameter[0].Value = lotteryDrawModel.UserFlag;
            mySqlParameter[1].Value = lotteryDrawModel.Points + 10;
            mySqlParameter[1].Value = DateTime.Now;

            //更新用户信息
            String lotteryDrawPointStr = "UPDATE lotterydraw SET POINTS= @pointsStr,LastLotteryDrawTime=@lastLotteryDrawTime WHERE UserFlag= @userFlag;";
            return MySqlHelper.ExecuteNonQuery(mySqlTransaction, CommandType.Text, lotteryDrawPointStr, mySqlParameter);
        }
    }
}