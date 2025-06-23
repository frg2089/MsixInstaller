using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MsixInstaller;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly MainWindow _view;

    [ObservableProperty]
    private bool _enabledButton = true;

    [ObservableProperty]
    private bool _flyoutOpen = false;

    public string Path { get; }
    public string Title { get; }
    public string Publisher { get; }
    public string Subject { get; }
    public string Version { get; }
    public ImageSource? Logo { get; }

    public bool IsConfirm { get; private set; }

    public MainWindowViewModel(MainWindow view, string path, X509Certificate2 cert)
    {
        _view = view;
        Path = path;
        Subject = cert.Subject;
        Publisher = Subject.Split(',').Select(i => i.Split('=')).Where(i => i?.Length is 2).ToDictionary(i => i[0], i => i[1])["CN"];

        using var zip = ZipFile.OpenRead(path);
        var manifest = zip.GetEntry("AppxManifest.xml") ?? throw new Exception("AppxManifest.xml not found");
        using var fs = manifest.Open();
        XmlDocument doc = new();
        doc.Load(fs);
        var identity = doc.GetElementsByTagName("Identity").Item(0) as XmlElement;
        Version = identity?.GetAttribute("Version") ?? "1.0.0.0";

        var properties = doc.GetElementsByTagName("Properties").Item(0)?.ChildNodes.OfType<XmlElement>();
        Title = properties
            .FirstOrDefault(i => i is { LocalName: "DisplayName", NamespaceURI: "http://schemas.microsoft.com/appx/manifest/foundation/windows10" })
            ?.InnerText
            .Trim() ?? "未知的软件包";

        var logo = zip.Entries.Where(i => i.FullName.StartsWith("Images/StoreLogo")).Reverse().FirstOrDefault();

        if (logo is { })
        {
            BitmapImage bmp = new();
            using var stream = logo.Open();
            MemoryStream ms = new();
            stream.CopyTo(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.EndInit();
            Logo = bmp;
        }
    }

    [RelayCommand]
    private void OpenFlyout()
    {
        FlyoutOpen = true;
    }

    [RelayCommand]
    private void Cancel()
    {
        _view.Close();
    }

    [RelayCommand]
    private void Confirm()
    {
        try
        {
            EnabledButton = false;
            var app = Assembly.GetExecutingAssembly().Location;
            Process process = new()
            {
                StartInfo = new()
                {
                    FileName = app,
                    Arguments = $"--confirm \"{Path}\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "RunAs"
                }
            };
            process.Start();
            process.WaitForExit();

            IsConfirm = true;
            _view.Close();
        }
        catch
        {
        }
        finally
        {
            EnabledButton = true;
        }
    }
}
