using System;
using System.Collections.Generic;
using System.Text;

namespace WhaleBot
{
    public static class TimeSpanExtensions
    {
        public static string ToReadable(this TimeSpan? timee)
        {          
            if(timee == null) return "Infinite";

            var time = (TimeSpan)timee;


            string timedays = time.Days > 0 ? time.Days > 1 ? time.Days + " days " : time.Days + " day " : "";
            string timehours = time.Hours > 0 ? time.Hours > 1 ? time.Hours + " hours " : time.Hours + " hour " : "";
            string timeminutes = time.Minutes > 0 ? time.Minutes > 1 ? time.Minutes + " minutes " : time.Minutes + " minute " : "";
            string timeseconds = time.Seconds > 0 ? time.Seconds > 1 ? time.Seconds + " seconds" : time.Seconds + " second" : "";

            return timedays + timehours + timeminutes + timeseconds;
        }
    }
}
