using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using static Microsoft.Build.Framework.MessageImportance;

namespace MSBuild.CompactJsonResources
{
    public class CompactJsonTask : Task
    {
        public ITaskItem[] JsonFiles { get; set; }

        [Required]
        public string TempOutputPath { get; set; }

        public string LogTag { get; set; }

        [Output]
        public ITaskItem[] OutputJsonFiles { get; set; }

        public override bool Execute()
        {
            LogMessage($"{nameof(TempOutputPath)}: {TempOutputPath}", High);
            try
            {
                if (JsonFiles?.Length > 0)
                    OutputJsonFiles = ParseAndCopyFiles(JsonFiles)?.ToArray();
                else
                    LogMessage($"{nameof(JsonFiles)} is null or empty");

            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
            
            return true;
        }

        IEnumerable<ITaskItem> ParseAndCopyFiles(ITaskItem[] items)
        {
            foreach(var item in items)
            {
                LogMessage(item.ToString("Original File"), Low);
                var json = new JsonFile(item, TempOutputPath);
                LogMessage($"Temp File Full Path: {json.TempFullPath}");
                var outputItem = json.WriteCompactTempFile();
                LogMessage(outputItem.ToString("Temp File"), Low);
                yield return outputItem;
            }
        }
       
        void LogMessage(string mess, MessageImportance importance = Normal)
            => Log.LogMessage(importance, $"{LogTag}{mess}");
    }
}
