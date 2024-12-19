namespace StefanStolz.TestHelpers.Threading;

/// <summary>
///     Creates and installs a <see cref="SynchronizationContext" /> that simulates an UserInterface
/// </summary>
public sealed class FakeUi : IDisposable
{
    private readonly List<Exception> exceptions = new();

    private readonly SynchronizationContext? originalSynchronizationContext;
    private readonly FakeUiSynchronisationContext synchronisationContext;
    private readonly FakeUiSynchronizationContextHandling synchronizationContextHandling;
    private readonly object syncRoot = new();
    private readonly ThreadWorker threadWorker;
    private bool disposed;

    /// <summary>
    ///     Creates a new instance of the <see cref="FakeUi" /> class.
    /// </summary>
    public FakeUi()
        : this(FakeUiSynchronizationContextHandling.SetAndRestoreSynchronizationContext)
    {
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="FakeUi" /> class.
    /// </summary>
    /// <param name="synchronizationContextHandling">
    ///     Sets a value how to handle the <see cref="SynchronizationContext" />
    /// </param>
    public FakeUi(FakeUiSynchronizationContextHandling synchronizationContextHandling)
    {
        this.synchronizationContextHandling = synchronizationContextHandling;
        this.threadWorker = new ThreadWorker(this.OnThreadException);
        this.synchronisationContext = new FakeUiSynchronisationContext(this.threadWorker);

        this.originalSynchronizationContext = SynchronizationContext.Current;
        if (synchronizationContextHandling == FakeUiSynchronizationContextHandling.SetSynchronizationContext ||
            synchronizationContextHandling ==
            FakeUiSynchronizationContextHandling.SetAndRestoreSynchronizationContext)
        {
            SynchronizationContext.SetSynchronizationContext(this.synchronisationContext);
        }
    }

    public SynchronizationContext SynchronizationContext => this.synchronisationContext;

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

            this.disposed = true;
        }
    }

    /// <summary>
    ///     Returns an instance of <see cref="ExceptionHistory" /> with all Exceptions that happend since the last TakeOver.
    ///     This call clears the Exception-Historyn of this <see cref="FakeUi" /> instance
    /// </summary>
    /// <returns>An instance of <see cref="ExceptionHistory" /></returns>
    public ExceptionHistory TakeOverExceptions()
    {
        lock (this.syncRoot)
        {
            ExceptionHistory result = new ExceptionHistory(this.exceptions);
            this.exceptions.Clear();
            return result;
        }
    }

    private void OnThreadException(Exception exception)
    {
        lock (this.syncRoot)
        {
            this.exceptions.Add(exception);
        }
    }
}

public enum FakeUiSynchronizationContextHandling
{
    SetAndRestoreSynchronizationContext,
    SetSynchronizationContext,
    DontUpdateSynchronizationContext
}