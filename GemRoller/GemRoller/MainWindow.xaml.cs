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
        public ObservableCollection<VisibleGems> _myCollection= new ObservableCollection<VisibleGems>();
        public ObservableCollection<VisibleGems> MyCollection { get { return _myCollection; } set { _myCollection = value; OnPropertyChanged("MyCollection"); } }

        public GemsLibrary Library { get => _library; set => _library = value; }

        GemsLibrary _library;
        public MainWindow()
        {
            InitializeComponent();
            Library = JsonConvert.DeserializeObject< GemsLibrary > (File.ReadAllText("Gems.json"));
        }

        private void RollGems_Click(object sender, RoutedEventArgs e)
        {
            Random rnd=new Random();
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
                int nextGroup = rnd.Next(lowGroup, highGroup + 1);
                if (nextGroup > totalGems)
                    nextGroup = totalGems;

                int gemRoll = rnd.Next(1, 101); // Percentile Throw for 1 to 100
                int gemClass = -1;
                foreach(var gemsClass in Library.Gems)
                {
                    if (gemRoll >= gemsClass.LowRoll && gemRoll <= gemsClass.HighRoll)
                        gemClass = gemsClass.Class;
                }

                int gemIndex = rnd.Next(Library.Gems[gemClass].GemDetails.Count);

                var mygems = myGemsList.FirstOrDefault(o => o.Class == gemClass && o.GemIndex == gemIndex);
                if (mygems != null)
                    mygems.Count += nextGroup;
                else
                {
                    MyGems newGem = new MyGems();
                    newGem.Class = gemClass;
                    newGem.GemIndex = gemIndex;
                    newGem.Count = nextGroup;
                    myGemsList.Add(newGem);
                }

                totalGems -= nextGroup;

            } while (totalGems > 0);

            int totalValue = 0;
            foreach(var gemSet in myGemsList)
            {
                VisibleGems vg = new VisibleGems();
                vg.GemCount = gemSet.Count;
                vg.GemName = Library.Gems[gemSet.Class].GemDetails[gemSet.GemIndex].Name;
                vg.GemValue = Library.Gems[gemSet.Class].BaseValue;
                MyCollection.Add(vg);
                totalValue += vg.TotalValue;
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
