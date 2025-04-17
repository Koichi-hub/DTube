using System;
using System.IO;

namespace DTube.Common
{
    public static class Constants
    {
        public readonly static string ConfigFileName = "config.json";
        public readonly static string MediaMetaDataFileName = "media.json";
        public readonly static string MediaContentDefaultPath = "media";
        public readonly static string TMPFilesFolderName = "tmp";
        public readonly static string FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TMPFilesFolderName);
    }
}
