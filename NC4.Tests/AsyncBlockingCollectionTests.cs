using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NC4.Tests
{
    public class AsyncBlockingCollectionTests
    {
        [Fact]
        public async Task TestTakeListAsync()
        {
            var bag = new ConcurrentBag<int>();
            using (var collection = new AsyncBlockingCollection<int>())
            {
                var addTask = Task.Run(async () =>
                {
                    Random r = new Random();
                    foreach (var item in Enumerable.Range(1, 20))
                    {
                        await Task.Delay(r.Next(100));
                        collection.Add(item);
                    }
                    collection.CompleteAdding();
                });

                var takeTask1 = collection.TakeListAsync(2, results =>
                {
                    foreach (var item in results)
                    {
                        bag.Add(item);
                    }
                    return Task.CompletedTask;
                });

                var takeTask2 = collection.TakeListAsync(3, results =>
                {
                    foreach (var item in results)
                    {
                        bag.Add(item);
                    }
                    return Task.CompletedTask;
                });

                await Task.WhenAll(addTask, takeTask1, takeTask2);

                Assert.True(Enumerable.Range(1, 20).SequenceEqual(bag.OrderBy(o => o)));
            }
        }
    }
}
