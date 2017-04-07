using System;
using System.Collections.Generic;
using System.Linq;

namespace KL7A.Utility
{
    internal static class RandomUtil
    {
        internal static Random _rand = new Random();

        public static string GenerateRotor()
        {
            List<Tuple<char, int>> values = new List<Tuple<char, int>>();
            foreach (char c in Constants.ALPHANUMERIC)
            {
                values.Add(new Tuple<char, int>(c, _rand.Next()));
            }
            return string.Concat(values.OrderBy(v => v.Item2).Select(v => v.Item1));
        }
        public static string GenerateNotchRing()
        {
            List<Tuple<bool, int>> values = new List<Tuple<bool, int>>();
            for (int i = 0; i < Constants.CONTACT_COUNT; i++)
            {
                values.Add(new Tuple<bool, int>(i % 2 == 0, _rand.Next()));
            }
            return string.Concat(values.OrderBy(v => v.Item2).Select(v => v.Item1 ? "1" : "0"));
        }
        public static string GetRandomAlpha(int length)
        {
            List<char> values = new List<char>();

            while (values.Count < length)
            {
                values.Add(Constants.ALPHA[_rand.Next(Constants.ALPHA.Length)]);
            }

            return string.Concat(values);
        }
        public static string GetRandomNumeric(int length)
        {
            List<char> values = new List<char>();

            while (values.Count < length)
            {
                values.Add(Constants.NUMERIC[_rand.Next(Constants.NUMERIC.Length)]);
            }

            return string.Concat(values);
        }
        public static List<T> GetRandomEnumSequence<T>(int length)
        {
            List<Tuple<T, int>> values = new List<Tuple<T, int>>();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                values.Add(new Tuple<T, int>(value, _rand.Next()));
            }

            return values.OrderBy(v => v.Item2).Select(v => v.Item1).Take(length).ToList();
        }
        public static List<string> GetRandomMoves()
        {
            List<Tuple<string, int>> moves = new List<Tuple<string, int>>();

            foreach (string s in Constants.Moves)
            {
                moves.Add(new Tuple<string, int>(s, _rand.Next()));
            }

            return moves.OrderBy(v => v.Item2).Select(v => v.Item1).ToList();
        }
    }
}
