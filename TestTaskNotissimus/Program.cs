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

            // Test
            //string address = "https://www.toy.ru/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/";

            //TestGetProduct("/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/");
            //TestGetProduct("/catalog/mashinki_iz_multfilmov/fortnite_fnt0163_mashina_quadcrasher/");

            //TestGetPageProducts("/catalog/boy_transport/");
            //TestGetProducts("/catalog/boy_transport/");

            
            await TestToCsvProducts("/catalog/boy_transport/");

            return;
        }

        async static Task TestToCsvProducts(string address)
        {

            //Console.WriteLine(address);

            var parser = new ToyRuParser();
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

            var parser = new ToyRuParser();
            var products = parser.GetProductsAsync(address).Result;


            File.WriteAllLines(filename, products.Select((pr) => pr.ToCSVLine()));

            Console.WriteLine();
            Console.WriteLine($"Кол-во товаров:{products.Count()}");
        }

        static void TestGetPageProducts(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser();
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

            var parser = new ToyRuParser();

            parser.GetProductAsync(address).Result.ToConsole();

        }
    }
}
