using System;
using System.Linq;
using System.Threading.Tasks;

namespace NC4.Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Random r = new Random();
            using (var collection = new AsyncBlockingCollection<int>())
            {
                var addTask = Task.Run(async () =>
                {
                    foreach (var item in Enumerable.Range(1, 20))
                    {
                        await Task.Delay(r.Next(2000));
                        Console.WriteLine("Add:" + item);
                        collection.Add(item);
                    }
                    collection.CompleteAdding();
                });

                var takeTask1 = collection.TakeListAsync(2, results =>
                {
                    Console.WriteLine("Take1:" + string.Join(",", results.Select(r => r.ToString())));
                    return Task.CompletedTask;
                });

                var takeTask2 = collection.TakeListAsync(3, results =>
                {
                    Console.WriteLine("Take2:" + string.Join(",", results.Select(r => r.ToString())));
                    return Task.CompletedTask;
                });

                await Task.WhenAll(addTask, takeTask1, takeTask2);
            }
        }
    }
}
