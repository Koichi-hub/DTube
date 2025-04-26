using DTube.Common.DAO;
using DTube.Common.Models;
using System.Collections.Generic;

namespace DTube.Common
{
    public class AppDataContext(MediaMetaDataDAO mediaMetaDataDAO)
    {
        public delegate void UpdateHandler();
        public event UpdateHandler? OnUpdated;

        public MediaMetaDataView? CurrentMedia { get; set; }
        public List<MediaMetaData> MediaList { get; set; } = [];

        public void AddMedia(MediaMetaData media)
        {
            MediaList.Add(media);
            mediaMetaDataDAO.SaveMediaMetaData(MediaList);
            OnUpdated?.Invoke();
        }

        public void DeleteMedia(MediaMetaData media)
        {
            MediaList.RemoveAll(x => x.Id == media.Id);
            mediaMetaDataDAO.SaveMediaMetaData(MediaList);
            OnUpdated?.Invoke();
        }

        public void InitMediaList()
        {
            MediaList.AddRange(mediaMetaDataDAO.GetMediaMetaData());
            OnUpdated?.Invoke();
        }

        public void SetCurrentMedia(MediaMetaDataView media)
        {
            CurrentMedia = media;
            OnUpdated?.Invoke();
        }
    }
}
