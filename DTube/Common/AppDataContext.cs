using DTube.Common.DAO;
using DTube.Common.Models;
using System.Collections.Generic;

namespace DTube.Common
{
    public class AppDataContext(MediaMetaDataDAO mediaMetaDataDAO)
    {
        public delegate void UpdateHandler();
        public event UpdateHandler? OnUpdated;

        public MediaMetaDataModel? CurrentMedia { get; set; }
        public List<MediaMetaDataModel> MediaList { get; set; } = [];

        public void AddMedia(MediaMetaDataModel media)
        {
            MediaList.Add(media);
            mediaMetaDataDAO.SaveMediaMetaData(MediaList);
            OnUpdated?.Invoke();
        }

        public void InitMediaList()
        {
            MediaList.AddRange(mediaMetaDataDAO.GetMediaMetaData());
            OnUpdated?.Invoke();
        }

        public void SetCurrentMedia(MediaMetaDataModel media)
        {
            CurrentMedia = media;
            OnUpdated?.Invoke();
        }
    }
}
