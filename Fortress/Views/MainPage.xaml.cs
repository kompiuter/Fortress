using System;
using Fortress.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace Fortress.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        public MainPageViewModel Vm => (MainPageViewModel)DataContext;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var z = this.DataContext;
        }
    }
}
