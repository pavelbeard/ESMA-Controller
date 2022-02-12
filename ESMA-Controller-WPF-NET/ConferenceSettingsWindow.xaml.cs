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

        public ConferenceSettingsWindow()
        {
            InitializeComponent();
            IData.CsWindow = this;
            namesBox.ItemsSource = new ObservableCollection<string>(ConfigData.NamesList);

            dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
            loginField.Text = t["Login"];
            passwordField.Password = t["Password"];
            SilentModeCheckBox.IsChecked = t["SilentMode"];
            InNightBox.Text = t["InNight"];
            SNightBox.Text = t["SNight"];
            RefrBox.Text = t["RefrEmp"];
            BossBox.Text = t["Boss"];

            DataContext = new AppViewModel(new JsonIO());
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
                js.EditFile(ConfigData.ConfigurationFilePath, new Dictionary<string, string>
                { ["InNight"] = inNight.ToString(), ["SNight"] = sNight.ToString(), ["RefrEmp"] = refrEmp.ToString(), ["Boss"] = boss.ToString() });
            }
            catch (Exception)
            {
                MessageBox.Show($"Нужно заполнить пустые поля");
            }
        }
    }
}
