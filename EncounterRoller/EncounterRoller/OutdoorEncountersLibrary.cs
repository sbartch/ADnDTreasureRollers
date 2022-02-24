using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EncounterRoller
{
    public class OutdoorEncountersLibrary
    {
        public List<OutdoorEncounterBlob> OutdoorEncounterList { get; set; }

        public IEnumerable<OutdoorEncounterClass> OutdoorEncounters
        {
            get
            {
                var retVal = new List<OutdoorEncounterClass>();
                foreach (var oeb in OutdoorEncounterList)
                    retVal.Add(oeb.Encounters);
                return retVal;
            }
        }

        public List<string> Regions
        {
            get
            {
                var retVal = new List<string>();
                foreach (var oe in OutdoorEncounters)
                {
                    if (!retVal.Contains(oe.Region))
                        retVal.Add(oe.Region);
                }
                return retVal;
            }
        }

        public List<string> Terrains( string region )
        {
            var retVal = new List<string>();
            foreach( var oe in OutdoorEncounters.Where(x => x.Region == region))
            {
                retVal.Add(oe.Terrain);
            }
            return retVal;
        }

        public OutdoorEncounterClass EncountersByRegionAndTerrain(string region, string terrain)
        {
            return OutdoorEncounters.FirstOrDefault(x => x.Region == region && x.Terrain == terrain);
        }
    }

    public class OutdoorEncountersDiskAccessClass
    {
        [JsonProperty(PropertyName = "OutdoorEncounterClasses")]
        public List<OutdoorEncounterClass> OutdoorEncounterList { get; set; }
    }


    public class Utilities
    {
        public static List<OutdoorEncounterBlob> LoadOutdoorEncounterLibraries()
        {
            var returnLibraries = new List<OutdoorEncounterBlob>();

            var files = Directory.GetFiles(@".\EncounterJSONs\","*.json");
            foreach (var file in files)
            {
                var loadedLibrary = JsonConvert.DeserializeObject<OutdoorEncountersDiskAccessClass>(File.ReadAllText(file));
                if (loadedLibrary.OutdoorEncounterList.Any())
                {
                    foreach (var oe in loadedLibrary.OutdoorEncounterList)
                    {
                        var retBlob = new OutdoorEncounterBlob();
                        retBlob.Encounters = oe;
                        retBlob.SourceFileName = file;
                        var newCList = retBlob.Encounters.Creatures.OrderBy(x => x.Name).ToList();
                        retBlob.Encounters.Creatures = (newCList);
                        returnLibraries.Add(retBlob);
                    }
                }
            }
            return returnLibraries;
        }

        public static void WriteOutdoorEncountersLibraries()
        {

        }
    }
}
