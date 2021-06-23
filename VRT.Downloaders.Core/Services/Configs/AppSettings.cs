using System;
using System.Collections.Generic;

namespace VRT.Downloaders.Services.Configs
{
    public sealed class AppSettings : IEquatable<AppSettings>
    {
        public AppSettings(
            string outputDirectory, 
            bool enableClipboardMonitor)
        {
            EnableClipboardMonitor = enableClipboardMonitor;
            OutputDirectory = outputDirectory;
        }
        public bool EnableClipboardMonitor { get; }
        public string OutputDirectory { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as AppSettings);
        }

        public bool Equals(AppSettings other)
        {
            return other != null &&
                   EnableClipboardMonitor == other.EnableClipboardMonitor &&
                   OutputDirectory == other.OutputDirectory;
        }

        public override int GetHashCode()
        {
            int hashCode = 949157293;
            hashCode = hashCode * -1521134295 + EnableClipboardMonitor.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OutputDirectory);
            return hashCode;
        }
    }
}
