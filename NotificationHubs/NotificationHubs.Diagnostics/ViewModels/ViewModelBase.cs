using System.ComponentModel;
using System.Runtime.CompilerServices;
using NotificationHubs.Diagnostics.Annotations;

namespace NotificationHubs.Diagnostics.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (Equals(value, _isLoading)) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }
}
