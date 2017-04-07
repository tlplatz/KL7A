using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KL7A.Utility
{
    /*      
        Every wire starts from a different entry contact.

        Every wire goes to a different exit contact.

        Every wire displaces the signal, between the entry and exit contacts,
        by a different amount.
      
        Even # of contacts, 1 possible combination is excluded and 2 are duplicated
        Max number of unique contacts is n - 1     
     */

    internal class IntervalWiring
    {
        public static int CONTACT_COUNT = Constants.CONTACT_COUNT;
        public static string ALPHABET = Constants.ALPHANUMERIC;

        public List<ContactInstance> ComputeRotorSet(int count)
        {
            List<ContactInstance> instances = new List<ContactInstance>();
            while (instances.Count < count * 10)
            {
                instances.AddRange(Compute(ALPHABET));
                Console.WriteLine(instances.Count);
            }

            List<Tuple<ContactInstance, int>> values = new List<Tuple<ContactInstance, int>>();
            foreach (var item in instances)
            {
                values.Add(new Tuple<ContactInstance, int>(item, RandomUtil._rand.Next()));
            }

            return values.OrderBy(v => v.Item2).Select(v => v.Item1).Take(count).ToList();
        }

        private List<ContactInstance> Compute(string alpha)
        {
            var foo = InnerCompute(alpha);
            while (foo.Max(f => f.UniqueContactCounts.Count()) != alpha.Length - 1)
            {
                foo = InnerCompute(alpha);
            }
            return foo;
        }
        private List<ContactInstance> InnerCompute(string alpha)
        {
            ALPHABET = alpha;
            CONTACT_COUNT = alpha.Length;

            Cell[,] _array = new Cell[CONTACT_COUNT, CONTACT_COUNT];
            List<Cell> _cells = new List<Cell>();
            List<int> occupiedDiagonals = new List<int>();
            List<int> occupiedRows = new List<int>();
            List<int> occupiedColumns = new List<int>();

            for (int y = 0; y < CONTACT_COUNT; y++)
            {
                for (int x = 0; x < CONTACT_COUNT; x++)
                {
                    _array[x, y] = new Cell { X = x, Y = y, Flag = false, Index = 0 };
                    _cells.Add(_array[x, y]);
                }
            }

            for (int diag = 0; diag < CONTACT_COUNT; diag++)
            {
                int x = diag;
                int y = 0;

                for (int z = 0; z < CONTACT_COUNT; z++)
                {
                    int xPosition = x + z;
                    int yPosition = y - z;

                    if (xPosition >= CONTACT_COUNT) xPosition -= CONTACT_COUNT;
                    if (yPosition < 0) yPosition += CONTACT_COUNT;

                    _array[xPosition, yPosition].Diagonal = diag;
                }
            }

            //A flag indicates where the A starts in the diagonal alphabet
            //There must be only 1 per diagonal, column and row, when all
            //flags are set, there will be no more available cells
            List<Cell> availableCells = _cells.Where(c => !occupiedDiagonals.Contains(c.Diagonal) && !occupiedRows.Contains(c.Y) && !occupiedColumns.Contains(c.X)).ToList();
            while (availableCells.Any())
            {
                Cell c = availableCells[RandomUtil._rand.Next(availableCells.Count)];

                occupiedDiagonals.Add(c.Diagonal);
                occupiedRows.Add(c.Y);
                occupiedColumns.Add(c.X);

                c.Flag = true;

                availableCells = _cells.Where(cell => !occupiedDiagonals.Contains(cell.Diagonal) && !occupiedRows.Contains(cell.Y) && !occupiedColumns.Contains(cell.X)).ToList();
            }

            while (_cells.Any(c => c.Flag))
            {
                Cell cFirst = _cells.First(c => c.Flag);

                int x = cFirst.X;
                int y = cFirst.Y;

                for (int i = 0; i < CONTACT_COUNT; i++)
                {
                    int xPosition = x + i;
                    int yPosition = y - i;

                    if (xPosition >= CONTACT_COUNT) xPosition -= CONTACT_COUNT;
                    if (yPosition < 0) yPosition += CONTACT_COUNT;

                    _array[xPosition, yPosition].Index = i;
                    _array[xPosition, yPosition].Letter = ALPHABET[i].ToString();
                }

                cFirst.Flag = false;
            }

            List<string> rowMissingCharacters = new List<string>();

            for (int i = 0; i < CONTACT_COUNT; i++)
            {
                List<string> letters = new List<string>(ALPHABET.Select(c => c.ToString()));
                foreach (Cell c in _cells.Where(cell => cell.Y == i))
                {
                    letters.Remove(c.Letter);
                }
                rowMissingCharacters.Add(string.Concat(letters));
            }

            for (int i = 0; i < CONTACT_COUNT; i++)
            {
                List<Cell> empties = _cells.Where(c => c.Y == i && c.Letter == null).ToList();
                for (int j = 0; j < rowMissingCharacters[j].Length; j++)
                {
                    empties[j].Letter = rowMissingCharacters[i][j].ToString();
                    empties[j].Index = ALPHABET.IndexOf(rowMissingCharacters[i][j]);
                }
            }

            List<List<WiringContact>> contactList = new List<List<WiringContact>>();

            for (int y = 0; y < CONTACT_COUNT; y++)
            {
                contactList.Add(new List<WiringContact>());

                for (int x = 0; x < CONTACT_COUNT; x++)
                {
                    contactList[y].Add(new WiringContact { WireIn = x, WireOut = _array[x, y].Index });
                }
            }

            List<ContactInstance> instances = new List<ContactInstance>();
            foreach (var item in contactList)
            {
                instances.Add(new ContactInstance(item));
            }
            instances.Sort((i1, i2) => i1.UniqueContactCounts.Count.CompareTo(i2.UniqueContactCounts.Count));

            return instances;
        }
    }

    internal class ContactInstance
    {
        public ContactInstance(IEnumerable<WiringContact> data)
        {
            Contacts = new List<WiringContact>(data);
            UniqueContactCounts = new List<Tuple<int, int>>(Contacts.GroupBy(c => c.Difference).Select(c => new Tuple<int, int>(c.Key, c.Count())).ToList());
        }

        public List<WiringContact> Contacts { get; set; }
        public List<Tuple<int, int>> UniqueContactCounts { get; set; }

        public override string ToString()
        {
            return string.Concat(Contacts.OrderBy(c => c.WireIn).Select(c => IntervalWiring.ALPHABET[c.WireOut]));
        }

        public string Raw
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (WiringContact wc in Contacts)
                {
                    sb.AppendLine(string.Format("{0}\t{1}\t{2}", wc.WireIn, wc.WireOut, wc.Difference));
                }

                return sb.ToString();
            }
        }

        public int[] ContactWiring
        {
            get
            {
                return Contacts.Select(c => c.WireOut).ToArray();
            }
        }
    }

    internal class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Flag { get; set; }
        public int Index { get; set; }
        public string Letter { get; set; }
        public int Diagonal { get; set; }
    }

    internal class WiringContact
    {
        public int WireIn { get; set; }
        public int WireOut { get; set; }
        public int Difference
        {
            get
            {
                int result = WireIn - WireOut;
                if (result < 0) result += IntervalWiring.CONTACT_COUNT;
                return result;
            }
            set
            {
                int result = WireIn + value;
                if (result > 25) result -= IntervalWiring.CONTACT_COUNT;
                WireOut = result;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} ({2})", WireIn, WireOut, Difference);
        }
    }

    internal class Difference
    {
        public char Value { get; set; }
        public int OrdinalPosition { get; set; }
        public int CharacterIndex { get; set; }
        public int Diff { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Value, Diff);
        }

        public static List<Difference> ComputeDifferences(string input)
        {
            List<Difference> result = new List<Difference>();

            for (int i = 0; i < input.Length; i++)
            {
                result.Add(new Difference
                {
                    Value = input[i],
                    OrdinalPosition = i,
                    CharacterIndex = Constants.ALPHANUMERIC.IndexOf(input[i])
                });
            }

            result.ForEach(d =>
            {
                int x = d.CharacterIndex - d.OrdinalPosition;
                if (x < 0) x += Constants.CONTACT_COUNT;
                d.Diff = x;
            });

            return result;
        }
        public static int[] DifferenceCounts(string input)
        {
            var diffs = ComputeDifferences(input);
            int[] result = new int[Constants.CONTACT_COUNT];
            foreach (var item in diffs)
            {
                result[item.Diff]++;
            }
            return result;
        }
        public static bool Validate(string input)
        {
            var counts = DifferenceCounts(input);

            if (counts.Length == 36)
            {
                if (counts.Where(c => c == 0).Count() == 1)
                {
                    if (counts.Where(c => c == 2).Count() == 1)
                    {
                        if (counts.Where(c => c == 1).Count() == 34)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
