using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Modrinth.Console.Models
{
    public struct VersionFile
    {
        [JsonPropertyName("hashes")]
        public Dictionary<string, string> Hashes { get; set; }

        [JsonPropertyName("primary")]
        public bool IsPrimary { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("url")]
        public string DownloadUrl { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }
    }
}