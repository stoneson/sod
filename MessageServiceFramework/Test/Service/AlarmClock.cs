﻿using PWMIS.EnterpriseFramework.Service.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSample
{
    public class AlarmClockService : IService
    {
        System.Timers.Timer timer;
        DateTime AlarmTime;
        IServiceContext context;
        int publishCount;

        public event EventHandler Alarming;

        public AlarmClockService()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += timer_Elapsed;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (e.SignalTime >= this.AlarmTime)
            {
                if (Alarming != null)
                    Alarming(this, new EventArgs());

                context.PublishData(DateTime.Now); //e.SignalTime
                publishCount++;
                Console.WriteLine("AlarmClockService Publish Count:{0}",publishCount);
            }
            if (publishCount > 10)
            {
                timer.Stop();
                Console.WriteLine("AlarmClockService Timer Stoped. ");
            }
        }

       
        public ServiceEventSource SetAlarmTime(DateTime targetTime)
        {
            publishCount = 0;
            this.AlarmTime = targetTime;
            timer.Start();
            return new ServiceEventSource(timer,1);
        }


        public void CompleteRequest(IServiceContext context)
        {

        }

        public bool ProcessRequest(IServiceContext context)
        {
            this.context = context;
            return true;
        }

        public bool IsUnSubscribe
        {
            get { return false; }
        }
    }
}
