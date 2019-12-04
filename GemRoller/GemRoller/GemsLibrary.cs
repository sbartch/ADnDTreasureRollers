using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GemRoller
{
    public class GemsDetails
    {
        public string Name { get; set; }
    }
    public class GemsClasses
    {
        public string Type { get; set; }
        public int Class { get; set; }
        public int LowRoll { get; set; }
        public int HighRoll { get; set; }
        public int BaseValue { get; set; }
        public List<GemsDetails> GemDetails { get; set; } 
    }
    public class GemsBaseValue
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }
    public class GemsBaseValues
    {
        public List<GemsBaseValues> GemsValues { get; set; }
    }
    public class GemsLibrary
    {
        [JsonProperty(PropertyName ="GemsClasses")]
        public List<GemsClasses> Gems { get; set; }
    }

    public class GemsValueLibrary
    {
        [JsonProperty(PropertyName = "GemsBaseValues")]
        public List<GemsBaseValue> GemsValues { get; set; }
    }

    public class MyGems
    {
        public MyGems() { }
        public MyGems( ref MyGems myGems)
        {
            Count = myGems.Count;
            Class = myGems.Class;
            GemIndex = myGems.GemIndex;
            SpecialValue = myGems.SpecialValue;
        }
        public int Count { get; set; }
        public int Class { get; set; }
        public int GemIndex { get; set; }
        public float SpecialValue { get; set; }
    }

    public class VisibleGems : INotifyPropertyChanged
    {
        private int _count;
        private string _name;
        private float _value;
        private float _totalValue;
        private string _specialValue;

        public int GemCount
        {
            get { return _count; }
            set {
                _count = value;
                _totalValue = (float) _count * _value;
                RaisePropertyChanged("GemCount");
                RaisePropertyChanged("TotalValue");
            }
        }
        public string SpecialValue
        {
            get { return _specialValue; }
            set { _specialValue = value;
                RaisePropertyChanged("SpecialValue");
            }
        }
        public string GemName { get { return _name; } set { _name = value; RaisePropertyChanged("GemName"); } }
        public float GemValue
        {
            get { return _value;  }
            set {
                _value = value;
                _totalValue = _value * _count;
                RaisePropertyChanged("GemValue");
                RaisePropertyChanged("TotalValue");
            }
        }

        public float TotalValue
        {
            get { return _totalValue; }
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
