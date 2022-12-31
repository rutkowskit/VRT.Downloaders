using CommunityToolkit.Maui;
using MediatR;

namespace VRT.Downloaders.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp(Action<MauiAppBuilder>? setup = null)
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
            .ConfigureCoreServices()            
            .AddPresentation()
            .AddMediatR(typeof(MauiProgram).Assembly);

        if(setup is not null)
        {
            setup(builder);
        }
        return builder.Build();
    }
}