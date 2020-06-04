using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace WorkDurations
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime t1 = new DateTime(2020, 5, 3, 11, 03, 52);
            DateTime t2 = new DateTime(2020, 5, 4, 8, 34, 52);

            TimeSpan startTime = new TimeSpan(7, 30, 0);
            TimeSpan endTime = new TimeSpan(18, 30, 0);
            TimeSpan workingTime = new TimeSpan(11, 0, 0);
            List<DateTime> holidays = new List<DateTime>();
            holidays.Add(new DateTime(2020, 5, 1));

            var duration = getDuration(t1, t2, startTime, endTime, workingTime, holidays);
            if (duration.TotalSeconds < 0)
                duration = new TimeSpan(0);
            Console.WriteLine(
                t2.ToString("yyyy-MM-dd HH:mm:ss") + " - " + 
                t1.ToString("yyyy-MM-dd HH:mm:ss") + " = " + 
                duration.ToString());
            Console.ReadLine();
        }

        static TimeSpan getDuration(DateTime t1, DateTime t2,
            TimeSpan startTime, TimeSpan endTime, TimeSpan workingTime,
            List<DateTime> holidayList)
        {
            if (t1.Date > t2.Date)
                return new TimeSpan(0);

            //same date:
            if (t1.Date == t2.Date)
            {
                DateTime t1x = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
                if (t1.Subtract(t1.Date) < startTime)
                    t1x = t1.Date.AddSeconds(startTime.TotalSeconds);
                if (t1.Subtract(t1.Date) > endTime)
                    t1x = t1.Date.AddDays(1).AddSeconds(startTime.TotalSeconds);

                DateTime t2x = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
                if (t2.Subtract(t2.Date) < startTime)
                    t2x = t2.Date.AddDays(-1).AddSeconds(endTime.TotalSeconds);
                if (t2.Subtract(t2.Date) > endTime)
                    t2x = t2.Date.AddSeconds(endTime.TotalSeconds);

                if (holidayList.Any(x => x.Date == t1x.Date) ||
                    t1x.DayOfWeek == DayOfWeek.Saturday || t1x.DayOfWeek == DayOfWeek.Sunday)
                    return new TimeSpan(0);

                return t2x.Subtract(t1x);
            }

            TimeSpan duration = new TimeSpan(0);
            var days = t2.Date.Subtract(t1.Date).Days;
            for (int i = 0; i <= days; i++)
            {
                DateTime ti = t1.Date.AddDays(i);
                if (!holidayList.Any(x => x.Date == ti.Date) &&
                    ti.DayOfWeek != DayOfWeek.Saturday && ti.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (ti.Date == t1.Date)
                    {
                        if (t1.Subtract(t1.Date.Add(startTime)).TotalSeconds <= 0)
                            duration = duration.Add(workingTime);
                        else if (t1.Subtract(t1.Date.Add(endTime)).TotalSeconds <= 0)
                        {
                            duration = duration.Add(t1.Date.Add(endTime).Subtract(t1));
                        }
                    }
                    else if (ti.Date == t2.Date)
                    {
                        if (t2.Subtract(t2.Date.Add(endTime)).TotalSeconds >= 0)
                            duration = duration.Add(workingTime);
                        else if (t2.Subtract(t2.Date.Add(startTime)).TotalSeconds > 0)
                        {
                            duration = duration.Add(t2.Subtract(t2.Date.Add(startTime)));
                        }
                    }
                    else
                    {
                        duration = duration.Add(workingTime);
                    }
                }
            }
            int wdays = Convert.ToInt32(Math.Floor(duration.TotalSeconds / workingTime.TotalSeconds));
            TimeSpan result = new TimeSpan(wdays, 0, 0, 0);
            var remain = duration.TotalSeconds - wdays * workingTime.TotalSeconds;
            TimeSpan time = TimeSpan.FromSeconds(remain);
            result = result.Add(time);
            return result;
        }
    }
}
