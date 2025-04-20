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
using System.Reactive;
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

    private bool isMediaBlockVisible = false;
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

    private List<MediaMetaDataModel> mediaModels = [];
    public List<MediaMetaDataModel> MediaModels
    {
        get => mediaModels;
        set => this.RaiseAndSetIfChanged(ref mediaModels, value);
    }

    #region Commands
    public ICommand SearchCommand { get; }
    public ICommand DownloadMusicCommand { get; }
    public ICommand DownloadVideoCommand { get; }
    public ReactiveCommand<Guid, Task> CopyMediaCommand { get; }
    public ReactiveCommand<Guid, Task> OpenMediaInExplorerCommand { get; }
    public ReactiveCommand<Guid, Task> PlayMediaCommand { get; }
    public ReactiveCommand<Guid, Unit> DeleteMediaCommand { get; }
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
        CopyMediaCommand = ReactiveCommand.Create<Guid, Task>(CopyMedia);
        OpenMediaInExplorerCommand = ReactiveCommand.Create<Guid, Task>(OpenMediaInExplorer);
        PlayMediaCommand = ReactiveCommand.Create<Guid, Task>(PlayMedia);
        DeleteMediaCommand = ReactiveCommand.Create<Guid>(DeleteMedia);
    }

    private void UpdateMediaCache()
    {
        if (appDataContext.CurrentMedia != null)
        {
            MediaModel = appDataContext.CurrentMedia;
        }
        else
        {
            MediaModel = new MediaMetaDataModel
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

    public async Task CopyMedia(Guid mediaId)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } clipboard || desktop.MainWindow?.StorageProvider is not { } storageProvider)
        {
            IsError = true;
            ErrorMessage = "Буфер обмена недоступен";
            return;
        }

        MediaMetaDataModel? media = MediaModels.FirstOrDefault(x => x.Id == mediaId);
        if (media == null)
            return;

        IStorageFile? file = await storageProvider.TryGetFileFromPathAsync(media.FilePath);
        if (file != null)
        {
            DataObject dataObject = new();
            dataObject.Set(DataFormats.Files, new List<IStorageFile> { file });
            await clipboard.SetDataObjectAsync(dataObject);
        }
    }

    public async Task OpenMediaInExplorer(Guid mediaId)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Launcher is not { } launcher)
        {
            IsError = true;
            ErrorMessage = "Лаунчер недоступен";
            return;
        }

        MediaMetaDataModel? media = MediaModels.FirstOrDefault(x => x.Id == mediaId);
        if (media == null)
            return;

        await launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(Path.GetDirectoryName(media.FilePath)!));
    }

    public async Task PlayMedia(Guid mediaId)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Launcher is not { } launcher)
        {
            IsError = true;
            ErrorMessage = "Лаунчер недоступен";
            return;
        }

        MediaMetaDataModel? media = MediaModels.FirstOrDefault(x => x.Id == mediaId);
        if (media == null)
            return;

        await launcher.LaunchFileInfoAsync(new FileInfo(media.FilePath));
    }

    public void DeleteMedia(Guid mediaId)
    {
        MediaMetaDataModel? media = MediaModels.FirstOrDefault(x => x.Id == mediaId);
        if (media == null)
            return;

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
            MediaModels = new List<MediaMetaDataModel>(appDataContext.MediaList);
        } 
        else
        {
            MediaModels = appDataContext.MediaList.Where(x => (IsVideoFilter && x.Type == MediaType.Video) || (IsMusicFilter && x.Type == MediaType.Music)).ToList();
        }
    }
}
