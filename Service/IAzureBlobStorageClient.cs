using System.IO;
using System.Threading.Tasks;

namespace LLMS.Service
{
    public interface IAzureBlobStorageClient
    {
        Task<string> UploadFileAsync(string containerName, string blobName, Stream fileStream);
        // Other Methods
    }
}
