using Newtonsoft.Json;
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


namespace TreasureRoller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<VisibleTreasure> _myCollection = new ObservableCollection<VisibleTreasure>();
        public ObservableCollection<VisibleTreasure> MyCollection { get { return _myCollection; } set { _myCollection = value; OnPropertyChanged("MyCollection"); } }

        private Treasures _library;
        public Treasures Library { get => _library; set => _library = value; }
        public MainWindow()
        {
            InitializeComponent();
            Library = JsonConvert.DeserializeObject<Treasures>(File.ReadAllText("Treasure.json"));
        }

        #region Property Change
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private void treasureListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCopy.IsEnabled = (TreasureListView.SelectedItems.Count > 0);
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            foreach (var item in TreasureListView.SelectedItems)
            {
                VisibleTreasure treasure = (VisibleTreasure)item;
                string sOut = string.Format("{0} : {1} \r\n", treasure.Treasure, treasure.Count);
                buffer += sOut;
            }
            Clipboard.SetText(buffer);
        }

        private void generateTreasure_Click(object sender, RoutedEventArgs e)
        {
            string s = treasureTypes.Text.TrimStart().TrimEnd();
            string[] listOfTreasures = s.Split(new char[]{ ',',' '});

            Random rnd = new Random();
            MyCollection.Clear();

            List<VisibleTreasure> foundTreasure = new List<VisibleTreasure>();

            foreach (var t in listOfTreasures)
            {
                string treasure=t.TrimEnd();
                treasure = treasure.TrimStart();
                treasure = treasure.ToUpper();

                TreasureType tt = Library.TreasureTypes.Find(x => x.Type == treasure);
                if(tt!=null)
                {
                    foreach(var row in tt.Row)
                    {
                        if(rnd.Next(1,101)<=row.Percent)
                        {
                            int count = 0;
                            for (int j = 0; j < row.DieCount; j++)
                                count += (rnd.Next(1, row.DieType + 1) * row.DieMultiplier);
                            var myTreasure = foundTreasure.FirstOrDefault(o => o.Treasure == row.Treasure);
                            if(myTreasure!=null)
                            {
                                myTreasure.Count += count;
                            }
                            else
                            {
                                VisibleTreasure vt = new VisibleTreasure();
                                vt.Treasure = row.Treasure;
                                vt.Count = count;
                                foundTreasure.Add(vt);
                            }
                        }
                    }
                }
            }
    
            foreach(var vt in foundTreasure)
            {
                MyCollection.Add(vt);
            }
        }
    }
}
