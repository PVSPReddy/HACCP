using System.ComponentModel;
using SQLite.Net.Attributes;

namespace HACCP.Core
{
    public class User : INotifyPropertyChanged
    {
        [PrimaryKey]
        public long Id { get; set; }

        public string Name { get; set; }
        public string Permision { get; set; }
        public string Pin { get; set; }

        [Ignore]
        public bool IsSelected
        {
            get { return HaccpAppSettings.SharedInstance.CurrentUserId == Id; }
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
}