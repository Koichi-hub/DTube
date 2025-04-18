﻿using DTube.Common;
using DTube.Common.Enums;
using DTube.Common.Models;
using DTube.Controllers;
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
