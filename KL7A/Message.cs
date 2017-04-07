using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using KL7A.Configuration;
using KL7A.Mechanical;
using KL7A.Utility;
using MathNet.Numerics.Statistics;

namespace KL7A
{
    public class Message
    {
        private const char SPACE = ' ';
        private const char SPACE_REPLACE = 'Z';
        private const char ESCAPE_KEY = 'J';
        private const int GROUP_SIZE = 5;
        private const int GROUPS_PER_ROW = 10;
        private const int LINES_PER_SECTION = 1000;

        private enum ConstructorMode
        {
            Default,
            Settings,
            Id,
            FileName
        }

        private ConstructorMode _mode;

        private Settings _settings;
        private Machine _machine;

        private int _id;
        private string _fileName;

        public Message()
        {
            _mode = ConstructorMode.Default;
            _settings = Settings.Default();
            Wiring.Current = Wiring.Default();
            _machine = new Machine(_settings);

            MessageKey = "AAAAA";

        }
        public Message(Settings s)
        {
            _mode = ConstructorMode.Settings;

            _settings = s;
            _machine = new Machine(_settings);

            Wiring.Current = Wiring.Default();

            if (_settings.ParentSettings != null)
            {
                if (_settings.ParentSettings.Wiring != null)
                {
                    Wiring.Current = _settings.ParentSettings.Wiring;
                }
            }

            MessageKey = RandomUtil.GetRandomAlpha(5);
        }
        public Message(int id)
        {
            _mode = ConstructorMode.Id;

            YearlySettings ys = YearlySettings.ReadFromDb(id);

            _settings = ys.Today;
            Wiring.Current = _settings.ParentSettings.Wiring;

            _machine = new Machine(_settings);

            MessageKey = RandomUtil.GetRandomAlpha(5);

            _id = id;
        }
        public Message(string fileName)
        {
            _mode = ConstructorMode.FileName;

            YearlySettings ys = YearlySettings.Open(fileName);

            _settings = ys.Today;
            Wiring.Current = _settings.ParentSettings.Wiring;

            _machine = new Machine(_settings);

            MessageKey = RandomUtil.GetRandomAlpha(5);

            _fileName = fileName;
        }

        public string MessageKey { get; set; }

        private string CharToHex(char c)
        {
            return ((int)c).ToString("X2");
        }
        private char HexToChar(string hex)
        {
            return (char)Convert.ToInt32(hex, 16);
        }
        private string FormatHex(string hex)
        {
            return string.Concat(Constants.HEX_SUBS[Constants.HEX.IndexOf(hex[0])], 
                Constants.HEX_SUBS[Constants.HEX.IndexOf(hex[1])]);
        }
        private string UnFormatHex(string value)
        {
            return string.Concat(Constants.HEX[Constants.HEX_SUBS.IndexOf(value[0])], 
                Constants.HEX[Constants.HEX_SUBS.IndexOf(value[1])]);
        }

        private string FormatEncryptInput(string value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                if (c == SPACE)
                {
                    sb.Append(SPACE_REPLACE);
                    continue;
                }
                if (c == SPACE_REPLACE || c == ESCAPE_KEY || !Constants.ALPHA.Contains(c))
                {
                    sb.Append(ESCAPE_KEY);
                    sb.Append(FormatHex(CharToHex(c)));
                    continue;
                }
                sb.Append(c);
            }

            return sb.ToString();
        }
        private string FormatEncryptOutput(string value)
        {
            StringBuilder sb = new StringBuilder();

            //group, row, section
            int g = 0, r = 0, s = 0;

            foreach (char c in value)
            {
                sb.Append(c);
                g++;

                if (g == GROUP_SIZE)
                {
                    sb.Append(' ');
                    g = 0;

                    GroupCount++;

                    r++;

                    if (r == GROUPS_PER_ROW)
                    {
                        sb.Append(Environment.NewLine);
                        r = 0;

                        s++;

                        if (s == LINES_PER_SECTION)
                        {
                            sb.Append(Environment.NewLine);
                            s = 0;
                        }
                    }
                }
            }


            GroupCount -= 1;

            return sb.ToString().Trim();
        }
        private string FormatDecryptInput(string value, out string key)
        {
            string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            key = PhoneticToKey(lines[0].Substring(5));
            string noKey = string.Concat(lines.Skip(1));
            return string.Concat(noKey.Where(c => Constants.ALPHA.Contains(c)));
        }
        private string FormatDecryptOutput(string value)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == SPACE_REPLACE)
                {
                    sb.Append(SPACE);
                    continue;
                }
                if (value[i] == ESCAPE_KEY)
                {
                    sb.Append(HexToChar(UnFormatHex(value.Substring(i + 1, 2))));
                    i += 2;
                    continue;
                }
                sb.Append(value[i]);
            }

            return sb.ToString();
        }

        private int PadCount(string value)
        {
            int rem = value.Length % GROUP_SIZE;
            if (rem == 0)
            {
                return 0;
            }
            return GROUP_SIZE - rem;
        }
        private string PadString(string value)
        {
            //pads with spaces so that the trimmed output is the plain text
            return string.Concat(value, new string(SPACE_REPLACE, PadCount(value)));
        }

        private string KeyToPhonetic(string value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                sb.Append(Constants.PhoneticValues[Constants.ALPHA.IndexOf(c)]);
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }
        private string PhoneticToKey(string value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                sb.Append(Constants.ALPHA[Constants.PhoneticValues.IndexOf(s)]);
            }

            return sb.ToString().Trim();
        }

        public string CipherText { get; private set; }
        public string PlainText { get; private set; }
        public int GroupCount { get; private set; }

        public string CharacterDistribution
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int row = 0; row < 4; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        int index = col * 4 + row;

                        if (index < _machine._characterDistribution.Length)
                        {
                            sb.Append(string.Format("{0} = {1}", Constants.ALPHA[index], _machine._characterDistribution[index]));
                            sb.Append("\t");
                        }
                    }
                    sb.Append(Environment.NewLine);
                }

                return sb.ToString().Trim();
            }
        }
        public string RotorMoveCounts
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < _machine._rotorMoveCounts.Length; i++)
                {
                    if (_machine._rotorMoveCounts[i] != 0)
                    {
                        sb.AppendFormat("{0} = {1}\r\n", i, _machine._rotorMoveCounts[i]);
                    }
                }

                return sb.ToString().Trim();
            }
        }
        public string ReflexCounts
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < _machine._reflexCounts.Length; i++)
                {
                    if (_machine._reflexCounts[i] != 0)
                    {
                        sb.AppendFormat("{0} = {1}\r\n", i, _machine._reflexCounts[i]);
                    }
                }

                return sb.ToString().Trim();
            }
        }
        public string Histogram
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < _machine._characterDistribution.Length; i++)
                {
                    sb.AppendFormat("{0} = {1}\r\n", Constants.ALPHA[i], new string('o', _machine._characterDistribution[i]));
                }

                return sb.ToString().Trim();
            }
        }
        public string Statistics
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                var stats = GetStats();

                foreach (PropertyInfo prop in stats.GetType().GetProperties().Where(p => p.CanRead))
                {
                    sb.AppendFormat("{0} = {1:#0.00}\r\n", prop.Name, prop.GetValue(stats));
                }

                return sb.ToString().Trim();
            }
        }

        public string Indicator { get { return _settings.Indicator; } }
        public string NumericIndicator { get { return _settings.NumericIndicator; } }

        public string Encipher(string value)
        {
            PlainText = value;

            string expandedKey = string.Concat(MessageKey, _settings.StartPosition.Substring(5, 3));
            _machine.Settings = _settings.StartPosition;
            string trueKey = _machine.Encipher(expandedKey);
            _machine.Settings = trueKey;

            string indicator = string.Concat(_settings.NumericIndicator, " ", KeyToPhonetic(MessageKey));

            string result = FormatEncryptInput(value);
            result = PadString(result);
            result = _machine.Encipher(result) + _settings.NumericIndicator;
            result = FormatEncryptOutput(result);

            result = string.Join(Environment.NewLine, indicator, result).Trim();

            CipherText = result;

            return CipherText;
        }
        public string Decipher(string value)
        {
            CipherText = value;

            string key;

            if(_mode == ConstructorMode.FileName || _mode == ConstructorMode.Id)
            {
                YearlySettings ys;

                if(_mode == ConstructorMode.Id)
                {
                    ys = YearlySettings.ReadFromDb(_id);
                }
                else
                {
                    ys = YearlySettings.Open(_fileName);
                }

                string indicator = CipherText.Split(CipherText.Where(c => char.IsWhiteSpace(c)).Distinct().ToArray(), StringSplitOptions.RemoveEmptyEntries)[0];

                Settings s = ys[indicator];
                Wiring.Current = s.ParentSettings.Wiring;
            }

            string result = FormatDecryptInput(value, out key);

            MessageKey = key;

            string expandedKey = string.Concat(key, _settings.StartPosition.Substring(5, 3));
            _machine.Settings = _settings.StartPosition;
            string trueKey = _machine.Encipher(expandedKey);
            _machine.Settings = trueKey;

            result = _machine.Decipher(result);

            PlainText = FormatDecryptOutput(result).Trim();

            return PlainText;
        }

        internal DescriptiveStatistics GetStats()
        {
            return new MathNet.Numerics.Statistics.DescriptiveStatistics(_machine._characterDistribution.Select(v => (double)v));
        }

        public string FindBestKey(string plainText, string baseKey)
        {
            List<Tuple<double, string>> results = new List<Tuple<double, string>>();

            for (int x = 0; x < Constants.ALPHA.Length; x++)
            {
                for (int y = 0; y < Constants.ALPHA.Length; y++)
                {
                    for (int z = 0; z < Constants.ALPHA.Length; z++)
                    {
                        string key = string.Format("{0}{1}{2}{3}", baseKey, Constants.ALPHA[x], Constants.ALPHA[y], Constants.ALPHA[z]);
                        Message msg = new Message(_settings);
                        msg.MessageKey = key;
                        msg.Encipher(plainText);
                        results.Add(new Tuple<double, string>(msg.GetStats().StandardDeviation, key));
                    }
                }
            }

            var sorted = results.OrderByDescending(v => v.Item1);
            return sorted.Last().Item2;
        }

        public override string ToString()
        {
            return _settings.ToString();
        }
    }
}
