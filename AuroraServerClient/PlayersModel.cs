using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraServerClient
{
    public class PlayersModel
    {
        public string Endpoint { get; set; }
        public int Id { get; set; }
        public List<string> Identifiers { get; set; }
        public string Name { get; set; }
        public int Ping { get; set; }
    }
}
