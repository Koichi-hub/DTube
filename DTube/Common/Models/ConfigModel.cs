namespace DTube.Common.Models
{
    public class ConfigModel
    {
        public MediaModel Media { get; set; } = new();

        public class MediaModel
        {
            /// <summary>
            /// Путь до файла media.json со списком метаданных о сохраненном медиа
            /// </summary>
            public string MetaDataFilePath { get; set; } = null!;

            /// <summary>
            /// Путь до директории с самим медиа
            /// </summary>
            public string ContentFolderPath { get; set; } = null!;
        }
    }
}
