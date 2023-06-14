using UnityEngine;

namespace Common.UI
{
    /// <summary>
    /// UI基础类
    /// </summary>
    public class UIBase : MonoBehaviour
    {


        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
           
        }


        /// <summary>
        /// 入场动画播放之前的回调
        /// </summary>
        /// <param name="preUI">上一个界面</param>
        /// <param name="args">其他界面调用时给过来的参数列表, 和传递给OnShow的参数相同</param>
        public virtual void PreShow(UIBase preUI, params object[] args)
        {
        }

        /// <summary>
        /// 入场动画播放完后的回调
        /// </summary>
        /// <param name="preUI">上一个界面</param>
        /// <param name="args">其他界面调用时给过来的参数列表，和传递给PreShow的参数相同</param>
        public virtual void OnShow(UIBase preUI, params object[] args)
        {
        }

        /// <summary>
        /// 已被隐藏
        /// <param name="nextUI">下一个要显示的界面</param>
        /// </summary>
        public virtual void OnHide(UIBase nextUI)
        {
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        /// <returns></returns>
        public bool IsShow()
        {
            if (gameObject == null)
            {
                return false;
            }

            return gameObject.activeSelf;
        }

        public void HideSelf()
        {
            UIManager.Instance.HideUI(this);
        }
    }
}