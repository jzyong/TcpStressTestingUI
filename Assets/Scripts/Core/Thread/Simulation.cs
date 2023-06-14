using System;
using System.Collections.Generic;
using Platformer.Core;
using UnityEngine;

namespace Core.Thread
{
    /// <summary>
    /// The Simulation class implements the discrete event simulator pattern.
    /// Events are pooled, with a default capacity of 4 instances. @
    /// 模拟定时任务，线程池
    /// </summary>
    public static partial class Simulation
    {
        static HeapQueue<Simulation.Event> eventQueue = new HeapQueue<Simulation.Event>();

        static Dictionary<System.Type, Stack<Simulation.Event>> eventPools =
            new Dictionary<System.Type, Stack<Simulation.Event>>();

        static DateTime startTime =
            TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)); // 当地时区


        /// <summary>
        /// Create a new event of type T and return it, but do not schedule it. @
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static public T New<T>() where T : Simulation.Event, new()
        {
            Stack<Simulation.Event> pool;
            if (!eventPools.TryGetValue(typeof(T), out pool))
            {
                pool = new Stack<Simulation.Event>(4);
                pool.Push(new T());
                eventPools[typeof(T)] = pool;
            }

            if (pool.Count > 0)
                return (T) pool.Pop();
            else
                return new T();
        }

        /// <summary>
        /// Clear all pending events and reset the tick to 0.
        /// </summary>
        public static void Clear()
        {
            eventQueue.Clear();
        }

        /// <summary>
        /// Schedule an event for a future tick, and return it. @
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="tick">Tick.</param>
        /// <typeparam name="T">The event type parameter.</typeparam>
        static public T Schedule<T>(float tick = 0) where T : Simulation.Event, new()
        {
            var ev = New<T>();
            // ev.tick = Time.time + tick;
            long timeStamp = (long) ((DateTime.Now - startTime).TotalMilliseconds); // 相差毫秒数
            ev.tick = timeStamp + tick;
            eventQueue.Push(ev);
            return ev;
        }


        /// <summary>
        /// Reschedule an existing event for a future tick, and return it. @
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="tick">Tick.</param>
        /// <typeparam name="T">The event type parameter.</typeparam>
        static public T Reschedule<T>(T ev, float tick) where T : Simulation.Event, new()
        {
            // ev.tick = Time.time + tick;
            long timeStamp = (long) ((DateTime.Now - startTime).TotalMilliseconds); // 相差毫秒数
            ev.tick = timeStamp + tick;
            eventQueue.Push(ev);
            return ev;
        }

        /// <summary>
        /// Return the simulation model instance for a class. @
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static public T GetModel<T>() where T : class, new()
        {
            return Simulation.InstanceRegister<T>.instance;
        }

        /// <summary>
        /// Set a simulation model instance for a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static public void SetModel<T>(T instance) where T : class, new()
        {
            Simulation.InstanceRegister<T>.instance = instance;
        }

        /// <summary>
        /// Destroy the simulation model instance for a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static public void DestroyModel<T>() where T : class, new()
        {
            Simulation.InstanceRegister<T>.instance = null;
        }

        /// <summary>
        /// Tick the simulation. Returns the count of remaining events.
        /// If remaining events is zero, the simulation is finished unless events are
        /// injected from an external system via a Schedule() call. @
        /// </summary>
        /// <returns></returns>
        static public int Tick()
        {
            var time = (DateTime.Now - startTime).TotalMilliseconds;
            //  var time = Time.time;
            var executedEventCount = 0;
            while (eventQueue.Count > 0 && eventQueue.Peek().tick <= time)
            {
                var ev = eventQueue.Pop();
                var tick = ev.tick;
                ev.ExecuteEvent();
                if (ev.tick > tick)
                {
                    //event was rescheduled, so do not return it to the pool.
                }
                else
                {
                    // Debug.Log($"<color=green>{ev.tick} {ev.GetType().Name}</color>");
                    ev.Cleanup();
                    try
                    {
                        eventPools[ev.GetType()].Push(ev);
                    }
                    catch (KeyNotFoundException)
                    {
                        //This really should never happen inside a production build.
                        Debug.LogError($"No Pool for: {ev.GetType()}");
                    }
                }

                executedEventCount++;
            }

            return eventQueue.Count;
        }
    }
}