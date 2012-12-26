using ATConverter.Converters;
using System;
using System.Diagnostics;
using System.IO;

namespace ATConverter
{
    class Program
    {
        private static bool _failed = false;
        private const string OutputDir = "output";

        static void Main(string[] args)
        {
            if (!File.Exists("server.srg"))
            {
                Console.WriteLine("server.srg is missing.");
                ForceExit();
            }

            if (args.Length <= 0)
            {
                Console.WriteLine("Usage: {0} file_to_transform ...", Process.GetCurrentProcess().ProcessName);
                ForceExit();
            }

            TransformConverter.LoadMapping("server.srg");

            for (int i = 0; i < args.Length; i++)
            {
                ConvertFile(args[i]);
            }

            ForceExit(_failed);
        }

        private static void ConvertFile(string fileName)
        {
            Console.WriteLine("[INFO] Converting: {0}", fileName);

            var converter = new TransformConverter(fileName);
                
            var name = Path.GetFileName(fileName);
            var outputName = fileName.Replace(name, OutputDir + "/" + name);
            var outputDir = Path.GetDirectoryName(outputName);

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            bool converted = converter.Convert(outputName);
                
            Console.WriteLine(converted ? "[INFO] Converted '{0}' successfully" : "[ERROR] Failed to convert {0}", name);

            Console.WriteLine(new String('-', 32));
        }

        private static void ForceExit(bool prompt = true)
        {
            if(prompt){
                Console.Write("Press a key to exit...");
                Console.ReadKey();
            }

            Environment.Exit(0);
        }
    }
}
