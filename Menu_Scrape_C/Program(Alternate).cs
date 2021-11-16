using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Globalization;


namespace Menu_Scrape_C
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load("https://www.allmenus.com/ga/atlanta/281308-gato-bizco-cafe/menu/");

           

              HtmlNodeCollection LinkNodes = doc.DocumentNode.SelectNodes("//span[@class = 'unordered-list']");


            var Menus = new List<Dish>();
            foreach (var node in LinkNodes)
            {

                var DishNameXpath = "//span[@class='item-title']";
                var DishPriceXpath = "//span[@class='item-price']";

                var dish = new Dish();

                dish.DishName = doc.DocumentNode.SelectSingleNode(DishNameXpath).InnerText;
                dish.Price = doc.DocumentNode.SelectSingleNode(DishPriceXpath).InnerText;

                Menus.Add(dish);

            }



            using (var writer = new StreamWriter("C:/Users/Devin Greene/Documents/Gato_bizco.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))

            {
                csv.WriteRecordsAsync(Menus);




            }
        }
    }

    public class Dish
    {
        public string DishName { get; set; }
        public string Price { get; set; }
    }

  
    

   

 
  
}
