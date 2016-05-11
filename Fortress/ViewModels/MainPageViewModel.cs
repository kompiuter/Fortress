using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Fortress.Models;
using Windows.Storage;
using Fortress.Dialogs;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Fortress.Views;

namespace Fortress.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                Vault = new MockVaultFile();
        }

        #region Properties

        private IVaultFile _vault;
        public IVaultFile Vault
        {
            get { return _vault; }
            set { Set(ref _vault, value); }
        }


        private VaultFileEntry _selectedEntry;
        public VaultFileEntry SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                Set(ref _selectedEntry, value);
                EditEntryCommand.RaiseCanExecuteChanged();
                DeleteEntryCommand.RaiseCanExecuteChanged();
            }
        }

        private MessageDialog _deleteDialog;

        #endregion

        #region Commands

        DelegateCommand _editEntryCommand;
        public DelegateCommand EditEntryCommand
            => _editEntryCommand ?? (_editEntryCommand = new DelegateCommand(async () => await ExecuteEditEntryCommand(),
                                                                                   () => SelectedEntry != null));


        DelegateCommand _newEntryCommand;
        public DelegateCommand NewEntryCommand
            => _newEntryCommand ?? (_newEntryCommand = new DelegateCommand(async () => await ExecuteNewEntryCommand()));


        DelegateCommand _deleteEntryCommand;
        public DelegateCommand DeleteEntryCommand
            => _deleteEntryCommand ?? (_deleteEntryCommand = new DelegateCommand(async () => await ExecuteDeleteEntryCommand(),
                                                                                       () => SelectedEntry != null));

        DelegateCommand _saveVaultCommand;
        public DelegateCommand SaveVaultCommand
            => _saveVaultCommand ?? (_saveVaultCommand = new DelegateCommand(async () => await Vault.EncryptAndSave()));

        DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand
            => _goBackCommand ?? (_goBackCommand = new DelegateCommand(async () => await ExecuteGoBackCommand()));


        #endregion

        private async Task ExecuteEditEntryCommand()
        {
            var entryDialog = new VaultEntryDialog();
            var vaultCopy = SelectedEntry.Copy();
            entryDialog.DataContext = vaultCopy;

            var result = await entryDialog.ShowAsync();

            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                // Get index of old entry
                var indexEdit = Vault.Entries.IndexOf(Vault.Entries.Where(f => f.ID == vaultCopy.ID).First());

                // Remove old entry
                Vault.Entries.RemoveAt(indexEdit);

                vaultCopy.ModifiedAt = DateTime.Now;

                // Add edited entry
                Vault.Entries.Insert(indexEdit, vaultCopy);

                SelectedEntry = vaultCopy;
            }
        }

        private async Task ExecuteNewEntryCommand()
        {
            var entryDialog = new VaultEntryDialog();
            var newEntry = new VaultFileEntry()
            {
                IsNewEntry = true
            };
            entryDialog.DataContext = newEntry;

            var result = await entryDialog.ShowAsync();

            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                newEntry.IsNewEntry = false;
                newEntry.CreatedAt = DateTime.Now;
                newEntry.ModifiedAt = DateTime.Now;
                Vault.AddNewEntry(newEntry);
            }
        }

        private async Task ExecuteDeleteEntryCommand()
        {
            if (_deleteDialog == null)
            {

                UICommand yesCommand = new UICommand("Yes", e => DeleteEntry(SelectedEntry.ID));
                UICommand noCommand = new UICommand("No", e => { });

                _deleteDialog = new MessageDialog("Are you sure you want to delete the selected entry?", "Confirm Action");
                _deleteDialog.Commands.Add(yesCommand);
                _deleteDialog.Commands.Add(noCommand);
            }
            var content = "Are you sure you want to delete the following entry: "
                          + Environment.NewLine + Environment.NewLine
                          + $">{SelectedEntry.Title}";

            _deleteDialog.Content = content;

            await _deleteDialog.ShowAsync();
        }

        private Task ExecuteGoBackCommand()
        {
            if (SessionState.ContainsKey("Vault"))
                SessionState.Remove("Vault");

            Vault = null;
            SelectedEntry = null;

            NavigationService.Navigate(typeof(LoginPage));
            NavigationService.ClearHistory();

            return Task.CompletedTask;
        }

        private void DeleteEntry(Guid ID)
        {
            Vault.Entries.Remove(Vault.Entries.Single(f => f.ID == ID));
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Vault = SessionState.Get<VaultFile>("Vault");

            await Task.CompletedTask;
        }

    }
}

