using System.Collections.Concurrent;

namespace StefanStolz.TestHelpers.Threading;

internal sealed class ThreadWorker : IDisposable
{
    private readonly BlockingCollection<Action> actions = new(new ConcurrentQueue<Action>());
    private readonly CancellationTokenSource cts = new();
    private readonly Action<Exception> exceptionHandler;
    private readonly Thread workerThread;
    private bool disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ThreadWorker" /> class
    ///     with a custom Exception-Handler
    /// </summary>
    /// <param name="exceptionHandler">The Custom Exception-Handler</param>
    public ThreadWorker(Action<Exception> exceptionHandler)
    {
        this.exceptionHandler = exceptionHandler;
        this.workerThread = new Thread(this.Execute);
        this.workerThread.IsBackground = true;
        this.workerThread.Start();
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (!this.disposed)
        {
            this.cts.Cancel();
            this.workerThread.Join(new TimeSpan(0, 1, 0));
            this.cts.Dispose();

            this.disposed = true;
        }
    }

    /// <summary>
    ///     Enqueues an Action to execute in the <see cref="ThreadWorker" />.
    /// </summary>
    /// <param name="action">The Action to execute</param>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Post(Action action)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(ThreadWorker));
        }

        this.actions.Add(action);
    }

    private void Execute()
    {
        CancellationToken token = this.cts.Token;

        try
        {
            while (this.actions.Any() || !token.IsCancellationRequested)
            {
                Action action = this.actions.Take(token);

                this.ExecuteAction(action);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore Exceptions from this.actions.Take()
        }
    }

    private void ExecuteAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            this.exceptionHandler(exception);
        }
    }

    public void Send(Action action)
    {
        using ManualResetEvent mre = new ManualResetEvent(false);
        this.actions.Add(() =>
        {
            try
            {
                this.ExecuteAction(action);
            }
            finally
            {
                mre.Set();
            }
        });

        mre.WaitOne();
    }
}