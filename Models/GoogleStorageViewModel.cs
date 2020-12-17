using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegineDesignAdmin.Models
{
    public class GoogleStorageViewModel
    {
        public List<File> Files { get; set; }
        public List<string> Folders { get; set; }

    }

    public class File
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MediaLink { get; set; }

    }
}
