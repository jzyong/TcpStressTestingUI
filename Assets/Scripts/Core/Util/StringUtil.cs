using System;
using System.Text;

namespace Core.Util
{
    /// <summary>
    /// 字符串工具
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// byte 数组转换为16
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }

            return sb.ToString().ToUpper();
        }
    }
}