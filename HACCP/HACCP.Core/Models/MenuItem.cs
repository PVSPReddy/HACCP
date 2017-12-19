using System.ComponentModel;
using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class LocationMenuItem : INotifyPropertyChanged
    {
        private short _recordStatus;

        public TemperatureUnit TemperatureUnit;

        [PrimaryKey]
        public long RowId { get; set; }

        public long ItemId { get; set; }

        public string Name { get; set; }

        public long LocationId { get; set; }

        public string Ccpid { get; set; }

        public string Ccp { get; set; }

        public string Max { get; set; }

        public string Min { get; set; }

        public string Note { get; set; }

        public short RecordStatus
        {
            get { return _recordStatus; }
            set
            {
                _recordStatus = value;
                RaisePropertyChangeEvent("RecordStatus");
            }
        }

        public bool IsManualEntry { get; set; }

        public string RecordedTemperature { get; set; }

        public string Blue2Id { get; set; }

        [Ignore]
        public int RecordNo { get; set; }

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
}