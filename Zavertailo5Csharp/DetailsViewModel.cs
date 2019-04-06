using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Zavertailo5Csharp.Annotations;

namespace Zavertailo5Csharp.ViewModel
{
    class DetailsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProcessThread> _threads;
        private ObservableCollection<ProcessModule> _modules;

        public ObservableCollection<ProcessThread> Threads
        {
            get => _threads;
            private set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<ProcessModule> Modules
        {
            get => _modules;
            private set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }


        /// <inheritdoc />
        internal DetailsViewModel(ProcessModuleCollection modules, ProcessThreadCollection threads)
        {
            Threads = new ObservableCollection<ProcessThread>(threads.Cast<ProcessThread>());
            Modules = new ObservableCollection<ProcessModule>(modules.Cast<ProcessModule>());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
