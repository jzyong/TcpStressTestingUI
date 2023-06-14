using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Core.Util;
using Core.Util.Time;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using UnityEngine;
using XLua;

namespace StressTesting
{
    /// <summary>
    /// 网络请求类型
    /// </summary>
    public enum RequestType
    {
        StatisticsLog, //首页统计日志
        PushInterface, //服务器推送的消息接口列表
        RequestInterface, //请求消息接口 列表
        WorkerServerInfo, //压测服务器进程信息
    }

    /// <summary>
    /// 压力测试 .
    /// 网络请求应该使用单独的线程，收到消息然后通过事件传递给界面进行处理，界面通知开始进行数据请求；可避免unity主线程因网络问题卡死；可进行断线重连检测
    /// </summary>
    public class StressTestingManager : SingleInstance<StressTestingManager>
    {
        //缓存统计日志
        private LinkedList<StatisticLog> _statisticLogs = new LinkedList<StatisticLog>();

        //网络请求线程
        private Thread _thread;

        //接收消息队列
        private ConcurrentQueue<IMessage> receiveMessages = new ConcurrentQueue<IMessage>();

        //运行
        private bool run = true;

        // 当前网络请求类型
        public RequestType RequestType { get; set; }

        //最新请求接口统计信息
        public RepeatedField<RequestInterfaceResponse.Types.MessageInterfaceInfo> RequestMessageInterfaceInfos
        {
            get;
            set;
        }

        //最新推送接口统计信息
        public RepeatedField<PushInterfaceResponse.Types.MessageInterfaceInfo> PushMessageInterfaceInfos { get; set; }

        //最新工作组信息
        public RepeatedField<WorkerServerInfoResponse.Types.WorkerServerInfo> WorkerServerInfos { get; set; }

        //测试开始时间
        public Int64 TestStartTime { get; set; }

        /**
         * 端口 
         */
        public int Port { get; set; }


        private string host;

        /**
         * host
         */
        public string Host
        {
            get { return host; }
            set
            {
                //更换连接
                host = value;
                channel?.ShutdownAsync();
                channel = null;
            }
        }

        //连接
        private Channel channel;


        /// <summary>
        /// 启动，开启线程执行网络请求
        /// </summary>
        public void Start(String host, int port)
        {
            Host = host;
            Port = port;
            run = true;
            RequestType = RequestType.StatisticsLog;
            receiveMessages = new ConcurrentQueue<IMessage>();
            //关闭之前线程
            if (_thread is {IsAlive: true})
            {
                _thread.Join();
            }

            _thread = new Thread(() =>
            {
                while (run)
                {
                    Update();
                    Thread.Sleep(1000);
                }
            });
            _thread.Start();
        }


        /// <summary>
        /// 更新，每秒执行
        /// </summary>
        private void Update()
        {
            IMessage message = null;
            IMessage statisticsMessage = StatisticsLog(); //统计日志需要一直请求

            //执行消息
            switch (RequestType)
            {
                // case RequestType.StatisticsLog:
                //     message = StatisticsLog();
                //     break;
                case RequestType.PushInterface:
                    message = PushInterfaceInfoReq();
                    break;
                case RequestType.RequestInterface:
                    message = RequestInterfaceInfoReq();
                    break;
                case RequestType.WorkerServerInfo:
                    message = WorkerServerInfosReq();
                    break;
            }


            if (message != null)
            {
                receiveMessages.Enqueue(message);
            }


            if (statisticsMessage != null)
            {
                receiveMessages.Enqueue(statisticsMessage);
            }
            // 告知网络不可用
            else
            {
                receiveMessages.Enqueue(new StatisticsLogResponse()
                {
                    Status = 2
                });
            }
        }

        /// <summary>
        /// 处理返回消息
        /// </summary>
        public void HandResponseMessage()
        {
            IMessage message;
            receiveMessages.TryDequeue(out message);
            if (message != null)
            {
                if (message is StatisticsLogResponse statisticsLogResponse)
                {
                    StressEventManager.Instance.OnEvent(StressEvent.StatisticsLogResponse, statisticsLogResponse);
                    AddStatisticLog(statisticsLogResponse.StatisticLog);
                }
                else if (message is PushInterfaceResponse pushInterfaceResponse)
                {
                    StressEventManager.Instance.OnEvent(StressEvent.PushInterfaceResponse, pushInterfaceResponse);
                }
                else if (message is RequestInterfaceResponse requestInterfaceResponse)
                {
                    StressEventManager.Instance.OnEvent(StressEvent.RequestInterfaceResponse, requestInterfaceResponse);
                }
                else if (message is WorkerServerInfoResponse workerServerInfoResponse)
                {
                    StressEventManager.Instance.OnEvent(StressEvent.WorkerServerInfoResponse, workerServerInfoResponse);
                }
            }
        }


        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="statisticLog"></param>
        private void AddStatisticLog(StatisticLog statisticLog)
        {
            if (statisticLog == null)
            {
                return;
            }

            if (_statisticLogs.Count > short.MaxValue)
            {
                _statisticLogs.RemoveFirst();
            }

            _statisticLogs.AddLast(statisticLog);
        }

        /// <summary>
        /// 存储统计日志到excel
        /// </summary>
        public void SaveStatisticLog(String testType, String userCount)
        {
            if (string.IsNullOrEmpty(testType))
            {
                testType = "0";
            }

            if (string.IsNullOrEmpty(userCount))
            {
                userCount = "1";
            }

            string fileName = $"压测_{testType}_{userCount}_{DateTime.Now:yyyyMMddHHmmss}.md";
            //文件的绝对路径
            List<string> records = new List<string>();

            //总结内容
            records.Add("##总结");
            records.Add("###客户端");
            var testTime = (TimeUtil.CurrentTimeMillis() - TestStartTime) / 1000;
            records.Add($"* 时间：{testTime}s");
            var statisticLog = _statisticLogs.Last();
            if (statisticLog != null)
            {
                records.Add($"* 用户：{statisticLog.PlayerCount}");
                records.Add($"* 请求RPS：{statisticLog.Rps:F}");
                records.Add($"* 推送RPS：{statisticLog.PushRps:F}");
                records.Add($"* 失败RPS：{statisticLog.FailRps:F}");
                records.Add($"* 响应时间：{statisticLog.ResponseTime}ms");
                records.Add($"* 总请求数:{statisticLog.RequestCount}");
                records.Add($"* 总推送数:{statisticLog.PushCount}");
                records.Add($"* 总失败数:{statisticLog.FailCount}");
                records.Add($"* 上行频率:{statisticLog.RequestByteRate:F}m/s");
                records.Add($"* 下行频率:{statisticLog.ResponseByteRate:F}m/s");
                records.Add($"* 上行流量:{statisticLog.RequestBytes:F}m");
                records.Add($"* 下行流量:{statisticLog.ResponseBytes:F}m");
                records.Add($"* 覆盖率:{statisticLog.CoverRate:F}");
            }

            //服务器生成格式，需要手动填入信息
            records.Add("###服务器");
            records.Add("此部分内容需要手动填写、修改 ！    ");
            records.Add(" ");
            records.Add(" ");
            records.Add("服务   |压测环境   |cpu   |内存   |线程设置    |备注");
            records.Add("------|----------|------|------|----------|----");
            records.Add("API 1| Centos7 4核16G   |   |   | 24    |");
            records.Add("API 2| Centos7 12核96G  |   |   |  24   |");
            records.Add("Gate 1| Centos7 12核96G  |   |   |  24  |");
            records.Add("Gate 2| Centos7 12核96G  |   |   |  24  |");
            records.Add("Hall 1| Centos7 12核96G  |   |   |  24  |");
            records.Add("Hall 2| Centos7 12核96G  |   |   |  24  |");
            records.Add("World| Centos7 12核96G  |   |   |  24   |");
            records.Add("Manage 1| Centos7 12核96G  |   |   |  24|");
            records.Add("Manage 2| Centos7 12核96G  |   |   | 24 |");
            records.Add("小恶魔-devil| Centos7 12核96G  |   |   |  24 |");


            //worker 最终日志  
            WorkerServerInfosReq(); //获取最新数据
            records.Add("##worker");
            records.Add("名称   |Host   |CPU百分比   |内存百分比   |玩家数");
            records.Add("------|-------|-----------|-----------|----------");
            if (WorkerServerInfos != null)
            {
                for (int i = 0; i < WorkerServerInfos.Count; i++)
                {
                    var info = WorkerServerInfos[i];
                    records.Add(
                        $"{info.Name}|{info.RpcHost}|{info.CpuRate}|{info.MemorySize}|{info.PlayerCount}");
                }
            }


            //每条协议的最终日志 
            RequestInterfaceInfoReq(); //获取最新数据
            records.Add("##请求接口");
            records.Add("消息名称   |请求次数   |失败次数   |平均延迟   |最小延迟   |最大延迟   |平均大小   |rps    |每秒失败数");
            records.Add("----------|---------|----------|---------|----------|---------|----------|-------|--------");
            if (RequestMessageInterfaceInfos != null)
            {
                for (int i = 0; i < RequestMessageInterfaceInfos.Count; i++)
                {
                    var info = RequestMessageInterfaceInfos[i];
                    records.Add(
                        $"{info.MessageName}|{info.RequestCount}|{info.FailCount}|{info.DelayAverage}|{info.DelayMin}|{info.DelayMax}|{info.SizeAverage}|{info.Rps:F}|{info.FailSecondCount:F}");
                }
            }

            // 写推送日志
            PushInterfaceInfoReq(); //获取最新数据
            records.Add("##推送接口");
            records.Add("消息名称   |次数   |平均大小   |rps");
            records.Add("----------|---------|----------|---------");
            if (PushMessageInterfaceInfos != null)
            {
                for (int i = 0; i < PushMessageInterfaceInfos.Count; i++)
                {
                    var info = PushMessageInterfaceInfos[i];
                    records.Add(
                        $"{info.MessageName}|{info.Count}|{info.SizeAverage}|{info.Rps:F}");
                }
            }


            //压测统计图表日志
            var logList = _statisticLogs.ToList();
            records.Add("##统计日志");
            records.Add("Rps             |FailRps         |ResponseTime    |PlayerCount");
            records.Add("----------------|----------------|----------------|----------------");
            for (int i = 0; i < logList.Count; i++)
            {
                var log = logList[i];
                records.Add(
                    $"{log.Rps:F}            |{log.FailRps:F}            |{log.ResponseTime}             |{log.PlayerCount}");
            }

            Log.Records(fileName, records);


        }


        /**
         * rpc客户端   
         */
        public StressTestingOuterService.StressTestingOuterServiceClient ServiceClient
        {
            get
            {
                if (channel == null || channel.State == ChannelState.Shutdown ||
                    channel.State == ChannelState.TransientFailure)
                {
                    channel = new Channel(Host, Port, ChannelCredentials.Insecure);
                    Debug.Log($"创建压测grpc连接：{Host}：{Port}");
                }

                return new StressTestingOuterService.StressTestingOuterServiceClient(channel);
            }
        }


        /// <summary>
        /// 请求接口信息
        /// </summary>
        /// <returns></returns>
        private RequestInterfaceResponse RequestInterfaceInfoReq()
        {
            RequestInterfaceRequest request = new RequestInterfaceRequest();
            try
            {
                var response = ServiceClient.requestInterfaceInfo(request);
                if (response.InterfaceInfo.Count > 0)
                {
                    RequestMessageInterfaceInfos = response.InterfaceInfo;
                }

                return response;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        /// 推送接口信息
        /// </summary>
        /// <returns></returns>
        private PushInterfaceResponse PushInterfaceInfoReq()
        {
            PushInterfaceRequest request = new PushInterfaceRequest();
            try
            {
                var pushInterfaceResponse = ServiceClient.pushInterfaceInfo(request);
                if (pushInterfaceResponse.InterfaceInfo.Count > 0)
                {
                    PushMessageInterfaceInfos = pushInterfaceResponse.InterfaceInfo;
                }

                return pushInterfaceResponse;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        /// 请求统计日志
        /// </summary>
        /// <returns></returns>
        private StatisticsLogResponse StatisticsLog()
        {
            StatisticsLogRequest request = new StatisticsLogRequest();
            try
            {
                return ServiceClient.statisticsLog(request);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// 工作组信息
        /// </summary>
        /// <returns></returns>
        private WorkerServerInfoResponse WorkerServerInfosReq()
        {
            var request = new WorkerServerInfoRequest();
            try
            {
                var workerServerInfoResponse = ServiceClient.workerServerInfo(request);
                if (workerServerInfoResponse.WorkerServer != null)
                {
                    WorkerServerInfos = workerServerInfoResponse.WorkerServer;
                }

                return workerServerInfoResponse;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
            }

            return null;
        }


        /// <summary>
        /// 销毁关闭grpc
        /// </summary>
        public void Stop()
        {
            if (channel != null)
            {
                channel.ShutdownAsync();
                channel = null;
            }

            run = false;
        }
    }
}