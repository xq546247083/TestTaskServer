using System;
using System.Data;
using System.Reflection;
using System.Web;

namespace TestTaskServer
{
    /// <summary>
    /// 处理抽奖以及获取排行榜
    /// </summary>
    public class LotteryDrawHandler : IHttpHandler
    {
        #region method

        public bool IsReusable
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

            string operationStr = context.Request.Form["Operation"];
            string userFlagStr = context.Request.Form["UserFlag"];
            try
            {
                //获取操作方法
                MethodInfo methodInfo = this.GetType().GetMethod(operationStr);
                if (string.IsNullOrEmpty(userFlagStr))
                {
                    methodInfo.Invoke(this, null);
                }
                else
                {
                    methodInfo.Invoke(this, new string[] { userFlagStr });
                }
            }
            catch (Exception ex)
            {
                WriteResponseMsg(false, CommonConst.NoFindMethod);
            }
        }

        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(string userFlag)
        {
            try
            {
                // 用户抽奖
                UserInfoDal.Instance.LotteryDraw(userFlag);
                WriteResponseMsg(true, CommonConst.LotteryDrawSuccess);
            }
            catch (Exception ex)
            {
                WriteResponseMsg(false, ex.Message);
            }
        }

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void GetCharts(string userFlag)
        {
            try
            {
                // 用户获取排行榜
                DataTable chartTable = UserInfoDal.Instance.GetCharts(ref userFlag);
                WriteResponseMsg(true, userFlag, chartTable);
            }
            catch (Exception)
            {
                WriteResponseMsg(false, CommonConst.FailGetData);
            }
        }

        /// <summary>
        /// 向Http请求回写数据
        /// </summary>
        /// <param name="flag">是否成功的标志</param>
        /// <param name="msg">返回的消息</param>
        /// <param name="dataTable">返回的数据</param>
        private void WriteResponseMsg(bool flag, string msg, DataTable dataTable = null)
        {
            BaseMsg baseMsg = new BaseMsg(flag.ToString(), msg, dataTable);
            var sd = HandlerMsg.ToJson<BaseMsg>(baseMsg);
            HttpContext.Current.Response.Write(HandlerMsg.ToJson<BaseMsg>(baseMsg));
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion
    }
}
