using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        private readonly Stopwatch stopWatch = new Stopwatch();
        private readonly List<Timer> childs = new List<Timer>();
        private readonly StringWriter writer;
        private readonly string name;
        private readonly int level;

        public Timer(StringWriter writer, string name, int level)
        {
            this.writer = writer;
            this.name = name;
            this.level = level;
            stopWatch.Start();
        }

        private static string FormatReportLine(string timerName, int level, long value)
        {
            var intro = new string(' ', level * 4) + timerName;
            return $"{intro,-20}: {value}\n";
        }

        public void Dispose()
        {
            stopWatch.Stop();
            if (level == 0)
            {
                WriteReport();
                writer.Dispose();
            }
        }

        private void WriteReport()
        {
            writer.Write(FormatReportLine(name, level, stopWatch.ElapsedTicks));
            if (childs.Count != 0)
            { 
                childs.ForEach(child => child.WriteReport());
                var totalChildsTicks = childs.Sum(child => child.stopWatch.ElapsedTicks);
                writer.Write(FormatReportLine("Rest", level + 1, stopWatch.ElapsedTicks - totalChildsTicks));
            }
        }

        public static Timer Start(StringWriter writer, string timerName = "*")
        {
            return new Timer(writer, timerName, 0);
        }

        public Timer StartChildTimer(string timerName)
        {
            var timer = new Timer(this.writer, timerName, level + 1);
            childs.Add(timer);
            return timer;
        }
    }
}
