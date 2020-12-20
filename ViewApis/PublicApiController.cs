using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegineDesignAdmin.LogicLayer;
using RegineDesignAdmin.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RegineDesignAdmin.ViewApis
{
    [SkipStatusCodePages]
    [Route("[controller]/[action]")]
    public class PublicApiController : Controller
    {
        private GoogleStorageLL _googleStorageLL;

        public PublicApiController(GoogleStorageLL googleStorageLL)
        {
            _googleStorageLL = googleStorageLL;
        }

        public async Task<IActionResult> GetAllItemInStorage() {
            PublicViewModel vm = new PublicViewModel();
            vm.Folders = new List<string>();
            vm.AllFiles = new Dictionary<string, List<Models.File>>();
            var allFolders = await _googleStorageLL.GetAllFoldersAsync();
            foreach (var oneFolder in allFolders) { 
                string path = oneFolder.TrimEnd('/');
                if (!path.Contains("Secret"))
                {
                    var allItemInFolder = await _googleStorageLL.GetAllObjectInFolderAsync(path);
                    if (allItemInFolder.Files.Count != 0)
                    {
                        vm.Folders.Add(path);
                        vm.AllFiles.Add(path, allItemInFolder.Files);
                    }
                }
            }
            return Ok(vm);
        }

    }
}
