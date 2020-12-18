
using System.Collections.Generic;

namespace RegineDesignAdmin.Models
{
    public class PublicViewModel
    {
        public Dictionary<string, List<File>> AllFiles { get; set; }
        public List<string> Folders { get; set; }

    }
}
