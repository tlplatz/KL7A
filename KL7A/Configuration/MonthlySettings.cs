using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using KL7A.Utility;

namespace KL7A.Configuration
{
    [Serializable]
    public class MonthlySettings : IEquatable<MonthlySettings>, ICloneable, IHasIndicators
    {
        private const string DEFAULT_NAME = "Settings";
        private const string DEFAULT_CLASSIFICATION = "Secret";
        private const int GROUP_SIZE = 5;

        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Classification { get; set; }
        [XmlAttribute]
        public DateTime Date { get; set; }
        [XmlAttribute]
        public string Indicator { get; set; }
        [XmlAttribute]
        public string NumericIndicator { get; set; }

        public List<Settings> DailySettings { get; set; }

        public MonthlySettings()
        {
            DailySettings = new List<Settings>();
        }

        public static MonthlySettings Random(string name, string clf, DateTime date, bool interval = false)
        {
            MonthlySettings result = new MonthlySettings();
            result.Wiring = Wiring.Random(interval);
            Wiring.Current = result.Wiring;

            result.Name = name;
            result.Classification = clf;
            result.Date = new DateTime(date.Year, date.Month, 1);
            result.Indicator = RandomUtil.GetRandomAlpha(GROUP_SIZE);
            result.NumericIndicator = RandomUtil.GetRandomNumeric(GROUP_SIZE);

            for (int d = 1; d <= DateTime.DaysInMonth(date.Year, date.Month); d++)
            {
                result.DailySettings.Add(Settings.Random());

                result.DailySettings.Last().Date = new DateTime(date.Year, date.Month, d);
            }

            result.DailySettings.ForEach(s =>
            {
                s.ParentSettings = result;
                s.ParentId = result.Id;
            });

            IndicatorUtil.FixDuplicateIndicators(result.DailySettings);

            return result;
        }
        public static MonthlySettings Random(DateTime date, bool interval = false)
        {
            return Random(DEFAULT_NAME, DEFAULT_CLASSIFICATION, date, interval);
        }
        public static MonthlySettings Random(bool interval = false)
        {
            return Random(DateTime.Now);
        }

        public static MonthlySettings Open(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(typeof(MonthlySettings));
                return (MonthlySettings)ser.Deserialize(fs);
            }
        }

        public void Save(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(GetType());
                ser.Serialize(fs, this);
            }
        }

        public Settings Today
        {
            get
            {
                return DailySettings.FirstOrDefault(s => s.Date.Day == DateTime.Now.Day);
            }
        }
        public Settings this[int day]
        {
            get
            {
                return DailySettings.FirstOrDefault(s => s.Date.Day == day);
            }
        }
        public Settings this[string indicator]
        {
            get
            {
                if (indicator.All(c => char.IsNumber(c)))
                {
                    return DailySettings.FirstOrDefault(s => s.NumericIndicator == indicator);
                }
                else
                {
                    return DailySettings.FirstOrDefault(s => s.Indicator == indicator);
                }
            }
        }

        public override string ToString()
        {
            return Formatting.FormatMonthlySettings(this);
        }

        [XmlIgnore]
        public string Compact
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Join("\t", Classification, Name, Indicator, NumericIndicator, Date.ToString("MMM yyyy"), Environment.NewLine));
                foreach (var ds in DailySettings)
                {
                    sb.AppendLine(ds.Compact);
                }
                return sb.ToString().ToUpper();
            }
            set
            {
                string[] lines = value.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string[] header = lines[0].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

                Classification = header[0];
                Name = header[1];
                Indicator = header[2];
                NumericIndicator = header[3];

                string[] monYr = header[4].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int y = int.Parse(monYr[1]);
                int m = Constants.MonthLabels.IndexOf(monYr[0]) + 1;

                Date = new DateTime(y, m, 1);

                DailySettings.Clear();

                for (int i = 1; i < lines.Length; i++)
                {
                    DailySettings.Add(new Settings() { Compact = lines[i] });
                }
            }
        }

        public bool Equals(MonthlySettings other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (DailySettings.Count != other.DailySettings.Count) return false;

            for (int i = 0; i < DailySettings.Count; i++)
            {
                if (DailySettings[i] != other.DailySettings[i]) return false;
            }

            return Name == other.Name &&
                Classification == other.Classification &&
                Date == other.Date &&
                Indicator == other.Indicator &&
                NumericIndicator == other.NumericIndicator;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;
            MonthlySettings monSet = obj as MonthlySettings;
            if (monSet == null) return false;
            return Equals(monSet);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 7411;
                result = (result * 733) ^ Name.GetHashCode();
                result = (result * 733) ^ Classification.GetHashCode();
                result = (result * 733) ^ Date.GetHashCode();
                result = (result * 733) ^ Indicator.GetHashCode();
                result = (result * 733) ^ NumericIndicator.GetHashCode();

                foreach (Settings s in DailySettings)
                {
                    result = (result * 733) ^ s.GetHashCode();
                }

                return result;
            }
        }

        public static bool operator ==(MonthlySettings r1, MonthlySettings r2)
        {
            if (ReferenceEquals(r1, null))
            {
                return ReferenceEquals(r2, null);
            }
            return r1.Equals(r2);
        }
        public static bool operator !=(MonthlySettings r1, MonthlySettings r2)
        {
            return !(r1 == r2);
        }

        public object Clone()
        {
            MonthlySettings monSet = new MonthlySettings
            {
                Classification = this.Classification,
                Date = this.Date,
                Indicator = this.Indicator,
                Name = this.Name,
                NumericIndicator = this.NumericIndicator
            };

            foreach (Settings s in DailySettings)
            {
                monSet.DailySettings.Add(s.Clone() as Settings);
            }

            return monSet;
        }

        public Wiring Wiring { get; set; }

        [XmlIgnore]
        public YearlySettings ParentSettings { get; set; }

        [XmlAttribute]
        public int ParentId { get; set; }

    }
}
