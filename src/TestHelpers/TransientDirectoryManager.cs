namespace StefanStolz.TestHelpers;

public sealed class TransientDirectoryManager : IDisposable
{
    public TransientDirectoryManager()
        : this(Path.GetTempPath())
    {
    }

    public TransientDirectoryManager(string basePath)
    {
        if (basePath == null)
            throw new ArgumentNullException(nameof(basePath));

        this.WorkingPath = Path.Combine(basePath, "tmp" + DateTime.Now.Ticks.ToString("X"));
        Directory.CreateDirectory(this.WorkingPath);
    }

    public string CreateTransientPath(string fileName)
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
        Retry(() => { Directory.Delete(this.WorkingPath, true); }, numberOfTries: 5);
    }

    private static void Retry(Action action, int numberOfTries)
    {
        for (int i = 0; i < numberOfTries - 1; i++)
        {
            try
            {
                action();
                return;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case IOException:
                    case UnauthorizedAccessException:
                        Thread.Sleep(500 + (i + 1));
                        break;
                }
            }
        }

        action();
    }

    /// <summary>
    /// Gets the Path where the Files are stored
    /// </summary>
    public string WorkingPath { get; }
}