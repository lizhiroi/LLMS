using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LLMS.Service
{
    internal class ImageService : IImageService
    {
        private readonly IAzureBlobStorageClient _blobStorageClient;

        public ImageService(IAzureBlobStorageClient blobStorageClient)
        {
            _blobStorageClient = blobStorageClient;
        }

        public async Task<int> GetImageIdByUrlAsync(string imageUrl)
        {
            try
            {
                using (var context = new testdb1Entities())
                {
                    var image = await context.images.FirstOrDefaultAsync(img => img.image_url == imageUrl);
                    if (image == null)
                    {
                        return 0;
                    }
                    return image.id;
                }
            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in GetImageIdByUrlAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in GetImageIdByUrlAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.");
            }
        }

        public async Task<string> GetImageUrlByIdAsync(int imageId)
        {
            try
            {
                using (var context = new testdb1Entities())
                {
                    var image = await context.images.FindAsync(imageId);
                    return image?.image_url;
                }
            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in GetImageUrlByIdAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in GetImageUrlByIdAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.");
            }
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string imageName)
        {
            string containerName = "fsd10-demo-blob";
            try
            {
                 string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                 string uniqueImageName = $"{Path.GetFileNameWithoutExtension(imageName)}_{timestamp}{Path.GetExtension(imageName)}";

                return await _blobStorageClient.UploadFileAsync(containerName, uniqueImageName, imageStream);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while uploading the image: {ex.Message}", ex);
            }
        }

        public async Task<int> CreateImageRecordAsync(string imageUrl)
        {
            using (var context = new testdb1Entities())
            {
                var imageRecord = new image
                {
                    image_url = imageUrl,
                    description = ExtractImageNameFromUrl(imageUrl),
                    uploaded_at = DateTime.UtcNow
                };
                context.images.Add(imageRecord);
                await context.SaveChangesAsync();

                return imageRecord.id; 
            }
        }

        private string ExtractImageNameFromUrl(string imageUrl)
        {
            Uri uri = new Uri(imageUrl);
            string imageName = uri.Segments.Last(); 
            return imageName;
        }
    }
}
