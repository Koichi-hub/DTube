using DTube.Common.Enums;

namespace DTube.Common.Models
{
    public class MediaMetaDataModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string SourceUrl { get; set; } = null!;
        public MediaType Type { get; set; }
        public long SizeInBytes { get; set; }
        public int Duration { get; set; }
    }
}
