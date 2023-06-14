using Core.Util;

namespace StressTesting
{
    /// <summary>
    /// Stress Testing事件类型枚举
    /// </summary>
    public enum StressEvent
    {
        FilterEditEnd, //过滤器编辑完成
        StatisticsLogResponse, //统计日志返回
        PushInterfaceResponse, //推送接口消息返回
        RequestInterfaceResponse, //请求接口消息返回
        WorkerServerInfoResponse, //压测进程工作组信息返回
    }


    /// <summary>
    /// Stress Testing事件管理器
    /// </summary>
    public class StressEventManager : BaseEventManager<StressEvent>
    {
        public static StressEventManager Instance { get; } = new StressEventManager();

        private StressEventManager()
        {
        }
    }
}