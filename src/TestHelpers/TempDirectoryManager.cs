namespace TestHelpers;

public sealed class TempDirectoryManager : IDisposable
{
    public TempDirectoryManager()
        : this(Path.GetTempPath())
    { }

    public TempDirectoryManager(string basePath)
    {
        this.WorkingPath = Path.Combine(basePath, "tmp" + DateTime.Now.Ticks.ToString("X"));
        Directory.CreateDirectory(this.WorkingPath);
    }

    public string GetPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value must not be empty", nameof(fileName));
        return Path.Combine(this.WorkingPath, fileName);
    }

    public void Dispose()
    {
        this.Cleanup();
    }

    private void Cleanup()
    {
        Directory.Delete(this.WorkingPath, true);
    }

    /// <summary>
    /// Gets the Path where the Files are stored
    /// </summary>
    public string WorkingPath { get; }
}