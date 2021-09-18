using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface IJsonService
    {
        void SaveFileAsync<T>(string configFile, Dictionary<string, T> data);
        void EditFile<T>(string configFile, Dictionary<string, T> data);
        Task EditFileAsync<T>(string configFile, Dictionary<string, T> data);
        Task SaveFileDialogAsync<T>(Dictionary<string, T> data);
        dynamic OpenFileDialog();
        Task<dynamic> OpenFileDialogAsync();

        Task<dynamic> GetFieldOnJson(string fileName, string field);
    }

    public interface IDirectoryService
    {
        string Root { get; set; }

        string CreateRootDirectory(string rootName, Environment.SpecialFolder? specialFolder = null, string initDir = "");
        //DirectoryManager CreateSubDirectory(params string[] dirNames);
        void DeleteDirectory(string dirName);

        bool DirectoryExists(params string[] path);
    }

    public interface IFileService
    {
        string FilePath { get; set; }

        BindingList<T> OpenFile<T>(string fileName);
        Task<BindingList<T>> OpenFileAsync<T>(string fileName);
        void SaveFile<T>(string fileName, BindingList<T> list);
        Task SaveFileAsync<T>(string fileName, BindingList<T> list);
        BindingList<T> OpenFileDialog<T>(string extention);
        Task<BindingList<T>> OpenFileDialogAsync<T>(string extention);
        void SaveFileDialog<T>(string extention, BindingList<T> list);
        Task SaveFileDialogAsync<T>(string extention, BindingList<T> list);
        void DeleteFile(string path);
        bool FileExists(string path);
        void ShowMessage(string msg);
    }
}
