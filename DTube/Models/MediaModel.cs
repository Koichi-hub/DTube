namespace DTube.Models
{
    public class MediaModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public MediaType Type { get; set; }
        public long SizeInBytes { get; set; }
        public int Duration { get; set; }

        public enum MediaType
        {
            Video = 0, 
            Music = 1,
        }
    }
}
