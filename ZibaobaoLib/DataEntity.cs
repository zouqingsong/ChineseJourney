using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SQLite;

namespace ZibaobaoLib
{
    public class DataEntity: INotifyPropertyChanged
    {
        [PrimaryKey]
        public string ID { get; set; }
        public DateTime TimeStamp { get; set;}
        public DateTime CreateTimeStamp { get; set; }
        public override string ToString()
        {
            return ID;
        }

        bool _isSelected;

        [Ignore, JsonIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public static string GetAutoKey(string prefix, int num)
        {
            return $"{prefix}_{num:D6}";
        }

        public static string ConcatAutoName(params string[] keys)
        {
            string key = string.Empty;
            if (keys != null)
            {
                foreach (var s in keys)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            key += "_";
                        }
                        key += s;
                    }
                }
            }
            return key;
        }

        public void FirePropertyChange(string property)
        {
            OnPropertyChanged(property);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
