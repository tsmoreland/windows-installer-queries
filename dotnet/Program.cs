using TSMoreland.WindowsInstaller.ProductFinder;

if (args.Length < 1)
{
    Console.WriteLine("usage: productFinder {upgrade code}");
    return;
}

string upgradeCode = args[0];

int index = 0;
StringBuilder builder = new(512);

while (NativeMethods.EnumRelatedProducts(upgradeCode, index++, builder))
{
    Console.Out.WriteLine($"Found product {index - 1}");

    string productCode = builder.ToString();
    builder.Clear();
    Console.Out.WriteLine(productCode);

    int length = builder.Capacity;
    if (NativeMethods.GetProductInfo(productCode, MsiProperty.VersionString, builder, ref length))
    {
        Console.Out.WriteLine(builder.ToString());
    }
    builder.Clear();
}
