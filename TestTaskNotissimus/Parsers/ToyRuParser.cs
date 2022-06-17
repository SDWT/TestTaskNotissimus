using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTaskNotissimus.Entities;
using TestTaskNotissimus.Extensions;

namespace TestTaskNotissimus.Parsers
{
    public class ToyRuParser
    {
        //private string catalogURL = "https://www.toy.ru/catalog/boy_transport/";
        private string baseURL = "https://www.toy.ru";
        private readonly string filename = "out.csv";
        private readonly string region;

        //private object locker = new();
        public int countActiveThreads = 0;

        //public async Task<Product> GetProduct(string address)

        public ToyRuParser(string filename, string region = "")
        {
            this.filename = filename;
            this.region = region;
        }

        #region Get

        /// <summary>
        /// Получение товаров из каталога
        /// </summary>
        /// <param name="catalogAddress">Ссылка на каталог</param>
        /// <returns>Товары</returns>
        public async Task<IEnumerable<Product>> GetProductsAsync(string catalogAddress)
        {
            var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
            using var context = BrowsingContext.New(config);
            if (!string.IsNullOrEmpty(region))
                context.SetCookie(new AngleSharp.Dom.Url($"{baseURL}"), "BITRIX_SM_city=61000001000");
            using var document = await context.OpenAsync($"{baseURL}{catalogAddress}");


            var elements = document.GetElementsByClassName("page-link");

            if (elements.FirstOrDefault() is null || elements.Length < 2)
                return await GetPageProductsAsync(catalogAddress);

            //for (int i = 0; i < elements.Length; i++)
            //{
            //    Console.WriteLine($"Страница {i}: {elements[i].GetAttribute("href")}");
            //}

            string? url = elements[elements.Length - 2].GetAttribute("href");
            if (url is null || url.StartsWith('#'))
                return await GetPageProductsAsync(catalogAddress);

            int cnt = 0; // количество страниц

            var lastPage = url.Split('=').Last();
            var pageUrl = url.Substring(0, url.Length - lastPage.Length);

            if (!int.TryParse(lastPage, out cnt))
                return await GetPageProductsAsync(catalogAddress);

            //Console.WriteLine($"Кол-во страниц: {cnt}");

            List<Product> products = new();
            //Console.Write($"Страница 1: {catalogAddress} | ");
            products.AddRange(await GetPageProductsAsync(catalogAddress));
            //Console.Write($"кол-во товаров на странице 1: {products.Count} | ");
            //Console.WriteLine($"всего: {products.Count}");

            for (int i = 2; i <= cnt; i++)
            {
                //Console.Write($"Страница {i}: {pageUrl}{i} | ");
                int tmp = products.Count;
                products.AddRange(await GetPageProductsAsync($"{pageUrl}{i}"));
                //Console.Write($"кол-во товаров на странице {i}: {products.Count - tmp} | ");
                //Console.WriteLine($"всего: {products.Count}");
            }

            return products;
        }

        /// <summary>
        /// Получение товаров с одной страницы
        /// </summary>
        /// <param name="pageAddress">Ссылка на страницу каталога</param>
        /// <returns>Товары</returns>
        public async Task<IEnumerable<Product>> GetPageProductsAsync(string pageAddress)
        {
            var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
            using var context = BrowsingContext.New(config);
            if (!string.IsNullOrEmpty(region))
                context.SetCookie(new AngleSharp.Dom.Url($"{baseURL}"), "BITRIX_SM_city=61000001000");
            using var document = await context.OpenAsync($"{baseURL}{pageAddress}");

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

            var products = new List<Product>();

            var threads = new Thread[productsUrls.Count];
            for (int i = 0; i < threads.Length; i++)
            {
                var url = productsUrls[i];
                threads[i] = new Thread(start: async () => products.Add(await GetProductAsync(url)));
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            //foreach (var url in productsUrls)
            //    products.Add(await GetProductAsync(url));

            return products;
        }

        /// <summary>
        /// Получение данных о товаре
        /// </summary>
        /// <param name="address">Ссылка на товар</param>
        /// <returns>Товар</returns>
        public async Task<Product> GetProductAsync(string address)
        {
            var config = Configuration.Default.WithDefaultCookies().WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            context.SetCookie(new AngleSharp.Dom.Url($"{baseURL}"), "BITRIX_SM_city=61000001000");
            using var document = await context.OpenAsync($"{baseURL}{address}");
            //using var document = await context.OpenAsync(res =>
            //    res.Address($"{_BaseURL}{address}").
            //        Header(HeaderNames.SetCookie, "BITRIX_SM_city=61000001000"));

            //Console.WriteLine(document.Cookie);

            Product product = new();

            // Если не загрузился документ
            if (document is null)
            {
                //Console.WriteLine("doc is null");
                return product;
            }

            product.ProductUrl = $"{baseURL}{address}";                            // Ссылка на товар
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
            product.BreadCrumbs = strB.Length <= 1 ? String.Empty : strB.ToString(0, strB.Length - 1);
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
            product.ImageUrls = strB.Length <= 1 ? String.Empty : strB.ToString(0, strB.Length - 1);
            strB.Clear();

            return product;
        }

        #endregion

        #region To CSV

        /// <summary>
        /// Получение товаров из каталога
        /// </summary>
        /// <param name="catalogAddress">Ссылка на каталог</param>
        /// <returns>Товары</returns>
        public async Task ToCSVProductsAsync(string catalogAddress)
        {
            var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
            using var context = BrowsingContext.New(config);
            if (!string.IsNullOrEmpty(region))
                context.SetCookie(new AngleSharp.Dom.Url($"{baseURL}"), "BITRIX_SM_city=61000001000");
            using var document = await context.OpenAsync($"{baseURL}{catalogAddress}");

            await ToCSVPageProductsAsync(catalogAddress); // Первая страница каталога

            var elements = document.GetElementsByClassName("page-link");

            if (elements.FirstOrDefault() is null || elements.Length < 2)
                return;

            string? url = elements[elements.Length - 2].GetAttribute("href");
            if (url is null || url.StartsWith('#'))
                return;

            int cnt = 0; // количество страниц

            var lastPage = url.Split('=').Last();
            var pageUrl = url.Substring(0, url.Length - lastPage.Length);

            if (!int.TryParse(lastPage, out cnt))
                return;

            //Console.WriteLine($"Кол-во страниц: {cnt}");

            for (int i = 2; i <= cnt; i++)
            {
                //Console.WriteLine($"Страница {i}");
                await ToCSVPageProductsAsync($"{pageUrl}{i}");
            }
        }

        /// <summary>
        /// Получение товаров с одной страницы
        /// </summary>
        /// <param name="pageAddress">Ссылка на страницу каталога</param>
        /// <returns>Товары</returns>
        public async Task ToCSVPageProductsAsync(string pageAddress)
        {
            var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
            using var context = BrowsingContext.New(config);
            if (!string.IsNullOrEmpty(region))
                context.SetCookie(new AngleSharp.Dom.Url($"{baseURL}"), "BITRIX_SM_city=61000001000");
            using var document = await context.OpenAsync($"{baseURL}{pageAddress}");

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

            #region Counter Variant

            var threads = new Thread[productsUrls.Count];
            for (int i = 0; i < threads.Length; i++)
            {
                var url = productsUrls[i];
                countActiveThreads++;
                threads[i] = new Thread(start: async () =>
                {
                    await ToCSVProductAsync(url);
                });
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            #endregion
            #region ThreadPool

            //foreach (var url in productsUrls)
            //{
            //    _ = ThreadPool.QueueUserWorkItem(async (Object stateInfo) => await ToCSVProductAsync(url));
            //}

            #endregion
        }

        /// <summary>
        /// Получение данных о товаре
        /// </summary>
        /// <param name="address">Ссылка на товар</param>
        /// <returns>Товар</returns>
        public async Task ToCSVProductAsync(string address)
        {
            var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
            using var context = BrowsingContext.New(config);
            if (!string.IsNullOrEmpty(region))
                context.SetCookie(new AngleSharp.Dom.Url($"{baseURL}"), "BITRIX_SM_city=61000001000");
            using var document = await context.OpenAsync($"{baseURL}{address}");

            Product product = new();

            // Если не загрузился документ
            if (document is null)
            {
                //Console.WriteLine("doc is null");
                return;
            }

            product.ProductUrl = $"{baseURL}{address}";                            // Ссылка на товар
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

            lock (filename)
            {
                File.AppendAllText(filename, product.ToCSVLine());
                //Console.WriteLine(product.ProductName);
            }
            countActiveThreads--;
        }

        #endregion

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
