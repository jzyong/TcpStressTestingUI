using Core.Util.Time;
using UnityEngine;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 累计统计面板
    /// </summary>
    public class AccumulativePanel : MonoBehaviour
    {
        public Text userCountText;
        public Text runTimeText;
        public Text requestBytesText;
        public Text responseBytesText;
        public Text requestCountText;
        public Text pushCountText;
        public Text failCountText;

        /// <summary>
        /// 更新显示内容
        /// </summary>
        /// <param name="statisticLog"></param>
        public void UpdateContent(StatisticLog statisticLog)
        {
            userCountText.text = $"用户数：{statisticLog.PlayerCount}";
            var runTime = (TimeUtil.CurrentTimeMillis() - StressTestingManager.Instance.TestStartTime) / 1000;
            runTimeText.text = $"运行时间：{runTime}S";
            requestBytesText.text = $"上行流量：{statisticLog.RequestBytes:F3}M";
            responseBytesText.text = $"下行流量：{statisticLog.ResponseBytes:F3}M";
            requestCountText.text = $"总请求数：{statisticLog.RequestCount}";
            pushCountText.text = $"总推送数：{statisticLog.PushCount}";
            failCountText.text = $"总失败数：{statisticLog.FailCount}";
        }
    }
}