using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace Reformat
{
    public class MagicItemInfo
    {
        public string RollLow { get; set; }
        public string RollHigh { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string GPValue { get; set; }
    }
    public class ReformFiles
    {
        public ReformFiles()
        {
            bool leadWithComma = false;
            DirectoryInfo d = new DirectoryInfo(@".\Data");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.csv"); //Getting Text files
            string str = "";
            foreach (FileInfo infile in Files)
            {
                string line;
                string outFileName = Path.GetFileNameWithoutExtension(infile.FullName) + ".json";
                System.IO.StreamReader infilestream =  new System.IO.StreamReader(infile.FullName);
                System.IO.StreamWriter outfilestream = new System.IO.StreamWriter(infile.DirectoryName + "\\" + outFileName);
                outfilestream.WriteLine("[");
                while ((line = infilestream.ReadLine()) != null)
                {
                    line = Regex.Replace(line, @"[^\u0000-\u007F]+", string.Empty);
                    MagicItemInfo mii = Reformat(line);
                    if (leadWithComma)
                        outfilestream.WriteLine(",");
                    outfilestream.WriteLine("{\r\n\t\"LowRoll\":" + mii.RollLow + ",");
                    outfilestream.WriteLine("\t\"HighRoll\":" + mii.RollHigh + ",");
                    outfilestream.WriteLine("\t\"Name\":\""+ mii.Description + "\",");
                    outfilestream.WriteLine("\t\"Experience\":\"" + mii.Experience + "\",");
                    outfilestream.WriteLine("\t\"Price\":\"" + mii.GPValue+ "\"");
                    outfilestream.Write("}");
                    leadWithComma = true;
                }
                outfilestream.WriteLine("\r\n]");
                outfilestream.Close();
            }
        }
        private MagicItemInfo Reformat(string csv)
        {
            string rollLow = "", rollHigh = "", description = "", experience = "", gpValue = "";

            TextFieldParser parser = new TextFieldParser(new StringReader(csv));

            // You can also read from a file
            // TextFieldParser parser = new TextFieldParser("mycsvfile.csv");

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] fields;

            while (!parser.EndOfData)
            {
                fields = parser.ReadFields();
                int i = 0;
                foreach (string field in fields)
                {
                    switch (i)
                    {
                        case 0:
                            ParseRolls(field, out rollLow, out rollHigh);
                            break;
                        case 1:
                            description = field;
                            break;
                        case 2:
                            experience = field;
                            break;
                        case 3:
                            gpValue = field;
                            break;
                    }
                    i++;
                }
            }
            parser.Close();
            return new MagicItemInfo() { RollLow = rollLow, RollHigh = rollHigh, Description = description, Experience = experience, GPValue = gpValue };
        }
        private void ParseRolls(string range, out string rollLow, out string rollHigh)
        {
            range = range.Trim();
            range = RemoveWhitespace(range);
            string[] lowhi = range.Split('-');
            rollLow = lowhi[0];
            rollHigh = lowhi.Length > 1 ? lowhi[1] : rollLow;

            return;
        }
        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
