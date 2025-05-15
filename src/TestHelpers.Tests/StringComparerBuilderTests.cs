using FluentAssertions;

namespace StefanStolz.TestHelpers.Tests;

[TestFixture]
[TestOf(typeof(StringComparerBuilder))]
public class StringComparerBuilderTests
{

    [Test]
    public void BuildComparerIgnoringLineEndings()
    {
        var input1 = "one\ntwo\nthree";
        var input2 = "one\r\ntwo\r\nthree";

        var sut = new StringComparerBuilder().IgnoreLineEndings().Build();

        var result = sut.Equals(input1, input2);

        result.Should().BeTrue();
    }

    [Test]
    public void TrimLines()
    {
        var input1 = "  one\ntwo  ";
        var input2 = "one\ntwo";

        var sut = new StringComparerBuilder().TrimLines().Build();

        var result = sut.Equals(input1, input2);

        result.Should().BeTrue();
    }


    [Test]
    public void TrimLinesWithDifferentLineEndings()
    {
        var input1 = "  one\ntwo  ";
        var input2 = "one\r\ntwo";

        var sut = new StringComparerBuilder().TrimLines().Build();

        var result = sut.Equals(input1, input2);

        result.Should().BeFalse();
    }
}