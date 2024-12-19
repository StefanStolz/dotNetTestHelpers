namespace StefanStolz.TestHelpers;

public class FileSource : ITransientFileManagerSource
{
    private readonly string filePath;

    public FileSource(string filePath)
    {
        this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public Stream GetDataStream()
    {
        return File.OpenRead(this.filePath);
    }

    public string FileName => Path.GetFileName(this.filePath);
}