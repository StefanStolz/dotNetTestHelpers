namespace StefanStolz.TestHelpers.Threading
{
    public sealed class FakeUiSynchronisationContext : SynchronizationContext
    {
        private readonly ThreadWorker threadWorker;

        public FakeUiSynchronisationContext(ThreadWorker threadWorker, bool setAsSynchronisationContextInThread = true)
        {
            this.threadWorker = threadWorker;

            if (setAsSynchronisationContextInThread) {
                this.threadWorker.Post(() => SetSynchronizationContext(this));
            }
        }

        public FakeUiSynchronisationContext(bool setAsSynchronisationContextInThread = true)
            : this(new ThreadWorker(), setAsSynchronisationContextInThread)
        { }

        public override void Post(SendOrPostCallback d, object? state)
        {
            this.threadWorker.Post(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            using var mre = new ManualResetEvent(false);
            this.threadWorker.Post(
                () =>
                {
                    try {
                        d(state);
                    }
                    finally {
                        // ReSharper disable once AccessToDisposedClosure
                        mre.Set();
                    }
                });

            mre.WaitOne();
        }

        public override SynchronizationContext CreateCopy()
        {
            return new FakeUiSynchronisationContext(this.threadWorker);
        }
    }
}
