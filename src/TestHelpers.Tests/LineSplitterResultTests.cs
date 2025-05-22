using FluentAssertions;

namespace StefanStolz.TestHelpers.Tests;

[TestFixture]
[TestOf(typeof(LineSplitterResult))]
public class LineSplitterResultTests
{
    [Test]
    public void ToStringConcatenatesValues()
    {
        var sut = new LineSplitterResult([
            new LineSplitterItem("a", LineSplitterKind.Text),
            new LineSplitterItem("\n", LineSplitterKind.LineEnding),
            new LineSplitterItem("b", LineSplitterKind.LineEnding)
        ]);

        var result = sut.ToString();

        result.Should().Be("a\nb");
    }
}