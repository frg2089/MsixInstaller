using System.Windows;

using Wpf.Ui.Appearance;

namespace MsixInstaller;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ApplicationThemeManager.ApplySystemTheme();
    }
}
