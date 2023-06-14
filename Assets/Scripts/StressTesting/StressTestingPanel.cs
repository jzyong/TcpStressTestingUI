using System;
using System.Collections;
using Common;
using Common.UI;
using Core.Util;
using Core.Util.Time;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StressTesting
{
    /// <summary>
    /// 压力测试面板
    /// </summary>
    public class StressTestingPanel : MonoBehaviour
    {
        public Button startTestButton;
        public Button stopTestButton;
        public Button statisticInfoButton;
        public Button requestInterfaceInfoButton;
        public Button pushInterfaceInfoButton;
        public Button workerInfoButton;
        public Button closeButton;

        [Tooltip("网关地址")] public InputField gateUrlsInputField;
        [Tooltip("测试类型")] public InputField testTypeInputField;
        [Tooltip("压测总人数")] public InputField peopleCountInputField;
        [Tooltip("登录频率，人/s")] public InputField spawnRateInputField;

        //暂时内容
        [Tooltip("内容展示面板")] public GameObject[] contents;

        [Tooltip("运行状态动画；空闲idle、速度慢（大于200ms）walk rm，速度快run rm，异常hit，开始压测shoot、连接服务器失败冰冻状态")]
        public SkeletonAnimation boySkeletonAnimation;

        //配置
        public StressTestingConfig stressTestingConfig;

        public StatisticContent statisticContent;


        void Start()
        {
            UIManager.Instance.Canvas = transform.parent.GetComponent<Canvas>();
            //获取ui对象组件
            startTestButton.onClick.AddListener(StartTest);
            stopTestButton.onClick.AddListener(StopTest);
            closeButton.onClick.AddListener(ClosePanel);
            statisticInfoButton.onClick.AddListener(ShowStatisticInfo);
            requestInterfaceInfoButton.onClick.AddListener(RequestShowInterfaceInfo);
            pushInterfaceInfoButton.onClick.AddListener(PushShowInterfaceInfo);
            workerInfoButton.onClick.AddListener(ShowWorkerInfo);

            UIManager.Instance.Canvas = GetComponentInParent<Canvas>();
            //禁用背景音
            AudioManager.Instance.StopMusic();
            StressTestingManager.Instance.Start(stressTestingConfig.RpcHost, stressTestingConfig.RpcPort);
        }

        private void OnEnable()
        {
            StressEventManager.Instance.AddEvent<StatisticsLogResponse>(StressEvent.StatisticsLogResponse,
                StatisticsLogResponse);
            StressTestingManager.Instance.RequestType = RequestType.StatisticsLog;
        }

        private void OnDisable()
        {
            StressEventManager.Instance.RemoveEvent<StatisticsLogResponse>(StressEvent.StatisticsLogResponse,
                StatisticsLogResponse);
        }

        private void OnDestroy()
        {
            StressTestingManager.Instance.Stop();
        }


        // Update is called once per frame
        void Update()
        {
            StressTestingManager.Instance.HandResponseMessage();
        }

        /// <summary>
        /// 收到统计消息返回
        /// </summary>
        /// <param name="response"></param>
        private void StatisticsLogResponse(StatisticsLogResponse response)
        {
            //死亡，压测服关闭
            if (response.Status == 2 && !"death".Equals(boySkeletonAnimation.AnimationName))
            {
                // boySkeletonAnimation.AnimationState.SetAnimation(0, "death", false);
                SetBoyAnimation("death",false);
                UIManager.Instance.ShowUI("NoticePanel", "压测服已关闭，开启压测服再重试");
                Log.Println("压测服务器未开启");
            }
            //空闲中
            else if (response.Status == 0 && !"idle".Equals(boySkeletonAnimation.AnimationName))
            {
                BoyAnimationIdle();
            }
            //压测中
            else if (response.Status == 1 && !"run".Equals(boySkeletonAnimation.AnimationName))
            {
                // boySkeletonAnimation.AnimationState.SetAnimation(0, "run", true);
                SetBoyAnimation("run");
            }

            statisticContent.UpdateContent(response.StatisticLog);
        }


        private void ShowStatisticInfo()
        {
            ChangeContent(0);
            StressTestingManager.Instance.RequestType = RequestType.StatisticsLog;
        }

        private void RequestShowInterfaceInfo()
        {
            ChangeContent(1);
        }

        private void PushShowInterfaceInfo()
        {
            ChangeContent(2);
        }

        private void ShowWorkerInfo()
        {
            ChangeContent(3);
        }


        /// <summary>
        /// 切换显示内容
        /// </summary>
        /// <param name="index"></param>
        private void ChangeContent(int index)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                if (i == index)
                {
                    contents[i].SetActive(true);
                    var scrollRect = contents[i].transform.parent.parent.gameObject.GetComponent<ScrollRect>();
                    if (scrollRect != null)
                    {
                        scrollRect.content = contents[i].GetComponent<RectTransform>();
                    }
                    else
                    {
                        Debug.LogError("内容未找到scrollRect组件");
                    }
                }
                else
                {
                    contents[i].SetActive(false);
                }
            }
        }


        /**
         * 关闭面板
         */
        private void ClosePanel()
        {
            SceneManager.LoadScene("MainScene");
            AudioManager.Instance.PlaySfx("button");
        }

        /// <summary>
        /// 设置动画
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="loop"></param>
        private void SetBoyAnimation(string animationName, bool loop = true)
        {
            boySkeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        }

        /// <summary>
        /// 空闲状态
        /// </summary>
        private void BoyAnimationIdle()
        {
            SetBoyAnimation("idle");
        }

        /**
         * 开始测试
         */
        private void StartTest()
        {
            AudioManager.Instance.PlaySfx("button");
            // boySkeletonAnimation.AnimationState.SetAnimation(0, "shoot", true);
            SetBoyAnimation("shoot", true);
            //默认外侧服
            var gateUrls = stressTestingConfig.GateUrls;
            if (!string.IsNullOrEmpty(gateUrlsInputField.text))
            {
                gateUrls = gateUrlsInputField.text;
            }

            var testType = 0;
            if (!string.IsNullOrEmpty(testTypeInputField.text))
            {
                testType = Convert.ToInt32(testTypeInputField.text);
            }

            //默认一个人
            var peopleCount = 1;
            if (!string.IsNullOrEmpty(peopleCountInputField.text))
            {
                peopleCount = Convert.ToInt32(peopleCountInputField.text);
            }

            //默认每秒登录一人
            var spawnRate = 1;
            if (!string.IsNullOrEmpty(spawnRateInputField.text))
            {
                spawnRate = Convert.ToInt32(spawnRateInputField.text);
            }

            StartTestRequest request = new StartTestRequest()
            {
                ServerHosts = gateUrls,
                SpawnRate = spawnRate,
                PlayerCount = peopleCount,
                TestType = testType,
            };
            StartCoroutine(RequestStartTest(request));
            StressTestingManager.Instance.TestStartTime = TimeUtil.CurrentTimeMillis();
        }


        /// <summary>
        /// 结束测试
        /// </summary>
        private void StopTest()
        {
            AudioManager.Instance.PlaySfx("button");
            // boySkeletonAnimation.AnimationState.SetAnimation(0, "frozen", false);
            SetBoyAnimation("frozen", false);
            StartCoroutine(RequestStopTest());
        }

        private IEnumerator RequestStopTest()
        {
            // StressTestingManager.Instance.ServiceClient.stopTest(new StopTestRequest());
            try
            {
                StressTestingManager.Instance.SaveStatisticLog(testTypeInputField.text, peopleCountInputField.text);
                StressTestingManager.Instance.ServiceClient.stopTest(new StopTestRequest());
                Invoke(nameof(BoyAnimationIdle), 3);
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowUI("NoticePanel", $"停止失败: {e.Message}");
                Debug.LogError(e);
                Log.Println($"停止失败: {e.Message}");
            }

            yield return null;
        }


        /// <summary>
        /// 请求压力测试
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IEnumerator RequestStartTest(StartTestRequest request)
        {
            try
            {
                var startTestResponse = StressTestingManager.Instance.ServiceClient.startTest(request);
                if (startTestResponse.Status != 0)
                {
                    UIManager.Instance.ShowUI("NoticePanel", $"压测失败: {startTestResponse.Result}");
                    SetBoyAnimation("idle");
                }
                else
                {
                    SetBoyAnimation("run");
                }
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowUI("NoticePanel", $"压测失败: {e.Message}");
                Debug.LogError(e);
                Log.Println($"压测失败: {e.Message}");
                SetBoyAnimation("idle");
            }

            yield return null;
        }
    }
}