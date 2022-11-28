using System;
using System.Collections.Generic;
using System.Linq;

namespace DA_Assets.Shared.CodeHelpers
{
    public static class ListExtensions
    {
        public static IEnumerable<TSource> Exclude<TSource, TKey>(this IEnumerable<TSource> source,
IEnumerable<TSource> exclude, Func<TSource, TKey> keySelector)
        {
            var excludedSet = new HashSet<TKey>(exclude.Select(keySelector));
            return source.Where(item => !excludedSet.Contains(keySelector(item)));
        }
        /// <summary>
        /// https://stackoverflow.com/a/36856433
        /// </summary>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
        {
            return values.All(value => source.Contains(value));
        }
        /// <summary>
        /// The method checks if collection1 contains at least one element of collection2.
        /// </summary>
        public static bool Contains<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            foreach (T item1 in collection1)
            {
                if (collection2.Contains(item1))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Checks if the IEnumerable contains only duplicates.
        /// </summary>
        public static bool IsContainsElementOnly<T>(this IEnumerable<T> ienum, T element)
        {
            if (ienum == null)
            {
                return false;
            }

            if (ienum.Count() == 0)
            {
                return false;
            }

            foreach (var item in ienum)
            {
                if (item.Equals(element) == false)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Finds the maximum element in the list by a specific field, not by all fields of an object.
        /// <para><see href="https://stackoverflow.com/a/31864296"/></para>
        /// </summary>
        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }
        public static IEnumerable<T> GetBetweenElement<T>(this T element, IEnumerable<T> ienumerable)
        {
            List<T> between = new List<T>();

            foreach (T item in ienumerable)
            {
                if (element.Equals(item) == false)
                {
                    between.Add(item);
                }
                else
                {
                    break;
                }
            }

            return between;
        }
        /// <summary>
        /// Extensions to shorten the check for the presence of elements in the IEnumerable.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection != null && collection.Count() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Split's list to X chunks.
        /// <para><see href="https://stackoverflow.com/a/419063"/></para>
        /// </summary>
        public static List<List<T>> ToChunks<T>(this IEnumerable<T> array, int shunkSize)
        {
            int i = 0;
            List<List<T>> result = array.GroupBy(s => i++ / shunkSize).Select(g => g.ToList()).ToList();
            return result;
        }
        public static List<T> FromChunks<T>(this IEnumerable<IEnumerable<T>> chunks)
        {
            return chunks.SelectMany(x => x).Where(x => x.Equals(default) == false).ToList();
        }
        /// <summary>
        /// Adds dictionary to another dictionary.
        /// <para><see href="https://stackoverflow.com/a/3982463"/></para>
        /// </summary>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (T element in source)
                target.Add(element);
        }
        public static List<string> Reverse(this List<string> array)
        {
            for (int i = 0; i < array.Count() / 2; i++)
            {
                string tmp = array[i];
                array[i] = array[array.Count() - i - 1];
                array[array.Count() - i - 1] = tmp;
            }

            return array;
        }
    }
}