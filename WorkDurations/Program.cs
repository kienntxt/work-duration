using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDurations
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime t1 = new DateTime(2020, 06, 01, 11, 0, 1);
            DateTime t2 = new DateTime(2020, 06, 8, 8, 1, 1);

            TimeSpan startTime = new TimeSpan(7, 30, 0);
            TimeSpan endTime = new TimeSpan(18, 30, 0);
            TimeSpan workingTime = new TimeSpan(11, 0, 0);
            List<DateTime> holidays = new List<DateTime>();
            holidays.Add(new DateTime(2020, 6, 3));

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
            DateTime t1x = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second);
            if (t1.Subtract(new DateTime(t1.Year, t1.Month, t1.Day)) < startTime)
                t1x = new DateTime(t1.Year, t1.Month, t1.Day).AddSeconds(startTime.TotalSeconds);
            if (t1.Subtract(new DateTime(t1.Year, t1.Month, t1.Day)) > endTime)
                t1x = new DateTime(t1.Year, t1.Month, t1.Day).AddDays(1).AddSeconds(startTime.TotalSeconds);

            DateTime t2x = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second);
            if (t2.Subtract(new DateTime(t2.Year, t2.Month, t2.Day)) < startTime)
                t2x = new DateTime(t2.Year, t2.Month, t2.Day).AddDays(-1).AddSeconds(endTime.TotalSeconds);

            if (t2.Subtract(new DateTime(t2.Year, t2.Month, t2.Day)) > endTime)
                t2x = new DateTime(t2.Year, t2.Month, t2.Day).AddSeconds(endTime.TotalSeconds);

            var minT1 = new DateTime(t1x.Year, t1x.Month, t1x.Day).AddSeconds(startTime.TotalSeconds);
            var maxT1 = new DateTime(t1x.Year, t1x.Month, t1x.Day).AddSeconds(endTime.TotalSeconds);
            var minT2 = new DateTime(t2x.Year, t2x.Month, t2x.Day).AddSeconds(startTime.TotalSeconds);
            var maxT2 = new DateTime(t2x.Year, t2x.Month, t2x.Day).AddSeconds(endTime.TotalSeconds);

            TimeSpan ts = maxT2.Subtract(minT1);
            //trong 1 ngay lam viec:
            if (ts.TotalSeconds <= workingTime.TotalSeconds)
            {
                if (!holidayList.Any(x => x.Date == t1x.Date) &&
                    t1x.DayOfWeek != DayOfWeek.Saturday && t1x.DayOfWeek != DayOfWeek.Sunday)
                    //ngay hien tai khong phai la cuoi tuan va khong phai la ngay nghi:
                    return ts.Add(t1x.Subtract(minT1)).Add(t2x.Subtract(maxT2));
                else
                    //ngay hien tai la cuoi tuan hoac ngay nghi:
                    return new TimeSpan(0);
            }
            else //> 1 ngay
            {
                var days = ts.TotalDays;
                for (int i = 0; i <= days; i++)
                {
                    DateTime ti = minT1.AddDays(i);
                    if (holidayList.Any(x => x.Date == ti.Date) ||
                        ti.DayOfWeek == DayOfWeek.Saturday || ti.DayOfWeek == DayOfWeek.Sunday)
                        ts = ts.Add(new TimeSpan(-24, 0, 0));
                }
            }
            //doan thoi gian tu startTime den thoi diem t1 (trong ngay t1)
            //first time la so am
            var firstTime = minT1.Subtract(t1x);
            //doan thoi gian tu startTime cua den thoi diem t2 (trong ngay t2)
            //last time la so duong
            var lastTime = t2x.Subtract(minT2);
            var duration = ts.Add(firstTime).Add(lastTime);
            return duration.TotalDays > 0 ? duration : new TimeSpan(0);
        }
    }
}
