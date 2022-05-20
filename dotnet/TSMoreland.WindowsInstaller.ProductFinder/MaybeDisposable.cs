namespace TSMoreland.WindowsInstaller.ProductFinder;

internal sealed class MaybeDisposable<T> : IDisposable
    where T : IDisposable
{
    private readonly T? _value;

    public MaybeDisposable(T? value)
    {
        _value = value;
    }

    /// <summary>
    /// Returns value
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// if <see cref="HasValue"/> is <see langword="false"/>
    /// </exception>
    public T Value => _value is not null
        ? _value
        : throw new InvalidOperationException("Cannot access value when not present.");

    public bool HasValue => _value is not null;

    public static implicit operator T?(MaybeDisposable<T> source) =>
        source._value;

    /// <inheritdoc/>
    public void Dispose()
    {
        _value?.Dispose();
    }
}
