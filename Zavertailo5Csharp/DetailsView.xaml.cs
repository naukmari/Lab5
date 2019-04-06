using System.Windows.Controls;
using Zavertailo5Csharp.ViewModel;

namespace Zavertailo5Csharp
{
    /// <summary>
    /// Interaction logic for DetailsView.xaml
    /// </summary>
    public partial class DetailsView : UserControl
    {
        internal DetailsView(System.Diagnostics.Process process)
        {
            InitializeComponent();
            DataContext = new DetailsViewModel(process.Modules, process.Threads);
        }
    }
}
