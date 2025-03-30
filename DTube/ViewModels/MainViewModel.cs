using DTube.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

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

    private MediaModel mediaModel = new()
    {
        Title = "Title",
        Description = "Description",
    };
    public MediaModel MediaModel
    {
        get => mediaModel;
        set => this.RaiseAndSetIfChanged(ref mediaModel, value);
    }

    private readonly static List<MediaModel> mediaModelsData = [
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Video,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Video,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Video,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Video,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Video,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Music,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Music,
        },
        new MediaModel {
            Title = "Title",
            Description = "Description",
            Type = MediaModel.MediaType.Music,
        },
        ];

    private List<MediaModel> mediaModels = mediaModelsData;
    public List<MediaModel> MediaModels
    {
        get => mediaModels;
        set => this.RaiseAndSetIfChanged(ref mediaModels, value);
    }

    #region Commands
    public ICommand SearchCommand { get; }
    #endregion

    public MainViewModel()
    {
        SearchCommand = ReactiveCommand.Create(Search);
    }

    public async Task Search()
    {
        if (IsLoaderVisible)
            return;

        IsLoaderVisible = true;

        await Task.Delay(TimeSpan.FromSeconds(1));

        IsLoaderVisible = false;
    }

    private void FilterMedia()
    {
        if (IsVideoFilter == IsMusicFilter)
        {
            MediaModels = mediaModelsData;
        } 
        else
        {
            MediaModels = mediaModelsData.Where(x => (IsVideoFilter && x.Type == MediaModel.MediaType.Video) || (IsMusicFilter && x.Type == MediaModel.MediaType.Music)).ToList();
        }
    }
}
