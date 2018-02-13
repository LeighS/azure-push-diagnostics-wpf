using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using NotificationHubs.Diagnostics.Commands;
using NotificationHubs.Diagnostics.Services;

namespace NotificationHubs.Diagnostics.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _historyFileName = "history.json";
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

        private ObservableCollection<Models.HistoryItem> _historyItems;
        public ObservableCollection<Models.HistoryItem> HistoryItems
        {
            get { return _historyItems; }
            set
            {
                if (value == _historyItems) return;
                _historyItems = value;
                OnPropertyChanged();
            }
        }

        private Models.HistoryItem _selectedHistoryItem;
        public Models.HistoryItem SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }
            set
            {
                if (Equals(value, _selectedHistoryItem)) return;
                _selectedHistoryItem = value;
                SelectHistoryItem();
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
            _selectedRegistration?.GetTags().ForEach(x => RegistrationTags.Add(x));
        }

        private async Task SelectHistoryItem()
        {
            RegistrationTags.Clear();
            ConnectionString = SelectedHistoryItem.ConnectionString;
            HubName = SelectedHistoryItem.Name;

            await GetRegistrationsInternalAsync();
        }

        public ICommand GetRegistrationsCommand { get; set; }
        public ICommand SendNotificationCommand { get; set; }
        public ICommand DeleteRegistrationsCommand { get; set; }

        public MainViewModel(string connectionString, string hubName)
        {
            ConnectionString = connectionString;
            HubName = hubName;
            HistoryItems = new ObservableCollection<Models.HistoryItem>();
            Registrations = new ObservableCollection<RegistrationViewModel>();
            RegistrationTags = new ObservableCollection<string>();
            GetRegistrationsCommand = new SimpleDelegateCommand(GetRegistrationsAsync);
            SendNotificationCommand = new SimpleDelegateCommand(SendNotificationAsync);
            DeleteRegistrationsCommand = new SimpleDelegateCommand(DeleteRegistrationsAsync);

            LoadHistory();
        }

        private void LoadHistory()
        {
            if (!File.Exists(_historyFileName))
                return;

            var json = File.ReadAllText(_historyFileName);
            var items = JsonConvert.DeserializeObject<List<Models.HistoryItem>>(json);
            if (items == null || !items.Any())
                return;

            items.ForEach(item => HistoryItems.Add(item));
        }

        public async Task DeleteRegistrationsAsync()
        {
            IsLoading = true;
            if ((MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                await _notificationsFacade.DeleteAllRegistrationsAsync();
            IsLoading = false;
        }

        public async Task SendNotificationAsync()
        {
            IsLoading = true;
            var message = $"{DateTime.Now:s}: Test push notification";
            await _notificationsFacade.SendNotificationAsync(SelectedTag, message);
            MessageBox.Show($"Message '{message}' sent.", "Message Sent");
            IsLoading = false;
        }

        public async Task GetRegistrationsAsync()
        {
            IsLoading = true;

            SaveHistoryItem();
            await GetRegistrationsInternalAsync();
        }

        public async Task GetRegistrationsInternalAsync()
        {
            IsLoading = true;
            try
            {
                Registrations.Clear();
                var reg = await _notificationsFacade.GetRegistrationsAsync();
                reg.ForEach(x => Registrations.Add(new RegistrationViewModel(x)));

                if (reg.Count == 0)
                    MessageBox.Show("No items found.", "No Items");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An Error Occured. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
 
        private void SaveHistoryItem()
        {
            var mostRecent = HistoryItems.Count > 0 ? HistoryItems[0] : null;
            if (mostRecent == null || (mostRecent.Name != _hubName || mostRecent.ConnectionString != _connectionString))
            {
                HistoryItems.Insert(0, new Models.HistoryItem { ConnectionString = _connectionString, Name = _hubName });
                var json = JsonConvert.SerializeObject(HistoryItems);
                File.WriteAllText(_historyFileName, json);
            }
        }
   }
}
