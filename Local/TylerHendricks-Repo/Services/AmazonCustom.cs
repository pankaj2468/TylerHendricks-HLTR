using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Utility;
namespace TylerHendricks_Repo.Services
{
    public class AmazonCustom : IAmazonCustom
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {

        }

        public async Task<string> UploadFileToS3(IFormFile file, string bucketName, string accessKeyId, string secretAccessKey, string directory="")
        {
            try
            {
                string location = bucketName;
                string fileName = DateTime.UtcNow.Ticks.ToString()+"-"+ file.FileName.Replace(" ", "-");
                string returnPath = fileName;
                if (!string.IsNullOrEmpty(directory))
                {
                    directory = directory.TrimStart('/');
                    directory = directory.TrimEnd('/');
                    location += "/"+directory;
                    returnPath = directory + "/"+ fileName;
                }
                using (var client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.USEast2))
                {
                    using (var newMemoryStream = new MemoryStream())
                    {
                        file.CopyTo(newMemoryStream);
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = newMemoryStream,
                            Key = fileName,
                            BucketName = location,
                            CannedACL = S3CannedACL.PublicRead,
                        };
                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                    }
                }
                return returnPath;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> UploadFileToS3(IFormFile file, string fileName, string bucketName, string accessKeyId, string secretAccessKey, string directory = "")
        {
            try
            {
                string location = bucketName;
                fileName = fileName + Path.GetExtension(file.FileName);
                string returnPath = fileName;
                if (!string.IsNullOrEmpty(directory))
                {
                    directory = directory.TrimStart('/');
                    directory = directory.TrimEnd('/');
                    location += "/" + directory;
                    returnPath = directory + "/" + fileName;
                }
                using (var client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.USEast2))
                {
                    using (var newMemoryStream = new MemoryStream())
                    {
                        file.CopyTo(newMemoryStream);
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = newMemoryStream,
                            Key = fileName,
                            BucketName = location,
                            CannedACL = S3CannedACL.PublicRead,
                        };
                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                    }
                }
                return returnPath;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetImage(string s3fileLocation,string bucketName, string accessKeyId, string secretAccessKey)
        {
            try
            {
                string directory = string.Empty;
                string keyName = string.Empty;
                string base64 = string.Empty;
                s3fileLocation = s3fileLocation.TrimStart('/');
                s3fileLocation = s3fileLocation.TrimEnd('/');
                if (s3fileLocation.Contains("/"))
                {
                    keyName = s3fileLocation.Split('/')[s3fileLocation.Split('/').Length-1];
                    directory = s3fileLocation.Replace("/"+keyName, "");
                    bucketName = bucketName + "/"+ directory;
                }
                else
                {
                    keyName = s3fileLocation;
                }
                using (var client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.USEast2))
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = keyName,
                    };
                    using (GetObjectResponse response = await client.GetObjectAsync(request).ConfigureAwait(true))
                    using (Stream responseStream = response.ResponseStream)
                    {
                        base64 = Comman.ConvertToBase64(responseStream);
                    }
                }
                string contentType = "";
                new FileExtensionContentTypeProvider().TryGetContentType(keyName, out contentType);
                return "data:" + contentType + ";base64," + base64;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<string> GetTemplate(string s3fileLocation, string bucketName, string accessKeyId, string secretAccessKey)
        {
            try
            {
                string content = string.Empty;
                string directory = string.Empty;
                string keyName = string.Empty;
                string base64 = string.Empty;
                s3fileLocation = s3fileLocation.TrimStart('/');
                s3fileLocation = s3fileLocation.TrimEnd('/');
                if (s3fileLocation.Contains("/"))
                {
                    keyName = s3fileLocation.Split('/')[s3fileLocation.Split('/').Length - 1];
                    directory = s3fileLocation.Replace("/" + keyName, "");
                    bucketName = bucketName + "/" + directory;
                }
                else
                {
                    keyName = s3fileLocation;
                }
                using (var client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.USEast2))
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = keyName,
                    };
                    using (GetObjectResponse response = await client.GetObjectAsync(request).ConfigureAwait(true))
                    using (StreamReader responseStream = new StreamReader(response.ResponseStream))
                    {
                        content = responseStream.ReadToEnd();
                    }
                }
                return content;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
