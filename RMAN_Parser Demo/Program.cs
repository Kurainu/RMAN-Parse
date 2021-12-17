using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RMAN_Parse;
using RMAN_Parse.RMAN;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;

namespace RMAN_Parser
{
    class Program
    {
        static void Main(string[] args)
        {

            RMANParser parser = new();
            var src = parser.Parse(@"E:\0F5DF3ECCA289691.manifest");

            MemoryStream stream = src.Manifest.Files.FirstOrDefault(f => f.Name == "ClientInternalConfig.json").GetStream(@"https://bacon.secure.dyn.riotcdn.net/channels/public/bundles");
            Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
        }
    }
}
