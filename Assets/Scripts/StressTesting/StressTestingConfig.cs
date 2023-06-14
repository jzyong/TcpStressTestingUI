using System;
using UnityEngine;

namespace StressTesting
{
    /// <summary>
    /// 压测配置
    /// </summary>
    [CreateAssetMenu(menuName = "配置/创建压测配置(StressTestingConfig)")]
    public class StressTestingConfig : ScriptableObject
    {
        /// <summary>
        /// Rpc 地址
        /// </summary>
        [SerializeField] private string rpcHost;

        /// <summary>
        /// Rpc 端口
        /// </summary>
        [SerializeField] private Int32 rpcPort;

        /// <summary>
        /// 网关默认地址
        /// </summary>
        [SerializeField] private string gateUrls = "47.108.13.34:40017,47.108.13.34:40018";


        public string RpcHost => rpcHost;

        public Int32 RpcPort => rpcPort;

        public string GateUrls => gateUrls;
    }
}