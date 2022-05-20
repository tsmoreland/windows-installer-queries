namespace TSMoreland.WindowsInstaller.ProductFinder;

public sealed class MsiProperty : IEquatable<MsiProperty>
{

    private MsiProperty(string property)
    {
        Property = property;
    }

    public string Property { get; }

    public static implicit operator string(MsiProperty property)
    {
        return property.Property;
    }

    /// <inheritdoc />
    public bool Equals(MsiProperty other) =>
        ReferenceEquals(other, this) ||
        string.Equals(other.Property, Property);

    public static bool operator ==(MsiProperty left, MsiProperty right) =>
        left.Equals(right);
    public static bool operator !=(MsiProperty left, MsiProperty right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is MsiProperty other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Property.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Property;

    public static MsiProperty HelpLink = new("INSTALLPROPERTY_HELPLINK"); // Support link. For more information, see the ARPHELPLINK property.
    public static MsiProperty HelpTelephone = new("INSTALLPROPERTY_HELPTELEPHONE"); // Support telephone. For more information, see the ARPHELPTELEPHONE property.
    public static MsiProperty InstallDate = new("INSTALLPROPERTY_INSTALLDATE"); // The last time this product received service. The value of this property is replaced each time a patch is applied or removed from the product or the /v Command-Line Option is used to repair the product. If the product has received no repairs or patches this property contains the time this product was installed on this computer.
    public static MsiProperty Installedlanguage = new("INSTALLPROPERTY_INSTALLEDLANGUAGE"); // Installed language.  Windows Installer 4.5 and earlier:  Not supported.  
    public static MsiProperty InstalledProductName = new("INSTALLPROPERTY_INSTALLEDPRODUCTNAME"); // Installed product name. For more information, see the ProductName property.
    public static MsiProperty InstallLocation = new("INSTALLPROPERTY_INSTALLLOCATION"); // Installation location. For more information, see the ARPINSTALLLOCATION property.
    public static MsiProperty InstallSource = new("INSTALLPROPERTY_INSTALLSOURCE"); // Installation source. For more information, see the SourceDir property.
    public static MsiProperty LocalPackage = new("INSTALLPROPERTY_LOCALPACKAGE"); // Local cached package.
    public static MsiProperty Publisher = new("INSTALLPROPERTY_PUBLISHER"); // Publisher. For more information, see the Manufacturer property.
    public static MsiProperty UrlInfoAbout = new("INSTALLPROPERTY_URLINFOABOUT"); // URL information. For more information, see the ARPURLINFOABOUT property.
    public static MsiProperty UrlUpdateInfo = new("INSTALLPROPERTY_URLUPDATEINFO"); // URL update information. For more information, see the ARPURLUPDATEINFO property.
    public static MsiProperty VersionMinor = new("INSTALLPROPERTY_VERSIONMINOR"); // Minor product version derived from the ProductVersion property.
    public static MsiProperty VersionMajor = new("INSTALLPROPERTY_VERSIONMAJOR"); // Major product version derived from the ProductVersion property.
    public static MsiProperty VersionString = new("INSTALLPROPERTY_VERSIONSTRING"); // Product version. For more information, see the ProductVersion property.
}
