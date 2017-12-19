using System.ComponentModel;
using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class MenuLocation : INotifyPropertyChanged
    {
        private short _recordStatus;

        [PrimaryKey]
        public long RowId { get; set; }

        public long LocationId { get; set; }
        public string Name { get; set; }

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
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChangeEvent(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }

    public class MenuLocationId
    {
        public long LocationId { get; set; }
    }
}