using FluentAssertions;

namespace StefanStolz.TestHelpers.Tests
{
    [TestFixture]
    [TestOf(typeof(LineSplitter))]
    public class LineSplitterTests
    {
        [Test]
        public void SingleLineWithText()
        {
            var result = LineSplitter.Execute("Hello World").ToList();

            result.Count.Should().Be(1);
            result[0].Value.Should().Be("Hello World");
            result[0].Kind.Should().Be(StringTokenKind.Text);
        }

        [Test]
        public void TwoLinesDelimitedByNewLine()
        {
            var result = LineSplitter.Execute("Hello\nWorld").ToList();
            result.Count.Should().Be(3);
            result[0].Value.Should().Be("Hello");
            result[0].Kind.Should().Be(StringTokenKind.Text);
            result[1].Value.Should().Be("\n");
            result[1].Kind.Should().Be(StringTokenKind.LineEnding);
            result[2].Value.Should().Be("World");
            result[2].Kind.Should().Be(StringTokenKind.Text);
        }
    }
}