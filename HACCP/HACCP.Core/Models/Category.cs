using System.ComponentModel;
using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class Category : INotifyPropertyChanged
    {
        private short _recordStatus;

        [PrimaryKey]
        public long CategoryId { get; set; }

        public string CategoryName { get; set; }

        public short RecordStatus
        {
            get { return _recordStatus; }
            set
            {
                _recordStatus = value;
                RaisePropertyChangeEvent("RecordStatus");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChangeEvent(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }

    public class CategoryStatus
    {
        public long CategoryId { get; set; }
    }
}