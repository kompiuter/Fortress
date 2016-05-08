using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Fortress.Dialogs
{
    public sealed partial class PasswordDialog : ContentDialog, INotifyPropertyChanged
    {
        public PasswordDialog()
        {
            this.InitializeComponent();
            PrimaryButtonClick += PasswordDialog_PrimaryButtonClick;
            IsPrimaryButtonEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Event Handlers

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == repeatPasswordBox.Password)
            {
                IsPrimaryButtonEnabled = true;
                PasswordErrorVisibility = Visibility.Collapsed;
            }
            else
            {
                IsPrimaryButtonEnabled = false;
                PasswordErrorVisibility = Visibility.Visible;
            }

            if (string.IsNullOrEmpty(passwordBox.Password) || string.IsNullOrEmpty(repeatPasswordBox.Password))
                IsPrimaryButtonEnabled = false;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PasswordErrorVisibility)));
        }

        private void PasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Password = passwordBox.Password;
            PrimaryButtonClick -= PasswordDialog_PrimaryButtonClick;
        }

        #endregion

        #region Properties

        public string Password { get; private set; }

        private string TimeToCrack { get; set; }

        private Visibility PasswordErrorVisibility { get; set; } = Visibility.Collapsed;

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(PasswordDialog), new PropertyMetadata(string.Empty, (s, e) =>
            {
                ((PasswordDialog)s).fileTextBlock.Text = e.NewValue.ToString();
            }));


        #endregion

        public void TestPrimary()
        {

        }


    }
}
