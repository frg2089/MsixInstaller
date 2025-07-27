namespace MsixInstaller.Resources;

public static partial class Resource
{
    static Resource()
    {
        s_resourceManager = new SingleFileResourceManager("MsixInstaller.Resources.Resource", typeof(Resource).Assembly);
    }
}
