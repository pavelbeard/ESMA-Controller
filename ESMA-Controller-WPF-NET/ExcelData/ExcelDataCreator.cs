using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Text.Json;
using ESMA.ExcelData;
using System.Threading.Tasks;
using System.Text.Unicode;

namespace ESMA
{
    public static class ExcelDataCreator
    {
        private readonly static string reportFolderPath;

        static ExcelDataCreator()
        {
            reportFolderPath = Directory.CreateDirectory(ConfigData.ReportsFolderPath).FullName;
        }

        private static Dictionary<string, string> EmpsList = FillEmpsList();

        private static Dictionary<string, string> FillEmpsList()
        {
            var file = File.ReadAllText(ConfigData.EmpListFileJSON);
            var employesList = System.Text.Json.JsonSerializer.Deserialize<EmployesList>(file);
            employesList.LengthCheck();
            var dictionary = new Dictionary<string, string>();

            for (int i = 0; i < employesList.Count; i++)
            {
                dictionary.Add(employesList.FullNameList[i], employesList.ShortNameList[i]);
            }

            return dictionary;
        }

        public static void NewReport(ReportData reportData)
        {
            try
            {
                int numOfRows = EmpsList.Values.Count + 1;
                //Col headers
                string[] colHeaders =
                {
                    "РВБ",
                    "ФИО Работника",
                    "ОПЕРАТИВНЫЕ РАБОТЫ\n(КРОМЕ ГТП)",
                    "План работы на сутки согласно\n4хнед\\ год. ГТП\n(Станция, пункты плана)",
                    "Факт.выполнение за сутки\nсогласно 4хнед\\ год.ГТП\n(Станция, пункты плана)"
                };

                char[] columns =
                {
                    'A', 'B', 'C', 'D', 'E'
                };

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var excelFile = new ExcelPackage())
                {
                    ExcelWorksheet ews = excelFile.Workbook.Worksheets.Add("Отчет");

                    //Работа с первой строкой
                    ews.Cells[1, 1].Value = $"Отчет по занятости работников и проведенным работам за сутки _{DateTime.Now:d}_ Связь Совещаний  УМЖД";
                    ews.SelectedRange["A1:E1"].Merge = true;
                    ews.Row(1).Height = 25;
                    ews.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ews.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                    ews.Row(1).Style.Font.Bold = true;
                    ews.DefaultColWidth = 25;
                    //Работа со второй строкой
                    ews.Row(2).Height = 60;
                    ews.Row(2).Style.WrapText = true;
                    ews.Row(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ews.Row(2).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    //Работа с остальными строками и столбцами
                    ews.SelectedRange[$"A3:E{numOfRows}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ews.SelectedRange[$"A3:E{numOfRows}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ews.SelectedRange[$"A3:E{numOfRows}"].Style.WrapText = true;

                    for (int rows = 3; rows <= numOfRows; rows++)
                    {
                        ews.Row(rows).Height = /*ews.Row(2).Height*/50; //Высота всех строк
                    }

                    ews.SelectedRange[$"A3:A{numOfRows}"].Merge = true; //Объединение столбца РВБ

                    for (int col = 0; col < 5; col++)
                    {
                        for (int row = 2; row <= numOfRows; row++)
                        {
                            ews.Cells[$"{columns[col]}{row}:{columns[col]}{row}"].
                                Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //Границы для всех ячеек
                        }
                    }

                    for (int col = 0; col < columns.Length; col++)
                    {
                        ews.Cells[$"{columns[col]}2:{columns[col]}2"].Value = colHeaders[col]; //Названия столбцов
                    }

                    for (int row = 3; row <= numOfRows; row++)
                    {
                        int i = row - 3;
                        ews.Cells[$"B{row}:B{row}"].Value = EmpsList.Values.ToList()[i]; // Фамилии
                    }
                    //Заполнение ячеек данными
                    ews.SelectedRange[$"C3:C{numOfRows}"].Value = "выходной";

                    var newLrps = reportData.Lrps.Select(x => x.Insert(2, "-")).ToList(); //Вставка в номер ЛР "-"

                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));

                    //здесь будет код, который привязывает лрп к списку имен
                    //реализация multivalue dictionary
                    var newList = new Dictionary<string, List<string>>();
                    for (int i = 0; i < reportData.Emps.Count; i++)
                    {
                        if (newList.ContainsKey(reportData.Emps[i]))
                        {
                            newList[reportData.Emps[i]].Add(newLrps[i]);
                        }
                        else
                        {
                            newList[reportData.Emps[i]] = new List<string>
                            {
                                newLrps[i]
                            };
                        }
                    }
                    //переработка
                    for (int empsCounter = 0; empsCounter < newList.Keys.Count; empsCounter++)
                    {
                        for (int row = 3; row <= numOfRows; row++)
                        {
                            if (EmpsList.Values.ToList()[row - 3] == Convert.ToString(t["InNight"]))
                            {
                                ews.SelectedRange[$"C{row}:C{row}"].Value = "в ночь";
                            }
                            if (EmpsList.Values.ToList()[row - 3] == Convert.ToString(t["SNight"]))
                            {
                                ews.SelectedRange[$"C{row}:C{row}"].Value = "с ночи";
                                ews.SelectedRange[$"E{row}:E{row}"].Value = "выполнено";
                                ews.SelectedRange[$"D{row}:D{row}"].Value = "т.к 4.5, 8.4, 11, 6.9, 13, 4, 20,\n24, 3, 2, 8, 5.9, 5, 5.13, п14.1, 14.4";
                            }
                            //код, который выводит имена в соответствии с ключами
                            if (EmpsList.Keys.ToList()[row - 3] == newList.Keys.ToList()[empsCounter])
                            {
                                if (newList.Values.ToList()[empsCounter].Count < 2)
                                {
                                    ews.Row(row).Height = 50 * newList.Values.ToList()[empsCounter].Count;
                                }
                                else
                                {
                                    ews.Row(row).Height = 25 * newList.Values.ToList()[empsCounter].Count;
                                }
                                ews.SelectedRange[$"E{row}:E{row}"].Value = "выполнено";
                                ews.SelectedRange[$"D{row}:D{row}"].Value = "т.к 4.5, 8.4, 11, 6.9, 13, 4, 20,\n24, 3, 2, 8, 5.9, 5, 5.13, п14.1, 14.4";
                                ews.SelectedRange[$"C{row}:C{row}"].Value = string.Join(",\n", newList.Values.ToList()[empsCounter]);
                            }
                        }
                    }

                    if (Convert.ToString(t["Boss"]) == "Васильева И.А.")
                    {
                        ews.Cells[$"C{numOfRows + 3}:C{numOfRows + 3}"].Value = "и.о. Ст. электромеханика";
                        var sign = ews.Drawings.AddPicture("sign", new FileInfo($"{Environment.CurrentDirectory}\\vsign.png"));
                        sign.SetPosition(16, 5, 3, 0);
                        ews.Cells[$"E{numOfRows + 3}:E{numOfRows + 3}"].Value = Convert.ToString(t["Boss"]);
                    }
                    else if (Convert.ToString(t["Boss"]) == "Степанов М.А.")
                    {
                        ews.Cells[$"C{numOfRows + 3}:C{numOfRows + 3}"].Value = "Старший электромеханик";
                        var sign = ews.Drawings.AddPicture("sign", new FileInfo($"{Environment.CurrentDirectory}\\msign.png"));
                        sign.SetPosition(16, 5, 3, 0);
                        ews.Cells[$"E{numOfRows + 3}:E{numOfRows + 3}"].Value = Convert.ToString(t["Boss"]);
                    }

                    //Сохранение данных
                    excelFile.SaveAs(new FileInfo($"{reportFolderPath}\\Отчет за {DateTime.Now:d} связь совещаний.xlsx"));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
