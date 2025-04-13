using DTube.Common.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DTube.Common.DAO
{
    public class ConfigDAO
    {
        private readonly string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.ConfigFileName);

        public ConfigModel Read()
        {
            return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(configFilePath))!;
        }

        public void Restore()
        {
            ConfigModel configModel = new();

            if (File.Exists(configFilePath))
            {
                configModel = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(configFilePath)) ?? new();
            }

            if (string.IsNullOrWhiteSpace(configModel.Media.MetaDataFilePath))
            {
                configModel.Media.MetaDataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.MediaMetaDataFileName);
            }
            if (string.IsNullOrWhiteSpace(configModel.Media.ContentFolderPath))
            {
                configModel.Media.ContentFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(configModel));
        }
    }
}
