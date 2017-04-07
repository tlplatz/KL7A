namespace KL7A.Mechanical
{
    internal class Contact
    {
        public int WireLeft { get; set; }
        public int WireRight { get; set; }
        public bool Notch { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1} {2}", WireLeft, WireRight, Notch);
        }
    }
}
