using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    public class LotteryDrawDal
    {
        /// <summary>
        /// 抽奖类Dal实例
        /// </summary>
        public volatile static LotteryDrawDal Instance = new LotteryDrawDal();

        /// <summary>
        /// 插入初始化的用户抽奖数据
        /// </summary>
        /// <param name="lotteryDrawModel">抽奖数据</param>
        public void InsertInitLotteryDrawData(LotteryDrawModel lotteryDrawModel)
        {
            MySqlHelper.ExecuteNonQuery(CommandType.Text, "INSERT INTO lotterydraw (userName,userflag) VALUES ('" + lotteryDrawModel.UserName + "','" + lotteryDrawModel.UserFlag + "')");
        }

        /// <summary>
        /// 获取用户抽奖数据
        /// </summary>
        /// <returns>抽奖数据</returns>
        public List<LotteryDrawModel> GetLotteryDrawData()
        {
            return MySqlHelper.GetDataList<LotteryDrawModel>(CommandType.Text, "SELECT * FROM lotterydraw");
        }
    }
}