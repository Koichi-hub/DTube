using ATL;
using DTube.Common.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace DTube.Common.Helper
{
    public static class MediaHelper
    {
        public static async Task<string> ConvertToMP3(string inputFilePath)
        {
            FFmpeg.SetExecutablesPath("C:\\Users\\gogot\\Utils\\ffmpeg-2025-04-17-git-7684243fbe-full_build\\bin");
            string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath)!, Path.GetFileNameWithoutExtension(inputFilePath) + ".mp3");
            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(inputFilePath, outputFilePath);
            await conversion.Start();
            return outputFilePath;
        }

        public static async Task<string> CombineVideoAndAudio(string videoFilePath, string audioFilePath)
        {
            FFmpeg.SetExecutablesPath("C:\\Users\\gogot\\Utils\\ffmpeg-2025-04-17-git-7684243fbe-full_build\\bin");
            string outputFilePath = Path.Combine(Path.GetDirectoryName(videoFilePath)!, "full_" + Path.GetFileNameWithoutExtension(videoFilePath) + ".mp4");
            var conversion = await FFmpeg.Conversions.FromSnippet.AddAudio(videoFilePath, audioFilePath, outputFilePath);
            await conversion.Start();
            return outputFilePath;
        }

        public static void AddMP3Tags(MediaMetaData mediaData)
        {
            if (mediaData.Type != Enums.MediaType.Music)
                throw new Exception("Неверный формат медиа-файла. Требуется mp3-файл");

            Track track = new(mediaData.FilePath)
            {
                Title = mediaData.Title,
            };
            track.EmbeddedPictures.Clear();
            track.EmbeddedPictures.Add(PictureInfo.fromBinaryData(File.ReadAllBytes(mediaData.PreviewFilePath), PictureInfo.PIC_TYPE.Front));
            track.Save();
        }
    }
}
