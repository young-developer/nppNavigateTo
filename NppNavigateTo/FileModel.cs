using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NppPluginNET
{
    public class FileModel
    {
        public FileModel(string fileName, string filePath, long fileIndex, long fileBufferId, string view)
        {
            FileName = fileName;
            FilePath = filePath;
            FileBufferId = fileBufferId;
            FileIndex = fileIndex;
            View = view;
        }

        public string FileName { get; set; }
        public long FileBufferId { get; set; }
        public long FileIndex { get; set; }
        public string FilePath { get; set; }
        public string View { get; set; }
    }
}