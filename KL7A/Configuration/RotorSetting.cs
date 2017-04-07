using System;
using System.Xml.Serialization;
using KL7A.Enums;
using KL7A.Mechanical;
using KL7A.Utility;

namespace KL7A.Configuration
{
    [Serializable]
    public class RotorSetting : IEquatable<RotorSetting>, ICloneable, IValidate
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public RotorName RotorName { get; set; }
        [XmlAttribute]
        public int AlphabetRingPosition { get; set; }
        [XmlAttribute]
        public NotchRingName NotchRingName { get; set; }
        [XmlAttribute]
        public int NotchRingPosition { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", RotorName.GetRotorName(), AlphabetRingPosition, NotchRingName.GetNotchRingName(), Rotor.PositionToSetting(NotchRingPosition));
        }

        [XmlIgnore]
        public string CompactView
        {
            get
            {
                return ToString();
            }
            set
            {
                string[] tokens = value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                RotorName = tokens[0].GetRotorNameFromString() ?? RotorName.RotorA;
                NotchRingName = tokens[2].GetNotchRingNameFromString() ?? NotchRingName.NotchRing1;
                AlphabetRingPosition = int.Parse(tokens[1]);
                NotchRingPosition = Rotor.SettingToPosition(tokens[3]);
            }
        }

        public bool Equals(RotorSetting other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return RotorName == other.RotorName &&
                AlphabetRingPosition == other.AlphabetRingPosition &&
                NotchRingName == other.NotchRingName &&
                NotchRingPosition == other.NotchRingPosition;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            RotorSetting rs = obj as RotorSetting;
            if (rs == null) return false;
            return Equals(rs);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 7411;
                result = (result * 733) ^ RotorName.GetHashCode();
                result = (result * 733) ^ AlphabetRingPosition;
                result = (result * 733) ^ NotchRingName.GetHashCode();
                result = (result * 733) ^ NotchRingPosition;
                return result;
            }
        }

        public object Clone()
        {
            return new RotorSetting
            {
                AlphabetRingPosition = this.AlphabetRingPosition,
                NotchRingName = this.NotchRingName,
                NotchRingPosition = this.NotchRingPosition,
                RotorName = this.RotorName
            };
        }

        public bool Validate()
        {
            if (AlphabetRingPosition < 0) throw new ArgumentOutOfRangeException("AlphabetRingPosition");
            if (AlphabetRingPosition >=36 ) throw new ArgumentOutOfRangeException("AlphabetRingPosition");

            if (NotchRingPosition < 0) throw new ArgumentOutOfRangeException("NotchRingPosition");
            if (NotchRingPosition >= 36) throw new ArgumentOutOfRangeException("NotchRingPosition");

            return true;
        }

        public static bool operator ==(RotorSetting r1, RotorSetting r2)
        {
            if (ReferenceEquals(r1, null))
            {
                return ReferenceEquals(r2, null);
            }
            return r1.Equals(r2);
        }
        public static bool operator !=(RotorSetting r1, RotorSetting r2)
        {
            return !(r1 == r2);
        }

        [XmlIgnore]
        public Settings ParentSettings { get; set; }
        [XmlAttribute]
        public int ParentId { get; set; }

    }
}
