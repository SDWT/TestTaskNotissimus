using System.Text;
using TestTaskNotissimus.Entities;

namespace TestTaskNotissimus.Extensions
{
    public static class ProductExtension
    {
        public static string ToCSVLine(this Product product)
        {
            char separator = ';';
            StringBuilder strB = new();

            strB.Append(product.RegionName);
            strB.Append(separator);

            strB.Append(product.BreadCrumbs);
            strB.Append(separator);

            strB.Append(product.ProductName);
            strB.Append(separator);

            strB.Append(product.Price);
            strB.Append(separator);

            strB.Append(product.OldPrice);
            strB.Append(separator);

            strB.Append(product.IsInStock);
            strB.Append(separator);

            strB.Append(product.ImageUrls);
            strB.Append(separator);

            strB.Append(product.ProductUrl);
            strB.Append('\n');
            return strB.ToString();
        }

        public static void ToConsole(this Product product)
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
