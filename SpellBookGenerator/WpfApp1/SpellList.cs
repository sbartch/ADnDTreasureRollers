using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellBookGenerator
{
    public enum SpellClass
    { Offense=0, Defense, Other }

    public class Spell
    {
        public string Name { get; set; }
        public SpellClass Classification { get; set; }
    }
    public class SpellList
    {
        public int Level { get; set; }
        public List<Spell> Spells { get; set; }
    }

    public class SpellLibrary
    {
        public List<SpellList> SpellLists { get; set; }
    }
}
