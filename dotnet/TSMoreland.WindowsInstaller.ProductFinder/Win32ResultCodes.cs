using System.Runtime.CompilerServices;

namespace TSMoreland.WindowsInstaller.ProductFinder;

internal enum Win32ResultCodes : int
{
    Success = 0,
    NoMoreData = 259,
    PropertyNotFound = 1608,
    ProductNotFound = 1605,
}

internal static class Win32ResultCodesExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Value(this Win32ResultCodes @enum)
    {
        return (int)@enum;
    }
}
