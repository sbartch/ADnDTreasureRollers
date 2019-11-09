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

namespace JewelryRoller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<VisibleJewelry> _myCollection = new ObservableCollection<VisibleJewelry>();
        public ObservableCollection<VisibleJewelry> MyCollection { get { return _myCollection; } set { _myCollection = value; OnPropertyChanged("MyCollection"); } }

        private JewelryLibrary _library;
        public JewelryLibrary Library { get => _library; set => _library = value; }
        public MainWindow()
        {
            InitializeComponent();
            Library = JsonConvert.DeserializeObject<JewelryLibrary>(File.ReadAllText("Jewelry.json"));
        }

        private void jewelryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCopy.IsEnabled = (jewelryListView.SelectedItems.Count > 0);
        }

        private void RollJewelry_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            MyCollection.Clear();
            List<MyJewelry> myJewelryList = new List<MyJewelry>();

            int totalJewelry = 0;
            Int32.TryParse(TotalJewelryCount.Text, out totalJewelry);

            if (totalJewelry <= 0)
                return;

            for (int i=0;i<totalJewelry;i++)
            {
                int jewelryRoll = rnd.Next(1, 101);
                int jewelryClass = -1;

                foreach(var libClass in Library.Jewelry)
                {
                    if (jewelryRoll >= libClass.LowRoll && jewelryRoll <= libClass.HighRoll)
                        jewelryClass = libClass.Class;
                }

                if (jewelryClass >= 0)
                {
                    int value = 0;
                    for (int j = 0; j < Library.Jewelry[jewelryClass].DieCount; j++)
                    {
                        value += rnd.Next(1, Library.Jewelry[jewelryClass].DieType + 1);
                    }
                    value *= Library.Jewelry[jewelryClass].DieMultiplier;

                    var myJewelry = myJewelryList.FirstOrDefault(o => o.Class == jewelryClass && o.Value == value);
                    if (myJewelry != null)
                    {
                        myJewelry.Count++;
                    }
                    else
                    {
                        MyJewelry newJewelry = new MyJewelry();
                        newJewelry.Class = jewelryClass;
                        newJewelry.Value = value;
                        newJewelry.Count = 1;
                        myJewelryList.Add(newJewelry);
                    }
                }
            }
            foreach(var jewelrySet in myJewelryList)
            {
                VisibleJewelry vj = new VisibleJewelry();
                vj.JewelryCount = jewelrySet.Count;
                vj.JewelryName = Library.Jewelry[jewelrySet.Class].Type;
                vj.JewelryAverageValue = jewelrySet.Value;
                bool found = false;
                foreach(VisibleJewelry jp in MyCollection )
                {
                    if(jp.JewelryName == vj.JewelryName)
                    {
                        found = true;
                        int totJewels = jp.JewelryCount + vj.JewelryCount;
                        float totValue = jp.JewelryAgregateValue + vj.JewelryAgregateValue;
                        jp.JewelryCount = totJewels;
                        jp.JewelryAverageValue = totValue / totJewels;
                        break;
                    }
                }
                if(!found)
                    MyCollection.Add(vj);
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            foreach (var item in jewelryListView.SelectedItems)
            {
                VisibleJewelry jewelry = (VisibleJewelry)item;
                string sOut = string.Format("{0} X {1} @ {2}gp\n", jewelry.JewelryCount, jewelry.JewelryName, jewelry.JewelryAverageValue);
                buffer += sOut;
            }
            Clipboard.SetText(buffer);
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
