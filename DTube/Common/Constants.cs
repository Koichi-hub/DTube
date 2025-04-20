using System;
using System.IO;

namespace DTube.Common
{
    public static class Constants
    {
        public readonly static string ConfigFileName = "config.json";
        public readonly static string MediaMetaDataFileName = "media.json";
        public readonly static string MediaContentDefaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media");
        public readonly static string MediaPreviewImageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media_preview");
        public readonly static string TMPFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp");
    }
}
