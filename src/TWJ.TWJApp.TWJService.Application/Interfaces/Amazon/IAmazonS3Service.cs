using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Interfaces.Amazon
{
    public interface IAmazonS3Service
    {
        Task<string> UploadFileToS3Async(string localFilePath, string subDirectoryInBucket, string fileNameInS3);
    }
}
