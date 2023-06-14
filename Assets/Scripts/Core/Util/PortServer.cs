using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Core.Util
{
    /**
     * 本机端口工具
     */
    public static class PortServer
    {
        private static Log logger
        {
            get { return new Log(); }
        }

        /**
         * 获取本机可用的随机端口
         */
        public static int GenerateRandomPort(int minPort, int maxPort)
        {
            while (true)
            {
                int count = 0;
                int seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
                Random ran = new Random(seed);
                int port = ran.Next(minPort, maxPort);
                if (count < 1000 && !IsPortInUsed(port))
                {
                    return port;
                }

                count++;
            }
        }

        /**
         * 端口是否被占用
         */
        private static bool IsPortInUsed(int port)
        {
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
                if (ipsTCP.Any(p => p.Port == port))
                {
                    return true;
                }

                IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
                if (ipsUDP.Any(p => p.Port == port))
                {
                    return true;
                }

                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                if (tcpConnInfoArray.Any(conn => conn.LocalEndPoint.Port == port))
                {
                    return true;
                }
            }
            catch (NotImplementedException e)
            {
                Log.Println("error: " + e.Message);
            }

            return false;
        }
    }
}