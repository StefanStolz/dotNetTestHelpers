namespace UnitTestHelpers;

public sealed class TempDirectoryManager : IDisposable
{
    private bool disposed;

    public TempDirectoryManager()
        : this(Path.GetTempPath())
    {
    }

    public TempDirectoryManager(string basePath)
    {
        this.WorkingPath = Path.Combine(basePath, "tmp" + DateTime.Now.Ticks.ToString("X"));
        Directory.CreateDirectory(this.WorkingPath);
    }

    public string GetPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
        return Path.Combine(this.WorkingPath, fileName);
    }

    public void Dispose()
    {
        this.Dispose(true);
    }

    private void Cleanup()
    {
        Directory.Delete(this.WorkingPath, true);
    }

    private void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.Cleanup();
            }

            this.disposed = true;
        }
    }

    /// <summary>
    /// Gets the Path where the Files are stored
    /// </summary>
    public string WorkingPath { get; }
}