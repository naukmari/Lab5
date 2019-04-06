using System.ComponentModel;
using System.Windows;
using FontAwesome.WPF;
using Zavertailo5Csharp.Tools;

namespace Zavertailo5Csharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageAwesome _loader;
        private ShowProcesses _processes;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            Main.Children.Clear();
            if (_processes == null)
                _processes = new ShowProcesses(ShowLoader);
            Main.Children.Add(_processes);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _processes?.Close();
            Database.Close();
            base.OnClosing(e);
        }

        private void ShowLoader(bool isShow)
        {
            Loader.OnRequestLoader(Main, ref _loader, isShow);
        }
    }
}
