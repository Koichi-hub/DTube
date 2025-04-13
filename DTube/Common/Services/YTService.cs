using DTube.Common.Models;
using System;
using System.Collections.Generic;
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
                SourceUrl = videoThumbnail.Url,
                Title = video.Title,
                Description = video.Description,
                SizeInBytes = sizeInBytes,
                Duration = video.Duration!.Value.Seconds,
            };
        }
    }
}
