using UnityEngine;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 消息接口Item 信息
    /// </summary>
    public class RequestInterfaceItem : MonoBehaviour
    {
        public Text id;
        public Text name;
        public Text requestCount;
        public Text failCount;
        public Text delayAverage;
        public Text delayMin;
        public Text delayMax;
        public Text byteSizeAverage;
        public Text rps;
        public Text failCountSecond;


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
        public void UpdateContent(RequestInterfaceResponse.Types.MessageInterfaceInfo info)
        {
            id.text = $"{info.MessageId}";
            name.text = info.MessageName;
            requestCount.text = $"{info.RequestCount}";
            failCount.text = $"{info.FailCount}";
            delayAverage.text = $"{info.DelayAverage}";
            delayMin.text = $"{info.DelayMin}";
            delayMax.text = $"{info.DelayMax}";
            byteSizeAverage.text = $"{info.SizeAverage}";
            rps.text = $"{info.Rps:F}";
            failCountSecond.text = $"{info.FailSecondCount:F}";

            delayAverage.color = info.DelayAverage > 1000 ? Color.red : Color.black;
            byteSizeAverage.color = info.SizeAverage > 1454 ? Color.red : Color.black;
            failCountSecond.color = info.FailSecondCount > 10 ? Color.red : Color.black;
        }
    }
}