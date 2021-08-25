using System;
using Microsoft.Build.Framework;

namespace CompactJson
{
    public static class Extension
    {
        public static string ToString(this ITaskItem item, string title)
        {
            var names = item?.MetadataNames;
            if (!(names?.Count > 0))
                return $"{title} Metadata is null or empty";

            var result = $"{title} Metadata:{Environment.NewLine}";
            foreach (var name in names)
                result += $"{name}: {item.GetMetadata(name.ToString())}{Environment.NewLine}";

            return result;
        }
    }
}
