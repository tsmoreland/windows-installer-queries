using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TSMoreland.WindowsInstaller.ProductFinder;

/// <summary>
/// Facade methods around Native Win32 methods
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// Gets <paramref name="property"/> for product given by  <paramref name="product"/>
    /// </summary>
    /// <param name="product">product to retrieve from</param>
    /// <param name="property">property to retrieve</param>
    /// <param name="value">on success stores the retrieved property</param>
    /// <param name="logError">optional action used to log errors or warnings</param>
    /// <returns>
    /// <see langword="true"/> on succes; otherwise, <see langword="false"/>
    /// </returns>
    public static bool GetProductInfo(string product, MsiProperty property, out string? value, Action<string>? logError = null)
    {
        StringBuilder builder = new (512);
        if (GetProductInfo(product, property, builder, logError))
        {
            value = builder.ToString();
            return true;
        }

        value = null;
        return false;
    }


    /// <summary>
    /// Gets <paramref name="property"/> for product given by  <paramref name="product"/>
    /// </summary>
    /// <param name="product">product to retrieve from</param>
    /// <param name="property">property to retrieve</param>
    /// <param name="buffer">buffer used to store the value on success</param>
    /// <param name="logError">optional action used to log errors or warnings</param>
    /// <returns>
    /// <see langword="true"/> on succes; otherwise, <see langword="false"/>
    /// </returns>
    public static bool GetProductInfo(string product, MsiProperty property, StringBuilder buffer, Action<string>? logError = null)
    {
        int length = buffer.Capacity;
        int result = MsiGetProductInfo(product, property, buffer, ref length);

        if (result == Success)
        {
            return true;
        }

        if (result != PropertyNotFound && result != ProductNotFound)
        {
            logError?.Invoke($"Error: {result} ({result:X})");
            return false;
        }

        try
        {
            Guid productCode = new(product);
            buffer.Clear();

            buffer.Append(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products\");
            _ = productCode.ToByteArray()
                .Select(b => ((b & 0xf) << 4) + ((b & 0xf0) >> 4))
                .Aggregate<int, StringBuilder, StringBuilder>(
                    buffer,
                    (builder, value) => builder.AppendFormat("{0:X2}", value),
                    builder => builder);
            buffer.Append(@"\InstallProperties");
            string registryKey = buffer.ToString();
            using MaybeDisposable<RegistryKey> maybeKey = new(Registry.LocalMachine.OpenSubKey(registryKey));
            if (maybeKey.Value is null)
            {
                logError?.Invoke($"resgistry key {registryKey} not found");
                return false;
            }
            RegistryKey key = maybeKey.Value;

            buffer.Clear();

            if (property == MsiProperty.VersionString)
            {
                buffer.Append(key.GetValue("DisplayVersion"));
                return true;
            }

            logError?.Invoke($"Unable to find: {property}");
        }
        catch (Exception ex)
        {
            logError?.Invoke(ex.ToString());
        }


        return false;
    }

    /// <summary>
    /// retrieves all related product codes for upgrade code given by  <paramref name="upgradeCode"/>
    /// </summary>
    /// <param name="upgradeCode">upgrade search to find related products for</param>
    /// <param name="logError">optional action used to log errors or warnings</param>
    /// <returns><see cref="IEnumerable{String}"/> containing the related product codes</returns>
    public static IEnumerable<string> GetRelatedProducts(string upgradeCode, Action<string>? logError = null)
    {
        int index = 0;
        StringBuilder productCodeBuilder = new(39); // capacity for a GUID

        int result;
        while ((result = MsiEnumRelatedProducts(upgradeCode, 0, index++, productCodeBuilder)) == Success)
        {
            yield return productCodeBuilder.ToString();
            productCodeBuilder.Clear();
        }

        if (result != NoMoreData)
        {
            logError?.Invoke($"Ended early with: {result} ({result:X})");
        }
    }

    private const int Success = 0;
    private const int NoMoreData = 259;
    private const int PropertyNotFound = 1608;
    private const int ProductNotFound = 1605;

    [DllImport("msi.dll", CharSet=CharSet.Unicode)]
    private static extern int MsiGetProductInfo(string product, string property, [Out] StringBuilder valueBuffer, ref int len);


    [DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError=true)]
    private static extern int MsiEnumRelatedProducts(string upgradeCode, int reserved, int index, StringBuilder productCodeBuilder);
}
