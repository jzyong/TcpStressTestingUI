using System;
using System.Collections.Generic;
using Core.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Common.UI
{
    /// <summary>
    /// UI 管理
    /// </summary>
    public class UIManager : SingleInstance<UIManager>
    {


        private Camera uiCamera = null;

        public Camera UICamera
        {
            get { return uiCamera; }
        }

        public Canvas Canvas { get; set; }


        /// <summary>
        /// 所有已经被加载的界面
        /// </summary>
        private List<UIBase> loadedUIs = new List<UIBase>();



        public void ShowUI(string uiName, params object[] args)
        {
            UIBase uiBase = GetLoadedUI(uiName);
            if (uiBase != null && uiBase.IsShow())
            {
                uiBase.transform.SetAsLastSibling();
                uiBase.PreShow(null, args);
                return;
            }

            if (uiBase == null)
            {
                CreateUI(uiName, uiObject => { ShowWindow(uiObject, args); });
            }
            else
            {
                ShowWindow(uiBase, args);
            }
        }


        public void HideUI(String uiName)
        {
            UIBase obj = GetLoadedUI(uiName);
            if (obj == null)
            {
                return;
            }

            HideUI(obj);
        }

        public void HideUI(UIBase uiBase)
        {
            if (uiBase == null)
            {
                return;
            }

            _handleUIHide(uiBase);
        }


        /// <summary>
        /// 获取已经加载的Ui
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        private UIBase GetLoadedUI(string uiName)
        {
            foreach (var loadedUI in loadedUIs)
            {
                if (loadedUI != null && loadedUI.gameObject.name.Equals(uiName))
                {
                    return loadedUI;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="action"></param>
        private void CreateUI(string uiName, Action<UIBase> action)
        {
            UIBase uiBase = null;


            var asyncOperationHandle = Addressables.InstantiateAsync(uiName, Canvas.transform, false);
            asyncOperationHandle.Completed += handle =>
            {
                var handleResult = handle.Result;
                if (handleResult!=null)
                {
                   handleResult.SetActive(false);
                   handleResult.name = uiName;
                   // 放置ui到对应的层
                   uiBase = handleResult.GetComponent<UIBase>();

                   uiBase.Initialize();
                   loadedUIs.Add(uiBase);
                   action(uiBase);
                }
                else
                {
                    Debug.LogFormat("ui: {0} 不存在!", uiName);
                }
            };

            // ResourceManager.Instance.LoadUI(uiName, (prefab) =>
            // {
            //     if (prefab == null)
            //     {
            //         Debug.LogFormat("ui: {0} 不存在!", uiName);
            //         return;
            //     }
            //
            //     GameObject uiObject = Object.Instantiate(prefab, Canvas.transform, false) as GameObject;
            //     uiObject.SetActive(false);
            //     uiObject.name = uiName;
            //
            //     // 放置ui到对应的层
            //     uiBase = uiObject.GetComponent<UIBase>();
            //
            //     uiBase.Initialize();
            //     loadedUIs.Add(uiBase);
            //     action(uiBase);
            // });
        }


        private void ShowWindow(UIBase uiBase, params object[] args)
        {
            //播放当前界面的入场动画和事件回调
            uiBase.transform.SetAsLastSibling();
            uiBase.PreShow(null, args);
            uiBase.OnShow(null, args);
            uiBase.gameObject.SetActive(true);
        }


        private bool _handleUIHide(UIBase uiBase)
        {
            uiBase.gameObject.SetActive(false);
            uiBase.OnHide(null);
            return true;
        }


        /// <summary>
        /// UI面板是否展示
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public bool IsShowPanel(string uiName)
        {
            var ui = GetLoadedUI(uiName);
            return ui != null && ui.IsShow();
        }
    }
}