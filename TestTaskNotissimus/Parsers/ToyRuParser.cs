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
        public async Task<Product> GetProductAsync(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(address);

            Product product = new Product();

            if (document is null)
            {
                Console.WriteLine("doc is null");
                return product;
            }

            //document.GetElementsByClassName("price").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent);
            //    return true;
            //});

            //document.GetElementsByClassName("old-price").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent);
            //    return true;
            //});


            product.ProductUrl = address;
            product.Price = GetElementByClassName(document, "price");
            product.OldPrice = GetElementByClassName(document, "old-price");

            //if (decimal.TryParse(doc.GetElementsByClassName("price").FirstOrDefault().TextContent.Split(' ')[0], out product.Price);

            //product.RegionName;
            //product.BreadCrumbs;
            //product.ProductName;
            //product.IsInStock;
            //product.ImageUrls;

            Console.WriteLine($"Регион: {product.RegionName}");
            Console.WriteLine($"Хлебные крошки: {product.BreadCrumbs}");
            Console.WriteLine($"Название: {product.ProductName}");
            Console.WriteLine($"Цена: {product.Price}");
            Console.WriteLine($"Старая цена: {product.OldPrice}");
            Console.WriteLine($"Наличие: {product.IsInStock}");
            Console.WriteLine($"Ссылки на изображения: {product.ImageUrls}");
            Console.WriteLine($"Ссылка на товар: {product.ProductUrl}");

            return product;
        }

        private string GetElementByClassName(AngleSharp.Dom.IDocument document, string className)
        {
            var element = document.GetElementsByClassName(className).FirstOrDefault();
            return element is null ? String.Empty : element.TextContent;
        }
    }
}
