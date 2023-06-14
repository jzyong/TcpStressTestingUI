
namespace Core.Util.Time
{
    /// <summary>
    /// float 时间计数器
    /// </summary>
    public class FloatTimeCounter
    {
        float startTime = 0.0f;
        float endTime = 0f;
        
        public FloatTimeCounter()
        {
            startTime = UnityEngine.Time.time;
        }
        public FloatTimeCounter(float endTime)
        {
            startTime = UnityEngine.Time.time;        
            this.endTime = endTime;
        }

        public float GetPassTime()
        {
            return UnityEngine.Time.time - startTime;        
        }
        public float GetPassTime(float factor)
        {
            return UnityEngine.Time.time * factor - startTime;
        }
        public float GetEndTime()
        {
            return endTime - GetPassTime();
        }
        public void AddStartTime(float time)
        {
            startTime += time;
        }
        public void ResetCounter()
        {
            startTime = UnityEngine.Time.time;        
        }
    }

    /// <summary>
    ///  
    /// </summary>
    public class LongTimeCounter
    {
        long startTime = 0;
        public LongTimeCounter()
        {
            startTime = System.DateTime.Now.Ticks;
        }

        public float GetPassTime()
        {
            float f = (System.DateTime.Now.Ticks - startTime) * 0.0000001f;
            return f;
        }

        public long GetPassLongTime()
        {
            long f = (System.DateTime.Now.Ticks - startTime);
            return f;
        }

        public void ResetCounter()
        {
            startTime = System.DateTime.Now.Ticks;
        }
    }

    // public class ServerUnixTimeCounter
    // {
    //     int m_endTime;
    //     int m_startTime;
    //     int m_createTime;
    //
    //     public void SetEndTime(int unitTime)
    //     {
    //         m_endTime = unitTime;
    //     }
    //     public void SetStartTime(int unitTime)
    //     {
    //         m_startTime = unitTime;
    //     }
    //     public void SetCreateTime(int unitTime)
    //     {
    //         m_createTime = unitTime;
    //     }
    //     public string GetRemainTimeString()
    //     {
    //         return Util.TimeHelper.SecondToMinString(m_endTime - Util.TimeHelper.UnixTime());
    //     }
    //     public string GetEnterTime()
    //     {
    //         return Util.TimeHelper.SecondToMinString(Util.TimeHelper.UnixTime()-m_startTime);
    //     }
    //
    //     public string GetStartTimeString()
    //     {
    //         //Debug.Log( "收到的时间："+ m_startTime+ "服务器时间:" +Util.TimeHelper.GetServerTime());
    //         return Util.TimeHelper.SecondToMinString(m_startTime - Util.TimeHelper.UnixTime()-1);
    //     }
    //     public int GetStartTime()
    //     {
    //         //Debug.Log("收到的时间：" + m_startTime + "服务器时间:" + Util.TimeHelper.UnixTime());
    //         return m_startTime - Util.TimeHelper.UnixTime()-1;
    //     }
    //     public int GetRemainTime()
    //     {
    //         return m_endTime - Util.TimeHelper.UnixTime(); 
    //     }
    //
    //     public int GetCreateTime()
    //     {
    //         return m_createTime - Util.TimeHelper.UnixTime(); 
    //     }
    // }
}