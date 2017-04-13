using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NotificationHubs.Diagnostics.Services
{
    public class NotificationHubsFacade
    {
        private readonly string _connectionstring;
        private readonly string _hubName;

        public NotificationHubsFacade(string connectionstring, string hubName)
        {
            _connectionstring = connectionstring;
            _hubName = hubName;
        }

        public async Task<List<RegistrationDescription>> GetRegistrationsAsync()
        {
            var cs = _connectionstring;
            NotificationHubClient client = NotificationHubClient.CreateClientFromConnectionString(_connectionstring, _hubName);
            var items = await client.GetAllRegistrationsAsync(400);
            return items.ToList();

        }

        public async Task DeleteRegistrationAsync(string registrationId)
        {
            NotificationHubClient client = NotificationHubClient.CreateClientFromConnectionString(_connectionstring, _hubName);
            await client.DeleteRegistrationAsync(registrationId);
        }

        public async Task<NotificationDetails> GetNotificationOutcomeAsync(string notificationId)
        {
            NotificationHubClient client = NotificationHubClient.CreateClientFromConnectionString(_connectionstring, _hubName);
            var outcome = await client.GetNotificationOutcomeDetailsAsync(notificationId);
            return outcome;
        }

        public async Task<NotificationOutcome> SendNotificationAsync(string tag, string messageBody)
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(_connectionstring, _hubName, true);
            var notification = new Dictionary<string, string> { { "messageParam", messageBody } };
            return await hub.SendTemplateNotificationAsync(notification, tag);
        }

        //private static void SendNotifications()
        //{
        //    NotificationHubClient client = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
        //    var registrations = client.GetAllRegistrationsAsync(20).Result;
        //    foreach (var reg in registrations)
        //    {
        //        if (reg.Tags.Count > 0)
        //        {
        //            var tag = reg.Tags.First();

        //            // Get the Notification Hubs credentials for the Mobile App.

        //            // Create a new Notification Hub client.
        //            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName, true);

        //            var body =
        //                "Test: This is a test push notification sent from Leigh Shayler.";
        //            var notification = new Dictionary<string, string> { { "messageParam", body } };

        //            Task<NotificationOutcome> task = hub.SendTemplateNotificationAsync(notification, tag);
        //            Task.Run(async () => { await task; }).Wait();

        //        }
        //    }
        //}
    }
}
