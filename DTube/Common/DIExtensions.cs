﻿using DTube.Common.DAO;
using DTube.Common.Services;
using DTube.Controllers;
using DTube.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DTube.Common
{
    public static class DIExtensions
    {
        public static void AddCommonServices(this IServiceCollection services)
        {
            //IHttpClientFactory
            services.AddHttpClient();

            services.AddSingleton<AppDataContext>();
            services.AddSingleton<ConfigDAO>();
            services.AddSingleton<ConfigManager>();
            services.AddSingleton<MediaMetaDataDAO>();
            services.AddSingleton<YTService>();
            services.AddSingleton<ImageService>();

            services.AddTransient<MainViewModelController>();
            services.AddTransient<MainViewModel>();
        }
    }
}
