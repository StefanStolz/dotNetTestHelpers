using StefanStolz.TestHelpers;

namespace TestHelpers.UseCases;

[TestFixture]
public class IntegrateTransientFileManagerWithThisAssembly
{
    [Test]
    public void AccessEmbeddedResourceFile()
    {
        using var sut = new TransientFileManager(new StreamSource(
            Path.GetFileName(ThisAssembly.Constants.Assets.SomeEmbeddedTextFile),
            ThisAssembly.Resources.Assets.SomeEmbeddedTextFile.GetStream));

        var tempFileName = sut.CreateTempVersionOfFile();

        Assert.That(File.Exists(tempFileName));
        var content = File.ReadAllText(tempFileName);
        Assert.That(content, Is.EqualTo("lorem ipsum"));
    }

    [Test]
    public void AccessContentFile()
    {
        var fileSource = new FileSource(ThisAssembly.Constants.Assets.SomeContentTextFile);

        using var sut = new TransientFileManager(fileSource);

        var tempFileName = sut.CreateTempVersionOfFile();

        Assert.That(File.Exists(tempFileName));
        var content = File.ReadAllText(tempFileName);
        Assert.That(content, Is.EqualTo("lorem ipsum"));
    }
}