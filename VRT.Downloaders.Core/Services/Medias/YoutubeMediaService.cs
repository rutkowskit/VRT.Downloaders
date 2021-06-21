﻿using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace VRT.Downloaders.Services.Medias
{
    public sealed class YoutubeMediaService : IMediaService
    {
        public async Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl)
        {
            var uri = new Uri(resourceUrl);
            var result = await Result.Success(new YoutubeClient())
                .Map(async grabber =>
                {
                    var videoInfo = await grabber.Videos.GetAsync(resourceUrl);
                    var streams = await grabber.Videos.Streams.GetManifestAsync(resourceUrl);
                    return ToMediaInfo(videoInfo, streams);
                });
            return result;
        }

        private Result<MediaInfo[]> ToMediaInfo(Video videoInfo, StreamManifest streams)
        {
            var results = new List<MediaInfo>();            
            results.AddRange(GetAudioStreams(videoInfo, streams));
            results.AddRange(GetMuxedStreams(videoInfo, streams));
            return results.ToArray();
        }
        private IEnumerable<MediaInfo> GetAudioStreams(Video videoInfo, StreamManifest manifest)
        {
            return manifest
                .GetAudioOnlyStreams()
                .OrderByDescending(s => s.Bitrate)
                .Select(a => new MediaInfo()
                {
                    Uri = new Uri(a.Url),
                    Title = videoInfo.Title,
                    Extension = a.Container.Name,
                    FormatDescription = $"{a.AudioCodec}({a.Container.Name})",
                    OutputFileName = videoInfo.Title.SanitizeAsFileName()
                })
                .ToArray();
        }

        private IEnumerable<MediaInfo> GetMuxedStreams(Video videoInfo, StreamManifest manifest)
        {
            return manifest
                .GetMuxedStreams()
                .OrderByDescending(s => s.VideoResolution.Width)
                .Select(a => new MediaInfo()
                {
                    Uri = new Uri(a.Url),
                    Title = videoInfo.Title,
                    Extension = a.Container.Name,
                    FormatDescription = $"{a.Container.Name} {a.VideoQuality} {a.VideoResolution} {a.VideoCodec}",
                    OutputFileName = videoInfo.Title.SanitizeAsFileName()
                })
                .ToArray();
        }


    }
}
