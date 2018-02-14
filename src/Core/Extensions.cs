using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LINQPadHelpers
{
    public static class Extensions
    {
        public static async Task<TResult> Then<TSource, TResult>(this Task<TSource> task, Func<TSource, Task<TResult>> action)
        {
            var source = await task;
            return await action(source);
        }

        public static async Task<TResult> Then<TSource, TResult>(this Task<TSource> task, Func<TSource, TResult> action)
        {
            var source = await task;
            return action(source);
        }

        public static async Task WhenAll(this IEnumerable<Task> tasks, int maxConcurrency = int.MaxValue)
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

        public static async Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks, int maxConcurrency = int.MaxValue)
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

        public static string FormatDuration(this TimeSpan timespan)
        {
            var result = new StringBuilder("P");

            if (timespan.Days > 0)
            {
                result.Append($"{timespan.Days}D");
            }

            var timePortion = timespan.Subtract(TimeSpan.FromDays(timespan.Days));
            if (timePortion.TotalSeconds < 1)
            {
                return result.ToString();
            }

            result.Append("T");

            if (timePortion.Hours > 0)
            {
                result.Append($"{timePortion.Hours}H");
            }

            if (timePortion.Minutes > 0)
            {
                result.Append($"{timePortion.Minutes}M");
            }

            if (timePortion.Seconds > 0)
            {
                result.Append($"{timePortion.Seconds}S");
            }

            return result.ToString();
        }

        public static IEnumerable<T[]> InChunks<T>(this IEnumerable<T> source, int chunkSize)
        {
            var list = new List<T>();

            foreach (var item in source)
            {
                list.Add(item);

                if (list.Count != chunkSize)
                {
                    continue;
                }

                yield return list.ToArray();
                list = new List<T>();
            }

            if (list.Count > 0)
            {
                yield return list.ToArray();
            }
        }
    }
}