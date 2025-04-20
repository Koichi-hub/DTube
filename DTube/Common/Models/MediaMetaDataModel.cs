using DTube.Common.Enums;
using System;

namespace DTube.Common.Models
{
    public class MediaMetaDataModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string SourceUrl { get; set; } = null!;
        public string PreviewSourceUrl { get; set; } = null!;
        public string PreviewFilePath { get; set; } = null!;
        public MediaType Type { get; set; }
        public long SizeInBytes { get; set; }
        public int Duration { get; set; }
    }
}
