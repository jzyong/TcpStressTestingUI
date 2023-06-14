using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 消息接口显示内容
    /// </summary>
    public class RequestInterfaceContent : MonoBehaviour
    {
        private List<RequestInterfaceItem> interfaceItems = new List<RequestInterfaceItem>();

        public Button idButton;
        public Button nameButton;
        public Button requestCountButton;
        public Button failCountButton;
        public Button delayAverageButton;
        public Button delayMinButton;
        public Button delayMaxButton;
        public Button byteSizeAverageButton;
        public Button rpsButton;
        public Button failCountSecondButton;


        //升序排序
        private bool ascendingOrder;

        //过滤面板
        private FilterPanel _filterPanel;

        // Start is called before the first frame update
        void Start()
        {
            _filterPanel = GameObject.Find("FilterGroup").GetComponent<FilterPanel>();
            StressEventManager.Instance.AddEvent(StressEvent.FilterEditEnd, DestroyItems);
            nameButton.onClick.AddListener(NameClick);
            idButton.onClick.AddListener(IdClick);
            requestCountButton.onClick.AddListener(RequestCountClick);
            failCountButton.onClick.AddListener(FailCountClick);
            delayAverageButton.onClick.AddListener(DelayAverageClick);
            delayMinButton.onClick.AddListener(DelayMinClick);
            delayMaxButton.onClick.AddListener(DelayMaxClick);
            byteSizeAverageButton.onClick.AddListener(ByteSizeAverageClick);
            rpsButton.onClick.AddListener(RpsClick);
            failCountSecondButton.onClick.AddListener(FailCountSecondClick);

        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnEnable()
        {
            StressEventManager.Instance.AddEvent<RequestInterfaceResponse>(StressEvent.RequestInterfaceResponse,RequestInterfaceResponse);
            StressTestingManager.Instance.RequestType = RequestType.RequestInterface;
        }

        private void OnDisable()
        {
            DestroyItems();
            StressEventManager.Instance.RemoveEvent<RequestInterfaceResponse>(StressEvent.RequestInterfaceResponse,RequestInterfaceResponse);
        }

        private void OnDestroy()
        {
            StressEventManager.Instance.RemoveEvent(StressEvent.FilterEditEnd);
        }


        /// <summary>
        /// 清除显示的项目
        /// </summary>
        private void DestroyItems()
        {
            var items = transform.GetComponentsInChildren<RequestInterfaceItem>();
            foreach (var infoItem in items)
            {
                DestroyImmediate(infoItem.gameObject);
            }

            interfaceItems.Clear();
        }

        /// <summary>
        /// 点击名字
        /// </summary>
        private void IdClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) => string.CompareOrdinal(o1.id.text, o2.id.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) => string.CompareOrdinal(o2.id.text, o1.id.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 点击名字
        /// </summary>
        private void NameClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) => string.CompareOrdinal(o1.name.text, o2.name.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) => string.CompareOrdinal(o2.name.text, o1.name.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 请求数
        /// </summary>
        private void RequestCountClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o1.requestCount.text) - Convert.ToInt32(o2.requestCount.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o2.requestCount.text) - Convert.ToInt32(o1.requestCount.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 失败数
        /// </summary>
        private void FailCountClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o1.failCount.text) - Convert.ToInt32(o2.failCount.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o2.failCount.text) - Convert.ToInt32(o1.failCount.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 平均延迟
        /// </summary>
        private void DelayAverageClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o1.delayAverage.text) - Convert.ToInt32(o2.delayAverage.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o2.delayAverage.text) - Convert.ToInt32(o1.delayAverage.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 最小延迟
        /// </summary>
        private void DelayMinClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) => Convert.ToInt32(o1.delayMin.text) - Convert.ToInt32(o2.delayMin.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) => Convert.ToInt32(o2.delayMin.text) - Convert.ToInt32(o1.delayMin.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 最大延迟
        /// </summary>
        private void DelayMaxClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) => Convert.ToInt32(o1.delayMax.text) - Convert.ToInt32(o2.delayMax.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) => Convert.ToInt32(o2.delayMax.text) - Convert.ToInt32(o1.delayMax.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 消息平均大小
        /// </summary>
        private void ByteSizeAverageClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o1.byteSizeAverage.text) - Convert.ToInt32(o2.byteSizeAverage.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o2.byteSizeAverage.text) - Convert.ToInt32(o1.byteSizeAverage.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// rps
        /// </summary>
        private void RpsClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    (int) (Convert.ToDouble(o1.rps.text) * 100 - Convert.ToDouble(o2.rps.text) * 100));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    (int) (Convert.ToDouble(o2.rps.text) * 100 - Convert.ToDouble(o1.rps.text) * 100));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 每秒失败数
        /// </summary>
        private void FailCountSecondClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    string.CompareOrdinal(o1.failCountSecond.text, o2.failCountSecond.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    string.CompareOrdinal(o2.failCountSecond.text, o1.failCountSecond.text));
            }

            ChangeOrder();
        }

        /// <summary>
        /// 改变顺序
        /// </summary>
        private void ChangeOrder()
        {
            foreach (var workerInfo in interfaceItems)
            {
                workerInfo.transform.SetAsLastSibling();
            }

            ascendingOrder = !ascendingOrder;
        }

        /// <summary>
        /// 请求消息接口返回
        /// </summary>
        /// <param name="response"></param>
        private void RequestInterfaceResponse(RequestInterfaceResponse response)
        {
            if (response != null)
            {
                UpdateContent(response);
            }
        }


        /// <summary>
        /// 更新显示内容
        /// </summary>
        /// <param name="response"></param>
        private void UpdateContent(RequestInterfaceResponse response)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (response == null)
            {
                interfaceItems.Clear();
                return;
            }

            // 一般压测中不会更新节点，不做动态删除
            foreach (var serverInfo in response.InterfaceInfo)
            {
                // 做筛选过滤
                if (_filterPanel == null || _filterPanel.IsShowRequestInterfaceInfo(serverInfo) == false)
                {
                    continue;
                }

                var workerInfoItem = interfaceItems.Find(info => info.name.text.Equals(serverInfo.MessageName));
                if (workerInfoItem != null)
                {
                    workerInfoItem.UpdateContent(serverInfo);
                }
                else
                {
                    // //新建
                    // ResourceManager.Instance.LoadPrefabGameObject("RequestInterfaceItem", g =>
                    // {
                    //     var g2 = Instantiate(g, transform, false);
                    //     var infoItem = g2.GetComponent<RequestInterfaceItem>();
                    //     infoItem.UpdateContent(serverInfo);
                    //     g2.SetActive(true);
                    //     interfaceItems.Add(infoItem);
                    // });
                    
                    var asyncOperationHandle = Addressables.InstantiateAsync("RequestInterfaceItem",transform,false);
                    asyncOperationHandle.Completed += handle =>
                    {
                        var handleResult = handle.Result;
                        if (handleResult!=null)
                        {
                            var infoItem = handleResult.GetComponent<RequestInterfaceItem>();
                            infoItem.UpdateContent(serverInfo);
                            handleResult.SetActive(true);
                            interfaceItems.Add(infoItem);
                        }
                    };
                }
            }
        }
    }
}