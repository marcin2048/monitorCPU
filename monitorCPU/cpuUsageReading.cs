using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace monitorCPU
{
    class CpuUsageReading
    {
        PerformanceCounter cpuUsageCounter;

        double cpuUsage = 0;
        bool firstRun = true;

        /// <summary>
        /// Returns CPU usage value
        /// </summary>
        /// <returns></returns>
        public double getCpuUsage()
        {
            if (firstRun)
            {
                cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                cpuUsage = cpuUsageCounter.NextValue();
                firstRun = false;
            }
            else
            {
                cpuUsage = cpuUsageCounter.NextValue();
            }
            return cpuUsage;

        }

    }
}
