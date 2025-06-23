using System.Windows;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace MsixInstaller;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SystemThemeWatcher.Watch(
            this,
            WindowBackdropType.Auto,
            true
        );
    }
}