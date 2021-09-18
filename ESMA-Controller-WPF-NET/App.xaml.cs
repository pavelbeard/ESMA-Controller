using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ESMA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "ESMA-Controller-WPF";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Current.Shutdown();
                MessageBox.Show("Приложение запущено!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //SystemSounds.Hand.Play();
            }

            base.OnStartup(e);
        }
    }
}
