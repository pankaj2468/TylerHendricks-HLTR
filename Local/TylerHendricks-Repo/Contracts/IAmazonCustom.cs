using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TylerHendricks_Repo.Contracts
{
    public interface IAmazonCustom:IDisposable
    {
        Task<string> UploadFileToS3(IFormFile file, string bucketName, string accessKeyId, string secretAccessKey, string directory);
        Task<string> UploadFileToS3(IFormFile file, string fileName, string bucketName, string accessKeyId, string secretAccessKey, string directory);
        Task<string> GetImage(string s3fileLocation, string bucketName, string accessKeyId, string secretAccessKey);
        Task<string> GetTemplate(string s3fileLocation, string bucketName, string accessKeyId, string secretAccessKey);
    }
}
