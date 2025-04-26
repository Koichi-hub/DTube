using DTube.Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DTube.Common.DAO
{
    public class MediaMetaDataDAO(ConfigManager configManager)
    {
        public void SaveMediaMetaData(List<MediaMetaData> mediaList)
        {
            File.WriteAllText(configManager.Config.Media.MetaDataFilePath, JsonConvert.SerializeObject(new MediaJSONScheme
            {
                Media = mediaList
            }));
        }

        public List<MediaMetaData> GetMediaMetaData()
        {
            if (!File.Exists(configManager.Config.Media.MetaDataFilePath))
            {
                File.WriteAllText(configManager.Config.Media.MetaDataFilePath, JsonConvert.SerializeObject(new MediaJSONScheme()));
            }

            string json = File.ReadAllText(configManager.Config.Media.MetaDataFilePath);
            MediaJSONScheme mediaJson = JsonConvert.DeserializeObject<MediaJSONScheme>(json) ?? new();

            return mediaJson.Media;
        }
    }
}
