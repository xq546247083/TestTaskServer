using System.Data;

namespace TestTaskServer
{
    public class LotteryDrawBll
    {
        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <returns>排行榜数据</returns>
        public DataTable GetCharts(ref string userFlag)
        {
            //获取当前用户分数
            userFlag = LotteryDrawDal.Instance.GetUserPoints(userFlag).ToString();
            //刷新排行榜
            LotteryDrawDal.Instance.RefreshCharts(0);
            return LotteryDrawDal.Instance.LotteryDrawChartsDT;
        }
    }
}