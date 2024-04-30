using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using System.Threading.Tasks;
using LLMS.Service;
using System;

public class AzureBlobStorageClient : IAzureBlobStorageClient
{
    private readonly CloudBlobClient _blobClient;

    public AzureBlobStorageClient(string connectionString)
    {
        try
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }
        catch (FormatException)
        {
            throw new ApplicationException("Azure Storage connection string is malformed.");
        }
        catch (ArgumentException)
        {
            throw new ApplicationException("Azure Storage connection string is null or empty.");
        }
    }

    public async Task<string> UploadFileAsync(string containerName, string blobName, Stream fileStream)
    {
        try
        {
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            var blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.UploadFromStreamAsync(fileStream);
            return blockBlob.Uri.ToString();
        }
        catch (StorageException ex)
        {
            throw new ApplicationException($"An error occurred while uploading to Azure Blob Storage: {ex.Message}", ex);
        }
    }

    // Add more methods for other operations like downloading, listing blobs, etc.
}
