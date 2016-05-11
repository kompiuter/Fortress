using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Fortress.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Mvvm;
using Template10.Common;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.ViewManagement;

namespace Fortress
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();

            #region App settings

            ShowShellBackButton = false;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            string arguments = string.Empty;
            if (DetermineStartCause(args) == AdditionalKinds.JumpListItem)
            {
                arguments = (args as LaunchActivatedEventArgs).Arguments;
                FileReceived?.Invoke(this, arguments);
            }
            NavigationService.Navigate(typeof(Views.LoginPage), arguments);
            return Task.CompletedTask;
        }


        public static event EventHandler<string> FileReceived;
    }
}

