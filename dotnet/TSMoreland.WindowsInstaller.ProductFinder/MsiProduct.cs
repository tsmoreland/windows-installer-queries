using System.Collections.Generic;
using System.Threading;
using Microsoft.Win32;

namespace TSMoreland.WindowsInstaller.ProductFinder;

internal sealed class MsiProduct 
{
    private ThreadLocal<StringBuilder> _builder = new (() => new StringBuilder());

    public MsiProduct(Guid productCode)
    {
        ProductCode = productCode;
    }

    public static IEnumerable<MsiProduct> GetProductsFromUpgradeCode(Guid upgradeCode)
    {
        int index = 0;
        StringBuilder productCodeBuilder = new(39); // capacity for a GUID

        int result;
        while ((result = NativeMethods. MsiEnumRelatedProducts(upgradeCode.ToString("B"), 0, index++, productCodeBuilder)) == Win32ResultCodes.Success.Value())
        {
            if (Guid.TryParse(productCodeBuilder.ToString(), out Guid productCode))
            {
                yield return new MsiProduct(productCode);
            }
            productCodeBuilder.Clear();
        }

        if (result != Win32ResultCodes.NoMoreData.Value())
        {
            // ...
        }
    }

    public Guid ProductCode { get; }

    public bool TryGetVersion(out Version? version)
    {
        return TryGetVersionFromProductInfo(out version) ||
               TryGetVersionFromRegistry(out version);
    }

    private bool TryGetVersionFromProductInfo(out Version? version)
    {
        version = null;

        _builder.Value.Clear();
        _builder.Value.EnsureCapacity(2048);
        int length = _builder.Value.Capacity;

        try
        {
            int result = NativeMethods
                .MsiGetProductInfo(ProductCode.ToString("c"), MsiProperty.VersionString, _builder.Value, ref length);
            if (result != Win32ResultCodes.Success.Value())
            {
                return false;
            }

            if (!Version.TryParse(_builder.ToString(), out Version productVersion))
            {
                return false;
            }

            version = productVersion;
            return true;
        }
        finally
        {
            _builder.Value.Clear();
        }
    }

    private bool TryGetVersionFromRegistry(out Version? version)
    {
        version = null;
        try
        {

            _builder.Value.Clear();
            _builder.Value.EnsureCapacity(39);

            _builder.Value.Append(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products\");
            _ = ProductCode.ToByteArray()
                .Select(b => ((b & 0xf) << 4) + ((b & 0xf0) >> 4))
                .Aggregate<int, StringBuilder, StringBuilder>(
                    _builder.Value,
                    (builder, value) => builder.AppendFormat("{0:X2}", value),
                    builder => builder);
            _builder.Value.Append(@"\InstallProperties");
            string registryKey = _builder.Value.ToString();
            using MaybeDisposable<RegistryKey> maybeKey = new(Registry.LocalMachine.OpenSubKey(registryKey));
            if (!maybeKey.HasValue)
            {
                return false;
            }

            RegistryKey key = maybeKey.Value;
            _builder.Value.Clear();
            _builder.Value.Append(key.GetValue("DisplayVersion"));

            if (!Version.TryParse(_builder.Value.ToString(), out Version productVersion))
            {
                return false;
            }

            version = productVersion;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _builder.Value.Clear();
        }
    }

}
