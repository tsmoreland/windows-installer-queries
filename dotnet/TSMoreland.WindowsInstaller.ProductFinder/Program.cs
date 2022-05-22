using TSMoreland.WindowsInstaller.ProductFinder;

if (args.Length < 1)
{
    Console.WriteLine("usage: productFinder {upgrade code}");
    return;
}

string rawUpgradeCode = args[0];
if (!Guid.TryParse(rawUpgradeCode, out Guid upgradeCode))
{
    Console.WriteLine("invalid uuid");
    return;
}


foreach (MsiProduct product in MsiProduct.GetProductsFromUpgradeCode(upgradeCode))
{
    if (product.TryGetVersion(out Version? version))
    {
        Console.WriteLine($"{product.ProductCode}: {version}");
    }

}

