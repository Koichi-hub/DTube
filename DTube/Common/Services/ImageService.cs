using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DTube.Common.Services
{
    public class ImageService(IHttpClientFactory httpClientFactory)
    {
        public async Task<string> DownloadAsync(string url, string fileName, string outputDirectory)
        {
            using HttpClient client = httpClientFactory.CreateClient();
            byte[] bytes = await client.GetByteArrayAsync(url);
            string tmpImageFilePath = Path.Combine(Constants.TMPFolderPath, Guid.NewGuid().ToString());
            await File.WriteAllBytesAsync(tmpImageFilePath, bytes);

            string previewFilePath = string.Empty;
            using (Image image = await Image.LoadAsync(tmpImageFilePath))
            {
                previewFilePath = Path.Combine(Constants.MediaPreviewImageFolderPath, fileName + ".jpeg");
                await image.SaveAsync(previewFilePath, new JpegEncoder());
            }

            File.Delete(tmpImageFilePath);

            return previewFilePath;
        }
    }
}
