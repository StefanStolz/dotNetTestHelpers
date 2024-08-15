namespace StefanStolz.TestHelpers.Threading
{
    internal sealed class FakeUiSynchronisationContext : SynchronizationContext
    {
        private readonly ThreadWorker threadWorker;

        public FakeUiSynchronisationContext(ThreadWorker threadWorker)
        {
            this.threadWorker = threadWorker;

            this.threadWorker.Post(() => SetSynchronizationContext(this));
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            this.threadWorker.Post(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            this.threadWorker.Send(() => d(state));
        }

        public override SynchronizationContext CreateCopy()
        {
            return new FakeUiSynchronisationContext(this.threadWorker);
        }
    }
}