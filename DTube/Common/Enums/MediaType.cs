using System.ComponentModel.DataAnnotations;

namespace DTube.Common.Enums
{
    public enum MediaType
    {
        [Display(Name = "Видео")]
        Video = 0,

        [Display(Name = "Музыка")]
        Music = 1,
    }
}
