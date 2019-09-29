using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraServerClient
{
    public class ProgramModel
    {
        public string Version { get; set; }
        public List<string> ProgramList { get; set; }
        public string DiscordUrl { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string WebSite { get; set; }
        public List<string> Adversiment { get; set; }
        public string DownloadLink { get; set; }
        public int MaxClient { get; set; }
    }
}
