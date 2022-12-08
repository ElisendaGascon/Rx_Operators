using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Rx_Operators
{
    internal class GroupByOperator<TKey, TItem> : IObservable<IGroupedObservable<TKey, TItem>>
    {
        private readonly IObservable<TItem> source;
        private readonly Func<TItem, TKey> keySelector;

        public GroupByOperator(IObservable<TItem> source, Func<TItem, TKey> keySelector)
        {
            this.source = source;
            this.keySelector = keySelector;
        }

        public IDisposable Subscribe(IObserver<IGroupedObservable<TKey, TItem>> observer)
        {
            return new Implementation(observer, this.source, this.keySelector);
        }

        private class Implementation : IObserver<TItem>, IDisposable
        {
            private IObserver<IGroupedObservable<TKey, TItem>> observer;
            private IObservable<TItem> source;
            private Func<TItem, TKey> keySelector;
            private IDisposable sub;
            private Dictionary<TKey, Group> groups = new();

            public Implementation(IObserver<IGroupedObservable<TKey, TItem>> observer, IObservable<TItem> source, Func<TItem, TKey> keySelector)
            {
                this.observer = observer;
                this.source = source;
                this.keySelector = keySelector;
                this.sub = source.Subscribe(this);
            }

            public class Group : IGroupedObservable<TKey, TItem>
            {
                private readonly Subject<TItem> subject = new();
                public Group(TKey key)
                {
                    this.Key = key;
                }

                public void OnNext(TItem value)
                {
                    this.subject.OnNext(value);
                }

                public TKey Key { get; }

                public IDisposable Subscribe(IObserver<TItem> observer)
                {
                    return this.subject.Subscribe(observer);
                }

                internal void OnCompleted()
                {
                    this.subject.OnCompleted();
                }
            }

            public void Dispose()
            {
                this.sub.Dispose();
            }

            public void OnCompleted()
            {
                foreach( (_, Group group) in this.groups)
                {
                    group.OnCompleted();
                }
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(TItem value)
            {
                
                TKey groupKey = keySelector(value);
                if (!groups.TryGetValue(groupKey, out Group? group))
                {
                    group = new Group(groupKey);
                    groups.Add(groupKey, group);
                    this.observer.OnNext(group);
                }
                group.OnNext(value);
            }
        }
    }
}
