namespace UnitTestHelpers;

public class StreamSource : ITempFileManagerSource
{
    private readonly Func<Stream> openMethod;

    public StreamSource(string name, Func<Stream> openMethod)
    {
            this.openMethod = openMethod;
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            this.FileName = name;
        }

    public Stream GetDataStream()
    {
            return this.openMethod();
        }

    public string FileName { get; }
}