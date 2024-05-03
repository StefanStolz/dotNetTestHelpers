using StefanStolz.TestHelpers.Threading;

namespace StefanStolz.TestHelpers.Tests.Threading;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class FakeUiTests
{
    [Test]
    public void ScheduleInFakeUi()
    {
        using var sut = new FakeUi();

        int i = 0;

        sut.SynchronizationContext.Send(_ =>
        {
            i++;
        }, null);

        Assert.That(i, Is.EqualTo(1));
    }

    [Test]
    public void ExceptionsInFakeUiAreCaught()
    {
        using var sut = new FakeUi();

        sut.SynchronizationContext.Send(_ => throw new InvalidOperationException("Some Exception"), null);

        var exceptionHistory = sut.TakeOverExceptions();

        Assert.That(exceptionHistory.Count, Is.EqualTo(1));
        var exception =Assert.Throws<InvalidOperationException>(() => exceptionHistory.Throw());

        Assert.That(exception.Message, Is.EqualTo("Some Exception"));
    }
}