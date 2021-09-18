using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyLibrary
{
    public class DirectoryManager : IDirectoryService
    {
        public string Root { get; set; }

        public string CreateRootDirectory(string rootName, Environment.SpecialFolder? specialFolder = null, string initDir = "")
        {  
            Root = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)specialFolder) ?? initDir, rootName)).FullName;
            return Root;
        }

        public DirectoryManager CreateSubDirectory(params string[] dirNames)
        {
            return this;//Directory.CreateDirectory(Path.Combine(dirNames)).FullName;
        }

        public void DeleteDirectory(string dirName)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExists(params string[] path)
        {
            return Directory.Exists(Path.Combine(path));
        }
    }
}
