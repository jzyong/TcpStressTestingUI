using System;
using UnityEngine;
using XCharts.Runtime;

namespace StressTesting
{
    /// <summary>
    /// 统计显示内容
    /// </summary>
    public class StatisticContent : MonoBehaviour
    {
        public SimplifiedLineChart rpsLineChart;
        public SimplifiedLineChart responseTimeLineChart;
        public SimplifiedLineChart loginUserLineChart;
        public SimplifiedLineChart byteRateLineChart;
        public SimplifiedLineChart coverRateLineChart;
        public AccumulativePanel accumulativePanel;

        private Int32 logCount;

        // Start is called before the first frame update
        void Start()
        {
            rpsLineChart.theme.sharedTheme.themeType = ThemeType.Dark;
            rpsLineChart.ClearData();
            responseTimeLineChart.ClearData();
            loginUserLineChart.ClearData();
            byteRateLineChart.ClearData();
            coverRateLineChart.ClearData();
            rpsLineChart.SetMaxCache(short.MaxValue);
            responseTimeLineChart.SetMaxCache(short.MaxValue);
            loginUserLineChart.SetMaxCache(short.MaxValue);
            byteRateLineChart.SetMaxCache(short.MaxValue);
            coverRateLineChart.SetMaxCache(short.MaxValue);
            rpsLineChart.GetChartComponent<XAxis>().type = Axis.AxisType.Time;
            responseTimeLineChart.GetChartComponent<XAxis>().type = Axis.AxisType.Time;
            loginUserLineChart.GetChartComponent<XAxis>().type = Axis.AxisType.Time;
            byteRateLineChart.GetChartComponent<Axis>().type = Axis.AxisType.Time;
            coverRateLineChart.GetChartComponent<Axis>().type = Axis.AxisType.Time;
            rpsLineChart.GetChartComponent<XAxis>().splitNumber = 0;
            responseTimeLineChart.GetChartComponent<XAxis>().splitNumber = 0;
            loginUserLineChart.GetChartComponent<XAxis>().splitNumber = 0;
            byteRateLineChart.GetChartComponent<XAxis>().splitNumber = 0;
            coverRateLineChart.GetChartComponent<XAxis>().splitNumber = 0;
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }


        /// <summary>
        /// 更新显示内容，未激活也要填充数据
        /// </summary>
        /// <param name="statisticLog"></param>
        public void UpdateContent(StatisticLog statisticLog)
        {
            if (statisticLog == null)
            {
                return;
            }

            logCount++;
            // Debug.Log($"统计数据： {statisticLog}");


            rpsLineChart.AddData(0, DateTime.Now, statisticLog.Rps);
            rpsLineChart.AddData(1, DateTime.Now, statisticLog.PushRps);
            rpsLineChart.AddData(2, DateTime.Now, statisticLog.FailRps);
            responseTimeLineChart.AddData(0, DateTime.Now, statisticLog.ResponseTime);
            loginUserLineChart.AddData(0, DateTime.Now, statisticLog.PlayerCount);
            byteRateLineChart.AddData(0, DateTime.Now, statisticLog.RequestByteRate);
            byteRateLineChart.AddData(1, DateTime.Now, statisticLog.ResponseByteRate);
            coverRateLineChart.AddData(0, DateTime.Now, statisticLog.CoverRate);
            accumulativePanel.UpdateContent(statisticLog);
            if (logCount is < 7 and > 2)
            {
                rpsLineChart.GetChartComponent<XAxis>().splitNumber = logCount - 1;
                responseTimeLineChart.GetChartComponent<XAxis>().splitNumber = logCount - 1;
                loginUserLineChart.GetChartComponent<XAxis>().splitNumber = logCount - 1;
                byteRateLineChart.GetChartComponent<XAxis>().splitNumber = logCount - 1;
                coverRateLineChart.GetChartComponent<XAxis>().splitNumber = logCount - 1;
            }
        }
    }
}