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
   }
}