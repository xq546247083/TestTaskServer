using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace TestTaskServer
{
    /// <summary>
    /// 处理消息类
    /// </summary>
    public class HandlerMsg
    {

        /// <summary>
        /// 将对象转为Json
        /// </summary>
        /// <typeparam name="T">对象T</typeparam>
        /// <param name="t">参数T</param>
        /// <returns>Json字符串</returns>
        public static string ToJson<T>(T t)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ds.WriteObject(ms, t);

            string strReturn = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return strReturn;
        }

        /// <summary>
        /// 向Http请求回写数据
        /// </summary>
        /// <param name="flag">是否成功的标志</param>
        /// <param name="msg">返回的消息</param>
        /// <param name="dataTable">返回的数据</param>
        public static void WriteResponseMsg(HttpContext httpContext, bool flag, string msg, DataTable dataTable = null)
        {
            BaseMsg baseMsg = new BaseMsg(flag.ToString(), msg, dataTable);
            var sd = HandlerMsg.ToJson<BaseMsg>(baseMsg);
            httpContext.Response.Write(HandlerMsg.ToJson<BaseMsg>(baseMsg));
            httpContext.ApplicationInstance.CompleteRequest();
        }
    }
}