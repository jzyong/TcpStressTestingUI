using System;
using System.Threading;
using Core.Util;

namespace Core.Manager
{
    /// <summary>
    /// 线程池
    /// </summary>
    public class ThreadPoolManager : SingleInstance<ThreadPoolManager>
    {
        public void Execute(System.Action action)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                if (action != null) action();
            });
        }
    }
}