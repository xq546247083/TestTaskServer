using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    /// <summary>
    /// 抽奖配置Dal类
    /// </summary>
    public class LotteryDrawConfigDal
    {
        /// <summary>
        /// 抽奖配置Dal实例
        /// </summary>
        private static LotteryDrawConfigDal mInstance = new LotteryDrawConfigDal();

        /// <summary>
        /// 抽奖配置信息Dal实例
        /// </summary>
        public static LotteryDrawConfigDal Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 获取抽奖配置数据
        /// </summary>
        /// <returns>抽奖配置数据</returns>
        public List<LotteryDrawConfigModel> GetLotteryDrawConfigData()
        {
            return MySqlHelper.GetDataList<LotteryDrawConfigModel>(CommandType.Text, SqlConst.GetLotteryConfigInfoStr);
        }
    }
}