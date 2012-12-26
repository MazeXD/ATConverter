using ATConverter.Classes;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ATConverter.Converters
{
    class TransformConverter
    {
        private static readonly Regex Pattern = new Regex(@"(?<=L)(.*?)(?=;)");
        private static SRGFile Mapping;

        private readonly TransformerFile _file;

        public TransformConverter(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(string.Format("Transformer file({0}) not found", fileName));
            }

            try
            {
                _file = TransformerFile.Load(fileName);
            }
            catch
            {
                throw new Exception(string.Format("Failed to parse transformer file: {0}", fileName));
            }
        }

        public bool Convert(string outFileName)
        {
            try
            {
                for (int i = 0; i < _file.Modifiers.Count; i++)
                {
                    var modifier = _file.Modifiers[i];

                    if (!Mapping.ClassMap.ContainsKey(modifier.Descriptor))
                    {
                        RemoveModifier("Removed class: {0}", modifier.Descriptor, ref i);
                        continue;
                    }

                    var newDescriptor = Mapping.ClassMap[modifier.Descriptor];

                    if (!modifier.ModifyClassVisibility)
                    {
                        if (string.IsNullOrEmpty(modifier.Desc))
                        {
                            if (!ConvertField(ref modifier, ref i))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (modifier.Name == "<init>")
                            {
                                ConvertConstructor(ref modifier);
                            }
                            else if (!ConvertMethod(ref modifier, ref i))
                            {
                                continue;
                            }
                        }
                    }

                    modifier.Descriptor = newDescriptor;
                }
            }
            catch (Exception)
            {
                return false;
            }

            _file.Save(outFileName);

            return true;
        }

        private bool ConvertField(ref Modifier modifier, ref int counter)
        {
            if (modifier.Name == "*")
            {
                return true;
            }
            
            var key = modifier.Descriptor + "/" + modifier.Name;

            if (!Mapping.FieldMap.ContainsKey(key))
            {
                RemoveModifier("Removed field: {0}", key, ref counter);
                return false;
            }

            var temp = Mapping.FieldMap[key];

            modifier.Name = temp.Substring(temp.LastIndexOf('/') + 1);

            return true;
        }

        private bool ConvertMethod(ref Modifier modifier, ref int counter)
        {
            var key = modifier.Descriptor + "/" + modifier.Name + " " + modifier.Desc;

            if (!Mapping.MethodMap.ContainsKey(key) && modifier.Name != "*")
            {
                RemoveModifier("Removed method: {0}", key, ref counter);
                return false;
            }

            if (modifier.Name != "*")
            {
                var temp = Mapping.MethodMap[key];

                modifier.Name = temp.Substring(temp.LastIndexOf('/') + 1);
                modifier.Desc = Mapping.MethodSigMap[key];
            }
            else
            {
                var matches = Pattern.Matches(modifier.Desc);

                for (int i = 0; i < matches.Count; i++)
                {
                    var temp = matches[i].Value;

                    try
                    {
                        modifier.Desc = modifier.Desc.Replace("L" + temp + ";", "L" + Mapping.ClassMap[temp] + ";");
                    }
                    catch { }
                }
            }

            return true;
        }

        private void ConvertConstructor(ref Modifier modifier)
        {
            var matches = Pattern.Matches(modifier.Desc);

            for (int i = 0; i < matches.Count; i++)
            {
                var temp = matches[i].Value;

                try
                {
                    modifier.Desc = modifier.Desc.Replace("L" + temp + ";", "L" + Mapping.ClassMap[temp] + ";");
                }
                catch { }
            }
        }

        private void RemoveModifier(string message, string key, ref int counter)
        {
            Console.WriteLine("[INFO] " + message+ ": {0}", key);
            _file.Modifiers.RemoveAt(counter);
            counter--;
        }

        public static void LoadMapping(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Missing mapping file...");
            }

            Mapping = SRGFile.Load(fileName);
        }
    }
}
