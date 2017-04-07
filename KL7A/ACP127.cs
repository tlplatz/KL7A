using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KL7A
{
    public class ACP127
    {
        public string TransmittingStationCallSign { get; set; }
        public int SerialNumber { get; set; }

        public string DestinationRoutingIndicators { get; set; }

        public string SourceRoutingIndicators { get; set; }
        public DateTime Timestamp { get; set; }

        public SecurityWarning SecurityWarning { get; set; }
        public string TransmissionInstructions { get; set; }

        public Precedence Precedence { get; set; }
        public string OpSigs { get; set; }

        public string OriginatorPlainAddress { get; set; }
        public string RecipientPlainAddress { get; set; }
        public string InfoAddress { get; set; }
        public string ExemptAddress { get; set; }

        public int GroupCount { get; set; }

        public Classification Classification { get; set; }

        public string MessageBody { get; set; }
        public bool IsPlaindress { get; set; }

        public string Line1
        { 
            get
            {
                return string.Format("VZCZC{0}{1:00#}", TransmittingStationCallSign, SerialNumber).ToUpper();
            }
        }
        public string Line2
        {
            get
            {
                return Precedence.ToString()[0].ToString() + Precedence.ToString()[0].ToString() + " " +  DestinationRoutingIndicators.ToUpper();
            }
        }
        public string Line3
        {
            get
            {
                Timestamp = DateTime.UtcNow;
                int julianDayNumber = (Timestamp - new DateTime(Timestamp.Year, 1, 1)).Days;
                return string.Format("DE {0} #{1:000#} {2:00#}{3:HHmm}", SourceRoutingIndicators, SerialNumber, julianDayNumber, Timestamp).ToUpper();
            }
        }
        public string Line4
        {
            get
            {
                return string.Format("{0} {1}", SecurityWarning.ToString().Replace("_", " "), TransmissionInstructions).ToUpper();
            }
        }
        public string Line5
        {
            get
            {
                return string.Format("{0} {1:dd}{1:HHmm}Z {1:MMM} {1:yyyy} {2}", Precedence.ToString()[0], Timestamp, OpSigs).ToUpper();
            }
        }
        public string Line6
        {
            get
            {
                return "FM " + OriginatorPlainAddress;
            }
        }
        public string Line7
        {
            get
            {
                return "TO " + RecipientPlainAddress;
            }
        }
        public string Line8
        {
            get
            {
                if (string.IsNullOrEmpty(InfoAddress)) return null;
                return "INFO " + InfoAddress;
            }
        }
        public string Line9
        {
            get
            {
                if (string.IsNullOrEmpty(ExemptAddress)) return null;
                return "XMT "  + ExemptAddress;
            }
        }
        public string Line10
        {
            get
            {
                if (IsPlaindress) return null;
                return "GR " + GroupCount.ToString();
            }
        }
        public string Line11
        {
            get
            {
                return "BT";
            }
        }
        public string Line12
        {
            get
            {
                string classification;
                switch (this.Classification)
                {
                    case Classification.Unclassified:
                        classification = "UNCLAS";
                        break;
                    case Classification.Confidential:
                        classification = "C O N F I D E N T I A L";
                        break;
                    case Classification.Secret:
                        classification = "S E C R E T";
                        break;
                    case Classification.TopSecret:
                        classification = "T O P S E C R E T";
                        break;
                    default:
                        classification = "UNCLAS";
                        break;
                };

                if (IsPlaindress)
                    return string.Join(Environment.NewLine, classification, MessageBody);

                return MessageBody;

            }
        }
        public string Line13
        {
            get
            {
                return "BT";
            }
        }
        public string Line14
        {
            get
            {
                return string.Format("#{0:000#}", SerialNumber);
            }
        }
        public string Line15
        {
            get
            {
                return
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    System.Environment.NewLine +
                    System.Environment.NewLine;

            }
        }
        public string Line16
        {
            get
            {
                return "NNNN";
            }
        }

        public string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                sb.AppendLine(Line1);
                sb.AppendLine(Line2);
                sb.AppendLine(Line3);
                sb.AppendLine(Line4);
                sb.AppendLine(Line5);

                if (IsPlaindress)
                {
                    if (!string.IsNullOrEmpty(Line6))
                        sb.AppendLine(Line6);

                    if (!string.IsNullOrEmpty(Line7))
                        sb.AppendLine(Line7);

                    if (!string.IsNullOrEmpty(Line8))
                        sb.AppendLine(Line8);

                    if (!string.IsNullOrEmpty(Line9))
                        sb.AppendLine(Line9);
                }

                if (!IsPlaindress)
                    sb.AppendLine(Line10);

                sb.AppendLine(Line11);
                sb.AppendLine(Line12);
                sb.AppendLine(Line13);
                sb.AppendLine(Line14);
                sb.AppendLine(Line15);
                sb.AppendLine(Line16);

                return sb.ToString();

            }
        }

        public string MessageHeaderForPagination
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Line1);
                sb.AppendLine(Line2);
                sb.AppendLine(Line3);
                sb.AppendLine(Line4);

                return sb.ToString();

            }
        }
        public string MessageBodyForPagination
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Line5);

                if (IsPlaindress)
                {
                    if (!string.IsNullOrEmpty(Line6))
                        sb.AppendLine(Line6);

                    if (!string.IsNullOrEmpty(Line7))
                        sb.AppendLine(Line7);

                    if (!string.IsNullOrEmpty(Line8))
                        sb.AppendLine(Line8);

                    if (!string.IsNullOrEmpty(Line9))
                        sb.AppendLine(Line9);
                }

                if (!IsPlaindress)
                    sb.AppendLine(Line10);

                sb.AppendLine(Line11);
                sb.AppendLine(Line12);

                return sb.ToString();
            }
        }
        public string MessageFooterForPagination
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Line13);
                sb.AppendLine(Line14);
                sb.AppendLine(Line15);
                sb.AppendLine(Line16);

                return sb.ToString();
            }
        }

        public string PageHeading(int index)
        {
            //PAGE 2 UHXPAE 0002 UNCLAS

            string classification;
            switch (this.Classification)
            {
                case Classification.Unclassified:
                    classification = "UNCLAS";
                    break;
                case Classification.Confidential:
                    classification = "C O N F I D E N T I A L";
                    break;
                case Classification.Secret:
                    classification = "S E C R E T";
                    break;
                case Classification.TopSecret:
                    classification = "T O P S E C R E T";
                    break;
                default:
                    classification = "UNCLAS";
                    break;
            };

            return string.Format("PAGE {0} {1} {2:0000} {3}", index, SourceRoutingIndicators, SerialNumber, classification);
        }

        public List<string> Lines
        {
            get
            {
                return MessageBodyForPagination.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            }
        }
        public List<List<string>> Pages
        {
            get
            {
                int rowNumber = 0;

                List<List<string>> result = new List<List<string>>();

                foreach(string s in Lines)
                {
                    if(rowNumber == 0)
                    {
                        result.Add(new List<string>());
                    }
                    result.Last().Add(s);
                    rowNumber++;
                    if(rowNumber == 20)
                    {
                        rowNumber = 0;
                    }
                }

                return result;
            }
        }

        public string Paginated
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(MessageHeaderForPagination);

                for(int i=0; i<Pages.Count; i++)
                {
                    if (i >= 1)
                    {
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.AppendLine();

                        sb.AppendLine(PageHeading(i + 1));

                        sb.AppendLine();
                    }

                    foreach(string s in Pages[i])
                    {
                        sb.AppendLine(s.Trim());
                    }
                }
                
                return sb.ToString().Trim() + Environment.NewLine + MessageFooterForPagination;
            }            
        }

    }

    public enum SecurityWarning
    {
        ZNR_UUUUU,                  //unclassified
        ZNY_EEEEE,                  //do not forward unless encrypted
        ZNY_CCCCC,                  //classified
        ZNY_SSSSS,                  //secret
        ZNY_TTTTT                   //top secret
    }

    public enum Precedence
    {
        Z_Flash,
        O_Immediate,
        P_Priority,
        R_Routine
    }

    public enum Classification
    {
        Unclassified,
        Confidential,
        Secret,
        TopSecret
    }

}
