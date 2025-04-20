using DTube.Common.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace DTube.Common.Services
{
    public class YTService
    {
        private readonly YoutubeClient youtubeClient = new();

        public async Task<MediaMetaDataModel> GetMediaByLinkAsync(string url)
        {
            Video video = await youtubeClient.Videos.GetAsync(url);
            Thumbnail videoThumbnail = video.Thumbnails.GetWithHighestResolution();

            var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(url);

            List<AudioOnlyStreamInfo> AudioOnlyStreamInfos = streamManifest
                .GetAudioOnlyStreams()
                .OrderByDescending(s => s.Bitrate)
                .ToList();

            List<VideoOnlyStreamInfo> VideoOnlyStreamInfos = streamManifest
                .GetVideoOnlyStreams()
                .OrderByDescending(s => s.VideoQuality)
                .ToList();

            long sizeInBytes = 0;
            if (AudioOnlyStreamInfos.Count > 0)
            {
                sizeInBytes = AudioOnlyStreamInfos.First().Size.Bytes;
            }
            else if (VideoOnlyStreamInfos.Count > 0)
            {
                sizeInBytes = VideoOnlyStreamInfos.First().Size.Bytes;
            }

            return new MediaMetaDataModel
            {
                SourceUrl = url,
                PreviewSourceUrl = videoThumbnail.Url,
                Title = video.Title,
                Description = video.Description,
                SizeInBytes = sizeInBytes,
                Duration = video.Duration!.Value.Seconds,
            };
        }

        public async Task<string> DownloadMediaAsync(string url, string fileName, string outputDirectory, bool isAudio = true)
        {
            var video = await youtubeClient.Videos.GetAsync(url);

            var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
            IStreamInfo streamInfo;

            if (isAudio)
                streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            else
                streamInfo = streamManifest.GetVideoOnlyStreams().GetWithHighestBitrate();

            string filePath = Path.Combine(outputDirectory, $"{fileName}.{streamInfo.Container}");
            await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath);

            return filePath;
        }
    }
}
