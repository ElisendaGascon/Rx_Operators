using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx_Operators
{
    internal class WhereOperator<T> : IObservable<T>
    {
        private readonly IObservable<T> source;
        private readonly Func<T, bool> filter;

        public WhereOperator(IObservable<T> source, Func<T, bool> filter)
        {
            this.source = source;
            this.filter = filter;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var op = new Implementation(observer, this.source, this.filter);
            return op;
        }

        private class Implementation : IObserver<T>, IDisposable
        {
            private IObserver<T> observer;
            private readonly Func<T, bool> filter;
            private IDisposable sub;

            public Implementation(IObserver<T> observer, IObservable<T> source, Func<T, bool> filter)
            {
                this.observer = observer;
                this.filter = filter;
                this.sub = source.Subscribe(this);
            }

            public void Dispose()
            {
                this.sub.Dispose();
            }

            public void OnCompleted()
            {
                observer.OnCompleted();
            }

            public void OnError(Exception error)
            {
                observer.OnError(error);
            }

            public void OnNext(T value)
            {
                if( this.filter(value))
                {
                    observer.OnNext(value);
                }
            }
        }
    }
}
