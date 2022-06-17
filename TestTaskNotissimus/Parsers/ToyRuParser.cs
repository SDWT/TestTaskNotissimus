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
        private string _CatalogURL = "https://www.toy.ru/catalog/boy_transport/";
        private string _BaseURL = "https://www.toy.ru";

        //public async Task<Product> GetProduct(string address)

        public async Task<IEnumerable<Product>> GetPageProductsAsync(string pageAddress)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync($"{_BaseURL}{pageAddress}");

            var products = new List<Product>();

            var productsUrls = new List<string>();

            
            var strB = new StringBuilder();
            // Ссылки на первой странице каталога
            _ = document.QuerySelectorAll("*[itemprop=\"name\"]").All((el) =>
            {
                string? url = el.GetAttribute("href");
                if (url == null)
                    return false;

                //Console.WriteLine(url);
                productsUrls.Add(url);
                return true;
            });

            foreach (var url in productsUrls)
                products.Add(await GetProductAsync(url));

            return products;
        }

        /// <summary>
        /// Получение данных о товаре
        /// </summary>
        /// <param name="address">Ссылка на товар</param>
        /// <returns>Товар</returns>
        public async Task<Product> GetProductAsync(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync($"{_BaseURL}{address}");

            Product product = new();

            // Если не загрузился документ
            if (document is null)
            {
                //Console.WriteLine("doc is null");
                return product;
            }

            product.ProductUrl = address;                                           // Ссылка на товар
            product.ProductName = GetElementByClassName(document, "detail-name");   // Название товара
            product.Price = GetElementByClassName(document, "price");               // Новая цена
            product.OldPrice = GetElementByClassName(document, "old-price");        // Старая цена
            product.IsInStock = GetElementByClassName(document, "ok").Trim();       // Наличие товара, проверить что будет если товара нет

            // Регион, по атрибуту data-src
            var element = document.QuerySelectorAll("*[data-src=\"#region\"]").FirstOrDefault();
            product.RegionName = element is null ? String.Empty : element.TextContent.Trim();

            var strB = new StringBuilder();
            // Хлебные крошки
            _ = document.GetElementsByClassName("breadcrumb-item").All((el) =>
            {
                if (el.TextContent == "Вернуться")
                {
                    return false;

                }
                strB.Append($"{el.TextContent.Trim()}\t");

                // Хлебные крошки с ссылками

                //if (el.TagName == "A")
                //    strB.Append($"{el.TextContent.Trim()}({_BaseURL}{el.GetAttribute("href")})\t");
                //else
                //    strB.Append($"{el.TextContent.Trim()}\t");
                return true;
            });
            product.BreadCrumbs = strB.ToString(0, strB.Length - 1);
            strB.Clear();

            // Ссылки на изображения
            _ = document.GetElementsByClassName("card-slider-nav").All((el) =>
            {
                //Console.WriteLine($"{el.ChildElementCount}");
                foreach (var item in el.Children)
                {
                    if (item.FirstElementChild is null)
                        continue;
                    // Маленькое изображение
                    string? imgUrl = item.FirstElementChild?.GetAttribute("src");

                    if (imgUrl == null)
                        continue;

                    // Большое изображение
                    var path = imgUrl.Split('/');
                    var pathBuilder = new StringBuilder();
                    for (int i = 0; i < path.Length; i++)
                    {
                        if (i == 4 || i == 7)
                            continue;
                        pathBuilder.Append(path[i]);
                        pathBuilder.Append('/');
                    }
                    strB.Append(imgUrl);
                    strB.Append('\t');
                    strB.Append(pathBuilder.ToString(0, pathBuilder.Length - 1));
                    strB.Append('\t');
                }

                return true;
            });
            product.ImageUrls = strB.ToString(0, strB.Length - 1);
            strB.Clear();

            return product;
        }

        /// <summary>
        /// Получение текстого контента первого элемента по классу
        /// </summary>
        /// <param name="document">Документ</param>
        /// <param name="className">Имя класс</param>
        /// <returns>Если элемент отсутсвет, возвращает пустую строку, иначе текст элемента</returns>
        private string GetElementByClassName(AngleSharp.Dom.IDocument document, string className)
        {
            var element = document.GetElementsByClassName(className).FirstOrDefault();
            return element is null ? String.Empty : element.TextContent;
        }
    }
}
