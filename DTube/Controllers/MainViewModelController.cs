using DTube.Common;
using DTube.Common.Helper;
using DTube.Common.Models;
using DTube.Common.Services;
using Newtonsoft.Json;
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
            MediaMetaDataView media = await ytService.GetMediaByLinkAsync(url);
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

            Guid mediaId = Guid.NewGuid();
            string tmpFileName = "audio_" + mediaId.ToString();
            string tmpFilePath = await ytService.DownloadMediaAsync(url, tmpFileName, Constants.TMPFolderPath);

            string mp3FilePath = await MediaHelper.ConvertToMP3(tmpFilePath);
            File.Delete(tmpFilePath);

            string filePath = Path.Combine(configManager.Config.Media.ContentFolderPath, Path.GetFileName(mp3FilePath));
            File.Move(mp3FilePath, filePath);

            FileInfo fileInfo = new(filePath);
            MediaMetaData media = appDataContext.CurrentMedia!;

            media.SizeInBytes = fileInfo.Length;
            media.FilePath = filePath;
            media.PreviewFilePath = await imageService.DownloadAsync(
                url: media.PreviewSourceUrl, 
                fileName: Path.GetFileNameWithoutExtension(filePath), 
                outputDirectory: Constants.MediaPreviewImageFolderPath);

            MediaHelper.AddMP3Tags(media);

            media.Id = mediaId;
            media.Type = Common.Enums.MediaType.Music;
            appDataContext.AddMedia(media);
        }

        public async Task DownloadVideoAsync(string url)
        {
            if (!Directory.Exists(Constants.TMPFolderPath))
                Directory.CreateDirectory(Constants.TMPFolderPath);

            if (!Directory.Exists(configManager.Config.Media.ContentFolderPath))
                Directory.CreateDirectory(configManager.Config.Media.ContentFolderPath);

            if (!Directory.Exists(Constants.MediaPreviewImageFolderPath))
                Directory.CreateDirectory(Constants.MediaPreviewImageFolderPath);

            Guid mediaId = Guid.NewGuid();
            string audioFileName = "audio_" + mediaId.ToString();
            string videoFileName = "video_" + mediaId.ToString();

            string tmpAudioFilePath = await ytService.DownloadMediaAsync(url, audioFileName, Constants.TMPFolderPath);
            string tmpVideoFilePath = await ytService.DownloadMediaAsync(url, videoFileName, Constants.TMPFolderPath, isAudio: false);

            string fullVideoFilePath = await MediaHelper.CombineVideoAndAudio(tmpVideoFilePath, tmpAudioFilePath);
            File.Delete(tmpAudioFilePath);
            File.Delete(tmpVideoFilePath);

            string filePath = Path.Combine(configManager.Config.Media.ContentFolderPath, Path.GetFileName(fullVideoFilePath));
            File.Move(fullVideoFilePath, filePath);

            FileInfo fileInfo = new(filePath);
            MediaMetaData media = JsonConvert.DeserializeObject<MediaMetaData>(JsonConvert.SerializeObject(appDataContext.CurrentMedia!))!;

            media.SizeInBytes = fileInfo.Length;
            media!.FilePath = filePath;
            media.PreviewFilePath = await imageService.DownloadAsync(
                url: media.PreviewSourceUrl,
                fileName: Path.GetFileNameWithoutExtension(filePath),
                outputDirectory: Constants.MediaPreviewImageFolderPath);

            media.Id = mediaId;
            media.Type = Common.Enums.MediaType.Video;
            appDataContext.AddMedia(media);
        }

        public void DeleteMediaAsync(MediaMetaData media)
        {
            File.Delete(media.FilePath);
            File.Delete(media.PreviewFilePath);
            appDataContext.DeleteMedia(media);
        }
    }
}
