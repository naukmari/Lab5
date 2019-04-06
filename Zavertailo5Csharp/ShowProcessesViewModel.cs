using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zavertailo5Csharp.Tools;

namespace Zavertailo5Csharp.ViewModel
{
    internal class ShowProcessesViewModel : INotifyPropertyChanged
    {
        private readonly Action<bool> _showLoaderAction;
        private ObservableCollection<Process> _processes;
        private readonly Thread _updateThread;
        private Process _selectedProcess;
        private RelayCommand<object> _getInfoCommand;
        private RelayCommand<object> _getFileLocationCommand;
        private RelayCommand<object> _killTaskCommand;

        private Details _details;

        public Process SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
                OnPropertyChanged("IsItemSelected");
            }
        }

        public ObservableCollection<Process> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal ShowProcessesViewModel(Action<bool> showLoaderAction)
        {
            _showLoaderAction = showLoaderAction;
            _updateThread = new Thread(UpdateUsers);
            Thread initializationThread = new Thread(InitializeProcesses);
            initializationThread.Start();
        }

        public RelayCommand<object> GetInfo => _getInfoCommand ?? (_getInfoCommand = new RelayCommand<object>(GetInfoImpl));
        public RelayCommand<object> GetFileLocation => _getFileLocationCommand ?? (_getFileLocationCommand = new RelayCommand<object>(OpenFileLocationImpl));
        public RelayCommand<object> KillTaskCommand => _killTaskCommand ?? (_killTaskCommand = new RelayCommand<object>(KillTaskImpl));

        private async void GetInfoImpl(object o)
        {
            try
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        var process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                        _details?.Close();
                        try
                        {
                            _details = new Details(process);
                            _details.Show();
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async void OpenFileLocationImpl(object o)
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", process.MainModule.FileName.Remove(process.MainModule.FileName.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private void KillTaskImpl(object o)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
            try
            {
                process.Kill();
            }
            catch (Win32Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async void UpdateUsers()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        try
                        {
                            lock (Processes)
                            {
                                var toRemove =
                                    new List<Process>(
                                        Processes.Where(proc => !Database.ProcessesList.ContainsKey(proc.Id)));
                                foreach (Process proc in toRemove)
                                {
                                    Processes.Remove(proc);
                                }

                                var toAdd =
                                    new List<Process>(
                                        Database.ProcessesList.Values.Where(proc => !Processes.Contains(proc)));
                                foreach (var proc in toAdd)
                                {
                                    Processes.Add(proc);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });
                });
                Thread.Sleep(4000);
            }
        }

        private async void InitializeProcesses()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate { _showLoaderAction.Invoke(true); });
            await Task.Run(() =>
            {
                Processes = new ObservableCollection<Process>(Database.ProcessesList.Values);
            });
            _updateThread.Start();
            while (Database.ProcessesList.Count == 0)
                Thread.Sleep(3000);
            System.Windows.Application.Current.Dispatcher.Invoke(delegate { _showLoaderAction.Invoke(false); });
        }

        internal void Close()
        {
            _updateThread.Join(3000);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
