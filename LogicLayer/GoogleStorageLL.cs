using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using RegineDesignAdmin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using File = RegineDesignAdmin.Models.File;

namespace RegineDesignAdmin.LogicLayer
{
    public class GoogleStorageLL
    {
        public static StorageClient client;
        public static UrlSigner urlSigner;

        public static string selectedBucket;
        public static void GetBucket(string json, string bucketName, string clientEmail, string privateKey)
        {
            var credential = GoogleCredential.FromJson(json);
            var serviceAccount = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(clientEmail)
            {
                Scopes = new[] {
                    StorageService.Scope.CloudPlatformReadOnly
                }
            }.FromPrivateKey(privateKey.Replace("\\n", "\n")));
            urlSigner = UrlSigner.FromServiceAccountCredential(serviceAccount);
            client = StorageClient.Create(credential);
            selectedBucket = bucketName;
        }

        public async Task<List<string>> GetAllFoldersAsync()
        {
            Google.Apis.Storage.v1.Data.Objects response = await GetResponseFromSource();
            if (response != null)
            {
                return (List<string>)response.Prefixes;
            }
            else{
                return null;
            }

        }

        public async Task<bool> CreateNewFolder(string folderName)
        {
            try
            {
                var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(""));
                var result = await client.Service.Objects.Insert(
                    bucket: selectedBucket,
                    stream: uploadStream,
                    contentType: "application/x-directory",
                    body: new Google.Apis.Storage.v1.Data.Object() { Name = folderName + '/' }
                    ).UploadAsync();
                
                if(result.Status.ToString().ToLower() == "completed") {
                    return true;
                }

                return false;
            }
            catch (Exception e) {
                return false;
            }
        }

        public async Task<bool> DeleteFolder(string folderName)
        {
            try
            {
                Google.Apis.Storage.v1.Data.Objects response = await GetResponseFromSource(folderName + '/');
                if (response != null && response.Items != null)
                {
                    foreach (var item in response.Items)
                    {
                        await client.DeleteObjectAsync(item);
                    }

                }
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFile(string fileName)
        {
            try
            {
                await client.DeleteObjectAsync(selectedBucket, fileName);
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> AddMetadataToFile(string fileName, string videoUrl)
        {
            try
            {
                var item = await client.GetObjectAsync(selectedBucket, fileName);
                item.Metadata = new Dictionary<string, string>();
                item.Metadata.Add("video", videoUrl);
                await client.UpdateObjectAsync(item);
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateFolderName(string oldFolderName, string newFolderName)
        {
            try
            {

                Google.Apis.Storage.v1.Data.Objects response = await GetResponseFromSource(oldFolderName + '/');
                if (response.Items != null)
                {
                    foreach (var item in response.Items)
                    {

                        await client.CopyObjectAsync(selectedBucket, item.Name, selectedBucket, item.Name.Replace(oldFolderName, newFolderName));
                        
                    }
                }
                return await DeleteFolder(oldFolderName);
            }
            catch (Exception e) {
                return false;
            }

        }

        public async Task<Google.Apis.Storage.v1.Data.Objects> GetResponseFromSource(string path = null) {
            try
            {
                var storageService = client.Service;
                ObjectsResource.ListRequest request = storageService.Objects.List(selectedBucket);
                request.Delimiter = "/";
                if (path != null)
                {
                    request.Prefix = path;
                }
                return await request.ExecuteAsync();
            }
            catch (Exception e) {
                return null;
            }
        }

        public async Task<bool> UploadFileToFolder(string path, IFormFile file) {
            try
            {
                var result = await client.Service.Objects.Insert(
                    bucket: selectedBucket,
                    stream: file.OpenReadStream(),
                    contentType: file.ContentType,
                    body: new Google.Apis.Storage.v1.Data.Object() { Name = path + '/' +file.FileName}
                    ).UploadAsync();

                if (result.Status.ToString().ToLower() == "completed")
                {
                    return true;
                }

                return false;
            }
            catch (Exception e) {
                return false;
            }
        }

        public async Task<GoogleStorageViewModel> GetAllObjectInFolderAsync(string path)
        {
            try
            {
                var fullPath = path + "/"; ;

                Google.Apis.Storage.v1.Data.Objects response = await GetResponseFromSource(fullPath);
                GoogleStorageViewModel vm = new GoogleStorageViewModel();
                if (response.Prefixes != null)
                {
                    vm.Folders = new List<string>();
                    foreach (var folder in response.Prefixes)
                    {
                        vm.Folders.Add(folder.Replace(fullPath, "").ToString().Trim('/'));
                    }
                }
                if (response.Items != null)
                {
                    vm.Files = new List<File>();
                    foreach (var item in response.Items)
                    {
                        if (item.ContentType.Contains("image"))
                        {
                            File f = new File();
                            f.Id = item.Id.ToString();
                            f.MediaLink = await urlSigner.SignAsync(selectedBucket, item.Name, TimeSpan.FromDays(1));
                            f.Name = item.Name.Replace(fullPath, "").ToString();
                            if (item.Metadata != null && item.Metadata.ContainsKey("video"))
                            {
                                item.Metadata.TryGetValue("video", out string videoLink);
                                f.VideoLink = videoLink;
                            }
                            else {
                                f.VideoLink = null;
                            }

                            vm.Files.Add(f);
                        }
                    }
                }
                return vm;
            }
            catch (Exception e) {
                return null;
            }

        }
    }
}
