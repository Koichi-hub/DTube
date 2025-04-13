using DTube.Common.DAO;
using DTube.Common.Models;
using DTube.Common.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace DTube.Controllers
{
    public class MainViewModelController(
        MediaMetaDataDAO mediaMetaDataDAO,
        YTService ytService)
    {
        public List<MediaMetaDataModel> GetMediaMetaData() => mediaMetaDataDAO.GetMediaMetaData();

        public async Task<MediaMetaDataModel> GetMediaAsync(string url) => await ytService.GetMediaByLinkAsync(url);
    }
}
