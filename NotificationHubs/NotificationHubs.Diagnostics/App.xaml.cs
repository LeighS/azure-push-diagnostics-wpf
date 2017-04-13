using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NotificationHubs.Diagnostics.ViewModels;

namespace NotificationHubs.Diagnostics
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            var vm = new MainViewModel(
                ConfigurationManager.ConnectionStrings["NotificationHubs"].ConnectionString,
                ConfigurationManager.AppSettings["NotificationHubName"]);
            window.DataContext = vm;
            window.Show(); 
        }
    }
}
