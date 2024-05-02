using System.Runtime.ExceptionServices;

namespace StefanStolz.TestHelpers.Threading
{
    public sealed class FakeUi : IDisposable
    {
        private readonly List<Exception> exceptions = new List<Exception>();
        private readonly FakeUiSynchronisationContext synchronisationContext;
        private readonly ThreadWorker threadWorker;
        private bool disposed;

        private FakeUi()
        {
            this.threadWorker = new ThreadWorker(this.OnThreadException);
            this.synchronisationContext = new FakeUiSynchronisationContext(this.threadWorker, true);
        }

        public void Dispose()
        {
            if (!this.disposed) {
                if (true) {
                    this.threadWorker.Dispose();
                }

                if (this.exceptions.Any()) {
                    if (this.exceptions.Count == 1) {
                        ExceptionDispatchInfo.Capture(this.exceptions.First()).Throw();
                    }
                    else {
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

        public static FakeUi Create(bool updateSynchronisationContext = true)
        {
            var result = new FakeUi();

            if (updateSynchronisationContext) {
                SynchronizationContext.SetSynchronizationContext(result.SynchronizationContext);
            }

            return result;
        }
    }
}
