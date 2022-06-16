using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskNotissimus.Entities
{
    public class Product
    {
        /// <summary>
        /// Название региона
        /// </summary>
        public string RegionName { get; set; } = string.Empty;

        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public string BreadCrumbs { get; set; } = string.Empty;

        /// <summary>
        /// Название товара
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Цена
        /// </summary>
        public string Price { get; set; } = string.Empty;

        /// <summary>
        /// Цена старая;
        /// </summary>
        public string OldPrice { get; set; } = string.Empty;

        /// <summary>
        /// Раздел с наличием(В наличии товар или не в наличии)
        /// </summary>
        public string IsInStock { get; set; } = string.Empty;

        /// <summary>
        /// Ссылки на картинки
        /// </summary>
        public string ImageUrls { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на товар
        /// </summary>
        public string ProductUrl { get; set; } = string.Empty;

    }
}
