namespace Core.Util.Collection
{
    /// <summary>
    /// 交换List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SwapList<T>
    {
        BetterList<T>[] lists = new BetterList<T>[2];
        BetterList<T> workingList = null;
        BetterList<T> waitingList = null;

        public SwapList()
        {
            lists[0] = new BetterList<T>();
            lists[1] = new BetterList<T>();
            workingList = lists[0];
            waitingList = lists[1];
        }

        /// <summary>
        /// 交换列表内容
        /// </summary>
        public void Swap()
        {
            lock (workingList)
            {
                BetterList<T> temp = workingList;
                workingList = waitingList;
                waitingList = temp;
            }
        }

        /// <summary>
        /// 工作列表长度
        /// </summary>
        /// <returns></returns>
        public int GetWorkingLength()
        {
            return workingList.size;
        }

        
        public void Add(T t)
        {
            lock (workingList)
            {
                workingList.Add(t);
            }
        }

        public T[] GetWaitingData()
        {
            return waitingList.ToArray();
        }

        public void ClearWaitingData()
        {
            lock (workingList)
            {
                waitingList.Clear();
            }
        }

        public void Clear()
        {
            waitingList.Clear();
            workingList.Clear();
        }
    }
}