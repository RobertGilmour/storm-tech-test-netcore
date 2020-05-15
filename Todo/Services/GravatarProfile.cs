using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Todo.Services
{
    public class GravatarProfile
    {
        [JsonProperty("entry")]
        public List<GravatarProfileEntry> Entry { get; set; }
    }

    public class GravatarProfileEntry
    {
        [JsonProperty("name")]
        public GravatarProfileEntryName Name { get; set; }
    }

    public class GravatarProfileEntryName
    {
        [JsonProperty("formatted")]
        public string Formatted { get; set; }
    }
}
