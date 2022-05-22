using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TSMoreland.WindowsInstaller.ProductFinder;

/// <summary>
/// Facade methods around Native Win32 methods
/// </summary>
internal static class NativeMethods
{
    [DllImport("msi.dll", CharSet=CharSet.Unicode)]
    public static extern int MsiGetProductInfo(string product, string property, [Out] StringBuilder valueBuffer, ref int len);


    [DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError=true)]
    public static extern int MsiEnumRelatedProducts(string upgradeCode, int reserved, int index, StringBuilder productCodeBuilder);
}
