using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LINQPadHelpers
{
   public static class AsyncEnumerable
    {
        public static async Task LimitConcurrencyAsync(this IEnumerable<Task> tasks, int maxConcurrency = 1)
        {
            if (maxConcurrency == int.MaxValue)
            {
                await Task.WhenAll(tasks);
                return;
            }

            if (maxConcurrency == 1)
            {
                foreach (var task in tasks)
                {
                    await task;
                }
                return;
            }

            var pending = new List<Task>();

            foreach (var task in tasks)
            {
                if (pending.Count == maxConcurrency)
                {
                    var completed = await Task.WhenAny(pending);
                    pending.Remove(completed);
                }

                pending.Add(task);
            }

            await Task.WhenAll(pending);
        }

        public static async Task<T[]> LimitConcurrencyAsync<T>(this IEnumerable<Task<T>> tasks, int maxConcurrency = 1)
        {

            if (maxConcurrency == int.MaxValue)
            {
                return await Task.WhenAll(tasks);
            }

            var results = new List<T>();

            if (maxConcurrency == 1)
            {
                foreach (var task in tasks)
                {
                    results.Add(await task);
                }

                return results.ToArray();
            }

            var pending = new List<Task<T>>();

            foreach (var task in tasks)
            {
                if (pending.Count == maxConcurrency)
                {
                    var completed = await Task.WhenAny(pending);
                    pending.Remove(completed);
                    results.Add(await completed);
                }

                pending.Add(task);
            }

            results.AddRange(await Task.WhenAll(pending));

            return results.ToArray();
        }

        public static async Task WhenAll(this IEnumerable<Task> tasks)
        {
            await Task.WhenAll(tasks);
        }

        public static async Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return await Task.WhenAll(tasks);
        }


        public static async Task Then(this Task firstThis, Action thenThis)
        {
            await firstThis;
            thenThis();
        }

        public static async Task Then(this Task firstThis, Func<Task> thenThis)
        {
            await firstThis;
            var task = thenThis();
            await task;
        }

        public static async Task Then(this Task firstThis, Func<IEnumerable<Task>> thenThese)
        {
            await firstThis;
            var tasks = thenThese();
            await tasks.WhenAll();
        }

        public static async Task<T> Then<T>(this Task firstThis, Func<Task<T>> thenThis)
        {
            await firstThis;
            var task = thenThis();
            return await task;
        }

        public static async Task<T[]> Then<T>(this Task firstThis, Func<IEnumerable<Task<T>>> thenThese)
        {
            await firstThis;
            var tasks = thenThese();
            return await tasks.WhenAll();
        }

        public static async Task<T> Then<T>(this Task firstThis, Func<T> thenThis)
        {
            await firstThis;
            var retult = thenThis();
            return retult;
        }

        public static async Task Then<T>(this Task<T> firstThis, Action<T> thenThis)
        {
            var firstResult = await firstThis;
            thenThis(firstResult);
        }

        public static async Task Then<T>(this Task<T> firstThis, Func<T, Task> thenThis)
        {
            var firstResult = await firstThis;
            var task = thenThis(firstResult);
            await task;
        }

        public static async Task Then<T>(this Task<T> firstThis, Func<T, IEnumerable<Task>> thenThis)
        {
            var firstResult = await firstThis;
            var tasks = thenThis(firstResult);
            await tasks.WhenAll();
        }

        public static async Task<TThen> Then<TFirst, TThen>(this Task<TFirst> firstThis, Func<TFirst, Task<TThen>> thenThis)
        {
            var firstResult = await firstThis;
            var task = thenThis(firstResult);
            return await task;
        }

        public static async Task<TThen[]> Then<TFirst, TThen>(this Task<TFirst> firstThis, Func<TFirst, IEnumerable<Task<TThen>>> thenThis)
        {
            var firstResult = await firstThis;
            var tasks = thenThis(firstResult);
            return await tasks.WhenAll();
        }

        public static async Task<TThen> Then<TFirst, TThen>(this Task<TFirst> firstThis, Func<TFirst, TThen> thenThis)
        {
            var firstResult = await firstThis;
            var thenResult = thenThis(firstResult);
            return thenResult;
        }


        public static async Task<float?> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float?> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<double> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<double?> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double?> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<decimal> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<decimal?> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal?> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<int> MinAsync(this Task<IEnumerable<int>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<int?> MinAsync(this Task<IEnumerable<int?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<long> MinAsync(this Task<IEnumerable<long>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<long?> MinAsync(this Task<IEnumerable<long?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<float> MinAsync(this Task<IEnumerable<float>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<float?> MinAsync(this Task<IEnumerable<float?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<double> MinAsync(this Task<IEnumerable<double>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<double?> MinAsync(this Task<IEnumerable<double?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<decimal> MinAsync(this Task<IEnumerable<decimal>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<decimal?> MinAsync(this Task<IEnumerable<decimal?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<TSource> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Min());
        }

        public static async Task<int> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<int?> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int?> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<long> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<long?> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long?> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<float> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<float?> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float?> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<double> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<double?> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double?> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<decimal> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<decimal?> MinAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal?> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<TResult> MinAsync<TSource, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TResult> selector)
        {
            return await sourceTask.Then(source => source.Min(selector));
        }

        public static async Task<int> MaxAsync(this Task<IEnumerable<int>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<int?> MaxAsync(this Task<IEnumerable<int?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<long> MaxAsync(this Task<IEnumerable<long>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<long?> MaxAsync(this Task<IEnumerable<long?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<double> MaxAsync(this Task<IEnumerable<double>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<double?> MaxAsync(this Task<IEnumerable<double?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<float> MaxAsync(this Task<IEnumerable<float>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<float?> MaxAsync(this Task<IEnumerable<float?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<decimal> MaxAsync(this Task<IEnumerable<decimal>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<decimal?> MaxAsync(this Task<IEnumerable<decimal?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<TSource> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Max());
        }

        public static async Task<int> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<int?> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int?> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<long> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<long?> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long?> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<float> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<float?> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float?> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<double> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<double?> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double?> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<decimal> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<decimal?> MaxAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal?> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<TResult> MaxAsync<TSource, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TResult> selector)
        {
            return await sourceTask.Then(source => source.Max(selector));
        }

        public static async Task<double> AverageAsync(this Task<IEnumerable<int>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<double?> AverageAsync(this Task<IEnumerable<int?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<double> AverageAsync(this Task<IEnumerable<long>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<double?> AverageAsync(this Task<IEnumerable<long?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<float> AverageAsync(this Task<IEnumerable<float>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<float?> AverageAsync(this Task<IEnumerable<float?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<double> AverageAsync(this Task<IEnumerable<double>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<double?> AverageAsync(this Task<IEnumerable<double?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<decimal> AverageAsync(this Task<IEnumerable<decimal>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<decimal?> AverageAsync(this Task<IEnumerable<decimal?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Average());
        }

        public static async Task<double> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<double?> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int?> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<double> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<double?> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long?> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<float> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<float?> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float?> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<double> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<double?> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, double?> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<decimal> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<decimal?> AverageAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, decimal?> selector)
        {
            return await sourceTask.Then(source => source.Average(selector));
        }

        public static async Task<IEnumerable<TSource>> ExceptAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return await firstTask.Then(first => first.Except(second, comparer));
        }

        public static async Task<IEnumerable<TSource>> ReverseAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Reverse());
        }

        public static async Task<bool> SequenceEqualAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second)
        {
            return await firstTask.Then(first => first.SequenceEqual(second));
        }

        public static async Task<bool> SequenceEqualAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return await firstTask.Then(first => first.SequenceEqual(second, comparer));
        }

        public static async Task<IEnumerable<TSource>> AsEnumerableAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.AsEnumerable());
        }

        public static async Task<TSource[]> ToArrayAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.ToArray());
        }

        public static async Task<List<TSource>> ToListAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.ToList());
        }

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.ToDictionary(keySelector));
        }

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.ToDictionary(keySelector, comparer));
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return await sourceTask.Then(source => source.ToDictionary(keySelector, elementSelector));
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.ToDictionary(keySelector, elementSelector, comparer));
        }

        public static async Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.ToLookup(keySelector));
        }

        public static async Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.ToLookup(keySelector, comparer));
        }

        public static async Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return await sourceTask.Then(source => source.ToLookup(keySelector, elementSelector));
        }

        public static async Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.ToLookup(keySelector, elementSelector, comparer));
        }

        public static async Task<IEnumerable<TSource>> DefaultIfEmptyAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.DefaultIfEmpty());
        }

        public static async Task<IEnumerable<TSource>> DefaultIfEmptyAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, TSource defaultValue)
        {
            return await sourceTask.Then(source => source.DefaultIfEmpty(defaultValue));
        }

        public static async Task<TSource> FirstAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.First());
        }

        public static async Task<TSource> FirstAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.First(predicate));
        }

        public static async Task<TSource> FirstOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.FirstOrDefault());
        }

        public static async Task<TSource> FirstOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.FirstOrDefault(predicate));
        }

        public static async Task<TSource> LastAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Last());
        }

        public static async Task<TSource> LastAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.Last(predicate));
        }

        public static async Task<TSource> LastOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.LastOrDefault());
        }

        public static async Task<TSource> LastOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.LastOrDefault(predicate));
        }

        public static async Task<TSource> SingleAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Single());
        }

        public static async Task<TSource> SingleAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.Single(predicate));
        }

        public static async Task<TSource> SingleOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.SingleOrDefault());
        }

        public static async Task<TSource> SingleOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.SingleOrDefault(predicate));
        }

        public static async Task<TSource> ElementAtAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, int index)
        {
            return await sourceTask.Then(source => source.ElementAt(index));
        }

        public static async Task<TSource> ElementAtOrDefaultAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, int index)
        {
            return await sourceTask.Then(source => source.ElementAtOrDefault(index));
        }

        public static async Task<bool> AnyAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Any());
        }

        public static async Task<bool> AnyAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.Any(predicate));
        }

        public static async Task<bool> AllAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.All(predicate));
        }

        public static async Task<int> CountAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Count());
        }

        public static async Task<int> CountAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.Count(predicate));
        }

        public static async Task<long> LongCountAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.LongCount());
        }

        public static async Task<long> LongCountAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.LongCount(predicate));
        }

        public static async Task<bool> ContainsAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, TSource value)
        {
            return await sourceTask.Then(source => source.Contains(value));
        }

        public static async Task<bool> ContainsAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, TSource value, IEqualityComparer<TSource> comparer)
        {
            return await sourceTask.Then(source => source.Contains(value, comparer));
        }

        public static async Task<TSource> AggregateAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TSource, TSource> func)
        {
            return await sourceTask.Then(source => source.Aggregate(func));
        }

        public static async Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(this Task<IEnumerable<TSource>> sourceTask, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return await sourceTask.Then(source => source.Aggregate(seed, func));
        }

        public static async Task<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this Task<IEnumerable<TSource>> sourceTask, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return await sourceTask.Then(source => source.Aggregate(seed, func, resultSelector));
        }

        public static async Task<int> SumAsync(this Task<IEnumerable<int>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<int?> SumAsync(this Task<IEnumerable<int?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<long> SumAsync(this Task<IEnumerable<long>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<long?> SumAsync(this Task<IEnumerable<long?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<float> SumAsync(this Task<IEnumerable<float>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<float?> SumAsync(this Task<IEnumerable<float?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<double> SumAsync(this Task<IEnumerable<double>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<double?> SumAsync(this Task<IEnumerable<double?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<decimal> SumAsync(this Task<IEnumerable<decimal>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<decimal?> SumAsync(this Task<IEnumerable<decimal?>> sourceTask)
        {
            return await sourceTask.Then(source => source.Sum());
        }

        public static async Task<int> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<int?> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int?> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<long> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<long?> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, long?> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<float> SumAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, float> selector)
        {
            return await sourceTask.Then(source => source.Sum(selector));
        }

        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.Where(predicate));
        }

        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int, bool> predicate)
        {
            return await sourceTask.Then(source => source.Where(predicate));
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TResult> selector)
        {
            return await sourceTask.Then(source => source.Select(selector));
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int, TResult> selector)
        {
            return await sourceTask.Then(source => source.Select(selector));
        }

        public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, IEnumerable<TResult>> selector)
        {
            return await sourceTask.Then(source => source.SelectMany(selector));
        }

        public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            return await sourceTask.Then(source => source.SelectMany(selector));
        }

        public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TCollection, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return await sourceTask.Then(source => source.SelectMany(collectionSelector, resultSelector));
        }

        public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TCollection, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return await sourceTask.Then(source => source.SelectMany(collectionSelector, resultSelector));
        }

        public static async Task<IEnumerable<TSource>> TakeAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, int count)
        {
            return await sourceTask.Then(source => source.Take(count));
        }

        public static async Task<IEnumerable<TSource>> TakeWhileAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.TakeWhile(predicate));
        }

        public static async Task<IEnumerable<TSource>> TakeWhileAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int, bool> predicate)
        {
            return await sourceTask.Then(source => source.TakeWhile(predicate));
        }

        public static async Task<IEnumerable<TSource>> SkipAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, int count)
        {
            return await sourceTask.Then(source => source.Skip(count));
        }

        public static async Task<IEnumerable<TSource>> SkipWhileAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, bool> predicate)
        {
            return await sourceTask.Then(source => source.SkipWhile(predicate));
        }

        public static async Task<IEnumerable<TSource>> SkipWhileAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, int, bool> predicate)
        {
            return await sourceTask.Then(source => source.SkipWhile(predicate));
        }

        public static async Task<IEnumerable<TResult>> JoinAsync<TOuter, TInner, TKey, TResult>(this Task<IEnumerable<TOuter>> outerTask, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return await outerTask.Then(outer => outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector));
        }

        public static async Task<IEnumerable<TResult>> JoinAsync<TOuter, TInner, TKey, TResult>(this Task<IEnumerable<TOuter>> outerTask, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return await outerTask.Then(outer => outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer));
        }

        public static async Task<IEnumerable<TResult>> GroupJoinAsync<TOuter, TInner, TKey, TResult>(this Task<IEnumerable<TOuter>> outerTask, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            return await outerTask.Then(outer => outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector));
        }

        public static async Task<IEnumerable<TResult>> GroupJoinAsync<TOuter, TInner, TKey, TResult>(this Task<IEnumerable<TOuter>> outerTask, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return await outerTask.Then(outer => outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer));
        }

        public static async Task<IOrderedEnumerable<TSource>> OrderByAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.OrderBy(keySelector));
        }

        public static async Task<IOrderedEnumerable<TSource>> OrderByAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.OrderBy(keySelector, comparer));
        }

        public static async Task<IOrderedEnumerable<TSource>> OrderByDescendingAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.OrderByDescending(keySelector));
        }

        public static async Task<IOrderedEnumerable<TSource>> OrderByDescendingAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.OrderByDescending(keySelector, comparer));
        }

        public static async Task<IOrderedEnumerable<TSource>> ThenByAsync<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.ThenBy(keySelector));
        }

        public static async Task<IOrderedEnumerable<TSource>> ThenByAsync<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.ThenBy(keySelector, comparer));
        }

        public static async Task<IOrderedEnumerable<TSource>> ThenByDescendingAsync<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.ThenByDescending(keySelector));
        }

        public static async Task<IOrderedEnumerable<TSource>> ThenByDescendingAsync<TSource, TKey>(this Task<IOrderedEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.ThenByDescending(keySelector, comparer));
        }

        public static async Task<IEnumerable<IGrouping<TKey, TSource>>> GroupByAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector));
        }

        public static async Task<IEnumerable<IGrouping<TKey, TSource>>> GroupByAsync<TSource, TKey>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, comparer));
        }

        public static async Task<IEnumerable<IGrouping<TKey, TElement>>> GroupByAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, elementSelector));
        }

        public static async Task<IEnumerable<IGrouping<TKey, TElement>>> GroupByAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, elementSelector, comparer));
        }

        public static async Task<IEnumerable<TResult>> GroupByAsync<TSource, TKey, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, resultSelector));
        }

        public static async Task<IEnumerable<TResult>> GroupByAsync<TSource, TKey, TElement, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, elementSelector, resultSelector));
        }

        public static async Task<IEnumerable<TResult>> GroupByAsync<TSource, TKey, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, resultSelector, comparer));
        }

        public static async Task<IEnumerable<TResult>> GroupByAsync<TSource, TKey, TElement, TResult>(this Task<IEnumerable<TSource>> sourceTask, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return await sourceTask.Then(source => source.GroupBy(keySelector, elementSelector, resultSelector, comparer));
        }

        public static async Task<IEnumerable<TSource>> ConcatAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second)
        {
            return await firstTask.Then(first => first.Concat(second));
        }

        public static async Task<IEnumerable<TResult>> ZipAsync<TFirst, TSecond, TResult>(this Task<IEnumerable<TFirst>> firstTask, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return await firstTask.Then(first => first.Zip(second, resultSelector));
        }

        public static async Task<IEnumerable<TSource>> DistinctAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask)
        {
            return await sourceTask.Then(source => source.Distinct());
        }

        public static async Task<IEnumerable<TSource>> DistinctAsync<TSource>(this Task<IEnumerable<TSource>> sourceTask, IEqualityComparer<TSource> comparer)
        {
            return await sourceTask.Then(source => source.Distinct(comparer));
        }

        public static async Task<IEnumerable<TSource>> UnionAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second)
        {
            return await firstTask.Then(first => first.Union(second));
        }

        public static async Task<IEnumerable<TSource>> UnionAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return await firstTask.Then(first => first.Union(second, comparer));
        }

        public static async Task<IEnumerable<TSource>> IntersectAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second)
        {
            return await firstTask.Then(first => first.Intersect(second));
        }

        public static async Task<IEnumerable<TSource>> IntersectAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return await firstTask.Then(first => first.Intersect(second, comparer));
        }

        public static async Task<IEnumerable<TSource>> ExceptAsync<TSource>(this Task<IEnumerable<TSource>> firstTask, IEnumerable<TSource> second)
        {
            return await firstTask.Then(first => first.Except(second));
        }
    }
}