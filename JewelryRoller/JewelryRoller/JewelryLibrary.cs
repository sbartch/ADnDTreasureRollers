using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace JewelryRoller
{
    public class JewelryClasses
    {
        public int Class { get; set; }
        public string Type { get; set; }
        public int LowRoll { get; set; }
        public int HighRoll { get; set; }
        public int DieType { get; set; }
        public int DieCount { get; set; }
        public int DieMultiplier { get; set; }
    }
    public class JewelryLibrary
    {
        [JsonProperty(PropertyName="JewelryClasses")]
        public List<JewelryClasses> Jewelry { get; set; }
    }
    public class MyJewelry
    {
        public int Count { get; set; }
        public int Class { get; set; }
        public int Value { get; set; }
    }
    public class VisibleJewelry : INotifyPropertyChanged
    {
        private int _count;
        private string _name;
        private float _value;
        public int JewelryCount { get { return _count; } set { _count = value; RaisePropertyChanged("JewelryCount"); } }
        public string JewelryName { get { return _name; } set { _name = value; RaisePropertyChanged("JewelryName"); } }
        public float JewelryAverageValue { get { return _value; } set { _value = value; RaisePropertyChanged("JewelryValue"); } }
        public float JewelryAgregateValue { get { return (_value * _count); } }
        /// Property changed Notification        
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

    }
}
