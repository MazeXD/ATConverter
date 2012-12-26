using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ATConverter.Classes
{
    class TransformerFile
    {
        public List<Modifier> Modifiers = new List<Modifier>();

        public static TransformerFile Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Can't find transformer file...");
            }

            var file = new TransformerFile();

            var stream = File.OpenRead(fileName);
            var reader = new StreamReader(stream);

            string line = "";
            int i = 0;
            while ((line = reader.ReadLine()) != null)
            {
                i++;

                if (line.Contains("#"))
                {
                    line = line.Substring(0, line.IndexOf("#"));
                }

                if (line.Length <= 0)
                {
                    continue;
                }

                var parts = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 2)
                {
                    Console.WriteLine("[ERROR] Invalid config file line {0}", i);
                    continue;
                }

                var modifier = new Modifier();
                modifier.AccessTarget = parts[0].Trim();

                var descriptor = parts[1].Trim().Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (descriptor.Length == 1)
                {
                    modifier.ModifyClassVisibility = true;
                }
                else
                {
                    var nameReference = descriptor[1].Trim();
                    int parenIdx = nameReference.IndexOf('(');
                    if (parenIdx > 0)
                    {
                        modifier.Desc = nameReference.Substring(parenIdx);
                        modifier.Name = nameReference.Substring(0, parenIdx);
                    }
                    else
                    {
                        modifier.Name = nameReference;
                    }
                }

                modifier.Descriptor = descriptor[0].Trim();

                file.Modifiers.Add(modifier);
            }

            reader.Close();
            reader.Dispose();
            stream.Close();
            stream.Dispose();

            return file;
        }

        public void Save(string fileName)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Modifiers.Count; i++)
            {
                var m = Modifiers[i];

                builder.Append(m.ToString());

                if(i != Modifiers.Count - 1)
                {
                    builder.AppendLine();
                }
            }

            File.WriteAllText(fileName, builder.ToString());
        }
    }
}
