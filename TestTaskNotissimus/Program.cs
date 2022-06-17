using System;
using System.Text;
using TestTaskNotissimus.Entities;
using TestTaskNotissimus.Extensions;
using TestTaskNotissimus.Parsers;

namespace TestTaskNotissimus
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test
            //string address = "https://www.toy.ru/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/";

            //TestGetProduct("/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/");
            //TestGetProduct("/catalog/mashinki_iz_multfilmov/fortnite_fnt0163_mashina_quadcrasher/");

            //TestGetProducts("/catalog/boy_transport/");
            //TestGetPageProducts("/catalog/boy_transport/");
        }
        static void TestGetProducts(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser();
            var products = parser.GetProductsAsync(address).Result;

            foreach (var product in products)
            {
                //product.ToConsole();
            }
            Console.WriteLine();
            Console.WriteLine($"Кол-во товаров:{products.Count()}");
        }

        static void TestGetPageProducts(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser();
            var products = parser.GetPageProductsAsync(address).Result;

            foreach (var product in products)
            {
                //product.ToConsole();
            }
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
