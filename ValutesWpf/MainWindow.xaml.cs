using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Xml;
using ValutesWpf.Data;
using ValutesWpf.Model;

namespace ValutesWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Valute> valutes;

        public MainWindow()
        {
            InitializeComponent();

            LoadDataFromNetworkOrFile();
        }

        private void LoadDataFromNetworkOrFile()
        {
            try
            {
                string xmlFilePath = "Data/valutes.xml";

                if (File.Exists(xmlFilePath))
                {
                  
                    string xmlText = File.ReadAllText(xmlFilePath);
                    DateTime fileDate = GetXmlFileDate(xmlText);

                    if (fileDate.Date == DateTime.Today)
                    {
                        
                        valutes = ValuteLoader.LoadValutes(xmlText);
                        valutes.Insert(0, new Valute { Name = "Российский Рубль", Value = 1, CharCode = "RUB", Nominal = 1 });

                        FromComboBox.ItemsSource = valutes;
                        ToComboBox.ItemsSource = valutes;

                        return;
                    }
                }

               
                LoadDataFromInternet();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DateTime GetXmlFileDate(string xmlText)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlText);

               
                string dateString = xmlDoc.SelectSingleNode("/ValCurs").Attributes["Date"].Value;
                return DateTime.ParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении даты из файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return DateTime.MinValue;
            }
        }

        private async void LoadDataFromInternet()
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("https://www.cbr.ru/scripts/XML_daily.asp");

                if (response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync();

                   
                    File.WriteAllText("Data/valutes.xml", text);

                    valutes = ValuteLoader.LoadValutes(text);
                    valutes.Insert(0, new Valute { Name = "Российский Рубль", Value = 1, CharCode = "RUB", Nominal = 1 });

                    FromComboBox.ItemsSource = valutes;
                    ToComboBox.ItemsSource = valutes;
                }
                else
                {
                    
                    MessageBox.Show($"Ошибка при загрузке данных: {response.StatusCode}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadDataFromFile();
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadDataFromFile();
            }
        }

        private void LoadDataFromFile()
        {
            try
            {
                string xmlText = File.ReadAllText("Data/valutes.xml");
                valutes = ValuteLoader.LoadValutes(xmlText);
                valutes.Insert(0, new Valute { Name = "Российский Рубль", Value = 1, CharCode = "RUB", Nominal = 1 });

                FromComboBox.ItemsSource = valutes;
                ToComboBox.ItemsSource = valutes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных из файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InputBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out int x))
            {
                e.Handled = true;
            }
        }

        private void InputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Convertation();
            
            
        }

        private void Convertation ()
        {
            Valute inValute = FromComboBox.SelectedItem as Valute;
            Valute outValute = ToComboBox.SelectedItem as Valute;

            if (inValute == null || outValute == null)
            {
                MessageBox.Show("Выберите валюты для конвертации", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(InputBox.Text))
            {
                OutputBox.Text = string.Empty;
                return;
            }

            int value;
            bool succ = int.TryParse(InputBox.Text, out value);
            if (!succ)
                return;

            double rubles = value * inValute.Value;
            double result = Math.Round(rubles / outValute.Value, 2);

            OutputBox.Text = result.ToString();
        }

        private void ToComboBox_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Valute inValute = FromComboBox.SelectedItem as Valute;
            Valute outValute = ToComboBox.SelectedItem as Valute;

            if (inValute == null || outValute == null)
            {
                return;
            }

            Convertation();
        }

        private void FromComboBox_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Valute inValute = FromComboBox.SelectedItem as Valute;
            Valute outValute = ToComboBox.SelectedItem as Valute;

            if (inValute == null || outValute == null)
            {
                return;
            }

            Convertation();
        }
    }
}
