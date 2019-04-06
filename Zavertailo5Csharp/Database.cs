using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zavertailo5Csharp
{
    internal static class Database
    {
        private static readonly Thread UpdateDatabaseThread;
        private static readonly Thread UpdateProcessesThread;

        internal static Dictionary<int, Process> ProcessesList { get; set; }

        static Database()
        {
            ProcessesList = new Dictionary<int, Process>();
            UpdateProcessesThread = new Thread(UpdateProcesses);
            UpdateDatabaseThread = new Thread(UpdateDatabase);
            UpdateDatabaseThread.Start();
            UpdateProcessesThread.Start();
        }

        private static async void UpdateDatabase()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (ProcessesList)
                    {
                        var processes = System.Diagnostics.Process.GetProcesses().ToList();
                        var keys = ProcessesList.Keys.ToList()
                            .Where(id => processes.All(proc => proc.Id != id));
                        foreach (var key in keys)
                        {
                            ProcessesList.Remove(key);
                        }

                        foreach (var p in processes)
                        {
                            if (ProcessesList.ContainsKey(p.Id)) continue;
                            try
                            {
                                ProcessesList[p.Id] = new Process(p);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                });
                Thread.Sleep(4000);
            }
        }

        private static async void UpdateProcesses()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (ProcessesList)
                    {
                        for (var i = 0; i < ProcessesList.Keys.ToList().Count; i++)
                        {
                            var id = ProcessesList.Keys.ToList()[i];
                            System.Diagnostics.Process process;
                            try
                            {
                                process = System.Diagnostics.Process.GetProcessById(id);
                            }
                            catch (ArgumentException)
                            {
                                ProcessesList.Remove(id);
                                continue;
                            }
                            ProcessesList[id].RamUsed = (int) (ProcessesList[id].RamCnt.NextValue() / 1048576);
                            ProcessesList[id].CpuUsed = (int) ProcessesList[id].CpuCnt.NextValue();
                            ProcessesList[id].ThreadsCnt = process.Threads.Count;
                        }
                    }
                });
                Thread.Sleep(2000);
            }
        }

        internal static void Close()
        {
            UpdateDatabaseThread.Join(3500);
            UpdateProcessesThread.Join(1500);
        }
    }
}
