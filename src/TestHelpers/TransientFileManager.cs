﻿namespace StefanStolz.TestHelpers;

public sealed class TransientFileManager : IDisposable
{
    private readonly List<TransientDirectoryManager> tempDirectories = new();
    private readonly ITransientFileManagerSource source;

    public TransientFileManager(ITransientFileManagerSource source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public TransientFileManager(string sourceFile)
    {
        if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
        if (!File.Exists(sourceFile)) throw new FileNotFoundException("SourceFile not found", sourceFile);
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
    public string CreateTempVersionOfFile()
    {
        var tempDirectoryManager = new TransientDirectoryManager(Path.GetTempPath());

        var fileName = tempDirectoryManager.CreateTransientPath(this.source.FileName);
        using (var inputStream = this.source.GetDataStream())
        using (var outputStream = File.OpenWrite(fileName))
        {
            inputStream.CopyTo(outputStream);
        }

        this.tempDirectories.Add(tempDirectoryManager);

        return fileName;
    }

    private void Cleanup()
    {
        foreach (var tempDirectory in this.tempDirectories)
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

        this.tempDirectories.Clear();
    }
}