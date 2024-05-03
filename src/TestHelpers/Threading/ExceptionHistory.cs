// Copyright © Stefan Stolz, 2024

using System.Collections;
using System.Runtime.ExceptionServices;

namespace StefanStolz.TestHelpers.Threading;

public class ExceptionHistory : IReadOnlyList<Exception>
{
    private readonly List<Exception> exceptions = new();

    internal ExceptionHistory(IEnumerable<Exception> exceptions)
    {
        this.exceptions.AddRange(exceptions);
    }

    public IEnumerator<Exception> GetEnumerator()
    {
        return this.exceptions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count => this.exceptions.Count;
    public Exception this[int index] {
        get {
            return this.exceptions[index];
        }
    }

    public void Throw()
    {
        if (!this.IsEmpty) {
            if (this.Count == 1) {
                ExceptionDispatchInfo.Capture(this.exceptions.Single()).Throw();
            } else {
                throw new AggregateException(this.exceptions);
            }
        }
    }

    public bool IsEmpty => this.Count == 0;
}
