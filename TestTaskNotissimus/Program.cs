using System;
using TestTaskNotissimus.Entities;
using TestTaskNotissimus.Parsers;

namespace TestTaskNotissimus
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test
            string address = "https://www.toy.ru/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/";

            //TestGetProduct("/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/");
            //TestGetProduct("/catalog/mashinki_iz_multfilmov/fortnite_fnt0163_mashina_quadcrasher/");

            TestGetProducts("/catalog/boy_transport/");
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
                //WriteProduct(product);
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
                //WriteProduct(product);
            }
            Console.WriteLine();
            Console.WriteLine($"Кол-во товаров:{products.Count()}");
        }

        static void TestGetProduct(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser();

            WriteProduct(parser.GetProductAsync(address).Result);

        }

        static void WriteProduct(Product product)
        {
            Console.WriteLine($"Регион: {product.RegionName}");
            Console.WriteLine($"Хлебные крошки: {product.BreadCrumbs}");
            Console.WriteLine($"Название: {product.ProductName}");
            Console.WriteLine($"Цена: {product.Price}");
            Console.WriteLine($"Старая цена: {product.OldPrice}");
            Console.WriteLine($"Наличие: {product.IsInStock}");
            Console.WriteLine($"Ссылки на изображения: {product.ImageUrls}");
            Console.WriteLine($"Ссылка на товар: {product.ProductUrl}");
            Console.WriteLine();
        }
    }
}
