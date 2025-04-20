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
            string outputFilePath = Path.Combine(Path.GetDirectoryName(videoFilePath)!, "full_" + Path.GetFileNameWithoutExtension(videoFilePath) + ".mp4");
            var conversion = await FFmpeg.Conversions.FromSnippet.AddAudio(videoFilePath, audioFilePath, outputFilePath);
            await conversion.Start();
            return outputFilePath;
        }
    }
}
