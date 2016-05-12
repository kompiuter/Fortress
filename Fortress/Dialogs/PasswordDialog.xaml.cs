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
            if (ValidatePassword(passwordBox.Password))
            {
                ValidationVisibility = Visibility.Collapsed;

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
            }
            else
            {
                IsPrimaryButtonEnabled = false;
                ValidationVisibility = Visibility.Visible;
                PasswordErrorVisibility = Visibility.Collapsed;
            }

            //if (string.IsNullOrEmpty(passwordBox.Password) || string.IsNullOrEmpty(repeatPasswordBox.Password))
            //    IsPrimaryButtonEnabled = false;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValidationVisibility)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PasswordErrorVisibility)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLongEnough)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoesContainDigit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoesContainUpper)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoesContainLower)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoesContainSymbol)));
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // In order for password to be valid it must be
            // 1 - At least 8 characters
            // 2 - Contains three of the following:
            //   i. Upper case characters
            //   ii. Lower case characters
            //   iii. Symbols
            //   iiii. Digits
            if (!IsLongEnough)
                return false;

            int conditionsMet = 0;

            if (DoesContainDigit)
                ++conditionsMet;
            if (DoesContainUpper)
                ++conditionsMet;
            if (DoesContainLower)
                ++conditionsMet;
            if (DoesContainSymbol)
                ++conditionsMet;

            if (conditionsMet >= 3)
                return true;
            else
                return false;
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
        private Visibility ValidationVisibility { get; set; } = Visibility.Collapsed;

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

        public bool IsLongEnough => passwordBox.Password?.Length >= 8;
        public bool DoesContainDigit => passwordBox.Password?.Any(p => char.IsDigit(p)) ?? false;
        public bool DoesContainUpper => passwordBox.Password?.Any(p => char.IsUpper(p)) ?? false;
        public bool DoesContainLower => passwordBox.Password?.Any(p => char.IsLower(p)) ?? false;
        public bool DoesContainSymbol => passwordBox.Password?.Any(p => char.IsSymbol(p) || char.IsPunctuation(p)) ?? false;

    }
}
