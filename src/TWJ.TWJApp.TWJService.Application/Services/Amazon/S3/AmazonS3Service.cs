using TWJ.TWJApp.TWJService.Application.Interfaces.Amazon;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.S3.Model;
using Amazon.Runtime;
using System;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Amazon.S3
{
    public class AmazonS3Service : IAmazonS3Service
    {
        private string bucketName;
        private string arn;
        private string amazonS3URL;
        private string accessKey;
        private string secretAccessKey;
        private string currentClassName;
        private IGlobalHelperService _globalHelperService;
        public AmazonS3Service(IConfiguration configuration, IGlobalHelperService globalHelperService) 
        {
            bucketName = configuration["Amazon:S3:Bucket"];
            arn = configuration["Amazon:S3:ARN"];
            amazonS3URL = configuration["Amazon:S3:URL"];
            accessKey = configuration["Amazon:S3:AccessKey"];
            secretAccessKey = configuration["Amazon:S3:SecretAccessKey"];
            _globalHelperService = globalHelperService;
            currentClassName = GetType().Name;
        }
        public async Task<string> UploadFileToS3Async(string localFilePath, string subDirectoryInBucket, string fileNameInS3)
        {
            try
            {
                var awsCredentials = new BasicAWSCredentials(accessKey, secretAccessKey);

                using (IAmazonS3 client = new AmazonS3Client(awsCredentials, RegionEndpoint.EUNorth1))
                {
                    TransferUtility utility = new TransferUtility(client);
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = (string.IsNullOrEmpty(subDirectoryInBucket) ? "" : subDirectoryInBucket + "/") + fileNameInS3,
                        FilePath = localFilePath,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    if (fileNameInS3.EndsWith(".jpg") || fileNameInS3.EndsWith(".jpeg"))
                    {
                        request.ContentType = "image/jpeg";
                    }
                    else if (fileNameInS3.EndsWith(".png"))
                    {
                        request.ContentType = "image/png";
                    }

                    await utility.UploadAsync(request);
                }

                string fileUrl = $"{amazonS3URL}/{(string.IsNullOrEmpty(subDirectoryInBucket) ? "" : subDirectoryInBucket + "/")}{fileNameInS3}";

                return fileUrl;
            }
            catch (Exception ex)
            {
                await _globalHelperService.Log(ex, currentClassName);
                throw ex;
            }
        }

    }
}
