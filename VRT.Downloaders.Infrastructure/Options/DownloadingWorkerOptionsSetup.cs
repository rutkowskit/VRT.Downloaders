using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRT.Downloaders.Common.Options;

namespace VRT.Downloaders.Infrastructure.Options;
internal sealed class DownloadingWorkerOptionsSetup : IConfigureOptions<DownloadingWorkerOptions>
{
    private const string ConfigurationSectionName = nameof(DownloadingWorkerOptions);
    private readonly IConfiguration _configuration;

    public DownloadingWorkerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void Configure(DownloadingWorkerOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName).Bind(options);        
    }
}
