using Fortress.Dialogs;
using Fortress.Models;
using Fortress.Services;
using Fortress.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Fortress.Services.SettingsServices;

namespace Fortress.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        DataService _DataService;
        SettingsService _SettingService;

        public LoginPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _DataService = new DataService();
                _SettingService = new SettingsService();
            }
        }


        #region Properties

        private StorageFile _inputFile;
        public StorageFile InputFile
        {
            get
            {
                return _inputFile;
            }
            private set
            {
                if (Set(ref _inputFile, value))
                {
                    UnlockFileCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(InputFilePath));
                }
            }
        }

        public string InputFilePath => InputFile?.Path;

        #endregion

        #region Commands

        DelegateCommand _newFileCommand;
        public DelegateCommand NewFileCommand
            => _newFileCommand ?? (_newFileCommand = new DelegateCommand(async () => await ExecuteNewFileCommand()));

        DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand
            => _openFileCommand ?? (_openFileCommand = new DelegateCommand(async () => await ExecuteOpenFileCommand()));

        DelegateCommand<string> _unlockFileCommand;
        public DelegateCommand<string> UnlockFileCommand
            => _unlockFileCommand ?? (_unlockFileCommand = new DelegateCommand<string>(async (p) => await ExecuteUnlockFileCommand(p),
                                                                                             (p) => !string.IsNullOrEmpty(p) && !string.IsNullOrWhiteSpace(InputFilePath)));

        #endregion

        #region Methods

        private async Task ExecuteUnlockFileCommand(string key)
        {
            var vaultFile = new VaultFile(InputFile, key);
            if (await vaultFile.Decrypt(key))
            {
                await GoToMainPage(vaultFile);
            }
            else
            {
                string dialogContent = InputFilePath
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Failed to unlock the specified file!"
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "The key is invalid.";

                var dialog = new MessageDialog(dialogContent, "Error");
                await dialog.ShowAsync();
            }
        }

        private async Task ExecuteNewFileCommand()
        {
            FileSavePicker saver = new FileSavePicker
            {
                DefaultFileExtension = ".fvf",
                SuggestedFileName = "NewVault",
                CommitButtonText = "Save",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            saver.FileTypeChoices.Add("Fortress Vault Files", new List<string> { ".fvf" });

            var newFile = await saver.PickSaveFileAsync();

            if (newFile != null)
            {
                var passDialog = new PasswordDialog()
                {
                    FilePath = newFile.Path
                };

                var result = await passDialog.ShowAsync();

                if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    InputFile = newFile;
                    var vaultFile = new VaultFile(InputFile, passDialog.Password);
                    await vaultFile.EncryptAndSave();

                    await GoToMainPage(vaultFile);
                }
                else
                {
                    await newFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
        }
        
        private async Task ExecuteOpenFileCommand()
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                CommitButtonText = "Open",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add(".fvf");

            var pickedFile = await picker.PickSingleFileAsync();

            if (pickedFile != null)
                InputFile = pickedFile;      
        }

        private async Task GoToMainPage(VaultFile vaultFile)
        {
            if (SessionState.ContainsKey("Vault"))
                SessionState.Remove("Vault");
            SessionState.Add("Vault", vaultFile);

            await _DataService.RegisterFile(InputFile);

            NavigationService.Navigate(typeof(MainPage));
            NavigationService.ClearHistory();
        }

        private async void FileReceivedHandler(object sender, string path)
        {
            InputFile = await _DataService.GetFileInfoAsync(path);

            NavigationService.ClearHistory();
        }

        #endregion

        #region Overrides

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            // Jump-file passed as parameter on activation
            if (string.IsNullOrWhiteSpace(parameter as string))
                InputFile = await _DataService.GetFileInfoAsync(_SettingService.Recent);
            else
                InputFile = await _DataService.GetFileInfoAsync(parameter.ToString());

            App.FileReceived += FileReceivedHandler;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            _SettingService.Recent = InputFilePath;
            App.FileReceived -= FileReceivedHandler;

            return Task.CompletedTask;
        }

        #endregion

    }
}
