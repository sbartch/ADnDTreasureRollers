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

            for (int i=0;i<rollCount;i++)
            {
                VisibleMagicClass typeItem = MagicClass.SelectedItem as VisibleMagicClass;
                if (typeItem == null) return;
                int category = GetCategoryFromInfo(typeItem.CategoryInfo);
                int dieRoll = _rnd.Next(1,101);
                var magicTable = Library.MagicItems.FirstOrDefault(item =>item.Class == category );
                if(magicTable!=null)
                {
                    var magicItem = magicTable.ItemEntries.FirstOrDefault(item => dieRoll >= item.LowRoll && dieRoll <= item.HighRoll);
                    if(magicItem!=null)
                    {
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
            }
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
                    var testCategory = Library.MagicItems.FirstOrDefault(item => dieRoll>=item.LowRoll && dieRoll<=item.HighRoll );
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
    }
}
