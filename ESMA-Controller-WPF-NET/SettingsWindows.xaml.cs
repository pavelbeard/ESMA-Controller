using ESMA.DataCollections;
using ESMA.ViewModel;
using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ESMA
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ConferenceSettingsWindow : Window
    {
        private IJsonService js;
        private EmpList List { get; set; }
        public string Config { get; set; }
        private string Switch { get; set; }

        public ConferenceSettingsWindow()
        {
            InitializeComponent();
            IData.CsWindow = this;
            //namesBox.ItemsSource = new ObservableCollection<string>(ConfigData.NamesList);           

            dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
            loginField.Text = t["Login"];
            passwordField.Password = t["Password"];
            SilentModeCheckBox.IsChecked = t["SilentMode"];
            InNightBox.Text = t["InNight"];
            SNightBox.Text = t["SNight"];
            RefrBox.Text = t["RefrEmp"];
            BossBox.Text = t["Boss"];
            //проверка на отсутствие файла
            if (t["EmpListFile"] != null)
            {
                Config = t["EmpListFile"];
                //если строка из конфига будет равна дефолтной
                if (t["EmpListFile"] == ConfigData.NamesListFileJSON && t["SwitchFile"] == "0")
                {
                    ChangeButton.Content = "Поменять на список для накрутки\n" +
                                           "часов на конференции НС/РЦС";
                }
                else
                {
                    ChangeButton.Content = "Поменять на обычный\nсписок работников";
                }
            }
            else
            {
                Config = ConfigData.NamesListFileJSON;
            }
            
            //new names list
            namesBox.ItemsSource = List = new EmpList(Config);

            DataContext = new AppViewModelSettings(new JsonIO(), List, this);
            
        }

        private void window_Closed(object sender, EventArgs e)
        {
            js = new JsonIO();
            var inNight = InNightBox.SelectedItem;
            var sNight = SNightBox.SelectedItem;
            var refrEmp = RefrBox.SelectedItem;
            var boss = BossBox.SelectedItem;

            try
            {
                var lists = new JSONEmpCollection();

                for (int i = 0; i < List.Count; i++)
                {
                    lists.NamesList.Add(List[i].Name);
                    lists.CheckList.Add(List[i].IsChecked);
                }

                var str = JsonConvert.SerializeObject(lists);
                File.WriteAllText(Config, str);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }

            try
            {
                js.EditFile(ConfigData.ConfigurationFilePath, new Dictionary<string, string>
                { 
                    ["InNight"] = inNight.ToString(), 
                    ["SNight"] = sNight.ToString(), 
                    ["RefrEmp"] = refrEmp.ToString(), 
                    ["Boss"] = boss.ToString()
                });;
            }
            catch (Exception)
            {
                MessageBox.Show($"Нужно заполнить пустые поля");
            }
        }
    }
}
