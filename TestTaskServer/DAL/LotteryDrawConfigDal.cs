using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    public class LotteryDrawConfigDal
    {
        /// <summary>
        /// 抽奖配置Dal实例
        /// </summary>
        public volatile static LotteryDrawConfigDal Instance = new LotteryDrawConfigDal();

        /// <summary>
        /// 获取抽奖配置数据
        /// </summary>
        /// <returns>抽奖配置数据</returns>
        public List<LotteryDrawConfigModel> GetLotteryDrawConfigData()
        {
            return MySqlHelper.GetDataList<LotteryDrawConfigModel>(CommandType.Text, "SELECT * FROM LotterydrawConfig");
        }
    }
}