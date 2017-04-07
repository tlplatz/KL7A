using System.Collections.Generic;
using System.Linq;
using System.Text;
using KL7A.Configuration;
using KL7A.Mechanical;

namespace KL7A.Utility
{
    internal static class Formatting
    {
        public static string SettingLineOne(Settings s)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format(" {0:00} |  ", s.Date.Day));
            sb.Append(string.Join("  ", s.Rotors.Select(r => r.RotorName.GetRotorName())));
            sb.Append(" | ");
            sb.Append(string.Join(" ", s.Rotors.Select(r =>
            {
                string result = r.NotchRingName.GetNotchRingName();
                if (result.Length == 1)
                {
                    return string.Concat(" ", result);
                }
                return result;
            })));
            sb.AppendFormat(" | {0} {1} ", s.StartPosition.Substring(0, 4), s.StartPosition.Substring(4, 4));
            sb.AppendFormat("| {0} {1} ", s.Check.Substring(0, 5), s.Check.Substring(5, 5));
            sb.AppendFormat("| {0}", s.Indicator);

            return sb.ToString();
        }
        public static string SettingLineTwo(Settings s)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("    | ", s.Date.Day));
            sb.Append(string.Join(" ", s.Rotors.Select(r => (r.AlphabetRingPosition + 1).ToString("00"))));
            sb.Append(" | ");
            sb.Append(string.Join(" ", s.Rotors.Select(r =>
            {
                string result = Rotor.PositionToSetting(r.NotchRingPosition);
                if (result.Length == 1)
                {
                    return string.Concat(" ", result);
                }
                return result;
            })));
            sb.AppendFormat(" |           ", s.StartPosition.Substring(0, 4), s.StartPosition.Substring(4, 4));
            sb.AppendFormat("|             ", s.Check.Substring(0, 5), s.Check.Substring(5, 5));
            sb.AppendFormat("| {0}", s.NumericIndicator);

            return sb.ToString();
        }

        public const string LINE = "----+-------------------------+-------------------------+-----------+-------------+-------";
        public const string HEADING_ONE = "DAY |          ROTOR          |        NOTCH RING       |   START   | 36-45 CHAR  | INDI- ";
        public const string HEADING_TWO = "    |         SETTINGS        |         SETTINGS        | POSITION  | CHECK GRPS  | CATOR ";

        public static string Title(MonthlySettings s)
        {
            string left = string.Format("{0} CRYPTO", s.Classification.ToUpper());
            string center = string.Format("{0} {1} {2}", s.Name.ToUpper(), s.Indicator, s.NumericIndicator);
            string right = string.Format("{0:MMM yyyy}", s.Date).ToUpper();

            int leftSpaces = (LINE.Length - left.Length - center.Length - right.Length) / 2;
            int rightSpaces = LINE.Length - leftSpaces - left.Length - center.Length - right.Length;

            return string.Format("{0}{1}{2}{3}{4}", left, new string(' ', leftSpaces), center, new string(' ', rightSpaces), right);
        }

        public static string FormatSettings(Settings s)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(HEADING_ONE);
            sb.AppendLine(HEADING_TWO);
            sb.AppendLine(LINE);
            sb.AppendLine(SettingLineOne(s));
            sb.AppendLine(SettingLineTwo(s));
            sb.AppendLine(LINE);

            return sb.ToString().Trim();
        }
        public static string FormatMonthlySettings(MonthlySettings ms)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(Title(ms));
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(LINE);
            sb.AppendLine(HEADING_ONE);
            sb.AppendLine(HEADING_TWO);
            sb.AppendLine(LINE);

            foreach (Settings s in ms.DailySettings)
            {
                sb.AppendLine(SettingLineOne(s));
                sb.AppendLine(SettingLineTwo(s));
                sb.AppendLine(LINE);
            }

            return sb.ToString().Trim();
        }

        public static string FormatMessage(string input, int lineWidth)
        {
            StringBuilder sb = new StringBuilder(input);
            int lineCounter = 0;
            List<string> lines = new List<string>();
            StringBuilder currentLine = new StringBuilder();

            for (int i = 0; i < sb.Length; i++)
            {
                currentLine.Append(sb[i]);
                lineCounter++;

                if (lineCounter == lineWidth)
                {
                    for (int x = currentLine.Length - 1; x >= 0; x--)
                    {
                        if (!char.IsSeparator(currentLine[x]))
                        {
                            currentLine.Remove(x, 1);
                            i--;
                        }
                        else
                        {
                            lineCounter = 0;
                            lines.Add(currentLine.ToString());
                            currentLine = new StringBuilder();
                            break;
                        }
                    }
                }
            }

            lines.Add(currentLine.ToString());
            lines.ForEach(l => l.Trim());

            return string.Join("\r\n", lines);
        }
    }
}
