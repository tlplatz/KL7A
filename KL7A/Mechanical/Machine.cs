using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KL7A.Configuration;
using KL7A.Enums;

namespace KL7A.Mechanical
{
    internal class Machine
    {
        private FixedRotor _plate1;
        private FixedRotor _plate2;

        private List<Rotor> _rotors = new List<Rotor>();

        internal int[] _characterDistribution = new int[26];
        internal int[] _rotorMoveCounts = new int[10];
        internal int[] _reflexCounts = new int[16];
        internal List<string> _rotorPositions = new List<string>();

        public Machine()
        {
            _plate1 = new FixedRotor("Plate1", Wiring.Current.Plate1);
            _plate2 = new FixedRotor("Plate2", Wiring.Current.Plate2);

            for (int i = 0; i < 8; i++)
            {
                Rotor r = new Rotor((RotorName)i);

                r.AlphabetRingPosition = i;
                r.NotchRingName = (NotchRingName)i;
                r.NotchRingPosition = i;

                _rotors.Add(r);
            }
        }
        public Machine(Settings s) : this()
        {
            _rotors.Clear();

            foreach (var c in s.Rotors)
            {
                Rotor newRotor = new Rotor(c.RotorName);

                newRotor.AlphabetRingPosition = c.AlphabetRingPosition;
                newRotor.NotchRingName = c.NotchRingName;
                newRotor.NotchRingPosition = c.NotchRingPosition;

                _rotors.Add(newRotor);
            }
        }

        private int CharToWire(char c)
        {
            return Constants.ALPHA.IndexOf(c);
        }
        private char WireToChar(int wire)
        {
            return Constants.ALPHA[wire];
        }

        private int Encipher(int left)
        {
            int result = _plate1.WireRight(left);

            for (int i = 0; i < 8; i++)
            {
                result = _rotors[i].WireRight(result);
            }

            result = _plate2.WireRight(result);

            return result;
        }
        private int Decipher(int right)
        {
            int result = _plate2.WireLeft(right);

            for (int i = 7; i >= 0; i--)
            {
                result = _rotors[i].WireLeft(result);
            }

            result = _plate1.WireLeft(result);

            return result;
        }

        private char Encipher(char left)
        {
            int reflex = 0;

            int result = CharToWire(left);

            result = Encipher(result);

            while (result >= Constants.ALPHA.Length)
            {
                reflex += 1;
                result = Encipher(result);
            }

            _reflexCounts[reflex] += 1;

            return WireToChar(result);
        }
        private char Decipher(char left)
        {
            int reflex = 0;

            int result = CharToWire(left);

            result = Decipher(result);

            while (result >= Constants.ALPHA.Length)
            {
                reflex += 1;
                result = Decipher(result);
            }

            _reflexCounts[reflex] += 1;

            return WireToChar(result);
        }

        private int RotorPositionIndex()
        {
            int i = 0;
            for (int j = 0; j < _rotors.Count; j++)
            {
                if (_rotors[j].OnNotch)
                {
                    i += (int)Math.Pow(2.0, j);
                }
            }
            return i;
        }
        private void MoveRotors()
        {
            int positionIndex = RotorPositionIndex();

            string move = Wiring.Current.Moves[positionIndex];
            _rotorMoveCounts[move.Length] += 1;

            for (int i = 0; i < move.Length; i++)
            {
                int index = int.Parse(move[i].ToString());

                if (index % 2 == 0)
                {
                    _rotors[index].Advance();
                }
                else
                {
                    _rotors[index].Retreat();
                }
            }
        }

        public string Settings
        {
            get
            {
                return string.Join(", ", _rotors.Select(r => r.Setting));
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (i < _rotors.Count)
                    {
                        _rotors[i].Setting = value[i].ToString();
                    }
                }
            }
        }

        public string Encipher(string input)
        {
            StringBuilder sb = new StringBuilder();

            ResetAll();

            _rotorPositions.Add(Settings);

            foreach (char c in input)
            {
                MoveRotors();
                _rotorPositions.Add(Settings);

                char enc = Encipher(c);
                _characterDistribution[Constants.ALPHA.IndexOf(enc)] += 1;
                sb.Append(enc);
            }

            return sb.ToString();
        }
        public string Decipher(string input)
        {
            StringBuilder sb = new StringBuilder();

            ResetAll();

            _rotorPositions.Add(Settings);

            foreach (char c in input)
            {
                MoveRotors();
                _rotorPositions.Add(Settings);

                char dec = Decipher(c);
                _characterDistribution[Constants.ALPHA.IndexOf(dec)] += 1;
                sb.Append(dec);
            }

            return sb.ToString();
        }

        private void ResetArray(int[] values)
        {
            values.ToList().ForEach(v => v = 0);
        }
        private void ResetAll()
        {
            ResetArray(_characterDistribution);
            ResetArray(_rotorMoveCounts);
            ResetArray(_reflexCounts);
            _rotorPositions.Clear();
        }
    }
}
