using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace EncounterRoller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<string> TerrainList = new ObservableCollection<string>();

        public ObservableCollection<string> RegionList = new ObservableCollection<string>();

        OutdoorEncountersLibrary AllLibraries = new OutdoorEncountersLibrary();
        public MainWindow()
        {
            InitializeComponent();
            AllLibraries.OutdoorEncounterList = Utilities.LoadOutdoorEncounterLibraries();
            foreach( var EncounterSet in AllLibraries.OutdoorEncounterList )
            {
                if( !RegionList.Contains(EncounterSet.Encounters.Terrain))
                    RegionList.Add(EncounterSet.Encounters.Terrain);
            }
            RaisePropertyChanged("RegionList");
            StuffRegionsFromEncounters(AllLibraries.OutdoorEncounterList[0].Encounters.Terrain);
        }

        private void StuffRegionsFromEncounters(string terrain )
        {
            TerrainList = new ObservableCollection<string>();
            foreach (var blob in AllLibraries.OutdoorEncounterList)
            {
                if (blob.Encounters.Terrain == terrain && !TerrainList.Contains(blob.Encounters.Region) )
                    TerrainList.Add(blob.Encounters.Region);
            }
            RaisePropertyChanged("TerrainList");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// Property changed Notification        
        public void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
