using System.Collections.Generic;
using System.Linq;

namespace KL7A.Mechanical
{
    internal class FixedRotor
    {
        protected List<Contact> _contacts = new List<Contact>();
        protected string _initializer;

        public FixedRotor(string name, string init)
        {
            Name = name;
            _initializer = init;
            Initialize();
        }

        public string Name { get; protected set; }

        public virtual int WireRight(int left)
        {
            return _contacts[left].WireRight;
        }
        public virtual int WireLeft(int right)
        {
            return _contacts.FirstOrDefault(c => c.WireRight == right).WireLeft;
        }

        protected void Initialize()
        {
            _contacts.Clear();

            for (int i = 0; i < Constants.ALPHANUMERIC.Length; i++)
            {
                _contacts.Add(new Contact { WireLeft = i, WireRight = Constants.ALPHANUMERIC.IndexOf(_initializer[i]) });
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
