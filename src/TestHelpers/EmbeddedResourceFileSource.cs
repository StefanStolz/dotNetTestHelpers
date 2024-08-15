using System.Reflection;

namespace StefanStolz.TestHelpers
{
    public class EmbeddedResourceFileSource : ITransientFileManagerSource
    {
        private readonly Assembly assembly;
        private readonly string fileNamespace;
        private readonly string name;

        public EmbeddedResourceFileSource(Assembly assembly, string name, string fileNamespace)
        {
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.fileNamespace = fileNamespace ?? throw new ArgumentNullException(nameof(fileNamespace));
            this.FileName = name;
        }

        public Stream GetDataStream()
        {
            string fullName = $"{this.fileNamespace}.{this.name}";

            Stream? stream = this.assembly.GetManifestResourceStream(fullName);
            if (stream is null)
            {
                throw new InvalidOperationException($"EmbeddedResource not found: {fullName}");
            }

            return stream;
        }

        public string FileName { get; }
    }
}