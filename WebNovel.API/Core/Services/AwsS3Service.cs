using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace WebNovel.API.Core.Services
{
    public interface IAwsS3Service
    {
        Task<string> UploadToS3(IFormFile file, string fileName, string folder);
        Task<Stream> GetFile(string folder, string fileName);
        Task DeleteFromS3(string folder, List<string> objects);
        string GetFileImg(string folder, string fileName);
    }
    public class AwsS3Service : IAwsS3Service
    {
        private readonly IConfiguration _configuration;

        private TransferUtility fileTransferUtility = null;

        public AwsS3Service(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private TransferUtility GetFileTransfer()
        {
            var awsConfig = _configuration.GetSection("Aws");
            var credentials = new BasicAWSCredentials(awsConfig["AccessKeyId"], awsConfig["SecretAccessKey"]);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfig["Region"])
            };
            var client = new AmazonS3Client(credentials, config);
            return new TransferUtility(client);
        }
        
        public async Task DeleteFromS3(string folder, List<string> objects)
        {
            var files = new List<KeyVersion>();
            foreach (var file in objects)
            {
                files.Add(new KeyVersion() {Key = $"{folder}/{file}"});
            }
            var awsConfig = _configuration.GetSection("Aws");
            if (fileTransferUtility == null)
            {
                fileTransferUtility = GetFileTransfer();
            }
            Task t = fileTransferUtility.S3Client.DeleteObjectsAsync(new DeleteObjectsRequest()
            {
                BucketName = awsConfig["BucketName"],
                Objects = files
            });
        }

        public async Task<Stream> GetFile(string folder, string fileName)
        {
            var awsConfig = _configuration.GetSection("Aws");

            if (fileTransferUtility == null)
            {
                fileTransferUtility = GetFileTransfer();
            }
            GetObjectResponse file = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
            {
                BucketName = awsConfig["BucketName"],
                Key = $"{folder}/{fileName}",
            });
            Stream responseStream = file.ResponseStream;
            MemoryStream stream = new MemoryStream();
            responseStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public async Task<string> UploadToS3(IFormFile file, string fileName, string folder)
        {
            var awsConfig = _configuration.GetSection("Aws");
            if (fileTransferUtility == null)
            {
                fileTransferUtility = GetFileTransfer();
            }
            string bucketName = awsConfig["BucketName"];
            MemoryStream newMemoryStream = new MemoryStream();
            await file.CopyToAsync(newMemoryStream);
            fileName = $"{folder}/{fileName}";
            TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = fileName,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead
            };

            await fileTransferUtility.UploadAsync(uploadRequest);
            return $"https://{awsConfig["BucketName"]}.s3-{awsConfig["Region"]}.amazonaws.com/{folder}/{fileName}";
        }

        public string GetFileImg(string folder, string fileName)
        {
            var awsConfig = _configuration.GetSection("Aws");
            return $"https://{awsConfig["BucketName"]}.s3-{awsConfig["Region"]}.amazonaws.com/{fileName}";
        }
    }
}