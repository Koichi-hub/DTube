using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using DTube.Common;
using DTube.Common.Enums;
using DTube.Common.Models;
using DTube.Controllers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
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
        set
        {
            this.RaiseAndSetIfChanged(ref isLoaderVisible, value);
            IsMediaBlockVisible = !value;
        }
    }

    private bool isMediaBlockVisible = true;
    public bool IsMediaBlockVisible
    {
        get => isMediaBlockVisible;
        set => this.RaiseAndSetIfChanged(ref isMediaBlockVisible, value);
    }

    private string errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => errorMessage;
        set => this.RaiseAndSetIfChanged(ref errorMessage, value);
    }

    private bool isError = false;
    public bool IsError
    {
        get => isError;
        set => this.RaiseAndSetIfChanged(ref isError, value);
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

    public MediaMetaData? SelectedMediaListItem { get; set; }

    private MediaMetaDataView mediaModel = new()
    {
        Title = "Title",
        Description = "Description",
    };
    public MediaMetaDataView MediaModel
    {
        get => mediaModel;
        set => this.RaiseAndSetIfChanged(ref mediaModel, value);
    }

    private List<MediaMetaData> mediaModels = [];
    public List<MediaMetaData> MediaModels
    {
        get => mediaModels;
        set => this.RaiseAndSetIfChanged(ref mediaModels, value);
    }

    #region Commands
    public ICommand SearchCommand { get; }
    public ICommand DownloadMusicCommand { get; }
    public ICommand DownloadVideoCommand { get; }
    public ICommand CopyMediaCommand { get; }
    public ICommand OpenMediaInExplorerCommand { get; }
    public ICommand PlayMediaCommand { get; }
    public ICommand DeleteMediaCommand { get; }
    #endregion

    private readonly MainViewModelController controller;
    private readonly AppDataContext appDataContext;

    public MainViewModel(MainViewModelController controller, AppDataContext appDataContext)
    {
        this.controller = controller;
        this.appDataContext = appDataContext;
        appDataContext.OnUpdated += UpdateMediaCache;
        appDataContext.InitMediaList();

        SearchCommand = ReactiveCommand.Create(Search);
        DownloadMusicCommand = ReactiveCommand.Create(DownloadMusic);
        DownloadVideoCommand = ReactiveCommand.Create(DownloadVideo);
        CopyMediaCommand = ReactiveCommand.Create(CopyMedia);
        OpenMediaInExplorerCommand = ReactiveCommand.Create(OpenMediaInExplorer);
        PlayMediaCommand = ReactiveCommand.Create(PlayMedia);
        DeleteMediaCommand = ReactiveCommand.Create(DeleteMedia);
    }

    private void UpdateMediaCache()
    {
        if (appDataContext.CurrentMedia != null)
        {
            MediaModel = appDataContext.CurrentMedia;
        }
        else
        {
            MediaModel = new MediaMetaDataView
            {
                Title = "Title",
                Description = "Description",
            };
        }
        FilterMedia();
    }

    public async Task Search()
    {
        if (IsLoaderVisible)
            return;

        IsLoaderVisible = true;
        IsError = false;

        try
        {
            await controller.GetMediaAsync(searchText!);
            IsError = false;
            ErrorMessage = string.Empty;
        }
        catch (Exception e)
        {
            IsError = true;
            ErrorMessage = e.Message;
        }

        IsLoaderVisible = false;
    }

    public async Task DownloadMusic()
    {
        if (IsLoaderVisible)
            return;

        IsLoaderVisible = true;
        IsError = false;

        if (string.IsNullOrWhiteSpace(searchText))
            return;

        try
        {
            await controller.DownloadMusicAsync(searchText!);
            IsError = false;
            ErrorMessage = string.Empty;
        }
        catch (Exception e)
        {
            IsError = true;
            ErrorMessage = e.Message;
        }

        IsLoaderVisible = false;
    }

    public async Task DownloadVideo()
    {
        if (IsLoaderVisible)
            return;

        IsLoaderVisible = true;
        IsError = false;

        if (string.IsNullOrWhiteSpace(searchText))
            return;

        try
        {
            await controller.DownloadVideoAsync(searchText!);
            IsError = false;
            ErrorMessage = string.Empty;
        }
        catch (Exception e)
        {
            IsError = true;
            ErrorMessage = e.Message;
        }

        IsLoaderVisible = false;
    }

    public async Task CopyMedia()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } clipboard || desktop.MainWindow?.StorageProvider is not { } storageProvider)
        {
            IsError = true;
            ErrorMessage = "Буфер обмена недоступен";
            return;
        }

        MediaMetaData media = SelectedMediaListItem!;

        IStorageFile? file = await storageProvider.TryGetFileFromPathAsync(media.FilePath);
        if (file != null)
        {
            DataObject dataObject = new();
            dataObject.Set(DataFormats.Files, new List<IStorageFile> { file });
            await clipboard.SetDataObjectAsync(dataObject);
        }
    }

    public async Task OpenMediaInExplorer()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Launcher is not { } launcher)
        {
            IsError = true;
            ErrorMessage = "Лаунчер недоступен";
            return;
        }

        MediaMetaData media = SelectedMediaListItem!;

        await launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(Path.GetDirectoryName(media.FilePath)!));
    }

    public async Task PlayMedia()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Launcher is not { } launcher)
        {
            IsError = true;
            ErrorMessage = "Лаунчер недоступен";
            return;
        }

        MediaMetaData media = SelectedMediaListItem!;

        await launcher.LaunchFileInfoAsync(new FileInfo(media.FilePath));
    }

    public void DeleteMedia()
    {
        MediaMetaData media = SelectedMediaListItem!;

        try
        {
            controller.DeleteMediaAsync(media);
            IsError = false;
            ErrorMessage = string.Empty;
        }
        catch (Exception e)
        {
            IsError = true;
            ErrorMessage = e.Message;
        }
    }

    private void FilterMedia()
    {
        if (IsVideoFilter == IsMusicFilter)
        {
            MediaModels = new List<MediaMetaData>(appDataContext.MediaList);
        } 
        else
        {
            MediaModels = appDataContext.MediaList.Where(x => (IsVideoFilter && x.Type == MediaType.Video) || (IsMusicFilter && x.Type == MediaType.Music)).ToList();
        }
    }
}
