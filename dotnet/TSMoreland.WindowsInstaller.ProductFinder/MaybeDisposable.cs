namespace TSMoreland.WindowsInstaller.ProductFinder;

internal sealed class MaybeDisposable<T> : IDisposable
    where T : IDisposable
{
    public MaybeDisposable(T? value)
    {
        Value = value;
    }
    public T? Value { get; }

    public static implicit operator T?(MaybeDisposable<T> source) =>
        source.Value;

    /// <inheritdoc/>
    public void Dispose()
    {
        Value?.Dispose();
    }
}
