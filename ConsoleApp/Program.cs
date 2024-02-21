using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main (string[] args)
        {
            var doc = XDocument.Load("valutes.xml").ToString();

            List<Valute> valutes = LoadValutes(doc);

            Task1(valutes);
            var resultTask1 = Task1(valutes);
            foreach (var valute in resultTask1)
            {
                Console.WriteLine($"Название валюты: {valute.Key}  Значение: {valute.Value}");
            }

            var resultTask2 = Task2(valutes);
            Console.WriteLine(resultTask2);


            Console.WriteLine("Введите валюту для поиска:");
            string searchValute = Console.ReadLine();
            var resultTask3 = Task3(valutes, searchValute);
            Console.WriteLine(resultTask3);


        }


        public static List<Valute> LoadValutes (string xmlFileName)
        {
            List<Valute> result = new List<Valute>();

            var valutes = XDocument.Parse(xmlFileName).Element("ValCurs").Elements("Valute");

            foreach (var valute in valutes)
            {
                Valute new_valute = new Valute
                {
                    NumCode = Convert.ToInt32(valute.Element("NumCode").Value),
                    CharCode = valute.Element("CharCode").Value,
                    Nominal = Convert.ToInt32(valute.Element("Nominal").Value),
                    Name = valute.Element("Name").Value,
                    Value = Convert.ToDouble(valute.Element("Value").Value)
                };
                result.Add(new_valute);

            }
            return result;
        }
        public static Dictionary<string, double> Task1 (List<Valute> valutes)
        {
            var nameAndValue = new Dictionary<string, double>();
            foreach (var valute in valutes)
            {
                string name = valute.Name;
                double value = valute.Value;
                nameAndValue.Add(name, value);
            }
            return nameAndValue;
        }
        public static string Task2 (List<Valute> valutes)
        {
            var minValue = valutes.Min(valut => $"Самый низкий курс = '{valut.Value}' {valut.Name}");
            var maxValue = valutes.Max(valut => $"Самый высокий курс = '{valut.Value}' {valut.Name}");
            return $"{minValue}\n{maxValue}";
        }
        public static string Task3 (List<Valute> valutes, string searchValute)
        {
            var valute = valutes.FirstOrDefault(valut => valut.CharCode == searchValute.ToUpper());
            if (valute is null)
            {
                return "Такой валюты не существует";

            }
            else
            {
                return $"{valute.Name}\nТекущий курс: {valute.Value}";

            }
        }
       
    }
}
