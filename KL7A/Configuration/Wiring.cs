using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using KL7A.Utility;

namespace KL7A.Configuration
{
    [Serializable]
    public class Wiring : IEquatable<Wiring>, ICloneable
    {
        static Wiring()
        {
            Current = Default();
        }

        public static Wiring Current { get; set; }

        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Plate1 { get; set; }
        [XmlAttribute]
        public string Plate2 { get; set; }

        public List<string> Rotors { get; set; }
        public List<string> Notches { get; set; }
        public List<string> Moves { get; set; }

        public Wiring()
        {
            Rotors = new List<string>();
            Notches = new List<string>();
            Moves = new List<string>();
        }

        public static Wiring Default()
        {
            Wiring result = new Wiring();

            result.Plate1 = Constants.PLATE1;
            result.Plate2 = Constants.PLATE2;

            result.Rotors.AddRange(Constants.RotorDefinitions);
            result.Notches.AddRange(Constants.NotchRingDefinitions);
            result.Moves.AddRange(Constants.Moves);

            return result;
        }
        public static Wiring Random(bool useInterval = false)
        {
            Wiring result = new Wiring();

            if (!useInterval)
            {
                result.Plate1 = RandomUtil.GenerateRotor();
                result.Plate2 = RandomUtil.GenerateRotor();

                for (int i = 0; i < 16; i++)
                {
                    result.Rotors.Add(RandomUtil.GenerateRotor());
                    result.Notches.Add(RandomUtil.GenerateNotchRing());
                }
            }
            else
            {
                IntervalWiring iw = new IntervalWiring();
                var rotors = iw.ComputeRotorSet(18);

                result.Plate1 = rotors[0].ToString();
                result.Plate2 = rotors[1].ToString();

                for (int i = 0; i < 16; i++)
                {
                    result.Rotors.Add(rotors[i + 2].ToString());
                    result.Notches.Add(RandomUtil.GenerateNotchRing());
                }
            }

            result.Moves.AddRange(RandomUtil.GetRandomMoves());

            return result;
        }
        public static Wiring Open(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Wiring));
                return (Wiring)ser.Deserialize(fs);
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

        public bool Equals(Wiring other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Moves.Count != other.Moves.Count) return false;
            if (Rotors.Count != other.Rotors.Count) return false;
            if (Notches.Count != other.Notches.Count) return false;

            for (int i = 0; i < Moves.Count; i++)
            {
                if (Moves[i] != other.Moves[i]) return false;
            }
            for (int i = 0; i < Rotors.Count; i++)
            {
                if (Rotors[i] != other.Rotors[i]) return false;
            }
            for (int i = 0; i < Notches.Count; i++)
            {
                if (Notches[i] != other.Notches[i]) return false;
            }

            return Plate1 == other.Plate1 &&
                Plate2 == other.Plate2;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            Wiring w = obj as Wiring;
            if (w == null) return false;
            return Equals(w);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 7411;
                result = (result * 733) ^ Plate1.GetHashCode();
                result = (result * 733) ^ Plate2.GetHashCode();

                foreach (string s in Rotors)
                {
                    result = (result * 733) ^ s.GetHashCode();
                }
                foreach (string s in Notches)
                {
                    result = (result * 733) ^ s.GetHashCode();
                }
                foreach (string s in Moves)
                {
                    result = (result * 733) ^ s.GetHashCode();
                }

                return result;
            }
        }

        public object Clone()
        {
            Wiring result = new Wiring
            {
                Plate1 = this.Plate1,
                Plate2 = this.Plate2
            };

            foreach (string s in Rotors)
            {
                result.Rotors.Add(s);
            }
            foreach (string s in Notches)
            {
                result.Notches.Add(s);
            }
            foreach (string s in Moves)
            {
                result.Moves.Add(s);
            }

            return result;
        }

        public static bool operator ==(Wiring r1, Wiring r2)
        {
            if (ReferenceEquals(r1, null))
            {
                return ReferenceEquals(r2, null);
            }
            return r1.Equals(r2);
        }
        public static bool operator !=(Wiring r1, Wiring r2)
        {
            return !(r1 == r2);
        }

        [XmlIgnore]
        public MonthlySettings ParentSettings { get; set; }
        public int ParentId { get; set; }

    }
}
