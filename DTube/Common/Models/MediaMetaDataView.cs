using DTube.Common.Helper;
using System.Collections.Generic;

namespace DTube.Common.Models
{
    public class MediaMetaDataView : MediaMetaData
    {
        public VideoResolutionDataView SelectedVideoResolution { get; set; } = null!;

        public List<VideoResolutionDataView> VideoResolutions { get; set; } = [];

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

        public class VideoResolutionDataView
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

            public string Resolution { get; set; } = string.Empty;
        }
    }
}
