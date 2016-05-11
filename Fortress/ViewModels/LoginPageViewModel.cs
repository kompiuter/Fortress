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

namespace Fortress.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private StorageFile _inputFile;
        Services.DataService _DataService;
        Services.SettingsServices.SettingsService _SettingService;

        public LoginPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _DataService = new Services.DataService();
                _SettingService = new Services.SettingsServices.SettingsService();
            }
        }


        #region Properties

        private string _fileInputPath;
        public string FileInputPath
        {
            get { return _fileInputPath; }
            set
            {
                if (Set(ref _fileInputPath, value))
                    UnlockFileCommand.RaiseCanExecuteChanged();
            }
        }

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
                                                                                             (p) => !string.IsNullOrEmpty(p) && !string.IsNullOrWhiteSpace(_fileInputPath)));

        #endregion

        #region Methods

        private async Task ExecuteUnlockFileCommand(string key)
        {
            var vaultFile = new VaultFile(_inputFile, key);
            if (await vaultFile.Decrypt(key))
            {

                GoToMainPage(vaultFile);
            }
            else
            {
                string dialogContent = _fileInputPath
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
                    _inputFile = newFile;
                    var vaultFile = new VaultFile(_inputFile, passDialog.Password);
                    await vaultFile.EncryptAndSave();

                    await _DataService.RegisterFile(_inputFile);

                    GoToMainPage(vaultFile);
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
            {
                _inputFile = pickedFile;
                await _DataService.RegisterFile(_inputFile);
                FileInputPath = _inputFile.Path;             
            }
        }

        private void GoToMainPage(VaultFile vaultFile)
        {
            if (SessionState.ContainsKey("Vault"))
                SessionState.Remove("Vault");

            SessionState.Add("Vault", vaultFile);

            NavigationService.Navigate(typeof(MainPage));
            NavigationService.ClearHistory();
        }

        #endregion

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (string.IsNullOrWhiteSpace(parameter as string))
                _inputFile = await _DataService.GetFileInfoAsync(_SettingService.Recent);
            else
                _inputFile = await _DataService.GetFileInfoAsync(parameter.ToString());

            if (_inputFile != null)
                FileInputPath = _inputFile.Path;

            App.FileReceived += FileReceivedHandler;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            _SettingService.Recent = _inputFile?.Path;
            App.FileReceived -= FileReceivedHandler;
            return Task.CompletedTask;
        }

        private async void FileReceivedHandler(object sender, string path)
        {
            _inputFile = await _DataService.GetFileInfoAsync(path);
            FileInputPath = _inputFile?.Path;
            NavigationService.ClearHistory();
        }

    }
}
