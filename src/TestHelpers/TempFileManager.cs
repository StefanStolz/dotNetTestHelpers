﻿namespace TestHelpers;

public sealed class TempFileManager : IDisposable
{
    private readonly List<TempDirectoryManager> directories = new();
    private readonly ITempFileManagerSource source;

    public TempFileManager(ITempFileManagerSource source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public TempFileManager(string sourceFile)
    {
        if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
        if (!File.Exists(sourceFile)) throw new FileNotFoundException("Sourcefile not found", sourceFile);
        this.source = new FileSource(sourceFile);
    }

    public void Dispose()
    {
        this.Cleanup();
    }

    /// <summary>
    /// Creates a duplicate File in a temporary Directory and returns the Path to the File.
    /// </summary>
    /// <returns>The Path to the file</returns>
    public string CreateDuplicate()
    {
        var td = new TempDirectoryManager(Path.GetTempPath());

        var fileName = td.GetPath(this.source.FileName);
        using (var inputStream = this.source.GetDataStream())
        using (var outputStream = File.OpenWrite(fileName))
        {
            inputStream.CopyTo(outputStream);
        }

        this.directories.Add(td);
        return fileName;
    }

    public void Cleanup()
    {
        foreach (var tempDirectory in this.directories)
        {
            try
            {
                tempDirectory.Dispose();
            }
            catch (Exception)
            {
                // general catch is intented
            }
        }

        this.directories.Clear();
    }
}