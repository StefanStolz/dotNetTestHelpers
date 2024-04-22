using System.Reflection;

namespace StefanStolz.TestHelpers.Tests;

[TestFixture]
public class TransientFileManagerTests
{
    [Test]
    public void TempFileFromEmbeddedResource()
    {
        var embeddedResourceFileSource =
            new EmbeddedResourceFileSource(
                Assembly.GetExecutingAssembly(),
                "SomeTextFile.txt",
                "StefanStolz.TestHelpers.Tests.Assets");

        string path;
        using (var sut = new TransientFileManager(embeddedResourceFileSource))
        {
            path = sut.CreateTempVersionOfFile();

            Assert.That(File.Exists(path), Is.True);
        }

        Assert.That(File.Exists(path), Is.False);
    }
}