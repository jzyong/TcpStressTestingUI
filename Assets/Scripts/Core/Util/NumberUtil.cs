namespace Core.Util
{
    /// <summary>
    /// 数值 工具
    /// </summary>
    public class NumberUtil
    {
        /// <summary>
        /// @
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] IntToBytes(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte) ((value >> 24) & 0xFF);
            src[1] = (byte) ((value >> 16) & 0xFF);
            src[2] = (byte) ((value >> 8) & 0xFF);
            src[3] = (byte) (value & 0xFF);
            return src;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ShortToBytes(short value)
        {
            byte[] src = new byte[2];

            src[0] = (byte) ((value >> 8) & 0xFF);
            src[1] = (byte) (value & 0xFF);
            return src;
        }

        public static short BytesToShort(byte[] values)
        {
            return (short) (values[1] | (values[0] << 8));
        }

        public static int BytesToInt(byte[] values)
        {
            return (int) (values[0] | (int) (values[1] << 8) | (int) (values[2] << 16) | (int) (values[3] << 24));
        }

        public static short BytesToShort(byte[] values, int offer)
        {
            return (short) (values[offer + 1] | (values[offer] << 8));
        }

        public static int BytesToInt(byte[] values, int offer)
        {
            return (int) (values[offer + 3] | (int) (values[offer + 2] << 8) | (int) (values[offer + 1] << 16) |
                          (int) (values[offer] << 24));
        }
    }
}