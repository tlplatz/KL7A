using System.Linq;
using KL7A.Configuration;
using KL7A.Enums;

namespace KL7A.Mechanical
{
    internal class Rotor : FixedRotor
    {
        public Rotor(RotorName name) :
            base(name.ToString(), Wiring.Current.Rotors[(int)name])
        {
            _rotorName = name;
            _notchRingInitializer = Wiring.Current.Notches[(int)_notchRingName];
            InitNotches();
        }

        private RotorName _rotorName;
        private int _alphabetRingPosition;
        private NotchRingName _notchRingName;
        private int _notchRingPosition;

        private string _notchRingInitializer;

        public RotorName RotorName
        {
            get
            {
                return _rotorName;
            }
            set
            {
                if (_rotorName != value)
                {
                    Name = value.ToString();
                    _initializer = Wiring.Current.Rotors[(int)_rotorName];
                    Initialize();
                }
            }
        }
        public int AlphabetRingPosition
        {
            get
            {
                return _alphabetRingPosition;
            }
            set
            {
                if (_alphabetRingPosition != value)
                {
                    _alphabetRingPosition = value;
                }
            }
        }
        public NotchRingName NotchRingName
        {
            get
            {
                return _notchRingName;
            }
            set
            {
                if (_notchRingName != value)
                {
                    _notchRingName = value;
                    _notchRingInitializer = Wiring.Current.Notches[(int)_notchRingName];
                    InitNotches();
                }
            }
        }
        public int NotchRingPosition
        {
            get
            {
                return _notchRingPosition;
            }
            set
            {
                if (_notchRingPosition != value)
                {
                    _notchRingPosition = value;
                    InitNotches();
                }
            }
        }

        private void InitNotches()
        {
            //reset all notches to have no notch
            _contacts.ForEach(c => c.Notch = false);

            for (int i = 0; i < _contacts.Count; i++)
            {
                int index = i + _notchRingPosition;

                if (index >= _contacts.Count)
                    index -= _contacts.Count;

                _contacts[i].Notch = _notchRingInitializer[index] == '1';
            }
        }

        public int Position { get; private set; }
        public string Setting
        {
            get
            {
                return PositionToSetting(Position);
            }
            set
            {
                Position = SettingToPosition(value);
            }
        }

        public static string PositionToSetting(int position)
        {
            if (Constants.ROTOR_LABELS[position] == '-')
            {
                return string.Concat(Constants.ROTOR_LABELS[position - 1], "+");
            }
            return Constants.ROTOR_LABELS[position].ToString();
        }
        public static int SettingToPosition(string setting)
        {
            if (setting.Length == 1)
            {
                return Constants.ROTOR_LABELS.IndexOf(setting[0]);
            }
            return Constants.ROTOR_LABELS.IndexOf(setting[0]) + 1;
        }

        public void Advance()
        {
            Position++;
            if (Position >= _contacts.Count)
                Position -= _contacts.Count;
        }
        public void Retreat()
        {
            Position--;
            if (Position < 0)
                Position += _contacts.Count;
        }

        public bool OnNotch
        {
            get
            {
                return _contacts[Position].Notch;
            }
        }

        public override int WireRight(int left)
        {
            int offset = Position - _alphabetRingPosition;
            if (offset < 0)
                offset += _contacts.Count;
            int index = offset + left;
            if (index >= _contacts.Count)
                index -= _contacts.Count;
            index = _contacts[index].WireRight;
            index -= offset;
            if (index < 0)
                index += _contacts.Count;
            return index;
        }
        public override int WireLeft(int right)
        {
            int offset = Position - _alphabetRingPosition;
            if (offset < 0)
                offset += _contacts.Count;
            int index = offset + right;
            if (index >= _contacts.Count)
                index -= _contacts.Count;
            index = _contacts.FirstOrDefault(c => c.WireRight == index).WireLeft;
            index -= offset;
            if (index < 0)
                index += _contacts.Count;
            return index;
        }
    }
}
