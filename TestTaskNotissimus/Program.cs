using System;
using System.Text;
using TestTaskNotissimus.Entities;
using TestTaskNotissimus.Extensions;
using TestTaskNotissimus.Parsers;

namespace TestTaskNotissimus
{
    class Program
    {
        static string filename = "out.csv";

        async static Task Main(string[] args)
        {
            if (File.Exists(filename))
                File.Delete(filename);
            File.Create(filename).Close();
            Dictionary<string, string> citiesCode = new Dictionary<string, string>();
            citiesCode.Add("Санкт-Петербург", "78000000000");
            citiesCode.Add("Ростов-на-Дону", "61000001000");

            string address = "/catalog/boy_transport/";

            await ParseToCsvProducts(address);
            await ParseToCsvProducts(address, citiesCode["Ростов-на-Дону"]);


            // Test
            //string address = "https://www.toy.ru/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/";

            //TestGetProduct("/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/");
            //TestGetProduct("/catalog/mashinki_iz_multfilmov/fortnite_fnt0163_mashina_quadcrasher/");

            //TestGetPageProducts("/catalog/boy_transport/");
            //TestGetProducts("/catalog/boy_transport/");
            //await TestToCsvProducts("/catalog/boy_transport/");

        }


        async static Task ParseToCsvProducts(string address, string region = "")
        {
            var parser = new ToyRuParser(filename, region);
            await parser.ToCSVProductsAsync(address);

            // Ожидания окончания выполнения всех потоков
            while (parser.countActiveThreads > 0)
            { }
        }

        async static Task TestToCsvProducts(string address)
        {

            //Console.WriteLine(address);

            var parser = new ToyRuParser(filename);
            await parser.ToCSVProductsAsync(address);
            
            // Найти решение ожидания
            //while (ThreadPool.ThreadCount > 0)
            while (parser.countActiveThreads > 0)
            {
                
                //Console.WriteLine($"Активные потоки: {ThreadPool.ThreadCount}");
                //Console.WriteLine($"Активные потоки: {parser.countActiveThreads}");
                //Task.Delay(500);
            }
        }

        static void TestGetProducts(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser(filename);
            var products = parser.GetProductsAsync(address).Result;


            File.WriteAllLines(filename, products.Select((pr) => pr.ToCSVLine()));

            Console.WriteLine();
            Console.WriteLine($"Кол-во товаров:{products.Count()}");
        }

        static void TestGetPageProducts(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser(filename);
            var products = parser.GetPageProductsAsync(address).Result;

            //foreach (var product in products)
            //{
            //    //product.ToConsole();
            //}

            IEnumerable<string> contents = products.Select((pr) => pr.ToCSVLine());
            File.WriteAllLines(filename, contents);

            Console.WriteLine();
            Console.WriteLine($"Кол-во товаров:{products.Count()}");
        }

        static void TestGetProduct(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser(filename);

            parser.GetProductAsync(address).Result.ToConsole();

        }
    }
}
