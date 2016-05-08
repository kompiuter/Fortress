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
        public VaultFileEntry()
        {
            ID = Guid.NewGuid();
        }
           
        public VaultFileEntry Copy() => (VaultFileEntry)MemberwiseClone();

        public readonly Guid ID;
        public string Title { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
