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
            using var fs = File.OpenRead(FullPath);
            using var jDoc = JsonDocument.Parse(fs, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });

            var directory = Path.GetDirectoryName(TempFullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (File.Exists(TempFullPath))
                File.Delete(TempFullPath);

            using var stream = File.OpenWrite(TempFullPath);
            using var writer = new Utf8JsonWriter(stream);
            jDoc.WriteTo(writer);
            writer.Flush();
            stream.Flush();

            return GetTaskItem();
        }

        public override string ToString() => FullPath;

        TaskItem GetTaskItem()
            => new TaskItem(TempFullPath, new Dictionary<string, string>
            {
                { nameof(Link), Link },
                { nameof(FullPath), TempFullPath },
                { nameof(Filename), Filename },
                { nameof(Extension), Extension },
                { nameof(DefiningProjectDirectory), DefiningProjectDirectory },
                { definingProjectFullPath, GetMetadata(definingProjectFullPath) },
                { definingProjectName, GetMetadata(definingProjectName) },
                { definingProjectExtension, GetMetadata(definingProjectExtension) }
            });

        string GetMetadata(string name) => item?.GetMetadata(name);

        void Verify()
        {
            if (string.IsNullOrWhiteSpace(DefiningProjectDirectory) || string.IsNullOrWhiteSpace(FullPath))
                throw new Exception();

            if (string.IsNullOrWhiteSpace(Link))
                SetLink();

            if (string.IsNullOrWhiteSpace(Link))
                throw new Exception();

            TempFullPath = Path.Combine(tempOutputPath, Link);
        }

        void SetLink()
        {
            var locatedInProject = FullPath.StartsWith(DefiningProjectDirectory);
            Link = locatedInProject
                ? FullPath.Replace(DefiningProjectDirectory, string.Empty)
                : $"{Filename}{Extension}";
        }
    }
}
