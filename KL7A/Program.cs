using System;
using System.IO;
using System.Reflection;
using KL7A.Configuration;
using KL7A.DataAccess;
using KL7A.Utility;

namespace KL7A
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //YearlySettings ys = Repository.ReadFromDb(8);
            //IoUtil.SaveYearlyFiles(ys, "..\\..\\Year2017");

            //string wiringFileName = "..\\..\\Wiring.xml";
            //string settingFileName = "..\\..\\Settings.xml";
            //ys.Save(settingFileName);

            //Settings s = ys.Today;
            //Wiring.Current = s.ParentSettings.Wiring;

            //Console.WriteLine(s.ToString().Trim());
            //Console.WriteLine();

            string plainText = @"We intend to begin on the first of February unrestricted submarine warfare. We shall endeavor in spite of this to keep the United States of America neutral. In the event of this not succeeding, we make Mexico a proposal of alliance on the following basis: make war together, make peace together, generous financial support and an understanding on our part that Mexico is to reconquer the lost territory in Texas, New Mexico, and Arizona. The settlement in detail is left to you. You will inform the President of the above most secretly as soon as the outbreak of war with the United States of America is certain and add the suggestion that he should, on his own initiative, invite Japan to immediate adherence and at the same time mediate between Japan and ourselves. Please call the President's attention to the fact that the ruthless employment of our submarines now offers the prospect of compelling England in a few months to make peace. Signed, ZIMMERMANN";

            ACP127 acp = new ACP127();

            acp.IsPlaindress = true;
            acp.TransmittingStationCallSign = "3AV";
            acp.SerialNumber = 1;
            acp.DestinationRoutingIndicators = "UHEHCMD";
            acp.SourceRoutingIndicators = "UHEACVA";
            acp.Timestamp = DateTime.UtcNow;
            acp.SecurityWarning = SecurityWarning.ZNY_TTTTT;
            acp.Precedence = Precedence.O_Immediate;
            acp.OriginatorPlainAddress = "ARMY MARS STATION AAR3AV";
            acp.RecipientPlainAddress = "UHEUCMD/ARMY MARS MARYLAND DIRECTOR";
            acp.Classification = Classification.TopSecret;
            acp.MessageBody = Utility.Formatting.FormatMessage(plainText.ToUpper(), 60);

            Message msg = new Message(8);
            msg.MessageKey = "VQXJW";

            Console.WriteLine(msg.ToString());
            Console.WriteLine();

            Console.WriteLine("Message Key");
            Console.WriteLine(msg.MessageKey);
            Console.WriteLine();

            Console.WriteLine("Plain Text");
            Console.WriteLine(acp.Paginated);

            //string best = msg.FindBestKey(acp.Paginated, "VQ");

            //Console.WriteLine(plainTextFormatted);
            //Console.WriteLine();

            string cipherText = msg.Encipher(acp.Paginated);

            acp.MessageBody = cipherText;
            acp.GroupCount = msg.GroupCount;
            acp.Classification = Classification.Unclassified;
            acp.IsPlaindress = false;
            acp.SecurityWarning = SecurityWarning.ZNR_UUUUU;
            acp.OpSigs = msg.Indicator;
            acp.SerialNumber = 2;

            Console.WriteLine("Cipher text");
            Console.WriteLine(cipherText);
            Console.WriteLine();

            Console.WriteLine("Enciphered message text");
            Console.WriteLine(acp.Paginated);
            Console.WriteLine();

            Console.WriteLine("Groups");
            Console.WriteLine(msg.GroupCount);
            Console.WriteLine();

            Console.WriteLine("Character Distribution");
            Console.WriteLine(msg.CharacterDistribution.Trim());
            Console.WriteLine();

            Console.WriteLine("Histogram");
            Console.WriteLine(msg.Histogram.Trim());
            Console.WriteLine();

            Console.WriteLine("Statistics");
            Console.WriteLine(msg.Statistics.Trim());
            Console.WriteLine();

            Console.WriteLine("Rotor Move Counts");
            Console.WriteLine(msg.RotorMoveCounts.Trim());
            Console.WriteLine();

            Console.WriteLine("Reflex Counts");
            Console.WriteLine(msg.ReflexCounts.Trim());
            Console.WriteLine();

            Console.WriteLine("Deciphered Text");
            string decipherText = msg.Decipher(cipherText);
            Console.WriteLine(decipherText);
            Console.WriteLine();

            Console.WriteLine("Press any key...");
            Console.ReadLine();
        }
    }
}
