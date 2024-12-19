namespace StefanStolz.TestHelpers;

public class StreamSource : ITransientFileManagerSource
{
    private readonly Func<Stream> openMethod;

    public StreamSource(string name, Func<Stream> openMethod)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        this.openMethod = openMethod ?? throw new ArgumentNullException(nameof(openMethod));
        this.FileName = name;
    }

    public Stream GetDataStream()
    {
        return this.openMethod();
    }

    public string FileName { get; }
}