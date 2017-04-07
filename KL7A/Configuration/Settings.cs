using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using KL7A.Enums;
using KL7A.Mechanical;
using KL7A.Utility;

namespace KL7A.Configuration
{
    [Serializable]
    public class Settings : IEquatable<Settings>, ICloneable, IHasIndicators, IValidate
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public DateTime Date { get; set; }
        [XmlAttribute]
        public string Indicator { get; set; }
        [XmlAttribute]
        public string NumericIndicator { get; set; }
        [XmlAttribute]
        public string StartPosition { get; set; }
        [XmlAttribute]
        public string Check { get; set; }

        public List<RotorSetting> Rotors = new List<RotorSetting>();

        public Settings()
        {

        }

        public static Settings Default()
        {
            Settings result = new Settings();

            result.Date = new DateTime(2000, 1, 1);
            result.Indicator = "AAAAA";
            result.NumericIndicator = "11111";
            result.StartPosition = "AAAAAAAA";

            Machine m = new Machine();
            result.Check = m.Encipher(new string('L', 45)).Substring(35, 10);

            for (int i = 0; i < 8; i++)
            {
                result.Rotors.Add(new RotorSetting
                {
                    AlphabetRingPosition = i,
                    NotchRingName = (NotchRingName)i,
                    NotchRingPosition = i,
                    RotorName = (RotorName)i
                });
            }

            return result;
        }
        public static Settings Random()
        {
            Settings result = new Settings();

            result.Date = DateTime.Now.Date;
            result.Indicator = RandomUtil.GetRandomAlpha(5);
            result.NumericIndicator = RandomUtil.GetRandomNumeric(5);
            result.StartPosition = RandomUtil.GetRandomAlpha(8);

            List<RotorName> rotors = RandomUtil.GetRandomEnumSequence<RotorName>(8);
            List<NotchRingName> notches = RandomUtil.GetRandomEnumSequence<NotchRingName>(8);

            for (int i = 0; i < 8; i++)
            {
                result.Rotors.Add(new RotorSetting
                {
                    RotorName = rotors[i],
                    NotchRingName = notches[i],
                    AlphabetRingPosition = RandomUtil._rand.Next(36),
                    NotchRingPosition = RandomUtil._rand.Next(36),
                    ParentId = result.Id,
                    ParentSettings = result
                });
            }

            Machine m = new Machine(result);
            result.Check = m.Encipher(new string('L', 45)).Substring(35, 10);

            return result;
        }

        public static Settings Open(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                return (Settings)ser.Deserialize(fs);
            }
        }

        public static Settings Create(IEnumerable<RotorName> rotors, IEnumerable<int> alphabetRingPositions, IEnumerable<NotchRingName> notchRings, IEnumerable<string> notchRingPositions, string startPosition)
        {
            RotorName[] rotorNamesAry = rotors.ToArray();
            int[] alphabetRingPositionAry = alphabetRingPositions.ToArray();
            NotchRingName[] notchRingNamesAry = notchRings.ToArray();
            string[] notchRingPositionAry = notchRingPositions.ToArray();

            Settings result = Settings.Default();

            for (int i = 0; i < Constants.ROTOR_COUNT; i++)
            {
                if (i < rotorNamesAry.Length)
                {
                    result.Rotors[i].RotorName = rotorNamesAry[i];
                }

                if (i < alphabetRingPositionAry.Length)
                {
                    result.Rotors[i].AlphabetRingPosition = alphabetRingPositionAry[i] - 1;
                }

                if (i < notchRingNamesAry.Length)
                {
                    result.Rotors[i].NotchRingName = notchRingNamesAry[i];
                }

                if (i < notchRingPositionAry.Length)
                {
                    result.Rotors[i].NotchRingPosition = Rotor.SettingToPosition(notchRingPositionAry[i]);
                }
            }

            result.StartPosition = startPosition;

            return result;
        }
        public static Settings Create(IEnumerable<object> args)
        {
            return Create
                (
                    args.Where(a => a.GetType() == typeof(RotorName)).Cast<RotorName>(),
                    args.Where(a => a.GetType() == typeof(int)).Cast<int>(),
                    args.Where(a => a.GetType() == typeof(NotchRingName)).Cast<NotchRingName>(),
                    args.Where(a => a.GetType() == typeof(string)).Cast<string>().Where(s => s.Length != 8),
                    args.Where(a => a.GetType() == typeof(string)).Cast<string>().FirstOrDefault(s => s.Length == 8)
                );
        }

        public void Save(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(GetType());
                ser.Serialize(fs, this);
            }
        }

        public override string ToString()
        {
            return Formatting.FormatSettings(this);
        }

        [XmlIgnore]
        public string Compact
        {
            get
            {
                return string.Join("\t",
                    Date.ToString("yyyyMMdd"),
                    Rotors[0].CompactView,
                    Rotors[1].CompactView,
                    Rotors[2].CompactView,
                    Rotors[3].CompactView,
                    Rotors[4].CompactView,
                    Rotors[5].CompactView,
                    Rotors[6].CompactView,
                    Rotors[7].CompactView,
                    StartPosition,
                    Check,
                    NumericIndicator,
                    Indicator);
            }
            set
            {
                string[] tokens = value.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

                Date = ParseForDate(tokens[0]);

                Rotors.Clear();

                for (int i = 1; i <= 8; i++)
                {
                    Rotors.Add(new RotorSetting() { CompactView = tokens[i] });
                }

                StartPosition = tokens[9];
                Check = tokens[10];
                NumericIndicator = tokens[11];
                Indicator = tokens[12];
            }
        }

        private DateTime ParseForDate(string input)
        {
            string y = input.Substring(0, 4);
            string m = input.Substring(4, 2);
            string d = input.Substring(6, 2);

            return new DateTime(int.Parse(y), int.Parse(m), int.Parse(d));
        }

        public bool Equals(Settings other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Rotors.Count != other.Rotors.Count) return false;

            for (int i = 0; i < Rotors.Count; i++)
            {
                if (Rotors[i] != other.Rotors[i]) return false;
            }
            return Date == other.Date &&
                Indicator == other.Indicator &&
                NumericIndicator == other.NumericIndicator &&
                StartPosition == other.StartPosition &&
                Check == other.Check;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            Settings s = obj as Settings;
            if (s == null) return false;
            return Equals(s);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 7411;
                result = (result * 733) ^ Date.GetHashCode();
                result = (result * 733) ^ Indicator.GetHashCode();
                result = (result * 733) ^ NumericIndicator.GetHashCode();
                result = (result * 733) ^ StartPosition.GetHashCode();
                result = (result * 733) ^ Check.GetHashCode();

                foreach (RotorSetting r in Rotors)
                {
                    result = (result * 733) ^ r.GetHashCode();
                }

                return result;
            }
        }

        public object Clone()
        {
            Settings s = new Settings
            {
                Check = this.Check,
                Date = this.Date,
                Indicator = this.Indicator,
                NumericIndicator = this.NumericIndicator,
                StartPosition = this.StartPosition
            };

            foreach (RotorSetting rot in Rotors)
            {
                s.Rotors.Add(rot.Clone() as RotorSetting);
            }

            return s;
        }

        public bool Validate()
        {
            if (Rotors.Count != Constants.ROTOR_COUNT) throw new ArgumentException("Rotor count invalid");
            if (Rotors.GroupBy(r => r.RotorName).Select(r => r.Count()).Count() != 8) throw new ArgumentException("Duplicate rotor present");
            if (Rotors.GroupBy(r => r.NotchRingName).Select(r => r.Count()).Count() != 8) throw new ArgumentException("Duplicate notch ring present");

            return true;
        }

        public static bool operator ==(Settings r1, Settings r2)
        {
            if (ReferenceEquals(r1, null))
            {
                return ReferenceEquals(r2, null);
            }
            return r1.Equals(r2);
        }
        public static bool operator !=(Settings r1, Settings r2)
        {
            return !(r1 == r2);
        }

        [XmlIgnore]
        public MonthlySettings ParentSettings { get; set; }
        [XmlAttribute]
        public int ParentId { get; set; }

    }
}
