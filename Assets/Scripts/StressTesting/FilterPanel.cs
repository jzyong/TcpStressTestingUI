using UnityEngine;
using UnityEngine.UI;
using XLua;
using Object = System.Object;

namespace StressTesting
{
    /// <summary>
    /// 过滤器面板
    /// </summary>
    [LuaCallCSharp]
    public class FilterPanel : MonoBehaviour
    {
        [Tooltip("过滤器帮助面板按钮")] public Button helpButton;

        [Tooltip("关闭帮助面板按钮")] public Button helpCloseButton;

        [Tooltip("Lua 过滤字符串")] public InputField filterInputField;

        //帮助面板
        private GameObject helpPanel;

        private LuaEnv _luaEnv = null;

        // string messageName = 1; //消息名称
        // int32 requestCount = 2; //请求次数
        // int32 failCount = 3; //失败次数
        // int32 delayAverage = 4; //平均延迟 ms
        // int32 delayMin = 5; //最小延迟 ms
        // int32 delayMax = 6; //最大延迟 ms
        // int32 sizeAverage = 7; //平均大小 bytes
        // float rps = 8; //每秒请求次数
        // float failSecondCount = 9; //每秒失败次数
        // int32 messageId = 10 ; //消息id
        private Object[] args = new object[10];


        //执行是否需要展示的item


        [CSharpCallLua]
        public delegate bool IsNeedItem(Object[] args);

        //Lua脚本
        public string LuaScript { get; set; }

        private void Start()
        {
            _luaEnv = new LuaEnv();
            _luaEnv.DoString("print('压力测试过滤器')");
            helpPanel = gameObject.transform.Find("HelpPanel").gameObject;
            helpButton.onClick.AddListener(ShowHelpPanel);
            helpCloseButton.onClick.AddListener(CloseHelpPanel);
            filterInputField.onEndEdit.AddListener(UpdateFilterScript);
        }

        private void Update()
        {
            if (_luaEnv != null)
            {
                _luaEnv.Tick();
            }
        }

        /// <summary>
        /// 更新脚本
        /// </summary>
        /// <param name="filterCondition"></param>
        private void UpdateFilterScript(string filterCondition)
        {
            Debug.Log("过滤器脚本更新:" + filterCondition);
            //清除过滤器
            if (string.IsNullOrEmpty(filterCondition))
            {
                LuaScript = null;
                return;
            }

            // TODO 完善，有性能问题，添加过滤条件后，动画都卡帧了？ for循环拆分为多帧执行？
            LuaScript = @" 
                i={
                    id=nil;
                    name=nil;
                    rps=nil;
                    count=nil;
                    failCount=nil;
                    delayAverage=nil;
                    delayMin=nil;
                    delayMax=nil;
                    size=nil;
                    failSecondCount=nil;
                }  
                -- 实现内容过滤
                function isNeed(args)
                    i.name=args[0]  
                    i.count=args[1]                            
                    i.failCount=args[2]                            
                    i.delayAverage=args[3]                            
                    i.delayMin=args[4]                            
                    i.delayMax=args[5]                            
                    i.size=args[6]                            
                    i.rps=args[7]                     
                    i.failSecondCount=args[8]                     
                    i.id=args[9]                  

                    --print('args object:',args)
                    --print('args0 object:',args[0])
                    --print('args1 object:',args[1])
                    --print('name object:',i.name)
                    --print('UnityEngine.Time.deltaTime:', CS.UnityEngine.Time.deltaTime) --读静态属性
                    if (filterCondition) then
                        return true
                    end
                    return false
                end
                ";
            LuaScript = LuaScript.Replace("filterCondition", filterCondition);
            //Debug.Log($"lua 脚本：{LuaScript}");
            _luaEnv.DoString(LuaScript);
            StressEventManager.Instance.OnEvent(StressEvent.FilterEditEnd);
        }

       /// <summary>
       /// 是否显示推送接口信息
       /// </summary>
       /// <param name="info"></param>
       /// <returns></returns>
        public bool IsShowPushInterfaceInfo(PushInterfaceResponse.Types.MessageInterfaceInfo info)
        {
            //没有过滤全部显示
            if (LuaScript == null)
            {
                return true;
            }

            // Debug.Log(info);
            var isNeedItem = _luaEnv.Global.Get<IsNeedItem>("isNeed");
            args[0] = info.MessageName;
            args[1] = info.Count;
            args[6] = info.SizeAverage;
            args[7] = info.Rps;
            args[9] = info.MessageId;
            var needItem = isNeedItem(args);
            // Debug.Log("是否为需要item：" + needItem);
            return needItem;
        }


        /// <summary>
        /// 是否展示请求接口信息
        /// </summary>
        /// <param name="luaEnv"></param>
        /// <returns></returns>
        public bool IsShowRequestInterfaceInfo(RequestInterfaceResponse.Types.MessageInterfaceInfo info)
        {
            //没有过滤全部显示
            if (LuaScript == null)
            {
                return true;
            }

            // Debug.Log(info);
            var isNeedItem = _luaEnv.Global.Get<IsNeedItem>("isNeed");
            args[0] = info.MessageName;
            args[1] = info.RequestCount;
            args[2] = info.FailCount;
            args[3] = info.DelayAverage;
            args[4] = info.DelayMin;
            args[5] = info.DelayMax;
            args[6] = info.SizeAverage;
            args[7] = info.Rps;
            args[8] = info.FailSecondCount;
            args[9] = info.MessageId;
            var needItem = isNeedItem(args);
           // Debug.Log("是否为需要item：" + needItem);
            return needItem;
        }


        /// <summary>
        /// 显示帮助面板
        /// </summary>
        private void ShowHelpPanel()
        {
            helpPanel.SetActive(true);
        }

        /// <summary>
        ///  关闭显示帮助面板
        /// </summary>
        private void CloseHelpPanel()
        {
            helpPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            _luaEnv.Dispose();
        }
    }
}