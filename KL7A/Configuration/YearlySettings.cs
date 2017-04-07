using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using KL7A.DataAccess;
using KL7A.Utility;

namespace KL7A.Configuration
{
    [Serializable]
    public class YearlySettings : IEquatable<YearlySettings>, IHasIndicators
    {
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

        public List<MonthlySettings> MonthlySettings { get; set; }

        public YearlySettings()
        {
            MonthlySettings = new List<KL7A.Configuration.MonthlySettings>();
        }

        public static YearlySettings Random(string title, string classification, DateTime date, bool interval = false)
        {
            YearlySettings result = new YearlySettings
            {
                Date = date,
                Classification = classification,
                Name = title,
                Indicator = RandomUtil.GetRandomAlpha(5),
                NumericIndicator = RandomUtil.GetRandomNumeric(5)
            };

            for (int i = 1; i <= 12; i++)
            {
                result.MonthlySettings.Add(KL7A.Configuration.MonthlySettings.Random(title, classification, new DateTime(date.Year, i, 1), interval));
            }

            result.MonthlySettings.ForEach(m =>
            {
                m.ParentId = result.Id;
                m.ParentSettings = result;
            });

            IndicatorUtil.FixDuplicateIndicators(result.MonthlySettings);
            IndicatorUtil.FixDuplicateIndicators(result.MonthlySettings.SelectMany(m => m.DailySettings));

            return result;
        }
        public static YearlySettings Random(DateTime date, bool interval = false)
        {
            return YearlySettings.Random("Settings", "Secret", date, interval);
        }
        public static YearlySettings Random(bool interval = false)
        {
            return YearlySettings.Random(DateTime.Now, interval);
        }

        public static YearlySettings Open(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(typeof(YearlySettings));
                YearlySettings result =  (YearlySettings)ser.Deserialize(fs);

                //set parent entity properties. These aren't serialized because of circular references
                foreach(MonthlySettings monSet in result.MonthlySettings)
                {
                    monSet.ParentSettings = result;

                    foreach(Settings daySet in monSet.DailySettings)
                    {
                        daySet.ParentSettings = monSet;

                        foreach(RotorSetting rotSet in daySet.Rotors)
                        {
                            rotSet.ParentSettings = daySet;
                        }
                    }
                }

                return result;
            }
        }
        public static YearlySettings ReadFromDb(int id)
        {
            return Repository.ReadFromDb(id);
        }

        public void Save(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(GetType());
                ser.Serialize(fs, this);
            }
        }
        public void SaveToDb()
        {
            Repository.SaveToDb(this);
        }

        public Settings Today
        {
            get
            {
                int mon = DateTime.Now.Month;
                return MonthlySettings.FirstOrDefault(m => m.Date.Month == mon).Today;
            }
        }
        public Settings this[string indicator]
        {
            get
            {
                if (indicator.All(c => char.IsNumber(c)))
                {
                    return MonthlySettings.SelectMany(m => m.DailySettings).FirstOrDefault(s => s.NumericIndicator == indicator);
                }
                else
                {
                    return MonthlySettings.SelectMany(m => m.DailySettings).FirstOrDefault(s => s.Indicator == indicator);
                }
            }
        }

        public bool Equals(YearlySettings other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (MonthlySettings.Count != other.MonthlySettings.Count) return false;

            for (int i = 0; i < MonthlySettings.Count; i++)
            {
                if (MonthlySettings[i] != other.MonthlySettings[i]) return false;
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
            YearlySettings monSet = obj as YearlySettings;
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

                foreach (MonthlySettings m in MonthlySettings)
                {
                    result = (result * 733) ^ m.GetHashCode();
                }

                return result;
            }
        }

        public object Clone()
        {
            YearlySettings yearSet = new YearlySettings
            {
                Classification = this.Classification,
                Date = this.Date,
                Indicator = this.Indicator,
                Name = this.Name,
                NumericIndicator = this.NumericIndicator
            };

            foreach (MonthlySettings m in MonthlySettings)
            {
                yearSet.MonthlySettings.Add((MonthlySettings)m.Clone());
            }

            return yearSet;
        }

        public static bool operator ==(YearlySettings r1, YearlySettings r2)
        {
            if (ReferenceEquals(r1, null))
            {
                return ReferenceEquals(r2, null);
            }
            return r1.Equals(r2);
        }
        public static bool operator !=(YearlySettings r1, YearlySettings r2)
        {
            return !(r1 == r2);
        }
    }
}
