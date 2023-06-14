using System;
using Core.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    /**
     * 通用提示面板
     */
    public class NoticePanel : UIBase
    {
        private Button cancelButton;
        private Button confirmButton;
        public Text contentText;

        private void Start()
        {
            confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
            confirmButton.onClick.AddListener(Confirm);
            // contentText = GameObject.Find("ContentText").GetComponent<Text>();
        }

        private void Update()
        {
        }

        public override void OnShow(UIBase preUI, params object[] args)
        {
            base.OnShow(preUI, args);
            this.contentText.text = (string) args[0];
           
        }


        /**
         * 确认关闭界面
         */
        private void Confirm()
        {
            gameObject.SetActive(false);
            AudioManager.Instance.PlaySfx("button");
        }

        /**
         * 线上内容
         */
        public void Show(string content)
        {
            gameObject.SetActive(true);
            contentText.text = content;
        }
    }

    /**
     * 线上通知面板事件
     */
    public class NoticePanelShowEvent : Simulation.Event<NoticePanelShowEvent>
    {
        public string message;


        public override void Execute()
        {
            UIManager.Instance.ShowUI("NoticePanel",message);
        }
    }
}