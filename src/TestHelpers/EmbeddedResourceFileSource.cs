using System.Reflection;

namespace StefanStolz.TestHelpers;

public class EmbeddedResourceFileSource : ITempFileManagerSource
{
    private readonly Assembly assembly;
    private readonly string name;
    private readonly string fileNamespace;

    public EmbeddedResourceFileSource(Assembly assembly, string name, string fileNamespace)
    {
        this.assembly = assembly;
        this.name = name;
        this.fileNamespace = fileNamespace;
        this.FileName = name;
    }

    public Stream GetDataStream()
    {
        var fullName = $"{this.fileNamespace}.{this.name}";

        var stream = this.assembly.GetManifestResourceStream(fullName);
        if (stream is null)
        {
            throw new InvalidOperationException($"EmbeddedResource not found: {fullName}");
        }

        return stream;
    }

    public string FileName { get; }
}