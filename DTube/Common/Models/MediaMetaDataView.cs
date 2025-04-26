using DTube.Common.Helper;

namespace DTube.Common.Models
{
    public class MediaMetaDataView : MediaMetaData
    {
        private long videoSizeInBytes;
        public long VideoSizeInBytes
        {
            get => videoSizeInBytes;
            set
            {
                videoSizeInBytes = value;
                VideoSizeView = FileSizeConverter.ToMiB(value);
            }
        }
        public string VideoSizeView { get; set; } = string.Empty;

        private long audioSizeInBytes;
        public long AudioSizeInBytes
        {
            get => audioSizeInBytes;
            set
            {
                audioSizeInBytes = value;
                AudioSizeView = FileSizeConverter.ToMiB(value);
            }
        }
        public string AudioSizeView { get; set; } = string.Empty;
    }
}
