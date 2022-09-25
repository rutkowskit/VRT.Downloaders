using CommunityToolkit.Maui;

namespace VRT.Downloaders.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp(Action<MauiAppBuilder> setup = null)
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
            });

        builder.Services
            .AddSingleton<AppShell>()
            .ConfigureCoreServices()            
            .AddViews();

        if(setup is not null)
        {
            setup(builder);
        }
        return builder.Build();
    }
}