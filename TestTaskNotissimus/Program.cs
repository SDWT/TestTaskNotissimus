using System;
using TestTaskNotissimus.Parsers;

namespace TestTaskNotissimus
{
    class Program
    {
        static void Main(string[] args)
        {

            // Test
            string address = "https://www.toy.ru/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/";

            Testing("https://www.toy.ru/catalog/toys-spetstekhnika/childs_play_lvy025_fermerskiy_traktor/");
            Testing("https://www.toy.ru/catalog/mashinki_iz_multfilmov/fortnite_fnt0163_mashina_quadcrasher/");
        }

        static void Testing(string address)
        {
            Console.WriteLine(address);
            Console.WriteLine();

            var parser = new ToyRuParser();
            var product = parser.GetProductAsync(address).Result;

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
