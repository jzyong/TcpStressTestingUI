using UnityEngine;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 工作组Item 信息
    /// </summary>
    public class WorkerItem : MonoBehaviour
    {
        public Text name;
        public Text rpcHost;
        public Text cpu;
        public Text memory;
        public Text userCount;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// 更新显示内容
        /// </summary>
        /// <param name="workerServerInfo"></param>
        public void UpdateContent(WorkerServerInfoResponse.Types.WorkerServerInfo workerServerInfo)
        {
            // CPU，内存等超标红色显示，并且修改动画状态？什么状态？
            name.text = workerServerInfo.Name;
            rpcHost.text = workerServerInfo.RpcHost;
            cpu.text = $"{workerServerInfo.CpuRate:F}%";
            cpu.color = workerServerInfo.CpuRate > 90 ? Color.red : Color.black;

            memory.text = $"{workerServerInfo.MemorySize}%";
            memory.color = workerServerInfo.MemorySize > 90 ? Color.red : Color.black;

            userCount.text = $"{workerServerInfo.PlayerCount}";
        }
    }
}