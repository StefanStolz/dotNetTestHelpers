using StefanStolz.TestHelpers;

namespace TestHelpers.UseCases
{
    [TestFixture]
    public class IntegrateTransientFileManagerWithThisAssembly
    {
        [Test]
        public void AccessEmbeddedResourceFile()
        {
            using TransientFileManager sut = new TransientFileManager(new StreamSource(
                Path.GetFileName(ThisAssembly.Constants.Assets.SomeEmbeddedTextFile),
                ThisAssembly.Resources.Assets.SomeEmbeddedTextFile.GetStream));

            string tempFileName = sut.CreateTempVersionOfFile();

            Assert.That(File.Exists(tempFileName));
            string content = File.ReadAllText(tempFileName);
            Assert.That(content, Is.EqualTo("lorem ipsum"));
        }

        [Test]
        public void AccessContentFile()
        {
            FileSource fileSource = new FileSource(ThisAssembly.Constants.Assets.SomeContentTextFile);

            using TransientFileManager sut = new TransientFileManager(fileSource);

            string tempFileName = sut.CreateTempVersionOfFile();

            Assert.That(File.Exists(tempFileName));
            string content = File.ReadAllText(tempFileName);
            Assert.That(content, Is.EqualTo("lorem ipsum"));
        }
    }
}