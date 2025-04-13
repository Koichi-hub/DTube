using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DTube.Common;
using DTube.ViewModels;
using DTube.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DTube;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCommonServices();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        ConfigManager configManager = serviceProvider.GetRequiredService<ConfigManager>();
        configManager.Init();

        MainViewModel mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
