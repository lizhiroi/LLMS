using System.IO;
using System.Threading.Tasks;

namespace LLMS.Service
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string imageName);
        Task<int> GetImageIdByUrlAsync(string imageUrl);
        Task<string> GetImageUrlByIdAsync(int imageId);
        Task<int> CreateImageRecordAsync(string imageUrl);
    }
}
