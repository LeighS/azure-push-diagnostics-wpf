using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;

namespace NotificationHubs.Diagnostics.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly RegistrationDescription _model;

        public RegistrationViewModel(RegistrationDescription x)
        {
            _model = x;
        }

        public string RegistrationId => _model.RegistrationId;
        public DateTime? ExpirationTime => _model.ExpirationTime;
        public int TagCount => _model.Tags.Count;
        
        public string RegistrationType => _model.GetType().Name;

        public object Model => _model;

        public List<string> GetTags()
        {
            var tags = new List<string>();
            tags.AddRange(_model.Tags);
            return tags;
        }

    }
}