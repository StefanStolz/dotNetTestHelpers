using System.Collections.Concurrent;

namespace StefanStolz.TestHelpers.Threading;

internal sealed class ThreadWorker : IDisposable
{
    private readonly BlockingCollection<Action> actions = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
    private readonly CancellationTokenSource cts = new CancellationTokenSource();
    private readonly Action<Exception> exceptionHandler = x => { };
    private readonly Thread workerThread;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadWorker" /> class.
    /// </summary>
    public ThreadWorker()
    {
        this.workerThread = new Thread(this.Execute);
        this.workerThread.IsBackground = true;
        this.workerThread.Start();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadWorker" /> class
    /// with a custom Exception-Handler
    /// </summary>
    /// <param name="exceptionHandler">The Custom Exception-Handler</param>
    public ThreadWorker(Action<Exception> exceptionHandler)
        : this()
    {
        this.exceptionHandler = exceptionHandler;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
    /// Enqueues an Action to execute in the <see cref="ThreadWorker" />.
    /// </summary>
    /// <param name="action">The Action to execute</param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public void Post(Action action)
    {
        if (this.disposed) throw new ObjectDisposedException(nameof(ThreadWorker));
        if (action == null) throw new ArgumentNullException(nameof(action));

        this.actions.Add(action);
    }

    private void Execute()
    {
        var token = this.cts.Token;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var action = this.actions.Take(token);

                this.ExecuteAction(action);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            while (this.actions.Any())
            {
                var action = this.actions.Take();

                this.ExecuteAction(action);
            }
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
}