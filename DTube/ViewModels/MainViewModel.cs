using ReactiveUI;
using System;
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
}
