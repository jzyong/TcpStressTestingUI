using System;
using UnityEngine;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 消息接口Item 信息
    /// </summary>
    public class PushInterfaceItem : MonoBehaviour
    {
        public Text id;
        public Text name;
        public Text count;
        public Text byteSizeAverage;
        public Text rps;


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
        /// <param name="info"></param>
        public void UpdateContent(PushInterfaceResponse.Types.MessageInterfaceInfo info)
        {
            id.text = $"{info.MessageId}";
            name.text = info.MessageName;
            count.text = $"{info.Count}";
            byteSizeAverage.text = $"{info.SizeAverage}";
            rps.text = $"{info.Rps:F}";

            byteSizeAverage.color = info.SizeAverage > 1454 ? Color.red : Color.black;
        }
    }

}