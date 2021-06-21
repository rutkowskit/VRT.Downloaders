﻿using Microsoft.Extensions.DependencyInjection;
using VRT.Downloaders.Desktop.Wpf.MainWindow;

namespace VRT.Downloaders.Desktop.Wpf
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            return services
                .AddSingleton<MainWindowView>();
        }
    }
}
