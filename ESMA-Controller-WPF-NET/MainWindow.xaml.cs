using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;
using Process = ESMA.ViewModel.Process;
using ESMA.ViewModel;
using System.Collections.Generic;
using MyLibrary;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Security.AccessControl;
using ESMA.ChangesCloser;

namespace ESMA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BindingList<VideoConference> videoList;
        public BindingList<Changes> changesList;
        public BindingList<Process> processList;
        public BindingList<ChangesCreate> chCreateList;
        public BindingList<PlanCoordinator> pcList;
        public BindingList<ChangesCloserElement> cceList;

        public MainWindow()
        {
            try
            {
                if (!(Directory.Exists(ConfigData.NativeTaskFolderPath) && Directory.Exists(ConfigData.LogsFolderPath)))
                {
                    string t = Directory.CreateDirectory(ConfigData.NativeTaskFolderPath).FullName;
                    string l = Directory.CreateDirectory(ConfigData.LogsFolderPath).FullName;
                    new JsonIO().EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string> { ["TasksFolder"] = t, ["LogsFolder"] = l });
                }

                InitializeComponent();
                HeightListener();

                VC.Header = "Конференции\n";
                C.Header = "ЗИ\n";
                P.Header = "ГТП\n";
                CC.Header = "Создание ЗИ\n";
                PC.Header = "Согласование \nсут.плана";
                CTCl.Header = "Уничтожение ЗИ";

                IData.Window = this;

                Conference.ItemsSource = videoList = new BindingList<VideoConference>();
                Changes.ItemsSource = changesList = new BindingList<Changes>();
                Process.ItemsSource = processList = new BindingList<Process>();
                ChangesCreate.ItemsSource = chCreateList = new BindingList<ChangesCreate>();
                pcList = new BindingList<PlanCoordinator>();
                ChangesClose.ItemsSource = cceList = new BindingList<ChangesCloserElement>();

                DataContext = new AppViewModel(new JsonIO());
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}");
            }

            #if RELEASE
            TestBtn.Visibility = Visibility.Hidden;
            FirstDateDebug.Visibility = Visibility.Hidden;
            FirstDateLabel.Visibility = Visibility.Hidden;
            #endif
        }
        
        private async void HeightListener()
        {
            while (true)
            {
                await Task.Delay(5);
                Height = ActualHeight;
            }
        }
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }
        private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }
        private static void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly) return;

            var dataGrid = FindVisualParent<DataGrid>(cell);
            if (dataGrid == null) return;

            if (!cell.IsFocused) cell.Focus();

            if(cell.Content is CheckBox)
            {
                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                {
                    if (!cell.IsSelected) cell.IsSelected = true;
                }
                else
                {
                    var row = FindVisualParent<DataGridRow>(cell);
                    if (row != null && !row.IsSelected)
                    {
                        row.IsSelected = true;
                    }
                }
            }
            else
            {
                var cb = cell.Content as ComboBox;
                if (cb != null)
                {
                    dataGrid.BeginEdit(e);
                    cell.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                    cb.IsDropDownOpen = true;
                }
            }
        }
        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;

            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null) return correctlyTyped;
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Height > SystemParameters.MaximizedPrimaryScreenHeight - 15 || WindowState == WindowState.Maximized)
            {
                ScrollBar.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                Col1.Width = new GridLength(175, GridUnitType.Pixel);
            }
            else
            {
                ScrollBar.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                Col1.Width = new GridLength(210, GridUnitType.Pixel);
            }
        }

        private void window_Closed(object sender, EventArgs e)
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE");
            foreach (var p in processes)
            {
                p?.Close();
            }
            processes = System.Diagnostics.Process.GetProcessesByName("chromedriver.exe");
            foreach (var p in processes)
            {
                p?.Close();
            }
        }

        private void modulesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppViewModel.MwCurrentTab = modulesList.SelectedIndex;
            ScanBtn.Content = modulesList.SelectedIndex switch
            {
                0 => "Сканировать\nконференции",
                1 => "Сканировать ЗИ",
                2 => "Сканировать ГТП",
                3 => "Сканировать\n текущие ЗИ",
                5 => "Сканировать\n текущие ЗИ",
                _ => "****************",
            };
            AddBtn.Content = modulesList.SelectedIndex switch
            {
                4 => "Задать интервал\nсогласования\nсут.плана",
                _ => "Добавить строку"
            };
            ResetBtn.Content = modulesList.SelectedIndex switch
            {
                4 => "Сброс дат\nсогласования",
                _ => "Сброс"
            };
        }
    }
}
