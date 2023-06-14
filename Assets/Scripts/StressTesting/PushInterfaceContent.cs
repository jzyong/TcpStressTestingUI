using System;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 推送消息接口显示内容
    /// </summary>
    public class PushInterfaceContent : MonoBehaviour
    {
        private List<PushInterfaceItem> interfaceItems = new List<PushInterfaceItem>();

        public Button idButton;
        public Button nameButton;
        public Button countButton;
        public Button rpsButton;
        public Button byteSizeAverageButton;

        //过滤面板
        private FilterPanel _filterPanel;

        //升序排序
        private bool ascendingOrder;

        // Start is called before the first frame update
        void Start()
        {
            _filterPanel = GameObject.Find("FilterGroup").GetComponent<FilterPanel>();
            StressEventManager.Instance.AddEvent(StressEvent.FilterEditEnd, DestroyItems);
            nameButton.onClick.AddListener(NameClick);
            idButton.onClick.AddListener(IdClick);
            countButton.onClick.AddListener(CountClick);
            byteSizeAverageButton.onClick.AddListener(ByteSizeAverageClick);
            rpsButton.onClick.AddListener(RpsClick);
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnEnable()
        {
            StressTestingManager.Instance.RequestType = RequestType.PushInterface; //设置消息请求类型 
            StressEventManager.Instance.AddEvent<PushInterfaceResponse>(StressEvent.PushInterfaceResponse,
                PushInterfaceResponse);
        }

        private void OnDisable()
        {
            DestroyItems();
            StressEventManager.Instance.RemoveEvent<PushInterfaceResponse>(StressEvent.PushInterfaceResponse,
                PushInterfaceResponse);
        }

        private void OnDestroy()
        {
            StressEventManager.Instance.RemoveEvent(StressEvent.FilterEditEnd);
        }


        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="response"></param>
        private void PushInterfaceResponse(PushInterfaceResponse response)
        {
            if (response != null)
            {
                UpdateContent(response);
            }
        }

        /// <summary>
        /// 清除显示的项目
        /// </summary>
        private void DestroyItems()
        {
            var items = transform.GetComponentsInChildren<PushInterfaceItem>();
            foreach (var infoItem in items)
            {
                DestroyImmediate(infoItem.gameObject);
            }

            interfaceItems.Clear();
        }

        /// <summary>
        /// 点击Id
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
        private void CountClick()
        {
            if (ascendingOrder == false)
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o1.count.text) - Convert.ToInt32(o2.count.text));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    Convert.ToInt32(o2.count.text) - Convert.ToInt32(o1.count.text));
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
                    (int)(Convert.ToDouble(o1.rps.text) * 100 - Convert.ToDouble(o2.rps.text) * 100));
            }
            else
            {
                interfaceItems.Sort((o1, o2) =>
                    (int)(Convert.ToDouble(o2.rps.text) * 100 - Convert.ToDouble(o1.rps.text) * 100));
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
        /// 更新显示内容
        /// </summary>
        /// <param name="response"></param>
        private void UpdateContent(PushInterfaceResponse response)
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
                if (_filterPanel == null || _filterPanel.IsShowPushInterfaceInfo(serverInfo) == false)
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
                    // ResourceManager.Instance.LoadPrefabGameObject("PushInterfaceItem", g =>
                    // {
                    //     var g2 = Instantiate(g, transform, false);
                    //     var infoItem = g2.GetComponent<PushInterfaceItem>();
                    //     infoItem.UpdateContent(serverInfo);
                    //     g2.SetActive(true);
                    //     interfaceItems.Add(infoItem);
                    // });

                    var asyncOperationHandle = Addressables.InstantiateAsync("PushInterfaceItem",transform,false);
                    asyncOperationHandle.Completed += handle =>
                    {
                        var handleResult = handle.Result;
                        if (handleResult!=null)
                        {
                            var infoItem = handleResult.GetComponent<PushInterfaceItem>();
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