using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace MyLibrary
{
    public class FileManager : IFileService
    {
        public string FilePath { get; set; }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public BindingList<T> OpenFile<T>(string fileName)
        {
            var toLoad = new BindingList<T>();
            var reader = new XmlSerializer(typeof(BindingList<T>));
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    toLoad = (BindingList<T>)reader.Deserialize(fs);
                }
                return toLoad;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public Task<BindingList<T>> OpenFileAsync<T>(string fileName)
        {
            return Task.Run(() => OpenFile<T>(fileName));
        }

        public void SaveFile<T>(string fileName, BindingList<T> list)
        {
            var writer = new XmlSerializer(typeof(BindingList<T>));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                try
                {
                    writer.Serialize(fs, list);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public Task SaveFileAsync<T>(string fileName, BindingList<T> list)
        {
            return Task.Run(() => SaveFile(fileName, list));
        }

        public BindingList<T> OpenFileDialog<T>(string extention)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Открыть файл",
                Filter = $"Открыть файл .{extention}|*.{extention}"
            };

            var toLoad = new BindingList<T>();
            if (ofd.ShowDialog() == true)
            {
                FilePath = ofd.FileName;
                var reader = new XmlSerializer(typeof(BindingList<T>));
                using(FileStream fs = new FileStream(ofd.FileName, FileMode.OpenOrCreate))
                {
                    try
                    {
                        toLoad = (BindingList<T>)reader.Deserialize(fs);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                return null;
            }
            return toLoad;
        }

        public Task<BindingList<T>> OpenFileDialogAsync<T>(string extention)
        {
            return Task.Run(() => OpenFileDialog<T>(extention));
        }

        public void SaveFileDialog<T>(string extention, BindingList<T> list)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Сохранить файл",
                Filter = $"Сохранить файл .{extention}|*.{extention}"
            };

            if (sfd.ShowDialog() == true)
            {
                FilePath = sfd.FileName;
                var writer = new XmlSerializer(typeof(BindingList<T>));
                using(FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    try
                    {
                        writer.Serialize(fs, list);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public Task SaveFileDialogAsync<T>(string extention, BindingList<T> list)
        {
            return Task.Run(() => SaveFileDialog(extention, list));
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg, "Сообщение...", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
