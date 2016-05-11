using Fortress.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Fortress.Models
{
    public class VaultFileEntry
    {
        [JsonIgnore]
        public bool IsNewEntry { get; set; } = false;
        public readonly Guid ID = Guid.NewGuid();
        public string Title { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public VaultFileEntry Copy() => (VaultFileEntry)MemberwiseClone();
    }
}
