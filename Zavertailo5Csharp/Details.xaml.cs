using System.Windows;


namespace Zavertailo5Csharp
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : Window
    {
        private DetailsView _infoView;

        internal Details(System.Diagnostics.Process process)
        {
            InitializeComponent();
            Title = $"Info about {process.ProcessName}";
            ShowInfoView(process);
        }

        private void ShowInfoView(System.Diagnostics.Process process)
        {
            Main.Children.Clear();
            if (_infoView == null)
                _infoView = new DetailsView(process);
            Main.Children.Add(_infoView);
        }
    }
}
