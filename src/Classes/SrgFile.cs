using System;
using System.Collections.Generic;
using System.IO;

namespace ATConverter.Classes
{
    class SRGFile
    {
        public Dictionary<string, string> PackageMap = new Dictionary<string, string>();
        public Dictionary<string, string> ClassMap = new Dictionary<string, string>();
        public Dictionary<string, string> FieldMap = new Dictionary<string, string>();
        public Dictionary<string, string> MethodMap = new Dictionary<string, string>();
        public Dictionary<string, string> MethodSigMap = new Dictionary<string, string>();

        public static SRGFile Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Can't find srg file...");
            }

            var file = new SRGFile();

            var stream = File.OpenRead(fileName);
            var reader = new StreamReader(stream);

            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                var splitted = line.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                var args = splitted[1].Split(' ');

                var kind = splitted[0];

                switch (kind)
                {
                    case "PK":
                        file.PackageMap.Add(args[0], args[1]);
                        break;
                    case "CL":
                        file.ClassMap.Add(args[0], args[1]);
                        break;
                    case "FD":
                        file.FieldMap.Add(args[0], args[1]);
                        break;
                    case "MD":
                        file.MethodMap.Add(args[0] + " " + args[1], args[2]);
                        file.MethodSigMap.Add(args[0] + " " + args[1], args[3]);
                        break;
                }
            }

            reader.Close();
            reader.Dispose();
            stream.Close();
            stream.Dispose();

            return file;
        }
    }
}
