using System;
using System.Collections.Generic;
using System.Linq;
using KL7A.Utility;

namespace KL7A
{
    internal static class Constants
    {
        static Constants()
        {
            Moves = new List<string>();

            int[] ary = Enumerable.Range(0, 8).ToArray();

            for (int i = 3; i <= 8; i++)
            {
                Moves.AddRange(ary.GenerateAllPermutations(i).Select(v => string.Concat(v)));

                if (i == 6 || i == 7 || i == 8)
                {
                    //permutations for 6 7 and 8 moves are doubled to make the total number 256 (2^8)
                    Moves.AddRange(ary.GenerateAllPermutations(i).Select(v => string.Concat(v)));
                }
            }

            PhoneticValues = PHONETIC.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim().ToUpper()).ToList();

            MonthLabels = new List<string>();

            for (int i = 1; i <= 12; i++)
            {
                MonthLabels.Add(new DateTime(2000, i, 1).ToString("MMM").ToUpper());
            }
        }

        public const string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string NUMERIC = "0123456789";

        public const string ALPHANUMERIC = ALPHA + NUMERIC;

        public const int ROTOR_COUNT = 8;
        public const int CONTACT_COUNT = 36;

        public const string PLATE1 = "B7QAWI02PLC1G4NVE3XMKTF69O8SZYR5JHUD";
        public const string PLATE2 = "XBPN0JHDWG2O68VRI7MAT1K93SQZLCFUEY54";

        public static List<string> RotorDefinitions = new List<string>() {
            "02NFZJWHR7C3LGU1XOBAPTQK8VD654I9EMSY",
            "JY6Q748CIHSPTOL0B3U9GMD2Z5FX1NEVKAWR",
            "BNFZXQ4OGT08HVK6WLDSE25CRPI3U71AJYM9",
            "GQ8CYP6VL72U9H1IFJNTS304ZWBME5KRXODA",
            "MI9WVAEB5TGYRQP3UZ7DJLN80K4H2CSXO61F",
            "AM7ZTS6FYO5RN1CJ2IULHE40V3WB9KDGQPX8",
            "ERCM27YGBPWSJ65KOLF3Q810ZD49HNTVXIAU",
            "P42VG7KENWBZMO0SCA3H1T6DLU8XJ9YQ5RFI",
            "JMWV3EGSD5ZYCL4UBXT7IP8O0RNKA6192HFQ",
            "I7TJ80F1PSZEC5QHUOX6L9WYA2MKDRB3GNV4",
            "EC8RBXJ13QMD2H5OWF4YNLUG7AP9T0ZS6KIV",
            "5XRQ4DWM3PLZAH0GSJFC2YT1U97IBEONV68K",
            "CRY4VKHNXFJ5WD2SE91GO8PMQU0ZA7B63ITL",
            "KXGEATDZL35SOF4J7QYH60PNWI9CRBV21U8M",
            "X6K9VLA2H3RU1GE7SJWQZ8NBY0C4OMFTD5IP",
            "4O526AGFQNRMJY91S7EKB0X3DVZLCTI8UPHW"
        };

        public static List<string> NotchRingDefinitions = new List<string>()
        {
            "000101111010011111101101010001000010",
            "101001000101011101101100111011000010",
            "011001000111101110000111100011011000",
            "010001110011110011000010100111110010",
            "100001110111100100000110110101110001",
            "101111110100010000100011110100001101",
            "111000011111010111010100100010001001",
            "011101011001101011001100000011011100",
            "010010100011100001100101111010101101",
            "000110111010000000110111110011010101",
            "000010101110100111110000001010011111",
            "011000010000111001111111000011101100",
            "110010011010010100010000111111101010",
            "101000011011000110100010010111110110",
            "111010001001101100111100110001001001",
            "101101100001011000100100101011101110"
        };

        public const string PHONETIC = "Alfa, Bravo, Charlie, Delta, Echo, Foxtrot, Golf, Hotel, India, Juliett, Kilo, Lima, Mike, November, Oscar, Papa, Quebec, Romeo, Sierra, Tango, Uniform, Victor, Whiskey, X-ray, Yankee, Zulu";

        public const string ROTOR_LABELS = "AB-CDE-FGH-IJ-KL-MN-OP-QR-STU-VWX-YZ";

        public const string HEX = "0123456789ABCDEF";
        public const string HEX_SUBS = "QWERTYUIOPASDFGH";

        public static List<string> Moves;
        public static List<string> PhoneticValues;
        public static List<string> MonthLabels;
    }
}
