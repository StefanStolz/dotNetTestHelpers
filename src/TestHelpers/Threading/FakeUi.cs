using System.Runtime.ExceptionServices;

namespace StefanStolz.TestHelpers.Threading;

public sealed class FakeUi : IDisposable
{
    private readonly FakeUiSynchronizationContextHandling synchronizationContextHandling;
    private readonly List<Exception> exceptions = new List<Exception>();
    private readonly FakeUiSynchronisationContext synchronisationContext;
    private readonly ThreadWorker threadWorker;
    private bool disposed;

    private SynchronizationContext? originalSynchronizationContext;

    public FakeUi()
        : this(FakeUiSynchronizationContextHandling.SetAndRestoreSynchronizationContext)
    {
    }

    public FakeUi(FakeUiSynchronizationContextHandling synchronizationContextHandling)
    {
        this.synchronizationContextHandling = synchronizationContextHandling;
        this.threadWorker = new ThreadWorker(this.OnThreadException);
        this.synchronisationContext = new FakeUiSynchronisationContext(this.threadWorker, true);

        this.originalSynchronizationContext = SynchronizationContext.Current;
        if (synchronizationContextHandling == FakeUiSynchronizationContextHandling.SetSynchronizationContext ||
            synchronizationContextHandling == FakeUiSynchronizationContextHandling.SetAndRestoreSynchronizationContext)
        {
            SynchronizationContext.SetSynchronizationContext(this.synchronisationContext);
        }
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            if (this.synchronizationContextHandling ==
                FakeUiSynchronizationContextHandling.SetAndRestoreSynchronizationContext)
            {
                SynchronizationContext.SetSynchronizationContext(this.originalSynchronizationContext);
            }

            this.threadWorker.Dispose();

            if (this.exceptions.Any())
            {
                if (this.exceptions.Count == 1)
                {
                    ExceptionDispatchInfo.Capture(this.exceptions.First()).Throw();
                }
                else
                {
                    throw new AggregateException(this.exceptions);
                }
            }

            this.disposed = true;
        }
    }

    private void OnThreadException(Exception exception)
    {
        this.exceptions.Add(exception);
    }

    public SynchronizationContext SynchronizationContext => this.synchronisationContext;
}

public enum FakeUiSynchronizationContextHandling
{
    SetAndRestoreSynchronizationContext,
    SetSynchronizationContext,
    DontUpdateSynchronizationContext
}