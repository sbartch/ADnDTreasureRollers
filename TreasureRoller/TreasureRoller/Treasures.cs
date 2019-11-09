using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasureRoller
{
    public class Row
    {
        public string Treasure { get; set; }
        public int Percent { get; set; }
        public int DieType { get; set; }
        public int DieCount { get; set; }
        public int DieMultiplier { get; set; }
    }

    public class TreasureType
    {
        public string Type { get; set; }
        public List<Row> Row { get; set; }
    }

    public class Treasures
    {
        public List<TreasureType> TreasureTypes { get; set; }
    }

    public class VisibleTreasure : INotifyPropertyChanged
    {
        private string _treasure;
        private int _count;
        public string Treasure { get { return _treasure; } set { _treasure = value; RaisePropertyChanged("Treasure"); } }
        public int Count { get { return _count; } set { _count = value; RaisePropertyChanged("Count"); } }
        #region Property Change Notification
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
