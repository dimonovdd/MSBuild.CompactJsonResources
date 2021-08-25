using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
            LogMessage($"{nameof(TempOutputPath)}: {TempOutputPath}");
            try
            {
                if (JsonFiles?.Length > 0)
                    OutputJsonFiles = ParseAndCopyFiles(JsonFiles)?.ToArray();
                else
                    Log.LogMessage($"{nameof(JsonFiles)} is null or empty");

            }
            catch (Exception ex)
            {
                LogMessage(ex.StackTrace);
                Log.LogErrorFromException(ex);
                return false;
            }
            
            return true;
        }


        List<ITaskItem> ParseAndCopyFiles(ITaskItem[] items)
        {
            var output = new List<ITaskItem>();
            foreach(var item in items)
            {
                LogMessage(item.ToString("Original File"));
                var json = new JsonFile(item, TempOutputPath);
                LogMessage($"Temp File Full Path: {json.TempFullPath}");
                var outputItem = json.WriteCompactTempFile();
                LogMessage(outputItem.ToString("Temp File"));
                output.Add(outputItem);
            }
            return output;
        }

       
        void LogMessage(string mess) => Log.LogMessage($"{LogTag}{mess}");
    }
}
