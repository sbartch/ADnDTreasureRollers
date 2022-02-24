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

namespace MagicRoller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<VisibleMagicItem> _myMagicCollection = new ObservableCollection<VisibleMagicItem>();
        public ObservableCollection<VisibleMagicItem> MyMagicCollection { get { return _myMagicCollection; } set { _myMagicCollection = value; OnPropertyChanged("MyMagicCollection"); } }

        public ObservableCollection<VisibleMagicClass> _myMagicClassList = new ObservableCollection<VisibleMagicClass>();
        public ObservableCollection<VisibleMagicClass> MyMagicClassList { get { return _myMagicClassList; } set { _myMagicClassList = value; OnPropertyChanged("MyMagicClassList"); } }

        public RootObject Library { get => _library; set => _library = value; }

        private int _lowEndValue = 0;
        private int _highEndValue = 0;
               
        RootObject _library;
        
        private Random _rnd = new Random();
        public MainWindow()
        {
            InitializeComponent();
            Library = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText("MagicTables.json"));
            foreach (var magicClass in Library.UseCategories)
            {
                VisibleMagicClass vmc = new VisibleMagicClass();
                vmc.ClassName = magicClass.UseClassName;
                vmc.CategoryInfo = magicClass;
                MyMagicClassList.Add(vmc);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler(this, new PropertyChangedEventArgs(name));
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            foreach (var item in magicListView.SelectedItems)
            {
                VisibleMagicItem vi = (VisibleMagicItem)item;
                string sOut = string.Format("{0}\t{1}\t{2}\t{3}\n", vi.MagicClass, vi.MagicName, vi.MagicExperience, vi.MagicValue);
                buffer += sOut;
            }
            Clipboard.SetText(buffer);
        }

        private void magicListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCopy.IsEnabled = (magicListView.SelectedItems.Count > 0);
            ButtonCopyCSV.IsEnabled = (magicListView.SelectedItems.Count > 0);
        }

        private int ParseRollCount(string rollCountSource)
        {
            int rollCount = 1;


            // Simple Digit Field...return value
            if (!Int32.TryParse(rollCountSource, out rollCount))
            {
                // Maybe it's in the format "xDy+z" format...
                rollCountSource = rollCountSource.ToLower();
                if(rollCountSource.Contains("d"))
                {
                    int dieCount = 0;
                    int dieType = 0;
                    int dieAdj = 0;
                    char[] splitChar = { 'd', '+' };
                    string[] pieces = rollCountSource.Split(splitChar);

                    if (pieces.Length > 0)
                    {
                        Int32.TryParse(pieces[0], out dieCount);
                    }
                    if (pieces.Length > 1)
                    {
                        Int32.TryParse(pieces[1], out dieType);
                    }
                    if (pieces.Length > 2)
                    {
                        Int32.TryParse(pieces[2], out dieAdj);
                    }

                    for (int i = 0; i < dieCount; i++)
                    {
                        if(dieType>0)
                        {
                            int roll = _rnd.Next(1, dieType + 1);
                            rollCount += roll;
                        }
                    }
                        
                    rollCount += dieAdj;
                }
                else
                {
                    MessageBox.Show("Unrecognized roll count format: " + rollCountSource, "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return rollCount;
        }

        private void RollMagic_Click(object sender, RoutedEventArgs e)
        {
            int rollCount = 1;

            rollCount = ParseRollCount(TotalRollCount.Text);
            MyMagicCollection.Clear();

            for (int i=0;i<rollCount;)
            {
                VisibleMagicClass typeItem = MagicClass.SelectedItem as VisibleMagicClass;
                if (typeItem == null) return;
                int category = GetCategoryFromInfo(typeItem.CategoryInfo);
                int dieRoll = _rnd.Next(1,101);
                var magicTable = Library.MagicItems.FirstOrDefault(item =>item.Class == category );
                if(magicTable!=null)
                {
                    var magicItem = magicTable.ItemEntries.FirstOrDefault(item => dieRoll >= item.LowRoll && dieRoll <= item.HighRoll);
                    if(magicItem!=null && inPriceRange(category, magicItem))
                    {
                        i++;
                        VisibleMagicItem vmi = new VisibleMagicItem();
                        vmi.MagicName = magicItem.Name;
                        vmi.MagicExperience = magicItem.Experience;
                        vmi.MagicValue = magicItem.Price;
                        vmi.MagicClass = FindClassName(category);

                        bool found = false;
                        foreach(VisibleMagicItem eVMI in MyMagicCollection)
                        {
                            if(eVMI.MagicClass == vmi.MagicClass && eVMI.MagicName == vmi.MagicName)
                            {
                                eVMI.MagicCount++;
                                found = true;
                                break;
                            }
                        }
                        if(!found)
                            MyMagicCollection.Add(vmi);
                    }
                }
                else
                {
                    MessageBox.Show("Magic Class Load Issue!");
                }
            }
        }
        private bool inPriceRange(int category, ItemEntry magicItem)
        {
            int parsePrice = 0;
            if (_lowEndValue == _highEndValue && _lowEndValue == 0)
                return true;
            // SCROLLS!
            if(category == 1 && (magicItem.Experience.Contains("-") || magicItem.Price.Contains("variable")) )
            {
                int spellCount = 0;
                string[] getCount = magicItem.Name.Split(' ');
                
                if (getCount.Count()>= 1 && Int32.TryParse(getCount[0], out spellCount) && magicItem.Experience.Contains("-"))
                {
                    // we have a count of spells...get the range if we can...
                    string[] levels = magicItem.Experience.Split('-');
                    // NOTE Levles is a problem...because "multiclass" scrolls...but ignore extrta for now
                    if (levels.Count() >= 2)
                    {
                        int low, high;
                        if(levels.Count()>2)
                        {
                            // Multi class scroll, so we need to get levels[1] stripped a bit...
                            if(levels[1].Contains("or"))
                            {
                                string []levelExtra=levels[1].Split(' ');
                                if (levelExtra.Count() > 1)
                                    levels[1] = levelExtra[0];
                            }
                        }
                        try
                        {
                            low = Int32.Parse(levels[0]);
                            high = Int32.Parse(levels[1]);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error parsing scroll levels for " + magicItem.Name + " " + ex.Message);
                            return true;
                        }
                        parsePrice = 100 * spellCount * ((low + high) / 2 + 1);
                    }
                }
                else return true;//for now, protection scrolls return true...
            }
            else
            if(!Int32.TryParse(magicItem.Price,out parsePrice))
            {
                // Special price? Depends on what it is.
                // "Cursed" items could be available anywhere
                if (magicItem.Price.Contains("***"))
                    return true;
                if(magicItem.Price.Contains("-"))
                {
                    // This is a low to high range, and the low end must be in the shop's
                    // price range.
                    string[] prices = magicItem.Price.Split('-');
                    try
                    {
                        parsePrice = Int32.Parse(prices[0]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Price Parse failed for Item " + magicItem.Name + " " + ex.Message);
                        parsePrice = 0;
                        return true;
                    }
                }
            }
            
            if (parsePrice >= _lowEndValue && parsePrice <= _highEndValue)
                return true;
            return false;
        }
        private string FindClassName(int cat)
        {
            string retVal = "unknown";
            foreach(var useCategory in Library.UseCategories)
            {
                if(useCategory.ClassName)
                {
                    var temp = useCategory.UseClassClasses.FirstOrDefault(item => item.UseClass == cat);
                    if (temp != null)
                    {
                        retVal = useCategory.UseClassName;
                        break;
                    }
                }
            }
            return retVal;
        }
        private int GetCategoryFromInfo(UseCategory category)
        {
            int retVal = -1;
            int dieRoll;
            // If this is a Category Name - the table to use is one of an equal chance in the current list...
            if (category.ClassName)
            {
                dieRoll = _rnd.Next(category.UseClassClasses.Count);
                retVal = category.UseClassClasses[dieRoll].UseClass;
            }
            else
            {
                while(retVal==-1)
                {
                    dieRoll = _rnd.Next(1,101);
                    var lowHighIndex = ((bool)ingredientsButton.IsChecked) ? 0 : 1;
                    var testCategory = Library.MagicItems.FirstOrDefault(item => dieRoll>=item.LowRoll[lowHighIndex] && dieRoll<=item.HighRoll[lowHighIndex] );
                    if(testCategory!=null)
                    {
                        var value = category.UseClassClasses.FirstOrDefault(item => item.UseClass == testCategory.Class);
                        if (value != null)
                        {
                            retVal = value.UseClass;
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        private void ButtonCopyCSV_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            foreach (var item in magicListView.SelectedItems)
            {
                VisibleMagicItem vi = (VisibleMagicItem)item;
                string sOut = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"\n", vi.MagicClass, vi.MagicName,vi.MagicCount, vi.MagicExperience, vi.MagicValue);
                buffer += sOut;
            }
            Clipboard.SetText(buffer);
        }

        private void IngredientsButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ValuesGrid == null) return;
            ValuesGrid.IsEnabled = false;
            lowValueEntry.Text = "0";
            highValueEntry.Text = "1000";
        }

        private void LowEndButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ValuesGrid == null) return;
            ValuesGrid.IsEnabled = false;
            lowValueEntry.Text = "0";
            highValueEntry.Text = "2500";
        }

        private void MidRangeButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ValuesGrid == null) return;
            ValuesGrid.IsEnabled = false;
            lowValueEntry.Text = "0";
            highValueEntry.Text = "10000";
        }

        private void HighEndButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ValuesGrid == null) return;
            ValuesGrid.IsEnabled = false;
            lowValueEntry.Text = "0";
            highValueEntry.Text = "0";
        }

        private void ArbitraryButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ValuesGrid == null) return;
            ValuesGrid.IsEnabled = true;
        }

        private void LowValueEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!Int32.TryParse(lowValueEntry.Text, out _lowEndValue) || _lowEndValue<0)
            {
                _lowEndValue = 0;
                lowValueEntry.Text = "0";
            }
            RollMagic.IsEnabled = (_lowEndValue <= _highEndValue);
        }

        private void HighValueEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Int32.TryParse(highValueEntry.Text, out _highEndValue) || _highEndValue<0)
            {
                _highEndValue = 0;
                highValueEntry.Text = "0";
            }
            RollMagic.IsEnabled = (_lowEndValue <= _highEndValue);
        }
    }
}
