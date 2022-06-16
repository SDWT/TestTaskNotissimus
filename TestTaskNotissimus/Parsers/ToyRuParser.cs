﻿using AngleSharp;
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

            // цена
            //document.GetElementsByClassName("price").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent);
            //    return true;
            //});

            // старая цена
            //document.GetElementsByClassName("old-price").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent);
            //    return true;
            //});

            // Регион
            //var elements = document.QuerySelectorAll("*[data-src=\"#region\"]").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent.Trim());
            //    return true;
            //});

            // Имя
            //document.GetElementsByClassName("detail-name").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent);
            //    return true;
            //});

            // наличие
            //document.GetElementsByClassName("ok").All((el) =>
            //{
            //    Console.WriteLine(el.TextContent);
            //    return true;
            //});

            product.ProductUrl = address;
            product.ProductName = GetElementByClassName(document, "detail-name");
            product.Price = GetElementByClassName(document, "price");
            product.OldPrice = GetElementByClassName(document, "old-price");
            product.IsInStock = GetElementByClassName(document, "ok").Trim();

            var element = document.QuerySelectorAll("*[data-src=\"#region\"]").FirstOrDefault();
            product.RegionName = element is null ? String.Empty : element.TextContent.Trim();

            // Хлебные крошки
            var strB = new StringBuilder();


            document.GetElementsByClassName("breadcrumb-item").All((el) =>
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

                //Console.WriteLine($"{el.TagName} {el.TextContent}");
                return true;
            });
            product.BreadCrumbs = strB.ToString(0, strB.Length - 1);


            //product.OldPrice = document.GetElement(className).FirstOrDefault();

            //if (decimal.TryParse(doc.GetElementsByClassName("price").FirstOrDefault().TextContent.Split(' ')[0], out product.Price);

            //product.BreadCrumbs;
            //product.ImageUrls;

            Console.WriteLine();
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
