using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LINQPadHelpers
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using LINQPad;

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
                pending.Add(task);

                if (pending.Count < maxConcurrency) continue;

                var completed = await Task.WhenAny(pending);
                pending.Remove(completed);
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
                pending.Add(task);

                if (pending.Count < maxConcurrency) continue;

                var completed = await Task.WhenAny(pending);
                pending.Remove(completed);
                results.Add(await completed);
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

        public static IEnumerable<string> ToCsv<T>(this IEnumerable<T> source)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            yield return string.Join(",", properties.Select(x => x.Name));

            foreach (var element in source)
            {
                yield return string.Join(",", properties.Select(x =>
                    x.PropertyType == typeof(string) ? $"\"{x.GetValue(element)}\""
                    : x.PropertyType == typeof(DateTimeOffset) | x.PropertyType == typeof(DateTimeOffset?) ? $"\"{x.GetValue(element):yyyy-MM-dd}\"" :
                    x.GetValue(element)));
            }
        }

        public static IEnumerable<T> MonitorProgress<T>(this IQueryable<T> source, string description = null, string prefix = null, int updateEvery = 1)
        {
            var complete = 0;
            var total = source.Count();

            var progressBar = new Util.ProgressBar(prefix).Dump(description);
            var stopwatch = Stopwatch.StartNew();

            foreach (var item in source)
            {
                yield return item;

                if (Interlocked.Increment(ref complete) % updateEvery == 0)
                {
                    UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
                }
            }

            UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
        }

        public static IEnumerable<T> MonitorProgress<T>(this IEnumerable<T> source, string description = null, string prefix = null, int updateEvery = 1)
        {
            var complete = 0;
            var items = source.ToArray();
            var total = items.Length;

            var progressBar = new Util.ProgressBar(prefix).Dump(description);
            var stopwatch = Stopwatch.StartNew();

            foreach (var item in items)
            {
                yield return item;

                if (Interlocked.Increment(ref complete) % updateEvery == 0)
                {
                    UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
                }
            }

            UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
        }

        public static async Task<TResult[]> MonitorProgressAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> taskFactory, string description = null, string prefix = null, int updateEvery = 1, int concurrencyLimit = 1)
        {
            var progressBar = new Util.ProgressBar($"{prefix}Initializing").Dump(description);

            var queue = new Queue<TSource>(source);
            var total = queue.Count;
            var complete = 0;

            var stopwatch = Stopwatch.StartNew();

            UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);

            var results = new List<TResult>();
            var pending = new List<Task<TResult>>();

            while (queue.Count > 0 || pending.Any())
            {
                if (queue.Count > 0)
                {
                    var entry = queue.Dequeue();
                    pending.Add(taskFactory(entry));
                }

                if (pending.Any(x => x.IsCompleted || x.IsFaulted || x.IsCanceled) || pending.Count >= concurrencyLimit)
                {
                    var completed = await Task.WhenAny(pending);
                    results.Add(await completed);
                    pending.Remove(completed);

                    if (Interlocked.Increment(ref complete) % updateEvery == 0)
                    {
                        UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
                    }

                    continue;
                }

                await Task.Delay(100);
            }

            UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);

            return results.ToArray();
        }

        public static async Task MonitorProgressAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task> taskFactory, string description = null, string prefix = null, int updateEvery = 1, int concurrencyLimit = 1)
        {
            var progressBar = new Util.ProgressBar($"{prefix}Initializing").Dump(description);

            var queue = new Queue<TSource>(source);
            var total = queue.Count;
            var complete = 0;

            var stopwatch = Stopwatch.StartNew();

            UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);

            var pending = new List<Task>();

            while (queue.Count > 0 || pending.Any())
            {
                if (queue.Count > 0)
                {
                    var entry = queue.Dequeue();
                    pending.Add(taskFactory(entry));
                }

                if (pending.Any(x => x.IsCompleted || x.IsFaulted || x.IsCanceled) || pending.Count >= concurrencyLimit)
                {
                    var completed = await Task.WhenAny(pending);
                    pending.Remove(completed);

                    if (Interlocked.Increment(ref complete) % updateEvery == 0)
                    {
                        UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
                    }

                    continue;
                }

                await Task.Delay(100);
            }

            UpdateProgress(progressBar, complete, total, stopwatch.Elapsed, prefix);
        }

        private static void UpdateProgress(Util.ProgressBar progressBar, int complete, int total, TimeSpan elapsed, string prefix)
        {
            lock (progressBar)
            {
                var rate = total == 0 ? 0 : complete / elapsed.TotalSeconds;
                var remaining = total == 0 || complete == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds((total - complete) / rate);
                progressBar.Fraction = total == 0 ? 1 : (double)complete / total;
                progressBar.Caption = $"{prefix}{complete} / {total} @ {rate:f0} / s, {remaining.TotalMinutes:f0}:{remaining.Seconds:00} rem";
            }
        }

        public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> source, double rate)
        {
            var random = new Random();
            foreach (var item in source)
            {
                if (random.NextDouble() < rate)
                    yield return item;
            }
        }
    }
}