using FluentAssertions;

namespace StefanStolz.TestHelpers.Tests
{
    [TestFixture]
    [TestOf(typeof(DetectLineEnding))]
    public class DetectLineEndingTests
    {

        [Test]
        public void DetectUnknownIfNoEndings()
        {
           var sut = new DetectLineEnding();

           var result = sut.DetectFromString("abcd");

           result.Should().Be(LineEndingKind.Unknown);
        }

        [Test]
        public void DetectWindows()
        {
            var sut = new DetectLineEnding();

            var result = sut.DetectFromString("a\r\nb");

            result.Should().Be(LineEndingKind.Windows);
        }

        [Test]
        public void DetectUnix()
        {
            var sut = new DetectLineEnding();

            var result = sut.DetectFromString("a\nb");

            result.Should().Be(LineEndingKind.Unix);
        }

        [Test]
        public void DetectMac()
        {
            var sut = new DetectLineEnding();

            var result = sut.DetectFromString("a\rb");

            result.Should().Be(LineEndingKind.Mac);
        }
    }
}