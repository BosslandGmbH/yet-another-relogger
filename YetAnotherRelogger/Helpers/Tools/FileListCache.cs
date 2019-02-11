using System.Collections.Generic;
using System.IO;

namespace YetAnotherRelogger.Helpers.Tools
{
    public class FileListCache
    {
        private readonly string _rootpath;
        public HashSet<MyFile> FileList;

        public FileListCache(string path)
        {
            _rootpath = path;
            UpdateList(path);
        }

        private void UpdateList(string path, bool newlist = true)
        {
            if (newlist)
                FileList = new HashSet<MyFile>();
            if (!path.Equals(_rootpath))
            {
                FileList.Add(new MyFile
                {
                    Path = path.Substring(_rootpath.Length + 1),
                    Directory = true
                });
            }
            foreach (var file in Directory.GetFiles(path))
            {
                FileList.Add(new MyFile
                {
                    Path = file.Substring(_rootpath.Length + 1),
                    Directory = false
                });
            }

            foreach (var dir in Directory.GetDirectories(path))
                UpdateList(dir, false);
        }

        public struct MyFile
        {
            public string Path;
            public bool Directory;
        }
    }
}
