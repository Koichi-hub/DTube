using DTube.Common.Enums;
using DTube.Common.Extensions;
using DTube.Common.Helper;
using System;

namespace DTube.Common.Models
{
    public class MediaMetaData
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string SourceUrl { get; set; } = null!;
        public string PreviewSourceUrl { get; set; } = null!;
        public string PreviewFilePath { get; set; } = null!;
        private MediaType type;
        public MediaType Type
        {
            get => type;
            set
            {
                type = value;
                TypeView = value.GetDisplayName();
            }
        }
        public string TypeView { get; set; } = string.Empty;
        private long sizeInBytes;
        public long SizeInBytes
        {
            get => sizeInBytes;
            set
            {
                sizeInBytes = value;
                FileSizeView = FileSizeConverter.ToMiB(value);
            }
        }
        public string FileSizeView { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }
}
