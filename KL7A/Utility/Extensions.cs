using System;
using System.Collections.Generic;
using System.Linq;
using KL7A.Enums;

namespace KL7A.Utility
{
    internal static class Extensions
    {
        private static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
        public static IEnumerable<T[]> GenerateAllPermutations<T>(this IEnumerable<T> source, int count) where T : IComparable
        {
            return GetKCombs(source, count).Select(x => x.ToArray());
        }

        public static string GetRotorName(this RotorName source)
        {
            return source.ToString().Last().ToString();
        }
        public static string GetNotchRingName(this NotchRingName source)
        {
            return string.Concat(source.ToString().Where(c => Char.IsNumber(c)));
        }

        public static RotorName? GetRotorNameFromString(this string s)
        {
            RotorName result = RotorName.RotorA;

            if (s.Contains("Rotor"))
            {
                if (Enum.TryParse<RotorName>(s, out result))
                {
                    return result;
                }
            }

            string check = string.Format("Rotor{0}", s);

            if (Enum.TryParse<RotorName>(check, out result))
            {
                return result;
            }

            return null;
        }
        public static NotchRingName? GetNotchRingNameFromString(this string s)
        {
            NotchRingName result = NotchRingName.NotchRing1;

            if (s.Contains("NotchRing"))
            {
                if (Enum.TryParse<NotchRingName>(s, out result))
                {
                    return result;
                }
            }

            string check = string.Join("NotchRing{0}", s);

            if (Enum.TryParse<NotchRingName>(check, out result))
            {
                return result;
            }

            return null;
        }
    }


}
