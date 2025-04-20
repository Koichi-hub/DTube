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
        ImageService imageService,
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
            if (!Directory.Exists(Constants.TMPFolderPath))
                Directory.CreateDirectory(Constants.TMPFolderPath);

            if (!Directory.Exists(configManager.Config.Media.ContentFolderPath))
                Directory.CreateDirectory(configManager.Config.Media.ContentFolderPath);

            if (!Directory.Exists(Constants.MediaPreviewImageFolderPath))
                Directory.CreateDirectory(Constants.MediaPreviewImageFolderPath);

            string tmpFileName = "audio_" + Guid.NewGuid().ToString();
            string tmpFilePath = await ytService.DownloadMediaAsync(url, tmpFileName, Constants.TMPFolderPath);

            string mp3FilePath = await MediaHelper.ConvertToMP3(tmpFilePath);
            File.Delete(tmpFilePath);

            string filePath = Path.Combine(configManager.Config.Media.ContentFolderPath, Path.GetFileName(mp3FilePath));
            File.Move(mp3FilePath, filePath);
            appDataContext.CurrentMedia!.FilePath = filePath;

            appDataContext.CurrentMedia.PreviewFilePath = await imageService.DownloadAsync(
                url: appDataContext.CurrentMedia.PreviewSourceUrl, 
                fileName: Path.GetFileNameWithoutExtension(filePath), 
                outputDirectory: Constants.MediaPreviewImageFolderPath);

            appDataContext.CurrentMedia.Type = Common.Enums.MediaType.Music;
            appDataContext.AddMedia(appDataContext.CurrentMedia);
        }

        public async Task DownloadVideoAsync(string url)
        {
            if (!Directory.Exists(Constants.TMPFolderPath))
                Directory.CreateDirectory(Constants.TMPFolderPath);

            if (!Directory.Exists(configManager.Config.Media.ContentFolderPath))
                Directory.CreateDirectory(configManager.Config.Media.ContentFolderPath);

            if (!Directory.Exists(Constants.MediaPreviewImageFolderPath))
                Directory.CreateDirectory(Constants.MediaPreviewImageFolderPath);

            string guid = Guid.NewGuid().ToString();
            string audioFileName = "audio_" + guid;
            string videoFileName = "video_" + guid;

            string tmpAudioFilePath = await ytService.DownloadMediaAsync(url, audioFileName, Constants.TMPFolderPath);
            string tmpVideoFilePath = await ytService.DownloadMediaAsync(url, videoFileName, Constants.TMPFolderPath, isAudio: false);

            string fullVideoFilePath = await MediaHelper.CombineVideoAndAudio(tmpVideoFilePath, tmpAudioFilePath);
            File.Delete(tmpAudioFilePath);
            File.Delete(tmpVideoFilePath);

            string filePath = Path.Combine(configManager.Config.Media.ContentFolderPath, Path.GetFileName(fullVideoFilePath));
            File.Move(fullVideoFilePath, filePath);
            appDataContext.CurrentMedia!.FilePath = filePath;

            appDataContext.CurrentMedia.PreviewFilePath = await imageService.DownloadAsync(
                url: appDataContext.CurrentMedia.PreviewSourceUrl,
                fileName: Path.GetFileNameWithoutExtension(filePath),
                outputDirectory: Constants.MediaPreviewImageFolderPath);

            appDataContext.CurrentMedia.Type = Common.Enums.MediaType.Video;
            appDataContext.AddMedia(appDataContext.CurrentMedia);
        }
    }
}
