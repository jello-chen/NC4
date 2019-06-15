using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace NC4
{
    public class AsyncBlockingCollection<T> : BlockingCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the System.Collections.Concurrent.BlockingCollection`1
        //  class without an upper-bound.
        /// </summary>
        public AsyncBlockingCollection() { }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Concurrent.BlockingCollection`1
        /// class without an upper-bound and using the provided System.Collections.Concurrent.IProducerConsumerCollection`1
        /// as its underlying data store.
        /// </summary>
        /// <param name="collection"></param>
        public AsyncBlockingCollection(IProducerConsumerCollection<T> collection) : base(collection) { }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Concurrent.BlockingCollection`1
        /// class with the specified upper-bound.
        /// </summary>
        /// <param name="boundedCapacity">The bounded size of the collection.</param>
        public AsyncBlockingCollection(int boundedCapacity) : base(boundedCapacity) { }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Concurrent.BlockingCollection`1
        //  class with the specified upper-bound and using the provided System.Collections.Concurrent.IProducerConsumerCollection`1
        //  as its underlying data store.
        /// </summary>
        /// <param name="collection">The collection to use as the underlying data store.</param>
        /// <param name="boundedCapacity">The bounded size of the collection.</param>
        public AsyncBlockingCollection(IProducerConsumerCollection<T> collection, int boundedCapacity) : base(collection, boundedCapacity) { }

        public async Task TakeListAsync(int limit, Func<List<T>, Task> func)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            int _limit = limit;
            await Task.Run(async () =>
            {
                while (!IsCompleted)
                {
                    var list = new List<T>(limit);
                    while (limit > 0)
                    {
                        bool success = TryTake(out T item, Timeout.Infinite);
                        if (success)
                        {
                            list.Add(item);
                            --limit;
                        }
                        else break;
                    }
                    if (list.Count > 0)
                        await func(list);
                    limit = _limit;
                    await Task.Delay(1000);
                }
            });
        }
    }
}
