using System;
using System.Windows.Controls;
using Zavertailo5Csharp.ViewModel;

namespace Zavertailo5Csharp
{
    /// <summary>
    /// Interaction logic for ShowProcesses.xaml
    /// </summary>
    public partial class ShowProcesses : UserControl
    {
        internal ShowProcesses(Action<bool> loader)
        {
            InitializeComponent();
            DataContext = new ShowProcessesViewModel(loader);
        }

       internal void Close() => ((ShowProcessesViewModel)DataContext).Close();
    }
}
