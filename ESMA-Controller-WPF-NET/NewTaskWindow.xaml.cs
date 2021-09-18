using ESMA.ViewModel;
using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ESMA
{
    /// <summary>
    /// Логика взаимодействия для NewTaskWindow.xaml
    /// </summary>
    public partial class NewTaskWindow : Window
    {
        public NewTaskWindow()
        {
            InitializeComponent();

            projectField.Text = (ConfigData.InitialDirectory == "" || ConfigData.InitialDirectory == null) ? ConfigData.NativeTaskFolderPath : ConfigData.InitialDirectory;
            IData.NtWindow = this;

            DataContext = new AppViewModel(new JsonIO());
        }
    }
}
