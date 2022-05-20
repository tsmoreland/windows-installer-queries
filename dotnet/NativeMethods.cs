using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TSMoreland.Samples.ProductFinder;

internal static class NativeMethods
{

    public static bool GetProductInfo(string product, MsiProperty property, StringBuilder buffer, ref int length)
    {
        const int success = 0;
        const int propertyNotFound = 1608;
        const int productNotFound = 1605;

        int result = MsiGetProductInfo(product, property, buffer, ref length);
        if (result == success)
        {
            return true;
        }

        if (result != propertyNotFound && result != productNotFound)
        {
            Console.WriteLine($"Error: {result} ({result:X})");
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
                Console.WriteLine($"resgistry key {registryKey} not found");
                return false;
            }
            RegistryKey key = maybeKey.Value;

            buffer.Clear();

            if (property == MsiProperty.VersionString)
            {
                buffer.Append(key.GetValue("DisplayVersion"));
                return true;
            }
            else
            {
                Console.WriteLine($"Unable to find: {property}");
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }


        return false;
    }

    public static bool EnumRelatedProducts(string upgradeCode, int index, StringBuilder productCodeBuilder)
    {
        const uint success = 0u;
        const uint noMoreData = 259u;

        uint result = MsiEnumRelatedProducts(upgradeCode, 0, index, productCodeBuilder);
        switch (result)
        {
            case success:
                return true;
            case noMoreData:
                return false;
            default:
                Console.WriteLine($"Error: {result} ({result:X})");
                return false;
        }
    }


    [DllImport("msi.dll", CharSet=CharSet.Unicode)]
    private static extern int MsiGetProductInfo(string product, string property, [Out] StringBuilder valueBuffer, ref int len);


    [DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError=true)]
    private static extern uint MsiEnumRelatedProducts(string upgradeCode, int reserved, int index, StringBuilder productCodeBuilder);
}
