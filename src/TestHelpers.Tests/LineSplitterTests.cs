using FluentAssertions;

namespace StefanStolz.TestHelpers.Tests;

[TestFixture]
[TestOf(typeof(LineSplitter))]
public class LineSplitterTests
{
    [Test]
    public void SingleLineWithText()
    {
        var sut = new LineSplitter(LineEnding.Unix);
        var result = sut.Execute("Hello World");

        result.Count.Should().Be(1);
        result[0].Value.Should().Be("Hello World");
        result[0].Kind.Should().Be(LineSplitterKind.Text);
    }

    [Test]
    public void TwoLinesDelimitedByNewLine()
    {
        var sut = new LineSplitter(LineEnding.Unix);
        var result = sut.Execute("Hello\nWorld");
        result.Count.Should().Be(3);
        result[0].Value.Should().Be("Hello");
        result[0].Kind.Should().Be(LineSplitterKind.Text);
        result[1].Value.Should().Be("\n");
        result[1].Kind.Should().Be(LineSplitterKind.LineEnding);
        result[2].Value.Should().Be("World");
        result[2].Kind.Should().Be(LineSplitterKind.Text);
    }

    [Test]
    public void DetectMultipleLineEndings()
    {
        var sut = new LineSplitter(LineEnding.Windows);
        var result  = sut.Execute($"a\r\n\r\nb");

        result.Count.Should().Be(4);
    }
}