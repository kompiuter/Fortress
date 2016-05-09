using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress.Models
{
    public interface IVaultFile
    {
        ObservableCollection<VaultFileEntry> Entries { get; set; }
        Task EncryptAndSave();
        void AddNewEntry(VaultFileEntry entry);
    }
}
