using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LINQPadHelpers
{
    public static class Extensions
    {
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

        public static bool Matches(this string input, string positivePattern, string negativePattern = null, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if(!string.IsNullOrEmpty(positivePattern) && !Regex.IsMatch(input, positivePattern, options))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(negativePattern) && Regex.IsMatch(input, negativePattern, options))
            {
                return false;
            }

            return true;
        }
    }
}