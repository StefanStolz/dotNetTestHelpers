namespace StefanStolz.TestHelpers
{
    public sealed class TransientDirectoryManager : IDisposable
    {
        private readonly RandomNameGenerator randomNameGenerator = new();

        public TransientDirectoryManager()
            : this(Path.GetTempPath())
        {
        }

        public TransientDirectoryManager(string basePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            this.WorkingPath = Path.Combine(basePath, "tdm" + DateTime.Now.Ticks.ToString("X"));
            Directory.CreateDirectory(this.WorkingPath);
        }

        /// <summary>
        ///     Gets the Path where the Files are stored
        /// </summary>
        public string WorkingPath { get; }

        public void Dispose()
        {
            this.Cleanup();
        }

        public string CreateTransientPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value must not be empty", nameof(fileName));
            }

            return Path.Combine(this.WorkingPath, fileName);
        }

        public string CreateTransientFile(string extension, bool createFile = false)
        {
            string fileName = this.randomNameGenerator.GetRandomName(16) + "." + extension;

            var path = this.CreateTransientPath(fileName);

            if (createFile)
            {
                File.WriteAllText(path, "");
            }

            return path;
        }

        private void Cleanup()
        {
            Retry(() => { Directory.Delete(this.WorkingPath, true); }, 5);
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
                            Thread.Sleep(500 + i + 1);
                            break;
                        default:
                            throw;
                    }
                }
            }

            action();
        }
    }
}