using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MsixInstaller;
internal sealed class SingleFileResourceManager : ResourceManager
{
    private readonly HashSet<CultureInfo> _cultures;
    public SingleFileResourceManager()
    {
        _cultures = Init();
    }

    public SingleFileResourceManager(Type resourceSource) : base(resourceSource)
    {
        _cultures = Init();
    }

    public SingleFileResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
    {
        _cultures = Init();
    }

    public SingleFileResourceManager(string baseName, Assembly assembly, Type usingResourceSet) : base(baseName, assembly, usingResourceSet)
    {
        _cultures = Init();
    }

    private HashSet<CultureInfo> Init()
    {
        return [.. MainAssembly
            .GetManifestResourceNames()
            .Where(i => i.StartsWith(BaseName) && i.EndsWith(".resources"))
            .Select(i =>
            {
                var name = i.Substring(BaseName.Length , i.Length - BaseName.Length - ".resources".Length );
                if (string.IsNullOrEmpty(name))
                    return CultureInfo.InvariantCulture;

                name = name.Substring(1);

                return CultureInfo.GetCultureInfo(name);
            })];
    }

    protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
    {
        Dictionary<CultureInfo, int> levels = new(_cultures.Count);

        foreach (var item in _cultures)
        {
            var current = culture;
            int level = 0;
            while (item.Name != current.Name)
            {
                level++;
                current = current.Parent;
                if (current.Name == CultureInfo.InvariantCulture.Name)
                {
                    level = int.MaxValue;
                    break;
                }
            }

            levels[item] = level;
        }

        int v = int.MaxValue;
        foreach (var item in levels)
        {
            if (item.Value <= v)
            {
                v = item.Value;
                culture = item.Key;
            }
        }

        var name = GetResourceFileName(culture);

        var stream = MainAssembly.GetManifestResourceStream(name);

        return new(stream);
    }

}
