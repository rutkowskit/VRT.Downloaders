# VRT Media downloader

This application downloads medias from supported sources to allow the user to convert it to other formats (using for example ```ffmpeg``` or ```format factory```).

## Supported media sources

| Source name  |External lib used |External lib url|
|:-:|:-:|:-:|
|<b>YouTube</b>| Yes | <a href="https://github.com/Tyrrrz/YoutubeExplode">YoutubeExplode</a> |
|<b>TVP VOD</b>| No | - |
|<b>Direct</b>| No | - |
|<b>Html5Player Videos</b>| No | - |

## Buidilng and installation

### Building and installation on Android device

1. Open command prompt
1. Go to ```VRT.Downloaders.Maui``` project directory 
1. Execute command: ``` dotnet publish -c Release -f net7.0-android ```
1. Upload ```.\bin\Release\net7.0-android\publish\com.vrt.downloaders.maui-Signed.apk ``` file to your Android device
1. On the android device
    1. Open file explorer and select the folder where you uploded the file,
    1. Tap the ```com.vrt.downloaders.maui-Signed.apk``` file,
    1. Confirm that you want to install the app


### Installation on Android device

1. Copy 


### Publish Release for Windows 10
``` dotnet publish -c Release -f net7.0-windows10.0.19041.0 --force -r win10-x64 --self-contained /p:PublishSingleFile=true /p:RuntimeIdentifierOverride=win10-x64```

If there are problems with assembly optimisation, you can use below command instead:
``` dotnet publish -c Release -f net7.0-windows10.0.19041.0 --force -r win10-x64 --self-contained /p:RuntimeIdentifierOverride=win10-x64 ```
