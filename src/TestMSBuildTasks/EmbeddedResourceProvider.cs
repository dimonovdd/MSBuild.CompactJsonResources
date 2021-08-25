using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TestMSBuildTasks
{
    public static class EmbeddedResourceProvider
    {
        static readonly Assembly assembly = typeof(EmbeddedResourceProvider).GetTypeInfo().Assembly;
        static readonly string[] resources = assembly.GetManifestResourceNames();

        public static IEnumerable<(string name, string data)> GetAllJsonResources()
        {
            var names = GetAllJsonResourcesNames();

            if (names != null)
                foreach (var name in names)
                {
                    using var stream = assembly.GetManifestResourceStream(name);
                    using var sr = new StreamReader(stream);
                    yield return (name,  sr.ReadToEnd());
                }   
        }

        static IEnumerable<string> GetAllJsonResourcesNames() => resources.Where(n => n.EndsWith($".json"));
    }
}
