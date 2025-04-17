using DTube.Common;
using DTube.Common.Helper;
using DTube.Common.Models;
using DTube.Common.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DTube.Controllers
{
    public class MainViewModelController(
        YTService ytService,
        ConfigManager configManager,
        AppDataContext appDataContext
        )
    {
        public async Task GetMediaAsync(string url)
        {
            MediaMetaDataModel media = await ytService.GetMediaByLinkAsync(url);
            appDataContext.SetCurrentMedia(media);
        }

        public async Task DownloadMusicAsync(string url)
        {
            string tmpFileName = Guid.NewGuid().ToString();

            if (!Directory.Exists(Constants.FolderPath))
                Directory.CreateDirectory(Constants.FolderPath);

            string tmpFilePath = await ytService.DownloadMusicAsync(url, tmpFileName, Constants.FolderPath);

            if (!Directory.Exists(configManager.Config.Media.ContentFolderPath))
                Directory.CreateDirectory(configManager.Config.Media.ContentFolderPath);

            string mp3FilePath = await MediaConvertHelper.ConvertToMP3(tmpFilePath);
            File.Delete(tmpFilePath);

            string filePath = Path.Combine(configManager.Config.Media.ContentFolderPath, Path.GetFileName(mp3FilePath));
            File.Move(mp3FilePath, filePath);
            appDataContext.CurrentMedia!.FilePath = filePath;

            appDataContext.AddMedia(appDataContext.CurrentMedia!);
        }
    }
}
