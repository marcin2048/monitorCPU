using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace monitorCPU
{
    class CpuFreqReading
    {

        PerformanceCounter cpuValueCounter;

        double cpuValue;
        double cpuMaxFreq;
        double cpuFreq;
        bool firstRun = true;

        /// <summary>
        /// Returns actual CPU frequency value
        /// </summary>
        /// <returns></returns>
        public double getCpuFreq()
        {
            if (firstRun)
            {
                cpuValueCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total");
                cpuValue = cpuValueCounter.NextValue();
                //get max clock speed only once
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT *, Name FROM Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    cpuMaxFreq = Convert.ToDouble(obj["MaxClockSpeed"]) / 1000;
                }
                firstRun = false;
            }
            else
            {
                cpuValue = cpuValueCounter.NextValue();
                cpuFreq = cpuMaxFreq * cpuValue / 100 * 1000;
            }
            return cpuFreq;

        }



    }
}
