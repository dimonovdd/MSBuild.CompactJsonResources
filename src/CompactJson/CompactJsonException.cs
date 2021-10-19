using System;

namespace MSBuild.CompactJsonResources
{
    public class CompactJsonException : Exception
    {
        public CompactJsonException(string filePath, Exception inner)
            :base(CreateMess(filePath, inner), inner)
        {
        }

        static string CreateMess(string filePath, Exception inner) =>
            $"Failed to process {filePath} See Build Output. {Environment.NewLine}" +
            $"{inner.GetType()}: {inner.Message}";
    }
}

