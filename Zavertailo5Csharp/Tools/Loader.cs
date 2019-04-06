using System.Windows.Controls;
using FontAwesome.WPF;

namespace Zavertailo5Csharp.Tools
{
    class Loader
    {
        internal static void OnRequestLoader(Grid grid, ref ImageAwesome loaderIcon, bool visible)
        {
            if (visible && loaderIcon == null)
            {
                loaderIcon = new ImageAwesome();
                grid.Children.Add(loaderIcon);
                loaderIcon.Width = loaderIcon.Height = 20;

                loaderIcon.Icon = FontAwesomeIcon.Refresh;
                loaderIcon.Spin = true;
                Grid.SetRow(loaderIcon, 1);
            }
            else
            {
                grid.Children.Remove(loaderIcon);
            }
        }
    }
}
