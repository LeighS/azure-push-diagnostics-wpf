using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using NotificationHubs.Diagnostics.Commands;
using NotificationHubs.Diagnostics.Services;

namespace NotificationHubs.Diagnostics.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private RegistrationViewModel _selectedRegistration;
        private string _connectionString;
        private NotificationHubsFacade _notificationsFacade;
        private string _hubName;
        private ObservableCollection<string> _registrationTags;
        private string _selectedTag;

        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (value == _connectionString) return;
                _connectionString = value;
                _notificationsFacade = new NotificationHubsFacade(_connectionString, _hubName);
                OnPropertyChanged();
            }
        }

        public string HubName
        {
            get { return _hubName; }
            set
            {
                if (value == _hubName) return;
                _hubName = value;
                _notificationsFacade = new NotificationHubsFacade(_connectionString, _hubName);
                OnPropertyChanged();
            }
        }


        public ObservableCollection<RegistrationViewModel> Registrations { get; set; }

        public ObservableCollection<string> RegistrationTags
        {
            get { return _registrationTags; }
            set
            {
                if (value == _registrationTags) return;
                _registrationTags = value;
                OnPropertyChanged();
            }
        }

        public RegistrationViewModel SelectedRegistration
        {
            get { return _selectedRegistration; }
            set
            {
                if (Equals(value, _selectedRegistration)) return;
                _selectedRegistration = value;
                UpdateRegistrationTags();
                OnPropertyChanged();
            }
        }

        public string SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                if (value == _selectedTag) return;
                _selectedTag = value;
                OnPropertyChanged();
            }
        }

        private void UpdateRegistrationTags()
        {
            RegistrationTags.Clear();
            _selectedRegistration?.GetTags().ForEach(x=> RegistrationTags.Add(x));
        }

        public ICommand GetRegistrationsCommand { get; set; }
        public ICommand SendNotificationCommand { get; set; }
        public ICommand DeleteRegistrationsCommand { get; set; }

        public MainViewModel(string connectionString, string hubName)
        {
            ConnectionString = connectionString;
            HubName = hubName;
            Registrations = new ObservableCollection<RegistrationViewModel>();
            RegistrationTags = new ObservableCollection<string>();
            GetRegistrationsCommand = new SimpleDelegateCommand(GetRegistrationsAsync);
            SendNotificationCommand = new SimpleDelegateCommand(SendNotificationAsync);
            DeleteRegistrationsCommand = new SimpleDelegateCommand(DeleteRegistrationsAsync);
        }

        public async Task DeleteRegistrationsAsync()
        {
            await _notificationsFacade.DeleteAllRegistrationsAsync();
        }

        public async Task SendNotificationAsync()
        {
            await _notificationsFacade.SendNotificationAsync(SelectedTag, $"{DateTime.Now:s}: Test push notification");
        }

        public async Task GetRegistrationsAsync()
        {
            Registrations.Clear();
            var reg = await _notificationsFacade.GetRegistrationsAsync();
            reg.ForEach(x=> Registrations.Add(new RegistrationViewModel(x)));
        }
    }
}
