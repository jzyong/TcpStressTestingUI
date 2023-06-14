using System;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 工作组显示内容
    /// </summary>
    public class WorkerContent : MonoBehaviour
    {
        private List<WorkerItem> workerInfos = new List<WorkerItem>();

        public Button nameButton;
        public Button rpcButton;
        public Button cpuButton;
        public Button memoryButton;
        public Button userCountButton;

        //升序排序
        private bool ascendingOrder;

        // Start is called before the first frame update
        void Start()
        {
            nameButton.onClick.AddListener(NameClick);
            rpcButton.onClick.AddListener(HostClick);
            cpuButton.onClick.AddListener(CpuClick);
            memoryButton.onClick.AddListener(MemoryClick);
            userCountButton.onClick.AddListener(UserCountClick);
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnEnable()
        {
            StressTestingManager.Instance.RequestType = RequestType.WorkerServerInfo;
            StressEventManager.Instance.AddEvent<WorkerServerInfoResponse>(StressEvent.WorkerServerInfoResponse,
                WorkerServerInfoResponse);
        }

        private void OnDisable()
        {
            DestroyItems();
            StressEventManager.Instance.RemoveEvent<WorkerServerInfoResponse>(StressEvent.WorkerServerInfoResponse,
                WorkerServerInfoResponse);
        }

        /// <summary>
        /// 清除显示的项目
        /// </summary>
        private void DestroyItems()
        {
            var workerInfoItems = transform.GetComponentsInChildren<WorkerItem>();
            foreach (var infoItem in workerInfoItems)
            {
                DestroyImmediate(infoItem.gameObject);
            }

            workerInfos.Clear();
        }

        /// <summary>
        /// 点击名字
        /// </summary>
        private void NameClick()
        {
            if (ascendingOrder == false)
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o1.name.text, o2.name.text));
            }
            else
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o2.name.text, o1.name.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 点击host
        /// </summary>
        private void HostClick()
        {
            if (ascendingOrder == false)
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o1.rpcHost.text, o2.rpcHost.text));
            }
            else
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o2.rpcHost.text, o1.rpcHost.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 点击cpu
        /// </summary>
        private void CpuClick()
        {
            if (ascendingOrder == false)
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o1.cpu.text, o2.cpu.text));
            }
            else
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o2.cpu.text, o1.cpu.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 点击名字
        /// </summary>
        private void MemoryClick()
        {
            if (ascendingOrder == false)
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o1.memory.text, o2.memory.text));
            }
            else
            {
                workerInfos.Sort((o1, o2) => string.CompareOrdinal(o2.memory.text, o1.memory.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 点击名字
        /// </summary>
        private void UserCountClick()
        {
            if (ascendingOrder == false)
            {
                workerInfos.Sort((o1, o2) => Convert.ToInt32(o1.userCount.text) - Convert.ToInt32(o2.userCount.text));
            }
            else
            {
                workerInfos.Sort((o1, o2) => Convert.ToInt32(o2.userCount.text) - Convert.ToInt32(o1.userCount.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 改变顺序
        /// </summary>
        private void ChangeOrder()
        {
            foreach (var workerInfo in workerInfos)
            {
                workerInfo.transform.SetAsLastSibling();
            }

            ascendingOrder = !ascendingOrder;
        }

        /// <summary>
        /// 工作组信息返回
        /// </summary>
        /// <param name="response"></param>
        void WorkerServerInfoResponse(WorkerServerInfoResponse response)
        {
            if (response != null)
            {
                UpdateContent(response);
            }
        }


        /// <summary>
        /// 更新显示内容
        /// </summary>
        /// <param name="workerServerInfoResponse"></param>
        private void UpdateContent(WorkerServerInfoResponse workerServerInfoResponse)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (workerServerInfoResponse == null)
            {
                workerInfos.Clear();
                return;
            }

            // 一般压测中不会更新节点，不做动态删除
            foreach (var serverInfo in workerServerInfoResponse.WorkerServer)
            {
                var workerInfoItem = workerInfos.Find(info => info.name.text.Equals(serverInfo.Name));
                if (workerInfoItem != null)
                {
                    workerInfoItem.UpdateContent(serverInfo);
                }
                else
                {
                    //新建
                    // ResourceManager.Instance.LoadPrefabGameObject("WorkerItem", g =>
                    // {
                    //     var g2 = Instantiate(g, transform, false);
                    //     var infoItem = g2.GetComponent<WorkerItem>();
                    //     infoItem.UpdateContent(serverInfo);
                    //     g2.SetActive(true);
                    //     workerInfos.Add(infoItem);
                    // });
                    
                    var asyncOperationHandle = Addressables.InstantiateAsync("WorkerItem",transform,false);
                    asyncOperationHandle.Completed += handle =>
                    {
                        var handleResult = handle.Result;
                        if (handleResult!=null)
                        {
                            var infoItem = handleResult.GetComponent<WorkerItem>();
                            infoItem.UpdateContent(serverInfo);
                            handleResult.SetActive(true);
                            workerInfos.Add(infoItem);
                        }
                    };
                }
            }
        }
    }
}