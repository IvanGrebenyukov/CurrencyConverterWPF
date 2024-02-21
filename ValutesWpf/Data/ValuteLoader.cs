using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ValutesWpf.Model;

namespace ValutesWpf.Data
{
    public static class ValuteLoader
    {
        /// <summary>
        /// Получает список валют из текста XML
        /// </summary>
        /// <param name="XMLText">Строка с информацией о валютах в формате XML</param>
        /// <returns>Список валют</returns>
        public static List<Valute> LoadValutes(string XMLText)
        {
            List<Valute> valutes = new List<Valute>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(XMLText);

                XmlNodeList valuteNodes = xmlDoc.SelectNodes("/ValCurs/Valute");

                foreach (XmlNode valuteNode in valuteNodes)
                {
                    Valute valute = new Valute
                    {
                        Code = Convert.ToInt32(valuteNode.SelectSingleNode("NumCode").InnerText),
                        CharCode = valuteNode.SelectSingleNode("CharCode").InnerText,
                        Nominal = Convert.ToInt32(valuteNode.SelectSingleNode("Nominal").InnerText),
                        Name = valuteNode.SelectSingleNode("Name").InnerText,
                        Value = Convert.ToDouble(valuteNode.SelectSingleNode("Value").InnerText)
                    };

                    valutes.Add(valute);
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки валют: {ex.Message}");
            }

            return valutes;
        }
    }
}
