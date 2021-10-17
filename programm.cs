using Newtonsoft.Json;
using RMAN_Parse.RMAN;
using System;
using System.IO;

namespace RMAN_Parse
{
    class programm
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please Provide Path of a valid RMAN Manifest");
                Environment.Exit(1);
            }

            FileInfo info = new FileInfo(args[0]);

            RMANParser parser = new RMANParser();

            RMANFile file = parser.Parse(info.FullName);

            string JsonManifest = JsonConvert.SerializeObject(file, Formatting.Indented);

            File.WriteAllText($"{Path.GetFileNameWithoutExtension(info.Name)}.json", JsonManifest);

        }
    }
}
