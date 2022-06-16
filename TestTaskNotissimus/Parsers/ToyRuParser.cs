using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTaskNotissimus.Entities;

namespace TestTaskNotissimus.Parsers
{
    public class ToyRuParser
    {
        private string CatalogURL = "https://www.toy.ru/catalog/boy_transport/";

        //public async Task<Product> GetProduct(string address)

        /// <summary>
        /// Получение данных о товаре
        /// </summary>
        /// <param name="address">Ссылка на товар</param>
        /// <returns>Товар</returns>
        public async Task GetProduct(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(address);

            Product product = new Product();

            if (document is null)
            {

                Console.WriteLine("doc is null");
                return;
            }

            document.GetElementsByClassName("price").All((el) =>
            {
                Console.WriteLine(el.TextContent);
                return true;
            });

            document.GetElementsByClassName("old-price").All((el) =>
            {
                Console.WriteLine(el.TextContent);
                return true;
            });

            //if (decimal.TryParse(doc.GetElementsByClassName("price").FirstOrDefault().TextContent.Split(' ')[0], out product.Price);
            
            product.Price = GetElementByClassName(document, "price");
            Console.WriteLine($"Цена: {product.Price}");

            product.OldPrice = GetElementByClassName(document, "old-price");
            Console.WriteLine($"Старая цена: {product.OldPrice}");

        }

        private string GetElementByClassName(AngleSharp.Dom.IDocument? document, string className)
        {
            var element = document.GetElementsByClassName(className).FirstOrDefault();
            return element is null ? String.Empty : element.TextContent;
        }
    }
}
