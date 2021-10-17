using HardwareProviders.CPU;
using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;
using System.Management;

namespace monitorCPU
{
    class TemperatureReading
    {

        private double cpuTemperature = 0;


        /// <summary>
        /// Main temperature procedure
        /// </summary>
        /// <returns></returns>
        public double getTemperature(int tempMode=0)
        {
            switch (tempMode)
            {
                case 0: getTemp_OpenHardware_Package(); break;
                case 1: getTemp_Win32_TemperatureProbe(); break;
                case 2: getTemp_HardwareProviders_Package(); break;
                case 3: getTemp_LibreHardware_Packgae();  break;
                case 4: getTemp_MSAcpi_ThermalZone(); break;
                default: return 0.0;
            }
            return cpuTemperature;
        }


        /// <summary>
        /// Class for Libre Hardware package procedure
        /// </summary>
        private class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }


        /// <summary>
        /// Get temperature using Libre Hardware package
        /// </summary>
        void getTemp_LibreHardware_Packgae()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = false,
                IsMemoryEnabled = false,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = false,
                IsStorageEnabled = false,
                IsPsuEnabled = true
            };
            //
            computer.Open();
            //
            computer.Accept(new UpdateVisitor());
            for (int i = 0; i < computer.Hardware.Count; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.Cpu)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == LibreHardwareMonitor.Hardware.SensorType.Temperature)
                        {
                            Console.WriteLine(computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\r");
                            if (computer.Hardware[i].Sensors[j].Value != null)
                                cpuTemperature = Convert.ToDouble(computer.Hardware[i].Sensors[j].Value.ToString());
                        }
                    }
                }
            }
            computer.Close();
        }

        /// <summary>
        /// Get Temperature using Win32_TemperatureProbe object
        /// </summary>
        void getTemp_Win32_TemperatureProbe()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root/CIMV2", "SELECT * FROM Win32_TemperatureProbe ");
            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj.GetPropertyValue("CurrentReading") != null)
                {
                    cpuTemperature = Convert.ToDouble(obj.GetPropertyValue("CurrentReading").ToString());
                }
            }
        }


        /// <summary>
        /// Get Temperature using MSAcpi_ThermalZoneTemperature object
        /// </summary>
        void getTemp_MSAcpi_ThermalZone()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj.GetPropertyValue("CurrentTemperature") != null)
                    {
                        cpuTemperature = Convert.ToDouble(obj.GetPropertyValue("CurrentTemperature").ToString());

                    }
                }

            }
            catch (Exception exception1)
            {
                //Exception for some hardware configurations
            }
        }


        /// <summary>
        /// Get temperature using hardware providers nuget package
        /// </summary>
        void getTemp_HardwareProviders_Package()
        {
            var cpu = Cpu.Discover();
            foreach (var item in cpu)
            {
                var s2 = item.PackageTemperature;
                if (s2 != null)
                {
                    cpuTemperature = Convert.ToDouble(s2.Value.ToString());
                }
                var ss = item.CoreTemperatures;
                if (ss.Count() > 0)
                {
                    var s1 = ss[0];
                    cpuTemperature = Convert.ToDouble(s1.Value.ToString());
                }
            }
        }

        /// <summary>
        /// Get temperature using OpenHardware package
        /// </summary>
        void getTemp_OpenHardware_Package()
        {
            var myComputer = new OpenHardwareMonitor.Hardware.Computer();
            myComputer.CPUEnabled = true;
            myComputer.Open();
            foreach (var hardwareItem in myComputer.Hardware)
            {
                if (hardwareItem.HardwareType == OpenHardwareMonitor.Hardware.HardwareType.CPU)
                {
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == OpenHardwareMonitor.Hardware.SensorType.Temperature)
                        {
                            if (sensor.Value != null)
                            {
                                if (sensor.Index == 1)
                                {
                                    cpuTemperature = Convert.ToDouble(sensor.Value.ToString());
                                }
                            }
                        }
                    }
                }

            }

        }


    }
}
