using ESMA.Chromedriver;
using ESMA.Controllers;
using ESMA.DataLoaders;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using MyLibrary;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Diagnostics;
using ESMA.ChangesCloser;

namespace ESMA.ViewModel
{
    public class AppViewModel
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private IProgress<int> scanProgress;
        private IProgress<double> runProgress;
        private ChromeController cc;
        private StringBuilder path;

        public static int MwCurrentTab { get; set; } = 0;

        private VideoConference AddConference
        {
            get => new VideoConference
            {
                IdConference = 0,
                VC_Theme = "null",
                VC_Date = DateTime.Parse(DateTime.Now.ToString("D")),
                VC_TimeStart = DateTime.Parse("00:00"),
                VC_TimeEnd = DateTime.Parse("00:00"),
                VC_Job = "null",
                VC_Names = new ObservableCollection<string>(ConfigData.NamesList),
                OperPersonal = true,
                CloseConference = true
            };
        }
        private Changes AddChanges
        {
            get => new Changes
            {
                IdChanges = 0,
                C_Description = "null",
                C_TimeStart = DateTime.Parse("00:00"),
                C_TimeEnd = DateTime.Parse("00:00"),
                C_Job = "null",
                C_Names = new ObservableCollection<string>(ConfigData.NamesList)
            };
        }
        private Process AddProcess
        {
            get => new Process
            {
                IdProcess = 0,
                P_Description = "null",
                P_TimeStart = DateTime.Parse("00:00"),
                P_Job = "null",
                P_Event = "null",
                P_Names = new ObservableCollection<string>(ConfigData.NamesList)
            };
        }
        private CTC AddCTC
        {
            get => new CTC
            {
                IdCTC = 0,
                CTC_Description = "null",
                CTC_DateStart = DateTime.Now,
                CTC_DateEnd = DateTime.Now,
                CTC_TimeStart = DateTime.Parse("00:00"),
                CTC_TimeEnd = DateTime.Parse("00:00")
            };
        }
        private PlanCoordinator AddPC
        {
            get => new()
            {
                StartDate = DateTime.Parse(IData.Window.FirstDP.Text),
                EndDate = DateTime.Parse(IData.Window.SecondDP.Text)
            };
        }
        
        private readonly IJsonService js;

        public AppViewModel()
        {

        }
        public AppViewModel(IJsonService jsonService)
        {
            js = jsonService;
        }

        public RelayCommand Scan
        {
            get => new(async obj =>
            {
                IData.Window.StopBtn.IsEnabled = false;
                scanProgress = new Progress<int>(value =>
                {
                    Duration duration = new Duration(TimeSpan.FromSeconds(2));
                    DoubleAnimation da = new DoubleAnimation(value, duration);
                    IData.Window.ProgressBar_Main.BeginAnimation(ProgressBar.ValueProperty, da);
                    IData.Window.ProgressBar_Main_Text.Text = $"{value}%";
                });
                ShowProgressBar();

                dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));

                try
                {
                    if (MwCurrentTab == 0)
                    {
                        cc = new VCLoader
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        var vcLoad = await cc.LoadDataAsync<VideoConference>(scanProgress);
                        if (vcLoad != null)
                        {
                            foreach (var item in vcLoad)
                            {
                                IData.Window.videoList.Add(item);
                            }
                        }
                    }
                    if (MwCurrentTab == 1)
                    {
                        cc = new ChangesLoader
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        var cLoad = await cc.LoadDataAsync<Changes>(scanProgress);
                        if (cLoad != null)
                        {
                            foreach (var item in cLoad)
                            {
                                IData.Window.changesList.Add(item);
                            }
                        }
                    }
                    if (MwCurrentTab == 2)
                    {
                        cc = new ProcessLoader
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        var pLoad = await cc.LoadDataAsync<Process>(scanProgress);
                        if (pLoad != null)
                        {
                            foreach (var item in pLoad)
                            {
                                IData.Window.processList.Add(item);
                            }
                        }
                    }
                    if (MwCurrentTab == 3)
                    {
                        cc = new CTCLoader
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        var ctcLoad = await cc.LoadDataAsync<CTC>(scanProgress);
                        if (ctcLoad != null)
                        {
                            foreach (var item in ctcLoad)
                            {
                                IData.Window.ctcList.Add(item);
                            }
                        }
                    }
                    if (MwCurrentTab == 5)
                    {
                        cc = new ChangesCloserLoader
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };

                        var cceLoad = await cc.LoadDataAsync<ChangesCloserElement>(scanProgress);
                        if (cceLoad != null)
                        {
                            foreach (var item in cceLoad)
                            {
                                IData.Window.cceList.Add(item);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                HideProgressBar();
                IData.Window.StopBtn.IsEnabled = true;
            },
            obj =>
            {
                dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));

                if (IData.Window?.modulesList.IsVisible ?? false)
                {
                    bool check = (t["Login"] != null) && (t["Password"] != null) && 
                    IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden;
                    return MwCurrentTab switch
                    {
                        0 => IData.Window?.videoList?.Count == 0 && check,
                        1 => IData.Window?.changesList?.Count == 0 && check,
                        2 => IData.Window?.processList?.Count == 0 && check,
                        3 => IData.Window?.ctcList?.Count == 0 && check,
                        5 => IData.Window?.cceList?.Count == 0 && check,
                        _ => false,
                    };
                }
                return false;
            });
        }
        public RelayCommand NewTask
        {
            get => new RelayCommand(obj =>
            {
                if (IData.Window.modulesList.IsVisible)
                {
                    IData.Window.modulesList.Visibility = System.Windows.Visibility.Hidden;
                    IData.Window.videoList?.Clear();
                    IData.Window.changesList?.Clear();
                    IData.Window.processList?.Clear();
                    IData.Window.ctcList?.Clear();
                    IData.Window.cceList?.Clear();
                }
                var newTask = new NewTaskWindow
                {
                    Owner = IData.Window
                };

                newTask.ShowDialog();
            });
        }
        public RelayCommand CreateTask
        {
            get => new RelayCommand(async obj =>
            {
                await Task.Run(() =>
                {
                    IData.Window.Dispatcher.Invoke(() =>
                    {
                        path = new StringBuilder(Path.Combine(ConfigData.InitialDirectory, IData.NtWindow.inputField.Text));
                        js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string> { ["UserTasksFolder"] = path.ToString() });

                        var taskRoot = Directory.CreateDirectory(path.ToString());

                        var file = $"{taskRoot.CreateSubdirectory("TablesConfig").FullName}\\{IData.NtWindow.inputField.Text}.json";
                        File.WriteAllText(file, JsonConvert.SerializeObject(new
                        {
                            Conferences = new BindingList<VideoConference>(),
                            Changes = new BindingList<Changes>(),
                            Processes = new BindingList<Process>(),
                            CTCs = new BindingList<CTC>(),
                            ChangesCloserElement = new BindingList<ChangesCloserElement>()
                        }));

                        ConfigData.TablesConfigPath = file;
                        ConfigData.TaskFilePath = taskRoot.FullName;

                        File.WriteAllText($"{path}\\{IData.NtWindow.inputField.Text}.tjson", JsonConvert.SerializeObject(new { TablesConfig = file }));

                        IData.Window.VC.Header = $"Конференции\n{Path.GetFileName(file)}";
                        IData.Window.C.Header = $"ЗИ\n{Path.GetFileName(file)}";
                        IData.Window.P.Header = $"ГТП\n{Path.GetFileName(file)}";
                        IData.Window.CC.Header = $"Создание ЗИ\n{Path.GetFileName(file)}";
                        IData.Window.PC.Header = "Согласование \nсут.плана (Бета-версия)";
                        IData.Window.CTCl.Header = $"Уничтожение ЗИ\n{Path.GetFileName(file)}";

                        IData.NtWindow.Close();
                        IData.Window.modulesList.Visibility = Visibility.Visible;
                    });
                });
            });
        }
        public RelayCommand OpenTask
        {
            get => new RelayCommand(async obj => 
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Открыть задачу",
                    InitialDirectory = await js.GetFieldOnJson(ConfigData.ConfigurationFilePath, "TasksFolder") as string,
                    Filter = "Файл задачи |*.tjson"
                };

                if (ofd.ShowDialog() == true)
                {
                    ConfigData.TaskFilePath = ofd.FileName;
                    dynamic task = JsonConvert.DeserializeObject(File.ReadAllText(ofd.FileName));
                    string file = task["TablesConfig"];
                    if (File.Exists(file))
                    {
                        ConfigData.TablesConfigPath = file;
                        await js.OpenConfig(file);

                        IData.Window.modulesList.Visibility = Visibility.Visible; 
                    }
                    else
                    {
                        MessageBox.Show($"Файла по адресу {file} не существует", "Не найден файл", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
        }
        public RelayCommand OpenConfiguration
        {
            get => new RelayCommand(async obj =>
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Открыть таблицы",
                    RestoreDirectory = true,
                    Filter = "Открыть файл таблиц |*.json"
                };

                if (ofd.ShowDialog() == true)
                {
                   await js.OpenConfig(ofd.FileName);
                }
            },
                obj => IData.Window.modulesList.Visibility == Visibility.Visible);
        }
        public RelayCommand SaveConfiguration
        {
            get => new RelayCommand(async obj =>
            {
                try
                {
                    string t = JsonConvert.SerializeObject(new
                    {
                        Conferences = IData.Window.videoList,
                        Changes = IData.Window.changesList,
                        Processes = IData.Window.processList,
                        CTCs = IData.Window.ctcList,
                        ChangesCloserElements = IData.Window.cceList
                    });

                    File.WriteAllText(ConfigData.TablesConfigPath, t);
                    await Info($"Файл {Path.GetFileName(ConfigData.TablesConfigPath)} сохранен");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e}");
                }
            },
                obj => IData.Window.modulesList.Visibility == Visibility.Visible);
        }
        public RelayCommand SaveAsCongiguration
        {
            get => new RelayCommand(async obj => 
            {
                try
                {
                    await Task.Run(() =>
                    {
                        IData.Window.Dispatcher.Invoke(() =>
                        {
                            string t = JsonConvert.SerializeObject(new
                            {
                                Conferences = IData.Window.videoList,
                                Changes = IData.Window.changesList,
                                Processes = IData.Window.processList,
                                CTSs = IData.Window.ctcList,
                                ChangesCloserElements = IData.Window.cceList
                            });

                            SaveFileDialog sfd = new SaveFileDialog
                            {
                                Title = "Сохранить таблицы",
                                RestoreDirectory = true,
                                Filter = "Файл таблиц|*.json"
                            };

                            if (sfd.ShowDialog() == true)
                            {
                                ConfigData.TablesConfigPath = sfd.FileName;
                                js.EditFile(ConfigData.TaskFilePath, new Dictionary<string, string> { ["TablesConfig"] = sfd.FileName });
                                File.WriteAllText(sfd.FileName, t);
                                IData.Window.VC.Header = $"Конференции\n{Path.GetFileName(sfd.FileName)}";
                                IData.Window.C.Header = $"ЗИ\n{Path.GetFileName(sfd.FileName)}";
                                IData.Window.P.Header = $"ГТП\n{Path.GetFileName(sfd.FileName)}";
                                IData.Window.CC.Header = $"Создание ЗИ\n{Path.GetFileName(sfd.FileName)}";
                                IData.Window.PC.Header = $"Согласование сут. плана";
                                IData.Window.CTCl.Header = $"Уничтожение ЗИ\n{Path.GetFileName(sfd.FileName)}";
                            }
                        });
                    });

                    await Info("Конфиг успешно сохранен");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e}");
                }
            },
                obj => IData.Window.modulesList.Visibility == Visibility.Visible);
        }
        public RelayCommand Add
        {
            get => new RelayCommand(obj =>
            {
                switch (MwCurrentTab)
                {
                    case 0: IData.Window.videoList.Add(AddConference); break;
                    case 1: IData.Window.changesList.Add(AddChanges); break;
                    case 2: IData.Window.processList.Add(AddProcess); break;
                    case 3: IData.Window.ctcList.Add(AddCTC); break;
                    case 4: IData.Window.pcList.Add(AddPC); break;
                    case 5: IData.Window.cceList.Add(new ChangesCloserElement { IdCCE = 0, CCE_Description = "null" }); break;
                    default: break;
                }
            },
                (obj) => IData.Window?.modulesList.IsVisible == true && 
                IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden);
        }
        public RelayCommand Insert
        {
            get => new(obj => 
            {
                try
                {
                    switch (MwCurrentTab)
                    {
                        case 0:
                            if (IData.Window.videoList.Count < 1)
                                IData.Window.videoList.Add(AddConference);
                            else
                                IData.Window.videoList.Insert(IData.Window.Conference.SelectedIndex, AddConference);
                            break;
                        case 1:
                            if (IData.Window.changesList.Count < 1)
                                IData.Window.changesList.Add(AddChanges);
                            else
                                IData.Window.changesList.Insert(IData.Window.Changes.SelectedIndex, AddChanges);
                            break;
                        case 2:
                            if (IData.Window.processList.Count < 1)
                                IData.Window.processList.Add(AddProcess);
                            else
                                IData.Window.processList.Insert(IData.Window.Process.SelectedIndex, AddProcess);
                            break;
                        case 3:
                            if (IData.Window.ctcList.Count < 1)
                                IData.Window.ctcList.Add(AddCTC);
                            else
                                IData.Window.ctcList.Insert(IData.Window.ChangesCreate.SelectedIndex, AddCTC);
                            break;
                        case 5:
                            if (IData.Window.ctcList.Count < 1)
                                IData.Window.cceList.Add(new ChangesCloserElement { IdCCE = 0, CCE_Description = "null" });
                            else
                                IData.Window.cceList.Insert(IData.Window.ChangesClose.SelectedIndex, new ChangesCloserElement { IdCCE = 0, CCE_Description = "null" });
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                }
            },
                (obj) => IData.Window?.modulesList.IsVisible == true && 
                IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden &&
                MwCurrentTab != 4);
        }
        public RelayCommand Delete
        {
            get => new(obj =>
            {
                switch (MwCurrentTab)
                {
                    case 0:
                        if (IData.Window.videoList.Count > 0)
                            IData.Window.videoList.RemoveAt(IData.Window.Conference.SelectedIndex);
                        break;
                    case 1:
                        if (IData.Window.changesList.Count > 0)
                            IData.Window.changesList.RemoveAt(IData.Window.Changes.SelectedIndex);
                        break;
                    case 2:
                        if (IData.Window.processList.Count > 0)
                            IData.Window.processList.RemoveAt(IData.Window.Process.SelectedIndex);
                        break;
                    case 3:
                        if (IData.Window.ctcList.Count > 0)
                            IData.Window.ctcList.RemoveAt(IData.Window.ChangesCreate.SelectedIndex);
                        break;
                    case 5:
                        if (IData.Window.cceList.Count > 0)
                            IData.Window.cceList.RemoveAt(IData.Window.ChangesClose.SelectedIndex);
                        break;
                    default:
                        break;
                }
            },
            (obj) =>
            {
                if (IData.Window?.modulesList.IsVisible == true && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden)
                {
                    return MwCurrentTab switch
                    {
                        0 => IData.Window?.videoList?.Count > 0,
                        1 => IData.Window?.changesList?.Count > 0,
                        2 => IData.Window?.processList?.Count > 0,
                        3 => IData.Window?.ctcList?.Count > 0,
                        5 => IData.Window?.cceList?.Count > 0,
                        _ => false,
                    }; 
                }
                return false;
            });
        }
        public RelayCommand Start
        {
            get => new(async obj => 
            {
                try
                {
                    if (!StopTask.PauseRequest)
                    {
                        ShowProgressBar();
                    }

                    runProgress = new Progress<double>(value =>
                    {
                        Duration duration = new(TimeSpan.FromSeconds(2));
                        DoubleAnimation da = new(value, duration);
                        IData.Window.ProgressBar_Main.BeginAnimation(ProgressBar.ValueProperty, da);
                        IData.Window.ProgressBar_Main_Text.Text = $"{Math.Round(value)}%";
                    });
       
                    if (cts.IsCancellationRequested)
                    {
                        cts.Dispose();
                        cts = new CancellationTokenSource();
                    }

                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));

                    if (MwCurrentTab == 0)
                    {
                        IData.VideoConferences = IData.Window.videoList;
                        cc = new VideoConferenceController
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        await cc.RunAsync(cts.Token, runProgress);
                    }
                    if (MwCurrentTab == 1)
                    {
                        IData.Changes = IData.Window.changesList;
                        cc = new ChangesController
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        await cc.RunAsync(cts.Token, runProgress);
                    }
                    if (MwCurrentTab == 2)
                    {
                        IData.Processes = IData.Window.processList;
                        cc = new ProcessController
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        await cc.RunAsync(cts.Token, runProgress);
                    }
                    if (MwCurrentTab == 3)
                    {
                        IData.CTCs = IData.Window.ctcList;
                        ChangesCreatorController cc = new()
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        await cc.CreateAsync(runProgress);
                    }
                    if (MwCurrentTab == 4)
                    {
                        IData.PCs = IData.Window.pcList;
                        PlanCoordinatorController pcc = new()
                        {
                            Login = "17_1_andruhin_vs",
                            Password = "rcs1_3",
                            CurrentTableIndex = MwCurrentTab
                        };
                        await pcc.Coordinate(runProgress);
                    }
                    if (MwCurrentTab == 5)
                    {
                        IData.ChangesCloserElements = IData.Window.cceList;
                        ChangesCloserController ccc = new()
                        {
                            Login = t["Login"],
                            Password = t["Password"],
                            CurrentTableIndex = MwCurrentTab
                        };
                        await ccc.RunAsync(cts.Token, runProgress);
                    }

                    if (!StopTask.PauseRequest)
                    {
                        HideProgressBar();
                        if (!StopTask.StopRequest && StopTask.NoConnectionException == false)
                        {
                            MessageBox.Show("Завершено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            ResetInfo();
                        }
                        else
                        {
                            StopTask.StopRequest = false;
                        }
                        runProgress.Report(0);
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    HideProgressBar();
                    ResetInfo();
                    runProgress.Report(0);
                }
            },
            obj =>
            {
                dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));

                bool check = (t["Login"] != null) && (t["Password"] != null) && 
                IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden || StopTask.PauseRequest == true;
                return MwCurrentTab switch
                {
                    0 => IData.Window?.videoList?.Count > 0 && check,
                    1 => IData.Window?.changesList?.Count > 0 && check,
                    2 => IData.Window?.processList?.Count > 0 && check,
                    3 => IData.Window?.ctcList?.Count > 0 && check,
                    4 => IData.Window?.pcList?.Count == 0,
                    5 => IData.Window?.cceList?.Count > 0 && check,
                    _ => false,
                };
            });
        }
        public RelayCommand Stop
        {
            get => new RelayCommand(obj => 
            {
                cts.Cancel();
                StopTask.StopRequest = true;
            },
                obj => IData.Window.ProgressBar_Grid.Visibility == Visibility.Visible && IData.Window.StartBtn.IsEnabled == false);
        }
        public RelayCommand Pause
        {
            get => new(obj => 
            {
                cts.Cancel();
                StopTask.PauseRequest = true;
            }, 
               obj => IData.Window.ProgressBar_Grid.Visibility == Visibility.Visible && IData.Window.StartBtn.IsEnabled == false);
        }
        public RelayCommand StopDoubleClick
        {
            get => new(obj => 
            {
                cts.Cancel();
                StopTask.PauseRequest = true;
            },
               obj => IData.Window.ProgressBar_Grid.Visibility == Visibility.Visible);
        }
        public RelayCommand Reset
        {
            get => new(obj => 
            {
                static async void ClearTableInfo(string t)
                {
                    IData.Window.StatusBar.Content = $"{t} очищена";
                    await Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        IData.Window.Dispatcher.Invoke(() => { IData.Window.StatusBar.Content = ""; });
                    });
                }

                switch (MwCurrentTab)
                {
                    case 0:
                        IData.Window.videoList?.Clear();
                        ClearTableInfo("Таблица \"Конференции\"");
                        break;
                    case 1:
                        IData.Window.changesList?.Clear();
                        ClearTableInfo("Таблица \"ЗИ\"");
                        break;
                    case 2:
                        IData.Window.processList?.Clear();
                        ClearTableInfo("Таблица \"ГТП\"");
                        break;
                    case 3:
                        IData.Window.ctcList?.Clear();
                        MessageBox.Show("Таблица \"Создание ЗИ\" очищена", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case 4:
                        IData.Window.pcList?.Clear();
                        IData.Window.FirstDP.SelectedDate = IData.Window.SecondDP.SelectedDate = default;
                        MessageBox.Show("Модуль \"Согласование сут.плана\" очищен", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case 5:
                        IData.Window.cceList?.Clear();
                        MessageBox.Show("Модуль \"Уничтожение ЗИ\" очищен", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    default:
                        break;
                }
            },
            (obj) =>
            {
                return MwCurrentTab switch
                {
                    0 => IData.Window?.videoList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
                    1 => IData.Window?.changesList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
                    2 => IData.Window?.processList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
                    3 => IData.Window?.ctcList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
                    4 => IData.Window?.pcList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
                    5 => IData.Window?.cceList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
                    _ => false,
                };
            });
        }
        public RelayCommand ChangeNames
        {
            get => new RelayCommand(obj => 
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Открыть ФИО работников",
                    Filter = "Открыть .txt файл|*.txt",
                    RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    switch (MwCurrentTab)
                    {
                        case 0:
                            IData.Window.videoList[IData.Window.Conference.SelectedIndex].VC_Names = 
                                new ObservableCollection<string>(File.ReadAllLines(openFileDialog.FileName));
                            break;
                        case 1:
                            IData.Window.changesList[IData.Window.Changes.SelectedIndex].C_Names =
                               new ObservableCollection<string>(File.ReadAllLines(openFileDialog.FileName));
                            break;
                        case 2:
                            IData.Window.processList[IData.Window.Process.SelectedIndex].P_Names =
                               new ObservableCollection<string>(File.ReadAllLines(openFileDialog.FileName));
                            break;
                        default:
                            break;
                    }
                }
            });
        }
        public RelayCommand ChangePrimNames
        {
            get => new RelayCommand(async obj => 
            {
                System.Diagnostics.Process p = null;

                if (Directory.Exists(@"C:\Program Files\Notepad++") && ConfigData.NamesListFile != "null")
                   p = System.Diagnostics.Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", ConfigData.NamesListFile);
                else
                   p = System.Diagnostics.Process.Start("notepad.exe", ConfigData.NamesListFile);

                await Task.Run(() => 
                {
                    while (true)
                    {
                        if (p.HasExited)
                        {
                            IData.CsWindow.Dispatcher.Invoke(() =>
                            {
                                IData.CsWindow.namesBox.ItemsSource = new ObservableCollection<string>(ConfigData.NamesList);
                            });
                            break;
                        }
                    }
                });
            });
        }
        public RelayCommand OpenConferenceSettings
        {
            get => new RelayCommand(obj => 
            {
                var conference = new ConferenceSettingsWindow
                {
                    Owner = IData.Window
                };

                conference.ShowDialog();
            },
            obj => IData.Window?.modulesList.IsVisible ?? false);
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
        public RelayCommand Authorization
        {
            get => new RelayCommand(async obj => 
            {
                string login = IData.CsWindow.loginField.Text;
                string password = IData.CsWindow.passwordField.Password;
                if (login != "" && password != "")
                {
                    await js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string> { ["Login"] = login, ["Password"] = password });
                    MessageBox.Show("Авторизация завершена", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Пустые поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public RelayCommand ResetData
        {
            get => new RelayCommand(async obj => 
            { 
                //await js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, T>)
            });
        }
        public RelayCommand SilentMode
        {
            get => new RelayCommand(obj => 
            {
                bool t = (bool)IData.CsWindow.SilentModeCheckBox.IsChecked;
                js.EditFile(ConfigData.ConfigurationFilePath, new Dictionary<string, bool> { ["SilentMode"] = t});
            });
        }
        public RelayCommand BrowseFolder
        {
            get => new RelayCommand(obj =>
            {
                var folderDialog = new CommonOpenFileDialog
                {
                    Title = "Назначить папку для задачи корневой",
                    IsFolderPicker = true,
                    InitialDirectory = ConfigData.NativeTaskFolderPath
                };

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    IData.NtWindow.projectField.Text = folderDialog.FileName;
                    js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, string> { ["TasksFolder"] = folderDialog.FileName });
                }
            });
        }
        
        public RelayCommand About
        {
            get => new RelayCommand(obj =>
            {
                string about =
                string.Format(
                $"{"Эта программа создана для того, чтобы облегчить \nи без того трудную жизнь на связи совещаний\n", 10}"+
                $"ESMA-Controller " +
                $"v{Assembly.GetExecutingAssembly().GetName().Version}");
                MessageBox.Show(about);
            });
        }

        public RelayCommand CreateReport
        {
            get => new(obj =>
            {
                if (ReportData.Report == null)
                {
                    if (MessageBox.Show("Программа еще не отработала, вы точно хотите создать отчет?",
                    "Создать отчет",
                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        ReportData reportData = new ReportData();

                        for (int i = 0; i < IData.Window.changesList.Count; i++)
                        {
                            for (int j = 0; j < IData.Window.changesList[i].C_Names.Count; j++)
                            {
                                reportData.Emps.Add(IData.Window.changesList[i].C_Names[j]);
                            }

                            if (IData.Window.changesList[i].IdChanges.ToString().Length < 4)
                            {
                                reportData.Lrps.Add("0000");
                            }
                            else
                            {
                                reportData.Lrps.Add(IData.Window.changesList[i].IdChanges.ToString());
                            }
                        }

                        ExcelDataCreator.NewReport(reportData);
                        MessageBox.Show($"Отчет создан.\nОн находится в:\n{Environment.CurrentDirectory}\\Reports");
                    }
                }
                else
                {
                    ReportData reportData = ReportData.Report;
                    ExcelDataCreator.NewReport(reportData);
                    ReportData.Report = null;
                    MessageBox.Show($"Отчет создан.\nОн находится в:\n{Environment.CurrentDirectory}\\Reports");
                }
                
            },
                check => IData.Window.changesList.Count > 0);
        }

        #region DEBUG
        #if RELEASE
        public RelayCommand Debug
        {
            get => new RelayCommand(async obj => 
            {
                //#region INIT
                //if (!StopTask.PauseRequest)
                //{
                //    ShowProgressBar();
                //}

                //if (cts.IsCancellationRequested)
                //{
                //    cts.Dispose();
                //    cts = new CancellationTokenSource();
                //}

                //runProgress = new Progress<double>(value =>
                //{
                //    Duration duration = new Duration(TimeSpan.FromSeconds(2));
                //    DoubleAnimation da = new(value, duration);
                //    IData.Window.ProgressBar_Main.BeginAnimation(ProgressBar.ValueProperty, da);
                //    IData.Window.ProgressBar_Main_Text.Text = $"{Math.Round(value)}%";
                //});
                //#endregion
                //#region EXECUTE
                //dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                ////var chromeController = new VideoConferenceController
                ////{
                ////    Login = t["Login"],
                ////    Password = t["Password"],
                ////    CurrentTableIndex = MwCurrentTab
                ////};
                ////IData.VideoConferences = IData.Window.videoList;
                //var chromeController = new ChangesController
                //{
                //    Login = t["Login"],
                //    Password = t["Password"],
                //    CurrentTableIndex = MwCurrentTab
                //};
                //IData.Changes = IData.Window.changesList;
                //await chromeController.TestReport(cts.Token, runProgress);
                //#endregion
                //#region END
                //if (!StopTask.PauseRequest)
                //{
                //    HideProgressBar();
                //    if (!StopTask.StopRequest)
                //    {
                //        MessageBox.Show("Завершено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                //    }
                //    else
                //    {
                //        StopTask.StopRequest = false;
                //    }
                //    runProgress.Report(0);
                //}
                //#endregion

                #region DEBUGING
                #if RELEASE
                string fd = IData.Window.FirstDateDebug.Text;

                if ((fd != "" || fd != null))
                {
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                    var chromeController = new VideoConferenceController
                    {
                        Login = t["Login"],
                        Password = t["Password"],
                        CurrentTableIndex = MwCurrentTab
                    };
                    IData.VideoConferences = IData.Window.videoList;
                    await chromeController.Debug(fd);   
                }
                else
                {
                    MessageBox.Show("input is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                #endif
                #endregion


            },
            obj => IData.Window?.ProgressBar_Grid.IsVisible == false || StopTask.PauseRequest == true);
        }
        public RelayCommand DebugHours
        {
            get => new RelayCommand(action =>
            {
                IData.Window.FirstDateDebug.Visibility = IData.CsWindow.DebugBox.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
                IData.Window.FirstDateLabel.Visibility = IData.CsWindow.DebugBox.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
                IData.Window.TestBtn.Visibility = IData.CsWindow.DebugBox.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
            });
        }
        #endif
        #endregion

        public static Task Info(string info)
        {
            return Task.Run(() => 
            {
                SystemSounds.Asterisk.Play();
                IData.Window.Dispatcher.Invoke(() => IData.Window.StatusBar.Content = info);
                Task.Delay(3000);
                IData.Window.Dispatcher.Invoke(() => IData.Window.StatusBar.Content = "");
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
        public static async void MainWindowInfo(string info, bool @void = false)
        {
            if (@void)
            {
                SystemSounds.Hand.Play();
            }
            IData.Window.StatusBar.Content = info;
            await Task.Delay(4000);
            IData.Window.StatusBar.Content = "";
        }

        public static async void ShowProgressBar()
        {
            await Task.Run(() => 
            {
                IData.Window.Dispatcher.Invoke(() =>
                {
                    IData.Window.DockPanel_Bottom.SetBinding(DockPanel.WidthProperty, new Binding
                    {
                        ElementName = "ProgressBar_Grid",
                        Path = new PropertyPath("ActualWidth"),
                        Mode = BindingMode.OneWay
                    });
                    IData.Window.Info.Visibility = Visibility.Visible;
                    IData.Window.modulesList.Margin = new Thickness(5, 24, 5, 130);
                    IData.Window.ProgressBar_Grid.Visibility = Visibility.Visible;
                    
                });
                Thread.Sleep(5000);
            });
        }
        public static async void HideProgressBar()
        {
            await Task.Run(() => 
            {
                IData.Window.Dispatcher.Invoke(() =>
                {
                    IData.Window.DockPanel_Bottom.Width = IData.Window.dockPanel.ActualWidth;
                    IData.Window.Info.Visibility = Visibility.Hidden;
                    IData.Window.modulesList.Margin = new Thickness(5, 24, 5, 65);
                    IData.Window.ProgressBar_Grid.Visibility = Visibility.Hidden;
                });
            });
        }
        private static void ResetInfo()
        {
            IData.Window.Info.Text = default;
            var chromdrivers = System.Diagnostics.Process.GetProcessesByName("chromedriver");
            foreach (var p in chromdrivers)
            {
                p.Close();
            }
        }
    }

    public class VideoConference : INotifyPropertyChanged
    {
        private ObservableCollection<string> vc_Names;
        private string vc_Status;

        public int IdConference { get; set; }
        public string VC_Theme { get; set; }
        public DateTime VC_Date { get; set; }
        public DateTime VC_TimeStart { get; set; }
        public DateTime VC_TimeEnd { get; set; }
        public string VC_Job { get; set; }
        public bool OperPersonal { get; set; }
        public ObservableCollection<string> VC_Names
        {
            get => vc_Names;
            set
            {
                vc_Names = value;
                OnPropertyChanged("VC_Names");
            }
        }
        public bool CloseConference { get; set; }
        public string CloseCode { get; set; }
        public string VC_Status
        {
            get => vc_Status;
            set
            {
                vc_Status = value;
                OnPropertyChanged("VC_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class Changes : INotifyPropertyChanged
    {
        private ObservableCollection<string> c_Names;
        private string c_Status;

        public int IdChanges { get; set; }
        public string C_Description { get; set; }
        public DateTime C_TimeStart { get; set; }
        public DateTime C_TimeEnd { get; set; }
        public string C_Job { get; set; }
        public ObservableCollection<string> C_Names
        {
            get { return c_Names; }
            set
            {
                c_Names = value;
                OnPropertyChanged("C_Names");
            }
        }
        public string C_Status
        {
            get => c_Status;
            set
            {
                c_Status = value;
                OnPropertyChanged("C_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class Process : INotifyPropertyChanged
    {
        private ObservableCollection<string> p_Names;
        private string p_Status;

        public int IdProcess { get; set; }
        public string P_Description { get; set; }
        public DateTime P_TimeStart { get; set; }
        public string P_Job { get; set; }
        public string P_Event { get; set; }
        public ObservableCollection<string> P_Names
        {
            get => p_Names;
            set
            {
                p_Names = value;
                OnPropertyChanged("P_Names");
            }
        }
        public string P_Status
        {
            get => p_Status;
            set
            {
                p_Status = value;
                OnPropertyChanged("P_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class CTC : INotifyPropertyChanged
    {
        private string ctc_Status;

        public int IdCTC { get; set; }
        public string CTC_Description { get; set; }
        public DateTime CTC_DateStart { get; set; }
        public DateTime CTC_DateEnd { get; set; }
        public DateTime CTC_TimeStart { get; set; }
        public DateTime CTC_TimeEnd { get; set; }
        public string CTC_Status
        {
            get => ctc_Status;
            set
            {
                ctc_Status = value;
                OnPropertyChanged("CTC_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class StringConverter : IMultiValueConverter
    {
        public static string path;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (parameter as string) switch
            {
                "Concat" => path = $@"{values[0]}\{values[1]}",
                "GetPathConference" => $"Конференции\n",
                "GetPathChanges" => $"ЗИ\n",
                "GetPathProcess" => $"ГТП\n",
                _ => "null"
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
