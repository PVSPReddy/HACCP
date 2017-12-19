using System.ComponentModel;
using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class Question : INotifyPropertyChanged
    {
        private short _recordStatus;

        [PrimaryKey]
        public long QuestionId { get; set; }

        public long CategoryId { get; set; }

        public string QuestionName { get; set; }

        public short QuestionType { get; set; }

        public string Min { get; set; }

        public string Max { get; set; }

        [Ignore]
        public string CategoryName { get; set; }

        [Ignore]
        public int RecordNo { get; set; }

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
}