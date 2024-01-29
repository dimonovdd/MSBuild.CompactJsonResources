using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.CompactJsonResources
{
    public class JsonFile
    {
        const string definingProjectFullPath = "DefiningProjectFullPath";
        const string definingProjectName = "DefiningProjectName";
        const string definingProjectExtension = "DefiningProjectExtension";
        const string dot = ".";

        readonly ITaskItem item;
        readonly string tempOutputPath;

        public JsonFile(ITaskItem item, string tempOutputPath)
        {
            this.item = item;
            this.tempOutputPath = tempOutputPath;
            Link = GetMetadata(nameof(Link));
            Filename = GetMetadata(nameof(Filename));
            Extension = GetMetadata(nameof(Extension));
            FullPath = GetMetadata(nameof(FullPath));
            DefiningProjectDirectory = GetMetadata(nameof(DefiningProjectDirectory));
            Verify();
        }

        /// <summary>For tests</summary>
        public JsonFile(string link, string filename, string extension, string fullPath, string definingProjectDirectory)
        {
            Link = link;
            Filename = filename;
            Extension = extension;
            FullPath = fullPath;
            DefiningProjectDirectory = definingProjectDirectory;
            Verify();
        }

        public string Filename { get; private set; }

        public string Extension { get; private set; }

        public string Link { get; private set; }

        public string FullPath { get; private set; }

        public string DefiningProjectDirectory { get; private set; }

        public string TempFullPath { get; private set; }

        public TaskItem WriteCompactTempFile()
        {
            try
            {
                using var frs = File.OpenRead(FullPath);
                using var jDoc = JsonDocument.Parse(frs,
                    new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });

                var directory = Path.GetDirectoryName(TempFullPath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (File.Exists(TempFullPath))
                    File.Delete(TempFullPath);

                using var fws = File.OpenWrite(TempFullPath);
                using var writer = new Utf8JsonWriter(fws);
                jDoc.WriteTo(writer);
                writer.Flush();
                fws.Flush();
            }
            catch(Exception ex)
            {
                throw new CompactJsonException(FullPath, ex);
            }
            
            return GetTaskItem();
        }

        public override string ToString() => FullPath;

        TaskItem GetTaskItem()
            => new(TempFullPath, new Dictionary<string, string>
            {
                { nameof(Link), Link },
                { nameof(FullPath), TempFullPath },
                { nameof(Filename), Path.GetFileNameWithoutExtension(TempFullPath) },
                { nameof(Extension), Path.GetExtension(TempFullPath) },
                { nameof(DefiningProjectDirectory), DefiningProjectDirectory },
                { definingProjectFullPath, GetMetadata(definingProjectFullPath) },
                { definingProjectName, GetMetadata(definingProjectName) },
                { definingProjectExtension, GetMetadata(definingProjectExtension) }
            });

        string GetMetadata(string name) => item?.GetMetadata(name);

        void Verify()
        {
            if (DefiningProjectDirectory.IsEmpty() || FullPath.IsEmpty())
                throw new KeyNotFoundException($"{nameof(DefiningProjectDirectory)} or {nameof(FullPath)} not found in {item.ToString(null)}");

            if (!File.Exists(FullPath))
                throw new FileNotFoundException(FullPath);

            if (!Directory.Exists(DefiningProjectDirectory))
                throw new DirectoryNotFoundException(DefiningProjectDirectory);

            VerifyFileNameWithExtension();

            if (Link.IsEmpty())
                Link = FullPath.StartsWith(DefiningProjectDirectory)
                    ? FullPath.Replace(DefiningProjectDirectory, string.Empty)
                    : $"{Filename}{Extension}";

            if (Link.IsEmpty())
                throw new ArgumentException($"The relative {nameof(Link)} of the file location in the project could not be determined in {item.ToString(null)}");

            TempFullPath = Path.Combine(tempOutputPath, Link);
        }

        void VerifyFileNameWithExtension()
        {
            if (Filename.IsEmpty())
                Filename = Path.GetFileNameWithoutExtension(FullPath);

            if (Extension.IsEmpty())
                Extension = Path.GetExtension(FullPath);

            if (!Extension.IsEmpty() && !Extension.StartsWith(dot))
                Extension = dot + Extension;

            if (Filename.IsEmpty() && Extension.IsEmpty())
                throw new ArgumentException($"The {nameof(Filename)} and {nameof(Extension)} could not be determined in {item.ToString(null)}");
        }
    }
}
