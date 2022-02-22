using ESMA.DataCollections;
using Microsoft.Win32;
using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESMA
{
    public class AppViewModelSettings
    {
        private readonly IJsonService js;
        private EmpList List { get; set; }
        private ConferenceSettingsWindow CsWindow { get; set; }
        public AppViewModelSettings()
        {

        }
        public AppViewModelSettings(IJsonService jsonService = null, EmpList empList = null, ConferenceSettingsWindow csw = null)
        {
            js = jsonService;
            List = empList;
            CsWindow = csw;
        }

        public RelayCommand Accept
        {
            get => new RelayCommand(obj =>
            {
                if ((IData.CsWindow.StartDate.Text != "" && IData.CsWindow.EndDate.Text != "")
                || (IData.CsWindow.StartDate.Text != "" || IData.CsWindow.EndDate.Text != ""))
                {
                    IData.StartDateValue = DateTime.Parse(IData.CsWindow.StartDate.Text);
                    IData.EndDateValue = DateTime.Parse(IData.CsWindow.EndDate.Text);
                    SettingsInfo("Применено");
                }
                else
                {
                    SettingsInfo("Пусто", true);
                }
            });
        }
        public RelayCommand ResetTime
        {
            get => new RelayCommand(obj =>
            {
                IData.CsWindow.StartDate.Text = DateTime.Now.ToString();
                IData.CsWindow.EndDate.Text = DateTime.Now.ToString();
                IData.StartDateValue = DateTime.Parse(IData.CsWindow.StartDate.Text);
                IData.EndDateValue = DateTime.Parse(IData.CsWindow.EndDate.Text);
                SettingsInfo("Время сброшено");
            });
        }
        public RelayCommand ChangePrimNames
        {
            get => new RelayCommand(async obj =>
            {
                dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                //value-switcher
                string sww = t["SwitchFile"];
                int sw = int.Parse(sww);

                async void Change(string fileName, string def, int @sw)
                {
                    List = new EmpList(fileName);

                    CsWindow.Config = fileName;
                    CsWindow.namesBox.ItemsSource = List;
                    CsWindow.ChangeButton.Content = def;
                    await js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string>
                    {
                        ["EmpListFile"] = fileName,
                        ["SwitchFile"] = @sw.ToString()
                    });

                    var lists = new JSONEmpCollection();

                    for (int i = 0; i < List.Count; i++)
                    {
                        lists.NamesList.Add(List[i].Name);
                        lists.CheckList.Add(List[i].IsChecked);
                    }

                    var str = JsonConvert.SerializeObject(lists);
                    File.WriteAllText(CsWindow.Config, str);
                }

                if (sw == 0)
                {
                    sw++;
                    Change(ConfigData.BoostListFileJSON, "Поменять на обычный\nсписок работников", sw);
                }
                else
                {
                    sw--;
                    Change(ConfigData.NamesListFileJSON, "Поменять на список для накрутки\nчасов на конференции НС/РЦС", sw);
                }
                
                #region old
                //System.Diagnostics.Process p = null;

                //if (Directory.Exists(@"C:\Program Files\Notepad++") && ConfigData.NamesListFile != "null")
                //    p = System.Diagnostics.Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", ConfigData.NamesListFile);
                //else
                //    p = System.Diagnostics.Process.Start("notepad.exe", ConfigData.NamesListFile);

                //await Task.Run(() =>
                //{
                //    while (true)
                //    {
                //        if (p.HasExited)
                //        {
                //            IData.CsWindow.Dispatcher.Invoke(() =>
                //            {
                //                IData.CsWindow.namesBox.ItemsSource = new ObservableCollection<string>(ConfigData.NamesList);
                //            });
                //            break;
                //        }
                //    }
                //}); 
                #endregion
            });
        }
        public RelayCommand Authorization
        {
            get => new RelayCommand(async obj =>
            {
                string login = CsWindow.loginField.Text;
                string password = CsWindow.passwordField.Password;
                try
                {
                    if (login != "" && password != "")
                    {
                        await js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string>
                        {
                            ["Login"] = login,
                            ["Password"] = password
                        });
                        MessageBox.Show("Авторизация завершена", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Пустые поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error");
                }

            });
        }
        public RelayCommand ResetAuthorizationData
        {
            get => new RelayCommand(async obj =>
            {
                await js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string> { ["Login"] = default, ["Password"] = default });
                IData.CsWindow.loginField.Text = default;
                IData.CsWindow.passwordField.Password = default;
            });
        }
        public RelayCommand SilentMode
        {
            get => new RelayCommand(obj =>
            {
                bool t = (bool)CsWindow.SilentModeCheckBox.IsChecked;
                js.EditFile(ConfigData.ConfigurationFilePath, new Dictionary<string, bool> { ["SilentMode"] = t });
            });
        }
        public static async void SettingsInfo(string info, bool @void = false)
        {
            if (@void)
            {
                SystemSounds.Hand.Play();
            }
            IData.CsWindow.infoLabel.Content = info;
            await Task.Delay(4000);
            IData.CsWindow.infoLabel.Content = "";
        }
    }
}
