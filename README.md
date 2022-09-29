# VRT Media downloader

This application downloads medias from supported sources to allow the user to convert it to other formats (using for example ```ffmpeg``` or ```format factory```).

## Supported media sources

| Source name  |External lib used |External lib url|
|:-:|:-:|:-:|
|<b>YouTube</b>| Yes | <a href="https://github.com/Tyrrrz/YoutubeExplode">YoutubeExplode</a> |
|<b>TVP VOD</b>| No | - |

## Buidilng the Project

### Publish Release for Android

``` dotnet publish -c Release -f net6.0-android ```


### Publish Release for Windows 10

``` dotnet publish -c Release -f net6.0-windows10.0.19041.0 --force -r win10-x64 --self-contained /p:PublishSingleFile=true /p:RuntimeIdentifierOverride=win10-x64 ```