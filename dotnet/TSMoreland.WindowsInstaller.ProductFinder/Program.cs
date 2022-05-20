using TSMoreland.WindowsInstaller.ProductFinder;

if (args.Length < 1)
{
    Console.WriteLine("usage: productFinder {upgrade code}");
    return;
}

string upgradeCode = args[0];

StringBuilder builder = new(512);

foreach (string productCode in NativeMethods.GetRelatedProducts(upgradeCode, Console.WriteLine))
{
    Console.Out.WriteLine(productCode);

    int length = builder.Capacity;
    if (NativeMethods.GetProductInfo(productCode, MsiProperty.VersionString, builder, Console.WriteLine))
    {
        Console.Out.WriteLine(builder.ToString());
    }
    builder.Clear();
}

