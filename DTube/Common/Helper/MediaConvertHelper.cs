using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace DTube.Common.Helper
{
    public static class MediaConvertHelper
    {
        public static async Task<string> ConvertToMP3(string inputFilePath)
        {
            FFmpeg.SetExecutablesPath("");
            string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath)!, Path.GetFileNameWithoutExtension(inputFilePath) + ".mp3");
            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(inputFilePath, outputFilePath);
            await conversion.Start();
            return outputFilePath;
        }
    }
}
