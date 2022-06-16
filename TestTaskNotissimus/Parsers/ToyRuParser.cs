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
            using var context = BrowsingContext.New(Configuration.Default);
            using var doc = await context.OpenAsync(address);


        }
    }
}
