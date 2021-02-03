using System;
using System.Collections;
using System.Collections.Generic;
using Functional.Type;

namespace Functional
{
    public static class HOF
    {
        public static T[] Filter<T> (Predicate<T> conditions, T[] iter)
        {
            var result = new List<T>();

            for (int i = 0; i < iter.Length; i++)
            {
                if (conditions(iter[i]))
                {
                    result.Add(iter[i]);
                }
            }

            return result.ToArray();
        }

        public static IEnumerable<T> filter<T>(Predicate<T> conditions, IEnumerator iter)
        {
            while (iter.MoveNext())
            {
                if (conditions((T)iter.Current))
                {
                    yield return (T)iter.Current;
                }
            }
        }

        public static T Reduce<T>(Func<T, T, T> function, T[] iter)
        {
            if (iter.Length == 0) return default(T);

            T currentVal = iter[0];

            for (int i = 0; i < iter.Length; i++)
            {
                currentVal = function(currentVal, iter[i]);
            }

            return currentVal;
        }

        public static T Reduce<T, U>(Func<T, U, T> function, U[] iter, T startValue)
        {
            T currentVal = startValue;

            for (int i = 0; i < iter.Length; i++)
            {
                currentVal = function(currentVal, iter[i]);
            }

            return currentVal;
        }

        public static T Reduce<T>(Func<T, T, T> function, IEnumerator iter)
        {
            if (!iter.MoveNext()) return default(T);

            T currentVal = (T) iter.Current;

            while (iter.MoveNext())
            {
                currentVal = function(currentVal, (T)iter.Current);
            }

            return currentVal;
        }

        public static T Reduce<T, U>(Func<T, U, T> function, IEnumerator iter, T startValue)
        {
            T currentVal = startValue;

            while (iter.MoveNext())
            {
                currentVal = function(currentVal, (U)iter.Current);
            }

            return currentVal;
        }

        public static void Map<T>(Action<T> function, T[] iter)
        {
            for (int i = 0; i < iter.Length; i++)
            {
                function(iter[i]);
            }
        }

        public static void Map<T>(Action<T, int> function, T[] iter)
        {
            for (int i = 0; i < iter.Length; i++)
            {
                function(iter[i], i);
            }
        }

        public static U[] Map<T, U>(Func<T, U> function, T[] iter)
        {
            var result = new List<U>();

            for (int i = 0; i < iter.Length; i++)
            {
                result.Add(function(iter[i]));
            }

            return result.ToArray();
        }
    }
}
