using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using MsixInstaller;

if (args.Length is >= 2 && args[0] is "--confirm")
{
    X509Certificate2 x509Certificate = new(X509Certificate.CreateFromSignedFile(args[1]));
    X509Store store = new(StoreName.TrustedPeople, StoreLocation.LocalMachine);
    store.Open(OpenFlags.ReadWrite);
    store.Add(x509Certificate);
    return;
}

string path = $"{Path.GetTempFileName()}.msix";
using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream("msix"))
using (FileStream fileStream = File.Create(path))
    await stream.CopyToAsync(fileStream);
try
{
    X509Certificate2 x509Certificate = new(X509Certificate.CreateFromSignedFile(path));
    X509Store store = new(StoreName.TrustedPeople, StoreLocation.LocalMachine);
    store.Open(OpenFlags.ReadOnly);
    if (store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, x509Certificate.Subject, true) is { Count: 0 })
    {
        bool confirm = false;
        Thread thread = new(() =>
        {
            App app = new();
            MainWindow window = new();
            MainWindowViewModel viewModel = new(window, path, x509Certificate);
            window.DataContext = viewModel;
            app.Run(window);
            confirm = viewModel.IsConfirm;
        })
        {
            Name = "UI Thread",
            IsBackground = false,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (!confirm)
            return;
    }
    //store.Open(OpenFlags.ReadWrite);

    Process.Start(path);
    // 这里因为返回的是 null 所以使用了一点魔法
    Task.WaitAll([.. Process.GetProcesses()
        .Where(i => i.ProcessName is "AppInstaller")
        .Select(i => Task.Run(i.WaitForExit))]);
}
finally
{
    File.Delete(path);
}