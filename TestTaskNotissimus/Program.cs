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

            var parser = new ToyRuParser();
            parser.GetProductAsync(address).Wait();
            Console.WriteLine();
        }
    }
}
