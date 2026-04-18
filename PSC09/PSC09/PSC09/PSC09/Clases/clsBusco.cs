using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC09
{
    public class cnn
    {
        public static string db = @"server=PCFUERTE; database=sistemaFacturacion; integrated security=true";
    }

    public class Item
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Item(string _name, int _value)
        {
            Name = _name; 
            Value = _value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
