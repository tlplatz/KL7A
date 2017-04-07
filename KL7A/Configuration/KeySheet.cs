using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KL7A.Utility;

namespace KL7A.Configuration
{
    public class KeySheet
    {
        internal const string SHEET_NAMES = "ABCDEFGHIJKLM";

        public KeySheet(int year)
        {
            Year = year;

            Sheets = new List<DailySheet>();
            SheetDefinitions = new List<Sheet>();

            List<Tuple<DailySheet, int>> values = new List<Tuple<DailySheet, int>>();

            for (int m = 1; m <= 12; m++)
            {
                for (int d = 1; d <= DateTime.DaysInMonth(year, m); d++)
                {
                    DailySheet ds = new DailySheet { Month = m, Day = d };
                    values.Add(new Tuple<DailySheet, int>(ds, RandomUtil._rand.Next()));
                }
            }

            DailySheet[] ary = values.OrderBy(v => v.Item2).Select(v => v.Item1).ToArray();

            int pointer = 0;
            foreach (var ss in ary)
            {
                ss.SheetName = SHEET_NAMES[pointer].ToString();
                pointer++;
                if (pointer == SHEET_NAMES.Length)
                {
                    pointer = 0;
                }
            }

            Sheets.AddRange(values.Select(v => v.Item1));

            foreach (char c in SHEET_NAMES)
            {
                Sheet s = new Sheet();

                s.Name = c.ToString();

                for (int x = 1; x <= 6; x++)
                {
                    for (int y = 1; y <= 6; y++)
                    {
                        for (int z = 1; z <= 6; z++)
                        {
                            s.Values.Add(new SheetEntry { Value = RandomUtil.GetRandomAlpha(5) });
                        }
                    }
                }

                SheetEntry[] entries = s.Values.OrderBy(v => v.Value).ToArray();
                pointer = 0;

                for (int x = 1; x <= 6; x++)
                {
                    for (int y = 1; y <= 6; y++)
                    {
                        for (int z = 1; z <= 6; z++)
                        {
                            entries[pointer].Key = string.Format("{0}{1}{2}", x, y, z);
                            pointer++;
                        }
                    }
                }

                SheetDefinitions.Add(s);
            }
        }

        public int Year { get; set; }

        internal List<DailySheet> Sheets { get; set; }
        internal List<Sheet> SheetDefinitions { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            const string line = "----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+";

            string title = string.Format("Key Sheet {0}", Year);
            int spacesLeft = (line.Length - 14) / 2;
            string heading = string.Concat(new string(' ', spacesLeft), title);

            string[,] values = new string[12, 31];

            for (int m = 1; m <= 12; m++)
            {
                for (int d = 1; d <= 31; d++)
                {
                    var s = Sheets.FirstOrDefault(e => e.Month == m && e.Day == d);
                    if (s != null)
                    {
                        values[m - 1, d - 1] = s.SheetName;
                    }
                    else
                    {
                        values[m - 1, d - 1] = " ";
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine(heading);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(line);
            sb.Append("    |");
            for (int m = 1; m <= 12; m++)
            {
                sb.Append(string.Format(" {0:MMM} ", new DateTime(Year, m, 1)).ToUpper());
                sb.Append("|");
            }
            sb.Append(Environment.NewLine);
            sb.AppendLine(line);
            for (int d = 0; d < 31; d++)
            {
                sb.AppendFormat(" {0:00} |", d + 1);
                for (int m = 0; m < 12; m++)
                {
                    sb.AppendFormat("  {0}  |", values[m, d]);
                }
                sb.Append(Environment.NewLine);
            }
            sb.AppendLine(line);

            sb.AppendLine();

            foreach (var item in SheetDefinitions)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }
    }

    internal class DailySheet
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public string SheetName { get; set; }
    }

    internal class Sheet
    {
        public Sheet()
        {
            Values = new List<SheetEntry>();
        }

        public string Name { get; set; }

        public List<SheetEntry> Values { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            const int Width = 70;
            string title = string.Format("Sheet {0}", Name);

            sb.AppendLine();
            sb.AppendLine(string.Concat(new string(' ', 31), title));
            sb.AppendLine();
            sb.AppendLine();

            SheetEntry[] sortedValues = Values.OrderBy(v => v.Key).ToArray();
            int index = 0;

            for (int row = 0; row < 36; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    index = row * 6 + col;
                    sb.Append(sortedValues[index].Key);
                    sb.Append(" ");
                    sb.Append(sortedValues[index].Value);
                    sb.Append("    ");
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }

    internal class SheetEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }


}
