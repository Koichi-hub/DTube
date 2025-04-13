using DTube.Common.DAO;
using DTube.Common.Models;

namespace DTube.Common
{
    public class ConfigManager(ConfigDAO configDAO)
    {
        public ConfigModel Config { get; set; } = new();

        public void Init()
        {
            configDAO.Restore();
            Config = configDAO.Read();
        }
    }
}
