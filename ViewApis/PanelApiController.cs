using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegineDesignAdmin.LogicLayer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RegineDesignAdmin.ViewApis
{
    [SkipStatusCodePages]
    [Route("local/[controller]/[action]")]
    [Authorize]
    public class PanelApiController : Controller
    {
        private GoogleStorageLL _googleStorageLL;

        public PanelApiController(GoogleStorageLL googleStorageLL)
        {
            _googleStorageLL = googleStorageLL;
        }

        public async Task<IActionResult> GetAllFolder()
        {
            var allFolder = await _googleStorageLL.GetAllFoldersAsync();
            return Ok(allFolder);
        }

        [HttpGet("{path}")]
        public async Task<IActionResult> GetAllObjectInFolder(string path)
        {
            var allItemInFolder = await _googleStorageLL.GetAllObjectInFolderAsync(path);
            return Ok(allItemInFolder);
        }

        [HttpPost("{folderName}")]
        public async Task<IActionResult> CreateFolder(string folderName)
        {
            var result = await _googleStorageLL.CreateNewFolder(folderName);
            return Ok(result);
        }

        [HttpPut("{oldFolderName}/{newFolderName}")]
        public async Task<IActionResult> RenameFolder(string oldFolderName, string newFolderName)
        {
            var result = await _googleStorageLL.UpdateFolderName(oldFolderName, newFolderName);
            return Ok(result);
        }

        [HttpDelete("{folder}")]
        public async Task<IActionResult> DeleteFolder(string folder)
        {
            var result = await _googleStorageLL.DeleteFolder(folder);
            return Ok(result);
        }

        [HttpDelete("{folderName}/{file}")]
        public async Task<IActionResult> DeleteFile(string folderName, string file)
        {
            var result = await _googleStorageLL.DeleteFile(folderName+'/'+file);
            return Ok(result);
        }

        [HttpPost("{path}")]
        public async Task<IActionResult> SaveUploadedFile(string path, [FromForm]IFormFile file)
        {

            var result = await _googleStorageLL.UploadFileToFolder(path, file);
            if (result)
            {
                return Ok("Done");
            }
            else {
                return NotFound();
            }
        }

        [HttpPut("{folderName}/{file}/{videoURL}")]
        public async Task<IActionResult> AddVideoLink(string folderName, string file, string videoURL)
        {
            string decodedUrl = HttpUtility.UrlDecode(videoURL);
            var result = await _googleStorageLL.AddMetadataToFile(folderName + '/' + file, decodedUrl);
            return Ok(result);
        }
    }
}
