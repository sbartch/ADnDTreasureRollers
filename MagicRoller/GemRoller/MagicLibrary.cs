using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MagicRoller
{
    #region JSON Classes
    // JSON Mapped Classses
    public class UseClassClass
    {
        public int UseClass { get; set; }
    }

    public class UseCategory
    {
        public string UseClassName { get; set; }
        public bool ClassName { get; set; }
        public List<UseClassClass> UseClassClasses { get; set; }
    }

    public class ItemEntry
    {
        public int LowRoll { get; set; }
        public int HighRoll { get; set; }
        public string Name { get; set; }
        public string Experience { get; set; }
        public string Price { get; set; }
    }

    public class MagicItem
    {
        public int Class { get; set; }
        public int LowRoll { get; set; }
        public int HighRoll { get; set; }
        public List<ItemEntry> ItemEntries { get; set; }
    }

    public class RootObject
    {
        public List<UseCategory> UseCategories { get; set; }
        public List<MagicItem> MagicItems { get; set; }
    }
    #endregion

    public class MyMagic
    {
        public int Class { get; set; }
        public int ItemIndex { get; set; }
    }

    public class VisibleMagicItem : INotifyPropertyChanged
    {
        private int _count=1;
        private string _class;
        private string _name;
        private string _expierience;
        private string _gpvalue;

        public string MagicClass { get { return _class; }  set { _class = value; RaisePropertyChanged("MagicClass"); } }
        public string MagicName { get { return _name; } set { _name = value; RaisePropertyChanged("MagicName"); } }
        public string MagicExperience { get { return _expierience; } set { _expierience = value; RaisePropertyChanged("MagicExperience"); } }
        public string MagicValue { get { return _gpvalue;  } set { _gpvalue = value; RaisePropertyChanged("MagicValue"); } }

        public int MagicCount { get { return _count; } set { _count = value; RaisePropertyChanged("MagicCount"); } }

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

    public class VisibleMagicClass : INotifyPropertyChanged
    {
        private string _name;
        private UseCategory _useCategory;
        public int Count { get; set; }
        public string ClassName { get { return _name; } set { _name = value; RaisePropertyChanged("ClassName"); } }

        public UseCategory CategoryInfo { get { return _useCategory; } set { _useCategory = value; RaisePropertyChanged("UseCategory"); } }

        #region NOTIFY
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
        #endregion
    }
    public class MagicClassList : ObservableCollection<VisibleMagicClass>
    {
        public MagicClassList():base()
        {
        }
    }

}
