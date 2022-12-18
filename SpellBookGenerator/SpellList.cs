using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellBookGenerator
{

    public class MasterLibrary
    {
        public string Name { get; set; }
        public List<SpellList> SpellList { get; set; }
    }

    public class SpellLibrary
    {
        public List<MasterLibrary> MasterLibrary { get; set; }
    }

    public class SpellList
    {
        public int Level { get; set; }
        public List<string> Spells { get; set; }
    }
}
