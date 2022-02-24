using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EncounterRoller
{
    public class OutdoorEncounterBlob
    {
        public OutdoorEncounterClass Encounters { get; set; }
        public string SourceFileName { get; set; }
    }
    public class OutdoorEncounterClass
    {
        public string Region { get; set; }
        public string Terrain { get; set; }
        public List<Creatures> Creatures { get; set; }

        public IEnumerable<Creatures> CreaturesByFrequency(string frequency)
        {
            return Creatures.Where(x => x.Frequency == frequency).OrderBy(x => x.Name);
        }
        public IEnumerable<Creatures> CreaturesByPower(int power)
        {
            return Creatures.Where(x => x.PowerLevel == power).OrderByDescending(x => x.PowerLevel).ThenBy(x=>x.Name);
        }

    }
    public class Creatures
    {
        public string Name { get; set; }
        [JsonProperty(PropertyName = "Power Lvl")]
        public int PowerLevel;
        public string Source { get; set; }
    }
}
