using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace GemRoller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private float _totalValue = 0;
        private int _gemCount = 0;
        private Random _rnd = new Random();
        public GemsValueLibrary _valueLibrary;
        public GemsLibrary _library;

        public ObservableCollection<VisibleGems> _myCollection = new ObservableCollection<VisibleGems>();
        public ObservableCollection<VisibleGems> MyCollection { get { return _myCollection; } set { _myCollection = value; OnPropertyChanged("MyCollection"); } }
        public float TotalValue
        {
            get { return _totalValue; }
            set
            {
                _totalValue = value;
                totalValuelbl.Content = "Value: " + _totalValue.ToString()+" gp";
            }
        }

        public int GemCount
        {
            get { return _gemCount; }
            set
            {
                _gemCount = value;
                totalGemCountlbl.Content = "Total Gems: " + _gemCount;
            }
        }

        public GemsLibrary Library { get => _library; set => _library = value; }
        public GemsValueLibrary ValueLibrary { get => _valueLibrary; set => _valueLibrary = value; }
        public MainWindow()
        {
            InitializeComponent();
            Library = JsonConvert.DeserializeObject< GemsLibrary > (File.ReadAllText("Gems.json"));
            ValueLibrary = JsonConvert.DeserializeObject<GemsValueLibrary>(File.ReadAllText("Gems.json"));
        }

        private void RollGems_Click(object sender, RoutedEventArgs e)
        {
            MyCollection.Clear();
            List<MyGems> myGemsList = new List<MyGems>();
            int totalGems = 0;
            int lowGroup = 0;
            int highGroup = 0;

            Int32.TryParse(TotalGemCount.Text, out totalGems);
            Int32.TryParse(LowGroupCount.Text, out lowGroup);
            Int32.TryParse(HighGroupCount.Text, out highGroup);
            if(totalGems<=0 || lowGroup<=0 || highGroup<=0 || lowGroup>highGroup)
            {
                return;
            }
            do
            {
                int nextGroupCount = _rnd.Next(lowGroup, highGroup + 1);
                if (nextGroupCount > totalGems)
                    nextGroupCount = totalGems;

                int gemRoll = _rnd.Next(1, 101); // Percentile Throw for 1 to 100
                int gemClass = -1;
                foreach(var gemsClass in Library.Gems)
                {
                    if (gemRoll >= gemsClass.LowRoll && gemRoll <= gemsClass.HighRoll)
                        gemClass = gemsClass.Class;
                }

                int gemIndex = _rnd.Next(Library.Gems[gemClass].GemDetails.Count);

                var mygems = myGemsList.FirstOrDefault(o => o.Class == gemClass && o.GemIndex == gemIndex);
                if (mygems != null)
                    mygems.Count += nextGroupCount;
                else
                {
                    MyGems newGem = new MyGems();
                    newGem.Class = gemClass;
                    newGem.GemIndex = gemIndex;
                    newGem.Count = nextGroupCount;
                    myGemsList.Add(newGem);
                }

                totalGems -= nextGroupCount;

            } while (totalGems > 0);
            // Now, take the gems, and do the "extra roll" on them, which can change value of
            // individual gems
            myGemsList = ExtraRoll(myGemsList);

            TotalValue = 0;
            GemCount = 0;
            foreach (var gemSet in myGemsList)
            {
                VisibleGems vg = new VisibleGems();
                vg.GemCount = gemSet.Count;
                GemCount += gemSet.Count;
                vg.GemName = Library.Gems[gemSet.Class].GemDetails[gemSet.GemIndex].Name;
                if (gemSet.SpecialValue > 0)
                {
                    vg.GemValue = gemSet.SpecialValue;
                    vg.SpecialValue = "*";
                }
                else
                {
                    vg.GemValue = Library.Gems[gemSet.Class].BaseValue;
                }
                MyCollection.Add(vg);
                TotalValue += vg.TotalValue;
            }
        }
        private List<MyGems> ExtraRoll(List<MyGems> gemsList)
        {
            int i;
            List<MyGems> extraValueGems = new List<MyGems>();
            List<MyGems> originalGems = gemsList;
            foreach(var gemSet in gemsList)
            {
                int setCount = gemSet.Count;
                for(i=0;i< setCount; i++)
                {
                    // We'll use Value Index throughout this routine for EACH gem...
                    int valueIndex = ValueIndex(Library.Gems[gemSet.Class].BaseValue);
                    int rollStep = 0;
                    int lastRoll = 0;
                    bool rollAgain = true;
                    MyGems newGem = new MyGems();
                    while (rollAgain)
                    {
                        bool reRoll = false;
                        int roll;
                        do
                        {
                            reRoll = false;
                            roll = _rnd.Next(1, 10);
                            if(lastRoll!=0)
                            {
                                switch(lastRoll)
                                {
                                    case 1:
                                        if (roll > 8)
                                            reRoll = true;
                                        break;
                                    case 10:
                                        if(roll == 1)
                                            reRoll = true;
                                        break;
                                }
                            }
                        } while (reRoll);

                        if (roll >= 4 && roll <= 8)
                            rollAgain = false;
                        else
                        {
                            // This is the Gem that we're adjusting....
                            if(newGem.Count<1)
                            {
                                // Special Gem....remove it from the main list...
                                lastRoll = roll;
                                gemSet.Count--;
                                newGem.Class = gemSet.Class;
                                newGem.Count = 1;
                                newGem.GemIndex = gemSet.GemIndex;
                                newGem.SpecialValue = _valueLibrary.GemsValues[valueIndex].Value;
                                extraValueGems.Add(newGem);
                            }

                            if (roll == 1 )
                            {
                                rollStep++;
                                valueIndex += 1;
                                if (valueIndex < _valueLibrary.GemsValues.Count && rollStep < 7)
                                    newGem.SpecialValue = _valueLibrary.GemsValues[valueIndex].Value;
                                rollAgain = (rollStep < 7);
                            }
                            else if(roll==10)
                            {
                                rollStep++;
                                valueIndex -= 1;
                                if(valueIndex>0)
                                    newGem.SpecialValue = _valueLibrary.GemsValues[valueIndex].Value;
                                rollAgain = (rollStep < 7);
                            }
                            else if(roll==2)
                            {
                                rollAgain = false;
                                newGem.SpecialValue *= 2;
                            }
                            else if(roll==3)
                            {
                                rollAgain = false;
                                float multiPlier = 1.0f + ((float)_rnd.Next(1,6)/10.0f);
                                newGem.SpecialValue *= multiPlier;
                            }
                            else if(roll==9)
                            {
                                rollAgain = false;
                                float multiPlier = 1.0f - ((float)_rnd.Next(1, 4) / 10.0f);
                                newGem.SpecialValue *= multiPlier;
                            }
                        }
                    }
                }

            }
            if(extraValueGems.Count>0)
            {
                // Need to update some things....
                gemsList = new List<MyGems>();
                foreach(var myGem in originalGems)
                {
                    if (myGem.Count > 0)
                    {
                        var mygems = gemsList.FirstOrDefault(o => o.Class ==myGem.Class && o.GemIndex == myGem.GemIndex);
                        if (mygems == null)
                            gemsList.Add(myGem);
                        else
                            mygems.Count += myGem.Count;
                    }
                }
                foreach(var myGem in extraValueGems)
                {
                    var mygems = gemsList.FirstOrDefault(o => o.Class == myGem.Class && o.GemIndex == myGem.GemIndex && o.SpecialValue==myGem.SpecialValue);
                    if (mygems == null)
                        gemsList.Add(myGem);
                    else
                        mygems.Count += myGem.Count;
                }

            }
            gemsList.Sort(delegate(MyGems a, MyGems b)
            {
                int r = a.Class.CompareTo(b.Class);
                if (r == 0)
                    r = a.GemIndex.CompareTo(b.GemIndex);
                if (r == 0)
                    r = a.SpecialValue.CompareTo(b.SpecialValue);
                return r;
            }
            );

            return gemsList;
        }
        private int ValueIndex(int value)
        {
            int retIndex = 0;
            foreach(var gemValue in _valueLibrary.GemsValues)
            {
                if (gemValue.Value == value)
                    return retIndex;
                retIndex++;
            }
            throw new ArgumentException();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler(this, new PropertyChangedEventArgs(name));
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            string buffer="";
            foreach(var item in gemListView.SelectedItems  )
            {
                VisibleGems gem = (VisibleGems)item;
                string sOut = string.Format("{0} X {1} @ {2}gp\n",  gem.GemCount, gem.GemName, gem.GemValue);
                buffer += sOut;
            }
            Clipboard.SetText(buffer);
        }

        private void gemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCopy.IsEnabled = (gemListView.SelectedItems.Count > 0);
        }
    }
}
