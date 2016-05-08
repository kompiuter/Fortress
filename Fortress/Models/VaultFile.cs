using Fortress.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;

namespace Fortress.Models
{
    public class VaultFile : BindableBase, IVaultFile
    {
        public VaultFile(StorageFile file, string key)
        {
            _file = file;
            _key = key;
            VaultID = Guid.NewGuid();
        }

        [JsonRequired]
        public Guid VaultID { get; private set; }
        
        [JsonIgnore]
        private StorageFile _file { get; }

        [JsonIgnore]
        private string _key { get; set; }

        [JsonIgnore]
        private string _fileTitle = string.Empty;
        [JsonIgnore]
        public string FileTitle
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_fileTitle))
                {
                    var index = _file.Path.LastIndexOf('\\');
                    _fileTitle = _file.Path.Substring(index + 1);
                }

                return _fileTitle;
            }
        }

        [JsonIgnore]
        static Random rnd = new Random();

        [JsonRequired]
        // Used to randomly pad the encrypted file, to throw off attackers
        private string _internalPadding1 { get; } = EncryptionService.GetRandomString(rnd.Next(25, 125));


        private ObservableCollection<VaultFileEntry> _entries;
        public ObservableCollection<VaultFileEntry> Entries
        {
            get { return _entries; }
            set { Set(ref _entries, value); }
        }

        [JsonRequired]
        // Used to randomly pad the encrypted file, to throw off attackers
        private string _internalPadding2 { get; } = EncryptionService.GetRandomString(rnd.Next(25, 125));


        public async Task<bool> Decrypt(string key)
        {
            string encrTxt = string.Empty;
            using (var fileStream = await _file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(fileStream))
                    encrTxt = await reader.ReadToEndAsync();
            
            var decrText = EncryptionService.Decrypt(encrTxt, key);

            if (string.IsNullOrEmpty(decrText))
                return false;
            else
            {
                // Success in decryption, save key & deserialize ...
                _key = key;

                var vaultFile = JsonConvert.DeserializeObject<VaultFile>(decrText);
                VaultID = vaultFile.VaultID;
                Entries = vaultFile.Entries;

                return true;
            }
        }

        public async Task EncryptAndSave()
        {
            // Serialize VaultFile
            var serVault = JsonConvert.SerializeObject(this);

            // Encrypt serialized object
            string encrStr = EncryptionService.Encrypt(serVault, _key);

            using (var fileStream = await _file.OpenStreamForWriteAsync())
            {
                // Deletes any content already present in file
                fileStream.SetLength(0);

                using (var writer = new StreamWriter(fileStream))
                    await writer.WriteAsync(encrStr);
            }
        }

        public void AddNewEntry(VaultFileEntry entry)
        {
            if (Entries == null)
                Entries = new ObservableCollection<VaultFileEntry>();

            Entries.Add(entry);
        }
    }

    public class MockVaultFile : BindableBase, IVaultFile
    {
        public MockVaultFile()
        {
            Entries = new ObservableCollection<VaultFileEntry>
            {
                new VaultFileEntry { Username = "Myusername", Password = "password123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now, Title = "Ebay" },
                new VaultFileEntry { Username = "Myuser1", Password = "password123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now.AddMinutes(-2), ModifiedAt = DateTime.Now.AddDays(1), Title = "Amazon" },
                new VaultFileEntry { Username = "Myuser2", Password = "passwfawfaword123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now.AddMinutes(-5), ModifiedAt = DateTime.Now.AddDays(2), Title = "Gmail" },
                new VaultFileEntry { Username = "Myuser3", Password = "passwordawdwad123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now.AddDays(3), Title = "Intranet" },
                new VaultFileEntry { Username = "Myusern5", Password = "passworddwad123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now.AddDays(4), Title = "Main" },
                new VaultFileEntry { Username = "Myusern6783", Password = "3213135fdasfa", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now.AddDays(5), Title = "Facebook" },
                new VaultFileEntry { Username = "Myusern121", Password = "passwor3132131d123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now.AddDays(6), Title = "Hotmail" },
                new VaultFileEntry { Username = "Myuser321e", Password = "passwor11d123", Notes = "Lorem ipsum diolti watl", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now.AddDays(7), Title = "Wut" }
            };
        }

        private ObservableCollection<VaultFileEntry> _entries;
        public ObservableCollection<VaultFileEntry> Entries
        {
            get { return _entries; }
            set { Set(ref _entries, value); }
        }

        public Task EncryptAndSave()
        {
            return Task.CompletedTask;
        }

        public void AddNewEntry(VaultFileEntry entry)
        {
        }
    }
}
