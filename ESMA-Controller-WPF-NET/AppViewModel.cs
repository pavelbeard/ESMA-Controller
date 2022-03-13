using ESMA.ChangesCloser;
using ESMA.Chromedriver;
using ESMA.Controllers;
using ESMA.DataCollections;
using ESMA.DataCollections.CoreDataCollections;
using ESMA.DataLoaders;
using ESMA.ExcelData;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using VideoConference = ESMA.DataCollections.CoreDataCollections.VideoConference;

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
            get
            {
                try
                {
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                    string file = t["EmpListFile"];

                    var list = new EmpList(file);

                    var newList = new ObservableCollection<EmpUnit>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].IsChecked)
                        {
                            newList.Add(new EmpUnit { Name = list[i].Name, IsChecked = list[i].IsChecked });
                        }
                    }

                    return new VideoConference
                    {
                        IdConference = 0,
                        VC_Theme = "null",
                        VC_Date = DateTime.Parse(DateTime.Now.ToString("D")),
                        VC_TimeStart = DateTime.Parse("00:00"),
                        VC_TimeEnd = DateTime.Parse("00:00"),
                        VC_Job = "null",
                        VC_Names = newList,
                        VC_Names_For_Content = newList,
                        OperPersonal = true,
                        CloseConference = true,
                        Escort = false
                    };
                }
                catch (Exception)
                {
                    MessageBox.Show("Файл не найден, либо отсутствует путь к нему в файле конфигурации");
                    return null;
                }
            }
        }
        private Changes AddChanges
        {
            get
            {
                try
                {
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                    string file = t["EmpListFile"];

                    var list = new EmpList(file);

                    ObservableCollection<EmpUnit> NewList()
                    {
                        var newList = new ObservableCollection<EmpUnit>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].IsChecked)
                            {
                                newList.Add(new EmpUnit
                                {
                                    Name = list[i].Name,
                                    IsChecked = list[i].IsChecked
                                });
                            }
                        }

                        return newList;
                    }

                    return new Changes
                    {
                        IdChanges = 0,
                        C_Description = "null",
                        C_Job = "null",
                        C_TimeStart = DateTime.Parse("00:00"),
                        C_TimeEnd = DateTime.Parse("00:00"),
                        C_Names = NewList()
                    };
                }
                catch (Exception)
                {
                    MessageBox.Show("Файл не найден, либо отсутствует путь к нему в файле конфигурации");
                    return null;
                }
            }
        }
        private Process AddProcess
        {
            get 
            {
                try
                {
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                    string file = t["EmpListFile"];

                    var list = new EmpList(file);

                    var newList = new ObservableCollection<EmpUnit>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].IsChecked)
                        {
                            newList.Add(new EmpUnit { Name = list[i].Name, IsChecked = list[i].IsChecked });
                        }
                    }

                    return new Process
                    {
                        IdProcess = 0,
                        P_Description = "null",
                        P_TimeStart = DateTime.Parse("00:00"),
                        P_Job = "null",
                        P_Event = "null",
                        P_Names = newList
                    };
                }
                catch (Exception)
                {
                    MessageBox.Show("Файл не найден, либо отсутствует путь к нему в файле конфигурации");
                    return null;
                }
            } 
        }
        private ChangesCreate AddCTC
        {
            get => new ChangesCreate
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
                        var ctcLoad = await cc.LoadDataAsync<ChangesCreate>(scanProgress);
                        if (ctcLoad != null)
                        {
                            foreach (var item in ctcLoad)
                            {
                                IData.Window.chCreateList.Add(item);
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
                        3 => IData.Window?.chCreateList?.Count == 0 && check,
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
                    IData.Window.chCreateList?.Clear();
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
                            CTCs = new BindingList<ChangesCreate>(),
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
                        CTCs = IData.Window.chCreateList,
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
                                CTSs = IData.Window.chCreateList,
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
                    case 3: IData.Window.chCreateList.Add(AddCTC); break;
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
                            if (IData.Window.chCreateList.Count < 1)
                                IData.Window.chCreateList.Add(AddCTC);
                            else
                                IData.Window.chCreateList.Insert(IData.Window.ChangesCreate.SelectedIndex, AddCTC);
                            break;
                        case 5:
                            if (IData.Window.chCreateList.Count < 1)
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
                        if (IData.Window.chCreateList.Count > 0)
                            IData.Window.chCreateList.RemoveAt(IData.Window.ChangesCreate.SelectedIndex);
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
                        3 => IData.Window?.chCreateList?.Count > 0,
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
                        IData.CTCs = IData.Window.chCreateList;
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
                    3 => IData.Window?.chCreateList?.Count > 0 && check,
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
                        IData.Window.chCreateList?.Clear();
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
                    3 => IData.Window?.chCreateList?.Count > 0 && IData.Window.ProgressBar_Grid.Visibility == Visibility.Hidden,
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
                            {
                                //IData.Window.videoList[IData.Window.Conference.SelectedIndex].VC_Names =
                                //new ObservableCollection<EmpUnit>(File.ReadAllLines(openFileDialog.FileName));
                                //IData.Window.videoList[IData.Window.Conference.SelectedIndex].VC_Names_For_Content =
                                //new ObservableCollection<string>(File.ReadAllLines(openFileDialog.FileName));
                            }
                            break;
                        case 1:
                            //IData.Window.changesList[IData.Window.Changes.SelectedIndex].C_Names =
                            //   new ObservableCollection<EmpUnit>(File.ReadAllLines(openFileDialog.FileName));
                            break;
                        case 2:
                            //IData.Window.processList[IData.Window.Process.SelectedIndex].P_Names =
                            //   new ObservableCollection<EmpUnit>(File.ReadAllLines(openFileDialog.FileName));
                            break;
                        default:
                            break;
                    }
                }
            });
        }
        public RelayCommand ChangeNamesNew
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
                            //IData.Window.videoList[IData.Window.Conference.SelectedIndex].VC_Names_For_Content =
                            //    new ObservableCollection<string>(File.ReadAllLines(openFileDialog.FileName));
                            break;
                    }
                }
            });
        }
        //change prim names
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
        //accept here
        //reset time here
        //auth here
        //reset auth here
        public RelayCommand ResetData
        {
            get => new RelayCommand(async obj =>
            {
                //await js.EditFileAsync(ConfigData.ConfigurationFilePath, new Dictionary<string, T>)
            });
        }
        //silent mode
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
                $"{"Эта программа создана для того, чтобы облегчить \nи без того трудную жизнь на связи совещаний\n",10}" +
                $"ESMA-Controller " +
                $"v{Assembly.GetExecutingAssembly().GetName().Version}");
                MessageBox.Show(about);
            });
        }

        public RelayCommand CreateReport
        {
            get => new(obj =>
            {
                ReportData reportData = new ReportData();

                for (int i = 0; i < IData.Window.changesList.Count; i++)
                {
                    var list = IData.Window.changesList[i].C_Names;
                    foreach (var name in list)
                    {
                        if (name.IsChecked)
                        {
                            reportData.Emps.Add(name.Name);
                            if (IData.Window.changesList[i].IdChanges.ToString().Length < 4)
                            {
                                reportData.Lrps.Add("0000");

                            }
                            else
                            {
                                reportData.Lrps.Add(IData.Window.changesList[i].IdChanges.ToString());
                            }
                        }

                    }
                }

                ExcelDataCreator.NewReport(reportData);
                MessageBox.Show($"Отчет создан.\nОн находится в:\n{Environment.CurrentDirectory}\\Reports");

            },
                check => IData.Window.changesList.Count > 0);
        }

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
        //settings info
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
