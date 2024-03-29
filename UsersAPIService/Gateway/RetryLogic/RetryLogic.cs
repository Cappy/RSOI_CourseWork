﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.RetryLogic
{
    public class Retry
    {
        private static ConcurrentQueue<Func<Task<bool>>> tasks = new ConcurrentQueue<Func<Task<bool>>>();
        private static Timer timer = new Timer(TimerCallback, null, 0, 500);

        public static void RetryUntilSuccess(Func<Task<bool>> func) => tasks.Enqueue(func);

        private static void TimerCallback(object o)
        {
            Func<Task<bool>> task = null;
            while (tasks.Count > 0)
                if (tasks.TryDequeue(out task) && !task.Invoke().Result)
                    tasks.Enqueue(task);
        }
    }
}

