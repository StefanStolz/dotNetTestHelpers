namespace UnitTestHelpers;

public class FileSource : ITempFileManagerSource
{
    private readonly string filePath;

    public FileSource(string filePath)
    {
        this.filePath = filePath;
    }

    public Stream GetDataStream()
    {
        return File.OpenRead(this.filePath);
    }

    public string FileName => Path.GetFileName(this.filePath);
}