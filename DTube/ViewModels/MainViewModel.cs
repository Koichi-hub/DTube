using DTube.Common.Enums;
using DTube.Common.Models;
using DTube.Controllers;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using static DTube.Common.Models.ConfigModel;

namespace DTube.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string? searchText = "";
    public string? SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    private bool isLoaderVisible;
    public bool IsLoaderVisible
    {
        get => isLoaderVisible;
        set => this.RaiseAndSetIfChanged(ref isLoaderVisible, value);
    }

    private bool isMediaBlockVisible = true;
    public bool IsMediaBlockVisible
    {
        get => isMediaBlockVisible;
        set => this.RaiseAndSetIfChanged(ref isMediaBlockVisible, value);
    }

    private bool isVideoFilter;
    public bool IsVideoFilter
    {
        get => isVideoFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref isVideoFilter, value);
            FilterMedia();
        }
    }

    private bool isMusicFilter;
    public bool IsMusicFilter
    {
        get => isMusicFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref isMusicFilter, value);
            FilterMedia();
        }
    }

    private MediaMetaDataModel mediaModel = new()
    {
        Title = "Title",
        Description = "Description",
    };
    public MediaMetaDataModel MediaModel
    {
        get => mediaModel;
        set => this.RaiseAndSetIfChanged(ref mediaModel, value);
    }

    private List<MediaMetaDataModel> mediaModelsCache = [];
    private List<MediaMetaDataModel> mediaModels = [];
    public List<MediaMetaDataModel> MediaModels
    {
        get => mediaModels;
        set => this.RaiseAndSetIfChanged(ref mediaModels, value);
    }

    #region Commands
    public ICommand SearchCommand { get; }
    #endregion

    private readonly MainViewModelController controller;

    public MainViewModel(MainViewModelController controller)
    {
        this.controller = controller;
        mediaModelsCache = controller.GetMediaMetaData();
        MediaModels = mediaModelsCache;

        SearchCommand = ReactiveCommand.Create(Search);
    }

    public async Task Search()
    {
        if (IsLoaderVisible)
            return;

        IsLoaderVisible = true;

        MediaModel = await controller.GetMediaAsync(searchText!);

        IsLoaderVisible = false;
    }

    private void FilterMedia()
    {
        if (IsVideoFilter == IsMusicFilter)
        {
            MediaModels = mediaModelsCache;
        } 
        else
        {
            MediaModels = mediaModelsCache.Where(x => (IsVideoFilter && x.Type == MediaType.Video) || (IsMusicFilter && x.Type == MediaType.Music)).ToList();
        }
    }
}
