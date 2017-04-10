using System;
using System.Threading;

namespace TestTaskServer
{
    public class UserInfoBll
    {
        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(string userFlag)
        {
            LotteryDrawConfigDal.Instance.CheckLotteryTimeConfig();
            string userName = UserInfoDal.Instance.CheckUserInfo(userFlag);

            //获取当前用户的最新抽奖时间，当前活动积分，以及判断宝石是否足够
            int currenUserPoints = 0;
            DateTime lastLotteryDrawTime= LotteryDrawDal.Instance.GetUserLotteryInfo(userFlag, userName, ref currenUserPoints);
            UserInfoDal.Instance.LotteryDraw(userFlag, lastLotteryDrawTime);
            //异步更新排行榜
            ThreadPool.QueueUserWorkItem((obj) => { LotteryDrawDal.Instance.RefreshCharts(1, currenUserPoints); });
        }
    }
}