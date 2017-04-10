using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace TestTaskServer
{
    /// <summary>
    /// 处理抽奖以及获取排行榜
    /// </summary>
    public class LotteryDrawHandler : IHttpHandler
    {
        #region 方法

        public Boolean IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 处理数据库的方法
        /// </summary>
        /// <param name="context">Http内容</param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            String operationStr = context.Request.Form["Operation"];
            String userFlagStr = context.Request.Form["UserFlag"];
            try
            {
                //获取操作方法
                MethodInfo methodInfo = this.GetType().GetMethod(operationStr);
                if (String.IsNullOrEmpty(userFlagStr))
                {
                    methodInfo.Invoke(this, null);
                }
                else
                {
                    methodInfo.Invoke(this, new String[] { userFlagStr });
                }
            }
            catch
            {
                HandlerMsg.WriteResponseMsg(HttpContext.Current, false, CommonConst.NoFindMethod);
            }
        }

        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(String userFlag)
        {
            try
            {
                // 用户抽奖
                LotteryDrawBll lotteryDrawBll = new LotteryDrawBll();
                lotteryDrawBll.LotteryDraw(userFlag);

                HandlerMsg.WriteResponseMsg(HttpContext.Current, true, CommonConst.LotteryDrawSuccess);
            }
            catch (Exception ex)
            {
                HandlerMsg.WriteResponseMsg(HttpContext.Current, false, ex.Message);
            }
        }

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void GetCharts(String userFlag)
        {
            try
            {
                // 用户获取排行榜
                LotteryDrawBll lotteryDrawBll = new LotteryDrawBll();
                List<LotteryDrawModel> chartData = lotteryDrawBll.GetCharts(ref userFlag);

                HandlerMsg.WriteResponseMsg(HttpContext.Current, true, userFlag, ModelHandler<LotteryDrawModel>.ToDataTable(chartData));
            }
            catch (Exception)
            {
                HandlerMsg.WriteResponseMsg(HttpContext.Current, false, CommonConst.FailGetData);
            }
        }

        #endregion
    }
}
