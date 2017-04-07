using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

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
    }
}