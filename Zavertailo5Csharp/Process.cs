using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management;

namespace Zavertailo5Csharp
{
    internal class Process
    { 
        public string ProcessName { get; }

        public int Id { get; }

        public bool IsActive { get; }

        public int CpuUsed { get; set; }

        public int RamUsed { get; set; }

        public int ThreadsCnt { get; set; }

        public string Owner { get; }

        public string FilePath { get; }

        public string WasStarted { get; }

        internal PerformanceCounter RamCnt { get; }

        internal PerformanceCounter CpuCnt { get; }

        internal Process(System.Diagnostics.Process systemProcess)
        {
            RamCnt = new PerformanceCounter("Process", "Working Set", systemProcess.ProcessName);
            CpuCnt = new PerformanceCounter("Process", "% Processor Time", systemProcess.ProcessName);
            ProcessName = systemProcess.ProcessName;
            Id = systemProcess.Id;
            IsActive = systemProcess.Responding;
            CpuUsed = (int)CpuCnt.NextValue();
            RamUsed = (int)(RamCnt.NextValue() / 1024 / 1024);
            ThreadsCnt = systemProcess.Threads.Count;
            Owner = GetOwner(systemProcess.Id);
            try
            {
                FilePath = systemProcess.MainModule.FileName;
            }
            catch (Win32Exception e)
            {
                FilePath = e.Message;
            }
            try
            {
                WasStarted = systemProcess.StartTime.ToString(CultureInfo.InvariantCulture);
            }
            catch (Win32Exception e)
            {
                WasStarted = e.Message;
            }
        }

        private static string GetOwner(int id)
        {
            ManagementObjectCollection processes = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ProcessID = " + id).Get();

            foreach (var o in processes)
            {
                var obj = (ManagementObject) o;
                object[] args = { "", "" };
                if (Convert.ToInt32(value: obj.InvokeMethod("GetOwner", args)) == 0)
                {
                    return args[1] + "\\" + args[0];
                }
            }
            return "OWNER NOT FOUND";
        }
    }
}
